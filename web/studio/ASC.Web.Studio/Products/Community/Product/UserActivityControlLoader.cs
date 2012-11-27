using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.Activity;

namespace ASC.Web.Community.Product
{
    internal class UserActivityControlLoader : IUserActivityControlLoader
    {
        private class UAControlViewer : WebControl
        {
            private Guid _userID;
            public UAControlViewer(Guid userID)
            {
                _userID = userID;
            }

            protected override void OnLoad(EventArgs e)
            {
                this.Controls.Add(new Literal(){Text = "<div style=height:20px;>&nbsp;</div>"});

                var recentActivity = (RecentUserActivity)this.Page.LoadControl(RecentUserActivity.Location);
                recentActivity.UserID = _userID;
                recentActivity.ProductID = CommunityProduct.ID;
                this.Controls.Add(recentActivity);
                base.OnLoad(e);
            }
        }


        #region IUserActivityControlLoader Members

        public Control LoadControl(Guid userID)
        {
            return new UAControlViewer(userID);
        }

        #endregion
    }
}
