using System;
using System.Collections.Generic;

namespace ASC.Web.Studio.UserControls.Users.Activity
{
    public partial class UserActivity : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/Activity/UserActivity.ascx"; }
        }

        public List<UserContentActivity> Activities { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                //lblNoActivities.Text = Resources.Resource.NoActivities;
                BindData();
            }
        }

        public void BindData()
        {
            if(Activities.Count > 0)
            {
                rpUserActivity.DataSource = Activities;
                rpUserActivity.DataBind();
            }
            else
            {
                lblNoActivities.Visible = true;
                rpUserActivity.Visible = false;
            }
            
        }
    }
}