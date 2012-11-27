using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Community.News.Code.Module
{
	public class StatisticProvider : IStatisticProvider
	{
		private static long GetPostCount(Guid userID)
		{
			return FeedStorageFactory.Create().GetFeedsCount(FeedType.All, userID);
		}

		public List<StatisticItem> GetAllStatistic(Guid userID)
		{
			return new List<StatisticItem>();
		}

		public StatisticItem GetMainStatistic(Guid userID)
		{
			return new StatisticItem()
			{
				Count = GetPostCount(userID),
				URL = FeedUrls.GetFeedListUrl(userID),
				Name = Resources.NewsResource.PostCount
			};
		}
	}
}