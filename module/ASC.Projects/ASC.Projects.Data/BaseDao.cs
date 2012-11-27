#region Usings

using System;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using System.Web;

#endregion

namespace ASC.Projects.Data
{
    abstract class BaseDao
    {
        //HACK: for basecamp import in thread pool
        [ThreadStatic]
        private static DbManager staticDbManager;


        protected int Tenant { get; private set; }

        protected DbManager DbManager { get; private set; }


        protected BaseDao(string dbId, int tenant)
        {
            Tenant = tenant;

            if (HttpContext.Current != null)
            {
                DbManager = DbManager.FromHttpContext(dbId);
            }
            else
            {
                if (staticDbManager == null)
                {
                    lock (typeof(BaseDao))
                    {
                        if (staticDbManager == null) staticDbManager = new DbManager(dbId);
                    }
                }
                DbManager = staticDbManager;
            }
        }


        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where("tenant_id", Tenant);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumnValue("tenant_id", Tenant);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where("tenant_id", Tenant);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where("tenant_id", Tenant);
        }

        protected static Guid ToGuid(object guid)
        {
            var str = guid as string;
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }
    }
}
