using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;

namespace ASC.Projects.Engine
{
    public class ReportEngine
    {
        private readonly IReportDao reportDao;
        private readonly IProjectDao projectDao;
        private readonly ITaskDao taskDao;
        private readonly IMilestoneDao milestoneDao;


        public ReportEngine(IDaoFactory daoFactory)
        {
            reportDao = daoFactory.GetReportDao();
            projectDao = daoFactory.GetProjectDao();
            taskDao = daoFactory.GetTaskDao();
            milestoneDao = daoFactory.GetMilestoneDao();
        }


        public IList<object[]> BuildReport(ReportType type, ReportFilter filter)
        {
            filter = (ReportFilter)filter.Clone();
            switch (type)
            {
                case ReportType.MilestonesExpired:
                case ReportType.MilestonesNearest:
                    return BuildMilestonesReport(filter);

                case ReportType.UsersWithoutActiveTasks:
                    return BuildUsersWithoutActiveTasks(filter);

                case ReportType.ProjectsWithoutActiveMilestones:
                    return BuildProjectsWithoutActiveMilestones(filter);

                case ReportType.ProjectsWithoutActiveTasks:
                    return BuildProjectsWithoutActiveTasks(filter);

                case ReportType.UsersActivity:
                    throw new NotImplementedException();

                case ReportType.UsersWorkload:
                    return BuildUsersWorkload(filter);

                case ReportType.ProjectsList:
                    return BuildProjectsListReport(filter);

                case ReportType.TasksByUsers:
                case ReportType.TasksByProjects:
                case ReportType.TasksExpired:
                    return BuildTasksListReport(filter);

                case ReportType.TimeSpend:
                    return BuildTimeSpendReport(filter);

                default:
                    throw new ArgumentOutOfRangeException("reportType");
            }
        }


        public List<ReportTemplate> GetTemplates(Guid userId)
        {
            return reportDao.GetTemplates(userId);
        }

        public List<ReportTemplate> GetAutoTemplates()
        {
            return reportDao.GetAutoTemplates();
        }

        public ReportTemplate GetTemplate(int id)
        {
            return reportDao.GetTemplate(id);
        }

        public ReportTemplate SaveTemplate(ReportTemplate template)
        {
            if (template == null) throw new ArgumentNullException("template");

            if (template.CreateOn == default(DateTime)) template.CreateOn = TenantUtil.DateTimeNow();
            if (string.IsNullOrEmpty(template.CreateBy)) template.CreateBy = SecurityContext.CurrentAccount.ID.ToString();
            return reportDao.SaveTemplate(template);
        }

        public void DeleteTemplate(int id)
        {
            reportDao.DeleteTemplate(id);
        }

        public IList<object[]> BuildProjectsListReport(ReportFilter filter)
        {
            filter = (ReportFilter)filter.Clone();
            return SelectOnlyCanReadProjects(reportDao.BuildProjectListReport(filter));
        }


        private IList<object[]> BuildMilestonesReport(ReportFilter filter)
        {
            return SelectOnlyCanReadMilestones(reportDao.BuildMilestonesReport(filter));
        }

        private IList<object[]> BuildUsersWithoutActiveTasks(ReportFilter filter)
        {
            var result = new List<object[]>();

            var users = new List<Guid>();
            if (filter.UserId != Guid.Empty) users.Add(filter.UserId);
            else if (filter.DepartmentId != Guid.Empty) users.AddRange(CoreContext.UserManager.GetUsersByGroup(filter.DepartmentId).Select(u => u.ID));
            else if (!filter.HasProjectIds) users.AddRange(CoreContext.UserManager.GetUsers().Select(u => u.ID));
            else filter.ProjectIds.ForEach(id => users.AddRange(projectDao.GetTeam(id).Select(r=> r.ID)));

            foreach (object[] row in reportDao.BuildUsersStatisticsReport(filter))
            {
                users.Remove((Guid)row[0]);
                if ((long)row[1] == 0 && (long)row[2] == 0)
                {
                    result.Add(row);
                }
            }
            foreach (var u in users)
            {
                result.Add(new object[] { u, 0, 0, 0 });
            }

            return result;
        }

