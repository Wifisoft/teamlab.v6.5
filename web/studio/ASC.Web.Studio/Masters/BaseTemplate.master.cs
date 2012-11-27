using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.Mobile;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Globalization;
using System.Configuration;

namespace ASC.Web.Studio.Masters
{
    [AjaxNamespace("WebStudio")]
    public partial class BaseTemplate : MasterPage, IStudioMaster
    {
        private string getDatepikerDateFormat(String s)
        {
            var templates = new Dictionary<String, String>();
            templates.Add("yyyy", "yy");
            //templates.Add("yy", "y");
            templates.Add("yy", "yy");
            templates.Add("MMMM", "MM");
            templates.Add("MMM", "M");
            templates.Add("MM", "mm");
            //templates.Add("M", "m");
            templates.Add("M", "mm");
            templates.Add("dddd", "DD");
            templates.Add("ddd", "D");
            //templates.Add("dd", "dd");
            //templates.Add("d", "d");
            templates.Add("dd", "11");
            templates.Add("d", "dd");
            templates.Add("11", "dd");
            foreach (var template in templates)
            {
                s = s.Replace(template.Key, template.Value);
            }
            return s;
        }

        private String EscapeJsString(String s)
        {
            return s.Replace("'", "\\'");
        }

        private int getFirtsDay()
        {
            return (int)System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        }

        private string GetMonthNames()
        {
            return String.Join(",", System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames);
        }

        private string GetShortMonthNames()
        {
            return String.Join(",", System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames);
        }

        private string GetDayNames()
        {
            return String.Join(",", System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DayNames);
        }

        private string GetShortDayNames()
        {
            return String.Join(",", System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedDayNames);
        }

        private string GetFullDateTimePattern()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FullDateTimePattern;
        }

