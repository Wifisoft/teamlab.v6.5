using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Notify.Config;
using ASC.Notify.Messages;
using log4net;

namespace ASC.Notify
{
    class DbWorker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotifyService));
        private readonly string dbid;
        private readonly object syncRoot = new object();


        public DbWorker()
        {
            var connectionString = NotifyServiceCfg.ConnectionString;
            dbid = connectionString.Name;
            if (!DbRegistry.IsDatabaseRegistered(dbid))
            {
                DbRegistry.RegisterDatabase(dbid, connectionString);
            }
        }

        public void CheckStates()
        {
            using (var db = GetDb())
            {
                var u = new SqlUpdate("notify_info").Set("state", 0).Where("state", 1);
                db.ExecuteNonQuery(u);
            }
        }

        public void AddToDbQueue(NotifyMessage m)
        {
            using (var db = GetDb())
            using (var tx = db.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var i = new SqlInsert("notify_queue")
                    .InColumns("notify_id", "tenant_id", "sender", "reciever", "subject", "content_type", "content", "sender_type", "creation_date", "reply_to")
                    .Values(0, m.Tenant, m.From, m.To, m.Subject, m.ContentType, m.Content, m.Sender, m.CreationDate, m.ReplyTo)
                    .Identity(0, 0, true);

                var id = db.ExecuteScalar<int>(i);

                i = new SqlInsert("notify_info")
                    .InColumns("notify_id", "state", "attempts", "modify_date")
                    .Values(id, 0, 0, DateTime.UtcNow);
                db.ExecuteNonQuery(i);

                tx.Commit();
            }
        }

        public IDictionary<int, NotifyMessage> GetLetters(int count)
        {
            lock (syncRoot)
            {
                var letters = new Dictionary<int, NotifyMessage>();
                using (var db = GetDb())
                using (var tx = db.BeginTransaction())
                {

                    var q = new SqlQuery("notify_queue q")
                        .InnerJoin("notify_info i", Exp.EqColumns("q.notify_id", "i.notify_id"))
                        .Select("q.notify_id", "q.tenant_id", "q.sender", "q.reciever", "q.subject", "q.content_type", "q.content", "q.sender_type", "q.creation_date", "q.reply_to")
                        .Where(Exp.Eq("i.state", 0) | (Exp.Eq("i.state", 3) & Exp.Lt("i.modify_date", DateTime.UtcNow - NotifyServiceCfg.AttemptsInterval)))
                        .OrderBy("i.notify_id", true)
                        .SetMaxResults(count);

                    var res = db.ExecuteList(q);
                    var tempQ = res.Select(p => (int)p[0]).ToList();
                    var u = new SqlUpdate("notify_info").Set("state", 1).Where(Exp.In("notify_id", tempQ));

                    foreach (var item in res)
                    {
                        var id = Convert.ToInt32(item[0]);
                        var m = new NotifyMessage
                        {
                            Tenant = Convert.ToInt32(item[1]),
                            From = (string)item[2],
                            To = (string)item[3],
                            Subject = (string)item[4],
                            ContentType = (string)item[5],
                            Content = (string)item[6],
                            Sender = (string)item[7],
                            CreationDate = Convert.ToDateTime(item[8]),
                            ReplyTo = (string)item[9],
                        };
                        letters.Add(id, m);
                    }

                    db.ExecuteNonQuery(u);
                    tx.Commit();
                }
                return letters;
            }
        }

        public void SetResult(int id, MailSendingState result)
        {
            using (var db = GetDb())
            using (var tx = db.BeginTransaction())
            {
                if (result == MailSendingState.Sended)
                {
                    var d = new SqlDelete("notify_info").Where("notify_id", id);
                    db.ExecuteNonQuery(d);
                }
                else
                {
                    if (result == MailSendingState.Error)
                    {
                        var q = new SqlQuery("notify_info").Select("attempts").Where("notify_id", id);
                        var attempts = db.ExecuteScalar<int>(q);
                        if (NotifyServiceCfg.MaxAttempts <= attempts + 1)
                        {
                            result = MailSendingState.FatalError;
                        }
                    }
                    var u = new SqlUpdate("notify_info")
                        .Set("state", (int)result)
                        .Set("attempts = attempts + 1")
                        .Set("modify_date", DateTime.UtcNow)
                        .Where("notify_id", id);
                    db.ExecuteNonQuery(u);
                }
                tx.Commit();
            }
        }


        private DbManager GetDb()
        {
            return new DbManager(dbid);
        }
    }
}
