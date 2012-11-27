#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using ASC.Collections;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Entities;
using ASC.Files.Core;
using ASC.FullTextIndex;
using ASC.Web.Files.Api;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;
using ASC.Core.Caching;

#endregion

namespace ASC.CRM.Core.Dao
{


    public class CachedContactDao : ContactDao
    {

        private readonly HttpRequestDictionary<Contact> _contactCache = new HttpRequestDictionary<Contact>("crm_contact");

        public CachedContactDao(int tenantID, string storageKey)
            : base(tenantID, storageKey)
        {

        }

        public override Contact GetByID(int contactID)
        {
            return _contactCache.Get(contactID.ToString(), () => GetByIDBase(contactID));

        }

        private Contact GetByIDBase(int contactID)
        {
            return base.GetByID(contactID);
        }

        public override void DeleteContact(int contactID)
        {

            ResetCache(contactID);

            base.DeleteContact(contactID);

        }

        public override void UpdateContact(Contact contact)
        {
            if (contact != null && contact.ID > 0)
                ResetCache(contact.ID);

            base.UpdateContact(contact);
        }


        public override int SaveContact(Contact contact)
        {

            if (contact != null)
            {
                ResetCache(contact.ID);
            }

            return base.SaveContact(contact);

        }

        private void ResetCache(int contactID)
        {
            _contactCache.Reset(contactID.ToString());
        }
    }




    public class ContactDao : AbstractDao
    {
        #region Constructor

        public ContactDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {
        }

        #endregion

        #region Members

        private readonly String _displayNameSeparator = "!=!";


        #endregion

        public List<Contact> GetContactsByPrefix(String prefix, int searchType, int from, int count)
        {
            if (count == 0)
                throw new ArgumentException();

            prefix = prefix.Trim();

            var keywords = prefix.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var q = GetContactSqlQuery(null);

            switch (searchType)
            {
                case 0: // Company
                    q.Where(Exp.Eq("is_company", true));
                    break;
                case 1: // Persons
                    q.Where(Exp.Eq("is_company", false));
                    break;
                case 2: // PersonsWithoutCompany
                    q.Where(Exp.Eq("company_id", 0) & Exp.Eq("is_company", false));

                    break;
                case 3: // CompaniesAndPersonsWithoutCompany
                    q.Where(Exp.Eq("company_id", 0));

                    break;
                default:

                    break;
            }

            if (keywords.Length == 1)
            {
                q.Where(Exp.Like("display_name", keywords[0]));
            }
            else
            {
                foreach (var k in keywords)
                {
                    q.Where(Exp.Like("display_name", k));
                }
            }

            if (0 < from && from < int.MaxValue) q.SetFirstResult(from);
            if (0 < count && count < int.MaxValue) q.SetMaxResults(count);

            var sqlResult = DbManager.ExecuteList(q).ConvertAll(row => ToContact(row)).FindAll(CRMSecurity.CanAccessTo);

            return sqlResult.OrderBy(contact => contact.GetTitle()).ToList();

        }

        public int GetAllContactsCount()
        {
            return GetContactsCount(String.Empty, null, 0, ContactListViewType.All);
        }


        public List<Contact> GetAllContacts()
        {
            return DbManager.ExecuteList(GetContactSqlQuery(null)).ConvertAll(contact => ToContact(contact)).FindAll(CRMSecurity.CanAccessTo);
        }

