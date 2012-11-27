var UserSettingsManager = new function() {

    this.SaveUserLanguageSettings = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_lngTimeSettingsBox').block();
            else
                jq('#studio_lngTimeSettingsBox').unblock();
        };

        UserCustomisation.SaveUserLanguageSettings(jq('#studio_lng').val(), function(result) {
            var res = result.value;
            if (res.rs1 == '2') {
                jq('#studio_lngTimeSettingsInfo').html(res.rs2);
                setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);

            }
            else if (res.rs1 == '1') {
                window.location.reload(true);
            }
            else {
                jq('#studio_lngTimeSettingsInfo').html(res.rs2);
                setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);
            }
        });
    };
    this.SaveUserSkinSettings = function(e) {
        if (this.TimeoutHandler)
            clearTimeout(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_skinSettingsBox').block();
            else
                jq('#studio_skinSettingsBox').unblock();
        };

        UserCustomisation.SaveSkinSettings(jq('input.studio_skin:checked').val(), function(result) {

            var res = result.value;
            if (res.Status == 1)
                window.location.reload(true);
            else {
                jq('#studio_skinSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
                setTimeout(function() { jq('#studio_skinSettingsInfo').html(''); }, 4000);
            }
            return false;
        });

        e.stopImmediatePropagation();
        return false;
    };
}
jq(function() {
jq('#saveSkinSettingBtn').click(UserSettingsManager.SaveUserSkinSettings);
    
});
