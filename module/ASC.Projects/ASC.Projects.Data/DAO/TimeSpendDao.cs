using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Data.DAO
{
    class TimeSpendDao : BaseDao, ITimeSpendDao
    {
        private readonly string[] columns = new[] { "id", "note", "date", "hours", "relative_task_id", "person_id", "project_id" };
        private readonly string table = "projects_time_tracking";

        public TimeSpendDao(string dbId, int tenantID) : base(dbId, tenantID) { }

        public List<TimeSpend> GetByFilter(TaskFilter filter)
        {
            var query = CreateQuery();

            if (filter.Max != 0 && !filter.Max.Equals(int.MaxValue))
            {
                query.SetFirstResult((int)filter.Offset);
                query.SetMaxResults((int)filter.Max * 2);
            }

            if (filter.ProjectIds.Count != 0)
            {
                query.Where(Exp.In("t.project_id", filter.ProjectIds));
            }

            if (filter.TagId != 0)
            {
                query.InnerJoin("projects_project_tag ptag", Exp.EqColumns("ptag.project_id", "t.project_id"));
                query.Where("ptag.tag_id", filter.TagId);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["TimeSpend"];
                sortColumns.Remove(filter.SortBy);

                query.OrderBy("t." + filter.SortBy, filter.SortOrder);

                foreach (var sort in sortColumns.Keys)
                {
                    query.OrderBy("t." + sort, sortColumns[sort]);
                }
            }

            if (filter.UserId != Guid.Empty)
            {
                query.Where("t.person_id", filter.UserId);
            }


            if (filter.DepartmentId != Guid.Empty)
            {
                query.InnerJoin("core_usergroup cug", Exp.Eq("cug.removed", false) & Exp.EqColumns("cug.userid", "t.person_id") & Exp.EqColumns("cug.tenant", "t.tenant_id"));
                query.Where("cug.groupid", filter.DepartmentId);
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.FromDate.Equals(DateTime.MaxValue) &&             
                !filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Between("t.date", filter.FromDate, filter.ToDate));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query.Where(Exp.Like("t.note", filter.SearchText, SqlLike.AnyWhere));
            }

            return DbManager.ExecuteList(query).ConvertAll(r => ToTimeSpend(r));;
        }

        public List<TimeSpend> GetByProject(int projectId)
        {
            return DbManager
                .ExecuteList(
                    CreateQuery()
                    .Where("t.project_id", projectId)
                )
                .ConvertAll(r => ToTimeSpend(r));
        }

        public List<TimeSpend> GetByTask(int taskId)
        {
            return DbManager
                .ExecuteList(
                    CreateQuery()
                    .Where("t.relative_task_id", taskId)
                )
                .ConvertAll(r => ToTimeSpend(r));
        }

        public TimeSpend GetById(int id)
        {
            return DbManager
                .ExecuteList(
                    CreateQuery()
                    .Where("t.id", id)
                )
                .ConvertAll(r => ToTimeSpend(r))
                .SingleOrDefault();
        }

        public List<TimeSpend> GetUpdates(DateTime from, DateTime to)
        {
            return DbManager
                .ExecuteList(CreateQuery()
                .Where(Exp.Between("t.date", from, to))).Select(x => ToTimeSpend(x)).ToList();
        }

        public bool HasTime(int taskId)
        {
            var count = DbManager.ExecuteScalar<int>(Query("projects_time_tracking").SelectCount().Where("relative_task_id", taskId));
            return count != 0;
        }

        public Dictionary<int, bool> HasTime(params int[] tasks)
        {
            var result = new Dictionary<int, bool>();
            if (tasks == null || 0 == tasks.Length) return result;

            tasks.ToList().ForEach(id => result[id] = false);

            DbManager
                .ExecuteList(
                    Query("projects_time_tracking")
                    .Select("relative_task_id")
                    .SelectCount()
                    .Where(Exp.In("relative_task_id", tasks))
                    .GroupBy(1)
                )
                .ForEach(r => result[Convert.ToInt32(r[0])] = (long)r[1] != 0);

            return result;
        }


        public TimeSpend Save(TimeSpend timeSpend)
        {
            timeSpend.Date = TenantUtil.DateTimeToUtc(timeSpend.Date);
            var insert = Insert("projects_time_tracking")
                .InColumns(columns)
                .Values(
                    timeSpend.ID,
                    timeSpend.Note,
                    timeSpend.Date,
                    timeSpend.Hours,
                    timeSpend.Task.ID,
                    timeSpend.Person.ToString(),
                    timeSpend.Task.Project.ID
                )
                .Identity(1, 0, true);
            timeSpend.ID = DbManager.ExecuteScalar<int>(insert);
            return timeSpend;
        }

        public void Delete(int id)
        {
            DbManager.ExecuteNonQuery(Delete("projects_time_tracking").Where("id", id));
        }

        private SqlQuery CreateQuery()
        {
            return new SqlQuery(table + " t")
                .Select(columns.Select(c => "t." + c).ToArray())
                .Where("t.tenant_id", Tenant);
        }

        private TimeSpend ToTimeSpend(object[] r)
        {
            return new TimeSpend()
            {
                ID = Convert.ToInt32(r[0]),
                Note = (string)r[1],
                Date = TenantUtil.DateTimeFromUtc((DateTime)r[2]),
                Hours = Convert.ToSingle(r[3]),
                Task = new Task { ID = Convert.ToInt32(r[4]) },
                Person = ToGuid(r[5])
            };
        }
    }
}
