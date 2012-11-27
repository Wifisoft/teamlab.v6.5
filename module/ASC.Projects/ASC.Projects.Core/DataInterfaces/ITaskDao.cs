using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ITaskDao
    {
        List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant);

        List<Task> GetByResponsible(Guid responsibleId, IEnumerable<TaskStatus> statuses);
        
        List<Task> GetLastTasks(Guid participant, int max, params int[] projects);

        List<Task> GetMilestoneTasks(int milestoneId);

        List<Task> GetUpdates(DateTime from, DateTime to);

        List<Task> GetByFilter(TaskFilter filter);

        void SetTaskOrders(int? milestoneId, int taskID, int? prevTaskID, int? nextTaskID);

        List<Task> GetById(ICollection<int> ids);

        Task GetById(int id);

        bool IsExists(int id);

        int GetTaskCount(int milestoneId, params TaskStatus[] statuses);

        List<object[]> GetTasksForReminder(DateTime deadline);


        Task Save(Task task);

        void TaskTrace(int target, Guid owner, DateTime date, TaskStatus status);

        void Delete(int id);

        void SaveRecurrence(Task task, string cron, DateTime startDate, DateTime endDate);

        void DeleteReccurence(int taskId);

        List<object[]> GetRecurrence(DateTime date);
    }
}