using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class EventEngine
    {
        private readonly IEventDao dao;


        public EventEngine(IDaoFactory daoFactory)
        {
            dao = daoFactory.GetEventDao();
        }

        public Event GetByID(int id)
        {
            return dao.GetById(id);
        }

        public List<Event> GetByProject(int projectId)
        {
            return dao.GetByProject(projectId);
        }

        public Event SaveOrUpdate(Event value)
        {
            if (value == null) throw new ArgumentNullException("event");

            if (value.ID == default(int))
            {
                if (value.CreateBy == default(Guid)) value.CreateBy = SecurityContext.CurrentAccount.ID;
                if (value.CreateOn == default(DateTime)) value.CreateOn = TenantUtil.DateTimeNow();
            }

            value.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            value.LastModifiedOn = TenantUtil.DateTimeNow();

            return dao.Save(value);
        }

        public void Delete(int id)
        {
            dao.Delete(id);
        }
    }
}
