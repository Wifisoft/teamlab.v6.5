using System;
using System.Web.UI;

namespace ASC.Web.Files.Controls
{
    public partial class FileViewer : UserControl
    {
        public static string Location
        {
            get { return Classes.PathProvider.GetFileStaticRelativePath("FileViewer/FileViewer.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}