using System;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;
using System.Web;

namespace ASC.Web.Community.Forum
{
    public partial class Topics : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.TopicList);

            int idThread;
            if (!int.TryParse(Request["f"], out idThread))
                Response.Redirect("default.aspx");

            var thread = ForumDataProvider.GetThreadByID(TenantProvider.CurrentTenantID, idThread);

            if (thread == null)
                Response.Redirect("default.aspx");

            if (thread.TopicCount > 0)
            {
                var topicsControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/TopicListControl.ascx") as UserControls.Forum.TopicListControl;
                topicsControl.SettingsID = ForumManager.Settings.ID;
                topicsControl.ThreadID = thread.ID;
                topicsHolder.Controls.Add(topicsControl);
            }
            else
            {
                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("forums_icon.png", ForumManager.Settings.ModuleID),
                                                 Header = Resources.ForumResource.EmptyScreenTopicCaption,
                                                 Describe = Resources.ForumResource.EmptyScreenTopicText,
                                                 ButtonHTML = ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.TopicCreate, thread) ? String.Format("<a class='linkAddMediumText' href='newpost.aspx?f=" + thread.ID + "&m=0'>{0}</a>", Resources.ForumResource.EmptyScreenTopicLink) : String.Empty
                                             };

                topicsHolder.Controls.Add(emptyScreenControl);
            }

            Utility.RegisterTypeForAjax(typeof (Subscriber));
            var subscriber = new Subscriber();

            var isThreadSubscribe = subscriber.IsThreadSubscribe(thread.ID);

            var master = Master as ForumMasterPage;
            master.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(subscriber.RenderThreadSubscription(!isThreadSubscribe, thread.ID)));

            //bread crumbs
            var breadCrumbs = (Master as ForumMasterPage).BreadCrumbs;
            breadCrumbs.Add(new BreadCrumb {Caption = Resources.ForumResource.ForumsBreadCrumbs, NavigationUrl = "default.aspx"});
            breadCrumbs.Add(new BreadCrumb {Caption = thread.Title});

            Title = HeaderStringHelper.GetPageTitle(Resources.ForumResource.AddonName, breadCrumbs);
        }
    }
}