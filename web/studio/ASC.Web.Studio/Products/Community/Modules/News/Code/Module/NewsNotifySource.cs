using System;
using ASC.Core;
using ASC.Core.Notify;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.Community.News.Resources;

namespace ASC.Web.Community.News.Code.Module
{
	public enum FeedSubscriptionType
	{
		NewFeed = 0,
		NewOrder = 1
	}
	
	public class NewsNotifyClient
	{
		public static INotifyClient NotifyClient { get; private set; }

		static NewsNotifyClient()
		{
			NotifyClient = WorkContext.NotifyContext.NotifyService.RegisterClient(NewsNotifySource.Instance);
		}
	}

	public class NewsNotifySource : NotifySource, IDependencyProvider
	{
		private static NewsNotifySource _instance;

		public static NewsNotifySource Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (typeof(NewsNotifySource))
					{
						if (_instance == null) _instance = new NewsNotifySource();
					}
				}
				return _instance;
			}
		}

		private NewsNotifySource()
            : base(new Guid("{6504977C-75AF-4691-9099-084D3DDEEA04}"))
		{

		}

		protected override IActionPatternProvider CreateActionPatternProvider()
		{
			return new XmlActionPatternProvider(
					GetType().Assembly,
					"ASC.Web.Community.News.Resources.accordings.xml",
					ActionProvider, 
					PatternProvider 
				);

		}


		protected override IActionProvider CreateActionProvider()
		{
			return new ConstActionProvider(NewsConst.NewFeed, NewsConst.NewComment);
		}

		protected override IDependencyProvider CreateDependencyProvider()
		{
			return this;
		}

		protected override IPatternProvider CreatePatternsProvider()
		{
            return new XmlPatternProvider(NewsPatternsResource.news_patterns);
		}
	}
}