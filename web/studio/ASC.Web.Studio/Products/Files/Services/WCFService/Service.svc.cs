using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Xml.Linq;
using System.Xml.XPath;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Notify.Model;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.NotifyService;
using ASC.Web.Studio.Utility;
using Microsoft.ServiceModel.Web;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Services.WCFService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceErrorLoggingBehavior]
    public class Service : IFileStorageService
    {
        private static readonly ProgressQueue Tasks = new ProgressQueue(10, TimeSpan.FromMinutes(5), true);
        private static readonly List<object> Updates = new List<object>();

        public ItemList<Folder> GetFolders(String parentId, OrderBy orderBy)
        {
            using (var folderDao = GetFolderDao())
            {
                int total;
                var folders = GetEntries(folderDao.GetFolder(parentId), FilterType.FoldersOnly, Guid.Empty, orderBy, string.Empty, 0, 0, out total);
                return new ItemList<Folder>(folders.OfType<Folder>());
            }
        }

        public Folder GetFolder(String folderId)
        {
            using (var folderDao = GetFolderDao())
            {
                var f = folderDao.GetFolder(folderId);
                return FileSecurity.CanRead(f) ? f : null;
            }
        }

        public DataWrapper GetFolderItems(String parentId, String sfrom, String scount, String sfilter, OrderBy orderBy, String ssubject, String searchText, bool compactView)
        {
            var from = Convert.ToInt32(sfrom);
            var count = Convert.ToInt32(scount);
            var filter = (FilterType) Convert.ToInt32(sfilter);
            var subjectId = string.IsNullOrEmpty(ssubject) ? Guid.Empty : new Guid(ssubject);

            using (var folderDao = GetFolderDao())
            using (var tagDao = GetTagDao())
            {
                var parent = folderDao.GetFolder(parentId);
                ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
                ErrorIf(!FileSecurity.CanRead(parent), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
                ErrorIf(parent.RootFolderType == FolderType.TRASH && !Equals(parent.ID, Global.FolderTrash), FilesCommonResource.ErrorMassage_ViewTrashItem);

                if (Equals(parent.ID, Global.FolderShare))
                    orderBy = new OrderBy(SortedByType.DateAndTime, false);

                int total;
                var entries = GetEntries(parent, filter, subjectId, orderBy, searchText, from, count, out total);

                var breadCrumbs = Global.GetBreadCrumbs(parentId, folderDao);

                var prevVisible = breadCrumbs.ElementAtOrDefault(breadCrumbs.Count() - 2);
                if (prevVisible != null)
                {
                    parent.ParentFolderID = prevVisible.ID;
                }

                parent.Shareable = (parent.RootFolderType == FolderType.USER && parent.RootFolderCreator == SecurityContext.CurrentAccount.ID) ||
                                   (parent.RootFolderType == FolderType.COMMON && Global.IsAdministrator);

                var result = new DataWrapper
                                 {
                                     Folders = entries.OfType<Folder>().ToList(),
                                     Files = entries.OfType<File>().ToList(),
                                     Total = total,

                                     FolderPathParts = new ItemDictionary<object, String>(breadCrumbs.ToDictionary(f => f.ID, f => f.Title)),
                                     FolderInfo = parent
                                 };

                var newtags = tagDao.GetTags(SecurityContext.CurrentAccount.ID, TagType.New);
                result.CountNewInShare = newtags.Count();
                result.Files.ForEach(f => { if (newtags.Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).Contains(f.ID)) f.FileStatus |= FileStatus.IsNew; });
                result.Folders.ForEach(f => { if (newtags.Where(t => t.EntryType == FileEntryType.Folder).Select(t => t.EntryId).Contains(f.ID)) f.NewForMe = true; });

                if (newtags.Count(t => t.EntryType == FileEntryType.Folder && Equals(t.EntryId, parent.ID)) > 0)
                {
                    tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, parent));
                    result.CountNewInShare--;
                }

                var filesSettings = SettingsManager.Instance.LoadSettingsFor<FilesSettings>(SecurityContext.CurrentAccount.ID);
                filesSettings.ContentOrderBy = orderBy;
                filesSettings.CompactViewFolder = compactView;
                SettingsManager.Instance.SaveSettingsFor(filesSettings, SecurityContext.CurrentAccount.ID);

                return result;
            }
        }

        private static IEnumerable<FileEntry> GetEntries(Folder parent, FilterType filter, Guid subjectId, OrderBy orderBy, String searchText, int from, int count, out int total)
        {
            using (var folderDao = GetFolderDao())
            {
                ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);

                var entries = Enumerable.Empty<FileEntry>();

                if (parent.FolderType == FolderType.SHARE)
                {
                    //share
                    var shared = (IEnumerable<FileEntry>) FileSecurity.GetSharesForMe();
                    shared = FilterEntries(shared, filter, subjectId, searchText)
                        .Where(f => f.CreateBy != SecurityContext.CurrentAccount.ID && // don't show my files
                                    f.RootFolderType == FolderType.USER); // don't show common files (common files can read)
                    entries = entries.Concat(shared);

                    parent.TotalFiles = entries.Aggregate(0, (a, f) => a + (f is Folder ? ((Folder) f).TotalFiles : 1));
                    parent.TotalSubFolders = entries.Aggregate(0, (a, f) => a + (f is Folder ? ((Folder) f).TotalSubFolders + 1 : 0));
                }
                else
                {
                    try
                    {
                        var folders = folderDao.GetFolders(parent.ID, orderBy, filter, subjectId, searchText).Cast<FileEntry>();
                        folders = FileSecurity.FilterRead(folders);
                        entries = entries.Concat(folders);

                        var files = folderDao.GetFiles(parent.ID, orderBy, filter, subjectId, searchText).Cast<FileEntry>();
                        files = FileSecurity.FilterRead(files);
                        entries = entries.Concat(files);
                    }
                    catch (Exception ex)
                    {
                        ErrorIf(true, ex.Message);
                    }

                    if (ImportConfiguration.SupportInclusion && (parent.ID.Equals(Global.FolderMy) || parent.ID.Equals(Global.FolderCommon)))
                    {
                        using (var providerDao = GetProviderDao())
                        {
                            var providers = providerDao.GetProvidersInfo(parent.RootFolderType);

                            var folderList = providers
                                .Select(providerInfo =>
                                        //Fake folder. Don't send request to third party
                                        new Folder
                                            {
                                                ID = providerInfo.RootFolderId,
                                                ParentFolderID = parent.ID,
                                                CreateBy = providerInfo.Owner,
                                                CreateOn = providerInfo.CreateOn,
                                                FolderType = FolderType.DEFAULT,
                                                ModifiedBy = providerInfo.Owner,
                                                ModifiedOn = providerInfo.CreateOn,
                                                ProviderId = providerInfo.ID,
                                                ProviderName = providerInfo.ProviderName,
                                                ProviderUserName = providerInfo.UserName,
                                                RootFolderCreator = providerInfo.Owner,
                                                RootFolderId = providerInfo.RootFolderId,
                                                RootFolderType = providerInfo.RootFolderType,
                                                Shareable = false,
                                                Title = providerInfo.CustomerTitle,
                                                TotalFiles = 0,
                                                TotalSubFolders = 0
                                            }
                                )
                                .Where(folder => FileSecurity.CanRead(folder));

                            var thirdPartyFolder = FilterEntries(folderList.Cast<FileEntry>(), filter, subjectId, searchText);
                            entries = entries.Concat(thirdPartyFolder);

                            parent.TotalFiles = thirdPartyFolder.Aggregate(parent.TotalFiles, (a, f) => a + (f is Folder ? ((Folder) f).TotalFiles : 1));
                            parent.TotalSubFolders = thirdPartyFolder.Aggregate(parent.TotalSubFolders, (a, f) => a + (f is Folder ? ((Folder) f).TotalSubFolders + 1 : 0));
                        }
                    }
                }

                entries = SortEntries(entries, orderBy);

                total = entries.Count();
                if (0 < from) entries = entries.Skip(from);
                if (0 < count) entries = entries.Take(count);

                return entries.ToList();
            }
        }

        private static IEnumerable<FileEntry> FilterEntries(IEnumerable<FileEntry> entries, FilterType filter, Guid subjectId, String searchText)
        {
            Func<FileEntry, bool> where = _ => true;

            if (filter == FilterType.ByUser)
            {
                where = f => f.CreateBy == subjectId;
            }
            else if (filter == FilterType.ByDepartment)
            {
                where = f => CoreContext.UserManager.GetUsersByGroup(subjectId).Any(s => s.ID == f.CreateBy);
            }
            else if (filter == FilterType.FilesOnly || filter == FilterType.DocumentsOnly ||
                     filter == FilterType.ImagesOnly || filter == FilterType.PresentationsOnly ||
                     filter == FilterType.SpreadsheetsOnly)
            {
                where = f => f is File && (((File) f).FilterType == filter || filter == FilterType.FilesOnly);
            }
            else if (filter == FilterType.FoldersOnly)
            {
                where = f => f is Folder;
            }
            return entries.Where(where).Where(f => f.Title.ToLower().Contains(searchText.ToLower().Trim()));
        }

        private static IEnumerable<FileEntry> SortEntries(IEnumerable<FileEntry> entries, OrderBy orderBy)
        {
            Comparison<FileEntry> sorter;

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var c = orderBy.IsAsc ? 1 : -1;
            switch (orderBy.SortedBy)
            {
                case SortedByType.Type:
                    sorter = (x, y) =>
                                 {
                                     var cmp = 0;
                                     if (x is File && y is File)
                                         cmp = c*(FileUtility.GetFileExtension((x.Title)).CompareTo(FileUtility.GetFileExtension(y.Title)));
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.Author:
                    sorter = (x, y) =>
                                 {
                                     var cmp = c*string.Compare(x.ModifiedByString, y.ModifiedByString);
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.Size:
                    sorter = (x, y) =>
                                 {
                                     var cmp = 0;
                                     if (x is File && y is File)
                                         cmp = c*((File) x).ContentLength.CompareTo(((File) y).ContentLength);
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.AZ:
                    sorter = (x, y) => c*x.Title.CompareTo(y.Title);
                    break;
                case SortedByType.DateAndTime:
                    sorter = (x, y) =>
                                 {
                                     var cmp = c*DateTime.Compare(x.ModifiedOn, y.ModifiedOn);
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                default:
                    sorter = (x, y) => c*x.Title.CompareTo(y.Title);
                    break;
            }

            // folders on top
            var folders = entries.OfType<Folder>().Cast<FileEntry>().ToList();
            var files = entries.OfType<File>().Cast<FileEntry>().ToList();
            folders.Sort(sorter);
            files.Sort(sorter);

            return folders.Concat(files);
        }

        #region File Manager

        public ItemDictionary<String, String> GetSiblingsFile(String fileId, String sfilter, OrderBy orderBy, String ssubject, String searchText)
        {
            var filter = (FilterType) Convert.ToInt32(sfilter);
            var subjectId = string.IsNullOrEmpty(ssubject) ? Guid.Empty : new Guid(ssubject);

            using (var fileDao = GetFileDao())
            using (var folderDao = GetFolderDao())
            {
                var file = fileDao.GetFile(fileId);
                ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound, true);
                ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile, true);

                var folder = folderDao.GetFolder(file.FolderID);
                ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound, true);
                ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem, true);

                var entries = Enumerable.Empty<FileEntry>();
                if (!FileSecurity.CanRead(folder) &&
                    folder.RootFolderType == FolderType.USER && !Equals(folder.RootFolderId, Global.FolderMy))
                {
                    orderBy = new OrderBy(SortedByType.DateAndTime, false);

                    var shared = (IEnumerable<FileEntry>) FileSecurity.GetSharesForMe();
                    shared = FilterEntries(shared, filter, subjectId, searchText)
                        .Where(f => f is File &&
                                    f.CreateBy != SecurityContext.CurrentAccount.ID && // don't show my files
                                    f.RootFolderType == FolderType.USER); // don't show common files (common files can read)
                    entries = entries.Concat(shared);
                }
                else
                {
                    entries = entries.Concat(folderDao.GetFiles(folder.ID, orderBy, filter, subjectId, searchText).Cast<FileEntry>());
                }

                entries = SortEntries(entries, orderBy);

                var result = new ItemDictionary<string, String>();
                FileSecurity.FilterRead(entries)
                    .OfType<File>()
                    .Where(f => FileUtility.ExtsImagePreviewed.Contains(FileUtility.GetFileExtension(f.Title)))
                    .ToList()
                    .ForEach(f => result.Add(f.ID.ToString(), f.Version + "#" + f.Title));

                return result;
            }
        }

        public File CreateNewFile(String parentId, String fileTitle)
        {
            fileTitle = Global.ReplaceInvalidCharsAndTruncate(fileTitle);

            using (var fileDao = GetFileDao())
            using (var folderDao = GetFolderDao())
            {
                var folder = folderDao.GetFolder(parentId);
                ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
                ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_CreateNewFolderInTrash);
                ErrorIf(!FileSecurity.CanCreate(folder), FilesCommonResource.ErrorMassage_SecurityException_Create);

                var fileExt = FileUtility.GetFileExtension(fileTitle);

                fileTitle = fileTitle.Substring(0, fileTitle.LastIndexOf(fileExt));

                var file = new File
                               {
                                   FolderID = folder.ID
                               };

                switch (FileUtility.GetFileTypeByExtention(fileExt))
                {
                    case FileType.Document:
                        fileExt = ".docx";
                        file.ConvertedType = ".zip";
                        file.ContentType = "application/msword";
                        break;
                    case FileType.Spreadsheet:
                        fileExt = ".xlsx";
                        file.ConvertedType = ".xlsx";
                        file.ContentType = "application/vnd.ms-excel";
                        break;
                    case FileType.Presentation:
                        fileExt = ".pptx";
                        file.ConvertedType = ".zip";
                        file.ContentType = "application/vnd.ms-powerpoint";
                        break;
                    case FileType.Image:
                        fileExt = ".svg";
                        file.ConvertedType = ".zip";
                        file.ContentType = "image/svg+xml";
                        break;
                    default:
                        throw new NotSupportedException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
                }

                file.Title = fileTitle + fileExt;

                if (!string.IsNullOrEmpty(folder.ProviderName))
                    file.ConvertedType = null;

                //For ThirdParty use original file type
                var templatePath =
                    (
                        string.IsNullOrEmpty(folder.ProviderName)
                            ? ("imaginary" + (FileUtility.UsingHtml5(file.Title) ? "html5" : string.Empty))
                            : "original"
                    )
                    + fileExt;

                
                var storeTemp = GetStoreTemplate();

                file.ContentLength = storeTemp.GetFileSize(templatePath);

                try
                {
                    using (var stream = storeTemp.IronReadStream("", templatePath, 10))
                    {
                        file = fileDao.SaveFile(file, stream);
                    }

                    FilesActivityPublisher.CreateFile(fileDao.GetFile(file.ID));
                }
                catch (ArgumentException e)
                {
                    throw GenerateException(e, false);
                }

                return file;
            }
        }

        public void TrackEditFile(String fileId, String docKeyForTrack, String shareLink, bool isFinish)
        {
            var id = DocumentUtils.ParseShareLink(shareLink);
            if (string.IsNullOrEmpty(id))
            {
                ErrorIf(!SecurityContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException, true);
                ErrorIf(!string.IsNullOrEmpty(shareLink), FilesCommonResource.ErrorMassage_BadRequest, true);
                id = fileId;
            }

            ErrorIf(docKeyForTrack != DocumentUtils.GetDocKey(id, -1, DateTime.MinValue), FilesCommonResource.ErrorMassage_SecurityException, true);

            if (isFinish)
            {
                FileLocker.Remove(id);
            }
            else
            {
                try
                {
                    FileLocker.Add(id);
                }
                catch (Exception exception)
                {
                    ErrorIf(true, exception.Message, true);
                }
            }
        }

        public ItemDictionary<String, String> CheckEditing(ItemList<String> filesId)
        {
            ErrorIf(!SecurityContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException, true);
            var result = new ItemDictionary<string, string>();
            foreach (var sid in filesId.Where(sid => (File.GetFileStatus(sid, FileStatus.None) & FileStatus.IsEditing) == FileStatus.IsEditing))
            {
                result[sid.ToString(CultureInfo.InvariantCulture)] = FileEntry.GetUserName(FileLocker.GetLockedBy(sid));
            }
            return result;
        }

        public bool CanEdit(String fileId, String shareLink)
        {
            using (var fileDao = GetFileDao())
            {
                File file;

                var checkLink = DocumentUtils.CheckShareLink(shareLink, false, fileDao, out file);
                if (!checkLink && file == null)
                    file = fileDao.GetFile(fileId);

                ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound, true);
                ErrorIf(!checkLink && !FileSecurity.CanEdit(file), FilesCommonResource.ErrorMassage_SecurityException_RenameFile, true);
                ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem,true);
                ErrorIf(!FileUtility.ExtsWebEdited.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase), FilesCommonResource.ErrorMassage_NotSupportedFormat, true);
                ErrorIf((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing, FilesCommonResource.ErrorMassage_UpdateEditingFile, true);
            }

            return true;
        }

        public File FileRename(String fileId, String newTitle)
        {
            using (var fileDao = GetFileDao())
            {
                var file = fileDao.GetFile(fileId);
                ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
                ErrorIf(!FileSecurity.CanEdit(file), FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
                ErrorIf((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing, FilesCommonResource.ErrorMassage_UpdateEditingFile);

                newTitle = Global.ReplaceInvalidCharsAndTruncate(newTitle);

                var fileAccess = file.Access;

                if (String.Compare(file.Title, newTitle, false) != 0)
                {
                    var newFileID = fileDao.FileRename(file.ID, newTitle);

                    file = fileDao.GetFile(newFileID);
                    file.Access = fileAccess;

                }
                return file;
            }
        }

        public ItemList<File> GetFileHistory(String fileId)
        {
            using (var fileDao = GetFileDao())
            {
                var file = fileDao.GetFile(fileId);
                ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                return new ItemList<File>(fileDao.GetFileHistory(fileId));
            }
        }

        public File UpdateToVersion(String fileId, String version)
        {
            using (var fileDao = GetFileDao())
            {
                var fromFile = fileDao.GetFile(fileId, Convert.ToInt32(version));
                ErrorIf(!FileSecurity.CanEdit(fromFile), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
                ErrorIf((fromFile.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing, FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile);

                lock (Updates)
                {
                    ErrorIf(Updates.Contains(fromFile.ID), FilesCommonResource.ErrorMassage_UpdateEditingFile);
                    Updates.Add(fromFile.ID);
                }

                try
                {
                    var currFile = fileDao.GetFile(fileId);
                    var newFile = new File
                                      {
                                          ID = fromFile.ID,
                                          Version = currFile.Version + 1,
                                          Title = fromFile.Title,
                                          ContentLength = fromFile.ContentLength,
                                          ContentType = fromFile.ContentType,
                                          FileStatus = fromFile.FileStatus,
                                          FolderID = fromFile.FolderID,
                                          CreateBy = fromFile.CreateBy,
                                          CreateOn = fromFile.CreateOn,
                                          ModifiedBy = fromFile.ModifiedBy,
                                          ModifiedOn = fromFile.ModifiedOn,
                                          ConvertedType = fromFile.ConvertedType
                                      };

                    using (var stream = fileDao.GetFileStream(fromFile))
                    {
                        newFile = fileDao.SaveFile(newFile, stream);
                    }

                    Global.PublishUpdateDocument(newFile.ID);

                    return newFile;
                }
                finally
                {
                    lock (Updates) Updates.Remove(fromFile.ID);
                }
            }
        }

        #endregion

        #region News

        public IEnumerable<File> GetFileUpdates(DateTime from, DateTime to)
        {
            return GetFileDao().GetUpdates(from, to).Where(x => FileSecurity.CanRead(x));
        }

        public IEnumerable<Folder> GetFolderUpdates(DateTime from, DateTime to)
        {
            return GetFolderDao().GetUpdates(from, to).Where(x => FileSecurity.CanRead(x));
        }

        #endregion

        #region Import

        public ItemList<DataToImport> GetImportDocs(string source, AuthData authData)
        {
            switch (source)
            {
                case "boxnet":
                    return DocumentsToDataImport(new BoxDocumentProvider(authData).GetDocuments());
                case "google":
                    using (var google = new GoogleDocumentProvider(authData))
                    {
                        return DocumentsToDataImport(google.GetDocuments());
                    }
                case "zoho":
                    try
                    {
                        var zoho = new ZohoDocumentProvider(authData);
                        return DocumentsToDataImport(zoho.GetDocuments());
                    }
                    catch (Exception e)
                    {
                        ErrorIf(true, e.Message);
                        return null;
                    }
                default:
                    ErrorIf(true, "Unknown provider");
                    return null;
            }
        }

        public ItemList<FileOperationResult> ExecImportDocs(string login, string password, string token, string source, string parentId, string ignoreCoincidenceFiles, List<DataToImport> dataToImport)
        {
            var authData = new AuthData(login, password, token);

            IDocumentProvider provider;
            String folderName;

            switch (source)
            {
                case "boxnet":
                    provider = new BoxDocumentProvider(authData);
                    folderName = FilesCommonResource.ImportFromBoxNet;
                    return ImportDocuments(provider, dataToImport, parentId, ignoreCoincidenceFiles, folderName);
                case "google":
                    folderName = FilesCommonResource.ImportFromGoogle;
                    using (var google = new GoogleDocumentProvider(authData))
                    {
                        return ImportDocuments(google, dataToImport, parentId, ignoreCoincidenceFiles, folderName);
                    }
                case "zoho":
                    provider = new ZohoDocumentProvider(authData);
                    folderName = FilesCommonResource.ImportFromZoho;
                    return ImportDocuments(provider, dataToImport, parentId, ignoreCoincidenceFiles, folderName);
                default:
                    ErrorIf(true, "Unknown provider");
                    return null;
            }
        }

        private ItemList<FileOperationResult> ImportDocuments(IDocumentProvider provider, List<DataToImport> documents, String parentId, String overwriteStr, String folderName)
        {
            bool overwrite;
            bool.TryParse(overwriteStr, out overwrite);

            var import = new FileImportOperation(
                CoreContext.TenantManager.GetCurrentTenant(),
                provider,
                documents,
                parentId,
                overwrite,
                folderName);

            lock (Tasks)
            {
                var oldTask = Tasks.GetStatus(import.Id.ToString());
                ErrorIf(oldTask != null && !oldTask.IsCompleted, FilesCommonResource.ErrorMassage_ManyImports, true);
                Tasks.Add(import);
            }
            return GetTasksStatuses();
        }

        private static ItemList<DataToImport> DocumentsToDataImport(IEnumerable<Document> documents)
        {
            var folders = documents.Where(d => d.IsFolder).ToDictionary(d => d.Id);
            return new ItemList<DataToImport>(
                documents
                    .Where(d => !d.IsFolder)
                    .Select(d => new DataToImport
                                     {
                                         Title = GetDocumentPath(folders, d.Parent) + Global.ReplaceInvalidCharsAndTruncate(d.Title),
                                         ContentLink = d.ContentLink,
                                         CreateBy = d.CreateBy ?? CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).DisplayUserName(),
                                         CreateOn = d.CreateOn.ToShortDateString()
                                     })
                    .OrderBy(d => d.Title));
        }

        private static string GetDocumentPath(IDictionary<string, Document> folders, string parentId)
        {
            if (string.IsNullOrEmpty(parentId) || !folders.ContainsKey(parentId)) return string.Empty;
            var parent = folders[parentId];
            return GetDocumentPath(folders, parent.Parent) + Global.ReplaceInvalidCharsAndTruncate(parent.Title) + "/";
        }

        #endregion Import

        #region ThirdParty

        public Folder SaveThirdParty(ThirdPartyParams thirdPartyParams)
        {
            using (var folderDao = GetFolderDao())
            using (var providerDao = GetProviderDao())
            {
                var parentFolder = folderDao.GetFolder(thirdPartyParams.Corporate ? Global.FolderCommon : Global.FolderMy);
                ErrorIf(!FileSecurity.CanCreate(parentFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);

                var folderType = thirdPartyParams.Corporate ? FolderType.COMMON : FolderType.USER;

                int curProviderId;

                if (string.IsNullOrEmpty(thirdPartyParams.ProviderId))
                {
                    ErrorIf(!ImportConfiguration.SupportInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);
                    curProviderId = providerDao.SaveProviderInfo(thirdPartyParams.ProviderName, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
                }
                else
                {
                    curProviderId = int.Parse(thirdPartyParams.ProviderId);
                    curProviderId = providerDao.UpdateProviderInfo(curProviderId, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
                }

                var provider = providerDao.GetProviderInfo(curProviderId);

                var folder = GetFolder(provider.RootFolderId.ToString());

                return folder;
            }
        }

        public void DeleteThirdParty(String folderId)
        {
            using (var folderDao = GetFolderDao())
            using (var providerDao = GetProviderDao())
            {
                var folder = folderDao.GetFolder(folderId);
                ErrorIf(!FileSecurity.CanDelete(folder), FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder, true);

                providerDao.RemoveProviderInfo(folder.ProviderId);
            }
        }

        #endregion

        public ItemList<FileOperationResult> GetTasksStatuses()
        {
            ErrorIf(!SecurityContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException, true);
            var operations = Tasks.GetItems()
                .Where(t => t is FileOperation && ((FileOperation) t).Owner == SecurityContext.CurrentAccount.ID)
                .Select(o => Tasks.GetStatus(o.Id.ToString()))
                .Select(o => ((FileOperation) o).GetResult())
                .ToList();

            return new ItemList<FileOperationResult>(operations);
        }

        public ItemList<FileOperationResult> TerminateTasks(bool import)
        {
            var statuses = GetTasksStatuses().ToList();
            statuses.ForEach(s => { if ((s.OperationType == FileOperationType.Import) == import) s.Progress = 100; });

            foreach (var i in Tasks.GetItems())
            {
                try
                {
                    var task = i as FileOperation;
                    if (task != null && (task.GetResult().OperationType == FileOperationType.Import) == import)
                    {
                        task.Terminate();
                        Tasks.Remove(task);
                    }
                }
                catch (Exception ex)
                {
                    log4net.LogManager.GetLogger("ASC.Web.Files").Error(ex);
                }
            }
            return new ItemList<FileOperationResult>(statuses);
        }

        public ItemList<FileOperationResult> BulkDownload(ItemList<String> items)
        {
            List<object> folders;
            List<object> files;
            ParseArrayItems(items, out folders, out files);

            var task = new FileDownloadOperation(CoreContext.TenantManager.GetCurrentTenant(), folders, files);

            lock (Tasks)
            {
                var oldTask = Tasks.GetStatus(task.Id.ToString());
                if (oldTask != null)
                {
                    ErrorIf(!oldTask.IsCompleted, FilesCommonResource.ErrorMassage_ManyDownloads, true);
                    Tasks.Remove(oldTask);
                }
                Tasks.Add(task);
            }

            return GetTasksStatuses();
        }


        public Folder CreateNewFolder(String title, String parentId)
        {
            if (string.IsNullOrEmpty(title) || String.IsNullOrEmpty(parentId)) throw new ArgumentException();

            title = Global.ReplaceInvalidCharsAndTruncate(title);
            using (var folderDao = GetFolderDao())
            {
                var parent = folderDao.GetFolder(parentId);
                ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
                ErrorIf(!FileSecurity.CanCreate(parent), FilesCommonResource.ErrorMassage_SecurityException_Create);

                try
                {
                    return folderDao.GetFolder(folderDao.SaveFolder(new Folder { Title = title, ParentFolderID = parent.ID }));
                }
                catch (ArgumentException e)
                {
                    throw GenerateException(e, false);
                }
            }
        }

        public Folder FolderRename(String folderId, String newTitle)
        {
            using (var folderDao = GetFolderDao())
            {
                var folder = folderDao.GetFolder(folderId);
                ErrorIf(!FileSecurity.CanEdit(folder), FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);

                var folderAccess = folder.Access;

                newTitle = Global.ReplaceInvalidCharsAndTruncate(newTitle);

                if (String.Compare(folder.Title, newTitle, false) != 0)
                {
                    var newFolderID = folderDao.RenameFolder(folder.ID, newTitle);
                    folder = folderDao.GetFolder(newFolderID);
                    folder.Access = folderAccess;
                }

                return folder;
            }
        }

        public File GetLastFileVersion(String fileId)
        {
            using (var fileDao = GetFileDao())
            {
                var file = fileDao.GetFile(fileId);
                ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
                ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                return file;
            }
        }

        public ItemDictionary<String, String> MoveOrCopyFilesCheck(ItemList<String> items, String destFolderId)
        {
            var result = new ItemDictionary<string, String>();
            if (items.Count == 0) return result;

            List<object> folders;
            List<object> files;
            ParseArrayItems(items, out folders, out files);

            using (var folderDao = GetFolderDao())
            using (var fileDao = GetFileDao())
            {
                var toFolder = folderDao.GetFolder(destFolderId);
                if (toFolder == null) return result;
                ErrorIf(!FileSecurity.CanCreate(toFolder), FilesCommonResource.ErrorMassage_SecurityException_Create, true);

                foreach (var id in files)
                {
                    var file = fileDao.GetFile(id);
                    if (file != null && fileDao.IsExist(file.Title, toFolder.ID))
                    {
                        result.Add(id.ToString(), file.Title);
                    }
                }

                foreach (var pair in folderDao.CanMoveOrCopy(folders.ToArray(), toFolder.ID))
                {
                    result.Add(pair.Key.ToString(), pair.Value);
                }
            }
            return result;
        }

        public ItemList<FileOperationResult> MoveOrCopyItems(ItemList<String> items, String destFolderId, String overwriteFiles, String isCopyOperation)
        {
            if (items.Count != 0)
            {
                List<object> foldersId;
                List<object> filesId;
                ParseArrayItems(items, out foldersId, out filesId);

                var task = new FileMoveCopyOperation(
                    CoreContext.TenantManager.GetCurrentTenant(),
                    foldersId,
                    filesId,
                    destFolderId,
                    Convert.ToBoolean(isCopyOperation),
                    Convert.ToBoolean(overwriteFiles) ? FileConflictResolveType.Overwrite : FileConflictResolveType.Skip);

                Tasks.Add(task);
            }
            return GetTasksStatuses();
        }

        public ItemList<FileOperationResult> DeleteItems(ItemList<String> items)
        {
            List<object> foldersId;
            List<object> filesId;
            ParseArrayItems(items, out foldersId, out filesId);

            using (var folderDao = GetFolderDao())
            using (var fileDao = GetFileDao())
            {
                foldersId.ForEach(folderId =>
                                      {
                                          var folder = folderDao.GetFolder(folderId);
                                          if (folder != null)
                                          {
                                              if (folder.RootFolderType == FolderType.BUNCH ||
                                                  folder.RootFolderType == FolderType.TRASH)
                                                  FilesActivityPublisher.DeleteFolder(folder);
                                          }
                                      });
                filesId.ForEach(fileId =>
                                    {
                                        var file = fileDao.GetFile(fileId);
                                        if (file != null)
                                        {
                                            if (file.RootFolderType == FolderType.BUNCH ||
                                                file.RootFolderType == FolderType.TRASH)
                                                FilesActivityPublisher.DeleteFile(file);
                                        }
                                    });
            }

            var task = new FileDeleteOperation(CoreContext.TenantManager.GetCurrentTenant(), foldersId, filesId);
            Tasks.Add(task);

            return GetTasksStatuses();
        }

        public ItemList<FileOperationResult> EmptyTrash()
        {
            using (var folderDao = GetFolderDao())
            using (var fileDao = GetFileDao())
            {
                var trashId = folderDao.GetFolderIDTrash(true);
                var foldersId = folderDao.GetFolders(trashId).Select(f => f.ID).ToList();
                var filesId = folderDao.GetFiles(trashId, false).ToList();

                foldersId.ForEach(folderId =>
                                      {
                                          var folder = folderDao.GetFolder(folderId);
                                          if (folder.RootFolderType == FolderType.BUNCH ||
                                              folder.RootFolderType == FolderType.TRASH)
                                              FilesActivityPublisher.DeleteFolder(folder);
                                      });
                filesId.ForEach(fileId =>
                                    {
                                        var file = fileDao.GetFile(fileId);
                                        if (file.RootFolderType == FolderType.BUNCH ||
                                            file.RootFolderType == FolderType.TRASH)
                                            FilesActivityPublisher.DeleteFile(file);
                                    });

                var task = new FileDeleteOperation(CoreContext.TenantManager.GetCurrentTenant(), foldersId, filesId);
                Tasks.Add(task);
            }
            return GetTasksStatuses();
        }

        public ItemList<AceWrapper> GetSharedInfo(String objectId)
        {
            ErrorIf(!SecurityContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException, true);
            ErrorIf(string.IsNullOrEmpty(objectId), FilesCommonResource.ErrorMassage_BadRequest, true);

            var entryType = objectId.StartsWith("file_") ? FileEntryType.File : FileEntryType.Folder;
            var entryId = objectId.Substring((entryType == FileEntryType.File ? "file_" : "folder_").Length);

            var shareLink = FileShare.Restrict;

            var result = new ItemList<AceWrapper>();

            using (var folderDao = GetFolderDao())
            using (var fileDao = GetFileDao())
            {
                var entry = entryType == FileEntryType.File
                                ? (FileEntry) fileDao.GetFile(entryId)
                                : (FileEntry) folderDao.GetFolder(entryId);

                ErrorIf(entry.RootFolderType == FolderType.COMMON && !Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException, true);
                ErrorIf(entry.RootFolderType == FolderType.USER && !Equals(entry.RootFolderId, Global.FolderMy), FilesCommonResource.ErrorMassage_SecurityException, true);

                var records = FileSecurity
                    .GetShares(entry)
                    .GroupBy(r => r.Subject)
                    .Select(g => g.OrderBy(r => r.Level).ThenByDescending(r => r.Share).FirstOrDefault());

                foreach (var r in records)
                {
                    if (r.Subject == FileConstant.ShareLinkId)
                    {
                        shareLink = r.Share;
                    }
                    else
                    {
                        var u = CoreContext.UserManager.GetUsers(r.Subject);
                        var isgroup = false;
                        var title = u.DisplayUserName(false);

                        if (u.ID == ASC.Core.Users.Constants.LostUser.ID)
                        {
                            var g = CoreContext.GroupManager.GetGroupInfo(r.Subject);
                            isgroup = true;
                            title = g.Name;

                            if (g.ID == ASC.Core.Users.Constants.GroupAdmin.ID)
                                title = FilesCommonResource.Admin;
                            if (g.ID == ASC.Core.Users.Constants.GroupEveryone.ID)
                                title = FilesCommonResource.Everyone;

                            if (g.ID == ASC.Core.Users.Constants.LostGroupInfo.ID)
                            {
                                FileSecurity.RemoveSubject(r.Subject);
                                continue;
                            }
                        }

                        var w = new AceWrapper
                                    {
                                        SubjectId = r.Subject,
                                        SubjectName = title,
                                        SubjectGroup = isgroup,
                                        Share = r.Share,
                                        Owner =
                                            entry.RootFolderType == FolderType.USER
                                                ? entry.RootFolderCreator == r.Subject
                                                : entry.CreateBy == r.Subject,
                                    };
                        result.Add(w);
                    }
                }

                if (entryType == FileEntryType.File && !result.Any(w => w.SubjectId == FileConstant.ShareLinkId))
                {
                    var w = new AceWrapper
                                {
                                    SubjectId = FileConstant.ShareLinkId,
                                    SubjectName = DocumentUtils.GetShareLinkParam(entryId),
                                    SubjectGroup = true,
                                    Share = shareLink,
                                    Owner = false
                                };
                    result.Add(w);
                }

                if (!result.Any(w => w.Owner))
                {
                    var ownerId = entry.RootFolderType == FolderType.USER ? entry.RootFolderCreator : entry.CreateBy;
                    var w = new AceWrapper
                                {
                                    SubjectId = ownerId,
                                    SubjectName = FileEntry.GetUserName(ownerId),
                                    SubjectGroup = false,
                                    Share = FileShare.ReadWrite,
                                    Owner = true,
                                };
                    result.Add(w);
                }
                if (entry.RootFolderType == FolderType.COMMON)
                {
                    if (result.All(w => w.SubjectId != ASC.Core.Users.Constants.GroupAdmin.ID))
                    {
                        var w = new AceWrapper
                                    {
                                        SubjectId = ASC.Core.Users.Constants.GroupAdmin.ID,
                                        SubjectName = FilesCommonResource.Admin,
                                        SubjectGroup = true,
                                        Share = FileShare.ReadWrite,
                                        Owner = false,
                                        LockedRights = true,
                                    };
                        result.Add(w);
                    }
                    if (result.All(w => w.SubjectId != ASC.Core.Users.Constants.GroupEveryone.ID))
                    {
                        var w = new AceWrapper
                                    {
                                        SubjectId = ASC.Core.Users.Constants.GroupEveryone.ID,
                                        SubjectName = FilesCommonResource.Everyone,
                                        SubjectGroup = true,
                                        Share = FileSecurity.DefaultCommonShare,
                                        Owner = false,
                                    };
                        result.Add(w);
                    }
                }
            }

            result.Sort((x, y) => string.Compare(x.SubjectName, y.SubjectName));

            return result;
        }

        public bool SetAceObject(ItemList<AceWrapper> aceWrappers, String objectID, bool notify, String message)
        {
            ErrorIf(string.IsNullOrEmpty(objectID), FilesCommonResource.ErrorMassage_BadRequest, true);

            using (var ddao = GetFolderDao())
            using (var fdao = GetFileDao())
            using (var tagDao = GetTagDao())
            {
                Debug.Assert(objectID != null, "objectID != null");
                var entryType = objectID.StartsWith("file_") ? FileEntryType.File : FileEntryType.Folder;
                var entryId = objectID.Substring((entryType == FileEntryType.File ? "file_" : "folder_").Length);
                var entry = entryType == FileEntryType.File
                                ? (FileEntry) fdao.GetFile(entryId)
                                : (FileEntry) ddao.GetFolder(entryId);

                ErrorIf(entry == null, FilesCommonResource.ErrorMassage_BadRequest, true);

                ErrorIf(entry.RootFolderType == FolderType.COMMON && !Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException, true);
                ErrorIf(entry.RootFolderType == FolderType.USER && !Equals(entry.RootFolderId, Global.FolderMy), FilesCommonResource.ErrorMassage_SecurityException, true);
                //ErrorIf(entry.RootFolderType == FolderType.USER && entryType == FileEntryType.Folder, FilesCommonResource.ErrorMassage_NotSupportedFormat, true);

                var defaultShare = entry.RootFolderType == FolderType.COMMON
                                       ? FileSecurity.DefaultCommonShare
                                       : FileSecurity.DefaultMyShare;

                var subscriptionProvider = NotifySource.Instance.GetSubscriptionProvider();
                var recipientsProvider = NotifySource.Instance.GetRecipientsProvider();

                foreach (var w in aceWrappers.OrderByDescending(ace => ace.SubjectGroup))
                {
                    var subjects = FileSecurity.GetUserSubjects(w.SubjectId);

                    if (entry.RootFolderType == FolderType.COMMON
                        && subjects.Contains(ASC.Core.Users.Constants.GroupAdmin.ID))
                        continue;

                    var ace = FileSecurity.GetShares(entry)
                        .Where(r => subjects.Contains(r.Subject))
                        .OrderBy(r => subjects.IndexOf(r.Subject))
                        .ThenByDescending(r => r.Share)
                        .FirstOrDefault();

                    var parentShare = ace != null ? ace.Share : defaultShare;
                    var share = parentShare == w.Share ? FileShare.None : w.Share;

                    if (w.SubjectId == FileConstant.ShareLinkId)
                        share = w.Share == FileShare.Restrict ? FileShare.None : w.Share;

                    FileSecurity.Share(entryId, entryType, w.SubjectId, share);

                    if (entry.RootFolderType != FolderType.USER
                        || entryType == FileEntryType.Folder
                        || w.SubjectId == FileConstant.ShareLinkId)
                        continue;

                    var recipients = new List<Notify.Recipients.IRecipient>();
                    var listUsersId = new List<Guid>();
                    if (w.SubjectGroup)
                        listUsersId = CoreContext.UserManager.GetUsersByGroup(w.SubjectId).Select(ui => ui.ID).ToList();
                    else
                        listUsersId.Add(w.SubjectId);

                    listUsersId.ForEach(id =>
                                            {
                                                if (id == SecurityContext.CurrentAccount.ID)
                                                    return;

                                                var recipient = recipientsProvider.GetRecipient(id.ToString());

                                                if (share == FileShare.Read || share == FileShare.ReadWrite)
                                                {
                                                    if (subscriptionProvider.IsSubscribed(NotifyConstants.Event_ShareDocument, recipient, null))
                                                    {
                                                        subscriptionProvider.Subscribe(NotifyConstants.Event_UpdateDocument, entry.UniqID, recipient);
                                                        recipients.Add(recipient);
                                                    }

                                                    tagDao.SaveTags(Tag.New(id, entry));
                                                }
                                                else
                                                {
                                                    subscriptionProvider.UnSubscribe(NotifyConstants.Event_UpdateDocument, entry.UniqID, recipient);
                                                    recipients.Remove(recipient);
                                                    tagDao.RemoveTags(Tag.New(id, entry));
                                                }
                                            });

                    if (notify)
                    {
                        NotifyClient.SendShareNotice((File) entry, share, recipients.ToArray(), message);
                    }
                }
                entry = entryType == FileEntryType.File
                            ? (FileEntry) fdao.GetFile(entryId)
                            : (FileEntry) ddao.GetFolder(entryId);

                return entry.SharedByMe;
            }
        }

        public int RemoveAce(ItemList<String> items)
        {
            ErrorIf(!SecurityContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException, true);
            List<object> foldersId;
            List<object> filesId;
            ParseArrayItems(items, out foldersId, out filesId);

            using (var folderDao = GetFolderDao())
            using (var fileDao = GetFileDao())
            using (var tagDao = GetTagDao())
            {

                foldersId.ForEach(folderId =>
                                      {
                                          var folder = folderDao.GetFolder(folderId);

                                          if (folder.RootFolderType != FolderType.USER || Equals(folder.RootFolderId, Global.FolderMy))
                                              return;

                                          FileSecurity.Share(folder.ID, FileEntryType.Folder, SecurityContext.CurrentAccount.ID, FileSecurity.DefaultMyShare);

                                          var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());
                                          NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_UpdateDocument, folder.UniqID, recipient);

                                          tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, folder));
                                      });

                filesId.ForEach(fileId =>
                                    {
                                        var file = fileDao.GetFile(fileId);

                                        if (file.RootFolderType != FolderType.USER || Equals(file.RootFolderId, Global.FolderMy))
                                            return;

                                        FileSecurity.Share(file.ID, FileEntryType.File, SecurityContext.CurrentAccount.ID, FileSecurity.DefaultMyShare);

                                        var recipient = NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());
                                        NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_UpdateDocument, file.UniqID, recipient);

                                        tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, file));
                                    });

                return tagDao.GetTags(SecurityContext.CurrentAccount.ID, TagType.New).Count();
            }
        }

        public String GetShortenLink(String fileId, String longUrl, String docKey)
        {
            var id = fileId;
            var longUri = new Uri(longUrl);
            var keyId = DocumentUtils.ParseShareLink(docKey);
            ErrorIf(id != keyId, FilesCommonResource.ErrorMassage_BadRequest, true);

            var url = string.Format(Global.BitlyUrl, Uri.EscapeDataString(longUri.ToString()));

            var response = XDocument.Load(url);

            ErrorIf(response.XPathSelectElement("/response/status_code").Value != ((int) HttpStatusCode.OK).ToString(CultureInfo.InvariantCulture), FilesCommonResource.ErrorMassage_BadRequest, true);
            var data = response.XPathSelectElement("/response/data/url");

            return data.Value;
        }

        public int MarkAsRead(ItemList<String> items)
        {
            List<object> foldersId;
            List<object> filesId;
            ParseArrayItems(items, out foldersId, out filesId);

            using (var folderDao = GetFolderDao())
            using (var fileDao = GetFileDao())
            using (var tagDao = GetTagDao())
            {
                foldersId.ForEach(folderId =>
                                      {
                                          var folder = folderDao.GetFolder(folderId);
                                          ErrorIf(!FileSecurity.CanRead(folder), FilesCommonResource.ErrorMassage_SecurityException_ReadFolder, true);
                                          tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, folder));
                                      });

                filesId.ForEach(fileId =>
                                    {
                                        var file = fileDao.GetFile(fileId);
                                        ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile, true);
                                        tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, file));
                                    });

                return tagDao.GetTags(SecurityContext.CurrentAccount.ID, TagType.New).Count();
            }
        }

        private static FileSecurity FileSecurity
        {
            get { return Global.GetFilesSecurity(); }
        }

        private static IFolderDao GetFolderDao()
        {
            return Global.DaoFactory.GetFolderDao();
        }

        private static IFileDao GetFileDao()
        {
            return Global.DaoFactory.GetFileDao();
        }

        private static ITagDao GetTagDao()
        {
            return Global.DaoFactory.GetTagDao();
        }

        private static IDataStore GetStoreTemplate()
        {
            return Global.GetStoreTemplate();
        }

        private static IProviderDao GetProviderDao()
        {
            return Global.DaoFactory.GetProviderDao();
        }

        private static void ParseArrayItems(ItemList<String> data, out List<object> folderId, out List<object> filesId)
        {
            //TODO:!!!!Fix
            folderId = new List<object>();
            filesId = new List<object>();
            foreach (var id in data)
            {
                if (id.StartsWith("file_")) filesId.Add(id.Substring("file_".Length));
                if (id.StartsWith("folder_")) folderId.Add(id.Substring("folder_".Length));
            }
        }

        private static void ErrorIf(bool condition, string errorMessage)
        {
            ErrorIf(condition, errorMessage, false);
        }

        private static void ErrorIf(bool condition, string errorMessage, bool json)
        {
            if (condition) throw GenerateException(new Exception(errorMessage), json);
        }

        private static WebProtocolException GenerateException(Exception error, bool json)
        {
            var weberror = new WebProtocolException(HttpStatusCode.BadRequest, error.Message, error);
            if (!json)
            {
                var element = new XElement("error", new XElement("message", FilesCommonResource.ErrorMassage_BadRequest));
                var current = element;
                var inner = error;
                while (inner != null)
                {
                    var el = new XElement("inner",
                                          new XElement("message", inner.Message),
                                          new XElement("type", inner.GetType()),
                                          new XElement("source", inner.Source),
                                          new XElement("stack", inner.StackTrace));
                    current.Add(el);
                    current = el;
                    inner = inner.InnerException;
                }

                weberror = new WebProtocolException(HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest, element, false, error);
            }

            weberror.Data["message"] = error.Message;

            return weberror;
        }
    }
}