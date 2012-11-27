using System;
using AjaxPro;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;
using System.Web;

namespace ASC.Web.Community.Forum
{   
    public partial class Posts : MainPage
    {   
        protected void Page_Load(object sender, EventArgs e)
        {            
            ForumManager.Instance.SetCurrentPage(ForumPage.PostList);               
           
            int idTopic = 0;            
            if (!String.IsNullOrEmpty(Request["t"]))
            {
                try
                {
                    idTopic = Convert.ToInt32(Request["t"]);
                }
                catch { idTopic = 0; }
            }
            if (idTopic == 0)
                Response.Redirect("default.aspx");
            

            var topic = ForumDataProvider.GetTopicByID(TenantProvider.CurrentTenantID, idTopic);
            if (topic == null)
                Response.Redirect("default.aspx");

            var postListControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/PostListControl.ascx") as PostListControl;
            postListControl.SettingsID = ForumManager.Settings.ID;
            postListControl.Topic = topic;
            this.postListHolder.Controls.Add(postListControl);
            
            Utility.RegisterTypeForAjax(typeof(Subscriber));
            var subscriber = new Subscriber();

            var isThreadSubscribe = subscriber.IsThreadSubscribe(topic.ThreadID);

            var isTopicSubscribe = subscriber.IsTopicSubscribe(topic.ID);

            var master = this.Master as ForumMasterPage;
            master.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(subscriber.RenderThreadSubscription(!isThreadSubscribe, topic.ThreadID)));
            master.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(subscriber.RenderTopicSubscription(!isTopicSubscribe, topic.ID)));
            
            var breadCrumbs = (this.Master as ForumMasterPage).BreadCrumbs;
            breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.ForumsBreadCrumbs, NavigationUrl = "default.aspx" });
            breadCrumbs.Add(new BreadCrumb() { Caption = topic.ThreadTitle, NavigationUrl = "topics.aspx?f=" + topic.ThreadID.ToString() });
            breadCrumbs.Add(new BreadCrumb() { Caption = topic.Title });

            this.Title = HeaderStringHelper.GetPageTitle(Resources.ForumResource.AddonName, breadCrumbs);
        }        
    }
}
