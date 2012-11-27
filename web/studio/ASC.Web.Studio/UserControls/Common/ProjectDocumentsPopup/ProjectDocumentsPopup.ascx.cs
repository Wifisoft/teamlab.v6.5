using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using Resources;

namespace ASC.Web.Studio.UserControls.Common.ProjectDocumentsPopup
{
    public partial class ProjectDocumentsPopup : System.Web.UI.UserControl
    {
        public string PopupName { get; set; }
        public int ProjectId { get; set; }

        public static string Location { get { return "~/UserControls/Common/ProjectDocumentsPopup/ProjectDocumentsPopup.ascx"; } }

        public ProjectDocumentsPopup()
        {
            PopupName = UserControlsCommonResource.AttachOfProjectDocuments;
            ProjectId = 0;
        }

        private void InitScripts()
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "projectDocumentsPopup_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/projectdocumentspopup/css/<theme_folder>/projectDocumentsPopup.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "projectDocumentsPopup_script", WebPath.GetPath("usercontrols/common/projectdocumentspopup/js/projectDocumentsPopup.js"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _documentUploader.Options.IsPopup = true;
            InitScripts();
            var emptyParticipantScreenControl = new EmptyScreenControl
            {
                ImgSrc = VirtualPathUtility.ToAbsolute("~/UserControls/Common/ProjectDocumentsPopup/Images/project-documents.png"),
                Header = UserControlsCommonResource.ProjectDocuments,
                HeaderDescribe = UserControlsCommonResource.EmptyDocsHeaderDescription,
                Describe = Resources.UserControlsCommonResource.EmptyDocsDescription
            };
            _phEmptyDocView.Controls.Add(emptyParticipantScreenControl);
        }
    }
}