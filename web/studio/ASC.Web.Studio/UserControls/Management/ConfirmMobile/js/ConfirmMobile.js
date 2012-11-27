
var ConfirmMobileManager = new function() {
    this._cellHint = '';
    this._codeHint = '';
    this._emptyCell = '';
    this._emptyCode = '';

    this._activate = null;

    this.Init = function() {
        jq('#studio_phone').Watermark(this._cellHint, 'waterText');
        jq('#studio_phone_authcode').Watermark(this._codeHint, 'waterText');

        jq.dropdownToggle({
            switcherSelector: ".questionPhoneFormat",
            dropdownID: "answerPhoneFormat",
            fixWinSize: false,
            alwaysUp: true
        });
    }
    this.SendAuthCode = function() {
        jq("#error").hide();
        if (jq('#studio_phone').val() == '' || jq('#studio_phone').val() == ConfirmMobileManager._cellHint) {
            jq("#error").show();
            jq("#error").html(ConfirmMobileManager._emptyCell);
            return;
        }
        MobileActivationController.SendAuthCode(jq("#studio_phone").val(), function(result) { ConfirmMobileManager.SendAuthCodeCallback(result); });
    }

    this.SendAuthCodeCallback = function(result) {
        var res = result.value;
        if (res.Status == 1) {
            jq("#putAuthCode .label .noise").html(res.Noise);
            if (!ConfirmMobileManager._activate) {
                jq("#sendAuthCode").hide();
            }
            else {
                jq("#putAuthCode .label").hide();
                jq("#sendAuthCode .btn").hide();

            }
            jq("#putAuthCode").show();
        }
        else {
            jq("#error").show();
            jq("#error").html(res.Message);
        }
    }

    this.SendAuthCodeAgain = function() {
        if (!ConfirmMobileManager._activate)
            MobileActivationController.SendAuthCodeAgain(function(result) { ConfirmMobileManager.SendAuthCodeCallback(result); });
        else
            MobileActivationController.SendAuthCode(jq("#studio_phone").val(), function(result) { ConfirmMobileManager.SendAuthCodeCallback(result); });
    }

    this.ValidateAuthCode = function() {
        jq("#error").hide();
        if (jq('#studio_phone_authcode').val() == '' || jq('#studio_phone_authcode').val() == ConfirmMobileManager._codeHint) {
            jq("#error").show();
            jq("#error").html(ConfirmMobileManager._emptyCode);
            return;
        }
        MobileActivationController.ValidateAuthCode(jq("#studio_phone_authcode").val(), jq('#studio_phone_keepUser').is(':checked'), function(result) { ConfirmMobileManager.ValidateAuthCodeCallback(result.value); });
    }

    this.ValidateAuthCodeCallback = function(result) {
        if (result.Status == 1)
            location.href = "./";
        else {
            jq("#error").show();
            jq("#error").html(result.Message);
        }
    }
}