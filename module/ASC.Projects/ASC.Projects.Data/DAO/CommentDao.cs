using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Projects.Data.DAO
{
    class CommentDao : BaseDao, ICommentDao
    {
        private readonly string[] columns = new[] { "id", "target_uniq_id", "content", "inactive", "create_by", "create_on", "parent_id" };


        public CommentDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }


        public List<Comment> GetAll(DomainObject<int> target)
        {
            return DbManager
                .ExecuteList(
                    Query("projects_comments")
                    .Select(columns)
                    .Where("target_uniq_id", target.UniqID))
                .ConvertAll(r => ToComment(r))
                .OrderBy(c => c.CreateOn)
                .ToList();
        }

        public Comment GetById(Guid id)
        {
            return DbManager
                .ExecuteList(Query("projects_comments").Select(columns).Where("id", id.ToString()))
                .ConvertAll(r => ToComment(r))
                .SingleOrDefault();
        }

        public Comment GetLast(DomainObject<Int32> target)
        {
            return DbManager
                .ExecuteList(
                    Query("projects_comments")
                    .Select(columns)
                    .Where("target_uniq_id", target.UniqID)
                    .Where("inactive", false)
                    .OrderBy("create_on", false)
                    .SetMaxResults(1))
                .ConvertAll(r => ToComment(r))
                .SingleOrDefault();
        }

        public List<int> GetCommentsCount(List<ProjectEntity> targets)
        {
            var pairs =
                DbManager
                    .ExecuteList(
                        Query("projects_comments")
                        .Select("target_uniq_id", "count(*)")
                        .Where(Exp.In("target_uniq_id", targets.ConvertAll(target => target.UniqID)))
                        .Where("inactive", false)
                        .GroupBy(1)
                    ).ConvertAll(r => new object[] { Convert.ToString(r[0]), Convert.ToInt32(r[1]) });

            return targets.ConvertAll(
                        target =>
                        {
                            var pair = pairs.Find(p => String.Equals(Convert.ToString(p[0]), target.UniqID));
                            if (pair == null)
                                return 0;
                            else
                                return Convert.ToInt32(pair[1]);

                        });
        }

        public List<CommentUpdate> GetUpdates(DateTime from, DateTime to)
        {
            //splitting target_uniq_id to targetTable and targetId
            SqlQuery splitUniqId = Query("projects_comments as c")
                .Select(columns.Select(x=>"c." + x).ToArray())
                .Select("c.tenant_id")
                .Select("SUBSTRING_INDEX(c.target_uniq_id, '_', 1) as targetTable")
                .Select("SUBSTRING_INDEX(c.target_uniq_id, '_', -1) as targetId")
                .Where(Exp.Between("c.create_on", from, to) & Exp.Eq("c.tenant_id", Tenant));

            var query = new SqlQuery()
                .Select(columns.Select(x => "spl." + x).ToArray())
                .Select("m.id", "m.title", "m.project_id")
                .Select("t.id", "t.title", "t.project_id")
                .Select("msg.id", "msg.title", "msg.project_id")
                .From(splitUniqId, "spl")
                .LeftOuterJoin("projects_milestones as m", Exp.Eq("spl.targetTable", "Milestone") & Exp.EqColumns("spl.targetId", "m.id") & Exp.Eq("m.tenant_id", Tenant))
                .LeftOuterJoin("projects_tasks as t", Exp.Eq("spl.targetTable", "Task") & Exp.EqColumns("spl.targetId", "t.id") & Exp.Eq("t.tenant_id", Tenant))
                .LeftOuterJoin("projects_messages as msg", Exp.Eq("spl.targetTable", "Message") & Exp.EqColumns("spl.targetId", "msg.id") & Exp.Eq("msg.tenant_id", Tenant));
                
            return DbManager.ExecuteList(query)
                .Select(x=> ToCommentUpdate(x)).ToList();
        }

        public int Count(DomainObject<Int32> target)
        {
            return DbManager.ExecuteScalar<int>(
                Query("projects_comments")
                .SelectCount()
                .Where("target_uniq_id", target.UniqID)
                .Where("inactive", false));
        }


        public Comment Save(Comment comment)
        {
            if (comment.ID == default(Guid)) comment.ID = Guid.NewGuid();

            var insert = Insert("projects_comments")
                .InColumns(columns)
                .Values(
                    comment.ID,
                    comment.TargetUniqID,
                    comment.Content,
                    comment.Inactive,
                    comment.CreateBy.ToString(),
                    TenantUtil.DateTimeToUtc(comment.CreateOn),
                    comment.Parent.ToString());
            DbManager.ExecuteNonQuery(insert);
            return comment;
        }

        public void Delete(Guid id)
        {
            DbManager.ExecuteNonQuery(Delete("projects_comments").Where("id", id.ToString()));
        }


        private Comment ToComment(object[] r)
        {
            return new Comment()
            {
                ID = ToGuid(r[0]),
                TargetUniqID = (string)r[1],
                Content = (string)r[2],
                Inactive = Convert.ToBoolean(r[3]),
                CreateBy = ToGuid(r[4]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[5]),
                Parent = ToGuid(r[6]),
            };
        }

        private CommentUpdate ToCommentUpdate(object[] r)
        {
            int offset = columns.Count();
            var upd = new CommentUpdate();
            upd.Comment = ToComment(r);
            if (r[0 + offset] != null)
            {
                upd.CommentedId = Convert.ToInt32(r[0 + offset]);
                upd.CommentedTitle = Convert.ToString(r[1 + offset]);
                upd.ProjectId = Convert.ToInt32(r[2 + offset]);
                upd.CommentedType = EntityType.Milestone;
            }
            else if (r[2 + offset] != null)
            {
                upd.CommentedId = Convert.ToInt32(r[3 + offset]);
                upd.CommentedTitle = Convert.ToString(r[4 + offset]);
                upd.ProjectId = Convert.ToInt32(r[5 + offset]);
                upd.CommentedType = EntityType.Task;
            }
            else
            {
                upd.CommentedId = Convert.ToInt32(r[6 + offset]);
                upd.CommentedTitle = Convert.ToString(r[7 + offset]);
                upd.ProjectId = Convert.ToInt32(r[8 + offset]);
                upd.CommentedType = EntityType.Message;
            }
            return upd;
        }
    }
}
