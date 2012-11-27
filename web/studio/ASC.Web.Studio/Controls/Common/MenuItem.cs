using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;

namespace ASC.Web.Studio.Controls.Common
{
    [ToolboxData("<{0}:MenuItem runat=server />"),
     DefaultProperty("Name"), PersistChildren(false)]
    public class MenuItem : Control
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div style='margin-top:10px;'>");
            RenderContents(writer);
            writer.Write("</div>");
        }

        protected virtual void RenderContents(HtmlTextWriter writer)
        {
            writer.Write(this.Name.HtmlEncode());
        }

    }

    [ToolboxData("<{0}:HtmlMenuItem runat=server />")]
    public class HtmlMenuItem : MenuItem
    {
        public HtmlMenuItem(string html)
        {
            Html = html;
        }

        public string Html { get; set; }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (String.IsNullOrEmpty(Html))
                base.RenderContents(writer);
            else
                writer.Write(Html);
        }
    }

    [ToolboxData("<{0}:NavigationItem runat=server />")]
    public class NavigationItem : MenuItem
    {

        public string URL { get; set; }

        public bool IsPromo { get; set; }

        public bool Selected { get; set; }

        public bool RightAlign { get; set; }

        public List<NavigationItem> SubItems { get; set; }

        public int Width { get; set; }

        public string ModuleStatusIconFileName { get; set; }

        public string LinkId { get; set; }

        public NavigationItem()
        {
            RightAlign = false;
            this.SubItems = new List<NavigationItem>();
        }

        public NavigationItem(string name, string url)
            : this()
        {
            Name = name;
            URL = url;
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(URL))
            {
                writer.Write(@"<a href=""{0}"" title=""{1}"" class=""linkAction{2}"" {3}>{4}</a>",
                    ResolveUrl(this.URL),
                    Description.HtmlEncode(),
                    IsPromo ? " promoAction" : string.Empty,
                    string.IsNullOrEmpty(LinkId) ? string.Empty : string.Format(@"id=""{0}""", LinkId),
                    this.Name.HtmlEncode()
                    );
            }
            else
                base.RenderContents(writer);
        }
    }
}
