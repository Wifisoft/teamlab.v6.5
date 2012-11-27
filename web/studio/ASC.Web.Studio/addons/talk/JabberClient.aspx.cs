using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Controls.FileUploader.HttpModule;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;
using ASC.Xmpp.Common;

namespace ASC.Web.Talk
{
    public class UploadFileHanler : FileUploadHandler
    {
        private String GetJabberAccount()
        {
            try
            {
                return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLower() + "@" +
                       CoreContext.TenantManager.GetCurrentTenant().TenantDomain;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private String MD5Hash(String input)
        {
            var data = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
            var sBuilder = new StringBuilder();

            for (Int32 i = 0, n = data.Length; i < n; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            try
            {
                if (context.Request.Files.Count == 0)
                {
                    throw new Exception("there is no file");
                }

                var file = context.Request.Files[0];

                if (file.ContentLength > SetupInfo.MaxImageUploadSize)
                {
                    throw FileSizeComment.FileImageSizeException;
                }

                var UserId = context.Request["ownjid"];
                var FileName = file.FileName.Replace("~", "-");
                var FileURL = String.Empty;
                var FileSize = file.InputStream.Length;
                //if (String.IsNullOrEmpty(UserId))
                //{
                //    throw new Exception("there is no userId");
                //}
                var FilePath = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "talk").Save(Path.Combine(MD5Hash(UserId), FileName), file.InputStream);
                FileURL = FilePath.ToString();
                FileName = FileURL.Substring(FileURL.LastIndexOf("/") + 1);
                return new FileUploadResult
                           {
                               FileName = FileName,
                               Data = FileSizeComment.FilesSizeToString(FileSize),
                               FileURL = CommonLinkUtility.GetFullAbsolutePath(FileURL),
                               Success = true
                           };
            }
            catch (Exception ex)
            {
                return new FileUploadResult()
                           {
                               Success = false,
                               Message = ex.Message
                           };
            }
        }
    }

    [AjaxPro.AjaxNamespace("JabberClient")]
    public partial class JabberClient : MainPage
    {
        private string JabberResource
        {
            get { return "TMTalk"; }
        }

        private TalkConfiguration cfg;

        private String EscapeJsString(String s)
        {
            return s.Replace("'", "\\'");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cfg = new TalkConfiguration();

            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            ((IStudioMaster) this.Master).DisabledSidePanel = true;

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "gears.init", WebPath.GetPath("addons/talk/js/gears.init.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "iscroll", WebPath.GetPath("addons/talk/js/iscroll.js"));
            if (cfg.EnabledFirebugLite)
            {
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "firebug.lite", "https://getfirebug.com/firebug-lite.js");
            }

            var culture = CultureInfo.CurrentCulture;

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "talk.style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("addons/talk/css/<theme_folder>/talk.style.css") + "\" />", false);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "talk.location-style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("addons/talk/css/<theme_folder>/talk.style" + "." + culture.Name.ToLower() + ".css") + "\" />", false);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "talk.text-overflow", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + VirtualPathUtility.ToAbsolute("~/addons/talk/css/" + WebSkin.DefaultSkin.FolderName + "/talk.text-overflow.css") + "\" />", false);

