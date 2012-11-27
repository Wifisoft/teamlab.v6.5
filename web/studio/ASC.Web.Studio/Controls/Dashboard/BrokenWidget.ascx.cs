using System;

namespace ASC.Web.Studio.Controls.Dashboard
{
    public partial class BrokenWidget : System.Web.UI.UserControl
    {
        public static string Path = "~/controls/dashboard/BrokenWidget.ascx"; 

        public Exception Exception { get; set; }

        public BrokenWidget()
        {
        }

        public BrokenWidget(Exception exception)
        {
            Exception = exception;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}