using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Web.Community.News.Code
{
	[Serializable]
	public class FeedPoll : Feed
	{
		public FeedPollType PollType
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		public List<FeedPollVariant> Variants
		{
			get;
			private set;
		}

		internal List<FeedPollAnswer> answers;


		public FeedPoll()
		{
			FeedType = FeedType.Poll;
			PollType = FeedPollType.SimpleAnswer;
            StartDate = TenantUtil.DateTimeNow();
			EndDate = StartDate.AddYears(100);
			Variants = new List<FeedPollVariant>();
			answers = new List<FeedPollAnswer>();
		}

		public int GetVariantVoteCount(long variantId)
		{
			return answers.FindAll(a => a.VariantId == variantId).Count;
		}

		public bool IsUserVote(string userId)
		{
			return answers.Exists(a => a.UserId == userId);
		}
	}
}