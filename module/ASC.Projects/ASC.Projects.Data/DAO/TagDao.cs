using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Projects.Core.DataInterfaces;

namespace ASC.Projects.Data.DAO
{
    class TagDao : BaseDao, ITagDao
    {
        public TagDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }

        public Dictionary<int, string> GetTags()
        {
            return DbManager
                .ExecuteList(GetTagQuery())
                .ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString().HtmlEncode());
        }

        public Dictionary<int, string> GetTags(string prefix)
        {
            var query = GetTagQuery()
                .Where(Exp.Like("title", prefix, SqlLike.StartWith));

            return DbManager
                .ExecuteList(query)
                .ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
        }

        public int[] GetTagProjects(string tag)
        {
            var query = new SqlQuery("projects_project_tag")
                .Select("project_id")
                .InnerJoin("projects_tags", Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("lower(title)", tag.ToLower()))
                .Where("tenant_id", Tenant);

            return DbManager
                .ExecuteList(query)
                .ConvertAll(r => Convert.ToInt32(r[0]))
                .ToArray();
        }

        public Dictionary<int, string> GetProjectTags(int projectId)
        {
            var query = GetTagQuery()
                .InnerJoin("projects_project_tag", Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("project_id", projectId));

            return DbManager
                .ExecuteList(query)
                .ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
        }

        public Dictionary<int, string> GetProjectRequestTags(int requestId)
        {
            var query = GetTagQuery()
                .InnerJoin("projects_project_tag_change_request", Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("project_id", requestId));

            return DbManager
                .ExecuteList(query)
                .ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
        }

        public void SetProjectTags(int projectId, string[] tags)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(new SqlDelete("projects_project_tag").Where("project_id", projectId));
                DbManager.ExecuteNonQuery(Delete("projects_tags").Where(!Exp.In("id", new SqlQuery("projects_project_tag").Select("tag_id"))));

                foreach (var tag in tags)
                {
                    var tagId = DbManager.ExecuteScalar<int>(Query("projects_tags").Select("id").Where("lower(title)", tag.ToLower()));
                    if (tagId == 0)
                    {
                        tagId = DbManager.ExecuteScalar<int>(
                            Insert("projects_tags")
                            .InColumnValue("id", 0)
                            .InColumnValue("title", tag)
                            .InColumnValue("last_modified_by", DateTime.UtcNow)
                            .Identity(1, 0, true));
                    }
                    DbManager.ExecuteNonQuery(new SqlInsert("projects_project_tag", true).InColumnValue("tag_id", tagId).InColumnValue("project_id", projectId));
                }
                tx.Commit();
            }
        }

        public void SetProjectRequestTags(int requestId, string[] tags)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(new SqlDelete("projects_project_tag_change_request").Where("project_id", requestId));
                foreach (var tag in tags)
                {
                    var tagId = DbManager.ExecuteScalar<int>(Query("projects_tags").Select("id").Where("lower(title)", tag.ToLower()));
                    if (tagId == 0)
                    {
                        tagId = DbManager.ExecuteScalar<int>(
                            Insert("projects_tags")
                            .InColumnValue("id", 0)
                            .InColumnValue("title", tag)
                            .InColumnValue("last_modified_by", DateTime.UtcNow)
                            .Identity(1, 0, true));
                    }
                    DbManager.ExecuteNonQuery(new SqlInsert("projects_project_tag_change_request").InColumnValue("tag_id", tagId).InColumnValue("project_id", requestId));
                }
                tx.Commit();
            }
        }


        private SqlQuery GetTagQuery()
        {
            return Query("projects_tags")
                .Select("id", "title")
                .OrderBy("title", true);
        }
    }
}
