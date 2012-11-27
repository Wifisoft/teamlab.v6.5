using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Web.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;

namespace ASC.Web.Files
{
    public class FilesSpaceUsageStatManager : SpaceUsageStatManager
    {
        private const string PROJECTS_DBID = "projects";


        public override List<UsageSpaceStatItem> GetStatData()
        {
            if (!DbRegistry.IsDatabaseRegistered(PROJECTS_DBID))
            {
                DbRegistry.RegisterDatabase(PROJECTS_DBID, ConfigurationManager.ConnectionStrings[PROJECTS_DBID]);
            }
            if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
            {
                DbRegistry.RegisterDatabase(FileConstant.DatabaseId, ConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
            }
            using (var db = new DbManager(FileConstant.DatabaseId))
            {
                var q = new SqlQuery("files_file f")
                    .Select("f.create_by")
                    .SelectSum("f.content_length")
                    .InnerJoin("files_folder_tree t", Exp.EqColumns("f.folder_id", "t.folder_id"))
                    .InnerJoin("files_bunch_objects b", Exp.EqColumns("t.parent_id", "b.left_node"))
                    .Where("b.tenant_id", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                    .Where(Exp.Like("b.right_node", "files/my/", SqlLike.StartWith))
                    .GroupBy(1);

                var result = db.ExecuteList(q)
                    .Select(
                        r =>
                        new
                            {
                                User = CoreContext.UserManager.GetUsers(new Guid((string) r[0])),
                                Size = Convert.ToInt64(r[1])
                            })
                    .GroupBy(r => r.User).ToDictionary(g => g.Key, g => g.Sum(a => a.Size));

                q = new SqlQuery("files_file f")
                    .SelectSum("f.content_length")
                    .InnerJoin("files_folder_tree t", Exp.EqColumns("f.folder_id", "t.folder_id"))
                    .InnerJoin("files_bunch_objects b", Exp.EqColumns("t.parent_id", "b.left_node"))
                    .Where("b.tenant_id", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                    .Where("b.right_node", "files/common/");

                var common = db.ExecuteScalar<long>(q);
                if (result.ContainsKey(Constants.LostUser))
                {
                    result[Constants.LostUser] += common;
                }
                else
                {
                    result.Add(Constants.LostUser, common);
                }
                return result.Select(i => new UsageSpaceStatItem
                                              {
                                                  Name = i.Key.Equals(Constants.LostUser) ? FilesUCResource.CorporateFiles : i.Key.DisplayUserName(false),
                                                  ImgUrl = i.Key.Equals(Constants.LostUser) ? PathProvider.GetImagePath("corporatefiles_big.png") : i.Key.GetSmallPhotoURL(),
                                                  Url = i.Key.Equals(Constants.LostUser) ? PathProvider.GetFolderUrl(Global.FolderCommon, false, null) : i.Key.GetUserProfilePageURL(),
                                                  SpaceUsage = i.Value
                                              })
                    .OrderByDescending(t => t.SpaceUsage).ToList();
            }
        }
    }
}