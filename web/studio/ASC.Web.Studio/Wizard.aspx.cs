using System;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.FirstTime;

namespace ASC.Web.Studio
{
    public partial class Wizard : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = Resources.Resource.WizardPageTitle;

            content.Controls.Add(LoadControl(Cloud.Location));
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableProductNavigation = true;
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableUserInfo = true;
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableSearch = true;
        }
    }
}
