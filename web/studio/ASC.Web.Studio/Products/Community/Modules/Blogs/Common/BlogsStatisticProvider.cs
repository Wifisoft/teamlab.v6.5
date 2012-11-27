using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ASC.Web.Core.ModuleManagement.Common;
using System.Collections.Generic;

namespace ASC.Web.Community.Blogs
{
    public class BlogsStatisticProvider 
        : IStatisticProvider
    {
        private long GetPostCount(Guid userID)
        {
            return BasePage.GetEngine().GetPostCountByAuthor(userID);
        }

        #region IStatisticProvider Members

        public List<StatisticItem> GetAllStatistic(Guid userID)
        {
            return new List<StatisticItem> { GetMainStatistic(userID) };
        }

        public StatisticItem GetMainStatistic(Guid userID)
        {
            return new StatisticItem()
            {
                Count = GetPostCount(userID),
                URL = ASC.Blogs.Core.Constants.UserPostsPageUrl+userID.ToString(),
                Name = ASC.Blogs.Core.Resources.BlogsResource.MainStatisticName
            };
        }

        #endregion
    }
}
