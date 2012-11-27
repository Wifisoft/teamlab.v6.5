using System;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("ProfileOperation")]
    public partial class ProfileOperation : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/ProfileOperation.ascx"; } }
        
        public UserInfo User { get; set; }
        public string Key{get;set;}
        public string Email { get; set; }
        public ConfirmType Type { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void DeleteProfile(object sender, EventArgs e)
        {
            try
            {
                var uid = User.ID;
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                
                UserPhotoManager.RemovePhoto(Guid.Empty, uid);
                CoreContext.UserManager.DeleteUser(uid);

                operationBlock.Visible = false;
                result.InnerHtml = Resources.Resource.DeleteProfileSuccess;
            }
            catch (Exception ex)
            {
                result.InnerHtml = ex.Message;
            }
            finally
            {
                SecurityContext.Logout();
                CookiesManager.ClearCookies(CookiesType.AuthKey);
            }
        }
    }
}