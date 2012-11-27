#region Import

using System;
using ASC.Web.Studio.Controls.Common;

#endregion

namespace ASC.Web.CRM.Controls.Common
{
    public partial class FilesView : BaseUserControl
    {
        #region Property

        public static String Location { get { return PathProvider.GetFileStaticRelativePath("Common/FilesView.ascx"); } }
        public static int ContactID { get; set; }

        #endregion
        
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
           // CommonControlsConfigurer.InitProgressFileUploader(_fileUploader);

            _uploadSwitchHolder.Controls.Add(new FileUploaderModeSwitcher());
        }

        #endregion

    }
}