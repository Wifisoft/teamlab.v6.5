using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Bookmarking.Common;
using ASC.Bookmarking.Pojo;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Common.Util;
using ASC.Web.UserControls.Bookmarking.Resources;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.UserControls.Bookmarking
{
    public partial class TagsUserControl : System.Web.UI.UserControl
    {
        private BookmarkingServiceHelper ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

        private ViewSwitcher SortControl;

        private BookmarkingSortUtil sortUtil = new BookmarkingSortUtil();

        protected void Page_Load(object sender, EventArgs e)
        {
            SortControl = new ViewSwitcher {SortItemsHeader = BookmarkingUCResource.ShowLabel};
            InitSortControl();
            TagsSortPanel.Controls.Add(SortControl);
            InitTags();
        }

        #region Most Popular Tags

        private void InitTags()
        {
            ServiceHelper.SetPagination();

            IList<Tag> tags;
            switch (sortUtil.SortBy)
            {
                case SortByEnum.Name:
                    tags = ServiceHelper.GetTagsSortedByName();
                    break;
                case SortByEnum.Popularity:
                    tags = ServiceHelper.GetMostPopularTags();
                    break;
                default:
                    tags = ServiceHelper.GetMostRecentTags();
                    break;
            }

            if (tags == null || tags.Count == 0)
            {
                var emptyScreenControl = new EmptyScreenControl
                                             {
                                                 ImgSrc = WebImageSupplier.GetAbsoluteWebPath("bookmarks_icon.png", BookmarkingSettings.ModuleId),
                                                 Header = BookmarkingUCResource.EmptyScreenTagsCaption,
                                                 Describe = BookmarkingUCResource.EmptyScreenText,
                                                 ButtonHTML = String.Format("<a class='linkAddMediumText' href='CreateBookmark.aspx'>{0}</a>", BookmarkingUCResource.EmptyScreenLink)
                                             };

                TagsContainer.Controls.Add(emptyScreenControl);
                TagsSortingPanel.Visible = false;
                return;
            }
            var i = 0;

            var bookmarks = ServiceHelper.GetMostPopularBookmarksByTag(tags);

            foreach (var tag in tags)
            {
                using (var c = LoadControl(BookmarkUserControlPath.TagInfoUserControlPath) as TagInfoUserControl)
                {
                    c.Tag = tag;
                    c.BookmarkList = GetBookmarksByTag(bookmarks, tag);
                    c.IsOdd = i++%2 == 0;
                    TagsContainer.Controls.Add(c);
                }
            }
            SetBookmarkingPagination();
        }

        private static IList<Bookmark> GetBookmarksByTag(IList<Bookmark> bookmarks, Tag tag)
        {

            var r = (from b in bookmarks
                     where b.Tags.Contains(tag)
                     orderby b.UserBookmarks.Count descending
                     select b)
                .Take(BookmarkingBusinessConstants.MostPopularBookmarksByTagLimit)
                .ToList();
            return r;
        }

        #endregion


        #region Init Sort Control

        private void InitSortControl()
        {
            var sortBy = Request.QueryString[BookmarkingRequestConstants.SortByParam];
            SortControl.SortItems = sortUtil.GetTagsSortItems(sortBy);
        }

        #endregion

        #region Pagination

        private void SetBookmarkingPagination()
        {
            var pageNavigator = new PageNavigator();
            ServiceHelper.InitPageNavigator(pageNavigator, ServiceHelper.GetTagsCount());
            BookmarkingPaginationContainer.Controls.Add(pageNavigator);
        }

        #endregion
    }
}