using System;
using System.Collections.Generic;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Masters;
using ASC.Web.Controls;
using ASC.Web.Studio.UserControls.Users;
using ASC.Core;

namespace ASC.Web.Studio.Personal
{
    public partial class profile : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Profile, null, null);
            (this.Master as StudioTemplate).DisabledSidePanel = true;

            var topNavigator = (this.Master as StudioTemplate).TopNavigationPanel;
            PersonalHelper.AdjustTopNavigator(topNavigator, PersonalPart.Profile);

            _myStaffContainer.BreadCrumbs = new List<BreadCrumb>();
            _myStaffContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.Profile});

            var userProfileControl = LoadControl(UserProfileControl.Location) as UserProfileControl;
            userProfileControl.UserInfo = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            userProfileControl.MyStaffMode = true;

            _myStaffContainer.Body.Controls.Add(userProfileControl);

        }
    }
}
