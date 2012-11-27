using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;

namespace ASC.Web.Projects.Configuration
{

    public class SearchHandler : BaseSearchHandlerEx
    {
        public override Guid ProductID
        {
            get { return new Guid("{1E044602-43B5-4d79-82F3-FD6208A11960}"); }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions {ImageFileName = "common_search_icon.png"}; }
        }

        public override Guid ModuleID
        {
            get { return new Guid("1e044602-43b5-4d79-82f3-fd6208a11960"); }
        }

        public override string SearchName
        {
            get { return ProjectsCommonResource.SearchText; }
        }

        public override string AbsoluteSearchURL
        {
            get { return VirtualPathUtility.ToAbsolute(PathProvider.BaseVirtualPath + "/search.aspx"); }
        }

        public override string PlaceVirtualPath
        {
            get { return PathProvider.BaseVirtualPath; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }

        public override SearchResultItem[] Search(string text)
        {
            // make here
            var items = Global.EngineFactory.GetSearchEngine().Search(text, 0);

            var list = new List<SearchResultItem>();

            foreach (var searchGroup in items)
            {
                if (searchGroup.Items.Count == 0)
                {
                    var item = new SearchResultItem
                                   {
                                       Name = searchGroup.ProjectTitle,
                                       Additional = new Dictionary<string, object>
                                                        {
                                                            {"Type", EntityType.Project},
                                                            {"imageRef", WebImageSupplier.GetAbsoluteWebPath(GetImage(EntityType.Project), ProductEntryPoint.ID)},
                                                            {"Hint", GetHint(EntityType.Project)}
                                                        },
                                       URL = GetItemPath(
                                               EntityType.Project,
                                               searchGroup.ProjectID.ToString(),
                                               searchGroup.ProjectID),
                                   };

                    list.Add(item);
                }

                list.AddRange(searchGroup.Items.Select(searchResultItem => new SearchResultItem
                                                                               {
                                                                                   Name = searchResultItem.Title, 
                                                                                   Description = searchResultItem.Description, 
                                                                                   Additional = new Dictionary<string, object>
                                                                                                       {
                                                                                                           {"ProjectName", searchGroup.ProjectTitle}, 
                                                                                                           {"Type", searchResultItem.EntityType}, 
                                                                                                           {"imageRef", WebImageSupplier.GetAbsoluteWebPath(GetImage(searchResultItem.EntityType), ProductEntryPoint.ID)}, 
                                                                                                           {"Hint", GetHint(searchResultItem.EntityType)}
                                                                                                       }, 
                                                                                    URL = GetItemPath(searchResultItem.EntityType, searchResultItem.ID, searchGroup.ProjectID), Date = searchResultItem.CreateOn
                                                                               }));
            }

            return list.ToArray();
        }

        public string GetItemPath(EntityType type, string id, int projectId)
        {
            var virtPath = VirtualPathUtility.ToAbsolute(PlaceVirtualPath);
            switch (type)
            {
                case EntityType.Message:
                    return string.Format("{2}messages.aspx?prjID={0}&ID={1}", projectId, id, virtPath);
                case EntityType.Milestone:
                    return string.Format("{2}milestones.aspx?prjID={0}&ID={1}", projectId, id, virtPath);
                case EntityType.Project:
                    return string.Format("{1}projects.aspx?prjID={0}", projectId, virtPath);
                case EntityType.Task:
                    return string.Format("{2}tasks.aspx?prjID={0}&ID={1}", projectId, id, virtPath);
                case EntityType.Team:
                    return string.Format("{1}projectteam.aspx?prjID={0}", projectId, virtPath);
                case EntityType.File:
                    return id;
                default:
                    return string.Empty;
            }
        }

        private string GetImage(EntityType type)
        {
            switch (type)
            {
                case EntityType.Message:
                    return "filetype/discussion.png";
                case EntityType.Milestone:
                    return "filetype/milestone.png";
                case EntityType.Project:
                    return "filetype/project.png";
                case EntityType.Task:
                    return "filetype/task.png";
                case EntityType.Team:
                    return "filetype/employee.png";

                case EntityType.File:
                default:
                    return "filetype/projectfile.png";
            }
        }

        private string GetHint(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Team:
                    return ProjectResource.Team;
                case EntityType.Comment:
                    return ProjectsCommonResource.Comment;
                case EntityType.Task:
                    return TaskResource.Task;
                case EntityType.Project:
                    return ProjectResource.Project;
                case EntityType.Milestone:
                    return MilestoneResource.Milestone;
                case EntityType.Message:
                    return MessageResource.Message;
                case EntityType.File:
                    return ProjectsFileResource.Documents;
                case EntityType.TimeSpend:
                    return ProjectsCommonResource.Time;
                case EntityType.SubTask:
                    return TaskResource.Subtask;
                default:
                    return String.Empty;
            }
        }
    }
}