        public int GetContactsCount(String searchText,
                                   IEnumerable<String> tags,
                                   int contactStatus,
                                   ContactListViewType contactListView)
        {

            var cacheKey = TenantID.ToString() +
                           "contacts" +
                           SecurityContext.CurrentAccount.ID.ToString() +
                           searchText +
                           contactStatus +
                           (int)contactListView;
            
            if (tags != null)
                cacheKey += String.Join("", tags.ToArray());

            var fromCache = _cache.Get(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = !(String.IsNullOrEmpty(searchText) && (tags == null || !tags.Any()) && contactStatus <= 0 &&
                             contactListView == ContactListViewType.All);

            int result;

            if (withParams)
            {
                ICollection<int> excludedContactIDs;

                switch (contactListView)
                {
                    case ContactListViewType.Person:
                        excludedContactIDs = CRMSecurity.GetPrivateItems(typeof(Person)).ToList();
                        break;
                    case ContactListViewType.Company:
                        excludedContactIDs = CRMSecurity.GetPrivateItems(typeof(Company)).ToList();
                        break;
                    default:
                        excludedContactIDs = CRMSecurity.GetPrivateItems(typeof(Company)).Union(CRMSecurity.GetPrivateItems(typeof(Person))).ToList();
                        break;
                }

                var whereConditional = WhereConditional(excludedContactIDs, searchText, tags, contactStatus, contactListView);

                if (whereConditional != null)
                    result = DbManager.ExecuteScalar<int>(Query("crm_contact").Where(whereConditional).SelectCount());
                else
                    result = 0;
            }
            else
            {

                var countWithoutPrivate = DbManager.ExecuteScalar<int>(Query("crm_contact").SelectCount());
                var privateCount = CRMSecurity.GetPrivateItemsCount(typeof(Person)) +
                                   CRMSecurity.GetPrivateItemsCount(typeof(Company));

                if (privateCount > countWithoutPrivate)
                {
                    _log.Error("Private contacts count more than all contacts");

                    privateCount = 0;
                }

                result = countWithoutPrivate - privateCount;
            }

            if (result > 0)
                _cache.Insert(cacheKey, result, new CacheDependency(null, new[] { _contactCacheKey }), Cache.NoAbsoluteExpiration,
                                      TimeSpan.FromSeconds(30));

            return result;
        }

        public List<Contact> GetContacts(String searchText, IEnumerable<String> tags, int contactStatus, ContactListViewType contactListView, int from,
                                         int count, OrderBy orderBy)
        {
            if (CRMSecurity.IsAdmin)
                return GetCrudeContacts(
                                        searchText,
                                        tags,
                                        contactStatus,
                                        contactListView,
                                        from,
                                        count,
                                        orderBy);

            var crudeContacts = GetCrudeContacts(
                                        searchText,
                                        tags,
                                        contactStatus,
                                        contactListView,
                                        0,
                                        from + count,
                                        orderBy);

            if (crudeContacts.Count == 0) return crudeContacts;

            if (crudeContacts.Count < from + count) return crudeContacts
                                                           .FindAll(CRMSecurity.CanAccessTo).Skip(from).ToList();

            var result = crudeContacts.FindAll(CRMSecurity.CanAccessTo);

            if (result.Count == crudeContacts.Count) return result.Skip(from).ToList();

            var localCount = count;
            var localFrom = from + count;

            while (true)
            {
                crudeContacts = GetCrudeContacts(
                                        searchText,
                                        tags,
                                        contactStatus,
                                        contactListView,
                                        localFrom,
                                        localCount,
                                        orderBy);

                if (crudeContacts.Count == 0) break;

                result.AddRange(crudeContacts.Where(CRMSecurity.CanAccessTo));

                if ((result.Count >= count + from) || (crudeContacts.Count < localCount)) break;

                localFrom += localCount;
                localCount = localCount * 2;
            }

            return result.Skip(from).Take(count).ToList();
        }

        private Exp WhereConditional(
            ICollection<int> exceptIDs,
            String searchText,
            IEnumerable<String> tags,
            int contactStatus,
            ContactListViewType contactListView)
        {

            var conditions = new List<Exp>();

            var ids = new List<int>();

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (!FullTextSearch.SupportModule(FullTextSearch.CRMContactsModule))
                    conditions.Add(BuildLike(new[] { "display_name" }, keywords));
                else
                {
                    ids = FullTextSearch.Search(searchText, FullTextSearch.CRMContactsModule)
                                 .GetIdentifiers()
                                 .Select(item => Convert.ToInt32(item.Split('_')[1])).Distinct()
                                 .ToList();

                    if (ids.Count == 0) return null;

                }

            }

            if (tags != null && tags.Any())
            {
                ids = SearchByTags(EntityType.Contact, ids.ToArray(), tags);

                if (ids.Count == 0) return null;
            }

            switch (contactListView)
            {
                case ContactListViewType.Company:
                    conditions.Add(Exp.Eq("is_company", true));
                    break;
                case ContactListViewType.Person:
                    conditions.Add(Exp.Eq("is_company", false));
                    break;
                case ContactListViewType.WithOpportunity:
                    if (ids.Count > 0)
                        ids = DbManager.ExecuteList(Query("crm_deal").Select("contact_id")
                                             .Distinct()
                                             .Where(Exp.In("contact_id", ids))).ConvertAll(row => Convert.ToInt32(row[0]));
                    else
                        ids = DbManager.ExecuteList(Query("crm_deal").Select("contact_id")
                                             .Distinct()
                                             .Where(!Exp.Eq("contact_id", 0))).ConvertAll(row => Convert.ToInt32(row[0]));

                    if (ids.Count == 0) return null;

                    break;
            }

            if (contactStatus > 0)
                conditions.Add(Exp.Eq("status_id", contactStatus));

            if (ids.Count > 0)
            {
                if (exceptIDs.Count > 0)
                {
                    ids = ids.Except(exceptIDs).ToList();
                    if (ids.Count == 0) return null;
                }

                conditions.Add(Exp.In("id", ids));

            }
            else if (exceptIDs.Count > 0)
            {
                conditions.Add(!Exp.In("id", exceptIDs.ToArray()));
            }

            if (conditions.Count == 0) return null;

            if (conditions.Count == 1) return conditions[0];

            return conditions.Aggregate((i, j) => i & j);
        }

