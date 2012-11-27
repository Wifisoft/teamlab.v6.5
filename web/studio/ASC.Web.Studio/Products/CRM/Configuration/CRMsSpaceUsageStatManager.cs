using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Web.Core;
using System.Web;

namespace ASC.Web.CRM.Configuration
{
    public class CRMSpaceUsageStatManager : SpaceUsageStatManager
    {        
        private const string FILES_DBID = "files";

        public override List<SpaceUsageStatManager.UsageSpaceStatItem> GetStatData()
        {
            var data = new List<SpaceUsageStatManager.UsageSpaceStatItem>();
            
            if (!DbRegistry.IsDatabaseRegistered(FILES_DBID)) DbRegistry.RegisterDatabase(FILES_DBID, ConfigurationManager.ConnectionStrings[FILES_DBID]);
            using (var filedb = new DbManager(FILES_DBID))            
            {
                var q = new SqlQuery("files_file f")
                    .Select("b.right_node")
                    .SelectSum("f.content_length")
                    .InnerJoin("files_folder_tree t", Exp.EqColumns("f.folder_id", "t.folder_id"))
                    .InnerJoin("files_bunch_objects b", Exp.EqColumns("t.parent_id", "b.left_node"))
                    .Where("b.tenant_id", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                    .Where(Exp.Like("b.right_node", "crm/crm_common/", SqlLike.StartWith))
                    .GroupBy(1);

                var size = filedb.ExecuteList(q).Select(r =>  Convert.ToInt64(r[1])).FirstOrDefault();
                
                
                    data.Add(new UsageSpaceStatItem()
                    {
                         
                        Name = Resources.CRMCommonResource.ProductName,
                        SpaceUsage = size,
                        Url = VirtualPathUtility.ToAbsolute(PathProvider.StartURL())
                    });
            }        

            return data;
        }
    }
}
