using System;

namespace ASC.Web.Community.News.Code
{
	[Flags]
	public enum FeedType
	{
		News = 1,
		Order = 2,
		Advert = 4,
		Poll = 8,

		None = 0,
		AllNews = News | Advert | Order,
		All = AllNews | Poll,
	}
}