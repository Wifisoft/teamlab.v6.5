using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Data.Storage;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    [AjaxNamespace("AccountLinkControl")]
    public partial class AccountLinkControl : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Users/UserProfile/AccountLinkControl.ascx"; } }
        public bool SettingsView { get; set; }

        protected ICollection<AccountInfo> Infos = new List<AccountInfo>();

        public AccountLinkControl()
        {
            ClientCallback = "loginCallback";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "accountlink_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/userprofile/css/<theme_folder>/accountlink_style.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "accountlink_script", WebPath.GetPath("usercontrols/users/userprofile/js/accountlinker.js"));
            InitProviders();
        }


        public string ClientCallback { get; set; }

        private void InitProviders()
        {
            IEnumerable<LoginProfile> linkedAccounts = new List<LoginProfile>();
            if (SecurityContext.IsAuthenticated)
            {
                linkedAccounts = GetLinker().GetLinkedProfiles(SecurityContext.CurrentAccount.ID.ToString());
            }
            AddProvider(ProviderConstants.OpenId, "https://www.google.com/accounts/o8/id", linkedAccounts);
            AddProvider(ProviderConstants.Facebook, linkedAccounts);
            AddProvider(ProviderConstants.Twitter, linkedAccounts);
            AddProvider(ProviderConstants.LinkedIn, linkedAccounts);
        }

        private void AddProvider(string provider, IEnumerable<LoginProfile> linkedAccounts)
        {
            AddProvider(provider, null, linkedAccounts);
        }

        private void AddProvider(string provider, string oid, IEnumerable<LoginProfile> linkedAccounts)
        {
            Infos.Add(new AccountInfo()
            {
                Linked = linkedAccounts.Any(x => x.Provider == provider),
                Provider = provider,
                Url = VirtualPathUtility.ToAbsolute("~/login.ashx") + string.Format("?auth={0}&mode=popup&callback={1}"
                                                                    + (string.IsNullOrEmpty(oid) ? "" : ("&oid=" + HttpUtility.UrlEncode(oid))), provider, ClientCallback)
            });
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse LinkAccount(string serializedProfile)
        {
            //Link it
            var profile = new LoginProfile(serializedProfile);
            GetLinker().AddLink(SecurityContext.CurrentAccount.ID.ToString(), profile);
            return RenderControlHtml();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UnlinkAccount(string provider)
        {
            //Link it
            GetLinker().RemoveProvider(SecurityContext.CurrentAccount.ID.ToString(), provider);
            return RenderControlHtml();
        }

        private AjaxResponse RenderControlHtml()
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new HtmlTextWriter(stringWriter))
                {
                    var ctrl = (AccountLinkControl)LoadControl(Location);
                    ctrl.SettingsView = true;
                    ctrl.InitProviders();
                    ctrl.RenderControl(writer);
                    return new AjaxResponse() { rs1 = stringWriter.GetStringBuilder().ToString() };
                }
            }
        }

        public AccountLinker GetLinker()
        {
            return new AccountLinker(WebConfigurationManager.ConnectionStrings["webstudio"]);
        }
    }

    public class AccountInfo
    {
        public string Provider { get; set; }
        public string Url { get; set; }
        public bool Linked { get; set; }
        public string Class { get; set; }
    }

}