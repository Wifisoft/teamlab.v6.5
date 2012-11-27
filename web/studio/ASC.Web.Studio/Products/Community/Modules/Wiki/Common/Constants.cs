using System;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.UserControls.Wiki.Resources;
using AuthAction = ASC.Common.Security.Authorizing.Action;

namespace ASC.Web.Community.Wiki.Common
{
    public class Constants
    {
        public static readonly AuthAction Action_AddPage = new AuthAction(new Guid("D49F4E30-DA10-4b39-BC6D-B41EF6E039D3"), "New Page");
        public static readonly AuthAction Action_EditPage = new AuthAction(new Guid("D852B66F-6719-45e1-8657-18F0BB791690"), "Edit page");
        public static readonly AuthAction Action_RemovePage = new AuthAction(new Guid("557D6503-633B-4490-A14C-6473147CE2B3"), "Delete page");
        public static readonly AuthAction Action_UploadFile = new AuthAction(new Guid("088D5940-A80F-4403-9741-D610718CE95C"), "Upload file");
        public static readonly AuthAction Action_RemoveFile = new AuthAction(new Guid("7CB5C0D1-D254-433f-ABE3-FF23373EC631"), "Delete file");
        public static readonly AuthAction Action_AddComment = new AuthAction(new Guid("C426C349-9AD4-47cd-9B8F-99FC30675951"), "Add Comment");
        public static readonly AuthAction Action_EditRemoveComment = new AuthAction(new Guid("B630D29B-1844-4bda-BBBE-CF5542DF3559"), "Edit/Delete comment");

        public static INotifyAction NewPage = new NotifyAction("new wiki page", WikiResource.NotifyAction_NewPage);
        public static INotifyAction EditPage = new NotifyAction("edit wiki page", WikiResource.NotifyAction_ChangePage);
        public static INotifyAction AddPageToCat = new NotifyAction("add page to cat", WikiResource.NotifyAction_AddPageToCat);

        public static ITag TagPageName = new Tag("PageName");
        public static ITag TagURL = new Tag("URL");

        public static ITag TagUserName = new Tag("UserName");
        public static ITag TagUserURL = new Tag("UserURL");
        public static ITag TagDate = new Tag("Date");

        public static ITag TagPostPreview = new Tag("PagePreview");
        public static ITag TagCommentBody = new Tag("CommentBody");

        public static ITag TagChangePageType = new Tag("ChangeType");
        public static ITag TagCatName = new Tag("CategoryName");

    }
}