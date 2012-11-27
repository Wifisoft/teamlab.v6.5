using System;
using System.Web.UI;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.Controls.Common
{
    /// <summary>
    /// Side navigation panel
    /// </summary>
    [ToolboxData("<{0}:SideNavigator runat=server></{0}:SideNavigator>")]
    public class SideNavigator : SideActions
    {
        public SideNavigator()
        {
            HeaderCSSClass = "studioSideBoxNavigationHeader";
            ImageURL = WebImageSupplier.GetAbsoluteWebPath("navigation.png");
        }

        protected override void OnPreRender(EventArgs e)
        {
            Title = (!String.IsNullOrEmpty(Title) ? Title : Resources.Resource.Navigation);
            base.OnPreRender(e);
        }
    }
}