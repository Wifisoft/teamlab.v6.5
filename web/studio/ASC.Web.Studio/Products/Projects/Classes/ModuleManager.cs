using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Projects.Resources;

namespace ASC.Web.Projects.Classes
{
    public class ModuleManager
    {
        private sealed class ProjectOverviewModule : Module
        {
            public override string Name
            {
                get { return ProjectResource.ProjectOverview; }
            }

            public ProjectOverviewModule()
            {
                ID = new Guid("{52F1574D-8656-4bf1-B494-F5CBE64EF327}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "projects.aspx?prjID={0}");
                ModuleSysName = "Projects";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 0;
            }
        }

        private sealed class MilestonesModule : Module
        {
            public override string Name
            {
                get { return MilestoneResource.Milestones; }
            }

            public MilestonesModule()
            {
                ID = new Guid("{AF4AFD50-5553-47f3-8F91-651057BC930B}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "milestones.aspx?prjID={0}");
                ModuleSysName = "Milestones";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 1;
            }
        }

        private sealed class TasksModule : Module
        {
            public override string Name
            {
                get { return TaskResource.Tasks; }
            }

            public TasksModule()
            {
                ID = new Guid("{04339423-70E6-4b81-A2DF-3C31C723BD90}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "tasks.aspx?prjID={0}");
                ModuleSysName = "Tasks";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 2;
            }
        }

        private sealed class MessagesModule : Module
        {
            public override string Name
            {
                get { return MessageResource.Messages; }
            }

            public MessagesModule()
            {
                ID = new Guid("{9FF0FADE-6CFA-44ee-901F-6185593E4594}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "messages.aspx?prjID={0}");
                ModuleSysName = "Messages";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 4;
            }
        }

        private sealed class DocumentsModule : Module
        {
            public override string Name
            {
                get { return ProjectsFileResource.Documents; }
            }

            public DocumentsModule()
            {
                ID = new Guid("{81402440-557D-401d-9EE1-D570748F426D}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "tmdocs".ToLower() + ".aspx?prjID={0}");
                ModuleSysName = "TMDocs";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 6;
            }
        }

        private sealed class TimeTrackingModule : Module
        {
            public override string Name
            {
                get { return ProjectsCommonResource.TimeTracking; }
            }

            public TimeTrackingModule()
            {
                ID = new Guid("{57E87DA0-D59B-443d-99D1-D9ABCAB31084}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "timetracking.aspx?prjID={0}");
                ModuleSysName = "TimeTracking";
                Context.DefaultSortOrder = 7;
            }
        }

        private sealed class HistoryModule : Module
        {
            public override string Name
            {
                get { return ProjectsCommonResource.History; }
            }

            public HistoryModule()
            {
                ID = new Guid("{85E7CE26-46F6-4c3c-B25D-4BDE833C9742}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "history.aspx?prjID={0}");
                ModuleSysName = "History";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 0;
                Context.SortDisabled = true;
            }
        }

        private sealed class ProjectTeamModule : Module
        {
            public override string Name
            {
                get { return ProjectResource.ProjectTeam; }
            }

            public ProjectTeamModule()
            {
                ID = new Guid("{C42F993E-5D22-497e-AC26-1E9592515898}");
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "projectTeam.aspx?prjID={0}");
                ModuleSysName = "ProjectTeam";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 0;
                Context.SortDisabled = true;
            }
        }

        
        private readonly Dictionary<string, IModule> visibles;

        public List<IModule> Modules
        {
            get;
            private set;
        }


        public ModuleManager()
        {
            var modules = new List<IModule>
            {
                new ProjectOverviewModule(),
                new MilestonesModule(),
                new TasksModule(),
                new MessagesModule(),
                new DocumentsModule(),
                new TimeTrackingModule(),
                new HistoryModule(),
                new ProjectTeamModule(),
            };

            visibles = new Dictionary<string, IModule>(modules.Count);
            Modules = new List<IModule>();

            modules.ForEach(m =>
            {
                if (!WebItemManager.Instance.RegistryItem(m)) return;
                Modules.Add(m);
                visibles.Add(m.ModuleSysName, m);
            });
        }

        public bool IsVisible(ModuleType type)
        {
            IModule wi;
            visibles.TryGetValue(type.ToString(), out wi);
            return wi != null && !wi.IsDisabled();
        }
    }
}
