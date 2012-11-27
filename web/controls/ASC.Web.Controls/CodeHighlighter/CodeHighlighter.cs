using System;
using System.ComponentModel;
using System.Web.UI;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [ToolboxData("<{0}:CodeHighlighter runat=server></{0}:CodeHighlighter>")]
    public class CodeHighlighter : System.Web.UI.Control
    {  
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string cssFileName = "default.css";
            switch (this.HighlightStyle)
            {
                case HighlightStyle.Ascetic:
                    cssFileName = "ascetic.css";
                    break;
                case HighlightStyle.Dark:
                    cssFileName = "dark.css";
                    break;
                case HighlightStyle.Default:
                    cssFileName = "default.css";
                    break;
                case HighlightStyle.Far:
                    cssFileName = "far.css";
                    break;
                case HighlightStyle.Idea:
                    cssFileName = "idea.css";
                    break;
                case HighlightStyle.Magula:
                    cssFileName = "magula.css";
                    break;
                case HighlightStyle.Sunburst:
                    cssFileName = "sunburst.css";
                    break;

                case HighlightStyle.VS:
                    cssFileName = "vs.css";
                    break;

                case HighlightStyle.Zenburn:
                    cssFileName = "zenburn.css";
                    break;
            }

            Page.ClientScript.RegisterClientScriptInclude("codeHighlighter_jsparser", Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.CodeHighlighter.js.highlight.pack.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "codeHighlighter_jsparser_init", "hljs.initHighlightingOnLoad();", true);

            
            
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "codeHighlighter_style", 
                                                       "<link href=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.CodeHighlighter.css."+cssFileName) + "\" type=\"text/css\" rel=\"stylesheet\"/>", false);

        }

        public static string GetJavaScriptLiveHighlight()
        {
            return GetJavaScriptLiveHighlight(false);
        }

        public static string GetJavaScriptLiveHighlight(bool addScriptTags)
        {
            if (addScriptTags)
                return "<script language='javascript' type='text/javascript'>jq('code').each(function(){ hljs.highlightBlock(jq(this).get(0));}); </script>";

            return "jq('code').each(function(){ hljs.highlightBlock(jq(this).get(0));});";
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(HighlightStyle), "VS")]
        [Localizable(true)]
        public HighlightStyle HighlightStyle
        {
            get
            {
                object hs = ViewState["HighlightStyle"];
                return (hs == null ? HighlightStyle.VS : (HighlightStyle)hs);
            }
            set
            {
                ViewState["HighlightStyle"] = value;
            }
        }
    }
}
