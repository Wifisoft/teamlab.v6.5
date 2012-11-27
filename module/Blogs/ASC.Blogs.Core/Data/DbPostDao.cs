using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Blogs.Core.Domain;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;

namespace ASC.Blogs.Core.Data
{
    public class DbPostDao : DbDao, IPostDao
    {
        public DbPostDao(DbManager db, int tenant) : base(db, tenant) { }

        #region IPostDao

        private Exp GetWhere(Guid? id, long? blogId, Guid? userId, string tag)
        {
            var where = Exp.Empty;
            if (id.HasValue) where &= Exp.Eq("id", id.Value.ToString());
            if (blogId.HasValue) where &= Exp.Eq("blog_id", blogId.Value);
            if (userId.HasValue) where &= Exp.Eq("created_by", userId.Value.ToString());
            if (tag != null) where &= Exp.In("id", Query("blogs_tags").Select("post_id").Where("name", tag));
            return where;
        }

        public int GetCount(Guid? id, long? blogId, Guid? userId)
        {
            return GetCount(id, blogId, userId, null);
        }

        public int GetCount(Guid? id, long? blogId, Guid? userId, string tag)
        {
            var where = GetWhere(id, blogId, userId, tag);
            var query = Query("blogs_posts").SelectCount().Where(where);
            return Db.ExecuteScalar<int>(query);
        }

        public List<Post> Select(Guid? id, long? blogId, Guid? userId, string tag, bool withContent, bool asc, int? from, int? count, bool fillTags, bool withCommentsCount)
        {
            var where = GetWhere(id, blogId, userId, tag);
            var query = Query("blogs_posts")
                .Select("id", "title", "created_by", "created_when", "blog_id")
                .Where(where);

            if (withContent) query.Select("content");
            if (count.HasValue) query.SetMaxResults(count.Value);
            if (from.HasValue) query.SetFirstResult(from.Value);

            query.OrderBy("created_when", asc);

            var posts = Db.ExecuteList<Post>(query, RowMappers.ToPost);

            if (posts.Count > 0)
            {
                var pids = posts.ConvertAll(p => p.ID.ToString());
                var postQ = Query("blogs_posts").Select("id").Where(where).OrderBy("created_when", asc);
                if (count.HasValue) postQ.SetMaxResults(count.Value);
                if (from.HasValue) postQ.SetFirstResult(from.Value);
                var postsFilter = Exp.In("post_id", pids);

                if (fillTags)
                {
                    var tagQuery = Query("blogs_tags").Where(postsFilter).Select("name", "post_id");
                    var tlist = Db.ExecuteList<Tag>(tagQuery, RowMappers.ToTag);
                    foreach (var post in posts)
                    {
                        post.TagList = tlist.FindAll(t => t.PostId == post.ID);
                    }
                }
            }
            return posts;
        }

        public List<Post> Select(Guid? id, long? blogId, Guid? userId, bool withContent, bool asc, int? from, int? count, bool fillTags, bool withCommentsCount)
        {
            return Select(id, blogId, userId, null, withContent, true, null, null, fillTags, withCommentsCount);
        }

        public List<Post> Select(Guid? id, long? blogId, Guid? userId, bool withContent, bool fillTags, bool withCommentsCount)
        {
            return Select(id, blogId, userId, withContent, true, null, null, fillTags, withCommentsCount);
        }

        public List<Guid> SearchPostsByWord(string word)
        {
            var q1 = Query("blogs_posts")
                .Select("id")
                .Where(Exp.Like("lower(title)", word, SqlLike.StartWith) | Exp.Like("lower(title)", " " + word) | Exp.Like("lower(content)", word, SqlLike.StartWith) | Exp.Like("lower(content)", " " + word));

            var q2 = Query("blogs_tags")
                .Select("post_id")
                .Where(Exp.Like("lower(name)", word, SqlLike.StartWith) | Exp.Like("lower(name)", " " + word));

            return Db
                    .ExecuteList(q1.Union(q2))
                    .ConvertAll(row => new Guid(Convert.ToString(row[0])));
        }

        public List<Tag> SelectTags(Guid? postId)
        {
            var query = Query("blogs_tags").Select("name", "post_id");
            if (postId.HasValue) query.Where("post_id", postId.Value.ToString());
            return Db.ExecuteList<Tag>(query, RowMappers.ToTag);
        }

