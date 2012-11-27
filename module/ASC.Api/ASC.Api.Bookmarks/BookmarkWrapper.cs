using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Bookmarking.Business;
using ASC.Bookmarking.Pojo;
using ASC.Specific;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Util;

namespace ASC.Api.Bookmarks
{
    [DataContract(Name = "bookmark", Namespace = "")]
    public class BookmarkWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public string Url { get; set; }

        [DataMember(Order = 3)]
        public string Thumbnail { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }
        

        public BookmarkWrapper(Bookmark bookmark)
        {
            Id=bookmark.ID;
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(bookmark.UserCreatorID)); 
            Title = bookmark.Name;
            Url = bookmark.URL;
            Description = bookmark.Description;
            Updated = Created = (ApiDateTime) bookmark.Date;
            Thumbnail = ThumbnailHelper.Instance.GetThumbnailUrl(bookmark.URL, BookmarkingSettings.ThumbSmallSize);
        }

        private BookmarkWrapper()
        {
        }

        public static BookmarkWrapper GetSample()
        {
            return new BookmarkWrapper()
            {
                Id = 11,
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Updated = (ApiDateTime)DateTime.Now,
                Description = "Google",
                Thumbnail = "Url to thumbnail",
                Title = "Google inc.",
                Url = "http://www.google.com"
            };
        }
    }
}