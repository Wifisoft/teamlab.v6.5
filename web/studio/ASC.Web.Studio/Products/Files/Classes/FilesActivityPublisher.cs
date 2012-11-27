using System;
using System.Linq;
using ASC.Core;
using ASC.Files.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Classes
{
    public class FilesActivityPublisher : BaseUserActivityPublisher, IUserActivityFilter
    {
        private static readonly Guid ProjectModuleId = new Guid("1e044602-43b5-4d79-82f3-fd6208a11960");
        private static readonly Guid CrmModuleId = new Guid("6743007C-6F95-4d20-8C88-A8601CE5E76D");

        public bool FilterActivity(UserActivity activity)
        {
            if (activity == null) return true;

            if (string.IsNullOrEmpty(activity.SecurityId)) return true;

            try
            {
                string entryId;
                if (activity.ContentID.StartsWith("file_"))
                {
                    entryId = activity.ContentID.Substring("file_".Length);
                    var file = Global.DaoFactory.GetFileDao().GetFile(entryId);
                    return Global.GetFilesSecurity().CanRead(file);
                }

                if (activity.ContentID.StartsWith("folder_"))
                {
                    entryId = activity.ContentID.Substring("folder_".Length);
                    var folder = Global.DaoFactory.GetFolderDao().GetFolder(entryId);
                    return Global.GetFilesSecurity().CanRead(folder);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static void PublishInternal(UserActivity activity)
        {
            if (activity == null) return;

            UserActivityPublisher.Publish<FilesActivityPublisher>(activity);
        }

        private static UserActivity ApplyCustomeActivityParams(FileEntry entry, string imgFileName, string actionText, int actionType, int businessValue, string containerId)
        {
            if (entry == null) return null;

            string url;
            var moduleId = ProductEntryPoint.ID;
            var additionalData = "";
            var securityId = "0";

            if (entry.RootFolderType == FolderType.BUNCH)
            {
                var title = Global.DaoFactory.GetFolderDao().GetFolder(entry.RootFolderId).Title;

                if (title.StartsWith("projects/project/"))
                {
                    moduleId = ProjectModuleId;
                    additionalData = "File||";
                    containerId = title.Replace("projects/project/", "");
                    securityId = "File||" + containerId;
                }
                else if (title.StartsWith("crm/crm_common/"))
                {
                    moduleId = CrmModuleId;
                    securityId = "6|" + entry.UniqID;
                }
            }

            if (entry is File)
            {
                url = FileUtility.ExtsWebPreviewed.Contains(FileUtility.GetFileExtension(entry.Title), StringComparer.CurrentCultureIgnoreCase)
                          ? CommonLinkUtility.GetFileWebViewerUrl(entry.ID)
                          : ((File) entry).ViewUrl;
            }
            else
            {
                url = PathProvider.GetFolderUrl((Folder) entry);
            }

            var ua = new UserActivity
                         {
                             Title = entry.Title,
                             ContentID = entry.UniqID,
                             URL = url,
                             ModuleID = moduleId,
                             ProductID = moduleId,
                             TenantID = TenantProvider.CurrentTenantID,
                             Date = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                             ImageOptions = new ImageOptions {PartID = ProductEntryPoint.ID, ImageFileName = imgFileName},
                             ActionText = actionText,
                             UserID = SecurityContext.CurrentAccount.ID,
                             ActionType = actionType,
                             BusinessValue = businessValue,
                             AdditionalData = additionalData,
                             ContainerID = containerId,
                             SecurityId = securityId
                         };
            return ua;
        }

        public static void CreateFile(File file)
        {
            PublishInternal(ApplyCustomeActivityParams(file,
                                                       "new_file.png",
                                                       FilesCommonResource.ActivityCreateDocument,
                                                       UserActivityConstants.ContentActionType,
                                                       UserActivityConstants.NormalContent,
                                                       "CreateFile"));
        }

        public static void UploadFile(File file)
        {
            PublishInternal(ApplyCustomeActivityParams(file,
                                                       "upload.png",
                                                       FilesCommonResource.ActivityUploadDocument,
                                                       UserActivityConstants.ContentActionType,
                                                       UserActivityConstants.NormalContent,
                                                       "UploadFile"));
        }

        public static void OpenEditorFile(File file)
        {
            PublishInternal(ApplyCustomeActivityParams(file,
                                                       "openeditor.png",
                                                       FilesCommonResource.ActivityOpenEditorDocument,
                                                       UserActivityConstants.ActivityActionType,
                                                       UserActivityConstants.SmallActivity,
                                                       "OpenEditorFile"));
        }

        public static void UpdateFile(File file)
        {
            PublishInternal(ApplyCustomeActivityParams(file,
                                                       "updatefile.png",
                                                       FilesCommonResource.ActivityUpdateDocument,
                                                       UserActivityConstants.ActivityActionType,
                                                       UserActivityConstants.NormalActivity,
                                                       "UpdateFile"));
        }

        public static void DeleteFile(File file)
        {
            var ua = ApplyCustomeActivityParams(file,
                                                "trash_big.png",
                                                FilesCommonResource.ActivityDeleteDocument,
                                                UserActivityConstants.ActivityActionType,
                                                UserActivityConstants.NormalActivity,
                                                "DeleteFile");

            ua.URL = file.RootFolderType == FolderType.TRASH
                         ? string.Empty
                         : PathProvider.GetFolderUrl(file.FolderID, file.RootFolderType == FolderType.BUNCH, file.RootFolderId);

            PublishInternal(ua);
        }

        public static void DeleteFolder(Folder folder)
        {
            var ua = ApplyCustomeActivityParams(folder,
                                                "trash_big.png",
                                                FilesCommonResource.ActivityDeleteFolder,
                                                UserActivityConstants.ActivityActionType,
                                                UserActivityConstants.NormalActivity,
                                                "DeleteFolder");

            ua.URL = folder.RootFolderType == FolderType.TRASH
                         ? string.Empty
                         : PathProvider.GetFolderUrl(folder.ParentFolderID, folder.RootFolderType == FolderType.BUNCH, folder.RootFolderId);

            PublishInternal(ua);
        }
    }
}