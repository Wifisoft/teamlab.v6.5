using System;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.UserControls.Users;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class SharingSettings : System.Web.UI.UserControl
    {
        protected string bodyCaption = Resources.Resource.SharingSettingsItemsTitle;

        public string BodyCaption
        {
            get { return bodyCaption; }
            set { bodyCaption = value; }
        }

        public static string Location
        {
            get { return "~/UserControls/Common/SharingSettings/SharingSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(typeof (string), "sharing_settings_script", WebPath.GetPath("usercontrols/common/sharingsettings/js/sharingsettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "sharing_settings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/sharingsettings/css/<theme_folder>/sharingsettings.css") + "\">", false);

            _sharingDialogContainer.Options.IsPopup = true;

            shareUserSelector.IsLinkView = true;
            shareUserSelector.LinkText = Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddUsersForSharingButton");

            var shareGroupSelector = (GroupSelector) LoadControl(GroupSelector.Location);
            shareGroupSelector.JsId = "shareGroupSelector";
            shareGroupSelector.WithGroupEveryone = true;
            shareGroupSelector.WithGroupAdmin = true;
            _groupSelectorHolder.Controls.Add(shareGroupSelector);

        }
    }
}