using System;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Studio
{   
    public partial class _Default : MainPage
    {
        protected bool _showGettingStarted;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            var isAdmin = CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID);
            var settings = SettingsManager.Instance.LoadSettingsFor<DisplayUserSettings>(SecurityContext.CurrentAccount.ID);
            _showGettingStarted = isAdmin && !settings.IsDisableGettingStarted;
            _gettingStartedVideoContainer.Options.IsPopup = true;
            
            ((StudioTemplate)this.Master).DisabledSidePanel = true;

            //top panel
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableProductNavigation = true;
            ((StudioTemplate)this.Master).TopNavigationPanel.DisableSearch = true;

            this.Title = Resources.Resource.MainPageTitle;
            var panel = LoadControl(NavigationPanel.Location) as NavigationPanel;

            navPanelHolder.Controls.Add(panel);

            _productRepeater.DataSource = WebItemManager.Instance.GetItems(ASC.Web.Core.WebZones.WebZoneType.StartProductList);
            _productRepeater.DataBind();

            _welcomeBoxContainer.Options.IsPopup = true;
            var showWelcomePopup = ((Request["first"] ?? "") == "1");
            if(showWelcomePopup && Session["first"]==null)            
                Session["first"] = new object();
            else
                showWelcomePopup = false;

            _afterRegistryWelcomePopupBoxHolder.Visible = showWelcomePopup;
        }


		#region Is Projects Enabled
		private static readonly Guid ProjectsID = new Guid("{1E044602-43B5-4d79-82F3-FD6208A11960}");
		protected bool IsProjectsEnabled
		{
			get
			{
				var projects = ProductManager.Instance[ProjectsID];
				if (projects !=null && projects is IWebItem)
				{
					return !(projects as IWebItem).IsDisabled();
				}
				return false;
			}
		} 
		#endregion
    }
}
