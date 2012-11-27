using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [ParseChildren(true, "CodeText")]
    [PersistChildren(false)]
    [DefaultProperty("CodeText")]
    [ToolboxData("<{0}:LanguageHighlightItem runat=server></{0}:LanguageHighlightItem>")]
    public class LanguageHighlightItem : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string CodeText
        {
            get
            {
                String s = (String)ViewState["CodeText"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["CodeText"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(CodeLanguage),"Any")]
        [Localizable(true)]
        public CodeLanguage CodeLanguage 
        {
            get 
            {
                object code_lng = ViewState["CodeLanguage"];
                return (code_lng == null ? CodeLanguage.Any : (CodeLanguage)code_lng);
            }
            set 
            {
                ViewState["CodeLanguage"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
           
            switch (this.CodeLanguage)
            {
                case CodeLanguage.Any:
                    output.Write("<pre><code>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.CPP:
                    output.Write("<pre><code class='cpp'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.CS:
                    output.Write("<pre><code class='cs'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.CSS:
                    output.Write("<pre><code class='css'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.Delphi:
                    output.Write("<pre><code class='delphi'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.DOS:
                    output.Write("<pre><code class='doc'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.HtmlXml:
                    output.Write("<pre><code class='html-xml'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.Java:
                    output.Write("<pre><code class='java'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.Javascript:
                    output.Write("<pre><code class='javascript'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.SQL:
                    output.Write("<pre><code class='sql'>" + this.CodeText + "</code></pre>");
                    break;

                case CodeLanguage.VBScript:
                    output.Write("<pre><code class='vbscript'>" + this.CodeText + "</code></pre>");
                    break;

            }
        }
    }
}
