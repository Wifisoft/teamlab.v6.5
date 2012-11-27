using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using ASC.Common.Web;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Thrdparty.Configuration;
using ASC.Web.Files.Import;
using ASC.Web.Files.Services.NotifyService;
using ASC.Web.Studio.Utility;
using File = ASC.Files.Core.File;
using ASC.Files.Core.Security;
using ASC.Core;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files.Classes
{
    public class Global
    {
        public const int MAX_TITLE = 170;

        public static readonly Regex InvalidTitleChars = new Regex("[@#$%&*\\+:;\"'<>?|\\\\/]");

        public static IDaoFactory DaoFactory
        {
            get { return new DaoFactory(TenantProvider.CurrentTenantID, FileConstant.DatabaseId); }
        }

        public static object FolderMy
        {
            get { return SecurityContext.IsAuthenticated ? GetFolderIdAndProccessFirstVisit(true) : 0; }
        }

        public static object FolderCommon
        {
            get { return GetFolderIdAndProccessFirstVisit(false); }
        }

        public static object FolderShare
        {
            get
            {
                using (var dao = DaoFactory.GetFolderDao())
                    return EnableShare ? dao.GetFolderIDShare(true) : 0;
            }
        }

        public static object FolderTrash
        {
            get
            {
                using (var dao = DaoFactory.GetFolderDao())
                    return SecurityContext.IsAuthenticated ? dao.GetFolderIDTrash(true) : 0;
            }
        }

        public static bool EnableShare { get; private set; }

        public static bool EnableThirdParty { get; private set; }

        public static string BitlyUrl { get; private set; }

        public static bool IsAdministrator
        {
            get { return FileSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID); }
        }


        static Global()
        {
            const StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

            EnableShare = bool.TrueString.Equals(WebConfigurationManager.AppSettings["files.sharing"] ?? "true", cmp)
                          && !SetupInfo.IsPersonal
                          && SecurityContext.IsAuthenticated;

            EnableThirdParty = bool.TrueString.Equals(WebConfigurationManager.AppSettings["files.thirdparty.enable"] ?? "true", cmp)
                               && ImportConfiguration.SupportInclusion;

            BitlyUrl = KeyStorage.Get("bitly-url");
        }

        public static IDataStore GetStore()
        {
            return StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), FileConstant.StorageModule);
        }

        public static IDataStore GetStoreTemplate()
        {
            return StorageFactory.GetStorage(string.Empty, FileConstant.StorageDomainTemplate);
        }

        public static FileSecurity GetFilesSecurity()
        {
            return new FileSecurity(DaoFactory);
        }

        public static List<Folder> GetBreadCrumbs(object folderId)
        {
            using (var folderDao = DaoFactory.GetFolderDao())
            {
                return GetBreadCrumbs(folderId, folderDao);
            }
        }

        public static List<Folder> GetBreadCrumbs(object folderId, IFolderDao folderDao)
        {
            var breadCrumbs = GetFilesSecurity().FilterRead(folderDao.GetParentFolders(folderId)).ToList();

            var firstVisible = breadCrumbs.ElementAtOrDefault(0);

            if (firstVisible != null && firstVisible.FolderType == FolderType.DEFAULT) //not first level
            {
                Folder root = null;

                if (string.IsNullOrEmpty(firstVisible.ProviderName))
                {
                    root = folderDao.GetFolder(folderDao.GetFolderIDShare(false));
                }
                else
                {
                    switch (firstVisible.RootFolderType)
                    {
                        case FolderType.USER:
                            root = folderDao.GetFolder(folderDao.GetFolderIDUser(false));
                            break;
                        case FolderType.COMMON:
                            root = folderDao.GetFolder(folderDao.GetFolderIDCommon(false));
                            break;
                    }
                }
                if (root != null)
                {
                    firstVisible.ParentFolderID = root.ID;
                    breadCrumbs.Insert(0, root);
                }
            }

            return breadCrumbs;
        }

        public static string ReplaceInvalidCharsAndTruncate(string title)
        {
            if (string.IsNullOrEmpty(title)) return title;
            if (MAX_TITLE < title.Length)
            {
                var pos = title.LastIndexOf('.');
                if (MAX_TITLE - 20 < pos)
                {
                    title = title.Substring(0, MAX_TITLE - (title.Length - pos)) + title.Substring(pos);
                }
                else
                {
                    title = title.Substring(0, MAX_TITLE);
                }
            }
            return InvalidTitleChars.Replace(title, "_");
        }

        public static void PublishUpdateDocument(object fileId)
        {
            var file = DaoFactory.GetFileDao().GetFile(fileId);

            FilesActivityPublisher.UpdateFile(file);

            NotifyClient.SendUpdateNotice(file);

            var newTags =
                NotifySource.Instance.GetSubscriptionProvider()
                    .GetRecipients(NotifyConstants.Event_UpdateDocument, file.UniqID)
                    .Select(id => new Guid(id.ID))
                    .Where(id => id != SecurityContext.CurrentAccount.ID)
                    .Select(r => Tag.New(r, file)).ToArray();


            DaoFactory.GetTagDao().SaveTags(newTags);
        }

        #region Generate start documents

        private const string StartDocPath = "startdocuments/";

        private static object GetFolderIdAndProccessFirstVisit(bool my)
        {
            using (var folderDao = DaoFactory.GetFolderDao())
            using (var fileDao = DaoFactory.GetFileDao())
            {
                var id = my ? folderDao.GetFolderIDUser(false) : folderDao.GetFolderIDCommon(false);

                if (Equals(id, 0)) //TODO: think about 'null'
                {
                    id = my ? folderDao.GetFolderIDUser(true) : folderDao.GetFolderIDCommon(true);

                    //Copy start document
                    var culture = my ? CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture() : CoreContext.TenantManager.GetCurrentTenant().GetCulture();
                    var storeTemp = GetStoreTemplate();

                    //TODO: rename folder in Files and API.
                    //var path = StartDocPath + culture.Name.ToLower() + "/";
                    var path = StartDocPath + culture.TwoLetterISOLanguageName + "/";
                    if (!storeTemp.IsDirectory(path))
                        path = StartDocPath + "default/";

                    SaveStartDocument(folderDao, fileDao, id, path + (my ? "my/" : "corporate/"), storeTemp);
                }

                return id;
            }
        }

        private static void SaveStartDocument(IFolderDao folderDao, IFileDao fileDao, object folderId, string path, IDataStore storeTemp)
        {
            foreach (var file in storeTemp.ListFilesRelative("", path, "*", false))
            {
                SaveFile(fileDao, folderId, path + file, storeTemp);
            }

            foreach (var folderUri in storeTemp.List(path, false))
            {
                var folderName = Path.GetFileName(folderUri.ToString());

                var subFolderid = folderDao.SaveFolder(new Folder
                                                           {
                                                               Title = ReplaceInvalidCharsAndTruncate(folderName),
                                                               ParentFolderID = folderId
                                                           });

                SaveStartDocument(folderDao, fileDao, subFolderid, path + folderName + "/", storeTemp);
            }
        }

        private static void SaveFile(IFileDao fileDao, object folder, string filePath, IDataStore storeTemp)
        {
            using (var stream = storeTemp.IronReadStream("", filePath, 10))
            {
                var fileName = Path.GetFileName(filePath);
                var file = new File
                               {
                                   Title = ReplaceInvalidCharsAndTruncate(fileName),
                                   ContentLength = stream.Length,
                                   ContentType = MimeMapping.GetMimeMapping(fileName),
                                   FolderID = folder,
                               };
                stream.Position = 0;
                try
                {
                    fileDao.SaveFile(file, stream);
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}