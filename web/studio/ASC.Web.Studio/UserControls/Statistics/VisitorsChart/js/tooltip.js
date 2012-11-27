
(function () {
  if (typeof window.ASC === 'undefined') {
    window.ASC = {};
  }
  if (typeof window.ASC.Common === 'undefined') {
    window.ASC.Common = {};
  }

  window.ASC.Common.toolTip = (function () {
    var
      wrapperId = '',
      wrapperClassName = 'tooltip-wrapper',
      wrapperHandler = null;

    var uniqueId = function (prefix) {
      return (typeof prefix != 'undefined' ? prefix + '-' : '') + Math.floor(Math.random() * 1000000);
    };

    var create = function () {
      if (!wrapperHandler) {
        wrapperId = uniqueId('tooltipWrapper');
        wrapperHandler = document.createElement('div');
        wrapperHandler.id = wrapperId;
        wrapperHandler.className = wrapperClassName;
        wrapperHandler.style.display = 'none';
        wrapperHandler.style.left = '0';
        wrapperHandler.style.top = '0';
        wrapperHandler.style.position = 'absolute';
        document.body.appendChild(wrapperHandler);
      }
      return wrapperHandler;
    };

    var show = function (content, handler) {
      create();
      wrapperHandler.innerHTML = content;
      wrapperHandler.style.display = 'block';

      if (typeof handler === 'function') {
        handler.call(wrapperHandler);
      }
    };

    var hide = function () {
      if (wrapperHandler && typeof wrapperHandler === 'object') {
        wrapperHandler.style.display = 'none';
      }
    };

    return {
      show  : show,
      hide  : hide
    }
  })();
})();
