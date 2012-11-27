using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Files.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Studio.Core;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Controls
{
    public partial class MainContent : UserControl
    {
        protected OrderBy ContentOrderBy;
        protected bool CompactViewFolder;
        protected bool UserAdmin;

        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("MainContent/MainContent.ascx"); }
        }

        public object FolderIDUserRoot { get; set; }
        public object FolderIDCommonRoot { get; set; }
        public object FolderIDShare { get; set; }
        public object FolderIDTrash { get; set; }
        public object FolderIDCurrentRoot { get; set; }

        public String TitlePage { get; set; }

        public bool CurrentUserAdmin
        {
            get { return UserAdmin || Global.IsAdministrator; }
            set { UserAdmin = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var filesSettings = SettingsManager.Instance.LoadSettingsFor<FilesSettings>(SecurityContext.CurrentAccount.ID);
            ContentOrderBy = filesSettings.ContentOrderBy;
            CompactViewFolder = filesSettings.CompactViewFolder;

            Page.ClientScript.RegisterJavaScriptResource(typeof (FilesJSResource), "ASC.Files.FilesJSResources");

            InitScriptConstants();
            InitControls();

            InitStartData();
        }

        private void InitStartData()
        {
            var scriptInline = new StringBuilder();
            var service = new Service();

            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof (ItemList<FileOperationResult>));
                var tasks = service.GetTasksStatuses();
                serializer.WriteObject(ms, tasks);
                ms.Seek(0, SeekOrigin.Begin);

                scriptInline.AppendFormat(" var startTasksStatuses = {0};", Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int) ms.Length));
            }

            if (!Page.ClientScript.IsStartupScriptRegistered(GetType(), "{1A03ADE5-F7C8-4f6c-8027-43A2E22E5702}"))
                Page.ClientScript.RegisterStartupScript(GetType(), "{1A03ADE5-F7C8-4f6c-8027-43A2E22E5702}", scriptInline.ToString(), true);
        }

        private void InitScriptConstants()
        {
            var inlineScript = new StringBuilder();

            inlineScript.AppendFormat(
                @"
                                ASC.Files.Constants.FOLDER_ID_MY_FILES = ""{0}"";
                                ASC.Files.Constants.FOLDER_ID_COMMON_FILES = ""{1}"";
                                ASC.Files.Constants.FOLDER_ID_SHARE = ""{2}"";
								ASC.Files.Constants.FOLDER_ID_TRASH = ""{3}"";
                                ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT = ""{4}"";
                                ",
                FolderIDUserRoot,
                FolderIDCommonRoot,
                FolderIDShare,
                FolderIDTrash,
                FolderIDCurrentRoot
                );

            inlineScript.AppendFormat(
                @"
                                ASC.Files.Constants.USER_ID = ""{0}"";
                                ASC.Files.Constants.USER_ADMIN = (true === {1});
                                ASC.Files.Constants.TITLE_PAGE = ""{2}"";
                                ASC.Files.Constants.MAX_NAME_LENGTH = ""{3}"";
                                ASC.Files.Constants.MAX_UPLOAD_SIZE = ""{4}"";
                                ASC.Files.Constants.MAX_UPLOAD_EXCEED = ""{5}"";
                                ASC.Files.Constants.URL_HANDLER_UPLOAD = ""{6}"";
                                ",
                SecurityContext.CurrentAccount.ID,
                CurrentUserAdmin.ToString().ToLower(),
                TitlePage,
                Global.MAX_TITLE,
                SetupInfo.MaxUploadSize,
                FileSizeComment.FileSizeExceptionString.HtmlEncode(),
                FileHandler.FileHandlerPath + UrlConstant.ParamsUpload
                );

            inlineScript.AppendFormat("ASC.Files.TemplateManager.init(\"{0}\", \"{1}\");",
                                      VirtualPathUtility.ToAbsolute("~/template.ashx"),
                                      PathProvider.TemplatePath);


            inlineScript.AppendFormat("serviceManager.init(\"{0}\");", PathProvider.GetFileServicePath);


            if (!Page.ClientScript.IsStartupScriptRegistered(GetType(), "{19F9A54F-EAC2-4ca1-93BC-FB0E1B94D5BD}"))
                Page.ClientScript.RegisterStartupScript(GetType(), "{19F9A54F-EAC2-4ca1-93BC-FB0E1B94D5BD}",
                                                        inlineScript.ToString(), true);
        }

        private void InitControls()
        {
            confirmRemoveDialog.Options.IsPopup = true;
            confirmOverwriteDialog.Options.IsPopup = true;

            var emptyFolder = (EmptyFolder) LoadControl(EmptyFolder.Location);
            emptyFolder.FolderIDUserRoot = FolderIDUserRoot;
            emptyFolder.FolderIDShare = FolderIDShare;
            emptyFolder.FolderIDCommonRoot = FolderIDCommonRoot;
            emptyFolder.FolderIDTrash = FolderIDTrash;
            emptyFolder.FolderIDCurrentRoot = FolderIDCurrentRoot;
            EmptyScreenFolder.Controls.Add(emptyFolder);

            if (FileUtility.ExtsImagePreviewed.Count != 0)
                CommonContainer.Controls.Add(LoadControl(FileViewer.Location));

            if (ImportConfiguration.SupportInclusion)
            {
                var thirdParty = (ThirdParty) LoadControl(ThirdParty.Location);
                thirdParty.CurrentUserAdmin = CurrentUserAdmin;
                CommonContainer.Controls.Add(thirdParty);
            }
        }
    }
}