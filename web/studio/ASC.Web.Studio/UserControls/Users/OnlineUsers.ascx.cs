using System;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class OnlineUsers : System.Web.UI.UserControl
    {
        private Guid productID = Guid.Empty;
        public Guid ProductId
        {
            private get { return productID; }
            set { productID = value; }
        }

        public static string Location
        {
            get { return "~/UserControls/Users/OnlineUsers.ascx"; }
        }

        protected string RenderProfileLink(UserInfo userInfo)
        {
            return userInfo.RenderProfileLink(ProductId);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            rpOnlineUsers.DataSource = UserOnlineManager.Instance.OnlineUsers;
            rpOnlineUsers.DataBind();
        }
    }
}