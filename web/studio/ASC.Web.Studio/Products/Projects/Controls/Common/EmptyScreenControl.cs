using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Projects.Controls
{
    public class EmptyScreenControl : WebControl
    {
        public string HeaderContent
        {
            get;
            set;
        }

        public string HeaderDescribe
        {
            get;
            set;
        }


        public EmptyScreenControl()
        {

        }
        
        public EmptyScreenControl(string headerDesc)
        {
            HeaderDescribe = headerDesc;
        }


        protected override void Render(HtmlTextWriter writer)
        {
            var html = new StringBuilder("<div class=\"noContentBlock\">")
                .AppendFormat(@"<div>{0}</div>", HeaderContent)
                .AppendFormat(@"<div style='font-size: 14px; margin-top: 5px;'>{0}</div>", HeaderDescribe)
                .Append("</div>");
            writer.WriteLine(html.ToString());
        }
    }
}