#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASC.Common.Threading.Workers;
using ASC.Common.Utils;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Data.Storage;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Studio.Utility;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;
using Constants = ASC.Core.Users.Constants;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Web.Files.Classes;

#endregion

namespace ASC.Web.CRM.Classes
{

    class ExportDataOperation : IProgressItem
    {
        #region Constructor

        public ExportDataOperation(ICollection externalData)
        {
            _authorID = ASC.Core.SecurityContext.CurrentAccount.ID;
            _dataStore = Global.GetStore();
            _tenantID = TenantProvider.CurrentTenantID;
            _daoFactory = Global.DaoFactory;
            _notifyClient = NotifyClient.Instance;
            _log = LogManager.GetLogger("ASC.CRM");
            Id = TenantProvider.CurrentTenantID;
            _externalData = externalData;

        }


        public ExportDataOperation()
            : this(null)
        {

        }

        #endregion

        #region Members

        private readonly ILog _log;

        private readonly IDataStore _dataStore;

        private readonly Guid _authorID;

        private readonly int _tenantID;

        private readonly DaoFactory _daoFactory;

        private readonly NotifyClient _notifyClient;

        private readonly ICollection _externalData;

        private double _totalCount;

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ExportDataOperation)) return false;
            if (_tenantID == ((ExportDataOperation)obj)._tenantID) return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _tenantID.GetHashCode();
        }

        public object Clone()
        {
            var cloneObj = new ExportDataOperation();

            cloneObj.Error = Error;
            cloneObj.Id = Id;
            cloneObj.IsCompleted = IsCompleted;
            cloneObj.Percentage = Percentage;
            cloneObj.Status = Status;

            return cloneObj;
        }

        #region Property

        public object Id { get; set; }

        public object Status { get; set; }

        public object Error { get; set; }

        public double Percentage { get; set; }

        public bool IsCompleted { get; set; }

        #endregion

        #region Private Methods

        private String WrapDoubleQuote(String value)
        {
            return "\"" + value.Trim().Replace("\"", "\"\"").Replace(Environment.NewLine, "") + "\"";
        }

        private String DataTableToCSV(DataTable dataTable)
        {
            var result = new StringBuilder();

            var columnsCount = dataTable.Columns.Count;

            for (int index = 0; index < columnsCount; index++)
            {

                if (index != columnsCount - 1)
                    result.Append(dataTable.Columns[index].Caption + ",");
                else
                    result.Append(dataTable.Columns[index].Caption);

            }

            result.Append(Environment.NewLine);

            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < columnsCount; i++)
                {

                    var itemValue = WrapDoubleQuote(row[i].ToString());

                    if (i != columnsCount - 1)
                        result.Append(itemValue + ",");
                    else
                        result.Append(itemValue);
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        #endregion

        public void RunJob()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(_tenantID);

            var userCulture = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture();

            System.Threading.Thread.CurrentThread.CurrentCulture = userCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = userCulture;

            _log.Debug("Start Export Data");

            if (_externalData == null)
                ExportAllData();
            else
                ExportPartData();

        }

        private void Complete()
        {
            IsCompleted = true;

            Percentage = 100;

            _log.Debug("Export is completed");

        }

        private void ExportAllData()
        {
            var stream = TempStream.Create();

            using (var contactDao = _daoFactory.GetContactDao())
            using (var contactInfoDao = _daoFactory.GetContactInfoDao())
            using (var dealDao = _daoFactory.GetDealDao())
            using (var casesDao = _daoFactory.GetCasesDao())
            using (var taskDao = _daoFactory.GetTaskDao())
            using (var historyDao = _daoFactory.GetRelationshipEventDao())
            {

                _totalCount += contactDao.GetAllContactsCount();
                _totalCount += dealDao.GetDealsCount();
                _totalCount += casesDao.GetCasesCount();
                _totalCount += taskDao.GetAllTasksCount();
                _totalCount += historyDao.GetAllItemsCount();

                using (var zipStream = new ZipOutputStream(stream))
                {

                    zipStream.UseZip64 = UseZip64.Off;

                    var zipEntry = new ZipEntry("contacts.csv") { DateTime = DateTime.UtcNow, IsUnicodeText = true };
                    zipStream.PutNextEntry(zipEntry);

                    var contactData = contactDao.GetAllContacts();

                    var contactInfos = new StringDictionary();

                    contactInfoDao.GetAll().ForEach(item =>
                    {
                        var contactInfoKey = String.Format("{0}_{1}_{2}", item.ContactID,
                                                           (int)item.InfoType,
                                                           item.Category);

                        if (contactInfos.ContainsKey(contactInfoKey))
                            contactInfos[contactInfoKey] += "," + item.Data;
                        else
                            contactInfos.Add(contactInfoKey, item.Data);
                    });

                    var zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportContactsToCSV(contactData, contactInfos)));

                    StreamUtils.Copy(zipEntryData, zipStream, new byte[4096]);

                    zipStream.CloseEntry();

                    zipEntry = new ZipEntry("deals.csv") { DateTime = DateTime.UtcNow, IsUnicodeText = true };

                    zipStream.PutNextEntry(zipEntry);

                    var dealData = dealDao.GetAllDeals();
                    zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportDealsToCSV(dealData)));

                    StreamUtils.Copy(zipEntryData, zipStream, new byte[4096]);

                    zipStream.CloseEntry();

                    zipEntry = new ZipEntry("cases.csv") { DateTime = DateTime.UtcNow, IsUnicodeText = true };

                    zipStream.PutNextEntry(zipEntry);

                    var casesData = casesDao.GetAllCases();

                    zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportCasesToCSV(casesData)));

                    StreamUtils.Copy(zipEntryData, zipStream, new byte[4096]);

                    zipStream.CloseEntry();

                    zipEntry = new ZipEntry("tasks.csv") { DateTime = DateTime.UtcNow, IsUnicodeText = true };

                    zipStream.PutNextEntry(zipEntry);

                    var taskData = taskDao.GetAllTasks();

                    zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportTasksToCSV(taskData)));

                    StreamUtils.Copy(zipEntryData, zipStream, new byte[4096]);

                    zipStream.CloseEntry();

                    zipEntry = new ZipEntry("history.csv") { DateTime = DateTime.UtcNow, IsUnicodeText = true };

                    zipStream.PutNextEntry(zipEntry);

                    var historyData = historyDao.GetAllItems();

                    zipEntryData = new MemoryStream(Encoding.UTF8.GetBytes(ExportHistoryToCSV(historyData)));

                    StreamUtils.Copy(zipEntryData, zipStream, new byte[4096]);

                    zipStream.CloseEntry();

                    zipStream.IsStreamOwner = false;

                    zipStream.Finish();
                    zipStream.Close();

                    stream.Position = 0;

                }
            }

            var assignedURI = _dataStore.SavePrivate(String.Empty, "exportdata.zip", stream, DateTime.Now.AddDays(1));

            Status = assignedURI;

            _notifyClient.SendAboutExportCompleted(_authorID, assignedURI);

            Complete();
        }

        private void ExportPartData()
        {
            _totalCount = _externalData.Count;

            if (_totalCount == 0)
                throw new ArgumentException("export data is null");

            if (_externalData is List<Contact>)
            {

                using (var contactInfoDao = _daoFactory.GetContactInfoDao())
                {

                    var contacts = (List<Contact>)_externalData;

                    var contactInfos = new StringDictionary();

                    contactInfoDao.GetAll(contacts.Select(item => item.ID).ToArray()).ForEach(item =>
                    {
                        var contactInfoKey = String.Format("{0}_{1}_{2}", item.ContactID,
                                                           (int)item.InfoType,
                                                           item.Category);

                        if (contactInfos.ContainsKey(contactInfoKey))
                            contactInfos[contactInfoKey] += "," + item.Data;
                        else
                            contactInfos.Add(contactInfoKey, item.Data);
                    });

                    Status = ExportContactsToCSV(contacts, contactInfos);

                }

            }
            else if (_externalData is List<Deal>)
                Status = ExportDealsToCSV((List<Deal>)_externalData);
            else if (_externalData is List<ASC.CRM.Core.Entities.Cases>)
                Status = ExportCasesToCSV((List<ASC.CRM.Core.Entities.Cases>)_externalData);
            else if (_externalData is List<RelationshipEvent>)
                Status = ExportHistoryToCSV((List<RelationshipEvent>)_externalData);
            else if (_externalData is List<Task>)
                Status = ExportTasksToCSV((List<Task>)_externalData);

            else
                throw new ArgumentException();

            Complete();

        }

        private String ExportContactsToCSV(IEnumerable<Contact> contacts, StringDictionary contactInfos)
        {
            using (var listItemDao = _daoFactory.GetListItemDao())
            using (var tagDao = _daoFactory.GetTagDao())
            using (var customFieldDao = _daoFactory.GetCustomFieldDao())
            using (var contactDao = _daoFactory.GetContactDao())
            {

                var dataTable = new DataTable();

                dataTable.Columns.AddRange(new[]{
                                               new DataColumn
                                                   {
                                                      Caption  = CRMContactResource.ContactType,
                                                      ColumnName = "contact_type"
                                                   }, 
                                               new DataColumn
                                                   {
                                                      Caption  = CRMContactResource.FirstName,
                                                      ColumnName = "firstname"
                                                   },
                                               new DataColumn
                                                   {
                                                      Caption  = CRMContactResource.LastName,
                                                      ColumnName = "lastname"
                                                   },
                                               new DataColumn
                                                   {
                                                      Caption  = CRMContactResource.CompanyName,
                                                      ColumnName = "companyname"
                                                   },
                                               new DataColumn
                                                   {
                                                      Caption  = CRMContactResource.JobTitle,
                                                      ColumnName = "jobtitle" 
                                                   },
                                               new DataColumn
                                                   {
                                                      Caption  = CRMContactResource.About,
                                                      ColumnName = "about" 
                                                   },
                                               new DataColumn
                                               {
                                                      Caption  = CRMContactResource.ContactStage,
                                                      ColumnName = "contact_stage" 
                                               },
                                               new DataColumn
                                               {
                                                      Caption  = CRMContactResource.ContactTagList,
                                                      ColumnName = "contact_tag_list" 
                                               }
                                           });

                foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
                    foreach (Enum categoryEnum in Enum.GetValues(ContactInfo.GetCategory(infoTypeEnum)))
                    {
                        var localTitle = String.Format("{1} ({0})", categoryEnum.ToLocalizedString().ToLower(), infoTypeEnum.ToLocalizedString());

                        if (infoTypeEnum == ContactInfoType.Address)
                            dataTable.Columns.AddRange((from AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart))
                                                        select new DataColumn
                                                        {
                                                            Caption = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString().ToLower()),
                                                            ColumnName = String.Format("contactInfo_{0}_{1}_{2}", (int)infoTypeEnum, categoryEnum, (int)addressPartEnum)
                                                        }).ToArray());

                        else
                            dataTable.Columns.Add(new DataColumn
                            {
                                Caption = localTitle,
                                ColumnName = String.Format("contactInfo_{0}_{1}", (int)infoTypeEnum, categoryEnum)
                            });
                    }


                customFieldDao.GetFieldsDescription(EntityType.Contact).ForEach(
                item =>
                {
                    if (item.FieldType == CustomFieldType.Heading) return;


                    dataTable.Columns.Add(
                        new DataColumn
                        {
                            Caption = item.Label,
                            ColumnName = "customField_" + item.ID
                        }
                    );
                }
                );

                var customFieldEntity = new Dictionary<int, List<CustomField>>();

                customFieldDao.GetEnityFields(EntityType.Contact, 0, false).ForEach(
                    item =>
                    {

                        if (!customFieldEntity.ContainsKey(item.EntityID))
                            customFieldEntity.Add(item.EntityID, new List<CustomField>
                                                                         {
                                                                             item
                                                                         });
                        else
                            customFieldEntity[item.EntityID].Add(item);

                    }
                    );


                var tags = tagDao.GetEntitiesTags(EntityType.Contact);

                foreach (var contact in contacts)
                {

                    Percentage += 1.0 * 100 / _totalCount;

                    var isCompany = contact is Company;

                    var contactType = (isCompany) ? CRMContactResource.Company : CRMContactResource.Contact;

                    String contactTags = String.Empty;

                    if (tags.ContainsKey(contact.ID))
                        contactTags = String.Join(",", tags[contact.ID].ToArray());

                    String firstName;
                    String lastName;

                    String companyName;
                    String title;

                    if (contact is Company)
                    {
                        firstName = String.Empty;
                        lastName = String.Empty;
                        title = String.Empty;
                        companyName = ((Company)contact).CompanyName;

                    }
                    else
                    {
                        var people = (Person)contact;

                        firstName = people.FirstName;
                        lastName = people.LastName;
                        title = people.JobTitle;

                        companyName = String.Empty;

                        if (people.CompanyID > 0)
                        {

                            var personCompany = contacts.SingleOrDefault(item => item.ID == people.CompanyID);

                            if (personCompany == null)
                                personCompany = contactDao.GetByID(people.CompanyID);

                            if (personCompany != null)
                                companyName = personCompany.GetTitle();

                        }

                    }

                    var contactStatus = String.Empty;

                    if (contact.StatusID > 0)
                    {

                        var listItem = listItemDao.GetByID(contact.StatusID);

                        if (listItem != null)
                            contactStatus = listItem.Title;

                    }

                    var dataRowItems = new List<String>
                                           {
                                               contactType,
                                               firstName,
                                               lastName,
                                               companyName, 
                                               title,
                                               contact.About,
                                               contactStatus,
                                               contactTags
                                           };

                    foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
                        foreach (Enum categoryEnum in Enum.GetValues(ContactInfo.GetCategory(infoTypeEnum)))
                        {
                            var contactInfoKey = String.Format("{0}_{1}_{2}", contact.ID,
                                                                                  (int)infoTypeEnum,
                                                                                  Convert.ToInt32(categoryEnum));

                            var columnValue = "";

                            if (contactInfos.ContainsKey(contactInfoKey))
                                columnValue = contactInfos[contactInfoKey];

                            if (infoTypeEnum == ContactInfoType.Address)
                            {
                                if (!String.IsNullOrEmpty(columnValue))
                                {

                                    var addresses = JArray.Parse(String.Concat("[", columnValue, "]"));

                                    dataRowItems.AddRange((from AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart))
                                                           select String.Join(",", addresses.Select(item => (String)item.SelectToken(addressPartEnum.ToString().ToLower())).ToArray())).ToArray());
                                }
                                else
                                {
                                    dataRowItems.AddRange(new[] { "", "", "", "", "" });

                                }
                            }
                            else
                            {
                                dataRowItems.Add(columnValue);
                            }

                        }


                    var dataRow = dataTable.Rows.Add(dataRowItems.ToArray());

                    if (customFieldEntity.ContainsKey(contact.ID))
                        customFieldEntity[contact.ID].ForEach(item =>
                                                           dataRow["customField_" + item.ID] = item.Value
                            );
                }

                return DataTableToCSV(dataTable);
            }
        }

        private String ExportDealsToCSV(IEnumerable<Deal> deals)
        {

            using (var tagDao = _daoFactory.GetTagDao())
            using (var customFieldDao = _daoFactory.GetCustomFieldDao())
            using (var dealMilestoneDao = _daoFactory.GetDealMilestoneDao())
            using (var contactDao = _daoFactory.GetContactDao())
            {

                var dataTable = new DataTable();

                dataTable.Columns.AddRange(new[]
                                               {
                                                   new DataColumn
                                                       {
                                                           Caption =CRMDealResource.NameDeal,
                                                           ColumnName = "title" 
                                                       },
                                                   new DataColumn
                                                   {
                                                           Caption =CRMDealResource.ClientDeal,
                                                           ColumnName = "client_deal" 
                                                   },
                                                   new DataColumn
                                                    {
                                                           Caption =CRMDealResource.DescriptionDeal,
                                                           ColumnName = "description" 
                                                       },
                                                   new DataColumn
                                                    {
                                                           Caption =CRMCommonResource.Currency,
                                                           ColumnName = "currency" 
                                                       },
                                                   new DataColumn
                                                    {
                                                           Caption =CRMDealResource.DealAmount,
                                                           ColumnName = "amount" 
                                                       },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMDealResource.BidType,
                                                           ColumnName = "bid_type" 
                                                       },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMDealResource.BidTypePeriod,
                                                           ColumnName = "bid_type_period" 
                                                       },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMJSResource.ExpectedCloseDate,
                                                           ColumnName = "expected_close_date" 
                                                    },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMJSResource.ActualCloseDate,
                                                           ColumnName = "actual_close_date" 
                                                    },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMDealResource.ResponsibleDeal,
                                                           ColumnName = "responsible_deal" 
                                                    },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMDealResource.CurrentDealMilestone,
                                                           ColumnName = "current_deal_milestone" 
                                                    },
                                                   new DataColumn
                                                    {
                                                           Caption = CRMDealResource.DealMilestoneType,
                                                           ColumnName = "deal_milestone_type" 
                                                    },
                                                   new DataColumn
                                                    {
                                                           Caption = (CRMDealResource.ProbabilityOfWinning + " %"),
                                                           ColumnName = "probability_of_winning" 
                                                    },
                                                   new DataColumn
                                                    {
                                                           Caption = (CRMDealResource.DealTagList),
                                                           ColumnName = "tag_list" 
                                                    }
                                               });

                customFieldDao.GetFieldsDescription(EntityType.Opportunity).ForEach(
                    item =>
                    {
                        if (item.FieldType == CustomFieldType.Heading) return;

                        dataTable.Columns.Add(new DataColumn
                        {
                            Caption = item.Label,
                            ColumnName = "customField_" + item.ID
                        });
                    }
                    );

                var customFieldEntity = new Dictionary<int, List<CustomField>>();

                customFieldDao.GetEnityFields(EntityType.Opportunity, 0, false).ForEach(
                    item =>
                    {
                        if (!customFieldEntity.ContainsKey(item.EntityID))
                            customFieldEntity.Add(item.EntityID, new List<CustomField>
                                                                         {
                                                                             item
                                                                         });
                        else
                            customFieldEntity[item.EntityID].Add(item);

                    }
                    );

                var tags = tagDao.GetEntitiesTags(EntityType.Opportunity);

                foreach (var deal in deals)
                {

                    Percentage += 1.0 * 100 / _totalCount;

                    String contactTags = String.Empty;

                    if (tags.ContainsKey(deal.ID))
                        contactTags = String.Join(",", tags[deal.ID].ToArray());

                    String bidType;

                    switch (deal.BidType)
                    {
                        case BidType.FixedBid:
                            bidType = CRMDealResource.BidType_FixedBid;
                            break;
                        case BidType.PerDay:
                            bidType = CRMDealResource.BidType_PerDay;
                            break;
                        case BidType.PerHour:
                            bidType = CRMDealResource.BidType_PerHour;
                            break;
                        case BidType.PerMonth:
                            bidType = CRMDealResource.BidType_PerMonth;
                            break;
                        case BidType.PerWeek:
                            bidType = CRMDealResource.BidType_PerWeek;
                            break;
                        case BidType.PerYear:
                            bidType = CRMDealResource.BidType_PerYear;
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    var currentDealMilestone = dealMilestoneDao.GetByID(deal.DealMilestoneID);
                    var currentDealMilestoneStatus = currentDealMilestone.Status.ToLocalizedString();
                    String contactTitle = String.Empty;

                    if (deal.ContactID != 0)
                        contactTitle = contactDao.GetByID(deal.ContactID).GetTitle();

                    var dataRow = dataTable.Rows.Add(new[]
                                                         {
                                                             deal.Title,
                                                             contactTitle,
                                                             deal.Description,
                                                             deal.BidCurrency,
                                                             deal.BidValue.ToString(),
                                                             bidType,
                                                             deal.PerPeriodValue == 0 ? "" : deal.PerPeriodValue.ToString(),
                                                             deal.ExpectedCloseDate.Date == DateTime.MinValue.Date ? "" : deal.ExpectedCloseDate.ToString(DateTimeExtension.DateFormatPattern),
                                                             deal.ActualCloseDate.Date == DateTime.MinValue.Date ? "" : deal.ActualCloseDate.ToString(DateTimeExtension.DateFormatPattern),
                                                             ASC.Core.CoreContext.UserManager.GetUsers(deal.ResponsibleID).DisplayUserName(),
                                                             currentDealMilestone.Title,
                                                             currentDealMilestoneStatus,
                                                             deal.DealMilestoneProbability.ToString(),
                                                             contactTags
                                                         });

                    if (customFieldEntity.ContainsKey(deal.ID))
                        customFieldEntity[deal.ID].ForEach(item =>
                                                           dataRow["customField_" + item.ID] = item.Value
                            );
                }


                return DataTableToCSV(dataTable);
            }


        }

        private String ExportCasesToCSV(IEnumerable<ASC.CRM.Core.Entities.Cases> cases)
        {
            using (var tagDao = _daoFactory.GetTagDao())
            using (var customFieldDao = _daoFactory.GetCustomFieldDao())
            {


                var dataTable = new DataTable();

                dataTable.Columns.AddRange(new[]
                                               {
                                                   new DataColumn
                                                       {   
                                                           Caption = CRMCasesResource.CaseTitle,
                                                           ColumnName = "title"
                                                       },
                                                   new DataColumn(CRMCasesResource.CasesTagList)
                                                   {   
                                                           Caption = CRMCasesResource.CasesTagList,
                                                           ColumnName = "tag_list"
                                                    }
                                               });

                customFieldDao.GetFieldsDescription(EntityType.Case).ForEach(
                    item =>
                    {
                        if (item.FieldType == CustomFieldType.Heading) return;

                        dataTable.Columns.Add(new DataColumn
                        {
                            Caption = item.Label,
                            ColumnName = "customField_" + item.ID
                        });
                    }
                    );

                var customFieldEntity = new Dictionary<int, List<CustomField>>();

                customFieldDao.GetEnityFields(EntityType.Case, 0, false).ForEach(
                    item =>
                    {
                        if (!customFieldEntity.ContainsKey(item.EntityID))
                            customFieldEntity.Add(item.EntityID, new List<CustomField>
                                                                         {
                                                                             item
                                                                         });
                        else
                            customFieldEntity[item.EntityID].Add(item);

                    }
                    );

                var tags = tagDao.GetEntitiesTags(EntityType.Case);

                foreach (var item in cases)
                {
                    Percentage += 1.0 * 100 / _totalCount;

                    String contactTags = String.Empty;

                    if (tags.ContainsKey(item.ID))
                        contactTags = String.Join(",", tags[item.ID].ToArray());

                    var dataRow = dataTable.Rows.Add(new[]
                                                         {
                                                             item.Title,
                                                             contactTags
                                                         });

                    if (customFieldEntity.ContainsKey(item.ID))
                        customFieldEntity[item.ID].ForEach(row =>
                                                          dataRow["customField_" + row.ID] = row.Value
                            );
                }

                return DataTableToCSV(dataTable);
            }
        }

        private String ExportHistoryToCSV(IEnumerable<RelationshipEvent> events)
        {
            using (var listItemDao = _daoFactory.GetListItemDao())
            using (var eventsDao = _daoFactory.GetRelationshipEventDao())
            using (var dealDao = _daoFactory.GetDealDao())
            using (var casesDao = _daoFactory.GetCasesDao())
            using (var contactDao = _daoFactory.GetContactDao())
            {

                var dataTable = new DataTable();

                dataTable.Columns.AddRange(new[]
                                               {
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMContactResource.Content),
                                                           ColumnName = "content"
                                                       },
                                                   new DataColumn
                                                       {
                                                            Caption = (CRMCommonResource.Category),
                                                           ColumnName = "category"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMContactResource.ContactTitle),
                                                           ColumnName = "contact_title"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMContactResource.RelativeEntity),
                                                           ColumnName = "relative_entity"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMCommonResource.Author),
                                                           ColumnName = "author"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMCommonResource.CreateDate),
                                                           ColumnName = "create_date"
                                                       }
                                               });

                foreach (var item in events)
                {
                    Percentage += 1.0 * 100 / _totalCount;

                    String entityTitle = String.Empty;

                    if (item.EntityID > 0)
                        switch (item.EntityType)
                        {
                            case EntityType.Case:

                                var casesObj = casesDao.GetByID(item.EntityID);

                                if (casesObj != null)
                                    entityTitle = String.Format("{0}: {1}", CRMCasesResource.Case,
                                                               casesObj.Title);
                                break;
                            case EntityType.Opportunity:
                                var dealObj = dealDao.GetByID(item.EntityID);

                                if (dealObj != null)
                                    entityTitle = String.Format("{0}: {1}", CRMDealResource.Deal,
                                                                dealObj.Title);
                                break;
                            default:
                                break;
                        }

                    String contactTitle = String.Empty;


                    if (item.ContactID > 0)
                    {

                        var contactObj = contactDao.GetByID(item.ContactID);

                        if (contactObj != null)
                            contactTitle = contactObj.GetTitle();


                    }

                    String categoryTitle = String.Empty;

                    if (item.CategoryID > 0)
                    {
                        var categoryObj = listItemDao.GetByID(item.CategoryID);

                        if (categoryObj != null)
                            categoryTitle = categoryObj.Title;

                    }
                    else if (item.CategoryID == (int)HistoryCategorySystem.TaskClosed)
                        categoryTitle = HistoryCategorySystem.TaskClosed.ToLocalizedString();
                    else if (item.CategoryID == (int)HistoryCategorySystem.FilesUpload)
                        categoryTitle = HistoryCategorySystem.FilesUpload.ToLocalizedString();

                    dataTable.Rows.Add(new[]
                                           {
                                               item.Content,
                                               categoryTitle,
                                               contactTitle,  
                                               entityTitle,
                                               ASC.Core.CoreContext.UserManager.GetUsers(item.CreateBy).DisplayUserName(),
                                               item.CreateOn.ToString(DateTimeExtension.DateFormatPattern)
                                           });


                }

                return DataTableToCSV(dataTable);
            }

        }

        private String ExportTasksToCSV(IEnumerable<Task> tasks)
        {

            using (var listItemDao = _daoFactory.GetListItemDao())
            using (var dealDao = _daoFactory.GetDealDao())
            using (var casesDao = _daoFactory.GetCasesDao())
            using (var contactDao = _daoFactory.GetContactDao())
            {
                var dataTable = new DataTable();

                dataTable.Columns.AddRange(new[]
                                               {
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMTaskResource.TaskTitle),
                                                           ColumnName = "title"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMTaskResource.Description),
                                                           ColumnName = "description"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMTaskResource.DueDate),
                                                           ColumnName = "due_date"
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMTaskResource.Responsible),
                                                           ColumnName = "responsible" 
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMContactResource.ContactTitle),
                                                           ColumnName = "contact_title" 
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMTaskResource.TaskStatus),
                                                           ColumnName = "task_status" 
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMTaskResource.TaskCategory),
                                                           ColumnName = "task_category"  
                                                       },
                                                   new DataColumn
                                                       {
                                                           Caption = (CRMContactResource.RelativeEntity),
                                                           ColumnName = "relative_entity"  
                                                       }
                                               });


                foreach (var item in tasks)
                {

                    Percentage += 1.0 * 100 / _totalCount;

                    String entityTitle = String.Empty;

                    if (item.EntityID > 0)
                        switch (item.EntityType)
                        {
                            case EntityType.Case:
                                var caseObj = casesDao.GetByID(item.EntityID);

                                if (caseObj != null)
                                    entityTitle = String.Format("{0}: {1}", CRMCasesResource.Case, caseObj.Title);
                                break;
                            case EntityType.Opportunity:
                                var dealObj = dealDao.GetByID(item.EntityID);

                                if (dealObj != null)
                                    entityTitle = String.Format("{0}: {1}", CRMDealResource.Deal, dealObj.Title);
                                break;
                            default:
                                break;
                        }

                    var contactTitle = String.Empty;

                    if (item.ContactID > 0)
                    {
                        var contact = contactDao.GetByID(item.ContactID);

                        if (contact != null)
                            contactTitle = contact.GetTitle();
                    }


                    dataTable.Rows.Add(new[]
                                           {
                                               item.Title,
                                               item.Description,
                                               item.DeadLine == DateTime.MinValue
                                                   ? ""
                                                   : item.DeadLine.ToShortString(),
                                               ASC.Core.CoreContext.UserManager.GetUsers(item.ResponsibleID).
                                                   DisplayUserName(),
                                               contactTitle,
                                               item.IsClosed
                                                   ? CRMTaskResource.TaskStatus_Closed
                                                   : CRMTaskResource.TaskStatus_Open,
                                               listItemDao.GetByID(item.CategoryID).
                                                   Title,
                                               entityTitle
                                           });


                }

                return DataTableToCSV(dataTable);
            }

        }
    }

    public class ExportToCSV
    {

        #region Members

        private static readonly Object _syncObj = new Object();

        private static readonly ProgressQueue _exportQueue = new ProgressQueue(1, TimeSpan.FromSeconds(60), true);

        #endregion

        #region Public Methods

        public static IProgressItem GetStatus()
        {
            return _exportQueue.GetStatus(TenantProvider.CurrentTenantID);
        }

        public static IProgressItem Start()
        {
            lock (_syncObj)
            {

                var operation = _exportQueue.GetStatus(TenantProvider.CurrentTenantID);

                if (operation == null)
                {

                    operation = new ExportDataOperation();

                    _exportQueue.Add(operation);
                }

                if (!_exportQueue.IsStarted)
                    _exportQueue.Start(x => x.RunJob());

                return operation;

            }
        }

        private static String ExportEntityData(ICollection externalData, bool recieveURL, String fileName)
        {
            var operation = new ExportDataOperation(externalData);

            operation.RunJob();

            var data = (String)operation.Status;

            if (recieveURL) return SaveCSVFileInMyDocument(fileName, data);

            return data;

        }

        private static String SaveCSVFileInMyDocument(String title, String data)
        {

            string fileURL;

            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var file = DocumentUtils.UploadFile(ASC.Web.Files.Classes.Global.FolderMy.ToString(), title, data.Length, "text/csv", memStream, true);

                fileURL = CommonLinkUtility.GetFileWebEditorUrl((int)file.ID);
            }

            return fileURL;
        }

        public static String ExportContactsToCSV(List<Contact> contacts, bool recieveURL)
        {

            return ExportEntityData(contacts, recieveURL, "contacts.csv");
        }


        public static String ExportDealsToCSV(List<Deal> deals, bool recieveURL)
        {
            return ExportEntityData(deals, recieveURL, "opportunity.csv");
        }

        public static String ExportHistoryToCSV(List<RelationshipEvent> events, bool recieveURL)
        {
            return ExportEntityData(events, recieveURL, "history.csv");
        }

        public static String ExportCasesToCSV(List<ASC.CRM.Core.Entities.Cases> cases, bool recieveURL)
        {
            return ExportEntityData(cases, recieveURL, "cases.csv");
        }

        public static String ExportTasksToCSV(List<Task> tasks, bool recieveURL)
        {
            return ExportEntityData(tasks, recieveURL, "tasks.csv");
        }

        public static void Cancel()
        {
            lock (_syncObj)
            {
                var findedItem = _exportQueue.GetItems().Where(elem => (int)elem.Id == TenantProvider.CurrentTenantID);

                if (findedItem.Any())
                    _exportQueue.Remove(findedItem.ElementAt(0));
            }
        }

        #endregion

    }

}