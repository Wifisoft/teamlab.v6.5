using System;

namespace ASC.Web.CRM.SocialMedia
{
    public class SocialMediaAccountNotFound : Exception
    {
        public SocialMediaAccountNotFound(string message) : base(message) { }
    }
}
