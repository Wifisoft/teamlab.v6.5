#region Import

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
using ASC.Core.Users;
using ASC.CRM.Core.Entities;
using System.Collections.Generic;
using ASC.Security.Cryptography;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Core.Users;
using ASC.Web.CRM.Controls.Common;
using ASC.Core;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.WebToLeadFormView")]
    public partial class WebToLeadFormView : BaseUserControl
    {
        #region Members

        protected String _webFormKey;

        #endregion

        #region Properties

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/WebToLeadFormView.ascx"); } }

        protected string GetHandlerUrl { get { return CommonLinkUtility.ServerRootPath + PathProvider.BaseAbsolutePath + "HttpHandlers/WebToLeadFromHandler.ashx".ToLower(); } }

        protected List<String> TagList { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(WebToLeadFormView));

            _webFormKey = Global.TenantSettings.WebFormKey.ToString();

            RegisterContactFields();

            //init privatePanel
            var privatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);
            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode() };
            privatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            privatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            privatePanel.HideNotifyPanel = true;
            _phPrivatePanel.Controls.Add(privatePanel);

            //init tagPanel
            var tagPanel = (TagView)LoadControl(TagView.Location);
            tagPanel.TargetEntityType = EntityType.Contact;
            tagPanel.Tags = new List<string>().ToArray();
            _phTagPanel.Controls.Add(tagPanel);

            //init userSelectorListView
            var selector = (UserSelectorListView)LoadControl(UserSelectorListView.Location);
            selector.ID = "Notify";
            selector.SelectedUsers = new Dictionary<Guid, string>
                                         {
                                             {
                                                 SecurityContext.CurrentAccount.ID,
                                                 SecurityContext.CurrentAccount.Name
                                              }
                                         };

            selector.DisabledUsers = new List<Guid> ();
            _phUserSelectorListView.Controls.Add(selector);
        }

        [AjaxMethod]
        public String ChangeWebFormKey()
        {
            
            var tenantSettings = Global.TenantSettings;

            tenantSettings.WebFormKey = Guid.NewGuid();

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

            return tenantSettings.WebFormKey.ToString();
        }

        protected void RegisterContactFields()
        {
            var columnSelectorData = new[]
                                         {

                                             new
                                             {
                                                  name = "firstName",
                                                  title = CRMContactResource.FirstName
                                             },
                                             new
                                             {
                                                  name = "lastName",
                                                  title = CRMContactResource.LastName
                                                  
                                             },
                                             new
                                             {
                                                  name = "jobTitle",
                                                  title = CRMContactResource.JobTitle
                                                  
                                             },
                                             new
                                             {
                                                  name = "companyName",
                                                  title = CRMContactResource.CompanyName
                                                  
                                             },
                                             new
                                             {
                                                  name = "about",
                                                  title = CRMContactResource.About
                                                  
                                             }
                                         }.ToList();

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
            {

                var localName = String.Format("contactInfo_{0}_{1}", infoTypeEnum, ContactInfo.GetDefaultCategory(infoTypeEnum));
                var localTitle = infoTypeEnum.ToLocalizedString();

                if (infoTypeEnum == ContactInfoType.Address)
                    foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                        columnSelectorData.Add(new
                        {
                            name = String.Format(localName + "_{0}_{1}", addressPartEnum, (int)AddressCategory.Work),
                            title = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString().ToLower())
                        });
                else
                    columnSelectorData.Add(new
                    {
                        name = localName,
                        title = localTitle
                    });
            }

            columnSelectorData.AddRange(Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Contact)
            .FindAll(customField => customField.FieldType == CustomFieldType.TextField || customField.FieldType == CustomFieldType.TextArea)
                                                          .ConvertAll(customField => new
                                                          {
                                                              name = "customField_" + customField.ID,
                                                              title = customField.Label.HtmlEncode()
                                                          }));

            var tagList = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Contact);

            if (tagList.Length > 0)
            {
                TagList = tagList.ToList();

                Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                   Guid.NewGuid().ToString(),
                                                   String.Format(" var {0} = {1}; ", "tagList", JavaScriptSerializer.Serialize(TagList.Select(tagName =>
                                                                new
                                                                {
                                                                    name = "tag_" + tagName.HtmlEncode(),
                                                                    title = tagName.HtmlEncode()
                                                                }))),
                                                   true);
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                    Guid.NewGuid().ToString(),
                                                    String.Format(" var {0} = {1}; ", "columnSelectorData", JavaScriptSerializer.Serialize(columnSelectorData)),
                                                    true);
        }


    }
}