using System;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Core;
using ASC.Core.Users;
using AjaxPro;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Security.Cryptography;
using System.Collections.Generic;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.ModuleManagement;
using System.Web.UI.WebControls;
using ASC.Web.Controls;
using ASC.Web.Studio.UserControls.Users;
using System.Linq;

namespace ASC.Web.Studio.UserControls.Management
{
    public class Item
    {
        public bool Disabled { get; set; }
        public bool DisplayedAlways { get; set; }
        public bool HasPermissionSettings { get; set; }
        public bool CanNotBeDisabled { get; set; }
        public string Name { get; set; }
        public string ItemName { get; set; }
        public string IconUrl { get; set; }
        public string DisabledIconUrl { get; set; }
        public string AccessSwitcherLabel { get; set; }
        public string UserOpportunitiesLabel { get; set; }
        public List<string> UserOpportunities { get; set; }
        public Guid ID { get; set; }
        public List<Item> SubItems { get; set; }
        public List<UserInfo> SelectedUsers { get; set; }
        public List<GroupInfo> SelectedGroups { get; set; }
    }

    [AjaxNamespace("AccessRightsController")]
    public partial class AccessRights : UserControl
    {
        #region Properies

        public static string Location
        {
            get { return "~/UserControls/Management/AccessRights/AccessRights.ascx"; }
        }

        protected bool CanOwnerEdit;

        protected List<IProduct> Products;

        protected string[] FullAccessOpportunities { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControl();
            RegisterClientScript();
        }

