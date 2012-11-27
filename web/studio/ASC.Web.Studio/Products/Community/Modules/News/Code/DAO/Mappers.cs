using System;
using ASC.Core.Tenants;

namespace ASC.Web.Community.News.Code.DAO
{
	static class Mappers
	{
		public static Feed ToFeed(object[] row)
		{
			return new Feed()
			{
				Id = Convert.ToInt64(row[0]),
				FeedType = (FeedType)Convert.ToInt32(row[1]),
				Caption = Convert.ToString(row[2]),
				Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[3])),
				Creator = Convert.ToString(row[4]),
				Readed = Convert.ToBoolean(row[5]),
			};
		}

		public static Feed ToFeedFinded(object[] row)
		{
			return new Feed()
			{
				Id = Convert.ToInt64(row[0]),
				Caption = Convert.ToString(row[1]),
				Text = Convert.ToString(row[2]),
                Date =TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[3]))
			};
		}

		public static Feed ToNewsOrPoll(object[] row)
		{
			var feedType = (FeedType)Convert.ToInt32(row[1]);
			var feed = feedType == FeedType.Poll ? (Feed)new FeedPoll() : (Feed)new FeedNews();

			feed.Id = Convert.ToInt64(row[0]);
			feed.FeedType = feedType;
			feed.Caption = Convert.ToString(row[2]);
			feed.Text = Convert.ToString(row[3]);
			feed.Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[4]));
			feed.Creator = Convert.ToString(row[5]);
			feed.Readed = Convert.ToBoolean(row[6]);
			return feed;
		}

		public static FeedComment ToFeedComment(object[] row)
		{
			return new FeedComment(Convert.ToInt64(row[1]))
			{
				Id = Convert.ToInt64(row[0]),
				Comment = Convert.ToString(row[2]),
				ParentId = Convert.ToInt64(row[3]),
				Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[4])),
				Creator = Convert.ToString(row[5]),
				Inactive = Convert.ToBoolean(row[6]),
			};
		}
	}
}
