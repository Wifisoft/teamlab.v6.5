UserMakeResult = function(action) {
    //cancel
    //add_user 
    //edit_user

    this.Action = action;
    this.SelfProfile = false;
    this.Title = '';
    this.AddButton = '';
    this.SaveButton = '';
    this.SignUpButton = '';
};

StudioUserMaker = new function() {
    this.MaleStatus = '';
    this.FemaleStatus = '';

    jq(document).ready(function() {
        if (jq('#studio_usrPhotoUploader').length > 0) {
            new AjaxUpload('studio_usrPhotoUploader', {
                action: 'ajaxupload.ashx?type=ASC.Web.Studio.UserControls.Users.UserPhotoUploader,ASC.Web.Studio',
                onSubmit: function() { jq('#studio_um_dialogContent').block(); },
                onComplete: StudioUserMaker.ChangeUserPhoto,
                parentDialog: jq("#studio_userMakerDialog")[0]
            });
        }
    });

    this.ChangeUserPhoto = function(file, response) {
        jq('#studio_um_dialogContent').unblock();
        var result = eval("(" + response + ")");
        if (result.Success) {
            jq('#studio_um_uploadMessage').html('');
            jq('#studio_um_photoPath').val(result.Data);
            jq('#studio_um_photo').html('<img alt="" class="userPhoto" src="' + result.Data + '" />');
        }
        else {
            jq('#studio_um_uploadMessage').html('<div class="errorBox">' + result.Message + '</div>');
        }
    };

    this.OpenDialog = function() {
        HideRequiredError();

        jq('[id$="studio_umInfo_WorkFromDate"]')
            .datepick({ yearRange: 'c-80:+1', showAnim: '', popupContainer: "#studio_userMakerDialog",
                    minDate: jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('[id$=studio_umInfo_BirthDate]').val()),
                    onSelect: function(dates) {
                        var date = dates[0];
                        date.setDate(date.getDate());
                        jq('[id$="studio_umInfo_BirthDate"]').datepick('option', 'maxDate', date || null);
                    }
                });

        jq('[id$="studio_umInfo_BirthDate"]')
            .datepick({ yearRange: 'c-80:+1', showAnim: '', popupContainer: "#studio_userMakerDialog",
                    maxDate: jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('[id$=studio_umInfo_WorkFromDate]').val()),
                    onSelect: function(dates) {
                        var date = dates[0];
                        date.setDate(date.getDate());
                        jq('[id$="studio_umInfo_WorkFromDate"]').datepick('option', 'minDate', date || null);
                    }
                });

        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        try {
            jq.blockUI({ message: jq("#studio_userMakerDialog"),
                    css: {
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: '650px',
                    //              height: '700px',
                        cursor: 'default',
                        textAlign: 'left',
                        position: 'absolute',
                        'background-color': 'Transparent',
                        'margin-left': '-325px',
                        'top': 110 + jq(window).scrollTop() + "px"
                    },
                    overlayCSS: {
                        backgroundColor: '#aaaaaa',
                        cursor: 'default',
                        opacity: '0.3'
                    },
                    baseZ: 666,
                    focusInput: false,
                    fadeIn: 0,
                    fadeOut: 0,
                    onBlock: function() {
                        var
                            $blockUI = jq('#studio_userMakerDialog').parents('div.blockUI:first'),
                            blockUI = $blockUI.removeClass('blockMsg').addClass('blockDialog').get(0),
                            cssText = '';
                        if (jq.browser.msie && jq.browser.version < 9 && $blockUI.length !== 0) {
                            var
                                prefix = ' ',
                                cssText = prefix + blockUI.style.cssText,
                                startPos = cssText.toLowerCase().indexOf(prefix + 'filter:'),
                                endPos = cssText.indexOf(';', startPos);
                            if (startPos !== -1) {
                                if (endPos !== -1) {
                                    blockUI.style.cssText = [cssText.substring(prefix.length, startPos), cssText.substring(endPos + 1)].join('');
                                } else {
                                    blockUI.style.cssText = cssText.substring(prefix.length, startPos);
                                }
                            }
                        }
                    }
                });
        }
        catch(e) { };

        this.InitMask();

        jq('#studio_um_uploadMessage').html('');
        jq('input.fileuploadinput').attr('accept', 'image/png,image/jpeg,image/gif');

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = 'StudioUserMaker.SaveNewUser();';
        PopupKeyUpActionProvider.CtrlEnterAction = "jq('[id$=\"studio_umInfo_BirthDate\"],[id$=\"studio_umInfo_WorkFromDate\"]').datepick('destroy');";

        jq('#studio_um_dialogContent').show();
        jq('#studio_um_dialogMessage').hide();
    };

    this.NewUserDialogTitle = '';
    this.EditUserDialogTitle = '';

    this.UserData = null;

    this.TimeoutHandler = null;

    this.InfoPanelID = '';

    this.CallbackFunction = null;


    this.ShowEditUserDialog = function(userID, callbackFunc) {

        if (callbackFunc != undefined && callbackFunc != null)
            this.CallbackFunction = callbackFunc;
        else
            this.CallbackFunction = null;

        AjaxPro.onLoading = function(b) { };
        UserMaker.LoadUserInfo(userID, function(result) {
            StudioUserMaker.UserData = result.value;
            StudioUserMaker.ShowData();

            jq('div[id^="studio_um_new_"]').hide();
            jq('#email_property').hide();

            jq("#studio_userMakerDialogTitle").html(StudioUserMaker.EditUserDialogTitle);
            jq('#studio_um_saveButton').html(StudioUserMaker.SaveButton);

            jq('[id$="studio_umInfo_WorkFromDate"]')
                .datepick({ yearRange: 'c-80:+1', showAnim: '', popupContainer: "#studio_userMakerDialog",
                        minDate: jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('[id$=studio_umInfo_BirthDate]').val()),
                        onSelect: function(dates) {
                            var date = dates[0];
                            date.setDate(date.getDate());
                            jq('[id$="studio_umInfo_BirthDate"]').datepick('option', 'maxDate', date || null);
                        }
                    });

            jq('[id$="studio_umInfo_BirthDate"]')
                .datepick({ yearRange: 'c-80:+1', showAnim: '', popupContainer: "#studio_userMakerDialog",
                        maxDate: jq.datepick.parseDate(jq.datepick._defaults.dateFormat, jq('[id$=studio_umInfo_WorkFromDate]').val()),
                        onSelect: function(dates) {
                            var date = dates[0];
                            date.setDate(date.getDate());
                            jq('[id$="studio_umInfo_WorkFromDate"]').datepick('option', 'minDate', date || null);
                        }
                    });

            StudioUserMaker.OpenDialog();

            if (!jq('#studio_umInfo_Department').is(':disabled'))
                jq('#studio_umInfo_Department').tlcombobox();

            jq('#studio_umInfo_Sex').tlcombobox();


        });
        jq('#studio_um_dialogContent').removeClass('addUser');

    };

    this.ShowNewUserDialog = function(callbackFunc) {
        if (callbackFunc != undefined && callbackFunc != null)
            this.CallbackFunction = callbackFunc;
        else
            this.CallbackFunction = null;

        AjaxPro.onLoading = function(b) { };
        UserMaker.GetUserInfoClass(function(result) {
            StudioUserMaker.UserData = result.value;
            StudioUserMaker.ShowData();

            jq('div[id^="studio_um_new_"]').hide();
            jq("#studio_userMakerDialogTitle").html(StudioUserMaker.NewUserDialogTitle);
            jq('#studio_um_saveButton').html(StudioUserMaker.AddButton);

            StudioUserMaker.OpenDialog();
        });
        jq('#studio_um_dialogContent').addClass('addUser');
    };

    this.IsInitDateMask = false;
    this.InitMask = function() {
        if (!this.IsInitDateMask) {
            jq('[id$="studio_umInfo_BirthDate"]').mask(jq('[id$="_jqueryDateMask"]').val());
            jq('[id$="studio_umInfo_WorkFromDate"]').mask(jq('[id$="_jqueryDateMask"]').val());
            this.IsInitDateMask = true;
        }
    };

    this.TestEmailRegex = function(email) {
        var regexp = /^([a-z0-9_\.-]+)@([a-z0-9_\.-]+)\.([a-z\.]{2,6})$/i ;
        return regexp.test(email);
    };
    this.SocialContacts = { };

    this.ShowData = function() {
        jq("#studio_userMakerDialog").dialog('option', 'height', 400);

        //jq('#studio_um_message').html('');
        jq('#' + StudioUserMaker.InfoPanelID).html('');
        jq('#' + StudioUserMaker.InfoPanelID).hide();

        if (this.UserData == null)
            return;

        jq('#studio_umInfo_FirstName').val(this.UserData.Info.FirstName);
        jq('#studio_umInfo_LastName').val(this.UserData.Info.LastName);
        jq('#studio_umInfo_Sex').val("-1");
        if (this.UserData.Info.Sex != null) {
            jq('#studio_umInfo_Sex').val(this.UserData.Info.Sex ? "1" : "0");
        }

        jq('#studio_umInfo_Post').val(this.UserData.Info.Title);
        jq('#studio_um_UserLocation').val(this.UserData.Info.Location);
        jq('#studio_umInfo_Email').val(this.UserData.Info.Email);
        jq('#studio_umInfo_Location').val(this.UserData.Info.Country);
        jq('#studio_umInfo_PhoneOffice').val(this.UserData.Info.PhoneOffice);

        jq('[id$="studio_umInfo_BirthDate"]').val(this.UserData.BirthDate);
        jq('[id$="studio_umInfo_WorkFromDate"]').val(this.UserData.WorkFromDate);

        jq('#studio_umInfo_AboutMe').val(this.UserData.Info.Notes);
        jq('#studio_umInfo_Department').val(this.UserData.DepartmentID);

        jq('#studio_um_photo').html('<img alt="" class="userPhoto" src="' + this.UserData.PhotoPath + '" />');

        jq('#studio_um_photoPath').val('');

        var
            type = '',
            contacts = this.UserData.Contacts;

        for (var i = 0, n = contacts.length; i < n; i++) {
            type = contacts[i][0];
            if (!type || typeof type !== 'string') {
                continue;
            }
            this.SocialContacts[type] = {
                type: type,
                pattern: typeof contacts[i][1] === 'string' ? contacts[i][1] : ''
            };
        }

        var
            $contact = null,
            $defaultContact = jq('#contactsContainer ul:first').children('li.contact.default'),
            contacts = this.UserData.Info.Contacts;

        jq('#contactsContainer ul:first').children('li.contact').not('li.default').remove();
        if (contacts.length % 2 === 0) {
            $defaultContact.removeClass('default');
            for (var i = 0, n = contacts.length; i < n; i += 2) {
                $contact = this.createNewContact($defaultContact.clone());
                $contact.find('div.link input:first').val(contacts[i + 1]);
                $contact.find('select.type').val(contacts[i]);
                jq('#contactsContainer ul:first').append($contact);
                StudioManager.createCustomSelect($contact.find('select.type').get(0));
            }
            $defaultContact.addClass('default');
        }
        if (contacts.length === 0) {
            var $newContact = this.createNewContact(jq('#contactsContainer').find('ul:first').children('li.contact.default').clone().removeClass('default')).appendTo(jq('#contactsContainer').find('ul:first')).show();
            $newContact.find('div.link:first input').addClass('describeText');
            StudioManager.createCustomSelect($newContact.find('select.type:first').get(0));
        }
    };

    this.moveCaretToEnd = function(o) {
        if (typeof o === 'string')
            o = document.getElementById(o);
        if (!o || typeof o !== 'object')
            return undefined;
        if (o.createTextRange) {
            var range = o.createTextRange();
            //        range.collapse(false);
            range.select();
        }
        else if (o.selectionStart) {
            var end = o.value.length;
            //        o.setSelectionRange(end, end);
            o.setSelectionRange(0, end);
        }
        o.focus();
    };

    this.validateSocialContact = function(contactType, value) {
        if (!contactType || typeof this.SocialContacts[contactType] !== 'object')
            return true;

        return false;
    };

    this.ContactsMaxIndex = 1000;

    this.createNewContact = function($contact) {
        var
            $select = $contact.find('select.type:first'),
            $link = $contact.find('div.link input:first'),
            pattern = typeof this.SocialContacts[$select.val()] === 'object' ? this.SocialContacts[$select.val()].pattern : '';

        $link
            .focus(function() {
                if (jq(this).hasClass('describeText'))
                    jq(this).removeClass('describeText').val('');
            })
            .blur(function() {
                if (!this.value.length)
                    jq(this).addClass('describeText').val(StudioUserMaker.SocialContacts[jq(this).parents('li.contact:first').find('select.type').val()].pattern);
            })
            .val(pattern);

        $select.change(function() {
            var selectedContactType = StudioUserMaker.SocialContacts[this.value];
            if (typeof selectedContactType === 'object')
                jq(this).parents('li.contact:first').find('div.link input:first').addClass('describeText').val(selectedContactType.pattern);
        });
        return $contact.css({ zIndex: this.ContactsMaxIndex-- });
    };

    this.ReadData = function() {
        if (this.UserData == null)
            return;

        this.UserData.Info.FirstName = jq('#studio_umInfo_FirstName').val();
        this.UserData.Info.LastName = jq('#studio_umInfo_LastName').val();
        if (jq('#studio_umInfo_Sex').val() == '1') this.UserData.Info.Sex = true;
        if (jq('#studio_umInfo_Sex').val() == '0') this.UserData.Info.Sex = false;
        if (jq('#studio_umInfo_Sex').val() == '-1') this.UserData.Info.Sex = null;

        this.UserData.Info.Title = jq('#studio_umInfo_Post').val();
        this.UserData.Info.Location = jq('#studio_um_UserLocation').val();
        this.UserData.Info.Email = jq('#studio_umInfo_Email').val();

        this.UserData.Info.Notes = jq('#studio_umInfo_AboutMe').val();
        if (this.UserData.Info.Notes == null || this.UserData.Info.Notes == 'null')
            this.UserData.Info.Notes = '';

        this.UserData.Info.Country = jq('#studio_umInfo_Location').val();
        this.UserData.Info.PhoneOffice = jq('#studio_umInfo_PhoneOffice').val();
        this.UserData.Info.BirthDate = null;
        this.UserData.Info.WorkFromDate = null;
        this.UserData.BirthDate = jq('[id$="studio_umInfo_BirthDate"]').val();
        this.UserData.WorkFromDate = jq('[id$="studio_umInfo_WorkFromDate"]').val();
        this.UserData.DepartmentID = jq('#studio_umInfo_Department').val();

        var
            type = '',
            value = '',
            $contact = null,
            $contacts = jq('#contactsContainer ul:first').children('li.contact').not('li.default');

        this.UserData.Info.Contacts = [];
        for (var i = 0, n = $contacts.length; i < n; i++) {
            $contact = $contacts.slice(i, i + 1);
            type = $contact.find('select.type').val();
            value = $contact.find('div.link input:first').val();
            if (type && value && value !== this.SocialContacts[type].pattern) {
                this.UserData.Info.Contacts.push(type);
                this.UserData.Info.Contacts.push(value);
            }
        }
    };

    this.SaveNewUser = function() {
        HideRequiredError();
        var firstName = jq("input[id$='_FirstName']");
        var lastName = jq("input[id$='_LastName']");
        var email = jq("input[id$='_Email']");

        if (jq.trim(jq(firstName).val()) == "") {
            ShowRequiredError(firstName);
            return;
        }
        if (jq.trim((lastName).val()) == "") {
            ShowRequiredError(lastName);
            return;
        }

        if (jq('#email_property').is(":visible")) {
            var emailStr = jq.trim(jq(email).val());
            if (!this.TestEmailRegex(emailStr)) {
                ShowRequiredError(email);
                return;
            }
        }

        jq('#' + StudioUserMaker.InfoPanelID).html('');
        jq('#' + StudioUserMaker.InfoPanelID).hide();

        AjaxPro.onLoading = function(b) {
            if (b) {
                //jq('#studio_userMakerDialog').block();
                jq('#emp_add_panel_buttons').hide();
                jq('#emp_add_action_loader').show();
            }
            else {
                jq('#emp_add_action_loader').hide();
                jq('#emp_add_panel_buttons').show();
                //jq('#studio_userMakerDialog').unblock();
            }
        };

        this.ReadData();
        UserMaker.SaveUser(this.UserData, jq('#studio_um_photoPath').val(), function(result) {
            var res = result.value;
            if (res.rs1 == '1') {
                var makeResult = new UserMakeResult(res.rs3);
                makeResult.Title = res.rs10;
                if (res.rs5 == '1')
                    makeResult.SelfProfile = true;

                StudioUserMaker.CloseNewUserDialog(makeResult);
                document.location.href = document.location.href;
            }
            else {
                //jq('#studio_um_message').html(res.rs2);
                jq('#' + StudioUserMaker.InfoPanelID).html(res.rs2);
                jq('#' + StudioUserMaker.InfoPanelID).show();
            }
        });
    };


    this.editContacts = function(evt) {
        evt = evt || window.event;
        var $target = jq(evt.target || evt.srcElement);
        if ($target.hasClass('add')) {
            var $newContact = this.createNewContact(jq('#contactsContainer').find('ul:first').children('li.contact.default').clone().removeClass('default')).appendTo(jq('#contactsContainer').find('ul:first')).show();
            $newContact.find('div.link:first input').addClass('describeText');
            StudioManager.createCustomSelect($newContact.find('select.type:first').get(0));
        } else if ($target.hasClass('remove')) {
            $target.parents('li.contact:first').remove();
        }
    };

    this.ToggleTabs = function(tab) {
        if (tab.id && !jq(tab).hasClass('active')) {
            jq('li.tabTitle').removeClass('active');
            jq(tab).addClass('active');
            jq('div.propertiesContainer:first div.properties').removeClass('active').filter('div.' + tab.id).addClass('active');
        }
    };

    this.CloseNewUserDialog = function(result) {

        PopupKeyUpActionProvider.ClearActions();

        jq.unblockUI();

        AjaxPro.onLoading = function(b) { };
        UserMaker.RemoveTempPhoto(jq('#studio_um_photoPath').val(), function(result) { });

        if (result != undefined && result != null && this.CallbackFunction != null)
            this.CallbackFunction(result);
        else if (this.CallbackFunction != null)
            this.CallbackFunction(new UserMakeResult('cancel'));
    };
};