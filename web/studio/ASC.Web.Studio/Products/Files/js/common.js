/*******************************************************************************
JQuery Extension
*******************************************************************************/
if (typeof jq == "undefined")
    var jq = jQuery.noConflict();

(function($) {
    $.fn.yellowFade = function() {
        return (this.css({ backgroundColor: "#ffffcc" }).animate(
            { backgroundColor: "#ffffff" },
            1500,
            function() { jq(this).css({ backgroundColor: "" }); }));
    };
})(jQuery);

/*******************************************************************************
*******************************************************************************/

if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.Files === "undefined")
    ASC.Files = (function() { return { }; })();

if (typeof ASC.Files.Constants === "undefined") {
    ASC.Files.Constants = {
        REQUEST_STATUS_DELAY: 2000,

        MAX_NAME_LENGTH: 170,

        COUNT_ON_PAGE: 30,

        MAX_UPLOAD_SIZE: 0,

        anchorRegExp: {
            error: /^error\/(\S+)?/ ,
            imprt: /^import(\/(\w+))?/ ,
            preview: /^preview\/(\d+|[a-z]+-\d+(-.+)*)/ ,
            folder: /^(\d+|[a-z]+-\d+(-.+)*)/ ,
            anyanchor: /^(\S+)?/
        },

        typeNewDoc: {
            document: ".docx",
            spreadsheet: ".xlsx",
            presentation: ".pptx",
            image: ".svg"
        }
    };
}

if (typeof ASC.Files.Common === "undefined") {
    ASC.Files.Common = (function() {

        var isCorrectId = function(id) {
            if (typeof id === "undefined") return false;
            if (id === null) return false;
            if (id === 0) return false;
            return ASC.Files.Constants.anchorRegExp.folder.test(id);
        };

        var getSitePath = function() {
            var sitePath = jq.url.attr("protocol");
            sitePath += "://";
            sitePath += jq.url.attr("host");
            if (jq.url.attr("port") != null) {
                sitePath += ":";
                sitePath += jq.url.attr("port");
            }
            return sitePath;
        };

        var cancelBubble = function(e) {
            if (!e) e = window.event;
            e.cancelBubble = true;
            if (e.stopPropagation) e.stopPropagation();
        };

        var fixEvent = function(e) {
            e = e || window.event;

            if (e.pageX == null && e.clientX != null) {
                var html = document.documentElement;
                var body = document.body;
                e.pageX = e.clientX + (html && html.scrollLeft || body && body.scrollLeft || 0) - (html.clientLeft || 0);
                e.pageY = e.clientY + (html && html.scrollTop || body && body.scrollTop || 0) - (html.clientTop || 0);
            }

            if (!e.which && e.button) {
                e.which = e.button & 1 ? 1 : (e.button & 2 ? 3 : (e.button & 4 ? 2 : 0));
            }

            return e;
        };

        var blockUI = function(obj, width, height, top) {
            try {
                width = parseInt(width || 0);
                height = parseInt(height || 0);
                left = width > 0 ? parseInt(-width / 2) : -200;
                top = parseInt(top || -height / 2) + jq(window).scrollTop();


                jq.blockUI({
                    message: jq(obj),
                    css: {
                        left: "50%",
                        top: "50%",
                        opacity: "1",
                        border: "none",
                        padding: "0px",
                        width: width > 0 ? width + "px" : "auto",
                        height: height > 0 ? height + "px" : "auto",
                        cursor: "default",
                        textAlign: "left",
                        position: "absolute",
                        "margin-left": left + "px",
                        "margin-top": top + "px",
                        "background-color": "Transparent"
                    },

                    overlayCSS: {
                        backgroundColor: "#aaaaaa",
                        cursor: "default",
                        opacity: "0.3"
                    },

                    focusInput: false,
                    baseZ: 666,

                    fadeIn: 0,
                    fadeOut: 0,
                    onBlock: function() {
                        var $blockUI = jq(obj).parents("div.blockUI:first"), blockUI = $blockUI.removeClass("blockMsg").addClass("blockDialog").get(0), cssText = "";
                        if (jq.browser.msie && jq.browser.version < 9 && $blockUI.length !== 0) {
                            var prefix = " ", cssText = prefix + blockUI.style.cssText, startPos = cssText.toLowerCase().indexOf(prefix + "filter:"), endPos = cssText.indexOf(";", startPos);
                            if (startPos !== -1) {
                                if (endPos !== -1) {
                                    blockUI.style.cssText = [cssText.substring(prefix.length, startPos), cssText.substring(endPos + 1)].join("");
                                } else {
                                    blockUI.style.cssText = cssText.substring(prefix.length, startPos);
                                }
                            }
                        }
                    }
                });
            } catch(e) {
            }
            ;
        };

        var characterString = "@#$%&*+:;\"'<>?|\/";
        var characterRegExp = new RegExp("[@#$%&*\+:;\"'<>?|\\\\/]", "gim");

        var replaceSpecCharacter = function(str) {
            return str.trim().replace(ASC.Files.Common.characterRegExp, "_");
        };

        var keyCode = { enter: 13, esc: 27, spaceBar: 32, pageUP: 33, pageDown: 34, end: 35, home: 36, left: 37, up: 38, right: 39, down: 40, deleteKey: 46, a: 65, f: 70, n: 78 };

        return {
            getSitePath: getSitePath,
            cancelBubble: cancelBubble,
            fixEvent: fixEvent,
            blockUI: blockUI,
            keyCode: keyCode,

            characterString: characterString,
            characterRegExp: characterRegExp,
            replaceSpecCharacter: replaceSpecCharacter,

            isCorrectId: isCorrectId
        };
    })();
}

