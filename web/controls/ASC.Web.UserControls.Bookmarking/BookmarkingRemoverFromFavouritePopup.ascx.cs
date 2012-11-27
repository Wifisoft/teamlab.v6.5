using System;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking
{
	public partial class BookmarkingRemoverFromFavouritePopup : System.Web.UI.UserControl
	{

		public const string Location = "~/Products/Community/Modules/Bookmarking/Controls/BookmarkingRemoverFromFavouritePopup.ascx";

		protected void Page_Load(object sender, EventArgs e)
		{
			BookmarkingRemoveFromFavouriteContainer.Options.IsPopup = true;

			BookmarkingRemoveFromFavouriteLink.ButtonText = BookmarkingUCResource.RemoveButton;
			BookmarkingRemoveFromFavouriteLink.AjaxRequestText = BookmarkingUCResource.RemovingBookmarkFromFavouriteIsInProgressLabel;
		}
	}
}