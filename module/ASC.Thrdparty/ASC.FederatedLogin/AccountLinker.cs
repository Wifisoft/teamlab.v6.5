using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.FederatedLogin.Profile;

namespace ASC.FederatedLogin
{
    public class AccountLinker
    {
        private const string DbId = "login_linker";
        private const string LinkTable = "account_links";

        public AccountLinker(ConnectionStringSettings connectionString)
        {
            if (!DbRegistry.IsDatabaseRegistered(DbId))
            {
                DbRegistry.RegisterDatabase(DbId, connectionString);
            }
        }

        public IEnumerable<string> GetLinkedObjects(string id, string provider)
        {
            return GetLinkedObjects(new LoginProfile() {Id = id, Provider = provider});
        }


        public IEnumerable<string> GetLinkedObjects(LoginProfile profile)
        {
            //Retrieve by uinque id
            return GetLinkedObjectsByHashId(profile.HashId);
        }

        public IEnumerable<string> GetLinkedObjectsByHashId(string hashid)
        {
            //Retrieve by uinque id
            using (var db = new DbManager(DbId))
            {
                var query = new SqlQuery(LinkTable)
                    .Select("id").Where("uid", hashid).Where(!Exp.Eq("provider", string.Empty));
                return db.ExecuteList(query, (x) => (string)x[0]);
            }
        }

        public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
        {
            //Retrieve by uinque id
            using (var db = new DbManager(DbId))
            {
                var query = new SqlQuery(LinkTable)
                    .Select("profile").Where("id", obj);
                return db.ExecuteList(query, (x) => LoginProfile.CreateFromSerializedString((string)x[0]));
            }
        }

        public void AddLink(string obj, LoginProfile profile)
        {
            using (var db = new DbManager(DbId))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlInsert(LinkTable,true)
                            .InColumnValue("id", obj)
                            .InColumnValue("uid", profile.HashId)
                            .InColumnValue("provider", profile.Provider)
                            .InColumnValue("profile", profile.ToSerializedString())
                            .InColumnValue("linked", DateTime.UtcNow)
                        );
                    tx.Commit();
                }
            }
        }

        public void AddLink(string obj, string id, string provider)
        {
            AddLink(obj, new LoginProfile() { Id = id, Provider = provider });
        }

        public void RemoveLink(string obj, string id, string provider)
        {
            RemoveLink(obj, new LoginProfile() { Id = id, Provider = provider });
        }

        public void RemoveLink(string obj, LoginProfile profile)
        {
            using (var db = new DbManager(DbId))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlDelete(LinkTable)
                            .Where("id", obj)
                            .Where("uid", profile.HashId)
                        );
                    tx.Commit();
                }
            }
        }

        public void Unlink(string obj)
        {
            using (var db = new DbManager(DbId))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlDelete(LinkTable)
                            .Where("id", obj)
                        );
                    tx.Commit();
                }
            }
        }

        public void RemoveProvider(string obj, string provider)
        {
            using (var db = new DbManager(DbId))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlDelete(LinkTable)
                            .Where("id", obj)
                            .Where("provider", provider)
                        );
                    tx.Commit();
                }
            }
        }
    }
}