        public List<string> GetTags(string like, int limit)
        {
            var query = Query("blogs_tags").Select("name").Distinct();
            query.Where(Exp.Like("name", like, SqlLike.StartWith)).SetMaxResults(limit);
            return Db.ExecuteList<string>(query, RowMappers.ToString);
        }


        public List<Post> GetPosts(List<Guid> ids, bool withContent, bool withTags)
        {
            var postIds = ids.ConvertAll(id => id.ToString());
            var select = Query("blogs_posts")
                .Select("id", "title", "created_by", "created_when", "blog_id")
                .Where(Exp.In("id", postIds));

            if (withContent) select.Select("content");

            var posts = Db.ExecuteList<Post>(select, RowMappers.ToPost);

            if (posts.Count > 0)
            {
                if (withTags)
                {
                    var tagQuery = Query("blogs_tags").Where(Exp.In("post_id", postIds)).Select("name", "post_id");
                    var tlist = Db.ExecuteList<Tag>(tagQuery, RowMappers.ToTag);
                    foreach (var post in posts)
                    {
                        post.TagList = tlist.FindAll(t => t.PostId == post.ID);
                    }
                }
            }

            return posts;
        }

        public void SavePost(Post post)
        {
            bool isInsert = false;
            if (post.ID == Guid.Empty)
            {
                post.ID = Guid.NewGuid();
                isInsert = true;
            }

            using (var tx = Db.BeginTransaction())
            {
                var tagsForSave = new List<Tag>(post.TagList);
                var tagsForDelete = new List<Tag>();
                if (!isInsert)
                {
                    var saved_tags = SelectTags(post.ID);
                    tagsForDelete.AddRange(saved_tags);
                    tagsForDelete.RemoveAll(_1 => tagsForSave.Exists(_2 => String.Equals(_1.Content, _2.Content, StringComparison.CurrentCultureIgnoreCase)));
                    tagsForSave.RemoveAll(_1 => saved_tags.Exists(_2 => String.Equals(_1.Content, _2.Content, StringComparison.CurrentCultureIgnoreCase)));
                }

                if (tagsForDelete.Count > 0)
                {
                    var deleteq = Delete("blogs_tags")
                        .Where("post_id", post.ID.ToString())
                        .Where(Exp.In("name", tagsForDelete.ConvertAll(_ => (object)_.Content)));

                    Db.ExecuteNonQuery(deleteq);
                }

                if (tagsForSave.Count > 0)
                {
                    foreach (var tag in tagsForSave)
                    {
                        var insertq = Insert("blogs_tags")
                            .InColumns("name", "post_id")
                            .Values(tag.Content, post.ID.ToString());
                        Db.ExecuteNonQuery(insertq);
                    }
                }

                var queryi = Insert("blogs_posts")
                           .InColumns("id", "title", "content", "created_by", "created_when", "blog_id")
                           .Values(post.ID.ToString(), post.Title, post.Content, post.UserID.ToString(), ASC.Core.Tenants.TenantUtil.DateTimeToUtc(post.Datetime), post.BlogId);

                Db.ExecuteNonQuery(queryi);

                tx.Commit();
            }
        }

        public void DeletePost(Guid postId)
        {
            using (var tx = Db.BeginTransaction())
            {
                Db.ExecuteNonQuery(Delete("blogs_posts").Where("id", postId.ToString()));
                Db.ExecuteNonQuery(Delete("blogs_comments").Where("post_id", postId.ToString()));
                Db.ExecuteNonQuery(Delete("blogs_tags").Where("post_id", postId.ToString()));
                Db.ExecuteNonQuery(Delete("blogs_reviewposts").Where("post_id", postId.ToString()));

                tx.Commit();
            }

        }

        public List<Comment> GetComments(Guid postId)
        {
            var query = Query("blogs_comments")
                .Select("id", "post_id", "parent_id", "content", "created_by", "created_when", "inactive")
                .Where("post_id", postId.ToString())
                .OrderBy("created_when", true);

            return Db.ExecuteList<Comment>(query, RowMappers.ToComment);
        }

        public Comment GetCommentById(Guid commentId)
        {
            var select = Query("blogs_comments")
                .Select("id", "post_id", "parent_id", "content", "created_by", "created_when", "inactive")
                .Where("id", commentId.ToString());

            var list = Db.ExecuteList<Comment>(select, RowMappers.ToComment);
            return list.Count > 0 ? list[0] : null;
        }

