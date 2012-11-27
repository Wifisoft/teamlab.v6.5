using System;
using System.Web;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Controls;

namespace ASC.Web.Community.News.Code.Module
{
	public class SearchHandler : BaseSearchHandlerEx
	{
		#region ISearchHandler Members

		public override SearchResultItem[] Search(string text)
		{
			return FeedStorageFactory.Create()
				.SearchFeeds(text)
				.ConvertAll<SearchResultItem>(f => new SearchResultItem()
				{
					Name = f.Caption,
					Description = HtmlUtility.GetText(f.Text, 120),
                    URL = FeedUrls.GetFeedUrl(f.Id),
                    Date = f.Date
				})
				.ToArray();
		}

		#endregion

		public override string AbsoluteSearchURL
		{
			get { return FeedUrls.MainPageUrl; }
		}

		public override ImageOptions Logo
		{
			get { return new ImageOptions() { ImageFileName = "newslogo.png", PartID = NewsConst.ModuleId }; }
		}

		public override string SearchName
		{
			get { return NewsResource.SearchDefaultString; }
		}

		public override Guid ModuleID
		{
			get { return NewsConst.ModuleId; }
		}

		public override string PlaceVirtualPath
		{
			get { return FeedUrls.BaseVirtualPath; }
		}

		public override Guid ProductID
		{
			get { return CommunityProduct.ID; }
		}

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
	}
}