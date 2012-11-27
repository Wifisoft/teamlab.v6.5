using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Configuration;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Files.Core;
using ASC.Security.Cryptography;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Microsoft.Practices.ServiceLocation;
using Microsoft.ServiceModel.Web;
using File = ASC.Files.Core.File;
using FileShare = ASC.Files.Core.Security.FileShare;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.Classes
{
    public static class DocumentUtils
    {
        private const string FrameParams = "?key={0}&vkey={1}&url={2}&title={3}&filetype={4}&buttons={5}&lang={6}&outputtype={7}";

        private static readonly IEqualityComparer<string> Comparer = StringComparer.CurrentCultureIgnoreCase;

        private static readonly string SecretKey = WebConfigurationManager.AppSettings["files.docservice.key"] ?? "TeamLab";

        public static string GetShareLinkParam(string fileId)
        {
            return UrlConstant.DocUrlKey + "=" + Signature.Create(fileId, SecretKey);
        }

        public static string ParseShareLink(string key)
        {
            return Signature.Read<string>(key ?? string.Empty, SecretKey);
        }

        public static bool CheckShareLink(string key, bool checkRead, IFileDao fileDao, out File file)
        {
            var share = CheckShareLink(key, fileDao, out file);
            return (!checkRead && share == FileShare.ReadWrite) || (checkRead && share <= FileShare.Read);
        }

        public static string GetDocKey(object fileId, int fileVersion, DateTime modified)
        {
            var str = string.Format("teamlab_{0}_{1}_{2}_{3}",
                                    fileId,
                                    fileVersion,
                                    modified.GetHashCode(),
                                    GetDocDbKey());

            var keyDoc = Encoding.UTF8.GetBytes(str)
                .ToList()
                .Concat(MachinePseudoKeys.GetMachineConstant())
                .ToArray();

            return Global.InvalidTitleChars.Replace(Hasher.Base64Hash(keyDoc, HashAlg.SHA256), "_");
        }

        public static List<DocServiceParams.RecentDocument> GetRecentEditedDocument(bool forEdit, int count, string crumbsSeporator, IFileDao fileDao)
        {
            var recent = new List<DocServiceParams.RecentDocument>();

            var activity = UserActivityManager.GetUserActivities(TenantProvider.CurrentTenantID, SecurityContext.CurrentAccount.ID,
                                                                 ProductEntryPoint.ID, null, UserActivityConstants.AllActionType,
                                                                 new[] {"OpenEditorFile"}, 0, 100);

            foreach (var entryId in activity.Select(userActivity => userActivity.ContentID.Substring("file_".Length)))
            {
                if (recent.Exists(r => r.ID.Equals(entryId)))
                    continue;

                var fileAct = fileDao.GetFile(entryId);

                if (fileAct.RootFolderType == FolderType.TRASH)
                    continue;

                if (!FileUtility.UsingHtml5(fileAct.Title))
                    continue;

                if (!Global.GetFilesSecurity().CanRead(fileAct))
                    continue;

                string uri;
                if (forEdit && Global.GetFilesSecurity().CanEdit(fileAct))
                    uri = CommonLinkUtility.GetFileWebEditorUrl(fileAct.ID);
                else
                    uri = CommonLinkUtility.GetFileWebViewerUrl(fileAct.ID);

                var fileBreadCrumbs = Global.GetBreadCrumbs(fileAct.FolderID);
                recent.Add(new DocServiceParams.RecentDocument
                               {
                                   ID = entryId,
                                   Title = fileAct.Title,
                                   Uri = CommonLinkUtility.GetFullAbsolutePath(uri),
                                   FolderPath = string.Join(crumbsSeporator, fileBreadCrumbs.Select(folder => folder.Title).ToArray())
                               });

                if (recent.Count == count)
                    break;
            }
            return recent;
        }

        public static Stream GetConvertedFile(File file)
        {
            return GetConvertedFile(file, FileUtility.GetFileExtension(file.Title));
        }

        public static Stream GetConvertedFile(File file, string extension)
        {
            var fileUri = GetFileUri(file);
            return GetConvertedFile(file, extension, fileUri);
        }

        public static Stream GetConvertedFile(File file, string extension, string fileUri)
        { 
            if (file == null)
            {
                return null;
            }

            //original format
            if (file.ConvertedType == null
                && extension.Trim('.').Equals(FileUtility.GetFileExtension(file.Title).Trim('.')))
            {
                return Global.DaoFactory.GetFileDao().GetFileStream(file);
            }

            var documentConverterUrl = WebConfigurationManager.AppSettings["files.docservice.url.converter"];
            if (string.IsNullOrEmpty(documentConverterUrl))
            {
                return null;
            }

            var docKey = GetDocKey(file.ID, file.Version, file.ModifiedOn);
            var docVKey = GetValidateKey(docKey, false);
            var urlDocumentService = documentConverterUrl + FrameParams;
            var url = string.Format(urlDocumentService,
                                    docKey,
                                    docVKey,
                                    HttpUtility.UrlEncode(fileUri),
                                    HttpUtility.UrlEncode(file.Title),
                                    (file.ConvertedType ?? FileUtility.GetFileExtension(file.Title)).Trim('.'),
                                    string.Empty,
                                    string.Empty,
                                    extension.Trim('.'));

            var req = (HttpWebRequest) WebRequest.Create(url);
            req.Timeout = Convert.ToInt32(WebConfigurationManager.AppSettings["files.docservice.timeout"] ?? "120000");

            Stream stream = null;
            var countTry = 0;
            const int maxTry = 3;
            while (countTry < maxTry)
            {
                try
                {
                    countTry++;
                    stream = req.GetResponse().GetResponseStream();
                    break;
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.Timeout)
                    {
                        throw new WebProtocolException(HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest, ex);
                    }
                }
            }
            if (countTry == maxTry)
            {
                throw new WebException("Timeout", WebExceptionStatus.Timeout);
            }
            return stream;
        }

        public static File GetServiceParams(bool forEdit, object fileId, int version, bool itsNew, string shareLink, out DocServiceParams docServiceParams)
        {
            return GetServiceParams(forEdit, fileId, version, itsNew, shareLink, out docServiceParams, true);
        }

        public static File GetServiceParams(bool forEdit, object fileId, int version, bool itsNew, string shareLink, out DocServiceParams docServiceParams, bool forHtml5)
        {
            File file;
            const string crumbsSeporator = " > ";
            var breadCrumbs = new List<Folder>();
            var recent = new ItemList<DocServiceParams.RecentDocument>();
            var acesParams = new ItemList<DocServiceParams.Aces>();
            var folderUrl = "";

            var lastVersion = true;
            var checkLink = false;
            var canEdit = true;
            string fileExt;

            var documentType = string.Empty;

            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                if (forEdit)
                {
                    checkLink = CheckShareLink(shareLink, false, fileDao, out file);
                    if (!checkLink && file == null)
                    {
                        file = fileDao.GetFile(fileId);
                    }
                }
                else
                {
                    if (version < 1)
                    {
                        var editLink = CheckShareLink(shareLink, fileDao, out file);
                        checkLink = editLink <= FileShare.Read;
                        if (checkLink)
                        {
                            canEdit = canEdit && editLink <= FileShare.ReadWrite;
                        }
                        else if (file == null)
                        {
                            file = fileDao.GetFile(fileId);
                        }
                    }
                    else
                    {
                        file = fileDao.GetFile(fileId, version);
                        if (file != null)
                            lastVersion = file.Version == fileDao.GetFile(fileId).Version;
                    }
                }

                if (file == null) throw new Exception(FilesCommonResource.ErrorMassage_FileNotFound);

                if (forHtml5 && !FileUtility.UsingHtml5(file.Title, forEdit))
                {
                    docServiceParams = new DocServiceParams();
                    return file;
                }

                if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
                if (!checkLink)
                {
                    canEdit &= Global.GetFilesSecurity().CanEdit(file);
                    if (forEdit && !canEdit) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
                    if (!forEdit && !Global.GetFilesSecurity().CanRead(file)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
                }

                fileExt = FileUtility.GetFileExtension(file.Title);

                canEdit = canEdit && (file.FileStatus & FileStatus.IsEditing) != FileStatus.IsEditing;
                if (forEdit && !canEdit) throw new Exception(FilesCommonResource.ErrorMassage_UpdateEditingFile);

                canEdit = canEdit && FileUtility.ExtsWebEdited.Contains(fileExt, Comparer);
                if (forEdit && !canEdit) throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);


                if (!forEdit && !FileUtility.ExtsWebPreviewed.Contains(fileExt, Comparer)) throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);

                if (forHtml5)
                {
                    breadCrumbs = Global.GetBreadCrumbs(file.FolderID);

                    recent = new ItemList<DocServiceParams.RecentDocument>(GetRecentEditedDocument(forEdit, 10, crumbsSeporator, fileDao));

                    if (FileUtility.DocumentExts.Contains(fileExt, Comparer))
                        documentType = "text";
                    else if (FileUtility.SpreadsheetExts.Contains(fileExt, Comparer))
                        documentType = "spreadsheet";
                    else if (FileUtility.PresentationExts.Contains(fileExt, Comparer))
                        documentType = "presentation";
                }
            }

            var buttons = string.Empty;

            if (fileExt != ".pdf" && !FileUtility.PresentationExts.Contains(fileExt, Comparer))
                buttons += "download;";

            if (lastVersion && canEdit)
                buttons += "save;edit;";

            if (SecurityContext.IsAuthenticated)
            {
                if (!checkLink && Global.EnableShare)
                {
                    if (file.RootFolderType == FolderType.COMMON && Global.IsAdministrator)
                    {
                        buttons += "share;";
                    }
                    else
                    {
                        if (file.RootFolderType == FolderType.USER && Equals(file.RootFolderId, Global.FolderMy))
                        {
                            buttons += "share;";
                        }
                    }
                }

                if (forHtml5)
                {
                    buttons += "create;";

                    using (var folderDao = Global.DaoFactory.GetFolderDao())
                    {
                        var parent = folderDao.GetFolder(file.FolderID);
                        if (file.RootFolderType == FolderType.USER
                            && file.RootFolderCreator != SecurityContext.CurrentAccount.ID
                            && !Global.GetFilesSecurity().CanRead(parent))
                        {
                            folderUrl = PathProvider.GetFolderUrl(Global.FolderShare, false, null);
                        }
                        else
                        {
                            folderUrl = PathProvider.GetFolderUrl(parent);
                        }
                    }

                    try
                    {
                        var docService = ServiceLocator.Current.GetInstance<IFileStorageService>();
                        var aces = docService.GetSharedInfo(file.UniqID);
                        foreach (var aceWrapper in aces)
                        {
                            string permission;

                            switch (aceWrapper.Share)
                            {
                                case FileShare.Read:
                                    permission = FilesCommonResource.AceStatusEnum_Read;
                                    break;
                                case FileShare.ReadWrite:
                                    permission = FilesCommonResource.AceStatusEnum_ReadWrite;
                                    break;
                                default:
                                    continue;
                            }

                            var user = aceWrapper.SubjectName;
                            if (aceWrapper.SubjectId.Equals(FileConstant.ShareLinkId))
                            {
                                continue;
                                //var domain = "?";
                                //switch (aceWrapper.Share)
                                //{
                                //    case FileShare.Read:
                                //        user = domain + CommonLinkUtility.GetFileWebViewerUrl(file.ID) + user;
                                //        break;
                                //    case FileShare.ReadWrite:
                                //        user = domain + CommonLinkUtility.GetFileWebEditorUrl(file.ID) + user;
                                //        break;
                                //    default:
                                //        continue;
                                //}
                            }

                            acesParams.Add(new DocServiceParams.Aces
                                               {
                                                   User = user,
                                                   Permissions = permission
                                               });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            var versionForKey = file.Version;
            //CreateNewDoc
            if (forEdit)
            {
                versionForKey++;
                if (file.Version == 1 && file.ConvertedType != null && itsNew)
                {
                    versionForKey = 1;
                }
            }

            var docKey = GetDocKey(file.ID, versionForKey, file.ModifiedOn);
            var docVKey = GetValidateKey(docKey, true);

            docServiceParams = new DocServiceParams
                                   {
                                       File = file,

                                       FileUri = GetFileUri(file),
                                       OutputType = string.IsNullOrEmpty(file.ProviderName) ? string.Empty : FileUtility.GetFileExtension(file.Title).Trim('.'),
                                       FileType = (file.ConvertedType ?? FileUtility.GetFileExtension(file.Title)).Trim('.'),
                                       FilePath = string.Join(crumbsSeporator, breadCrumbs.Select(folder => folder.Title).ToArray()),
                                       FolderUrl = folderUrl,
                                       Recent = recent,
                                       SharingSettings = acesParams,
                                       Buttons = buttons,
                                       DocumentType = documentType,

                                       Mode = forEdit ? "edit" : "view",

                                       Key = docKey,
                                       Vkey = docVKey,
                                       Lang = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture().Name
                                   };
            return file;
        }

        public static File EditIframeSrc(object fileId, bool itsNew, string shareLink, out string srcIframe)
        {
            DocServiceParams docServiceParams;
            var file = GetServiceParams(true, fileId, 0, itsNew, shareLink, out docServiceParams, false);

            var ext = FileUtility.GetFileExtension(file.Title);

            var urlDocumentEditor = string.Empty;
            if (FileUtility.DocumentExts.Contains(ext, Comparer))
                urlDocumentEditor = WebConfigurationManager.AppSettings["files.docservice.url.doceditor"];
            else if (FileUtility.SpreadsheetExts.Contains(ext, Comparer))
                urlDocumentEditor = WebConfigurationManager.AppSettings["files.docservice.url.spreditor"];
            else if (FileUtility.PresentationExts.Contains(ext, Comparer))
                urlDocumentEditor = WebConfigurationManager.AppSettings["files.docservice.url.presenteditor"];
            else if (FileUtility.ImageExts.Contains(ext, Comparer))
                urlDocumentEditor = WebConfigurationManager.AppSettings["files.docservice.url.draweditor"];

            if (string.IsNullOrEmpty(urlDocumentEditor))
                throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            urlDocumentEditor = urlDocumentEditor + FrameParams;
            srcIframe = string.Format(urlDocumentEditor,
                                      docServiceParams.Key,
                                      docServiceParams.Vkey,
                                      HttpUtility.UrlEncode(docServiceParams.FileUri),
                                      HttpUtility.UrlEncode(docServiceParams.File.Title),
                                      docServiceParams.FileType,
                                      docServiceParams.Buttons,
                                      docServiceParams.Lang,
                                      docServiceParams.OutputType);

            return file;
        }

        public static File ViewIframeSrc(object fileId, bool itsNew, int fileVersion, string shareLink, out string srcIframe)
        {
            DocServiceParams docServiceParams;
            var file = GetServiceParams(false, fileId, fileVersion, itsNew, shareLink, out docServiceParams, false);

            var ext = FileUtility.GetFileExtension(file.Title);

            string urlDocumentViewer;
            if (FileUtility.DocumentExts.Contains(ext, Comparer))
                urlDocumentViewer = WebConfigurationManager.AppSettings["files.docservice.url.docviewer"];
            else if (FileUtility.SpreadsheetExts.Contains(ext, Comparer))
                urlDocumentViewer = WebConfigurationManager.AppSettings["files.docservice.url.sprviewer"];
            else if (FileUtility.PresentationExts.Contains(ext, Comparer))
                urlDocumentViewer = WebConfigurationManager.AppSettings["files.docservice.url.presentviewer"];
            else if (FileUtility.ImageExts.Contains(ext, Comparer))
                urlDocumentViewer = WebConfigurationManager.AppSettings["files.docservice.url.drawviewer"];
            else
                throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            if (string.IsNullOrEmpty(urlDocumentViewer))
                throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            urlDocumentViewer = urlDocumentViewer + FrameParams;
            srcIframe = string.Format(urlDocumentViewer,
                                      docServiceParams.Key,
                                      docServiceParams.Vkey,
                                      HttpUtility.UrlEncode(docServiceParams.FileUri),
                                      HttpUtility.UrlEncode(docServiceParams.File.Title),
                                      docServiceParams.FileType,
                                      docServiceParams.Buttons,
                                      docServiceParams.Lang,
                                      docServiceParams.OutputType);

            return file;
        }

        public static File UploadFile(string folderId, string title, long contentLength, string contentType, Stream data)
        {
            return UploadFile(folderId, title, contentLength, contentType, data, false);
        }

        public static File UploadFile(string folderId, string title, long contentLength, string contentType, Stream data, bool createNewIfExist)
        {
            if (contentLength > SetupInfo.MaxUploadSize)
            {
                throw FileSizeComment.FileSizeException;
            }
            if (contentLength <= 0)
            {
                throw new InvalidOperationException(FilesCommonResource.ErrorMassage_EmptyFile);
            }

            title = Global.ReplaceInvalidCharsAndTruncate(Path.GetFileName(title));

            using (var dao = Global.DaoFactory.GetFileDao())
            {
                var file = dao.GetFile(folderId, title);

                if (file == null || createNewIfExist)
                {
                    using (var folderDao = Global.DaoFactory.GetFolderDao())
                    {
                        var folder = folderDao.GetFolder(folderId);
                        if (folder == null) throw new Exception(FilesCommonResource.ErrorMassage_FolderNotFound);
                        if (!Global.GetFilesSecurity().CanCreate(folder)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
                        file = new File
                                   {
                                       FolderID = folder.ID,
                                       Title = title,
                                       ContentLength = contentLength,
                                       ContentType = contentType
                                   };
                    }

                    try
                    {
                        file = dao.SaveFile(file, data);
                    }
                    catch
                    {
                        dao.DeleteFile(file.ID);
                        throw;
                    }
                    FilesActivityPublisher.UploadFile(dao.GetFile(file.ID));
                }
                else
                {
                    if (!Global.GetFilesSecurity().CanEdit(file)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                    if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing) throw new Exception(FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFile);

                    file.Title = title;
                    file.ContentLength = contentLength;
                    file.ContentType = contentType;
                    file.ConvertedType = null;
                    file.Version++;

                    file = dao.SaveFile(file, data);

                    Global.PublishUpdateDocument(file.ID);
                }
                return file;
            }
        }

        public const string TemplateDocPath = "templatedocuments/";

        public static Dictionary<string, string> GetDocumentTemplates()
        {
            var result = new Dictionary<string, string>();

            var storeTemp = Global.GetStoreTemplate();

            var lang = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture().TwoLetterISOLanguageName;
            var path = TemplateDocPath + lang + "/";
            if(!storeTemp.IsDirectory(path))
                path = TemplateDocPath + "default/";

            const string docExt = ".docx";
            const string icnExt = ".png";
            foreach (var file in storeTemp.ListFilesRelative("", path, "*" + docExt, false))
            {
                var fileName = file;
                if (string.IsNullOrEmpty(fileName)) continue;

                fileName = fileName.Replace(docExt, string.Empty);

                var icnUri = storeTemp.GetUri(path + fileName + icnExt).ToString();

                result.Add(fileName, icnUri);
            }

            return result;
        }

        private static FileShare CheckShareLink(string key, IFileDao fileDao, out File file)
        {
            file = null;

            var fileId = ParseShareLink(key);
            file = fileDao.GetFile(fileId);
            if (file == null)
            {
                return FileShare.Restrict;
            }

            var filesSecurity = Global.GetFilesSecurity();
            if (filesSecurity.CanEdit(file, FileConstant.ShareLinkId))
            {
                return FileShare.ReadWrite;
            }
            if (filesSecurity.CanRead(file, FileConstant.ShareLinkId))
            {
                return FileShare.Read;
            }
            return FileShare.Restrict;
        }

        private static string GetDocDbKey()
        {
            const string dbKey = "UniqueDocument";
            var resultKey = CoreContext.Configuration.GetSetting(dbKey);

            if (!string.IsNullOrEmpty(resultKey)) return resultKey;

            resultKey = Guid.NewGuid().ToString();
            CoreContext.Configuration.SaveSetting(dbKey, resultKey);

            return resultKey;
        }

        private static string GetValidateKey(string docKey, bool addHost)
        {
            var serviceIp = WebConfigurationManager.AppSettings["files.docservice.address"];
            if (addHost && !string.IsNullOrEmpty(serviceIp))
            {
                return Signature.Create(new {expire = DateTime.UtcNow, key = docKey, ip = serviceIp}, SecretKey);
            }
            else
            {
                return Signature.Create(new {expire = DateTime.UtcNow, key = docKey}, SecretKey);
            }
        }

        private static string GetFileUri(File file)
        {
            //NOTE: Always build path to handler!
            var uriBuilder = new UriBuilder(CommonLinkUtility.GetFullAbsolutePath(FileHandler.FileHandlerPath));
            if (uriBuilder.Uri.IsLoopback)
            {
                uriBuilder.Host = Dns.GetHostName();
            }
            uriBuilder.Query += UrlConstant.Action + "=stream&";
            uriBuilder.Query += UrlConstant.FileId + "=" + file.ID + "&";
            uriBuilder.Query += UrlConstant.Version + "=" + file.Version + "&";
            uriBuilder.Query += UrlConstant.AuthKey + "=" + EmailValidationKeyProvider.GetEmailKey(file.ID.ToString() + file.Version.ToString()) + "&";
            uriBuilder.Query = uriBuilder.Query.Trim('?', '&');
            var uri = uriBuilder.Uri;

            return uri.ToString();
        }
    }
}