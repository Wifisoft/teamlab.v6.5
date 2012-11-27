jq(function() {
    jq('#editAdminBtn').click(AdminSettingsManager.ShowAdminSelectorDialog);
    jq('#changeOwnerBtn').click(AdminSettingsManager.ChangeOwner);

    var header = jq('.mainContainerClass>.containerHeaderBlock table td div').last();
    header.css('float', 'left');
    header.after('<div class="HelpCenterSwitcher title big" id="QuestionForHelpAdministrators" title="' + jq("#questionForHelp").text() + '"></div> ');

    jq('#QuestionForHelpAdministrators').click(function(){jq(this).helper({ BlockHelperID: 'AnswerForHelpAdministrators'});});
      
})

AdminSettingsManager = new function() {

    this.TimeoutHandler = null;

    this.ShowAdminSelectorDialog = function() {
        adminSelector.OnOkButtonClick = AdminSettingsManager.SaveAdminList;
        adminSelector.ShowDialog();
    };

    this.ChangeOwner = function() {
        if (this.TimeoutHandler)
            clearTimeout(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_adminSettingsBox').block();
            else
                jq('#studio_adminSettingsBox').unblock();
        };

        var ownerId = jq('select[id$="_ownerSelector"]').val();       

        AdminSettingsController.ChangeOwner(ownerId, function(result) {
            var res = result.value;
            if (res.Status == 1)
                jq('#studio_adminSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_adminSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');

            AdminSettingsManager.TimeoutHandler = setTimeout(function() { jq('#studio_adminSettingsInfo').html(''); }, 4000);

        });
    }

    this.SaveAdminList = function() {

        if (this.TimeoutHandler)
            clearTimeout(this.TimeoutHandler);

        var userIDs = new Array();
        jq(adminSelector.GetSelectedUsers()).each(function(i, el) {
            userIDs.push(el.ID);
        });

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_adminSettingsBox').block();
            else
                jq('#studio_adminSettingsBox').unblock();
        };

        AdminSettingsController.SaveAdminList(userIDs, function(result) {
            var res = result.value;
            if (res.Status == 1) {
                jq('#studio_adminsListBox').html(res.Message);
            }
            else {
                jq('#studio_adminSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
                AdminSettingsManager.TimeoutHandler = setTimeout(function() { jq('#studio_adminSettingsInfo').html(''); }, 4000);
            }
        });
    };

}