using System;
using ASC.SocialMedia.Twitter;
using ASC.Web.CRM.Resources;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class TwitterUserInfoView : System.Web.UI.UserControl
    {
        public TwitterUserInfo UserInfo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _ctrlChbRelateToAccount.Text = CRMSocialMediaResource.RelateAccountToContact;
            _ctrlChbAddImage.Text = CRMSocialMediaResource.RelateAvatar;
            _ctrlBtRelate.Value = CRMSocialMediaResource.Relate;

            _ctrlImageUserAvatar.ImageUrl = UserInfo.SmallImageUrl.Replace("_normal", "");
            _ctrlUserName.InnerText = UserInfo.UserName;
            _ctrlUserDescription.InnerText = UserInfo.Description;

            _ctrlHiddenContactID.Value = "";
            _ctrlHiddenTwitterUserID.Value = UserInfo.UserID.ToString();
            _ctrlHiddenTwitterUserScreenName.Value = UserInfo.ScreenName;
            _ctrlHiddenUserAvatarUrl.Value = UserInfo.SmallImageUrl.Replace("_normal", "");
        }
    }
}