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
    internal class CachedSubtaskDao : SubtaskDao
    {
        private readonly HttpRequestDictionary<Subtask> _subtaskCache = new HttpRequestDictionary<Subtask>("subtask");

        public CachedSubtaskDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }

        public override void Delete(int id)
        {
            ResetCache(id);
            base.Delete(id);
        }

        public override Subtask GetById(int id)
        {
            return _subtaskCache.Get(id.ToString(), () => GetBaseById(id));
        }

        private Subtask GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override Subtask Save(Subtask subtask)
        {
            if (subtask != null)
            {
                ResetCache(subtask.ID);
            }
            return base.Save(subtask);
        }

        private void ResetCache(int subtaskId)
        {
            _subtaskCache.Reset(subtaskId.ToString());
        }
    }

    class SubtaskDao : BaseDao, ISubtaskDao
    {
        private const string Table = "projects_subtasks";


        public SubtaskDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }

        public List<Subtask> GetSubtasks(int taskid)
        {   
            return DbManager
                .ExecuteList(CreateQuery().Where("task_id", taskid))
                .ConvertAll(r => ToSubTask(r));
        }

        public void GetSubtasks(ref List<Task> tasks)
        {
            var subtasks = DbManager
                .ExecuteList(CreateQuery())
                .ConvertAll(r => ToSubTask(r));

            tasks = tasks.GroupJoin(subtasks, task => task.ID, subtask => subtask.Task, (task, subtaskCol) =>
            {    
                task.SubTasks.AddRange(subtaskCol.ToList());
                return task;
            }).ToList();
        }

        public virtual Subtask GetById(int id)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("id", id))
                .ConvertAll(r => ToSubTask(r))
                .SingleOrDefault();
        }

        public List<Subtask> GetById(ICollection<int> ids)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where(Exp.In("id", ids.ToArray())))
                .ConvertAll(r => ToSubTask(r));
        }

        public List<Subtask> GetUpdates(DateTime from, DateTime to)
        {
            return DbManager.ExecuteList(CreateQuery()
                .Select("status_changed")
                .Where(Exp.Between("create_on", from, to) | Exp.Between("last_modified_on", from, to) | Exp.Between("status_changed", from, to)))
                .ConvertAll(x =>
                {
                    var st = ToSubTask(x);
                    st.StatusChangedOn = Convert.ToDateTime(x.Last());
                    return st;
                }).ToList();
        }

        public int GetSubtaskCount(int taskid, params TaskStatus[] statuses)
        {
            var query = Query(Table)
                .SelectCount()
                .Where("task_id", taskid);
            if (statuses != null && 0 < statuses.Length)
            {
                query.Where(Exp.In("status", statuses));
            }
            return DbManager.ExecuteScalar<int>(query);
        }

        public virtual Subtask Save(Subtask subtask)
        {
            using (var tr = DbManager.Connection.BeginTransaction())
            {
                var insert = Insert(Table)
                    .InColumnValue("id", subtask.ID)
                    .InColumnValue("task_id", subtask.Task)
                    .InColumnValue("title", subtask.Title)
                    .InColumnValue("responsible_id", subtask.Responsible.ToString())
                    .InColumnValue("status", subtask.Status)
                    .InColumnValue("create_by", subtask.CreateBy.ToString())
                    .InColumnValue("create_on", TenantUtil.DateTimeToUtc(subtask.CreateOn))
                    .InColumnValue("last_modified_by", subtask.LastModifiedBy.ToString())
                    .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(subtask.LastModifiedOn))
                    .InColumnValue("status_changed", TenantUtil.DateTimeToUtc(subtask.StatusChangedOn))
                    .Identity(1, 0, true);

                subtask.ID = DbManager.ExecuteScalar<int>(insert);

                tr.Commit();
            }

            return subtask;
        }

        public void CloseAllSubtasks(Task task)
        {
            DbManager.ExecuteNonQuery(
                Update(Table)
                .Set("status", TaskStatus.Closed)
                .Set("last_modified_by", SecurityContext.CurrentAccount.ID)
                .Set("last_modified_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                .Set("status_changed", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                .Where("status", TaskStatus.Open)
                .Where("task_id", task.ID));

        }

        public virtual void Delete(int id)
        {
            DbManager.ExecuteNonQuery(Delete(Table).Where("id", id));
        }

        private SqlQuery CreateQuery()
        {
            return new SqlQuery(Table)
                .Select("id", "title", "responsible_id", "status", "create_by", "create_on", "last_modified_by", "last_modified_on", "task_id")
                .OrderBy("status", true)
                .OrderBy("(case status when 1 then create_on else status_changed end)", true)
                .Where("tenant_id", Tenant);
        }

        private static Subtask ToSubTask(IList<object> r)
        {
            return new Subtask
                       {
                ID = Convert.ToInt32(r[0]),
                Title = (string)r[1],
                Responsible = ToGuid(r[2]),
                Status = (TaskStatus)Convert.ToInt32(r[3]),
                CreateBy = ToGuid(r[4]),
                CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[5])),
                LastModifiedBy = ToGuid(r[6]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[7])),
                Task = Convert.ToInt32(r[8])
                       };
        }
    }
}
