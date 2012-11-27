window.master = {
    init: function() {
        if (window.addEventListener) {
            window.addEventListener('message', master.listener, false);
        } else {
            window.attachEvent('onmessage', master.listener);
        }
    },
    listener: function(evt) {
        var obj;
        if (typeof evt.data == "string") {
            try {
                obj = jQuery.parseJSON(evt.data);
            }
            catch (err) { return; }
        }
        else
            obj = evt.data;

        if (obj.Tpr == null)
            return;

        if (obj.Tpr == undefined || obj.Tpr != "Importer")
            retuarn;

        if (obj.Data.length == 0) {
            ImportUsersManager.ShowImportErrorWindow(ImportUsersManager._emptySocImport);
            return;
        }

        jq('#userList').find("tr").not(".userItem").remove();
        var parent = jq('#userList');
        var items = parent.find(".userItem");
        for (var i = 0; i < obj.Data.length; i++)
            if (!(ImportUsersManager.isExists(items, obj.Data[i].Email, false)))
            ImportUsersManager.AppendUser(obj.Data[i]);
        ImportUsersManager.BindHints();

    }
};
master.init();

var ImportUsersManager = new function() {
    this.FName = '';
    this.EmptyFName = '';
    this.LName = '';
    this.EmptyLName = '';
    this.Email = '';
    this._info = true;
    this._mobile = false;

    this._errorImport = '';
    this._emptySocImport = '';

    this.ShowImportControl = function() {
        jq("#okwin").hide();
        jq('#checkAll').attr('checked', false);
        jq('#userList').find("tr").remove();
        jq('#fnError').css('visibility', 'hidden');
        jq('#lnError').css('visibility', 'hidden');
        jq('#eaError').css('visibility', 'hidden');
        jq('#donor tr').clone().appendTo('#userList');

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = 'ImportUsersManager.CheckAndAdd();';
        PopupKeyUpActionProvider.CtrlEnterAction = "ImportUsersManager.ImportList();";

        jq.blockUI({ message: jq("#importAreaBlock"),
            css: {
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '810px',
                height: '520px',
                height: '545px',
                cursor: 'default',
                textAlign: 'left',
                'margin-left': '-405px',
                'top': '20%'
            },
            overlayCSS: {
                backgroundColor: '#aaaaaa',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            fadeIn: 0,
            fadeOut: 0
        });
    }

    this.ShowFinish = function() {
        jq.blockUI({ message: jq('#successImportedArea'),
            css: {
                width: '700px',
                height: '130px',
                opacity: '1',
                border: 'none',
                padding: '0px',
                cursor: 'default',
                'margin-left': '-350px'
            },
            overlayCSS: {
                backgroundColor: '#aaaaaa',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            fadeIn: 0,
            fadeOut: 0
        });
    }

    this.ShowFinishWindow = function() {

        jq("#importAreaBlock").css('background-color', 'Transparent');
        jq("#importAreaBlock").css('background-color', '#aaaaaa');
        jq("#importAreaBlock").css('opacity', '0.3');
        jq("#okwin").show();
    }

    this.HideImportWindow = function() {
        ImportUsersManager.DefaultState();

        jq("#okwin").hide();
        jq('#userList').empty();
        jq.unblockUI();
    }
    this.ChangeVisionFileSelector = function() {
        if (jq('.importUsers div.file .fileSelector').css('display') == 'none')
            jq('.importUsers div.file .fileSelector').show();
        else
            jq('.importUsers div.file .fileSelector').hide();
    }

    this.DefaultState = function() {
        ImportUsersManager.AddHint('#firstName', ImportUsersManager.FName);
        ImportUsersManager.AddHint('#lastName', ImportUsersManager.LName);
        ImportUsersManager.AddHint('#email', ImportUsersManager.Email);

        jq('#email').removeClass('incorrectEmailBox');
        jq('#firstName').removeClass('incorrectEmailBox');
        jq('#lastName').removeClass('incorrectEmailBox');
    }

    this.AddUser = function() {
        var parent = jq('#userList');
        var items = jq('#userList').find(".userItem");

        var firstName = jQuery.trim(jq('#firstName').val());
        var lastName = jQuery.trim(jq('#lastName').val());
        var address = jQuery.trim(jq('#email').val());

        if (!(ImportUsersManager.isExists(items, address, true) || address == '' || firstName == '' || firstName == this.FName || lastName == '' || lastName == this.LName || !ImportUsersManager.ValidateEmail(address) || address == this.Email)) {
            jq('#userList').find("tr").not(".userItem").remove();
            jq('#email').removeClass('incorrectEmailBox');
            jq('#firstName').removeClass('incorrectEmailBox');
            jq('#lastName').removeClass('incorrectEmailBox');
            jq('#fnError').css('visibility', 'hidden');
            jq('#lnError').css('visibility', 'hidden');
            jq('#eaError').css('visibility', 'hidden');
            this.AppendUser({ FirstName: jq('#firstName').val(), LastName: jq('#lastName').val(), Email: jq('#email').val() });
            jq('#firstName').val('');
            jq('#firstName').blur();
            jq('#lastName').val('');
            jq('#lastName').blur();
            jq('#email').val('');
            jq('#email').blur();

            if (jq('#userList').find('.userItem').not('.saved, .error2').length == jq('#userList').find('.userItem .check input:checked').length)
                jq('#checkAll').attr('checked', true);
        }
        else {
            if (firstName == '' || firstName == this.FName) {
                jq('#firstName').addClass('incorrectEmailBox');
                jq("#firstName").focus();
                jq('#fnError').css('visibility', 'visible');
            }
            else {
                jq('#firstName').removeClass('incorrectEmailBox');
                jq('#fnError').css('visibility', 'hidden');
            }

            if (lastName == '' || lastName == this.LName) {
                jq('#lastName').addClass('incorrectEmailBox');
                jq('#lnError').css('visibility', 'visible');

                if (!ImportUsersManager.WasFocused("firstName"))
                    jq("#lastName").focus();
            }
            else {
                jq('#lastName').removeClass('incorrectEmailBox');
                jq('#lnError').css('visibility', 'hidden');
            }

            if (address == '' || !ImportUsersManager.ValidateEmail(address)) {
                jq('#email').addClass('incorrectEmailBox');
                jq('#eaError').css('visibility', 'visible');
                if (!(ImportUsersManager.WasFocused('firstName') || ImportUsersManager.WasFocused('lastName')))
                    jq("#email").focus();
            }
            else {
                jq('#email').removeClass('incorrectEmailBox');
                jq('#lnError').css('visibility', 'hidden');
            }
        }
    }

    this.CheckAndAdd = function() {
        var firstName = jQuery.trim(jq('#firstName').val());
        var lastName = jQuery.trim(jq('#lastName').val());
        var address = jQuery.trim(jq('#email').val());

        if (firstName == '' || firstName == this.FName) {
            if (ImportUsersManager.WasFocused("firstName")) {
                jq('#firstName').addClass('incorrectEmailBox');
                jq('#fnError').css('visibility', 'visible');
            }
            else {
                jq("#firstName").focus();
            }
            return;
        }
        else {
            jq('#firstName').removeClass('incorrectEmailBox');
            jq('#fnError').css('visibility', 'hidden');
        }

        if (lastName == '' || lastName == this.LName) {

            if (ImportUsersManager.WasFocused("lastName")) {
                jq('#lastName').addClass('incorrectEmailBox');
                jq('#lnError').css('visibility', 'visible');
            }
            else {
                jq("#lastName").focus();
            }
            return;
        }
        else {
            jq('#lastName').removeClass('incorrectEmailBox');
            jq('#lnError').css('visibility', 'hidden');
        }

        if (address == '' || !ImportUsersManager.ValidateEmail(address)) {
            if (ImportUsersManager.WasFocused("email")) {
                jq('#email').addClass('incorrectEmailBox');
                jq('#eaError').css('visibility', 'visible');
            }
            else {
                jq("#email").focus();
            }
            return;
        }
        else {
            jq('#email').removeClass('incorrectEmailBox');
            jq('#eaError').css('visibility', 'hidden');
        }

        ImportUsersManager.AddUser();
    }

    this.WasFocused = function(elementName) {
        return jQuery('#' + elementName).is(':focus');
    }

    this.ValidateEmail = function(email) {
        var reg = /^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i;
        if (reg.test(email) == true)
            return true;
        return false;
    }

    this.isExists = function(items, email, select) {
        for (var index = 0; index < items.length; index++) {
            if (jQuery.trim(jq(items[index]).find('.email input').val()) == email) {
                if (select)
                    jq(items[index]).find('.email').addClass('incorrectValue');
                return true;
            }
            jq(items[index]).find('.email').removeClass('incorrectValue');
        }
        return false;
    }

    this.AppendUser = function(item) {
        jq('#userRecord').tmpl(item).appendTo('#userList');
    }

    this.RemoveItem = function(item) {
        jq(item).parent().parent().addClass('remove');
        if (jq('#userList').find(".userItem").not('.remove').length == 0)
            jq('#addUserBlock').removeClass('bordered');
        jq(item).parent().parent().remove();

        if (jq('#userList').find('.userItem').length == 0)
            jq('#donor tr').clone().appendTo('#userList');
    }

    this.GetUsers = function() {

        var items = jq('#userList').find('.userItem').not('.remove, .saved');
        var allItems = jq('#userList').find('.userItem').not('.saved, .error2');
        var items = [];
        jQuery.each(allItems, function() {
            if (jq(this).find('.check input').is(':checked'))
                items.push(this);
        });
        var arr = new Array();

        for (var i = 0; i < items.length; i++) {
            arr.push({ "FirstName": this.CheckValue(jQuery.trim(jq(items[i]).find('.name .firstname .studioEditableInput').val()), this.EmptyFName), "LastName": this.CheckValue(jQuery.trim(jq(items[i]).find('.name .lastname .studioEditableInput').val()), this.EmptyLName), "Email": jQuery.trim(jq(items[i]).find('.email input').val()) });
        }
        return arr;
    }

    this.CheckValue = function(value, hint) {
        return (value == hint) ? "" : value;
    }

    this.DeleteSelected = function() {
        var items = jq('#userList .userItem');
        jQuery.each(items, function() {
            if (jq(this).find('.check input').is(':checked'))
                jq(this).remove();
        });
        if (jq('#userList .userItem').length == 0)
            jq('#checkAll').attr('checked', false);
    }

    this.ChangeAll = function() {
        var state = jq('#checkAll').is(':checked');
        var items = jq('#userList .userItem');
        jQuery.each(items, function() {
            jq(this).find('.check input').attr('checked', state);
        });
    }

    this.CheckState = function() {
        var checkedCount = jq('#userList .userItem .check input:checkbox:checked').length;
        var usersCount = jq('#userList').find('.userItem').length;
        var st = false;

        if (checkedCount == usersCount && usersCount > 0)
            st = true;

        jq('#checkAll').attr('checked', st);
    }

    this.EraseError = function(item) {
        jq(item).parents(".userItem").attr('class', jq(item).parents(".userItem").attr('class').replace(/saved\d/gi, ''));
    }

    this.ImportList = function() {

        if (jq('#userList').find('.userItem').not('fistable').length == 0)
            return;

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#importAreaBlock').block();
            else
                jq('#importAreaBlock').unblock();
        };
        var users = ImportUsersManager.GetUsers();

        if (users.length > 0) {
            ImportUsersController.SaveUsers(JSON.stringify(users), function(result) { var res = result.value; ImportUsersManager.SaveUsersCallback(res); });
            return;
        }
        ImportUsersManager.CloseWindow(false);
    }

    this.CloseWindow = function(checkMistakes, mistakes) {
        if (!checkMistakes) {
            if (jq('#userList').find('.saved').length > 0 && users.length != 0) {
                ImportUsersManager.InformImportedWindow();
            }
        }
        else {
            if (mistakes == 0 || jq('#userList').find('.saved').length > 0) {
                ImportUsersManager.InformImportedWindow();
            }
            else
                ImportUsersManager.BindHints();
        }
    }

    this.InformImportedWindow = function() {
        ImportUsersManager.DefaultState();

        jq.unblockUI();
        ImportUsersManager.ShowFinish();
        setTimeout(function() { jq.unblockUI(); }, 3000);
    }

    this.SaveUsersCallback = function(result) {
        var parent = jq('#userList .userItem');
        var mistakes = 0;
        jQuery.each(parent, function() {
            var _mail = jQuery.trim(jq(this).find('.email input').val());
            for (var i = 0; i < result.Data.length; i++) {
                if (result.Data[i].Email == _mail) {
                    if (result.Data[i].Result != '') {
                        mistakes++;
                        jq(this).addClass(result.Data[i].Class);
                        jq(this).find('.errors').attr('title', result.Data[i].Result);
                        jq(this).find('.remove').addClass('removeError');
                    }
                    else {
                        jq(this).addClass('saved');
                        var attrs = jq(this).attr('class');
                        jq(this).attr('class', attrs.replace(/error\d/gi, ''));
                        jq(this).find('.remove').removeClass('removeError');
                    }
                }
            }
        });
        if (mistakes == 0)
            jq.unblockUI();
        else
            ImportUsersManager.BindHints();
        ImportUsersManager.CloseWindow(true, mistakes);
    }

    this.BindHints = function() {
        var parent = jq('#userList .studioEditableInput');
        jQuery.each(parent, function() {
            if (jq(this).val() == '') {
                if (jq(this).is('[class*="firstName"]'))
                    ImportUsersManager.AddHint(this, ImportUsersManager.EmptyFName);
                if (jq(this).is('[class*="lastName"]'))
                    ImportUsersManager.AddHint(this, ImportUsersManager.EmptyLName);
            }
        });
    }

    this.HideInfoWindow = function(info) {
        jq('#blockProcess').hide();
        jq('.' + info).hide();
    }

    this.UploadResult = function(file, response) {
        jq('#upload').hide();

        var result = eval("(" + response + ")");
        if (result.Success) {
            var extractedUsers = JSON.parse(result.Message)
            jq('#userList').find("tr").not(".userItem").remove();

            var parent = jq('#userList');
            var mistakes = 0;
            var copy = 0;
            var items = jq('#userList').find(".userItem");
            for (var i = 0; i < extractedUsers.length; i++) {
                if (ImportUsersManager.ValidateEmail(extractedUsers[i].Email)) {

                    if (ImportUsersManager.isExists(items, extractedUsers[i].Email, false)) {
                        copy++;
                        continue;
                    }
                    ImportUsersManager.AppendUser(extractedUsers[i]);
                }
                else
                    mistakes++;
            }

            if (mistakes > 0 && copy != extractedUsers.length)
                ImportUsersManager.ShowImportErrorWindow(ImportUsersManager._errorImport);

            ImportUsersManager.BindHints();
        }
        else {
            jq('#blockProcess').show();
            jq('.okcss').show();
            ImportUsersManager.ShowImportErrorWindow(ImportUsersManager._errorImport);
        }
    }

    this.ShowImportErrorWindow = function(message) {
        jq('#blockProcess').show();
        jq('.okcss #infoMessage').html(message);
        jq('.okcss').show();
    }

    this.HideImportErrorWindow = function() {
        jq('#blockProcess').hide();
        jq('.okcss #infoMessage').html('');
        jq('.okcss').hide();

        ImportUsersManager.DefaultState();
    }

    this.SetControlHintSettings = function(controlName, defaultText) {
        jq(controlName).focus(function() {
            jq(controlName).removeClass('textEditDefault');
            jq(controlName).addClass('textEditMain');
            if (jq(controlName).val() == defaultText) {
                jq(controlName).val('')
            }
        });

        jq(controlName).blur(function() {
            if (jq(controlName).val() == '') {
                jq(controlName).removeClass('textEditMain');
                jq(controlName).addClass('textEditDefault');
                jq(controlName).val(defaultText);
            }
        });
    }

    this.AddHint = function(control, text) {
        jq(control).val(text);
        jq(control).addClass('textEditDefault');
        ImportUsersManager.SetControlHintSettings(control, text);
    }

    this.InitUploader = function(control, handler) {
        if (jq('#' + control).length > 0) {
            new AjaxUpload(control, {
                action: handler,
                onSubmit: function(file, ext) {
                    jq('#upload').show();
                },
                onComplete: ImportUsersManager.UploadResult,
                parentDialog: (jq.browser.msie && jq.browser.version == 9) ? jq("#"+control).parent() : false,
                isInPopup: (jq.browser.msie && jq.browser.version == 9)
            });
        }
    }
} 

jq(document).ready(function() {

    jq("input.fileuploadinput").attr('accept', 'text/plain,application/octet-stream');
    ImportUsersManager.InitUploader('import_flatUploader', 'ajaxupload.ashx?type=ASC.Web.Studio.UserControls.Users.ContactsUploader,ASC.Web.Studio&obj=txt');
    ImportUsersManager.InitUploader('import_msUploader', 'ajaxupload.ashx?type=ASC.Web.Studio.UserControls.Users.ContactsUploader,ASC.Web.Studio&obj=ms');

    ImportUsersManager.AddHint('#firstName', ImportUsersManager.FName);
    ImportUsersManager.AddHint('#lastName', ImportUsersManager.LName);
    ImportUsersManager.AddHint('#email', ImportUsersManager.Email);
});