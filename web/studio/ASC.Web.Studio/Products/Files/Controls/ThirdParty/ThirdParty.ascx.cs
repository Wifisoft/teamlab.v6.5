using System;
using System.Web.UI;

namespace ASC.Web.Files.Controls
{
    public partial class ThirdParty : UserControl
    {
        protected bool EnableBoxNet;
        protected bool EnableDropBox;
        protected bool EnableGoogle;

        public bool CurrentUserAdmin { get; set; }

        public static string Location
        {
            get { return Classes.PathProvider.GetFileStaticRelativePath("ThirdParty/ThirdParty.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ThirdPartyEditorTemp.Options.IsPopup = true;
            ThirdPartyDeleteTmp.Options.IsPopup = true;

            EnableBoxNet = Import.ImportConfiguration.SupportBoxNetInclusion;
            EnableDropBox = Import.ImportConfiguration.SupportDropboxInclusion;
            EnableGoogle = Import.ImportConfiguration.SupportGoogleInclusion;
        }
    }
}