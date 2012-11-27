using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web.UI;

namespace ASC.Web.Controls
{
    public enum TabsShowPosition
    {
        Top = 0,
        Bottom,
        Both,
        None
    }


    [
    ToolboxData("<{0}:Tabs runat=\"server\"/>"),
    ToolboxBitmap(typeof(Tabs), "Tabs.ToolboxBitmaps.Tabs.bmp"),
    ParseChildren(ChildrenAsProperties = true, DefaultProperty = "Pages"), PersistChildren(false),
    Themeable(true)
    ]
    public class Tabs : Control, IPostBackEventHandler
    {
        public delegate void TabSelectedHandler(object sender, string selectedTabId);
        public event TabSelectedHandler TabSelected;

        #region public properties
        //[Description("List of pages."),
        //Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
        //DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        //]
        public List<Tab> Pages { get; set; }

        [Description("Selected page."),
        Category("Options"), PersistenceMode(PersistenceMode.Attribute),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public string SelectedPageID
        {
            get
            {
                if (ViewState["SelectedPageID"] == null)
                {
                    return string.Empty;
                }
                return ViewState["SelectedPageID"].ToString();
            }
            set
            {
                ViewState["SelectedPageID"] = value;
            }
        }

        private TabsShowPosition _DefTabsShowPosition = TabsShowPosition.Top;
        [Description("Select Position for Tabs switchers"),
        Category("Options"), PersistenceMode(PersistenceMode.Attribute),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public TabsShowPosition TabsShowPosition
        {
            get
            {
                TabsShowPosition result = _DefTabsShowPosition;
                if (ViewState["TabsShowPosition"] != null)
                {
                    try
                    {
                        result = (TabsShowPosition)(Convert.ToInt32(ViewState["TabsShowPosition"]));
                    }
                    catch { }
                }


                return result;
            }
            set
            {
				ViewState["TabsShowPosition"] = (int)value;
            }
        }

        [Description("Reverse the pages. The operation needs when you use float:right for Tab switchers in your css classes."),
       Category("Options"), PersistenceMode(PersistenceMode.Attribute),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
       ]
        public bool Reverse
        {
            get
            {

                return Convert.ToBoolean(ViewState["Reverse"]);
            }
            set
            {
                ViewState["Reverse"] = value;
            }
        }
        [Description("Set the Name of main CSS Class"),
       Category("Options"), PersistenceMode(PersistenceMode.Attribute),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
       ]
        public string CssClass
        {
            get
            {
                if (ViewState["CssClass"] == null)
                {
                    return "TabsControl";
                }

                if (ViewState["CssClass"].ToString().Equals(string.Empty))
                {
                    return "TabsControl";
                }

                return ViewState["CssClass"].ToString();
            }
            set
            {
                ViewState["CssClass"] = value;
            }
        }

        [Description("Use client side switches"),
        Category("Options"), PersistenceMode(PersistenceMode.Attribute),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public bool UseClientSideSwitch
        {
            get
            {
                if (ViewState["UseClientSideSwitch"] == null)
                {
                    return false;
                }

                return Convert.ToBoolean(ViewState["UseClientSideSwitch"]);

            }
            set
            {
                ViewState["UseClientSideSwitch"] = value;
            }
        }

        #endregion

        #region private fields
        private bool _SetSelectedPage = false;
        #endregion

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            _SetSelectedPage = true;
            if (Pages == null)
            {
                Pages = new List<Tab>();
            }

            Tab sepectedPage = null;

            foreach (Tab page in Pages)
            {
                this.Controls.Add(page);

                if (!string.IsNullOrEmpty(SelectedPageID))
                {
                    if (SelectedPageID.Equals(page.ID))
                    {
                        sepectedPage = page;
                    }

                    page.Selected = false;
                }
                else
                {
                    if (page.Selected)
                    {
                        sepectedPage = page;
                    }

                    page.Selected = false; ;
                }

                page.TabSelectedSected += new Tab.TabSelectedSetHandler(page_TabSelectedSected);
                page.ClientSideSwitch = this.UseClientSideSwitch;
            }


            if (sepectedPage != null)
            {
                Pages[Pages.IndexOf(sepectedPage)].Selected = true;
                SelectedPageID = sepectedPage.ID;
            }
            else if (Pages.Count > 0)
            {
                Pages[0].Selected = true;
                SelectedPageID = Pages[0].ID;
            }


            _SetSelectedPage = false;
        }

        void page_TabSelectedSected(string id)
        {
            if (_SetSelectedPage)
                return;

            _SetSelectedPage = true;


            Tab sepectedPage = null;

            foreach (Tab page in Pages)
            {
                this.Controls.Add(page);


                if (id.Equals(page.ID))
                {
                    sepectedPage = page;
                }

                page.Selected = false;
            }

            if (sepectedPage != null)
            {
                Pages[Pages.IndexOf(sepectedPage)].Selected = true;
                SelectedPageID = sepectedPage.ID;
            }
            else if (Pages.Count > 0)
            {
                Pages[0].Selected = true;
                SelectedPageID = Pages[0].ID;
            }

            _SetSelectedPage = false;
        }


        #region Render

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Page.ClientScript.IsClientScriptBlockRegistered("TabsStyle"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<link rel=\"stylesheet\" text=\"text/css\" href=\"{0}\" />", Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.Tabs.Css.TabsDefault.css"));

                ((System.Web.UI.HtmlControls.HtmlHead)Page.Header).Controls.Add(new LiteralControl(sb.ToString()));
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TabsStyle", "");
            }

            if (UseClientSideSwitch)
            {
                //Tab selectedPage = Pages.Find(tb => tb.ID == SelectedPageID);

                if (!Page.ClientScript.IsClientScriptBlockRegistered(string.Format("ClientSideSwitrch_{0}", this.ClientID)))
                {
                    bool isFirst = true;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(@"<script language=""javascript"">");
                    sb.AppendFormat(@"var tabSelectedPanel_{0} = [", this.ClientID);
                    foreach (Tab page in Pages)
                    {
                        if (!isFirst)
                        {
                            sb.AppendLine(",");
                        }
                        else
                        {
                            isFirst = false;
                        }
                        sb.AppendFormat(@"{{id:'{0}', selected:{1}}}", page.ClientID, page.Selected.ToString().ToLower());
                    }
                    sb.Append(@"];");

                    sb.Append(@"</script>");
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Tabs), string.Format("ClientSideSwitrch_{0}", this.ClientID), sb.ToString());
                }

                if (!Page.ClientScript.IsClientScriptBlockRegistered("ClientSideSwitrch"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"<script language=""javascript"">");
                    sb.AppendLine(@"function OnTabClientClick(arr, oDiv){");
                    sb.AppendLine(@"var tabId = oDiv.id.replace(/_Top$/gi,'').replace(/_Bottom$/gi,'')");
                    sb.AppendLine(@"var elem;");
                    sb.AppendLine(@"for(var i = 0; i < arr.length; i ++){");
                    sb.AppendLine(@"    elem = arr[i];");
                    sb.AppendLine(@"    OnTabSelect(elem.id, elem.id == tabId)");
                    sb.AppendLine(@"}");
                    sb.AppendLine(@"}");
                    sb.AppendLine(@"function OnTabSelect(tabId, isSelected){");
                    sb.AppendLine(@" var topCaption = document.getElementById(tabId + '_Top');");
                    sb.AppendLine(@" var bottomCaption = document.getElementById(tabId + '_Bottom');");
                    sb.AppendLine(@" var tab = document.getElementById(tabId);");
                    sb.AppendLine(@" tab.style.display = isSelected ? '' : 'none';");
                    sb.AppendLine(@" topCaption.className = topCaption.className.replace(/[\s]Selected$/gi,'');");
                    sb.AppendLine(@" bottomCaption.className = bottomCaption.className.replace(/[\s]Selected$/gi,'');");
                    sb.AppendLine(@"if(isSelected){");
                    sb.AppendLine(@" topCaption.className += ' Selected';");
                    sb.AppendLine(@" bottomCaption.className += ' Selected';");
                    sb.AppendLine(@"}");
                    sb.AppendLine(@"}");
                    sb.AppendLine(@"</script>");
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Tabs), "ClientSideSwitrch", sb.ToString());
                }

            }

        }


        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class=\"{0}\">", CssClass);
            if (TabsShowPosition == TabsShowPosition.Both || TabsShowPosition == TabsShowPosition.Top)
            {
                bool first = true;
                if (Reverse)
                {
                    Pages.Reverse();
                }
                if (Pages != null && Pages.Count > 0)
                {
                    writer.Write("<div class='TabListTop tabs_clearFix'>");
                    foreach (Tab page in Pages)
                    {
                        writer.Write("<div class=\"Tab{0}{1}\" {2} {4} {3}><div>", page.Selected ? " Selected" : string.Empty, first ? " First" : string.Empty,
                            page.ImageUrl.Equals(string.Empty) ? string.Empty : string.Format("style=\"background-image: url('{0}');\"", page.ImageUrl),
                            GetPostBackLink(page),
                            UseClientSideSwitch ? string.Format(@"onclick=""javascript:OnTabClientClick(tabSelectedPanel_{1}, this);"" id='{0}_Top'", page.ClientID, this.ClientID) : string.Empty);
                        writer.Write(page.Name.HtmlEncode());
                        writer.Write("</div></div>");

                        first = false;
                    }
                    writer.Write("</div>");
                }
            }
            writer.Write("<div class=\"Content\">");
            base.Render(writer);
            writer.Write("</div>");
            if (TabsShowPosition == TabsShowPosition.Both || TabsShowPosition == TabsShowPosition.Bottom)
            {
                bool first = true;
                if (Pages != null && Pages.Count > 0)
                {
                    writer.Write("<div class='TabListBottom tabs_clearFix'>");
                    foreach (Tab page in Pages)
                    {
                        writer.Write("<div class=\"Tab{0}{1}\" {2} {4} {3}><div>",
                            page.Selected ? " Selected" : string.Empty,
                            first ? " First" : string.Empty,
                            page.ImageUrl.Equals(string.Empty) ? string.Empty : string.Format("style=\"background-image: url('{0}');\"", page.ImageUrl),
                            GetPostBackLink(page),
                            UseClientSideSwitch ? string.Format(@"onclick=""javascript:OnTabClientClick(tabSelectedPanel_{1}, this);"" id='{0}_Bottom'", page.ClientID, this.ClientID) : string.Empty);
                        writer.Write(page.Name.HtmlEncode());
                        writer.Write("</div></div>");

                        first = false;
                    }
                    writer.Write("</div>");
                }
            }
            writer.Write("</div>");
        }

        private string GetPostBackLink(Tab page)
        {
            if (this.UseClientSideSwitch || page.Selected)
                return string.Empty;

            return string.Format("onclick=\"javascript:{0};\"", Page.ClientScript.GetPostBackEventReference(this, page.ID));
        }
        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            page_TabSelectedSected(eventArgument);

            if (TabSelected != null)
            {
                TabSelected(this, eventArgument);
            }
        }

        #endregion
    }
}
