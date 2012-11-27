using System;
using System.Collections;
using System.Text;
using System.Web;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Resources;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using System.Collections.Generic;
using ASC.Blogs.Core.Data;

namespace ASC.Web.Community.Blogs.Controls
{
    public partial class TopList : BaseUserControl
    {
        public List<Typle<Comment, Post>> LastCommentedPosts
        {
            set
            {

                sideContainer.Title = BlogsResource.LiveBroadcastTitle;
                sideContainer.HeaderCSSClass = "studioSideBoxWhatToRead";
                sideContainer.ImageURL = WebImageSupplier.GetAbsoluteWebPath("what_to_read.png", ASC.Blogs.Core.Constants.ModuleID);



                StringBuilder sb = new StringBuilder();
                if (value != null && value.Count > 0)
                {
                    foreach (var typle in value)
                    {
                        Post post = typle.Value2;

                        Comment comment = typle.Value1;

                        sb.Append("<div style=\"margin-top:10px;\">" + CoreContext.UserManager.GetUsers(comment.UserID).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID) + "<span class=\"\"> / </span>");
                        sb.AppendFormat("<a class=\"\" href=\"viewblog.aspx?blogid={0}#{1}\">{2}</a>",
                                post.ID, comment.ID, HttpUtility.HtmlEncode(post.Title));
                        sb.Append("</div>");

                    }
                }
                else
                    sideContainer.Visible = false;

                ltrTopList.Text = sb.ToString();
            }
        }
    }
}