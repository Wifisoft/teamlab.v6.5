using System;
using System.Web.UI;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    public partial class StepContainer : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/StepContainer.ascx"; } }

        public uint StepNumber { get; set; }
        public string Title { get; set; }
        public bool ShowHeader { get; set; }
        public bool HideFooter { get; set; }
        public bool ShowSkip { get; set; }
        public string SaveButtonText { get; set; }

        public Control ChildControl { get; set; }
        public string SaveButtonEvent { get; set; }
        public string CancelButtonEvent { get; set; }
        public string SkipButtonEvent { get; set; }
        public bool LastStep { get; set; }


        protected override void OnInit(EventArgs e)
        {
            if (string.IsNullOrEmpty(SaveButtonText))
                SaveButtonText = Resources.Resource.ContainerDone;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShowSkip)
                btnSkip.Visible = false;

            if(ChildControl!= null)
                content1.Controls.Add(ChildControl);
        }
    }
}