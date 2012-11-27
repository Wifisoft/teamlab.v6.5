

TimeAndLanguageContentManager = function() {
    this.SaveTimeLangSettings = function(parentCallback) {
        TimeAndLanguageSettingsController.SaveLanguageTimeSettings(jq('#studio_lng').val(), jq('#studio_timezone').val(), function(result) {

            if (parentCallback != null)
                parentCallback(result.value);

        });
    }
}