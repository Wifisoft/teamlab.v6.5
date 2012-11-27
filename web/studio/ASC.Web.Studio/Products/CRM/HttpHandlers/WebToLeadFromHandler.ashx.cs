#region Import

using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.Core.Utility.Settings;
using AjaxPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using ASC.Web.CRM.Services.NotifyService;
using Newtonsoft.Json.Linq;
using ASC.Web.CRM.Resources;
using log4net;

#endregion

namespace ASC.Web.CRM.HttpHandlers
{

    public class WebToLeadFromHandler : IHttpHandler
    {

        private HttpContext _context;

        private String GetValue(String propertyName)
        {
            return _context.Request.Form[propertyName];
        }

        private bool CheckPermission()
        {
            try
            {
                var webFromKey = GetValue("web_form_key");

                if (String.IsNullOrEmpty(webFromKey))
                    return false;

                var webFromKeyAsGuid = new Guid(webFromKey);

                return Global.TenantSettings.WebFormKey == webFromKeyAsGuid;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                _context = context;

                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

                if (!CheckPermission())
                    throw new Exception(CRMSettingResource.WebToLeadsForm_InvalidKeyException);

                Contact contact;

                var fieldCollector = new NameValueCollection();

                var addressTemplate = new JObject();

                foreach (String addressPartName in Enum.GetNames(typeof(AddressPart)))
                    addressTemplate.Add(addressPartName.ToLower(), "");

                var addressTemplateStr = addressTemplate.ToString();

                var firstName = GetValue("firstName");
                var lastName = GetValue("lastName");
                var companyName = GetValue("companyName");

                if (!(String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName)))
                {
                    contact = new Person();

                    ((Person)contact).FirstName = firstName;
                    ((Person)contact).LastName = lastName;
                    ((Person)contact).JobTitle = GetValue("jobTitle");

                    fieldCollector.Add(CRMContactResource.FirstName, firstName);
                    fieldCollector.Add(CRMContactResource.LastName, lastName);

                    if (!String.IsNullOrEmpty(GetValue("jobTitle")))
                        fieldCollector.Add(CRMContactResource.JobTitle, ((Person)contact).JobTitle);

                }
                else if (!String.IsNullOrEmpty(companyName))
                {
                    contact = new Company();

                    ((Company)contact).CompanyName = companyName;

                    fieldCollector.Add(CRMContactResource.CompanyName, companyName);

                }
                else
                    throw new ArgumentException();

                contact.About = GetValue("about");

                if (!String.IsNullOrEmpty(contact.About))
                    fieldCollector.Add(CRMContactResource.About, contact.About);

                contact.ID = Global.DaoFactory.GetContactDao().SaveContact(contact);

                var contactInfos = new List<ContactInfo>();

                foreach (var key in _context.Request.Form.AllKeys)
                {
                    if (key.StartsWith("customField_"))
                    {
                        var fieldID = Convert.ToInt32(key.Split(new[] { '_' })[1]);
                        String fieldValue = GetValue(key);

                        if (String.IsNullOrEmpty(fieldValue)) continue;

                        var customField = Global.DaoFactory.GetCustomFieldDao().GetFieldDescription(fieldID);

                        if (customField == null) continue;

                        fieldCollector.Add(customField.Label, fieldValue);

                        Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Contact, contact.ID, fieldID, fieldValue);

                    }
                    else if (key.StartsWith("contactInfo_"))
                    {
                        var nameParts = key.Split(new[] { '_' }).Skip(1).ToList();
                        var contactInfoType = (ContactInfoType)Enum.Parse(typeof(ContactInfoType), nameParts[0]);
                        var category = Convert.ToInt32(nameParts[1]);

                        if (contactInfoType == ContactInfoType.Address)
                        {
                            var addressPart = (AddressPart)Enum.Parse(typeof(AddressPart), nameParts[2]);

                            var findedAddress = contactInfos.Find(item => (category == item.Category) && (item.InfoType == ContactInfoType.Address));

                            if (findedAddress == null)
                            {
                                findedAddress = new ContactInfo
                                {
                                    Category = category,
                                    InfoType = contactInfoType,
                                    Data = addressTemplateStr,
                                    ContactID = contact.ID
                                };

                                contactInfos.Add(findedAddress);
                            }

                            var addressParts = JObject.Parse(findedAddress.Data);

                            addressParts[addressPart.ToString().ToLower()] = GetValue(key);

                            findedAddress.Data = addressParts.ToString();

                            continue;
                        }

                        var fieldValue = GetValue(key);

                        if (String.IsNullOrEmpty(fieldValue)) continue;

                        contactInfos.Add(new ContactInfo
                        {
                            Category = category,
                            InfoType = contactInfoType,
                            Data = fieldValue,
                            ContactID = contact.ID,
                            IsPrimary = true
                        });

                    }
                    else if (String.Compare(key, "tag", true) == 0)
                    {
                        var tags = _context.Request.Form.GetValues("tag");

                        Global.DaoFactory.GetTagDao().SetTagToEntity(EntityType.Contact, contact.ID, tags);

                    }
                }

                contactInfos.ForEach(item => fieldCollector.Add(item.InfoType.ToLocalizedString(), PrepareteDataToView(item.InfoType, item.Data)));

                Global.DaoFactory.GetContactInfoDao().SaveList(contactInfos);

                var notifyList = GetValue("notify_list");

                if (!String.IsNullOrEmpty(notifyList))
                    NotifyClient.Instance.SendAboutCreateNewContact(
                        notifyList
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => new Guid(item)).ToList(), contact.ID, contact.GetTitle(), fieldCollector);


                SetPermission(contact, GetValue("private_list"));

                if (contact is Person && !String.IsNullOrEmpty(companyName))
                    AssignPersonToCompany((Person)contact, companyName, GetValue("private_list"));

                SecurityContext.Logout();

                context.Response.Redirect(GetValue("return_url"));
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.CRM").Error(error);
                throw;
            }
        }

        private String PrepareteDataToView(ContactInfoType contactInfoType, String data)
        {
            if (contactInfoType != ContactInfoType.Address) return data;

            var addressParts = JObject.Parse(data);

            var address = new StringBuilder();

            foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                address.Append(addressParts[addressPartEnum.ToString().ToLower()] + " ");

            return address.ToString();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        protected void SetPermission(Contact contact, String privateList)
        {
            if (String.IsNullOrEmpty(privateList)) return;

            var selectedUsers = privateList
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => new Guid(item)).ToList();

            CRMSecurity.SetAccessTo(contact, selectedUsers);

        }

        protected void AssignPersonToCompany(Person person, String companyName, String privateList)
        {
            Company company;

            var findedCompanies = Global.DaoFactory.GetContactDao().GetContactsByName(companyName)
                                  .Where(item => item is Company).ToList();

            if (findedCompanies.Count == 0)
            {
                company = new Company
                                     {
                                         CompanyName = companyName
                                     };

                company.ID = Global.DaoFactory.GetContactDao().SaveContact(company);

                SetPermission(company, privateList);

            }
            else
            {


                company = (Company)findedCompanies[0];
            }

            Global.DaoFactory.GetContactDao().AddMember(person.ID, company.ID);
        }
    }
}
