namespace ASC.Web.CRM.SocialMedia
{
    public class ContactRelationFacebookSettings
    {
        public int ContactID { get; set; }
        public string SocialNetwork { get; set; }
        public int FacebookUserID { get; set; }        
        public string UserAvatarUrl { get; set; }        
        public bool RelateAvatar { get; set; }
    }
}