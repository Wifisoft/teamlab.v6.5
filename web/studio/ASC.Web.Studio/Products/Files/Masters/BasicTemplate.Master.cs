using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Controls;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Personal;

namespace ASC.Web.Files.Masters
{
    public partial class BasicTemplate : MasterPage, IStudioMaster
    {
        #region Methods

        public List<BreadCrumb> BreadCrumbs
        {
            get { return _commonContainer.BreadCrumbs ?? (_commonContainer.BreadCrumbs = new List<BreadCrumb>()); }
        }

        protected void InitControls()
        {
            var searchHandler = (BaseSearchHandlerEx) (SearchHandlerManager.GetHandlersExForProduct(ProductEntryPoint.ID)).Find(sh => sh is SearchHandler);

            if (searchHandler != null)
            {
                searchHandler.AbsoluteSearchURL = VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "/search.aspx");
            }

            RenderHeader();

            var bottomNavigator = new BottomNavigator();
            _bottomNavigatorPlaceHolder.Controls.Add(bottomNavigator);
        }


        #endregion
        

        protected void RenderHeader()
        {
            var topNavigationPanel = (TopNavigationPanel) LoadControl(TopNavigationPanel.Location);

            topNavigationPanel.SingleSearchHandlerType = typeof (SearchHandler);
            _topNavigationPanelPlaceHolder.Controls.Add(topNavigationPanel);

            //for personal
            if (SetupInfo.IsPersonal)
            {
                PersonalHelper.AdjustTopNavigator(topNavigationPanel,
                                        ASC.Web.Studio.Personal.PersonalPart.WebItem, ProductEntryPoint.ID);
            }            
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();

            var onlineUsersControl = (OnlineUsers) LoadControl(OnlineUsers.Location);
            onlineUsersControl.ProductId = ProductEntryPoint.ID;
            phOnlineUsers.Controls.Add(onlineUsersControl);
        }

        #region Overrides of PublisherMasterPage

        public bool CommonContainerHeaderVisible
        {
            get { return _commonContainer.Options.HeadStyle.Contains("display:none"); }
            set { _commonContainer.Options.HeadStyle = value ? String.Empty : "display:none"; }
        }

        #endregion

        #region Implementation of IStudioMaster

        public PlaceHolder ContentHolder
        {
            get
            {
                _commonContainer.Visible = false;
                return _contentHolder;
            }
        }

        public PlaceHolder SideHolder
        {
            get { return (Master as IStudioMaster).SideHolder; }
        }

        public PlaceHolder TitleHolder
        {
            get { return (Master as IStudioMaster).TitleHolder; }
        }

        public PlaceHolder FooterHolder
        {
            get { return (Master as IStudioMaster).FooterHolder; }
        }


        public bool DisabledSidePanel
        {
            get { return (Master as IStudioMaster).DisabledSidePanel; }
            set { (Master as IStudioMaster).DisabledSidePanel = value; }
        }

        public bool? LeftSidePanel
        {
            get { return (Master as IStudioMaster).LeftSidePanel; }
            set { (Master as IStudioMaster).LeftSidePanel = value; }
        }

        #endregion
    }
}