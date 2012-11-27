using System;
using System.Web.UI;

namespace ASC.Web.Controls
{
    [ToolboxData("<{0}:TreeViewScriptManager runat=server></{0}:TreeViewScriptManager>")]
    public class TreeViewProScriptManager : System.Web.UI.Control
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.ClientScript.RegisterClientScriptInclude("treeviewpro_script", Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.TreeViewPro.js.treeviewprototype.js"));

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "treeviewpro_style",
                                                       "<link href=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.TreeViewPro.css.style.css") + "\" type=\"text/css\" rel=\"stylesheet\"/>", false);

        }
    }
}
