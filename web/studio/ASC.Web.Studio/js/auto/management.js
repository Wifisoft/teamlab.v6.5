
var StudioManagement = new function() {

    //--------------departments management---------------------------------------
    this.AddDepartmentOpenDialog = function() {
        try {
            HideRequiredError();
            jq.blockUI({ message: jq("#studio_departmentAddDialog"),
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

            //jq('#addDepartment_info').html('');
            jq('#' + jq('#addDepartment_infoID').val()).html("<div></div>");
            jq('#' + jq('#addDepartment_infoID').val()).hide();

            PopupKeyUpActionProvider.ClearActions();
            PopupKeyUpActionProvider.EnterAction = 'StudioManagement.AddDepartmentCallback()';
        }
        catch (e) { };
    };

    this.AddDepartmentCallback = function() {
        var departmentName = jq("#studio_newDepName");
        if (jq.trim(jq(departmentName).val()) == "") {
            ShowRequiredError(departmentName);
            return;
        }
        AjaxPro.onLoading = function(b) {
            if (b) {
                //jq('#addDepartment_info').block();
                jq('#dep_panel_buttons').hide();
                jq('#dep_action_loader').show();
            }
            else {
                jq('#dep_action_loader').hide();
                jq('#dep_panel_buttons').show();
                //jq('#addDepartment_info').unblock();
            }
        };

        EmployeeService.AddDepartment(
            jq('[id$="_depProductID"]').val(),
            jq("#studio_newDepName").val(),
            jq("#studio_addep_user_selector").val(),
            function(result) {
                if (result.value.rs1 == '0') {
                    jq('#' + jq('#addDepartment_infoID').val()).html("<div>" + result.value.rs2 + "</div>");
                    jq('#' + jq('#addDepartment_infoID').val()).show();
                }
                else
                    window.open(result.value.rs3, '_self');
            }
       );
    };

    this.CloseAddDepartmentDialog = function() {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };

    this.ShowEmployeeSelectorDialog = function() {
        empSelector.OnOkButtonClick = this.SaveEmployee2Department;
        empSelector.ShowDialog();
    };

    this.SaveEmployee2Department = function() {
        var userIDs = new Array();
        jq(empSelector.GetSelectedUsers()).each(function(i, el) {

            userIDs.push(el.ID);
        });

        AjaxPro.onLoading = function(b) {
            if (b)
                jq.blockUI();
            else
                jq.unblockUI();
        };

        EmployeeService.TransferUser2Department(jq('#studio_depEditDepID').val(),
            userIDs, function(result) {
                var res = result.value;
                if (res.rs1 == 1) {
                    window.location.reload(true);
                }
                else {
                    jq('#studio_depEditAddUserInfo').html("<div class='errorBox'>" + res.rs2 + '</div>');
                    setTimeout("jq('#studio_depEditAddUserInfo').html('');", 5000);
                }
            });
    };

    //----studio settings-----------------------------------
    this.TimeoutHandler = null;

    this.SaveDnsSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_enterDnsBox').block();
            else
                jq('#studio_enterDnsBox').unblock();
        };

        StudioSettings.SaveDnsSettings(jq('#studio_dnsName').val(), jq('#studio_TenantAlias').val(), jq('#studio_enableDnsName').is(':checked'),
            function(result) {
                jq('#dnsChange_sent').html(result.value.rs2);
                jq('#dnsChange_sent').show();
            }
		);
    };

    this.SaveEnterSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_enterSettingsBox').block();
            else
                jq('#studio_enterSettingsBox').unblock();
        };

        StudioSettings.SaveEnterSettings(jq("#studio_demoEnterState").is(":checked"), function(result) {
            jq('div[id^="studio_setInf"]').html('');
            jq('#studio_setInfEnterSettingsInfo').html(result.value.rs2);
            StudioManagement.TimeoutHandler = setTimeout("jq('#studio_setInfEnterSettingsInfo').html('');", 3000);
        });
    };

    this.SaveStudioViewSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_studioViewSettingsBox').block();
            else
                jq('#studio_studioViewSettingsBox').unblock();
        };

        StudioSettings.SaveStudioViewSettings(jq("#studio_sidePanelLeftViewType").is(":checked"), function(result) {
            jq('div[id^="studio_setInf"]').html('');
            jq('#studio_setInfStudioViewSettingsInfo').html(result.value.rs2);
            StudioManagement.TimeoutHandler = setTimeout("jq('#studio_setInfStudioViewSettingsInfo').html('');", 3000);
        });
    };

    

    this.SaveLanguageTimeSettings = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_lngTimeSettingsBox').block();
            else
                jq('#studio_lngTimeSettingsBox').unblock();
        };
        var timeManager = new TimeAndLanguageContentManager();
        timeManager.SaveTimeLangSettings(function(res) {
            if (res.Status == '2') {
                jq('#studio_lngTimeSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
                StudioManagement.TimeoutHandler = setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);

            }
            else if (res.Status == '1') {
                window.location.reload(true);
            }
            else {
                jq('#studio_lngTimeSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
                StudioManagement.TimeoutHandler = setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);
            }
        });
    };
};

jq(function() {

    var domainWidth = jq('#studio_TenantBaseDomain').width();
    if (domainWidth > 0) {
        domainWidth += 2;
    }
    if (domainWidth >= 200) {
        domainWidth = 0;
    }
    jq('#studio_TenantAlias').width(jq('#studio_dnsName').width() - domainWidth);


    if (jq('#dnsSettings').hasClass('disable')) {
        jq('#HelpQuetionDNSSettings,#HelpQuestionDataBackup').unbind('click').attr('title','');
        jq('#dnsSettings input').attr('disabled', 'disabled').css('cursor', 'default');        
    }
    else {
        jq('#HelpQuetionDNSSettings').bind('click', function() {
            jq(this).helper({ BlockHelperID: 'HelpAnswerDNSSettings' });
        });
    }

});
