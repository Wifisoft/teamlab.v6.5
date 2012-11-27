using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Users.Activity;

namespace ASC.Projects.Engine
{
    public class TimeTrackingEngine
    {
        private readonly ITimeSpendDao timeSpendDao;
        private readonly IProjectDao projectDao;
        private readonly ITaskDao taskDao;


        public TimeTrackingEngine(IDaoFactory daoFactory)
        {
            timeSpendDao = daoFactory.GetTimeSpendDao();
            projectDao = daoFactory.GetProjectDao();
            taskDao = daoFactory.GetTaskDao();
        }

        public List<TimeSpend> GetByFilter(TaskFilter filter)
        {
            var listTimeSpend = new List<TimeSpend>(); 

            while (true)
            {
                var timeSpend = timeSpendDao.GetByFilter(filter);
                timeSpend = GetTasks(timeSpend).FindAll(r => ProjectSecurity.CanRead(r.Task));

                if (filter.LastId != 0)
                {
                    var lastTimeSpendIndex = timeSpend.FindIndex(r => r.ID == filter.LastId);

                    if (lastTimeSpendIndex >= 0)
                    {
                        timeSpend = timeSpend.SkipWhile((r, index) => index <= lastTimeSpendIndex).ToList();
                    }
                }

                listTimeSpend.AddRange(timeSpend);

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listTimeSpend = listTimeSpend.Take((int)filter.Max).ToList();

                if (listTimeSpend.Count == filter.Max || timeSpend.Count == 0) break;

                if (listTimeSpend.Count != 0)
                    filter.LastId = listTimeSpend.Last().ID;

                filter.Offset += filter.Max;
            }

            return listTimeSpend;
        }

        public List<TimeSpend> GetByTask(int taskId)
        {
            var timeSpend = timeSpendDao.GetByTask(taskId);
            return GetTasks(timeSpend).FindAll(r => ProjectSecurity.CanRead(r.Task));
        }

        public List<TimeSpend> GetByProject(int projectId)
        {
            var timeSpend = timeSpendDao.GetByProject(projectId);
            return GetTasks(timeSpend).FindAll(r => ProjectSecurity.CanRead(r.Task)); 
        }

        public TimeSpend GetByID(int id)
        {
            var timeSpend = timeSpendDao.GetById(id);
            timeSpend.Task = taskDao.GetById(timeSpend.Task.ID);
            return timeSpend;
        }

        public List<TimeSpend> GetUpdates(DateTime from, DateTime to)
        {
            return timeSpendDao.GetUpdates(from, to)
                .Where(x=>ProjectSecurity.CanRead(x.Task.Project)).ToList();
        }

        public bool HasTime(int taskId)
        {
            return timeSpendDao.HasTime(taskId);
        }

        public Dictionary<int, bool> HasTime(params int[] tasks)
        {
            return timeSpendDao.HasTime(tasks);
        }

        public TimeSpend SaveOrUpdate(TimeSpend timeSpend)
        {
            return SaveOrUpdate(timeSpend, false);
        }
        
        public TimeSpend SaveOrUpdate(TimeSpend timeSpend, bool isImport)
        {
            TimeLinePublisher.TimeSpend(timeSpend, timeSpend.Task.Project,
                                        timeSpend.Task,
                                        isImport ? EngineResource.ActionText_Imported : EngineResource.ActionText_Add,
                                        UserActivityConstants.ActivityActionType, UserActivityConstants.SmallActivity);
            return timeSpendDao.Save(timeSpend);
        }

        public void Delete(int id)
        {
            timeSpendDao.Delete(id);
        }

        private List<TimeSpend> GetTasks(List<TimeSpend> listTimeSpend)
        {
            var listTasks = taskDao.GetById(listTimeSpend.Select(r => r.Task.ID).ToList());

            listTimeSpend.ForEach(t => t.Task = listTasks.Find(task => task.ID == t.Task.ID));

            return listTimeSpend;
        }
    }
}
