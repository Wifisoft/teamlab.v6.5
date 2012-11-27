using System;
using System.Collections.Generic;
using System.Text;
using ASC.Notify.Model;
using ASC.Core.Notify;
using ASC.Bookmarking.Common;
using ASC.Notify.Patterns;
using ASC.Bookmarking.Resources;

namespace ASC.Bookmarking.Business.Subscriptions
{
	internal class BookmarkingNotifySource : NotifySource, IDependencyProvider
	{

		private static BookmarkingNotifySource _instance = null;

		public static BookmarkingNotifySource Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (typeof(BookmarkingNotifySource))
					{
						if (_instance == null)
							_instance = new BookmarkingNotifySource();
					}
				}

				return _instance;
			}
		}

		private BookmarkingNotifySource()
            : base(BookmarkingBusinessConstants.BookmarkingCommunityModuleId)
        {
            
        }

		protected override IActionPatternProvider CreateActionPatternProvider()
		{
			return new XmlActionPatternProvider(
					GetType().Assembly,
					BookmarkingBusinessConstants.BookmarkingActionPattern,
					ActionProvider, 
					PatternProvider 
				);
		}

		protected override IActionProvider CreateActionProvider()
		{
			return new ConstActionProvider(
					BookmarkingBusinessConstants.NotifyActionNewBookmark,
					BookmarkingBusinessConstants.NotifyActionNewComment,
					BookmarkingBusinessConstants.NotifyActionTag
				);
		}

		protected override ASC.Notify.Patterns.IPatternProvider CreatePatternsProvider()
		{
			return new XmlPatternProvider(BookmarkingSubscriptionPatterns.BookmarkingPatterns);
		}

	}
}
