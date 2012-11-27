using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Forum;
using ASC.Web.Controls;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum;

namespace ASC.Web.Community.Forum
{
    public partial class ForumMasterPage : System.Web.UI.MasterPage
    {
        public string SearchText { get; set; }

        public PlaceHolder ActionsPlaceHolder { get; set; }

        public List<BreadCrumb> BreadCrumbs
        {
            get
            {
                if (ForumContainer.BreadCrumbs == null)
                    ForumContainer.BreadCrumbs = new List<BreadCrumb>();
                return ForumContainer.BreadCrumbs;
            }
        }

        public ForumMasterPage()
        {
            ActionsPlaceHolder = new PlaceHolder();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _scriptProvider.SettingsID = ForumManager.Settings.ID;
            if (this.Page is NewPost || this.Page is EditTopic)
                _scriptProvider.RegistrySearchHelper = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ForumContainer.Options.InfoType = InfoType.Alert;

            StringBuilder sb = new StringBuilder();
            sb.Append("<script language=\"javascript\"  type=\"text/javascript\"> ForumMakerProvider.All='" + Resources.ForumResource.All + "'; ");
            sb.Append(" ForumMakerProvider.ConfirmMessage='" + Resources.ForumResource.ConfirmMessage + "'; ");
            sb.Append(" ForumMakerProvider.SaveButton='" + Resources.ForumResource.SaveButton + "'; ");
            sb.Append(" ForumMakerProvider.CancelButton='" + Resources.ForumResource.CancelButton + "'; ");
            sb.Append(" ForumMakerProvider.NameEmptyString='" + Resources.ForumResource.NameEmptyString + "'; ");
            sb.Append(" ForumContainer_PanelInfoID = '" + ForumContainer.GetInfoPanelClientID() + "'; ");

            sb.Append("</script>");

            this.Page.ClientScript.RegisterClientScriptInclude(typeof(string), "forummaker_script", WebPath.GetPath(ForumManager.BaseVirtualPath.Substring(2) + "/js/forummaker.js"));
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "forummaker_script_init", sb.ToString(), false);

            SearchText = "";

            if (!String.IsNullOrEmpty(Request["search"]))
                SearchText = Request["search"];

            var tags = ForumDataProvider.GetTagCloud(TenantProvider.CurrentTenantID, 40);
            TabCloudContainer.Title = Resources.ForumResource.TagCloud;
            TabCloudContainer.HeaderCSSClass = "studioSideBoxTagCloudHeader";
            TabCloudContainer.ImageURL = WebImageSupplier.GetAbsoluteWebPath("tagcloud.png");

            foreach (RankTag tag in tags)
            {
                TagCloudItem item = new TagCloudItem();
                item.TagName = tag.Name;
                item.TagID = tag.ID.ToString();
                item.Rank = tag.Rank;
                item.URL = "search.aspx?tag=" + tag.ID;
                tagCloud.Items.Add(item);
            }
            if (tags.Count == 0)
                TabCloudContainer.Visible = false;

            sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
            sideRecentActivity.ProductId = Product.CommunityProduct.ID;
            sideRecentActivity.ModuleId = Forum.ForumManager.ModuleID;


            string script = "<link href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath(ForumManager.BaseVirtualPath.Substring(2) + "/app_themes/<theme_folder>/style.css") + "\" rel=\"stylesheet\" type=\"text/css\" />";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), Guid.NewGuid().ToString(), script);

            SetNavigation();
        }

        protected void SetNavigation()
        {
            var currentModule = UserOnlineManager.Instance.GetCurrentModule() as Module;
            if (currentModule == null) return;

            var actionsControl = new SideActions();

            if (ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessForumEditor, null))
            {
                _forumMakerHolder.Controls.Add(LoadControl(ForumMaker.Location));
                string actionURL = "";
                if (this.Page is ASC.Web.Community.Forum.Default)
                    actionURL = "javascript:ForumMakerProvider.ShowForumMakerDialog(false,'window.location.reload(true)');";

                else if (this.Page is ManagementCenter)
                {
                    if (String.IsNullOrEmpty(Request["type"]) || Request["type"] == "0")
                        actionURL = "javascript:ForumMakerProvider.ShowForumMakerDialog(true);";

                    else
                        actionURL = "javascript:ForumMakerProvider.ShowForumMakerDialog();";
                }

                else
                    actionURL = "javascript:ForumMakerProvider.ShowForumMakerDialog();";


                actionsControl.Controls.Add(new NavigationItem()
                {
                    Name = Resources.ForumResource.AddThreadCategoryButton,
                    Description = "",
                    URL = actionURL,
                    IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                });

            }

            if (currentModule.Actions.Any() || ActionsPlaceHolder.Controls.Count > 0)
            {
                foreach (var shortcut in currentModule.Actions)
                {
                    actionsControl.Controls.Add(new NavigationItem()
                    {
                        Name = shortcut.Name,
                        Description = shortcut.Description,
                        URL = shortcut.StartURL,
                        IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                    });
                }

            }

            Utility.RegisterTypeForAjax(typeof(Subscriber));
            var subscriber = new Subscriber();
            var isNewTopicSubscribe = subscriber.IsNewTopicSubscribe();
            this.ActionsPlaceHolder.Controls.AddAt(0, new HtmlMenuItem(subscriber.RenderNewTopicSubscription(!isNewTopicSubscribe)));

            if (ActionsPlaceHolder.Controls.Count > 0)
                actionsControl.Controls.Add(ActionsPlaceHolder);

            if (actionsControl.Controls.Count > 0)
                _actionHolder.Controls.Add(actionsControl);

            if (currentModule.Navigations.Any())
            {
                var navigationControl = new SideNavigator();
                foreach (var shortcut in currentModule.Navigations)
                {
                    navigationControl.Controls.Add(new NavigationItem()
                    {
                        Name = shortcut.Name,
                        Description = shortcut.Description,
                        URL = shortcut.StartURL
                    });
                }

                _actionHolder.Controls.Add(navigationControl);
            }
        }
    }
}
