/*******************************************************************************
    Auth for Import
*******************************************************************************/

var OAuthCallback = function(token, source) {
};

var OAuthError = function(error, source) {
    ASC.Files.UI.displayInfoPanel(error, true);
};

var OAuthPopup = function(url, width, height) {
    var newwindow;
    try {
        var params = "height=" + (height || 600) + ",width=" + (width || 1020) + ",resizable=0,status=0,toolbar=0,menubar=0,location=1";
        newwindow = window.open(url, "Authorization", params);
    } catch(err) {
        newwindow = window.open(url, "Authorization");
    }
    return newwindow;
};