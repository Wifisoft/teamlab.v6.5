
window.History = (function () {
  var
    domIsReady = false,
    currentAnchor = '',
    watcherInterval = 200,
    customEvents = {},
    supportedCustomEvents = {
      'onupdate' : true
    },
    browse = {
      ua      : window.navigator.userAgent,
      version : (window.navigator.userAgent.match(/.+(?:rv|it|ra|ie|ox|me|on)[\/:\s]([\d.]+)/i)||[0,'0'])[1],
      msie      : '\v' == 'v',
      opera   : window.opera ? true : false,
      chrome  : window.chrome ? true : false,
      safari  : /safari/i.test(window.navigator.userAgent),
      firefox : /firefox/i.test(window.navigator.userAgent),
      mozilla : /mozilla/i.test(window.navigator.userAgent) && !/(compatible|webkit)/.test(window.navigator.userAgent),
      gecko  : /gecko/i.test(window.navigator.userAgent),
      webkit  : /webkit/i.test(window.navigator.userAgent)
    };

  var getRandomId = function (prefix) {
    return (typeof prefix !== 'undefined' ? prefix + '-' : '') + Math.floor(Math.random() * 1000000);
  };

  var getUniqueId = function (o, prefix) {
    var
      iterCount = 0,
      maxIterations = 1000,
      uniqueId = getRandomId();

    while (o.hasOwnProperty(uniqueId) && iterCount++ < maxIterations) {
      uniqueId = getRandomId(prefix);
    }
    return uniqueId;
  };

  var customEventIsSupported = function (eventType) {
    if (supportedCustomEvents.hasOwnProperty(eventType = eventType.toLowerCase())) {
      return supportedCustomEvents[eventType];
    }
    return false;
  };

  var execCustomEvent = function (eventType, thisArg, argsArray) {
    eventType = eventType.toLowerCase();
    thisArg = thisArg || window;
    argsArray = argsArray || [];

    if (!customEvents.hasOwnProperty(eventType)) {
      return undefined;
    }
    var customEvent = customEvents[eventType];

    for (var eventId in customEvent) {
      if (customEvent.hasOwnProperty(eventId)) {
        customEvent[eventId].handler.apply(thisArg, argsArray);
        if (customEvent[eventId].type & 1) {
          delete customEvent[eventId];
        }
      }
    }
  };

  var addCustomEvent = function (eventType, handler, params) {
    if (typeof eventType !== 'string' || typeof handler !== 'function') {
      return undefined;
    }

    eventType = eventType.toLowerCase();
    if (!customEventIsSupported(eventType)) {
      return undefined;
    }

    params = params || {};
    var isOnceExec = params.hasOwnProperty('once') ? params.once : false;

    var handlerType = 0;
    handlerType |= +isOnceExec * 1;  

    if (!customEvents.hasOwnProperty(eventType)) {
      customEvents[eventType] = {};
    }

    var eventId = getUniqueId(customEvents[eventType]);

    customEvents[eventType][eventId] = {
      handler : handler,
      type    : handlerType
    };

    return eventId;
  };

  var removeCustomEvent = function (eventType, eventId) {
    if (typeof eventType !== 'string' || !customEventIsSupported(eventType = eventType.toLowerCase()) || typeof eventId === 'undefined') {
      return false;
    }

    if (customEvents(eventType) && customEvents[eventType].hasOwnProperty(eventId)) {
      delete userEventHandlers[eventType][eventId];
    }
    return true;
  };

  var domReady = function () {
    if (domIsReady === true) {
      return undefined;
    }
    // TODO
  };

  var init = (function () {
    // < ie8
    if (browse.msie && isFinite(parseInt(browse.version)) && parseInt(browse.version) < 8) {
      return function () {
        jQuery.historyInit(historyWatcher);
      };
    }
    return function () {
      jQuery.historyInit(historyWatcher);
    };
  })();

  var historyWatcher = (function () {
    // < ie8
    if (browse.msie && isFinite(parseInt(browse.version)) && parseInt(browse.version) < 8) {
      return function (label) {
        if (typeof label !== 'string' || label === '#') {
          label = '';
        }
        label = label.charAt(0) === '#' ? label.substring(1) : label;
        currentAnchor = '#' + label;

        execCustomEvent('onupdate', window, [label]);
      };
    }
    return function (label) {
      if (typeof label !== 'string' || label === '#') {
        label = '';
      }
      label = label.charAt(0) === '#' ? label.substring(1) : label;
      currentAnchor = '#' + label;

      execCustomEvent('onupdate', window, [label]);
    };
  })();

  var moveTo = (function () {
    // < ie8
    if (browse.msie && isFinite(parseInt(browse.version)) && parseInt(browse.version) < 8 && false) {
      return function (anchor) {
        return undefined;
      };
    }
    return function (anchor) {
      if (typeof anchor !== 'string' || anchor.length === 0 || anchor === '#') {
        return undefined;
      }

      if (anchor.charAt(0) !== '#') {
        anchor = '#' + anchor;
      }

      if (anchor === currentAnchor) {
        return undefined;
      }

      var label = anchor.substring(1);
      if (label.length === 0) {
        return undefiend;
      }
      anchor = '#' + label;

      jQuery.historyLoad(label);
    };
  })();


  return {
    init    : init,
    move    : moveTo,
    bind    : addCustomEvent,
    unbind  : removeCustomEvent
  };
})();
