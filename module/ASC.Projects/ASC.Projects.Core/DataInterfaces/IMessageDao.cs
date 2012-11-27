using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IMessageDao
    {
        List<Message> GetByProject(int projectId);

        List<Message> GetMessages(int startIndex, int maxResult);

        List<Message> GetRecentMessages(int offset, int maxResult, params int[] projects);

        List<Message> GetByFilter(TaskFilter filter);

        List<Message> GetUpdates(DateTime from, DateTime to);

        Message GetById(int id);

        bool IsExists(int id);

        Message Save(Message message);

        void Delete(int id);
    }
}
