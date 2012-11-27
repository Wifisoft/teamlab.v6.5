using System;
using ASC.Web.Community.News.Resources;

namespace ASC.Web.Community.News.Code
{
	class FeedTypeInfo
	{
		public int Id
		{
			get;
			private set;
		}

		public string TypeName
		{
			get;
			private set;
		}

		public string TypeLogoPath
		{
			get;
			private set;
		}

		public FeedTypeInfo(FeedType feedType, string name, string logo)
		{
			Id = (int)feedType;
			TypeName = name;
			TypeLogoPath = logo;
		}

		public static FeedTypeInfo FromFeedType(FeedType feedType)
		{
			switch (feedType)
			{
				case FeedType.News: return new FeedTypeInfo(feedType, NewsResource.FeedTypeNews, "32x_news.png");
				case FeedType.Order: return new FeedTypeInfo(feedType, NewsResource.NewsOrdersTypeName, "32x_order.png");
                case FeedType.Advert: return new FeedTypeInfo(feedType, NewsResource.NewsAnnouncementsTypeName, "32x_advert.png");
                case FeedType.Poll: return new FeedTypeInfo(feedType, NewsResource.FeedTypePoll, "32x_poll.png");
				default: throw new ArgumentOutOfRangeException(string.Format("Unknown feed type: {0}.", feedType));
			}
		}
	}
}
