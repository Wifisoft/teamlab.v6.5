using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Company;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio
{
    [AjaxNamespace("EmployeeService")]
    public partial class Employee : MainPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            base.SetProductMasterPage();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (this.Master is IStudioMaster)
            {
                var master = this.Master as IStudioMaster;

                //top navigator
                if (this.Master is StudioTemplate)
                {
                    (this.Master as StudioTemplate).TopNavigationPanel.CustomTitle = CustomNamingPeople.Substitute<Resources.Resource>("Employees");
                    (this.Master as StudioTemplate).TopNavigationPanel.CustomTitleURL = CommonLinkUtility.GetEmployees();
                    (this.Master as StudioTemplate).TopNavigationPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("home.png");
                }

                WriteEmployeeActions(this);

                if (SecurityContext.CheckPermissions(Constants.Action_AddRemoveUser))
                    master.SideHolder.Controls.Add(GetEmployeeNavigation());

                var contentControl = (EmployeeViewer)LoadControl(EmployeeViewer.Location);
                master.ContentHolder.Controls.Add(contentControl);

                master.SideHolder.Controls.Add(LoadControl(CompanyNavigation.Location));

                if (contentControl.WhatView == EmployeeViewer.ViewType.Departments)
                {
                    var ceoSelector = (SimpleUserSelector)LoadControl(SimpleUserSelector.Location);
                    ceoSelector.Title = CustomNamingPeople.Substitute<Resources.Resource>("CeoNameEditCaption");
                    ceoSelector.SelectTitle = CustomNamingPeople.Substitute<Resources.Resource>("CEO");
                    ceoSelector.AdditionalID = "ceo";
                    var ceo = CoreContext.UserManager.GetCompanyCEO();
                    ceoSelector.UserID = ceo != null ? ceo.ID : Guid.Empty;
                    ceoSelector.SelectJSCallback = "function(id) { EmployeeService.ChangeCEO(id); window.location.reload(); }";
                    master.ContentHolder.Controls.Add(ceoSelector);
                }
                else if (contentControl.WhatView == EmployeeViewer.ViewType.Department)
                {
                    var managerSelector = (SimpleUserSelector)LoadControl(SimpleUserSelector.Location);
                    managerSelector.Title = CustomNamingPeople.Substitute<Resources.Resource>("DepEditMaster");
                    managerSelector.SelectTitle = CustomNamingPeople.Substitute<Resources.Resource>("DepartmentMaster");
                    managerSelector.AdditionalID = "dep_manager";
                    managerSelector.UserID = CoreContext.UserManager.GetDepartmentManager(contentControl.DepartmentId);
                    managerSelector.SelectJSCallback = "function(id) { EmployeeService.ChangeDepartmentManager(id,'" + contentControl.DepartmentId + "'); window.location.reload(); }";
                    master.ContentHolder.Controls.Add(managerSelector);

                    var depNameControl = (Confirm)LoadControl(Confirm.Location);
                    depNameControl.Title = CustomNamingPeople.Substitute<Resources.Resource>("DepartmentTitle");
                    depNameControl.AdditionalID = "depname";
                    depNameControl.SelectTitle = Resources.Resource.NewTitle;
                    depNameControl.Value = contentControl.Department != null ? HttpUtility.HtmlEncode(contentControl.Department.Name) : "";
                    depNameControl.SelectJSCallback =
                        @"function(name) { 
    
    AjaxPro.onLoading = function(b){
            if(b)
                jq('#studio_confirmDialogdepname').block();
            else
                jq('#studio_confirmDialogdepname').unblock();
        };  

    EmployeeService.ChangeDepartmentName('" + contentControl.DepartmentId + @"',name,function(result){
       if(result.value.rs1 == '0')
            jq('#studio_confirmMessage').html('<div class=\'errorBox\'>'+result.value.rs2+'</div>');
       else{
            window.location.reload(true);
            jq.unblockUI();
       }              
    }); 
}";
                    master.ContentHolder.Controls.Add(depNameControl);

                }
            }
        }

        public static SideNavigator GetEmployeeNavigation()
        {
            var navigator = new SideNavigator();

            navigator.Controls.Add(new NavigationItem()
                                       {
                                           Name = CustomNamingPeople.Substitute<Resources.Resource>("ActiveEmployeesTitle"),
                                           URL = CommonLinkUtility.GetEmployees(CommonLinkUtility.GetProductID())
                                       });

            navigator.Controls.Add(new NavigationItem()
                                       {
                                           Name = CustomNamingPeople.Substitute<Resources.Resource>("DisableEmployeesTitle"),
                                           URL = CommonLinkUtility.GetEmployees(CommonLinkUtility.GetProductID(), EmployeeStatus.Terminated)
                                       });

            navigator.Controls.Add(new NavigationItem()
                                       {
                                           Name =CustomNamingPeople.Substitute<Resources.Resource>("PendingEmployeesTitle"),
                                           URL = CommonLinkUtility.GetEmployees(CommonLinkUtility.GetProductID(), EmployeeActivationStatus.Pending)
                                       });

            

            return navigator;

        }

        public static void WriteEmployeeActions(Page page)
        {
            var master = page.Master as IStudioMaster;
            if (master != null)
            {

                var actionsControl = new SideActions();

                WriteAddDepartmentAction(page, actionsControl);

                WriteNewUserAction(actionsControl);

                WriteAddEmployeesAction(page, actionsControl);

                WriteResendInvitesAction(page, actionsControl);

                if (actionsControl.Controls.Count > 0)
                {
                    UserMaker.AddOnlyOne(page, actionsControl);
                    master.SideHolder.Controls.Add(actionsControl);
                }

            }
        }

        //user import
        private static void WriteAddEmployeesAction(Page page, SideActions actionsControl)
        {
            if (SecurityContext.CheckPermissions(Constants.Action_AddRemoveUser))
            {
                actionsControl.Controls.Add(new ImportUsersWebControl());

                const string onclickJavascript = "ImportUsersManager.ShowImportControl();";

                var importUsersHtml = String.Format(@"<a href='javascript:void(0);' class='linkAction{0}' onclick=""{2} return false;"">{1}</a>",
                                                    (SetupInfo.WorkMode == WorkMode.Promo) ? " promoAction" : string.Empty,
                                                    CustomNamingPeople.Substitute<Resources.Resource>("AddEmployeesButton").HtmlEncode(),
                                                    onclickJavascript);

                actionsControl.Controls.Add(new HtmlMenuItem(importUsersHtml));
            }
        }

        //new department
        private static void WriteAddDepartmentAction(Page page, SideActions actionsControl)
        {
            if (SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_EditGroups))
            {
                var depAddControl = (DepartmentAdd)page.LoadControl(DepartmentAdd.Location);
                depAddControl.ProductID = CommonLinkUtility.GetProductID();
                actionsControl.Controls.Add(depAddControl);
                actionsControl.Controls.Add(new NavigationItem()
                                                {
                                                    Name = CustomNamingPeople.Substitute<Resources.Resource>("RegistryNewDepartmentButton"),
                                                    URL = "javascript:StudioManagement.AddDepartmentOpenDialog();",
                                                    IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                                                });
            }
        }

        //new user
        private static void WriteNewUserAction(SideActions actionsControl)
        {
            if (SecurityContext.CheckPermissions(Constants.Action_AddRemoveUser))
            {
                actionsControl.Controls.Add(new NavigationItem
                                                {
                                                    Name = CustomNamingPeople.Substitute<Resources.Resource>("RegistryNewEmployeeButton"),
                                                    URL = "javascript:StudioUserMaker.ShowNewUserDialog();",
                                                    IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                                                });
            }
        }

        //resend invites to pending users
        private static void WriteResendInvitesAction(Page page, SideActions actionsControl)
        {
            if (SecurityContext.CheckPermissions(Constants.Action_AddRemoveUser))
            {
                var users = new List<UserInfo>(CoreContext.UserManager.GetUsers()).FindAll(u => u.ActivationStatus == EmployeeActivationStatus.Pending);
                if (users.Count > 0)
                {

                    var control = page.LoadControl(ResendInvitesControl.Location);                    
                    actionsControl.Controls.Add(control);

                    actionsControl.Controls.Add(new NavigationItem
                    {
                        Name = CustomNamingPeople.Substitute<Resources.Resource>("ResendInvitesToPendingUsersAction"),
                        URL = ResendInvitesControl.GetHrefAction(),
                        IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                    });
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
        }

        #region ajax methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse ChangeDepartmentName(Guid depID, string newName)
        {
            var resp = new AjaxResponse();
            resp.rs1 = "0";
            if (String.IsNullOrEmpty(newName.Trim()))
            {
                resp.rs2 = Resources.Resource.ErrorEmptyName;
            }
            else
            {
                try
                {
                    SecurityContext.DemandPermissions(Constants.Action_EditGroups);

                    UserManagerWrapper.RenameDepartment(depID, newName);
                    resp.rs1 = "1";
                    resp.rs3 = HttpUtility.HtmlEncode(newName);
                }
                catch (Exception exc)
                {
                    resp.rs2 = HttpUtility.HtmlEncode(exc.Message);
                }
            }
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse ChangeCEO(Guid userID)
        {
            var resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

                if (userID == Guid.Empty || !CoreContext.UserManager.UserExists(userID))
                {
                    CoreContext.UserManager.SetCompanyCEO(Guid.Empty);
                }
                else
                {
                    UserInfo ceo = CoreContext.UserManager.GetCompanyCEO();
                    if (ceo == null || ceo.ID != userID)
                        CoreContext.UserManager.SetCompanyCEO(userID);
                }
                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class='errorBox'>" + HttpUtility.HtmlEncode(e.Message) + "</div>";
            }
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse ChangeDepartmentManager(Guid userID, Guid departmentID)
        {
            var resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

                if (userID == Guid.Empty || !CoreContext.UserManager.UserExists(userID))
                {
                    var managerID = CoreContext.UserManager.GetDepartmentManager(departmentID);
                    if (!managerID.Equals(Guid.Empty))
                        CoreContext.UserManager.SetDepartmentManager(departmentID, Guid.Empty);
                }
                else
                {
                    CoreContext.UserManager.SetDepartmentManager(departmentID, userID);

                    var manager = CoreContext.UserManager.GetUsers(userID);
                    if (manager.GetUserDepartment() == null)
                        UserManagerWrapper.TransferUser2Department(manager.ID, departmentID);
                }

                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class='errorBox'>" + HttpUtility.HtmlEncode(e.Message) + "</div>";
            }
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse AddDepartment(Guid pid, string name, Guid managerID)
        {
            var resp = new AjaxResponse();
            resp.rs1 = "0";
            if (String.IsNullOrEmpty(name.Trim()))
            {
                resp.rs2 = Resources.Resource.ErrorEmptyName;
            }
            else
            {
                try
                {
                    SecurityContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

                    var newDep = CoreContext.GroupManager.SaveGroupInfo(new GroupInfo { Name = name });

                    if (CoreContext.UserManager.UserExists(managerID))
                    {
                        CoreContext.UserManager.SetDepartmentManager(newDep.ID, managerID);
                        var manager = CoreContext.UserManager.GetUsers(managerID);
                        if (manager.GetUserDepartment() == null)
                            UserManagerWrapper.TransferUser2Department(manager.ID, newDep.ID);

                    }

                    resp.rs3 = CommonLinkUtility.GetDepartment(pid, newDep.ID);
                    resp.rs1 = "1";
                }
                catch (Exception exc)
                {
                    resp.rs1 = "0";
                    resp.rs2 = exc.Message;
                }
            }
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse DeleteDepartment(string id)
        {
            var resp = new AjaxResponse { rs1 = "1" };
            try
            {
                SecurityContext.DemandPermissions(Constants.Action_EditGroups);
                CoreContext.GroupManager.DeleteGroup(new Guid(id));
            }
            catch (Exception exc)
            {
                resp.rs1 = "0";
                resp.rs2 = exc.Message;
            }
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse TransferUser2Department(Guid departmentID, List<Guid> userIDs)
        {
            var resp = new AjaxResponse();
            resp.rs1 = "0";
            try
            {
                SecurityContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

                var users = new List<UserInfo>(CoreContext.UserManager.GetUsersByGroup(departmentID));

                userIDs.ForEach(uid =>
                                    {
                                        if (users.Find(user => user.ID.Equals(uid)) == null)
                                            UserManagerWrapper.TransferUser2Department(uid, departmentID);
                                    });

                users.ForEach(user =>
                                  {
                                      if (userIDs.Find(uid => user.ID.Equals(uid)) == Guid.Empty)
                                      {
                                          user.Department = string.Empty;
                                          CoreContext.UserManager.RemoveUserFromGroup(user.ID, departmentID);
                                          UserManagerWrapper.SaveUserInfo(user);

                                      }
                                  });

                resp.rs1 = "1";

            }
            catch (Exception exc)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(exc.Message);
            }


            return resp;
        }

        #endregion
    }
}