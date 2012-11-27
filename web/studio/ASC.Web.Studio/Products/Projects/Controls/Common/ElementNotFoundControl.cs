#region Import

using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace ASC.Web.Projects.Controls
{
    public class ElementNotFoundControl : WebControl
    {
        #region Property

        public String Header { get; set; }
        public String Body { get; set; }
        public String RedirectURL { get; set; }
        public String RedirectTitle { get; set; }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {

            StringBuilder innerHTML = new StringBuilder("<div>");

            innerHTML.AppendFormat(@"<div class='pm-elementNotFoundControl-header'>{0}</div>", Header);
            innerHTML.AppendFormat(@"<div>{0}</div>", Body);
            innerHTML.AppendFormat(@"<div style='margin-top: 20px;'><a href='{0}'>{1}</a></div>", RedirectURL, RedirectTitle);

            innerHTML.Append("</div>");

            writer.WriteLine(innerHTML.ToString());
            
        }
    }
}
