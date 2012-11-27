if (typeof jq == "undefined")
    var jq = jQuery.noConflict();

jQuery.extend(
    jQuery.expr[":"],
    { reallyvisible: function(a) { return !(jQuery(a).is(':hidden') || jQuery(a).parents(':hidden').length); } }
);

jQuery.extend(
    jQuery.browser,
    {
        chrome: typeof(window.chrome) === "object",
        safari: jq.browser.safari && !window.chrome,
        versionCorrect: function () {
            var ua = navigator.userAgent;
            var ver = (ua.match( /.+(?:rv|it|ra|ie|ox|me|id|on|os)[\/:\s]([\d._]+)/i ) || [0, '0'])[1].replace('_', '');
            var floatVer = parseFloat(ver);
            return isFinite(floatVer) ? floatVer : ver;
        }()
    }
);

var fckPrevValue = new Array();

jq(document).ready(function() {

    //for mobile css
    if (jq.browser.mobile)
        jq("body").addClass("mobile");

    //fancy zoom settings
    jq('a.fancyzoom').each(function(i, el) {

        var url = jq(this).attr('href');
        jq(this).attr('href', '#studio_imgfancyzoom_' + i + '_scr');
        jq('form').append('<div id="studio_imgfancyzoom_' + i + '_scr" style="display:none;"><img style="max-height:800px;" id="studio_imgfancyzoom_' + i + '_img" src="' + url + '"></div>');

    });

    LoadingBanner.strLoading = StudioManager.LoadingProcessing;
    LoadingBanner.strDescription = StudioManager.LoadingDescription;

    //uploader settings
    if (typeof (ASC) != 'undefined' && typeof (ASC.Controls) != 'undefined' && typeof (ASC.Controls.FileUploaderGlobalConfig) != 'undefined') {
        ASC.Controls.FileUploaderGlobalConfig.DeleteText = StudioManager.RemoveMessage;
        ASC.Controls.FileUploaderGlobalConfig.ErrorFileSizeLimitText = StudioManager.ErrorFileSizeLimit;
        ASC.Controls.FileUploaderGlobalConfig.ErrorFileEmptyText = StudioManager.ErrorFileEmpty;
        ASC.Controls.FileUploaderGlobalConfig.ErrorFileTypeText = StudioManager.ErrorFileTypeText;

        ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass = 'studioFileUploaderDescription';
        ASC.Controls.FileUploaderGlobalConfig.FileNameCSSClass = 'studioFileUploaderFileName';
        ASC.Controls.FileUploaderGlobalConfig.DeleteLinkCSSClass = 'linkAction';
        ASC.Controls.FileUploaderGlobalConfig.ProgressBarCSSClass = 'studioFileUploaderProgressBar';
        ASC.Controls.FileUploaderGlobalConfig.ErrorBarCSSClass = 'studioFileUploaderErrorProgressBar';
        ASC.Controls.FileUploaderGlobalConfig.ErrorTextCSSClass = 'studioFileUploaderErrorDescription';
    }

    //settings preloaded
    jq.blockUI.defaults.css = {};
    jq.blockUI.defaults.overlayCSS = {};
    jq.blockUI.defaults.fadeOut = 0;
    jq.blockUI.defaults.fadeIn = 0;
    jq.blockUI.defaults.message = '<img alt="" src="' + SkinManager.GetImage('mini_loader.gif') + '"/>';

    var isPromo = false;
    try {
        if (window.PromoMode && PromoMode != 'undefined' && PromoMode != undefined && PromoMode != null && PromoMode)
            isPromo = true;
    }
    catch (e) { isPromo = false }

    if (isPromo) {
        jq('.promoAction').each(function(i, el) {
            jq(this).unbind('click');
            jq(this).attr('onclick', 'void(0)');
            jq(this).attr('href', PromoActionURL);
        });
    }

    //cancel fckeditor changes button prepare
    jq('.cancelFckEditorChangesButtonMarker').each(function() {
        var fckEditorId = jq(this).attr('name');
        if (fckEditorId == null || fckEditorId == '') {
            return;
        }

        var cancelButtonClick = jq(this).attr('onclick');
        jq(this).attr('onclick', '');
        jq(this).click(
            function(e) {
                if (!CancelButtonController.ConfirmCancellationWithPrevValue(fckEditorId, fckPrevValue[fckEditorId])) {
                    return false;
                }
                if (cancelButtonClick)
                    eval('(function(e){' + cancelButtonClick + '})')(e);
            }
        );

        addFckEditor_OnComplete(function(editorInstance, obj) {
            if (editorInstance.Name == jq(obj).attr('name')) {
                fckPrevValue[jq(obj).attr('name')] = editorInstance.GetXHTML();
            }
        }, this);

        window.onbeforeunload =
            function(e) {
                if (fckEditorId) {
                    var curLink = jq('[name="' + fckEditorId + '"]');
                    if (!curLink.is(':reallyvisible')) {
                        return;
                    }

                    var prev = '';
                    if (fckPrevValue[fckEditorId]) {
                        prev = fckPrevValue[fckEditorId];
                    }

                    if (!CancelButtonController.IsEmptyFckEditorTextFieldWithPrevValue(fckEditorId, prev)) {
                        return CommonJavascriptResources.CancelConfirmMessage;
                    } else {
                        return;
                    }
                }
            };

    });

    keepSessionAliveForFckEditor();

});


function keepSessionAliveForFckEditor() {
    setTimeout(function() {
        var hasFckEditor = false;

        try {
            for (var fckInstance in FCKeditorAPI.Instances) {
                hasFckEditor = true;
                if (!CancelButtonController.IsEmptyFckEditorTextField(fckInstance)) {
                    AjaxPro.onLoading = function(b) { };
                    WebStudio.KeepSessionAlive(function(res) { });
                }
            }
        } catch (err) { }

        if (hasFckEditor) {
            keepSessionAliveForFckEditor();
        }
    }, 3 * 60 * 1000);
};


