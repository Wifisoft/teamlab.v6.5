#region Usings

using System;

#endregion

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ProjectsList : BaseUserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopupTasks.Options.IsPopup = true;
            _hintPopupMilestones.Options.IsPopup = true;

            var apiServer = new Api.ApiServer();

            var tags = apiServer.GetApiResponse(string.Format("api/1.0/project/tag.json"), "GET");
            Page.JsonPublisher(tags, "tags");
        }
    }
}