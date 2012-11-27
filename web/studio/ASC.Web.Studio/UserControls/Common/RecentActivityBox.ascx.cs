using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Web.Core.Users.Activity;


namespace ASC.Web.Studio.UserControls.Common
{
	public partial class RecentActivityBox : UserControl
    {
		public static string Location { get { return "~/UserControls/Common/RecentActivityBox.ascx"; } }

		public List<UserActivity> userActivityList { get; set; }

		public int MaxLengthTitle { get; set; }

		public string ItemCSSClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
			repeaterRecentActivity.DataSource = userActivityList;
			repeaterRecentActivity.DataBind();
        }
    }
}