        private List<Contact> GetCrudeContacts(
            String searchText,
            IEnumerable<String> tags,
            int contactStatus,
            ContactListViewType contactListView,
            int from,
            int count,
            OrderBy orderBy)
        {

            var sqlQuery = GetContactSqlQuery(null);

            var withParams = !(String.IsNullOrEmpty(searchText) && (tags == null || !tags.Any()) && contactStatus <= 0 &&
                          contactListView == ContactListViewType.All);

            var whereConditional = WhereConditional(new List<int>(), searchText, tags, contactStatus, contactListView);

            if (withParams && whereConditional == null)
                return new List<Contact>();
            
            sqlQuery.Where(whereConditional);

            if (0 < from && from < int.MaxValue) sqlQuery.SetFirstResult(from);
            if (0 < count && count < int.MaxValue) sqlQuery.SetMaxResults(count);

            if (orderBy != null)
            {

                if (!Enum.IsDefined(typeof(ContactSortedByType), orderBy.SortedBy.ToString()))
                    orderBy.SortedBy = ContactSortedByType.DisplayName;

                switch ((ContactSortedByType)orderBy.SortedBy)
                {
                    case ContactSortedByType.DisplayName:
                        sqlQuery.OrderBy("display_name", orderBy.IsAsc);
                        break;
                    case ContactSortedByType.Created:
                        sqlQuery.OrderBy("create_on", orderBy.IsAsc);
                        break;
                    case ContactSortedByType.ContactType:
                        sqlQuery.OrderBy("status_id", orderBy.IsAsc);
                        break;
                    default:
                        sqlQuery.OrderBy("display_name", orderBy.IsAsc);
                        break;
                }
            }

            return DbManager.ExecuteList(sqlQuery).ConvertAll(contact => ToContact(contact));
        }

        public List<Contact> GetContactsByName(String title)
        {
            if (String.IsNullOrEmpty(title)) return new List<Contact>();

            title = title.Trim();

            var titleParts = title.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            if (titleParts.Length == 1)
                return DbManager.ExecuteList(GetContactSqlQuery(Exp.Eq("display_name", title)))
                       .ConvertAll(row => ToContact(row))
                       .FindAll(CRMSecurity.CanAccessTo);
            else if (titleParts.Length == 2)
                return DbManager.ExecuteList(GetContactSqlQuery(Exp.Eq("display_name", String.Concat(titleParts[0], _displayNameSeparator, titleParts[1]))))
                          .ConvertAll(row => ToContact(row))
                          .FindAll(CRMSecurity.CanAccessTo);
            
            return GetContacts(title, null, 0, ContactListViewType.All, 0, 0, null);
        }
        
