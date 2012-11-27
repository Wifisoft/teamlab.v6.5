#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Impl;
using System.Text;
using ASC.Api.Interfaces;
using ASC.CRM.Core;
using ASC.Web.Files.Api;
using ASC.Web.Core.Calendars;
using ASC.CRM.Core.Dao;
using ASC.Core;
using ASC.Common.Data;
using System.Web.Configuration;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi : CRMApiBase, IApiEntryPoint
    {
        public ApiContext _context;


        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "crm"; }
        }


        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="context"></param>
        public CRMApi(ApiContext context)
        {
            _context = context;

        }

        internal static List<BaseCalendar> GetUserCalendars(Guid userId)
        {
            if (!DbRegistry.IsDatabaseRegistered(CRMConstants.DatabaseId))
                DbRegistry.RegisterDatabase(CRMConstants.DatabaseId, WebConfigurationManager.ConnectionStrings[CRMConstants.DatabaseId]);

            var tenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var crmDaoFactory = new DaoFactory(tenantId,CRMConstants.DatabaseId);

            var cals = new List<BaseCalendar>();
            cals.Add(new CRMCalendar(crmDaoFactory,userId));
            return cals;
           
        }

    }
}
