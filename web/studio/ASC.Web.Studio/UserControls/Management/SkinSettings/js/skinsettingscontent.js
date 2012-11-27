
SkinSettingsContentManager = function() {

    this.SaveSkinSettings = function(parentCallback) {
        SkinSettingsController.SaveSkinSettings(jq('input.studio_skin:checked').val(), function(result) {

            //do somthin'
            if (parentCallback != null)
                parentCallback(result.value);
            else
                alert('null');
        });
    }

    this.SkinPreview = function(skinID) {
        var preview = jq('#skin_prev_' + skinID).attr('src');
        jq('#studio_skinPreview').attr('src', preview);
    }
}

jq(function() {
    var manager = new SkinSettingsContentManager();
    jq('input[name="studio_skin"]').click(function() { manager.SkinPreview(jq(this).val()); });
});