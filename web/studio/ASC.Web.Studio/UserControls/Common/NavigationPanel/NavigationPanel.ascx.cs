using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Studio.UserControls.Common
{
    public partial class NavigationPanel : System.Web.UI.UserControl
    {
        protected TenantInfoSettings TenantInfoSettings;


        protected override void OnInit(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "ajaxupload_script", WebPath.GetPath("js/ajaxupload.3.5.js"));
            TenantInfoSettings = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID);
        }

        private sealed class WidgetButton
        {
            public string Link { get; set; }
            public string Icon { get; set; }
            public string Label { get; set; }
        }

        private List<WidgetButton> _buttons = null;
        
        private List<WidgetButton> buttons
        {
            get
            {
                if (_buttons == null)
                {
                    _buttons = new List<WidgetButton>();

                    //check permissions 
                    //
                    Boolean showInvite = SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser);
                    Boolean ShowSettings = SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser,
                                                                            ASC.Core.Users.Constants.Action_EditUser,
                                                                            ASC.Core.Users.Constants.Action_EditGroups);

                    //adding buttons on panel
                    //
                    if (ShowSettings)
                        addButton(
                            Resources.UserControlsCommonResource.BtnChangeSettings,
                            WebImageSupplier.GetAbsoluteWebPath("btn_settings.png"),
                            CommonLinkUtility.GetAdministration(ManagementType.General));

                    if (SecurityContext.IsAuthenticated)
                        addButton(
                            Resources.UserControlsCommonResource.BtnChangeProfile,
                            WebImageSupplier.GetAbsoluteWebPath("btn_changeprofile.png"),
                            CommonLinkUtility.GetMyStaff(MyStaffType.General));

                    if (showInvite)
                    {
                        addButton(
                                CustomNamingPeople.Substitute<Resources.Resource>("AddEmployeesButton"),
                                WebImageSupplier.GetAbsoluteWebPath("btn_invitepeople.png"),
                                "javascript:ImportUsersManager.ShowImportControl()");
                        
                    }
                }
                return _buttons;
            }
        }

        public static string Location { get { return "~/UserControls/Common/NavigationPanel/NavigationPanel.ascx"; } }

        protected string RenderGreetingTitle()
        {
            return CoreContext.TenantManager.GetCurrentTenant().Name.HtmlEncode();
        }

        public void addButton(String name, String icon, String link)
        {
            buttons.Add(new WidgetButton { Link = link, Icon = icon, Label = name });
        }

        public void addButton(String name, String icon, String link, Int32 position)
        {
            if (position > 0 && position <= buttons.Count)
                buttons.Insert(buttons.Count - position + 1, new WidgetButton { Link = link, Icon = icon, Label = name });
            else
                buttons.Add(new WidgetButton { Link = link, Icon = icon, Label = name });
        }

        public void removeButtons()
        {
            buttons.Clear();
        }

        private void InitScripts()
        {
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "navpanel_script", WebPath.GetPath("usercontrols/common/navigationpanel/js/navigator.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "navpanel_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/navigationpanel/css/<theme_folder>/navigationpanel.css") + "\">", false);
        }

        protected Boolean isMinimized()
        {
            return !String.IsNullOrEmpty(CookiesManager.GetCookies(CookiesType.MinimizedNavpanel));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitScripts();
            buttonRepeater.DataSource = buttons;
            buttonRepeater.DataBind();
        }
    }
}