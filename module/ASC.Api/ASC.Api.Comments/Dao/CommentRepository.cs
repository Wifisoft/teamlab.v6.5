using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using ASC.Api.Comments.Model;
using ASC.Api.Employee;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Specific;

namespace ASC.Api.Comments.Dao
{
    internal class CommentRepository
    {
        private const string CommentTable = "comments";
        private const string CommentReadedTable = "comment_readed";

        private const string SelectFields =
            "id,parent_id,content,author,deleted,security_id,created,updated,comment_key";

        private readonly string _dbId;
        private readonly object _registerLock = new object();

        public CommentRepository(string dbId, ConnectionStringSettings connectionString)
        {
            _dbId = dbId;
            if (!DbRegistry.IsDatabaseRegistered(dbId))
            {
                lock (_registerLock)
                {
                    Thread.MemoryBarrier();
                    if (!DbRegistry.IsDatabaseRegistered(dbId))
                    {
                        DbRegistry.RegisterDatabase(dbId, connectionString);
                    }
                }
            }
        }

        private DbManager GetManager()
        {
            return new DbManager(_dbId);
        }


        public Comment Get(long id)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlQuery query = new SqlQuery(CommentTable).Select(SelectFields).Where("id", id);
                return dbMan.ExecuteList(query).Select(x => ToComment(x)).SingleOrDefault();
            }
        }

        public long GetCount(string key)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlQuery query = new SqlQuery(CommentTable).Select("count(id)").Where("comment_key", key);
                return dbMan.ExecuteScalar<long>(query);
            }
        }

        //NOTE: in tuple return first = total count, second = user readed count
        public CommentCount GetCountWithReaded(string key, Guid userId)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlQuery query = new SqlQuery(CommentTable).Select("count(id)", "count(user_id)")
                    .LeftOuterJoin(CommentReadedTable, Exp.EqColumns("id", "comment_id"))
                    .Where("comment_key", key);
                if (userId != Guid.Empty)
                    query = query.Where(Exp.Eq("user_id", userId.ToString()) | Exp.Eq("user_id", null));

                return
                    dbMan.ExecuteList(query).Select(x => new CommentCount(Convert.ToInt64(x[0]), Convert.ToInt64(x[1])))
                        .SingleOrDefault();
            }
        }

        public IEnumerable<Comment> GetAll(string key)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlQuery query = new SqlQuery(CommentTable).Select(SelectFields).Where("comment_key", key);
                return dbMan.ExecuteList(query).Select(x => ToComment(x));
            }
        }

        public IEnumerable<Comment> GetAll(string key, Guid userId)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlQuery query = new SqlQuery(CommentTable)
                    .LeftOuterJoin(CommentReadedTable, Exp.EqColumns("id", "comment_id"))
                    .Where(Exp.Eq("user_id", userId.ToString()) | Exp.Eq("user_id", null))
                    .Select(SelectFields + ",timestamp").Where("comment_key", key).OrderBy("created", true);
                return dbMan.ExecuteList(query).Select(x => ToComment(x));
            }
        }

        public void MarkAsReaded(string key, Guid userId)
        {
            using (DbManager dbMan = GetManager())
            {
                using (IDbTransaction transaction = dbMan.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    SqlInsert query = new SqlInsert(CommentReadedTable).IgnoreExists(true)
                        .InColumns("comment_id", "user_id")
                        .Values(
                            new SqlQuery(CommentTable).Select("id", string.Format("'{0}'", userId)).Where(
                                "comment_key", key));
                    dbMan.ExecuteNonQuery(query);
                    transaction.Commit();
                }
            }
        }

        public Comment SaveOrUpdate(Comment comment)
        {
            using (DbManager dbMan = GetManager())
            {
                using (IDbTransaction transaction = dbMan.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    SqlInsert query = new SqlInsert(CommentTable, true)
                        .Identity(0, (Int64) 0, true)
                        .InColumns(SelectFields.Split(','))
                        .Values(comment.Id, comment.ParentId, comment.Content, comment.Author.Id.ToString(),
                                comment.Deleted,
                                comment.SecurityId, comment.Created.Utc ?? DateTime.UtcNow, DateTime.UtcNow, comment.Key);
                    comment.Id = dbMan.ExecuteScalar<long>(query);
                    //Mark comment as readed
                    SqlInsert readedQuery =
                        new SqlInsert(CommentReadedTable).IgnoreExists(true).InColumns("comment_id", "user_id").Values(
                            comment.Id, comment.Author.Id.ToString());
                    dbMan.ExecuteNonQuery(readedQuery);
                    comment.Readed = DateTime.UtcNow;
                    transaction.Commit();
                }
            }
            return comment;
        }

        public Comment Delete(Comment comment)
        {
            Delete(comment.Id);
            comment.Deleted = true;
            return comment;
        }

        public void Delete(long id)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlUpdate query = new SqlUpdate(CommentTable).Where("id", id).Set("deleted", true);
                dbMan.ExecuteNonQuery(query);
            }
        }

        public void DeleteTree(string key)
        {
            using (DbManager dbMan = GetManager())
            {
                SqlDelete query = new SqlDelete(CommentTable).Where("comment_key", key);
                dbMan.ExecuteNonQuery(query);
            }
        }

        private static Comment ToComment(IList<object> row)
        {
            return new Comment
                       {
                           Id = Convert.ToInt64(row[0]),
                           ParentId = Convert.ToInt64(row[1]),
                           Content = (string) row[2],
                           Author = EmployeeWraper.Get(new Guid((string) row[3])),
                           Deleted = Convert.ToBoolean(row[4]),
                           SecurityId = (string) row[5],
                           Created = (ApiDateTime) Convert.ToDateTime(row[6]),
                           Updated = (ApiDateTime) Convert.ToDateTime(row[7]),
                           Key = (string) row[8],
                           Readed = row.Count == 10 && row[9] != null ? new DateTime?(Convert.ToDateTime(row[9])) : null
                       };
        }
    }
}