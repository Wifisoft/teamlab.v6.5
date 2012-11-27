using System;
using ASC.Blogs.Core;
using ASC.Web.Core.Users;

namespace ASC.Web.Community.Blogs
{
    public class ImageHTMLHelper
    {
        public static string GetHTMLUserAvatar(Guid userID)
        {
            string imgPath = UserPhotoManager.GetSmallPhotoURL(userID);
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" alt='' src=\"" + imgPath + "\"/>";

            return string.Empty;
        }
        
        public static string GetLinkUserAvatar(Guid userID)
        {
            string imgPath = UserPhotoManager.GetBigPhotoURL(userID);
            if (imgPath != null)
                return String.Format("<a href=\"{0}\"><img class=\"userPhoto\" src=\"{1}\"/></a>",
                        Constants.UserPostsPageUrl+userID.ToString(),
                        imgPath);

            return string.Empty;
        }
    }
}
