using System;
using System.Collections.Generic;
using System.Web;
using ASC.Forum;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Forum
{
    public class ForumStatisticProvider : IStatisticProvider
    {
        #region IStatisticProvider Members

        public List<StatisticItem> GetAllStatistic(Guid userID)
        {
            return new List<StatisticItem>();
        }

        public StatisticItem GetMainStatistic(Guid userID)
        {
            return new StatisticItem
                       {
                           Count = ForumDataProvider.GetUserPostCount(TenantProvider.CurrentTenantID, userID),
                           URL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/usertopics.aspx") + "?uid=" + userID.ToString(),
                           Name = Resources.ForumResource.PostCountStatistic
                       };
        }

        #endregion
    }
}