        public void RemoveMember(int[] peopleID)
        {
            if ((peopleID == null) || (peopleID.Length == 0)) return;

            DbManager.ExecuteNonQuery(Update("crm_contact").Set("company_id", 0).Where(Exp.In("id", peopleID)));

            RemoveRelative(null, EntityType.Person, peopleID);

        }

        public void RemoveMember(int peopleID)
        {

            DbManager.ExecuteNonQuery(Update("crm_contact").Set("company_id", 0).Where("id", peopleID));

            RemoveRelative(0, EntityType.Person, peopleID);

        }

        public void AddMember(int peopleID, int companyID)
        {
            DbManager.ExecuteNonQuery(Update("crm_contact")
                .Set("company_id", companyID)
                .Set("status_id", 0)
                .Where("id", peopleID));
            SetRelative(companyID, EntityType.Person, peopleID);
        }

        public void SetMembers(int companyID, params int[] peopleIDs)
        {
            if (companyID == 0)
                throw new ArgumentException();

            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(new SqlDelete("crm_entity_contact")
                                        .Where(Exp.Eq("entity_type", EntityType.Person) &
                                         Exp.Eq("contact_id", companyID)));

                DbManager.ExecuteNonQuery(Update("crm_contact")
                                       .Set("company_id", 0)
                                       .Where(Exp.Eq("company_id", companyID)));

                if (!(peopleIDs == null || peopleIDs.Length == 0))
                {
                    DbManager.ExecuteNonQuery(Update("crm_contact")
                                       .Set("company_id", companyID)
                                       .Set("status_id", 0)
                                       .Where(Exp.In("id", peopleIDs)));

                    foreach (var peopleID in peopleIDs)
                        SetRelative(companyID, EntityType.Person, peopleID);
                }

                tx.Commit();

            }

        }

        public List<Contact> GetMembers(int companyID)
        {
            return GetContacts(GetRelativeToEntity(companyID, EntityType.Person, null));
        }

        public int GetMembersCount(int companyID)
        {
            return DbManager.ExecuteScalar<int>(
                 new SqlQuery("crm_entity_contact")
                .SelectCount()
                .Where(Exp.Eq("contact_id", companyID) & Exp.Eq("entity_type", (int)EntityType.Person)));
        }

