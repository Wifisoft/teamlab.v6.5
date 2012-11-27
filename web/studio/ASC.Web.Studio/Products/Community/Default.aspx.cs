using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Web.Community.Controls;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Dashboard;
using ASC.Web.Studio.Controls.Dashboard.Settings;
using ASC.Web.Studio.Controls.Dashboard.Widgets;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.Activity;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core.Users;
using System.Web;


namespace ASC.Web.Community
{
    public partial class _Default : MainPage
    {
        private sealed class WidgetButton
        {
            public string Link { get; set; }
            public string Icon { get; set; }
            public string Label { get; set; }
        }

        private const String NavigationPanelCookieName = "asc_minimized_np";
        private WidgetTab _widgetTabControl;

        private bool ShowEmptyScreen()
        {
            return !UserActivityManager.HaveUserActivity(
                TenantProvider.CurrentTenantID, 
                CommunityProduct.ID,
                  UserActivityConstants.AllActionType
                );

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((CommunityMasterPage)this.Master).DisabledSidePanel = true;

            this.Title = Resources.CommunityResource.MainPageTitle;

            if (ShowEmptyScreen())
            {
                var dashboardEmptyScreen = (DashboardEmptyScreen)Page.LoadControl(DashboardEmptyScreen.Location);

                AddContent.Controls.Add(dashboardEmptyScreen);

                return;
            }


            _widgetTabControl = new WidgetTab(new Guid("{57DAD9FA-BBB8-4a3a-B208-D3CD58691D35}"),
                                                            ColumnSchemaType.Schema_25_50_25,
                                                            "communityDashboard"
                                                            );

            var modules = new List<ASC.Web.Core.ModuleManagement.Module>();

            foreach (var item in WebItemManager.Instance.GetSubItems(CommunityProduct.ID))
            {
                if ((item is ASC.Web.Core.ModuleManagement.Module) == false)
                    continue;

                var module = item as ASC.Web.Core.ModuleManagement.Module;
                modules.Add(module);


                foreach (var widget in module.Widgets)
                    try
                    {
                        _widgetTabControl.WidgetCollection.Add(GetWidgetControl(module, widget));
                    }
                    catch (Exception ex)
                    {
                        //TODO: draw error control or something   
                        _widgetTabControl.WidgetCollection.Add(GetBrokenWidgetControl(ex));
                    }
            }

            _widgetTabControl.WidgetCollection.Add(new Widget(BirthdayReminderWidget.WidgetID,
                                                            new BirthdayReminderWidget() { ProductID = CommunityProduct.ID },
                                                            Resources.CommunityResource.BirthdayReminderWidgetName,
                                                            Resources.CommunityResource.BirthdayReminderWidgetDescription)
                                                            {
                                                                ImageURL = WebImageSupplier.GetAbsoluteWebPath("birthday_widget.png"),
                                                                SettingsProviderType = typeof(StudioWidgetSettingsProvider),
                                                                UsePositionAttribute = true

                                                            });

            _widgetTabControl.WidgetCollection.Add(new Widget(NewEmployeeWidget.WidgetID,
                                                            new NewEmployeeWidget() { ProductID = CommunityProduct.ID },
                                                            CustomNamingPeople.Substitute<Resources.CommunityResource>("NewEmployeeWidgetName"),
                                                            Resources.CommunityResource.NewEmployeeWidgetDescription)
                                                            {
                                                                ImageURL = WebImageSupplier.GetAbsoluteWebPath("newemp_widget.png"),
                                                                SettingsProviderType = typeof(StudioWidgetSettingsProvider),
                                                                UsePositionAttribute = true
                                                            });

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ProductActivityWidgetSettings>(SecurityContext.CurrentAccount.ID);

            ProductActivity productActivityControl = (ProductActivity)LoadControl(ProductActivity.Location);
            productActivityControl.ProductId = CommunityProduct.ID;
            productActivityControl.Activities = UserActivityManager.GetUserActivities(
                TenantProvider.CurrentTenantID, null, CommunityProduct.ID, null, UserActivityConstants.ContentActionType, null, 0, widgetSettings.CountActivities)
                .ConvertAll(a => new UserContentActivity(a));


            _widgetTabControl.WidgetCollection.Add(new Widget(ProductActivity.WidgetID,
                                                            productActivityControl,
                                                            Resources.CommunityResource.CommunityActivityWidgetName,
                                                            Resources.CommunityResource.CommunityActivityWidgetDescription)
                                                            {
                                                                ImageURL = WebImageSupplier.GetAbsoluteWebPath("lastadded_widget.png"),
                                                                SettingsProviderType = typeof(StudioWidgetSettingsProvider),
                                                                Position = new Point(0, 2),
                                                                WidgetURL = WhatsNew.GetUrlForModule(Product.CommunityProduct.ID, null)
                                                            });


            WidgetsContent.Controls.Add(_widgetTabControl);

            NavigationPanel NavigationPanel = (NavigationPanel)this.LoadControl(NavigationPanel.Location);
            NavigationPanelContent.Controls.Add(NavigationPanel);

            if (SecurityContext.CurrentAccount.IsAuthenticated)
            {
                NavigationPanel.addButton(Resources.CommunityResource.BtnCustomizeWidgets, WebImageSupplier.GetAbsoluteWebPath("btn_managewidgets.png"), "javascript:communityDashboard.ShowSettings()", 3);
                if (modules.Count > 0)
                {
                    NavigationPanel.addButton(Resources.CommunityResource.BtnAddContent, WebImageSupplier.GetAbsoluteWebPath("btn_addcontent.png"), "javascript:StudioManager.ShowAddContentDialog()", 2);
                    AddContentControl AddCntnt = (AddContentControl)this.LoadControl(AddContentControl.Location);

                    foreach (var module in modules)
                        try
                        {
                            AddCntnt.Types.Add(new AddContentControl.ContentTypes { Link = module.Context.GetCreateContentPageAbsoluteUrl(), Icon = (module as IWebItem).GetIconAbsoluteURL(), Label = module.Name });
                        }
                        catch (Exception)
                        {
                            AddCntnt.Types.Add(new AddContentControl.ContentTypes { Link = "#", Icon = string.Empty, Label = "Error loading " + module.Name });
                        }


                    AddContent.Controls.Add(AddCntnt);
                }
            }

        }

