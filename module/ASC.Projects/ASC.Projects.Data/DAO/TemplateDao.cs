using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Data.DAO
{
    class TemplateDao : BaseDao, ITemplateDao
    {
        private readonly string[] _templateColumns = new[] { "id", "title", "description", "create_by", "create_on" };

        public TemplateDao(string dbId, int tenant)
            : base(dbId, tenant)
        {

        }

        public List<Template> GetTemplates()
        {
            var q = Query("projects_templates p")
                   .Select(_templateColumns);

            return DbManager
                .ExecuteList(q)
                .ConvertAll(r => ToTemplate(r));
        }

        public Template GetTemplate(int id)
        {
            var query = Query("projects_templates p").Select(_templateColumns).Where("p.id", id);
            return DbManager.ExecuteList(query).ConvertAll(r => ToTemplate(r)).SingleOrDefault(); ;
        }

        public Template SaveTemplate(Template template)
        {
            var insert = Insert("projects_templates")
                    .InColumnValue("id", template.Id)
                    .InColumnValue("title", template.Title)
                    .InColumnValue("description", template.Description)
                    .InColumnValue("create_by", template.CreateBy.ToString())
                    .InColumnValue("create_on", TenantUtil.DateTimeToUtc(template.CreateOn))
                    .InColumnValue("last_modified_by", template.LastModifiedBy.ToString())
                    .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(template.LastModifiedOn))
                    .Identity(1, 0, true);

            template.Id = DbManager.ExecuteScalar<int>(insert);

            return template;
        }

        public void RemoveTemplate(int id)
        {
            DbManager.ExecuteNonQuery(Delete("projects_templates").Where("id", id));
        }

        private static Template ToTemplate(IList<object> r)
        {
            return new Template
            {
                Id = Convert.ToInt32(r[0]),
                Title = (string)r[1],
                Description = (string)r[2],
                CreateBy = new Guid((string)r[3]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[4])
            };
        }
    }
}
