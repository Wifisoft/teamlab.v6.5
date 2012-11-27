using System;
using System.Web.UI;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Controls
{
    public partial class MainMenu : UserControl
    {
        public bool EnableCreateFile = true;
        public bool EnableUpload = true;
        public bool EnableCreateFolder = true;
        public bool EnableImport = true;
        public bool EnableThirdParty = true;

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("MainMenu/MainMenu.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _uploadSwitchHolder.Controls.Add(new FileUploaderModeSwitcher());
            uploadDialogTemp.Options.IsPopup = true;

            EnableCreateFile = EnableCreateFile
                               && !Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)
                               && FileUtility.ExtsWebEdited.Count != 0;

            EnableUpload = EnableUpload
                           && !Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);

            EnableImport = EnableImport
                           && ImportConfiguration.SupportImport;

            EnableThirdParty = EnableThirdParty
                               && ImportConfiguration.SupportInclusion;
        }
    }
}