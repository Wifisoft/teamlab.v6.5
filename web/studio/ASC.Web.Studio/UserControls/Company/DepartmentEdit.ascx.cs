using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users;

namespace ASC.Web.Studio.UserControls.Company
{
    public partial class DepartmentEdit : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Company/DepartmentEdit.ascx"; } }

        public Guid DepId
        {
            get
            {
                Guid result = Guid.Empty;
                try
                {
                    result = new Guid(ViewState["DepId"].ToString());
                }
                catch { }

                return result;
            }
            set
            {
                ViewState["DepId"] = value;
            }
        }

        protected Guid _productID;
        private UserSelector _userSelector;

        protected void Page_Load(object sender, EventArgs e)
        {
            _productID = CommonLinkUtility.GetProductID();

            _userSelector = Page.LoadControl(UserSelector.Location) as UserSelector;
            _userSelector.BehaviorID = "empSelector";
            var depName = CoreContext.GroupManager.GetGroupInfo(DepId).Name;
            if (depName.Length > 30)
                depName = depName.Substring(0, 27) + "...";
            _userSelector.SelectedUserListTitle = depName;

            if (!Page.IsPostBack)
                BindUserList();

            _userSelectorHolder.Controls.Add(_userSelector);
        }


        private void BindUserList()
        {
            var master = CoreContext.UserManager.GetUsers((CoreContext.UserManager.GetDepartmentManager(DepId))); 

            ucMasterUserCard.EmployeeInfo = master;
            ucMasterUserCard.EmployeeUrl = CommonLinkUtility.GetUserProfile(master.ID, _productID);
            ucMasterUserCard.EmployeeDepartmentUrl = CommonLinkUtility.GetUserDepartment(master.ID, _productID);
            ucMasterUserCard.Height = new Unit("100%");


            List<UserInfo> list = new List<UserInfo>();
            list.AddRange(CoreContext.UserManager.GetUsersByGroup(DepId));
            list = list.SortByUserName();

            _userSelector.SelectedUsers.AddRange(list.ConvertAll<Guid>(u => u.ID));

            rptEmployeesList.DataSource = list;
            rptEmployeesList.DataBind();
        }


        protected string GetEmployeeUrl(UserInfo ui)
        {
            return ui.GetUserProfilePageURL(CommonLinkUtility.GetProductID());
        }

        protected string GetEmployeeName(UserInfo ui)
        {
            return ui.DisplayUserName();
        }

        protected string GetContainGroupCaption()
        {
            return string.Format(Resources.Resource.DepartmentHeadCount, CoreContext.UserManager.GetUsersByGroup(DepId).Length);
        }

        protected bool CanEditDepartment()
        {
            return SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_EditGroups);
        }
    }
}