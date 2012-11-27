
jq(document).ready(function() {
bindEvent('studio_confirm_Email');
bindEvent('studio_confirm_FirstName');
bindEvent('studio_confirm_LastName');
bindEvent('studio_confirm_pwd');
bindEvent('studio_confirm_repwd');
});

function bindEvent(controlId) {
    jq('#' + controlId).keyup(function(event) {
        var code;
        if (!e) var e = event;
        if (e.keyCode) code = e.keyCode;
        else if (e.which) code = e.which;

        if (code == 13)
            document.forms[0].submit();
    });
}