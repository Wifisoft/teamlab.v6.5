using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.CRM.Core.Entities;
using System.Collections.Generic;
using ASC.Web.CRM.Classes;
using ASC.Web.Core.Utility.Skins;
using ASC.CRM.Core;
using AjaxPro;
using ASC.Core.Users;
using System.IO;
using ASC.Data.Storage;
using Newtonsoft.Json;
using ASC.Web.CRM.Resources;

namespace ASC.Web.CRM.Controls.Common
{
    [AjaxNamespace("AjaxPro.ContactSelector")]
    public partial class ContactSelector : BaseUserControl
    {
        public enum SelectorType
        {
            All = 0,
            Companies = 1,
            Persons = 2,
            PersonsWithoutCompany = 3,
            CompaniesAndPersonsWithoutCompany = 4
        }

        protected string selectorID = Guid.NewGuid().ToString().Replace('-', '_');
        protected string jsObjName;

        #region Properties

        public static    String Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/ContactSelector.ascx");
            }
        }

        public SelectorType CurrentType { get; set; }

        public List<Contact> SelectedContacts { get; set; }

        public List<Int32> ExcludedArrayIDs { get; set; }

        public EntityType EntityType { get; set; }

        public int EntityID { get; set; }

        public bool ShowContactImg { get; set; }

        public bool ShowChangeButton { get; set; }

        public bool ShowDeleteButton { get; set; }

        public bool ShowAllDeleteButton { get; set; }

        public bool ShowAddButton { get; set; }

        public bool ShowOnlySelectorContent { get; set; }

        public bool ShowNewCompanyContent { get; set; }

        public bool ShowNewContactContent { get; set; }

        public string DescriptionText { get; set; }

        public string DeleteContactText { get; set; }

        public string AddButtonText { get; set; }

        public bool IsInPopup { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "contactselector_script", WebPath.GetPath("js/jquery.watermarkinput.js"));

            jsObjName = String.IsNullOrEmpty(this.ID) ? "contactSelector_" + selectorID : this.ID;
            if (SelectedContacts != null && SelectedContacts.Count > 0 && !ShowOnlySelectorContent)
            {
                rpt.DataSource = SelectedContacts;
                rpt.DataBind();
            }
            else
            {
                var selector = (ContactSelectorRow)LoadControl(ContactSelectorRow.Location);
                selector.ObjName = jsObjName;
                selector.ObjID = jsObjName + "_0";
                selector.SelectedContact = null;
                selector.ShowChangeButton = ShowChangeButton;
                selector.ShowContactImg = ShowContactImg;
                selector.ShowDeleteButton = ShowAllDeleteButton ? true : ShowOnlySelectorContent ? false : ShowDeleteButton;
                selector.ShowAddButton = ShowAddButton;
                selector.ShowNewCompanyContent = ShowNewCompanyContent;
                selector.ShowNewContactContent = ShowNewContactContent;
                selector.DescriptionText = DescriptionText;
                selector.DeleteContactText = DeleteContactText;
                selector.AddButtonText = AddButtonText;
                ph.Controls.Add(selector);
            }


            var selectedContacts = JavaScriptSerializer.Serialize(SelectedContacts != null && SelectedContacts.Count > 0 ? SelectedContacts.ConvertAll(item => item.ID) : new List<int>());
            var excludedContacts = JavaScriptSerializer.Serialize(ExcludedArrayIDs != null && ExcludedArrayIDs.Count > 0 ? ExcludedArrayIDs : new List<int>());
            var script = String.Format(
                @"<script type='text/javascript'>
                    var {0}= new ASC.CRM.ContactSelector.ContactSelector('{0}', '{1}', {2}, '{3}', '{4}', '{5}', {6}, {7});
                    {0}.newCompanyTitleWatermark = '{8}';
                    {0}.newContactFirstNameWatermark = '{9}';
                    {0}.newContactLastNameWatermark = '{10}';
                </script>",
                jsObjName,
                Convert.ToInt32(CurrentType),
                selectedContacts,
                DescriptionText,
                DeleteContactText,
                AddButtonText,
                excludedContacts,
                IsInPopup.ToString().ToLower(),
                CRMContactResource.CompanyName.ReplaceSingleQuote(),
                CRMContactResource.FirstName.ReplaceSingleQuote(),
                CRMContactResource.LastName.ReplaceSingleQuote());


            Page.ClientScript.RegisterStartupScript(typeof(ContactSelector), Guid.NewGuid().ToString(), script);
        }

        protected void rpt_OnItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (!(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)) return;

            var row = (ContactSelectorRow)e.Item.FindControl("_row");
            var item = (Contact)e.Item.DataItem;

            row.ObjName = jsObjName;
            row.ObjID = jsObjName + "_" + e.Item.ItemIndex;
            row.SelectedContact = item;
            row.ShowContactImg = ShowContactImg;
            row.ShowChangeButton = ShowChangeButton;
            row.ShowDeleteButton = ShowAllDeleteButton ? true : e.Item.ItemIndex > 0;
            row.ShowAddButton = ShowAddButton;
            row.ShowNewCompanyContent = ShowNewCompanyContent;
            row.ShowNewContactContent = ShowNewContactContent;
            row.DescriptionText = DescriptionText;
            row.DeleteContactText = DeleteContactText;
            row.AddButtonText = AddButtonText;
        }

        #endregion

        #region Methods

        protected string GetContactsByPrefixJSON(EntityType targetEntityType, int targetEntityID, string prefix)
        {
            var allContacts = new List<Contact>();
            var findedContacts = new List<Contact>();

            switch (targetEntityType)
            {
                case EntityType.Opportunity:
                    allContacts = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetDealDao().GetMembers(targetEntityID));
                    break;
                case EntityType.Case:
                    allContacts = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetCasesDao().GetMembers(targetEntityID));
                    break;
            }

            foreach (var c in allContacts)
            {
                if (c is Person)
                {
                    var people = (Person)c;
                    if (UserFormatter.GetUserName(people.FirstName, people.LastName).IndexOf(prefix) != -1)
                        findedContacts.Add(c);
                }
                else
                {
                    var company = (Company)c;
                    if (company.CompanyName.IndexOf(prefix) != -1)
                        findedContacts.Add(c);

                }

            }

            return JavaScriptSerializer.Serialize(findedContacts.ConvertAll(contact =>
            {


                var displayTitle = String.Empty;
                var imgPath = String.Empty;

                if (contact is Person)
                {
                    var people = (Person)contact;
                    imgPath = ContactPhotoManager.GetSmallSizePhoto(people.ID, false); ;
                    displayTitle = UserFormatter.GetUserName(people.FirstName, people.LastName);
                }
                else
                {
                    var company = (Company)contact;
                    imgPath = ContactPhotoManager.GetSmallSizePhoto(company.ID, true);
                    displayTitle = company.CompanyName;

                }


                return new
                {
                    id = contact.ID,
                    title = displayTitle,
                    img = imgPath
                };
            }
                   ));
        }

        protected string GetContactImgSrc(int contactID)
        {
            if (contactID > 0)
            {
                var contact = Global.DaoFactory.GetContactDao().GetByID(contactID);
                return ContactPhotoManager.GetSmallSizePhoto(contactID, contact is Company);
            }
            return WebImageSupplier.GetAbsoluteWebPath("blank.gif");
        }

        protected string GetContactTitle(int contactID)
        {
            return contactID > 0 ? Global.DaoFactory.GetContactDao().GetByID(contactID).GetTitle() : string.Empty;
        }

        #endregion

        #region AjaxMethods

        [AjaxMethod]
        public string GetContactsByPrefix(string prefix, SelectorType selectorType, int targetEntityType, int targetEntityID)
        {
            if (targetEntityID > 0)
            {
                return GetContactsByPrefixJSON((EntityType)targetEntityType, targetEntityID, prefix);
            }

            var maxItemCount = 30;

            switch (selectorType)
            {
                case SelectorType.All:
                    return ContactsToJSON(Global.DaoFactory.GetContactDao().GetContactsByPrefix(prefix, -1, 0, maxItemCount));
                case SelectorType.Companies:
                    return ContactsToJSON(Global.DaoFactory.GetContactDao().GetContactsByPrefix(prefix, 0, 0, maxItemCount));
                case SelectorType.Persons:
                    return ContactsToJSON(Global.DaoFactory.GetContactDao().GetContactsByPrefix(prefix, 1, 0, maxItemCount));
                case SelectorType.PersonsWithoutCompany:
                    return ContactsToJSON(Global.DaoFactory.GetContactDao().GetContactsByPrefix(prefix, 2, 0, maxItemCount));
                case SelectorType.CompaniesAndPersonsWithoutCompany:
                    return ContactsToJSON(Global.DaoFactory.GetContactDao().GetContactsByPrefix(prefix, 3, 0, maxItemCount));
                default:
                    throw new ArgumentException();
            }
        }

        public String ContactsToJSON(List<Contact> contacts)
        {

            var result = new List<Object>();

            foreach (var contact in contacts)
            {
                if (contact is Company)
                {
                    result.Add(new
                    {
                        id = contact.ID,
                        title = contact.GetTitle().HtmlEncode(),
                        img = ContactPhotoManager.GetSmallSizePhoto(contact.ID, true),
                        parentID = 0,
                        parentTitle = "",
                        parentImg = ""
                    });
                }
                else
                {
                    var people = (Person)contact;

                    var title = "";
                    var img = "";

                    if (people.CompanyID != 0)
                    {
                        title = Global.DaoFactory.GetContactDao().GetByID(people.CompanyID).GetTitle();
                        img = ContactPhotoManager.GetSmallSizePhoto(people.CompanyID, true);
                    }

                    result.Add(new
                    {
                        id = contact.ID,
                        title = contact.GetTitle().HtmlEncode(),
                        img = ContactPhotoManager.GetSmallSizePhoto(contact.ID, false),
                        parentID = people.CompanyID,
                        parentTitle = title.HtmlEncode(),
                        parentImg = img
                    });
                }

            }

            return JsonConvert.SerializeObject(result);
        }


        [AjaxMethod]
        public String AddNewSelector(string jsObjName, int index, string descriptionText, string deleteContactText, string addContactText)
        {
            var cntrlContactSelector = (ContactSelectorRow)LoadControl(ContactSelectorRow.Location);
            cntrlContactSelector.ObjName = jsObjName;
            cntrlContactSelector.ObjID = jsObjName + "_" + index;
            cntrlContactSelector.SelectedContact = null;
            cntrlContactSelector.ShowContactImg = true;
            cntrlContactSelector.ShowChangeButton = true;
            cntrlContactSelector.ShowDeleteButton = true;
            cntrlContactSelector.ShowAddButton = true;
            cntrlContactSelector.ShowNewCompanyContent = true;
            cntrlContactSelector.ShowNewContactContent = true;
            cntrlContactSelector.DescriptionText = descriptionText;
            cntrlContactSelector.DeleteContactText = deleteContactText;
            cntrlContactSelector.AddButtonText = addContactText;

            var page = new Page();

            page.Controls.Add(cntrlContactSelector);
            var writer = new StringWriter();
            HttpContext.Current.Server.Execute(page, writer, false);
            var output = writer.ToString();
            writer.Close();

            return output;

        }

        [AjaxMethod]
        public String AddNewContact(bool isCompany, string companyName, string firstName, string lastName)
        {
            if (isCompany)
            {
                var company = new Company
                {
                    CompanyName = companyName.Trim()
                };

                company.ID = Global.DaoFactory.GetContactDao().SaveContact(company);
                CRMSecurity.MakePublic(company);

                return JsonConvert.SerializeObject(new
                {
                    id = company.ID,
                    title = company.GetTitle().HtmlEncode(),
                    img = ContactPhotoManager.GetSmallSizePhoto(company.ID, true)
                });
            }
            else
            {
                var contact = new Person
                {
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim()
                };

                contact.ID = Global.DaoFactory.GetContactDao().SaveContact(contact);
                CRMSecurity.MakePublic(contact);
            

                return JsonConvert.SerializeObject(new
                {
                    id = contact.ID,
                    title = contact.GetTitle().HtmlEncode(),
                    img = ContactPhotoManager.GetSmallSizePhoto(contact.ID, false)
                });
            }

        }



        #endregion
    }
}