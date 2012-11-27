using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Collections;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Data.DAO
{
    internal class CachedTaskDao : TaskDao
    {
        private readonly HttpRequestDictionary<Task> _taskCache = new HttpRequestDictionary<Task>("task");

        public CachedTaskDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }

        public override void Delete(int id)
        {
            ResetCache(id);
            base.Delete(id);
        }

        public override void SetTaskOrders(int? milestoneId, int taskID, int? prevTaskID, int? nextTaskID)
        {
            ResetCache(taskID);
            base.SetTaskOrders(milestoneId, taskID, prevTaskID, nextTaskID);
        }

        public override Task GetById(int id)
        {
            return _taskCache.Get(id.ToString(), () => GetBaseById(id));
        }

        private Task GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override Task Save(Task task)
        {
            if (task != null)
            {
                ResetCache(task.ID);
            }
            return base.Save(task);
        }

        private void ResetCache(int taskId)
        {
            _taskCache.Reset(taskId.ToString());
        }
    }


    class TaskDao : BaseDao, ITaskDao
    {
        private readonly string table = "projects_tasks";
        private readonly string responsibleTable = "projects_tasks_responsible";

        public TaskDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }


        public List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant)
        {
            var query = CreateQuery()
                .LeftOuterJoin("projects_milestones m", Exp.EqColumns("m.id", "t.milestone_id") & Exp.EqColumns("m.tenant_id", "t.tenant_id"))
                .Where("t.project_id", projectId)
                .OrderBy("t.sort_order", false)
                .OrderBy("m.status", true)
                .OrderBy("m.deadLine", true)
                .OrderBy("m.id", true)
                .OrderBy("t.status", true)
                .OrderBy("t.priority", true)
                .OrderBy("t.create_on", true);
            if (status != null)
            {
                if (status == TaskStatus.Open)
                    query.Where(!Exp.Eq("t.status", TaskStatus.Closed));
                else
                    query.Where("t.status", TaskStatus.Closed);
            }

            if (participant != Guid.Empty)
            {
                var existSubtask = new SqlQuery("projects_subtasks pst").Select("pst.task_id").Where(Exp.EqColumns("t.tenant_id", "pst.tenant_id") & Exp.EqColumns("t.id", "pst.task_id") & Exp.Eq("pst.status", TaskStatus.Open));
                var existResponsible = new SqlQuery("projects_tasks_responsible ptr1").Select("ptr1.task_id").Where(Exp.EqColumns("t.tenant_id", "ptr1.tenant_id") & Exp.EqColumns("t.id", "ptr1.task_id"));

                existSubtask.Where(Exp.Eq("pst.responsible_id", participant.ToString()));
                existResponsible.Where(Exp.Eq("ptr1.responsible_id", participant.ToString()));

                query.Where(Exp.Exists(existSubtask) | Exp.Exists(existResponsible));
            }

            return DbManager.ExecuteList(query).ConvertAll(r => ToTask(r));
        }

        public List<Task> GetByFilter(TaskFilter filter)
        {
            var query = CreateQuery()
                .LeftOuterJoin("projects_milestones m", Exp.EqColumns("m.id", "t.milestone_id") & Exp.EqColumns("t.tenant_id", "m.tenant_id"))
                .Select("m.title", "m.deadline");

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query.SetFirstResult((int)filter.Offset);
                query.SetMaxResults((int)filter.Max * 2);
            }

            if (filter.ProjectIds.Count != 0)
            {
                query.Where(Exp.In("t.project_id", filter.ProjectIds));
            }
            else
            {
                query.Where(Exp.Eq("p.status", ProjectStatus.Open));

                if (filter.MyProjects)
                {
                    query.InnerJoin("projects_project_participant ppp", Exp.EqColumns("p.id", "ppp.project_id") & Exp.Eq("ppp.removed", false) & Exp.EqColumns("ppp.tenant", "t.tenant_id"));
                    query.Where("ppp.participant_id", SecurityContext.CurrentAccount.ID);
                }
            }

            if (filter.TagId != 0)
            {
                query.InnerJoin("projects_project_tag ptag", Exp.EqColumns("ptag.project_id", "t.project_id"));
                query.Where("ptag.tag_id", filter.TagId);
            }

            query.OrderBy("(case t.status when 2 then 1 else 0 end)", true);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Task"];
                sortColumns.Remove(filter.SortBy);

                query.OrderBy(GetSortFilter(filter.SortBy, filter.SortOrder), filter.SortOrder);

                foreach (var sort in sortColumns.Keys)
                {
                    query.OrderBy(GetSortFilter(sort, sortColumns[sort]), sortColumns[sort]);
                }
            }

            if (filter.TaskStatuses.Count != 0)
            {
                var status = filter.TaskStatuses.First();
                if (status == TaskStatus.Open)
                    query.Where(!Exp.Eq("t.status", TaskStatus.Closed));
                else
                    query.Where("t.status", TaskStatus.Closed);
            }

            if (filter.HasUserId || filter.ParticipantId.HasValue)
            {
                query.LeftOuterJoin("projects_subtasks pst", Exp.EqColumns("t.tenant_id", "pst.tenant_id") & Exp.EqColumns("t.id", "pst.task_id") & Exp.Eq("pst.status", TaskStatus.Open));
            }

            if (filter.ParticipantId.HasValue)
            {
                if (filter.ParticipantId != Guid.Empty)
                {
                    query.Select("group_concat(distinct pst.responsible_id) subtask_resp");
                    query.Having(Exp.Like("subtask_resp", filter.ParticipantId.ToString(), SqlLike.AnyWhere) | Exp.Like("task_resp", filter.ParticipantId.ToString(), SqlLike.AnyWhere));
                }
                else
                {
                    query.Where(Exp.Eq("ptr.task_id", null));
                }
            }

            if (filter.DepartmentId != Guid.Empty)
            {
                query.InnerJoin("core_usergroup cug", Exp.EqColumns("cug.tenant", "t.tenant_id") & Exp.Eq("cug.removed", false) & (Exp.EqColumns("cug.userid", "pst.responsible_id") | Exp.EqColumns("cug.userid", "ptr.responsible_id")));
                query.Where("cug.groupid", filter.DepartmentId);
            }

            if (filter.Milestone.HasValue)
            {
                query.Where("t.milestone_id", filter.Milestone);
            }
            else
            {
                if (filter.MyMilestones)
                {
                    if(!filter.MyProjects)
                    {
                        query.InnerJoin("projects_project_participant ppp", Exp.EqColumns("p.id", "ppp.project_id") & Exp.Eq("ppp.removed", false) & Exp.EqColumns("ppp.tenant", "t.tenant_id"));
                        query.Where("ppp.participant_id", SecurityContext.CurrentAccount.ID); 
                    }

                    query.Where(Exp.Gt("m.id", 0));

                }
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.FromDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Ge(GetSortFilter("deadline", true), TenantUtil.DateTimeFromUtc(filter.FromDate)));
            }

            if (!filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Le(GetSortFilter("deadline", true), TenantUtil.DateTimeFromUtc(filter.ToDate)));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query.Where(Exp.Like("t.title", filter.SearchText, SqlLike.AnyWhere));
            }

            return DbManager.ExecuteList(query).ConvertAll(r => ToTaskMilestone(r));
        }

        public List<Task> GetByResponsible(Guid responsibleId, IEnumerable<TaskStatus> statuses)
        {
            var q = CreateQuery()
                .LeftOuterJoin("projects_subtasks pst", Exp.EqColumns("t.tenant_id", "pst.tenant_id") & Exp.EqColumns("t.id", "pst.task_id"))
                .Where((Exp.Eq("pst.responsible_id", responsibleId) & !Exp.Eq("Coalesce(pst.status,-1)", TaskStatus.Closed)) | Exp.Eq("ptr.responsible_id", responsibleId))
                .OrderBy("t.sort_order", false)
                .OrderBy("t.status", true)
                .OrderBy("t.priority", true)
                .OrderBy("t.create_on", true);

            if (statuses != null && statuses.Any())
            {
                var status = statuses.First();
                if (status == TaskStatus.Open)
                    q.Where(!Exp.Eq("t.status", TaskStatus.Closed));
                else
                    q.Where("t.status", TaskStatus.Closed);
            }

            return DbManager.ExecuteList(q).ConvertAll(r => ToTask(r));
        }

        public List<Task> GetLastTasks(Guid participant, int max, params int[] projects)
        {

            var query = CreateQuery()
                .LeftOuterJoin("projects_subtasks pst", Exp.EqColumns("pst.task_id", "t.id") & Exp.EqColumns("pst.tenant_id", "t.tenant_id"))
                .Where(Exp.Eq("pst.responsible_id", participant) | Exp.Eq("ptr.responsible_id", participant))
                .Where("p.status", ProjectStatus.Open)
                .Where(!Exp.Eq("t.status", TaskStatus.Closed))
                .SetMaxResults(max)
                .OrderBy("t.create_on", false);
            if (projects != null && 0 < projects.Length)
            {
                query.Where(Exp.In("t.project_id", projects));
            }

            return DbManager.ExecuteList(query).ConvertAll(r => ToTask(r));
        }

        public List<Task> GetMilestoneTasks(int milestoneId)
        {
            var query = CreateQuery()
                .Where("t.milestone_id", milestoneId)
                .OrderBy("t.sort_order", false)
                .OrderBy("t.status", true)
                .OrderBy("t.priority", false)
                .OrderBy("t.create_on", false);

            return DbManager.ExecuteList(query).ConvertAll(r => ToTask(r));
        }

        public List<Task> GetUpdates(DateTime from, DateTime to)
        {
            return DbManager.ExecuteList(CreateQuery()
                .Select("t.status_changed")
                .Where(Exp.Between("t.create_on", from, to) | Exp.Between("t.last_modified_on", from, to) | Exp.Between("t.deadline", from, to)))
                .ConvertAll(r=>
                {
                    var task = ToTask(r);
                    task.SubTasks = null;
                    task.StatusChangedOn = Convert.ToDateTime(r.Last());
                    return task;
                }).ToList();
        }

        public int GetTaskCount(int milestoneId, params  TaskStatus[] statuses)
        {
            var query = Query(table)
                .SelectCount()
                .Where("milestone_id", milestoneId);
            if (statuses != null && 0 < statuses.Length)
            {
                query.Where(Exp.In("status", statuses));
            }
            return DbManager.ExecuteScalar<int>(query);
        }

        public List<Task> GetById(ICollection<int> ids)
        {
            var query = CreateQuery().Where(Exp.In("t.id", ids.ToArray()));
            return DbManager.ExecuteList(query).ConvertAll(r => ToTask(r));
        }

        public virtual Task GetById(int id)
        {
            var query = CreateQuery().Where("t.id", id);
            return DbManager.ExecuteList(query).ConvertAll(r => ToTask(r)).SingleOrDefault();
        }

        public bool IsExists(int id)
        {
            var count = DbManager.ExecuteScalar<long>(Query(table).SelectCount().Where("id", id));
            return 0 < count;
        }

        public List<object[]> GetTasksForReminder(DateTime deadline)
        {
            var q = new SqlQuery("projects_tasks")
                .Select("tenant_id", "id", "deadline")
                .Where(Exp.Between("deadline", deadline.Date.AddDays(-1), deadline.Date.AddDays(1)))
                .Where(!Exp.Eq("status", TaskStatus.Closed));

            return DbManager
                .ExecuteList(q)
                .ConvertAll(r => new object[] { Convert.ToInt32(r[0]), Convert.ToInt32(r[1]), Convert.ToDateTime(r[2]) });
        }

        public virtual void SetTaskOrders(int? milestoneId, int taskID, int? prevTaskID, int? nextTaskID)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                var projID = DbManager.ExecuteScalar<int>(new SqlQuery(table).Select("project_id").Where("id", taskID));

                var query = CreateQuery()
                .Where(Exp.Eq("t.milestone_id", milestoneId) & Exp.Eq("t.project_id", projID))
                .OrderBy("t.sort_order", false)
                .OrderBy("t.status", true)
                .OrderBy("t.priority", false)
                .OrderBy("t.create_on", false);

                var tasks = DbManager
                    .ExecuteList(query)
                    .ConvertAll(r => ToTask(r));

                var sortTask = tasks.Find(t => t.ID == taskID);

                if (sortTask != null)
                {
                    tasks.RemoveAll(t => t.ID == taskID);
                    if (prevTaskID.HasValue)
                    {
                        var ind = tasks.FindIndex(t => t.ID == prevTaskID);
                        if (ind != -1 && ind != tasks.Count - 1)
                            tasks.Insert(ind + 1, sortTask);
                        else
                            tasks.Add(sortTask);
                    }
                    else if (nextTaskID.HasValue)
                    {
                        var ind = tasks.FindIndex(t => t.ID == nextTaskID);
                        if (ind > 0)
                            tasks.Insert(ind - 1, sortTask);
                        else
                            tasks.Insert(0, sortTask);
                    }
                }

                for (int i = 0; i < tasks.Count; i++)
                    DbManager.ExecuteNonQuery(new SqlUpdate(table).Set("sort_order", tasks.Count - i)
                                                  .Where("id", tasks[i].ID));

                tr.Commit();
            }
        }

        public virtual Task Save(Task task)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                if (task.SortOrder == 0)
                {
                    task.SortOrder = DbManager.ExecuteScalar<int>(
                        new SqlQuery(table)
                        .SelectMax("sort_order")
                        .Where("project_id", task.Project != null ? task.Project.ID : 0)
                        .Where("milestone_id", task.Milestone));
                    task.SortOrder++;
                }

                if (task.Responsibles == null) 
                    task.Responsibles = new HashSet<Guid> { task.Responsible };
                task.Responsibles.RemoveWhere(r => r.Equals(Guid.Empty));

                if (task.Deadline.Kind != DateTimeKind.Local && task.Deadline != DateTime.MinValue)
                    task.Deadline = TenantUtil.DateTimeFromUtc(task.Deadline);

                var insert = Insert(table)
                    .InColumnValue("id", task.ID)
                    .InColumnValue("project_id", task.Project != null ? task.Project.ID : 0)
                    .InColumnValue("title", task.Title)
                    .InColumnValue("create_by", task.CreateBy.ToString())
                    .InColumnValue("create_on", TenantUtil.DateTimeToUtc(task.CreateOn))
                    .InColumnValue("last_modified_by", task.LastModifiedBy.ToString())
                    .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(task.LastModifiedOn))
                    .InColumnValue("description", task.Description)
                    .InColumnValue("responsible_id", task.Responsibles.FirstOrDefault().ToString())
                    .InColumnValue("priority", task.Priority)
                    .InColumnValue("status", task.Status)
                    .InColumnValue("milestone_id", task.Milestone)
                    .InColumnValue("sort_order", task.SortOrder)
                    .InColumnValue("deadline", task.Deadline)
                    .InColumnValue("status_changed", task.StatusChangedOn)
                    .Identity(1, 0, true);

                task.ID = DbManager.ExecuteScalar<int>(insert);

                DbManager.ExecuteNonQuery(Delete(responsibleTable).Where("task_id", task.ID));

                foreach (var responsible in task.Responsibles)
                {
                    insert = Insert(responsibleTable)
                        .InColumnValue("task_ID", task.ID)
                        .InColumnValue("responsible_id", responsible);

                    DbManager.ExecuteNonQuery(insert);
                }

                tr.Commit();
            }
            return task;
        }

        public virtual void Delete(int id)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("projects_comments").Where("target_uniq_id", ProjectEntity.BuildUniqId<Task>(id)));
                DbManager.ExecuteNonQuery(Delete("projects_review_entity_info").Where("entity_uniqID", "Task_" + id));
                DbManager.ExecuteNonQuery(Delete("projects_tasks_trace").Where("task_id", id));
                DbManager.ExecuteNonQuery(Delete(responsibleTable).Where("task_id", id));
                DbManager.ExecuteNonQuery(Delete(table).Where("id", id));

                tx.Commit();
            }
        }

        public void TaskTrace(int target, Guid owner, DateTime date, TaskStatus status)
        {
            var insert = Insert("projects_tasks_trace")
                .InColumnValue("task_id", target)
                .InColumnValue("action_owner_id", owner.ToString())
                .InColumnValue("action_date", TenantUtil.DateTimeToUtc(date))
                .InColumnValue("status", status);
            DbManager.ExecuteNonQuery(insert);
        }

        #region Recurrence

        public List<object[]> GetRecurrence(DateTime date)
        {
            var q = new SqlQuery("projects_tasks")
                .Select("tenant_id", "task_id")
                .InnerJoin("projects_tasks_recurrence as ptr", Exp.Eq("ptr.task_id", "t.id"))
                .Where(Exp.Ge("ptr.start_date", date))
                .Where(Exp.Le("ptr.end_date", date))
                .Where(Exp.Eq("t.status", TaskStatus.Open));

            return DbManager.ExecuteList(q).ConvertAll(r => new object[] { Convert.ToInt32(r[0]), Convert.ToInt32(r[1]) });
        }

        public void SaveRecurrence(Task task, string cron, DateTime startDate, DateTime endDate)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                var insert = Insert("projects_tasks_recurrence")
                    .InColumnValue("task_id", task.ID)
                    .InColumnValue("cron", cron)
                    .InColumnValue("title", task.Title)
                    .InColumnValue("startDate", startDate)
                    .InColumnValue("endDate", endDate);

                tr.Commit();
            }
        }

        public void DeleteReccurence(int taskId)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("projects_tasks_recurrence").Where("task_id", taskId));
                tx.Commit();
            }
        }

        #endregion

        private SqlQuery CreateQuery()
        {
            return new SqlQuery(table + " t")
                .InnerJoin(ProjectDao.PROJECT_TABLE + " p", Exp.EqColumns("t.project_id", "p.id") & Exp.EqColumns("t.tenant_id", "p.tenant_id"))
                .LeftOuterJoin(responsibleTable + " ptr", Exp.EqColumns("t.tenant_id", "ptr.tenant_id") & Exp.EqColumns("t.id", "ptr.task_id"))
                .Select(ProjectDao.PROJECT_COLUMNS.Select(c => "p." + c).ToArray())
                .Select("t.id", "t.title", "t.create_by", "t.create_on", "t.last_modified_by", "t.last_modified_on")
                .Select("t.description", "t.responsible_id", "t.priority", "t.status", "t.milestone_id", "t.sort_order", "t.deadline")
                .Select("group_concat(distinct ptr.responsible_id) task_resp")
                .Where("t.tenant_id", Tenant)
                .GroupBy("t.id");
        }

        private Task ToTask(object[] r)
        {
            var offset = ProjectDao.PROJECT_COLUMNS.Length;
            var task = new Task
            {
                Project = r[0] != null ? ProjectDao.ToProject(r) : null,
                ID = Convert.ToInt32(r[0 + offset]),
                Title = (string)r[1 + offset],
                CreateBy = ToGuid(r[2 + offset]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[3 + offset]),
                LastModifiedBy = ToGuid(r[4 + offset]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[5 + offset]),
                Description = (string)r[6 + offset],
                Responsible = ToGuid(r[7 + offset]),
                Priority = (TaskPriority)Convert.ToInt32(r[8 + offset]),
                Status = (TaskStatus)Convert.ToInt32(r[9 + offset]),
                Milestone = r[10 + offset] == null ? 0 : Convert.ToInt32(r[10 + offset]),
                SortOrder = Convert.ToInt32(r[11 + offset]),
                Deadline = r[12 + offset] != null ? DateTime.SpecifyKind((DateTime)r[12 + offset], DateTimeKind.Local) : default(DateTime),
                Responsibles = !string.IsNullOrEmpty((string)r[13 + offset]) ? new HashSet<Guid>(((string)r[13 + offset]).Split(',').Select(resp => ToGuid(resp))) : new HashSet<Guid>(),
                SubTasks = new List<Subtask>()
            };

            if (!task.Responsible.Equals(Guid.Empty))
                task.Responsibles.Add(task.Responsible);

            return task;
        }

        private Task ToTaskMilestone(object[] r)
        {
            var task = ToTask(r);
            var offset = ProjectDao.PROJECT_COLUMNS.Length + 14;

            if (task.Milestone > 0)
                task.MilestoneDesc = new Milestone
                                         {
                                             ID = task.Milestone,
                                             Title = (string) r[0 + offset],
                                             DeadLine = TenantUtil.DateTimeFromUtc((DateTime) r[1 + offset])
                                         };

            return task;
        }

        internal List<Task> GetTasks(Exp where)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where(where))
                .ConvertAll(r => ToTask(r));
        }

        private static string GetSortFilter(string sortBy, bool sortOrder)
        {
            if (sortBy != "deadline") return "t." + sortBy;

            var sortDate = sortOrder ? DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
            return string.Format("COALESCE(COALESCE(NULLIF(t.deadline,'{0}'),m.deadline), '{1}')",
                                 DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss"), sortDate);

        }
    }
}
