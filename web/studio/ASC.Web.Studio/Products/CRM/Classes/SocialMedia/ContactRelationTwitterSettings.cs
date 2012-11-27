namespace ASC.Web.CRM.SocialMedia
{
    public class ContactRelationTwitterSettings
    {
        public int ContactID { get; set; }        
        public int TwitterUserID { get; set; }
        public string TwitterScreenName { get; set; }
        public string UserAvatarUrl { get; set; }
        public bool RelateAccount { get; set; }
        public bool RelateAvatar { get; set; }
    }
}