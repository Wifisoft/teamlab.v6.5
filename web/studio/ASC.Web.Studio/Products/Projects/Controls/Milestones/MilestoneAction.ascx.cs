#region Usings

using System;
using System.Web.UI;

#endregion

namespace ASC.Web.Projects.Controls.Milestones
{
    public partial class MilestoneAction : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            milestoneActionContainer.Options.IsPopup = true;
        }
    }
}
