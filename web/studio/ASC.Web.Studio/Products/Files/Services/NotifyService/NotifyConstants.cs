using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Web.Files.Services.NotifyService
{
    public static class NotifyConstants
    {
        #region Events

        public static readonly INotifyAction Event_UpdateDocument = new NotifyAction("UpdateDocument", "update document");
        public static readonly INotifyAction Event_ShareDocument = new NotifyAction("ShareDocument", "share document");

        #endregion

        #region  Tags

        public static readonly ITag Tag_FolderID = new Tag("FolderID");
        public static readonly ITag Tag_DocumentTitle = new Tag("DocumentTitle");
        public static readonly ITag Tag_DocumentUrl = new Tag("DocumentURL");
        public static readonly ITag Tag_VersionNumber = new Tag("VersionNumber");
        public static readonly ITag Tag_FolderTitle = new Tag("FolderTitle");
        public static readonly ITag Tag_AccessRights = new Tag("AccessRights");
        public static readonly ITag Tag_Message = new Tag("Message");

        #endregion
    }
}