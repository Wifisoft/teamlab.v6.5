#region Usings

using System;
using System.Web.Configuration;
using ASC.Api.Interfaces;
using ASC.Common.Data;
using ASC.Files.Core;
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Core.Calendars;

#endregion

namespace ASC.Api.Projects
{
    public class ProjectBootstrap : IApiBootstrapper
    {
        public void Configure()
        {
            if (!DbRegistry.IsDatabaseRegistered(ProjectApiBase.DbId))
            {
                DbRegistry.RegisterDatabase(ProjectApiBase.DbId, WebConfigurationManager.ConnectionStrings[ProjectApiBase.DbId]);
            }

            if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
            {
                DbRegistry.RegisterDatabase(FileConstant.DatabaseId, WebConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
            }

            ConfigurationManager.Configure(ProductEntryPoint.ID, PathProvider.BaseVirtualPath, String.Empty, ProjectApiBase.DbId);
            FilesIntegration.RegisterFileSecurityProvider("projects", "project", new SecurityAdapterProvider());
            //Register user activity
            ProductManager.RegisterUserActivityPublisher(new TimeLinePublisher());//That shit adds themselve in publisher list in constructor   

            //Register prodjects' calendar events
            CalendarManager.Instance.RegistryCalendarProvider(ProjectApi.GetUserCalendars);

        }
    }
}
