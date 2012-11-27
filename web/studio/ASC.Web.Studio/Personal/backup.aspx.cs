using System;
using System.Collections.Generic;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Masters;
using ASC.Web.Controls;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Studio.Personal
{
    public partial class backup : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Backup, null, null);
            (this.Master as StudioTemplate).DisabledSidePanel = true;

            var topNavigator = (this.Master as StudioTemplate).TopNavigationPanel;
            PersonalHelper.AdjustTopNavigator(topNavigator, PersonalPart.Backup);

            _backupContainer.BreadCrumbs = new List<BreadCrumb>();
            _backupContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.Backup });
            _backupContainer.Body.Controls.Add(LoadControl(Backup.Location));
            
        }
    }
}
