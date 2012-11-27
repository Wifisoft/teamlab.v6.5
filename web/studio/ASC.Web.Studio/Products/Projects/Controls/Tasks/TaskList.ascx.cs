#region Usings

using System;
using ASC.Web.Projects.Classes;

#endregion

namespace ASC.Web.Projects.Controls.Tasks
{
    public partial class TaskList : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopup.Options.IsPopup = true;
            _hintPopupTaskRemove.Options.IsPopup = true;
            timeTrackingContainer.Options.IsPopup = true;
            moveTaskContainer.Options.IsPopup = true;

            var subtasks = (Subtasks)LoadControl(PathProvider.GetControlVirtualPath("Subtasks.ascx"));
            _subtasksTemplates.Controls.Add(subtasks);
        }
    }
}
