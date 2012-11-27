using System;
using System.Collections.Generic;
using ASC.SocialMedia;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;

namespace ASC.Web.UserControls.SocialMedia.UserControls
{
    public partial class ListActivityMessageView : BaseUserControl
    {
        public List<Message> MessageList { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MessageList != null)
            {
                _ctrlRptrUserActivity.DataSource = MessageList;
                _ctrlRptrUserActivity.DataBind();
            }
        }

        protected void _ctrlRptrUserActivity_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Message msg = (Message)e.Item.DataItem;
            ((Image)e.Item.FindControl("_ctrlImgSocialMediaIcon")).ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), String.Format("ASC.Web.UserControls.SocialMedia.images.{0}.png", msg.Source.ToString().ToLower()));

        }
    }
}