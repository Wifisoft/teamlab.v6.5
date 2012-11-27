using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel.Security;
using System.Text;
using System.Web;
using System.Xml;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users
{
    [AjaxNamespace("UserProfileControl")]
    public partial class UserProfileControl : System.Web.UI.UserControl
    {
        public class SavePhotoThumbnails : IThumbnailsData
        {

            #region User ID

            private Guid _userID;

            public Guid UserID
            {
                get
                {
                    if (_userID == null || Guid.Empty.Equals(_userID))
                    {
                        _userID = SecurityContext.CurrentAccount.ID;
                    }
                    return _userID;
                }
                set { _userID = value; }
            }

            #endregion

            public Bitmap MainImgBitmap
            {
                get { return UserPhotoManager.GetPhotoBitmap(UserID); }
            }

            public string MainImgUrl
            {
                get { return UserPhotoManager.GetPhotoAbsoluteWebPath(UserID); }
            }

            public List<ThumbnailItem> ThumbnailList
            {
                get
                {
                    List<ThumbnailItem> tmp = new List<ThumbnailItem>();
                    tmp.Add(new ThumbnailItem()
                                {
                                    id = UserPhotoManager.BigFotoSize.ToString(),
                                    size = UserPhotoManager.BigFotoSize,
                                    imgUrl = UserPhotoManager.GetBigPhotoURL(UserID)
                                });
                    tmp.Add(new ThumbnailItem()
                                {
                                    id = UserPhotoManager.BigFotoSize.ToString(),
                                    size = UserPhotoManager.MediumFotoSize,
                                    imgUrl = UserPhotoManager.GetMediumPhotoURL(UserID)
                                });
                    tmp.Add(new ThumbnailItem()
                                {
                                    id = UserPhotoManager.BigFotoSize.ToString(),
                                    size = UserPhotoManager.SmallFotoSize,
                                    imgUrl = UserPhotoManager.GetSmallPhotoURL(UserID)
                                });
                    return tmp;
                }
            }

            public void Save(List<ThumbnailItem> bitmaps)
            {
                foreach (var item in bitmaps)
                    UserPhotoManager.SaveThumbnail(UserID, item.bitmap, MainImgBitmap.RawFormat);
            }
        }

        public UserInfo UserInfo { get; set; }

        public string MainImgUrl
        {
            get { return UserPhotoManager.GetPhotoAbsoluteWebPath(UserInfo.ID); }
        }

        protected bool UserHasAvatar
        {
            get { return !MainImgUrl.Contains("default/images/"); }
        }

        protected bool ShowUserLocation
        {
            get { return UserInfo != null && !string.IsNullOrEmpty(UserInfo.Location) && !string.IsNullOrEmpty(UserInfo.Location.Trim()) && !"null".Equals(UserInfo.Location.Trim(), StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool MyStaffMode { get; set; }

        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/UserProfileControl.ascx"; }
        }

        protected bool _self;
        protected bool _allowDelete;
        protected bool _allowChangePwd;
        protected bool _allowEdit;
        protected bool _allowEditSelf;
        protected bool _isAdmin;

        protected void Page_Load(object sender, EventArgs e)
        {
            _emailChangerContainer.Options.IsPopup = true;
            _deleteProfileContainer.Options.IsPopup = true;
            _changePhoneContainer.Options.IsPopup = true;
            _isAdmin = CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "userprofile_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/userprofile/css/<theme_folder>/userprofilecontrol_style.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "userprofile_script", WebPath.GetPath("usercontrols/users/userprofile/js/userprofilecontrol.js"));
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            _self = SecurityContext.CurrentAccount.ID.Equals(UserInfo.ID);
            _allowDelete = SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser);
            _allowChangePwd = SecurityContext.CheckPermissions(new UserSecurityProvider(SecurityContext.CurrentAccount.ID), ASC.Core.Users.Constants.Action_EditUser);
            _allowEdit = _allowDelete;

            var isOwner = UserInfo.IsOwner();

            if (_self || isOwner)
                _allowDelete = false;

            if (MyStaffMode)
            {
                _allowEditSelf = (SecurityContext.CheckPermissions(new UserSecurityProvider(SecurityContext.CurrentAccount.ID), ASC.Core.Users.Constants.Action_EditUser) && !SecurityContext.DemoMode);
            }
            else if (!MyStaffMode && isOwner && !_self)
            {
                _allowDelete = false;
                _allowChangePwd = false;
                _allowEdit = false;
            }

            if (UserInfo.ActivationStatus == EmployeeActivationStatus.Pending)
                _allowChangePwd = false;


            if (_allowChangePwd || (_self && MyStaffMode))
            {
                _editControlsHolder.Controls.Add(LoadControl(PwdTool.Location));
            }


            if (_allowEdit || _allowEditSelf)
                UserMaker.AddOnlyOne(Page, _editControlsHolder);

            if (!ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
            {
                ThumbnailEditor thumbnailEditorControl = (ThumbnailEditor) LoadControl(ThumbnailEditor.Location);
                thumbnailEditorControl.Title = Resources.Resource.TitleThumbnailPhoto;
                thumbnailEditorControl.BehaviorID = "UserPhotoThumbnail";
                thumbnailEditorControl.JcropMinSize = UserPhotoManager.SmallFotoSize;
                thumbnailEditorControl.JcropAspectRatio = 1;
                thumbnailEditorControl.SaveFunctionType = typeof (SavePhotoThumbnails);
                _editControlsHolder.Controls.Add(thumbnailEditorControl);
            }

            List<Dictionary<String, String>> contacts = new List<Dictionary<String, String>>();
            for (Int32 i = 0, n = UserInfo.Contacts.Count; i < n; i += 2)
            {
                if (i + 1 < UserInfo.Contacts.Count)
                {
                    contacts.Add(GetSocialContact(UserInfo.Contacts[i], UserInfo.Contacts[i + 1]));
                }
            }
            SocialContacts.DataSource = contacts;
            SocialContacts.DataBind();


            if (MyStaffMode && SetupInfo.ThirdPartyAuthEnabled)
            {
                var accountLink = (AccountLinkControl) LoadControl(AccountLinkControl.Location);
                accountLink.ClientCallback = "loginCallback";
                accountLink.SettingsView = true;
                _accountPlaceholder.Controls.Add(accountLink);                
            }

            var emailControl = (UserEmailControl) LoadControl(UserEmailControl.Location);
            emailControl.User = UserInfo;
            emailControl.Viewer = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            _phEmailControlsHolder.Controls.Add(emailControl);
        }

        private Dictionary<String, String> GetSocialContact(String type, String value)
        {
            XmlElement node = SocialContactsManager.xmlSocialContacts.GetElementById(type);
            String title = node != null ? node.Attributes["title"].Value : type;
            String template = node != null ? node.GetElementsByTagName("template")[0].InnerXml : "{0}";

            Dictionary<String, String> contact = new Dictionary<String, String>();
            contact.Add("type", type);
            contact.Add("value", System.Web.HttpUtility.HtmlEncode(value));
            contact.Add("title", title);
            contact.Add("template", template);
            return contact;
        }

        protected string GetContactsName(Dictionary<String, String> contact)
        {
            return contact.ContainsKey("title") ? contact["title"] : "";
        }

        protected string GetContactsLink(Dictionary<String, String> contact)
        {
            if (contact.ContainsKey("value") && contact.ContainsKey("template"))
                return String.Format(contact["template"], contact["value"]);
            return "";
        }

        protected string RenderDepartment()
        {
            if (UserInfo.Status == EmployeeStatus.Terminated)
                return HttpUtility.HtmlEncode(UserInfo.Department ?? "");

            var dep = UserInfo.GetUserDepartment();
            if (dep != null)
                return "<a href=\"" + CommonLinkUtility.GetDepartment(CommonLinkUtility.GetProductID(), dep.ID) + "\">" + HttpUtility.HtmlEncode(dep.Name) + "</a>";

            return HttpUtility.HtmlEncode(UserInfo.Department ?? "");
        }

        protected bool UserIsOnline()
        {
            return UserInfo.IsOnline();
        }

        protected string UserName()
        {
            return UserInfo.UserName;
        }      
       
        protected string RenderEditDelete()
        {
            var sb = new StringBuilder();

            if (UserInfo.Status == EmployeeStatus.Terminated && _allowDelete)
            {
                sb.AppendFormat("<a href=\"javascript:void(0);\" title=\"{2}\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" onclick=\"AuthManager.DisableUser('{1}',false);\" >{0}</a>"
                                , Resources.Resource.EnableUserButton, UserInfo.ID, CustomNamingPeople.Substitute<Resources.Resource>("EnableUserHelp").HtmlEncode());

                sb.Append("<span class='splitter'>|</span>");

                sb.AppendFormat("<a href=\"javascript:void(0);\" onclick=\"AuthManager.RemoveUser('{1}', '{2}');\"; class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" >{0}</a> ", Resources.Resource.DeleteButton, UserInfo.ID, UserInfo.DisplayUserName().ReplaceSingleQuote());
            }
            else
            {
                if (_allowEdit)
                {
                    sb.AppendFormat("<a href=\"javascript:void(0);\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" onclick=\"StudioUserMaker.ShowEditUserDialog('{1}',function(ok){{if(ok.Action!='cancel') window.location.reload(true)}});\" >{0}</a>"
                                    , Resources.Resource.EditButton, UserInfo.ID);
                }

                if (_allowEdit && _allowChangePwd)
                {
                    sb.Append("<span class='splitter'>|</span>");

                    sb.AppendFormat("<a href=\"javascript:void(0);\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" onclick=\"AuthManager.ShowPwdReminderDialog('1','" + UserInfo.Email + "');\" >{0}</a>"
                                    , Resources.Resource.ChangePwd);
                }

                if ((_allowDelete && _allowEdit) ||
                    (_allowDelete && _allowChangePwd))
                {
                    sb.Append("<span class='splitter'>|</span>");

                    sb.AppendFormat("<a href=\"javascript:void(0);\" title=\"{2}\" class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" onclick=\"AuthManager.DisableUser('{1}',true);\" >{0}</a>"
                                    , Resources.Resource.DisableUserButton, UserInfo.ID, CustomNamingPeople.Substitute<Resources.Resource>("DisableUserHelp").HtmlEncode());

                }
                
                if (_allowEdit && UserHasAvatar && !ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
                {
                    sb.Append("<span class='splitter'>|</span>");
                    sb.AppendFormat(@"<a onclick='UserPhotoThumbnail.ShowDialog();' href='javascript:void(0);' class='linkAction{0}'>{1}</a>",
                                    SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : string.Empty, Resources.Resource.EditThumbnailPhoto);
                }
            }

            return sb.ToString();
        }

        #region Ajax
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse RemoveUser(Guid userID)
        {
            AjaxResponse resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser);
                UserPhotoManager.RemovePhoto(Guid.Empty, userID);
                CoreContext.UserManager.DeleteUser(userID);

                resp.rs1 = "1";
                resp.rs2 = Resources.Resource.SuccessfullyDeleteUserInfoMessage;
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SendInstructionsToDelete()
        {
            try
            {
                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                StudioNotifyService.Instance.SendMsgProfileDeletion(user.Email);

                return new { Status = 1, Message = Resources.Resource.SuccessfullySentNotificationDeleteUserInfoMessage };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message };
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse DisableUser(Guid userID, bool disabled)
        {
            AjaxResponse resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(ASC.Core.Users.Constants.Action_EditUser);

                var user = CoreContext.UserManager.GetUsers(userID);
                user.Status = disabled ? EmployeeStatus.Terminated : EmployeeStatus.Active;

                CoreContext.UserManager.SaveUserInfo(user);
                resp.rs1 = "1";

            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse ResendActivation(Guid userID)
        {
            var resp = new AjaxResponse();
            try
            {
                if (CoreContext.UserManager.UserExists(userID))
                {
                    var user = CoreContext.UserManager.GetUsers(userID);
                    if (!user.IsActive)
                    {
                        StudioNotifyService.Instance.UserInfoActivation(user);
                    }
                    resp.rs1 = "1";
                }
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        } 
        #endregion

        [AjaxMethod]
        public object SendNotificationToChange()
        {
            return NotifyUserToChangeMobilePhone(SecurityContext.CurrentAccount.ID, false);
        }

        [AjaxMethod]
        public object SendNotificationToErase(string id)
        {
            if (!CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID))
                throw new SecurityAccessDeniedException("Current user has no permissions to perform the operation");

            Guid userID;
            try
            {
                userID = new Guid(id);

                var usr = CoreContext.UserManager.GetUsers(userID);
                usr.MobilePhone = String.Empty;
                usr.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;
                CoreContext.UserManager.SaveUserInfo(usr);
            }
            catch(Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }

            return NotifyUserToChangeMobilePhone(userID,true);
        }

        private object NotifyUserToChangeMobilePhone(Guid userID, bool isErase)
        {
            try
            {
                StudioNotifyService.Instance.SendMsgMobilePhoneChange(CoreContext.UserManager.GetUsers(userID));
                return new {Status = isErase ? 2 : 1};
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }
    }
}