            StringBuilder jsResources = new StringBuilder();
            jsResources.Append("window.ASC=window.ASC||{};");
            jsResources.Append("window.ASC.TMTalk=window.ASC.TMTalk||{};");
            jsResources.Append("window.ASC.TMTalk.Resources={};");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles={}" + ';');
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.offline='" + EscapeJsString(Talk.Resources.TalkResource.StatusOffline) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.online='" + EscapeJsString(Talk.Resources.TalkResource.StatusOnline) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.away='" + EscapeJsString(Talk.Resources.TalkResource.StatusAway) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.xa='" + EscapeJsString(Talk.Resources.TalkResource.StatusNA) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("product_logo.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon16='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk16.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon32='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk32.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon48='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk48.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon128='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk128.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.iconNewMessage='" + EscapeJsString(WebSkin.GetUserSkin().GetAbsoluteWebPath("addons/talk/css/<theme_folder>/imagescss/icon-new-message.ico")) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.productName='" + EscapeJsString(Talk.Resources.TalkResource.ProductName) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.updateFlashPlayerUrl='" + EscapeJsString(Talk.Resources.TalkResource.UpdateFlashPlayerUrl) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.selectUserBookmarkTitle='" + EscapeJsString(Talk.Resources.TalkResource.SelectUserBookmarkTitle) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.defaultConferenceSubjectTemplate='" + EscapeJsString(Talk.Resources.TalkResource.DefaultConferenceSubjectTemplate) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.labelNewMessage='" + EscapeJsString(Talk.Resources.TalkResource.LabelNewMessage) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.labelRecvInvite='" + EscapeJsString(Talk.Resources.TalkResource.LabelRecvInvite) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.titleRecvInvite='" + EscapeJsString(Talk.Resources.TalkResource.TitleRecvInvite) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintChangeState='" + EscapeJsString(Talk.Resources.TalkResource.HintChangeState) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintClientConnecting='" + EscapeJsString(Talk.Resources.TalkResource.HintClientConnecting) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintClientDisconnected='" + EscapeJsString(Talk.Resources.TalkResource.HintClientDisconnected) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintCtrlEnterSender='" + EscapeJsString(Talk.Resources.TalkResource.HintCtrlEnterSender) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintDisableSounds='" + EscapeJsString(Talk.Resources.TalkResource.HintDisableSounds) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintEmotions='" + EscapeJsString(Talk.Resources.TalkResource.HintEmotions) + "',");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintEnableSounds='" + EscapeJsString(Talk.Resources.TalkResource.HintEnableSounds) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintEnterSender='" + EscapeJsString(Talk.Resources.TalkResource.HintEnterSender) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintFlastPlayerIncorrect='" + EscapeJsString(Talk.Resources.TalkResource.HintFlastPlayerIncorrect) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintGroups='" + EscapeJsString(Talk.Resources.TalkResource.HintGroups) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintHideGroups='" + EscapeJsString(Talk.Resources.TalkResource.HintHideGroups) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintHideOfflineContacts='" + EscapeJsString(Talk.Resources.TalkResource.HintHideOfflineContacts) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintNoFlashPlayer='" + EscapeJsString(Talk.Resources.TalkResource.HintNoFlashPlayer) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintOfflineContacts='" + EscapeJsString(Talk.Resources.TalkResource.HintOfflineContacts) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintSendAddition='" + EscapeJsString(Talk.Resources.TalkResource.HintSendAddition) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSendFile='" + EscapeJsString(Talk.Resources.TalkResource.HintSendFile) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintSendMessage='" + EscapeJsString(Talk.Resources.TalkResource.HintSendMessage) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintShowGroups='" + EscapeJsString(Talk.Resources.TalkResource.HintShowGroups) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintShowLeftTab='" + EscapeJsString(Talk.Resources.TalkResource.HintShowLeftTab) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintShowOfflineContacts='" + EscapeJsString(Talk.Resources.TalkResource.HintShowOfflineContacts) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintShowRightTab='" + EscapeJsString(Talk.Resources.TalkResource.HintShowRightTab) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSounds='" + EscapeJsString(Talk.Resources.TalkResource.HintSounds) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.hintToggleSender='" + EscapeJsString(Talk.Resources.TalkResource.HintToggleSender) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintUpdateHrefText='" + EscapeJsString(Talk.Resources.TalkResource.HintUpdateHrefText) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSelectContact='" + EscapeJsString(Talk.Resources.TalkResource.HintSelectContact) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSendInvite='" + EscapeJsString(Talk.Resources.TalkResource.HintSendInvite) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintPossibleClientConflict='" + EscapeJsString(Talk.Resources.TalkResource.HintPossibleClientConflict) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintCreateShortcutDialog='" + EscapeJsString(Talk.Resources.TalkResource.HintCreateShortcutDialog) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.sendFileMessage='" + EscapeJsString(string.Format(Talk.Resources.TalkResource.SendFileMessage, "{0}<br/>", "{1}")) + "';");
            // jsResources.Append("window.ASC.TMTalk.Resources.noneGroupTitle='" + EscapeJsString(Talk.Resources.TalkResource.NoneGroupTitle) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.mailingsGroupName='" + EscapeJsString(Talk.Resources.TalkResource.MailingsGroupName) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.conferenceGroupName='" + EscapeJsString(Talk.Resources.TalkResource.ConferenceGroupName) + "';");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "talk.resources", jsResources.ToString(), true);

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.customevents", WebPath.GetPath("addons/talk/js/talk.customevents.js"));

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.common", WebPath.GetPath("addons/talk/js/talk.common.js"));

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.navigationitem", WebPath.GetPath("addons/talk/js/talk.navigationitem.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.msmanager", WebPath.GetPath("addons/talk/js/talk.msmanager.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.mucmanager", WebPath.GetPath("addons/talk/js/talk.mucmanager.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.roomsmanager", WebPath.GetPath("addons/talk/js/talk.roomsmanager.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.contactsmanager", WebPath.GetPath("addons/talk/js/talk.contactsmanager.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.messagesmanager", WebPath.GetPath("addons/talk/js/talk.messagesmanager.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.connectionmanager", WebPath.GetPath("addons/talk/js/talk.connectiomanager.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.default", WebPath.GetPath("addons/talk/js/talk.default.js"));

