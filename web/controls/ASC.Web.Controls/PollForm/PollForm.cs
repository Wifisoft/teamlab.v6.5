using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;

namespace ASC.Web.Controls
{
    public interface IVoteHandler
    {
        bool VoteCallback(string pollID, List<string> selectedVariantIDs, string additionalParams, out string errorMessage);
    }

    [Themeable(true)]
    [AjaxNamespace("PollFormControl")]
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:PollForm runat=server></{0}:PollForm>")]
    public class PollForm : WebControl
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
        public bool Answered
        {
            get
            {
                object o = ViewState["Answered"];
                return ((o == null) ? false : (bool)o);
            }

            set
            {
                ViewState["Answered"] = value;
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
        public Type VoteHandlerType
        {
            get
            {
                object o = ViewState["VoteHandlerType"];
                return ((o == null) ? null : (Type)o);
            }

            set
            {
                ViewState["VoteHandlerType"] = value;
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
        public string AdditionalParams
        {
            get
            {
                object o = ViewState["AdditionalParams"];
                return ((o == null) ? null : (string)o);
            }

            set
            {
                ViewState["AdditionalParams"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        public string ButtonCSSClass
        {
            get
            {
                object o = ViewState["ButtonCSSClass"];
                return ((o == null) ? null : (string)o);
            }

            set
            {
                ViewState["ButtonCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        public string StatisticBarCSSClass
        {
            get
            {
                object o = ViewState["StatisticBarCSSClass"];
                return ((o == null) ? null : (string)o);
            }

            set
            {
                ViewState["StatisticBarCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        public string LiderStatisticBarCSSClass
        {
            get
            {
                object o = ViewState["LiderStatisticBarCSSClass"];
                return ((o == null) ? null : (string)o);
            }

            set
            {
                ViewState["LiderStatisticBarCSSClass"] = value;
            }
        }


        [Bindable(true)]
        [Category("Appearance")]
        public string VariantTextCSSClass
        {
            get
            {
                object o = ViewState["VariantTextCSSClass"];
                return ((o == null) ? null : (string)o);
            }

            set
            {
                ViewState["VariantTextCSSClass"] = value;
            }
        }


        [Bindable(true)]
        [Category("Appearance")]
        public string VoteCountTextCSSClass
        {
            get
            {
                object o = ViewState["VoteCountTextCSSClass"];
                return ((o == null) ? null : (string)o);
            }

            set
            {
                ViewState["VoteCountTextCSSClass"] = value;
            }
        }
       
        public string PollID { get; set; }

        public string BehaviorID { get; set; }

        private string _jsObjName;

        public PollForm()
        {
            this.AnswerVariants = new List<AnswerViarint>();
        }

        protected override void OnInit(EventArgs e)
        {
            Utility.RegisterTypeForAjax(this.GetType());

            string scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.PollForm.js.pollform.js");
            Page.ClientScript.RegisterClientScriptInclude("pollform_script", scriptLocation);
            
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "pollform_style",
                                                       "<link href=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.PollForm.css.style.css") + "\" type=\"text/css\" rel=\"stylesheet\"/>", false);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string statBarCSSClass = String.IsNullOrEmpty(StatisticBarCSSClass) ? "poll_statisticBar" : StatisticBarCSSClass;
            string liderBarCSSClass = String.IsNullOrEmpty(LiderStatisticBarCSSClass) ? "poll_liaderBar" : LiderStatisticBarCSSClass;            
            string variantNameCSSClass = String.IsNullOrEmpty(VariantTextCSSClass) ? "poll_VariantName" : VariantTextCSSClass;
            string voteCountCSSClass = String.IsNullOrEmpty(VoteCountTextCSSClass) ? "poll_CountText" : VoteCountTextCSSClass;

            _jsObjName = String.IsNullOrEmpty(this.BehaviorID) ? ("__pollForm" + this.UniqueID) : this.BehaviorID;

            StringBuilder script = new StringBuilder();
            script.Append(" var " + _jsObjName + " = new VotingPollPrototype('" + _jsObjName + "','" + this.VoteHandlerType.AssemblyQualifiedName + "','" + this.PollID + "','"+Resources.PollForm.EmptySelect+"','"+statBarCSSClass+"','"+liderBarCSSClass+"','"+variantNameCSSClass+"','"+voteCountCSSClass+"','" + AdditionalParams + "'); ");
            foreach (var variant in AnswerVariants)
                script.Append(_jsObjName + ".RegistryVariant(new AnswerVariantPrototype('" + variant.ID + "','" + ReplaceSingleQuote(HttpUtility.HtmlEncode(variant.Name)) + "'," + variant.VoteCount + ")); ");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);
        }

        protected override void RenderContents(HtmlTextWriter output)
        {  
           output.Write(RenderPollForm());
        }

        private string RenderPollForm()
        {
            string buttonCSSClass = String.IsNullOrEmpty(ButtonCSSClass) ? "poll_customButton" : ButtonCSSClass;
            string statBarCSSClass = String.IsNullOrEmpty(StatisticBarCSSClass) ? "poll_statisticBar" : StatisticBarCSSClass;
            string liderBarCSSClass = String.IsNullOrEmpty(LiderStatisticBarCSSClass) ? "poll_liaderBar" : LiderStatisticBarCSSClass;
            string voteCountCSSClass = String.IsNullOrEmpty(VoteCountTextCSSClass) ? "poll_CountText" : VoteCountTextCSSClass;
            string variantNameCSSClass = String.IsNullOrEmpty(VariantTextCSSClass) ? "poll_VariantName" : VariantTextCSSClass;

            StringBuilder sb = new StringBuilder();

            sb.Append("<div>");
            sb.AppendFormat(@"<div id=""__pollForm_{0}_result""></div>",this.PollID);
            sb.AppendFormat(@"<div id=""__pollForm_{0}_voteBox"">", this.PollID);            

            if (!Answered)
            {
                foreach (var variant in this.AnswerVariants)
                {
                    sb.Append(@"<div style=""padding:6px 0px"">");

                    sb.AppendFormat(@"<input style=""vertical-align:middle;"" name=""__pollForm_{0}_av"" value=""{1}"" id=""__pollForm_{0}_av_{1}""  style=""vertical-align:middle;"" type=""{2}""/>
                                      <label for=""__pollForm_{0}_av_{1}"" style=""padding-left:12px; vertical-align:middle;"">{3}</label>",
                                      this.PollID, variant.ID, (this.Singleton ? "radio" : "checkbox"), ReplaceSingleQuote(HttpUtility.HtmlEncode(variant.Name)));

                    sb.Append("</div>");                           
                }
            }
            else
            {
                sb.Append(RenderAnsweredVariants(this.AnswerVariants, statBarCSSClass, liderBarCSSClass, variantNameCSSClass, voteCountCSSClass));
            }
            

            if (!Answered)
                sb.AppendFormat(@"<div id=""__pollForm_{0}_answButtonBox"" style=""padding:18px 0px 12px 0px;"">
                                  <a class=""{3}"" href=""javascript:{1}.Vote();"">{2}</a>
                                  </div>", this.PollID, _jsObjName, Resources.PollForm.VoteButton, buttonCSSClass);

            sb.Append(@"</div>");
            sb.Append(@"</div>");
            return sb.ToString();
        }

        private string RenderAnsweredVariants(List<AnswerViarint> variants, string statBarCSSClass, string liderBarCSSClass, string variantNameCSSClass, string voteCountCSSClass)
        {            
            float width = 70;
            double k = 0;
            double userCount = .0;
            double max = 0, fullCount = 0;

            foreach (var variant in variants)
            {
                userCount = variant.VoteCount;
                fullCount += userCount;
                if (max < userCount)
                    max = userCount;
            }

            if(max!=0)
                k = width / max;

            StringBuilder sb = new StringBuilder();

            foreach (var variant in variants)
            {
                userCount = variant.VoteCount;

                sb.AppendFormat(@"<div class=""{1}"">{0}</div>", HttpUtility.HtmlEncode(variant.Name), variantNameCSSClass);
                sb.AppendFormat(@"<div class=""{1}"" style=""width:{0}%;"">&nbsp;</div>", Math.Round(k * userCount), max == userCount ? liderBarCSSClass : statBarCSSClass);
                sb.AppendFormat(@"<div class=""{2}""><span id=""strong"">{0}</span><span> ({1}%)</span></div>", userCount, fullCount != 0 ? Math.Round((userCount * 100) / fullCount) : 0 , voteCountCSSClass);
                sb.Append(@"<div style=""clear:both;"">&nbsp;</div>");
            }
            sb.AppendFormat(@"<div class=""{2}"">{0}: {1}</div>", Resources.PollForm.AllVoting, fullCount, variantNameCSSClass);
           
            return sb.ToString();
        }

        public class AnswerViarint
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public int VoteCount { get; set; }
        }

        private class VoteResult 
        {
            public string Message { get; set; }
            public string Success { get; set; }
            public string HTML { get; set; }
            public string PollID { get; set; }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object Vote(string voteHandlerTypeName, string pollID, string variantIDs, List<AnswerViarint> allVariants,
            string statBarCSSClass, string liderBarCSSClass, string variantNameCSSClass, string voteCountCSSClass,
            string additionalParams)
        {
            var result = new VoteResult()
            {
                PollID = pollID
            };

            var voteHandler = (IVoteHandler)Activator.CreateInstance(Type.GetType(voteHandlerTypeName));
            List<string> selectedVariantIDs = new List<string>();
            
            selectedVariantIDs.AddRange(variantIDs.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries));

            string errorMessage = "";
            if (voteHandler.VoteCallback(pollID, selectedVariantIDs, additionalParams, out errorMessage))
            {
                result.Success = "1";
                foreach (var var in allVariants)
                {
                    var selectedID = selectedVariantIDs.Find(id => String.Equals(id, var.ID, StringComparison.InvariantCultureIgnoreCase));
                    if (!String.IsNullOrEmpty(selectedID))
                        var.VoteCount++;
                }

                allVariants.ForEach(v => v.Name = HttpUtility.HtmlDecode(v.Name));

                result.HTML = RenderAnsweredVariants(allVariants,statBarCSSClass, liderBarCSSClass, variantNameCSSClass, voteCountCSSClass);
            }
            else
            {
                result.Success = "0";
                result.Message = HttpUtility.HtmlEncode(errorMessage);
            }

            return result;
        }

        public static string ReplaceSingleQuote(string str)
        {
            if (str == null)
                return null;

            return str.Replace("'", "′");
        }
    }
}
