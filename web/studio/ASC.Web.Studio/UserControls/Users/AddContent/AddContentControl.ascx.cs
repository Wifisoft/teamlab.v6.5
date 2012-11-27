using System;
using System.Collections.Generic;
using System.Web;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class AddContentControl : System.Web.UI.UserControl
    {
      public sealed class ContentTypes
      {
          public string Link { get; set; }
          public string Icon { get; set; }
          public string Label { get; set; }
        }

        public static string Location
        {
          get { return "~/UserControls/Users/AddContent/AddContentControl.ascx"; }
        }

        public List<ContentTypes> Types { get; set; }

        public AddContentControl()
        {
            Types = new List<ContentTypes>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "addcontent_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/addcontent/css/<theme_folder>/addcontentcontrol_style.css") + "\">", false);

          AddContentContainer.Options.IsPopup = true;


          ContentTypesRepeater.DataSource = Types;
          ContentTypesRepeater.DataBind();
        }
    }
}
