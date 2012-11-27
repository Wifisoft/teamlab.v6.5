using ASC.Bookmarking.Common;
using ASC.Bookmarking.Pojo;
using ASC.Web.Community.Product;

namespace ASC.Bookmarking.Business.Permissions
{
    public static class BookmarkingPermissionsCheck
    {
        public static bool PermissionCheckCreateBookmark()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkCreateAction);
        }

        public static bool PermissionCheckAddToFavourite()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkAddToFavouriteAction);
        }

        public static bool PermissionCheckRemoveFromFavourite(UserBookmark b)
        {
            return CommunitySecurity.CheckPermissions(new BookmarkPermissionSecurityObject(b.UserID), BookmarkingBusinessConstants.BookmarkRemoveFromFavouriteAction);
        }

        public static bool PermissionCheckCreateComment()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkCreateCommentAction);
        }

        public static bool PermissionCheckEditComment()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkEditCommentAction);
        }

        public static bool PermissionCheckEditComment(Comment c)
        {
            return CommunitySecurity.CheckPermissions(new BookmarkPermissionSecurityObject(c.UserID, c.ID), BookmarkingBusinessConstants.BookmarkEditCommentAction);
        }
    }
}
