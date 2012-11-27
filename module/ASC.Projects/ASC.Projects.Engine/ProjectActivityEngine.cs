#region Usings

using System.Collections.Generic;
using System.Linq;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;
#endregion

namespace ASC.Projects.Engine

{
    public class ProjectActivityEngine
    {

        public ProjectActivityEngine(IDaoFactory daoFactory, EngineFactory engineFactory)
        {
            
        }

        public List<UserActivity> GetByFilter(ProjectActivityFilter filter)
        {
            var listProjects = new List<UserActivity>();

            while (true)
            {
                var projects = UserActivityManager.GetProjectsActivities(
                    TenantProvider.CurrentTenantID,
                    EngineFactory.ProductId,
                    filter.UserId,
                    filter.ProjectId,
                    filter.Type,
                    filter.SearchText,
                    filter.From, filter.To,
                    filter.Offset, filter.Max, filter.LastId,
                    filter.SortBy, filter.SortOrder);

                if (filter.LastId != 0)
                {
                    var lastProjectIndex = projects.FindIndex(r => r.ID == filter.LastId);

                    if (lastProjectIndex >= 0)
                    {
                        projects = projects.SkipWhile((r, index) => index <= lastProjectIndex).ToList();
                    }
                }

                listProjects.AddRange(projects);

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listProjects = listProjects.Take((int)filter.Max).ToList();

                if (listProjects.Count == filter.Max || projects.Count == 0) break;


                if (listProjects.Count != 0)
                    filter.LastId = (int)listProjects.Last().ID;

                filter.Offset += filter.Max;
            }

            return listProjects;
        }

    }
}
