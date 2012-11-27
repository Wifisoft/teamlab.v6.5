#region Import

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Caching;
using ASC.Collections;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.CRM.Core.Entities;
using System.Drawing;
using ASC.Files.Core;
using ASC.FullTextIndex;
using ASC.Web.Files.Api;
using AjaxPro;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class CachedDealDao : DealDao
    {

        private readonly HttpRequestDictionary<Deal> _dealCache = new HttpRequestDictionary<Deal>("crm_deal");

        private readonly HttpRequestDictionary<DealMilestone> _dealMilestoneCache = new HttpRequestDictionary<DealMilestone>("crm_dealmilestone");

        public CachedDealDao(int tenantID, string storageKey)
            : base(tenantID, storageKey)
        {

        }

        public override void EditDeal(Deal deal)
        {
            ResetCache(deal.ID);

            base.EditDeal(deal);
        }

        public override Deal GetByID(int dealID)
        {
            return _dealCache.Get(dealID.ToString(), () => GetByIDBase(dealID));
        }

        private Deal GetByIDBase(int dealID)
        {
            return base.GetByID(dealID);
        }

        public override void DeleteDeal(int dealID)
        {
            ResetCache(dealID);

            base.DeleteDeal(dealID);
        }

        public override int CreateNewDeal(Deal deal)
        {

            deal.ID = base.CreateNewDeal(deal);

            _dealCache.Add(deal.ID.ToString(), deal);

            return deal.ID;

        }

        private void ResetCache(int dealID)
        {
            _dealCache.Reset(dealID.ToString());
        }
    }

    public class DealDao : AbstractDao
    {
        #region Constructor

        public DealDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {

        }

        #endregion

        #region Methods

        public void AddMember(int dealID, int memberID)
        {
            SetRelative(memberID, EntityType.Opportunity, dealID);
        }


        public Dictionary<int, int[]> GetMembers(int[] dealID)
        {

            return GetRelativeToEntity(null, EntityType.Opportunity, dealID);

        }

        public int[] GetMembers(int dealID)
        {
            return GetRelativeToEntity(null, EntityType.Opportunity, dealID);
        }

        public void SetMembers(int dealID, int[] memberID)
        {
            SetRelative(memberID, EntityType.Opportunity, dealID);
        }

        public void RemoveMember(int dealID, int memberID)
        {
            RemoveRelative(memberID, EntityType.Opportunity, dealID);
        }

        public virtual List<Deal> GetByID(int[] id)
        {
            return DbManager.ExecuteList(GetDealQuery(Exp.In("tblDeal.id", id))).ConvertAll(row => ToDeal(row));
        }

        public virtual Deal GetByID(int dealID)
        {
            if (dealID <= 0) return null;

            var deals = GetByID(new[] { dealID });

            if (deals.Count == 0) return null;

            return deals[0];

        }

        public virtual int CreateNewDeal(Deal deal)
        {

            if (String.IsNullOrEmpty(deal.Title) || deal.ResponsibleID == Guid.Empty || deal.DealMilestoneID == 0)
                throw new ArgumentException();

            // Delete relative  keys
            _cache.Insert(_dealCacheKey, String.Empty);


            var dealID = DbManager.ExecuteScalar<int>(
                               Insert("crm_deal")
                              .InColumnValue("id", 0)
                              .InColumnValue("title", deal.Title)
                              .InColumnValue("description", deal.Description)
                              .InColumnValue("responsible_id", deal.ResponsibleID)
                              .InColumnValue("contact_id", deal.ContactID)
                              .InColumnValue("bid_currency", deal.BidCurrency)
                              .InColumnValue("bid_value", deal.BidValue)
                              .InColumnValue("bid_type", deal.BidType)
                              .InColumnValue("deal_milestone_id", deal.DealMilestoneID)
                              .InColumnValue("deal_milestone_probability", deal.DealMilestoneProbability)
                              .InColumnValue("expected_close_date", TenantUtil.DateTimeToUtc(deal.ExpectedCloseDate))
                              .InColumnValue("actual_close_date", TenantUtil.DateTimeToUtc(deal.ActualCloseDate))
                              .InColumnValue("per_period_value", deal.PerPeriodValue)
                              .InColumnValue("create_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                              .InColumnValue("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                              .InColumnValue("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                              .InColumnValue("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                              .Identity(1, 0, true));

            //    if (deal.ContactID > 0)
            //      AddMember(dealID, deal.ContactID);

            return dealID;

        }
        
        public virtual int[] SaveDealList(List<Deal> items)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var result = new List<int>();

                foreach (var item in items)
                {

                    result.Add(CreateNewDeal(item));

                }

                tx.Commit();

                return result.ToArray();
            }
        }
        
        public virtual void EditDeal(Deal deal)
        {
            CRMSecurity.DemandAccessTo(deal);

            //   var oldDeal = GetByID(deal.ID);

            //   if (oldDeal.ContactID > 0)
            //      RemoveMember(oldDeal.ID, oldDeal.ContactID);

            //    AddMember(deal.ID, deal.ContactID);

            DbManager.ExecuteNonQuery(
                  Update("crm_deal")
                  .Set("title", deal.Title)
                  .Set("description", deal.Description)
                  .Set("responsible_id", deal.ResponsibleID)
                  .Set("contact_id", deal.ContactID)
                  .Set("bid_currency", deal.BidCurrency)
                  .Set("bid_value", deal.BidValue)
                  .Set("bid_type", deal.BidType)
                  .Set("deal_milestone_id", deal.DealMilestoneID)
                  .Set("deal_milestone_probability", deal.DealMilestoneProbability)
                  .Set("expected_close_date", TenantUtil.DateTimeToUtc(deal.ExpectedCloseDate))
                  .Set("per_period_value", deal.PerPeriodValue)
                  .Set("actual_close_date", TenantUtil.DateTimeToUtc(deal.ActualCloseDate))
                  .Set("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                  .Set("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                  .Where(Exp.Eq("id", deal.ID))
                );
        }
      

        public int GetDealsCount()
        {
            return GetDealsCount(String.Empty,Guid.Empty,0,null,0, null,null,DateTime.MinValue,DateTime.MinValue);

        }

        public List<Deal> GetAllDeals()
        {

            return GetDeals(String.Empty,
                            Guid.Empty,
                            0,
                            null,
                            0,
                            null,
                            null,
                            DateTime.MinValue,
                            DateTime.MinValue,
                            0,
                            0,
                            new OrderBy(DealSortedByType.Stage, true));
        }

        private Exp WhereConditional(
                                  ICollection<int> exceptIDs,
                                  String searchText,
                                  Guid responsibleID,
                                  int milestoneID,
                                  IEnumerable<String> tags,
                                  int contactID,
                                  DealMilestoneStatus? stageType,
                                  bool? contactAlsoIsParticipant)
        {

            var conditions = new List<Exp>();

            var ids = new List<int>();
          
            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                    if (FullTextSearch.SupportModule(FullTextSearch.CRMDealsModule))
                    {
                        ids = FullTextSearch.Search(searchText, FullTextSearch.CRMDealsModule)
                            .GetIdentifiers()
                            .Select(item => Convert.ToInt32(item.Split('_')[1])).Distinct().ToList();

                        if (ids.Count == 0) return null;

                    }
                    else
                      conditions.Add(BuildLike(new[] { "tblDeal.title", "tblDeal.description" }, keywords));
            }

            if (tags != null && tags.Any())
            {
                ids = SearchByTags(EntityType.Opportunity, ids.ToArray(), tags);

                if (ids.Count == 0) return null;
            }

            if (contactID > 0)
            {
                if (contactAlsoIsParticipant.HasValue && contactAlsoIsParticipant.Value)
                {
                    var relativeContactsID = GetRelativeToEntity(contactID, EntityType.Opportunity, null).ToList();

                    if (relativeContactsID.Count == 0)
                    {
                        conditions.Add(Exp.Eq("tblDeal.contact_id", contactID));
                    }
                    else
                    {
                        
                        if (ids.Count > 0)
                        {
                            ids = relativeContactsID.Intersect(ids).ToList();

                            if (ids.Count == 0) return null;
                        }
                        else
                        {
                            ids = relativeContactsID;
                        }
                    }

                  
                }
                else
                    conditions.Add(Exp.Eq("tblDeal.contact_id", contactID));
            }
            
            if (0 < milestoneID && milestoneID < int.MaxValue)
                conditions.Add(Exp.Eq("tblDeal.deal_milestone_id", milestoneID));

            if (responsibleID != Guid.Empty)
                conditions.Add(Exp.Eq("tblDeal.responsible_id", responsibleID));

            if (stageType != null)
                conditions.Add(Exp.Eq("tblDM.status", (int)stageType.Value));

            if (ids.Count > 0)
            {
                if (exceptIDs.Count > 0)
                {
                    ids = ids.Except(exceptIDs).ToList();

                    if (ids.Count == 0) return null;
                }

                conditions.Add(Exp.In("tblDeal.id", ids));

            }
            else if (exceptIDs.Count > 0)
            {
                conditions.Add(!Exp.In("tblDeal.id", exceptIDs.ToArray()));
            }

            if (conditions.Count == 0) return null;

            if (conditions.Count == 1) return conditions[0];

            return conditions.Aggregate((i, j) => i & j);
        }


        public int GetDealsCount(String searchText,
                                  Guid responsibleID,
                                  int milestoneID,
                                  IEnumerable<String> tags,
                                  int contactID,
                                  DealMilestoneStatus? stageType,
                                  bool? contactAlsoIsParticipant,
                                  DateTime fromDate,
                                  DateTime toDate)
        {

            var cacheKey = TenantID.ToString() +
                        "deals" +
                        SecurityContext.CurrentAccount.ID.ToString() +
                        searchText +
                        responsibleID +
                        milestoneID +
                        contactID;

            if (tags != null)
                cacheKey += String.Join("", tags.ToArray());

            if (stageType.HasValue)
                cacheKey += stageType.Value;

            if (contactAlsoIsParticipant.HasValue)
                cacheKey += contactAlsoIsParticipant.Value;

            if (fromDate != DateTime.MinValue)
                cacheKey += fromDate.ToString();
            
            if (toDate != DateTime.MinValue)
                cacheKey += toDate.ToString();
            
            var fromCache = _cache.Get(cacheKey);

            if (fromCache != null) return Convert.ToInt32(fromCache);

            var withParams = !(String.IsNullOrEmpty(searchText) &&
                               responsibleID == Guid.Empty &&
                               milestoneID <= 0 &&
                               (tags == null || !tags.Any()) &&
                               contactID <= 0 &&
                               stageType == null &&
                               contactAlsoIsParticipant == null &&
                               fromDate == DateTime.MinValue &&
                               toDate == DateTime.MinValue);

            ICollection<int> exceptIDs = CRMSecurity.GetPrivateItems(typeof(Deal)).ToList();

            int result;

            if (withParams)
            {
                

                var whereConditional = WhereConditional(exceptIDs, searchText, responsibleID, milestoneID, tags,
                                                        contactID, stageType, contactAlsoIsParticipant);


                var sqlQuery = GetDealQuery(whereConditional);

                if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                {
                    sqlQuery.Having(Exp.Between("close_date", TenantUtil.DateTimeToUtc(fromDate), TenantUtil.DateTimeToUtc(toDate)));

                    result = DbManager.ExecuteList(sqlQuery).Count;

                }
                else if (whereConditional == null)
                {
                    result = 0;
                }
                else
                {
                    result = DbManager.ExecuteList(sqlQuery).Count;
                }

            }
            else
            {

                var countWithoutPrivate = DbManager.ExecuteScalar<int>(Query("crm_deal").SelectCount());
                var privateCount = exceptIDs.Count;

                if (privateCount > countWithoutPrivate)
                {
                    _log.Error("Private deals count more than all deals");

                    privateCount = 0;
                }

                result = countWithoutPrivate - privateCount;
            }

            if (result > 0)
                _cache.Insert(cacheKey, result, new CacheDependency(null, new[] { _dealCacheKey }), Cache.NoAbsoluteExpiration,
                                      TimeSpan.FromSeconds(30));


            return result;

        }

        public List<Deal> GetDeals(
                                  String searchText,
                                  Guid responsibleID,
                                  int milestoneID,
                                  IEnumerable<String> tags,
                                  int contactID,
                                  DealMilestoneStatus? stageType,
                                  bool? contactAlsoIsParticipant,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {

            if (CRMSecurity.IsAdmin)
                return GetCrudeDeals(searchText,
                                     responsibleID,
                                     milestoneID,
                                     tags,
                                     contactID,
                                     stageType,
                                     contactAlsoIsParticipant,
                                     fromDate,
                                     toDate,
                                     from,
                                     count,
                                     orderBy);

            var crudeDeals = GetCrudeDeals(searchText,
                                     responsibleID,
                                     milestoneID,
                                     tags,
                                     contactID,
                                     stageType,
                                     contactAlsoIsParticipant,
                                      fromDate,
                                      toDate,
                                      0,
                                      from + count,
                                      orderBy);

            if (crudeDeals.Count == 0) return crudeDeals;

            if (crudeDeals.Count < from + count) return crudeDeals.FindAll(CRMSecurity.CanAccessTo).Skip(from).ToList();

            var result = crudeDeals.FindAll(CRMSecurity.CanAccessTo);

            if (result.Count == crudeDeals.Count) return result.Skip(from).ToList();

            var localCount = count;
            var localFrom = from + count;

            while (true)
            {
                crudeDeals = GetCrudeDeals(searchText,
                                              responsibleID,
                                              milestoneID,
                                              tags,
                                              contactID,
                                              stageType,
                                              contactAlsoIsParticipant,
                                              fromDate,
                                              toDate,
                                              localFrom,
                                              localCount,
                                              orderBy);

                if (crudeDeals.Count == 0) break;
            
                result.AddRange(crudeDeals.Where(CRMSecurity.CanAccessTo));

                if ((result.Count >= count + from) || (crudeDeals.Count < localCount)) break;

                localFrom += localCount;
                localCount = localCount * 2;
            }

            return result.Skip(from).Take(count).ToList();
        }


        private List<Deal> GetCrudeDeals(
                                   String searchText,
                                   Guid responsibleID,
                                   int milestoneID,
                                   IEnumerable<String> tags,
                                   int contactID,
                                   DealMilestoneStatus? stageType,
                                   bool? contactAlsoIsParticipant,
                                   DateTime fromDate,
                                   DateTime toDate,
                                   int from,
                                   int count,
                                   OrderBy orderBy)
        {
            var sqlQuery = GetDealQuery(null);

            var withParams = !(String.IsNullOrEmpty(searchText) &&
                           responsibleID == Guid.Empty &&
                           milestoneID <= 0 &&
                           (tags == null || !tags.Any()) &&
                           contactID <= 0 &&
                           stageType == null &&
                           contactAlsoIsParticipant == null &&
                           fromDate == DateTime.MinValue &&
                           toDate == DateTime.MinValue);

            var whereConditional = WhereConditional(new List<int>(),
                                                    searchText,
                                                    responsibleID,
                                                    milestoneID,
                                                    tags,
                                                    contactID,
                                                    stageType,
                                                    contactAlsoIsParticipant);

          

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                sqlQuery.Having(Exp.Between("close_date", TenantUtil.DateTimeToUtc(fromDate), TenantUtil.DateTimeToUtc(toDate)));
            else if (withParams && whereConditional == null)
               return new List<Deal>();

            sqlQuery.Where(whereConditional);

            if (0 < from && from < int.MaxValue)
                sqlQuery.SetFirstResult(from);

            if (0 < count && count < int.MaxValue)
                sqlQuery.SetMaxResults(count);

            if (orderBy != null && Enum.IsDefined(typeof(DealSortedByType), orderBy.SortedBy))
                switch ((DealSortedByType)orderBy.SortedBy)
                {
                    case DealSortedByType.Title:
                        sqlQuery.OrderBy("tblDeal.title", orderBy.IsAsc);
                        break;
                    case DealSortedByType.BidValue:
                        sqlQuery.OrderBy("tblDeal.bid_value", orderBy.IsAsc);
                        break;
                    case DealSortedByType.Responsible:

                        sqlQuery.OrderBy("tblDeal.responsible_id", orderBy.IsAsc)
                                .OrderBy("tblDM.sort_order", orderBy.IsAsc)
                                .OrderBy("tblDeal.contact_id", true)
                                .OrderBy("tblDeal.actual_close_date", false)
                                .OrderBy("tblDeal.expected_close_date", true)
                                .OrderBy("tblDeal.title", true);

                        break;
                    case DealSortedByType.Stage:
                        sqlQuery.OrderBy("tblDM.sort_order", orderBy.IsAsc)
                                .OrderBy("tblDeal.contact_id", true)
                                .OrderBy("tblDeal.actual_close_date", false)
                                .OrderBy("tblDeal.expected_close_date", true)
                                .OrderBy("tblDeal.title", true);
                        break;
                    case DealSortedByType.DateAndTime:
                        sqlQuery.OrderBy("close_date", orderBy.IsAsc);
                        break;
                    default:
                        throw new ArgumentException();
                }
            else
                sqlQuery.OrderBy("tblDM.sort_order", true)
                    .OrderBy("tblDeal.contact_id", true)
                    .OrderBy("tblDeal.title", true);

            return DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToDeal(row));

        }

        public List<Deal> GetDealsByContactID(int contactID)
        {

            return GetDeals(String.Empty, Guid.Empty, 0, null, contactID, null, true, DateTime.MinValue,
                            DateTime.MinValue, 0, 0, new OrderBy(DealSortedByType.Title, true));

        }

      
        public virtual void DeleteDeal(int dealID)
        {
            var deal = GetByID(dealID);

            CRMSecurity.DemandAccessTo(deal);

            // Delete relative  keys
            _cache.Insert(_dealCacheKey, String.Empty);


            var tagNames = DbManager.ExecuteList(Query("crm_relationship_event").Select("id").Where(Exp.Eq("have_files", true) & Exp.Eq("entity_id", dealID) & Exp.Eq("entity_type", (int)EntityType.Opportunity)))
                                   .Select(row => String.Format("RelationshipEvent{0}", row[0])).ToArray();

            using (var tx = DbManager.BeginTransaction(true))
            {

                DbManager.ExecuteNonQuery(Delete("crm_field_value").Where(Exp.Eq("entity_id", dealID) & Exp.Eq("entity_type", (int)EntityType.Opportunity)));

                DbManager.ExecuteNonQuery(new SqlDelete("crm_entity_contact")
                                          .Where(Exp.Eq("entity_id", dealID) & Exp.Eq("entity_type", EntityType.Opportunity)));

                DbManager.ExecuteNonQuery(Delete("crm_relationship_event")
                                         .Where(Exp.Eq("entity_id", dealID) & Exp.Eq("entity_type", EntityType.Opportunity)));

                DbManager.ExecuteNonQuery(Delete("crm_task")
                                         .Where(Exp.Eq("entity_id", dealID) & Exp.Eq("entity_type", EntityType.Opportunity)));

                DbManager.ExecuteNonQuery(new SqlDelete("crm_entity_tag").Where(Exp.Eq("entity_id", dealID) & Exp.Eq("entity_type", EntityType.Opportunity)));

                DbManager.ExecuteNonQuery(Delete("crm_deal").Where(Exp.Eq("id", dealID)));

                tx.Commit();
            }

            CoreContext.AuthorizationManager.RemoveAllAces(deal);

            if (tagNames.Length == 0) return;

            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {
                var filesIDs = tagdao.GetTags(tagNames, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();

                var store = FilesIntegration.GetStore();

                foreach (var filesID in filesIDs)
                {
                    filedao.DeleteFolder(filesID);
                    filedao.DeleteFile(filesID);
                }
            }

        }

        #region Private Methods

        private static Deal ToDeal(object[] row)
        {
            return new Deal
            {
                ID = Convert.ToInt32(row[0]),
                Title = Convert.ToString(row[1]),
                Description = Convert.ToString(row[2]),
                ResponsibleID = ToGuid(row[3]),
                ContactID = Convert.ToInt32(row[4]),
                BidCurrency = Convert.ToString(row[5]),
                BidValue = Convert.ToDecimal(row[6]),
                BidType = (BidType)Convert.ToInt32(row[7]),
                DealMilestoneID = Convert.ToInt32(row[8]),
                ExpectedCloseDate = Convert.ToDateTime(row[9]) == DateTime.MinValue ? DateTime.MinValue : TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[9])),
                PerPeriodValue = Convert.ToInt32(row[10]),
                DealMilestoneProbability = Convert.ToInt32(row[11]),
                CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[12])),
                CreateBy = ToGuid(row[13]),
                ActualCloseDate = Convert.ToDateTime(row[14]) == DateTime.MinValue ? DateTime.MinValue : TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[14]))
            };
        }

        private SqlQuery GetDealQuery(Exp where)
        {

            SqlQuery sqlQuery = Query("crm_deal tblDeal")
                .Select(
                    "tblDeal.id",
                    "tblDeal.title",
                    "tblDeal.description",
                    "tblDeal.responsible_id",
                    "tblDeal.contact_id",
                    "tblDeal.bid_currency",
                    "tblDeal.bid_value",
                    "tblDeal.bid_type",
                    "tblDeal.deal_milestone_id",
                    "tblDeal.expected_close_date",
                    "tblDeal.per_period_value",
                    "tblDeal.deal_milestone_probability",
                    "tblDeal.create_on",
                    "tblDeal.create_by",
                    "tblDeal.actual_close_date"
                )
                .Select(@"case tblDM.status
                        when 0
                        then tblDeal.expected_close_date
                        else
                           tblDeal.actual_close_date
                        end as close_date")
                .LeftOuterJoin("crm_deal_milestone tblDM",
                               Exp.EqColumns("tblDeal.deal_milestone_id", "tblDM.id"));

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        #endregion

        #endregion
    }


}


/*
//if (ids.Count > 0)
//{
//}
//else
//{
//    ids = relativeContactsID.ToList();
//    if (ids.Count == 0)
//        conditions.Add(Exp.Eq("tblDeal.contact_id", contactID));
*/
//}
//   sqlQuery.Where(Exp.Or(Exp.In("tblDeal.id", ), Exp.Eq("tblDeal.contact_id", contactID)));