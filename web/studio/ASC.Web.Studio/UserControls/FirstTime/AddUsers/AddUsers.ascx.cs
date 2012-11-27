using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    [AjaxNamespace("AddUsersController")]
    public partial class AddUsers : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/AddUsers/AddUsers.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "adduser_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/firsttime/addusers/css/<theme_folder>/addusers.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "adduser_script", WebPath.GetPath("usercontrols/firsttime/addusers/js/addusers.js"));

        }

        [AjaxMethod]
        public object SaveUsers(string userList)
        {
            try
            {
                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var ruleObj = jsSerializer.Deserialize<List<UserData>>(userList);

                foreach (var userData in ruleObj)
                {
                    if (!(UserManagerWrapper.ValidateEmail(userData.Email) || String.IsNullOrEmpty(userData.FirstName) || String.IsNullOrEmpty(userData.LastName)))
                        continue;

                    var us = CoreContext.UserManager.GetUserByEmail(userData.Email);
                    if (us.ID != ASC.Core.Users.Constants.LostUser.ID)
                        continue;

                    UserManagerWrapper.AddUser(new UserInfo
                                                   {
                                                       Email = userData.Email,
                                                       FirstName = userData.FirstName,
                                                       LastName = userData.LastName
                                                   }, UserManagerWrapper.GeneratePassword());
                }
            }
            catch (Exception ex)
            {
                return new {Status = 0, Message = ex.Message};
            }

            return new { Status = 1, Message = Resources.Resource.WizardUsersSaved };
        }
    }
}