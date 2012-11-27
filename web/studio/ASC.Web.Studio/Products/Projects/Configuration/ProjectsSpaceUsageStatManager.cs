using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Core;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using System.Configuration;
using ASC.Core;
using ASC.Web.Projects.Classes;
using System.Collections;

namespace ASC.Web.Projects.Configuration
{
    public class ProjectsSpaceUsageStatManager : SpaceUsageStatManager
    {
        private const string PROJECTS_DBID = "projects";
        private const string FILES_DBID = "files";

        public override List<SpaceUsageStatManager.UsageSpaceStatItem> GetStatData()
        {
            var data = new List<SpaceUsageStatManager.UsageSpaceStatItem>();

            if (!DbRegistry.IsDatabaseRegistered(PROJECTS_DBID)) DbRegistry.RegisterDatabase(PROJECTS_DBID, ConfigurationManager.ConnectionStrings[PROJECTS_DBID]);
            if (!DbRegistry.IsDatabaseRegistered(FILES_DBID)) DbRegistry.RegisterDatabase(FILES_DBID, ConfigurationManager.ConnectionStrings[FILES_DBID]);

            using (var filedb = new DbManager(FILES_DBID))
            using (var projdb = new DbManager(PROJECTS_DBID))
            {
                var q = new SqlQuery("files_file f")
                    .Select("b.right_node")
                    .SelectSum("f.content_length")
                    .InnerJoin("files_folder_tree t", Exp.EqColumns("f.folder_id", "t.folder_id"))
                    .InnerJoin("files_bunch_objects b", Exp.EqColumns("t.parent_id", "b.left_node"))
                    .Where("b.tenant_id", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                    .Where(Exp.Like("b.right_node", "projects/project/", SqlLike.StartWith))
                    .GroupBy(1);

                var sizes = filedb.ExecuteList(q)
                    .Select(r => new { ProjectId = Convert.ToInt32(((string)r[0]).Substring(17)), Size = Convert.ToInt64(r[1]) })
                    .GroupBy(r => r.ProjectId)
                    .ToDictionary(g => g.Key, g => g.Sum(a => a.Size));
                
                q = new SqlQuery("projects_projects")
                    .Select("id", "title")
                    .Where("tenant_id", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                    .Where(Exp.In("id", sizes.Keys));

                var dbData = projdb.ExecuteList(q)
                    .Select(r => Tuple.Create(Convert.ToInt32(r[0]), (string)r[1], sizes[Convert.ToInt32(r[0])]))
                    .OrderByDescending(t=> t.Item3)
                    .ToList();

                foreach (var row in dbData)
                    data.Add(new UsageSpaceStatItem()
                    {
                        Name = row.Item2,
                        SpaceUsage = row.Item3,
                        Url = String.Concat(PathProvider.BaseAbsolutePath, "projects.aspx?prjID=" + row.Item1.ToString())
                    });
            }        

            return data;
        }
    }
}
