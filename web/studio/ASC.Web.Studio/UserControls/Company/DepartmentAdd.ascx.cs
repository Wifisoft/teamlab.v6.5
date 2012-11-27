using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Company
{
    public partial class DepartmentAdd : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Company/DepartmentAdd.ascx"; } }

        public Guid ProductID { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            _departmentAddContainer.Options.IsPopup = true;
            _depProductID.Value = ProductID.ToString();
            _departmentAddContainer.Options.InfoMessageText = "";
            _departmentAddContainer.Options.InfoType = ASC.Web.Controls.InfoType.Alert;
        }

        public IEnumerable<UserInfo> GetSortedUsers()
        {
            return CoreContext.UserManager.GetUsers().SortByUserName();
        }
    }
}