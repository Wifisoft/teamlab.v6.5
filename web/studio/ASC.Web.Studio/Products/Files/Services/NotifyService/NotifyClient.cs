using System;
using System.Linq;
using ASC.Core;
using ASC.Files.Core;
using ASC.Notify;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;
using ASC.Files.Core.Security;

namespace ASC.Web.Files.Services.NotifyService
{
    public static class NotifyClient
    {
        public static INotifyClient Instance { get; private set; }

        static NotifyClient()
        {
            Instance = WorkContext.NotifyContext.NotifyService.RegisterClient(NotifySource.Instance);
        }

        public static void SendShareNotice(File document, FileShare fileShare, IRecipient[] recipients, string message)
        {
            if (document == null || recipients.Length == 0) return;

            using (var dao = Global.DaoFactory.GetFolderDao())
            {
                var folder = dao.GetFolder(document.FolderID);
                if (folder == null) return;

                string aceString;
                switch (fileShare)
                {
                    case FileShare.Read:
                        aceString = Resources.FilesCommonResource.AceStatusEnum_Read;
                        break;
                    case FileShare.ReadWrite:
                        aceString = Resources.FilesCommonResource.AceStatusEnum_ReadWrite;
                        break;
                    default:
                        return;
                }

                var url = document.ViewUrl;
                if (FileUtility.ExtsWebPreviewed.Contains(FileUtility.GetFileExtension(document.Title), StringComparer.CurrentCultureIgnoreCase))
                    url = CommonLinkUtility.GetFileWebViewerUrl(document.ID);

                foreach (var rec in recipients)
                {
                    Instance.SendNoticeAsync(
                        NotifyConstants.Event_ShareDocument,
                        document.UniqID,
                        rec,
                        null,
                        new TagValue(NotifyConstants.Tag_DocumentTitle, document.Title),
                        new TagValue(NotifyConstants.Tag_DocumentUrl, CommonLinkUtility.GetFullAbsolutePath(url)),
                        new TagValue(NotifyConstants.Tag_AccessRights, aceString),
                        new TagValue(NotifyConstants.Tag_Message, message.HtmlEncode())
                        );
                }
            }
        }

        public static void SendUpdateNotice(File document)
        {
            if (document == null ||
                SecurityContext.CurrentAccount.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID)) return;

            var url = document.ViewUrl;
            if (FileUtility.ExtsWebPreviewed.Contains(FileUtility.GetFileExtension(document.Title), StringComparer.CurrentCultureIgnoreCase))
                url = CommonLinkUtility.GetFileWebViewerUrl(document.ID);

            var interceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            try
            {
                Instance.AddInterceptor(interceptor);
                Instance.SendNoticeAsync(
                    NotifyConstants.Event_UpdateDocument,
                    document.UniqID,
                    null,
                    new TagValue(NotifyConstants.Tag_DocumentTitle, document.Title),
                    new TagValue(NotifyConstants.Tag_DocumentUrl, CommonLinkUtility.GetFullAbsolutePath(url)),
                    new TagValue(NotifyConstants.Tag_VersionNumber, document.Version));
            }
            finally
            {
                Instance.RemoveInterceptor(interceptor.Name);
            }

            if (document.RootFolderType == FolderType.USER &&
                document.CreateBy != SecurityContext.CurrentAccount.ID)
            {
                Instance.SendNoticeAsync(
                    NotifyConstants.Event_UpdateDocument,
                    document.UniqID,
                    NotifySource.Instance.GetRecipientsProvider().GetRecipient(document.CreateBy.ToString()),
                    null,
                    new TagValue(NotifyConstants.Tag_DocumentTitle, document.Title),
                    new TagValue(NotifyConstants.Tag_DocumentUrl, CommonLinkUtility.GetFullAbsolutePath(url)),
                    new TagValue(NotifyConstants.Tag_VersionNumber, document.Version));
            }
        }
    }
}