using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class TemplateEngine
    {
        private readonly EngineFactory _factory;
        private readonly ITemplateDao _dao;

		public TemplateEngine(IDaoFactory daoFactory, EngineFactory factory)
		{
		    _factory = factory;
		    _dao = daoFactory.GetTemplateDao();			
		}

        public List<Template> GetTemplates()
        {
            return _dao.GetTemplates();
        }

        public Template GetTemplate(int id)
        {
            return _dao.GetTemplate(id);
        }

        public Template SaveTemplate(Template template)
        {
            if (template.Id == default(int))
            {
                if (template.CreateBy == default(Guid)) template.CreateBy = SecurityContext.CurrentAccount.ID;
                if (template.CreateOn == default(DateTime)) template.CreateOn = TenantUtil.DateTimeNow();
            }

            template.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            template.LastModifiedOn = TenantUtil.DateTimeNow();

            return _dao.SaveTemplate(template);
        }

        public void RemoveTemplate(int id)
        {
            _dao.RemoveTemplate(id);
        }
    }
}
