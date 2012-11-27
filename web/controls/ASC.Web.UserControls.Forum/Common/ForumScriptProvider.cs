using System;
using System.Web.UI;
using System.Text;

namespace ASC.Web.UserControls.Forum.Common
{
    public class ForumScriptProvider : Control
    {
        public bool RegistrySearchHelper { get; set; }

        public Guid SettingsID { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(ForumManager.ForumScriptKey, Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.UserControls.Forum.js.forum.js"));

            if (RegistrySearchHelper)
                Page.ClientScript.RegisterClientScriptInclude(ForumManager.SearchHelperScriptKey, Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.UserControls.Forum.js.searchhelper.js"));


            StringBuilder script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" language=\"javascript\">");
            script.Append("ForumManager.QuestionEmptyMessage = '" + Resources.ForumUCResource.QuestionEmptyMessage + "';");
            script.Append("ForumManager.SubjectEmptyMessage = '" + Resources.ForumUCResource.SubjectEmptyMessage + "';");
            script.Append("ForumManager.ApproveTopicButton = '" + Resources.ForumUCResource.ApproveButton + "';");
            script.Append("ForumManager.OpenTopicButton = '" + Resources.ForumUCResource.OpenTopicButton + "';");
            script.Append("ForumManager.CloseTopicButton = '" + Resources.ForumUCResource.CloseTopicButton + "';");
            script.Append("ForumManager.StickyTopicButton = '" + Resources.ForumUCResource.StickyTopicButton + "';");
            script.Append("ForumManager.ClearStickyTopicButton = '" + Resources.ForumUCResource.ClearStickyTopicButton + "';");
            script.Append("ForumManager.DeleteTopicButton = '" + Resources.ForumUCResource.DeleteButton + "';");
            script.Append("ForumManager.EditTopicButton = '" + Resources.ForumUCResource.EditButton + "';");
            script.Append("ForumManager.ConfirmMessage = '" + Resources.ForumUCResource.ConfirmMessage + "';");
            script.Append("ForumManager.NameEmptyString = '" + Resources.ForumUCResource.NameEmptyString + "';");
            script.Append("ForumManager.SaveButton = '" + Resources.ForumUCResource.SaveButton + "';");
            script.Append("ForumManager.CancelButton = '" + Resources.ForumUCResource.CancelButton + "';");
            script.Append("ForumManager.SettingsID = '" + SettingsID + "';");
            script.Append("</script>");

            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "__forum_core_lng_script", script.ToString());

        }

    }
}
