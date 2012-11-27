using System;
using System.Collections.Generic;
using System.Text;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Bookmarking.Resources;
using System.Web;
using ASC.Bookmarking.Common;

namespace ASC.Bookmarking.Business.Statistics
{
	public class BookmarkingStatisticProvider : IStatisticProvider
	{

		private long GetBookmarksCount(Guid userID)
		{			
			var service = BookmarkingService.GetCurrentInstanse();
			return service.GetBookmarksCountCreatedByUser(userID);			
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
				Count = GetBookmarksCount(userID),
				URL = VirtualPathUtility.ToAbsolute(BookmarkingBusinessConstants.UserBookmarksPageName) + "?uid=" + userID,
				Name = BookmarkingBusinessResources.MainStatisticName
			};
		}

		#endregion
	}
}
