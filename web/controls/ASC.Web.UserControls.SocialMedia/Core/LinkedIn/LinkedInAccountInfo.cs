using System;

namespace ASC.SocialMedia.LinkedIn
{
    public class LinkedInAccountInfo
    {
        public enum TokenTypes { AccessToken, RequestToken, InvalidToken }

        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public Guid AssociatedID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public TokenTypes TokenType { get; set; }
    }
}
