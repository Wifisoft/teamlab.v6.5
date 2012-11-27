using System;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users.Activity
{
    public partial class RecentUserActivity : System.Web.UI.UserControl
    {
        public Guid UserID { get; set; }
        public Guid ProductID { get; set; }

        public static string Location
        {
            get { return "~/UserControls/Users/Activity/RecentUserActivity.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ltRecentActivity.Text = Resources.Resource.RecentActivity;
            uaRecentActivity.Activities = UserActivityManager.GetUserActivities(TenantProvider.CurrentTenantID, UserID, ProductID, null, UserActivityConstants.AllActionType, null, 0, 10)
                                          .ConvertAll<UserContentActivity>(a => new UserContentActivity(a));
        }
    }
}