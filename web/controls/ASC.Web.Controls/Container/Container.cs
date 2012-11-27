using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Drawing;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web;

namespace ASC.Web.Controls
{

    

    [
    ToolboxData("<{0}:Container runat=\"server\"/>"),
    ToolboxBitmap(typeof(Container), "Container.ToolboxBitmaps.Container.bmp"),
    ParseChildren(true), PersistChildren(false),
    Themeable(true)
    ]
    public class Container : Control
    {
        [Browsable(true)]
        public override bool EnableTheming { get { return true; } set { } }

        #region public properties

        [Description("Provides Header items."),
        Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public PlaceHolder Header { get; set; }


        [Description("Provides Body items."),
        Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public PlaceHolder Body { get; set; }

        [Description("Provides Style options."),
        Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public StyleOptions Options { get; set; }


        [Description("Bread Crumbs items."),
        Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public List<BreadCrumb> BreadCrumbs { get; set; }

        public String ContainerHeaderId { get; set; }

        #endregion

        public Container()
            : base()
        {
            BreadCrumbs = new List<BreadCrumb>();
        }

        private bool _isBreadCrumbs
        {
            get { return BreadCrumbs != null && BreadCrumbs.Count > 0; }
        }

        #region Load
        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);
            if (Header != null)
            {
                this.Controls.Add(Header);
            }

            if (Body != null)
            {
                this.Controls.Add(Body);
            }
        }
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            InitStyleOptions();

        }

        private void InitStyleOptions()
        {
            if (Options == null)
            {
                Options = new StyleOptions();
            }

            if (string.IsNullOrEmpty(Options.ContainerCssClass))
            {
                Options.ContainerCssClass = "containerClass";
            }


            if (string.IsNullOrEmpty(Options.ContainerCssClass))
            {
                Options.ContainerCssClass = "containerClass";
            }

            if (string.IsNullOrEmpty(Options.HeadCssClass))
            {
                Options.HeadCssClass = "containerHeaderBlock";
            }


            if (string.IsNullOrEmpty(Options.BodyCssClass))
            {
                Options.BodyCssClass = "containerBodyBlock";
            }

            if (string.IsNullOrEmpty(Options.BreadCrumbLinkCssClass))
            {
                Options.BreadCrumbLinkCssClass = "containerBreadCrumbLink";
            }

            if (string.IsNullOrEmpty(Options.BreadCrumbSpiterCssClass))
            {
                Options.BreadCrumbSpiterCssClass = "containerBreadCrumbSpiter";
            }

            if (string.IsNullOrEmpty(Options.BreadCrumbSpiter))
            {
                Options.BreadCrumbSpiter = " > ";
            }

            if(string.IsNullOrEmpty(Options.InfoMessageText))
            {
                Options.InfoMessageText = string.Empty;
            }




        }
        #endregion


        #region Render
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Page.ClientScript.IsClientScriptBlockRegistered("ContainerStyle"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<link rel=\"stylesheet\" text=\"text/css\" href=\"{0}\" />", Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Container.Css.ContainerDefault.css"));

                ((System.Web.UI.HtmlControls.HtmlHead)Page.Header).Controls.Add(new LiteralControl(sb.ToString()));



                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ContainerStyle", "");
            }

        }

        public string GetInfoPanelClientID()
        {
            return string.Format("{0}_InfoPanel", ClientID);
        }

        public string GetInfoPanelUniqueID()
        {
            return string.Format("{0}$InfoPanel", UniqueID);
        }

