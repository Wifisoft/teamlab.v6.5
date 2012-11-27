
if (!window.userProfileControl) {
  window.userProfileControl = {
    openContact : function (name) {
      var tcExist = false;
      try {
        tcExist = !!ASC.Controls.JabberClient;
      } catch (err) {
        tcExist = false;
      }
      if (tcExist === true) {
        try {ASC.Controls.JabberClient.open(name)} catch (err) {}
      }
    }
  };
}

jq(function () {
  var tcExist = false;
  try {
    tcExist = !!ASC.Controls.JabberClient;
  } catch (err) {
    tcExist = false;
  }
  if (tcExist === true) {
    jq('div.userProfileCard:first ul.info:first li.contact span')
      .addClass('link')
      .click(function () {
        var username = jq(this).parents('li.contact:first').attr('data-username');
        if (!username) {
          var
            search = location.search,
            arr = null,
            ind = 0,
            b = null;
          if (search.charAt(0) === '?') {
            search = search.substring(1);
          }
          arr = search.split('&');
          ind = arr.length;
          while (ind--) {
            b = arr[ind].split('=');
            if (b[0].toLowerCase() !== 'user') {
              continue;
            }
            username = b[1];
            
            break;
          }
        }
        if (typeof username === 'string') {
          userProfileControl.openContact(username);
        }
      });
  }
  
});

var UserMobilePhoneManager = new function() {
    this._erase = false;
    this._eraseText = "";
    this._createText = "";
    this.uid = null;

    this.OpenDialogCreatePhone = function() {
        UserMobilePhoneManager._erase = false;
        jq('#phoneChangeInst').on("click", function() { UserMobilePhoneManager.NotifyToChange(); });
        jq("#studio_mobilePhoneOperationContent #desc").html(UserMobilePhoneManager._createText);
        UserMobilePhoneManager.OpenDialog();
    }
    this.OpenDialogErasePhone = function() {
        UserMobilePhoneManager._erase = true;
        jq('#phoneChangeInst').on("click", function() { UserMobilePhoneManager.NotifyToErase(); });
        jq("#studio_mobilePhoneOperationContent #desc").html(UserMobilePhoneManager._eraseText);
        UserMobilePhoneManager.OpenDialog();
    }
    this.OpenDialog = function() {
        jq('#studio_mobilePhoneOperationContent').show();
        jq('#studio_mobilePhoneOperationProgress').hide();
        jq('#studio_mobilePhoneOperationContentResult').hide();
        try {
            jq.blockUI({ message: jq("#studio_mobilePhoneChangeDialog"),
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
    }
    this.NotifyToChange = function() {
        jq('#studio_mobilePhoneOperationContent').hide();
        jq('#studio_mobilePhoneOperationProgress').show();
        UserProfileControl.SendNotificationToChange(function(result) { if (result.value != null) UserMobilePhoneManager.NotifyCallback(result.value); });
    }

    this.NotifyToErase = function() {
        jq('#studio_mobilePhoneOperationContent').hide();
        jq('#studio_mobilePhoneOperationProgress').show();
        UserProfileControl.SendNotificationToErase(UserMobilePhoneManager.uid, function(result) { if (result.value != null) UserMobilePhoneManager.NotifyCallback(result.value); });
    }

    this.NotifyCallback = function(result) {
        jq('#studio_mobilePhoneOperationProgress').hide();
        jq('#studio_mobilePhoneOperationContentResult').show();
        setTimeout(function() { jq.unblockUI(); if (result.Status == 2) window.location.reload(true); }, 3000);
    }
};