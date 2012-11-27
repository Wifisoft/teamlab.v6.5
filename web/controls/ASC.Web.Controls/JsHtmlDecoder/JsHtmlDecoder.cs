using System;
using System.ComponentModel;
using System.Web.UI;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [ToolboxData("<{0}:JsHtmlDecoder runat=server></{0}:JsHtmlDecoder>")]
    public class JsHtmlDecoder : System.Web.UI.Control
    {  
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.ClientScript.RegisterClientScriptInclude("JsHtmlDecoder_script", Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.JsHtmlDecoder.js.decoder.js"));            
        }
    }
}
