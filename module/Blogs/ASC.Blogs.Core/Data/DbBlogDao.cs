using System;
using System.Collections.Generic;
using ASC.Blogs.Core.Domain;
using ASC.Common.Data;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Blogs.Core.Data
{
    public class DbBlogDao : DbDao, IBlogDao
    {
        public DbBlogDao(DbManager db, int tenant)
            : base(db, tenant)
        {
        }


        #region IBlogDao Members

        public List<Blog> Select(long? id, Guid? userId, Guid? groupId)
        {
            var q = Query("blogs_blogs")
                .Select("id", "name", "user_id", "group_id");

            if (id.HasValue) q.Where(Exp.Eq("id", id.Value));
            if (userId.HasValue) q.Where(Exp.Eq("user_id", userId.Value.ToString()));
            if (groupId.HasValue) q.Where(Exp.Eq("group_id", groupId.Value.ToString()));

            return Db.ExecuteList<Blog>(q, RowMappers.ToBlog);
        }

        public void SaveBlog(Blog blog)
        {
            var query = Insert("blogs_blogs")
                .InColumns("id", "name", "user_id", "group_id")
                .Values(blog.BlogID, blog.Name, blog.UserID.ToString(), blog.GroupID.ToString())
                .Identity(1, 0L, true);

            blog.BlogID = Db.ExecuteScalar<int>(query);
        }


        public int GetCount(Guid? userId, Guid? groupId)
        {
            var query = Query("blogs_blogs").SelectCount();

            if (userId.HasValue) query.Where("user_id", userId.Value.ToString());
            if (groupId.HasValue) query.Where("group_id", groupId.Value.ToString());

            return Db.ExecuteScalar<int>(query);
        }

        #endregion
    }
}
