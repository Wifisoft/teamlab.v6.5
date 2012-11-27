using System;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.UserControls.Common;

namespace ASC.Web.Studio.Masters
{
    public partial class StudioTemplate : System.Web.UI.MasterPage, IStudioMaster
    {
        public TopNavigationPanel TopNavigationPanel;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            TopNavigationPanel = (TopNavigationPanel) LoadControl(TopNavigationPanel.Location);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _topNavigatorPlaceHolder.Controls.Add(TopNavigationPanel);

            var bottomNavigator = new BottomNavigator();
            _bottomNavigatorPlaceHolder.Controls.Add(bottomNavigator);
        }

        #region IStudioMaster Members

        public PlaceHolder ContentHolder
        {
            get { return _contentHolder; }
        }

        public PlaceHolder SideHolder
        {
            get { return _sideHolder; }
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


        public PlaceHolder TitleHolder
        {
            get { return (this.Master as IStudioMaster).TitleHolder; }
        }

        public PlaceHolder FooterHolder
        {
            get { return (this.Master as IStudioMaster).FooterHolder; }
        }


        #endregion
    }

}