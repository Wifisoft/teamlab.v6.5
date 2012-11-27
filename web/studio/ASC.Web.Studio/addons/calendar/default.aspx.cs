using System;
using System.Web;
using ASC.Web.Core;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.Calendar.UserControls;
using ASC.Web.Studio.Masters;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Personal;

namespace ASC.Web.Calendar
{
    [WidePage(true)]
    public partial class _default : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = HeaderStringHelper.GetPageTitle(Resources.CalendarAddonResource.AddonName, null, null);

            (Master as StudioTemplate).TopNavigationPanel.DisableSearch = true;
            (Master as StudioTemplate).TopNavigationPanel.CustomTitle = Resources.CalendarAddonResource.AddonName;

            (Master as StudioTemplate).TopNavigationPanel.CustomTitleURL = VirtualPathUtility.ToAbsolute(CalendarAddon.BaseVirtualPath);
            (Master as StudioTemplate).TopNavigationPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("logo.png", CalendarAddon.AddonID);

            (Master as IStudioMaster).ContentHolder.Controls.Add(LoadControl(CalendarControl.Location));
            (Master as IStudioMaster).DisabledSidePanel = true;

            //for personal
            if (SetupInfo.IsPersonal)
            {
                PersonalHelper.AdjustTopNavigator((Master as StudioTemplate).TopNavigationPanel, PersonalPart.WebItem, CalendarAddon.AddonID);
            }
        }
    }
}