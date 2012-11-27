#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Collections;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using ASC.CRM.Core.Entities;
using ASC.FullTextIndex;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class CachedTaskDao : TaskDao
    {

        private readonly HttpRequestDictionary<Task> _contactCache = new HttpRequestDictionary<Task>("crm_task");

        public CachedTaskDao(int tenantID, string storageKey)
            : base(tenantID, storageKey)
        {

        }

        public override Task GetByID(int taskID)
        {
            return _contactCache.Get(taskID.ToString(), () => GetByIDBase(taskID));

        }

        private Task GetByIDBase(int taskID)
        {
            return base.GetByID(taskID);
        }

        public override void DeleteTask(int taskID)
        {
            ResetCache(taskID);

            base.DeleteTask(taskID);
        }

        public override int SaveOrUpdateTask(Task task)
        {

            if (task != null && task.ID > 0)
            {
                ResetCache(task.ID);
            }

            return base.SaveOrUpdateTask(task);

        }

        private void ResetCache(int taskID)
        {
            _contactCache.Reset(taskID.ToString());
        }

    }

    public class TaskDao : AbstractDao
    {
        #region Constructor

        public TaskDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {


        }

        #endregion

        #region Methods

        public void OpenTask(int taskID)
        {
            var task = GetByID(taskID);

            if (task == null)
                throw new ArgumentException();

            CRMSecurity.DemandEdit(task);

            DbManager.ExecuteNonQuery(
                 Update("crm_task")
                 .Set("is_closed", false)
                 .Where(Exp.Eq("id", taskID))
              );

        }

        public void CloseTask(int taskID)
        {
            var task = GetByID(taskID);

            if (task == null)
                throw new ArgumentException();

            CRMSecurity.DemandEdit(task);

            DbManager.ExecuteNonQuery(
                   Update("crm_task")
                   .Set("is_closed", true)
                   .Where(Exp.Eq("id", taskID))
                );
        }

        public virtual Task GetByID(int taskID)
        {

            var tasks = DbManager.ExecuteList(GetTaskQuery(Exp.Eq("id", taskID)))
                    .ConvertAll(row => ToTask(row));

            return tasks.Count > 0 ? tasks[0] : null;

        }

        public List<Task> GetTasks(EntityType entityType, int entityID, bool? onlyActiveTask)
        {
            return GetTasks(String.Empty, Guid.Empty, 0, onlyActiveTask, DateTime.MinValue, DateTime.MinValue,
                            entityType, entityID, 0, 0, null);
        }
        public int GetAllTasksCount()
        {

            return DbManager.ExecuteScalar<int>(Query("crm_task").SelectCount());

        }

        public List<Task> GetAllTasks()
        {

            return DbManager.ExecuteList(
           GetTaskQuery(null)
           .OrderBy("deadline", true)
           .OrderBy("title", true))
          .ConvertAll(row => ToTask(row)).FindAll(CRMSecurity.CanAccessTo);
        }

        public List<Task> GetTasks(
                                  String searchText,
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {

            if (CRMSecurity.IsAdmin)
                return GetCrudeTasks(
                    searchText,
                    responsibleID,
                    categoryID,
                    isClosed,
                    fromDate,
                    toDate,
                    entityType,
                    entityID,
                    from,
                    count,
                    orderBy);


            var crudeTasks = GetCrudeTasks(
                    searchText,
                    responsibleID,
                    categoryID,
                    isClosed,
                    fromDate,
                    toDate,
                    entityType,
                    entityID,
                    0,
                    from + count,
                    orderBy);

            if (crudeTasks.Count == 0) return crudeTasks;

            if (crudeTasks.Count < from + count) return crudeTasks.FindAll(CRMSecurity.CanAccessTo).Skip(from).ToList();

            var result = crudeTasks.FindAll(CRMSecurity.CanAccessTo);

            if (result.Count == crudeTasks.Count) return result.Skip(from).ToList();

            var localCount = count;
            var localFrom = from + count;

            while (true)
            {
                crudeTasks = GetCrudeTasks(
                    searchText,
                    responsibleID,
                    categoryID,
                    isClosed,
                    fromDate,
                    toDate,
                    entityType,
                    entityID,
                    localFrom,
                    localCount,
                    orderBy);

                if (crudeTasks.Count == 0) break;

                result.AddRange(crudeTasks.Where(CRMSecurity.CanAccessTo));

                if ((result.Count >= count + from) || (crudeTasks.Count < localCount)) break;

                localFrom += localCount;
                localCount = localCount * 2;
            }

            return result.Skip(from).Take(count).ToList();
        }


        private List<Task> GetCrudeTasks(
                                  String searchText,
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {
            var sqlQuery = WhereConditional(GetTaskQuery(null), responsibleID,
                                        categoryID, isClosed, fromDate, toDate, entityType, entityID, from, count,
                                        orderBy);

            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();


                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                    if (FullTextSearch.SupportModule(FullTextSearch.CRMTasksModule))
                    {
                        var taskIDs = FullTextSearch.Search(searchText, FullTextSearch.CRMTasksModule)
                            .GetIdentifiers()
                            .Select(item => Convert.ToInt32(item)).ToArray();

                        if (taskIDs.Length != 0)
                            sqlQuery.Where(Exp.In("id", taskIDs));
                        else
                            return new List<Task>();
                    }
                    else
                        sqlQuery.Where(BuildLike(new[] { "title" }, keywords));
            }

            return DbManager.ExecuteList(sqlQuery)
                .ConvertAll(row => ToTask(row));

        }

        public int GetTasksCount(
                                  String searchText,
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int count,
                                  OrderBy orderBy)
        {

            var sqlQuery = Query("crm_task").SelectCount();

            sqlQuery = WhereConditional(sqlQuery, responsibleID,
                                        categoryID, isClosed, fromDate, toDate, entityType, entityID, 0, count,
                                        orderBy);


            if (!String.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();


                var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToArray();

                if (keywords.Length > 0)
                    if (FullTextSearch.SupportModule(FullTextSearch.CRMTasksModule))
                    {
                        var taskIDs = FullTextSearch.Search(searchText, FullTextSearch.CRMTasksModule)
                            .GetIdentifiers()
                            .Select(item => Convert.ToInt32(item)).ToArray();

                        if (taskIDs.Length != 0)
                            sqlQuery.Where(Exp.In("id", taskIDs));
                        else
                            return 0;
                    }
                    else
                        sqlQuery.Where(BuildLike(new[] { "title" }, keywords));
            }

            return DbManager.ExecuteScalar<int>(sqlQuery);
        }

        private SqlQuery WhereConditional(
                                  SqlQuery sqlQuery,
                                  Guid responsibleID,
                                  int categoryID,
                                  bool? isClosed,
                                  DateTime fromDate,
                                  DateTime toDate,
                                  EntityType entityType,
                                  int entityID,
                                  int from,
                                  int count,
                                  OrderBy orderBy)
        {

            if (responsibleID != Guid.Empty)
                sqlQuery.Where(Exp.Eq("responsible_id", responsibleID));

            if (entityID > 0)
                switch (entityType)
                {
                    case EntityType.Contact:
                        var isCompany = Convert.ToBoolean(DbManager.ExecuteScalar(Query("crm_contact").Select("is_company").Where(Exp.Eq("id", entityID))));

                        if (isCompany)
                            return WhereConditional(sqlQuery, responsibleID, categoryID, isClosed, fromDate, toDate, EntityType.Company, entityID, from, count, orderBy);
                        else
                            return WhereConditional(sqlQuery, responsibleID, categoryID, isClosed, fromDate, toDate, EntityType.Person, entityID, from, count, orderBy);
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



            if (isClosed.HasValue)
                sqlQuery.Where("is_closed", isClosed);

            if (categoryID > 0)
                sqlQuery.Where(Exp.Eq("category_id", categoryID));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Between("deadline", TenantUtil.DateTimeToUtc(fromDate), TenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1))));
            else if (fromDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Ge("deadline", TenantUtil.DateTimeToUtc(fromDate)));
            else if (toDate != DateTime.MinValue)
                sqlQuery.Where(Exp.Le("deadline", TenantUtil.DateTimeToUtc(toDate.AddDays(1).AddMinutes(-1))));

            if (0 < from && from < int.MaxValue)
                sqlQuery.SetFirstResult(from);

            if (0 < count && count < int.MaxValue)
                sqlQuery.SetMaxResults(count);

            sqlQuery.OrderBy("is_closed", true);

            if (orderBy != null && Enum.IsDefined(typeof(TaskSortedByType), orderBy.SortedBy))
                switch ((TaskSortedByType)orderBy.SortedBy)
                {
                    case TaskSortedByType.Title:
                        sqlQuery
                            .OrderBy("title", orderBy.IsAsc)
                            .OrderBy("deadline", true);
                        break;
                    case TaskSortedByType.DeadLine:
                        sqlQuery.OrderBy("deadline", orderBy.IsAsc)
                                .OrderBy("title", true);
                        break;
                    case TaskSortedByType.Category:
                        sqlQuery.OrderBy("category_id", orderBy.IsAsc)
                                .OrderBy("deadline", true)
                                .OrderBy("title", true);
                        break;
                }
            else
                sqlQuery.OrderBy("deadline", true)
                                   .OrderBy("title", true);


            return sqlQuery;

        }

        public Dictionary<int, Task> GetNearestTask(int[] contactID)
        {

            var nestedQuery = Query("crm_task")
                             .Select("min(deadline) as deadline")
                             .Select("contact_id")
                             .Where(Exp.Eq("is_closed", false) & Exp.In("contact_id", contactID))
                             .GroupBy("contact_id");

            var sqlQuery = GetTaskQuery(null, "tblTask")
                          .From(nestedQuery, "tblSub")
                          .Where(Exp.EqColumns("tblTask.deadline", "tblSub.deadline") & Exp.EqColumns("tblTask.contact_id", "tblSub.contact_id"));

            var sqlResult = DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToTask(row)).FindAll(CRMSecurity.CanAccessTo);

            var result = new Dictionary<int, Task>();

            foreach (var task in sqlResult)
            {
                if (result.ContainsKey(task.ContactID)) continue;

                result.Add(task.ID, task);

            }

            return result;
        }

        public IEnumerable<Guid> GetResponsibles(int categoryID)
        {
            var q = Query("crm_task")
                .Select("responsible_id")
                .GroupBy(1);

            if (0 < categoryID) q.Where("category_id", categoryID);
            return DbManager.ExecuteList(q)
                .ConvertAll(r => (string)r[0])
                .Select(r => new Guid(r))
                .Where(g => g != Guid.Empty)
                .ToList();
        }


        public Dictionary<int, int> GetTasksCount(int[] contactID)
        {
            var sqlQuery = Query("crm_task")
                          .Select("contact_id")
                          .SelectCount()
                          .Where(Exp.In("contact_id", contactID))
                          .GroupBy("contact_id");

            var sqlResult = DbManager.ExecuteList(sqlQuery);

            return sqlResult.ToDictionary(item => Convert.ToInt32(item[0]), item => Convert.ToInt32(item[1]));

        }

        public int GetTasksCount(int contactID)
        {
            var result = GetTasksCount(new[] { contactID });

            if (result.Count == 0) return 0;

            return result[contactID];
        }

        public Dictionary<int, bool> HaveLateTask(int[] contactID)
        {
            var sqlQuery = Query("crm_task")
                          .Select("contact_id")
                          .Where(Exp.In("contact_id", contactID))
                          .Where(Exp.Eq("is_closed", false) &
                           Exp.Lt("deadline", DateTime.UtcNow))
                          .SelectCount()
                          .GroupBy("contact_id");

            var sqlResult = DbManager.ExecuteList(sqlQuery);

            return sqlResult.ToDictionary(item => Convert.ToInt32(item[0]), item => Convert.ToInt32(item[1]) > 0);
        }


        public bool HaveLateTask(int contactID)
        {
            var result = HaveLateTask(new[] { contactID });

            if (result.Count == 0) return false;

            return result[contactID];
        }

        public bool IsExist(int id)
        {
            return DbManager.ExecuteScalar<int>(Query("crm_task").SelectCount().Where(Exp.Eq("id", id))) > 0;
        }

        public virtual int SaveOrUpdateTask(Task newTask)
        {
            if (String.IsNullOrEmpty(newTask.Title) || newTask.DeadLine == DateTime.MinValue ||
                newTask.CategoryID == 0)
                throw new ArgumentException();

            if (!IsExist(newTask.ID))
            {
                newTask.ID = DbManager.ExecuteScalar<int>(
                               Insert("crm_task")
                              .InColumnValue("id", 0)
                              .InColumnValue("title", newTask.Title)
                              .InColumnValue("description", newTask.Description)
                              .InColumnValue("deadline", TenantUtil.DateTimeToUtc(newTask.DeadLine))
                              .InColumnValue("responsible_id", newTask.ResponsibleID)
                              .InColumnValue("contact_id", newTask.ContactID)
                              .InColumnValue("entity_type", (int)newTask.EntityType)
                              .InColumnValue("entity_id", newTask.EntityID)
                              .InColumnValue("is_closed", false)
                              .InColumnValue("category_id", newTask.CategoryID)
                              .InColumnValue("create_on", DateTime.UtcNow)
                              .InColumnValue("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                              .InColumnValue("last_modifed_on", DateTime.UtcNow)
                              .InColumnValue("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                              .Identity(1, 0, true));


            }
            else
            {

                var oldTask = GetByID(newTask.ID);

                CRMSecurity.DemandEdit(oldTask);

                DbManager.ExecuteNonQuery(
                     Update("crm_task")
                     .Set("title", newTask.Title)
                     .Set("description", newTask.Description)
                     .Set("deadline", TenantUtil.DateTimeToUtc(newTask.DeadLine))
                     .Set("responsible_id", newTask.ResponsibleID)
                     .Set("contact_id", newTask.ContactID)
                     .Set("entity_type", (int)newTask.EntityType)
                     .Set("entity_id", newTask.EntityID)
                     .Set("category_id", newTask.CategoryID)
                     .Set("last_modifed_on", DateTime.UtcNow)
                     .Set("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                     .Where(Exp.Eq("id", newTask.ID)));
            }

            return newTask.ID;
        }

        public virtual int[] SaveTaskList(List<Task> items)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var result = new List<int>();

                foreach (var item in items)
                {

                    result.Add(SaveOrUpdateTask(item));

                }

                tx.Commit();

                return result.ToArray();
            }
        }


        public virtual void DeleteTask(int taskID)
        {
            var task = GetByID(taskID);

            if (task == null) return;

            CRMSecurity.DemandEdit(task);

            DbManager.ExecuteNonQuery(Delete("crm_task").Where("id", taskID));
        }

        public List<Task> CreateByTemplate(List<TaskTemplate> templateItems, EntityType entityType, int entityID)
        {

            if (templateItems == null || templateItems.Count == 0) return new List<Task>();

            var result = new List<Task>();
            
            using (var tx = DbManager.BeginTransaction())
            {
                foreach (var templateItem in templateItems)
                {
                    
                    var task = new Task
                    {
                        ResponsibleID = templateItem.ResponsibleID,
                        Description = templateItem.Description,
                        DeadLine = TenantUtil.DateTimeNow().AddTicks(templateItem.Offset.Ticks),
                        CategoryID = templateItem.CategoryID,
                        Title = templateItem.Title,
                        CreateOn = TenantUtil.DateTimeNow(),
                        CreateBy = templateItem.CreateBy
                    };

                    switch (entityType)
                    {
                        case EntityType.Contact:
                        case EntityType.Person:
                        case EntityType.Company:
                            task.ContactID = entityID;
                            break;
                        case EntityType.Opportunity:
                            task.EntityType = EntityType.Opportunity;
                            task.EntityID = entityID;
                            break;
                        case EntityType.Case:
                            task.EntityType = EntityType.Case;
                            task.EntityID = entityID;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    task.ID = SaveOrUpdateTask(task);

                    result.Add(task);

                    DbManager.ExecuteNonQuery(Insert("crm_task_template_task")
                                     .InColumnValue("task_id", task.ID)
                                     .InColumnValue("task_template_id", templateItem.ID));
                    
                }

                tx.Commit();
            }

            return result;
        }


        #region Private Methods

        private static Task ToTask(object[] row)
        {
            return new Task
                       {
                           ID = Convert.ToInt32(row[0]),
                           ContactID = Convert.ToInt32(row[1]),
                           Title = Convert.ToString(row[2]),
                           Description = Convert.ToString(row[3]),
                           DeadLine = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[4])),
                           ResponsibleID = ToGuid(row[5]),
                           IsClosed = Convert.ToBoolean(row[6]),
                           CategoryID = Convert.ToInt32(row[7]),
                           EntityID = Convert.ToInt32(row[8]),
                           EntityType = (EntityType)Convert.ToInt32(row[9]),
                           CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[10])),
                           CreateBy = ToGuid(row[11])
                       };
        }



        private String[] GetTaskColumnsTable(String alias)
        {
            if (!String.IsNullOrEmpty(alias))
                alias = alias + ".";

            var result = new List<String>
                             {
                                "id",
                                "contact_id",
                                "title",
                                "description",
                                "deadline",
                                "responsible_id",
                                "is_closed",
                                "category_id",
                                "entity_id",
                                "entity_type",
                                "create_on",
                                "create_by"                 
                             };

            if (String.IsNullOrEmpty(alias)) return result.ToArray();

            return result.ConvertAll(item => String.Concat(alias, item)).ToArray();
        }

        private SqlQuery GetTaskQuery(Exp where, String alias)
        {

            var sqlQuery = Query("crm_task");

            if (!String.IsNullOrEmpty(alias))
            {
                sqlQuery = new SqlQuery(String.Concat("crm_task ", alias))
                           .Where(Exp.Eq(alias + ".tenant_id", TenantID));
                sqlQuery.Select(GetTaskColumnsTable(alias));

            }
            else
                sqlQuery.Select(GetTaskColumnsTable(String.Empty));


            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;

        }

        private SqlQuery GetTaskQuery(Exp where)
        {
            return GetTaskQuery(where, String.Empty);

        }


        #endregion

        #endregion

    }
}