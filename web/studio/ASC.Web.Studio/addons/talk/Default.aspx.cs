using System;
using System.Web;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;

namespace ASC.Web.Talk
{
	public partial class DefaultTalk : MainPage
	{
		private TalkConfiguration cfg;

		protected void Page_Load(object sender, EventArgs e)
		{
      //верхняя навигационная панель
      var topNavPanel = (TopNavigationPanel)LoadControl(TopNavigationPanel.Location);

      topNavPanel.CustomTitle = Resources.TalkResource.ProductName;
      topNavPanel.CustomTitleURL = VirtualPathUtility.ToAbsolute("~/addons/talk/default.aspx");
      topNavPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("product_logo.png", TalkAddon.AddonID);
      topNavPanel.DisableSearch = true;

      _topNavigatorPlaceHolder.Controls.Add(topNavPanel);

      //нижняя панель
      BottomNavigator bottomNavigator = new BottomNavigator();
      _bottomNavigatorPlaceHolder.Controls.Add(bottomNavigator);

      Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "talk.overview", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("addons/talk/css/<theme_folder>/talk.overview.css") + "\" />", false);

			Title = HeaderStringHelper.GetPageTitle(Resources.TalkResource.ProductName, null, null);
			((IStudioMaster)this.Master).DisabledSidePanel = true;

			cfg = new TalkConfiguration();
		}

		public string ServerAddress
		{
			get { return cfg.ServerAddress; }
		}

		public string ServerName
		{
			get { return cfg.ServerName; }
		}

		public string ServerPort
		{
			get { return cfg.ServerPort; }
		}

		public string UserName
		{
			get { return cfg.UserName; }
		}

		public string JID
		{
			get { return cfg.Jid; }
		}
	}
}
