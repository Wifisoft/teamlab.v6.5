using System;

namespace ASC.SocialMedia.Twitter
{
    public class TwitterAccountInfo
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public Guid AssociatedID { get; set; }
        public string ScreenName { get; set; }
        public decimal UserID { get; set; }        
        public string UserName { get; set; }
    }
}
