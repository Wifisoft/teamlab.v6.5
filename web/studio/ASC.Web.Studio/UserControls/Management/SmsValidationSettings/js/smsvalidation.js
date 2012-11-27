
var SmsValidationSettings = new function() {

    this.SaveSmsValidationSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);


        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_smsValidationSettingsInfo').block();
            else
                jq('#studio_smsValidationSettingsInfo').unblock();
        };

        var jsonObj = { "Enable": jq("#chk_studio_2FactorAuth").is(":checked") };

        SmsValidationSettingsController.SaveSettings(JSON.stringify(jsonObj), function(result) {

            var res = result.value;
            if (res.Status == 1)
                jq('#studio_smsValidationSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_smsValidationSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
            SmsValidationSettings.TimeoutHandler = setTimeout(function() { jq('#studio_smsValidationSettingsInfo').html(''); }, 4000);
        });
    }
}