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
    class EventDao : BaseDao, IEventDao
    {
        private readonly string table = "projects_events";


        public EventDao(string dbId, int tenant)
            : base(dbId, tenant)
        {
        }


        public List<Event> GetByProject(int projectId)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.project_id", projectId).OrderBy("t.title", true))
                .ConvertAll(r => ToEvent(r));
        }

        public Event GetById(int id)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.id", id))
                .ConvertAll(r => ToEvent(r))
                .SingleOrDefault();
        }

        public Event Save(Event ev)
        {
            var insert = Insert(table)
                .InColumnValue("id", ev.ID)
                .InColumnValue("project_id", ev.Project != null ? ev.Project.ID : 0)
                .InColumnValue("title", ev.Title)
                .InColumnValue("create_by", ev.CreateBy.ToString())
                .InColumnValue("create_on", TenantUtil.DateTimeToUtc(ev.CreateOn))
                .InColumnValue("last_modified_by", ev.LastModifiedBy.ToString())
                .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(ev.LastModifiedOn))
                .InColumnValue("from_date", TenantUtil.DateTimeToUtc(ev.FromDate))
                .InColumnValue("to_date", TenantUtil.DateTimeToUtc(ev.ToDate))
                .Identity(1, 0, true);
            ev.ID = DbManager.ExecuteScalar<int>(insert);
            return ev;
        }

        public void Delete(int id)
        {
            DbManager.ExecuteNonQuery(Delete(table).Where("id", id));
        }


        private SqlQuery CreateQuery()
        {
            return new SqlQuery(table + " t")
                .LeftOuterJoin(ProjectDao.PROJECT_TABLE + " p", Exp.EqColumns("t.project_id", "p.id") & Exp.EqColumns("p.tenant_id", "t.tenant_id"))
                .Select(ProjectDao.PROJECT_COLUMNS.Select(c => "p." + c).ToArray())
                .Select("t.id", "t.title", "t.create_by", "t.create_on", "t.last_modified_by", "t.last_modified_on")
                .Select("t.from_date", "t.to_date")
                .Where("t.tenant_id", Tenant);
        }

        private Event ToEvent(object[] r)
        {
            var offset = ProjectDao.PROJECT_COLUMNS.Length;
            return new Event()
            {
                Project = r[0] != null ? ProjectDao.ToProject(r) : null,
                ID = Convert.ToInt32(r[0 + offset]),
                Title = (string)r[1 + offset],
                CreateBy = ToGuid(r[2 + offset]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[3 + offset]),
                LastModifiedBy = ToGuid(r[4 + offset]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[5 + offset]),
                FromDate = TenantUtil.DateTimeFromUtc((DateTime)r[6 + offset]),
                ToDate = TenantUtil.DateTimeFromUtc((DateTime)r[7 + offset]),
            };
        }
    }
}
