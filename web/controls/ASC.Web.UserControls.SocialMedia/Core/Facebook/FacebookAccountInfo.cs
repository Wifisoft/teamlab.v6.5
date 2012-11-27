using System;

namespace ASC.SocialMedia.Facebook
{
    public class FacebookAccountInfo
    {
        public string AccessToken { get; set; }
        public Guid AssociatedID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