var PopupKeyUpActionProvider = new function() {
    //close dialog by esc
    jq(document).keyup(function(event) {

        if (!jq('.popupContainerClass').is(':visible'))
            return;

        var code;
        if (!e) var e = event;
        if (e.keyCode) code = e.keyCode;
        else if (e.which) code = e.which;

        if (code == 27 && PopupKeyUpActionProvider.EnableEsc) {
            PopupKeyUpActionProvider.CloseDialog();
        }

        else if ((code == 13) && e.ctrlKey) {
            if (PopupKeyUpActionProvider.CtrlEnterAction != null && PopupKeyUpActionProvider.CtrlEnterAction != '')
                eval(PopupKeyUpActionProvider.CtrlEnterAction);
        }
        else if (code == 13) {
            if (e.target.nodeName.toLowerCase() !== 'textarea' && PopupKeyUpActionProvider.EnterAction != null && PopupKeyUpActionProvider.EnterAction != '')
                eval(PopupKeyUpActionProvider.EnterAction);
        }

    });

    this.CloseDialog = function() {
        jq.unblockUI();

        if (PopupKeyUpActionProvider.CloseDialogAction != null && PopupKeyUpActionProvider.CloseDialogAction != '')
            eval(PopupKeyUpActionProvider.CloseDialogAction);

        PopupKeyUpActionProvider.ClearActions();
    };

    this.CloseDialogAction = '';
    this.EnterAction = '';
    this.CtrlEnterAction = '';
    this.EnableEsc = true;

    this.ClearActions = function() {
        this.CloseDialogAction = '';
        this.EnterAction = '';
        this.CtrlEnterAction = '';
        this.EnableEsc = true;
    };
};

