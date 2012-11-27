using System;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.Specific;
using ASC.Web.Community.PhotoManager;

namespace ASC.Api.Photo
{
    [DataContract(Name = "item", Namespace = "")]
    public class PhotoAlbumItemWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 10)]
        public string Location { get; set; }

        [DataMember(Order = 10)]
        public string Thumbnail { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public long Comments { get; set; }

        public PhotoAlbumItemWrapper(AlbumItem faceItem)
        {
            Id = faceItem.Id;
            Title = faceItem.Name;
            Updated = Created = (ApiDateTime)faceItem.Timestamp;

            Description = faceItem.Description;
            var storage =
                Data.Storage.StorageFactory.GetStorage(
                    CoreContext.TenantManager.GetCurrentTenant().TenantId.ToString(), "photo");
            Thumbnail = ImageHTMLHelper.GetImageUrl(faceItem.ExpandedStorePreview, storage);
            Location = ImageHTMLHelper.GetImageUrl(faceItem.ExpandedStoreThumb, storage);
            Comments = faceItem.CommentsCount;
        }

        private PhotoAlbumItemWrapper()
        {

        }

        public static PhotoAlbumItemWrapper GetSample()
        {
            return new PhotoAlbumItemWrapper()
                       {
                           Comments = 1234,
                           Created = (ApiDateTime)DateTime.Now,
                           Description = "sample description",
                           Id = 10,
                           Location = "somewhere",
                           Thumbnail = "url to thumbnail",
                           Title = "sample title",
                           Updated = (ApiDateTime)DateTime.Now,
                       };
        }
    }
}