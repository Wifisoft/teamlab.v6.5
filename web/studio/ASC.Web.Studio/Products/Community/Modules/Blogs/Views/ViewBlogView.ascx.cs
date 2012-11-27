using System;
using System.Text;
using System.Web;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;

namespace ASC.Web.Community.Blogs.Views
{
    public partial class ViewBlogView : BaseUserControl
    {
        public EventHandler UpdateCompleted;
        public EventHandler UpdateCancelled;

        public bool IsPreview { get; set; }

        public Post post
        {
            set { ShowBlogDetails(value); }
        }

        private void ShowBlogDetails(Post post)
        {
            if (post == null)
            {

            }
            else
            {
                var sb = new StringBuilder();

                sb.Append("<div style=\"padding: 0px " + (IsPreview ? "" : " 14px;") + "\">");
                if (IsPreview)
                {
                    sb.Append("<div id=\"previewTitle\" class='containerHeaderBlock' style='padding:0px 0px 10px 0px;'>" + HttpUtility.HtmlEncode(post.Title) + "</div>");
                    //    //sb.Append("<script type=\"text/javascript\">jq(document).ready (function(){window.scrollTo(jq('#blogs_preview').position().top, {speed:500});});</script>");
                }
                sb.Append("<table class='MainBlogsTable' cellspacing='0' cellpadding='0' border='0'><tr><td valign='top'>");
                sb.Append("<div style='padding-top:4px;'>" + ImageHTMLHelper.GetLinkUserAvatar(post.UserID) + "</div>");
                sb.Append("</td><td><div class='InfoMainBlog'>");

                sb.Append("<div class='clearFix CreaterMainBlog'>");

                sb.Append("<div style='float:left'>");
                sb.Append("<a class='linkHeaderSmall' href='" + VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath) + "?userid=" + post.UserID + "'><span style='font-weight:normal;'>" + ASC.Blogs.Core.Resources.BlogsResource.BlogOfTitle + ":&nbsp;&nbsp;</span>" + DisplayUserSettings.GetFullUserName(post.UserID) + "</a>");
                sb.Append("</div>");



                sb.Append("</div>");
                sb.Append("<div >");
                sb.Append("<span class='textMediumDescribe' style='margin-right:5px;'>" + ASC.Blogs.Core.Resources.BlogsResource.PostedTitle + ":</span> " + CoreContext.UserManager.GetUsers(post.UserID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID));
                sb.Append("<span class='textMediumDescribe'>&nbsp;&nbsp;" + post.Datetime.Ago() + "</span>");
                sb.Append("</div>");

                sb.Append("</div>");

                //if (post.Question != null)
                //    sb.Append(PollForm(post));

                sb.Append("<div id='previewBody' class='longWordsBreak ContentMainBlog'>");

                sb.Append(HtmlUtility.GetFull(post.Content, Product.CommunityProduct.ID));

                sb.Append("</div>");

                if (!IsPreview)
                {
                    sb.Append("<div  class='clearFix CommentsMainBlog'>");
                    if (post.TagList.Count > 0)
                    {
                        sb.Append("<div class=\"textMediumDescribe TagsMainBlog\">");
                        sb.Append("<img class=\"TagsImgMainBlog\" src=\"" + WebImageSupplier.GetAbsoluteWebPath("tags.png", BlogsSettings.ModuleID) + "\">");

                        var j = 0;
                        foreach (var tag in post.TagList)
                        {
                            if (j != 0)
                                sb.Append(", ");
                            j++;
                            sb.Append("<a style='margin-left:5px;' class=\"linkDescribe\" href=\"./?tagname=" + HttpUtility.UrlEncode(tag.Content) + "\">" + HttpUtility.HtmlEncode(tag.Content) + "</a>");
                        }

                        sb.Append("</div>");
                    }

                    if (CommunitySecurity.CheckPermissions(post, ASC.Blogs.Core.Constants.Action_EditRemovePost) && !IsPreview)
                    {
                        sb.Append("<div class='FunctionsMainBlog'>");
                        sb.Append("<a class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"editblog.aspx?blogid=" + Request.QueryString["blogID"] + "\" >" + ASC.Blogs.Core.Resources.BlogsResource.EditBlogLink + "</a>");
                        sb.Append("<span class=\"splitter\">|</span><a class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" onclick=\"javascript:return confirm('" + ASC.Blogs.Core.Resources.BlogsResource.ConfirmRemovePostMessage + "');\" href=\"editblog.aspx?blogid=" + Request.QueryString["blogID"] + "&action=delete\" >" + ASC.Blogs.Core.Resources.BlogsResource.DeleteBlogLink + "</a>&nbsp;");
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");
                }
                sb.Append("</td></tr></table>");
                sb.Append("</div>");

                ltrContent.Text = sb.ToString();
            }
        }

        public void UpdateValuesOn(Post post)
        {


        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            if (UpdateCancelled != null)
            {
                UpdateCancelled(this, EventArgs.Empty);
            }
        }
    }
}