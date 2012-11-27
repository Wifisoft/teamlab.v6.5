using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class SimpleUserSelector : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Users/SimpleUserSelector/SimpleUserSelector.ascx"; } }

        public SimpleUserSelector()
        {
            AdditionalID = "";
        }
        public Guid UserID { get; set; }
        public string Title { get; set; }
        public string SelectTitle { get; set; }
        public string AdditionalID { get; set; }
        public string SelectJSCallback { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _simpleUserSelectorContainer.Options.IsPopup = true;
            _selectEnterCode.Value = String.Format("StudioSimpleUserSelector.Select('{0}',{1});", AdditionalID, SelectJSCallback);

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "simpleuserselector_script", WebPath.GetPath("usercontrols/users/simpleuserselector/js/simpleuserselector.js"));
        }

        public IEnumerable<UserInfo> GetSortedUsers()
        {
            return CoreContext.UserManager.GetUsers().SortByUserName();
        }

    }
}