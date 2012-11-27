using System.Runtime.Remoting.Messaging;
using System.Web.UI;
using ASC.Common.Security;
using System.Web.Configuration;
using ASC.Web.CRM.Classes;

namespace ASC.Web.CRM
{
    public abstract class BaseUserControl : UserControl
    {

        public new BasePage Page
        {
            get { return base.Page as BasePage; }
        }
    }
}