        protected Boolean isMinimized()
        {
            return !String.IsNullOrEmpty(CookiesManager.GetCookies(CookiesType.MinimizedNavpanel));
        }

        private Widget GetWidgetControl(IModule module, Core.ModuleManagement.Widget widget)
        {
            var control = (WebControl)Activator.CreateInstance(widget.WidgetType);
            var url = widget.StartURL.Contains("?") ?
                VirtualPathUtility.ToAbsolute(widget.StartURL.Substring(0, widget.StartURL.IndexOf('?'))) + widget.StartURL.Substring(widget.StartURL.IndexOf('?')) :
                VirtualPathUtility.ToAbsolute(widget.StartURL);

            var wc = new Widget(widget.ID,
                                 control,
                                 widget.Name,
                                 widget.Description)
                                 {
                                     ImageURL = widget.Context.IconFileName != null ? WebImageSupplier.GetAbsoluteWebPath(widget.Context.IconFileName, module.ID) : module.GetSmallIconAbsoluteURL(),
                                     SettingsProviderType = widget.SettingsType,
                                     UsePositionAttribute = true,
                                     WidgetURL = url
                                 };

            return wc;
        }

        private Widget GetBrokenWidgetControl(Exception e)
        {
            var broken = Page.LoadControl(BrokenWidget.Path) as BrokenWidget;
            if (broken != null)
            {
                broken.Exception = e;
            }
            return new Widget(Guid.NewGuid(),
                                 broken,
                                 "Broken",
                                 "Widget failed to load") { UsePositionAttribute = true, };
        }

        protected string RenderGreetingTitle()
        {
            return CoreContext.TenantManager.GetCurrentTenant().Name.HtmlEncode();
        }
    }
}
