using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ISubtaskDao
    {
        List<Subtask> GetSubtasks(int taskid);

        void GetSubtasks(ref List<Task> tasks);

        void CloseAllSubtasks(Task task);

        List<Subtask> GetById(ICollection<int> ids);

        Subtask GetById(int id);

        List<Subtask> GetUpdates(DateTime from, DateTime to); 

        int GetSubtaskCount(int taskid, params TaskStatus[] statuses);

        Subtask Save(Subtask task);

        void Delete(int id);
    }
}
