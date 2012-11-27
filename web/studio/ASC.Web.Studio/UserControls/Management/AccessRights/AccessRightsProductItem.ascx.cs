using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using ASC.Web.Controls;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Core;
using AjaxPro;
using ASC.Web.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class AccessRightsProductItem : UserControl
    {
        #region Properies

        public static string Location
        {
            get { return "~/UserControls/Management/AccessRights/AccessRightsProductItem.ascx"; }
        }

        public Item ProductItem { get; set; }

        protected string PeopleImgSrc
        {
            get { return WebImageSupplier.GetAbsoluteWebPath("user_12.png"); }
        }

        protected string GroupImgSrc
        {
            get { return WebImageSupplier.GetAbsoluteWebPath("group_12.png"); }
        }

        protected string TrashImgSrc
        {
            get { return WebImageSupplier.GetAbsoluteWebPath("trash_12.png"); }
        }

        protected bool PublicModule
        {
            get { return ProductItem.SelectedUsers.Count == 0 && ProductItem.SelectedGroups.Count == 0; }
        }

        protected Guid PortalOwnerId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().OwnerId; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ProductItem.ItemName)) return;

            var userSelector = new AdvancedUserSelector
                                   {
                                       ID = "userSelector_" + ProductItem.ItemName,
                                       LinkText = CustomNamingPeople.Substitute<Resources.Resource>("AccessRightsAddUser"),
                                       IsLinkView = true,
                                       DisabledUsers = new List<Guid> { PortalOwnerId }
                                   };
            phUserSelector.Controls.Add(userSelector);

            var groupSelector = (GroupSelector)LoadControl(GroupSelector.Location);
            groupSelector.JsId = "groupSelector_" + ProductItem.ItemName;
            groupSelector.LinkText = CustomNamingPeople.Substitute<Resources.Resource>("AccessRightsAddGroup");
            groupSelector.WithGroupEveryone = true;
            phGroupSelector.Controls.Add(groupSelector);

            var ids = ProductItem.SelectedUsers.Select(i => i.ID).ToArray();
            var names = ProductItem.SelectedUsers.Select(i => i.DisplayUserName()).ToArray();
            var key = Guid.NewGuid().ToString();

            Page.ClientScript.RegisterClientScriptBlock(typeof(AccessRightsProductItem), key, "SelectedUsers_" + ProductItem.ItemName + " = " +
                                                                          JavaScriptSerializer.Serialize(
                                                                              new
                                                                              {
                                                                                  IDs = ids,
                                                                                  Names = names,
                                                                                  PeopleImgSrc = PeopleImgSrc,
                                                                                  TrashImgSrc = TrashImgSrc,
                                                                                  TrashImgTitle = Resources.Resource.DeleteButton,
                                                                                  CurrentUserID = SecurityContext.CurrentAccount.ID
                                                                              }) + "; ", true);

            ids = ProductItem.SelectedGroups.Select(i => i.ID).ToArray();
            names = ProductItem.SelectedGroups.Select(i => i.Name).ToArray();
            key = Guid.NewGuid().ToString();

            Page.ClientScript.RegisterClientScriptBlock(typeof(AccessRightsProductItem), key, "SelectedGroups_" + ProductItem.ItemName + " = " +
                                                                          JavaScriptSerializer.Serialize(
                                                                              new
                                                                              {
                                                                                  IDs = ids,
                                                                                  Names = names,
                                                                                  GroupImgSrc = GroupImgSrc,
                                                                                  TrashImgSrc = TrashImgSrc,
                                                                                  TrashImgTitle = Resources.Resource.DeleteButton
                                                                              }) + "; ", true);

            _managePanel.Options.IsPopup = true;
        }
    }
}