            switch (cfg.RequestTransportType.ToLower())
            {
                case "flash":
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.checkplayer", VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/checkplayer.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.flensed", VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/flensed.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.flxhr", VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/flxhr.js"));

                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.base64", WebPath.GetPath("addons/talk/js/jlib/strophe/base64.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.md5", WebPath.GetPath("addons/talk/js/jlib/strophe/md5.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.core", WebPath.GetPath("addons/talk/js/jlib/strophe/core.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.strophe.flxhr", WebPath.GetPath("addons/talk/js/jlib/plugins/strophe.flxhr.js"));

                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.swfobject", WebPath.GetPath("addons/talk/js/jlib/flxhr/swfobject.js"));
                    break;
                default:
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.base64", WebPath.GetPath("addons/talk/js/jlib/strophe/base64.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.md5", WebPath.GetPath("addons/talk/js/jlib/strophe/md5.js"));
                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.core", WebPath.GetPath("addons/talk/js/jlib/strophe/core.js"));

                    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.lib.swfobject", WebPath.GetPath("addons/talk/js/jlib/flxhr/swfobject.js"));
                    break;
            }

            try
            {
                Page.Title = Resources.TalkResource.ProductName + " - " + CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).DisplayUserName();
            }
            catch (System.Security.SecurityException)
            {
                Page.Title = Resources.TalkResource.ProductName + " - " + HeaderStringHelper.GetPageTitle(Resources.TalkResource.DefaultContactTitle, null, null);
            }
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite), ASC.Web.Core.Security.Ajax.AjaxSecurityPassthrough]
        public string GetAuthToken()
        {
            try
            {
                return new JabberServiceClient().GetAuthToken(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            catch
            {
                return String.Empty;
            }
        }

        //[AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        //public String SendTestMessage(String from, String to, String command)
        //{
        //  try
        //  {
        //    new ASC.Xmpp.Common.JabberServiceClient().SendCommand(from, to, command);
        //  }
        //  catch (Exception ex)
        //  {
        //    return ex.Message;
        //  }
        //  return "success";
        //}

        protected String GetBoshUri()
        {
            return cfg.BoshUri;
        }

        protected String GetResourcePriority()
        {
            return cfg.ResourcePriority;
        }

        protected String GetInactivity()
        {
            return cfg.ClientInactivity;
        }

        protected String GetFileTransportType()
        {
            return cfg.FileTransportType ?? String.Empty;
        }

        protected String GetRequestTransportType()
        {
            return cfg.RequestTransportType ?? String.Empty;
        }

        protected String GetMassendState()
        {
            return cfg.EnabledMassend.ToString().ToLower() ?? String.Empty;
        }

        protected String GetConferenceState()
        {
            return cfg.EnabledConferences.ToString().ToLower() ?? String.Empty;
        }

        protected String GetMonthNames()
        {
            return String.Join(",", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames);
        }

        protected String GetHistoryLength()
        {
            return cfg.HistoryLength ?? String.Empty;
        }

        protected String GetValidSymbols()
        {
            return cfg.ValidSymbols ?? String.Empty;
        }

        protected String GetFullDateFormat()
        {
            return Resources.TalkResource.FullDateFormat;
            //return GetShortDateFormat() + " " + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        protected String GetShortDateFormat()
        {
            return Resources.TalkResource.ShortDateFormat;
            //return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
        }

        protected String GetUserPhotoHandlerPath()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/userphoto.ashx");
        }

        protected String GetNotificationHandlerPath()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/notification.html");
        }

        protected String GetJabberAccount()
        {
            try
            {
                return EscapeJsString(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLower()) + "@" +
                       CoreContext.TenantManager.GetCurrentTenant().TenantDomain + "/" + JabberResource;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}