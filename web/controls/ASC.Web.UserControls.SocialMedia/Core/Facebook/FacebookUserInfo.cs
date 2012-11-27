using System;

namespace ASC.SocialMedia.Facebook
{
    public class FacebookUserInfo
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string SmallImageUrl
        {
            get
            {
                return String.Format("http://graph.facebook.com/{0}/picture?type=small", UserID);
            }
        }
    }
}
