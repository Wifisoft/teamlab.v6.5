using System.Collections.Generic;
using ASC.Web.Controls;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking.Common.Util
{
	public class BookmarkingSortUtil
	{
		public SortByEnum? SortBy { get; set; }

		private BookmarkingServiceHelper serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

		#region Sort Items
		private ViewSwitcherLinkItem MostRecentSortItem
		{
			get
			{				
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.MostRecent == SortBy,
					SortUrl = serviceHelper.GenerateSortUrl(BookmarkingRequestConstants.MostRecentParam),
					SortLabel = BookmarkingUCResource.MostRecentLabel
				};
			}
		}

		private ViewSwitcherLinkItem TopOfTheDaySortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.TopOfTheDay == SortBy,
					SortUrl = serviceHelper.GenerateSortUrl(BookmarkingRequestConstants.TopOfTheDayParam),
					SortLabel = BookmarkingUCResource.TopOfTheDayLabel
				};
			}
		}

		private ViewSwitcherLinkItem WeekSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Week == SortBy,
					SortUrl = serviceHelper.GenerateSortUrl(BookmarkingRequestConstants.WeekParam),
					SortLabel = BookmarkingUCResource.WeekLabel
				};
			}
		}

		private ViewSwitcherLinkItem MonthSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Month == SortBy,
					SortUrl = serviceHelper.GenerateSortUrl(BookmarkingRequestConstants.MonthParam),
					SortLabel = BookmarkingUCResource.MonthLabel
				};
			}
		}

		private ViewSwitcherLinkItem YearSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Year == SortBy,
					SortUrl = serviceHelper.GenerateSortUrl(BookmarkingRequestConstants.YearParam),
					SortLabel = BookmarkingUCResource.YearLabel,
				};
			}
		}

		private ViewSwitcherLinkItem PopularitySortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Popularity == SortBy,
					SortUrl = serviceHelper.GenerateSortUrl(BookmarkingRequestConstants.PopularityParam),
					SortLabel = BookmarkingUCResource.PopularityLabel,
				};
			}
		}

		#region Tags Sort Items
		private ViewSwitcherLinkItem MostRecentTags
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.MostRecent == SortBy,
					SortUrl = serviceHelper.GenerateSortUrlWithPageName(BookmarkingRequestConstants.TagsPageName, BookmarkingRequestConstants.MostRecentParam),
					SortLabel = BookmarkingUCResource.MostRecentLabel
				};
			}
		}

		private ViewSwitcherLinkItem MostPopularTags
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Popularity == SortBy,
					SortUrl = serviceHelper.GenerateSortUrlWithPageName(BookmarkingRequestConstants.TagsPageName, BookmarkingRequestConstants.PopularityParam),
					SortLabel = BookmarkingUCResource.PopularityLabel
				};
			}
		}

		private ViewSwitcherLinkItem TagsCloud
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Cloud == SortBy,
					SortUrl = serviceHelper.GenerateSortUrlWithPageName(BookmarkingRequestConstants.TagsPageName, BookmarkingRequestConstants.CloudParam),
					SortLabel = BookmarkingUCResource.TagsCloudLabel,
				};
			}
		}

		private ViewSwitcherLinkItem SortByTagNameSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Name == SortBy,
					SortUrl = serviceHelper.GenerateSortUrlWithPageName(BookmarkingRequestConstants.TagsPageName, BookmarkingRequestConstants.NameParam),
					SortLabel = BookmarkingUCResource.AtoZ,
				};
			}
		}

		private ViewSwitcherLinkItem SearchBookmarksMostRecentSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.MostRecent == SortBy,
					SortUrl = serviceHelper.GetSearchMostRecentBookmarksUrl(),
					SortLabel = BookmarkingUCResource.MostRecentLabel
				};
			}
		}

		private ViewSwitcherLinkItem SearchBookmarksPopularitySortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Popularity == SortBy,
					SortUrl = serviceHelper.GetSearchMostPopularBookmarksUrl(),
					SortLabel = BookmarkingUCResource.PopularityLabel,
				};
			}
		}

		private ViewSwitcherLinkItem BookmarkCreatedByUserMostRecentSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.MostRecent == SortBy,
					SortUrl = serviceHelper.GetMostRecentBookmarksCreateByUserUrl(),
					SortLabel = BookmarkingUCResource.MostRecentLabel
				};
			}
		}

		private ViewSwitcherLinkItem BookmarkCreatedByUserPopularitySortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Popularity == SortBy,
					SortUrl = serviceHelper.GetMostPopularBookmarksCreateByUserUrl(),
					SortLabel = BookmarkingUCResource.PopularityLabel,
				};
			}
		}
		#endregion

		#region Get Bookmarks By Tag Sort Items

		private ViewSwitcherLinkItem MostRecentBookmarksByTag
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.MostRecent == SortBy,
					SortUrl = serviceHelper.GetSearchMostRecentBookmarksByTagUrl(),
					SortLabel = BookmarkingUCResource.MostRecentLabel
				};
			}
		}

		private ViewSwitcherLinkItem MostPopularBookmarksByTagSortItem
		{
			get
			{
				return new ViewSwitcherLinkItem()
				{
					IsSelected = SortByEnum.Popularity == SortBy,
					SortUrl = serviceHelper.GetSearchMostPopularBookmarksByTagUrl(),
					SortLabel = BookmarkingUCResource.PopularityLabel,
				};
			}
		}

		#endregion

		#endregion

		#region Generate Sort Items Collections
		public List<ViewSwitcherBaseItem> GetMainPageSortItems(SortByEnum sortBy)
		{
			SortBy = sortBy;
			var sortItems = new List<ViewSwitcherBaseItem>();
			sortItems.Add(MostRecentSortItem);
			sortItems.Add(TopOfTheDaySortItem);
			sortItems.Add(WeekSortItem);
			sortItems.Add(MonthSortItem);
			sortItems.Add(YearSortItem);
			return sortItems;
		}

		public List<ViewSwitcherBaseItem> GetFavouriteBookmarksSortItems(SortByEnum sortBy)
		{
			SortBy = sortBy;
			var sortItems = new List<ViewSwitcherBaseItem>();
			sortItems.Add(MostRecentSortItem);
			sortItems.Add(PopularitySortItem);
			return sortItems;
		}

		public List<ViewSwitcherBaseItem> GetTagsSortItems(SortByEnum sortBy)
		{
			SortBy = sortBy;
			var sortItems = new List<ViewSwitcherBaseItem>();
			sortItems.Add(MostRecentTags);
			sortItems.Add(MostPopularTags);
			sortItems.Add(SortByTagNameSortItem);			
			return sortItems;
		}

		public List<ViewSwitcherBaseItem> GetTagsSortItems(string sortBy)
		{
			return GetTagsSortItems(ConvertToSortByEnum(sortBy));
		}

		public List<ViewSwitcherBaseItem> GetBookmarksByTagSortItems(SortByEnum sortBy)
		{
			SortBy = sortBy;
			var sortItems = new List<ViewSwitcherBaseItem>();
			sortItems.Add(MostRecentBookmarksByTag);
			sortItems.Add(MostPopularBookmarksByTagSortItem);
			return sortItems;
		}

		public List<ViewSwitcherBaseItem> GetBookmarksByTagSortItems(string sortBy)
		{
			return GetBookmarksByTagSortItems(ConvertToSortByEnum(sortBy));
		}

		public List<ViewSwitcherBaseItem> GetSearchBookmarksSortItems(SortByEnum sortBy)
		{
			SortBy = sortBy;
			var sortItems = new List<ViewSwitcherBaseItem>();
			sortItems.Add(SearchBookmarksMostRecentSortItem);
			sortItems.Add(SearchBookmarksPopularitySortItem);
			return sortItems;
		}
		public List<ViewSwitcherBaseItem> GetSearchBookmarksSortItems(string sortBy)
		{
			return GetSearchBookmarksSortItems(ConvertToSortByEnum(sortBy));
		}

		public List<ViewSwitcherBaseItem> GetBookmarksCreatedByUserSortItems(string sortBy)
		{
			SortBy = ConvertToSortByEnum(sortBy);
			var sortItems = new List<ViewSwitcherBaseItem>();
			sortItems.Add(BookmarkCreatedByUserMostRecentSortItem);
			sortItems.Add(BookmarkCreatedByUserPopularitySortItem);
			return sortItems;
		}
		#endregion

		#region Converter
		public static SortByEnum ConvertToSortByEnum(string param)
		{
			SortByEnum sortBy = SortByEnum.MostRecent;
			switch (param)
			{
				case BookmarkingRequestConstants.MostRecentParam:
					sortBy = SortByEnum.MostRecent;
					return sortBy;
				case BookmarkingRequestConstants.TopOfTheDayParam:
					sortBy = SortByEnum.TopOfTheDay;
					return sortBy;
				case BookmarkingRequestConstants.WeekParam:
					sortBy = SortByEnum.Week;
					return sortBy;
				case BookmarkingRequestConstants.MonthParam:
					sortBy = SortByEnum.Month;
					return sortBy;
				case BookmarkingRequestConstants.YearParam:
					sortBy = SortByEnum.Year;
					return sortBy;
				case BookmarkingRequestConstants.PopularityParam:
					sortBy = SortByEnum.Popularity;
					return sortBy;
				case BookmarkingRequestConstants.CloudParam:
					sortBy = SortByEnum.Cloud;
					return sortBy;
				case BookmarkingRequestConstants.NameParam:
					sortBy = SortByEnum.Name;
					return sortBy;
			}
			return sortBy;
		} 
		#endregion
	}
}
