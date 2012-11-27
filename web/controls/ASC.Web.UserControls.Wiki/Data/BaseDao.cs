﻿using System;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Web.UserControls.Wiki.Data
{
    class BaseDao : IDisposable
    {
        private const string TenantColumn = "tenant";

        protected readonly DbManager db;
        protected readonly int tenant;


        public BaseDao(string dbid, int tenant)
        {
            this.db = new DbManager(dbid);
            this.tenant = tenant;
        }


        public void Dispose()
        {
            db.Dispose();
        }

        
        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), tenant);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumnValue(GetTenantColumnName(table), tenant);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), tenant);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), tenant);
        }

        private string GetTenantColumnName(string table)
        {
            var pos = table.LastIndexOf(' ');
            return (0 < pos ? table.Substring(pos).Trim() + '.' : string.Empty) + TenantColumn;
        }
    }
}