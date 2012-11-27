using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Service;
using ASC.Web.Community.Product;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.PhotoManager.Model
{
    public class ImageStorage2 : IImageStorage
    {
        private DbManager dbManager;

        private string dbId;

        private int tenant = 0;


        public INotifyClient NotifyClient
        {
            get;
            private set;
        }

        public INotifySource NotifySource
        {
            get;
            private set;
        }


        public ImageStorage2(string dbId, int tenant)
        {
            this.dbId = dbId;
            this.tenant = tenant;

            NotifySource = new PhotoManagerNotifySource();
            NotifyClient = WorkContext.NotifyContext.NotifyService.RegisterClient(NotifySource);
        }

        #region Photo Events

        public List<Event> GetEvents(int offset, int count)
        {
            return DbManager
                .ExecuteList(GetEventsQuery().Select(Mappers.EventColumns).SetFirstResult(offset).SetMaxResults(count))
                .ConvertAll(r => Mappers.ToEvent(r));
        }

        public long GetEventsCount()
        {
            return DbManager.ExecuteScalar<long>(GetEventsQuery().SelectCount());
        }

        public Event GetEvent(long id)
        {
            var events = DbManager
                .ExecuteList(Query("photo_event").Select(Mappers.EventColumns).Where("Id", id))
                .ConvertAll(r => Mappers.ToEvent(r));

            return 0 < events.Count ? events[0] : null;
        }

        public void SaveEvent(Event ev)
        {
            CommunitySecurity.DemandPermissions(PhotoConst.Action_AddEvent);

            if (ev == null) throw new ArgumentNullException("event");

            ev.Id = DbManager.ExecuteScalar<long>(
                Insert("photo_event")
                .InColumns(Mappers.EventColumns)
                .Values(ev.Id, ev.Name, ev.Description, ev.UserID, PrepareTimestamp(ev.Timestamp))
                .Identity<long>(1, 0, true)
            );

            var subscriber = NotifySource.GetSubscriptionProvider();
            subscriber.Subscribe(PhotoConst.NewEventComment, ev.Id.ToString(), NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()));
        }

        public void RemoveEvent(long id)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var albumIds = Query("photo_album a").Select("a.Id").Where("a.Event", id);
                var imageIds = Query("photo_image i").Select("i.Id").Where(Exp.In("i.Album", albumIds));
                DbManager.ExecuteNonQuery(Delete("photo_imageview").Where(Exp.In("Image", imageIds)));
                DbManager.ExecuteNonQuery(Delete("photo_comment").Where(Exp.In("Image", imageIds)));
                DbManager.ExecuteNonQuery(Delete("photo_image").Where(Exp.In("Album", albumIds)));
                DbManager.ExecuteNonQuery(Delete("photo_album").Where("Event", id));
                DbManager.ExecuteNonQuery(Delete("photo_event").Where("Id", id));

                tx.Commit();
            }
        }

        #endregion


        #region Albums

        public List<Album> GetAlbums(long eventId, string userId)
        {
            var query = GetAlbumsQuery(eventId, userId)
                .Select(Mappers.AlbumColumns)
                .OrderBy("Id", true);
            return ToAlbums(DbManager.ExecuteList(query));
        }

        public long GetAlbumsCount(long eventId, string userId)
        {
            return DbManager.ExecuteScalar<long>(GetAlbumsQuery(eventId, userId).SelectCount());
        }

        public List<Album> GetLastAlbums(int count)
        {
            var query = Query("photo_album")
                .Select(Mappers.AlbumColumns)
                .Where(!Exp.Eq("ImagesCount", 0))
                .OrderBy("Timestamp", false)
                .OrderBy("Id", false)
                .SetMaxResults(count);
            return ToAlbums(DbManager.ExecuteList(query));
        }

        public Album GetAlbum(long id)
        {
            var query = Query("photo_album")
                .Select(Mappers.AlbumColumns)
                .Where("Id", id);
            var albums = ToAlbums(DbManager.ExecuteList(query));
            return 0 < albums.Count ? albums[0] : null;
        }

        public void SaveAlbum(Album album)
        {
            SaveAlbum(album, null);
        }

        public void SaveAlbum(Album album, IEnumerable<AlbumItem> newItems)
        {
            if (album == null) throw new ArgumentNullException("album");
            if (album.Event == null) throw new ArgumentNullException("album.Event can not be null.");

            if (album.Id != 0)
            {
                DbManager
                    .ExecuteList(Query("photo_album").Select("ImagesCount", "ViewsCount", "CommentsCount").Where("Id", album.Id))
                    .ForEach(r => { album.ImagesCount = (int)Convert.ToInt64(r[0]); album.ViewsCount = (int)Convert.ToInt64(r[1]); album.CommentsCount = (int)Convert.ToInt64(r[2]); });
            }

            album.Id = DbManager.ExecuteScalar<long>(
                Insert("photo_album")
                .InColumns(Mappers.AlbumColumns)
                .Values(album.Id, album.Caption, album.UserID, album.Event.Id, album.FaceItem != null ? album.FaceItem.Id : 0, DateTime.UtcNow, album.ImagesCount, album.ViewsCount, album.CommentsCount)
                .Identity(1, 0L, true)
            );
            if (newItems != null && newItems.Count() > 0)
            {
                NotifyAlbumSave(album, newItems);
            }

        }

        private void NotifyAlbumSave(Album currentAlbum, IEnumerable<AlbumItem> newItems)
        {
            var initiatorInterceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            try
            {
                NotifyClient.AddInterceptor(initiatorInterceptor);
                NotifyClient.BeginSingleRecipientEvent("photo uploaded");

                var albumUrl = UrlHelper.GetAbsoluteAlbumUrl(currentAlbum.Id);
                var eventUrl = UrlHelper.GetAbsoluteEventUrl(currentAlbum.Event.Id);
                var userName = DisplayUserSettings.GetFullUserName(SecurityContext.CurrentAccount.ID);
                var userUrl = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(SecurityContext.CurrentAccount.ID, ASC.Web.Community.Product.CommunityProduct.ID));

                NotifyClient.SendNoticeAsync(
                    PhotoConst.NewPhotoUploaded,
                    null,
                    null,
                    new[]{
                        new TagValue(PhotoConst.TagEventName, currentAlbum.Event.Name),
                        new TagValue(PhotoConst.TagUserName, userName),
                        new TagValue(PhotoConst.TagUserURL, userUrl),
                        new TagValue(PhotoConst.TagPhotoCount, newItems.Count()),
                        new TagValue(PhotoConst.TagDate, string.Format("{0:d} {0:t}", TenantUtil.DateTimeNow())),
                        new TagValue(PhotoConst.TagURL, albumUrl),
                        new TagValue(PhotoConst.TagEventUrl, eventUrl),
                        new TagValue("PHOTO_UPLOAD", true)});
            }
            finally
            {
                NotifyClient.EndSingleRecipientEvent("photo uploaded");
                NotifyClient.RemoveInterceptor(initiatorInterceptor.Name);
            }

            PhotoUserActivityPublisher.AddPhoto(currentAlbum, SecurityContext.CurrentAccount.ID, newItems.ToList());
        }

        public void RemoveAlbum(long albumId)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var imageIds = Query("photo_image i").Select("i.Id").Where("i.Album", albumId);
                DbManager.ExecuteNonQuery(Delete("photo_imageview").Where(Exp.In("Image", imageIds)));
                DbManager.ExecuteNonQuery(Delete("photo_comment").Where(Exp.In("Image", imageIds)));
                DbManager.ExecuteNonQuery(Delete("photo_image").Where("Album", albumId));
                RemoveAlbumInternal(albumId);
                tx.Commit();
            }
        }

        #endregion


        #region Image

        public List<AlbumItem> GetAlbumItems(Album album)
        {
            if (album == null) return new List<AlbumItem>();

            var images = DbManager
                .ExecuteList(Query("photo_image").Select(Mappers.ImageColumns).Where("Album", album.Id).OrderBy("Name", true).OrderBy("Id", true))
                .ConvertAll(r => Mappers.ToImage(r));

            images.ForEach(i =>
            {
                i.Album = album;
                if (i.Id == album.FaceImageId) album.FaceItem = i;
            });

            return images;
        }

        public List<AlbumItem> SearchAlbumItems(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<AlbumItem>();

            var words = new List<string>(text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAll(w => w.Length < 3);
            words.ForEach(w => w.Trim());

            var query = Query("photo_image").Select("Id");
            words.ForEach(w => query.Where(Exp.Like("Name", w)));
            var ids = DbManager
                .ExecuteList(query)
                .ConvertAll<long>(r => Convert.ToInt64(r[0]));

            query = Query("photo_comment").Select("Image").GroupBy(1);
            words.ForEach(w => query.Where(Exp.Like("Text", w)));
            DbManager
                .ExecuteList(query)
                .ConvertAll<long>(r => Convert.ToInt64(r[0]))
                .ForEach(id => { if (!ids.Contains(id)) ids.Add(id); });

            return ids.ConvertAll<AlbumItem>(id => GetAlbumItem(id));
        }

        public List<AlbumItem> GetAlbumItemsLastCommented(int start, int count)
        {
            return DbManager
                .ExecuteList(
                    Query("photo_comment")
                    .Select("Image")
                    .GroupBy(1)
                    .OrderBy("max(Timestamp)", false)
                    .SetFirstResult(start).SetMaxResults(count))
                .ConvertAll<long>(r => Convert.ToInt64(r[0]))
                .ConvertAll<AlbumItem>(id => GetAlbumItem(id));
        }

        public List<Comment> GetLatestCommentsByImage(long ImageId, int count)
        {
            return DbManager
                           .ExecuteList(
                               Query("photo_comment")
                               .Select("Id")
                               .Where("Image", ImageId)
                               .OrderBy("Timestamp", false)
                               .SetMaxResults(count))
                           .ConvertAll<long>(r => Convert.ToInt64(r[0]))
                           .ConvertAll<Comment>(id => GetComment(id));
        }

        public long GetAlbumItemsLastCommentedCount()
        {
            return DbManager.ExecuteScalar<long>(new SqlQuery().SelectCount().From(Query("photo_comment").Select("Image").GroupBy(1), "t"));
        }

        public AlbumItem GetAlbumItem(long id)
        {
            var images = DbManager
                .ExecuteList(Query("photo_image").Select(Mappers.ImageColumns).Where("Id", id))
                .ConvertAll(r => Mappers.ToImage(r));

            if (images.Count == 0) return null;

            var image = images[0];
            image.Album = GetAlbum(image.AlbumId);
            if (image.Album != null && image.Album.FaceImageId == image.Id) image.Album.FaceItem = image;

            return image;
        }

        public AlbumItem GetAlbumItem(Album album, int index)
        {
            if (album == null) return null;

            var images = DbManager
                .ExecuteList(
                    Query("photo_image")
                    .Select(Mappers.ImageColumns)
                    .Where("Album", album.Id)
                    .OrderBy("Name", true)
                    .OrderBy("Id", true)
                    .SetMaxResults(1).SetFirstResult(index))
                .ConvertAll(r => Mappers.ToImage(r));

            if (images.Count == 0) return null;

            var image = images[0];
            image.Album = album;
            if (album != null && album.FaceImageId == image.Id) album.FaceItem = image;

            return image;
        }

        public void SaveAlbumItem(AlbumItem i)
        {
            if (i == null) throw new ArgumentNullException("image");

            if (i.Id != 0)
            {
                DbManager
                    .ExecuteList(Query("photo_image").Select("ViewsCount", "CommentsCount").Where("Id", i.Id))
                    .ForEach(r => { i.ViewsCount = (int)Convert.ToInt64(r[0]); i.CommentsCount = (int)Convert.ToInt64(r[1]); });
            }

            using (var tx = DbManager.BeginTransaction())
            {
                i.Id = DbManager.ExecuteScalar<long>(
                    Insert("photo_image")
                    .InColumns(Mappers.ImageColumns)
                    .Values(i.Id, i.Album.Id, i.Name, i.Description, i.Location, DateTime.UtcNow, i.UserID, i.ThumbnailSize.Width, i.ThumbnailSize.Height, /*i.OriginalSize.Width, i.OriginalSize.Height,*/ i.PreviewSize.Width, i.PreviewSize.Height, i.ViewsCount, i.CommentsCount)
                    .Identity(1, 0L, true)
                );
                UpdateAlbumImagesCount(i.Album.Id);

                tx.Commit();
            }
        }

        public void ReadAlbumItem(long imageId, string userId)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var isReaded = DbManager.ExecuteScalar<long>(Query("photo_imageview").SelectCount().Where(Exp.Eq("Image", imageId) & Exp.Eq("User", userId)));
                if (0 < isReaded)
                {
                    DbManager.ExecuteNonQuery(
                        Update("photo_imageview")
                        .Set("Timestamp", DateTime.UtcNow)
                        .Where(Exp.Eq("Image", imageId) & Exp.Eq("User", userId))
                    );
                }
                else
                {
                    DbManager.ExecuteNonQuery(
                        Insert("photo_imageview")
                        .InColumns("Image", "User", "Timestamp")
                        .Values(imageId, userId, DateTime.UtcNow)
                    );
                    DbManager.ExecuteNonQuery(
                        Update("photo_image")
                        .Set("ViewsCount = ViewsCount + 1")
                        .Where("Id", imageId)
                    );
                    DbManager.ExecuteNonQuery(
                        Update("photo_album")
                        .Set("ViewsCount = ViewsCount + 1")
                        .Where("Id", GetAlbumByImage(imageId))
                    );
                }
                tx.Commit();
            }
        }

        public void RemoveAlbumItem(long imageId)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var albumId = GetAlbumByImage(imageId);

                DbManager.ExecuteNonQuery(Delete("photo_imageview").Where("Image", imageId));
                DbManager.ExecuteNonQuery(Delete("photo_comment").Where("Image", imageId));
                DbManager.ExecuteNonQuery(Delete("photo_image").Where("Id", imageId));

                if (UpdateAlbumImagesCount(albumId) == 0)
                {
                    RemoveAlbumInternal(albumId);
                }
                else
                {
                    UpdateAlbumCommentsCount(albumId);
                }

                tx.Commit();
            }
        }

        #endregion


        #region Comment

        public List<Comment> GetComments(long imageId)
        {
            var comments = DbManager
                .ExecuteList(Query("photo_comment").Select(Mappers.CommentColumns).Where("Image", imageId))
                .ConvertAll(r => Mappers.ToComment(r));

            var timestamp = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(DbManager.ExecuteScalar<DateTime>(
                Query("photo_imageview")
                .Select("Timestamp")
                .Where("User", SecurityContext.CurrentAccount.ID.ToString())
                .Where("Image", imageId)));

            comments.ForEach(c =>
            {
                if (c.ParentId != 0)
                {
                    var parent = comments.Find(c2 => c2.Id == c.ParentId);
                    if (parent != null) parent.Comments.Add(c);
                }
                c.IsRead = c.Timestamp < timestamp;
            });
            comments.RemoveAll(c => c.ParentId != 0);

            return comments;
        }

        public Comment GetComment(long id)
        {
            var comments = DbManager
                .ExecuteList(Query("photo_comment").Select(Mappers.CommentColumns).Where("Id", id))
                .ConvertAll(r => Mappers.ToComment(r));

            return 0 < comments.Count ? comments[0] : null;
        }

        public long SaveComment(AlbumItem image, Comment comment)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                comment.Id = DbManager.ExecuteScalar<long>(
                    Insert("photo_comment")
                    .InColumns(Mappers.CommentColumns)
                    .Values(comment.Id, comment.Text, comment.UserID, PrepareTimestamp(comment.Timestamp), comment.ItemID, comment.ParentId, comment.Inactive)
                    .Identity(1, 0L, true)
                );
                var count = UpdateImageCommentsCount(comment.ItemID);

                tx.Commit();

                NotifyCommentAdd(image, comment);
                return count;
            }
        }

        private void NotifyCommentAdd(AlbumItem image, Comment newComment)
        {
            var initiatorInterceptor = new InitiatorInterceptor(new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""));
            try
            {
                NotifyClient.AddInterceptor(initiatorInterceptor);

                var albumUrl = UrlHelper.GetAbsoluteAlbumUrl(image.Album.Id);
                var albumName = DisplayUserSettings.GetFullUserName(new Guid(image.UserID));
                var eventUrl = UrlHelper.GetAbsoluteEventUrl(image.Album.Event.Id);
                var userName = DisplayUserSettings.GetFullUserName(new Guid(newComment.UserID));
                var userUrl = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(new Guid(newComment.UserID), ASC.Web.Community.Product.CommunityProduct.ID));
                var commentsUrl = UrlHelper.GetAbsolutePhotoUrl(image.Id);
                var commentText = newComment.Text;

                NotifyClient.SendNoticeAsync(
                            PhotoConst.NewEventComment,
                            image.Album.Event != null ? image.Album.Event.Id.ToString() : "0",
                            null,
                            new TagValue(PhotoConst.TagEventName, image.Album.Event.Name),
                            new TagValue(PhotoConst.TagUserName, userName),
                            new TagValue(PhotoConst.TagUserURL, userUrl),
                            new TagValue(PhotoConst.TagEventUrl, eventUrl),
                            new TagValue(PhotoConst.TagAlbumName, albumName),
                            new TagValue(PhotoConst.TagAlbumURL, albumUrl),
                            new TagValue(PhotoConst.TagPhotoName, image.Name),
                            new TagValue(PhotoConst.TagDate, string.Format("{0:d} {0:t}", newComment.Timestamp)),
                            new TagValue(PhotoConst.TagCommentBody, commentText),
                            new TagValue(PhotoConst.TagURL, commentsUrl));
            }
            finally
            {
                NotifyClient.RemoveInterceptor(initiatorInterceptor.Name);
            }

            PhotoUserActivityPublisher.AddComment(image, newComment);
        }

        public void RemoveComment(long commentId)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var imageId = DbManager.ExecuteScalar<long>(Query("photo_comment").Select("Image").Where("Id", commentId));
                DbManager.ExecuteNonQuery(Delete("photo_comment").Where("Parent", commentId));
                DbManager.ExecuteNonQuery(Delete("photo_comment").Where("Id", commentId));
                UpdateImageCommentsCount(imageId);

                tx.Commit();
            }
        }

        #endregion

        #region News

        /// <summary>
        /// Get photo comments posted during time interval
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<CommentNews> GetComments(DateTime from, DateTime to)
        {
            var query = new SqlQuery("photo_comment c")
                .Select("c.Id", "c.Text", "c.User", "c.Timestamp", "c.Image", "c.Parent", "c.Inactive", "i.Name", "i.Location", "i.User", "i.Album")
                .LeftOuterJoin("photo_image i", Exp.EqColumns("i.Id", "c.Image"))
                .Where(Exp.Between("c.Timestamp", from, to) & Exp.Eq("c.Tenant", tenant))
                .OrderBy("c.Timestamp", false);
            return DbManager.ExecuteList(query).Select(x => Mappers.ToCommentNews(x)).ToList();
        }

        /// <summary>
        /// Get photos added during time interval
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<AlbumItem> GetPhotos(DateTime from, DateTime to)
        {
            var query = new SqlQuery("photo_image i")
                .Select("i.Id", "i.Album", "i.Name", "i.Description", "i.Location", "i.Timestamp", "i.User")
                .Select("a.Caption")
                .Select("fi.Location", "fi.User")
                .LeftOuterJoin("photo_album a", Exp.EqColumns("a.Id", "i.Album"))
                .LeftOuterJoin("photo_image fi", Exp.EqColumns("a.FaceImage", " fi.Id"))
                .Where(Exp.Between("i.Timestamp", from, to) & Exp.Eq("i.Tenant", tenant));
            return DbManager.ExecuteList(query).Select(x => Mappers.ToImageNews(x)).ToList();
        }

        #endregion

        private DbManager DbManager
        {
            get
            {
                if (dbManager == null)
                {
                    dbManager = HttpContext.Current != null ? DbManager.FromHttpContext(dbId) : new DbManager(dbId);
                }
                return dbManager;
            }
        }

        private SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumn(table), tenant);
        }

        private SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumnValue(GetTenantColumn(table), tenant);
        }

        private SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumn(table), tenant);
        }

        private SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumn(table), tenant);
        }

        private string GetTenantColumn(string table)
        {
            return table.Contains(" ") ?
                table.Substring(table.IndexOf(" ")).Trim() + ".Tenant" :
                "Tenant";
        }


        private SqlQuery GetEventsQuery()
        {
            return Query("photo_event e")
                .Where(Exp.In("e.Id", Query("photo_album a").Select("a.Event").Where(!Exp.EqColumns("a.ImagesCount", "0"))))
                .OrderBy("e.Timestamp", false);
        }


        private SqlQuery GetAlbumsQuery(long eventId, string userId)
        {
            var query = Query("photo_album").Where(!Exp.EqColumns("ImagesCount", "0"));
            if (eventId != 0) query.Where("Event", eventId);
            if (!string.IsNullOrEmpty(userId)) query.Where("User", userId);
            return query;
        }

        private List<Album> ToAlbums(List<object[]> rawAlbums)
        {
            var eventIds = new List<long>();
            var faceIds = new List<long>();
            var albums = new List<Album>(rawAlbums.Count);
            foreach (var row in rawAlbums)
            {
                albums.Add(Mappers.ToAlbum(row));

                var eventId = Convert.ToInt64(row[3]);
                if (!eventIds.Contains(eventId)) eventIds.Add(eventId);

                var faceId = Convert.ToInt64(row[4]);
                if (!faceIds.Contains(faceId)) faceIds.Add(faceId);
            }

            var events = DbManager
                .ExecuteList(Query("photo_event").Select(Mappers.EventColumns).Where(Exp.In("Id", eventIds.OrderBy(id => id).ToArray())))
                .ConvertAll(r => Mappers.ToEvent(r));

            var faces = DbManager
                .ExecuteList(Query("photo_image").Select(Mappers.ImageColumns).Where(Exp.In("Id", faceIds.OrderBy(id => id).ToArray())))
                .ConvertAll(r => Mappers.ToImage(r));

            albums.ForEach(a =>
            {
                a.Event = events.Find(e => e.Id == a.EventId);
                a.FaceItem = faces.Find(f => f.Id == a.FaceImageId);
                if (a.FaceItem != null) a.FaceItem.Album = a;
            });

            return albums;
        }

        private void RemoveAlbumInternal(long id)
        {
            var eventId = DbManager.ExecuteScalar<long>(Query("photo_album").Select("Event").Where("Id", id));
            DbManager.ExecuteNonQuery(Delete("photo_album").Where("Id", id));

            var eventsCount = DbManager.ExecuteScalar<long>(Query("photo_album").SelectCount().Where("Event", eventId));
            if (eventsCount == 0) DbManager.ExecuteNonQuery(Delete("photo_event").Where("Id", eventId));
        }

        private long UpdateAlbumImagesCount(long id)
        {
            var count = DbManager.ExecuteScalar<long>(
                Query("photo_image").SelectCount().Where("Album", id)
            );
            DbManager.ExecuteNonQuery(
                Update("photo_album")
                .Set("Timestamp", DateTime.UtcNow)
                .Set("ImagesCount", count)
                .Where("Id", id));

            return count;
        }

        private long UpdateAlbumCommentsCount(long id)
        {
            var count = DbManager.ExecuteScalar<long>(
                Query("photo_comment").SelectCount().Where(Exp.In("Image", Query("photo_image i").Select("i.Id").Where("i.Album", id)))
            );
            DbManager.ExecuteNonQuery(
                Update("photo_album")
                .Set("Timestamp", DateTime.UtcNow)
                .Set("CommentsCount", count)
                .Where("Id", id));

            return count;
        }

        private long GetAlbumByImage(long imageId)
        {
            return DbManager.ExecuteScalar<long>(Query("photo_image").Select("Album").Where("Id", imageId));
        }


        private long UpdateImageCommentsCount(long imageId)
        {
            var count = DbManager.ExecuteScalar<long>(Query("photo_comment").SelectCount().Where("Image", imageId));
            DbManager.ExecuteNonQuery(Update("photo_image").Set("CommentsCount", count).Where("Id", imageId));
            UpdateAlbumCommentsCount(GetAlbumByImage(imageId));
            return count;
        }

        private DateTime PrepareTimestamp(DateTime timestamp)
        {
            if (timestamp < new DateTime(1971, 1, 1) || timestamp > new DateTime(2037, 1, 1)) return DateTime.UtcNow;
            return TenantUtil.DateTimeToUtc(timestamp);
        }
    }
}