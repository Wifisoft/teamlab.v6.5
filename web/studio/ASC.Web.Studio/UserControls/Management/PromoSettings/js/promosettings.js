
var PromoSettings = new function() {

this.SaveNotifyBarSettings = function() {
    if (this.TimeoutHandler)
        clearInterval(this.TimeoutHandler);

    AjaxPro.onLoading = function(b) {
        if (b)
            jq('#studio_NotifyBarSettingsBox').block();
        else
            jq('#studio_NotifyBarSettingsBox').unblock();
    };

    PromoSettingsController.SaveNotifyBarSettings(jq("#studio_showPromotions").is(":checked"), function(result) {
        jq('div[id^="studio_setInf"]').html('');
        jq('#studio_setInfNotifyBarSettingsInfo').html(result.value.rs2);
        StudioManagement.TimeoutHandler = setTimeout("jq('#studio_setInfNotifyBarSettingsInfo').html('');", 3000);
    });
}
}