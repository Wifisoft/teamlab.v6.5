using System;
using System.Collections.Generic;
using ASC.Api.Documents;
using ASC.Api.Impl;
using ASC.Api.Projects.Calendars;
using ASC.Web.Core.Calendars;
using ASC.Common.Data;
using System.Web.Configuration;
using ASC.Projects.Engine;
using ASC.Core;

namespace ASC.Api.Projects
{
    ///<summary>
    /// Projects access
    ///</summary>
    public partial class ProjectApi : ProjectApiBase, Interfaces.IApiEntryPoint
    {
        private readonly DocumentsApi _documentsApi;

        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "project"; }
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="context"></param>
        ///<param name="documentsApi">Docs api</param>
        public ProjectApi(ApiContext context, Documents.DocumentsApi documentsApi)
        {
            _documentsApi = documentsApi;
            _context = context;
        }

        internal static List<BaseCalendar> GetUserCalendars(Guid userId)
        {
            if (!DbRegistry.IsDatabaseRegistered(DbId))            
                DbRegistry.RegisterDatabase(DbId, WebConfigurationManager.ConnectionStrings[DbId]);

            var tenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var engineFactory = new EngineFactory(DbId, tenantId, ASC.Data.Storage.StorageFactory.GetStorage(tenantId.ToString(), DbId));

            var cals = new List<BaseCalendar>();
            var engine = engineFactory.GetProjectEngine();
            var projects = engine.GetByParticipant(userId);

            if (projects != null)
            {                
                foreach (var p in projects)
                {
                    var team = engine.GetTeam(p.ID);
                    var sharingOptions = new SharingOptions();
                    foreach (var participant in team)                    
                        sharingOptions.PublicItems.Add(new SharingOptions.PublicItem() { Id = participant.ID, IsGroup = false });

                    var index = p.ID % CalendarColors.BaseColors.Count;
                    cals.Add(new ProjectCalendar(engineFactory, userId, p, CalendarColors.BaseColors[index].BackgroudColor, CalendarColors.BaseColors[index].TextColor, sharingOptions, false));
                }
            }

            var folowingProjects = engine.GetFollowing(userId);
            if(folowingProjects!=null)
            {
                foreach (var p in folowingProjects)
                {
                    if (projects != null && projects.Exists(proj => proj.ID == p.ID))
                        continue;


                    var team = engine.GetTeam(p.ID);
                    var sharingOptions = new SharingOptions();
                    sharingOptions.PublicItems.Add(new SharingOptions.PublicItem() { Id = userId, IsGroup = false });

                    foreach (var participant in team)
                        sharingOptions.PublicItems.Add(new SharingOptions.PublicItem() { Id = participant.ID, IsGroup = false });

                    var index = p.ID % CalendarColors.BaseColors.Count;
                    cals.Add(new ProjectCalendar(engineFactory, userId, p, CalendarColors.BaseColors[index].BackgroudColor, CalendarColors.BaseColors[index].TextColor, sharingOptions, true));
                }
            }

            return cals;
        }
    }
}
