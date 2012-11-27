namespace ASC.Web.UserControls.Bookmarking.Common.Presentation
{
	public static class BookmarkingRequestConstants
	{
		//Page address to be used in a get request
		public const string BookmarkingPageName = "default.aspx";
		public const string Question_Sybmol = "?";
		public const string BookmarkInfoPageName = "bookmarkinfo.aspx";
		public const string FavouriteBookmarksPageName = "favouriteBookmarks.aspx";
		public const string TagsPageName = "tags.aspx";
		public const string CreateBookmarkPageName = "createbookmark.aspx";
		public const string UrlGetRequest = "url";
		public const string SelectedTab = "selectedtab";
		public const string SelectedTabBookmarkCommnets = "bookmarkcommnetstab";
		public const string SelectedTabBookmarkAddedBy = "bookmarkaddedbytab";
		public const string BookmarksCreatedByUserPageName = "userbookmarks.aspx";
		public const string UidParam = "uid";

		//Sorting constants
		public const string SortByParam = "sortby";
		public const string SortByDateParam = "date";
		public const string SortByRaitingParam = "raiting";
		public const string MostRecentParam = "mostrecent";
		public const string TopOfTheDayParam = "topoftheday";
		public const string WeekParam = "week";
		public const string MonthParam = "month";
		public const string YearParam = "year";
		public const string PopularityParam = "popularity";
		public const string CloudParam = "cloud";
		public const string NameParam = "name";

		public const string EqualsSymbol = "=";
		public const string AMPERSAND_SYMBOL = "&";

		//Search
		public const string Search = "search";

		//Tags
		public const string Tag = "tag";

		//URL
		public const string URL_Prefix = "http://";
		public const string URL_HTTPS_Prefix = "https://";
		public const string Default_URL = "http://";

		//Image for the Raiting star
		public const string UserRaitingOne = "goldstar.png";

		//Tags image
		public const string TagsImageName = "tags.png";

		//Pagination
		public const string Pagination = "p";

		//No thumbnail available image
		public const string NoImageAvailable = "noimageavailable.jpg";

		public const string BookmarkingBasePath = "~/products/community/modules/bookmarking";

		public const string BookmarkingStorageManagerID = "bookmarking";
		
	}
}
