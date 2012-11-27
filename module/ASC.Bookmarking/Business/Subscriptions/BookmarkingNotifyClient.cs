using System;
using System.Collections.Generic;
using System.Text;
using ASC.Notify;

namespace ASC.Bookmarking.Business.Subscriptions
{
	public class BookmarkingNotifyClient
	{
		public static INotifyClient NotifyClient { get; private set; }

		static BookmarkingNotifyClient()
		{
			NotifyClient = ASC.Core.WorkContext.NotifyContext.NotifyService.RegisterClient(BookmarkingNotifySource.Instance);
		}
	}
}
