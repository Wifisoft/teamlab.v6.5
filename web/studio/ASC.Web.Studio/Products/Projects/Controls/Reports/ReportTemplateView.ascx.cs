using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Configuration;
using System.Text;
using ASC.Core.Tenants;
using ASC.Web.Projects.Resources;
using System.Collections.Generic;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Studio.UserControls.Users;
using ASC.Projects.Core.Domain;
using ASC.Core;
using ASC.Web.Projects.Classes;
using ASC.Core.Users;

namespace ASC.Web.Projects.Controls.Reports
{
    public partial class ReportTemplateView : BaseUserControl
    {
        public ReportTemplate Template { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            report_template_container.Options.IsPopup = true;
        }

        public string InitPeriodsDdl()
        {
            var sb = new StringBuilder()
                .AppendFormat("<option value='0' id='ddlPeriodOfGeneration0' selected='selected'>{0}</option>", ReportResource.EveryDay)
                .AppendFormat("<option value='1' id='ddlPeriodOfGeneration1'>{0}</option>", ReportResource.EveryWeek)
                .AppendFormat("<option value='2' id='ddlPeriodOfGeneration2'>{0}</option>", ReportResource.EveryMonth);

            return sb.ToString();
        }

        public string InitPeriodItemsDdl()
        {
            var sb = new StringBuilder();

            string value = string.Empty;
            for (int i = 0; i < 24; i++)
            {
                if (i < 10)
                    value = string.Format("0{0}:00", i.ToString());
                else value = string.Format("{0}:00", i.ToString());

                if (i == 12)
                    sb.AppendFormat("<option value='{0}' id='ddlPeriodItems{0}' selected='selected'>{1}</option>", i, value);
                else
                    sb.AppendFormat("<option value='{0}' id='ddlPeriodItems{0}'>{1}</option>", i, value);
            }

            return sb.ToString();
        }

        public string InitHoursDdl()
        {
            var sb = new StringBuilder();

            string value = string.Empty;
            for (int i = 0; i < 24; i++)
            {
                if (i < 10)
                    value = string.Format("0{0}:00", i.ToString());
                else value = string.Format("{0}:00", i.ToString());

                if (i == 12)
                    sb.AppendFormat("<option value='{0}' id='ddlHours{0}' selected='selected'>{1}</option>", i, value);
                else
                    sb.AppendFormat("<option value='{0}' id='ddlHours{0}'>{1}</option>", i, value);
            }

            return sb.ToString();
        }

        public string GetHeaderTitle()
        {
            if (HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("reports.aspx") > 0)
                return ReportResource.CrteateTemplate;
            return ReportResource.EditTemplate;
        }

    }
}