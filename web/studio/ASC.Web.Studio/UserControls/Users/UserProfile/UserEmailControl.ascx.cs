using System;
using ASC.Core.Users;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    public partial class UserEmailControl : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Users/UserProfile/UserEmailControl.ascx"; } }

        /// <summary>
        /// The user represented by the control
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// The user who is viewing the page
        /// </summary>
        public UserInfo Viewer { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(EmailOperationService));
        }
    }
}