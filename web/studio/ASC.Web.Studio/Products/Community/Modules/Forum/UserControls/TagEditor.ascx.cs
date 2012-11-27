using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using AjaxPro;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Forum
{
    [AjaxNamespace("TagEditor")]
    public partial class TagEditor : System.Web.UI.UserControl                                                  
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessTagEditor, null))
            {
                Response.Redirect(ForumManager.Instance.PreviousPage.Url);
                return;
            }
                 
            Utility.RegisterTypeForAjax(typeof(TagEditor));

            var breadCrumbs = (this.Page.Master as ForumMasterPage).BreadCrumbs;
            breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.ForumsBreadCrumbs, NavigationUrl = "default.aspx" });
            breadCrumbs.Add(new BreadCrumb() { Caption = Resources.ForumResource.TagEditorBreadCrumbs });

            this.Page.Title = HeaderStringHelper.GetPageTitle(Resources.ForumResource.AddonName, breadCrumbs);
        }

        public void RenderTags()
        {
            var topics = ForumDataProvider.GetTopicsByAllTags(TenantProvider.CurrentTenantID);
            var tags = new List<Tag>();
            foreach (var top in topics)
            {
                top.Tags.ForEach(t =>
                {
                    if (tags.Find(_t => _t.ID == t.ID) == null)
                        tags.Add(t);
                });
            }

            tags.Sort((t1, t2) => Comparer<string>.Default.Compare(t1.Name, t2.Name));

            StringBuilder sb = new StringBuilder();
           
            if (tags.Count > 0)
            {
                foreach (var tag in tags)                
                    sb.Append(TagInfo(tag, topics));                
            }
            else            
                sb.Append("<b>" + Resources.ForumResource.SearchNotFoundMessage + "</b>");
           
            Response.Write(sb.ToString());
        }
        
        private string TagInfo(Tag tag, List<Topic> topics)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("<div id=\"forum_tag_" + tag.ID + "\" class=\"tintMedium borderBase cornerAll clearFix\" style='padding:5px; margin:5px;'>");
            sb.Append("<input type=\"hidden\" id=\"forum_tag_approved_" + tag.ID + "\"  value=\"" + (tag.IsApproved ? "1" : "0") + "\"/>");
            
            sb.Append("<div style=\"float:left; width:30%;\">");
            
            sb.Append("<div id=\"forum_tag_name_info_" + tag.ID + "\" class=\"clearFix\">");                        
            sb.Append("<div id=\"forum_tni_" + tag.ID + "\" style=\"float:left; font-weight:bolder; width:66%;\">");
            sb.Append(HttpUtility.HtmlEncode(tag.Name));            
            sb.Append("</div>");

            //edit remove
            sb.Append("<div style=\"float:right; width:33%;\">");
            sb.Append("<a href=\"javascript:ForumMakerProvider.ShowEditTag('" + tag.ID + "');\"><img title='" + Resources.ForumResource.EditShortButton + "' src=\"" + WebImageSupplier.GetAbsoluteWebPath("mail_edit.png", ForumManager.ModuleID) + "\" border=\"0\"/></a>");
            sb.Append("<a style='margin-left:5px;' href=\"javascript:ForumMakerProvider.DeleteTag('" + tag.ID + "')\"><img title='" + Resources.ForumResource.EditShortButton + "' src=\"" + WebImageSupplier.GetAbsoluteWebPath("remove.png", ForumManager.ModuleID) + "\" border=\"0\"/></a>");
            sb.Append("</div>");
            sb.Append("</div>");
            
            sb.Append("<div id=\"forum_tag_name_edit_" + tag.ID + "\" class=\"clearFix\"></div>");

            sb.Append("</div>");

            //topics
            sb.Append("<div style=\"float:left; width:69%;\">");
            foreach (var topicTag in topics)
            {
                if (topicTag.Tags.Find(t => t.ID == tag.ID) == null)
                    continue;

                sb.Append("<div id=\"forum_tag_topic_" + tag.ID + "_" + topicTag.ID + "\" class=\"tintLight borderBase cornerAll clearFix\" style='padding:2px; margin:2px 0px;'>");

                sb.Append("<div style=\"float:left; width:70%;\">");
                sb.Append("<a href=\"posts.aspx?t=" + topicTag.ID + "\">" + HttpUtility.HtmlEncode(topicTag.Title) + "</a>");
                sb.Append("</div>");

                sb.Append("<div style=\"float:right; width:29%px; text-align:right;\">");
                sb.Append("<a class=\"linkAction\" href=\"javascript:ForumMakerProvider.DeleteTagFromTopic('" + tag.ID + "','" + topicTag.ID + "')\">" + Resources.ForumResource.DeleteTagFomTopicButton + "</a>");                
                sb.Append("</div>");

                sb.Append("</div>");
            }
            sb.Append("</div>");
     

            sb.Append("</div>");

            return sb.ToString();
        }



        #region DoDeleteTagFromTopic
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse DoDeleteTagFromTopic(int idTag, int idTopic)
        {
            AjaxResponse resp = new AjaxResponse();
            resp.rs3 = idTag.ToString();
            resp.rs4 = idTopic.ToString();
            
            if (!ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessTagEditor, null))
            {
                resp.rs1 = "0";
                resp.rs2 = Resources.ForumResource.ErrorAccessDenied;
                return resp;
            }

            try
            {
                ForumDataProvider.RemoveTagFromTopic(TenantProvider.CurrentTenantID, idTag, idTopic);
                resp.rs1 = "1";
            }
            catch(Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = e.Message.HtmlEncode();
            }
            
            return resp;
        }
        #endregion

        #region DoDeleteTag
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse DoDeleteTag(int id)
        {
            AjaxResponse resp = new AjaxResponse();            
            resp.rs3 = id.ToString();
            
            if (!ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessTagEditor, null))
            {
                resp.rs1 = "0";
                resp.rs2 = Resources.ForumResource.ErrorAccessDenied;
                return resp;
            }

            try
            {
                ForumDataProvider.RemoveTag(TenantProvider.CurrentTenantID, id);
                resp.rs1 = "1";
            }
            catch(Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = e.Message.HtmlEncode();
            }
            
            return resp;
        }
        #endregion
        
        #region DoApproveTag
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse DoApproveTag(int id, int isApprove)
        {
            AjaxResponse resp = new AjaxResponse();
            resp.rs2 = isApprove.ToString(); 
            resp.rs3 = id.ToString();

            var tag = ForumDataProvider.GetTagByID(TenantProvider.CurrentTenantID, id);
            if (tag == null ||
                !ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessTagEditor, null))
            {
                resp.rs1 = "0";
                resp.rs2 = Resources.ForumResource.ErrorAccessDenied;
                return resp;
            }

            bool isApproved = true;
            if (isApprove == 0)
                isApproved = false;

            tag.IsApproved = isApproved;

            try
            {
                ForumDataProvider.UpdateTag(TenantProvider.CurrentTenantID, tag.ID, tag.Name, tag.IsApproved);
                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = e.Message.HtmlEncode();
            }

            return resp;
        }
        #endregion
        
        #region DoEditTagName
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse DoEditTagName(int id, string name)
        {
            AjaxResponse resp = new AjaxResponse();
            resp.rs3 = id.ToString();
            resp.rs4 = HttpUtility.HtmlEncode(name);

            var tag = ForumDataProvider.GetTagByID(TenantProvider.CurrentTenantID, id);
            if (tag == null ||
                !ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessTagEditor, null))
            {
                resp.rs1 = "0";
                resp.rs2 = Resources.ForumResource.ErrorAccessDenied;
                return resp;
            }

            tag.Name = name;
            
            try
            {
                ForumDataProvider.UpdateTag(TenantProvider.CurrentTenantID, tag.ID, tag.Name, tag.IsApproved);
                resp.rs1 = "1";            
            }
            catch(Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = e.Message.HtmlEncode();
            }
           
            return resp;
        }
        #endregion
        
    }
}