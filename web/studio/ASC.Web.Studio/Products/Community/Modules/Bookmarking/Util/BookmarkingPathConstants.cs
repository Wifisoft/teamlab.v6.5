using System.Web;

namespace ASC.Web.Community.Bookmarking.Util
{
	public class BookmarkingPathConstants
	{
		public static readonly string BookmarkingPageUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/default.aspx");

		public static readonly string FavouriteBookmarksPageUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/favouritebookmarks.aspx");

		public static readonly string TagsPageUrl = VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/tags.aspx");		
	}
}
