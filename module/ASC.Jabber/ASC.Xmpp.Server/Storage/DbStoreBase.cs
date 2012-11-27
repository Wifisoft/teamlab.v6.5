using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Storage
{
    public abstract class DbStoreBase : IConfigurable
    {
        protected static readonly int MESSAGE_COLUMN_LEN = (int)Math.Pow(2, 24) - 1;

        private readonly object syncRoot = new object();
        private DbManager db;


        public virtual void Configure(IDictionary<string, string> properties)
        {
            var dbid = string.Empty;

            if (properties.ContainsKey("connectionString"))
            {
                dbid = GetType().FullName;
                var connectionString = properties["connectionString"];
                if (!DbRegistry.IsDatabaseRegistered(dbid))
                {
                    DbRegistry.RegisterDatabase(dbid, "System.Data.SQLite", connectionString);
                }
                CreateDbFolderIfNotExists(connectionString);
            }
            else if (properties.ContainsKey("connectionStringName"))
            {
                dbid = properties["connectionStringName"];
                var connectionString = ConfigurationManager.ConnectionStrings[dbid];
                if (connectionString == null)
                {
                    throw new ConfigurationErrorsException("Can not find connection string with name " + dbid);
                }
                if (!DbRegistry.IsDatabaseRegistered(dbid))
                {
                    DbRegistry.RegisterDatabase(dbid, connectionString);
                }
                CreateDbFolderIfNotExists(connectionString.ConnectionString);
            }
            else
            {
                throw new ConfigurationErrorsException("Can not create database connection: no connectionString or connectionStringName properties.");
            }

            db = new DbManager(dbid);

            if (!properties.ContainsKey("generateSchema") || Convert.ToBoolean(properties["generateSchema"]))
            {
                var creates = GetCreateSchemaScript();
                if (creates != null && 0 < creates.Length)
                {
                    foreach (var c in creates)
                    {
                        db.ExecuteNonQuery(c);
                    }
                }
            }
        }


        protected virtual SqlCreate[] GetCreateSchemaScript()
        {
            return new SqlCreate[0];
        }


        protected List<object[]> ExecuteList(ISqlInstruction sql)
        {
            lock (syncRoot) return db.ExecuteList(sql);
        }

        protected T ExecuteScalar<T>(ISqlInstruction sql)
        {
            lock (syncRoot) return db.ExecuteScalar<T>(sql);
        }

        protected int ExecuteNonQuery(ISqlInstruction sql)
        {
            lock (syncRoot) return db.ExecuteNonQuery(sql);
        }

        protected int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            lock (syncRoot) return db.ExecuteBatch(batch);
        }


        private void CreateDbFolderIfNotExists(string connectionString)
        {
            if (connectionString.ToLower().Contains("data source="))
            {
                var dbDir = Path.GetDirectoryName(PathUtils.GetAbsolutePath(GetDbSQLitePath(connectionString)));
                if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);
            }
        }

        private string GetDbSQLitePath(string connectionString)
        {
            return new SQLiteConnectionStringBuilder(connectionString).DataSource;
        }
    }
}