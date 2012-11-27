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
    class CachedMilestoneDao : MilestoneDao
    {
        private readonly HttpRequestDictionary<Milestone> _projectCache = new HttpRequestDictionary<Milestone>("milestone");


        public CachedMilestoneDao(string dbId, int tenant)
            : base(dbId, tenant)
        {
        }

        public override Milestone GetById(int id)
        {
            return _projectCache.Get(id.ToString(), () => GetBaseById(id));
        }

        private Milestone GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override Milestone Save(Milestone milestone)
        {
            if (milestone != null)
            {
                ResetCache(milestone.ID);
            }
            return base.Save(milestone);
        }

        public override void Delete(int id)
        {
            ResetCache(id);
            base.Delete(id);
        }

        private void ResetCache(int milestoneId)
        {
            _projectCache.Reset(milestoneId.ToString());
        }
    }

    class MilestoneDao : BaseDao, IMilestoneDao
    {
        private readonly string table = "projects_milestones";


        public MilestoneDao(string dbId, int tenant)
            : base(dbId, tenant)
        {
        }


        public List<Milestone> GetByProject(int projectId)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.project_id", projectId))
                .ConvertAll(r => ToMilestone(r));
        }

        public List<Milestone> GetByFilter(TaskFilter filter)
        {
            var query = CreateQuery();

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query.SetFirstResult((int)filter.Offset);
                query.SetMaxResults((int)filter.Max * 2);
            }

            query.OrderBy("t.status", true);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Milestone"];
                sortColumns.Remove(filter.SortBy);

                query.OrderBy("t." + filter.SortBy, filter.SortOrder);

                foreach (var sort in sortColumns.Keys)
                {
                    query.OrderBy("t." + sort, sortColumns[sort]);
                }
            }

            if (filter.MilestoneStatuses.Count != 0)
            {
                query.Where("t.status", filter.MilestoneStatuses.First());
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

            if (filter.UserId != Guid.Empty)
            {
                query.Where(Exp.Eq("t.responsible_id", filter.UserId));
            }

            if (filter.TagId != 0)
            {
                query.InnerJoin("projects_project_tag ptag", Exp.EqColumns("ptag.project_id", "t.project_id"));
                query.Where("ptag.tag_id", filter.TagId);
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.FromDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Ge("t.deadline", TenantUtil.DateTimeFromUtc(filter.FromDate)));
            }

            if (!filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Le("t.deadline", TenantUtil.DateTimeFromUtc(filter.ToDate)));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query.Where(Exp.Like("t.title", filter.SearchText, SqlLike.AnyWhere));
            }

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToMilestone(r));
        }

        public List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.project_id", projectId).Where("t.status", milestoneStatus))
                .ConvertAll(r => ToMilestone(r));
        }

        public List<Milestone> GetUpcomingMilestones(int offset, int max, params int[] projects)
        {
            var query = CreateQuery()
                .SetFirstResult(offset)
                .Where("p.status", ProjectStatus.Open)
                .Where(Exp.Ge("t.deadline", TenantUtil.DateTimeNow().Date))
                .Where("t.status", MilestoneStatus.Open)
                .SetMaxResults(max)
                .OrderBy("t.deadline", true);
            if (projects != null && 0 < projects.Length)
            {
                query.Where(Exp.In("p.id", projects.Take(0 < max ? max : projects.Length).ToArray()));
            }
            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToMilestone(r));
        }

        public List<Milestone> GetLateMilestones(int offset, int max, params int[] projects)
        {
            var query = CreateQuery()
                .SetFirstResult(offset)
                .Where("p.status", ProjectStatus.Open)
                .Where(!Exp.Eq("t.status", MilestoneStatus.Closed))
                .Where(Exp.Le("t.deadline", TenantUtil.DateTimeNow().Date.AddDays(-1)))
                .SetMaxResults(max)
                .OrderBy("t.deadline", true);
            if (projects != null && 0 < projects.Length)
            {
                query.Where(Exp.In("p.id", projects.Take(0 < max ? max : projects.Length).ToArray()));
            }
            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToMilestone(r));
        }

        public List<Milestone> GetByDeadLine(DateTime deadline)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.deadline", deadline.Date).OrderBy("t.deadline", true))
                .ConvertAll(r => ToMilestone(r));
        }

        public List<Milestone> GetUpdates(DateTime from, DateTime to)
        {
            return DbManager.ExecuteList(CreateQuery()
                .Select("t.status_changed")
                .Where(Exp.Between("t.create_on", from, to) | Exp.Between("t.last_modified_on", from, to) | Exp.Between("t.deadline", from, to)))
                .ConvertAll(x =>
                {
                    var ms = ToMilestone(x);
                    ms.StatusChangedOn = Convert.ToDateTime(x.Last());
                    return ms;
                }).ToList();
        }

        public virtual Milestone GetById(int id)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.id", id))
                .ConvertAll(r => ToMilestone(r))
                .SingleOrDefault();
        }

        public bool IsExists(int id)
        {
            var count = DbManager.ExecuteScalar<long>(Query(table).SelectCount().Where("id", id));
            return 0 < count;
        }

        public List<object[]> GetInfoForReminder(DateTime deadline)
        {
            var q = new SqlQuery("projects_milestones")
                .Select("tenant_id", "id", "deadline")
                .Where(Exp.Between("deadline", deadline.Date.AddDays(-1), deadline.Date.AddDays(1)))
                .Where("status", MilestoneStatus.Open)
                .Where("is_notify", 1);

            return DbManager
                .ExecuteList(q)
                .ConvertAll(r => new object[] { Convert.ToInt32(r[0]), Convert.ToInt32(r[1]), Convert.ToDateTime(r[2]) });
        }

        
        public virtual Milestone Save(Milestone milestone)
        {
            if (milestone.DeadLine.Kind != DateTimeKind.Local)
                milestone.DeadLine = TenantUtil.DateTimeFromUtc(milestone.DeadLine);

            var insert = Insert(table)
                .InColumnValue("id", milestone.ID)
                .InColumnValue("project_id", milestone.Project != null ? milestone.Project.ID : 0)
                .InColumnValue("title", milestone.Title)
                .InColumnValue("create_by", milestone.CreateBy.ToString())
                .InColumnValue("create_on", TenantUtil.DateTimeToUtc(milestone.CreateOn))
                .InColumnValue("last_modified_by", milestone.LastModifiedBy.ToString())
                .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(milestone.LastModifiedOn))
                .InColumnValue("deadline", milestone.DeadLine)
                .InColumnValue("status", milestone.Status)
                .InColumnValue("is_notify", milestone.IsNotify)
                .InColumnValue("is_key", milestone.IsKey)
                .InColumnValue("description", milestone.Description)
                .InColumnValue("status_changed", milestone.StatusChangedOn)
                .InColumnValue("responsible_id", milestone.Responsible.ToString())
                .Identity(1, 0, true);
            milestone.ID = DbManager.ExecuteScalar<int>(insert);
            return milestone;
        }

        public virtual void Delete(int id)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("projects_comments").Where("target_uniq_id", ProjectEntity.BuildUniqId<Milestone>(id)));
                DbManager.ExecuteNonQuery(Delete("projects_review_entity_info").Where("entity_uniqID", "Milestone_" + id));
                DbManager.ExecuteNonQuery(Update("projects_tasks").Set("milestone_id", 0).Where("milestone_id", id));
                DbManager.ExecuteNonQuery(Delete(table).Where("id", id));

                tx.Commit();
            }
        }

        public bool CanReadMilestones(int projectId, Guid userId)
        {
            return 0 < DbManager.ExecuteScalar<int>(Query("projects_tasks").SelectCount().Where("project_id", projectId).Where(!Exp.Eq("milestone_id", 0)).Where("responsible_id", userId.ToString()));
        }


        private SqlQuery CreateQuery()
        {
            return new SqlQuery(table + " t")
                .InnerJoin(ProjectDao.PROJECT_TABLE + " p", Exp.EqColumns("t.project_id", "p.id") & Exp.EqColumns("t.tenant_id", "p.tenant_id"))
                .LeftOuterJoin("projects_tasks as pt", Exp.EqColumns("pt.milestone_id", "t.id") & Exp.EqColumns("pt.tenant_id", "t.tenant_id"))
                .Select(ProjectDao.PROJECT_COLUMNS.Select(c => "p." + c).ToArray())
                .Select("t.id", "t.title", "t.create_by", "t.create_on", "t.last_modified_by", "t.last_modified_on")
                .Select("t.deadline", "t.status", "t.is_notify", "t.is_key", "t.description", "t.responsible_id")
                .SelectSum(String.Format("case pt.responsible_id when '{0}' then 1 else 0 end", SecurityContext.CurrentAccount.ID))
                .SelectSum("case pt.status when 1 then 1 when 4 then 1 else 0 end")
                .SelectSum("case pt.status when 2 then 1 else 0 end")
                .GroupBy("t.id")
                .Where("t.tenant_id", Tenant);
        }

        private Milestone ToMilestone(object[] r)
        {
            var offset = ProjectDao.PROJECT_COLUMNS.Length;
            return new Milestone()
            {
                Project = r[0] != null ? ProjectDao.ToProject(r) : null,
                ID = Convert.ToInt32(r[0 + offset]),
                Title = (string)r[1 + offset],
                CreateBy = ToGuid(r[2 + offset]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[3 + offset]),
                LastModifiedBy = ToGuid(r[4 + offset]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[5 + offset]),
                DeadLine = DateTime.SpecifyKind((DateTime)r[6 + offset], DateTimeKind.Local),
                Status = (MilestoneStatus)Convert.ToInt32(r[7 + offset]),
                IsNotify = Convert.ToBoolean(r[8 + offset]),
                IsKey = Convert.ToBoolean(r[9 + offset]),
                Description = (string)r[10 + offset],
                Responsible = ToGuid(r[11 + offset]),
                CurrentUserHasTasks = Convert.ToBoolean(r[12 + offset]),
                ActiveTaskCount = Convert.ToInt32(r[13 + ProjectDao.PROJECT_COLUMNS.Length]),
                ClosedTaskCount = Convert.ToInt32(r[14 + ProjectDao.PROJECT_COLUMNS.Length])
            };
        }
        

        internal List<Milestone> GetMilestones(Exp where)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where(where))
                .ConvertAll(r => ToMilestone(r));
        }
    }
}
