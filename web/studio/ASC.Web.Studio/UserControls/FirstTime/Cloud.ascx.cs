using System;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    public partial class Cloud : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/Cloud.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.ClientScript.RegisterClientScriptInclude(typeof(string), "firsttimeview111_script", WebPath.GetPath("usercontrols/firsttime/js/view.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "steps_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/firsttime/css/<theme_folder>/stepcontainer.css") + "\">", false);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "firsttime_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/firsttime/css/<theme_folder>/EmailAndPassword.css") + "\">", false);

            var step0 = (StepContainer)LoadControl(StepContainer.Location);
            step0.ShowHeader = false;
            step0.ShowSkip = false;
            step0.SaveButtonEvent = "ASC.Controls.FirstTimeView.SaveRequiredStep();";
            step0.CancelButtonEvent = String.Format("ASC.Controls.FirstTimeView.GoToNextStep({0});",0);
            step0.SkipButtonEvent = "ASC.Controls.FirstTimeView.SkipAllSteps('');";
            step0.ChildControl = LoadControl(EmailAndPassword.Location);
            content.Controls.Add(step0);

            var step3 = (StepContainer)LoadControl(StepContainer.Location);
            step3.ShowHeader = true;
            step3.ShowSkip = true;
            step3.StepNumber = 1;
            step3.ChildControl = LoadControl(Customization.Location);
            step3.SaveButtonEvent = "ASC.Controls.FirstTimeView.SaveGeneralSettings();";
            step3.CancelButtonEvent = String.Format("ASC.Controls.FirstTimeView.GoToNextStep({0},'customization_skip');", step3.StepNumber);
            step3.SkipButtonEvent = "ASC.Controls.FirstTimeView.SkipAllSteps('customization');";
            step3.Title = Resources.Resource.WizardGeneralSettings;
            content.Controls.Add(step3);

            var step2 = (StepContainer)LoadControl(StepContainer.Location);
            step2.ShowHeader = true;
            step2.ShowSkip = true;
            step2.StepNumber = 2;
            step2.ChildControl = LoadControl(AddUsers.Location);
            step2.SaveButtonEvent = "ASC.Controls.FirstTimeView.SaveUsers();";
            step2.SaveButtonText = Resources.Resource.WizardButton;
            step2.CancelButtonEvent = "ASC.Controls.FirstTimeView.SkipAllSteps('user_skip');";
            step2.SkipButtonEvent = "ASC.Controls.FirstTimeView.SkipAllSteps('users');";
            step2.Title = CustomNamingPeople.Substitute<Resources.Resource>("WizardUsers");
            content.Controls.Add(step2);
        }
    }
}