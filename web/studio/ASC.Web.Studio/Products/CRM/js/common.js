/*******************************************************************************
JQuery Extension
*******************************************************************************/

jq.browser.safari = (jq.browser.safari && !/chrome/.test(navigator.userAgent.toLowerCase())) == !0;

jQuery.extend({

//    parseJSON: function(data) {
//        if (typeof data !== "string" || !data) {
//            return null;
//        }

//        // Make sure leading/trailing whitespace is removed (IE can't handle it)
//        data = jQuery.trim(data);

//        // Make sure the incoming data is actual JSON
//        // Logic borrowed from http://json.org/json2.js
//        if (/^[\],:{}\s]*$/.test(data.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@")
//            .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]")
//            .replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) {

//            // Try to use the native JSON parser first
//            var res = window.JSON && window.JSON.parse ?
//                window.JSON.parse(data) :
//                (new Function("return " + data))();

//            return res != null ? res : eval("(" + data + ")");

//        } else {
//            jQuery.error("Invalid JSON: " + data);
//        }
//    },


    /*
    var result = $.format("Hello, {0}.", "world");
    //result -> "Hello, world."
    */
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
    }



}
);


/*******************************************************************************

*******************************************************************************/

if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = (function() { return {} })();

jQuery.extend({

    forceNumericOnly: function() {
        return this.each(function() {
            jq(this).keydown(function(e) {
                var key = e.charCode || e.keyCode || 0;
                // allow backspace, tab, delete, arrows, numbers and keypad numbers ONLY
                return (
                key == 8 ||
                key == 9 ||
                key == 46 ||
                (key >= 37 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
            })
        })
    },

    forcePhoneSymbolsOnly: function(objSelector) {
        jq(objSelector).each(function() {
            jq(this).keypress(function(event) {

                var index = jq(this).val().indexOf("+");
                var caret = jq(this).caret().begin;
                var controlKeys = [8, 9];
                var isControlKey = controlKeys.join(",").match(new RegExp(event.which));

                if (!event.which ||
                    (48 <= event.which && event.which <= 57) || //number
                    isControlKey ||
                    (event.which == 43 && caret == 0 && index == -1) || //plus
                    (event.which == 32 && caret != 0) || //space
                    event.which == 45 || event.which == 40 || event.which == 41) {
                    return;
                } else {
                    event.preventDefault();
                }
            });
        });
    },

    forceIntegerOnly: function(ObjSelector, doIfInteger) {
        jq(ObjSelector).each(function() {
            jq(this).keypress(function(event) {
                // Backspace, Del
                var controlKeys = [8, 9];
                // IE doesn't support indexOf
                var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
                // Some browsers just don't raise events for control keys. Easy.

                if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                    (48 <= event.which && event.which <= 57) || // Always 0 through 9
                    isControlKey) {
                    return;
                } else {
                    event.preventDefault();
                }
            });

            jq(this).unbind('paste').bind('paste', function(e) {
                var oldValue = this.value;
                var $obj = this;
                setTimeout(
                    function() {
                        var text = jq($obj).val();
                        if (isNaN(text) || text.indexOf(".") != -1) {
                            jq($obj).val(oldValue);
                        }
                        else if (typeof (doIfInteger) != "undefined") {
                            doIfInteger();
                        }
                    }, 0);
                return true;
            });

        });
    },
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
    }
}
);


/*******************************************************************************
*******************************************************************************/

