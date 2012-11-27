using System;
using AjaxPro;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;
using ASC.Web.Controls;

namespace ASC.Web.UserControls.Bookmarking
{
	public partial class CreateBookmarkUserControl : System.Web.UI.UserControl
	{

		public bool IsNewBookmark { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Utility.RegisterTypeForAjax(typeof(BookmarkingUserControl));
			Utility.RegisterTypeForAjax(typeof(SingleBookmarkUserControl));
			Utility.RegisterTypeForAjax(typeof(CommentsUserControl));

			InitActionButtons();
		}

		#region Init Action Buttons
		private void InitActionButtons()
		{
			SaveBookmarkButton.ButtonText = BookmarkingUCResource.Save;
			SaveBookmarkButton.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

			SaveBookmarkButtonCopy.ButtonText = BookmarkingUCResource.AddToFavourite;
			SaveBookmarkButtonCopy.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

			AddToFavouritesBookmarkButton.ButtonText = BookmarkingUCResource.AddToFavourite;
			AddToFavouritesBookmarkButton.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

			CheckBookmarkUrlLinkButton.ButtonText = BookmarkingUCResource.CheckBookmarkUrlButton;
			CheckBookmarkUrlLinkButton.AjaxRequestText = BookmarkingUCResource.CheckingUrlIsInProgressLabel;
		}		
		#endregion

		public string NavigateToMainPage
		{
			get
			{
				return BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark.Equals(BookmarkingServiceHelper.GetCurrentInstanse().DisplayMode).ToString().ToLower();
			}
		}

		public bool CreateBookmarkMode
		{
			get
			{
				return BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark.Equals(BookmarkingServiceHelper.GetCurrentInstanse().DisplayMode);
			}
		}

		public bool IsEditMode
		{
			get
			{
				var serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
				return BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark.Equals(serviceHelper.DisplayMode) && serviceHelper.IsCurrentUserBookmark();
			}
		}
	}
}