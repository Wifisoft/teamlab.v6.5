using System;

namespace ASC.Web.UserControls.SocialMedia
{
    public class BaseUserControl : System.Web.UI.UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!Page.ClientScript.IsClientScriptBlockRegistered("DefaultSocialMediaCss"))
            {
                string script = string.Format("<link rel='stylesheet' text='text/css' href='{0}' />",
                    Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), "ASC.Web.UserControls.SocialMedia.css.socialmedia.css"));

                Page.ClientScript.RegisterClientScriptBlock(typeof(BaseUserControl), "DefaultSocialMediaCss", script);
            }
        }
    }
}
