using System;
using System.Data;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Blogs.Core.Data
{
    public class DbDao
    {
        private readonly DbManager _db;


        protected DbDao(DbManager db, int tenant)
        {
            if (db == null) throw new ArgumentNullException("db");

            _db = db;
            Tenant = tenant;
        }

        public int Tenant { get; private set; }

        public DbManager Db { get { return _db; } }

        public IDbConnection OpenConnection()
        {
            return _db.Connection;
        }

        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), Tenant);
        }
        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), Tenant);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), Tenant);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(TenantColumnName).Values(Tenant);
        }

        protected string TenantColumnName { get { return "Tenant"; } }
        
        protected string GetTenantColumnName(string table)
        {
            return String.Format("{0}.{1}", table, TenantColumnName);
        }
    }
}
