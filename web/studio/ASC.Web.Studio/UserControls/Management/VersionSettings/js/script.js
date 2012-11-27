var StudioVersionManagement = new function() {

    this.SwitchVersion = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_versionSetting').block();
            else
                jq('#studio_versionSetting').unblock();
        };
        VersionSettingsController.SwitchVersion(jq('#versionSelector  input:radio[name=version]:checked').val(), function(res) {
            if (res.value.Status == '1') {
                jq('#studio_versionSetting').block();
                setTimeout(function() { window.location.reload(true) }, 5000);
            } else {
                jq('#studio_versionSetting_info').html('<div class="errorBox">' + res.value.Message + '</div>');
                StudioManagement.TimeoutHandler = setTimeout("jq('#studio_versionSetting_info').html('');", 3000);
            }
        });
    };
};