        public void SaveComment(Comment comment)
        {
            bool IsInsert = (comment.ID == Guid.Empty);
            if (IsInsert) comment.ID = Guid.NewGuid();

            var query = Insert("blogs_comments")
                .InColumns("id", "post_id", "parent_id", "content", "created_by", "created_when", "inactive")
                .Values(comment.ID, comment.PostId, comment.ParentId, comment.Content, comment.UserID.ToString(), ASC.Core.Tenants.TenantUtil.DateTimeToUtc(comment.Datetime), comment.Inactive ? 1 : 0);

            using (var tx = Db.BeginTransaction())
            {
                Db.ExecuteNonQuery(query);

                if (IsInsert)
                {
                    var update = Update("blogs_posts")
                        .Set("LastCommentId", comment.ID.ToString())
                        .Where("id", comment.PostId.ToString());

                    Db.ExecuteNonQuery(update);
                }

                tx.Commit();
            }
        }

        public void UpdateCommentText(Guid commentId, string newText)
        {
            var update = Update("blogs_comments")
                .Set("content", newText)
                .Where("Id", commentId.ToString());
            Db.ExecuteNonQuery(update);
        }


        public List<TagStat> GetTagStat(int? top)
        {
            var query = Query("blogs_tags")
                        .Select("name", "count(*) as cnt")
                        .OrderBy("cnt", false)
                        .GroupBy("name");

            if (top.HasValue) query.SetMaxResults(top.Value);

            var result = Db.ExecuteList<TagStat>(query, (r => new TagStat { Name = r.Get<string>(0), Count = r.Get<int>(1) }));

            return result;
        }

        public List<int> GetCommentsCount(List<Guid> postsIds)
        {
            var select = Query("blogs_comments")
                .Select("post_id", "count(*)")
                .Where(Exp.In("post_id", postsIds.ConvertAll(id => id.ToString())))
                .Where(Exp.Like("inactive", "0"))
                .GroupBy("post_id");

            var list = Db.ExecuteList(select);

            var result = new List<int>(postsIds.Count);
            for (int i = 0; i < postsIds.Count; i++)
            {
                var finded = list.Find(row => new Guid(row[0].ToString()) == postsIds[i]);
                if (finded == null) result.Add(0);
                else result.Add(Convert.ToInt32(finded[1]));
            }
            return result;
        }

        public List<Typle<Guid, int>> GetPostsCountByAuthor(Guid? authorId)
        {
            var select = Query("blogs_posts")
                .Select("created_by").SelectCount()
                .GroupBy("created_by");
            if (authorId.HasValue) select.Where("created_by", authorId.Value.ToString());

            return Db.ExecuteList<Typle<Guid, int>>(select, r => new Typle<Guid, int>() { Value1 = r.Get<Guid>(0), Value2 = r.Get<int>(1) });
        }

        public List<Typle<Guid, int>> GetCommentsCountByAuthorPosts(Guid? authorId)
        {
            var select = Query("blogs_posts")
                               .Select("blogs_posts.created_by").SelectCount()
                               .InnerJoin("blogs_comments", Exp.EqColumns("blogs_posts.id", "blogs_comments.post_id"))
                               .Where(GetTenantColumnName("blogs_comments"), Tenant)
                               .GroupBy("blogs_posts.created_by");

            if (authorId.HasValue) select.Where("Blogs_.created_by", authorId.Value.ToString());

            return Db.ExecuteList<Typle<Guid, int>>(select, (row) => new Typle<Guid, int>() { Value1 = row.Get<Guid>(0), Value2 = row.Get<int>(1) });
        }

        public List<Typle<Guid, int>> GetReviewCountByAuthorPosts(Guid? authorId)
        {
            if (authorId.HasValue) throw new NotSupportedException();

            var sql = @"select blogs_posts.created_by,sum(t2.cnt) from blogs_posts join (
select post_id,count(*) as cnt from blogs_reviewposts where Tenant = @tenant group by post_id) t2 
on blogs_posts.id=t2.post_id
where blogs_posts.Tenant=@tenant 
group by blogs_posts.created_by";
            var cmd = Db.Connection.CreateCommand(sql)
                            .AddParameter("tenant", Tenant);

            var rows = cmd.ExecuteList();

            return rows.ConvertAll(row => new Typle<Guid, int>() { Value1 = new Guid(Convert.ToString(row[0])), Value2 = Convert.ToInt32(row[1]) });
        }

