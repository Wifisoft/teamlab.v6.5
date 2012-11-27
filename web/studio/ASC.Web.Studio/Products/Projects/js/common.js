/*******************************************************************************
   JQuery Extension
*******************************************************************************/

jQuery.extend({
    /**
    * Returns get parameters.
    *
    * If the desired param does not exist, null will be returned
    *
    * @example value = $.getURLParam("paramName");
    */
    getURLParam: function(strParamName) {

        strParamName = strParamName.toLowerCase();

        var strReturn = "";
        var strHref = window.location.href.toLowerCase();
        var bFound = false;

        var cmpstring = strParamName + "=";
        var cmplen = cmpstring.length;

        if (strHref.indexOf("?") > -1) {
            var strQueryString = strHref.substr(strHref.indexOf("?") + 1);
            var aQueryString = strQueryString.split("&");
            for (var iParam = 0; iParam < aQueryString.length; iParam++) {
                if (aQueryString[iParam].substr(0, cmplen) == cmpstring) {
                    var aParam = aQueryString[iParam].split("=");
                    strReturn = aParam[1];
                    bFound = true;
                    break;
                }
            }
        }
        if (bFound == false) return null;

        if (strReturn.indexOf("#") > -1)
            return strReturn.split("#")[0];

        return strReturn;
    },
    getAnchorParam: function(paramName, url) {
        var regex = new RegExp("[#&]" + paramName + "=([^&]*)");
        var results = regex.exec('#' + url);
        if (results == null)
            return "";
        else
            return results[1];
    },
    hasParam: function(paramName, url) {
        var regex = new RegExp('(\\#|&|^)' + paramName + '=', 'g'); //matches `#param=` or `&param=` or `param=`
        return regex.test(url);
    },
    removeParam: function(paramName, url) {
        var regex = new RegExp("[#&]" + paramName + "=([^&]*)");
        return url.replace(regex, '');
    },
    addParam: function(paramsList, name, value) {
        if (paramsList.length) paramsList += '&';
        paramsList = paramsList + name + '=' + value;
        return paramsList;
    },
    changeParamValue: function(paramsList, name, value) {
        if (jq.hasParam(name, paramsList)) {
            var regex = new RegExp(name + "[=][0-9a-z\-]*");
            return paramsList.replace(regex, name + '=' + value);
        } else {
            return jq.addParam(paramsList, name, value);
        }
    },
    format: function jQuery_dotnet_string_format(text) {
        //check if there are two arguments in the arguments list
        if (arguments.length <= 1) {
            //if there are not 2 or more arguments there's nothing to replace
            //just return the text
            return text;
        }
        //decrement to move to the second argument in the array
        var tokenCount = arguments.length - 2;
        for (var token = 0; token <= tokenCount; ++token) {
            //iterate through the tokens and replace their placeholders from the text in order
            text = text.replace(new RegExp("\\{" + token + "\\}", "gi"), arguments[token + 1]);
        }
        return text;
    },
    linksParser: function(val) {
        var replaceUrl = function(str) {
            return '<a target="_new" href="' + (/^http/.test(str) ? str : 'http://' + str) + '">' + str + '</a>';
        };
        var regUrl = /((?:(?:([a-z]*)\:)\/\/)|(?:www\.))[\da-zA-Z][\da-z-A-Z\.]+[\da-zA-Z](?::(\d+))?[&\-\_/\da-zA-Z\.\?=%#|\[\]\(\)]+/ig;
        return val.replace(regUrl, replaceUrl);
    },
    isValidDate: function(date) { // for dates of deytpiker with a mask
        var dateFormat = Teamlab.constants.dateFormats.date;
        var separator = "/";
        var dateComponent;
        var dateFormatComponent = dateFormat.split('/');
        if (dateFormatComponent.length == 1) {
            dateFormatComponent = dateFormat.split('.');
            separator = ".";
            if (dateFormatComponent.length == 1) {
                dateFormatComponent = dateFormat.split('-');
                separator = "-";
                if (dateFormatComponent.length == 1) {
                    return "Unknown format date";
                }
            }
        }
        dateComponent = date.split(separator);

        for (var i = 0; i < dateFormatComponent.length; i++) {
            if (dateFormatComponent[i][0].toLowerCase() == "d") {
                if (parseInt(dateComponent[i]) > 31) {
                    return false;
                }
            }
            if (dateFormatComponent[i][0].toLowerCase() == "m") {
                if (parseInt(dateComponent[i]) > 12) {
                    return false;
                }
            }

        }
        return true;
    },
    timeFormat: function(hours){ // convert time to format h:mm
        var h = Math.floor(parseFloat(hours));
        var m = Math.round((parseFloat(hours) - h) * 60);
        if (m < 10) {
            m = '0' + m;
        } else {
            if (m == 60) {
                m = "00";
                h = h + 1;
            }
        }
        return h + ':' + m;
    }
});

(function($) {
    $.fn.yellowFade = function() {
        return (this.css({ backgroundColor: "#ffffcc" }).animate(
            {
                backgroundColor: "#ffffff"
            }, 1500));
    };

    $.fn.autoResize = function(options) {
        var settings = $.extend({
            onResize: function() {

            },
            animate: true,
            animateDuration: 150,
            animateCallback: function() { },
            extraSpace: 20,
            limit: 200
        }, options);

        this.filter('textarea').each(function() {
            var textarea = $(this).css({ resize: 'none', 'overflow-y': 'hidden' }),
                origHeight = textarea.height(),
                clone = (function() {
                    var props = ['height', 'width', 'lineHeight', 'textDecoration', 'letterSpacing'],
                        propOb = {};
                    $.each(props, function(i, prop) {
                        propOb[prop] = textarea.css(prop);
                    });

                    return textarea.clone().removeAttr('id').removeAttr('name').css({
                        position: 'absolute',
                        top: 0,
                        left: -9999
                    }).css(propOb).attr('tabIndex', '-1').insertBefore(textarea);

                })(),
                lastScrollTop = null,
                updateSize = function() {
                    clone.height(0).width('99%').val($(this).val()).scrollTop(10000);

                    var scrollTop = Math.max(clone.scrollTop(), origHeight) + settings.extraSpace,
                        toChange = $(this).add(clone);

                    if (lastScrollTop === scrollTop) { return; }
                    lastScrollTop = scrollTop;

                    if (scrollTop >= settings.limit) {
                        $(this).css('overflow-y', '');
                        return;
                    }
                    settings.onResize.call(this);

                    settings.animate && textarea.css('display') === 'block' ?
                        toChange.stop().animate({ height: scrollTop }, settings.animateDuration, settings.animateCallback)
                        : toChange.height(scrollTop);
                };

            textarea
                .unbind('.dynSiz')
                .bind('keyup.dynSiz', updateSize)
                .bind('keydown.dynSiz', updateSize)
                .bind('change.dynSiz', updateSize);
        });

        return this;
    };
    // google analitics track
    $.fn.trackEvent = function(category, action, label) {
        jq(this).live("click", function() { // remove live!
            //console.log(category + " " + action + " " + label);
            try {
                if (window._gat) {
                    window._gaq.push(['_trackEvent', category, action, label]);
                }
            } catch (err) {
            }
            return true;
        });
    };
})(jQuery);


jQuery.fn.swap = function(b) {
    b = jQuery(b)[0];
    var a = this[0],
        a2 = a.cloneNode(true),
        b2 = b.cloneNode(true),
        stack = this;

    a.parentNode.replaceChild(b2, a);
    b.parentNode.replaceChild(a2, b);

    stack[0] = a2;
    return this.pushStack(stack);
};

/*******************************************************************************/
if (typeof ASC === 'undefined')
    ASC = { };

ASC.Projects = (function() { // Private Section
    return {// Public Section
    };
})();

ASC.Projects.Constants =
    {
        MINI_LOADER_IMG: ""
    };

ASC.Projects.Common = (function() { // Private Section
    return {// Public Section

        tooltip: function(target_items, name, isNew) {

            if (typeof(isNew) != "undefined" && jq(target_items).length == 1) {
                var id = jq.trim(jq(target_items).attr('id')).split('_')[1];
                var title = jq.trim(jq(target_items).attr('title'));
                if (title == "" || title == null) return;

                if (isNew) {
                    jq("body").append("<div style='display:none;' class='borderBase tintMedium " + name + "' id='" + name + id + "'><p>" + title + "</p></div>");
                } else {
                    jq("#" + name + id).html("<p>" + title + "</p>");
                }

                var tooltip = jq("#" + name + id);

                jq(target_items).removeAttr("title")
                    .mouseover(function() { tooltip.show(); })
                    .mousemove(function(kmouse) { tooltip.css({ left: kmouse.pageX + 15, top: kmouse.pageY + 15 }); })
                    .mouseout(function() { tooltip.hide(); });

                return false;
            }

            jq(target_items).each(function() {
                var id = jq.trim(jq(this).attr('id')).split('_')[1];
                var title = jq.trim(jq(this).attr('title'));
                if (title == "" || title == null) return;

                jq("body").append("<div style='display:none;' class='borderBase tintMedium " + name + "' id='" + name + id + "'><p>" + title + "</p></div>");

                var my_tooltip = jq("#" + name + id);

                jq(this).removeAttr("title")
                    .mouseover(function() { my_tooltip.show(); })
                    .mousemove(function(kmouse) { my_tooltip.css({ left: kmouse.pageX + 15, top: kmouse.pageY + 15 }); })
                    .mouseout(function() { my_tooltip.hide(); });
            });

        },
        changeImage: function(target_items, srcMouseOver, srcMouseOut) {

            jq(target_items).each(function(i) {
                jq(this)
                    .mouseover(
                        function() {
                            jq(this).attr('src', srcMouseOver);
                        }
                    )
                    .mousemove(
                        function(kmouse) {
                        }
                    )
                    .mouseout(
                        function() {
                            jq(this).attr('src', srcMouseOut);
                        }
                    );
            });

        },

        IAmIsManager: false
    };

})();

// Google Analytics const
var ga_Categories = {
    projects: "projects",
    milestones: "milestones",
    tasks: "tasks",
    subtask: "subtask",
    discussions: "discussions",
    timeTrack: "time-track",
    dashboard: "dashboard",
    projectTemplate: "project-template"
};

var ga_Actions = {
    presetClick: "preset-click",
    filterClick: "filter-click",
    createNew: "create-new",
    remove: "remove",
    edit: "edit",
    view: "view",
    changeStatus: "change-status",
    next: "next",
    userClick: "user-click",
    actionClick: "action-click",
    quickAction: "quick-action"
};
// end Google Analytics