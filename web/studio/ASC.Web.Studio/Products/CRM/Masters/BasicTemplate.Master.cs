#region Import

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.SocialMedia;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ListItem = ASC.CRM.Core.Entities.ListItem;

#endregion

namespace ASC.Web.CRM
{
    public partial class BasicTemplate : MasterPage, IStudioMaster
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            Utility.RegisterTypeForAjax(typeof (SocialMediaUI));
        }

        protected void ConfigurePortal()
        {

            #region Task Category

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Call,
                                                                  AdditionalParams = "task_category_call.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Deal,
                                                                  AdditionalParams = "task_category_deal.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Demo,
                                                                  AdditionalParams = "task_category_demo.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Email,
                                                                  AdditionalParams = "task_category_email.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Fax,
                                                                  AdditionalParams = "task_category_fax.png"
                                                              }
                );


            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_FollowUP,
                                                                  AdditionalParams = "task_category_follow_up.png"
                                                              }
                );


            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Lunch,
                                                                  AdditionalParams = "task_category_lunch.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Meeting,
                                                                  AdditionalParams = "task_category_meeting.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Note,
                                                                  AdditionalParams = "task_category_note.png"
                                                              }
                );


            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_Ship,
                                                                  AdditionalParams = "task_category_ship.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_SocialNetworks,
                                                                  AdditionalParams = "task_category_social_networks.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMTaskResource.TaskCategory_ThankYou,
                                                                  AdditionalParams = "task_category_thank_you.png"
                                                              }
                );

            #endregion

            #region Deal Milestone New

           Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone
                    {
                        Title = CRMDealResource.DealMilestone_InitialContact_Title,
                        Description =
                            CRMDealResource.DealMilestone_InitialContact_Description,
                        Probability = 1,
                        Color = "#ca3083",
                        Status = DealMilestoneStatus.Open
                    }
              );

            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone
                    {
                        Title = CRMDealResource.DealMilestone_Preapproach_Title,
                        Description =
                            CRMDealResource.DealMilestone_Preapproach_Description,
                        Probability = 2,
                        Color = "#bf0036",
                        Status = DealMilestoneStatus.Open
                    }
              );

            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Suspect_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Suspect_Description,
                                                                       Probability = 3,
                                                                       Color = "#e34603",
                                                                       SortOrder = 1,
                                                                       Status = DealMilestoneStatus.Open
                                                                   }
                );


            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Champion_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Champion_Description,
                                                                       Probability = 20,
                                                                       Color = "#884cbb",
                                                                       SortOrder = 2,
                                                                       Status = DealMilestoneStatus.Open
                                                                   }
                );

            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Opportunity_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Opportunity_Description,
                                                                       Probability = 50,
                                                                       Color = "#cb59ba",
                                                                       SortOrder = 3,
                                                                       Status = DealMilestoneStatus.Open
                                                                   }
                );


            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Prospect_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Prospect_Description,
                                                                       Probability = 75,
                                                                       Color = "#f88e14",
                                                                       SortOrder = 4,
                                                                       Status = DealMilestoneStatus.Open
                                                                   }
                );

            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Verbal_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Verbal_Description,
                                                                       Probability = 90,
                                                                       Color = "#ffb403",
                                                                       SortOrder = 5,
                                                                       Status = DealMilestoneStatus.Open
                                                                   }
                );

            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Won_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Won_Description,
                                                                       Probability = 100,
                                                                       Color = "#288e31",
                                                                       SortOrder = 6,
                                                                       Status = DealMilestoneStatus.ClosedAndWon
                                                                   }
                );

            Global.DaoFactory.GetDealMilestoneDao().Create(new DealMilestone

                                                                   {
                                                                       Title = CRMDealResource.DealMilestone_Lost_Title,
                                                                       Description =
                                                                           CRMDealResource.DealMilestone_Lost_Description,
                                                                       Probability = 0,
                                                                       Color = "#e24e78",
                                                                       SortOrder = 7,
                                                                       Status = DealMilestoneStatus.ClosedAndLost
                                                                   }
                );


            



            #endregion

            #region Contact Status

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.ContactStatus,
                                                          new ListItem
                                                              {
                                                                  Title = CRMContactResource.ContactStatus_Cold,
                                                                  Color = "#3552d2",
                                                                  SortOrder = 1
                                                              });


            Global.DaoFactory.GetListItemDao().CreateItem(ListType.ContactStatus,
                                                          new ListItem
                                                              {
                                                                  Title = CRMContactResource.ContactStatus_Warm,
                                                                  Color = "#ffb403",
                                                                  SortOrder = 2
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.ContactStatus,
                                                          new ListItem
                                                              {
                                                                  Title = CRMContactResource.ContactStatus_Hot,
                                                                  Color = "#bf0036",
                                                                  SortOrder = 3
                                                              }
                );

            #endregion

            #region History Category

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMCommonResource.HistoryCategory_Note,
                                                                  AdditionalParams = "event_category_note.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMCommonResource.HistoryCategory_Email,
                                                                  AdditionalParams = "event_category_email.png"
                                                              }
                );

            Global.DaoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMCommonResource.HistoryCategory_Call,
                                                                  AdditionalParams = "event_category_call.png"
                                                              }
                );


            Global.DaoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory,
                                                          new ListItem
                                                              {
                                                                  Title = CRMCommonResource.HistoryCategory_Meeting,
                                                                  AdditionalParams = "event_category_meeting.png"
                                                              });

            #endregion

            #region Tags

            #region Contacts

            Global.DaoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Lead);
            Global.DaoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Customer);
            Global.DaoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Supplier);
            Global.DaoFactory.GetTagDao().AddTag(EntityType.Contact, CRMContactResource.Staff);
                       
            #endregion

            #region Deals


            #endregion

            #region Cases


            #endregion


            #endregion

            #region Default Website Contact form Key

            var tenantSettings = Global.TenantSettings;

            tenantSettings.WebFormKey = Guid.NewGuid();

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

            #endregion
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();
            Page.ClientScript.RegisterJavaScriptResource(typeof (CRMJSResource), "CRMJSResources");


            if (!Global.TenantSettings.IsConfiguredPortal)
            {
                ConfigurePortal();

                var tenantSettings = Global.TenantSettings;
                tenantSettings.IsConfiguredPortal = true;

                SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            }


            var onlineUsersControl = (OnlineUsers) LoadControl(OnlineUsers.Location);
            onlineUsersControl.ProductId = ProductEntryPoint.ID;
            phOnlineUsers.Controls.Add(onlineUsersControl);

            Page.EnableViewState = false;
        }

        protected void RenderHeader()
        {
            var topNavigationPanel = (TopNavigationPanel) LoadControl(TopNavigationPanel.Location);

          //  topNavigationPanel.SingleSearchHandlerType = typeof(SearchHandler);

            //   topNavigationPanel.CustomInfoHTML = String.Format("<img vspace='9' hspace='10' align='middle' src='{0}' valign='middle'>", WebImageSupplier.GetAbsoluteWebPath("beta_module.png"));

            _topNavigationPanelPlaceHolder.Controls.Add(topNavigationPanel);

            var absolutePathWithoutQuery = Request.Url.AbsolutePath.Substring(0, Request.Url.AbsolutePath.IndexOf(".aspx"));
            var sysName = absolutePathWithoutQuery.Substring(absolutePathWithoutQuery.LastIndexOf('/') + 1);

            //topNavigationPanel.NavigationItems.Add(new NavigationItem
            //                                           {
            //                                               URL =
            //                                                   String.Concat(PathProvider.BaseAbsolutePath,
            //                                                                 "default.aspx"),
            //                                               Name = CRMCommonResource.DashboardModuleName,
            //                                               Description = "",
            //                                               Selected = String.Compare(sysName, "Default", true) == 0
            //                                           });

            topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                       {
                                                           URL =
                                                               String.Concat(PathProvider.BaseAbsolutePath,
                                                                             "default.aspx"),
                                                           Name = CRMContactResource.Contacts,
                                                           Description = "",
                                                           Selected = String.Compare(sysName, "default", true) == 0
                                                       });

            topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                       {
                                                           URL =
                                                               String.Concat(PathProvider.BaseAbsolutePath, "tasks.aspx"),
                                                           Name = CRMCommonResource.TaskModuleName,
                                                           Description = "",
                                                           Selected = String.Compare(sysName, "Tasks", true) == 0
                                                       });

            topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                       {
                                                           URL =
                                                               String.Concat(PathProvider.BaseAbsolutePath, "deals.aspx"),
                                                           Name = CRMCommonResource.DealModuleName,
                                                           Description = "",
                                                           Selected = String.Compare(sysName, "Deals", true) == 0
                                                       });

            topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                       {
                                                           URL =
                                                               String.Concat(PathProvider.BaseAbsolutePath, "cases.aspx"),
                                                           Name = CRMCommonResource.CasesModuleName,
                                                           Description = "",
                                                           Selected = String.Compare(sysName, "Cases", true) == 0
                                                       });

            //if (CRMSecurity.IsAdmin)
            //    topNavigationPanel.NavigationItems.Add(new NavigationItem
            //                                               {
            //                                                   URL =
            //                                                       String.Concat(PathProvider.BaseAbsolutePath,
            //                                                                     "reports.aspx"),
            //                                                   Name = CRMCommonResource.ReportModuleName,
            //                                                   Description = "",
            //                                                   Selected = String.Compare(sysName, "Reports", true) == 0
            //                                               });

            if (CRMSecurity.IsAdmin)
                topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                           {
                                                               URL =
                                                                   String.Concat(PathProvider.BaseAbsolutePath,
                                                                                 "settings.aspx?type=common"),
                                                               Name = CRMCommonResource.SettingModuleName,
                                                               Description = "",
                                                               Selected = String.Compare(sysName, "Settings", true) == 0,
                                                               RightAlign = true
                                                           });

            topNavigationPanel.NavigationItems.Add(new NavigationItem
                                                       {
                                                           URL = CommonLinkUtility.GetEmployees(ProductEntryPoint.ID),
                                                           Name =
                                                               CustomNamingPeople.Substitute<CRMCommonResource>(
                                                                   "Employees"),
                                                           Description = "",
                                                           Selected =
                                                               UserOnlineManager.Instance.IsEmployeesPage() ||
                                                               UserOnlineManager.Instance.IsUserProfilePage(),
                                                           RightAlign = true
                                                       });
        }

        protected void InitControls()
        {
            RenderHeader();

            var bottomNavigator = new BottomNavigator();
            _bottomNavigatorPlaceHolder.Controls.Add(bottomNavigator);
        }

        #region Methods

        public List<BreadCrumb> BreadCrumbs
        {
            get
            {
                if (_commonContainer.BreadCrumbs == null) _commonContainer.BreadCrumbs = new List<BreadCrumb>();
                return _commonContainer.BreadCrumbs;
            }
        }

        public bool CommonContainerHeaderVisible
        {
            get { return _commonContainer.Options.HeadStyle.Contains("display:none"); }
            set { _commonContainer.Options.HeadStyle = value ? String.Empty : "display:none"; }
        }

        public Container CommonContainer
        {
            get { return _commonContainer; }
        }

        public String CommonContainerHeader
        {
            set { _commonContainer.Options.HeaderBreadCrumbCaption = value; }
        }

        public PlaceHolder ContentHolder
        {
            get
            {
                _commonContainer.Visible = false;
                return _contentHolder;
            }
        }

        public PlaceHolder SideHolder
        {
            get { return (Master as IStudioMaster).SideHolder; }
        }

        public PlaceHolder TitleHolder
        {
            get { return (Master as IStudioMaster).TitleHolder; }
        }

        public PlaceHolder FooterHolder
        {
            get { return (Master as IStudioMaster).FooterHolder; }
        }

        public bool DisabledSidePanel
        {
            get { return (Master as IStudioMaster).DisabledSidePanel; }
            set { (Master as IStudioMaster).DisabledSidePanel = value; }
        }

        public bool? LeftSidePanel
        {
            get { return (Master as IStudioMaster).LeftSidePanel; }
            set { (Master as IStudioMaster).LeftSidePanel = value; }
        }

        #endregion
    }
}