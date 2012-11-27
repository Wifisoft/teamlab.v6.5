using System;
using System.Collections.Generic;

namespace ASC.Web.Community.News.Code.DAO
{
	public interface IFeedStorage : IDisposable
	{
		List<FeedType> GetUsedFeedTypes();

		
		List<Feed> GetFeeds(FeedType feedType, Guid userId, int count, int offset);

		List<Feed> SearchFeeds(string s, FeedType feedType, Guid userId, int count, int offset);

		long GetFeedsCount(FeedType feedType, Guid userId);

		long SearchFeedsCount(string s, FeedType feedType, Guid userId);

		List<Feed> SearchFeeds(string s);

		Feed GetFeed(long id);

	    List<Feed> GetFeedByDate(DateTime from, DateTime to, Guid userId);

	    List<FeedComment> GetCommentsByDate(DateTime from, DateTime to);

		Feed SaveFeed(Feed feed, bool isEdit, FeedType poll);
		
		void RemoveFeed(long id);

		void ReadFeed(long feedId, string reader);

		void PollVote(string userId, ICollection<long> variantIds);

		
		List<FeedComment> GetFeedComments(long feedId);
		
		FeedComment GetFeedComment(long commentId);
		
		void RemoveFeedComment(long commentId);
        FeedComment SaveFeedComment(Feed feed, FeedComment comment);
        void RemoveFeedComment(FeedComment comment);
        void UpdateFeedComment(FeedComment comment);
    }
}