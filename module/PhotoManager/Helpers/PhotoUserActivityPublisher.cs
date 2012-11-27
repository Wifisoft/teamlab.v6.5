using System;
using System.Collections.Generic;
using System.Text;
using ASC.Core;
using ASC.Data.Storage;
using ASC.PhotoManager.Data;
using ASC.Web.Core.Users.Activity;

namespace ASC.PhotoManager.Helpers
{
    public class PhotoUserActivityPublisher: BaseUserActivityPublisher
    {
        internal static int CurrentTenantID
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        internal static void PublishInternal(UserActivity activity)
        {
            UserActivityPublisher.Publish<PhotoUserActivityPublisher>(activity);
        }

        internal static string GetPhotoContentID(AlbumItem item)
        {
            return String.Format("photo#{0}", item.Id);
        }

        internal static string GetAlbumContentID(Album album)
        {
            return String.Format("album#{0}", album.Id);
        }

        internal static UserActivity ComposeActivityByPhoto(AlbumItem item)
        {
            UserActivity ua = new UserActivity();
            ua.TenantID = CurrentTenantID;
            ua.ContentID = GetPhotoContentID(item);
            ua.Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            ua.ModuleID = PhotoConst.ModuleID;
            ua.ProductID = ASC.Web.Community.Product.CommunityProduct.ID;
            ua.Title = item.Name;
            ua.URL = String.Format("{0}?photo={1}", PhotoConst.ViewPhotoPageUrl, item.Id);

            return ua;
        }

        internal static UserActivity ComposeActivityByPhotos(Album album)
        {
            UserActivity ua = new UserActivity();
            ua.ContentID = GetAlbumContentID(album);
            ua.TenantID = CurrentTenantID;
            ua.Date = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            ua.ModuleID = PhotoConst.ModuleID;
            ua.ProductID = ASC.Web.Community.Product.CommunityProduct.ID;
            ua.Title = album.Event.Name;
            ua.URL = String.Format("{0}?item={1}", PhotoConst.ViewAlbumPageUrl, album.Id);

            return ua;
        }

        internal static UserActivity ApplyCustomeActivityParams(UserActivity ua, string actionText, Guid userID, int actionType, int businessValue)
        {
            ua.ActionText = actionText;
            ua.UserID = userID;
            ua.ActionType = actionType;
            ua.BusinessValue = businessValue;

            return ua;
        }
        
        
        public static void AddPhoto(Album album, Guid authorID, IList<AlbumItem> images)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhotos(album),
                    Resources.PhotoManagerResource.UserActivity_AddPhoto,
                    authorID,
                    UserActivityConstants.ContentActionType,
                    PhotoConst.AddPhotoBusinessValue 
                );
            var builder = new StringBuilder();
            IDataStore store = ASC.Data.Storage.StorageFactory.GetStorage(CoreContext.TenantManager.GetCurrentTenant().TenantId.ToString(), "photo");
            foreach (var albumItem in images)
            {
                builder.AppendFormat("<img style=\"margin:10px;\" src=\"{0}\"/>",
                                     ImageHTMLHelper.GetImageUrl(albumItem.ExpandedStoreThumb, store));
            }
            ua.HtmlPreview = builder.ToString();
            PublishInternal(ua);
        }

        public static void EditPhoto(Album album, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhotos(album),
                    Resources.PhotoManagerResource.UserActivity_EditPhoto,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    PhotoConst.EditPhotoBusinessValue);

            PublishInternal(ua);
        }

        public static void DeletePhoto(Album album, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhotos(album),
                    Resources.PhotoManagerResource.UserActivity_DeletePhoto,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    PhotoConst.DeletePhotoBusinessValue);

            PublishInternal(ua);
        }

        public static void Evaluate(AlbumItem image, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhoto(image),
                    Resources.PhotoManagerResource.UserActivity_Evaluate,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    PhotoConst.EvaluateBusinessValue);

            PublishInternal(ua);
        }


        public static void RemoveComment(AlbumItem item, Comment comment, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhoto(item),
                    Resources.PhotoManagerResource.UserActivity_DeleteComment,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    PhotoConst.DeleteCommentBusinessValue);
            PublishInternal(ua);
        }

        public static void AddComment(AlbumItem item, Comment newComment)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhoto(item),
                    Resources.PhotoManagerResource.UserActivity_AddComment,
                    new Guid (newComment.UserID),
                    UserActivityConstants.ActivityActionType,
                    PhotoConst.AddCommentBusinessValue);
            ua.HtmlPreview = newComment.Text;
            PublishInternal(ua);
        }

		public static void UpdateComment(AlbumItem item, Comment comment, Guid authorID)
        {
            var ua =
                ApplyCustomeActivityParams(
                    ComposeActivityByPhoto(item),
                    Resources.PhotoManagerResource.UserActivity_EditComment,
                    authorID,
                    UserActivityConstants.ActivityActionType,
                    PhotoConst.EditCommentBusinessValue);
            PublishInternal(ua);
        }

    }
}