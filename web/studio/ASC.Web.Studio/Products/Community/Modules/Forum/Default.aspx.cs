using System;
using System.Collections.Generic;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public partial class Default : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.Default);

            List<ThreadCategory> categories;
            List<Thread> threads;

            ForumDataProvider.GetThreadCategories(TenantProvider.CurrentTenantID, true, out categories, out threads);

            if (0 < categories.Count)
            {
                var categoryListControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/ThreadCategoryListControl.ascx") as UserControls.Forum.ThreadCategoryListControl;
                categoryListControl.Categories = categories;
                categoryListControl.Threads = threads;
                categoryListControl.SettingsID = ForumManager.Settings.ID;
                forumListHolder.Controls.Add(categoryListControl);
            }
            else
            {
                _headerHolder.Visible = false;

                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("forums_icon.png", ForumManager.Settings.ModuleID),
                                                 Header = Resources.ForumResource.EmptyScreenForumCaption,
                                                 Describe = Resources.ForumResource.EmptyScreenForumText,
                                                 ButtonHTML = ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessForumEditor, null) ? String.Format("<a class='linkAddMediumText' href='javascript:ForumMakerProvider.ShowForumMakerDialog(false,\"window.location.reload(true)\");'>{0}</a>", Resources.ForumResource.EmptyScreenForumLink) : String.Empty
                                             };
                forumListHolder.Controls.Add(emptyScreenControl);
            }

            var breadCrumbs = (Master as ForumMasterPage).BreadCrumbs;
            breadCrumbs.Add(new BreadCrumb {Caption = Resources.ForumResource.ForumsBreadCrumbs, NavigationUrl = "default.aspx"});

            Title = HeaderStringHelper.GetPageTitle(Resources.ForumResource.AddonName, breadCrumbs);
        }
    }
}