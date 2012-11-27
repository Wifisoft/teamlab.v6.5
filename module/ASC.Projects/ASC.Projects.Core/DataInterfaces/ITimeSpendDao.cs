using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ITimeSpendDao
    {
        List<TimeSpend> GetByTask(int taskId);

        List<TimeSpend> GetByProject(int projectId);

        TimeSpend GetById(int id);

        List<TimeSpend> GetByFilter(TaskFilter filter);

        List<TimeSpend> GetUpdates(DateTime from, DateTime to); 
        
        bool HasTime(int taskId);

        Dictionary<int, bool> HasTime(params int[] tasks);


        TimeSpend Save(TimeSpend timeSpend);

        void Delete(int id);
    }
}
