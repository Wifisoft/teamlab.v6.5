using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Forum;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.UserControls.Forum.Common;
using ASC.Web.Controls;

namespace ASC.Web.UserControls.Forum
{
    public partial class ThreadCategoryListControl : System.Web.UI.UserControl
    {
        public List<ThreadCategory> Categories { get; set; }
        public List<Thread> Threads { get; set; }

        public Guid SettingsID { get; set; }
        protected Settings _settings { get; set; }

        public ThreadCategoryListControl()
        {
            Categories = new List<ThreadCategory>();
            Threads = new List<Thread>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _settings = ForumManager.GetSettings(SettingsID);

            _categoryRepeater.DataSource = Categories;
            _categoryRepeater.ItemDataBound += new RepeaterItemEventHandler(CategoryRepeater_ItemDataBound);
            _categoryRepeater.DataBind();
        }

        private void CategoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var threadRepeater = (Repeater)e.Item.FindControl("_threadRepeater");

                var category = (e.Item.DataItem as ThreadCategory);
                threadRepeater.DataSource = Threads.FindAll(t => t.CategoryID == category.ID);
                threadRepeater.DataBind();
            }
        }

        protected string RenderThreadCSSClass(int index, Thread thread)
        {
            var cssClass = (index % 2 ==0) ? "tintMedium" : "";

            if (!thread.IsApproved
               && _settings.ForumManager.ValidateAccessSecurityAction(ForumAction.ApprovePost, thread))
            {
                cssClass = "tintDangerous";
            }

            return cssClass;
        }

        protected string RenderRecentUpdate(Thread thread)
        {
            if (thread.RecentPostID == 0)
                return "";

            var sb = new StringBuilder();
            sb.Append("<div><a title=\"" + HttpUtility.HtmlEncode(thread.RecentTopicTitle) + "\" href=\"" + _settings.LinkProvider.PostList(thread.RecentTopicID) + "\">" + HttpUtility.HtmlEncode(HtmlUtility.GetText(thread.RecentTopicTitle, 20)) + "</a></div>");

            sb.Append("<div style='margin-top:5px;overflow: hidden;width: 180px;'>" + ASC.Core.Users.StudioUserInfoExtension.RenderProfileLink(CoreContext.UserManager.GetUsers(thread.RecentPosterID), _settings.ProductID) + "</div>");
            sb.Append("<div style='margin-top:5px;'>");
            sb.Append("<span class='textSmallDescribe'>" + DateTimeExtension.AgoSentence(thread.RecentPostCreateDate) + "</span>");
            sb.Append("<a href=\"" + _settings.LinkProvider.RecentPost(thread.RecentPostID, thread.RecentTopicID, thread.RecentTopicPostCount)  + "\"><img hspace=\"3\" align=\"absmiddle\" alt=\"&raquo;\" title=\"&raquo;\" border=\"0\" src=\"" + WebImageSupplier.GetAbsoluteWebPath("goto.png", _settings.ImageItemID) + "\"/></a>");
            sb.Append("</div>");

            return sb.ToString();
        }
    }
}