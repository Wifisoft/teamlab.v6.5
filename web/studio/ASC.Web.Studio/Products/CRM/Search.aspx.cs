#region Import

using System;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.CRM.Classes;
using ASC.Data.Storage;

#endregion

namespace ASC.Web.CRM
{
    public partial class Search : BasePage
    {
        #region Property

        public String SearchText { get; set; }
        public int NumberOfStaffFound { get; set; }

        #endregion

        #region Events

        protected override void PageLoad()
        {
            Master.DisabledSidePanel = true;

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "jquery-datepicker", WebPath.GetPath("js/jquery-datepicker.js"));

            SearchText = HttpContext.Current.Request[UrlConstant.Search];
            
            InitEmployeesSearchRepeater();

        }

        public void InitEmployeesSearchRepeater()
        {
            var users = CoreContext.UserManager.Search(SearchText, EmployeeStatus.Active);

            NumberOfStaffFound = users.Length;

            if (NumberOfStaffFound == 0) return;

            EmployeesSearchRepeater.DataSource = NumberOfStaffFound > 5 ? users.Take(5) : users;

            EmployeesSearchRepeater.DataBind();

        }

        #endregion
    }
}