using System;
using System.Web;
using ASC.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;

namespace ASC.Web.Talk.UserControls
{
    [AjaxPro.AjaxNamespace("TalkProvider")]
    public partial class TalkNavigationItem : System.Web.UI.UserControl
    {
        private String EscapeJsString(String s)
        {
            return s.Replace("'", "\\'");
        }

        public static string Location
        {
            get { return TalkAddon.BaseVirtualPath + "/UserControls/TalkNavigationItem.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
        }

        protected string GetTalkClientURL()
        {
            return TalkAddon.GetTalkClientURL();
        }

        protected string GetMessageStr()
        {
            return TalkAddon.GetMessageStr();
        }

        protected string GetOpenContactHandler()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/opencontact.ashx");
        }

        protected string GetJabberClientPath()
        {
            return TalkAddon.GetClientUrl();
        }

        protected string GetUnreadMessagesHandler()
        {
            var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            return VirtualPathUtility.ToAbsolute("~/addons/talk/unreadmessages.ashx") +
                "?u=" + (string.IsNullOrEmpty(user.UserName) ? string.Empty : user.UserName.ToLower());
        }

        protected string GetUserName()
        {
            try
            {
                return EscapeJsString(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLower());
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        protected string GetUpdateInterval()
        {
            return new TalkConfiguration().UpdateInterval;
        }
    }
}
