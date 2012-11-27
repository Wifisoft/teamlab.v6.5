#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using ASC.CRM.Core.Entities;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.FullTextIndex;
using ASC.Web.Files.Api;
using ASC.Web.CRM.Resources;
using ASC.Collections;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Utility;
using ASC.Web.CRM.Classes;
using System.Collections;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;

#endregion

namespace ASC.CRM.Core.Dao
{

    public class CachedRelationshipEventDao : RelationshipEventDao
    {
        #region Import

        private readonly HttpRequestDictionary<RelationshipEvent> _contactCache = new HttpRequestDictionary<RelationshipEvent>("crm_relationshipEvent");

        #endregion

        #region Constructor

        public CachedRelationshipEventDao(int tenantID, string storageKey)
            : base(tenantID, storageKey)
        {

        }


        #endregion

        #region Methods

        public override RelationshipEvent GetByID(int eventID)
        {
            return _contactCache.Get(eventID.ToString(), () => GetByIDBase(eventID));

        }

        private RelationshipEvent GetByIDBase(int eventID)
        {
            return base.GetByID(eventID);
        }

        private void ResetCache(int dealID)
        {
            _contactCache.Reset(dealID.ToString());
        }

        #endregion

    }

    public class RelationshipEventDao : AbstractDao
    {
        #region Constructor

        public RelationshipEventDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {

        }

        #endregion

        #region Methods

        public RelationshipEvent AttachFiles(int contactID, EntityType entityType, int entityID, int[] fileIDs)
        {
            if (entityID > 0 && !_supportedEntityType.Contains(entityType))
                throw new ArgumentException();


            var relationshipEvent = new RelationshipEvent
            {
                CategoryID = (int)HistoryCategorySystem.FilesUpload,
                ContactID = contactID,
                EntityType = entityType,
                EntityID = entityID,
                Content = HistoryCategorySystem.FilesUpload.ToLocalizedString()
            };

            var eventID = CreateItem(relationshipEvent).ID;

            AttachFiles(eventID, fileIDs);

            relationshipEvent.ID = eventID;

            return relationshipEvent;
        }

        public void AttachFiles(int eventID, int[] fileIDs)
        {
            if (fileIDs.Length == 0) return;

            using (var dao = FilesIntegration.GetTagDao())
            {

                var tags = fileIDs.ToList().ConvertAll(fileID => new Tag("RelationshipEvent_" + eventID, TagType.System, Guid.Empty) { EntryType = FileEntryType.File, EntryId = fileID });

                dao.SaveTags(tags.ToArray());
            }

            if (fileIDs.Length > 0)
                DbManager.ExecuteNonQuery(Update("crm_relationship_event").Set("have_files", true).Where("id", eventID));

        }

        public int GetFilesCount(int[] contactID, EntityType entityType, int entityID)
        {

            return GetFilesIDs(contactID, entityType, entityID).Length;
        }

