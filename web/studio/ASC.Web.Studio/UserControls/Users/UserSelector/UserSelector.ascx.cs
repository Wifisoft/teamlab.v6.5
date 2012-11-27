using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Data.Storage;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class UserSelector : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/UserSelector/UserSelector.ascx"; }
        }

        public string Title { get; set; }

        public string SelectedUserListTitle { get; set; }

        public string UserListTitle { get; set; }

        public string CustomBottomHtml { get; set; }

        public List<Guid> SelectedUsers { get; set; }

        public List<Guid> DisabledUsers { get; set; }

        public string BehaviorID { get; set; }

        protected string _jsObjName;

        private List<UserGroup> _userGroups = new List<UserGroup>();

        public bool isMobileVersion
        {
            get { return Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context); }
        }

        protected string _selectorID = Guid.NewGuid().ToString().Replace('-', '_');

        public UserSelector()
        {
            SelectedUsers = new List<Guid>();
            DisabledUsers = new List<Guid>();
            UserListTitle = HttpUtility.HtmlDecode(CustomNamingPeople.Substitute<Resources.Resource>("Employees"));
            SelectedUserListTitle = Resources.Resource.Selected;
            Title = CustomNamingPeople.Substitute<Resources.Resource>("UserSelectDialogTitle");
            CustomBottomHtml = "";
        }

        private class UserItem
        {
            public UserInfo UserInfo { get; set; }
            public bool Selected { get; set; }
        }

        private class UserGroup
        {
            public GroupInfo Group { get; set; }
            public List<UserItem> Users { get; set; }

            public UserGroup()
            {
                Users = new List<UserItem>();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _container.Options.IsPopup = true;

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "studio_usrselector_script", WebPath.GetPath("usercontrols/users/userselector/js/userselector.js"));
            _jsObjName = String.IsNullOrEmpty(BehaviorID) ? "__userSelector" + UniqueID : BehaviorID;


            Page.ClientScript.RegisterClientScriptBlock(GetType(), "studio_usrselector_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/userselector/css/<theme_folder>/userselector.css") + "\">", false);

            var script = new StringBuilder();
            script.AppendFormat("var {0} = new ASC.Studio.UserSelector.UserSelectorPrototype('{1}', '{0}', {2});\n",
                                _jsObjName,
                                _selectorID,
                                isMobileVersion.ToString().ToLower());

            var noDepGroup = new UserGroup {Group = new GroupInfo {Name = ""}};
            foreach (var u in CoreContext.UserManager.GetUsers().SortByUserName())
            {
                if (u.GetUserDepartment() == null)
                {
                    noDepGroup.Users.Add(new UserItem
                                             {
                                                 UserInfo = u,
                                                 Selected = (SelectedUsers.Find(sui => sui.Equals(u.ID)) != Guid.Empty)
                                             });
                }
            }
            if (noDepGroup.Users.Count > 0)
            {
                noDepGroup.Users.RemoveAll(ui => (DisabledUsers.Find(dui => dui.Equals(ui.UserInfo.ID)) != Guid.Empty));
                _userGroups.Add(noDepGroup);
            }


            foreach (var g in CoreContext.GroupManager.GetGroups())
                FillChildGroups(g);

            _userGroups.Sort((ug1, ug2) => String.Compare(ug1.Group.Name, ug2.Group.Name));

            foreach (var ug in _userGroups)
            {
                var groupVarName = _jsObjName + "_ug_" + ug.Group.ID.ToString().Replace('-', '_');
                script.AppendFormat("var {0} = new ASC.Studio.UserSelector.UserGroupItem('{1}','{2}'); ", groupVarName, ug.Group.ID, ug.Group.Name.HtmlEncode().ReplaceSingleQuote());
                foreach (var u in ug.Users)
                {
                    script.AppendFormat(" {0}.Users.push(new ASC.Studio.UserSelector.UserItem('{1}','{2}',{3},{0},{4},'{5}')); ", groupVarName,
                                        u.UserInfo.ID,
                                        u.UserInfo.DisplayUserName().ReplaceSingleQuote().Replace(@"\", @"\\"),
                                        u.Selected ? "true" : "false",
                                        u.Selected ? "true" : "false",
                                        string.IsNullOrEmpty(u.UserInfo.Title) ? string.Empty : u.UserInfo.Title.HtmlEncode().ReplaceSingleQuote().Replace(@"\", @"\\"));
                }

                script.AppendFormat(" {0}.Groups.push({1}); ", _jsObjName, groupVarName);

            }

            Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), script.ToString(), true);

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void FillChildGroups(GroupInfo groupInfo)
        {
            var users = new List<UserInfo>(CoreContext.UserManager.GetUsersByGroup(groupInfo.ID));
            users.RemoveAll(ui => (DisabledUsers.Find(dui => dui.Equals(ui.ID)) != Guid.Empty));
            users = users.SortByUserName();

            if (users.Count > 0)
            {
                var userGroup = new UserGroup {Group = groupInfo};
                _userGroups.Add(userGroup);


                foreach (var u in users)
                {
                    userGroup.Users.Add(new UserItem
                                            {
                                                UserInfo = u,
                                                Selected = (SelectedUsers.Find(sui => sui.Equals(u.ID)) != Guid.Empty)
                                            });
                }
            }

            foreach (var g in groupInfo.Descendants)
            {
                FillChildGroups(g);
            }
        }
    }
}