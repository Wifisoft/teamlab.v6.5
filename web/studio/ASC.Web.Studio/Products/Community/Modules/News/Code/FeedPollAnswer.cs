using System;
using System.Collections.Generic;

namespace ASC.Web.Community.News.Code
{
	[Serializable]
	public class FeedPollAnswer
	{
		public long VariantId
		{
			get;
			private set;
		}

		public string UserId
		{
			get;
			private set;
		}

		public FeedPollAnswer(long variantId, string userId)
		{
			VariantId = variantId;
			UserId = userId;
		}
	}
}