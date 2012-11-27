using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Xmpp.Common;

namespace ASC.Web.Talk.Addon
{
    [WebZoneAttribute(WebZoneType.CustomProductList)]
    public class TalkAddon : IAddon, IRenderCustomNavigation
    {
        public static Guid AddonID { get { return new Guid("{BF88953E-3C43-4850-A3FB-B1E43AD53A3E}"); } }

        public string Name
        {
            get { return Resources.TalkResource.ProductName; }
        }

        public string Description
        {
            get { return Resources.TalkResource.TalkDescription; }
        }

        public Guid ID
        {
            get { return AddonID; }
        }

        public AddonContext Context
        {
            get;
            private set;
        }

        WebItemContext IWebItem.Context
        {
            get { return Context; }
        }


        public void Init(AddonContext context)
        {
            Context = context;
            Context.ThemesFolderVirtualPath = "~/addons/talk/App_Themes";
            Context.ImageFolder = "images";
            Context.DisabledIconFileName = "product_logo_disabled.png";
            Context.IconFileName = "product_logo.png";
            Context.LargeIconFileName = "product_logolarge.png";
            Context.DefaultSortOrder = 60;
        }

        public void Shutdown()
        {

        }

        public string StartURL
        {
            get { return BaseVirtualPath + "/default.aspx"; }
        }

        public const string BaseVirtualPath = "~/addons/talk";

        public static string GetClientUrl()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/jabberclient.aspx");
        }

        public static string GetTalkClientURL()
        {
            return "javascript:window.ASC.Controls.JabberClient.open('" + VirtualPathUtility.ToAbsolute("~/addons/talk/jabberclient.aspx") + "')";
        }

        public static int GetMessageCount(string username)
        {
            try
            {
                return new JabberServiceClient().GetNewMessagesCount(username, CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            catch { }
            return 0;
        }

        public static string GetMessageStr()
        {
            return Resources.TalkResource.Chat;
        }

        public System.Web.UI.Control LoadCustomNavigationControl(System.Web.UI.Page page)
        {
            return null;
        }

        public string RenderCustomNavigation(Page page)
        {
            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                using (var hw = new HtmlTextWriter(tw))
                {
                    var ctrl = page.LoadControl(UserControls.TalkNavigationItem.Location);
                    ctrl.RenderControl(hw);
                    return sb.ToString();
                }
            }
        }
    }
}