        public Dictionary<Guid, Typle<int, int>> GetCommentsStatistic(ICollection<Guid> posts, Guid forUser)
        {
            var q = new SqlQuery("blogs_comments t1")
                .LeftOuterJoin("blogs_reviewposts t2", Exp.EqColumns("t1.post_id", "t2.post_id") & Exp.Sql("t1.created_when>t2.timestamp") & Exp.Eq("t2.reviewed_by", forUser.ToString()) & Exp.Eq("t2.Tenant", Tenant))
                .Select("t1.post_id")
                .Select("count(*) as all_cnt")
                .Select("count(t2.post_id) as notseen_cnt ")
                .Where("t1.tenant", Tenant)
                .Where("t1.inactive", 0)
                .Where(Exp.In("t1.post_id", posts.Select(g => g.ToString()).ToArray()))
                .GroupBy("t1.post_id");

            var rows = Db.ExecuteList(q);
            var result = new Dictionary<Guid, Typle<int, int>>(rows.Count);
            foreach (var row in rows)
            {
                var postId = new Guid(row[0].ToString());
                var stat = new Typle<int, int>();

                stat.Value1 = Convert.ToInt32(row[1]);
                stat.Value2 = Convert.ToInt32(row[2]);
                result.Add(postId, stat);
            }

            return result;

        }

        public List<Typle<Comment, Post>> GetLastCommentedPosts(int? top)
        {
            var lastCommentsQuery = Query("blogs_posts")
                .Select("blogs_comments.id", "blogs_comments.content", "blogs_comments.created_by", "blogs_comments.post_id", "blogs_posts.Title")
                .InnerJoin("blogs_comments", Exp.EqColumns("LastCommentId", "blogs_comments.Id"))
                .Where(GetTenantColumnName("blogs_comments"), Tenant)
                .OrderBy("blogs_comments.created_when", false);

            if (top.HasValue) lastCommentsQuery.SetMaxResults(top.Value);

            var rows = Db.ExecuteList(lastCommentsQuery);
            var result = new List<Typle<Comment, Post>>(rows.Count);
            foreach (var row in rows)
            {
                result.Add(new Typle<Comment, Post>()
                        {
                            Value1 = new Comment()
                            {
                                ID = new Guid(row[0].ToString()),
                                Content = Convert.ToString(row[1]),
                                UserID = new Guid(row[2].ToString()),
                            },
                            Value2 = new Post()
                            {
                                ID = new Guid(row[3].ToString()),
                                Title = Convert.ToString(row[4])
                            }
                        }
                  );
            }

            return result;
        }

        public void SavePostReview(Guid userId, Guid postId, DateTime datetime)
        {
            var insert = Insert("blogs_reviewposts")
                .InColumns("post_id", "reviewed_by", "timestamp")
                .Values(postId.ToString(), userId.ToString(), ASC.Core.Tenants.TenantUtil.DateTimeToUtc(datetime));
            Db.ExecuteNonQuery(insert);
        }

        public IEnumerable<Post> GetPostsByDate(DateTime @from, DateTime to)
        {
            var select = Query("blogs_posts")
                .Select("id", "title", "created_by", "created_when", "blog_id", "LastModified")
                .Where(Exp.Between("created_when", from, to) | Exp.Between("LastModified", from, to));
            return Db.ExecuteList<Post>(select, (x) => { var post = RowMappers.ToPost(x);
                                                           post.Updated = x.Get<DateTime>("LastModified");
                                                           return post;
            });
        }

        public IEnumerable<Comment> GetCommentsByDate(DateTime @from, DateTime to, Guid forUser)
        {
            var query = new SqlQuery("blogs_comments t1")
                .Select("t1.id", "t1.post_id", "t1.parent_id", "t1.content", "t1.created_by", "t1.created_when", "t1.inactive", "t2.reviewed_by")
                .LeftOuterJoin("blogs_reviewposts t2", Exp.EqColumns("t1.post_id", "t2.post_id") & Exp.Sql("t1.created_when>t2.timestamp") & Exp.Eq("t2.reviewed_by", forUser.ToString()) & Exp.Eq("t2.Tenant", Tenant))
                .Where(Exp.Between("t1.created_when", from, to))
                .OrderBy("t1.created_when", true);
            return Db.ExecuteList(query, (x) =>
            {
                var comment = RowMappers.ToComment(x);
                comment.IsReaded = x.Get<string>("reviewed_by") == forUser.ToString();
                return comment;
            });
        }

        #endregion
    }
}
