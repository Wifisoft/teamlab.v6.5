using System;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Core.Users;
using ASC.Web.CRM.Resources;
using System.Collections.Generic;


namespace ASC.Web.CRM.Controls.Common
{
    public partial class PrivatePanel : BaseUserControl
    {
        #region Properties

        public static String Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/PrivatePanel.ascx");
            }
        }

        public String Title { get; set; }

        public String Description { get; set; }

        public String CheckBoxLabel { get; set; }

        public String AccessListLable { get; set; }

        public bool IsPrivateItem { get; set; }

        public List<String> UsersWhoHasAccess { get; set; }

        public Dictionary<Guid, String> SelectedUsers { get; set; }

        public List<Guid> DisabledUsers { get; set; }

        public bool HideNotifyPanel { get; set; }

        #endregion

        #region Events

        protected void Page_Init(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Title))
                Title = CRMCommonResource.PrivatePanelHeader;

            if (String.IsNullOrEmpty(Description))
                Description = CustomNamingPeople.Substitute<CRMCommonResource>("PrivatePanelDescription");

            if (String.IsNullOrEmpty(CheckBoxLabel))
                CheckBoxLabel = CRMCommonResource.PrivatePanelCheckBoxLabel;

            if (String.IsNullOrEmpty(AccessListLable))
                AccessListLable = CustomNamingPeople.Substitute<CRMCommonResource>("PrivatePanelAccessListLable");  

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //init userSelectorListView
            var selector = (UserSelectorListView) LoadControl(UserSelectorListView.Location);
            selector.ShowNotifyPanel = !HideNotifyPanel;
            selector.SelectedUsers = SelectedUsers;
            selector.UsersWhoHasAccess = UsersWhoHasAccess;
            selector.DisabledUsers = DisabledUsers;
            _phUserSelectorListView.Controls.Add(selector);
        }
        
        #endregion
    }
}