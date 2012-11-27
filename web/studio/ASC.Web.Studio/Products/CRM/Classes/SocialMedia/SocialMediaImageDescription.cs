using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.SocialMedia;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public class SocialMediaImageDescription
    {
        public SocialNetworks SocialNetwork { get; set; }
        public string ImageUrl { get; set; }
        public string Identity { get; set; }
    }
}
