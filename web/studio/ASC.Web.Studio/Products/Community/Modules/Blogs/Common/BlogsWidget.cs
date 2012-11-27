using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Security;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard;

namespace ASC.Web.Community.Blogs
{
    [Serializable]
    [DataContract]
    public class BlogsWidgetSettings : ISettings
    {
        [DataMember(Name = "MaxCountPosts")]
        public int MaxCountPosts { get; set; }


        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{40FE6821-7629-4131-8559-8A093068CEA2}"); }
        }

        public ASC.Web.Core.Utility.Settings.ISettings GetDefault()
        {
            return new BlogsWidgetSettings() { MaxCountPosts = 3 };
        }

        #endregion
    }

    [AjaxNamespace("BlogsWidget")]
    [WidgetPosition(1, 0)]
    public class BlogsWidget : WebControl, ICheckEmptyContent
    {
        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Utility.RegisterTypeForAjax(typeof(BlogsWidget));
        }
        #endregion

        #region Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string UpdateContent()
        {
            return RenderLastUpdateContent();
        }


        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
            writer.Write("<div id=\"Blogs_DataContent\">");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            writer.Write(RenderLastUpdateContent());
        }

        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("</div>");
            base.RenderEndTag(writer);
        }

        private string RenderLastUpdateContent()
        {
            BlogsWidgetSettings widgetSettings = SettingsManager.Instance.LoadSettingsFor<BlogsWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var engine = BasePage.GetEngine();

            var posts = engine.SelectPosts(
                            new PostsQuery()
                                .SetCount(widgetSettings.MaxCountPosts)
                                .NoTags()
                                );

            var posts_with_stat = engine.GetPostsCommentsCountAndNew(posts, SecurityContext.CurrentAccount.ID);

            StringBuilder sb = new StringBuilder();

            //posts
            foreach (var entry in posts_with_stat)
            {
                Post post = entry.Value1;
                int commentsCount = entry.Value2;
                bool isNewComments = entry.Value3 > 0;

                sb.Append("<table cellpadding='0' cellspacing='0' border='0'>");
                sb.Append("<tr valign='top'>");

                sb.Append("<td width='30'>");
                sb.Append("<span class='textMediumDescribe'>" + post.Datetime.ToShortDayMonth() + " " + post.Datetime.ToShortTimeString() + "</span>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<div style='padding-left:10px;'>");

                sb.Append("<div style='margin-bottom:5px;'>");
                sb.Append("<a href='" + VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/viewblog.aspx") + "?blogid=" + post.ID + "'>" + post.Title.HtmlEncode() + "</a>");

                if (commentsCount > 0)
                {
                    if (isNewComments)
                        sb.AppendFormat("<span style='margin-left:7px;' class='errorText'>({0})</span>", commentsCount);
                    else
                        sb.AppendFormat("<span style='margin-left:7px;' class='describeText'>({0})</span>", commentsCount);
                }
                sb.Append("</div>");

                sb.Append("<div style='margin-bottom:5px;'>");
                sb.Append(HtmlUtility.GetText(post.Content, 120));
                sb.Append("</div>");

                sb.Append("<div style='margin-bottom:17px;'>");
                sb.Append("<span class='textBigDescribe'>" + ASC.Blogs.Core.Resources.BlogsResource.PostedTitle + ":</span>&nbsp;&nbsp;" + CoreContext.UserManager.GetUsers(post.UserID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID));
                sb.Append("</div>");

                sb.Append("</div>");
                sb.Append("</td>");

                sb.Append("</tr>");


                sb.Append("</table>");
            }

            if (posts_with_stat.Count > 0)
            {

                sb.AppendFormat("<div style='margin-top: 10px;'><a href='{0}'>{1}</a></div>",
                        ASC.Blogs.Core.Constants.DefaultPageUrl,
                        ASC.Blogs.Core.Resources.BlogsResource.AllBlogs
                    );
            }
            else
            {
                if (CommunitySecurity.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(
                                                                         SecurityContext.CurrentAccount.ID)), ASC.Blogs.Core.Constants.Action_AddPost))
                {
                    sb.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">"
                        + String.Format(ASC.Blogs.Core.Resources.BlogsResource.NoBlogsWidgetTitle,
                                        string.Format("<div style=\"padding-top:3px;\"><a class=\"promoAction\" href=\"{0}\">", VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath + "addblog.aspx")),
                                        "</a></div>") + "</div>");
                }
                else
                    sb.Append("<div class==\"empty-widget\" style=\"padding:40px; text-align: center;\">" + ASC.Blogs.Core.Resources.BlogsResource.NoBlogsWidgetMessage + "</div>");
            }


            return sb.ToString();
        }

        #endregion

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            BlogsWidgetSettings widgetSettings = SettingsManager.Instance.LoadSettingsFor<BlogsWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var engine = BasePage.GetEngine();

            var posts = engine.SelectPosts(
                            new PostsQuery()
                                .SetCount(widgetSettings.MaxCountPosts)
                                .NoTags()
                                );

            var posts_with_stat = engine.GetPostsCommentsCountAndNew(posts, SecurityContext.CurrentAccount.ID);

            

            if (posts_with_stat.Count > 0)
                return CheckEmptyContentResult.NotEmpty;

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}
