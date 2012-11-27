using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Import;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.UserControls.Users
{
    internal class ContactsUploader : IFileUploadHandler
    {
        #region IFileUploadHandler Members

        public FileUploadResult ProcessUpload(HttpContext context)
        {
            FileUploadResult result = new FileUploadResult();
            try
            {
                if (context.Request.Files.Count != 0)
                {
                    var logo = context.Request.Files[0];

                    if (!(logo.ContentType == "text/plain" || logo.ContentType == "application/octet-stream" || logo.ContentType == "text/comma-separated-values"))
                    {
                        result.Success = false;
                        result.Message = Resources.Resource.ErrorEmptyUploadFileSelected;
                        return result;
                    }

                    IUserImporter importer = null;
                    importer = context.Request["obj"] == "txt" ? new TextFileUserImporter(logo.InputStream) { DefaultHeader = "Email;FirstName;LastName", } : new OutlookCSVUserImporter(logo.InputStream);

                    var users = importer.GetDiscoveredUsers();

                    result.Success = true;
                    result.Message = JsonContacts(users);

                }
                else
                {
                    result.Success = false;
                    result.Message = Resources.Resource.ErrorEmptyUploadFileSelected;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message.HtmlEncode();
            }

            return result;
        }

        private static string JsonContacts(IEnumerable<ContactInfo> contacts)
        {
            var serializer = new DataContractJsonSerializer(contacts.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, contacts);
                return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        #endregion
    }

    [AjaxNamespace("ImportUsersController")]
    public partial class ImportUsers : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/ImportUsers/ImportUsers.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            icon.Options.IsPopup = true;
            icon.Options.PopupContainerCssClass = "okcss";
            icon.Options.OnCancelButtonClick = "ImportUsersManager.HideInfoWindow('okcss');";
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            this.Page.ClientScript.RegisterClientScriptInclude(GetType(), "ajaxupload_script", VirtualPathUtility.ToAbsolute("~/js/ajaxupload.3.5.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "import_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/importusers/css/import.css") + "\">", false);

            //Page.ClientScript.RegisterClientScriptInclude(typeof(string), "importuser_script_sdk", WebPath.GetPath("usercontrols/users/ImportUsers/js/jquery-1.7.1.min.js"));
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "importuser_script", WebPath.GetPath("usercontrols/users/ImportUsers/js/ImportUsers.js"));
        }

        [AjaxMethod]
        public object SaveUsers(string userList)
        {
            var coll = new List<UserResults>();
            try
            {
                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var ruleObj = jsSerializer.Deserialize<List<UserData>>(userList);
                foreach (var userData in ruleObj)
                {
                    if (!(UserManagerWrapper.ValidateEmail(userData.Email)) || String.IsNullOrEmpty(userData.FirstName) || String.IsNullOrEmpty(userData.LastName))
                    {
                        coll.Add(new UserResults() {Email = userData.Email, Result = Resources.Resource.ImportContactsIncorrectFields, Class = !UserManagerWrapper.ValidateEmail(userData.Email) ? "error3" : "error1"});
                        continue;
                    }

                    var us = CoreContext.UserManager.GetUserByEmail(userData.Email);
                    if (us.ID != ASC.Core.Users.Constants.LostUser.ID)
                    {
                        coll.Add(new UserResults() { Email = userData.Email, Result = Resources.Resource.ImportContactsAlreadyExists, Class = "error2" });
                        continue;
                    }

                    UserManagerWrapper.AddUser(new UserInfo
                    {
                        Email = userData.Email,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName
                    }, UserManagerWrapper.GeneratePassword());
                    coll.Add(new UserResults() { Email = userData.Email, Result = String.Empty });
                }
                return new { Status = 1, Data = coll };
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }

        internal class UserResults
        {
            public string Email { get; set; }
            public string Result { get; set; }
            public string Class { get; set; }
        }
    }
}