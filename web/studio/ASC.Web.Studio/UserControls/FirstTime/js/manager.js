if (typeof (ASC) == 'undefined')
    ASC = {};
if (typeof (ASC.Controls) == 'undefined')
    ASC.Controls = {};

ASC.Controls.FirstTimeManager = function() {

    this.OnAfterSaveRequiredData = null;
    this.OnAfterSaveData = null;

    this.SaveRequiredData = function(parentCalllback) {
        ASC.Controls.EmailAndPasswordManager.SaveRequiredData(parentCalllback);
    };
};

ASC.Controls.EmailAndPasswordManager = new function() {

    this.PassText = '';
    this.changeIt = '';
    this.ok = '';
    this.wrongPass = '';
    this.emptyPass = '';
    this.wrongEmail = '';
    this.portalName = '';

    this.ShowChangeEmailAddress = function() {
        var email = jQuery.trim(jq('.emailAddress').html());
        jq('.emailAddress').html('');
        jq('.emailAddress').append('<input type="textbox" id="newEmailAddress" maxlength="64" class="textEdit newEmail">');
        jq('.emailAddress #newEmailAddress').val(email);
        jq('.changeEmail').html('');
    };

    this.AcceptNewEmailAddress = function() {
        var email = jq('.changeEmail #dvChangeMail #newEmailAddress').val();

        if (email == '')
            return;

        jq('#requiredStep .emailBlock .email .emailAddress').html(email);
        jq('.changeEmail #dvChangeMail').html('');
        jq('.changeEmail #dvChangeMail').append('<a class="info baseLinkAction" onclick="ASC.Controls.EmailAndPasswordManager.ShowChangeEmailAddress();">' + ASC.Controls.EmailAndPasswordManager.changeIt + '</a>');
    };

    this.SaveRequiredData = function(parentCallback) {

        var email = jQuery.trim(jq('#requiredStep .emailBlock .email .emailAddress #newEmailAddress').val()); //
        if (email == '' || email == null)
            email = jQuery.trim(jq('#requiredStep .emailBlock .email .emailAddress').html());
        var pwd = jq('.passwordBlock .pwd #newPwd').val();
        var cpwd = jq('.passwordBlock .pwd #confPwd').val();
        var header = jq('#portalName').val();

        if (email == '' || !ASC.Controls.EmailAndPasswordManager.EmailValidate(email)) {
            var res = { "Status": 0, "Message": ASC.Controls.EmailAndPasswordManager.wrongEmail };
            if (parentCallback != null)
                parentCallback(res);
            return;
        }

        if (pwd != cpwd || pwd == '') {

            var res = { "Status": 0, "Message": pwd == '' ? ASC.Controls.EmailAndPasswordManager.emptyPass : ASC.Controls.EmailAndPasswordManager.wrongPass };
            if (parentCallback != null)
                parentCallback(res);
            return;
        }

        EmailAndPasswordController.SaveData(email, pwd, header, function(result) {

            if (parentCallback != null)
                parentCallback(result.value);
        });
    };

    this.EmailValidate = function(email) {
        if (email.match(/^(([\w-\s]+)|([\w-]+(?:\.[\w-]+)*)|([\w-\s]+)([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i))
            return true;
        return false;
    }

    this.SetPasswordControlSettings = function(controlName, defaultText) {
        jq('#' + controlName).focus(function() {
            jq('#' + controlName).removeClass('textEditDefault');
            jq('#' + controlName).addClass('textEditMain');
            if (jq('#' + controlName).val() == defaultText) {
                jq('#' + controlName).val('');
            }
        });

        jq('#' + controlName).blur(function() {
            if (jq('#' + controlName).val() == '') {
                jq('#' + controlName).removeClass('textEditMain');
                jq('#' + controlName).addClass('textEditDefault');
                jq('#' + controlName).val(defaultText);
            }
        });
    };
};

jq(function() {
    if (jQuery.trim(jq('.emailAddress').html()) == '') {
        ASC.Controls.EmailAndPasswordManager.ShowChangeEmailAddress();
    }
});