var AuthManager = new function() {
    this.ConfirmMessage = 'Are you sure?';

    jq(document).ready(function() {
        var currentURL = new String(document.location);
        if (currentURL.indexOf('auth.aspx') != -1) {
            jq('#pwd').keydown(function(event) {
                var code;
                if (!e) var e = event;
                if (e.keyCode) code = e.keyCode;
                else if (e.which) code = e.which;

                if (code == 13)
                    AuthManager.Login();
                if (code == 27)
                    jq('#pwd').val('');

            });

        };
    });

    this.ShowPwdReminderDialog = function(pswdRecoveryType, userEmail) {

        //pswdRecoveryType can be
        //1 if the password need for change
        //0 if the password need for recovery
        //if pswdRecoveryType == 1 => userEmail must be setted

        if (pswdRecoveryType == 1) {
            jq("#pswdRecoveryDialogPopupHeader").hide();
            jq("#pswdRecoveryDialogText").hide();
            jq("#pswdChangeDialogPopupHeader").show();
            jq("#pswdChangeDialogText").show();

            jq('#studio_emailPwdReminder').val(userEmail);
            jq("#studio_pwdReminderDialog [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }
        else {
            jq("#pswdRecoveryDialogPopupHeader").show();
            jq("#pswdRecoveryDialogText").show();
            jq("#pswdChangeDialogPopupHeader").hide();
            jq("#pswdChangeDialogText").hide();
        }

        //jq('#studio_pwdReminderInfo').html('');
        jq('#' + jq('#studio_pwdReminderInfoID').val()).html('<div></div>');
        jq('#' + jq('#studio_pwdReminderInfoID').val()).hide();

        try {

            jq.blockUI({ message: jq("#studio_pwdReminderDialog"),
                css: {
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: '350px',
                    height: '400px',
                    cursor: 'default',
                    textAlign: 'left',
                    'background-color': 'Transparent',
                    'margin-left': '-175px',
                    'top': '25%'
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
        catch (e) { };

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = 'AuthManager.RemindPwd();';

        jq('#studio_pwdReminderContent').show();
        jq('#studio_pwdReminderMessage').hide();

        var loginEmail = jq('#login').val();
        if (loginEmail != null && loginEmail != undefined && loginEmail.indexOf('@') >= 0) {
            jq('#studio_emailPwdReminder').val(loginEmail);
        }
    };


    this.RemindPwd = function() {
        AjaxPro.onLoading = function(b) {
            if (b) {
                //jq('#studio_pwdReminderDialog').block();
                jq('#pwd_rem_panel_buttons').hide();
                jq('#pwd_rem_action_loader').show();
            }
            else {
                //jq('#studio_pwdReminderDialog').unblock();
                jq('#pwd_rem_action_loader').hide();
                jq('#pwd_rem_panel_buttons').show();
            }
        };

        MySettings.RemaindPwd(jq('#studio_emailPwdReminder').val(), function(result) {
            var res = result.value;
            if (res.rs1 == "1") {
                jq('#studio_emailPwdReminder').val('');
                //jq('#studio_pwdReminderMessage').html(res.rs2);

                jq('#studio_pwdReminderContent').hide();
                //jq('#studio_pwdReminderMessage').show();

                jq('#' + jq('#studio_pwdReminderInfoID').val()).removeClass('alert')
                jq('#' + jq('#studio_pwdReminderInfoID').val()).addClass('info')
                jq('#' + jq('#studio_pwdReminderInfoID').val()).html("<div>" + res.rs2 + "</div>");
                jq('#' + jq('#studio_pwdReminderInfoID').val()).show();

                setTimeout("jq.unblockUI();", 2000)
            }
            else {
                jq('#' + jq('#studio_pwdReminderInfoID').val()).removeClass('info')
                jq('#' + jq('#studio_pwdReminderInfoID').val()).addClass('alert')
                jq('#' + jq('#studio_pwdReminderInfoID').val()).html(res.rs2);
                jq('#' + jq('#studio_pwdReminderInfoID').val()).show();
                //jq('#studio_pwdReminderInfo').html(res.rs2);
            }
        });
    };

    this.ClosePwdReminder = function() {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };

    this.ConfirmInvite = function() {
        __doPostBack('confirmInvite', '');
        //document.forms[0].submit();
    };

    this.Login = function(type) {
        if (type != undefined && type != null && type == 'demo')
            jq('#studio_authType').val('demo');
        else
            jq('#studio_authType').val('');

        if (type == undefined) {
            if (jq('#login').is('input[type="hidden"]'))
                jq('#studio_authType').val('name');
            else if (jq('#login').is('input[type="text"]'))
                jq('#studio_authType').val('email');
        }

        document.forms[0].submit();
    };

    this.CloseInviteJoinDialog = function() {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };


    this.ShowAdminMessageDialog = function() {

        if (jq('#studio_admMessDialog:visible').length > 0 && jq('#studio_admMessage:visible').length == 0) {
            this.SendAdmMail1stState();
            return;
        }

        jq('#GreetingBlock .submenu .signUpBlock .join').removeClass('opened');
        jq('#GreetingBlock .submenu .signUpBlock .mess').addClass('opened');
        jq('#studio_invJoinDialog').hide();
        jq('#studio_admMessDialog').show();
        jq('#studio_admMessInfo').html('');
        jq('#studio_yourEmail').val('');
        jq('#studio_yourSituation').val('');

        jq('#studio_admMessContent').show();
        jq('#studio_admMessage').hide();

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.CtrlEnterAction = 'AuthManager.SendAdminMessage();';
    }

    this.SendAdminMessage = function() {
        AjaxPro.onLoading = function(b) {
            if (b) {
                jq('#adm_panel_buttons').hide();
                jq('#adm_action_loader').show();
            }
            else {
                jq('#adm_action_loader').hide();
                jq('#adm_panel_buttons').show();

            }
        };

        AuthCommunicationsController.SendAdmMail(jq('#studio_yourEmail').val(), jq('#studio_yourSituation').val(), function(result) {
            var res = result.value;
            if (res.Status == 1) {
                jq('#studio_admMessage').html(res.Message);
                jq('#studio_admMessContent').hide();
                jq('#studio_admMessage').show();

                setTimeout('AuthManager.SendAdmMail1stState();', 3000);
            }
            else
                jq('#studio_admMessage').html('<div class="errorBox">' + res.Message + '</div>');

        });
    };

    this.SendAdmMail1stState = function() {
        jq('#GreetingBlock .submenu .signUpBlock .mess').removeClass('opened');
        jq('#studio_admMessDialog').hide();
    }

    this.ShowInviteJoinDialog = function(userID) {

        if (jq('#studio_invJoinDialog:visible').length > 0 && jq('#studio_invJoinMessage:visible').length == 0) {
            this.ShowInviteJoin1stState();
            return;
        }
        jq('#GreetingBlock .submenu .signUpBlock .mess').removeClass('opened');
        jq('#GreetingBlock .submenu .signUpBlock .join').addClass('opened');
        jq('#studio_invJoinInfo').html('');
        jq('#studio_joinEmail').val('');

        jq('#studio_admMessDialog').hide();
        jq('#studio_invJoinDialog').show();

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.CtrlEnterAction = 'AuthManager.SendInviteJoinMail();';

        jq('#studio_invJoinContent').show();
        jq('#studio_invJoinMessage').hide();
    };

    this.SendInviteJoinMail = function() {
        AjaxPro.onLoading = function(b) {
            if (b) {
                jq('#join_inv_panel_buttons').hide();
                jq('#join_inv_action_loader').show();
            }
            else {
                jq('#join_inv_action_loader').hide();
                jq('#join_inv_panel_buttons').show();

            }
        };

        AuthCommunicationsController.SendJoinInviteMail(jq('#studio_joinEmail').val(), function(result) {
            var res = result.value;
            if (res.rs1 == 1) {
                jq('#studio_invJoinMessage').html(res.rs2);
                jq('#studio_invJoinContent').hide();
                jq('#studio_invJoinMessage').show();
                setTimeout('AuthManager.ShowInviteJoin1stState();', 3000);

            }
            else
                jq('#studio_invJoinInfo').html('<div class="errorBox">' + res.rs2 + '</div>');

        });
    };

    this.ShowInviteJoin1stState = function() {
        jq('#GreetingBlock .submenu .signUpBlock .join').removeClass('opened');
        jq('#studio_invJoinDialog').hide();
    }

    this.EditProfileCallback = function(makeResult) {
        if (makeResult.Action == 'edit_user') {
            if (makeResult.SelfProfile)
                window.location.reload(true);
        }
    };

    this.RemoveUser = function(userID, userName) {
        if (!confirm(this.ConfirmRemoveUser.format(userName)))
            return;

        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };

        UserProfileControl.RemoveUser(userID, function(result) {
            var res = result.value;
            if (res.rs1 == '1')
                jq('.mainContainerClass .containerBodyBlock').html('<div class="okBox">' + res.rs2 + '</div>');

            else
                jq('#studio_userProfileCardInfo').html('<div class="errorBox">' + res.rs2 + '</div>');
        });
    };

    this.DisableUser = function(userID, disabled) {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };

        UserProfileControl.DisableUser(userID, disabled, function(result) {
            var res = result.value;
            if (res.rs1 == '1')
                window.location.reload(true);
            else
                jq('#studio_userProfileCardInfo').html('<div class="errorBox">' + res.rs2 + '</div>');

        });
    };

    this.ResendActivation = function(userID) {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };

        UserProfileControl.ResendActivation(userID, function(result) {
            var res = result.value;
            if (res.rs1 == '1')
                window.location.reload(true);
            else
                jq('#studio_userProfileCardInfo').html('<div class="errorBox">' + res.rs2 + '</div>');

        });
    };

    this.CreateAdmin = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };

        Wizard.CreateAdmin(jq('#studio_adminFirstName').val(),
            jq('#studio_adminLastName').val(),
            jq('#studio_adminEmail').val(),
            jq('#studio_adminPwd').val(),
            function(result) {

                var res = result.value;
                if (res.rs1 == '1')
                    window.location.reload(true);
                else
                    jq('#studio_wizardMessage').html('<div class="errorBox">' + res.rs2 + '</div>');
            });
    };
};

var StudioManager = new function() {
    this.ModuleQuickShortcutsMenuState = 0;

    this.SidePanelElementID = 'studio_sidePanel';
    this.SidePanelButtonElementID = 'studio_sidePanelButton';


    this.OnSideChangeState = null;


    this.ShowGettingStartedVideo = function() {
        jq.blockUI({
            message: jq("#studio_GettingStartedVideoDialog"),
            css: {
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '680px',
                height: '500px',
                cursor: 'default',
                textAlign: 'left',
                backgroundColor: 'transparent',
                marginLeft: '-340px',
                top: '25%'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            fadeIn: 0,
            fadeOut: 0
        });
    };

    this.SaveGettingStartedState = function() {
        var state = jq('#studio_gettingStartedState').is(':checked');

        AjaxPro.onLoading = function(b) { };

        WebStudio.SaveGettingStartedState(state, function(result) { });
    };


    this.createCustomSelect = function(selects, hiddenBorder, AdditionalBottomOption) {
        if (typeof selects === 'string') {
            selects = document.getElementById(selects);
        }
        if (!selects || typeof selects !== 'object') {
            return undefined;
        }
        if (typeof selects.join !== 'function' && !(selects instanceof String)) {
            selects = [selects];
        }

        for (var i = 0, n = selects.length; i < n; i++) {
            var select = selects[i];
            var selectValue = select.value;


            if (select.className.indexOf('originalSelect') !== -1) {
                continue;
            }

            var container = document.createElement('div');
            container.setAttribute('value', selectValue);
            container.className = select.className + (select.className.length ? ' ' : '') + 'customSelect';
            var position = (document.defaultView && document.defaultView.getComputedStyle) ? document.defaultView.getComputedStyle(select, '').getPropertyValue('position') : (select.currentStyle ? select.currentStyle['position'] : 'static');
            container.style.position = position === 'static' ? 'relative' : position;

            var title = document.createElement('div');
            title.className = 'title' + ' ' + selectValue;
            title.style.height = '100%';
            title.style.position = 'relative';
            title.style.zIndex = '1';
            container.appendChild(title);

            var selector = document.createElement('div');
            selector.className = 'selector';
            selector.style.position = 'absolute';
            selector.style.zIndex = '1';
            selector.style.right = '0';
            selector.style.top = '0';
            selector.style.height = '100%';
            container.appendChild(selector);

            var optionsList = document.createElement('ul');
            optionsList.className = 'options';
            optionsList.style.display = 'none';
            optionsList.style.position = 'absolute';
            optionsList.style.zIndex = '777';
            optionsList.style.width = '115%';
            optionsList.style.maxHeight = '200px';
            optionsList.style.overflow = 'auto';
            container.appendChild(optionsList);

            var optionHtml = '',
                optionValue = '',
                optionTitle = '',
                optionClassName = '',
                fullClassName = '',
                option = null,
                options = select.getElementsByTagName('option');
            for (var j = 0, m = options.length; j < m; j++) {
                option = document.createElement('li');
                optionValue = options[j].value;
                optionHtml = options[j].innerHTML;
                optionTitle = optionHtml.replace(/&amp;/gi, "&");
                optionsList.appendChild(option);
                fullClassName = 'option' + ' ' + optionValue + ((optionClassName = options[j].className) ? ' ' + optionClassName : '');
                option.setAttribute('title', optionTitle);
                option.setAttribute('value', optionValue);
                if (selectValue === optionValue) {
                    title.innerHTML = optionHtml;
                    fullClassName += ' selected';
                }
                option.selected = selectValue === optionValue;
                option.innerHTML = optionHtml;
                option.className = fullClassName;
            }
            if (AdditionalBottomOption !== undefined && AdditionalBottomOption !== "")
                optionsList.appendChild(AdditionalBottomOption);

            select.parentNode.insertBefore(container, select);
            container.appendChild(select);

            select.className += (select.className.length ? ' ' : '') + 'originalSelect';
            select.style.display = 'none';

            if (hiddenBorder) {
                jq(container).addClass('comboBoxHiddenBorder');
            }


            jq(optionsList).bind('selectstart', function() {
                return false;
            }).mousedown(function() {
                return false;
            }).click(function(evt) {
                var $target = jq(evt.target);
                if ($target.hasClass('option')) {
                    var 
                        containerNewValue = evt.target.getAttribute('value'),
                        $container = $target.parents('div.customSelect:first'),
                        container = $container.get(0);
                    if (!container || container.getAttribute('value') === containerNewValue) {
                        return undefined;
                    }
                    container.setAttribute('value', containerNewValue);
                    $container.find('li.option').removeClass('selected').filter('li.' + containerNewValue + ':first').addClass('selected');
                    $container.find('div.title:first').html($target.html() || '&nbsp;').attr('className', 'title ' + containerNewValue);
                    $container.find('select.originalSelect:first').val(containerNewValue).change();
                }
            });
            if (jq.browser.msie && jq.browser.version < 7) {
                jq(optionsList).find('li.option').hover(

                    function() {
                        jq(this).addClass('hover');
                    }, function() {
                        jq(this).removeClass('hover');
                    });
            }

            jq(selector).add(title).bind('selectstart', function() {
                return false;
            }).mousedown(function() {
                return false;
            }).click(function(evt) {
                var $options = jq(this.parentNode).find('ul.options:first');
                if ($options.is(':hidden')) {
                    $options.css({
                        top: jq(this.parentNode).height() + 1 + 'px'
                    }).slideDown(1, function() {
                        jq(document).one('click', function() {
                            jq('div.customSelect ul.options').hide();
                            jq(container).removeClass('comboBoxHiddenBorderFocused');
                        });
                    });

                    if (hiddenBorder) {
                        if (!jq(container).hasClass('comboBoxHiddenBorderFocused')) {
                            container.className = 'comboBoxHiddenBorderFocused ' + container.className;
                        }
                        var leftOffset = jq(container).width() - jq(select).width() - 1;
                        //                        if (jQuery.browser.mozilla) { leftOffset -= 1;    }
                        $options.css({
                            'width': jq(select).width(),
                            'border-top': '1px solid #c7c7c7',
                            'left': leftOffset,
                            'top': jq(container).height()
                        });
                    }
                }
            });
        }
    };

    this.CloseAddContentDialog = function() {
        jq.unblockUI();
        return false;
    };

    this.ShowAddContentDialog = function() {
        jq.blockUI({
            message: jq("#studio_AddContentDialog"),
            css: {
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '400px',
                height: '350px',
                cursor: 'default',
                textAlign: 'left',
                backgroundColor: 'transparent',
                marginLeft: '-200px',
                top: '25%'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            fadeIn: 0,
            fadeOut: 0
        });
    };

    this.ToggleSidePanel = function() {
        var isVisible = jq('#' + this.SidePanelElementID).is(':visible');

        if (this.OnSideChangeState != null)
            this.OnSideChangeState(!isVisible);

        AjaxPro.onLoading = function(b) { };
        if (isVisible) {
            WebStudio.SaveSidePanelState(false, function(res) { });
            jq('#' + this.SidePanelElementID).hide('fast', function() {
                jq('#' + StudioManager.SidePanelButtonElementID).attr('src', SkinManager.GetImage('side_expand.png'));
            });
        }
        else {
            WebStudio.SaveSidePanelState(true, function(res) { });
            jq('#' + this.SidePanelElementID).show();
            jq('#' + this.SidePanelButtonElementID).attr('src', SkinManager.GetImage('side_collapse.png'));
        }
    };

    this.ToggleProductActivity = function(productID) {
        if (jq('#studio_product_activity_' + productID).is(':visible')) {
            jq('#studio_activityProductState_' + productID).attr('src', SkinManager.GetImage('collapse_right_dark.png'));
            jq('#studio_product_activity_' + productID).hide();
        }
        else {
            jq('#studio_activityProductState_' + productID).attr('src', SkinManager.GetImage('collapse_down_dark.png'));
            jq('#studio_product_activity_' + productID).show();
        }
    };

    this.Disable = function(obj_id) {
        jq('#' + obj_id).addClass("disableLinkButton");
        jq('#' + obj_id).removeClass("baseLinkButton");
    };

    this.Enable = function(obj_id) {
        jq('#' + obj_id).addClass("baseLinkButton");
        jq('#' + obj_id).removeClass("disableLinkButton");
    };
};

//------------fck uploads for comments-------------------
var FCKCommentsController = new function() {
    this.Callback = null;
    this.EditCommentHandler = function(commentID, text, domain, isEdit, callback) {
        this.Callback = callback;
        if (text == null || text == undefined)
            text = "";

        CommonControlsConfigurer.EditCommentComplete(commentID, domain, text, isEdit, this.CallbackHandler);
    };

    this.CancelCommentHandler = function(commentID, domain, isEdit, callback) {
        this.Callback = callback;
        CommonControlsConfigurer.CancelCommentComplete(commentID, domain, isEdit, this.CallbackHandler);
    };

    this.RemoveCommentHandler = function(commentID, domain, callback) {
        this.Callback = callback;
        CommonControlsConfigurer.RemoveCommentComplete(commentID, domain, this.CallbackHandler);
    };

    this.CallbackHandler = function(result) {
        if (FCKCommentsController.Callback != null && FCKCommentsController.Callback != '') ;
        eval(FCKCommentsController.Callback + '()');
    };
};

//cancel controller
var CancelButtonController = new function() {

    this.ConfirmCancellationWithPrevValue = function(fckEditorID, prevValue) {
        try {
            if (!this.IsEmptyFckEditorTextFieldWithPrevValue(fckEditorID, prevValue)) {
                return window.confirm(CommonJavascriptResources.CancelConfirmMessage);
            }
        } catch(err) { }
        return true;
    };

    this.IsEmptyFckEditorTextFieldWithPrevValue = function(fckEditorID, prevValue) {
        try {
            var curValue = FCKeditorAPI.GetInstance(fckEditorID).GetXHTML();
            return this.IsEmptyTextField(curValue) || (prevValue && curValue == prevValue);
        } catch(err) { }
        return true;
    };

    this.IsEmptyTextField = function(text) {
        if (text.replace( /<\/?\s*(br|p|div)[\s\S]*?>|&nbsp;?|\s+/gi , '') == '') {
            return true;
        }
        return false;
    };

    this.IsEmptyFckEditorTextField = function(fckEditorID) {
        try {
            return this.IsEmptyTextField(FCKeditorAPI.GetInstance(fckEditorID).GetXHTML());
        } catch(err) { }
        return true;
    };
};

String.prototype.format = function() {
    var txt = this,
        i = arguments.length;

    while (i--) {
        txt = txt.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return txt;
};

var EventTracker = new function() {
    this.Track = function(event) {

        try {
            var pageTracker = _gat._getTracker("UA-12442749-4");

            if (pageTracker != null)
                pageTracker._trackPageview(event);

        } catch(err) { }
    };
};
/*--------Error messages for required field-------*/
function ShowRequiredError(item, withouthScroll) {
    jq("div[class='infoPanel alert']").hide();
    jq("div[class='infoPanel alert']").empty();
    var parentBlock = jq(item).parents(".requiredField");
    jq(parentBlock).addClass("requiredFieldError");

    if(typeof(withouthScroll)=="undefined" || withouthScroll == false)
        jq.scrollTo(jq(parentBlock).position().top - 50, { speed: 500 });
        
    jq(item).focus();
};

function HideRequiredError() {
    jq(".requiredField").removeClass("requiredFieldError");
};

function RemoveRequiredErrorClass(item) {
    var parentBlock = jq(item).parents(".requiredField");
    jq(parentBlock).removeClass("requiredFieldError");
};

function AddRequiredErrorText(item, text) {
    var parent = jq(item).parents(".requiredField");
    var errorSpan = jq(parent).children(".requiredErrorText");
    jq(errorSpan).text(text);
};


var EmailOperationManager = new function() {

    this.SendEmailActivationInstructionsOnChange = function(oldEmail, newEmail, newEmailConfirm, queryString) {
        jq("[id$='studio_confirmMessage']").hide();
        jq("[id$='studio_confirmMessage'] div").hide();
        jq("#emailInputContainer").hide();//hide email on send
        EmailOperationService.SendEmailActivationInstructionsOnChange(oldEmail, newEmail, newEmailConfirm, queryString, this._SendEmailActivationInstructionsOnChangeResponse);
    };

    this._SendEmailActivationInstructionsOnChangeResponse = function(response) {
        jq("[id$='studio_confirmMessage']").show();

        if (response.error != null) {
            jq("#studio_confirmMessage_errortext").text(response.error.Message).show();
            return;
        }
        if (response.value.status == "success") {
            jq("#studio_confirmMessage_successtext").html(response.value.message).show();
            jq("#emailInputContainer").hide();
            jq("#currentEmailText").hide();
        }
        if (response.value.status == "error") {
            jq("#studio_confirmMessage_errortext").text(response.value.message).show();
            jq("#emailInputContainer").show();
        }
        if (response.value.status == "fatalerror") {
            jq("#studio_confirmMessage_errortext").text(response.value.message).show();
            jq("#emailInputContainer").hide();
        }
    };

    this.SendEmailActivationInstructions = function(userEmail, userID) {
        EmailOperationService.SendEmailActivationInstructions(userID, userEmail, function(response) { EmailOperationManager._EmailOperationInstructionsServerResponse(response); });
    };

    this.SendEmailChangeInstructions = function(userEmail, userID) {
        EmailOperationService.SendEmailChangeInstructions(userID, userEmail, function(response) { EmailOperationManager._EmailOperationInstructionsServerResponse(response); });
    };

    this._EmailOperationInstructionsServerResponse = function(response) {
        if (response.error != null) {
            jq("#divEmailOperationError").html(response.error.Message).show();
        }
        else {
            jq("#divEmailOperationError").html("").hide();
            jq("#studio_emailOperationContent").hide();
            jq("#studio_emailOperationResult").show();
            jq("#studio_emailOperationResultText").html(response.value);
            document.location.reload(true);
        }
    };

    this.ShowEmailChangeWindow = function(userEmail, userID, adminMode) {
        jq("#divEmailOperationError").html("").hide();
        jq("#studio_emailOperationResult").hide();

        if (adminMode == true) {
            jq("#emailInputContainer").show();
            jq("#emailMessageContainer").hide();
        }
        else {
            jq("#emailInputContainer").hide();
            jq("#emailMessageContainer").show();

            jq("#resendInviteText").hide();
            jq("#emailActivationText").hide();
            jq("#emailChangeText").show();

            jq("#emailMessageContainer [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }

        jq("#resendInviteDialogPopupHeader").hide();
        jq("#emailActivationDialogPopupHeader").hide();
        jq("#emailChangeDialogPopupHeader").show();

        jq("#studio_emailOperationContent").show();

        jq("#emailChangeDialogText").show();
        jq("#resendInviteDialogText").hide();
        jq("#emailActivationDialogText").hide();

        jq("#emailOperation_email").val(userEmail);

        this.OpenPopupDialog();

        jq("#btEmailOperationSend").unbind("click");

        jq("#btEmailOperationSend").click(function() {
            var newEmail = jq("#emailOperation_email").val();
            EmailOperationManager.SendEmailChangeInstructions(newEmail, userID);
            return false;
        });
    };

    this.ShowEmailActivationWindow = function(userEmail, userID, adminMode) {
        jq("#divEmailOperationError").html("").hide();
        jq("#studio_emailOperationResult").hide();

        if (adminMode == true) {
            jq("#emailInputContainer").show();
            jq("#emailMessageContainer").hide();
        }
        else {
            jq("#emailInputContainer").hide();
            jq("#emailMessageContainer").show();

            jq("#resendInviteText").hide();
            jq("#emailActivationText").show();
            jq("#emailChangeText").hide();

            jq("#emailMessageContainer [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }

        jq("#emailActivationDialogPopupHeader").show();
        jq("#emailChangeDialogPopupHeader").hide();
        jq("#resendInviteDialogPopupHeader").hide();

        jq("#studio_emailOperationContent").show();

        jq("#emailChangeDialogText").hide();
        jq("#resendInviteDialogText").hide();
        jq("#emailActivationDialogText").show();

        jq("#emailOperation_email").val(userEmail);

        this.OpenPopupDialog();

        jq("#btEmailOperationSend").unbind("click");

        jq("#btEmailOperationSend").click(function() {
            var newEmail = jq("#emailOperation_email").val();
            EmailOperationManager.SendEmailActivationInstructions(newEmail, userID);
            return false;
        });

    };

    this.ShowResendInviteWindow = function(userEmail, userID, adminMode) {
        jq("#divEmailOperationError").html("").hide();
        jq("#studio_emailOperationResult").hide();

        if (adminMode == true) {
            jq("#emailInputContainer").show();
            jq("#emailMessageContainer").hide();
        }
        else {
            jq("#emailInputContainer").hide();
            jq("#emailMessageContainer").show();

            jq("#emailActivationText").hide();
            jq("#emailChangeText").hide();
            jq("#resendInviteText").show();

            jq("#emailMessageContainer [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }

        jq("#emailActivationDialogPopupHeader").hide();
        jq("#emailChangeDialogPopupHeader").hide();
        jq("#resendInviteDialogPopupHeader").show();

        jq("#studio_emailOperationContent").show();

        jq("#emailChangeDialogText").hide();
        jq("#emailActivationDialogText").hide();
        jq("#resendInviteDialogText").show();

        jq("#emailOperation_email").val(userEmail);

        this.OpenPopupDialog();

        jq("#btEmailOperationSend").unbind("click");

        jq("#btEmailOperationSend").click(function() {
            var newEmail = jq("#emailOperation_email").val();
            EmailOperationManager.SendEmailActivationInstructions(newEmail, userID);
            return false;
        });

    };

    this.OpenPopupDialog = function() {

        try {

            jq.blockUI({ message: jq("#studio_emailChangeDialog"),
                css: {
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: '400px',
                    height: '300px',
                    cursor: 'default',
                    textAlign: 'left',
                    'background-color': 'Transparent',
                    'margin-left': '-200px',
                    'top': '25%'
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
        catch (e) { };

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = "";
    };

    this.CloseEmailOperationWindow = function() {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };
};


var ProfileManager = new function() {
    this.SendInstrunctionsToRemoveProfile = function() {
        UserProfileControl.SendInstructionsToDelete(function(result) { ProfileManager.ShowResultStatus(result.value); });
    };

    this.ShowResultStatus = function(result) {
        var res = '<div class="operationInfoBox">' + result.Message + '</div>';
        jq('#remove_content').html(res);
        jq('#studio_deleteProfileDialog .buttons').html('');

        setTimeout("jq.unblockUI();", 2000);
    };

    this.ShowDeleteUserWindow = function() {

        this.OpenPopUpDialog('studio_deleteProfileDialog');
    };

    this.OpenPopUpDialog = function(dialog) {

        try {

            jq.blockUI({ message: jq("#" + dialog),
                css: {
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: '400px',
                    height: '300px',
                    cursor: 'default',
                    textAlign: 'left',
                    'background-color': 'Transparent',
                    'margin-left': '-200px',
                    'top': '25%'
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
        catch (e) { };
    };
};

LoadingBanner = function() {
    var animateDelay = 2000;
    var displayDelay = 500;
    var displayOpacity = 0.8;
    var loaderCss = "";
    var loaderId = "loadingBanner";
    var strLoading = "Loading...";
    var strDescription = "Please wait...";
    return {
        animateDelay: animateDelay,
        displayDelay: displayDelay,
        displayOpacity: displayOpacity,
        loaderCss: loaderCss,
        loaderId: loaderId,
        strLoading: strLoading,
        strDescription: strDescription,

        displayLoading: function(withoutDelay) {
            var id = "#" + LoadingBanner.loaderId;

            if (jq(id).length != 0) return;

            var innerHtml = '<div id="{0}" class="loadingBanner {1}">{2}<div>{3}</div></div>'
                .format(LoadingBanner.loaderId, LoadingBanner.loaderCss, LoadingBanner.strLoading, LoadingBanner.strDescription);

            jq("body").append(innerHtml).addClass("loading");

            if (jq.browser.mobile) jq(id).css("top", jq(window).scrollTop() + 150 + "px");

            jq(id).animate({ opacity: 0 }, withoutDelay === true ? 0 : LoadingBanner.displayDelay, function() { jq(id).animate({ opacity: LoadingBanner.displayOpacity }, LoadingBanner.animateDelay); });
        },

        hideLoading: function() {
            jq("#" + LoadingBanner.loaderId).remove();
            jq("body").removeClass("loading");
        }
    };
} ();

(function() {
    var
      dropdownToggleHash = {};

    jQuery.extend({
        dropdownToggle: function(options) {
            // default options
            options = jQuery.extend({
                //switcherSelector: "#id" or ".class",          - button
                //dropdownID: "id",                             - drop panel
                //anchorSelector: "#id" or ".class",            - near field
                //noActiveSwitcherSelector: "#id" or ".class",  - dont hide
                addTop: 0,
                addLeft: 0,
                position: "absolute",
                fixWinSize: true,
                enableAutoHide: true,
                showFunction: null,
                hideFunction: null,
                alwaysUp: false
            }, options);

            var _toggle = function(switcherObj, dropdownID, addTop, addLeft, fixWinSize, position, anchorSelector, showFunction, alwaysUp) {
                fixWinSize = fixWinSize === true;
                addTop = addTop || 0;
                addLeft = addLeft || 0;
                position = position || "absolute";

                var targetPos = jq(anchorSelector || switcherObj).offset();
                var dropdownItem = jq("#" + dropdownID);

                var elemPosLeft = targetPos.left;
                var elemPosTop = targetPos.top + jq(anchorSelector || switcherObj).outerHeight();

                var w = jq(window);
                var topPadding = w.scrollTop();
                var leftPadding = w.scrollLeft();

                if (position == "fixed") {
                    addTop -= topPadding;
                    addLeft -= leftPadding;
                }

                var scrWidth = w.width();
                var scrHeight = w.height();

                if (fixWinSize
                    && (targetPos.left + addLeft + dropdownItem.outerWidth()) > (leftPadding + scrWidth))
                    elemPosLeft = Math.max(0, leftPadding + scrWidth - dropdownItem.outerWidth()) - addLeft;

                if (fixWinSize
                    && (elemPosTop + dropdownItem.outerHeight()) > (topPadding + scrHeight)
                        && (targetPos.top - dropdownItem.outerHeight()) > topPadding
                         || alwaysUp)
                    elemPosTop = targetPos.top - dropdownItem.outerHeight();

                dropdownItem.css(
                    {
                        "position": position,
                        "top": elemPosTop + addTop,
                        "left": elemPosLeft + addLeft
                    });
                if (typeof showFunction === "function")
                    showFunction(switcherObj, dropdownItem);

                dropdownItem.toggle();
            };

            var _registerAutoHide = function(event, switcherSelector, dropdownSelector, hideFunction) {
                if (jq(dropdownSelector).is(":visible")) {
                    var $targetElement = jq((event.target) ? event.target : event.srcElement);
                    if (!$targetElement.parents().andSelf().is(switcherSelector + ", " + dropdownSelector)) {
                        if (typeof hideFunction === "function")
                            hideFunction($targetElement);
                        jq(dropdownSelector).hide();
                    }
                }
            };

            if (options.switcherSelector && options.dropdownID) {
                var toggleFunc = function(e) {
                    _toggle(jq(this), options.dropdownID, options.addTop, options.addLeft, options.fixWinSize, options.position, options.anchorSelector, options.showFunction, options.alwaysUp);
                };
                if (!dropdownToggleHash.hasOwnProperty(options.switcherSelector + options.dropdownID)) {
                    jq(options.switcherSelector).live("click", toggleFunc);
                    dropdownToggleHash[options.switcherSelector + options.dropdownID] = true;
                }
            }

            if (options.enableAutoHide && options.dropdownID) {
                var hideFunc = function(e) {
                    var allSwitcherSelectors = options.noActiveSwitcherSelector ?
                        options.switcherSelector + ", " + options.noActiveSwitcherSelector : options.switcherSelector;
                    _registerAutoHide(e, allSwitcherSelectors, "#" + options.dropdownID, options.hideFunction);

                };
                jq(document).unbind("click", hideFunc);
                jq(document).bind("click", hideFunc);
            }

            return {
                toggle: _toggle,
                registerAutoHide: _registerAutoHide
            };
        }
    });
})();

var SearchManager = new function() {
    this._containerId = null;
    this._target = null;
    this._count = null;
    this.Label = '';
    this.ShowAll = function(obj, control, product, item, count) {
        jq(obj).html('<img src=' + SkinManager.GetImage('loader_12.gif') + ' />');
        this._containerId = control;
        this._target = item;
        this._count = count;
        SearchController.GetAllData(product, jq('#studio_search').val(), function(result) { SearchManager.ShowAllCallback(result.value); });
    }

    this.ShowAllCallback = function(result) {
        jq('#oper_' + SearchManager._target).html(SearchManager.Label + ': ' + SearchManager._count);
        jq('#' + SearchManager._containerId).html(result);
    }
}

Encoder = { EncodeType: "entity", isEmpty: function(val) { if (val) { return ((val === null) || val.length == 0 || /^\s+$/.test(val)); } else { return true; } }, HTML2Numerical: function(s) { var arr1 = new Array('&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&Auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&Ouml;', '&times;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&Uuml;', '&yacute;', '&thorn;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&Oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&oelig;', '&oelig;', '&scaron;', '&scaron;', '&yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'); var arr2 = new Array('&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'); return this.swapArrayVals(s, arr1, arr2); }, NumericalToHTML: function(s) { var arr1 = new Array('&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'); var arr2 = new Array('&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&Auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&Ouml;', '&times;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&Uuml;', '&yacute;', '&thorn;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&Oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&oelig;', '&oelig;', '&scaron;', '&scaron;', '&yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'); return this.swapArrayVals(s, arr1, arr2); }, numEncode: function(s) { if (this.isEmpty(s)) return ""; var e = ""; for (var i = 0; i < s.length; i++) { var c = s.charAt(i); if (c < " " || c > "~") { c = "&#" + c.charCodeAt() + ";"; } e += c; } return e; }, htmlDecode: function(s) { var c, m, d = s; if (this.isEmpty(d)) return ""; d = this.HTML2Numerical(d); arr = d.match(/&#[0-9]{1,5};/g); if (arr != null) { for (var x = 0; x < arr.length; x++) { m = arr[x]; c = m.substring(2, m.length - 1); if (c >= -32768 && c <= 65535) { d = d.replace(m, String.fromCharCode(c)); } else { d = d.replace(m, ""); } } } return d; }, htmlEncode: function(s, dbl) { if (this.isEmpty(s)) return ""; dbl = dbl | false; if (dbl) { if (this.EncodeType == "numerical") { s = s.replace(/&/g, "&#38;"); } else { s = s.replace(/&/g, "&amp;"); } } s = this.XSSEncode(s, false); if (this.EncodeType == "numerical" || !dbl) { s = this.HTML2Numerical(s); } s = this.numEncode(s); if (!dbl) { s = s.replace(/&#/g, "##AMPHASH##"); if (this.EncodeType == "numerical") { s = s.replace(/&/g, "&#38;"); } else { s = s.replace(/&/g, "&amp;"); } s = s.replace(/##AMPHASH##/g, "&#"); } s = s.replace(/&#\d*([^\d;]|$)/g, "$1"); if (!dbl) { s = this.correctEncoding(s); } if (this.EncodeType == "entity") { s = this.NumericalToHTML(s); } return s; }, XSSEncode: function(s, en) { if (!this.isEmpty(s)) { en = en || true; if (en) { s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&quot;"); s = s.replace(/</g, "&lt;"); s = s.replace(/>/g, "&gt;"); } else { s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&#34;"); s = s.replace(/</g, "&#60;"); s = s.replace(/>/g, "&#62;"); } return s; } else { return ""; } }, hasEncoded: function(s) { if (/&#[0-9]{1,5};/g.test(s)) { return true; } else if (/&[A-Z]{2,6};/gi.test(s)) { return true; } else { return false; } }, stripUnicode: function(s) { return s.replace(/[^\x20-\x7E]/g, ""); }, correctEncoding: function(s) { return s.replace(/(&amp;)(amp;)+/, "$1"); }, swapArrayVals: function(s, arr1, arr2) { if (this.isEmpty(s)) return ""; var re; if (arr1 && arr2) { if (arr1.length == arr2.length) { for (var x = 0, i = arr1.length; x < i; x++) { re = new RegExp(arr1[x], 'g'); s = s.replace(re, arr2[x]); } } } return s; }, inArray: function(item, arr) { for (var i = 0, x = arr.length; i < x; i++) { if (arr[i] === item) { return i; } } return -1; } }