/*
* jQuery uri.
*/
jQuery.url = function() { var segments = {}; var parsed = {}; var options = { url: window.location, strictMode: false, key: ["source", "protocol", "authority", "userInfo", "user", "password", "host", "port", "relative", "path", "directory", "file", "query", "anchor"], q: { name: "queryKey", parser: /(?:^|&)([^&=]*)=?([^&]*)/g }, parser: { strict: /^(?:([^:\/?#]+):)?(?:\/\/((?:(([^:@]*):?([^:@]*))?@)?([^:\/?#]*)(?::(\d*))?))?((((?:[^?#\/]*\/)*)([^?#]*))(?:\?([^#]*))?(?:#(.*))?)/, loose: /^(?:(?![^:@]+:[^:@\/]*@)([^:\/?#.]+):)?(?:\/\/)?((?:(([^:@]*):?([^:@]*))?@)?([^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/} }; var parseUri = function() { str = decodeURI(options.url); var m = options.parser[options.strictMode ? "strict" : "loose"].exec(str); var uri = {}; var i = 14; while (i--) { uri[options.key[i]] = m[i] || "" } uri[options.q.name] = {}; uri[options.key[12]].replace(options.q.parser, function($0, $1, $2) { if ($1) { uri[options.q.name][$1] = $2 } }); return uri }; var key = function(key) { if (!parsed.length) { setUp() } if (key == "base") { if (parsed.port !== null && parsed.port !== "") { return parsed.protocol + "://" + parsed.host + ":" + parsed.port + "/" } else { return parsed.protocol + "://" + parsed.host + "/" } } return (parsed[key] === "") ? null : parsed[key] }; var param = function(item) { if (!parsed.length) { setUp() } return (parsed.queryKey[item] === null) ? null : parsed.queryKey[item] }; var setUp = function() { parsed = parseUri(); getSegments() }; var getSegments = function() { var p = parsed.path; segments = []; segments = parsed.path.length == 1 ? {} : (p.charAt(p.length - 1) == "/" ? p.substring(1, p.length - 1) : path = p.substring(1)).split("/") }; return { setMode: function(mode) { strictMode = mode == "strict" ? true : false; return this }, setUrl: function(newUri) { options.url = newUri === undefined ? window.location : newUri; setUp(); return this }, segment: function(pos) { if (!parsed.length) { setUp() } if (pos === undefined) { return segments.length } return (segments[pos] === "" || segments[pos] === undefined) ? null : segments[pos] }, attr: key, param: param} } ();


/*! Copyright (c) 2010 Brandon Aaron (http://brandonaaron.net)
* Licensed under the MIT License (LICENSE.txt).
* Thanks to: http://adomas.org/javascript-mouse-wheel/ for some pointers.
* Thanks to: Mathias Bank(http://www.mathias-bank.de) for a scope bug fix.
* Thanks to: Seamus Leahy for adding deltaX and deltaY
* Version: 3.0.4
* Requires: 1.2.2+
*/
(function($) { var types = ['DOMMouseScroll', 'mousewheel']; $.event.special.mousewheel = { setup: function() { if (this.addEventListener) { for (var i = types.length; i; ) { this.addEventListener(types[--i], handler, false); } } else { this.onmousewheel = handler; } }, teardown: function() { if (this.removeEventListener) { for (var i = types.length; i; ) { this.removeEventListener(types[--i], handler, false); } } else { this.onmousewheel = null; } } }; $.fn.extend({ mousewheel: function(fn) { return fn ? this.bind("mousewheel", fn) : this.trigger("mousewheel"); }, unmousewheel: function(fn) { return this.unbind("mousewheel", fn); } }); function handler(event) { var orgEvent = event || window.event, args = [].slice.call(arguments, 1), delta = 0, returnValue = true, deltaX = 0, deltaY = 0; event = $.event.fix(orgEvent); event.type = "mousewheel"; if (event.wheelDelta) { delta = event.wheelDelta / 120; } if (event.detail) { delta = -event.detail / 3; } deltaY = delta; if (orgEvent.axis !== undefined && orgEvent.axis === orgEvent.HORIZONTAL_AXIS) { deltaY = 0; deltaX = -1 * delta; } if (orgEvent.wheelDeltaY !== undefined) { deltaY = orgEvent.wheelDeltaY / 120; } if (orgEvent.wheelDeltaX !== undefined) { deltaX = -1 * orgEvent.wheelDeltaX / 120; } args.unshift(event, delta, deltaX, deltaY); return $.event.handle.apply(this, args); } })(jQuery);
