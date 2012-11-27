using System;
using System.Web;

namespace ASC.Web.Core.ModuleManagement
{
    public class Widget : NavigationWebItem
    {
        public virtual Type WidgetType
        {
            get;
            set;
        }

        public virtual Type SettingsType
        {
            get;
            set;
        }
    }
}
