#region Import

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.UI.WebControls;
using ASC.Web.Core.Users.Activity;
using AjaxPro;
using ASC.Core;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Data.Storage;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Controls.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ASC.Web.Studio.Core.Users;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{

    public partial class ContactActionView : BaseUserControl
    {
        #region Properies

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Contacts/ContactActionView.ascx"); } }
        public Contact TargetContact { get; set; }
        public String TypeAddedContact { get; set; }

        public String SaveContactButtonText { get; set; }
        public String SaveAndCreateContactButtonText { get; set; }

        public String AjaxProgressText { get; set; }

        protected List<Int32> OtherCompaniesID { get; set; }
        protected bool HavePermission { get; set; }

        protected List<ContactInfoType> ContactInfoTypes { get; set; }
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "ajaxupload_script", WebPath.GetPath("js/ajaxupload.3.5.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "contact_placeholder_script", WebPath.GetPath("js/jquery.watermarkinput.js"));

            var country = new List<string> { CRMJSResource.ChooseCountry };
            var additionalCountries = new List<string>
                                        {
                                            CRMCommonResource.Country_Gambia,
                                            CRMCommonResource.Country_Ghana,
                                            CRMCommonResource.Country_RepublicOfCyprus,
                                            CRMCommonResource.Country_SierraLeone,
                                            CRMCommonResource.Country_Tanzania,
                                            CRMCommonResource.Country_Zambia,
                                        };
            var standardCountries = Global.GetCountryList();
            standardCountries.AddRange(additionalCountries);
            country.AddRange(standardCountries.OrderBy(c => c));

            contactCountry.DataSource = country;

            contactCountry.Name = "contactInfo_Address_" + (int)ContactInfoBaseCategory.Work + "_"  + AddressPart.Country;
            contactCountry.DataBind();

            ContactInfoTypes = (from ContactInfoType item in Enum.GetValues(typeof(ContactInfoType))
                                where (item != ContactInfoType.Address && item != ContactInfoType.Email &&
                                item != ContactInfoType.Phone)
                                select item).ToList();

            List<CustomField> data;

            var networks = new List<ContactInfo>();

            saveContactButton.Text = SaveContactButtonText;
            saveContactButton.OnClientClick = String.Format("return ASC.CRM.ContactActionView.submitForm('{0}');", saveContactButton.UniqueID);

            saveAndCreateContactButton.Text = SaveAndCreateContactButtonText;
            saveAndCreateContactButton.OnClientClick = String.Format("return ASC.CRM.ContactActionView.submitForm('{0}');", saveAndCreateContactButton.UniqueID);

            if (TargetContact == null)
            {
                if (UrlParameters.Type != "people")
                {
                    data = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Company);

                    ExecAssignedContactsView(0);
                }
                else
                   data = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Person);


                var URLEmail = UrlParameters.Email;
                if (!String.IsNullOrEmpty(URLEmail))
                    networks.Add(new ContactInfo()
                            {
                               Category = (int) ContactInfoBaseCategory.Work,
                               ContactID = 0,
                               Data = URLEmail.HtmlEncode(),
                               ID = 0,
                               InfoType = ContactInfoType.Email,
                               IsPrimary = true
                            });
            }
            else
            {
                if (TargetContact is Person)
                  data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, TargetContact.ID, true);
                else
                  data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, TargetContact.ID, true);

                if (TargetContact is Company)
                {
                    deleteContactButton.Text = CRMContactResource.DeleteThisCompany;
                    deleteContactButton.OnClientClick = String.Format("return ASC.CRM.ContactActionView.confirmForDelete(1, '{0}');",
                                                                TargetContact.GetTitle().ReplaceSingleQuote().HtmlEncode());


                    ExecAssignedContactsView(TargetContact.ID);

                }
                else
                {
                    deleteContactButton.Text = CRMContactResource.DeleteThisPerson;
                    deleteContactButton.OnClientClick = String.Format("return ASC.CRM.ContactActionView.confirmForDelete(0, '{0}');",
                                                                    TargetContact.GetTitle().ReplaceSingleQuote().HtmlEncode());

                }

                networks = Global.DaoFactory.GetContactInfoDao().GetList(TargetContact.ID, null, null, null).ConvertAll(
                n => new ContactInfo()
                                    {
                                        Category = n.Category,
                                        ContactID = n.ContactID,
                                        Data = n.Data.HtmlEncode(),
                                        ID = n.ID,
                                        InfoType = n.InfoType,
                                        IsPrimary = n.IsPrimary
                                    });
            }

            String json;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(data.GetType());

                serializer.WriteObject(stream, data);

                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            var script =
                String.Format(
                    @"<script type='text/javascript'>
                                        var customFieldList = {0};
                                        var contactNetworks = {1};
                    </script>",
                              json,
                              JavaScriptSerializer.Serialize(networks));


            Page.ClientScript.RegisterStartupScript(typeof(ContactDetailsView), Guid.NewGuid().ToString(), script);



            if (TargetContact == null || TargetContact is Person)
            {
                InitContactSelectorAndCancelButton();
            }
            else
            {
                cancelButton.Attributes.Add("href", String.Format("default.aspx?{0}={1}", UrlConstant.ID, TargetContact.ID));
            }

            InitPrivatePanel();

            if (TargetContact != null)
                HavePermission = CRMSecurity.IsAdmin || TargetContact.CreateBy == SecurityContext.CurrentAccount.ID;
            else
                HavePermission = true;
        }

        #endregion

        #region Methods
        public String GetTitle()
        {
            if (TargetContact != null && TargetContact is Person)
                return ((Person) TargetContact).JobTitle.HtmlEncode();
            return String.Empty;
        }

        public String GetFirstName()
        {
            if (TargetContact != null && TargetContact is Person)
                return ((Person) TargetContact).FirstName.HtmlEncode();

            var URLFullName = UrlParameters.FullName;
            if (!String.IsNullOrEmpty(URLFullName))
            {
                var parts = URLFullName.Split(' ');
                return parts.Length < 2 ? String.Empty : parts[0];
            }

            return String.Empty;
        }

        public String GetLastName()
        {
            if (TargetContact != null && TargetContact is Person)
                return ((Person)TargetContact).LastName.HtmlEncode();

            var URLFullName = UrlParameters.FullName;
            if (!String.IsNullOrEmpty(URLFullName))
            {
                var parts = URLFullName.Split(' ');
                return parts.Length < 2 ? URLFullName : URLFullName.Remove(0, parts[0].Length);
            }
            return String.Empty;
        }

        public String GetCompanyName()
        {
            if (TargetContact != null && TargetContact is Company)
                return ((Company)TargetContact).CompanyName.HtmlEncode();
            return UrlParameters.FullName;
        }

        public String GetCompanyIDforPerson()
        {
            if ((TargetContact != null && TargetContact is Person))
                return ((Person)TargetContact).CompanyID.ToString();
            return String.Empty;
        }

        protected void InitContactSelectorAndCancelButton()
        {
            var cntrlContactSelector = (ContactSelector)LoadControl(ContactSelector.Location);
            cntrlContactSelector.CurrentType = ContactSelector.SelectorType.Companies;
            cntrlContactSelector.DescriptionText = CRMContactResource.FindCompanyByName;
            cntrlContactSelector.ShowNewCompanyContent = true;
            cntrlContactSelector.ID = "companySelector";

            if (TargetContact != null && TargetContact is Person)
            {
                var company = Global.DaoFactory.GetContactDao().GetByID(((Person)TargetContact).CompanyID);
                cntrlContactSelector.SelectedContacts = company != null ? new List<Contact> {company} : null;
                cntrlContactSelector.ShowChangeButton = true;
                cancelButton.Attributes.Add("href", String.Format("default.aspx?{0}={1}&{2}=people", UrlConstant.ID, TargetContact.ID, UrlConstant.Type));
            }
            else
            {
                cntrlContactSelector.SelectedContacts = null;
                cntrlContactSelector.ShowChangeButton = true;
                cancelButton.Attributes.Add("href", String.Format("default.aspx{0}",
                    !String.IsNullOrEmpty(UrlParameters.Type) ?
                    String.Format("?{0}={1}", UrlConstant.Type, UrlParameters.Type) :
                    String.Empty));
            }

            cntrlContactSelector.ShowContactImg = true;
            phContactSelector.Controls.Add(cntrlContactSelector);
            Utility.RegisterTypeForAjax(typeof(ContactSelector));
        }

        protected void InitPrivatePanel()
        {
            var cntrlPrivatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);

            if (TargetContact != null)
            {
                cntrlPrivatePanel.CheckBoxLabel = TargetContact is Person
                                                      ? CRMContactResource.PrivatePanelCheckBoxLabelForPeople
                                                      : CRMContactResource.PrivatePanelCheckBoxLabelForCompany;

                cntrlPrivatePanel.IsPrivateItem = CRMSecurity.IsPrivate(TargetContact);
                if (cntrlPrivatePanel.IsPrivateItem)
                    cntrlPrivatePanel.SelectedUsers = CRMSecurity.GetAccessSubjectTo(TargetContact);
            }
            else
            {
                cntrlPrivatePanel.CheckBoxLabel = UrlParameters.Type == "people"
                                        ? CRMContactResource.PrivatePanelCheckBoxLabelForPeople
                                        : CRMContactResource.PrivatePanelCheckBoxLabelForCompany;
            }

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode() };

            cntrlPrivatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            cntrlPrivatePanel.DisabledUsers = new List<Guid> {SecurityContext.CurrentAccount.ID};
            phPrivatePanel.Controls.Add(cntrlPrivatePanel);
        }

        protected void ExecAssignedContactsView(Int32 companyID)
        {
            var people = Global.DaoFactory.GetContactDao().GetMembers(companyID);

            //init ListContactView
            var listContactView = (ListContactView)LoadControl(ListContactView.Location);
            listContactView.IsSimpleView = true;
            listContactView.EntityID = companyID;
            listContactView.EntityType = EntityType.Company;
            _phAssignedPersonsContainer.Controls.Add(listContactView);

            // Init ContactSelector
            var cntrlContactSelector = (ContactSelector)LoadControl(ContactSelector.Location);
            cntrlContactSelector.CurrentType = ContactSelector.SelectorType.PersonsWithoutCompany;
            cntrlContactSelector.DescriptionText = CRMContactResource.FindContactByName;
            cntrlContactSelector.ID = "assignedContactSelector";
            cntrlContactSelector.ShowOnlySelectorContent = true;
            cntrlContactSelector.ShowAllDeleteButton = false;
            cntrlContactSelector.ShowDeleteButton = false;
            cntrlContactSelector.ShowNewContactContent = true;
            cntrlContactSelector.SelectedContacts = people;

            _phAssignedContactSelector.Controls.Add(cntrlContactSelector);
            Utility.RegisterTypeForAjax(typeof(ContactSelector));
        }

        protected void DeleteContact(object sender, EventArgs eventArgs)
        {
            var urlType = "";
            if (TargetContact is Person)
            {
                urlType = String.Format("?{0}=people", UrlConstant.Type);
            }

            Global.DaoFactory.GetContactDao().DeleteContact(TargetContact.ID);
            Response.Redirect(String.Format("default.aspx{0}", urlType));
        }

        protected void SaveOrUpdateContact(object sender, CommandEventArgs e)
        {
            Contact contact;

            var typeAddedContact = Request["typeAddedContact"];

            var companyID = 0;

            if (!String.IsNullOrEmpty(Request["baseInfo_compID"]))
                companyID = Convert.ToInt32(Request["baseInfo_compID"]);
            else if (!String.IsNullOrEmpty(Request["baseInfo_compName"]))
            {
                var peopleCompany = new Company
                {
                    CompanyName = Request["baseInfo_compName"].Trim()
                };

                peopleCompany.ID = Global.DaoFactory.GetContactDao().SaveContact(peopleCompany);

                CRMSecurity.MakePublic(peopleCompany);

                companyID = peopleCompany.ID;
            }

            if (typeAddedContact.Equals("people"))
                contact = new Person
                              {
                                  FirstName = Request["baseInfo_firstName"].Trim(),
                                  LastName = Request["baseInfo_lastName"].Trim(),
                                  JobTitle = Request["baseInfo_personPosition"].Trim(),
                                  CompanyID = companyID
                              };
            else
                contact = new Company
                              {
                                  CompanyName = Request["baseInfo_companyName"].Trim()
                              };


            if (!String.IsNullOrEmpty(Request["baseInfo_contactOverview"]))
                contact.About = Request["baseInfo_contactOverview"].Trim();


            if (TargetContact != null)
            {
                contact.ID = TargetContact.ID;
                contact.StatusID = TargetContact.StatusID;
                Global.DaoFactory.GetContactDao().UpdateContact(contact);
                contact = Global.DaoFactory.GetContactDao().GetByID(contact.ID);
                SetPermission(contact);
                TimeLinePublisher.Contact(contact,
                                          CRMCommonResource.ActionText_Update,
                                          UserActivityConstants.ActivityActionType,
                                          UserActivityConstants.ImportantActivity);
            }
            else
            {
                contact.ID = Global.DaoFactory.GetContactDao().SaveContact(contact);
                contact = Global.DaoFactory.GetContactDao().GetByID(contact.ID);
                SetPermission(contact);
                TimeLinePublisher.Contact(contact,
                                          CRMCommonResource.ActionText_Create,
                                          UserActivityConstants.ActivityActionType,
                                          UserActivityConstants.ImportantActivity);
            }

            if (contact is Company)
            {
                int[] assignedContactsIDs = null;

                if (!String.IsNullOrEmpty(Request["baseInfo_assignedContactsIDs"]))
                    assignedContactsIDs = Request["baseInfo_assignedContactsIDs"].Split(',').Select(item => Convert.ToInt32(item)).ToArray();

                Global.DaoFactory.GetContactDao().SetMembers(contact.ID, assignedContactsIDs);

            }

            var contactInfos = new List<ContactInfo>();
            var addressList = new Dictionary<int, ContactInfo>();
            var addressTemplate = new JObject();

            foreach (String addressPartName in Enum.GetNames(typeof (AddressPart)))
                addressTemplate.Add(addressPartName.ToLower(), "");

            var addressTemplateStr = addressTemplate.ToString();

            foreach (var item in Request.Form.AllKeys)
            {
                if (item.StartsWith("customField_"))
                {
                    int fieldID = Convert.ToInt32(item.Split('_')[1]);
                    String fieldValue = Request.Form[item].Trim();

                    if (contact is Person)
                       Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Person, contact.ID, fieldID,
                                                                        fieldValue);
                    else
                        Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Company, contact.ID, fieldID,
                                                     fieldValue);

                }
                else if (item.StartsWith("contactInfo_"))
                {
                    var nameParts = item.Split('_').Skip(1).ToList();
                    var contactInfoType = (ContactInfoType) Enum.Parse(typeof (ContactInfoType), nameParts[0]);
                    var category = Convert.ToInt32(nameParts[2]);

                    if (contactInfoType == ContactInfoType.Address)
                    {
                        var index = Convert.ToInt32(nameParts[1]);
                        var addressPart = (AddressPart)Enum.Parse(typeof(AddressPart), nameParts[3]);
                        var isPrimaryAddress = Convert.ToInt32(nameParts[4]) == 1;
                        var dataValues = Request.Form.GetValues(item).Select(n => n.Trim()).ToList();

                        if (!addressList.ContainsKey(index))
                        {
                            var newAddress = new ContactInfo
                                                 {
                                                     Category = category,
                                                     InfoType = contactInfoType,
                                                     Data = addressTemplateStr,
                                                     IsPrimary = isPrimaryAddress,
                                                     ContactID = contact.ID
                                                 };
                            addressList.Add(index, newAddress);
                        }

                        foreach (var data in dataValues)
                        {
                            var addressParts = JObject.Parse(addressList[index].Data);
                            addressParts[addressPart.ToString().ToLower()] = data;
                            addressList[index].Data = addressParts.ToString();
                        }
                        continue;
                    }

                    var isPrimary = Convert.ToInt32(nameParts[3]) == 1;
                    if (Request.Form.GetValues(item) != null)
                    {
                        var dataValues = Request.Form.GetValues(item).Where(n => !string.IsNullOrEmpty(n.Trim())).ToList();

                        contactInfos.AddRange(dataValues.Select(dataValue => new ContactInfo
                                                                                 {
                                                                                     Category = category,
                                                                                     InfoType = contactInfoType,
                                                                                     Data = dataValue.Trim(),
                                                                                     IsPrimary = isPrimary,
                                                                                     ContactID = contact.ID
                                                                                 }));
                    }
                }
            }

            if (addressList.Count>0)
                contactInfos.AddRange(addressList.Values.ToList());

            Global.DaoFactory.GetContactInfoDao().DeleteByContact(contact.ID);
            Global.DaoFactory.GetContactInfoDao().SaveList(contactInfos);


            if (!String.IsNullOrEmpty(Request["uploadPhotoPath"]))
            {
                ContactPhotoManager.UploadPhoto(Request["uploadPhotoPath"], contact.ID);
            }


            Response.Redirect(String.Compare(e.CommandArgument.ToString(), "0", true) == 0
                                  ? String.Format("default.aspx?id={0}{1}", contact.ID,
                                                  contact is Company
                                                      ? ""
                                                      : String.Format("&{0}=people", UrlConstant.Type))
                                  : String.Format("default.aspx?action=manage{0}",
                                                  contact is Company
                                                      ? ""
                                                      : String.Format("&{0}=people", UrlConstant.Type)));
        }

        protected void SetPermission(Contact contact)
        {
            if (CRMSecurity.IsAdmin || contact.CreateBy == SecurityContext.CurrentAccount.ID)
            {
                var isPrivate = Convert.ToBoolean(Request.Form["isPrivateContact"]);
                var notifyPrivateUsers = Convert.ToBoolean(Request.Form["notifyPrivateUsers"]);

                if (isPrivate)
                {
                    var selectedUsers = Request.Form["selectedPrivateUsers"]
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => new Guid(item)).ToList();

                    selectedUsers.Add(SecurityContext.CurrentAccount.ID);

                    CRMSecurity.SetAccessTo(contact, selectedUsers);

                    if (notifyPrivateUsers)
                    {
                        if (contact is Person)
                            Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Person, contact.ID, selectedUsers.ToArray());
                        else
                            Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Company, contact.ID, selectedUsers.ToArray());
                    }
                }
                else
                {
                    CRMSecurity.MakePublic(contact);



                }
            }
        }

        protected string GetContactPhone(int contactID)
        {
            var phones = Global.DaoFactory.GetContactInfoDao().GetList(contactID, ContactInfoType.Phone, null, true);
            return phones.Count == 0 ? String.Empty : phones[0].Data.HtmlEncode();
        }

        protected string GetContactEmail(int contactID)
        {
            var emails = Global.DaoFactory.GetContactInfoDao().GetList(contactID, ContactInfoType.Email, null, true);
            return emails.Count == 0 ? String.Empty : emails[0].Data.HtmlEncode();
        }
        #endregion
    }
}