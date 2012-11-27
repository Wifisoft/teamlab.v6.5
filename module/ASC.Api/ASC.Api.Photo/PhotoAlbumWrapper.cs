using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.PhotoManager.Data;
using ASC.Specific;

namespace ASC.Api.Photo
{
    [DataContract(Name = "album", Namespace = "")]
    public class PhotoAlbumWrapper
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public PhotoAlbumItemWrapper Image { get; set; }

        public PhotoAlbumWrapper(Album album)
        {
            Id=album.Id;
            Title=album.Caption;
            Image = new PhotoAlbumItemWrapper(album.FaceItem);
        }

        private PhotoAlbumWrapper()
        {
        }

        public static PhotoAlbumWrapper GetSample()
        {
            return new PhotoAlbumWrapper()
            {
                Id = 10,
                Title = "Sample title",
                Image = PhotoAlbumItemWrapper.GetSample()
            };
        }
    }
}