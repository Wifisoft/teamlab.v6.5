using System;
using System.Web.UI;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.Controls.Common
{
    /// <summary>
    /// Side action panel
    /// </summary>
    [ToolboxData("<{0}:SideActions runat=server></{0}:SideActions>")]
    [PersistChildren(false)]
    public class SideActions : SideContainer
    {
        public SideActions()
        {
            HeaderCSSClass = "studioSideBoxActionsHeader";
            BodyCSSClass = "studioSideBoxNavigationBody";
            ImageURL = WebImageSupplier.GetAbsoluteWebPath("actions.png");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Title = (!String.IsNullOrEmpty(Title) ? Title : Resources.Resource.Actions);
        }
    }
}