        private object[] GetFilesIDs(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();


            var sqlQuery = Query("crm_relationship_event").Select("id");

            if (contactID != null && contactID.Length > 0)
                sqlQuery.Where(Exp.In("contact_id", contactID));

            if (entityID > 0)
                sqlQuery.Where(Exp.Eq("entity_id", entityID) & Exp.Eq("entity_type", (int)entityType));

            sqlQuery.Where(Exp.Eq("have_files", true));

            var tagNames = DbManager.ExecuteList(sqlQuery).ConvertAll(row => String.Format("RelationshipEvent_{0}", row[0]));

            using (var tagdao = FilesIntegration.GetTagDao())
            {
                return tagdao.GetTags(tagNames.ToArray(), TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();
            }

        }

        public List<File> GetAllFiles(int[] contactID, EntityType entityType, int entityID)
        {
            using (var filedao = FilesIntegration.GetFileDao())
            {
                var ids = GetFilesIDs(contactID, entityType, entityID);
                var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File>();

                files.ForEach(CRMSecurity.SetAccessTo);

                return files.ToList();
            }
        }

        public Dictionary<int, List<File>> GetFiles(int[] eventID)
        {

            if (eventID == null || eventID.Length == 0)
                throw new ArgumentException("eventID");

            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {


                var findedTags = tagdao.GetTags(eventID.Select(item => String.Concat("RelationshipEvent_", item)).ToArray(),
                    TagType.System).Where(t => t.EntryType == FileEntryType.File);

                var filesID = findedTags.Select(t => t.EntryId).ToArray();
                
                var files = 0 < filesID.Length ? filedao.GetFiles(filesID) : new List<File>();

                var filesTemp = new Dictionary<object, File>();

                files.ForEach(item =>
                                  {
                                     if (!filesTemp.ContainsKey(item.ID))
                                         filesTemp.Add(item.ID, item);
                                  });



              return  findedTags.GroupBy(x => x.TagName).ToDictionary(x => Convert.ToInt32(x.Key.Split(new[] {'_'})[1]),
                                                                x => x.Select(item => filesTemp[item.EntryId]).ToList());



            }

        }

        public List<File> GetFiles(int eventID)
        {
            if (eventID == 0)
                throw new ArgumentException("eventID");

            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {
                var ids = tagdao.GetTags(String.Concat("RelationshipEvent_", eventID), TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();
                var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File>();

                files.ForEach(CRMSecurity.SetAccessTo);

                return files.ToList();
            }
        }

        private void RemoveAllFiles(int[] contactID, EntityType entityType, int entityID)
        {

            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();

            var files = GetAllFiles(contactID, entityType, entityID);

            using (var dao = FilesIntegration.GetFileDao())
            {
                foreach (var file in files)
                {
                    dao.DeleteFolder(file.ID);
                    dao.DeleteFile(file.ID);

                }
            }

        }

        public void RemoveFile(File file)
        {
            CRMSecurity.DemandEdit(file);
            
            var eventIDs = new List<int>();

            using (var dao = FilesIntegration.GetFileDao())
            {
                dao.DeleteFile(file.ID);

            }
            using (var tagdao = FilesIntegration.GetTagDao())
            {
                var tags = tagdao.GetTags(file.ID, FileEntryType.File, TagType.System).ToList().FindAll(tag => tag.TagName.StartsWith("RelationshipEvent_"));

                eventIDs = tags.Select(item => Convert.ToInt32(item.TagName.Split(new[] { '_' })[1])).ToList();
            }

            foreach (var eventID in eventIDs)
            {
                if (GetFiles(eventID).Count == 0)
                        DbManager.ExecuteNonQuery(Update("crm_relationship_event").Set("have_files", false).Where("id", eventID));
                
            }

        }
        
        
        public int GetCount(int[] contactID, EntityType entityType, int entityID)
        {

            if (entityID > 0 && entityType != EntityType.Opportunity && entityType != EntityType.Case)
                throw new ArgumentException();

            var sqlQuery = Query("crm_relationship_event").SelectCount();

            contactID = contactID.Where(item => item != 0).ToArray();

            if (contactID.Length > 0)
                sqlQuery.Where(Exp.In("contact_id", contactID));

            if (entityID > 0)
                sqlQuery.Where(Exp.Eq("entity_id", entityID) & Exp.Eq("entity_type", (int)entityType));

            return DbManager.ExecuteScalar<int>(sqlQuery);
        }

        public RelationshipEvent CreateItem(RelationshipEvent item)
        {

            if (String.IsNullOrEmpty(item.Content) || item.CategoryID == 0 || (item.ContactID == 0 && item.EntityID == 0))
                throw new ArgumentException();

            if (item.EntityID > 0 && item.EntityType != EntityType.Opportunity && item.EntityType != EntityType.Case)
                throw new ArgumentException();

            if (item.CreateOn == DateTime.MinValue)
                item.CreateOn = TenantUtil.DateTimeNow();

            item.ID = DbManager.ExecuteScalar<int>(
                           Insert("crm_relationship_event")
                          .InColumnValue("id", 0)
                          .InColumnValue("contact_id", item.ContactID)
                          .InColumnValue("content", item.Content)
                          .InColumnValue("create_on", TenantUtil.DateTimeToUtc(item.CreateOn))
                          .InColumnValue("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                          .InColumnValue("entity_type", (int)item.EntityType)
                          .InColumnValue("entity_id", item.EntityID)
                          .InColumnValue("category_id", item.CategoryID)
                          .InColumnValue("last_modifed_on", DateTime.UtcNow)
                          .InColumnValue("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                          .InColumnValue("have_files", false)
                          .Identity(1, 0, true));

            if (item.CreateOn.Kind == DateTimeKind.Utc)
                item.CreateOn = TenantUtil.DateTimeFromUtc(item.CreateOn);

            return item;
        }

        public void EditItem(RelationshipEvent item)
        {
            if (String.IsNullOrEmpty(item.Content) || item.CategoryID == 0 || (item.ContactID == 0 & item.EntityID == 0))
                throw new ArgumentException();

            if (item.EntityID > 0 && item.EntityType != EntityType.Opportunity && item.EntityType != EntityType.Case)
                throw new ArgumentException();


            DbManager.ExecuteNonQuery(Update("crm_relationship_event")
                                      .Set("contact_id", item.ContactID)
                                      .Set("content", item.Content)
                                      .Set("entity_type", (int)item.EntityType)
                                      .Set("entity_id", item.EntityID)
                                      .Set("category_id", item.CategoryID)
                                      .Set("last_modifed_on", DateTime.UtcNow)
                                      .Set("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                                      );
        }

        public virtual RelationshipEvent GetByID(int eventID)
        {
            return DbManager.ExecuteList(GetRelationshipEventQuery(Exp.Eq("id", eventID)))
              .ConvertAll(row => ToRelationshipEvent(row))[0];

        }

        public int GetAllItemsCount()
        {

            return DbManager.ExecuteScalar<int>(Query("crm_relationship_event").SelectCount());

        }

        public List<RelationshipEvent> GetAllItems()
        {
            return GetItems(String.Empty,
                EntityType.Any,
                0,
                Guid.Empty,
                0,
                DateTime.MinValue,
                DateTime.MinValue,
                0,
                0, null);
        }

        public List<RelationshipEvent> GetItems(
            String searchText,
            EntityType entityType,
            int entityID,
            Guid createBy,
            int categoryID,
            DateTime fromDate,
            DateTime toDate,
            int from,
            int count,
            OrderBy orderBy)
        {

            var sqlQuery = GetRelationshipEventQuery(null);

            if (entityID > 0)
                switch (entityType)
                {
                    case EntityType.Contact:
                        var isCompany = Convert.ToBoolean(DbManager.ExecuteScalar(Query("crm_contact").Select("is_company").Where(Exp.Eq("id", entityID))));

                        if (isCompany)
                            return GetItems(searchText, EntityType.Company, entityID, createBy, categoryID, fromDate, toDate, from, count, orderBy);
                        else
                            return GetItems(searchText, EntityType.Person, entityID, createBy, categoryID, fromDate, toDate, from, count, orderBy);
                    case EntityType.Person:
                        sqlQuery.Where(Exp.Eq("contact_id", entityID));
                        break;
                    case EntityType.Company:

                        var personIDs = GetRelativeToEntity(entityID, EntityType.Person, null).ToList();

                        if (personIDs.Count == 0)
                            sqlQuery.Where(Exp.Eq("contact_id", entityID));
                        else
                        {
                            personIDs.Add(entityID);
                            sqlQuery.Where(Exp.In("contact_id", personIDs));
                        }

                        break;
                    case EntityType.Case:
                    case EntityType.Opportunity:
                        sqlQuery.Where(Exp.Eq("entity_id", entityID) &
                                       Exp.Eq("entity_type", (int)entityType));
                        break;
                }

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
               sqlQuery.Where(Exp.Between("create_on", TenantUtil.DateTimeToUtc(fromDate), TenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1))));
            else if (fromDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Ge("create_on", TenantUtil.DateTimeToUtc(fromDate)));
            else if (toDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Le("create_on", TenantUtil.DateTimeToUtc(toDate).AddDays(1).AddMinutes(-1)));

            if (createBy != Guid.Empty)
                sqlQuery.Where(Exp.Eq("create_by", createBy));

            if (categoryID > 0)
                sqlQuery.Where(Exp.Eq("category_id", categoryID));

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();

                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                    sqlQuery.Where(BuildLike(new[] { "content" }, keywords));
            }

            if (0 < from && from < int.MaxValue)
                sqlQuery.SetFirstResult(from);

            if (0 < count && count < int.MaxValue)
                sqlQuery.SetMaxResults(count);

            if (orderBy != null && Enum.IsDefined(typeof(RelationshipEventByType), orderBy.SortedBy))
                switch ((RelationshipEventByType)orderBy.SortedBy)
                {

                    case RelationshipEventByType.Category:
                        sqlQuery.OrderBy("category_id", orderBy.IsAsc);
                        break;
                    case RelationshipEventByType.Content:
                        sqlQuery.OrderBy("content", orderBy.IsAsc);
                        break;
                    case RelationshipEventByType.CreateBy:
                        sqlQuery.OrderBy("create_by", orderBy.IsAsc);
                        break;
                    case RelationshipEventByType.Created:
                        sqlQuery.OrderBy("create_on", orderBy.IsAsc);
                        break;
                }
            else
                sqlQuery.OrderBy("create_on", false);

            return DbManager.ExecuteList(sqlQuery)
                  .ConvertAll(row => ToRelationshipEvent(row));
        }

        private static RelationshipEvent ToRelationshipEvent(object[] row)
        {
            return new RelationshipEvent
                       {

                           ID = Convert.ToInt32(row[0]),
                           ContactID = Convert.ToInt32(row[1]),
                           Content = Convert.ToString(row[2]),
                           EntityID = Convert.ToInt32(row[3]),
                           EntityType = (EntityType)Convert.ToInt32(row[4]),
                           CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[5])),
                           CreateBy = ToGuid(row[6]),
                           CategoryID = Convert.ToInt32(row[7])
                       };
        }

        private SqlQuery GetRelationshipEventQuery(Exp where)
        {
            SqlQuery sqlQuery = Query("crm_relationship_event")
               .Select("id",
                       "contact_id",
                       "content",
                       "entity_id",
                       "entity_type",
                       "create_on",
                       "create_by",
                       "category_id"
                  );

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        public void DeleteItem(int id)
        {
            var relativeFiles = GetFiles(id);

            var nowFilesEditing =  relativeFiles.Where(file => (file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing);

            if (nowFilesEditing.Count() != 0)
                throw new ArgumentException();

            relativeFiles.ForEach(RemoveFile);
                
            DbManager.ExecuteNonQuery(Delete("crm_relationship_event").Where(Exp.Eq("id", id)));

        }

        #endregion
    }
}
