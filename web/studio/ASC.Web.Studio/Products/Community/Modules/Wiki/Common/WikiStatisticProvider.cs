using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.Community.Wiki.Common
{
    public class WikiStatisticProvider : IStatisticProvider
    {
        private int GetPostCount(Guid userID)
        {
            return new WikiEngine().GetPagesCount(userID);
        }

        #region IStatisticProvider Members
    
        public List<StatisticItem> GetAllStatistic(Guid userID)
        {
            return new List<StatisticItem>();
        }
        
        public StatisticItem GetMainStatistic(Guid userID)
        {
            return new StatisticItem()
            {
                Count = GetPostCount(userID),
                URL = VirtualPathUtility.ToAbsolute(string.Format("{0}/ListPages.aspx", WikiManager.BaseVirtualPath)).ToLower() + "?uid=" + userID,
                Name = WikiResource.WikiPageCount
            };
        }

        #endregion
    }
}
