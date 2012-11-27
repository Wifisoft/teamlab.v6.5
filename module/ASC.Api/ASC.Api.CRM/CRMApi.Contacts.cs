#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.Api.Logging;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Common.Utils;
using ASC.Web.CRM.Classes;
using Newtonsoft.Json.Linq;
using Contact = ASC.CRM.Core.Entities.Contact;
using System.Runtime.Serialization;


#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {

        /// <summary>
        ///    Returns the detailed information about the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <returns>Contact</returns>
        /// <short>Get contact by ID</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"contact/{contactid:[0-9]+}")]
        public ContactWrapper GetContactByID(int contactid)
        {
            if (contactid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            return ToContactWrapper(contact);
        }

       

        /// <summary>
        ///    Returns the list of all contacts in the CRM module matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="tags">Tag</param>
        /// <param optional="true" name="contactType">Contact type ID</param>
        /// <param optional="true" name="contactListView" remark="Allowed values: Company, Person, WithOpportunity"></param>
        /// <short>Get contact list</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact list
        /// </returns>
        [Read(@"contact/filter")]
        public IEnumerable<ContactWrapper> GetContacts(IEnumerable<String> tags, int contactType, ContactListViewType contactListView)
        {
           
            IEnumerable<ContactWrapper> result;
          
            OrderBy contactsOrderBy;

            ContactSortedByType sortBy;

            var searchString = _context.FilterValue;

            if (ASC.Web.CRM.Classes.EnumExtension.TryParse(_context.SortBy, true, out sortBy))
                contactsOrderBy = new OrderBy(sortBy, !_context.SortDescending);
            else if (String.IsNullOrEmpty(_context.SortBy))
                contactsOrderBy = new OrderBy(ContactSortedByType.DisplayName, true);
            else
                contactsOrderBy = null;

            if (contactsOrderBy != null)
            {


                result = ToListContactWrapper(DaoFactory.GetContactDao()
                                                  .GetContacts(searchString,
                                                               tags,
                                                               contactType,
                                                               contactListView,
                                                               (int) _context.StartIndex,
                                                               (int) _context.Count,
                                                               contactsOrderBy));
                
             

                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();
            }
            else
            {

                result = ToListContactWrapper(DaoFactory.GetContactDao()
                                                  .GetContacts(searchString,
                                                               tags,
                                                               contactType,
                                                               contactListView,
                                                               0,
                                                               0,
                                                               null));
            }

            var totalCount = DaoFactory.GetContactDao().GetContactsCount(searchString,
                                                                          tags,
                                                                          contactType,
                                                                          contactListView);

            _context.SetTotalCount(totalCount);

            return result.ToSmartList();

        }

        /// <summary>
        ///    Returns the list of all the persons linked to the company with the ID specified in the request
        /// </summary>
        /// <param name="companyid">Company ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Get company linked persons list</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Linked persons
        /// </returns>
        [Read(@"contact/company/{companyid:[0-9]+}/person")]
        public IEnumerable<ContactWrapper> GetPeopleFromCompany(int companyid)
        {

            if (companyid <= 0)
                throw new ArgumentException();

            return ToListContactWrapper(DaoFactory.GetContactDao().GetMembers(companyid));
        }


        /// <summary>
        ///   Adds the selected person to the company with the ID specified in the request
        /// </summary>
        /// <param optional="true"  name="companyid">Company ID</param>
        /// <param optional="true" name="personid">Person ID</param>
        /// <short>Add person to company</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Person
        /// </returns>
        [Create(@"contact/company/{companyid:[0-9]+}/person")]
        public PersonWrapper AddPeopleToCompany(
            int companyid,
            int personid)
        {
            if ((companyid <= 0) || (personid <= 0))
                throw new ArgumentException();

            var company = DaoFactory.GetContactDao().GetByID(companyid);
            var person = DaoFactory.GetContactDao().GetByID(personid);

            if (person == null || company == null)
                throw new ItemNotFoundException();

            DaoFactory.GetContactDao().AddMember(personid, companyid);

            return (PersonWrapper)ToContactWrapper(person);
        }

        /// <summary>
        ///   Deletes the selected person from the company with the ID specified in the request
        /// </summary>
        /// <param optional="true"  name="companyid">Company ID</param>
        /// <param optional="true" name="personid">Person ID</param>
        /// <short>Delete person from company</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Person
        /// </returns>
        [Delete(@"contact/company/{companyid:[0-9]+}/person")]
        public PersonWrapper DeletePeopleFromCompany(
            int companyid,
            int personid)
        {
            if ((companyid <= 0) || (personid <= 0))
                throw new ArgumentException();


            var company = DaoFactory.GetContactDao().GetByID(companyid);
            var person = DaoFactory.GetContactDao().GetByID(personid);

            if (person == null || company == null)
                throw new ItemNotFoundException();

            DaoFactory.GetContactDao().RemoveMember(personid);


            return (PersonWrapper)ToContactWrapper(person);
        }


        /// <summary>
        ///    Creates the person with the parameters (first name, last name, description, etc.) specified in the request
        /// </summary>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param optional="true"  name="jobTitle">Post</param>
        /// <param optional="true" name="companyId">Company ID</param>
        /// <param optional="true" name="about">Person description text</param>
        /// <param name="isPrivate">Person privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short>Create person</short> 
        /// <category>Contacts</category>
        /// <return>Person</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/person")]
        public PersonWrapper CreatePerson(String firstName,
                                          String lastName,
                                          String jobTitle,
                                          int companyId,
                                          String about,
                                          bool isPrivate,
                                          IEnumerable<Guid> accessList,
                                          IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                          HttpPostedFileBase photo)
        {
            var peopleInst = new Person
                                 {
                                     FirstName = firstName,
                                     LastName = lastName,
                                     JobTitle = jobTitle,
                                     CompanyID = companyId,
                                     About = about
                                 };

            peopleInst.ID = DaoFactory.GetContactDao().SaveContact(peopleInst);
            peopleInst.CreateBy = ASC.Core.SecurityContext.CurrentAccount.ID;
            peopleInst.CreateOn = DateTime.UtcNow;


            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(peopleInst, accessListLocal);
            else
                CRMSecurity.MakePublic(peopleInst);

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Person, peopleInst.ID, field.Key, field.Value);
            }

            var result = (PersonWrapper)ToContactWrapper(peopleInst);

            if (photo != null)
                result.SmallFotoUrl = ChangeContactPhoto(peopleInst.ID, photo);

            return result;

        }

        /// <summary>
        ///    Changes the photo for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short> Change contact photo</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Path to contact photo
        /// </returns>
        [Update("@contact/{contactid:[0-9]+}/changephoto")]
        public String ChangeContactPhoto(int contactid, HttpPostedFileBase photo)
        {
            if (contactid == 0)
                throw new ArgumentException();

            if (!(photo.ContentType.StartsWith("image/") && photo.ContentLength > 0)) return String.Empty;

            if (!photo.InputStream.CanRead) return String.Empty;

            var buffer = new byte[photo.ContentLength];

            photo.InputStream.Read(buffer, 0, buffer.Length);

            return ContactPhotoManager.UploadPhoto(buffer, contactid);
        }


        /// <summary>
        ///    Updates the selected person with the parameters (first name, last name, description, etc.) specified in the request
        /// </summary>
        /// <param name="personid">Person ID</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param optional="true"  name="jobTitle">Post</param>
        /// <param optional="true" name="companyId">Company ID</param>
        /// <param optional="true" name="about">Person description text</param>
        /// <param name="isPrivate">Person privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short>Update person</short> 
        /// <category>Contacts</category>
        /// <return>Person</return>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update(@"contact/person/{personid:[0-9]+}")]
        public PersonWrapper UpdatePerson(
                                          int personid,
                                          String firstName,
                                          String lastName,
                                          String jobTitle,
                                          int companyId,
                                          String about,
                                          bool isPrivate,
                                          IEnumerable<Guid> accessList,
                                          IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                          HttpPostedFileBase photo)
        {

            if (personid == 0 || String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
                throw new ArgumentException();

            var peopleInst = new Person
            {
                ID = personid,
                FirstName = firstName,
                LastName = lastName,
                JobTitle = jobTitle,
                CompanyID = companyId,
                About = about
            };

            DaoFactory.GetContactDao().UpdateContact(peopleInst);

            peopleInst = (Person)DaoFactory.GetContactDao().GetByID(peopleInst.ID);

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(peopleInst, accessListLocal);
            else
                CRMSecurity.MakePublic(peopleInst);

            foreach (var field in customFieldList)
                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Person, peopleInst.ID, field.Key, field.Value);

            var result = (PersonWrapper)ToContactWrapper(peopleInst);

            if (photo != null)
                result.SmallFotoUrl = ChangeContactPhoto(peopleInst.ID, photo);

            return result;
        }

        /// <summary>
        ///    Creates the company with the parameters specified in the request
        /// </summary>
        /// <param  name="companyName">Company name</param>
        /// <param optional="true" name="about">Company description text</param>
        /// <param optional="true" name="personList">Linked person list</param>
        /// <param name="isPrivate">Company privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short>Create company</short> 
        /// <category>Contacts</category>
        /// <return>Company</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/company")]
        public CompanyWrapper CreateCompany(
                                           String companyName,
                                           String about,
                                           IEnumerable<int> personList,
                                           bool isPrivate,
                                           IEnumerable<Guid> accessList,
                                           IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                           HttpPostedFileBase photo)
        {
            var companyInst = new Company
                                  {
                                      CompanyName = companyName,
                                      About = about
                                  };


            companyInst.ID = DaoFactory.GetContactDao().SaveContact(companyInst);
            companyInst.CreateBy = ASC.Core.SecurityContext.CurrentAccount.ID;
            companyInst.CreateOn = DateTime.UtcNow;

            var personListLocal = personList.ToList();

            foreach (var personID in personListLocal)
                AddPeopleToCompany(companyInst.ID, personID);

            var accessListLocal = accessList.ToList();


            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(companyInst, accessListLocal);

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Company, companyInst.ID, field.Key, field.Value);
            }

            var result = (CompanyWrapper)ToContactWrapper(companyInst);

            if (photo != null)
                result.SmallFotoUrl = ChangeContactPhoto(companyInst.ID, photo);

            return result;
        }


		  /// <summary>
		  ///    Quickly creates the list of companies
		  /// </summary>
		  /// <short>
		  ///    Quick company list creation
		  /// </short>
		  /// <param name="companyName">Company name</param>
        /// <category>Contacts</category>
        /// <return>Contact list</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/company/quick")]
        public IEnumerable<ContactBaseWrapper> CreateCompany(IEnumerable<String> companyName)
        {
            var contacts = new List<Contact>();

            if (companyName == null) 
                throw new ArgumentException();

            foreach (var item in companyName)
            {
                if (String.IsNullOrEmpty(item)) continue;
              
                contacts.Add(new Company
                                 {
                                     CompanyName = item
                                 });
            }

            if (contacts.Count == 0) return null;

            DaoFactory.GetContactDao().SaveContactList(contacts);

            return contacts.ConvertAll(item => ToContactBaseWrapper(item));

        }

		  /// <summary>
		  ///    Quickly creates the list of persons with the first and last names specified in the request
		  /// </summary>
		  /// <short>
		  ///    Quick person list creation
		  /// </short>
		  /// <param name="data">Pairs: user first name, user last name</param>
        /// <category>Contacts</category>
        /// <return>Contact list</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/person/quick")]
        public IEnumerable<ContactBaseWrapper> CreatePerson(IEnumerable<ItemKeyValuePair<String, String>> data)
        {
            var contacts = new List<Contact>();

            if (data == null) return null;

            foreach (var item in data)
            {
                if (String.IsNullOrEmpty(item.Key) || String.IsNullOrEmpty(item.Value)) continue;

                contacts.Add(new Person
                {
                    FirstName = item.Key,
                    LastName = item.Value
                });
            }

            if (contacts.Count == 0) return null;

            var contactIDs = DaoFactory.GetContactDao().SaveContactList(contacts);

          
            return contacts.ConvertAll(item => ToContactBaseWrapper(item));
        }


        /// <summary>
        ///    Updates the selected company with the parameters specified in the request
        /// </summary>
        /// <param name="companyid">Company ID</param>
        /// <param  name="companyName">Company name</param>
        /// <param optional="true" name="about">Company description text</param>
        /// <param name="isPrivate">Company privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <short>Update company</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Company
        /// </returns>
        [Update(@"contact/company/{companyid:[0-9]+}")]
        public CompanyWrapper UpdateCompany(
                                           int companyid,
                                           String companyName,
                                           String about,
                                           bool isPrivate,
                                           IEnumerable<Guid> accessList,
                                           IEnumerable<ItemKeyValuePair<int, String>> customFieldList)
        {
            var companyInst = new Company
            {
                ID = companyid,
                CompanyName = companyName,
                About = about
            };

            DaoFactory.GetContactDao().UpdateContact(companyInst);

            companyInst = (Company)DaoFactory.GetContactDao().GetByID(companyInst.ID);

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(companyInst, accessListLocal);
            else
                CRMSecurity.MakePublic(companyInst);

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Company, companyInst.ID, field.Key, field.Value);
            }

            return (CompanyWrapper)ToContactWrapper(companyInst);

        }


        /// <summary>
        ///   Sets access rights for other users to the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="isPrivate">Contact privacy: private or not</param>
        /// <param name="accessList">List of users with access</param>
        /// <short>Set contact access rights</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact
        /// </returns>
        [Update("contact/{contactid:[0-9]+}/access")]
        public ContactWrapper SetAccessToContact(
            int contactid,
            bool isPrivate,
            IEnumerable<Guid> accessList)
        {

            if (contactid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            if (!(CRMSecurity.IsAdmin || contact.CreateBy == ASC.Core.SecurityContext.CurrentAccount.ID))
                throw new SecurityException(""); ;

            SetAccessToContact(contact, isPrivate, accessList);

            return ToContactWrapper(contact);
        }

        private void SetAccessToContact(
            Contact contact,
            bool isPrivate,
            IEnumerable<Guid> accessList)
        {

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(contact, accessListLocal);
            else
                CRMSecurity.MakePublic(contact);

        }


        /// <summary>
		  ///   Sets access rights for other users to the list of contacts with the IDs specified in the request
		  /// </summary>
        /// <param name="contactid">Contact ID list</param>
        /// <param name="isPrivate">Contact privacy: private or not</param>
        /// <param name="accessList">List of users with access</param>
        /// <short>Set contact access rights</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact list
        /// </returns>
        [Update("contact/access")]
        public IEnumerable<ContactWrapper> SetAccessToBatchContact(
            IEnumerable<int> contactid,
            bool isPrivate,
            IEnumerable<Guid> accessList
            )
        {

            var result = new List<ContactWrapper>();

            foreach (var id in contactid)
            {
                try
                {

                    var contactWrapper = SetAccessToContact(id, isPrivate, accessList);

                    result.Add(contactWrapper);

                }
                catch (Exception)
                {

                }
            }

            return result;
        }


        /// <summary>
        ///     Deletes the contact with the ID specified in the request from the portal
        /// </summary>
        /// <short>Delete contact</short> 
        /// <category>Contacts</category>
        /// <param name="contactid">Contact ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact
        /// </returns>
        [Delete(@"contact/{contactid:[0-9]+}")]
        public ContactWrapper DeleteContact(int contactid)
        {
            if (contactid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            var contactWrapper = ToContactWrapper(contact);

            DaoFactory.GetContactDao().DeleteContact(contactid);

            return contactWrapper;

        }

        /// <summary>
        ///   Deletes the group of contacts with the IDs specified in the request
        /// </summary>
        /// <param name="contactids">Contact ID list</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Delete contact group</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact list
        /// </returns>
        [Delete(@"contact")]
        public IEnumerable<ContactBaseWrapper> DeleteBatchContacts(IEnumerable<int> contactids)
        {
            if (contactids == null)
                throw new ArgumentException();

            var result = DaoFactory.GetContactDao().GetContacts(contactids.ToArray())
                .Select(contact => ToContactBaseWrapper(contact));

            DaoFactory.GetContactDao().DeleteBatchContact(contactids.ToArray());

            return result;

        }

        private IEnumerable<ContactWrapper> ToListContactWrapper(List<Contact> itemList)
        {

            if (itemList.Count == 0) return new List<ContactWrapper>();

            var result = new List<ContactWrapper>();

            var personsIDs = new List<int>();
            var companyIDs = new List<int>();
            var contactIDs = new int[itemList.Count];

            for (int index = 0; index < itemList.Count; index++)
            {
                var contact = itemList[index];

                if (contact is Company)
                    companyIDs.Add(contact.ID);
                else if (contact is Person)
                    personsIDs.Add(contact.ID);

                contactIDs[index] = itemList[index].ID;
            }

            
            var contactTypeIDs = itemList.Select(item => item.StatusID).Distinct().ToArray();
            var contactInfos = new Dictionary<int, List<ContactInfoWrapper>>();

            var haveLateTask = DaoFactory.GetTaskDao().HaveLateTask(contactIDs);
            var contactType = DaoFactory.GetListItemDao().GetItems(contactTypeIDs).ToDictionary(item => item.ID,
                                                                                                item =>
                                                                                                ToContactType(item));

            var personsCustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person,
                                                                                    personsIDs.ToArray());

            var companyCustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company,
                                                                                    companyIDs.ToArray());


            var customFields = personsCustomFields.Union(companyCustomFields).GroupBy(item => item.EntityID).ToDictionary(
                           item => item.Key, item => item.Select(x => ToCustomFieldWrapper(x)));


            var addresses = new Dictionary<int, List<Address>>();
            var taskCount = DaoFactory.GetTaskDao().GetTasksCount(contactIDs);

            var contactTags = DaoFactory.GetTagDao().GetEntitiesTags(EntityType.Contact);

            DaoFactory.GetContactInfoDao().GetAll(contactIDs).ForEach(
                 item =>
                 {
                     if (item.InfoType == ContactInfoType.Address)
                     {
                         if (!addresses.ContainsKey(item.ContactID))
                             addresses.Add(item.ContactID, new List<Address>
                                                               {
                                                                   new Address(item)
                                                               });
                         else
                             addresses[item.ContactID].Add(new Address(item));
                     }
                     else
                     {
                         if (!contactInfos.ContainsKey(item.ContactID))
                             contactInfos.Add(item.ContactID, new List<ContactInfoWrapper> { new ContactInfoWrapper(item) });
                         else
                             contactInfos[item.ContactID].Add(new ContactInfoWrapper(item));
                     }
                 }
            );

            foreach (var contact in itemList)
            {

                ContactWrapper contactWrapper;

                if (contact is Person)
                {
                    var people = (Person)contact;

                    var peopleWrapper = new PersonWrapper(people);

                    if (people.CompanyID > 0)
                    {
                        Contact peopleCompany;

                        if (companyIDs.Contains(people.CompanyID))
                            peopleCompany = itemList.Find(item => item.ID == people.CompanyID);
                        else
                            peopleCompany = DaoFactory.GetContactDao().GetByID(people.CompanyID);

                        peopleWrapper.Company = ToContactBaseWrapper(peopleCompany);
                    }

                    contactWrapper = peopleWrapper;

                }
                else if (contact is Company)
                    contactWrapper = new CompanyWrapper((Company)contact);
                else
                    throw new ArgumentException();

                if (contactTags.ContainsKey(contact.ID))
                    contactWrapper.Tags = contactTags[contact.ID];

                if (addresses.ContainsKey(contact.ID))
                    contactWrapper.Addresses = addresses[contact.ID];
                
                if (contactInfos.ContainsKey(contact.ID))
                    contactWrapper.CommonData = contactInfos[contact.ID];
                else
                    contactWrapper.CommonData = new List<ContactInfoWrapper>();

                if (contactType.ContainsKey(contact.StatusID))
                    contactWrapper.ContactType = contactType[contact.StatusID];

                if (haveLateTask.ContainsKey(contact.ID))
                    contactWrapper.HaveLateTasks = haveLateTask[contact.ID];
                else
                    contactWrapper.HaveLateTasks = false;

                if (customFields.ContainsKey(contact.ID))
                    contactWrapper.CustomFields = customFields[contact.ID];
                else
                    contactWrapper.CustomFields = new List<CustomFieldWrapper>();

                if (taskCount.ContainsKey(contact.ID))
                    contactWrapper.TaskCount = taskCount[contact.ID];
                else
                    contactWrapper.TaskCount = 0;

                result.Add(contactWrapper);
            }

            return result;

        }

        private ContactBaseWrapper ToContactBaseWrapper(Contact contact)
        {
            if (contact == null) return null;

            return new ContactBaseWrapper(contact);
        }

        private ContactWrapper ToContactWrapper(Contact contact)
        {
            ContactWrapper result;

            if (contact is Person)
            {
                var people = (Person)contact;

                var peopleWrapper = new PersonWrapper(people);

                if (people.CompanyID > 0)
                    peopleWrapper.Company = ToContactBaseWrapper(DaoFactory.GetContactDao().GetByID(people.CompanyID));

                result = peopleWrapper;

            }
            else if (contact is Company)
                result = new CompanyWrapper((Company)contact);
            else
                throw new ArgumentException();

            if (contact.StatusID > 0)
                result.ContactType = GetContactTypeByID(contact.StatusID);

            result.TaskCount = DaoFactory.GetTaskDao().GetTasksCount(contact.ID);
            result.HaveLateTasks = DaoFactory.GetTaskDao().HaveLateTask(contact.ID);

            var contactInfos = new List<ContactInfoWrapper>();
            var addresses = new List<Address>();

            var data = DaoFactory.GetContactInfoDao().GetList(contact.ID, null, null, null);

            foreach (var contactInfo in data)
                if (contactInfo.InfoType == ContactInfoType.Address)
                    addresses.Add(new Address(contactInfo));
                else
                    contactInfos.Add(new ContactInfoWrapper(contactInfo));

            result.Addresses = addresses;
            result.CommonData = contactInfos;

            if (contact is Person)
                result.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, contact.ID, false).ConvertAll(item => new CustomFieldWrapper(item)).ToSmartList();
            else
                result.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, contact.ID, false).ConvertAll(item => new CustomFieldWrapper(item)).ToSmartList();

            return result;
        }
    }
}