        protected override void Render(HtmlTextWriter writer)
        {

            writer.Write("<div class=\"{0}\" {1}>", (Options.IsPopup&& !string.IsNullOrEmpty(Options.PopupContainerCssClass)? Options.PopupContainerCssClass : Options.ContainerCssClass), string.IsNullOrEmpty(Options.ContainerStyle)
                            ? string.Empty : string.Format("style=\"{0}\"", Options.ContainerStyle));


            if (Header != null || _isBreadCrumbs || Options.IsPopup)
            {


                writer.Write("<div class=\"{0}\" {1}>", Options.HeadCssClass, string.IsNullOrEmpty(Options.HeadStyle)
                           ? string.Empty : string.Format("style=\"{0}\"", Options.HeadStyle));

                writer.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style='width:100%; height:0px;'>");
                writer.Write("<tr valign=\"top\"><td>");

                if (_isBreadCrumbs)
                {
                    BreadCrumb path;
                    if (BreadCrumbs.Count > 1)
                    {
                        writer.Write(@"<div style=""padding:0px 0px 8px 0px;"">");
                    }

                    for (int i = 0; i < (BreadCrumbs.Count - 1); i++)
                    {
                        if (i > 0)
                        {
                            writer.Write(@"<span class=""{0}""{1}>{2}</span>", Options.BreadCrumbSpiterCssClass,
                                string.IsNullOrEmpty(Options.BreadCrumbSpiterStyle) ? string.Empty : string.Format(@" style=""{0}""", Options.BreadCrumbSpiterStyle),
                                HttpUtility.HtmlEncode(Options.BreadCrumbSpiter));
                        }
                        path = BreadCrumbs[i];
                        writer.Write(@"<a class=""{0}""{1} title=""{2}"" href=""{3}"">{2}</a>", Options.BreadCrumbLinkCssClass,
                                string.IsNullOrEmpty(Options.BreadCrumbLinkStyle) ? string.Empty : string.Format(@" style=""{0}""", Options.BreadCrumbLinkStyle),
                                path.Caption.HtmlEncode(), path.NavigationUrl);
                    }
                    if (BreadCrumbs.Count > 1)
                    {
                        if (Options.ShowLastLevel)
                        {
                            writer.Write(@"<span class=""{0}""{1}>{2}</span>", Options.BreadCrumbSpiterCssClass,
                                    string.IsNullOrEmpty(Options.BreadCrumbSpiterStyle) ? string.Empty : string.Format(@" style=""{0}""", Options.BreadCrumbSpiterStyle),
                                    HttpUtility.HtmlEncode(Options.BreadCrumbSpiter));
                            path = BreadCrumbs[BreadCrumbs.Count - 1];
                            writer.Write(@"<span class=""{0}""{1} >{2}</span>", Options.BreadCrumbLinkCssClass,
                                    string.IsNullOrEmpty(Options.BreadCrumbLinkStyle) ? string.Empty : string.Format(@" style=""{0}""", Options.BreadCrumbLinkStyle),
                                    path.Caption.HtmlEncode());
                        }
                        writer.Write(@"</div>");
                    }
                    writer.Write("<div" + (String.IsNullOrEmpty(ContainerHeaderId) ? String.Empty : String.Format(@" id=""{0}""", ContainerHeaderId)) + ">{0}</div>", string.IsNullOrEmpty(Options.HeaderBreadCrumbCaption) ? BreadCrumbs[BreadCrumbs.Count - 1].Caption.HtmlEncode() : Options.HeaderBreadCrumbCaption);


                }
                else if (Header != null)
                {
                    Header.RenderControl(writer);
                }

                writer.Write("</td>");


                if (Options.IsPopup)
                {
                    writer.Write(@"<td class=""popupCancel"">{0}</td>",
                        string.Format(@"<div class=""cancelButton""{0}></div>", !string.IsNullOrEmpty(Options.OnCancelButtonClick) ? string.Format(@"onclick=""{0}""", Options.OnCancelButtonClick) : string.Empty));
                   
                }
                writer.Write("</tr></table>");

                writer.Write("</div>");

                //writer.Write(@"<div style=""clear:both""></div>");
            }

            writer.Write(string.Format(Options.IsPopup ?
                @"<div {1}{2} class=""infoPanel{3}""><div>{0}</div></div>" :
                @"<div class=""infoPanel{3}""><div {1}{2}>{0}</div></div>",
                    string.IsNullOrEmpty(Options.InfoMessageText) ? string.Empty : Options.InfoMessageText,
                    string.IsNullOrEmpty(Options.InfoMessageText) ? "style=\"display:none;\" " : string.Empty,
                    string.Format(@"id=""{0}"" name=""{1}"" ", GetInfoPanelClientID(), GetInfoPanelUniqueID()),
                    Options.InfoType.Equals(InfoType.Info) ? string.Empty : string.Format(" {0}", Options.InfoType.Equals(InfoType.Warning) ? "warn" : "alert")));

            if (Body != null)
            {
                writer.Write("<div class=\"{0}\" {1}>", Options.BodyCssClass, string.IsNullOrEmpty(Options.BodyStyle)
                            ? string.Empty : string.Format("style=\"{0}\"", Options.BodyStyle));
                Body.RenderControl(writer);
                writer.Write("</div>");
            }

            writer.Write(@"</div>");

        }
        #endregion
    }
}
