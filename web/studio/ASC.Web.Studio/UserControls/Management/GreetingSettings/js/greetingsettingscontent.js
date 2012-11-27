jq(function() {
    if (jq('#studio_logoUploader').length > 0) {
        new AjaxUpload('studio_logoUploader', {
            action: 'ajaxupload.ashx?type=ASC.Web.Studio.UserControls.Management.LogoUploader,ASC.Web.Studio',
            onSubmit: jq.blockUI,
            onComplete: GreetingSettingsUploadManager.SaveGreetingLogo
        });
    }
})

var GreetingSettingsUploadManager = new function() {
    this.BeforeEvent = null;
    this.SaveGreetingLogo = function(file, response) {
        var GreetManager1 = new GreetingSettingsContentManager();

        GreetManager1.SaveGreetingLogo(file, response);
    }
}

var GreetingSettingsContentManager = function() {

    this.OnAfterSaveData = null;

    this.SaveGreetingLogo = function(file, response) {
        jq.unblockUI();

        var result = eval("(" + response + ")");

        if (result.Success) {
            jq('#studio_greetingLogo').attr('src', result.Message);
            jq('#studio_greetingLogoPath').val(result.Message);
        }
        else {
            if (this.OnAfterSaveData != null)
                this.OnAfterSaveData(result);
        }
    }

    this.SaveGreetingOptions = function(parentCallback) {

        GreetingSettingsController.SaveGreetingSettings(jq('#studio_greetingLogoPath').val(),
                                                jq('#studio_greetingHeader').val(),
                                                function(result) {
                                                    //clean logo path input
                                                    jq('#studio_greetingLogoPath').val('');
                                                    if (parentCallback != null)
                                                        parentCallback(result.value);
                                                });
    };

    this.RestoreGreetingOptions = function() {
        GreetingSettingsController.RestoreGreetingSettings(function(result) {
            //clean logo path input
            jq('#studio_greetingLogoPath').val('');

            if (result.value.Status == 1) {
                jq('#studio_greetingHeader').val(result.value.CompanyName);
                jq('#studio_greetingLogo').attr('src', result.value.LogoPath);
            }
        });
    }
}