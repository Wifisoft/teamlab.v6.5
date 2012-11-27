using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Controls.Common
{
    public class EmptyScreenControl : WebControl
    {
        public string ImgSrc { get; set; }

        public string Header { get; set; }

        public string HeaderDescribe { get; set; }

        public string Describe { get; set; }

        public string ButtonHTML { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            var html = new StringBuilder();

            html.AppendFormat("<div id='{0}' class='noContentBlock emptyScrCtrl' >", ID);
            if (!String.IsNullOrEmpty(ImgSrc))
            {
                html.AppendFormat("<table><tr><td><img src='{0}' alt='' class='emptyScrImage' /></td>", ImgSrc)
                    .Append("<td><div class='emptyScrTd' >");
            }
            if (!String.IsNullOrEmpty(Header))
            {
                html.AppendFormat("<div class='emptyScrHead' >{0}</div>", Header);
            }
            if (!String.IsNullOrEmpty(HeaderDescribe))
            {
                html.AppendFormat("<div class='emptyScrHeadDscr' >{0}</div>", HeaderDescribe);
            }
            if (!String.IsNullOrEmpty(Describe))
            {
                html.AppendFormat("<div class='emptyScrDscr' >{0}</div>", Describe);
            }
            if (!String.IsNullOrEmpty(ButtonHTML))
            {
                html.AppendFormat("<div class='emptyScrBttnPnl' >{0}</div>", ButtonHTML);
            }
            if (!String.IsNullOrEmpty(ImgSrc))
            {
                html.Append("</div></td></tr></table>");
            }

            html.Append("</div>");

            writer.WriteLine(html.ToString());
        }
    }
}