        protected void RptProductsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var phProductItem = (PlaceHolder)e.Item.FindControl("phProductItem");
                var productItem = (AccessRightsProductItem)LoadControl(AccessRightsProductItem.Location);
                productItem.ProductItem = (Item)e.Item.DataItem;
                phProductItem.Controls.Add(productItem);
            }
        }

        #endregion

        #region Methods

        private void InitControl()
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Products = ProductManager.Instance.Products;

            //owner settings
            var curTenant = CoreContext.TenantManager.GetCurrentTenant();
            var currentOwner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);
            CanOwnerEdit = currentOwner.ID.Equals(SecurityContext.CurrentAccount.ID);
            var disabledUsers = new List<Guid>{ currentOwner.ID };
            ownerSelector.DisabledUsers = disabledUsers;

            _phOwnerCard.Controls.Add(new EmployeeUserCard
            {
                EmployeeInfo = currentOwner,
                EmployeeUrl = currentOwner.GetUserProfilePageURL(),
                EmployeeDepartmentUrl = CommonLinkUtility.GetUserDepartment(currentOwner.ID)
            });

            //admin settings
            adminSelector.IsLinkView = true;
            adminSelector.LinkText = Resources.Resource.AccessRightsAddAdministrator;
            adminSelector.AdditionalFunction = "ASC.Settings.AccessRights.addAdmin";
            adminSelector.DisabledUsers = disabledUsers;

            //product repeater
            rptProducts.DataSource = GetDataSource();
            rptProducts.ItemDataBound += RptProductsItemDataBound;
            rptProducts.DataBind();

            FullAccessOpportunities = Resources.Resource.AccessRightsFullAccessOpportunities.Split('|');
        }

        private void RegisterClientScript()
        {
            Page.ClientScript.RegisterClientScriptInclude(
                typeof(string),
                "accessrights_script",
                WebPath.GetPath("usercontrols/management/accessrights/js/accessrights.js")
                );

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "accessrights_style",
                "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/management/accessrights/css/<theme_folder>/accessrights.css") + "\">",
                false);

            var curTenant = CoreContext.TenantManager.GetCurrentTenant();
            var currentOwner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);
            var admins = WebItemSecurity.GetProductAdministrators(Guid.Empty).Where(admin => admin.ID != currentOwner.ID).SortByUserName();
            Page.ClientScript.RegisterClientScriptBlock(
                typeof (AccessRights),
                "54F36EE2-0595-4e01-82BD-35E140D94F5D",
                "adminList = " + JavaScriptSerializer.Serialize(admins.ConvertAll(u => new
                                                                                            {
                                                                                                id = u.ID,
                                                                                                smallFotoUrl = u.GetSmallPhotoURL(),
                                                                                                displayName = u.DisplayUserName(),
                                                                                                title = u.Title.HtmlEncode(),
                                                                                                userUrl = CommonLinkUtility.GetUserProfile(u.ID, new Guid()),
                                                                                                accessList = GetAccessList(u.ID)
                                                                                            }))
                + "; ", true);
        }

        private static string GetConfirmLink(Guid newOwnerId, string email)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email.ToLower() + ConfirmType.PortalOwnerChange.ToString().ToLower() + newOwnerId);

            return CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx") +
                string.Format("?type={0}&email={1}&key={2}&uid={3}", ConfirmType.PortalOwnerChange.ToString().ToLower(), email, validationKey, newOwnerId);
        }

        private List<Item> GetDataSource()
        {
            var data = new List<Item>();
            var modules = WebItemManager.Instance.GetItems(WebZoneType.All, ItemAvailableState.All).Where(item => !item.IsSubItem()).ToList();

            foreach (var p in Products)
            {
                foreach (var m in modules)
                    if (Guid.Equals(m.ID, p.ID))
                    {
                        modules.Remove(m);
                        break;
                    }

                var item = new Item
                {
                    ID = p.ID,
                    Name = p.Name,
                    IconUrl = p.GetIconAbsoluteURL(),
                    DisabledIconUrl = p.GetDisabledIconAbsoluteURL(),
                    SubItems = new List<Item>(),
                    ItemName = p.GetSysName(),
                    UserOpportunitiesLabel = String.Format(Resources.Resource.AccessRightsProductUsersCan, p.Name),
                    UserOpportunities = p.GetUserOpportunities(),
                    HasPermissionSettings = true,
                    CanNotBeDisabled = p.CanNotBeDisabled()
                };

                if (p.HasComplexHierarchyOfAccessRights())
                    item.UserOpportunitiesLabel = String.Format(Resources.Resource.AccessRightsProductUsersWithRightsCan, item.Name);

                var productInfo = WebItemSecurity.GetSecurityInfo(item.ID.ToString());
                item.Disabled = !productInfo.Enabled;
                item.SelectedGroups = productInfo.Groups.ToList();
                item.SelectedUsers = productInfo.Users.ToList();

                foreach (var m in p.Modules)
                {
                    if ((m as Module) != null && (m as IWebItem) != null)
                    {
                        var subItem = new Item
                        {
                            Name = m.Name,
                            ID = m.ID,
                            DisplayedAlways = (m as Module).DisplayedAlways,
                            ItemName = m.GetSysName()
                        };

                        var moduleInfo = WebItemSecurity.GetSecurityInfo(subItem.ID.ToString());
                        subItem.Disabled = !moduleInfo.Enabled;
                        subItem.SelectedGroups = moduleInfo.Groups.ToList();
                        subItem.SelectedUsers = moduleInfo.Users.ToList();

                        item.SubItems.Add(subItem);
                    }
                }

                data.Add(item);
            }

            foreach (var m in modules)
            {
                var item = new Item
                {
                    ID = m.ID,
                    Name = m.Name,
                    IconUrl = m.GetIconAbsoluteURL(),
                    DisabledIconUrl = m.GetDisabledIconAbsoluteURL(),
                    SubItems = new List<Item>(),
                    ItemName = m.GetSysName()
                };

                var moduleInfo = WebItemSecurity.GetSecurityInfo(item.ID.ToString());
                item.Disabled = !moduleInfo.Enabled;
                item.SelectedGroups = moduleInfo.Groups.ToList();
                item.SelectedUsers = moduleInfo.Users.ToList();

                data.Add(item);
            }

            return data;
        }

        private object GetAccessList(Guid uId)
        {
            var fullAccess = WebItemSecurity.IsProductAdministrator(Guid.Empty, uId);
            var res = new List<object>
                          {
                              new
                                  {
                                      pId = Guid.Empty,
                                      pName = "full",
                                      pAccess = fullAccess,
                                      disabled = uId == SecurityContext.CurrentAccount.ID
                                  }
                          };

            if (Products == null)
                Products = ProductManager.Instance.Products;

            foreach (var p in Products)
                res.Add(new {
                                pId = p.ID,
                                pName = p.GetSysName(),
                                pAccess = WebItemSecurity.IsProductAdministrator(p.ID, uId),
                                disabled = fullAccess
                            });

            return res;
        }

        #endregion

        #region AjaxMethods

        [AjaxMethod]
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
                return new { Status = 0, Message = Resources.Resource.ErrorAccessDenied };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }

        [AjaxMethod]
        public object AddAdmin(Guid id)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var user = CoreContext.UserManager.GetUsers(id);
            
            WebItemSecurity.SetProductAdministrator(Guid.Empty, id, true);
            
            var result = new
            {
                id = user.ID,
                smallFotoUrl = user.GetSmallPhotoURL(),
                displayName = user.DisplayUserName(),
                title = user.Title.HtmlEncode(),
                userUrl = CommonLinkUtility.GetUserProfile(user.ID, new Guid()),
                accessList = GetAccessList(user.ID)
            };

            return result;
        }

        #endregion
    }
}