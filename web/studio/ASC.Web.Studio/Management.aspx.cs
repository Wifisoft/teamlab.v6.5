using System;
using System.Collections.Generic;
using System.Web;
using ASC.Core;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Management : MainPage
    {
        private ManagementType _managementType;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);            

            if (!SecurityContext.CheckPermissions(SecutiryConstants.EditPortalSettings))
            {
                Response.Redirect(VirtualPathUtility.ToAbsolute("~/"));
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var studioMaster = (StudioTemplate)Master;
            studioMaster.DisabledSidePanel = true;

            _managementType = ManagementType.General;
            if (!String.IsNullOrEmpty(Request["type"]))
            {
                try
                {
                    _managementType = (ManagementType)Convert.ToInt32(Request["type"]);
                }
                catch
                {
                    _managementType = ManagementType.General;
                }
            }

            var standalone = CoreContext.Configuration.Standalone;            

            #region top navigation

            var topNavPanel = studioMaster.TopNavigationPanel;
            topNavPanel.CustomTitle = Resources.Resource.Administration;
            topNavPanel.CustomTitleURL = CommonLinkUtility.GetAdministration(ManagementType.General);
            topNavPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("settings.png");

            //general
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.GeneralSettings,
                URL = CommonLinkUtility.GetAdministration(ManagementType.General),
                Selected = (_managementType == ManagementType.General)
            });

            //access rights
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.AccessRights,
                URL = CommonLinkUtility.GetAdministration(ManagementType.AccessRights),
                Selected = (_managementType == ManagementType.AccessRights)
            });

            //customization
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Customization,
                URL = CommonLinkUtility.GetAdministration(ManagementType.Customization),
                Selected = (_managementType == ManagementType.Customization)
            });


            //mail  
            if (SetupInfo.IsVisibleSettings<MailSettings>() || standalone)
            {
                topNavPanel.NavigationItems.Add(new NavigationItem()
                {
                    Name = Resources.Resource.SmtpSettings,
                    URL = CommonLinkUtility.GetAdministration(ManagementType.Mail),
                    Selected = (_managementType == ManagementType.Mail)
                });
            }

            //account
            if (!standalone && SetupInfo.IsVisibleSettings<Backup>() && CoreContext.TenantManager.GetCurrentTenant().OwnerId == SecurityContext.CurrentAccount.ID)
            {   
                topNavPanel.NavigationItems.Add(new NavigationItem()
                {
                    Name = Resources.Resource.Backup,
                    URL = CommonLinkUtility.GetAdministration(ManagementType.Account),
                    Selected = (_managementType == ManagementType.Account)
                });
            }

            //statistic
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Statistic,
                URL = CommonLinkUtility.GetAdministration(ManagementType.Statistic),
                Selected = (_managementType == ManagementType.Statistic)
            });

            //tariff
            if (!standalone && SetupInfo.IsVisibleSettings<TariffSettings>())
            {
                topNavPanel.NavigationItems.Add(new NavigationItem()
                {
                    Name = Resources.Resource.TariffSettings,
                    URL = CommonLinkUtility.GetAdministration(ManagementType.Tariff),
                    Selected = (_managementType == ManagementType.Tariff)
                });
            }

            
            #endregion

            _settingsContainer.BreadCrumbs = new List<BreadCrumb>();
            _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.Administration, NavigationUrl = CommonLinkUtility.GetAdministration(ManagementType.General) });


            //content
            switch (_managementType)
            {
                case ManagementType.Mail:
                    if (!(standalone || SetupInfo.IsVisibleSettings<MailSettings>()))
                        Response.Redirect("~/", true);

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.SmtpSettings, null, null);
                    _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.SmtpSettings });
                    _settingsContainer.Body.Controls.Add(LoadControl(MailSettings.Location));
                    break;
                   
                case ManagementType.Tariff:
                    if (standalone)
                        Response.Redirect("~/", true);


                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.TariffSettings, null, null);
                    _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.TariffSettings });
                    _settingsContainer.Body.Controls.Add(LoadControl(TariffSettings.Location));
                    break;

                case ManagementType.Statistic:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.StatisticsTitle, null, null);
                    _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.StatisticsTitle });
                    _settingsContainer.Body.Controls.Add(LoadControl(ProductQuotes.Location));
                    _settingsContainer.Body.Controls.Add(LoadControl(VisitorsChart.Location));
                    break;

                case ManagementType.Account:

                    //No backup in standalone..
                    if (standalone)
                        Response.Redirect("~/", true);

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Backup, null, null);
                    _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.Backup });
                    _settingsContainer.Body.Controls.Add(LoadControl(Backup.Location));
                    break;

                case ManagementType.Customization:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Customization, null, null);
                    _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.Customization });

                    //greeting settings
                    _settingsContainer.Body.Controls.Add(LoadControl(GreetingSettings.Location));

                    //naming people
                    _settingsContainer.Body.Controls.Add(LoadControl(NamingPeopleSettings.Location));

                    //skin settings
                    _settingsContainer.Body.Controls.Add(LoadControl(SkinSettings.Location));

                    break;

                case ManagementType.AccessRights:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.AccessRights, null, null);
                    _settingsContainer.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.AccessRights });
                    _settingsContainer.Body.Controls.Add(LoadControl(AccessRights.Location));
                    break;

                case ManagementType.General:
                default:

                    this.Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Administration, null, null);
                    _settingsContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = Resources.Resource.Administration});         

                    var settingsControl = LoadControl(StudioSettings.Location) as StudioSettings;
                    _settingsContainer.Body.Controls.Add(settingsControl);
                    break;
            }
        }
    }
}
