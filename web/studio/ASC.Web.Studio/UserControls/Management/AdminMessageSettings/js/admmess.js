var AdmMess = new function() {
    this.SaveSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);


        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_admMessSettingsInfo').block();
            else
                jq('#studio_admMessSettingsInfo').unblock();
        };

        AdmMessController.SaveSettings(jq("#chk_studio_admMess").is(":checked"), function(result) {

            var res = result.value;
            if (res.Status == 1)
                jq('#studio_admMessSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_admMessSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
            AdmMess.TimeoutHandler = setTimeout(function() { jq('#studio_admMessSettingsInfo').html(''); }, 4000);
        });
    }
}