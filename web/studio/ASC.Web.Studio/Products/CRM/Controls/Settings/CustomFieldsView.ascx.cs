#region Import

using System;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.CustomFieldsView")]
    public partial class CustomFieldsView : BaseUserControl
    {
        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Settings/CustomFieldsView.ascx"); }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof (CustomFieldsView));
            EntityType entityType;

            var view = Request["view"];

            switch (view)
            {
                case "person":
                    entityType = EntityType.Person;
                    break;
                case "company":
                    entityType = EntityType.Company;
                    break;
                case "opportunity":
                    entityType = EntityType.Opportunity;
                    break;
                case "case":
                    entityType = EntityType.Case;
                    break;
                default:
                    entityType = EntityType.Contact;
                    break;
            }

            _manageFieldPopup.Options.IsPopup = true;

            _switcherEntityType.SortItemsHeader = CRMCommonResource.Show + ":";

            _switcherEntityType.SortItems[0].SortLabel = CRMSettingResource.BothPersonAndCompany;
            _switcherEntityType.SortItems[0].SortUrl = "settings.aspx?type=custom_field";
            _switcherEntityType.SortItems[0].IsSelected = entityType == EntityType.Contact;

            _switcherEntityType.SortItems[1].SortLabel = CRMSettingResource.JustForPerson;
            _switcherEntityType.SortItems[1].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}",
                                                                     EntityType.Person.ToString().ToLower());
            _switcherEntityType.SortItems[1].IsSelected = entityType == EntityType.Person;

            _switcherEntityType.SortItems[2].SortLabel = CRMSettingResource.JustForCompany;
            _switcherEntityType.SortItems[2].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}",
                                                                     EntityType.Company.ToString().ToLower());
            _switcherEntityType.SortItems[2].IsSelected = entityType == EntityType.Company;

            _switcherEntityType.SortItems[3].SortLabel = CRMCommonResource.DealModuleName;
            _switcherEntityType.SortItems[3].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}", EntityType.Opportunity.ToString().ToLower());
            _switcherEntityType.SortItems[3].IsSelected = entityType == EntityType.Opportunity;

            _switcherEntityType.SortItems[4].SortLabel = CRMCommonResource.CasesModuleName;
            _switcherEntityType.SortItems[4].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}", EntityType.Case.ToString().ToLower());
            _switcherEntityType.SortItems[4].IsSelected = entityType == EntityType.Case;

            var customFieldList = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(entityType);
            Page.JsonPublisher(customFieldList, "customFieldList");

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                        Guid.NewGuid().ToString(),
                                                        String.Format(" var relativeItems = {0};", Global.DaoFactory.GetCustomFieldDao().GetContactLinkCountJSON(entityType)),
                                                        true);
            var emptyScreenControl = new EmptyScreenControl
                                         {
                                             ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_userfields.png", ProductEntryPoint.ID),
                                             Header = CRMSettingResource.EmptyContentCustomFields,
                                             Describe = CRMSettingResource.EmptyContentCustomFieldsDescript,
                                             ButtonHTML = String.Format("<a class='linkAddMediumText baseLinkAction' onclick='ASC.CRM.SettingsPage.showAddFieldPanel();' >{0}</a>",
                                                                        CRMSettingResource.AddCustomField)
                                         };
            _phEmptyContent.Controls.Add(emptyScreenControl);

        }

        #endregion

        #region Ajax Methods

        [AjaxMethod]
        public void ReorderFields(int[] fieldID)
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            Global.DaoFactory.GetCustomFieldDao().ReorderFields(fieldID);
        }

        #endregion
    }
}