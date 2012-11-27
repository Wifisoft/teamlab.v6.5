using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI;
using AjaxPro;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Users;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AccessSettingsController")]
    public partial class AccessSettings : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/AccessSettings/AccessSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof (string), "accesssettings_script", WebPath.GetPath("usercontrols/management/AccessSettings/js/AccessSettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "accesssettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/AccessSettings/css/<theme_folder>/AccessSettings.css") + "\">", false);
                       
            
            shareUserSelector.LinkText = Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddUsersForSharingButton");

            var groupSelector = (GroupSelector) LoadControl(GroupSelector.Location);
            groupSelector.JsId = "accessGroupSelector";
            groupSelectorHolder.Controls.Add(groupSelector);

        }

        #region - Ajax methods -

        [AjaxMethod]
        public string LoadUsersRules()
        {
            var users = GetUsersRules().FindAll(x => !x.Internal);
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var obj = new {UserRules = users, Rule = (int) GetRuleType()};
            return serializer.Serialize(obj);
        }

        [AjaxMethod]
        public object SaveUsersRules(string userObj)
        {
            var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var ruleObj = jsSerializer.Deserialize<PermissionRule>(userObj);

            try
            {
                if (ruleObj.Rule == PermissionRule.AccessRule.Restrictions)
                    SaveUsersRules(ruleObj.UserRules);
                else
                {
                    var list = CoreContext.AuthorizationManager.GetAces(Guid.Empty, TcpIpFilterActions.TcpIpFilterAction.ID);
                    DeleteRulesList(list);

                    if (ruleObj.Rule == PermissionRule.AccessRule.Access)
                        CrealeRule(ASC.Core.Users.Constants.GroupEveryone.ID, "0.0.0.0", AceType.Allow);
                }

                return new {Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage};
            }
            catch (Exception)
            {
                return new {Status = 0, Message = Resources.Resource.UnSuccessfullySaveAccessSettingsMessage};
            }
        }

        #endregion

        /// <summary>
        /// Returns rule type for cuttent tenent
        /// </summary>
        /// <returns></returns>
        private PermissionRule.AccessRule GetRuleType()
        {
            var list = (List<AzRecord>) CoreContext.AuthorizationManager.GetAces(Guid.Empty, TcpIpFilterActions.TcpIpFilterAction.ID);

            if (list.Count == 0)
                return PermissionRule.AccessRule.Deny;

            if (list.FindAll(x => TcpIpFilterSecurityObject.ParseObjectId(x.ObjectId).ToString() != "0.0.0.0").Count > 0)
                return PermissionRule.AccessRule.Restrictions;

            var internalRules =
                list.FindAll(x => TcpIpFilterSecurityObject.ParseObjectId(x.ObjectId).ToString() == "0.0.0.0");

            foreach (var azRecord in internalRules)
            {
                if (azRecord.Reaction == AceType.Deny)
                    return PermissionRule.AccessRule.Deny;
            }

            return PermissionRule.AccessRule.Access;
        }

        /// <summary>
        /// Returns all users rules for current tenant
        /// </summary>
        /// <returns></returns>
        private List<UserData> GetUsersRules()
        {
            var list = CoreContext.AuthorizationManager.GetAces(Guid.Empty, TcpIpFilterActions.TcpIpFilterAction.ID);
            return SplitUsers(list);
        }

        /// <summary>
        /// Saves cpecial rules for users from certain user list
        /// </summary>
        /// <param name="userList"></param>
        private void SaveUsersRules(IEnumerable<UserData> userList)
        {
            CrealeRule(ASC.Core.Users.Constants.GroupEveryone.ID, "0.0.0.0", AceType.Allow);

            foreach (var userData in userList)
            {
                DeleteRulesForUser(userData.ID);

                if (userData.Keep)
                    SaveUserRules(userData);
            }
        }

        /// <summary>
        /// Saves special rules for certain user for its IP's addresses
        /// </summary>
        /// <param name="user"></param>
        private void SaveUserRules(UserData user)
        {
            CrealeRule(user.ID, "0.0.0.0", AceType.Deny);

            foreach (var ip in user.IPs)
                CrealeRule(user.ID, ip, AceType.Allow);
        }

        /// <summary>
        /// Creates a certain rule by specified parameters
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ip"></param>
        /// <param name="rule"></param>
        private void CrealeRule(Guid userID, string ip, AceType rule)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                var az = new AzRecord(userID, TcpIpFilterActions.TcpIpFilterAction.ID, rule,
                                      new TcpIpFilterSecurityObject(ipAddress));
                CoreContext.AuthorizationManager.AddAce(az);
            }
        }

        /// <summary>
        /// Deletes all rules for a specified user by its guid
        /// </summary>
        /// <param name="userID"></param>
        private void DeleteRulesForUser(Guid userID)
        {
            var list = CoreContext.AuthorizationManager.GetAces(userID, TcpIpFilterActions.TcpIpFilterAction.ID);
            DeleteRulesList(list);
        }

        /// <summary>
        /// Delete all rules for all users for certain tennant
        /// </summary>
        /// <param name="list"></param>
        private void DeleteRulesList(IEnumerable<AzRecord> list)
        {
            foreach (var azRecord in list)
                CoreContext.AuthorizationManager.RemoveAce(azRecord);
        }

        /// <summary>
        /// Group users with their ip addresses
        /// </summary>
        /// <param name="list">AzRecord collection</param>
        /// <returns>UserData collectionn</returns>
        private List<UserData> SplitUsers(IEnumerable<AzRecord> list)
        {
            var nList = new List<UserData>();

            foreach (var azRecord in list)
            {
                var ip = TcpIpFilterSecurityObject.ParseObjectId(azRecord.ObjectId);
                var user = GetUser(nList, azRecord.SubjectId);

                if (user == null)
                {
                    var userInfo = CoreContext.UserManager.GetUsers(azRecord.SubjectId);

                    if (userInfo.ID != ASC.Core.Users.Constants.LostUser.ID)
                    {
                        if (ip.ToString() != "0.0.0.0")
                            nList.Add(new UserData(userInfo)
                                          {
                                              IPs = new List<string> {ip.ToString()},
                                              Internal = (ip.ToString() == "0.0.0.0")
                                          });
                    }
                    else
                    {
                        var group = CoreContext.GroupManager.GetGroupInfo(azRecord.SubjectId);
                        nList.Add(new UserData(group.Name, group.ID, ip.ToString() == "0.0.0.0") {IPs = new List<string> {ip.ToString()}});
                    }

                }
                else
                {
                    if (user.Internal)
                        user.Internal = (ip.ToString() == "0.0.0.0");
                    if (ip.ToString() != "0.0.0.0")
                        user.IPs.Add(ip.ToString());
                }

            }
            return nList;
        }

        /// <summary>
        /// Finds a certain user in a collection by unique ID
        /// </summary>
        /// <param name="list">Users collection</param>
        /// <param name="userGuid">user ID</param>
        /// <returns></returns>
        private UserData GetUser(IEnumerable<UserData> list, Guid userGuid)
        {
            foreach (var userData in list)
            {
                if (userData.ID == userGuid)
                    return userData;
            }
            return null;
        }

        [Serializable]
        public class PermissionRule
        {
            public enum AccessRule
            {
                Access = 0,
                Deny = 1,
                Restrictions = 2
            }

            public AccessRule Rule { get; set; }
            public List<UserData> UserRules { get; set; }
        }

        [Serializable]
        public class UserData
        {
            public string Name { get; set; }
            public Guid ID { get; set; }
            public bool Keep { get; set; }
            public List<string> IPs { get; set; }
            internal bool Internal { get; set; }

            public UserData()
            {
            }

            public UserData(UserInfo info)
            {
                Name = info.FirstName + " " + info.LastName;
                ID = info.ID;
                IPs = new List<string>();
            }

            public UserData(string name, Guid id, bool inter)
            {
                Name = name;
                ID = id;
                IPs = new List<string>();
                Internal = inter;
            }
        }
    }
}