using System;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Studio;
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Web;   

namespace ASC.Web.Community.Forum
{
    public partial class NewPost : MainPage
    {
        private NewPostControl _newPostControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SetupInfo.WorkMode == WorkMode.Promo)            
                Response.Redirect(ForumManager.BaseVirtualPath + "/default.aspx");
                
            ForumManager.Instance.SetCurrentPage(ForumPage.NewPost);
            
            _newPostControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/NewPostControl.ascx") as NewPostControl;
            _newPostControl.SettingsID = ForumManager.Settings.ID;
            _newPostHolder.Controls.Add(_newPostControl);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            var breadCrumbs = (this.Master as ForumMasterPage).BreadCrumbs;
            breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.ForumsBreadCrumbs, NavigationUrl = "default.aspx" });

            if (_newPostControl.PostType == NewPostType.Post)
            {
                Topic topic = _newPostControl.Topic;
                breadCrumbs.Add(new BreadCrumb() { Caption = topic.ThreadTitle, NavigationUrl = "topics.aspx?f=" + topic.ThreadID.ToString() });
                breadCrumbs.Add(new BreadCrumb() { Caption = topic.Title, NavigationUrl = "posts.aspx?t=" + topic.ID.ToString() });
                breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.NewPostButton });
            }
            else if (_newPostControl.PostType == NewPostType.Topic)
            {
                if(!String.IsNullOrEmpty(Request["f"]))
                    breadCrumbs.Add(new BreadCrumb() { Caption = _newPostControl.Thread.Title, NavigationUrl = "topics.aspx?f=" + _newPostControl.Thread.ID.ToString() });

                breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.NewTopicButton });
            }
            else if (_newPostControl.PostType == NewPostType.Poll)
            {
                if (!String.IsNullOrEmpty(Request["f"]))
                    breadCrumbs.Add(new BreadCrumb() { Caption = _newPostControl.Thread.Title, NavigationUrl = "topics.aspx?f=" + _newPostControl.Thread.ID.ToString() });

                breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.NewPollButton });
            }

            this.Title = HeaderStringHelper.GetPageTitle(Resources.ForumResource.AddonName, breadCrumbs);
        }

    }   
}
