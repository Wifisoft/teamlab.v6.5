using System.Runtime.Remoting.Messaging;
using System.Web.UI;
using ASC.Common.Security;
using System.Web.Configuration;

namespace ASC.Web.Projects
{
    public abstract class BaseUserControl : UserControl
    {
        protected BaseUserControl()
        {
            if (CallContext.GetData("CURRENT_ACCOUNT") == null && ASC.Core.SecurityContext.IsAuthenticated)
                CallContext.SetData("CURRENT_ACCOUNT", ASC.Core.SecurityContext.CurrentAccount.ID);
        }
        
        public bool IsOldFile { get { return WebConfigurationManager.AppSettings["projects.oldfiles"] == "true"; } }

        public new BasePage Page
        {
            get { return base.Page as BasePage; }
        }
    }
}