using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IEventDao
    {
        List<Event> GetByProject(int projectId);

        Event GetById(int id);

        Event Save(Event value);

        void Delete(int id);
    }
}
