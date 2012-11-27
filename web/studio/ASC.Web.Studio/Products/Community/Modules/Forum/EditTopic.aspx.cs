using System;
using System.Web;
using ASC.Web.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{

    public partial class EditTopic : MainPage
    {  
        private TopicEditorControl _topicEditorControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(ForumManager.BaseVirtualPath + "/default.aspx");

            ForumManager.Instance.SetCurrentPage(ForumPage.EditTopic);

            _topicEditorControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/TopicEditorControl.ascx") as TopicEditorControl;
            _topicEditorControl.SettingsID = ForumManager.Settings.ID;
            topicEditorHolder.Controls.Add(_topicEditorControl);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            var breadCrumbs = (this.Master as ForumMasterPage).BreadCrumbs;
            breadCrumbs.Add(new BreadCrumb { Caption = Resources.ForumResource.ForumsBreadCrumbs, NavigationUrl = "default.aspx" });
            breadCrumbs.Add(new BreadCrumb { Caption = _topicEditorControl.EditableTopic.ThreadTitle, NavigationUrl = "topics.aspx?f=" + _topicEditorControl.EditableTopic.ThreadID.ToString() });
            breadCrumbs.Add(new BreadCrumb { Caption = _topicEditorControl.EditableTopic.Title, NavigationUrl = "posts.aspx?t=" + _topicEditorControl.EditableTopic.ID.ToString() });
            breadCrumbs.Add(new BreadCrumb { Caption = Resources.ForumResource.EditTopicBreadCrumbs });

            Title = HeaderStringHelper.GetPageTitle(Resources.ForumResource.AddonName, breadCrumbs);
        }
    }
}
