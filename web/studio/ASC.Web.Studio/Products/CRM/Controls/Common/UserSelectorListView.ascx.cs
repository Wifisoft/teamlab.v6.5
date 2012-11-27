using System;
using System.Linq;
using ASC.Web.Core.Utility.Skins;
using System.Collections.Generic;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Core.Users;
using ASC.Web.CRM.Resources;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Controls;

namespace ASC.Web.CRM.Controls.Common
{
    public partial class UserSelectorListView : BaseUserControl
    {
        #region Properties

        public static String Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/UserSelectorListView.ascx");
            }
        }

        public List<UserInfo> UserList { get; set; }

        public List<String> UsersWhoHasAccess { get; set; }

        public Dictionary<Guid, String> SelectedUsers { get; set; }

        public List<Guid> DisabledUsers { get; set; } 

        public bool ShowNotifyPanel { get; set; }

        protected String PeopleImgSrc
        {
            get
            {
                return WebImageSupplier.GetAbsoluteWebPath("people_icon.png", ProductEntryPoint.ID);
            }
        }

        protected String DeleteImgSrc
        {
            get
            {
                return WebImageSupplier.GetAbsoluteWebPath("trash_12.png");
            }
        }

        protected  String ObjId
        {
            get
            {
                return ID == null ? String.Empty : "_"+ID;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //init advUserSelector
            var userSelector = new AdvancedUserSelector
              {
                  ID = "advUserSelectorListView"+ObjId,
                  IsLinkView = true,
                  LinkText = CustomNamingPeople.Substitute<CRMCommonResource>("AddUser"),
                  AdditionalFunction = "ASC.CRM.UserSelectorListView" + ObjId + ".pushUserIntoList"
              };

            if (UserList != null && UserList.Count > 0)
                userSelector.UserList = UserList;

            if (DisabledUsers != null && DisabledUsers.Count > 0)
                userSelector.DisabledUsers = DisabledUsers;

            _phAdvUserSelector.Controls.Add(userSelector);

            var ids = SelectedUsers != null && SelectedUsers.Count > 0 ? SelectedUsers.Select(i => i.Key).ToArray() : new List<Guid>().ToArray();
            var names = SelectedUsers != null && SelectedUsers.Count > 0 ? SelectedUsers.Select(i => i.Value).ToArray() : new List<string>().ToArray();

            var key = Guid.NewGuid().ToString();

            Page.ClientScript.RegisterClientScriptBlock(typeof(PrivatePanel), key, "SelectedUsers" + ObjId + " = " +
                                                                          JavaScriptSerializer.Serialize(
                                                                              new
                                                                              {
                                                                                  IDs = ids,
                                                                                  Names = names,
                                                                                  PeopleImgSrc = PeopleImgSrc,
                                                                                  DeleteImgSrc = DeleteImgSrc,
                                                                                  DeleteImgTitle = CRMCommonResource.DeleteUser,
                                                                                  CurrentUserID = SecurityContext.CurrentAccount.ID
                                                                              }) + "; ", true);
        }
    }
}