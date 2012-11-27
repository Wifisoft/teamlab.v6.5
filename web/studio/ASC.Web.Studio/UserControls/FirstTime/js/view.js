if (typeof (ASC) == 'undefined')
    ASC = {};
if (typeof (ASC.Controls) == 'undefined')
    ASC.Controls = {};
    
var LogoManager = null;

ASC.Controls.FirstTimeView = new function() {
    this.CancelConfirmMessage = "";
    this.StepsCount = 3;
    this.currentStep = 0;
    this.TimeoutHandler = null;

    this.Finish = function() {
        EventTracker.Track('wizard_finish');
        location.href = "/";
    }

    this.SkipAllSteps = function(action) {
        EventTracker.Track('wizard_' + action + '_step_skipAll');
        location.href = "/";
    }

    this.GoToNextStep = function(currentStep, action) {
        if (action != '')
            EventTracker.Track('wizard_' + action + '_step');

        jq('.step' + currentStep).hide();
        currentStep++;
        ASC.Controls.FirstTimeView.currentStep++;
        if (currentStep < ASC.Controls.FirstTimeView.StepsCount) {
            jq('#steps .step' + currentStep + ' .stepData').show();
            if (currentStep != ASC.Controls.FirstTimeView.StepsCount - 1)
                jq('.step' + currentStep + ' .skip').show();
        }
        else
            jq('.wizardButton .btnBox').show();
    }

    this.SaveRequiredStep = function() {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('.step0').block();
            else
                jq('.step0').unblock();
        };

        EventTracker.Track('wizard_required_step');
        this.currentStep = 0;
        var FTManager = new ASC.Controls.FirstTimeManager();
        FTManager.SaveRequiredData(this.SaveRequiredStepCallback);
    }

    this.SaveRequiredStepCallback = function(result) {

        if (result.Status == 1) {
            jq('#steps .step0').hide();
            jq('.wizardDesc').hide();
            jq('.wizardHelper').show();

            ASC.Controls.FirstTimeView.GoToNextStep(ASC.Controls.FirstTimeView.currentStep, '');
        }
        else {
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
        }
    }

    this.SaveGreetingsCallback = function(result) {

        if (result.Status == 1)
            ASC.Controls.FirstTimeView.GoToNextStep(ASC.Controls.FirstTimeView.currentStep, '');
        else
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
    }

    this.SaveGreetings = function() {
        this.currentStep = 1;
        LogoManager.SaveGreetingOptions(this.SaveGreetingsCallback);
    }

    this.SaveUsers = function() {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('.step2').block();
            else
                jq('.step2').unblock();
        };
        EventTracker.Track('wizard_users_step');
        this.currentStep = 2;
        var users = new AddUsersManagerComponent();
        users.SaveUsers(this.SaveUsersCallback);
    }

    this.SaveUsersCallback = function(result) {
        if (result.Status == 1)
            ASC.Controls.FirstTimeView.Finish();
        else
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
    }

    this.SaveGeneralSettings = function() {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('.step1').block();
            else
                jq('.step1').unblock();
        };
        EventTracker.Track('wizard_customization_step');
        this.currentStep = 1;
        var time = new TimeAndLanguageContentManager();
        time.SaveTimeLangSettings(this.SaveTimeLangSettingsCallback);
    }

    this.SaveTimeLangSettingsCallback = function(result) {
        if (result.Status == 2 || result.Status == 1) {
            var contentManager = new NamingPeopleContentManager();
            contentManager.SaveSchema(ASC.Controls.FirstTimeView.SaveNamingSettingsCallback);
        }
        else
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
    }

    this.SaveNamingSettingsCallback = function(result) {
        if (result.Status == 1) {
            var contentManager = new SkinSettingsContentManager();
            contentManager.SaveSkinSettings(ASC.Controls.FirstTimeView.SaveSkinSettingsCallback);
        }
        else {
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
        }
    }

    this.SaveSkinSettingsCallback = function(result) {
        if (result.Status == 1)
            ASC.Controls.FirstTimeView.GoToNextStep(ASC.Controls.FirstTimeView.currentStep, '');
        else
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
    }

    this.ShowOperationInfo = function(result) {
        var classCSS = (result.Status == 1 ? "okBox" : "errorBox");
        jq('#wizard_OperationInfo' + ASC.Controls.FirstTimeView.currentStep).html('<div class="' + classCSS + '">' + result.Message + '</div>');
        ASC.Controls.FirstTimeView.TimeoutHandler = setTimeout(function() { jq('#wizard_OperationInfo' + ASC.Controls.FirstTimeView.currentStep).html(''); }, 4000);
    }
}

jq(document).ready(function() {
    ASC.Controls.FirstTimeView.StepsCount = 3;
    window.onbeforeunload = function(e) {
        if (ASC.Controls.FirstTimeView.currentStep == 0)
            return ASC.Controls.FirstTimeView.CancelConfirmMessage;
        else
            return;
    };
});