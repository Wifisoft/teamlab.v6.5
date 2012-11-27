using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Security.Cryptography;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AdminSettingsController")]
    public partial class AdminSettings : System.Web.UI.UserControl
    {
        private UserSelector _adminSelector;
        public Guid ProductID;

        protected UserInfo _owner = null;
        protected bool _canOwnerEdit;

        public static string Location { get { return "~/UserControls/Management/AdminSettings/AdminSettings.ascx"; } }       
        
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "adminsettings_script", WebPath.GetPath("usercontrols/management/adminsettings/js/adminsettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "adminsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/adminsettings/css/<theme_folder>/adminsettings.css") + "\">", false);

            _adminSelector = LoadControl(UserSelector.Location) as UserSelector;
            _adminSelector.BehaviorID = "adminSelector";

            var users = CoreContext.UserManager.GetUsersByGroup(ASC.Core.Users.Constants.GroupAdmin.ID).SortByUserName();

            var repeater = (AdminRepeater)this.LoadControl(AdminRepeater.Location);            
            repeater.BindData(GetAdminsData(users));
            _adminListHolder.Controls.Add(repeater);

            _adminSelector.SelectedUserListTitle = Resources.Resource.Admins;
            _adminSelector.Title = Resources.Resource.AdminSelectorTitle;

            _adminSelector.SelectedUsers.AddRange(users.ConvertAll<Guid>(u => u.ID));
            _adminSelector.DisabledUsers.Add(SecurityContext.CurrentAccount.ID);

            
            var curTenant = CoreContext.TenantManager.GetCurrentTenant();
            _canOwnerEdit = curTenant.OwnerId.Equals(SecurityContext.CurrentAccount.ID);
            _adminSelector.DisabledUsers.Add(curTenant.OwnerId);

            _userSelectorHolder.Controls.Add(_adminSelector);

            //owner
            _owner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);
                      
             users.ForEach(usr =>
             {
                 if (!usr.ID.Equals(_owner.ID))
                 {
                     var item = new System.Web.UI.WebControls.ListItem(usr.DisplayUserName(), usr.ID.ToString());
                     item.Selected = curTenant.OwnerId.Equals(usr.ID);
                     _ownerSelector.Items.Add(item);
                 }
             });
            
            
        }

        private  List<object> GetAdminsData(List<UserInfo> users)
        {
            var data = new List<object>();
            foreach (var ui in users)
            {
                GroupInfo dep = ui.GetUserDepartment();
                data.Add(new
                {
                    UserName = ui.DisplayUserName(),
                    UserPost = EncodeElement(ui.Title),
                    UserUrl = CommonLinkUtility.GetUserProfile(ui.ID, ProductID),
                    PhotoUrl = ui.GetSmallPhotoURL(),
                    DepName = (dep == null ? "" : EncodeElement(dep.Name)),
                    DepUrl = (dep == null ? "" : CommonLinkUtility.GetDepartment(ProductID, dep.ID))                    
                });
            }
            return data;                
        }

        private static string EncodeElement(string el)
        {
            return string.IsNullOrEmpty(el) ? "&nbsp;" : el.HtmlEncode();
        }

        private string RenderAdminList()
        {                        
            var users = CoreContext.UserManager.GetUsersByGroup(ASC.Core.Users.Constants.GroupAdmin.ID).SortByUserName();

            var page = new Page();
            var form = new HtmlForm();
            page.Controls.Add(form);
            var repeater = (AdminRepeater)page.LoadControl(AdminRepeater.Location);
            form.Controls.Add(repeater);
            repeater.BindData(GetAdminsData(users));

           
             StringBuilder sb = new StringBuilder();
             repeater.RenderControl(new System.Web.UI.HtmlTextWriter(new StringWriter(sb)));
             return sb.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveAdminList(List<Guid> adminIDs)
        {           
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var admins = new List<UserInfo>(CoreContext.UserManager.GetUsersByGroup(ASC.Core.Users.Constants.GroupAdmin.ID));

                adminIDs.ForEach(aid =>
                {
                    if (admins.Find(admin => admin.ID.Equals(aid)) == null)
                        CoreContext.UserManager.AddUserIntoGroup(aid, ASC.Core.Users.Constants.GroupAdmin.ID);
                });

                admins.ForEach(a =>
                {
                    if (adminIDs.Find(aid => aid.Equals(a.ID)) == Guid.Empty && a.ID != SecurityContext.CurrentAccount.ID)
                        CoreContext.UserManager.RemoveUserFromGroup(a.ID, ASC.Core.Users.Constants.GroupAdmin.ID);
                });

                return new {Status = 1, Message = RenderAdminList()};
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }            
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object ChangeOwner(Guid ownerId)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var curTenant = CoreContext.TenantManager.GetCurrentTenant();
                var owner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);
                if (curTenant.OwnerId.Equals(SecurityContext.CurrentAccount.ID) && !Guid.Empty.Equals(ownerId))
                {
                    StudioNotifyService.Instance.SendMsgConfirmChangeOwner(curTenant,
                                                                            CoreContext.UserManager.GetUsers(ownerId).DisplayUserName(),
                                                                           GetConfirmLink(ownerId, owner.Email));

                    var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", owner.Email);
                    return new { Status = 1, Message = Resources.Resource.ChangePortalOwnerMsg.Replace(":email", emailLink) };
                }
                else
                    return new { Status = 0, Message = Resources.Resource.ErrorAccessDenied };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }          
        }


        private string GetConfirmLink(Guid newOwnerId, string email)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email.ToLower() + ConfirmType.PortalOwnerChange.ToString().ToLower() + newOwnerId.ToString());

            return CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx") +
                string.Format("?type={0}&email={1}&key={2}&uid={3}", ConfirmType.PortalOwnerChange.ToString().ToLower(),HttpUtility.UrlEncode(email), validationKey,
                newOwnerId.ToString());
        }
    }
}