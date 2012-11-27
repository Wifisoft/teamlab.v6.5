using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.Core;
using ASC.Web.Core.Mobile;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using log4net;
using Constants = ASC.Core.Users.Constants;
using File = ASC.Files.Core.File;
using Global = ASC.Web.Files.Classes.Global;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files
{
    public partial class DocEditor : Page
    {
        protected bool WithLink;
        protected string ShareLink;

        protected DocServiceParams DocParams;
        protected string DocKeyForTrack;
        protected bool FileNew;

        protected WebSkin CurrentSkin { get; set; }

        protected static bool AutoAuthByPromo()
        {
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.Demo);
                WebItemManager.Instance.ItemGlobalHandlers.Login(SecurityContext.CurrentAccount.ID);
                return true;
            }
            catch
            {
            }

            return false;
        }

        protected static bool AutoAuthByCookies()
        {
            return AuthByCookies(CookiesManager.GetCookies(CookiesType.AuthKey));
        }

        protected static bool AuthByCookies(string cookiesKey)
        {
            var result = false;

            if (!string.IsNullOrEmpty(cookiesKey))
            {
                try
                {
                    if (SecurityContext.AuthenticateMe(cookiesKey))
                    {
                        result = true;
                        WebItemManager.Instance.ItemGlobalHandlers.Login(SecurityContext.CurrentAccount.ID);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger("ASC.Web").ErrorFormat("AutoAuthByCookies Error {0}", ex);
                }
            }

            return result;
        }

        private void ProcessSecureFilter()
        {
            var filter = SetupInfo.SecureFilter;
            if (HttpContext.Current != null)
            {
                //ssl enable only on subdomain of basedomain
                if (HttpContext.Current.Request.GetUrlRewriter().Host.EndsWith("." + SetupInfo.BaseDomain))
                    SecureFilter.GetInstance(filter).ProcessRequest(Request.GetUrlRewriter(), SetupInfo.SslPort, SetupInfo.HttpPort);
            }
        }

        private bool ValidateRefererUrl(string refererURL)
        {
            return !String.IsNullOrEmpty(refererURL)
                   && refererURL.IndexOf("Subgurim_FileUploader", StringComparison.InvariantCultureIgnoreCase) == -1
                   && refererURL.IndexOf("servererror.aspx", StringComparison.InvariantCultureIgnoreCase) == -1
                   && refererURL.IndexOf("error404.aspx", StringComparison.InvariantCultureIgnoreCase) == -1;
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (MobileDetector.IsRequestMatchesMobile(Context))
            {
                Response.Redirect(CommonLinkUtility.GetFileWebViewerUrl(Request[UrlConstant.FileId]) +
                                  (string.IsNullOrEmpty(Request[UrlConstant.DocUrlKey]) ? "" : "&" + UrlConstant.DocUrlKey + "=" + Request[UrlConstant.DocUrlKey]));
            }

            if (!FileUtility.EnableHtml5)
            {
                Server.Transfer("editor.aspx", true);
            }

            //check if cookie from this portal
            if (SecurityContext.CurrentAccount is IUserAccount &&
                ((IUserAccount) SecurityContext.CurrentAccount).Tenant != CoreContext.TenantManager.GetCurrentTenant().TenantId)
            {
                SecurityContext.Logout();
                Response.Redirect("~/");
            }

            var currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            if (currentUser == Constants.LostUser || currentUser.Status != EmployeeStatus.Active)
            {
                SecurityContext.Logout();
                Response.Redirect("~/");
            }

            ProcessSecureFilter();

            if (!SecurityContext.IsAuthenticated
                && DocumentUtils.ParseShareLink(Request[UrlConstant.DocUrlKey]) == null)
            {
                //for demo
                if (SetupInfo.WorkMode == WorkMode.Promo)
                {
                    if (AutoAuthByPromo())
                    {
                        UserOnlineManager.Instance.RegistryOnlineUser(SecurityContext.CurrentAccount.ID);

                        Response.Redirect("~/");
                        return;
                    }
                }

                var refererURL = Request.GetUrlRewriter().AbsoluteUri;
                if (!ValidateRefererUrl(refererURL))
                    refererURL = (string) Session["refererURL"];

                if (!AutoAuthByCookies() && !CoreContext.TenantManager.GetCurrentTenant().Public)
                {
                    Session["refererURL"] = refererURL;
                    Response.Redirect("~/auth.aspx");
                    return;
                }
            }

            if (SecurityContext.IsAuthenticated)
            {
                UserOnlineManager.Instance.RegistryOnlineUser(SecurityContext.CurrentAccount.ID);
            }

            CurrentSkin = WebSkin.GetUserSkin();
            Theme = CurrentSkin.ASPTheme;
        }

        protected override void OnLoad(EventArgs e)
        {
            PageLoad();
            InitScript();
        }

        protected void PageLoad()
        {
            var file = new File();
            try
            {
                ShareLink = Request[UrlConstant.DocUrlKey] ?? "";
                WithLink = !string.IsNullOrEmpty(ShareLink);

                var fileId = WithLink ? (object) -1 : Request[UrlConstant.FileId];
                FileNew = !string.IsNullOrEmpty(Request[UrlConstant.New]) && Request[UrlConstant.New] == "true";

                file = DocumentUtils.GetServiceParams(true, fileId, 0, FileNew, ShareLink, out DocParams);
            }
            catch (Exception ex)
            {
                Response.Redirect(PathProvider.BaseVirtualPath + "docviewer.aspx" + "?" + Request.QueryString + "#" + UrlConstant.Error + "/" + HttpUtility.UrlEncode(ex.Message));
            }

            if (!FileUtility.UsingHtml5(file.Title))
            {
                Server.Transfer("editor.aspx", true);
            }

            Title = file.Title;

            DocParams.Type = MobileDetector.IsRequestMatchesMobile(Context) ? "mobile" : "desktop";

            DocKeyForTrack = DocumentUtils.GetDocKey(file.ID, -1, DateTime.MinValue);

            using (var tagDao = Global.DaoFactory.GetTagDao())
            {
                tagDao.RemoveTags(Tag.New(SecurityContext.CurrentAccount.ID, file));
            }

            FilesActivityPublisher.OpenEditorFile(file);

            FileLocker.Add(file.ID);
        }

        private void InitScript()
        {
            Page.ClientScript.RegisterJavaScriptResource(typeof (FilesJSResource), "ASC.Files.FilesJSResources");

            var inlineScript = new StringBuilder();

#if (DEBUG)
            inlineScript.Append("<link href='" + PathProvider.GetFileStaticRelativePath("common.css") + "' type='text/css' rel='stylesheet' />");
#else
            inlineScript.Append("<link href="+PathProvider.GetFileStaticRelativePath("files-min.css")+" type='text/css' rel='stylesheet' />");
#endif
            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + WebPath.GetPath("js/auto/jquery_full.js") + "'></script>");

            inlineScript.Append("<script language='javascript' type='text/javascript'>");
            inlineScript.Append("var jq = jQuery.noConflict();");
            inlineScript.Append("var SkinManager = new function() { this.GetImage = function() { }; };");
            inlineScript.Append("jq.extend(jq.browser, {mobile : false});");
            inlineScript.Append("</script>");

            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + WebPath.GetPath("js/auto/common.js") + "'></script>");

            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + PathProvider.GetFileStaticRelativePath("common.js") + "'></script>");
            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + PathProvider.GetFileStaticRelativePath("servicemanager.js") + "'></script>");
            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + PathProvider.GetFileStaticRelativePath("ui.js") + "'></script>");

            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + CommonLinkUtility.DocServiceApiUrl + "'></script>");
            inlineScript.Append("<script language='javascript' type='text/javascript' src='" + PathProvider.GetFileStaticRelativePath("editor.js") + "'></script>");

            inlineScript.Append("<script language='javascript' type='text/javascript'>");

            inlineScript.AppendFormat(";\nASC.Files.Constants.URL_HANDLER_SAVE = \"{0}\";" +
                                      "ASC.Files.Constants.URL_HANDLER_CREATE = \"{1}\";",
                                      FileHandler.FileHandlerPath + UrlConstant.ParamsSave,
                                      CommonLinkUtility.GetFullAbsolutePath(FileHandler.FileHandlerPath)
                );

            inlineScript.AppendFormat("\nserviceManager.init(\"{0}\");", PathProvider.GetFileServicePath);

            inlineScript.AppendFormat("\nASC.Files.Editor.fileSaveAsNew = \"{0}\";" +
                                      "ASC.Files.Editor.docKeyForTrack = \"{1}\";" +
                                      "ASC.Files.Editor.ShareLink = \"{2}\";",
                                      FileNew ? "&" + UrlConstant.New + "=true" : "",
                                      DocKeyForTrack,
                                      WithLink ? "&" + UrlConstant.DocUrlKey + "=" + ShareLink : ""
                );

            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof (DocServiceParams));
                serializer.WriteObject(ms, DocParams);
                ms.Seek(0, SeekOrigin.Begin);
                inlineScript.AppendFormat("\nASC.Files.Editor.docServiceParams = {0};",
                                          Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int) ms.Length));
            }

            inlineScript.Append("</script>");

            inlineScript.Append(RenderCustomScript());
            inlineScript.Append(RenderPromoBar());

            aspHeaderContent.Controls.Add(new Literal {Text = inlineScript.ToString()});
        }

        protected string RenderCustomScript()
        {
            var sb = new StringBuilder();
            //custom scripts
            foreach (var script in SetupInfo.CustomScripts)
            {
                if (!String.IsNullOrEmpty(script))
                    sb.Append("<script language='javascript' type='text/javascript' src='" + script + "'></script>");
            }

            return sb.ToString();
        }

        protected string RenderPromoBar()
        {
            var notifyAddress = SetupInfo.NotifyAddress;
            var _notifyBarSettings = SettingsManager.Instance.LoadSettings<StudioNotifyBarSettings>(TenantProvider.CurrentTenantID);
            if (_notifyBarSettings.ShowPromotions && !string.IsNullOrEmpty(notifyAddress) && SecurityContext.IsAuthenticated)
            {
                var userId = SecurityContext.CurrentAccount.ID;
                var page = Request.GetUrlRewriter().PathAndQuery;
                var language = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant();

                return new StringBuilder().
                    AppendFormat("<script language='javascript' type='text/javascript'>jq(function(){{ jq.getScript('{0}/promotion/get?userId={1}&page={2}&language={3}&admin={4}'); }});</script>"
                                 , notifyAddress, userId, page, language, Global.IsAdministrator).ToString();
            }
            return string.Empty;
        }
    }
}