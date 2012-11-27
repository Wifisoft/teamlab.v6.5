using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.PhotoManager.Data;
using ASC.Specific;

namespace ASC.Api.Photo
{
    [DataContract(Name = "comment", Namespace = "")]
    public class PhotoAlbumItemCommentWrapper : IApiSortableDate
    {
        public PhotoAlbumItemCommentWrapper(Comment comment)
        {
            Author = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(new Guid(comment.UserID)));
            Updated = Created = (ApiDateTime)comment.Timestamp;
            Id = comment.Id;
            Text = comment.Text;
            ParentId = comment.ParentId;
            Comments = comment.Comments.Select(x => new PhotoAlbumItemCommentWrapper(x)).ToList();
        }

        private PhotoAlbumItemCommentWrapper()
        {
        }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Updated { get; set; }


        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public long ParentId { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper Author { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public List<PhotoAlbumItemCommentWrapper> Comments { get; set; }

        public static PhotoAlbumItemCommentWrapper GetSample()
        {
            return new PhotoAlbumItemCommentWrapper()
            {
                Author = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = 10,
                ParentId = 123,
                Text = "comment text",
                Updated = (ApiDateTime)DateTime.Now,
                Comments = new List<PhotoAlbumItemCommentWrapper>() { new PhotoAlbumItemCommentWrapper() { Text = "Child comment" } }
            };
        }
    }
}