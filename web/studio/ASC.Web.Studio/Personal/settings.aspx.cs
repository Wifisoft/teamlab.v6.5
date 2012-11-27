using System;
using System.Collections.Generic;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Controls;

namespace ASC.Web.Studio.Personal
{
    public partial class settings : MainPage
    {       

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Administration, null, null);
            (this.Master as StudioTemplate).DisabledSidePanel = true;

            var topNavigator = (this.Master as StudioTemplate).TopNavigationPanel;
            PersonalHelper.AdjustTopNavigator(topNavigator, PersonalPart.Settings);

            _settingsContainer.BreadCrumbs = new List<BreadCrumb>();
            _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.Administration});

            //language
            _timelngHolder.Controls.Add(LoadControl(TimeAndLanguage.Location));           

            //themes
            _themesHolder.Controls.Add(LoadControl(SkinSettings.Location));

        }
    }
}
