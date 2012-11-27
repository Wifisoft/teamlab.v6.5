using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ITemplateDao
    {
        List<Template> GetTemplates();

        Template GetTemplate(int id);

        Template SaveTemplate(Template template);

        void RemoveTemplate(int id);
	}
}
