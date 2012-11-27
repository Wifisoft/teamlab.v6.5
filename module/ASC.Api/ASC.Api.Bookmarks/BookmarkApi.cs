using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Impl;
using ASC.Bookmarking.Business;
using ASC.Bookmarking.Common;
using ASC.Bookmarking.Dao;
using ASC.Bookmarking.Pojo;
using ASC.Common.Data;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Api.Bookmarks
{
    ///<summary>
    /// Bookmark access
    ///</summary>
    public class BookmarkApi: Interfaces.IApiEntryPoint
    {
        private ApiContext _context;

        ///<summary>
        /// entry point
        ///</summary>
        public string Name
        {
            get { return "bookmark"; }
        }

        private BookmarkingService _bookmarkingDao;

        private BookmarkingService BookmarkingDao
        {
            get
            {
                if (_bookmarkingDao==null)
                {
                    _bookmarkingDao = BookmarkingService.GetCurrentInstanse();
                }
                return _bookmarkingDao;
            }
        }


        ///<summary>
        ///</summary>
        ///<param name="context"></param>
        public BookmarkApi(ApiContext context)
        {
            _context = context;
        }

		  ///<summary>
		  ///Returns the list of all bookmarks on the portal with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///All bookmarks
		  ///</short>
		  ///<returns>List of bookmarks</returns>
        [Read("")]
        public IEnumerable<BookmarkWrapper> GetBookmarks()
        {
            var bookmarks = BookmarkingDao.GetAllBookmarks((int) _context.StartIndex, (int) _context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all bookmarks for the current user with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///Added by me
		  ///</short>
		  ///<returns>List of bookmarks</returns>
        [Read("@self")]
        public IEnumerable<BookmarkWrapper> GetMyBookmarks()
        {
            var bookmarks = BookmarkingDao.GetBookmarksCreatedByUser(SecurityContext.CurrentAccount.ID,(int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns a list of bookmarks matching the search query with the bookmark title, date of creation and update, bookmark description and author
		  ///</summary>
		  ///<short>
		  ///Search
		  ///</short>
		  /// <param name="query">search query</param>
        ///<returns>List of bookmarks</returns>
        [Read("@search/{query}")]
        public IEnumerable<BookmarkWrapper> SearchBookmarks(string query)
        {
            var bookmarks = BookmarkingDao.SearchBookmarks(new List<string>() { query }, (int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of favorite bookmarks for the current user with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///My favorite
		  ///</short>
		  ///<returns>List of bookmarks</returns>
        [Read("@favs")]
        public IEnumerable<BookmarkWrapper> GetFavsBookmarks()
        {
            var bookmarks = BookmarkingDao.GetFavouriteBookmarksSortedByDate((int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns a list of all tags used for bookmarks with the number showing the tag usage
		  ///</summary>
		  ///<short>
		  ///All tags
		  ///</short>
		  ///<categroy>Tags</categroy>
        ///<returns>List of tags</returns>
        [Read("tag")]
        public IEnumerable<TagWrapper> GetTags()
        {
            var bookmarks = BookmarkingDao.GetTagsCloud();
            return bookmarks.Select(x => new TagWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all bookmarks marked by the tag specified with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///By tag
		  ///</short>
		  ///<param name="tag">tag</param>
        ///<returns>List of bookmarks</returns>
        [Read("tag/{tag}")]
        public IEnumerable<BookmarkWrapper> GetBookmarksByTag(string tag)
        {
            var bookmarks = BookmarkingDao.SearchBookmarksByTag(tag, (int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of recenty added bookmarks with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///Recently added
		  ///</short>
		  ///<returns>List of bookmarks</returns>
        [Read("top/recent")]
        public IEnumerable<BookmarkWrapper> GetRecentBookmarks()
        {
            var bookmarks = BookmarkingDao.GetMostRecentBookmarks((int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of the bookmarks most popular on the current date with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///Top of day
		  ///</short>
		  ///<returns>List of bookmarks</returns>
        [Read("top/day")]
        public IEnumerable<BookmarkWrapper> GetTopDayBookmarks()
        {
            var bookmarks = BookmarkingDao.GetTopOfTheDay((int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of the bookmarks most popular in the current month with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///Top of month
		  ///</short>
		  ///<returns>List of bookmarks</returns>
        [Read("top/month")]
        public IEnumerable<BookmarkWrapper> GetTopMonthBookmarks()
        {
            var bookmarks = BookmarkingDao.GetTopOfTheMonth((int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }
		  ///<summary>
		  ///Returns the list of the bookmarks most popular on the current week with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///Top of week
		  ///</short>
		  ///<returns>list of bookmarks</returns>
        [Read("top/week")]
        public IEnumerable<BookmarkWrapper> GetTopWeekBookmarks()
        {
            var bookmarks = BookmarkingDao.GetTopOfTheWeek((int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }
		  ///<summary>
		  ///Returns the list of the bookmarks most popular in the current year with the bookmark titles, date of creation and update, bookmark text and author
		  ///</summary>
		  ///<short>
		  ///Top of year
		  ///</short>
		  ///<returns>list of bookmarks</returns>
        [Read("top/year")]
        public IEnumerable<BookmarkWrapper> GetTopYearBookmarks()
        {
            var bookmarks = BookmarkingDao.GetTopOfTheYear((int)_context.StartIndex, (int)_context.Count);
            _context.SetDataPaginated();
            return bookmarks.Select(x => new BookmarkWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  /// Returns the list of all comments to the bookmark with the specified ID
		  ///</summary>
		  ///<short>
		  /// Get comments
		  ///</short>
		  ///<category>Comments</category>
        ///<param name="id">ID of bookmark</param>
        ///<returns>list of bookmark comments</returns>
        [Read("{id}/comment")]
        public IEnumerable<BookmarkCommentWrapper> GetBookmarkComments(long id)
        {
            var comments = BookmarkingDao.GetBookmarkComments(BookmarkingDao.GetBookmarkByID(id));
            return comments.Select(x => new BookmarkCommentWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  /// Adds a comment to the bookmark with the specified ID. The parent bookmark ID can be also specified if needed.
		  ///</summary>
		  ///<short>
		  /// Add comment
		  ///</short>
		  ///<category>Comments</category>
        ///<param name="id">Bookmark ID</param>
        ///<param name="content">comment content</param>
        ///<param name="parentId">parent comment ID</param>
        ///<returns>list of bookmark comments</returns>
        /// <example>
        /// <![CDATA[
        /// Sending data in application/json:
        /// 
        /// {
        ///     content:"My comment",
        ///     parentId:"9924256A-739C-462b-AF15-E652A3B1B6EB"
        /// }
        /// 
        /// Sending data in application/x-www-form-urlencoded
        /// content="My comment"&parentId="9924256A-739C-462b-AF15-E652A3B1B6EB"
        /// ]]>
        /// </example>
        /// <remarks>
        /// Send parentId=00000000-0000-0000-0000-000000000000 or don't send it at all if you want your comment to be on the root level
        /// </remarks>
        [Create("{id}/comment")]
        public BookmarkCommentWrapper AddBookmarkComment(long id, string content, Guid parentId)
        {
            var comment = new Comment()
                              {
                                        ID = Guid.NewGuid(),
                                      BookmarkID = id,
                                      Content = content,
                                      Datetime = DateTime.UtcNow,
                                      UserID = SecurityContext.CurrentAccount.ID,
                                      Parent = parentId.ToString()
                                  };
            BookmarkingDao.AddComment(comment);
            return new BookmarkCommentWrapper(comment);
        }
		  ///<summary>
		  /// Returns a detailed information on the bookmark with the specified ID
		  ///</summary>
		  ///<short>
		  /// Get bookmarks by ID
		  ///</short>
		  ///<param name="id">ID of bookmark</param>
        ///<returns>bookmark</returns>
        [Read("{id}")]
        public BookmarkWrapper GetBookmarkById(long id)
        {
            return new BookmarkWrapper(BookmarkingDao.GetBookmarkByID(id));
        }

		  ///<summary>
		  /// Adds a bookmark with a specified title, description and tags
		  ///</summary>
		  ///<short>
		  /// Add a bookmark
		  ///</short>
		  ///<param name="url">url of bookmarking page</param>
        ///<param name="title">title to show</param>
        ///<param name="description">description</param>
        ///<param name="tags">tags. separated by semicolon</param>
        ///<returns>newly added bookmark</returns>
        /// <example>
        /// <![CDATA[
        /// Sending data in application/json:
        /// 
        /// {
        ///     url:"www.teamlab.com",
        ///     title: "TeamLab",
        ///     description: "best site i've ever seen",
        ///     tags: "project management, collaboration"
        /// }
        /// 
        /// Sending data in application/x-www-form-urlencoded
        /// url="www.teamlab.com"&title="TeamLab"&description="best site i've ever seen"&tags="project management, collaboration"
        /// ]]>
        /// </example>
        [Create("")]
        public BookmarkWrapper AddBookmark(string url, string title, string description, string tags)
        {
            var bookmark = new Bookmark(url, Core.Tenants.TenantUtil.DateTimeNow(), title, description) { UserCreatorID = SecurityContext.CurrentAccount.ID};
            BookmarkingDao.AddBookmark(bookmark, !string.IsNullOrEmpty(tags)?tags.Split(',').Select(x=>new Tag(){Name = x}).ToList():new List<Tag>());
            return new BookmarkWrapper(bookmark);
        }
    }
}
