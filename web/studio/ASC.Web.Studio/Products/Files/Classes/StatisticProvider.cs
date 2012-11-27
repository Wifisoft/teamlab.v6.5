using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Files.Classes;

namespace ASC.Web.Files.Configuration
{
    public class StatisticProvider : IStatisticProvider
    {
        public StatisticItem GetMainStatistic(Guid userID)
        {
            var folder = Global.DaoFactory.GetFolderDao().GetFolder(Global.FolderMy);

            return new StatisticItem
                       {
                           Name = folder.Title,
                           Count = folder.TotalFiles,
                           URL = PathProvider.GetFolderUrl(folder.ID, false, null)
                           //AdditionalData 
                           //Description 
                       };
        }

        public List<StatisticItem> GetAllStatistic(Guid userID)
        {
            var folders = Global.DaoFactory.GetFolderDao().GetFolders(new[]
                                                                          {
                                                                              Global.FolderMy,
                                                                              Global.FolderShare,
                                                                              Global.FolderCommon,
                                                                              Global.FolderTrash
                                                                          });


            return new List<StatisticItem>(folders.Select(folder => new StatisticItem
                                                                        {
                                                                            Name = folder.Title,
                                                                            Count = folder.TotalFiles,
                                                                            URL = PathProvider.GetFolderUrl(folder.ID, false, null)
                                                                            //AdditionalData 
                                                                            //Description 
                                                                        }));

        }
    }
}