using System.IO;
using System.Text;
using System.Web.UI;
using ASC.Bookmarking.Pojo;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.UserControls.Bookmarking
{
	public partial class BookmarkRaitingUserControl : BookmarkInfoBase
	{

		private long _simpleModeRaiting;

		public long SimpleModeRaiting
		{
			get
			{
				return Bookmark == null ? _simpleModeRaiting : Raiting;
			}
			set
			{
				_simpleModeRaiting = value;
			}
		}

		/// <summary>
		/// ID of the div which contains raiting.
		/// This field is used to the copying raiting to the Add bookmark to Favourites panel.
		/// </summary>
		public string DivId { get; set; }

		public bool SimpleMode { get; set; }

		private string _singleBookmarkDivID;

		public string SingleBookmarkDivID
		{
			get
			{
				return _singleBookmarkDivID == null ? string.Empty : _singleBookmarkDivID;
			}
			set
			{
				_singleBookmarkDivID = value;
			}
		}

		public string GetUniqueIDFromSingleBookmark()
		{
			return GetUniqueIDFromSingleBookmark(SingleBookmarkDivID);
		}

		public bool FavouriteMode
		{
			get
			{
				return ASC.Web.UserControls.Bookmarking.Common.Presentation.BookmarkingServiceHelper.BookmarkDisplayMode.Favourites.Equals(ServiceHelper.DisplayMode);
			}
		}

		public string GetBookmarkRaiting(Bookmark b)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
				{
					using (var c = LoadControl(BookmarkUserControlPath.BookmarkRaitingUserControlPath) as BookmarkRaitingUserControl)
					{
						c.SimpleModeRaiting = ServiceHelper.GetUserBookmarksCount(b);
						c.SimpleMode = true;
						c.Bookmark = b;
						c.RenderControl(textWriter);
					}
				}
			}
			return sb.ToString();
		}

		public string GetBookmarkRaiting(Bookmark b, string divID)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
				{
					using (var c = LoadControl(BookmarkUserControlPath.BookmarkRaitingUserControlPath) as BookmarkRaitingUserControl)
					{
						c.Bookmark = b;
						c.DivId = divID;
						c.RenderControl(textWriter);
					}
				}
			}
			return sb.ToString();
		}

		public string GetBookmarkRaiting(Bookmark b, string divID, string singleBookmarkDivID)
		{
			return GetBookmarkRaiting(b, null, divID, singleBookmarkDivID);
		}

		public string GetBookmarkRaiting(Bookmark b, UserBookmark ub, string divID, string singleBookmarkDivID)
		{			
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
				{
					using (var c = LoadControl(BookmarkUserControlPath.BookmarkRaitingUserControlPath) as BookmarkRaitingUserControl)
					{
						c.Bookmark = b;
						c.UserBookmark = ub;
						c.DivId = divID;
						c.SingleBookmarkDivID = singleBookmarkDivID;
						c.RenderControl(textWriter);
					}
				}
			}
			return sb.ToString();
		}



		public override void InitUserControl() {	}		
	}
}