        private string GetShortTimePattern()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern;
        }

        private string GetShortDatePattern()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        protected bool _visibleSidePanel { get; set; }

        protected StudioViewSettings _panelViewSettings;

        protected StudioNotifyBarSettings _notifyBarSettings;

        protected bool _leftSidePanel;

        protected bool WidePageContent
        {
            get
            {
                var attrWide = Attribute.GetCustomAttribute(Page.GetType(), typeof(WidePageAttribute), true) as WidePageAttribute;
                return attrWide != null && attrWide.Wide;
            }
        }

        protected bool UserVoice { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _visibleSidePanel = true;
            _panelViewSettings = SettingsManager.Instance.LoadSettings<StudioViewSettings>(TenantProvider.CurrentTenantID);
            _notifyBarSettings = SettingsManager.Instance.LoadSettings<StudioNotifyBarSettings>(TenantProvider.CurrentTenantID);

            _leftSidePanel = LeftSidePanel.HasValue ? LeftSidePanel.Value : _panelViewSettings.LeftSidePanel;

            if (SecurityContext.IsAuthenticated)
            {
                AjaxPro.Utility.RegisterTypeForAjax(GetType());
                _visibleSidePanel = _panelViewSettings.VisibleSidePanel;
            }

            UserVoice = WebConfigurationManager.AppSettings["web.uservoice"] == "true";

            RegisterClientSideScript();
        }

        private void RegisterClientSideScript()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var mobileAgentStr = MobileDetector.IsRequestMatchesMobile(Context).ToString().ToLower();

            var sb = new StringBuilder();

            //Skin images
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append(JsSkinHash.GetJs());
            sb.Append("</script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/jquery_full.js") + "\"></script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/asc.customevents.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/api.factory.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/api.helper.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/api.service.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/asc.teamlab.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.AppendFormat("ServiceManager.init('{0}');", SetupInfo.WebApiBaseUrl);
            sb.AppendFormat("ServiceFactory.init({{responses:{{isme:'{0}'}},portaldatetime:{{utcoffsettotalminutes:{1}, displayname:'{2}'}},names:{{months:'{3}',shortmonths:'{4}',days:'{5}',shortdays:'{6}'}},formats:{{datetime:'{7}',time:'{8}',date:'{9}'}},avatars:{{small:'{10}',medium:'{11}',large:'{12}'}},supportedfiles:{{imgs:'{13}',docs:'{14}'}}}});",
                            new Api.ApiServer().GetApiResponse("api/1.0/people/@self.json", "GET"),
                            CoreContext.TenantManager.GetCurrentTenant().TimeZone.GetUtcOffset(DateTime.UtcNow).TotalMinutes,
                            CoreContext.TenantManager.GetCurrentTenant().TimeZone.DisplayName,
                            EscapeJsString(GetMonthNames()),
                            EscapeJsString(GetShortMonthNames()),
                            EscapeJsString(GetDayNames()),
                            EscapeJsString(GetShortDayNames()),
                            EscapeJsString(GetFullDateTimePattern()),
                            EscapeJsString(GetShortTimePattern()),
                            EscapeJsString(GetShortDatePattern()),
                            WebImageSupplier.GetAbsoluteWebPath("default_user_photo_size_32-32.gif"),
                            WebImageSupplier.GetAbsoluteWebPath("default_user_photo_size_48-48.gif"),
                            WebImageSupplier.GetAbsoluteWebPath("default_user_photo_size_82-82.gif"),
                            ConfigurationManager.AppSettings["files.viewed-images"],
                            ConfigurationManager.AppSettings["files.docservice.viewed-docs"]);
            sb.Append("Teamlab.init();");
            sb.Append("</script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/jquery.cookies.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/fancyzoom.min.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/jquery.helper.js") + "\"></script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/jquery-datepicker.js") + "\"></script>");
            sb.Append("<script type=\"text/javascript\">");
            sb.AppendFormat("if(jQuery.datepicker)jQuery.datepicker.setDefaults({{prevText:'',nextText:'',firstDay:{0},dateFormat:'{1}',dayNamesMin:'{2}'.split(','),monthNames:'{3}'.split(',')}});",
                            getFirtsDay().ToString(),
                            EscapeJsString(getDatepikerDateFormat(GetShortDatePattern())),
                            EscapeJsString(GetShortDayNames()),
                            EscapeJsString(GetMonthNames()));
            sb.Append("</script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append("jQuery.extend(jQuery.browser, {mobile : " + mobileAgentStr + "});");
            sb.Append("</script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/common.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/profile_info_tooltip.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/management.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/asc.anchorcontroller.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/asc.xsltmanager.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/jquery.datepick.lang.js") + "\"></script>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"" + WebPath.GetPath("js/auto/asc.files.utility.js") + "\"></script>");

            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");

            sb.Append(" jq.datepick.setDefaults(jq.datepick.regional['" + currentCulture + "']); ");
            sb.Append(" StudioManager.LoadingProcessing = \"" + Resources.Resource.LoadingProcessing + "\"; ");
            sb.Append(" StudioManager.LoadingDescription = \"" + Resources.Resource.LoadingDescription + "\"; ");
            sb.Append(" StudioManager.RemoveMessage = \"" + Resources.Resource.DeleteButton + "\"; ");
            sb.Append(" StudioManager.ErrorFileSizeLimit = \"" + FileSizeComment.FileSizeExceptionString + "\"; ");
            sb.Append(" StudioManager.ErrorFileEmpty = \"" + Resources.Resource.ErrorFileEmptyText + "\"; ");
            sb.Append(" StudioManager.ErrorFileTypeText = \"" + Resources.Resource.ErrorFileTypeText + "\"; ");
            sb.Append(" AuthManager.ConfirmMessage = \"" + Resources.Resource.ConfirmMessage + "\"; ");
            sb.Append(" AuthManager.ConfirmRemoveUser = \"" + Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("ConfirmRemoveUser").HtmlEncode() + "\"; ");
            sb.AppendFormat(" function GetMaxImageWidth() {{ return {0}; }};", ConfigurationManager.AppSettings["MaxImageFCKWidth"] ?? "620");
            sb.AppendFormat(" jq(document).ready(function() {{ jq('a.fancyzoom').fancyZoom({{scaleImg: true, closeOnClick: true, directory : '{0}'}}); }});", WebSkin.GetUserSkin().GetCSSFileAbsoluteWebPath("fancyzoom_img"));

            if (SetupInfo.WorkMode == WorkMode.Promo)
            {
                sb.Append(" PromoMode = true; PromoActionURL='" + SetupInfo.PromoActionURL + "'; ");
            }

            sb.Append("</script>");

            sb.Append(FileUtility.GetFileUtilityJScript());

            mainScript.Text = sb.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SaveSidePanelState(bool visible)
        {
            var personalView = SettingsManager.Instance.LoadSettingsFor<StudioViewSettings>(SecurityContext.CurrentAccount.ID);
            personalView.VisibleSidePanel = visible;
            return SettingsManager.Instance.SaveSettingsFor(personalView, SecurityContext.CurrentAccount.ID) ? "1" : "0";
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public JavaScriptObject GetUsersInfo(Guid userID, Guid productID, Object userValue)
        {
            const String host = "teamlab";

            var users = new List<UserInfo>(CoreContext.UserManager.GetUsers());
            users = users.SortByUserName();

            var jsUsers = new JavaScriptArray();
            for (int i = 0, n = users.Count; i < n; i++)
            {
                var jsUser = new JavaScriptObject();
                jsUser.Add("id", new JavaScriptString(users[i].ID.ToString()));
                jsUser.Add("departmentId", new JavaScriptString(users[i].GetUserDepartment().ID.ToString()));
                jsUser.Add("jid", new JavaScriptString(users[i].UserName + "@" + host));
                jsUser.Add("userName", new JavaScriptString(users[i].UserName));
                jsUser.Add("displayName", new JavaScriptString(users[i].DisplayUserName()));
                jsUser.Add("profileURL", new JavaScriptString(users[i].GetUserProfilePageURL(productID)));
                //            jsUser.Add("email", new JavaScriptString(users[i].Email));
                jsUser.Add("photo", new JavaScriptString(users[i].GetPhotoURL()));
                jsUser.Add("bigPhoto", new JavaScriptString(users[i].GetBigPhotoURL()));
                jsUser.Add("mediumPhoto", new JavaScriptString(users[i].GetMediumPhotoURL()));
                jsUser.Add("smallPhoto", new JavaScriptString(users[i].GetSmallPhotoURL()));
                jsUser.Add("title", new JavaScriptString(HttpUtility.HtmlEncode(users[i].Title)));
                jsUser.Add("departmentTitle", new JavaScriptString(HttpUtility.HtmlEncode(users[i].Department)));
                //            jsUser.Add("departmentURL", new JavaScriptString(CommonLinkUtility.GetDepartment(productID, users[i].GetUserDepartment().ID)));
                //            jsUser.Add("workFromDate", new JavaScriptString(users[i].WorkFromDate == null ? String.Empty : users[i].WorkFromDate.Value.ToShortDateString()));

                jsUsers.Add(jsUser);
            }
            var jsResult = new JavaScriptObject();
            jsResult.Add("users", jsUsers);
            jsResult.Add("userValue", new JavaScriptString(userValue.ToString()));

            return jsResult;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public JavaScriptObject GetFullUserInfo(Guid userID, Guid productID, Object userValue)
        {
            var userInfo = CoreContext.UserManager.GetUsers(userID);

            var jsUserInfo = new JavaScriptObject();
            jsUserInfo.Add("id", new JavaScriptString(userInfo.ID.ToString()));
            var department = userInfo.GetUserDepartment();
            if (department != null)
            {
                jsUserInfo.Add("departmentId", new JavaScriptString(department.ID.ToString()));
                jsUserInfo.Add("departmentURL", new JavaScriptString(CommonLinkUtility.GetDepartment(productID, department.ID)));
            }
            jsUserInfo.Add("userName", new JavaScriptString(userInfo.UserName));
            jsUserInfo.Add("displayName", new JavaScriptString(userInfo.DisplayUserName()));
            jsUserInfo.Add("profileURL", new JavaScriptString(userInfo.GetUserProfilePageURL(productID)));
            jsUserInfo.Add("email", new JavaScriptString(userInfo.Email));
            jsUserInfo.Add("photo", new JavaScriptString(userInfo.GetPhotoURL()));
            jsUserInfo.Add("bigPhoto", new JavaScriptString(userInfo.GetBigPhotoURL()));
            jsUserInfo.Add("mediumPhoto", new JavaScriptString(userInfo.GetMediumPhotoURL()));
            jsUserInfo.Add("smallPhoto", new JavaScriptString(userInfo.GetSmallPhotoURL()));
            jsUserInfo.Add("title", new JavaScriptString(HttpUtility.HtmlEncode(userInfo.Title)));
            jsUserInfo.Add("departmentTitle", new JavaScriptString(HttpUtility.HtmlEncode(userInfo.Department)));
            jsUserInfo.Add("workFromDate", new JavaScriptString(userInfo.WorkFromDate == null ? String.Empty : userInfo.WorkFromDate.Value.ToShortDateString()));
            jsUserInfo.Add("userValue", new JavaScriptString(userValue.ToString()));

            return jsUserInfo;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void SaveGettingStartedState(bool disableGettingStarted)
        {
            var settings = SettingsManager.Instance.LoadSettingsFor<DisplayUserSettings>(SecurityContext.CurrentAccount.ID);
            settings.IsDisableGettingStarted = disableGettingStarted;
            SettingsManager.Instance.SaveSettingsFor(settings, SecurityContext.CurrentAccount.ID);

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse GetUserInfo(Guid userID, Guid productID, string popupBoxID)
        {
            var resp = new AjaxResponse { rs1 = popupBoxID };

            var userInfo = CoreContext.UserManager.GetUsers(userID);
            var sb = new StringBuilder();
            sb.Append("<div style=\"overflow: hidden; margin-right: 10px; padding: 10px 0pt 10px 10px;\"><table cellspacing='0' cellpadding='0' style='width:100%;'><tr valign=\"top\">");

            //avatar
            sb.Append("<td style=\"width:90px;\">");
            sb.Append("<img alt=\"\" class='userPhoto' src=\"" + userInfo.GetBigPhotoURL() + "\"/>");
            sb.Append("</td>");

            sb.Append("<td style='padding-left:10px;'>");

            //name
            sb.Append("<div>");
            sb.Append("<a class='linkHeaderLight' href='" + userInfo.GetUserProfilePageURL(productID) + "'>" + userInfo.DisplayUserName() + "</a>");
            sb.Append("</div>");

            //department
            var dep = userInfo.GetUserDepartment();
            if (dep != null)
            {
                sb.Append("<div style=\"margin-top:6px;\">");
                sb.Append("<a href='" + CommonLinkUtility.GetDepartment(productID, dep.ID) + "'>" + dep.Name.HtmlEncode() + "</a>");
                sb.Append("</div>");
            }

            //title
            sb.Append("<div style=\"margin-top:6px;\">");
            sb.Append(HttpUtility.HtmlEncode(userInfo.Title));
            sb.Append("</div>");

            //communications
            sb.Append("<div style=\"margin-top:6px;\">");
            sb.Append(userInfo.RenderUserCommunication());
            sb.Append("</div>");

            sb.Append("</td>");
            sb.Append("</tr></table></div>");

            resp.rs2 = sb.ToString();
            return resp;
        }

        protected string GetCurrentLanguage()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }

        protected string RenderStatRequest()
        {
            if (string.IsNullOrEmpty(SetupInfo.StatisticTrackURL)) return string.Empty;

            var page = HttpUtility.UrlEncode(this.Page.AppRelativeVirtualPath.Replace("~", ""));
            return String.Format("<img style=\"display:none;\" src=\"{0}\"/>", SetupInfo.StatisticTrackURL + "&page=" + page);
        }

        protected string RenderHTMLInjections()
        {
            if (!SecurityContext.IsAuthenticated)
                return "";

            var sb = new StringBuilder();
            foreach (var product in ProductManager.Instance.Products)
            {
                var injectionProvider = product.GetInjectionProvider();
                if (injectionProvider != null)
                    sb.Append(injectionProvider.GetInjection());

                if (product.Modules == null)
                    continue;
                foreach (var module in product.Modules)
                {
                    injectionProvider = module.GetInjectionProvider();
                    if (injectionProvider != null)
                        sb.Append(injectionProvider.GetInjection());
                }

            }
            return sb.ToString();
        }

        protected string RenderCustomScript()
        {
            var sb = new StringBuilder();
            //custom scripts
            foreach (var script in SetupInfo.CustomScripts)
            {
                if (!String.IsNullOrEmpty(script))
                    sb.Append("<script language=\"javascript\" src=\"" + script + "\" type=\"text/javascript\"></script>");
            }

            return sb.ToString();
        }


        #region Keep session alive during FCKEditor is opened

        /// <summary>
        /// This method is called from common.js file when document is loaded and the page contains FCKEditor.
        /// It is used to keep session alive while FCKEditor is opened and not empty.
        /// </summary>
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void KeepSessionAlive()
        {
            try
            {
                HttpContext.Current.Session["KeepSessionAlive"] = DateTime.Now;
            }
            catch
            {
            }
        }

        #endregion


        #region IStudioMaster Members

        /// <summary>
        /// Block side panel
        /// </summary>
        public bool DisabledSidePanel { get; set; }

        public bool? LeftSidePanel { get; set; }

        public PlaceHolder ContentHolder
        {
            get { return _contentHolder; }
        }

        public PlaceHolder SideHolder
        {
            get { return _sideHolder; }
        }

        public PlaceHolder TitleHolder
        {
            get { return _titleHolder; }
        }

        public PlaceHolder FooterHolder
        {
            get { return _footerHolder; }
        }

        public ScriptManager ScriptManager
        {

            get { return ScriptManager1; }

        }

        #endregion

        protected string RenderPromoBar()
        {
            var notifyAddress = SetupInfo.NotifyAddress;
            if (!string.IsNullOrEmpty(notifyAddress) && SecurityContext.IsAuthenticated)
            {
                var userId = SecurityContext.CurrentAccount.ID;
                var page = Request.Url.PathAndQuery;
                var language = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant();
                var admin = CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID);

                return new StringBuilder().
                    AppendFormat(@"<script>jq(function(){{ jq.getScript('{0}/promotion/get?userId={1}&page={2}&language={3}&admin={4}&promo={5}&version={6}'); }});</script>"
                                 , notifyAddress, userId, page, language, admin.ToString().ToLower(), _notifyBarSettings.ShowPromotions.ToString().ToLower(),CoreContext.TenantManager.GetCurrentTenant().Version).ToString();
            }
            return string.Empty;
        }
    }
}