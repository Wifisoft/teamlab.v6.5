
SkinSettingsManager = new function() {

    this.TimeoutHandler = null;

    this.SaveSettings = function() {

        if (this.TimeoutHandler)
            clearTimeout(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_skinSettingsBox').block();
            else
                jq('#studio_skinSettingsBox').unblock();
        };
        var contentManager = new SkinSettingsContentManager();
        contentManager.SaveSkinSettings(SkinSettingsManager.SaveSettingsCallback);
    }

    this.SaveSettingsCallback = function(res) {
        if (res.Status == 1)
            window.location.reload(true);
        else {
            jq('#studio_skinSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
            SkinSettingsManager.TimeoutHandler = setTimeout(function() { jq('#studio_skinSettingsInfo').html(''); }, 4000);
        }
    }
}

jq(function() {
    jq('#saveSkinSettingBtn').click(SkinSettingsManager.SaveSettings);
   
})
