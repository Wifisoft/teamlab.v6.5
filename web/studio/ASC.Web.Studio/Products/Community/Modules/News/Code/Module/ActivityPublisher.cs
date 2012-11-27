using System;
using System.Globalization;
using ASC.Core.Tenants;
using ASC.Web.Community.News.Resources;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News.Code.Module
{
	public class ActivityPublisher : BaseUserActivityPublisher
	{
		internal static void PublishInternal(UserActivity activity)
		{
			UserActivityPublisher.Publish<ActivityPublisher>(activity);
		}

		internal static string GetContentID(Feed feed)
		{
			return String.Format(CultureInfo.CurrentCulture, "feed#{0}", feed.Id);
		}

		internal static UserActivity ComposeActivityByFeed(Feed post)
		{
			var ua = new UserActivity();
			ua.TenantID = TenantProvider.CurrentTenantID;
			ua.ContentID = GetContentID(post);
			ua.Date = TenantUtil.DateTimeNow();
			ua.ModuleID = NewsConst.ModuleId;
			ua.ProductID = Product.CommunityProduct.ID;
			ua.Title = post.Caption;
			ua.URL = FeedUrls.GetFeedVirtualPath(post.Id);

			return ua;
		}

		internal static UserActivity ApplyCustomeActivityParams(Feed feed, UserActivity ua, string actionText, Guid userID, int actionType, int businessValue)
		{
			ua.ImageOptions = new ImageOptions();
			ua.ImageOptions.PartID = NewsConst.ModuleId;
			ua.ImageOptions.ImageFileName = FeedTypeInfo.FromFeedType(feed.FeedType).TypeLogoPath;
			ua.ActionText = actionText;
			ua.UserID = userID;
			ua.ActionType = actionType;
			ua.BusinessValue = businessValue;
			return ua;
		}

		public static void AddFeed(Feed post)
		{
			string action = string.Empty;
			if (post is FeedNews)
			{
				action = NewsResource.UserActivity_AddNewsFeed;
			}
			else if (post is FeedPoll)
			{
				action = NewsResource.UserActivity_AddPollFeed;
			}
			if (!string.IsNullOrEmpty(action))
			{
				UserActivity ua = ApplyCustomeActivityParams(
					post,
					ComposeActivityByFeed(post),
					action,
					new Guid(post.Creator),
					UserActivityConstants.ContentActionType,
					UserActivityConstants.NormalContent
				);
			    ua.HtmlPreview = post.Text;
				PublishInternal(ua);
			}
		}

		public static void EditPost(Feed post, Guid authorId)
		{
			string action = string.Empty;
			if (post is FeedNews)
			{
				action = NewsResource.UserActivity_EditNewsFeed;
			}
			else if (post is FeedPoll)
			{
				action = NewsResource.UserActivity_EditPollFeed;
			}
			if (!string.IsNullOrEmpty(action))
			{
				UserActivity ua =
						ApplyCustomeActivityParams(post,
							ComposeActivityByFeed(post),
							action,
							authorId,
							UserActivityConstants.ActivityActionType,
							UserActivityConstants.SmallActivity);

				PublishInternal(ua);
			}
		}


		public static void DeletePost(Feed post, Guid author)
		{
			string action = string.Empty;
			if (post is FeedNews)
			{
				action = NewsResource.UserActivity_DeleteFeed;
			}
			else if (post is FeedPoll)
			{
				action = NewsResource.UserActivity_DeletePoll;
			}
			if (!string.IsNullOrEmpty(action))
			{
				UserActivity ua =
					ApplyCustomeActivityParams(post,
						ComposeActivityByFeed(post),
						action,
						author,
						UserActivityConstants.ActivityActionType,
						UserActivityConstants.SmallActivity);

				PublishInternal(ua);
			}
		}

        public static void AddFeedComment(FeedComment comment, Feed post, Guid author)
		{
			string action = string.Empty;
			if (post is FeedNews)
			{
				action = NewsResource.UserActivity_AddCommentNews;
			}
			else if (post is FeedPoll)
			{
				action = NewsResource.UserActivity_AddCommentFeed;
			}
			if (!string.IsNullOrEmpty(action))
			{
				UserActivity ua =
						ApplyCustomeActivityParams(post,
							ComposeActivityByFeed(post),
							action,
							author,
							UserActivityConstants.ActivityActionType,
							UserActivityConstants.NormalActivity
					);
			    ua.HtmlPreview = comment.Comment;
				PublishInternal(ua);
			}
		}

		public static void EditFeedComment(Feed post, Guid author)
		{
			string action = string.Empty;
			if (post is FeedNews)
			{
				action = NewsResource.UserActivity_EditCommentNews;
			}
			else if (post is FeedPoll)
			{
				action = NewsResource.UserActivity_EditCommentFeed;
			}
			if (!string.IsNullOrEmpty(action))
			{
				UserActivity ua =
				   ApplyCustomeActivityParams(post,
					   ComposeActivityByFeed(post),
					   action,
					   author,
					   UserActivityConstants.ActivityActionType,
					   UserActivityConstants.SmallActivity
					   );
				PublishInternal(ua);
			}
		}

		public static void DeleteFeedComment(Feed post, Guid author)
		{
			string action = string.Empty;
			if (post is FeedNews)
			{
				action = NewsResource.UserActivity_DeleteCommentNews;
			}
			else if (post is FeedPoll)
			{
				action = NewsResource.UserActivity_DeleteCommentFeed;
			}
			if (!string.IsNullOrEmpty(action))
			{
				UserActivity ua =
						ApplyCustomeActivityParams(post,
							ComposeActivityByFeed(post),
							action,
							author,
							UserActivityConstants.ActivityActionType,
							UserActivityConstants.SmallActivity
					);

				PublishInternal(ua);
			}
		}

		public static void Voted(Feed post, Guid authorId)
		{
			UserActivity ua =
					ApplyCustomeActivityParams(post,
						ComposeActivityByFeed(post),
						NewsResource.UserActivity_Vote,
						authorId,
						UserActivityConstants.ActivityActionType,
						UserActivityConstants.SmallActivity);

			PublishInternal(ua);
		}

	    
	}
}