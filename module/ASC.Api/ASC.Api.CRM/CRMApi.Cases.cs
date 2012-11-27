#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Core.Tenants;
using ASC.Specific;
using EnumExtension = ASC.Web.CRM.Classes.EnumExtension;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {


        /// <summary>
        ///   Open anew the case with the ID specified in the request
        /// </summary>
        /// <short>Resume case</short> 
        /// <category>Cases</category>
        /// <param name="caseid">Case ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Case
        /// </returns>
        [Update("case/{caseid:[0-9]+}/close")]
        public CasesWrapper CloseCases(int caseid)
        {
            if (caseid <= 0)
                throw new ArgumentException();

            var cases = DaoFactory.GetCasesDao().GetByID(caseid);

            if (cases == null)
                throw new ItemNotFoundException();

            DaoFactory.GetCasesDao().CloseCases(caseid);

            return ToCasesWrapper(cases);
        }

        /// <summary>
        ///   Close the case with the ID specified in the request
        /// </summary>
        /// <short>Close case</short> 
        /// <category>Cases</category>
        /// <param name="caseid">Case ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Case
        /// </returns>
        [Update("case/{caseid:[0-9]+}/reopen")]
        public CasesWrapper ReOpenCases(int caseid)
        {
            if (caseid <= 0)
                throw new ArgumentException();

            var cases = DaoFactory.GetCasesDao().GetByID(caseid);

            if (cases == null)
                throw new ItemNotFoundException();

            DaoFactory.GetCasesDao().ReOpenCases(caseid);

            return ToCasesWrapper(cases);

        }


        /// <summary>
        ///    Creates the case with the parameters specified in the request
        /// </summary>
        /// <short>Create case</short> 
        /// <param name="title">Case title</param>
        /// <param optional="true" name="members">Participants</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="isPrivate">Case privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access to the case</param>
        /// <returns>Case</returns>
        /// <category>Cases</category>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"case")]
        public CasesWrapper CreateCases(
                                      String title,
                                      IEnumerable<int> members,
                                      IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                      bool isPrivate,
                                      IEnumerable<Guid> accessList)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            int casesID = DaoFactory.GetCasesDao().CreateCases(title);

            var cases = new Cases
            {
                ID = casesID,
                Title = title
            };

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(cases, accessListLocal);
            else
                CRMSecurity.MakePublic(cases);

            if (members != null && members.Count() > 0)
                DaoFactory.GetDealDao().SetMembers(cases.ID, members.ToArray());

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Case, cases.ID, field.Key, field.Value);
            }

            return ToCasesWrapper(DaoFactory.GetCasesDao().GetByID(casesID));
        }


        /// <summary>
        ///   Updates the selected case with the parameters specified in the request
        /// </summary>
        /// <short>Update case</short> 
        /// <category>Cases</category>
        /// <param name="caseid">Case ID</param>
        /// <param name="title">Case title</param>
        /// <param optional="true" name="members">Participants</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="isPrivate">Case privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access to the case</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"case/{caseid:[0-9]+}")]
        public CasesWrapper UpdateCases(int caseid,
                                        String title,
                                        IEnumerable<int> members,
                                        IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                        bool isPrivate,
                                        IEnumerable<Guid> accessList)
        {

            if ((caseid <= 0) || (String.IsNullOrEmpty(title)))
                throw new ArgumentException();

            var cases = DaoFactory.GetCasesDao().GetByID(caseid);

            if (cases == null)
                throw new ItemNotFoundException();

            cases.Title = title;

            DaoFactory.GetCasesDao().UpdateCases(cases);

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(cases, accessListLocal);
            else
                CRMSecurity.MakePublic(cases);

            if (members != null && members.Count() > 0)
                DaoFactory.GetDealDao().SetMembers(cases.ID, members.ToArray());

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Case, cases.ID, field.Key, field.Value);
            }

            return ToCasesWrapper(cases);

        }

        /// <summary>
        ///   Sets access rights for the selected case with the parameters specified in the request
        /// </summary>
        /// <param name="casesid">Case ID</param>
        /// <param name="isPrivate">Case privacy: private or not</param>
        /// <param name="accessList">List of users with access to the case</param>
        /// <short>Set rights to case</short> 
        /// <category>Cases</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Мероприятие 
        /// </returns>
        [Update("case/{caseid:[0-9]+}/access")]
        public CasesWrapper SetAccessToCases(
            int casesid,
            bool isPrivate,
            IEnumerable<Guid> accessList)
        {

            if (casesid <= 0)
                throw new ArgumentException();

            var cases = DaoFactory.GetCasesDao().GetByID(casesid);

            if (cases == null)
                throw new ItemNotFoundException();

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(cases, accessListLocal);
            else
                CRMSecurity.MakePublic(cases);

            return ToCasesWrapper(cases);
        }

        /// <summary>
        ///   Sets access rights for other users to the list of cases with the IDs specified in the request
        /// </summary>
        /// <param name="casesid">Case ID list</param>
        /// <param name="isPrivate">Case privacy: private or not</param>
        /// <param name="accessList">List of users with access</param>
        /// <short>Set case access rights</short> 
        /// <category>Cases</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Case list
        /// </returns>
        [Update("case/access")]
        public IEnumerable<CasesWrapper> SetAccessToBatchCases(
            IEnumerable<int> casesid,
            bool isPrivate,
            IEnumerable<Guid> accessList
            )
        {

            var result = new List<CasesWrapper>();

            foreach (var id in casesid)
            {
                try
                {
                    result.Add(SetAccessToCases(id, isPrivate, accessList));

                }
                catch (Exception)
                {

                }
            }

            return result;
        }

        /// <summary>
        ///    Returns the detailed information about the case with the ID specified in the request
        /// </summary>
        /// <short>Get case by ID</short> 
        /// <category>Cases</category>
        /// <param name="caseid">Case ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"case/{caseid:[0-9]+}")]
        public CasesWrapper GetCaseByID(int caseid)
        {
            if (caseid <= 0)
                throw new ItemNotFoundException();

            var cases = DaoFactory.GetCasesDao().GetByID(caseid);

            if (cases == null)
                throw new ItemNotFoundException();

            return ToCasesWrapper(cases);

        }

        /// <summary>
        ///     Returns the list of all cases matching the parameters specified in the request
        /// </summary>
        /// <short>Get case list</short> 
        /// <param optional="true" name="contactid">Contact ID</param>
        /// <param optional="true" name="isClosed">Case status</param>
        /// <param optional="true" name="tags">Tags</param>
        /// <category>Cases</category>
        /// <returns>
        ///    Case list
        /// </returns>
        [Read(@"case/filter")]
        public IEnumerable<CasesWrapper> GetCases(int contactid, bool? isClosed, IEnumerable<String> tags)
        {
            IEnumerable<CasesWrapper> result;

            SortedByType sortBy;

            OrderBy casesOrderBy;

            var searchString = _context.FilterValue;


            if (EnumExtension.TryParse(_context.SortBy, true, out sortBy))
                casesOrderBy = new OrderBy(sortBy, !_context.SortDescending);
            else if (String.IsNullOrEmpty(_context.SortBy))
                casesOrderBy = new OrderBy(SortedByType.Title, true);
            else
                casesOrderBy = null;

            if (casesOrderBy != null)
            {

                result = ToListCasesWrappers(DaoFactory.GetCasesDao().GetCases(searchString, contactid, isClosed, tags, (int)_context.StartIndex,
                                                          (int)_context.Count,
                                                          casesOrderBy));





                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();

            }
            else
                result =
                    ToListCasesWrappers(DaoFactory.GetCasesDao().GetCases(searchString, contactid, isClosed,
                                                                          tags,
                                                                          0,
                                                                          0,
                                                                          null));

            var totalCount = DaoFactory.GetCasesDao().GetCasesCount(searchString,
                                                                          contactid,
                                                                          isClosed,
                                                                          tags);

            _context.SetTotalCount(totalCount);


            return result.ToSmartList();

        }

        /// <summary>
        ///   Deletes the case with the ID specified in the request
        /// </summary>
        /// <short>Delete case</short> 
        /// <param name="caseid">Case ID</param>
        /// <category>Cases</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Case
        /// </returns>
        [Delete(@"case/{caseid:[0-9]+}")]
        public CasesWrapper DeleteCase(int caseid)
        {
            if (caseid <= 0)
                throw new ArgumentException();

            var kase = DaoFactory.GetCasesDao().GetByID(caseid);

            if (kase == null)
                throw new ItemNotFoundException();

            var kaseWrapper = ToCasesWrapper(kase);

            DaoFactory.GetCasesDao().DeleteCases(caseid);

            return kaseWrapper;
        }


        /// <summary>
        ///   Deletes the group of cases with the IDs specified in the request
        /// </summary>
        /// <param name="casesid">Case ID list</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Delete case group</short> 
        /// <category>Cases</category>
        /// <returns>
        ///   Case list
        /// </returns>
        [Delete(@"case")]
        public IEnumerable<CasesWrapper> DeleteBatchCases(IEnumerable<int> casesid)
        {
            if (casesid == null)
                throw new ArgumentException();

            var result = DaoFactory.GetCasesDao().GetCases(casesid.ToArray())
                .Select(item => ToCasesWrapper(item));

            DaoFactory.GetCasesDao().DeleteBatchCases(casesid.ToArray());

            return result;
        }




        /// <summary>
        ///    Returns the list of all contacts associated with the case with the ID specified in the request
        /// </summary>
        /// <short>Get all case contacts</short> 
        /// <param name="caseid">Case ID</param>
        /// <category>Cases</category>
        /// <returns>Contact list</returns>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"case/{caseid:[0-9]+}/contact")]
        public IEnumerable<ContactWrapper> GetCasesMembers(int caseid)
        {
            var contactIDs = DaoFactory.GetCasesDao().GetMembers(caseid);

            if (contactIDs == null)
                return new ItemList<ContactWrapper>();

            return ToListContactWrapper(DaoFactory.GetContactDao().GetContacts(contactIDs));

        }

        /// <summary>
        ///   Adds the selected contact to the case with the ID specified in the request
        /// </summary>
        /// <short>Add case contact</short> 
        /// <category>Cases</category>
        /// <param name="caseid">Case ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Participant
        /// </returns>
        [Create(@"case/{caseid:[0-9]+}/contact")]
        public ContactWrapper AddMemberToCases(int caseid, int contactid)
        {
            if ((caseid <= 0) || (contactid <= 0))
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            var result = ToContactWrapper(contact);

            DaoFactory.GetCasesDao().AddMember(caseid, contactid);

            return result;
        }

        /// <summary>
        ///   Delete the selected contact from the case with the ID specified in the request
        /// </summary>
        /// <short>Delete case contact</short> 
        /// <category>Cases</category>
        /// <param name="caseid">Case ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Participant
        /// </returns>
        [Delete(@"case/{caseid:[0-9]+}/contact/{contactid:[0-9]+}")]
        public ContactWrapper DeleteMemberFromCases(int caseid, int contactid)
        {
            if ((caseid <= 0) || (contactid <= 0))
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            var result = ToContactWrapper(contact);

            DaoFactory.GetCasesDao().RemoveMember(caseid, contactid);

            return result;
        }

        private IEnumerable<CasesWrapper> ToListCasesWrappers(ICollection<Cases> items)
        {

            if (items == null || items.Count == 0) return new List<CasesWrapper>();

            var result = new List<CasesWrapper>();


            var contactIDs = new List<int>();

            var casesIDs = items.Select(item => item.ID).ToArray();

            var customFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Case, casesIDs)
                             .GroupBy(item => item.EntityID)
                             .ToDictionary(item => item.Key, item => item.Select(x => ToCustomFieldWrapper(x)));

            var casesMembers = DaoFactory.GetCasesDao().GetMembers(casesIDs);

            foreach (var value in casesMembers.Values)
                contactIDs.AddRange(value);

            var contacts = DaoFactory.GetContactDao().GetContacts(contactIDs.Distinct().ToArray())
                          .ToDictionary(item => item.ID, item => ToContactBaseWrapper(item));

            foreach (var cases in items)
            {

                var casesWrapper = new CasesWrapper(cases);

                if (customFields.ContainsKey(cases.ID))
                    casesWrapper.CustomFields = customFields[cases.ID];
                else
                    casesWrapper.CustomFields = new List<CustomFieldWrapper>();

                if (casesMembers.ContainsKey(cases.ID))
                    casesWrapper.Members = casesMembers[cases.ID].Where(contacts.ContainsKey).Select(item => contacts[item]);
                else
                    casesWrapper.Members = new List<ContactBaseWrapper>();

                result.Add(casesWrapper);

            }

            return result;
        }

        private CasesWrapper ToCasesWrapper(Cases cases)
        {
            var casesWrapper = new CasesWrapper(cases);

            casesWrapper.CustomFields = DaoFactory.GetCustomFieldDao()
                .GetEnityFields(EntityType.Case, cases.ID, false)
                .ConvertAll(item => new CustomFieldWrapper(item)).ToSmartList();

            casesWrapper.Members = new List<ContactBaseWrapper>();

            var memberIDs = DaoFactory.GetCasesDao().GetMembers(cases.ID);

            var membersList = new List<ContactBaseWrapper>();

            foreach (var contactID in memberIDs)
            {
                if (contactID == 0) continue;

                var contact = DaoFactory.GetContactDao().GetByID(contactID);

                if (contact == null) continue;

                membersList.Add(ToContactBaseWrapper(contact));
            }

            casesWrapper.Members = membersList;


            return casesWrapper;
        }

        private Cases FromCasesWrapper(CasesWrapper casesWrapper)
        {
            return new Cases
                       {
                           Title = casesWrapper.Title,
                           IsClosed = casesWrapper.IsClosed,
                           CreateOn = casesWrapper.Created,
                           CreateBy = casesWrapper.CreateBy.Id

                       };

        }
    }
}
