using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using System.Web;

namespace ASC.Web.Studio.Controls.Users
{
    [ToolboxData("<{0}:EmployeeUserCard runat=\"server\"/>")]
    public class EmployeeUserCard : Control
    {

        private string _CssClass = string.Empty;
        public string CssClass
        {
            get
            {
                if (ViewState["CssClass"] == null || ViewState["CssClass"].ToString().Equals(string.Empty))
                {
                    return _CssClass;
                }
                return ViewState["CssClass"].ToString();
            }
            set
            {
                ViewState["CssClass"] = value;
            }
        }

        public string EmployeeUrl
        {
            get
            {
                if (ViewState["EmployeeUrl"] == null || ViewState["EmployeeUrl"].ToString().Equals(string.Empty))
                {
                    return string.Empty; ;
                }
                return ViewState["EmployeeUrl"].ToString();
            }
            set
            {
                ViewState["EmployeeUrl"] = value;
            }
        }

        public Unit Height
        {
            get
            {
                if (ViewState["Height"] == null || ViewState["Height"].ToString().Equals(string.Empty))
                {
                    return Unit.Parse("122px");
                }
                return Unit.Parse(ViewState["Height"].ToString());
            }
            set
            {
                ViewState["Height"] = value.ToString();
            }
        }


        public Unit Width
        {
            get
            {
                if (ViewState["Width"] == null || ViewState["Width"].ToString().Equals(string.Empty))
                {
                    return Unit.Parse("352px");
                }
                return Unit.Parse(ViewState["Width"].ToString());
            }
            set
            {
                ViewState["Width"] = value.ToString();
            }
        }

        public string EmployeeDepartmentUrl
        {
            get
            {
                if (ViewState["EmployeeDepartmentUrl"] == null || ViewState["EmployeeDepartmentUrl"].ToString().Equals(string.Empty))
                {
                    return string.Empty; ;
                }
                return ViewState["EmployeeDepartmentUrl"].ToString();
            }
            set
            {
                ViewState["EmployeeDepartmentUrl"] = value;
            }
        }

        public UserInfo EmployeeInfo { get; set; }
        
        protected override void Render(HtmlTextWriter writer)
        {   
            if(EmployeeInfo != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<div {1} style=\"width: {0}px;overflow:hidden;\"><table cellpadding=\"0\" border=\"0\" cellspacing=\"0\" width=\"100%\" >",Width.Value,
                    EmployeeInfo.ActivationStatus == EmployeeActivationStatus.Pending? "class=\"pending\"":"");

                sb.Append("<tr valign='top'>");
                sb.Append("<td align=\"left\" style=\"width:90px; padding-right:10px;\">");
                sb.AppendFormat("<a class=\"borderBase\" {1} href=\"{0}\">", EmployeeUrl, "style=\"position:relative;  text-decoration:none; display:block; height:82px; width:82px;\"");
                sb.Append("<img align=\"center\" alt=\"\" style='display:block;margin:0; position:relative;' border=0 src=\"" + EmployeeInfo.GetBigPhotoURL() + "\"/>");
                if(EmployeeInfo.ActivationStatus == EmployeeActivationStatus.Pending)
                {
                    sb.Append("<div class=\"pendingInfo borderBase tintMedium\"><div>" + Resources.Resource.PendingTitle + "</div></div>");
                }
                sb.Append("</a>");
                sb.Append("</td>");

                sb.Append("<td>");
                if (!EmployeeInfo.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    sb.Append("<div>");
                    sb.AppendFormat("<a class=\"linkHeaderLight\" href=\"{0}\" title=\"{1}\">{1}</a>", EmployeeUrl, EmployeeInfo.DisplayUserName());
                    sb.Append("</div>");
                    
                    //department
                    sb.Append("<div style=\"padding-top: 6px;\">");
                    if (EmployeeInfo.Status == EmployeeStatus.Terminated)                    
                        sb.Append(HttpUtility.HtmlEncode(EmployeeInfo.Department) ?? "");                    
                    else
                    {
                        sb.AppendFormat("<a href=\"{0}\">", EmployeeDepartmentUrl);
                        sb.Append(HttpUtility.HtmlEncode(EmployeeInfo.Department) ?? "");
                        sb.Append("</a>");
                    }
                    sb.Append("&nbsp;</div>");

                    sb.Append("<div style=\"padding-top: 6px;\">");
                    sb.Append(HttpUtility.HtmlEncode(EmployeeInfo.Title) ?? "");
                    sb.Append("&nbsp;</div>");
                }
                
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table></div>");                
                writer.Write(sb.ToString());
            }

            base.Render(writer);
        }        
    }
}