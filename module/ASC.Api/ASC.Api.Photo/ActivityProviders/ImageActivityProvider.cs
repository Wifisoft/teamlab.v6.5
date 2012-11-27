using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Feed.Activity;
using ASC.Feed.ActivityProvider;
using ASC.Feed.Utils;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Model;

namespace ASC.Api.Photo.ActivityProviders
{
    public class ImageActivityProvider : IActivityProvider
    {
        private const string Images = "images";

        public string SourceName
        {
            get { return Images; }
        }

        private IImageStorage _storage;
        private IImageStorage ImageStorage
        {
            get
            {
                if (_storage == null)
                {
                    var dbId = StorageFactory.Id;
                    if (!DbRegistry.IsDatabaseRegistered(dbId))
                    {
                        DbRegistry.RegisterDatabase(dbId, WebConfigurationManager.ConnectionStrings[dbId]);
                    }
                    _storage = StorageFactory.GetStorage();
                }
                return _storage;
            }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var comments = ImageStorage.GetComments(range.From, range.To);
            var albums = ImageStorage.GetPhotos(range.From, range.To)
                .GroupBy(x => x.UserID)
                .ToDictionary(x => x.Key, y => y.GroupBy(z => z.Album));

            var activities = new List<Activity>(comments
                .Select(x => new Activity(Images, new { Comment = new PhotoAlbumItemCommentWrapper(x.Comment), Image = new PhotoAlbumItemWrapper(x.Image) }, new Guid(x.Comment.UserID), x.Comment.Timestamp,
                    (range.In(x.Comment.Timestamp) ? ActivityAction.Created : ActivityAction.Undefined) | (x.Comment.ParentId == 0 ? ActivityAction.Commented : ActivityAction.Reply))
                {ItemType = "comment"}));
            foreach (var album in albums)
            {
                activities.AddRange(album.Value
                    .Select(x => new Activity(Images, new { Album = new PhotoAlbumWrapper(x.Key), Uploaded = x.Select(y => new PhotoAlbumItemWrapper(y)) }, new Guid(album.Key), null,
                        (range.In(x.Key.LastUpdate) ? ActivityAction.Created : ActivityAction.Undefined))
                    {ItemType = "album"}));
            }

            return activities;
        }
    }
}
