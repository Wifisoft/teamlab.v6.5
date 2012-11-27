using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Controls
{
    using BBCodeParser;

    [Themeable(true)]
    [ParseChildren(true, "Text")]
    [PersistChildren(false)]
    [DefaultProperty("Text")]   
    [ToolboxData("<{0}:BBCodeTextArea runat=server></{0}:BBCodeTextArea>")]
    public class BBCodeTextArea : WebControl, INamingContainer
    {        
        public BBCodeTextArea()
        {  
            //smiles
            this.Smiles = new List<Smile>();

            //buttons
            this.BBCodeCustomButtons = new List<BBCodeCustomButton>();

            this.BoldButton = new BBCodeCustomButton() { AltText = "bold", BBCodeTagName = "B"};
            this.BBCodeCustomButtons.Add(BoldButton);

            this.ItalicButton = new BBCodeCustomButton() { AltText = "italic", BBCodeTagName = "I" };
            this.BBCodeCustomButtons.Add(ItalicButton);

            this.UnderlineButton = new BBCodeCustomButton() { AltText = "underline", BBCodeTagName = "U" };
            this.BBCodeCustomButtons.Add(UnderlineButton);

            this.StrikeButton = new BBCodeCustomButton() { AltText = "strike", BBCodeTagName = "STRIKE" };
            this.BBCodeCustomButtons.Add(StrikeButton);

            this.FixedButton = new BBCodeCustomButton() { AltText = "fixed", BBCodeTagName = "FIXED"};
            this.BBCodeCustomButtons.Add(FixedButton);

            this.URLButton = new BBCodeCustomButton() { AltText = "link", BBCodeTagName = "URL", TagType = BBCodeTagType.NormalWithEqual };
            this.BBCodeCustomButtons.Add(URLButton);

            this.MailButton = new BBCodeCustomButton() { AltText = "mail", BBCodeTagName = "MAIL", TagType = BBCodeTagType.NormalWithEqual };
            this.BBCodeCustomButtons.Add(MailButton);

            this.ImageButton = new BBCodeCustomButton() { AltText = "image", BBCodeTagName = "IMG" };
            this.BBCodeCustomButtons.Add(ImageButton);

            this.QuoteButton = new BBCodeCustomButton() { AltText = "quote", BBCodeTagName = "QUOTE", TagType = BBCodeTagType.NormalWithEqualAndQuote };
            this.BBCodeCustomButtons.Add(QuoteButton);

            //textSize
            this.BBCodeTextSizeItems = new List<BBCodeComboBoxItem>();

            this.BBCodeTextSizeItems.Add(new BBCodeComboBoxItem() { Name = "Size", BBCodeTagName = "nosize", IsTitle = true });
            this.BBCodeTextSizeItems.Add(new BBCodeComboBoxItem() { Name = "1", BBCodeTagName = "SIZE1" });
            this.BBCodeTextSizeItems.Add(new BBCodeComboBoxItem() { Name = "2", BBCodeTagName = "SIZE2" });
            this.BBCodeTextSizeItems.Add(new BBCodeComboBoxItem() { Name = "3", BBCodeTagName = "SIZE3" });
            this.BBCodeTextSizeItems.Add(new BBCodeComboBoxItem() { Name = "4", BBCodeTagName = "SIZE4" });
            this.BBCodeTextSizeItems.Add(new BBCodeComboBoxItem() { Name = "5", BBCodeTagName = "SIZE5" });


            //color select
            this.BBCodeTextColorItems = new List<BBCodeComboBoxItem>();
            this.BBCodeTextColorItems.Add(new BBCodeComboBoxItem() { Name = "TextColor", BBCodeTagName = "nocolor", IsTitle = true });
            this.BBCodeTextColorItems.Add(new BBCodeComboBoxItem() { Name = "Red", BBCodeTagName = "RED", Style = "background-color:red; color:#ffffff;" });
            this.BBCodeTextColorItems.Add(new BBCodeComboBoxItem() { Name = "Green", BBCodeTagName = "GREEN", Style = "background-color:green; color:#ffffff;" });
            this.BBCodeTextColorItems.Add(new BBCodeComboBoxItem() { Name = "Blue", BBCodeTagName = "BLUE", Style = "background-color:blue; color:#ffffff;" });

            //code highlight
            this.BBCodeHighlightItems = new List<BBCodeComboBoxItem>();
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "Code", BBCodeTagName = "nocode", IsTitle = true });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "C++", BBCodeTagName = "CPP" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "C#", BBCodeTagName = "CS" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "CSS", BBCodeTagName = "CSS", });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "Delphi", BBCodeTagName = "DELPHI" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "DOS", BBCodeTagName = "DOS" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "HTML-XML", BBCodeTagName = "HTMLXML" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "Java", BBCodeTagName = "JAVA" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "Javascript", BBCodeTagName = "JAVASCRIPT" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "SQL", BBCodeTagName = "SQL" });
            this.BBCodeHighlightItems.Add(new BBCodeComboBoxItem() { Name = "VbScript", BBCodeTagName = "VBSCRIPT" });
            

            if (String.IsNullOrEmpty(TextareaID))
                TextareaID = Guid.NewGuid().ToString().Replace('-', '_');

            if (String.IsNullOrEmpty(TextareaName))
                TextareaName = "bbcode_text";

            this.ButtonPanelStyle = "padding:2px;";

            _bbCodeEditorVarName = "bbcode_editor_" + Guid.NewGuid().ToString().Replace('-', '_');
        }

        public List<Smile> Smiles { get; set; }

        #region buttons

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public string SmileImage{ get; set; }

        [Bindable(true)]
        [Category("Appearance")]        
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton BoldButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton ItalicButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton UnderlineButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton StrikeButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton FixedButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton URLButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton MailButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton ImageButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public BBCodeCustomButton QuoteButton { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<BBCodeCustomButton> BBCodeCustomButtons { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<BBCodeComboBoxItem> BBCodeTextSizeItems { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<BBCodeComboBoxItem> BBCodeTextColorItems { get; set; }

        [Bindable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<BBCodeComboBoxItem> BBCodeHighlightItems { get; set; } 
        #endregion

        private void InitializeImages()
        {
            if (!this.BoldButton.Disabled && String.IsNullOrEmpty(this.BoldButton.ImageFileName))
                this.BoldButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.text_bold.png");

            if (!this.ItalicButton.Disabled && String.IsNullOrEmpty(this.ItalicButton.ImageFileName))
                this.ItalicButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.text_italic.png");

            if (!this.UnderlineButton.Disabled && String.IsNullOrEmpty(this.UnderlineButton.ImageFileName))
                this.UnderlineButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.text_underline.png");

            if (!this.StrikeButton.Disabled && String.IsNullOrEmpty(this.StrikeButton.ImageFileName))
                this.StrikeButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.text_strike.png");

            if (!this.FixedButton.Disabled && String.IsNullOrEmpty(this.FixedButton.ImageFileName))
                this.FixedButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.code.png");

            if (!this.URLButton.Disabled && String.IsNullOrEmpty(this.URLButton.ImageFileName))
                this.URLButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.world_link.png");

            if (!this.MailButton.Disabled && String.IsNullOrEmpty(this.MailButton.ImageFileName))
                this.MailButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.mail.png");

            if (!this.ImageButton.Disabled && String.IsNullOrEmpty(this.ImageButton.ImageFileName))
                this.ImageButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.image.png");

            if (!this.QuoteButton.Disabled && String.IsNullOrEmpty(this.QuoteButton.ImageFileName))
                this.QuoteButton.ImageFileName = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.comments.png");

            if(String.IsNullOrEmpty(this.SmileImage))
                this.SmileImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.Images.smile.png");
        }
       

        #region sys serializable props

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextareaID
        {
            get
            {
                String s = (String)ViewState["TextareaID"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextareaID"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextareaName
        {
            get
            {
                String s = (String)ViewState["TextareaName"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextareaName"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]   
        public string TextareaAdditionalParams
        {
            get
            {
                String s = (String)ViewState["TextareaAdditionalParams"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextareaAdditionalParams"] = value;
            }
        }
        

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        #endregion

        #region css & styles

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextareaStyle
        {
            get
            {
                string style = (string)ViewState["TextareaStyle"];
                return (style == null ? String.Empty : style);
            }

            set
            {
                ViewState["TextareaStyle"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextareaCSSClass
        {
            get
            {
                string style = (string)ViewState["TextareaCSSClass"];
                return (style==null? String.Empty : style);
            }

            set
            {
                ViewState["TextareaCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ButtonPanelStyle
        {
            get
            {   
                string style = (string)ViewState["ButtonPanelStyle"];
                return (style==null ? String.Empty : style);
            }

            set
            {
                ViewState["ButtonPanelStyle"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ButtonPanelCSSClass
        {
            get
            {
                string s = (string)ViewState["ButtonPanelCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["ButtonPanelCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string CustomButtonlCSSClass
        {
            get
            {
                string s = (string)ViewState["CustomButtonlCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["CustomButtonlCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ComboboxCSSClass
        {
            get
            {
                string s = (string)ViewState["ComboboxCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["ComboboxCSSClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string SmileboxCSSClass
        {
            get
            {
                string s = (string)ViewState["SmileboxCSSClass"];
                return (s == null ? String.Empty : s);
            }

            set
            {
                ViewState["SmileboxCSSClass"] = value;
            }
        }

        #endregion

        #region smyle combo

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool IsRenderSmiles
        {
            get
            {
                return ((ViewState["IsRenderSmiles"] == null) ? true : (bool)ViewState["IsRenderSmiles"]);
            }

            set
            {
                ViewState["IsRenderSmiles"] = value;
            }
        }


        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]        
        public bool IsRenderTextSize
        {
            get
            {
                return ((ViewState["IsRenderTextSize"] == null) ? true : (bool)ViewState["IsRenderTextSize"]);
            }

            set
            {
                ViewState["IsRenderTextSize"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]        
        public bool IsRenderTextColor
        {
            get
            {
                return ((ViewState["IsRenderTextColor"] == null) ? true : (bool)ViewState["IsRenderTextColor"]);
            }

            set
            {
                ViewState["IsRenderTextColor"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]        
        public bool IsRenderCodeHighlighter
        {
            get
            {
                return ((ViewState["IsRenderCodeHighlighter"] == null) ? true : (bool)ViewState["IsRenderCodeHighlighter"]);
            }

            set
            {
                ViewState["IsRenderCodeHighlighter"] = value;
            }
        } 
        #endregion       

        private string _bbCodeEditorVarName = "";        

       
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string scriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.js.bbcodeeditor.js");
            Page.ClientScript.RegisterClientScriptInclude("bbcodearea_script", scriptLocation);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "bbcodearea_style",
                                                       "<link href=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.BBCodeTextArea.css.style.css") + "\" type=\"text/css\" rel=\"stylesheet\"/>", false);

            if (Page != null && Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(Page.Request[this.TextareaName]))
                    this.Text = Page.Request[this.TextareaName];
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            string script = "var " + _bbCodeEditorVarName + " = new BBCodeTextAreaEditorPrototype('" + _bbCodeEditorVarName + "','" + TextareaID + "');";           
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), script, true);
            
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.InitializeImages();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div>");
            sb.Append("<div class='bbcodearea_clearFix " + (String.IsNullOrEmpty(this.ButtonPanelCSSClass) ? "bbcodearea_buttonPanel" : this.ButtonPanelCSSClass) + "' " + (String.IsNullOrEmpty(ButtonPanelStyle) ? "" : "style=\"" + ButtonPanelStyle + "\"") + ">");

            
            this.BBCodeCustomButtons.ForEach(b => { BBCodeButton _b = (BBCodeButton)b; ExecJavascriptAction(ref _b); });

            foreach (var bbcodeButton in BBCodeCustomButtons)
            {
                if(!bbcodeButton.Disabled)
                    sb.Append("<a title=\"" + HttpUtility.HtmlEncode(bbcodeButton.AltText) + "\" href=\"" + bbcodeButton.JavascriptAction + "\" class='"+(String.IsNullOrEmpty(this.CustomButtonlCSSClass)?"bbcodearea_customButton":this.CustomButtonlCSSClass)+"' style=\"float:left;\"><img alt=\"" + bbcodeButton.AltText + "\" src=\"" + bbcodeButton.ImageFileName + "\" border=\"0\" hspace=\"2\"/></a>");
            }

            if (this.IsRenderSmiles && this.Smiles != null && this.Smiles.Count > 0)
                sb.Append(RenderSmiles());

            if (IsRenderTextSize)
                sb.Append(RendexComboboxItems(this.BBCodeTextSizeItems));

            if (IsRenderTextColor)
                sb.Append(RendexComboboxItems(this.BBCodeTextColorItems));

            if (IsRenderCodeHighlighter)
                sb.Append(RendexComboboxItems(this.BBCodeHighlightItems));


            sb.Append("</div>");

            sb.Append("<textarea " + (TextareaAdditionalParams ?? "") + " id=\"" + TextareaID + "\" name=\"" + TextareaName + "\" style=\"width:100%; " + (TextareaStyle ?? "") + "\"");
            sb.Append((String.IsNullOrEmpty(TextareaCSSClass) ? "" : " class=\"" + TextareaCSSClass + "\"") + ">");
            sb.Append(HttpUtility.HtmlEncode(Text) + "</textarea>");

            sb.Append("</div>");

            writer.Write(sb.ToString());
        }

        private string RenderSmiles()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;

            Guid smileBoxID = Guid.NewGuid();

            sb.Append("<a id='sm_button_" + smileBoxID + "' title=\"\" class='" + (String.IsNullOrEmpty(this.CustomButtonlCSSClass) ? "bbcodearea_customButton" : this.CustomButtonlCSSClass) + "' href=\"javascript:" + _bbCodeEditorVarName + ".ShowSmileBox('" + smileBoxID + "');\" style=\"float:left;\"><img alt=\":)\" src=\"" + this.SmileImage+ "\" border=\"0\" hspace=\"2\"/></a>");

            sb.Append("<div id='sm_box_" + smileBoxID + "' class=\"" + (String.IsNullOrEmpty(this.SmileboxCSSClass) ? "bbcodearea_smileBox" : this.SmileboxCSSClass) + "\" style='display:none; width:260px; position:absolute;'>");
            foreach (var sm in this.Smiles)
            {
                var button = new BBCodeButton() { TagType = BBCodeTagType.Smile, BBCodeTagName = sm.JavascriptValue };
                this.ExecJavascriptAction(ref button);

                if (i == 0)
                {
                    sb.Append("<div class=\"bbcodearea_clearFix\">");
                }
                sb.Append("<div align=center style='width:50px; height:35px; float:left;'>");
                sb.Append("<a onclick=\""+_bbCodeEditorVarName + ".HideSmileBox('" + smileBoxID + "');\" href=\"" + button.JavascriptAction + "\"><img alt=\"" + sm.Value + "\" title=\"" + sm.Title + "\" border=0 src=\"" + sm.Img + "\"/></a>");

                sb.Append("</div>");
                i++;
                if (i == 5)
                {
                    sb.Append("</div>");
                    i = 0;
                }
            }
            if(i!=0)
                sb.Append("</div>");

            sb.Append("</div>");

            return sb.ToString();
        }

        private string RendexComboboxItems(List<BBCodeComboBoxItem> items)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div style='float:left;'>");
            sb.Append("<select class=\"" + (String.IsNullOrEmpty(this.ComboboxCSSClass) ? "bbcodearea_comboBox" : this.ComboboxCSSClass) + "\" style=\"width:100px; margin-left:5px; height:20px\" onchange=\"" + _bbCodeEditorVarName + ".InsTagSel(this.options[selectedIndex].value);this.selectedIndex=0;\">");

            
            items.ForEach(b => { BBCodeButton _b = (BBCodeButton)b; ExecJavascriptAction(ref _b); });

            BBCodeComboBoxItem firstItem = items.Find(b => b.IsTitle);
            if (firstItem != null)
                sb.Append("<option " + (String.IsNullOrEmpty(firstItem.Style) ? "" : "style=\"" + firstItem.Style + "\"") + " value=\"" + firstItem.BBCodeTagName + "\">" + HttpUtility.HtmlEncode(firstItem.Name) + "</option>");

            foreach (var item in items)
            {
                if (item.IsTitle)
                    continue;

                sb.Append("<option " + (String.IsNullOrEmpty(item.Style) ? "" : "style=\"" + item.Style + "\"") + " value=\"" + item.BBCodeTagName + "\">" + HttpUtility.HtmlEncode(item.Name) + "</option>");
            }

            sb.Append("</select>");
            sb.Append("</div>");

            return sb.ToString();
        }

        public void ExecJavascriptAction(ref BBCodeButton button)
        {
            string func = "";
            switch (button.TagType)
            {
                case BBCodeTagType.Normal:
                    func = "InsTagSel";
                    break;

                case BBCodeTagType.NormalWithEqual:
                    func = "InsTagArgW";
                    break;

                case BBCodeTagType.NormalWithEqualAndQuote:
                    func = "InsTagArgQuote";
                    break;

                case BBCodeTagType.Smile:
                    func = "InsSmile";
                    break;
            }
            button.JavascriptAction = "javascript:" + _bbCodeEditorVarName + "." + func + "('" + button.BBCodeTagName + "');";
        }

        public static ParserConfiguration DefaultParserConfig
        {
            get
            {
                ParserConfiguration config = new ParserConfiguration(true);
                config.ExpressionReplacements.Add(new ExpressionReplacement("\n", "<br/>"));

                TagConfiguration tc = new TagConfiguration("URL", "<a target=\"_blank\" href=\"{1}\">{0}</a>") { IsParseTextReqularExpressions = false };
                TagParamOption tpo = new TagParamOption();
                tpo.PreValue = "http://";
                tpo.ParamNumber = 1;
                tpo.IsUseAnotherParamValue = true;
                tpo.AnotherParamNumber = 0;
                tc.ParamOptions.Add(tpo);
                config.TagConfigurations.Add(tc);

                config.RegExpTemplates.Add(RegularExpressionTemplate.HTMLReferenceExpression);

                config.TagConfigurations.Add(new TagConfiguration("SIZE1", "<font size=\"1\">{0}</font>"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE2", "<font size=\"2\">{0}</font>"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE3", "<font size=\"3\">{0}</font>"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE4", "<font size=\"4\">{0}</font>"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE5", "<font size=\"5\">{0}</font>"));
                config.TagConfigurations.Add(new TagConfiguration("RED", "<span style=\"color:red;\">{0}</span>"));
                config.TagConfigurations.Add(new TagConfiguration("GREEN", "<span style=\"color:green;\">{0}</span>"));
                config.TagConfigurations.Add(new TagConfiguration("BLUE", "<span style=\"color:blue;\">{0}</span>"));
                config.TagConfigurations.Add(new TagConfiguration("MAIL", "<a href=\"mailto:{1}\">{0}</a>"));
                config.TagConfigurations.Add(new TagConfiguration("QUOTE", "<table style=\"border-color:gray; color:#dddddd; border-width: 2px;\" cellpadding=\"0\" cellspacing=\"0\"><tr height=\"20\"><td style=\"padding-left:5px;\"><span class='font-weight:bolder;'>{1}:</span></td></tr><tr><td style=\"padding:5px;\">{0}</td></tr></table>"));
                config.TagConfigurations.Add(new TagConfiguration("IMG", "<img alt='' src=\"{0}\"/>") { IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("FIXED", "<pre>{0}</pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("B", "<b>{0}</b>"));
                config.TagConfigurations.Add(new TagConfiguration("I", "<i>{0}</i>"));
                config.TagConfigurations.Add(new TagConfiguration("U", "<u>{0}</u>"));
                config.TagConfigurations.Add(new TagConfiguration("STRIKE", "<strike>{0}</strike>"));
                config.TagConfigurations.Add(new TagConfiguration("REPLY", "<span class=\"font-size:14px;\">To: {0}</span><br/>"));

                config.TagConfigurations.Add(new TagConfiguration("CPP", "<pre><code class='cpp'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("CS", "<pre><code class='cs'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("CSS", "<pre><code class='css'>{0}</code></pre>", false));
                config.TagConfigurations.Add(new TagConfiguration("DELPHI", "<pre><code class='delphi'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("DOS", "<pre><code class='dos'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("HTMLXML", "<pre><code class='html-xml'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("JAVA", "<pre><code class='java'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("JAVASCRIPT", "<pre><code class='javascript'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("SQL", "<pre><code class='sql'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("VBSCRIPT", "<pre><code class='vbscript'>{0}</code></pre>", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });

                return config;
            }
        }

        public static ParserConfiguration ClearDefaultParserConfig
        {
            get
            {
                ParserConfiguration config = new ParserConfiguration(false);

                config.TagConfigurations.Add(new TagConfiguration("URL", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE1", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE2", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE3", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE4", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("SIZE5", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("RED", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("GREEN", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("BLUE", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("MAIL", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("QUOTE", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("IMG", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("FIXED", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("B", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("I", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("U", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("STRIKE", "{0}"));
                config.TagConfigurations.Add(new TagConfiguration("REPLY", "To: {0}\n"));

                config.TagConfigurations.Add(new TagConfiguration("CPP", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("CS", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("CSS", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("DELPHI", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("DOS", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("HTMLXML", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("JAVA", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("JAVASCRIPT", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("SQL", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });
                config.TagConfigurations.Add(new TagConfiguration("VBSCRIPT", "{0}", false) { IsParseTextReplacement = false, IsParseTextReqularExpressions = false });

                return config;
            }
        }
    }
}
