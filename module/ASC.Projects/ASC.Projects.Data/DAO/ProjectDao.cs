using System;
using System.Collections;
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
    internal class CachedProjectDao : ProjectDao
    {
        private readonly HttpRequestDictionary<Project> _projectCache = new HttpRequestDictionary<Project>("project");

        public CachedProjectDao(string dbId, int tenantId)
            : base(dbId, tenantId)
        {
        }

        public override void Delete(int projectId)
        {
            ResetCache(projectId);
            base.Delete(projectId);
        }

        public override void RemoveFromTeam(int projectId, Guid participantId)
        {
            ResetCache(projectId);
            base.RemoveFromTeam(projectId, participantId);
        }

        public override Project Save(Project project)
        {
            if (project != null)
            {
                ResetCache(project.ID);
            }
            return base.Save(project);
        }

        public override Project GetById(int projectId)
        {
            return _projectCache.Get(projectId.ToString(), () => GetBaseById(projectId));
        }

        private Project GetBaseById(int projectId)
        {
            return base.GetById(projectId);
        }

        public override void AddToTeam(int projectId, Guid participantId)
        {
            ResetCache(projectId);
            base.AddToTeam(projectId, participantId);
        }

        private void ResetCache(int projectId)
        {
            _projectCache.Reset(projectId.ToString());
        }

    }

    class ProjectDao : BaseDao, IProjectDao
    {
        public static readonly string PROJECT_TABLE = "projects_projects";

        public static readonly string[] PROJECT_COLUMNS = new[] { "id", "title", "description", "status", "responsible_id", "private", "create_by", "create_on", "last_modified_by", "last_modified_on" };

        private static HttpRequestDictionary<TeamCacheItem> teamCache = new HttpRequestDictionary<TeamCacheItem>("ProjectDao-TeamCacheItem");



        public ProjectDao(string dbId, int tenantId)
            : base(dbId, tenantId)
        {
        }


        public List<Project> GetAll(ProjectStatus? status, int max)
        {
            var query = Query(PROJECT_TABLE)
                .Select(PROJECT_COLUMNS)
                .SetMaxResults(max)
                .OrderBy("title", true);
            if (status != null) query.Where("status", status);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToProject(r))
                .ToList();
        }

        public List<Project> GetLast(ProjectStatus? status, int offset, int max)
        {
            var query = Query(PROJECT_TABLE)
                .SetFirstResult(offset)
                .Select(PROJECT_COLUMNS)
                .SetMaxResults(max)
                .OrderBy("create_on", false);
            if (status != null) query.Where("status", status);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToProject(r))
                .ToList();
        }

        public List<Project> GetByParticipiant(Guid participantId, ProjectStatus status)
        {
            var query = Query(PROJECT_TABLE)
                .Select(PROJECT_COLUMNS)
                .InnerJoin("projects_project_participant", (Exp.EqColumns("id", "project_id") & Exp.Eq("removed", false) & Exp.EqColumns("tenant", "tenant_id")))
                .Where("status", status)
                .Where("participant_id", participantId.ToString())
                .OrderBy("title", true);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToProject(r))
                .ToList();
        }

        public List<Project> GetByFilter(TaskFilter filter)
        {
            var query = new SqlQuery("projects_projects p")
                           .Select(PROJECT_COLUMNS.Select(c => "p." + c).ToArray())
                           .Select(new SqlQuery("projects_milestones m").SelectCount().Where(Exp.EqColumns("m.project_id", "p.id")).Where(Exp.Eq("m.status", MilestoneStatus.Open)))
                           .Select(new SqlQuery("projects_tasks t").SelectCount().Where(Exp.EqColumns("t.project_id", "p.id")).Where(!Exp.Eq("t.status", TaskStatus.Closed)))
                           .Select(new SqlQuery("projects_project_participant pp").SelectCount().Where(Exp.EqColumns("pp.project_id", "p.id") & Exp.Eq("pp.removed", false)))
                           .Select("p.private")
                           .Where("p.tenant_id", Tenant);

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query.SetFirstResult((int)filter.Offset);
                query.SetMaxResults((int)filter.Max * 2);
            }

            if (filter.TagId != 0)
            {
                query.InnerJoin("projects_project_tag ptag", Exp.EqColumns("ptag.project_id", "p.id"));
                query.Where("ptag.tag_id", filter.TagId);
            }

            if (filter.ParticipantId.HasValue && filter.ParticipantId != Guid.Empty)
            {
                query.InnerJoin("projects_project_participant ppp", Exp.EqColumns("p.id", "ppp.project_id") & Exp.Eq("ppp.removed", false) & Exp.EqColumns("ppp.tenant", "p.tenant_id"));
                query.Where("ppp.participant_id", filter.ParticipantId);
            } 
            
            if (filter.UserId != Guid.Empty)
            {
                query.Where("responsible_id", filter.UserId);
            }

            if (filter.Follow)
            {
                query.InnerJoin("projects_following_project_participant pfpp", Exp.EqColumns("p.id", "pfpp.project_id"));
                query.Where(Exp.Eq("pfpp.participant_id", SecurityContext.CurrentAccount.ID));
            }

            query.OrderBy("(case p.status when 2 then 1 when 1 then 2 else 0 end)", true);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Project"];
                sortColumns.Remove(filter.SortBy);

                query.OrderBy("p." + filter.SortBy, filter.SortOrder);

                foreach (var sort in sortColumns.Keys)
                {
                    query.OrderBy("p." + sort, sortColumns[sort]);
                }
            }

            if (filter.ProjectStatuses.Count != 0)
            {
                query.Where(Exp.Eq("p.status", filter.ProjectStatuses.First()));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query.Where(Exp.Like("p.title", filter.SearchText, SqlLike.AnyWhere));
            }

            query.GroupBy("p.id");

            return DbManager
                .ExecuteList(query)
                .Select(r => new Project()
                {
                    ID = Convert.ToInt32(r[0]),
                    Title = (string)r[1],
                    Description = (string)r[2],
                    Status = (ProjectStatus)Convert.ToInt32(r[3]),
                    Responsible = ToGuid(r[4]),
                    Private = Convert.ToBoolean(r[5]),
                    CreateBy = ToGuid(r[6]),
                    CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[7]),
                    LastModifiedBy = ToGuid(r[8]),
                    LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[9]),
                    TaskCount = Convert.ToInt32(r[11]),
                    MilestoneCount = Convert.ToInt32(r[10]),
                    ParticipantCount = Convert.ToInt32(r[12])
                })
                .ToList();
        }

        public List<Project> GetFollowing(Guid participantId)
        {
            var query = Query(PROJECT_TABLE)
                    .Select(PROJECT_COLUMNS)
                    .InnerJoin("projects_following_project_participant", Exp.EqColumns("id", "project_id"))
                    .Where("participant_id", participantId.ToString())
                    .Where("status", ProjectStatus.Open)
                    .OrderBy("create_on", true);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToProject(r))
                .ToList();
        }

        public List<Project> GetUpdates(DateTime from, DateTime to)
        {
            return DbManager
                .ExecuteList(Query(PROJECT_TABLE + " p")
                    .Select(PROJECT_COLUMNS)
                    .Select("status_changed")
                    .Where(Exp.Between("p.create_on", from, to) | Exp.Between("p.last_modified_on", from, to)))
                .ConvertAll(r =>
                {
                    var proj = ToProject(r);
                    proj.StatusChangedOn = Convert.ToDateTime(r[PROJECT_COLUMNS.Count()]);
                    return proj;
                });
        }

        public virtual Project GetById(int projectId)
        {
            return DbManager
                .ExecuteList(Query(PROJECT_TABLE).Select(PROJECT_COLUMNS).Where("id", projectId))
                .ConvertAll(r => ToProject(r))
                .SingleOrDefault();
        }

        public List<Project> GetById(ICollection projectIDs)
        {
            return DbManager
             .ExecuteList(
                Query(PROJECT_TABLE)
                .Select(PROJECT_COLUMNS).Where(Exp.In("id", projectIDs)))
             .ConvertAll(r => ToProject(r))
             .ToList();
        }

        public bool IsExists(int projectId)
        {
            var count = DbManager.ExecuteScalar<int>(Query(PROJECT_TABLE).SelectCount().Where("id", projectId));
            return 0 < count;
        }


        public int Count()
        {
            return DbManager.ExecuteScalar<int>(Query(PROJECT_TABLE).SelectCount());
        }

        public List<int> GetTaskCount(List<int> projectId, params TaskStatus[] taskStatus)
        {
            var query = Query("projects_tasks")
                   .Select("project_id").SelectCount()
                   .Where(Exp.In("project_id", projectId))
                   .GroupBy("project_id");
            if (taskStatus != null && 0 < taskStatus.Length)
            {
                query.Where(Exp.In("status", taskStatus));
            }
            var result = DbManager
                            .ExecuteList(query);

            return projectId.ConvertAll(
                    pid =>
                    {
                        var res = result.Find(r => Convert.ToInt32(r[0]) == pid);
                        return res == null ? 0 : Convert.ToInt32(res[1]);
                    }
                );
        }

        public int GetMessageCount(int projectId)
        {
            var query = Query("projects_messages")
                .SelectCount()
                .Where("project_id", projectId);
            return DbManager.ExecuteScalar<int>(query);
        }

        public int GetTotalTimeCount(int projectId)
        {
            var query = Query("projects_time_tracking")
                .SelectCount()
                .Where("project_id", projectId);
            return DbManager.ExecuteScalar<int>(query);
        }

        public int GetMilestoneCount(int projectId, params MilestoneStatus[] statuses)
        {
            var query = Query("projects_milestones")
                .SelectCount()
                .Where("project_id", projectId);
            if (statuses != null && 0 < statuses.Length)
            {
                query.Where(Exp.In("status", statuses));
            }
            return DbManager.ExecuteScalar<int>(query);
        }

        public virtual Project Save(Project project)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var insert = Insert(PROJECT_TABLE)
                    .InColumns(PROJECT_COLUMNS)
                    .Values(
                        project.ID,
                        project.Title,
                        project.Description,
                        project.Status,
                        project.Responsible.ToString(),
                        project.Private,
                        project.CreateBy.ToString(),
                        TenantUtil.DateTimeToUtc(project.CreateOn),
                        project.LastModifiedBy.ToString(),
                        TenantUtil.DateTimeToUtc(project.LastModifiedOn))
                    .InColumnValue("status_changed", project.StatusChangedOn)
                    .Identity(1, 0, true);
                project.ID = DbManager.ExecuteScalar<int>(insert);

                //remove not used tags
                DbManager.ExecuteNonQuery(Delete("projects_tags").Where(!Exp.In("id", new SqlQuery("projects_project_tag").Select("tag_id"))));

                tx.Commit();
            }
            return project;
        }

        public virtual void Delete(int projectId)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("projects_events").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(Delete("projects_issues").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(new SqlDelete("projects_project_participant").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(new SqlDelete("projects_following_project_participant").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(new SqlDelete("projects_project_tag").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(Delete("projects_tags").Where(!Exp.In("id", new SqlQuery("projects_project_tag").Select("tag_id"))));
                DbManager.ExecuteNonQuery(Delete("projects_time_tracking").Where("project_id", projectId));

                DbManager.ExecuteNonQuery(new SqlDelete("projects_project_tag_change_request").Where(Exp.In("project_id", Query("projects_project_change_request").Select("id").Where("project_id", projectId))));
                DbManager.ExecuteNonQuery(Delete("projects_project_change_request").Where("project_id", projectId));

                var messages = DbManager.ExecuteList(Query("projects_messages").Select("concat('Message_', cast(id as char))").Where("project_id", projectId)).ConvertAll(r => (string)r[0]);
                var milestones = DbManager.ExecuteList(Query("projects_milestones").Select("concat('Milestone_', cast(id as char))").Where("project_id", projectId)).ConvertAll(r => (string)r[0]);
                var tasks = DbManager.ExecuteList(Query("projects_tasks").Select("concat('Task_', cast(id as char))").Where("project_id", projectId)).ConvertAll(r => (string)r[0]);

                DbManager.ExecuteNonQuery(Delete("projects_comments").Where(Exp.In("target_uniq_id", messages)));
                DbManager.ExecuteNonQuery(Delete("projects_comments").Where(Exp.In("target_uniq_id", milestones)));
                DbManager.ExecuteNonQuery(Delete("projects_comments").Where(Exp.In("target_uniq_id", tasks)));

                DbManager.ExecuteNonQuery(Delete("projects_review_entity_info").Where(Exp.In("entity_uniqID", messages)));
                DbManager.ExecuteNonQuery(Delete("projects_review_entity_info").Where(Exp.In("entity_uniqID", milestones)));
                DbManager.ExecuteNonQuery(Delete("projects_review_entity_info").Where(Exp.In("entity_uniqID", tasks)));

                DbManager.ExecuteNonQuery(Delete("projects_tasks_trace").Where(Exp.In("task_id", Query("projects_tasks").Select("id").Where("project_id", projectId))));

                DbManager.ExecuteNonQuery(Delete("projects_messages").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(Delete("projects_milestones").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(Delete("projects_tasks").Where("project_id", projectId));

                DbManager.ExecuteNonQuery(Delete(PROJECT_TABLE).Where("id", projectId));

                tx.Commit();
            }
        }


        public virtual void AddToTeam(int projectId, Guid participantId)
        {
            DbManager.ExecuteNonQuery(
                new SqlInsert("projects_project_participant", true)
                .InColumnValue("tenant", Tenant)
                .InColumnValue("project_id", projectId)
                .InColumnValue("participant_id", participantId.ToString())
                .InColumnValue("created", DateTime.UtcNow)
                .InColumnValue("updated", DateTime.UtcNow)
                .InColumnValue("removed", false));

            lock (teamCache)
            {
                var key = string.Format("{0}|{1}", projectId, participantId);
                var item = teamCache.Get(key, () => new TeamCacheItem(true, ProjectTeamSecurity.None));
                if (item != null) item.InTeam = true;
            }
        }

        public virtual void RemoveFromTeam(int projectId, Guid participantId)
        {
            DbManager.ExecuteNonQuery(
                new SqlUpdate("projects_project_participant")
                .Set("removed", true)
                .Set("updated", DateTime.UtcNow)
                .Where("tenant", Tenant)
                .Where("project_id", projectId)
                .Where("participant_id", participantId.ToString()));

            lock (teamCache)
            {
                var key = string.Format("{0}|{1}", projectId, participantId);
                var item = teamCache.Get(key, () => new TeamCacheItem(true, ProjectTeamSecurity.None));
                if (item != null) item.InTeam = false;
            }
        }

        public bool IsInTeam(int projectId, Guid participantId)
        {
            return GetTeamItemFromCacheOrLoad(projectId, participantId).InTeam;
        }

        public List<Participant> GetTeam(int projectId)
        {
            return DbManager.ExecuteList(
                new SqlQuery("projects_project_participant")
                    .Select("participant_id, security")
                    .Where("project_id", projectId)
                    .Where("removed", false)
                    .Where("tenant", Tenant))
                    .ConvertAll(r => new Participant(new Guid((string) r[0]), (ProjectTeamSecurity)Convert.ToInt32(r[1])));
        }

        public List<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to)
        {
            var query = new SqlQuery("projects_project_participant pp")
                .Select(PROJECT_COLUMNS.Select(x => "p." + x).ToArray())
                .Select("pp.participant_id", "pp.removed", "pp.created", "pp.updated")
                .LeftOuterJoin("projects_projects p", Exp.EqColumns("pp.project_id", "p.id") & Exp.EqColumns("p.tenant_id", "pp.tenant_id"))
                .Where("pp.tenant", Tenant)
                .Where(Exp.Between("pp.created", from, to) | Exp.Between("pp.updated", from, to));
            return DbManager.ExecuteList(query).Select(x =>ToParticipantFull(x)).ToList();
        }

        public void SetTeamSecurity(int projectId, Guid participantId, ProjectTeamSecurity teamSecurity)
        {
            DbManager.ExecuteNonQuery(
                new SqlUpdate("projects_project_participant")
                .Set("security", (int)teamSecurity)
                .Where("tenant", Tenant)
                .Where("project_id", projectId)
                .Where("participant_id", participantId.ToString()));

            lock (teamCache)
            {
                var key = string.Format("{0}|{1}", projectId, participantId);
                var item = teamCache.Get(key);
                if (item != null) teamCache[key].Security = teamSecurity;
            }
        }

        public ProjectTeamSecurity GetTeamSecurity(int projectId, Guid participantId)
        {
            return GetTeamItemFromCacheOrLoad(projectId, participantId).Security;
        }

        private TeamCacheItem GetTeamItemFromCacheOrLoad(int projectId, Guid participantId)
        {
            var key = string.Format("{0}|{1}", projectId, participantId);
            lock (teamCache)
            {
                var item = teamCache.Get(key);
                if (item != null) return item;
            }
            var result = DbManager.ExecuteList(
                new SqlQuery("projects_project_participant")
                .Select("security")
                .Where("project_id", projectId)
                .Where("participant_id", participantId.ToString())
                .Where("tenant", Tenant)
                .Where("removed", false));
            lock (teamCache)
            {
                var item = new TeamCacheItem(0 < result.Count, 0 < result.Count ? (ProjectTeamSecurity)Convert.ToInt32(result[0][0]) : ProjectTeamSecurity.None);
                teamCache.Add(key, item);
                return item;
            }
        }


        public static ParticipantFull ToParticipantFull(object[] x)
        {
            int offset = PROJECT_COLUMNS.Count();
            return new ParticipantFull(new Guid((string)x[0 + offset]))
            {
                Project = ToProject(x),
                Removed = Convert.ToBoolean(x[1 + offset]),
                Created = TenantUtil.DateTimeFromUtc((DateTime)x[2 + offset]),
                Updated = TenantUtil.DateTimeFromUtc((DateTime)x[3 + offset])
            };
        }

        public static Project ToProject(object[] r)
        {
            return new Project
            {
                ID = Convert.ToInt32(r[0]),
                Title = (string)r[1],
                Description = (string)r[2],
                Status = (ProjectStatus)Convert.ToInt32(r[3]),
                Responsible = ToGuid(r[4]),
                Private = Convert.ToBoolean(r[5]),
                CreateBy = ToGuid(r[6]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[7]),
                LastModifiedBy = ToGuid(r[8]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[9]),
            };
        }

        private class TeamCacheItem
        {
            public bool InTeam { get; set; }

            public ProjectTeamSecurity Security { get; set; }

            public TeamCacheItem(bool inteam, ProjectTeamSecurity security)
            {
                InTeam = inteam;
                Security = security;
            }
        }


        internal List<Project> GetProjects(Exp where)
        {
            return DbManager
             .ExecuteList(Query(PROJECT_TABLE + " p").Select(PROJECT_COLUMNS).Where(where))
             .ConvertAll(r => ToProject(r));
        }
    }
}