        private IList<object[]> BuildUsersWorkload(ReportFilter filter)
        {
            return reportDao.BuildUsersStatisticsReport(filter);
        }

        private IList<object[]> BuildTasksListReport(ReportFilter filter)
        {
            if (filter.TaskStatuses.Count == Enum.GetValues(typeof(TaskStatus)).Length)
            {
                filter.TaskStatuses.Clear();
            }
            return SelectOnlyCanReadTasks(reportDao.BuildTaskListReport(filter));
        }

        private IList<object[]> BuildTimeSpendReport(ReportFilter filter)
        {
            var data = SelectOnlyCanReadTimeSpend(reportDao.BuildTimeReport(filter)).ToList();

            if(filter.ViewType == 0)
            {
                data = data.GroupBy(r => r[0]).Select(r => new[] { r.Key, r.Sum(item => (double)item[4]) }).ToList();
                return data.ConvertAll(r => new object[] { new Guid((string)r[0]), 0, 0, "", ToTimeSpend((double)r[1]) });
            }

            return data.ConvertAll(r => new object[] {new Guid((string)r[0]), Convert.ToInt32(r[1]), Convert.ToInt32(r[2]),(string) r[3], ToTimeSpend((double)r[4])});
        }

        private string ToTimeSpend(double timeSpend)
        {
            var time = Math.Round(timeSpend, 4);
            var hours = (int)time;
            var minutes = (int)((time - hours) * 60);
            return hours + ":" + minutes.ToString("D2");
        }

        private IList<object[]> BuildProjectsWithoutActiveMilestones(ReportFilter filter)
        {
            return SelectOnlyCanReadProjects(reportDao.BuildProjectWithoutOpenMilestone(filter));
        }

        private IList<object[]> BuildProjectsWithoutActiveTasks(ReportFilter filter)
        {
            return SelectOnlyCanReadProjects(reportDao.BuildProjectWithoutActiveTask(filter));
        }

        private IList<object[]> SelectOnlyCanReadProjects(IList<object[]> rows)
        {
            var visibleProjects = new List<object[]>();
            foreach (var r in rows)
            {
                var project = new Project() { ID = Convert.ToInt32(r[0]), Title = (string)r[1], Responsible = (Guid)r[2], Private = (bool)r[7] };
                if (ProjectSecurity.CanRead(project))
                {
                    var row = new object[r.Length - 1];//don't copy last field (Private)
                    Array.Copy(r, row, row.Length);
                    visibleProjects.Add(row);
                }
            }

            return visibleProjects;
        }

        private IList<object[]> SelectOnlyCanReadMilestones(IList<object[]> rows)
        {
            var visibleMilestones = new List<object[]>();
            foreach (var r in rows)
            {
                var milestone = milestoneDao.GetById(Convert.ToInt32(r[2]));
                if (ProjectSecurity.CanRead(milestone))
                {
                    visibleMilestones.Add(r);
                }
            }

            return visibleMilestones;
        }

        private IList<object[]> SelectOnlyCanReadTimeSpend(IList<object[]> rows)
        {
            var visibleTime = new List<object[]>();
            foreach (var r in rows)
            {
                var relativeTask = taskDao.GetById(Convert.ToInt32(r[2]));
                if (ProjectSecurity.CanRead(relativeTask))
                {
                    visibleTime.Add(r);
                }
            }

            return visibleTime;
        }

        private IList<object[]> SelectOnlyCanReadTasks(IList<object[]> rows)
        {
            var visibleTasks = new List<object[]>();
            var unsortedTasks = new List<object[]>();
            var currentPrjID = -1;

            foreach (var r in rows)
            {
                if (currentPrjID != Convert.ToInt32(r[0]))
                {
                    currentPrjID = Convert.ToInt32(r[0]);
                    visibleTasks.AddRange(unsortedTasks);
                    unsortedTasks = new List<object[]>();
                }

                var task = taskDao.GetById(Convert.ToInt32(r[6]));
                if (ProjectSecurity.CanRead(task))
                {
                    if (Convert.ToInt32(r[2])!=0)
                        visibleTasks.Add(r);
                    else
                        unsortedTasks.Add(r);
                }
            }

            visibleTasks.AddRange(unsortedTasks);

            return visibleTasks;
        }
    }
}
