using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Services;
using AppLimit.CloudComputing.SharpBox;
using ASC.Common.Web;
using ASC.Core;
using ASC.Data.Storage.S3;
using ASC.Files.Core;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Utility;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FileHandler : AbstractHttpAsyncHandler
    {
        public static string FileHandlerPath
        {
            get { return CommonLinkUtility.FileHandlerPath; }
        }

        public override void OnProcessRequest(HttpContext context)
        {
            var action = context.Request[UrlConstant.Action].ToLower();

            var securityActions = new[] {"upload", "view", "download", "bulk", "save"};
            var publicActions = new[] {"download", "view", "bulk"};

            if (securityActions.Contains(action) && !SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey)))
            {
                if (publicActions.Contains(action) && string.IsNullOrEmpty(context.Request[UrlConstant.DocUrlKey]))
                {
                    if (!CoreContext.TenantManager.GetCurrentTenant().Public)
                    {
                        context.Response.Redirect("~/auth.aspx");
                        return;
                    }
                }
                else
                {
                    if (DocumentUtils.ParseShareLink(context.Request[UrlConstant.DocUrlKey]) == null)
                    {
                        throw new HttpException((int) HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException_EditFile);
                    }
                }
            }

            switch (action)
            {
                case "upload":
                    UploadFile(context);
                    break;
                case "view":
                    DownloadFile(context, true);
                    break;
                case "download":
                    DownloadFile(context, false);
                    break;
                case "bulk":
                    BulkDownloadFile(context);
                    break;
                case "save":
                    SaveFile(context);
                    break;
                case "stream":
                    StreamFile(context);
                    break;
                case "create":
                    CreateFile(context);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void BulkDownloadFile(HttpContext context)
        {
            var store = Global.GetStore();
            var path = string.Format(@"{0}\{1}.zip", SecurityContext.CurrentAccount.ID, UrlConstant.DownloadTitle);
            if (!store.IsFile(FileConstant.StorageDomainTmp, path))
            {
                var url = context.Request.UrlReferrer != null
                              ? context.Request.UrlReferrer.ToString()
                              : string.Format("{0}#{1}/{2}",
                                              PathProvider.StartURL,
                                              UrlConstant.Error,
                                              HttpUtility.UrlEncode(FilesCommonResource.ErrorMassage_FileNotFound));

                context.Response.Redirect(url);
            }
            else
            {
                if (store is S3Storage)
                {
                    var url = store.GetUri(FileConstant.StorageDomainTmp, path, TimeSpan.FromMinutes(5), null).ToString();
                    context.Response.Redirect(url);
                }
                else
                {
                    context.Response.Clear();
                    context.Response.ContentType = "application/zip";
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + UrlConstant.DownloadTitle + ".zip\"");

                    using (var readStream = store.IronReadStream(FileConstant.StorageDomainTmp, path, 40))
                    {
                        context.Response.AddHeader("Content-Length", readStream.Length.ToString());
                        readStream.StreamCopyTo(context.Response.OutputStream);
                    }
                    try
                    {
                        context.Response.Flush();
                        context.Response.End();
                    }
                    catch (HttpException)
                    {
                    }
                }
            }
        }

        private static void DownloadFile(HttpContext context, bool inline)
        {
            var id = context.Request[UrlConstant.FileId];
            var ver = context.Request[UrlConstant.Version];
            var shareLink = context.Request[UrlConstant.DocUrlKey] ?? "";

            var outType = context.Request[UrlConstant.OutType];

            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                File file;
                using (var tagDao = Global.DaoFactory.GetTagDao())
                {

                    var checkLink = DocumentUtils.CheckShareLink(shareLink, true, fileDao, out file);
                    if (!checkLink && file == null)
                        file = String.IsNullOrEmpty(ver)
                                   ? fileDao.GetFile(id)
                                   : fileDao.GetFile(id, Convert.ToInt32(ver));

                    if (file == null) throw new HttpException((int) HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FileNotFound);

                    if (!checkLink && !Global.GetFilesSecurity().CanRead(file))
                    {
                        context.Response.Redirect((context.Request.UrlReferrer != null
                                                       ? context.Request.UrlReferrer.ToString()
                                                       : PathProvider.StartURL)
                                                  + "#" + UrlConstant.Error + "/" +
                                                  HttpUtility.UrlEncode(FilesCommonResource.ErrorMassage_SecurityException_ReadFile));
                        return;
                    }

                    if (!fileDao.IsExistOnStorage(file))
                    {
                        context.Response.Redirect((context.Request.UrlReferrer != null
                                                       ? context.Request.UrlReferrer.ToString()
                                                       : PathProvider.StartURL)
                                                  + "#" + UrlConstant.Error + "/" +
                                                  HttpUtility.UrlEncode(FilesCommonResource.ErrorMassage_FileNotFound));
                        return;
                    }

                    tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, file));
                }

                context.Response.Clear();
                context.Response.ContentType = file.ContentType;
                context.Response.Charset = "utf-8";

                var browser = context.Request.Browser.Browser;
                if (browser == "AppleMAC-Safari" &&
                    0 <= context.Request.UserAgent.IndexOf("chrome", StringComparison.InvariantCultureIgnoreCase))
                    browser = "Chrome";
                var format = browser == "IE" || browser == "AppleMAC-Safari"
                                 ? "{0}; filename=\"{1}\""
                                 : "{0}; filename*=utf-8''{1}";
                var title = file.Title.Replace(',', '_');
                var filename = browser == "AppleMAC-Safari" ? title : HttpUtility.UrlPathEncode(title);
                var contentDisposition = string.Format(format, inline ? "inline" : "attachment", filename);
                context.Response.AddHeader("Content-Disposition", contentDisposition);

                if (inline && string.Equals(context.Request.Headers["If-None-Match"], GetEtag(file)))
                {
                    //Its cached. Reply 304
                    context.Response.StatusCode = 304;
                    context.Response.Cache.SetETag(GetEtag(file));
                }
                else
                {
                    context.Response.CacheControl = "public";
                    context.Response.Cache.SetETag(GetEtag(file));
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);

                    if (file.ConvertedType == null && (string.IsNullOrEmpty(outType) || inline))
                    {
                        //NOTE: always pass files through handler
                        using (var readStream = fileDao.GetFileStream(file))
                        {
                            context.Response.AddHeader("Content-Length", readStream.Length.ToString()); //BUG:Can be bugs
                            readStream.StreamCopyTo(context.Response.OutputStream);
                        }
                    }
                    else
                    {
                        var ext = FileUtility.GetFileExtension(file.Title).Trim('.');
                        if (!string.IsNullOrEmpty(outType) && !inline)
                        {
                            outType = outType.Trim('.');
                            if (FileUtility.ExtConvertible[ext].Contains(outType))
                                ext = outType;
                        }

                        //Take from converter
                        using (var readStream = DocumentUtils.GetConvertedFile(file, ext))
                        {
                            if (readStream != null)
                            {
                                readStream.StreamCopyTo(context.Response.OutputStream);
                            }
                        }
                    }
                    try
                    {
                        context.Response.Flush();
                        context.Response.End();
                    }
                    catch (HttpException)
                    {
                    }
                }
            }
        }

        private static void StreamFile(HttpContext context)
        {
            try
            {
                var id = context.Request[UrlConstant.FileId];
                var ver = context.Request[UrlConstant.Version];
                var auth = context.Request[UrlConstant.AuthKey];

                if (EmailValidationKeyProvider.ValidateEmailKey(id + ver, auth, TimeSpan.FromMinutes(1)) != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    throw new HttpException((int) HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
                }

                using (var dao = Global.DaoFactory.GetFileDao())
                {
                    var file = dao.GetFile(id, Convert.ToInt32(ver));
                    using (var stream = dao.GetFileStream(file))
                    {
                        context.Response.AddHeader("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
                        stream.StreamCopyTo(context.Response.OutputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Response.Write(ex.Message);
            }
            try
            {
                context.Response.Flush();
                context.Response.End();
            }
            catch (HttpException)
            {
            }
        }

        private static string GetEtag(File file)
        {
            return file.ID + ":" + file.Version + ":" + file.Title.GetHashCode();
        }

        private static void UploadFile(HttpContext context)
        {
            var result = new StringBuilder();
            try
            {
                var file = DocumentUtils.UploadFile(context.Request[UrlConstant.FolderId], context.Request[UrlConstant.FileTitle], context.Request.ContentLength, context.Request.ContentType, context.Request.InputStream);
                using (var ms = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof (File));
                    serializer.WriteObject(ms, file);
                    ms.Seek(0, SeekOrigin.Begin);
                    result.AppendFormat("{{ folderId: \"{0}\", file: {1} }}",
                                        file.FolderID,
                                        Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int) ms.Length));
                }
            }
            catch (Exception e)
            {
                result.AppendFormat("{{ error: true, message: \"{0}\" }}", e.Message.HtmlEncode());
            }
            context.Response.Write(result.ToString());
        }

        private static void SaveFile(HttpContext context)
        {
            try
            {
                var shareLink = context.Request[UrlConstant.DocUrlKey] ?? "";

                var fileID = context.Request[UrlConstant.FileId];

                if (string.IsNullOrEmpty(fileID)) throw new ArgumentNullException(fileID);

                var downloadUri = context.Request[UrlConstant.FileUri];
                if (string.IsNullOrEmpty(downloadUri)) throw new ArgumentNullException(downloadUri);

                using (var fileDao = Global.DaoFactory.GetFileDao())
                {
                    File file;

                    var checkLink = DocumentUtils.CheckShareLink(shareLink, false, fileDao, out file);
                    if (!checkLink && file == null)
                        file = fileDao.GetFile(fileID);

                    if (file == null) throw new HttpException((int) HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FileNotFound);
                    if (!checkLink && !Global.GetFilesSecurity().CanEdit(file)) throw new HttpException((int) HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
                    if (file.RootFolderType == FolderType.TRASH) throw new HttpException((int) HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_ViewTrashItem);

                    FileLocker.Add(file.ID);

                    var versionEdit = context.Request[UrlConstant.Version];
                    var currentType = file.ConvertedType ?? FileUtility.GetFileExtension(file.Title);
                    var newType = FileUtility.GetFileExtension(downloadUri);
                    var updateVersion = file.Version > 1 || file.ConvertedType == null || string.IsNullOrEmpty(context.Request[UrlConstant.New]);

                    if ((string.IsNullOrEmpty(versionEdit) || file.Version <= Convert.ToInt32(versionEdit) || currentType != newType)
                        && updateVersion)
                    {
                        file.Version++;
                    }
                    else
                    {
                        updateVersion = false;
                        fileDao.DeleteFileStream(file.ID);
                    }

                    file.ConvertedType = newType;

                    if (string.IsNullOrEmpty(file.ProviderName))
                    {
                        var bytes = new WebClient().DownloadData(downloadUri);
                        file.ContentLength = bytes.Length;

                        using (var stream = new MemoryStream(bytes))
                        {
                            file = fileDao.SaveFile(file, stream);
                        }
                    }
                    else
                    {
                        //TODO: service must convert with outputType param
                        var fileExt = FileUtility.GetFileExtension(file.Title);

                        //???HACK for google & msdoc
                        if (file.ProviderName == nSupportedCloudConfigurations.Google.ToString() && fileExt.Equals(".doc"))
                            fileExt = "docx";

                        using (var readStream = DocumentUtils.GetConvertedFile(file, fileExt, downloadUri).GetBuffered())
                        {
                            if (readStream != null)
                            {
                                file.ContentLength = readStream.Length;
                                file = fileDao.SaveFile(file, readStream);
                            }
                        }
                    }

                    if (!updateVersion) return;

                    Global.PublishUpdateDocument(file.ID);
                }
            }
            catch (Exception e)
            {
                context.Response.Write("{ \"error\": \"true\", \"message\": \"" + e.Message + "\"}");
            }
        }

        private static void CreateFile(HttpContext context)
        {
            var template = context.Request[UrlConstant.Template];
            var fileTitle = context.Request[UrlConstant.FileTitle] ?? context.Request["title"];

            using (var fileDao = Global.DaoFactory.GetFileDao())
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                var folder = folderDao.GetFolder(Global.FolderMy);
                if (folder == null) throw new HttpException((int) HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FolderNotFound);
                if (!Global.GetFilesSecurity().CanCreate(folder)) throw new HttpException((int) HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException_Create);

                var storeTemp = Global.GetStoreTemplate();

                var lang = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture().TwoLetterISOLanguageName;

                var templatePath = DocumentUtils.TemplateDocPath + lang + "/";
                if (!storeTemp.IsDirectory(templatePath))
                    templatePath = DocumentUtils.TemplateDocPath + "default/";

                string templateName;
                const string fileExt = ".docx";

                if (string.IsNullOrEmpty(template))
                {
                    //For ThirdParty use original file type
                    templateName = ("imaginary" + (FileUtility.UsingHtml5(fileExt) ? "html5" : string.Empty))
                                   + fileExt;

                    templatePath = templateName;
                }
                else
                {
                    templateName = template + fileExt;
                    templatePath += templateName;

                    if (!storeTemp.IsFile(templatePath))
                    {
                        templatePath = DocumentUtils.TemplateDocPath + "default/";
                        templatePath += templateName;
                    }
                }

                if (string.IsNullOrEmpty(fileTitle))
                {
                    fileTitle = templateName;
                }
                else
                {
                    fileTitle = fileTitle + fileExt;
                }
                var file = new File
                               {
                                   Title = Global.ReplaceInvalidCharsAndTruncate(fileTitle),
                                   ContentLength = storeTemp.GetFileSize(templatePath),
                                   ContentType = MimeMapping.GetMimeMapping(fileTitle),
                                   ConvertedType = ".zip",
                                   FolderID = folder.ID
                               };

                using (var stream = storeTemp.IronReadStream("", templatePath, 10))
                {
                    file = fileDao.SaveFile(file, stream);
                }

                FilesActivityPublisher.CreateFile(fileDao.GetFile(file.ID));

                context.Response.Redirect(CommonLinkUtility.GetFileWebEditorUrl(file.ID));
            }
        }
    }
}