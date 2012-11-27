using System;
using ASC.Web.CRM.Resources;
using ASC.SocialMedia.Facebook;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class FacebookUserInfoView : System.Web.UI.UserControl
    {
        public FacebookUserInfo UserInfo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {            
            _ctrlChbAddImage.Text = CRMSocialMediaResource.RelateAvatar;
            _ctrlBtRelate.Value = CRMSocialMediaResource.Relate;

            _ctrlImageUserAvatar.ImageUrl = UserInfo.SmallImageUrl.Replace("small", "large");
            _ctrlUserName.InnerText = UserInfo.UserName;            

            _ctrlHiddenContactID.Value = "";
            _ctrlHiddenFacebookUserID.Value = UserInfo.UserID;
            _ctrlHiddenUserAvatarUrl.Value = UserInfo.SmallImageUrl.Replace("small", "large");
        }
    }
}