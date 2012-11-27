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

namespace ASC.Web.Studio.UserControls.Common.Attachments
{
    public partial class Attachments : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Common/Attachments/Attachments.ascx"; } }

        public bool PortalDocUploaderVisible { get; set; }

        public string MenuNewDocument { get; set; }

        public string MenuUploadFile { get; set; }

        public string MenuProjectDocuments { get; set; }

        public string ModuleName { get; set; } 

        public string EntityType { get; set; }

        public int ProjectId { get; set; }

        protected string ExtsWebPreviewed = string.Join(", ", Utility.FileUtility.ExtsWebPreviewed.ToArray());
        protected string ExtsWebEdited = string.Join(", ", Utility.FileUtility.ExtsWebEdited.ToArray());

        public Attachments()
        {
            PortalDocUploaderVisible = true;
            MenuNewDocument = UserControlsCommonResource.NewFile;
            MenuUploadFile = UserControlsCommonResource.UploadFile;
            MenuProjectDocuments = UserControlsCommonResource.AttachOfProjectDocuments;

            EntityType = "";
            ModuleName = "";
        }

        private void InitScripts()
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "attachments_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/attachments/css/<theme_folder>/attachments.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fancybox_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/common/attachments/css/<theme_folder>/jquery.fancybox-1.3.4.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "attachments_script", WebPath.GetPath("usercontrols/common/attachments/js/attachments.js"));
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "fancybox_script", WebPath.GetPath("usercontrols/common/attachments/js/jquery.fancybox-1.3.4.pack.js"));
        }

        private void CreateEmptyPanel()
        {
            var buttons = "<a id='uploadFirstFile' class='baseLinkAction'>" + MenuUploadFile + "</a><br/>" +
                          "<a id='createFirstDocument' class='baseLinkAction'>" + MenuNewDocument + "</a><img id='firstDocComb' src= '" + VirtualPathUtility.ToAbsolute("~/UserControls/Common/Attachments/Images/combobox_black.png") + "' class='newDocComb'/>";
            if(ModuleName!="crm")
            {
                buttons += "<br/><a id='attachProjDocuments' class='baseLinkAction'>" + MenuProjectDocuments + "</a>";
            }

            var emptyParticipantScreenControl = new EmptyScreenControl
            {
                ImgSrc = VirtualPathUtility.ToAbsolute("~/UserControls/Common/Attachments/Images/documents-logo.png"),
                Header = Resources.UserControlsCommonResource.EmptyListDocumentsHead,
                Describe = String.Format(Resources.UserControlsCommonResource.EmptyListDocumentsDescr,
                    //create
                                         "<span class='hintCreate baseLinkAction' >", "</span>",
                    //upload
                                         "<span class='hintUpload baseLinkAction' >", "</span>",
                    //open
                                         "<span class='hintOpen baseLinkAction' >", "</span>",
                    //edit
                                         "<span class='hintEdit baseLinkAction' >", "</span>"),
                ButtonHTML = buttons
            };
            _phEmptyDocView.Controls.Add(emptyParticipantScreenControl);
        }

        private void InitProjectDocumentsPopup()
        {
            var projectDocumentsPopup = (ProjectDocumentsPopup.ProjectDocumentsPopup)LoadControl(ProjectDocumentsPopup.ProjectDocumentsPopup.Location);
            projectDocumentsPopup.ProjectId = ProjectId;
            _phDocUploader.Controls.Add(projectDocumentsPopup);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopup.Options.IsPopup = true;
            InitScripts();

            CreateEmptyPanel();

            if (ModuleName!="crm")
            {
                var projId = Request["prjID"];
                if (!String.IsNullOrEmpty(projId))
                {
                    ProjectId = Convert.ToInt32(projId);
                    InitProjectDocumentsPopup();
                }
                else
                {
                    ProjectId = 0;
                }
            }
            else
            {
                PortalDocUploaderVisible = false;
            }
        }
    }
}