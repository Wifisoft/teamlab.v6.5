using System;
using System.Collections.Generic;
using ASC.Bookmarking.Pojo;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.UserControls.Bookmarking
{
	public partial class TagInfoUserControl : System.Web.UI.UserControl
	{
		public Tag Tag { get; set; }

		private IList<Bookmark> _BookmarkList;

		public IList<Bookmark> BookmarkList
		{
			get
			{
				return _BookmarkList;
			}
			set
			{
				_BookmarkList = value;
				BookmarksRepeater.DataSource = value;
				BookmarksRepeater.DataBind();
			}
		}

		public bool IsOdd { get; set; }

		public String Name
		{
			get
			{
				return Tag.Name;
			}
		}		

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public string GetTagsWebPath()
		{
			return WebImageSupplier.GetAbsoluteWebPath("tag_32.png", BookmarkingSettings.ModuleId);
		}

		public string GetBookmarkInfoUrl(object url)
		{
			var stringUrl = (string)url;
			return BookmarkingServiceHelper.GenerateBookmarkInfoUrl(stringUrl);
		}

		public string GetBookmarkRaiting(object bookmark)
		{
			if (bookmark is Bookmark)
			{
				var b = bookmark as Bookmark;
				return new BookmarkRaitingUserControl().GetBookmarkRaiting(b);
			}
			return string.Empty;
		}

		public string GetSearchByTagUrl()
		{
			if (Tag != null && !string.IsNullOrEmpty(Tag.Name))
			{
				return BookmarkingServiceHelper.GetCurrentInstanse().GetSearchByTagUrl(Tag);
			}
			return string.Empty;
		}
	}
}