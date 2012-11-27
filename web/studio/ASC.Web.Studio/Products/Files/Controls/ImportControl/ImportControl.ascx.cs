using System;
using System.Web.UI;

namespace ASC.Web.Files.Controls
{
    public partial class ImportControl : UserControl
    {
        public static string Location
        {
            get { return Classes.PathProvider.GetFileStaticRelativePath("ImportControl/ImportControl.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ImportDialogTemp.Options.IsPopup = true;
            LoginDialogTemp.Options.IsPopup = true;
        }
    }
}