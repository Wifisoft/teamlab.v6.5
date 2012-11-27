using System;
using System.Web;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking.Common.Search
{
	public class BookmarkingSearchHandler : BaseSearchHandlerEx
	{

		private static readonly Guid BookmarkingModuleID = new Guid("28B10049-DD20-4f54-B986-873BC14CCFC7");

		public override string AbsoluteSearchURL
		{
			get
			{
				return VirtualPathUtility.ToAbsolute(PlaceVirtualPath + "/default.aspx");
			}
		}

		public override ASC.Web.Core.Utility.Skins.ImageOptions Logo
		{
			get {
				return new ImageOptions() { ImageFileName = "bookmarking_mini_icon.png", PartID =  ModuleID} ;
			}
		}

		public override string PlaceVirtualPath
		{
			get { return BookmarkingRequestConstants.BookmarkingBasePath; }
		}

		public override string SearchName
		{
			get { return BookmarkingUCResource.BookmarkingSearch; }
		}

		public override ASC.Web.Core.ModuleManagement.Common.SearchResultItem[] Search(string text)
		{
			return BookmarkingServiceHelper.GetCurrentInstanse().SearchBookmarksBySearchString(text);
		}

		public override Guid ModuleID
		{
			get { return BookmarkingModuleID; }
		}

		public override Guid ProductID
		{
			get { return new Guid("EA942538-E68E-4907-9394-035336EE0BA8"); }
		}

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
	}
}
