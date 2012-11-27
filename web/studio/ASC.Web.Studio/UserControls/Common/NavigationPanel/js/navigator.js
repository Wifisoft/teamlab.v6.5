(function ($) {
  $(document).ready(function () {
    if (typeof window.StudioManager !== 'object') {
      window.StudioManager = {};
    }
    if (window.StudioManager.ToggleNavigationPanel !== 'function') {
      window.StudioManager.ToggleNavigationPanel = function (self) {
        var
          cookieName = 'asc_minimized_np';
        if ($(self).parents('div.navigationPanel:first').toggleClass('minimized').hasClass('minimized')) {
          var
            expiresAt = new Date();
          expiresAt.setFullYear(expiresAt.getFullYear() + 1);
          $.cookies.set(cookieName, '1', {expiresAt : expiresAt});
        } else {
          $.cookies.del(cookieName);
        }
      };
    }
  });
})(jq);
