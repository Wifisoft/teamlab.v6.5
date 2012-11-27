using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:PollFormMaster runat=server></{0}:PollFormMaster>")]
    public class PollFormMaster : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Name
        {
            get
            {
                String s = (String)ViewState["Name"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Name"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        public bool Singleton
        {
            get
            {
                object o = ViewState["Singleton"];
                return ((o == null) ? false : (bool)o);
            }

            set
            {
                ViewState["Singleton"] = value;
            }
        }
       
        [Bindable(true)]
        [Category("Appearance")]
        public List<AnswerViarint> AnswerVariants
        {
            get
            {
                object o = ViewState["AnswerVariants"];
                return ((o == null) ? null : (List<AnswerViarint>)o);
            }

            set
            {
                ViewState["AnswerVariants"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string VariantPanelCSSClass
        {
            get
            {
                string s = (string)ViewState["VariantPanelCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["VariantPanelCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string VariantLabelCSSClass
        {
            get
            {
                string s = (string)ViewState["VariantLabelCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["VariantLabelCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string HeaderCSSClass
        {
            get
            {
                string s = (string)ViewState["HeaderCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["HeaderCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextEditCSSClass
        {
            get
            {
                string s = (string)ViewState["TextEditCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["TextEditCSSClass"] = value;
            }
        }

        public string QuestionFieldID { get; set; }

       
        public string PollID { get; set; }

        private string _uniqueID;

        public PollFormMaster()
        {
            this.AnswerVariants = new List<AnswerViarint>();
            this.Singleton = true;
            _uniqueID = Guid.NewGuid().ToString().Replace('-', '_');
        }

        protected override void OnInit(EventArgs e)
        {
            string scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.PollForm.js.pollform.js");
            Page.ClientScript.RegisterClientScriptInclude("pollform_script", scriptLocation);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "pollform_style",
                                                       "<link href=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.PollForm.css.style.css") + "\" type=\"text/css\" rel=\"stylesheet\"/>", false);

            InitProperties();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            InitProperties();
        }

        private void InitProperties()
        {
            if (Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(Page.Request["poll_question"]))
                    Name = Page.Request["poll_question"];

                try
                {
                    this.Singleton = (Convert.ToInt32(Page.Request["questiontype"]) == 1);
                }
                catch
                {
                    this.Singleton = true;
                }

                this.AnswerVariants.Clear();
                for (int i = 1; i < 20; i++)
                {
                    if (!String.IsNullOrEmpty(Page.Request["q" + i.ToString()]))
                    {
                        string id = Page.Request["qid_" + i.ToString()] ?? "";
                        this.AnswerVariants.Add(new AnswerViarint() { ID = id, Name = Page.Request["q" + i.ToString()] });
                    }
                }
            }
        }        

        protected override void RenderContents(HtmlTextWriter output)
        {

            var labelCSSClass = String.IsNullOrEmpty(VariantLabelCSSClass) ? "poll_variantLabel" : VariantLabelCSSClass;
            var divCSSClass = String.IsNullOrEmpty(VariantPanelCSSClass) ? "poll_variantDiv" : VariantPanelCSSClass;
            var headerCSSClass = String.IsNullOrEmpty(HeaderCSSClass) ? "poll_header" : HeaderCSSClass;
            var textEditCSSClass = String.IsNullOrEmpty(TextEditCSSClass) ? "poll_textEdit" : TextEditCSSClass;

            var qInputID = String.IsNullOrEmpty(QuestionFieldID) ? ("__poll_" + _uniqueID + "_question") : QuestionFieldID;

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"requiredField \" style=\"padding-top:3px; margin-bottom:20px;\">");
            sb.Append("<span class=\"requiredErrorText\">" + Resources.PollForm.EmptyQuestion + "</span>");
            sb.Append("<input name=\"" + this.UniqueID + "\" type='hidden' value=''/>");

            sb.Append("<div class=\"" + headerCSSClass + "\" style='margin-top:0px;'>");
            sb.Append(Resources.PollForm.PollQuestion);
            sb.Append("</div>");

            
            sb.Append("<input class=\"" + textEditCSSClass + "\" style=\"width:100%;\" maxlength=\"100\" name=\"poll_question\" id=\"" + qInputID + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Name) + "\" />");
            sb.Append("</div>");

            sb.Append("<div class=\"requiredField \">");
            sb.Append("<div class=\"" + headerCSSClass + "\" style='margin-top:0px;'>");
            sb.Append(Resources.PollForm.AnswerVariants);
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("<div id=\"__poll_" + _uniqueID + "_qbox\">");

            int i = 1;
            var leftMargin = 10;
            foreach (var answerVariant in this.AnswerVariants)
            {
                sb.Append("<div style='clear:left;' class=\"" + divCSSClass + " poll_clearFix\">");
                sb.Append("<label for=\"__poll_" + _uniqueID + "_q1\" style='float:left;' class=\"" + labelCSSClass + "\">" + Resources.PollForm.Variant + " " + i.ToString() + ":</label>");
                if (this.AnswerVariants.Count < 3)
                {
                    sb.Append("<span style='display:none;' class='poll_remove_span_"+i+"' onclick='PollMaster.RemoveAnswerVariant(this)'></span>");
                }
                else
                {
                    sb.Append("<span class='poll_remove_span_"+i+"' onclick='PollMaster.RemoveAnswerVariant(this)'></span>");
                }
                if (i > 9) leftMargin = 3;
                sb.Append("<input id=\"__poll_" + _uniqueID + "_q" + i + "\" name=\"q" + i + "\" maxlength=\"100\" value=\"" + HttpUtility.HtmlEncode(answerVariant.Name) + "\" type=\"text\" class=\"" + textEditCSSClass + "\" style=\"margin:2px 0 0 "+leftMargin+"px;  float:left; width:87%\"/>");
                sb.Append("<input id=\"__poll_" + _uniqueID + "_qid" + i + "\" name=\"qid_" + i + "\" value=\"" + (answerVariant.ID??"") + "\" type=\"hidden\"/>");
                sb.Append("</div>");
                i++;
            }

            if (i <= 2)
            {
                for (int j = i; j <= 2; j++)                
                {
                    sb.AppendFormat("<div class=\"" + divCSSClass + " poll_clearFix\">"
                        + "<label for=\"__poll_{1}_q{0}\" style='float:left;' class=\"" + labelCSSClass + "\">" + Resources.PollForm.Variant + " {0}:</label><span style='display:none;' class='poll_remove_span_"+j+"' onclick='PollMaster.RemoveAnswerVariant(this)'></span><input id=\"__poll_{1}_q{0}\" name=\"q{0}\" maxlength=\"100\" type=\"text\" class=\"" + textEditCSSClass + "\" style=\"margin:2px 0 0 10px; float:left; width:87%\"/>"
                        + "<input id=\"__poll_" + _uniqueID + "_qid" + j + "\" name=\"qid_" + j + "\" value=\"\" type=\"hidden\"/>"
                        +" </div>", j, _uniqueID);
                }

                i = 2;
            }
            sb.Append("</div>");

            sb.AppendFormat(@"<input type=""hidden"" id=""__poll_{1}_variantCaption"" value=""{0}""/>", Resources.PollForm.Variant, _uniqueID);

            sb.Append("<div class=\"poll_clearFix\" style='padding: 10px 0px 20px 70px;'>");
            sb.Append("<div id=\"__poll_" + _uniqueID + "_addButton\" style=\"margin-right:10px; float:left; " + ((i > 15) ? "display:none;" : "") + " \"><a href='#' onclick=\"PollMaster.AddAnswerVariant('" + _uniqueID + "','" + divCSSClass + "','"+labelCSSClass+"','"+textEditCSSClass+"');\">" + Resources.PollForm.AddAnswerVariantButton + "</a></div>");
            sb.Append("</div>");

            string multipleState = !Singleton? "checked=\"checked\"" : "";
            string singleState = Singleton? "checked=\"checked\"" : "";

            sb.Append("<div class=\"" + headerCSSClass + "\">");
            sb.Append(Resources.PollForm.PollType);
            sb.Append("</div>");

            sb.Append("<div style=\"padding-bottom:8px;\">");            
            sb.Append("<input style=\"vertical-align:middle; margin:0px 8px 0px 0px;\" type=\"radio\" " +
                  singleState + " id=\"__poll_" + _uniqueID + "_qt1\" value=\"1\" name=\"questiontype\"/>");
            sb.Append("<label for=\"__poll_"+_uniqueID+"_qt1\" style='padding-right:20px;'>" + Resources.PollForm.OneAnswerVariant + "</label>");
            sb.Append("</div>");

            sb.Append("<div>");
            sb.Append("<input style=\"vertical-align:middle; margin:0px 8px 0px 0px;\" type=\"radio\" " +
                      multipleState + " id=\"__poll_" + _uniqueID + "_qt2\" value=\"2\" name=\"questiontype\"/>");
            sb.Append("<label for=\"__poll_"+_uniqueID+"_qt2\">" + Resources.PollForm.FewAnswerVariant + "</label>");
            sb.Append("</div>");
            

            output.Write(sb.ToString());
        }
       
        public class AnswerViarint
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }     
    }
}
