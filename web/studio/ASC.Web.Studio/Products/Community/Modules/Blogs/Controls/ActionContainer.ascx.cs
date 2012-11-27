using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Blogs.Controls
{
    public partial class ActionContainer : BaseUserControl
    {
        public PlaceHolder ActionsPlaceHolder { get; set; }

        public ActionContainer()
        {
            ActionsPlaceHolder = new PlaceHolder();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentModule = UserOnlineManager.Instance.GetCurrentModule() as Module;
            if (currentModule == null) return;
            if (currentModule.Actions.Any() || ActionsPlaceHolder.Controls.Count > 0)
            {
                var actionsControl = new SideActions();
                foreach (var shortcut in currentModule.Actions)
                {
                    actionsControl.Controls.Add(new NavigationItem()
                    {
                        Name = shortcut.Name,
                        Description = shortcut.Description,
                        URL = shortcut.StartURL,
                        IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                    });
                }
                _actionHolder.Controls.Add(actionsControl);

                if (ActionsPlaceHolder.Controls.Count > 0)
                    actionsControl.Controls.Add(ActionsPlaceHolder);
            }

            if (currentModule.Navigations.Any())
            {
                var navigationControl = new SideNavigator();
                foreach (var shortcut in currentModule.Navigations)
                {
                    navigationControl.Controls.Add(new NavigationItem()
                    {
                        Name = shortcut.Name,
                        Description = shortcut.Description,
                        URL = shortcut.StartURL,

                    });
                }

                _actionHolder.Controls.Add(navigationControl);

            }

        }
    }
}