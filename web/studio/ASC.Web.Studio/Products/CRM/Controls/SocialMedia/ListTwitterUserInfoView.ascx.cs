﻿using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ASC.SocialMedia.Twitter;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class ListTwitterUserInfoView : System.Web.UI.UserControl
    {
        public List<TwitterUserInfo> UserInfoCollection { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserInfoCollection != null && UserInfoCollection.Count > 0)
            {
                BindRepeater();
                ShowRepeater();
            }
            else
                ShowNotFoundText();

        }

        private void BindRepeater()
        {
            _ctrlRptrUsers.DataSource = UserInfoCollection;
            _ctrlRptrUsers.DataBind();

        }

        private void ShowRepeater()
        {
            _ctrlDivNotFound.Visible = false;
            _ctrlRptrUsers.Visible = true;
        }

        private void ShowNotFoundText()
        {
            _ctrlDivNotFound.Visible = true;
            _ctrlRptrUsers.Visible = false;
        }

        protected void _ctrlRptrUsers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            decimal userID = ((TwitterUserInfo)e.Item.DataItem).UserID;
            string _ctrlRelateAccountScript = String.Format("ASC.CRM.SocialMedia.ShowAccountRelationPanel('{0}','{1}'); return false;", userID, "twitter");
            ((HtmlAnchor)e.Item.FindControl("_ctrlRelateContactWithAccount")).Attributes.Add("onclick", _ctrlRelateAccountScript);
        }
    }
}