ASC.CRM.Common = (function() {
    return {
        blockUI: function(obj, width, height, top) {
            try {
                width = parseInt(width || 0);
                height = parseInt(height || 0);
                left = parseInt(-width / 2);
                top = parseInt(top || -height / 2);
                jq.blockUI({ message: jq(obj),
                    css: {
                        left: '50%',
                        top: '50%',
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: width > 0 ? width + 'px' : 'auto',
                        height: height > 0 ? height + 'px' : 'auto',
                        cursor: 'default',
                        textAlign: 'left',
                        position: 'fixed',
                        'margin-left': left + 'px',
                        'margin-top': top + 'px',
                        'background-color': 'Transparent'
                    },

                    overlayCSS: {
                        backgroundColor: '#aaaaaa',
                        cursor: 'default',
                        opacity: '0.3'
                    },

                    focusInput: true,
                    baseZ: 666,

                    fadeIn: 0,
                    fadeOut: 0,
                    onBlock: function() {
                        var $blockUI = jq(obj).parents('div.blockUI:first'), blockUI = $blockUI.removeClass('blockMsg').addClass('blockDialog').get(0), cssText = '';
                        if (jq.browser.msie && jq.browser.version < 9 && $blockUI.length !== 0) {
                            var prefix = ' ', cssText = prefix + blockUI.style.cssText, startPos = cssText.toLowerCase().indexOf(prefix + 'filter:'), endPos = cssText.indexOf(';', startPos);
                            if (startPos !== -1) {
                                if (endPos !== -1) {
                                    blockUI.style.cssText = [cssText.substring(prefix.length, startPos), cssText.substring(endPos + 1)].join('');
                                } else {
                                    blockUI.style.cssText = cssText.substring(prefix.length, startPos);
                                }
                            }
                        }
                    }
                });
            }
            catch (e) { };
        },
        toogleAjaxInfoBlock: function(parentID) {


        },
        tooltip: function(target_items, name, isNew) {
            overTaskDescrPanel = false;
            var hideDescrPanel = function(tooltipItem) {
                setTimeout(function() {
                    if (!overTaskDescrPanel) tooltipItem.hide(100);
                }, 200);
            };


            if (typeof (isNew) != "undefined" && jq(target_items).length == 1) {
                var id = jq.trim(jq(target_items).attr('id')).split('_')[1];
                var title = jq.trim(jq(target_items).attr('title'));
                var params = ASC.CRM.Common.getTooltipParams(jq(target_items));
                if (title == "" && params == "") return;

                if (!isNew) jq("#" + name + id).remove();

                jq("body").append("<div class='dropDownDialog tooltip' style='display: none;' id='" + name + id + "'>\
                                            <div class='dropDownCorner'></div>"
                                            + Encoder.htmlEncode(title).replace(/&#10;/g, "<br/>") + params + "</div>");

                var tooltip = jq("#" + name + id);

                jq(target_items).removeAttr("title")
                        .mouseenter(function() {
                            overTaskDescrPanel = true;
                            var left = jq(target_items).position().left + 5;
                            var top = jq(target_items).position().top + jq(target_items).height() + 5;
                            jq(tooltip).css("top", top).css("left", left);
                            jq("div[id^='" + name + "']:not(div[id='" + name + id + "'])").hide();
                            tooltip.fadeIn(300);
                        })
                        .mouseleave(function() {
                            overTaskDescrPanel = false;
                            hideDescrPanel(tooltip);
                        });

                jq(tooltip)
                    .mouseenter(function() {
                        overTaskDescrPanel = true;
                    })
                    .mouseleave(function() {
                        overTaskDescrPanel = false;
                        hideDescrPanel(tooltip);
                    });
                return false;
            }

            jq(target_items).each(function() {
                var id = jq.trim(jq(this).attr('id')).split('_')[1];
                var title = jq.trim(jq(this).attr('title'));
                var params = ASC.CRM.Common.getTooltipParams(jq(this));
                if (title == "" && params == "") return;

                jq("body").append("<div style='display:none;' class='dropDownDialog tooltip' id='" + name + id + "'>\
                                        <div class='dropDownCorner'></div>"
                                        + Encoder.htmlEncode(title).replace(/&#10;/g, "<br/>") + params + "</div>");

                var my_tooltip = jq("#" + name + id);

                jq(this).removeAttr("title")
                    .mouseenter(function() {
                        overTaskDescrPanel = true;
                        var left = jq(this).position().left + 5;
                        var top = jq(this).position().top + jq(this).height() + 5;
                        jq(my_tooltip).css("top", top).css("left", left);
                        jq("div[id^='" + name + "']:not(div[id='" + name + id + "'])").hide();
                        my_tooltip.fadeIn(300);
                    })
                    .mouseleave(function() {
                        overTaskDescrPanel = false;
                        hideDescrPanel(my_tooltip);
                    });

                jq(my_tooltip)
                    .mouseenter(function() {
                        overTaskDescrPanel = true;
                    })
                    .mouseleave(function() {
                        overTaskDescrPanel = false;
                        hideDescrPanel(my_tooltip);
                    });
            });
        },

        getTooltipParams: function(obj) {
            var res = "";
            var label = jq.trim(jq(obj).attr("ttl_label"));
            var value = jq.trim(jq(obj).attr("ttl_value"));

            if (label != "" && value != "")
                res += "<div style='overflow: hidden;'><div class='param'>" + Encoder.htmlEncode(label).replace(/&#10;/g, "<br/>") +
                    ":</div><div class='value'>" + Encoder.htmlEncode(value).replace(/&#10;/g, "<br/>") + "</div></div>";

            jq(obj).removeAttr("ttl_label");
            jq(obj).removeAttr("ttl_value");

            label = jq.trim(jq(obj).attr("dscr_label"));
            value = jq.trim(jq(obj).attr("dscr_value"));

            if (label != "" && value != "")
                res += "<div style='overflow: hidden;'><div class='param'>" + Encoder.htmlEncode(label).replace(/&#10;/g, "<br/>") +
                    ":</div><div class='value'>" + Encoder.htmlEncode(value).replace(/&#10;/g, "<br/>") + "</div></div>";

            jq(obj).removeAttr("dscr_label");
            jq(obj).removeAttr("dscr_value");

            label = jq.trim(jq(obj).attr("resp_label"));
            value = jq.trim(jq(obj).attr("resp_value"));

            if (label != "" && value != "")
                res += "<div style='overflow: hidden;'><div class='param'>" + Encoder.htmlEncode(label).replace(/&#10;/g, "<br/>") +
                    ":</div><div class='value'>" + Encoder.htmlEncode(value).replace(/&#10;/g, "<br/>") + "</div></div>";

            jq(obj).removeAttr("resp_label");
            jq(obj).removeAttr("resp_value");

            return res;
        },

        setRowColorElements: function(rows) {
            rows.removeClass("tintMedium tintLight");

            for (var i = 0; i < rows.length; i++) {
                if (i % 2 == 0)
                    jq(rows[i]).addClass("tintMedium");
                else
                    jq(rows[i]).addClass("tintLight");
            }
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

        RegisterContactInfoCard: function() {
            var CRMContactInfoCard = new PopupBox('popup_ContactInfoCard', 320, 140, 'tintLight', 'borderBase', 'AjaxPro.AjaxProHelper.GetContactInfoCard');

            jq(window).bind("registerContactInfoCard", function(event, newElem) {
                var id = newElem.attr('id');
                if (id != null && id != '') {
                    CRMContactInfoCard.RegistryElement(id, "\"" + newElem.attr('data-id') + "\"");
                }
            });

            jq(".crm-companyInfoCardLink, .crm-peopleInfoCardLink").each(function(index) {
                jq(window).trigger("registerContactInfoCard", [jq(this)]);
            });
        },

        isHiddenForScroll: function(el) {
            var pos = el.position();
            var scrTop = jq(window).scrollTop();
            var scrLeft = jq(window).scrollLeft();
            if (pos.top > scrTop && pos.left > scrLeft &&
                pos.top + el.height() < scrTop + jq(window).height() &&
                pos.left + el.width() < scrLeft + jq(window).width())
                return false;
            return true;
        },

        renderCustomFields: function(customFieldList, custom_field_id_prefix, customFieldTmplID, customFieldTmplAppendToObjSelector) {
            var tmpSelectFields = [];
            for (var i = 0; i < customFieldList.length; i++) {
                var field = customFieldList[i];

                if (jQuery.trim(field.mask) == "") continue;

                field.mask = jq.evalJSON(field.mask);
                if (customFieldList[i].fieldType == 2) {
                    tmpSelectFields.push(customFieldList[i]);
                }
            }
            jq("#" + customFieldTmplID).tmpl(customFieldList).appendTo(customFieldTmplAppendToObjSelector);

            for (var k = 0; k < tmpSelectFields.length; k++) {
                jq('#' + custom_field_id_prefix + tmpSelectFields[k].id).val(tmpSelectFields[k].value);
            }
        },

        changeCountInTab: function(actionOrCount, tabAnchorName) {
            var contactTab;

            if (typeof (tabAnchorName) != "undefined" && tabAnchorName != "") {
                if (jq("li.viewSwitcherTab_" + tabAnchorName).length == 0)
                    return false;
                contactTab = jq("li.viewSwitcherTab_" + tabAnchorName);
            }
            else {
                if (jq("li[id$=_ViewSwitcherTab].viewSwitcherTabSelected").length == 0)
                    return false;
                contactTab = jq("li[id$=_ViewSwitcherTab].viewSwitcherTabSelected")[0];
            }

            var contactTabTextArray = jq(contactTab).text().split(" ");

            if (typeof (actionOrCount) == "string") {
                if (actionOrCount == "add") {
                    if (contactTabTextArray.length > 1) {
                        var newValue = parseInt(contactTabTextArray[contactTabTextArray.length - 1].replace("(", "").replace(")", "")) + 1;
                        contactTabTextArray[contactTabTextArray.length - 1] = jq.format("({0})", newValue);
                        jq(contactTab).text(contactTabTextArray.join(" "));
                    }
                    else
                        jq(contactTab).text(jq.format("{0} (1)", contactTabTextArray[0]));
                }
                else if (actionOrCount == "delete") {
                    var newValue = parseInt(contactTabTextArray[contactTabTextArray.length - 1].replace("(", "").replace(")", "")) - 1;
                    if (newValue > 0) {
                        contactTabTextArray[contactTabTextArray.length - 1] = jq.format("({0})", newValue);
                        jq(contactTab).text(contactTabTextArray.join(" "));
                    }
                    else
                        jq(contactTab).text(contactTabTextArray[0]);
                }
            }
            else if (typeof (actionOrCount) == "number") {
                var count = parseInt(actionOrCount);
                if (count > 0)
                    jq(contactTab).text(jq.format("{0} ({1})", contactTabTextArray[0], count));
                else
                    jq(contactTab).text(contactTabTextArray[0]);
            }
        },

        getHexRGBColor: function(color) {
            color = color.replace(/\s/g, "");
            var aRGB = color.match(/^rgb\((\d{1,3}[%]?),(\d{1,3}[%]?),(\d{1,3}[%]?)\)$/i);

            if (aRGB) {
                color = '#';
                for (var i = 1; i <= 3; i++) color += Math.round((aRGB[i][aRGB[i].length - 1] == "%" ? 2.55 : 1) * parseInt(aRGB[i])).toString(16).replace(/^(.)$/, '0$1');
            }
            else color = color.replace(/^#?([\da-f])([\da-f])([\da-f])$/i, '$1$1$2$2$3$3');

            return color;
        },

        ClientTimeToServer: function(clientTime, offset) {
            var date = new Date();
            var minutesOffset = date.getTimezoneOffset();

            if (offset != undefined)
                minutesOffset = (-1) * offset;

            date.setTime(clientTime.getTime() + (minutesOffset * 60 * 1000));

            var m = (date.getMonth() + 1);
            var d = date.getDate();
            var h = date.getHours();
            var min = date.getMinutes();

            var str = date.getFullYear() + '-' + (m > 9 ? m : ('0' + m)) + '-' + (d > 9 ? d : ('0' + d))
                        + 'T' + (h > 9 ? h : ('0' + h)) + '-' + (min > 9 ? min : ('0' + min)) + "-00.000Z";

            return str;
        },

        stickMenuToTheTop: function(options) {
            if (typeof (options) != "object" ||
                typeof (options.menuSelector) == "undefined" ||
                typeof (options.menuAnchorSelector) == "undefined" ||
                typeof (options.menuSpacerSelector) == "undefined") return;

            var $menuObj = jq(options.menuSelector);
            var $boxTop = jq(options.menuAnchorSelector);
            var $menuSpacerObj = jq(options.menuSpacerSelector);

            if ($menuObj.length == 0 || $boxTop.length == 0 || $menuSpacerObj.length == 0) return;

            var newTop = $boxTop.offset().top + $boxTop.outerHeight();
            var winScrollTop = jq(window).scrollTop();
            var tempTop = 0;

            if ($menuSpacerObj.css("display") == "none")
                tempTop += $menuObj.offset().top;
            else
                tempTop += $menuSpacerObj.offset().top;

            if (winScrollTop >= tempTop) {
                $menuSpacerObj.show();
                $menuObj.addClass("fixed");

                jq("#files_selectorPanel").css({
                    "position": "fixed",
                    "top": newTop - winScrollTop
                });

                if (typeof (options.userFuncNotInTop) == "function")
                    options.userFuncNotInTop();

                if (jq.browser.mobile) {
                    $menuObj.css(
                        {
                            "top": jq(document).scrollTop() + "px",
                            "position": "absolute"
                        });
                }
            }
            else {
                $menuSpacerObj.hide();
                $menuObj.removeClass("fixed");

                jq("#files_selectorPanel").css({
                    "position": "absolute",
                    "top": newTop
                });

                if (typeof (options.userFuncInTop) == "function")
                    options.userFuncInTop();

                if (jq.browser.mobile) {
                    $menuObj.css(
                        {
                            "position": "static"
                        });
                }
            }
        },

        animateElement: function(options) {
            if (typeof (options) != "object") return;
            var element = options.element;

            if (ASC.CRM.Common.isHiddenForScroll(element)) {
                jq.scrollTo(element);

                if (typeof (options.whenScrollFunc) === "function")
                    options.whenScrollFunc();
            }

            if (typeof (options.afterScrollFunc) === "function")
                options.afterScrollFunc();

            element.css({ "background-color": "#ffffcc" });
            element.animate({ backgroundColor: '#ffffff' }, 2000, function() {
                element.css({ "background-color": "" });
            });
        }
    }
})();


ASC.CRM.HistoryView = (function($) {

    Teamlab.bind(Teamlab.events.getException, onGetException);

    function onGetException(params, errors) {
        console.log('common.js ', errors);
        LoadingBanner.hideLoading();
    };

    var extendSelect = function($select, options) {
        $select.children('option[value="-2"]').siblings().remove();
        var html = [];
        for (var i = 0, n = options.length; i < n; i++) {
            html.push('<option value="' + options[i].id + '">' + i + '</option>');
        }
        $select.append(html.join(''));

        var
        ind = 0,
        $option = null,
        $options = $select.children('option[value="-2"]').siblings(),
        optionsInd = $options.length;
        while (optionsInd--) {
            $option = jq($options[optionsInd]);
            ind = $option.text();
            ind = isFinite(+ind) ? +ind : -1;
            if (ind > -1 && ind < options.length) {
                $option.text(Encoder.htmlDecode(options[ind].title));
            } else {
                $option.remove();
            }
        }
        return $select;
    };

    var _renderContent = function() {
        if (ASC.CRM.HistoryView.isFirstTime == true) {
            ASC.CRM.HistoryView.isFirstTime = false;
            return;
        }

        LoadingBanner.displayLoading();
        ASC.CRM.HistoryView.ShowMore = true;
        _getEvents(0);
    };

    var _addRecordsToContent = function() {
        if (!ASC.CRM.HistoryView.ShowMore) return false;
        ASC.CRM.HistoryView.EventsList = {};

        jq("#showMoreEventsButtons .crm-showMoreLink").hide();
        jq("#showMoreEventsButtons .crm-loadingLink").show();

        var startIndex = jq("#eventsTable tr").length;
        _getEvents(startIndex);
    };

    var _getEvents = function(startIndex) {
        var filterSettings = {};
        filterSettings = ASC.CRM.HistoryView.getFilterSettings();

        filterSettings.entityid = ASC.CRM.HistoryView.ContactID != 0 ? ASC.CRM.HistoryView.ContactID : ASC.CRM.HistoryView.EntityID;
        filterSettings.entitytype = ASC.CRM.ListTaskView.EntityType;
        filterSettings.Count = ASC.CRM.HistoryView.CountOfRows;

        filterSettings.startIndex = startIndex;

        Teamlab.getCrmHistoryEvents({ startIndex: startIndex || 0 }, { filter: filterSettings, success: ASC.CRM.HistoryView.CallbackMethods.get_events_by_filter });
    };

    var _readDataEvent = function() {
        var content = jq.trim(jq("#historyBlock textarea").val());
        var _contactID, _entityType, _entityID;
        var dataEvent = {
            content: content,
            categoryId: eventCategorySelector.CategoryID
        };
        if (ASC.CRM.HistoryView.ContactID != 0) {
            dataEvent.contactID = ASC.CRM.HistoryView.ContactID;
            if (jq("#itemID").val() != "" && parseInt(jq("#itemID").val()) > 0 && jq("#typeID").val() != "") {
                dataEvent.entityId = jq("#itemID").val();
                dataEvent.entityType = jq("#typeID").val();
            }
        } else {
            if (jq("#contactID").val() != 0) dataEvent.contactID = jq("#contactID").val();
            dataEvent.entityId = ASC.CRM.HistoryView.EntityID;
            dataEvent.entityType = ASC.CRM.HistoryView.EntityType;
        }

        if (jq.trim(jq("#historyBlock .textEditCalendar").val()) != "") {
            dataEvent.created = jq("#historyBlock .textEditCalendar").datepicker('getDate');
            var now = new Date();
            dataEvent.created.setHours(now.getHours(), now.getMinutes(), now.getSeconds());

        }
        else {
            dataEvent.created = new Date();
            jq("#historyBlock .textEditCalendar").datepicker('setDate', dataEvent.created);
        }



        if (SelectedUsers.IDs.length != 0) {
            dataEvent.notifyUserList = SelectedUsers.IDs;
        }
        return dataEvent;
    };

    var _resizeFilter = function() {
        var visible = jq("#eventsFilterContainer").is(":hidden") == false;
        if (ASC.CRM.HistoryView.isFilterVisible == false && visible) {
            ASC.CRM.HistoryView.isFilterVisible = true;
            if (ASC.CRM.HistoryView.advansedFilter)
                jq("#eventsAdvansedFilter").advansedFilter("resize");
        }
    };

    var _fillEntityItems = function() {
        AjaxPro.HistoryView.GetEntityItems(ASC.CRM.HistoryView.ContactID,
            function(res) {
                if (res.error != null) { alert(res.error.Message); return; }
                var entityItems = jq(jq.parseJSON(res.value));

                if (entityItems.length > 0) {

                    if (!jq.browser.mobile) {
                        for (var i = 0, n = entityItems.length; i < n; i++) {
                            var item = jq("<a></a>").addClass("dropDownItem")
                                .text(entityItems[i].title)
                                .attr("itemid", entityItems[i].id)
                                .unbind("click")
                                .click(function() {
                                    ASC.CRM.HistoryView.changeHistoryItem(jq(this));
                                });

                            if (entityItems[i].type == "case")
                                jq("#historyCasePanel div.dropDownContent").append(item);
                            else
                                jq("#historyDealPanel div.dropDownContent").append(item);
                        }
                    }
                    else {
                        for (var i = 0, n = entityItems.length; i < n; i++) {
                            var item = jq("<option></option>").text(entityItems[i].title).attr("value", entityItems[i].id);

                            if (entityItems[i].type == "case")
                                jq("#historyCaseSelect").append(item);
                            else
                                jq("#historyDealSelect").append(item);
                        }
                    }
                    jq("#eventLinkToPanel").removeClass("empty-select");
                }
            });
    };

    return {
        CallbackMethods: {
            get_events_by_filter: function(params, events) {

                if (typeof (params.__nextIndex) == "undefined")
                    ASC.CRM.HistoryView.ShowMore = false;

                ASC.CRM.HistoryView.EventsList = events;

                for (var i = 0; i < events.length; i++) {
                    ASC.CRM.HistoryView.eventItemFactory(events[i]);
                }

                if (params.startIndex === 0) {
                    jq("#eventsTable tbody tr").remove();

                    if (events.length == 0) {
                        if (typeof (ASC.CRM.HistoryView.advansedFilter) == "undefined"
                            || ASC.CRM.HistoryView.advansedFilter.advansedFilter().length == 1) {
                            jq("#eventsList").hide();
                            jq("#eventsFilterContainer").hide();
                            jq("#emptyContentForEventsFilter").hide();
                            ASC.CRM.HistoryView.ShowMore = false;
                            LoadingBanner.hideLoading();
                            return false;
                        }
                        else {
                            jq("#eventsList").hide();
                            jq("#eventsFilterContainer").show();
                            jq("#emptyContentForEventsFilter").show();
                            LoadingBanner.hideLoading();
                            return false;
                        }
                    }

                    jq("#historyRowTmpl").tmpl(events).appendTo("#eventsTable tbody");

                    jq("#emptyContentForEventsFilter").hide();
                    jq("#eventsFilterContainer").show();
                    jq("#eventsList").show();
                    _resizeFilter();

                    LoadingBanner.hideLoading();
                }
                else {
                    if (events.length == 0 && jq("#eventsTable tbody tr").length == 0) {
                        jq("#eventsList").hide();
                        jq("#eventsFilterContainer").show();
                        jq("#emptyContentForEventsFilter").show();
                        LoadingBanner.hideLoading();
                        return false;
                    }
                    jq("#historyRowTmpl").tmpl(events).appendTo("#eventsTable tbody");
                }
                for (var i = 0; i < events.length; i++) {
                    if (events[i].files != null) {
                        jq.dropdownToggle({ dropdownID: "eventAttach_" + events[i].id, switcherSelector: "#eventAttachSwither_" + events[i].id, addTop: 5 });
                    }
                }

                jq("#showMoreEventsButtons .crm-loadingLink").hide();
                if (ASC.CRM.HistoryView.ShowMore)
                    jq("#showMoreEventsButtons .crm-showMoreLink").show();
                else
                    jq("#showMoreEventsButtons .crm-showMoreLink").hide();
            },

            add_event: function(params, event) {

                ASC.CRM.HistoryView.addEventToHistoryLayout(params, event);

                if (event.files != null) {
                    //for (var i = 0; i < event.files.length; i++)
                    //    ASC.CRM.Common.changeCountInTab("add", "files");
                    if (typeof (Attachments) != "undefined") {
                        Attachments.appendFilesToLayout(event.files);
                    }
                }
            },

            delete_event: function(params, event) {

                jq("#eventAttach_" + event.id).find("div[id^=fileContent_]").each(function() {
                    var fileID = jq(this).attr("id").split("_")[1];
                    if (typeof (Attachments) != "undefined") {
                        Attachments.deleteFileFromLayout(fileID);
                    }
                    //ASC.CRM.Common.changeCountInTab("delete", "files");
                });
                jq("#event_" + event.id).animate({ opacity: "hide" }, 500);
                setTimeout(function() {
                    jq("#event_" + event.id).remove();
                    if (jq("#eventsTable tr").length == 0) {
                        if (typeof (ASC.CRM.HistoryView.advansedFilter) != "undefined"
                            && ASC.CRM.HistoryView.advansedFilter.advansedFilter().length == 1) {
                            jq("#eventsFilterContainer").hide();
                            jq("#eventsList").hide();
                        }
                        else {
                            _renderContent();
                        }
                    }
                }, 500);
            },

            delete_file_from_event: function(params, file) {
                if (typeof (Attachments) != "undefined") {
                    Attachments.deleteFileFromLayout(file.id);
                }
                //ASC.CRM.Common.changeCountInTab("delete", "files");
                ASC.CRM.HistoryView.deleteFileFromEventLayout(file.id, params.messageID);
            }
        },


        init: function(contactID, entityType, entityID, countOfRows, dateTimeNowShortDateString, dateMask) {
            ASC.CRM.HistoryView.ContactID = contactID;
            ASC.CRM.HistoryView.EntityID = entityID;
            ASC.CRM.HistoryView.EntityType = entityType;

            ASC.CRM.HistoryView.CountOfRows = countOfRows;

            ASC.CRM.HistoryView.EventsList = {};
            ASC.CRM.HistoryView.ShowMore = true;
            ASC.CRM.HistoryView.isFilterVisible = false;
            ASC.CRM.HistoryView.isTabActive = false;
            ASC.CRM.HistoryView.isFirstTime = true;

            jq("#showMoreEventsButtons .crm-showMoreLink").bind("click", function() {
                _addRecordsToContent();
            });

            jq("#DepsAndUsersContainer").css("z-index", 999);

            jq("#historyBlock input.textEditCalendar").mask(dateMask);
            jq("#historyBlock input.textEditCalendar").datepicker().val(dateTimeNowShortDateString);

            jq("#historyBlock table #selectedUsers").remove();

            if (!jq.browser.mobile) {

                ASC.CRM.FileUploader.OnBeginCallback_function = function() {
                    jq("#historyBlock div.action_block").hide();
                    jq("#historyBlock div.ajax_info_block").show();
                    jq("#historyBlock textarea, #historyBlock input, #historyBlock select").attr("disabled", true);
                    jq("#attachButtonsPanel .attachLink").addClass("disabledLink");
                };

                ASC.CRM.FileUploader.OnAllUploadCompleteCallback_function = function() {
                    var data = _readDataEvent();
                    data.fileId = ASC.CRM.FileUploader.fileIDs;

                    Teamlab.addCrmHistoryEvent({}, data,
                        {
                            success: ASC.CRM.HistoryView.CallbackMethods.add_event,
                            before: function(params) { },
                            after: function(params) {
                                jq("#historyBlock div.action_block a.baseLinkButton").addClass("disableLink").removeClass("baseLinkButton");
                                jq("#historyBlock div.action_block").show();
                                jq("#historyBlock div.ajax_info_block").hide();
                                jq("#historyBlock textarea, #historyBlock input, #historyBlock select").attr("disabled", false);
                                jq("#attachButtonsPanel .attachLink").removeClass("disabledLink");
                            }
                        });

                };

                ASC.CRM.FileUploader.activateUploader();
                ASC.CRM.FileUploader.fileIDs.clear();
            }

            setInterval(function() {
                var text = jq.trim(jq("#historyBlock textarea").val());
                var $input = jq("#contactProfileEdit .info_for_company input[name=baseInfo_companyName]");
                if (text == "") {
                    jq("#historyBlock .action_block a.baseLinkButton").addClass("disableLink").removeClass("baseLinkButton");
                }
                else {
                    jq("#historyBlock .action_block a.disableLink").addClass("baseLinkButton").removeClass("disableLink");
                }
            }, 500);

        },

        setFilter: function(evt, $container, filter, params, selectedfilters) { _renderContent(); },
        resetFilter: function(evt, $container, filter, selectedfilters) { _renderContent(); },

        activate: function() {
            if (ASC.CRM.HistoryView.isTabActive == false) {
                ASC.CRM.HistoryView.isTabActive = true;
                LoadingBanner.displayLoading();
                _fillEntityItems();
                _renderContent();
            }
        },

        getFilterSettings: function() {
            var settings = {};

            if (ASC.CRM.HistoryView.advansedFilter.advansedFilter == null) return settings;

            var param = ASC.CRM.HistoryView.advansedFilter.advansedFilter();

            jq(param).each(function(i, item) {
                switch (item.id) {
                    case "sorter":
                        settings.sortBy = item.params.id;
                        settings.sortOrder = item.params.dsc == true ? 'descending' : 'ascending';
                        break;
                    case "text":
                        settings.filterValue = item.params.value;
                        break;
                    case "fromToDate":
                        settings.fromDate = new Date(item.params.from);
                        settings.toDate = new Date(item.params.to);
                        break;
                    default:
                        if (item.hasOwnProperty("apiparamname") && item.params.hasOwnProperty("value") && item.params.value != null) {
                            try {
                                var apiparamnames = jq.parseJSON(item.apiparamname);
                                var apiparamvalues = jq.parseJSON(item.params.value);
                                if (apiparamnames.length != apiparamvalues.length) {
                                    settings[item.apiparamname] = item.params.value;
                                }
                                for (var i = 0, len = apiparamnames.length; i < len; i++) {
                                    if (apiparamvalues[i].trim().length != 0)
                                        settings[apiparamnames[i]] = apiparamvalues[i];
                                }
                            } catch (err) {
                                settings[item.apiparamname] = item.params.value;
                            }
                        }
                        break;
                }
            });
            return settings;
        },

        eventItemFactory: function(eventItem) {
            if (eventItem.entity != null)
                switch (eventItem.entity.entityType) {
                case "opportunity":
                    eventItem.entityURL = "deals.aspx?id=" + eventItem.entity.entityId;
                    eventItem.entityType = CRMJSResources.Deal;
                    break;
                case "case":
                    eventItem.entityURL = "cases.aspx?id=" + eventItem.entity.entityId;
                    eventItem.entityType = CRMJSResources.Case;
                    break;
                default:
                    eventItem.entityURL = "";
                    eventItem.entityType = "";
                    break;
            }
        },

        deleteFile: function(fileID, messageID) {
            Teamlab.removeCrmFile({ messageID: messageID }, fileID, {
                success: ASC.CRM.HistoryView.CallbackMethods.delete_file_from_event
            });
        },

        deleteFileFromEventLayout: function(fileID, messageID) {
            jq("#fileContent_" + fileID).remove();
            if (jq("#eventAttach_" + messageID + ">.dropDownContent>div").length == 0)
                jq(jq("#eventAttach_" + messageID).parent()).html("");

        },

        showAttachmentPanel: function(showPanel) {
            if(showPanel) {
                var showLink = jq("#attachButtonsPanel a:first");
                if(jq(showLink).hasClass('disabledLink')) {
                    return false;
                }
                jq('#attachOptions').show();
                jq(showLink).hide().next().show();
            } else {
                var hideLink = jq("#attachButtonsPanel a:last");
                if(jq(hideLink).hasClass('disabledLink')) {
                    return false;
                }
                jq('#attachOptions').hide();
                jq(hideLink).hide().prev().show();
            }
        },

        addEvent: function() {
            if (jq.trim(jq("#historyBlock textarea").val()) == "") return;

            if (!jq.browser.mobile && fileUploader.GetUploadFileCount() > 0) {
                jq("#" + fileUploader.ButtonID).css("visibility", "hidden");
                jq("#pm_upload_btn_html5").hide();
                fileUploader.Submit();
            }
            else {
                var data = _readDataEvent();
                Teamlab.addCrmHistoryEvent({}, data,
                {
                    success: ASC.CRM.HistoryView.CallbackMethods.add_event,
                    before: function(params) {
                        jq("#historyBlock div.action_block").hide();
                        jq("#historyBlock div.ajax_info_block").show();
                        jq("#historyBlock textarea, #historyBlock input, #historyBlock select").attr("disabled", true);
                    },
                    after: function(params) {
                        jq("#historyBlock div.action_block a.baseLinkButton").addClass("disableLink").removeClass("baseLinkButton");
                        jq("#historyBlock div.action_block").show();
                        jq("#historyBlock div.ajax_info_block").hide();
                        jq("#historyBlock textarea, #historyBlock input, #historyBlock select").attr("disabled", false);
                    }
                });
            }
        },
        addEventToHistoryLayout: function(params, event) {
            ASC.CRM.HistoryView.eventItemFactory(event);

            jq("#historyBlock textarea").val("");
            jq("#historyBlock textarea").focus();

            if (jq.browser.mobile) {
                jq("#historyTypeSelect option:first").attr("selected", true);
                jq("#historyTypeSelect").change();
                jq("#historyContactSelect option:first").attr("selected", true);
                jq("#historyContactSelect").change();
            } else {
                ASC.CRM.HistoryView.changeHistoryType('', jq("#historyTypePanel a.dropDownItem:first"));
                ASC.CRM.HistoryView.changeHistoryContact(-1, jq("#historyContactPanel a.dropDownItem:first"));
            }

            SelectedUsers.IDs.clear();
            SelectedUsers.Names.clear();
            jq("#selectedUsers div").remove();

            if (!jq.browser.mobile) {
                ASC.CRM.FileUploader.activateUploader();
                ASC.CRM.FileUploader.fileIDs.clear();
                ASC.CRM.HistoryView.showAttachmentPanel(false);
            }

            if (jq("#eventsTable tbody tr").length == 0) {
                jq("#emptyContentForEventsFilter").hide();
                jq("#eventsFilterContainer").show();
                jq("#eventsList").show();
                _resizeFilter();
            }
            jq("#historyRowTmpl").tmpl(event).prependTo("#eventsTable tbody");

            if (event.files != null)
                jq.dropdownToggle({ dropdownID: "eventAttach_" + event.id, switcherSelector: "#eventAttachSwither_" + event.id, addTop: 5 });
        },

        deleteEvent: function(id) {
            Teamlab.removeCrmHistoryEvent({}, id,
                {
                    success: ASC.CRM.HistoryView.CallbackMethods.delete_event,
                    before: function(params) { jq("#eventTrashImg_" + id).hide(); jq("#eventLoaderImg_" + id).show(); },
                    after: function(params) { jq("#eventLoaderImg_" + id).hide(); jq("#eventTrashImg_" + id).show(); }
                });
        },

        showDropdownPanel: function(swither, panel) {
            var left = jq(swither).find("a.linkMedium").width() - 26;
            jq.dropdownToggle().toggle(swither, panel, 5, left);
        },


        changeHistoryType: function(stringType, obj) {
            if (!jq.browser.mobile) {
                jq("#typeID").val(stringType);
                jq("#itemID").val(-1);
                jq("#historyTypePanel").hide();
                jq("#historyCasePanel").hide();
                jq("#historyDealPanel").hide();

                jq("#historyItemSwitcher a").text(CRMJSResources.Choose);

                if (stringType != "") {
                    jq("#historyTypeSwitcher a").text(jq(obj).text().trim());
                    jq("#historyItemSwitcher").show();
                } else {
                    jq("#historyTypeSwitcher a").text(CRMJSResources.Choose);
                    jq("#historyItemSwitcher").hide();
                    return;
                }

                switch (stringType) {
                    case "opportunity":
                        jq("#historyItemSwitcher").unbind("click").bind("click", function() {
                            ASC.CRM.HistoryView.showDropdownPanel('#historyItemSwitcher', 'historyDealPanel');
                        });
                        break;
                    case "case":
                        jq("#historyItemSwitcher").unbind("click").bind("click", function() {
                            ASC.CRM.HistoryView.showDropdownPanel('#historyItemSwitcher', 'historyCasePanel');
                        });
                        break;
                    default:
                        jq("#historyItemSwitcher").unbind("click");
                        break;
                };
            }
            else {
                jq("#typeID").val(jq("#historyTypeSelect option:selected").attr("type"));

                switch (jq("#historyTypeSelect option:selected").val()) {
                    case "1":
                        jq("#historyCaseSelect").hide();
                        jq("#historyDealSelect").show();
                        jq("#itemID").val(jq("#historyDealSelect option:selected").val());
                        break;
                    case "7":
                        jq("#historyDealSelect").hide();
                        jq("#historyCaseSelect").show();
                        jq("#itemID").val(jq("#historyCaseSelect option:selected").val());
                        break;
                    case "-1":
                        jq("#historyCaseSelect").hide();
                        jq("#historyDealSelect").hide();
                        jq("#itemID").val("0");
                        break;
                }
            }
        },

        changeHistoryItem: function(obj) {
            if (!jq.browser.mobile) {
                jq("#itemID").val(jq(obj).attr("itemid"));
                jq("#historyItemSwitcher a").text(jq(obj).text().trim());
                jq("#historyCasePanel").hide();
                jq("#historyDealPanel").hide();
            }
            else {
                switch (jq("#historyTypeSelect option:selected").val()) {
                    case "1":
                        jq("#itemID").val(jq("#historyDealSelect option:selected").val());
                        break;
                    case "7":
                        jq("#itemID").val(jq("#historyCaseSelect option:selected").val());
                        break;
                    case "-1":
                        jq("#itemID").val("0");
                        break;
                }
            }
        },

        changeHistoryContact: function(contact, obj) {
            if (!jq.browser.mobile) {
                jq("#contactID").val(contact);
                if (contact != -1)
                    jq("#historyContactSwitcher a").text(jq(obj).text().trim());
                else
                    jq("#historyContactSwitcher a").text(CRMJSResources.Choose);
                jq("#historyContactPanel").hide();
            } else {
                jq("#contactID").val(jq("#historyContactSelect option:selected").val());
            }
        },

        appendOption: function(container, option) {
            var contact;
            if (jq.browser.mobile)
                contact = jq("<option></option>").text(option.title).attr("value", option.value);
            else contact = jq("<a></a>").addClass("dropDownItem").text(option.title).attr("contactid", option.value)
                            .unbind("click").click(function() {
                                ASC.CRM.HistoryView.changeHistoryContact(option.value, jq(this));
                            });
            jq(container).append(contact);
            jq("#eventLinkToPanel").removeClass('empty-select');
            return jq(container);
        },

        removeOption: function(container, value) {
            if (jq.browser.mobile) {
                jq(container).find('option[value="' + value + '"]').remove();
                jq(container).find("option:first").attr("selected", true);
                jq(container).change();
            } else {
                jq(container).find('a[contactid="' + value + '"]').remove();
                ASC.CRM.HistoryView.changeHistoryContact(-1, jq(container).find("a:first"));
            }

            if (jq(container).children().length == 1)
                jq("#eventLinkToPanel").addClass('empty-select');

            return jq(container);
        },

        appendOptionToContact: function(option) {
            var container = jq.browser.mobile ? jq("#historyContactSelect") : jq("#historyContactPanel div.dropDownContent");
            return ASC.CRM.HistoryView.appendOption(container, option);
        },

        removeOptionFromContact: function(value) {
            var container = jq.browser.mobile ? jq("#historyContactSelect") : jq("#historyContactPanel div.dropDownContent");
            return ASC.CRM.HistoryView.removeOption(container, value);
        }
    };
})(jQuery);




ASC.CRM.ContactSelector = new function() {
    this.WatermarkClass = "crm-watermarked";

    var _initAutocomplete = function(input, parentTable, objName, obj, watermarkText) {
        jq(input).Watermark(watermarkText, ASC.CRM.ContactSelector.WatermarkClass);

        jq(input).autocomplete({
            minLength: 0,
            delay: 300,
            focus: function(event, ui) {
                if (typeof (ui.item) != "undefined")
                    jq(input).val(Encoder.htmlDecode(ui.item.title));
                return false;
            },
            select: function(event, ui) {
                if (typeof (obj.SelectItemEvent) == "undefined") {
                    obj.setContact(this, ui.item.id, ui.item.title, ui.item.img);
                    obj.showInfoContent(this);
                    return false;
                }
                else {
                    obj.SelectItemEvent(ui.item, this);
                    jq(this).val("");
                    return false;
                }
            },
            selectFirst: false,
            search: function(event, ui) { return true; },
            source: function(request, response) {
                var term = request.term;
                if (term in ASC.CRM.ContactSelector.Cache) {
                    response(ASC.CRM.ContactSelector.Cache[term]);
                    return;
                }

                AjaxPro.onLoading = function(b) {
                    if (b) {
                        jq("#searchImg_" + objName).hide();
                        jq("#loaderImg_" + objName).show();
                        //jq("#contactTitle_" + objName).attr("disabled", true);
                    }
                    else {
                        jq("#searchImg_" + objName).show();
                        jq("#loaderImg_" + objName).hide();
                        //jq("#contactTitle_" + objName).attr("disabled", false);
                        //jq("#contactTitle_" + objName).focus();
                    }
                }

                AjaxPro.ContactSelector.GetContactsByPrefix(term, obj.SelectorType, obj.EntityType, obj.EntityID, function(res) {
                    if (res.error != null) { alert(res.error.Message); return; }

                    var result = jq.parseJSON(res.value);
                    ASC.CRM.ContactSelector.Cache[term] = result;
                    response(ASC.CRM.ContactSelector.Cache[term]);
                    if (ASC.CRM.ContactSelector.Cache[term] == null || ASC.CRM.ContactSelector.Cache[term].length == 0) {
                        var $trObj = jq(parentTable).children('tbody').children('tr');
                        var width = $trObj.width() - ($trObj.children('td').length - 3) * 18 - 2;
                        jq("#noMatches_" + objName).css("width", width + "px");
                        jq("#noMatches_" + objName).show();
                    }
                });
            }
        }).data("autocomplete")._renderItem = function(ul, item) {
            jq("#noMatches_" + objName).hide();
            if (jq.inArray(item.id, obj.SelectedContacts) != -1) return; ;
            if (typeof (obj.ExcludedArrayIDs) != "undefined") {
                if (jq.inArray(item.id, obj.ExcludedArrayIDs) != -1) return;
            }

            var parent = item.parentID > 0 ? jq.format("<br/><span class='textMediumDescribe'>{0}: {1}</span>", CRMJSResources.Company, item.parentTitle) : "";

            if (obj.IsInPopup)
                jq(ul).css("position", "fixed");

            return jq("<li></li>").data("item.autocomplete", item)
                        .append(jq("<a href='javascript:void(0)'></a>").html(item.title + parent)).appendTo(ul);
        };

        jq(input).data("autocomplete")._resizeMenu = function() {
            var ul = this.menu.element;
            var $trObj = jq(parentTable).children('tbody').children('tr');
            var inputWidth = $trObj.width() - ($trObj.children('td').length - 3) * 18;
            ul.outerWidth(Math.max(inputWidth, this.element.outerWidth()));
        };

        jq(input).data("autocomplete")._suggest = function(a) {
            var b = this.menu.element.empty().zIndex(this.element.zIndex() + 1);
            this._renderMenu(b, a);
            this.menu.deactivate();
            this.menu.refresh();
            b.show();
            this._resizeMenu();
            b.position(jq.extend({ of: jq(this.element).parents("table:first") }, this.options.position));
        };
    };


    var _initClickEventHandler = function(input, descriptionText, objName) {
        jq(input).bind("click", function(e) {
            jq("div[id^='noMatches_" + objName + "']").hide();
            if (jq(this).val().trim() == descriptionText || jq(this).val().trim() == "")
                jq(this).autocomplete("search", "");
            else jq(this).autocomplete("search", jq(input).val().trim());
        });
    };

    var _initKeyUpEventHandler = function(input, descriptionText) {
        jq(input).bind("keyup", function(e) {
            if (jq(this).val().trim() == descriptionText || jq(this).val().trim() == "")
                jq(this).parents("table:first").find("img.crossButton").hide();
            else
                jq(this).parents("table:first").find("img.crossButton").show();
        });

    };

    this.ContactSelector = function(objName, selectorType, selectedContacts, descriptionText, deleteContactText, addContactText, excludedArrayIDs, isInPopup) {
        if (typeof (window[objName]) != "undefined") return window[objName];
        this.ObjName = objName;
        this.SelectorType = selectorType;
        this.SelectedContacts = selectedContacts;
        this.ExcludedArrayIDs = excludedArrayIDs;
        this.EntityType = jq("#entityType_" + objName).val();
        this.EntityID = jq("#entityID_" + objName).val();
        this.DescriptionText = descriptionText;
        this.DeleteContactText = deleteContactText;
        this.AddContactText = addContactText;
        this.CrossButtonEventClick;
        this.SelectItemEvent;
        this.IsInPopup = isInPopup;
        this.newCompanyTitleWatermark = "";
        this.newContactFirstNameWatermark = "";
        this.newContactLastNameWatermark = "";
        var CurrentItem = this;

        jq(document).ready(function() {
            ASC.CRM.ContactSelector.Cache = {};
            var $selectorRows = jq("#selector_" + objName).children("div:first").children("div.contactSelector-item");

            jq($selectorRows[$selectorRows.length - 1]).addClass("withPlus");

            jq("#selector_" + objName).find("input[id^=contactTitle_]").each(function(index) {
                _initAutocomplete(this, jq(this).parents("table:first"), objName + '_' + index, CurrentItem, descriptionText);
                _initClickEventHandler(this, CurrentItem.DescriptionText, CurrentItem.ObjName);
                _initKeyUpEventHandler(this, CurrentItem.DescriptionText);

            });
        });

        this.changeContact = function(objID) {
            var id = parseInt(jq("#contactID_" + objID).val());
            var index = jq.inArray(id, CurrentItem.SelectedContacts);
            if (index != -1)
                CurrentItem.SelectedContacts.splice(index, 1);

            jq(window).trigger("editContactInSelector", [jq("#item_" + objID), this.ObjName]);
            //CurrentItem.setContact(jq("#contactTitle_" + objID), 0, "", "");
            jq("#contactID_" + objID).val(0);
            CurrentItem.showSelectorContent(jq("#contactTitle_" + objID));
        };


        this.deleteContact = function(objID) {
            var id = parseInt(jq("#contactID_" + objID).val());
            var index = jq.inArray(id, CurrentItem.SelectedContacts);
            if (index != -1)
                CurrentItem.SelectedContacts.splice(index, 1);

            jq(window).trigger("deleteContactFromSelector", [jq("#item_" + objID), this.ObjName]);

            jq("#item_" + objID).remove();
            var $selectorRows = jq("#selector_" + CurrentItem.ObjName).children("div:first").children("div.contactSelector-item");
            $selectorRows.filter(".withPlus").removeClass("withPlus");
            jq($selectorRows[$selectorRows.length - 1]).addClass("withPlus");
        };

        this.crossButtonEventClick = function(objID) {
            jq("#selectorContent_" + objID).children(".contactSelector-inputContainer").find(".crossButton").hide();

            if (typeof (CurrentItem.CrossButtonEventClick) == "undefined") {
                var id = parseInt(jq("#contactID_" + objID).val());
                var index = jq.inArray(id, CurrentItem.SelectedContacts);
                if (index != -1)
                    CurrentItem.SelectedContacts.splice(index, 1);
                jq("#contactTitle_" + objID).val("").blur();
                jq("#contactID_" + objID).val(0);
                jq("#noMatches_" + objID).hide();
            }
            else {
                CurrentItem.CrossButtonEventClick();
            }
        };

        this.setContact = function(obj, id, title, img) {
            if (jq(obj).length == 0) return;
            var objID = jq(obj).attr('id').replace("contactTitle_", "");

            jq(obj).val(Encoder.htmlDecode(title));

            if (img != "") {
                jq("#infoContent_" + objID).find('img:first').attr("src", img);
            }
            jq("#infoContent_" + objID).find('b:first').html(title);
            jq("#contactID_" + objID).val(id);
            if (jq.inArray(id, CurrentItem.SelectedContacts) == -1 && id != 0)
                CurrentItem.SelectedContacts.push(id);
        };

        this.showInfoContent = function(obj) {
            //jq(obj).val(CurrentItem.)
            var objID = jq(obj).attr('id').replace("contactTitle_", "");
            jq('#selectorContent_' + objID).hide();
            jq("#newContactContent_" + objID).hide();
            jq('#infoContent_' + objID).show();
        };

        this.showSelectorContent = function(obj) {
            var objID = jq(obj).attr('id').replace("contactTitle_", "");

            if (jq(obj).val().trim() == descriptionText || jq(obj).val().trim() == "")
                jq(obj).parents("table:first").find("img.crossButton").hide();
            else
                jq(obj).parents("table:first").find("img.crossButton").show();

            jq('#infoContent_' + objID).hide();
            jq("#newContactContent_" + objID).hide();
            jq('#selectorContent_' + objID).show();
            jq(window).trigger("afterResetSelectedContact", [jq("#item_" + objID), this.ObjName]);
        };

        this.AddNewSelector = function(addSelectorLink) {
            if (addSelectorLink.hasClass("disabledLink")) return;

            addSelectorLink.addClass("disabledLink");
            AjaxPro.onLoading = function(b) { };

            var index = 0;
            var lastSelector = jq("#selector_" + CurrentItem.ObjName + " input[id^=contactTitle_]:last");
            if (jq(lastSelector).length > 0)
                var index = parseInt(jq(lastSelector).attr("id").replace("contactTitle_" + CurrentItem.ObjName + "_", "")) + 1;


            AjaxPro.ContactSelector.AddNewSelector(CurrentItem.ObjName, index, CurrentItem.DescriptionText, CurrentItem.DeleteContactText, CurrentItem.AddContactText, function(res) {
                if (res.error != null) { alert(res.error.Message); return; }

                jq("#selector_" + CurrentItem.ObjName + " div:first").append(res.value);

                var $selectorRows = jq("#selector_" + CurrentItem.ObjName).children("div:first").children("div.contactSelector-item");

                $selectorRows.filter(".withPlus").removeClass("withPlus");
                jq($selectorRows[$selectorRows.length - 1]).addClass("withPlus");

                var input = jq("#contactTitle_" + CurrentItem.ObjName + "_" + index);
                _initAutocomplete(input, jq(input).parents("table:first"), CurrentItem.ObjName + '_' + index, CurrentItem, CurrentItem.DescriptionText);
                _initClickEventHandler(input, CurrentItem.DescriptionText, CurrentItem.ObjName);
                _initKeyUpEventHandler(input, CurrentItem.DescriptionText);
                addSelectorLink.removeClass("disabledLink");
            });
        };

        this.quickSearch = function(objID) {
            var input = jq("#contactTitle_" + objID);
            if (jq(input).val().trim() == CurrentItem.DescriptionText || jq(input).val().trim() == "")
                jq(input).autocomplete("search", "");
            else jq(input).autocomplete("search", jq(input).val().trim());
        };

        this.showNewCompany = function(objID) {
            jq("#newContactContent_" + objID + " input").val("");
            jq("#hiddenIsCompany_" + objID).val("true");
            jq("#noMatches_" + objID).hide();
            jq("#infoContent_" + objID).hide();
            jq("#selectorContent_" + objID).hide();
            jq("#newContactImg_" + objID).hide();
            jq("#newContactFirstName_" + objID).hide();
            jq("#newContactLastName_" + objID).hide();
            jq("#newContactContent_" + objID).show();
            jq("#newCompanyImg_" + objID).show();
            jq("#newCompanyTitle_" + objID).val(jq("#contactTitle_" + objID).val()).show().focus();
            jq("#newCompanyTitle_" + objID).Watermark(this.newCompanyTitleWatermark, ASC.CRM.ContactSelector.WatermarkClass);
        };

        this.showNewContact = function(objID) {
            jq("#newContactContent_" + objID + " input").val("");
            jq("#hiddenIsCompany_" + objID).val("false");
            jq("#noMatches_" + objID).hide();
            jq("#infoContent_" + objID).hide();
            jq("#selectorContent_" + objID).hide();
            jq("#newCompanyImg_" + objID).hide();
            jq("#newCompanyTitle_" + objID).hide();
            jq("#newContactContent_" + objID).show();
            jq("#newContactImg_" + objID).show();
            jq("#newContactLastName_" + objID).Watermark(this.newContactLastNameWatermark, ASC.CRM.ContactSelector.WatermarkClass);
            jq("#newContactLastName_" + objID).show();
            jq("#newContactFirstName_" + objID).val(jq("#contactTitle_" + objID).val()).show().focus();
            jq("#newContactFirstName_" + objID).Watermark(this.newContactFirstNameWatermark, ASC.CRM.ContactSelector.WatermarkClass);
        };

        this.acceptNewContact = function(objID) {

            var isCompany = jq("#hiddenIsCompany_" + objID).val() == "true";
            var compName = jq("#newCompanyTitle_" + objID).length > 0 ? jq("#newCompanyTitle_" + objID).val().trim() : "";
            var firstName = jq("#newContactFirstName_" + objID).length > 0 ? jq("#newContactFirstName_" + objID).val().trim() : "";
            var lastName = jq("#newContactLastName_" + objID).length > 0 ? jq("#newContactLastName_" + objID).val().trim() : "";
            var obj = this;
            var input = jq("#contactTitle_" + objID);

            var isValid = true;

            if (isCompany && (compName == "" || jq("#newCompanyTitle_" + objID).hasClass(ASC.CRM.ContactSelector.WatermarkClass))) {
                jq("#newCompanyTitle_" + objID).addClass("requiredInputError");
                isValid = false;
            }
            else
                jq("#newCompanyTitle_" + objID).removeClass("requiredInputError");

            if (!isCompany && (firstName == "" || jq("#newContactFirstName_" + objID).hasClass(ASC.CRM.ContactSelector.WatermarkClass))) {
                jq("#newContactFirstName_" + objID).addClass("requiredInputError");
                isValid = false;
            }
            else
                jq("#newContactFirstName_" + objID).removeClass("requiredInputError");

            if (!isCompany && (lastName == "" || jq("#newContactLastName_" + objID).hasClass(ASC.CRM.ContactSelector.WatermarkClass))) {
                jq("#newContactLastName_" + objID).addClass("requiredInputError");
                isValid = false;
            }
            else
                jq("#newContactLastName_" + objID).removeClass("requiredInputError");

            if (!isValid)
                return false;

            AjaxPro.ContactSelector.AddNewContact(isCompany, compName, firstName, lastName, function(res) {
                if (res.error != null) { return; }

                var contactInfo = jq.parseJSON(res.value);

                if (typeof (obj.SelectItemEvent) == "undefined") {
                    jq("#infoContent_" + objID).show();
                    jq("#selectorContent_" + objID).hide();
                    jq("#newContactContent_" + objID).hide();
                    obj.setContact(input, contactInfo.id, contactInfo.title, contactInfo.img);
                    obj.showInfoContent(input);
                    if (isCompany)
                        jq("#infoContent_" + objID + " img:first").attr("src", jq("#newCompanyImg_" + objID).attr("src"));
                    else
                        jq("#infoContent_" + objID + " img:first").attr("src", jq("#newContactImg_" + objID).attr("src"));
                }
                else {
                    obj.showSelectorContent(input);
                    jq(input).val("");
                    obj.SelectItemEvent(contactInfo, input);
                }

            });
        };

        this.rejectNewContact = function(objID) {
            jq("#infoContent_" + objID).hide();
            jq("#selectorContent_" + objID).show();
            jq("#newContactContent_" + objID).hide();
            jq("#noMatches_" + objID).show();
            jq("#newContactContent_" + objID + " input").removeClass("requiredInputError");
        };
    };


};

ASC.CRM.CategorySelector = new function() {

    this.CategorySelectorPrototype = function(objName) {

        this.ObjName = objName;
        this.Me = function(){ return jq("#" + this.ObjName); };
        this.CategoryTitle = jq.browser.mobile == true ?
            jq('select[id='+this.ObjName+'_select] option:selected', this.Me()).text() :
            jq('input[id='+this.ObjName+'_categoryTitle]', this.Me()).val();

        this.CategoryID = jq.browser.mobile == true  ?
            parseInt(jq('select[id='+this.ObjName+'_select] option:selected', this.Me()).val()) :
            parseInt(jq('input[id='+this.ObjName+'_categoryID]', this.Me()).val());

        this.changeContact = function(obj)
        {
            if(!jq.browser.mobile) {
                var id = parseInt(jq(obj).attr("id").split("_")[2]);
                var title = jq("div", jq(obj)).text();
                this.setContact(id, title);
                this.hideSelectorContent();
            } else {
                var id = parseInt(jq(obj).val());
                var title = jq(obj).text();
                this.setContact(id, title);
            }
        };

        this.setContact = function(id, title)
        {
            this.CategoryID = id;
            this.CategoryTitle = title;

            if(!jq.browser.mobile) {
                jq('input[id=' + this.ObjName + '_categoryID]', this.Me()).val(id);
                jq('input[id=' + this.ObjName + '_categoryTitle]', this.Me()).val(title);
            } else {
                jq("option[id=" + this.ObjName + "_category_" + id + "]", this.Me()).attr("selected", true);
            }
        };

        this.hideSelectorContent = function()
        {
            jq('div[id='+this.ObjName+'_categoriesContainer]', this.Me()).hide();
        };

        this.showSelectorContent = function()
        {
            jq('div[id='+this.ObjName+'_categoriesContainer]', this.Me()).toggle();
        };

        this.getRowByContactID = function(id)
        {
            if(id > 0)
                return jq.browser.mobile ?
                    jq("option[id="+this.ObjName+"_category_"+id+"]", this.Me()) :
                    jq("div[id="+this.ObjName+"_category_"+id+"]", this.Me());
            else
                return jq.browser.mobile ?
                    jq("option[id^=" + this.ObjName + "_category_]:first", this.Me()) :
                    jq("div[id^=" + this.ObjName + "_category_]:first", this.Me());
        };
    };
};

ASC.CRM.TagView = (function($) {

    return {
        init: function(targetEntityType) {
            ASC.CRM.TagView.TargetEntityType = targetEntityType;

            if (jq("#addTagDialog a.dropDownItem").length == 0) {
                jq("#addTagDialog .h_line").hide();
            }

            jq.dropdownToggle({ dropdownID: 'addTagDialog', switcherSelector: '#addNewTag', addTop: 5, addLeft: jq("#addNewTag").width() - 35 });

            jq("#addTagDialog input").focus().unbind("keyup").keyup(function(e) {
                var code;
                if (!e) {
                    e = event;
                }
                if (e.keyCode) code = e.keyCode;
                else if (e.which) code = e.which;

                if (code == 13) {
                    if (jq("#addTagDialog input").val().trim() == "") return;
                    ASC.CRM.TagView.addNewTag();
                }

            });
        },

        addNewTag: function() {
            var text = jq("#addTagDialog input").val().trim();
            if (text != null && text != "") {

                jq("#tagContainer .adding_tag_loading").show();
                jq("#addTagDialog").hide();

                AjaxPro.TagView.AddTag(jq.getURLParam("id"), ASC.CRM.TagView.TargetEntityType, text, function(response) {
                    jq("#tagContainer .adding_tag_loading").hide();
                    if (response.error == null) {
                        jq("#taqTmpl").tmpl({ "tagText": text }).appendTo("#tagContainer div:first");
                        jq("#addTagDialog input").val("");
                    }

                });
            }
        },

        deleteTag: function($elementHTML) {
            var text = $elementHTML.children(".tag_title").text();
            AjaxPro.TagView.DeleteTag(jq.getURLParam("id"), ASC.CRM.TagView.TargetEntityType, text, function(response) {
                if (response.error == null) {
                    $elementHTML.remove();
                    jq("#addTagDialog .h_line").show();
                    jq("#tagInAllTagsTmpl").tmpl({ "tagText": text }).appendTo("#addTagDialog .dropDownContent");
                }
            });
        },

        addExistingTag: function(element) {
            var tagTitle = jq(element).text();

            jq("#tagContainer .adding_tag_loading").show();
            jq("#addTagDialog").hide();

            AjaxPro.TagView.AddTag(jq.getURLParam("id"), ASC.CRM.TagView.TargetEntityType, tagTitle, function(response) {
                jq("#tagContainer .adding_tag_loading").hide();
                if (response.error == null) {
                    jq("#taqTmpl").tmpl({ "tagText": tagTitle }).appendTo("#tagContainer div:first");

                    jq(element).remove();
                    if (jq("#addTagDialog a.dropDownItem").length == 0) {
                        jq("#addTagDialog .h_line").hide();
                    }
                    jq("#addTagDialog input").val("");
                }
            });
        }
    }
})(jQuery);


ASC.CRM.ImportContacts = (function($) {
    var _CSVFileURI;
    var _curIndexSampleRow = 0;
    var _ajaxUploader;
    var _entityType;
    var _sampleRowCache = new Object();
	var _settingsBase = new Object();


    var _refreshSampleRowItems = function(data) {

                var resultData = jq.parseJSON(data);

                var sampleRow = resultData.data;

                var sampleRowColumnIndex = 0;

                  jq("#columnMapping tbody tr td").each(
                    function(index) {

                        if ((index+1) % 3 == 0)
                         jq(this).text(sampleRow[sampleRowColumnIndex++]);
                    }
                );

               if (resultData.isMaxIndex)
                   jq("#nextSample").hide().prev().hide();

    };

    var _getSampleRow = function() {

        AjaxPro.onLoading = function(b) {
            if (b) {
                jq("#columnMapping thead tr td:last span:last").block();
            }
            else {
                jq("#columnMapping thead tr td:last span:last").unblock();
            }
        };

        var sampleRowCacheKey = _CSVFileURI + '' + _curIndexSampleRow;

        if (_sampleRowCache[sampleRowCacheKey]) {

            _refreshSampleRowItems(_sampleRowCache[sampleRowCacheKey]);

            return;
        }

        AjaxPro.Utils.ImportFromCSV.GetSampleRow(_CSVFileURI, _curIndexSampleRow, jq.toJSON(_settingsBase),
            function(result) {

                if (result.error != null) {
                    alert(result.error.Message);
                    return;
                }

                _sampleRowCache[sampleRowCacheKey] = result.value;

                _refreshSampleRowItems(result.value);

            }
          );
    };

    var _nextStep = function(step) {


        jq(jq.format("#importFromCSVSteps dd:eq({0})", step - 1)).hide();
        jq(jq.format("#importFromCSVSteps dt:eq({0})", step - 1)).hide();
        jq(jq.format("#importFromCSVSteps dd:eq({0})", step)).show();
        jq(jq.format("#importFromCSVSteps dt:eq({0})", step)).show();

    };
	
    return {
        init: function(entityType) {

            _entityType = entityType;

            if (_entityType == 0)
                jq("#removingDuplicatesBehaviorPanel").show();
            else
                jq("#removingDuplicatesBehaviorPanel").hide();

            jq("#columnSelectorTemplate").tmpl(columnSelectorData).appendTo("#columnSelectorBase");
            jq("#columnSelectorBase").find("option[name=is_header]").each(function() {
                var curItem = jq(this);

                curItem.nextUntil('option[name=is_header]').wrapAll(jq("<optgroup>").attr("label", curItem.val()));

                curItem.remove();
            });

         _ajaxUploader = new AjaxUpload('uploadCSVFile', {
                            action: 'ajaxupload.ashx?type=ASC.Web.CRM.Controls.Common.ImportFileHandler,ASC.Web.CRM',
                            autoSubmit: false,
                            onChange: function(file, extension) {

                                if (extension[0].toLowerCase() != "csv")
                                {
                                    alert(CRMJSResources.ErrorMessage_NotSupportedFileFormat);

                                    return false;
                                }

                                var messageContainer = jq("#uploadCSVFile").prev();
                                messageContainer.show();
                                messageContainer.text(jq.format(CRMJSResources.SelectedCSVFileLabel, file));

                                jq("#uploadCSVFile").removeClass("import_button").addClass("linkEditButton").text(CRMJSResources.Change);

                                jq("#importFromCSVSteps dd:first div.action_block a:first").removeClass("disableLinkButton").addClass("baseLinkButton");

                                return true;

                            },
                            onSubmit: function(file, extension) {

                            	_settingsBase = {
                            		"has_header": jq("#ignoreFirstRow").is(":checked"),
                                    "delimiter_character":jq("#delimiterCharacterSelect option:selected").val(),
                                    "encoding":jq("#encodingSelect option:selected").val(),
                                    "quote_character":jq("#quoteCharacterSelect option:selected").val()
                            	};

                            	  this._settings.data["importSettings"] = jq.toJSON(_settingsBase);
                                  jq("#importFromCSVSteps .action_block").hide();
                                  jq("#importFromCSVSteps div.ajax_info_block").show();



                              },
                             onComplete: function(file, response) {
                                var responseObj = jq.parseJSON(response);

                                if (!responseObj.Success){
                                    alert(responseObj.Error);
                                    return;
                                }

                                var responseData = jq.parseJSON(jQuery.base64.decode(responseObj.Data));

                                _CSVFileURI = responseData.assignedPath;
                             	
                             	 if (responseData.isMaxIndex)
                             	 	jq("#prevSample").parent().hide();
                             	 else 	
                             	    jq("#prevSample").parent().show();
                             	 

                                var stepTwoContent = jq("#columnMapping tbody");

                                stepTwoContent.html("");

                                jq("#columnMappingTemplate").tmpl(responseData, {
                                  renderSelector: function(rowIndex) {
                                    var columnSelector = jq("#columnSelectorBase").clone();

                                      columnSelector.attr("id", jq.format("columnSelector_{0}", rowIndex));
                                      columnSelector.show();

                                    return jq("<div>").append(columnSelector).html();

                                  }
                                }).appendTo(stepTwoContent);

                                var selectItems = jq("#columnMapping select");

                                 selectItems.each(function() {
                                   
                                 try {
                                    var curSelect = jq(this);
                                 	var columnHeader = curSelect.parent().prev().text().trim();
                                 	
                                 	if (jq.browser.msie) {
                                 		var leftBracketColumnCount = jq(columnHeader.match( /\(/g)).length;
                                 		var rightBracketColumnCount = jq(columnHeader.match( /\)/g)).length;
                                 		
                                 		if (leftBracketColumnCount != rightBracketColumnCount) return;
                                 		
                                 	}
                                 	
                                 	var findedItem = curSelect.find( jq.format("option:contains('{0}'):first", columnHeader)).attr("selected", true);
                                   
                                    }
                                     catch(e) {

                                     

                                    }

                                 });

                                selectItems.live("change", function() {
                                    var curSelector = jq(this);

                                    if (curSelector.find("option:first").is(":selected"))
                                        curSelector.parents("td").addClass("missing");
                                    else
                                        curSelector.parents("td").removeClass("missing");

                                });

                                selectItems.change();

                                jq("#uploadCSVFile").removeClass("linkEditButton")
                                                    .addClass("import_button");


                                 jq("#uploadCSVFile").text(CRMJSResources.SelectCSVFileButton);
                                                     jq("#uploadCSVFile").prev().hide();

                                _nextStep(1);
                                jq("#importFromCSVSteps .action_block").show();
                                jq("#importFromCSVSteps div.ajax_info_block").hide();
                             }});

        },
        prevStep : function(step) {

            jq(jq.format("#importFromCSVSteps dd:eq({0})", step + 1)).hide();
            jq(jq.format("#importFromCSVSteps dt:eq({0})", step + 1)).hide();

            jq(jq.format("#importFromCSVSteps dd:eq({0})", step)).show();
            jq(jq.format("#importFromCSVSteps dt:eq({0})", step)).show();

            if (step != 0) return;
        	
               jq("#importFromCSVSteps dd:first div.action_block a:first").addClass("disableLinkButton").removeClass("baseLinkButton");

//               jq("#delimiterCharacterSelect option[value=58]").attr("selected", "selected");
//               jq("#encodingSelect option[value=65001]").attr("selected", "selected");
//               jq("#quoteCharacterSelect option[value=34]").attr("selected", "selected");
//               
//               if (jq("#isPrivate").is(":checked"))
//                 jq("#isPrivate").click();

//               jq("#removingDuplicatesBehaviorPanel input[name=removingDuplicatesBehavior]:last").attr("checked", "checked");
//               jq("#ignoreFirstRow").attr("checked", "checked");
            	
                        

        },
        getPreviewImportData: function() {

            var columnMappingSelector = jq("#columnMapping select");

            var firstNameOptions = columnMappingSelector.find("option[name=firstName]:selected");
            var lastNameOptions = columnMappingSelector.find("option[name=lastName]:selected");
            var companyNameOptions = columnMappingSelector.find("option[name=companyName]:selected");

            if ((firstNameOptions.length == 0 || lastNameOptions.length == 0)  &&  companyNameOptions.length == 0) {
                alert(CRMJSResources.ErrorNotMappingBasicColumn);
                return;
            }

            if (!((firstNameOptions.length == 1 && lastNameOptions.length == 1) || (companyNameOptions.length == 1))) {
                alert(CRMJSResources.ErrorMappingMoreBasicColumn);
                return;
            }

            var companyNameColumnIndex = 0;
            var firstNameColumnIndex = 0;
            var lastNameColumnIndex = 0;

            if (companyNameOptions.length == 1) {

                companyNameColumnIndex = columnMappingSelector.index(jq(companyNameOptions[0]).parents("select"));
            }

            if (firstNameOptions.length == 1 && lastNameOptions.length == 1)
            {

                firstNameColumnIndex =  columnMappingSelector.index(jq(firstNameOptions[0]).parents("select"));
                lastNameColumnIndex =  columnMappingSelector.index(jq(lastNameOptions[0]).parents("select"));
            }


            AjaxPro.onLoading = function(b) {
                if (b) {
                    jq("#importFromCSVSteps .action_block").hide();
                    jq("#importFromCSVSteps div.ajax_info_block").show();
                }
                else {
                    jq("#importFromCSVSteps .action_block").show();
                    jq("#importFromCSVSteps div.ajax_info_block").hide();
                }
            };

            AjaxPro.Utils.ImportFromCSV.GetPreviewImportData(_CSVFileURI, companyNameColumnIndex,
                firstNameColumnIndex,
                lastNameColumnIndex,
                function(result) {
                       if (result.error != null) {

                           alert(result.error.Message);

                           return;

                       }

                var resultData = jq.parseJSON(result.value);

                jq("#previewImportData tbody tr").remove();

                jq("#previewImportDataTemplate").tmpl(resultData).appendTo("#previewImportData tbody");

                    jq("#importFromCSVSteps dd:eq(2) span:eq(1)").text(jq.format(CRMJSResources.ImportFromCSVStepThreeDescription, jq("#previewImportData tbody tr").length));

                    _nextStep(2);

               }
            );

        },

        getColumnMapping: function() {
            var result = { };
            jq("#columnMapping select").each(function() {
                var name = jq(this).find("option:selected").attr("name").trim();
                if (name == "" || name == -1) return true;
                if (typeof result[name] == "undefined") {
                    result[name] = new Array();
                }

                var idParts = this.id.split("_");

                if (idParts.length < 2) return true;

                result[name].push(idParts[1]);

            });
            return result;
        },

        startImport: function() {


            var columnMapping = ASC.CRM.ImportContacts.getColumnMapping();

            for (var p in columnMapping) {
               if (columnMapping[p].length > 1) {
                     alert(CRMJSResources.ErrorMappingMoreBasicColumn);

                  return;
               }
            }

            var columnMappingSelector = jq("#columnMapping select");

            switch (_entityType)
            {
                case 0:  //  Contact
                   {
                    var firstNameOptions = columnMappingSelector.find("option[name=firstName]:selected");
                    var lastNameOptions = columnMappingSelector.find("option[name=lastName]:selected");
                    var companyNameOptions = columnMappingSelector.find("option[name=companyName]:selected");

                    if ((firstNameOptions.length == 0 || lastNameOptions.length == 0)  &&  companyNameOptions.length == 0) {
                        alert(CRMJSResources.ErrorContactNotMappingBasicColumn);
                        return;
                    }
                   }
                    break;
                case 1:  //  Opportunity
                   {

                    var titleOptions = columnMappingSelector.find("option[name=title]:selected");
                    var responsibleOptions = columnMappingSelector.find("option[name=responsible]:selected");

                    if (titleOptions.length == 0 || responsibleOptions.length == 0) {
                        alert(CRMJSResources.ErrorOpportunityNotMappingBasicColumn);
                        return;
                    }

                  }
                    break;
                case 3:  //  Task
                 {
                  var titleOptions = columnMappingSelector.find("option[name=title]:selected");
                  var dueDateOptions = columnMappingSelector.find("option[name=due_date]:selected");
                  var responsibleOptions = columnMappingSelector.find("option[name=responsible]:selected");

                      if (titleOptions.length == 0 ||
                          dueDateOptions.length == 0 ||
                       responsibleOptions.length == 0)
                       {
                        alert(CRMJSResources.ErrorTaskNotMappingBasicColumn);
                        return;

                    }
                 }

                    break;
                case 7:  //  Case
                    var titleOptions = columnMappingSelector.find("option[name=title]:selected");

                    if (titleOptions.length == 0) {

                        alert(CRMJSResources.ErrorCasesNotMappingBasicColumn);
                        return;

                    }


                    break;
                default:

                    break;
            }

        	var importSetting = _settingsBase;

        	importSetting["column_mapping"] = columnMapping;
         

            if (_entityType != 3) {
                var privateUsers = new Array(SelectedUsers.CurrentUserID);

                for(var i=0;i<SelectedUsers.IDs.length;i++)
                   privateUsers.push(SelectedUsers.IDs[i]);

                importSetting["access_list"] = privateUsers;
                importSetting["is_private"] = jq("#isPrivate").is(":checked");

            }

            if (_entityType == 0)
               importSetting.removing_duplicates_behavior =  jq("input[name='removingDuplicatesBehavior']:checked").val();

            AjaxPro.onLoading = function(b) {
                if (b) {
                    jq("#importFromCSVSteps .action_block").hide();
                    jq("#importFromCSVSteps div.ajax_info_block").show();
                }
                else {
                    jq("#importFromCSVSteps .action_block").show();
                    jq("#importFromCSVSteps div.ajax_info_block").hide();

                }
            };


            AjaxPro.Utils.ImportFromCSV.StartImport(_entityType, _CSVFileURI, jq.toJSON(importSetting),
                function(result) {
                    if (result.error != null) {

                        alert(result.error.Message);

                        return;
                    }

                    jq("#importFromCSVSteps").hide();
                    jq("#importStartedFinalMessage").show();
                    ASC.CRM.ImportContacts.checkImportStatus(true);

                });

        },
        startUploadCSVFile: function() {

            _ajaxUploader.submit();
        },
        getPrevSampleRow:function() {

            _curIndexSampleRow--;

            jq("#nextSample").show().prev().show();

            if (_curIndexSampleRow == 0)
                jq("#prevSample").hide().next().hide();

            _getSampleRow();

        },
        getNextSampleRow: function() {
            _curIndexSampleRow++;

            jq("#prevSample").show().next().show();
            _getSampleRow();
        },

        checkImportStatus: function(isFirstVisit) {

            AjaxPro.onLoading = function(b) {};

            if (isFirstVisit) {
                jq("#importStartedFinalMessage div.progress_box div.progress").css("width", "0%");
                jq("#importStartedFinalMessage div.progress_box span.percent").text("0%");
                jq("#importErrorBox").hide();
                jq("#importStartedFinalMessage div.progressErrorBox").html("");
            }

            AjaxPro.Utils.ImportFromCSV.GetStatus(_entityType, function(res) {
                if (res.error != null) { alert(res.error.Message); return false; }

                if (res.value == null) {
                    jq("#importStartedFinalMessage div.progress_box div.progress").css("width", "100%");
                    jq("#importStartedFinalMessage div.progress_box span.percent").text("100%");
                    jq("#importErrorBox").hide();
                    jq("#importStartedFinalMessage div.progressErrorBox").html("");
                    return false;
                }

                jq("#importStartedFinalMessage div.progress_box div.progress").css("width", parseInt(res.value.Percentage) + "%");
                jq("#importStartedFinalMessage div.progress_box span.percent").text(parseInt(res.value.Percentage) + "%");

                if (res.value.Error != null && res.value.Error != "") {
                    ASC.CRM.ImportContacts.buildErrorList(res);
                } else {
                    if(!res.value.IsCompleted) {
                        setTimeout("ASC.CRM.ImportContacts.checkImportStatus(false)", 3000);
                    }
                }
            });
        },

        buildErrorList: function (res)
        {
            var mess = "error";
            switch (typeof res.value.Error) {
            case "object":
                mess = res.value.Error.Message + "<br/>";
                break;
            case "string":
                mess = res.value.Error;
                break;
            }
            var link = jq("#importStartedFinalMessage #importLinkBox a").clone().removeClass("baseLinkButton");
            jq("#importStartedFinalMessage div.progressErrorBox")
                .html(jq("<div></div>").addClass("redText").html(mess))
                .append(link);
            jq("#importStartedFinalMessage #importErrorBox").show();
            jq("#importStartedFinalMessage #importLinkBox").hide();
        }
    };
})(jQuery);