using System;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;
using System.Web.UI;

namespace ASC.Web.Community
{

    public partial class CommunityMasterPage : MasterPage, IStudioMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var topNavPanel = (TopNavigationPanel) LoadControl(TopNavigationPanel.Location);

            var dashboardItem = new NavigationItem()
                                    {
                                        Name = Resources.CommunityResource.Dashboard,
                                        URL = VirtualPathUtility.ToAbsolute("~/products/community/"),
                                        Selected = (this.Page is _Default)
                                    };

            topNavPanel.NavigationItems.Add(dashboardItem);


            var employeesItem = new NavigationItem()
                                    {
                                        Name = CustomNamingPeople.Substitute<Resources.CommunityResource>("Employees"),
                                        Selected = UserOnlineManager.Instance.IsEmployeesPage() || UserOnlineManager.Instance.IsUserProfilePage(),
                                        URL = CommonLinkUtility.GetEmployees(CommunityProduct.ID)
                                    };
            topNavPanel.NavigationItems.Add(employeesItem);

            var product = ProductManager.Instance[CommunityProduct.ID];

            var currentModule = UserOnlineManager.Instance.GetCurrentModule();
            foreach (var item in WebItemManager.Instance.GetSubItems(product.ID))
            {
                var moduleItem = new NavigationItem
                                     {
                                         URL = VirtualPathUtility.ToAbsolute(item.StartURL),
                                         Name = item.Name,
                                         Description = item.Description,
                                     };
                if (currentModule != null && currentModule.ID.Equals(item.ID)) moduleItem.Selected = true;
                topNavPanel.NavigationItems.Add(moduleItem);
            }

            _topNavigatorPlaceHolder.Controls.Add(topNavPanel);

            var bottomNavigator = new BottomNavigator();
            _bottomNavigatorPlaceHolder.Controls.Add(bottomNavigator);

            var onlineUsersControl = (OnlineUsers) LoadControl(OnlineUsers.Location);
            onlineUsersControl.ProductId = CommunityProduct.ID;
            phOnlineUsers.Controls.Add(onlineUsersControl);
        }


        #region IStudioMaster Members

        public PlaceHolder ContentHolder
        {
            get { return this._contentHolder; }
        }

        public PlaceHolder SideHolder
        {
            get { return this._sideHolder; }
        }

        public PlaceHolder TitleHolder
        {
            get { return (this.Master as IStudioMaster).TitleHolder; }
        }

        public PlaceHolder FooterHolder
        {
            get { return (this.Master as IStudioMaster).FooterHolder; }
        }

        public bool DisabledSidePanel
        {
            get { return (this.Master as IStudioMaster).DisabledSidePanel; }
            set { (this.Master as IStudioMaster).DisabledSidePanel = value; }
        }

        public bool? LeftSidePanel
        {
            get { return (this.Master as IStudioMaster).LeftSidePanel; }
            set { (this.Master as IStudioMaster).LeftSidePanel = value; }
        }

        #endregion
    }
}