#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.CRM.Core;
using ASC.Web.Core;
using ASC.Web.Files.Api;
using ASC.Api.Interfaces;
using ASC.Common.Data;
using ASC.Files.Core;
using System.Web.Configuration;
using ASC.Web.Core.Calendars;

#endregion

namespace ASC.Api.CRM
{
    public class CRMBootstrap : IApiBootstrapper
    {
        public void Configure()
        {
            if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
                DbRegistry.RegisterDatabase(FileConstant.DatabaseId, WebConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
            

            if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "crm_common"))
                FilesIntegration.RegisterFileSecurityProvider("crm", "crm_common", new FileSecurityProvider());

            //Register prodjects' calendar events
            CalendarManager.Instance.RegistryCalendarProvider(CRMApi.GetUserCalendars);

        }
    }
}
