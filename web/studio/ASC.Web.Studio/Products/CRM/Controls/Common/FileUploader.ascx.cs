using System;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.CRM.Controls.Common
{
    public partial class FileUploader : BaseUserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Common/FileUploader.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _uploadSwitchHolder.Controls.Add(new FileUploaderModeSwitcher());
        }
    }
}