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
using ASC.Projects.Core.Services.NotifyService;

namespace ASC.Projects.Data.DAO
{
    internal class CachedMessageDao : MessageDao
    {
        private readonly HttpRequestDictionary<Message> _messageCache = new HttpRequestDictionary<Message>("message");

        public CachedMessageDao(string dbId, int tenant) : base(dbId, tenant)
        {
        }

        public override Message GetById(int id)
        {
            return _messageCache.Get(id.ToString(), () => GetBaseById(id));
        }

        private Message GetBaseById(int id)
        {
            return base.GetById(id);
        }

        public override Message Save(Message msg)
        {
            if (msg != null)
            {
                ResetCache(msg.ID);
            }
            return base.Save(msg);
        }

        public override void Delete(int id)
        {
            ResetCache(id);
            base.Delete(id);
        }

        private void ResetCache(int messageId)
        {
            _messageCache.Reset(messageId.ToString());
        }
    }

    class MessageDao : BaseDao, IMessageDao
    {
        private readonly string table = "projects_messages";


        public MessageDao(string dbId, int tenant)
            : base(dbId, tenant)
        {
        }

        public List<Message> GetByProject(int projectId)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.project_id", projectId).OrderBy("t.create_on", false))
                .ConvertAll(r => ToMessage(r));
        }

        public List<Message> GetMessages(int startIndex, int max)
        {
            var query = CreateQuery()
                .OrderBy("t.create_on", false)
                .SetFirstResult(startIndex)
                .SetMaxResults(max);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToMessage(r));
        }

        public List<Message> GetRecentMessages(int offset, int max, params int[] projects)
        {
            var query = CreateQuery()
                .SetFirstResult(offset)
                .OrderBy("t.create_on", false)
                .SetMaxResults(max);
            if (projects != null && 0 < projects.Length)
            {
                query.Where(Exp.In("t.project_id", projects));
            }
            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => ToMessage(r));
        }

        public List<Message> GetByFilter(TaskFilter filter)
        {
            var query = CreateQuery();

            if (filter.Max > 0 && filter.Max < 150000)
            {
                query.SetFirstResult((int)filter.Offset);
                query.SetMaxResults((int)filter.Max * 2);
            }

            if(filter.Follow)
            {
                var objects = new List<String>(NotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(NotifyConstants.Event_NewCommentForMessage, NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString())));

                if (filter.ProjectIds.Count != 0)
                {
                    objects = objects.Where(r => r.Split('_')[2] == filter.ProjectIds[0].ToString()).ToList();
                }

                var ids = objects.Select(r => r.Split('_')[1]).ToArray();
                query.Where(Exp.In("t.id", ids));
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
                query.InnerJoin("projects_project_tag pt", Exp.EqColumns("pt.project_id", "t.project_id"));
                query.Where("pt.tag_id", filter.TagId);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["Message"];
                sortColumns.Remove(filter.SortBy);

                query.OrderBy("t." + filter.SortBy, filter.SortOrder);

                foreach (var sort in sortColumns.Keys)
                {
                    query.OrderBy("t." + sort, sortColumns[sort]);
                }
            }

            if (filter.UserId != Guid.Empty)
            {
                query.Where("t.create_by", filter.UserId);
            }

            if (filter.DepartmentId != Guid.Empty)
            {
                query.InnerJoin("core_usergroup cug", Exp.Eq("cug.removed", false) & Exp.EqColumns("cug.userid", "t.create_by") & Exp.EqColumns("cug.tenant", "t.tenant_id"));
                query.Where("cug.groupid", filter.DepartmentId);
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Between("t.create_on", filter.FromDate, filter.ToDate.AddDays(1)));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query.Where(Exp.Like("t.title", filter.SearchText, SqlLike.AnyWhere));
            }

            return DbManager.ExecuteList(query).ConvertAll(r => ToMessage(r));
        }

        public List<Message> GetUpdates(DateTime from, DateTime to)
        {
            return GetMessages(Exp.Between("t.create_on", from, to) | Exp.Between("t.last_modified_on", from, to));
        }

        public virtual Message GetById(int id)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where("t.id", id))
                .ConvertAll(r => ToMessage(r))
                .SingleOrDefault();
        }

        public bool IsExists(int id)
        {
            var count = DbManager.ExecuteScalar<long>(Query(table).SelectCount().Where("id", id));
            return 0 < count;
        }

        public virtual Message Save(Message msg)
        {
            var insert = Insert(table)
                .InColumnValue("id", msg.ID)
                .InColumnValue("project_id", msg.Project != null ? msg.Project.ID : 0)
                .InColumnValue("title", msg.Title)
                .InColumnValue("create_by", msg.CreateBy.ToString())
                .InColumnValue("create_on", TenantUtil.DateTimeToUtc(msg.CreateOn))
                .InColumnValue("last_modified_by", msg.LastModifiedBy.ToString())
                .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(msg.LastModifiedOn))
                .InColumnValue("content", msg.Content)
                .Identity(1, 0, true);
            msg.ID = DbManager.ExecuteScalar<int>(insert);
            return msg;
        }

        public virtual void Delete(int id)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("projects_comments").Where("target_uniq_id", ProjectEntity.BuildUniqId<Message>(id)));
                DbManager.ExecuteNonQuery(Delete("projects_review_entity_info").Where("entity_uniqID", "Message_" + id));
                DbManager.ExecuteNonQuery(Delete(table).Where("id", id));

                tx.Commit();
            }
        }


        private SqlQuery CreateQuery()
        {
            return new SqlQuery(table + " t")
                .InnerJoin(ProjectDao.PROJECT_TABLE + " p", Exp.EqColumns("t.project_id", "p.id") & Exp.EqColumns("t.tenant_id", "p.tenant_id"))
                .Select(ProjectDao.PROJECT_COLUMNS.Select(c => "p." + c).ToArray())
                .Select("t.id", "t.title", "t.create_by", "t.create_on", "t.last_modified_by", "t.last_modified_on", "t.content")
                .GroupBy("t.id")
                .Where("t.tenant_id", Tenant);
        }

        private Message ToMessage(object[] r)
        {
            var offset = ProjectDao.PROJECT_COLUMNS.Length;
            return new Message()
            {
                Project = r[0] != null ? ProjectDao.ToProject(r) : null,
                ID = Convert.ToInt32(r[0 + offset]),
                Title = (string)r[1 + offset],
                CreateBy = ToGuid(r[2 + offset]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[3 + offset]),
                LastModifiedBy = ToGuid(r[4 + offset]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[5 + offset]),
                Content = (string)r[6 + offset]
            };
        }


        internal List<Message> GetMessages(Exp where)
        {
            return DbManager
                .ExecuteList(CreateQuery().Where(where))
                .ConvertAll(r => ToMessage(r));
        }
    }
}
