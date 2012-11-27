using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WebZones;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.Studio
{
    public partial class MyStaff : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SecurityContext.DemoMode || (CoreContext.TenantManager.GetCurrentTenant().Public && !SecurityContext.CurrentAccount.IsAuthenticated))
                Response.Redirect("~/default.aspx");

            (this.Master as IStudioMaster).DisabledSidePanel = true;
            var _userInfo = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            var type = Request["type"] ?? "";

            var myToolsItems = WebItemManager.Instance.GetItems(WebZoneType.MyTools);
            IRenderMyTools myToolsRender = null;
            foreach (var item in myToolsItems)
            {
                myToolsRender = WebItemManager.Instance[item.ID] as IRenderMyTools;
                if (myToolsRender != null && String.Equals(myToolsRender.ParameterName, type, StringComparison.InvariantCultureIgnoreCase))
                    break;

                myToolsRender = null;
            }
           
            #region top panel

            bool isDefault;
            var enumType = typeof(MyStaffType).TryParseEnum<MyStaffType>(Request["type"] ?? "", MyStaffType.General, out isDefault);
            if (String.IsNullOrEmpty(Request["type"]))
                isDefault = false;


            //top panel
            ((StudioTemplate)this.Master).TopNavigationPanel.CustomTitle = Resources.Resource.MyStaff;
            ((StudioTemplate)this.Master).TopNavigationPanel.CustomTitleURL = CommonLinkUtility.GetMyStaff(MyStaffType.General);
            ((StudioTemplate)this.Master).TopNavigationPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("staff.gif");

            //profile
            ((StudioTemplate)this.Master).TopNavigationPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Profile,
                URL = CommonLinkUtility.GetMyStaff(MyStaffType.General),
                Selected = (enumType == MyStaffType.General && !isDefault)
            });


            //activity
            ((StudioTemplate)this.Master).TopNavigationPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.RecentActivity,
                URL = CommonLinkUtility.GetMyStaff(MyStaffType.Activity),
                Selected = (enumType == MyStaffType.Activity)
            });

            //subscriptions
            ((StudioTemplate)this.Master).TopNavigationPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Subscriptions,
                URL = CommonLinkUtility.GetMyStaff(MyStaffType.Subscriptions),
                Selected = (enumType == MyStaffType.Subscriptions)
            });

            //Customisation
            ((StudioTemplate)this.Master).TopNavigationPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Customization,
                URL = CommonLinkUtility.GetMyStaff(MyStaffType.Customization),
                Selected = (enumType == MyStaffType.Customization)
            });
            
            foreach (var item in myToolsItems)
            {
                var render = WebItemManager.Instance[item.ID] as IRenderMyTools;
                if (render == null)
                    continue;

                ((StudioTemplate)this.Master).TopNavigationPanel.NavigationItems.Add(new NavigationItem()
                {
                    Name = render.TabName,
                    URL = CommonLinkUtility.GetMyStaff(render.ParameterName),
                    Selected = String.Equals(Request["type"] ?? "", render.ParameterName, StringComparison.InvariantCultureIgnoreCase)
                });
            }
            
            #endregion
            
            if (String.Equals(type, MyStaffType.Activity.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {

                _myStaffContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() {Caption = Resources.Resource.RecentActivity});
                
                ClientScriptManager cs = Page.ClientScript;

                StringBuilder helpcstext = new StringBuilder();
                helpcstext.Append("<script type=text/javascript> var header = jq('.mainContainerClass>.containerHeaderBlock table td div').last();");
                helpcstext.Append("header.css('float', 'left');");
                helpcstext.Append("header.after('<div class=\"HelpCenterSwitcher title big\" id=\"QuestionForHelpActivityPortal\" title=\"" + String.Format(Resources.Resource.HelpQuestionActivityPortal) + "\"></div>');");
                helpcstext.Append("jq('#QuestionForHelpActivityPortal').click(function() {jq(this).helper({ BlockHelperID: 'AnswerForHelpActivityPortal' });}); </");
                helpcstext.Append("script>");
                cs.RegisterStartupScript(this.GetType(), "HelpScript", helpcstext.ToString());

                StringBuilder helptext = new StringBuilder();
                helptext.Append("<div id='AnswerForHelpActivityPortal' class='popup_helper'>");
                helptext.Append("<p>" + String.Format(Resources.Resource.HelpAnswerActivityPortal, "<br />", "<b>", "</b>") + "</p>");
                helptext.Append("</div>");
                _contentHolder.Controls.Add(new Literal() { Text = helptext.ToString() });

                bool isFirst = true;
                foreach (var product in WebItemManager.Instance.GetItems(WebZoneType.All).OfType<IProduct>())
                {
                    if (product.Context != null && product.Context.UserActivityControlLoader != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<div id='studio_product_activityBox_" + product.ID + "' class='borderBase tintMedium clearFix' style='border-left:none; border-right:none; margin-top:-1px; padding:10px;'>");
                        sb.Append("<div class='headerBase' style='float:left; cursor:pointer;' onclick=\"StudioManager.ToggleProductActivity('" + product.ID + "');\">");
                        string logoURL = product.GetIconAbsoluteURL();
                        if (!String.IsNullOrEmpty(logoURL))
                            sb.Append("<img alt='' style='margin-right:5px;' align='absmiddle' src='" + logoURL + "'/>");
                        sb.Append(product.Name.HtmlEncode());
                        sb.Append("<img alt='' align='absmiddle' id='studio_activityProductState_" + product.ID + "' style='margin-left:15px;'  src='" + WebImageSupplier.GetAbsoluteWebPath(isFirst ? "collapse_down_dark.png" : "collapse_right_dark.png") + "'/>");
                        sb.Append("</div>");
                        sb.Append("</div>");
                        sb.Append("<div id=\"studio_product_activity_" + product.ID + "\" style=\"padding-left:40px; " + (isFirst ? "" : "display:none;") + " padding-top:20px;\">");

                        _contentHolder.Controls.Add(new Literal() {Text = sb.ToString()});
                        var activityControl = product.Context.UserActivityControlLoader.LoadControl(_userInfo.ID);
                        _contentHolder.Controls.Add(activityControl);

                        sb = new StringBuilder();
                        sb.Append("</div>");
                        _contentHolder.Controls.Add(new Literal() {Text = sb.ToString()});

                        isFirst = false;
                    }
                }

            }
            else if (String.Equals(type, MyStaffType.Subscriptions.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {

                _myStaffContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() {Caption = Resources.Resource.Subscriptions});
                var userSubscriptions = LoadControl(UserSubscriptions.Location) as UserSubscriptions;
                _contentHolder.Controls.Add(userSubscriptions);

            }
            else if (String.Equals(type, MyStaffType.Customization.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {

                _myStaffContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = Resources.Resource.Customization });
                var userCustomisationControl = LoadControl(UserCustomisationControl.Location) as UserCustomisationControl;
                _contentHolder.Controls.Add(userCustomisationControl);

            }           

            else if (myToolsRender != null)
            {

                _myStaffContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() {Caption = myToolsRender.TabName});
                var control = myToolsRender.LoadMyToolsControl(this);

                if (control is IFullScreenControl)
                {
                    _myStaffContainer.Visible = false;
                    (this.Master as IStudioMaster).ContentHolder.Controls.Add(control);
                }
                else
                    _contentHolder.Controls.Add(control);
            }
            else
            {
                _myStaffContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() {Caption = _userInfo.DisplayUserName(false)});

                var userProfileControl = LoadControl(UserProfileControl.Location) as UserProfileControl;
                userProfileControl.UserInfo = _userInfo;
                userProfileControl.MyStaffMode = true;

                _contentHolder.Controls.Add(userProfileControl);

            }

            this.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.MyStaff, null, null);

        }
    }
}