using System;
using System.Web.UI;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public enum ManagementTab
    {
        EditForum = 0,      
        EditTags = 1        
    }

    public partial class ManagementCenter : MainPage
    {
        public ManagementTab ManagementTab { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(ForumManager.BaseVirtualPath + "/default.aspx");

            ForumManager.Instance.SetCurrentPage(ForumPage.ManagementCenter);
            ManagementTab = ManagementTab.EditForum;
            if (!String.IsNullOrEmpty(Request["type"]))
            {
                try
                {
                    ManagementTab = (ManagementTab)Convert.ToInt32(Request["type"]);

                }
                catch { ManagementTab = ManagementTab.EditForum; }
            }

            Control managementControl = null;
            switch (ManagementTab)
            {
                case ManagementTab.EditForum:                    
                    managementControl =LoadControl(ForumManager.BaseVirtualPath+ "/UserControls/ForumEditor.ascx");                    
                    break;

                case ManagementTab.EditTags:                    
                    managementControl = LoadControl(ForumManager.BaseVirtualPath + "/UserControls/TagEditor.ascx");
                    break;
            }

            controlPanel.Controls.Add(managementControl);
        }
    }
}