        public virtual void UpdateContact(Contact contact)
        {
            CRMSecurity.DemandAccessTo(contact);

            String firstName;
            String lastName;
            String companyName;
            String title;
            int companyID;

            var displayName = String.Empty;

            if (contact is Company)
            {
                firstName = String.Empty;
                lastName = String.Empty;
                title = String.Empty;
                companyName = ((Company)contact).CompanyName.Trim();
                companyID = 0;
                displayName = companyName;

                if (String.IsNullOrEmpty(companyName))
                    throw new ArgumentException();

            }
            else
            {
                var people = (Person)contact;

                firstName = people.FirstName.Trim();
                lastName = people.LastName.Trim();
                title = people.JobTitle;
                companyName = String.Empty;
                companyID = people.CompanyID;

                displayName = String.Concat(firstName, _displayNameSeparator, lastName);

                RemoveMember(people.ID);

                if (companyID > 0)
                {
                    AddMember(people.ID, companyID);

                    contact.StatusID = 0;
                }

                if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
                    throw new ArgumentException();

            }

            if (!String.IsNullOrEmpty(title))
                title = title.Trim();

            if (!String.IsNullOrEmpty(contact.About))
                contact.About = contact.About.Trim();

            if (!String.IsNullOrEmpty(contact.Industry))
                contact.Industry = contact.Industry.Trim();


            DbManager.ExecuteNonQuery(
                Update("crm_contact")
                    .Set("first_name", firstName)
                    .Set("last_name", lastName)
                    .Set("company_name", companyName)
                    .Set("title", title)
                    .Set("notes", contact.About)
                    .Set("industry", contact.Industry)
                    .Set("status_id", contact.StatusID)
                    .Set("company_id", companyID)
                    .Set("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                    .Set("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                    .Set("display_name", displayName)
                    .Where(Exp.Eq("id", contact.ID)));

            // Delete relative  keys
            _cache.Insert(_contactCacheKey, String.Empty);


        }

        public List<Object[]> FindDuplicateByEmail(List<ContactInfo> items)
        {

            if (items.Count == 0) return new List<Object[]>();

            var result = new List<Object[]>();

            using (var tx = DbManager.BeginTransaction())
            {

                var sqlQueryStr = @"
                                    CREATE  TEMPORARY TABLE IF NOT EXISTS `crm_dublicated` (
	                                `contact_id` INT(11) NOT NULL,
	                                `email` VARCHAR(255) NOT NULL DEFAULT '0',
	                                `tenant_id` INT(11) NOT NULL DEFAULT '0'	
                                );";
                
                DbManager.ExecuteNonQuery(sqlQueryStr);

                DbManager.ExecuteNonQuery(Delete("crm_dublicated"));

                foreach (var item in items)
                    DbManager.ExecuteNonQuery(
                         Insert("crm_dublicated")
                        .InColumnValue("contact_id", item.ContactID)
                        .InColumnValue("email", item.Data));

                var sqlQuery = Query("crm_dublicated tblLeft")
                               .Select("tblLeft.contact_id",
                                       "tblLeft.email",
                                       "tblRight.contact_id",
                                       "tblRight.data")
                               .LeftOuterJoin("crm_contact_info tblRight", Exp.EqColumns("tblLeft.tenant_id", "tblRight.tenant_id") & Exp.EqColumns("tblLeft.email", "tblRight.data"))
                               .Where(Exp.Eq("tblRight.tenant_id", TenantID) & Exp.Eq("tblRight.type", (int)ContactInfoType.Email ));

                result = DbManager.ExecuteList(sqlQuery);


                tx.Commit();

            }

            return result;


        }

        public virtual Dictionary<int,int>  SaveContactList(List<Contact> items)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var result = new Dictionary<int, int>();

                foreach (var item in items)
                {

                    result.Add(item.ID, SaveContact(item));

                }

                tx.Commit();


                return result;
            }
        }

        public virtual int SaveContact(Contact contact)
        {
            // Delete relative  keys
            _cache.Insert(_contactCacheKey, String.Empty);

            String firstName;
            String lastName;
            bool isCompany;
            String companyName;
            String title;
            int companyID;

            var displayName = String.Empty;

            if (contact is Company)
            {
                firstName = String.Empty;
                lastName = String.Empty;
                title = String.Empty;
                companyName = ((Company)contact).CompanyName.Trim();
                isCompany = true;
                companyID = 0;
                displayName = companyName;

                if (String.IsNullOrEmpty(companyName))
                    throw new ArgumentException();

            }
            else
            {
                var people = (Person)contact;

                firstName = people.FirstName.Trim();
                lastName = people.LastName.Trim();
                title = people.JobTitle;
                companyName = String.Empty;
                isCompany = false;
                companyID = people.CompanyID;

                displayName = String.Concat(firstName, _displayNameSeparator, lastName);


                if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
                    throw new ArgumentException();

            }

            if (!String.IsNullOrEmpty(title))
                title = title.Trim();

            if (!String.IsNullOrEmpty(contact.About))
                contact.About = contact.About.Trim();

            if (!String.IsNullOrEmpty(contact.Industry))
                contact.Industry = contact.Industry.Trim();

            var contactID = DbManager.ExecuteScalar<int>(
                Insert("crm_contact")
                    .InColumnValue("id", 0)
                    .InColumnValue("first_name", firstName)
                    .InColumnValue("last_name", lastName)
                    .InColumnValue("company_name", companyName)
                    .InColumnValue("title", title)
                    .InColumnValue("notes", contact.About)
                    .InColumnValue("is_company", isCompany)
                    .InColumnValue("industry", contact.Industry)
                    .InColumnValue("status_id", contact.StatusID)
                    .InColumnValue("company_id", companyID)
                    .InColumnValue("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                    .InColumnValue("create_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                    .InColumnValue("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                    .InColumnValue("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                    .InColumnValue("display_name", displayName)
                    .Identity(1, 0, true));

            contact.ID = contactID;

            if (companyID > 0)
                AddMember(contactID, companyID);

            return contactID;

        }

        public virtual Contact GetByID(int contactID)
        {
            SqlQuery sqlQuery = GetContactSqlQuery(Exp.Eq("id", contactID));

            var contacts = DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToContact(row));

            if (contacts.Count == 0) return null;

            return contacts[0];
        }

        public List<Contact> GetContacts(int[] contactID)
        {
            if (contactID == null || contactID.Length == 0) return new List<Contact>();

            SqlQuery sqlQuery = GetContactSqlQuery(Exp.In("id", contactID));

            return DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToContact(row)).FindAll(CRMSecurity.CanAccessTo);

        }

        public virtual void DeleteBatchContact(int[] contactID)
        {
            if (contactID == null || contactID.Length == 0) return;

            // Delete relative  keys
            _cache.Insert(_contactCacheKey, String.Empty);

            var contacts = GetContacts(contactID).Where(CRMSecurity.CanAccessTo).ToList();

            if (contacts.Count == 0) return;

            var personsID = new List<int>();
            var companyID = new List<int>();
            var newContactID = new List<int>();

            foreach (var contact in contacts)
            {
                newContactID.Add(contact.ID);

                if (contact is Company)
                    companyID.Add(contact.ID);
                else
                    personsID.Add(contact.ID);
            }

            contactID = newContactID.ToArray();

            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("crm_field_value").Where(Exp.In("entity_id", contactID)
                    & Exp.In("entity_type", new[] { (int)EntityType.Contact, (int)EntityType.Person, (int)EntityType.Company })));

                DbManager.ExecuteNonQuery(Delete("crm_task").Where(Exp.In("contact_id", contactID)));
                DbManager.ExecuteNonQuery(new SqlDelete("crm_entity_tag")
                                         .Where(Exp.In("entity_id", contactID) & Exp.Eq("entity_type", (int)EntityType.Contact)));

                DbManager.ExecuteNonQuery(Delete("crm_relationship_event").Where(Exp.In("contact_id", contactID)));
                DbManager.ExecuteNonQuery(Update("crm_deal").Set("contact_id", 0).Where(Exp.In("contact_id", contactID)));

                if (companyID.Count > 0)
                    DbManager.ExecuteNonQuery(Update("crm_contact").Set("company_id", 0).Where(Exp.In("company_id", companyID)));

                if (personsID.Count > 0)
                    RemoveRelative(null, EntityType.Person, personsID.ToArray());

                RemoveRelative(contactID, EntityType.Any, null);

                DbManager.ExecuteNonQuery(Delete("crm_contact_info").Where(Exp.In("contact_id", contactID)));

                DbManager.ExecuteNonQuery(Delete("crm_contact").Where(Exp.In("id", contactID)));

                tx.Commit();
            }

            contacts.ForEach(contact => CoreContext.AuthorizationManager.RemoveAllAces(contact));


            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {

                var tagNames = DbManager.ExecuteList(Query("crm_relationship_event").Select("id").Where(Exp.In("contact_id", contactID) & Exp.Eq("have_files", true)))
                        .Select(row => String.Format("RelationshipEvent{0}", row[0])).ToArray();

                if (tagNames.Length == 0) return;

                var filesIDs = tagdao.GetTags(tagNames, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();

                var store = FilesIntegration.GetStore();

                foreach (var filesID in filesIDs)
                {
                    filedao.DeleteFolder(filesID);
                    filedao.DeleteFile(filesID);
                }
            }

        }

        public virtual void DeleteContact(int contactID)
        {

            if (contactID <= 0) return;

            DeleteBatchContact(new[] { contactID });

        }

        private void MergeContactInfo(Contact fromContact, Contact toContact)
        {

            if ((toContact is Person) && (fromContact is Person))
            {
                var fromPeople = (Person)fromContact;
                var toPeople = (Person)toContact;

                if (toPeople.CompanyID == 0)
                    toPeople.CompanyID = fromPeople.CompanyID;

                if (String.IsNullOrEmpty(toPeople.JobTitle))
                    toPeople.JobTitle = fromPeople.JobTitle;
            }

            if (String.IsNullOrEmpty(toContact.Industry))
                toContact.Industry = fromContact.Industry;

            if (toContact.StatusID == 0)
                toContact.StatusID = fromContact.StatusID;

            if (String.IsNullOrEmpty(toContact.About))
                toContact.About = fromContact.About;

            UpdateContact(toContact);

        }

        public void MergeDublicate(int fromContactID, int toContactID)
        {
            var fromContact = GetByID(fromContactID);
            var toContact = GetByID(toContactID);

            if (fromContact == null || toContact == null)
                throw new ArgumentException();

            using (var tx = DbManager.BeginTransaction())
            {
                ISqlInstruction q = Update("crm_task")
                    .Set("contact_id", toContactID)
                    .Where(Exp.Eq("contact_id", fromContactID));
                DbManager.ExecuteNonQuery(q);

                // crm_entity_contact
                q = new SqlQuery("crm_entity_contact l")
                    .From("crm_entity_contact r")
                    .Select("l.entity_id", "l.entity_type", "l.contact_id")
                    .Where(Exp.EqColumns("l.entity_id", "r.entity_id") & Exp.EqColumns("l.entity_type", "r.entity_type"))
                    .Where("l.contact_id", fromContactID)
                    .Where("r.contact_id", toContactID);
                DbManager.ExecuteList(q)
                    .ForEach(row =>
                        DbManager.ExecuteNonQuery(new SqlDelete("crm_entity_contact").Where("entity_id", row[0]).Where("entity_type", row[1]).Where("contact_id", row[2]))
                 );

                q = new SqlUpdate("crm_entity_contact")
                    .Set("contact_id", toContactID)
                    .Where("contact_id", fromContactID);
                DbManager.ExecuteNonQuery(q);

                // crm_deal
                q = Update("crm_deal")
                    .Set("contact_id", toContactID)
                    .Where("contact_id", fromContactID);
                DbManager.ExecuteNonQuery(q);

                // crm_relationship_event
                q = Update("crm_relationship_event")
                    .Set("contact_id", toContactID)
                    .Where("contact_id", fromContactID);
                DbManager.ExecuteNonQuery(q);

                // crm_entity_tag
                q = new SqlQuery("crm_entity_tag l")
                    .Select("l.tag_id")
                    .From("crm_entity_tag r")
                    .Where(Exp.EqColumns("l.tag_id", "r.tag_id") & Exp.EqColumns("l.entity_type", "r.entity_type"))
                    .Where("l.entity_id", fromContactID)
                    .Where("r.entity_id", toContactID);
                var dublicateTagsID = DbManager.ExecuteList(q).ConvertAll(row => row[0]);

                q = new SqlDelete("crm_entity_tag").Where(Exp.Eq("entity_id", fromContactID) & Exp.Eq("entity_type", (int)EntityType.Contact) & Exp.In("tag_id", dublicateTagsID));
                DbManager.ExecuteNonQuery(q);

                q = new SqlUpdate("crm_entity_tag").Set("entity_id", toContactID).Where("entity_id", fromContactID).Where("entity_type", (int)EntityType.Contact);
                DbManager.ExecuteNonQuery(q);

                // crm_field_value
                q = Query("crm_field_value l")
                    .From("crm_field_value r")
                    .Select("l.field_id")
                    .Where(Exp.EqColumns("l.tenant_id", "r.tenant_id") & Exp.EqColumns("l.field_id", "r.field_id") & Exp.EqColumns("l.entity_type", "r.entity_type"))
                    .Where("l.entity_id", fromContactID)
                    .Where("r.entity_id", toContactID);
                var dublicateCustomFieldValueID = DbManager.ExecuteList(q).ConvertAll(row => row[0]);

                q = Delete("crm_field_value")
                    .Where("entity_id", fromContactID)
                    .Where(Exp.In("entity_type", new[] { (int)EntityType.Contact, (int)EntityType.Person, (int)EntityType.Company }))
                    .Where(Exp.In("field_id", dublicateCustomFieldValueID));
                DbManager.ExecuteNonQuery(q);

                q = Update("crm_field_value")
                    .Set("entity_id", toContactID)
                    .Where("entity_id", fromContactID)
                    .Where("entity_type", (int)EntityType.Contact);
                DbManager.ExecuteNonQuery(q);

                // crm_contact_info
                q = Query("crm_contact_info l")
                    .From("crm_contact_info r")
                    .Select("l.id")
                    .Where(Exp.EqColumns("l.tenant_id", "r.tenant_id"))
                    .Where(Exp.EqColumns("l.type", "r.type"))
                    .Where(Exp.EqColumns("l.is_primary", "r.is_primary"))
                    .Where(Exp.EqColumns("l.category", "r.category"))
                    .Where(Exp.EqColumns("l.data", "r.data"))
                    .Where("l.contact_id", fromContactID)
                    .Where("r.contact_id", toContactID);
                var dublicateContactInfoID = DbManager.ExecuteList(q).ConvertAll(row => row[0]);

                q = Delete("crm_contact_info")
                    .Where("contact_id", fromContactID)
                    .Where(Exp.In("id", dublicateContactInfoID));
                DbManager.ExecuteNonQuery(q);

                q = Update("crm_contact_info")
                    .Set("contact_id", toContactID)
                    .Where("contact_id", fromContactID);
                DbManager.ExecuteNonQuery(q);


                MergeContactInfo(fromContact, toContact);

                // crm_contact
                if ((fromContact is Company) && (toContact is Company))
                {
                    q = Update("crm_contact")
                        .Set("company_id", toContactID)
                        .Where("company_id", fromContactID);
                    DbManager.ExecuteNonQuery(q);
                }

                q = Delete("crm_contact").Where("id", fromContactID);
                DbManager.ExecuteNonQuery(q);

                tx.Commit();
            }

            CoreContext.AuthorizationManager.RemoveAllAces(fromContact);
        }

        protected static Contact ToContact(object[] row)
        {

            Contact contact;

            var isCompany = Convert.ToBoolean(row[6]);

            if (isCompany)
                contact = new Company
                           {
                               CompanyName = Convert.ToString(row[3])
                           };
            else
                contact = new Person
                       {
                           FirstName = Convert.ToString(row[1]),
                           LastName = Convert.ToString(row[2]),
                           JobTitle = Convert.ToString(row[4]),
                           CompanyID = Convert.ToInt32(row[9])
                       };

            contact.ID = Convert.ToInt32(row[0]);
            contact.About = Convert.ToString(row[5]);
            contact.Industry = Convert.ToString(row[7]);
            contact.StatusID = Convert.ToInt32(row[8]);
            contact.CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[10]));
            contact.CreateBy = ToGuid(row[11]);

            return contact;

        }

        private String[] GetContactColumnsTable(String alias)
        {
            if (!String.IsNullOrEmpty(alias))
                alias = alias + ".";

            var result = new List<String>
                             {
                                 "id",
                                 "first_name",
                                 "last_name",
                                 "company_name",
                                 "title",
                                 "notes",
                                 "is_company",
                                 "industry",
                                 "status_id",
                                 "company_id",
                                 "create_on",
                                 "create_by",
                                 "display_name"        
                             };

            if (String.IsNullOrEmpty(alias)) return result.ToArray();

            return result.ConvertAll(item => String.Concat(alias, item)).ToArray();
        }

        private SqlQuery GetContactSqlQuery(Exp where, String alias)
        {

            var sqlQuery = Query("crm_contact");

            if (!String.IsNullOrEmpty(alias))
            {
                sqlQuery = new SqlQuery(String.Concat("crm_contact ", alias))
                           .Where(Exp.Eq(alias + ".tenant_id", TenantID));
                sqlQuery.Select(GetContactColumnsTable(alias));

            }
            else
                sqlQuery.Select(GetContactColumnsTable(String.Empty));


            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;

        }

        private SqlQuery GetContactSqlQuery(Exp where)
        {
            return GetContactSqlQuery(where, String.Empty);

        }

    }
}