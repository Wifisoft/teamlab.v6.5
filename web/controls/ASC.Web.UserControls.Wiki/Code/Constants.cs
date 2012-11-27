using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.UserControls.Wiki
{
    public class Constants
    {
        public const string WikiCategoryKeyCaption = "Category";	
        public const string WikiInternalCategoriesKey = "Categories";
        public const string WikiInternalFilesKey = "Files";
        public const string WikiInternalHelpKey = "Help";
        public const string WikiInternalHomeKey = "Home";
        public const string WikiInternalIndexKey = "Index";
        public const string WikiInternalKeyCaption = "Internal";
        public const string WikiInternalNewPagesKey = "NewPages";
        public const string WikiInternalRecentlyKey = "Recently";

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