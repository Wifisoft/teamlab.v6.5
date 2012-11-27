using System;
using System.Globalization;
using System.Text;
using ASC.Web.Core.WebZones;
using ASC.Web.Studio.UserControls.Common;

namespace ASC.Web.Calendar.UserControls
{
    public partial class CalendarControl : System.Web.UI.UserControl, IFullScreenControl
    {
        public static string Location
        {
            get { return "~/addons/calendar/usercontrols/calendarcontrol.ascx"; }
        }

        protected string _currentCulture;

        protected void Page_Load(object sender, EventArgs e)
        {
            _currentCulture = CultureInfo.CurrentCulture.Name;

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "calendar_full_screen",
                    @"<style type=""text/css"">
                    #studioPageContent{padding-bottom:0px;}
                    #studio_sidePanelUpHeight20{display:none;}                    
                    </style>", false);

            _sharingContainer.Controls.Add(LoadControl(SharingSettings.Location));
        }

        protected string RenderTimeZones()
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                if (i > 0)
                    sb.Append(",");

                sb.AppendFormat("{{name:\"{0}\", id:\"{1}\", offset:{2}}}", tz.DisplayName, tz.Id, (int) tz.GetUtcOffset(DateTime.UtcNow).TotalMinutes);
                i++;
            }
            return sb.ToString();
        }
    }
}