if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = (function() { return {} })();

ASC.CRM.myFilter = {
    filterId: 'dealsAdvansedFilter',
    idFilterByContact: 'contactID',
    idFilterByParticipant: 'participantID',

    type: 'custom-contact',
    hiddenContainerId: 'hiddenBlockForContactSelector',
    containerId: 'selector_contactSelectorForFilter',

    onSelectContact: function(item) {
        var input = jq("#contactTitle_" + contactSelectorForFilter.ObjName + "_0");
        jq("#infoContent_" + contactSelectorForFilter.ObjName + "_0").show();
        jq("#selectorContent_" + contactSelectorForFilter.ObjName + "_0").hide();
        contactSelectorForFilter.setContact(input, item.id, item.title, item.img);
        contactSelectorForFilter.showInfoContent(input);
        contactSelectorForFilter.SelectedContacts = [];
        contactSelectorForFilter.SelectedContacts.push(item.id);

        if (ASC.CRM.myFilter.filterId) {
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter(ASC.CRM.myFilter.idFilterByContact, { id: item.id, displayName: item.title, photoUrl: item.img, value: jq.toJSON([item.id, false]) });
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter('resize');
        }
    },

    onSelectParticipant: function(item) {
        var input = jq("#contactTitle_" + contactSelectorForFilter.ObjName + "_0");
        jq("#infoContent_" + contactSelectorForFilter.ObjName + "_0").show();
        jq("#selectorContent_" + contactSelectorForFilter.ObjName + "_0").hide();
        contactSelectorForFilter.setContact(input, item.id, item.title, item.img);
        contactSelectorForFilter.showInfoContent(input);
        contactSelectorForFilter.SelectedContacts = [];
        contactSelectorForFilter.SelectedContacts.push(item.id);

        if (ASC.CRM.myFilter.filterId) {
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter(ASC.CRM.myFilter.idFilterByParticipant, { id: item.id, displayName: item.title, photoUrl: item.img, value: jq.toJSON([item.id, true]) });
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter('resize');
        }
    },


    createFilterByContact: function(filter) {
        var o = document.createElement('div');
        o.innerHTML = [
      '<div class="default-value">',
        '<span class="title">',
          filter.title,
        '</span>',
        '<span class="selector-wrapper">',
          '<span class="contact-selector"></span>',
        '</span>',
        '<span class="btn-delete"></span>',
      '</div>'
    ].join('');
        return o;
    },
    customizeFilterByContact: function($container, $filteritem, filter) {
        contactSelectorForFilter.SelectItemEvent = ASC.CRM.myFilter.onSelectContact;

        if (ASC.CRM.myFilter.containerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo($filteritem.find('span.contact-selector:first'));
        }
    },
    destroyFilterByContact: function($container, $filteritem, filter) {
        if (ASC.CRM.myFilter.containerId && ASC.CRM.myFilter.hiddenContainerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo(jq('#' + ASC.CRM.myFilter.hiddenContainerId));
            contactSelectorForFilter.changeContact('contactSelectorForFilter_0');
        }
    },


    createFilterByParticipant: function(filter) {
        var o = document.createElement('div');
        o.innerHTML = [
      '<div class="default-value">',
        '<span class="title">',
          filter.title,
        '</span>',
        '<span class="selector-wrapper">',
          '<span class="contact-selector"></span>',
        '</span>',
        '<span class="btn-delete"></span>',
      '</div>'
    ].join('');
        return o;
    },
    customizeFilterByParticipant: function($container, $filteritem, filter) {
        contactSelectorForFilter.SelectItemEvent = ASC.CRM.myFilter.onSelectParticipant;

        if (ASC.CRM.myFilter.containerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo($filteritem.find('span.contact-selector:first'));
        }
    },
    destroyFilterByParticipant: function($container, $filteritem, filter) {
        if (ASC.CRM.myFilter.containerId && ASC.CRM.myFilter.hiddenContainerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo(jq('#' + ASC.CRM.myFilter.hiddenContainerId));
            contactSelectorForFilter.changeContact('contactSelectorForFilter_0');
        }
    },

    processFilter: function($container, $filteritem, filtervalue, params) {
        if (params && params.id && isFinite(params.id)) {
            var input = jq("#contactTitle_" + contactSelectorForFilter.ObjName + "_0");
            contactSelectorForFilter.setContact(input, params.id,
                                params.hasOwnProperty('displayName') ? params.displayName : "",
                                params.hasOwnProperty('photoUrl') ? params.photoUrl : "");
            contactSelectorForFilter.showInfoContent(input);
            contactSelectorForFilter.SelectedContacts = [];
            contactSelectorForFilter.SelectedContacts.push(params.id);
        }
    }
};

ASC.CRM.ListDealView = (function($) {

    Teamlab.bind(Teamlab.events.getException, onGetException);

    function onGetException(params, errors) {
        console.log('deals.js ', errors);
        LoadingBanner.hideLoading();
    };

    var _setCookie = function(page, countOnPage) {
        if (ASC.CRM.ListDealView.cookieKey && ASC.CRM.ListDealView.cookieKey != "") {
            var cookie = {
                page: page,
                countOnPage: countOnPage
            };
            jq.cookies.set(ASC.CRM.ListDealView.cookieKey, cookie, { path: location.pathname });
        }
    };

    var _onGetDealsComplete = function() {

        jq("#dealTable tbody tr").remove();

        if (ASC.CRM.ListDealView.Total == 0) {
            jq("#dealList").hide();
            jq("#emptyContentForDealsFilter").show();
            LoadingBanner.hideLoading();
            return false;
        }

        jq("#dealButtonsPanel").show();
        jq("#dealList").show();

        jq("#dealTmpl").tmpl(ASC.CRM.ListDealView.dealList).prependTo("#dealTable tbody");

        ASC.CRM.Common.RegisterContactInfoCard();

        if (ASC.CRM.ListDealView.bidList.length == 0)
            jq("#dealList .showTotalAmount").hide();
        else
            jq("#dealList .showTotalAmount").show();

        LoadingBanner.hideLoading();
    };

    var _onGetMoreDealsComplete = function() {
        if (ASC.CRM.ListDealView.dealList.length == 0)
            return false;

        jq("#dealTmpl").tmpl(ASC.CRM.ListDealView.dealList).appendTo("#dealTable tbody");
        ASC.CRM.Common.RegisterContactInfoCard();

        if (ASC.CRM.ListDealView.bidList.length == 0)
            jq("#dealList .showTotalAmount").hide();
        else
            jq("#dealList .showTotalAmount").show();
    }

    var _addRecordsToContent = function() {
        if (!ASC.CRM.ListDealView.showMore) return false;
        ASC.CRM.ListDealView.dealList = new Array();
        var startIndex = jq("#dealTable tbody tr").length;

        jq("#showMoreDealsButtons .crm-showMoreLink").hide();
        jq("#showMoreDealsButtons .crm-loadingLink").show();

        _getDealsForContact(startIndex);
    };

    var _getDealsForContact = function(startIndex) {
        var filters = {
            contactID: ASC.CRM.ListDealView.contactID,
            count: ASC.CRM.ListDealView.entryCountOnPage,
            startIndex: startIndex,
            contactAlsoIsParticipant: true
        };
        Teamlab.getCrmOpportunities({ startIndex: startIndex || 0 }, { filter: filters, success: ASC.CRM.ListDealView.CallbackMethods.get_opportunities_for_contact });
    };

    var _getDeals = function(startIndex) {
        var filters = ASC.CRM.ListDealView.getFilterSettings(startIndex);

        //EventTracker.Track('crm_search_opportunities_by_filter');

        Teamlab.getCrmOpportunities({ startIndex: startIndex || 0 }, { filter: filters, success: ASC.CRM.ListDealView.CallbackMethods.get_opportunities_by_filter });
    };

    var _resizeFilter = function() {
        var visible = jq("#dealFilterContainer").is(":hidden") == false;
        if (ASC.CRM.ListDealView.isFilterVisible == false && visible) {
            ASC.CRM.ListDealView.isFilterVisible = true;
            if (ASC.CRM.ListDealView.advansedFilter)
                jq("#dealsAdvansedFilter").advansedFilter("resize");
        }
    };

    var _dealItemFactory = function(deal) {
        var tmpDate = new Date();
        var nowDate = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);

        deal.isOverdue = false;

        switch (deal.stage.stageType) {
            case 1:
                deal.closedStatusString = CRMJSResources.SuccessfullyClosed + ": " + deal.actualCloseDateString;
                break;
            case 2:
                deal.closedStatusString = CRMJSResources.UnsuccessfullyClosed + ": " + deal.actualCloseDateString;
                break;
            case 0:
                deal.closedStatusString = "";
                if (deal.expectedCloseDateString != "" && deal.expectedCloseDate.getTime() < nowDate.getTime()) {
                    deal.isOverdue = true;
                }
                break;
        }

        deal.bidNumberFormat = ASC.CRM.ListDealView.numberFormat(deal.bidValue,
                              { before: deal.bidCurrency.symbol, thousands_sep: " ", dec_point: ASC.CRM.ListDealView.currencyDecimalSeparator });

        if (typeof (deal.bidValue) != "undefined" && deal.bidValue != 0) {
            if (typeof (deal.perPeriodValue) == "undefined" || deal.perPeriodValue == 0)
                deal.perPeriodValue = 1;

            var isExist = false;
            for (var j = 0, len = ASC.CRM.ListDealView.bidList.length; j < len; j++)
                if (ASC.CRM.ListDealView.bidList[j].bidCurrencyAbbreviation == deal.bidCurrency.abbreviation) {
                ASC.CRM.ListDealView.bidList[j].bidValue += deal.bidValue * deal.perPeriodValue;
                isExist = true;
                break;
            }

            if (!isExist) {
                ASC.CRM.ListDealView.bidList.push(
                                  {
                                      bidValue: deal.bidValue * deal.perPeriodValue,
                                      bidCurrencyAbbreviation: deal.bidCurrency.abbreviation,
                                      bidCurrencySymbol: deal.bidCurrency.symbol,
                                      isConvertable: deal.bidCurrency.isConvertable
                                  });
            }
        }
    };

    var _changeFilter = function() {
        if (ASC.CRM.ListDealView.contactID == 0 && ASC.CRM.ListDealView.isFirstTime == true) {
            ASC.CRM.ListDealView.isFirstTime = false;

            jq("#emptyContentForDealsFilter").css("min-height", "250px");
            if (typeof (opportunitiesForFirstRequest) != "undefined")
                opportunitiesForFirstRequest = jq.parseJSON(jQuery.base64.decode(opportunitiesForFirstRequest));
            else
                opportunitiesForFirstRequest = [];

            var startIndex = ASC.CRM.ListDealView.entryCountOnPage * (ASC.CRM.ListDealView.currentPageNumber - 1);

            ASC.CRM.ListDealView.CallbackMethods.get_opportunities_by_filter(
                {
                    __startIndex: startIndex,
                    __nextIndex: opportunitiesForFirstRequest.nextIndex,
                    __total: opportunitiesForFirstRequest.total
                },
                Teamlab.create('crm-opportunities', null, opportunitiesForFirstRequest.response));
            return;
        }

        _setCookie(0, dealPageNavigator.EntryCountOnPage);
        _renderContent(0);
    };

    var _renderContent = function(startIndex) {
        ASC.CRM.ListDealView.dealList = new Array();
        ASC.CRM.ListDealView.bidList = new Array();

        LoadingBanner.displayLoading();

        _getDeals(startIndex);
    };

    return {
        CallbackMethods: {
            get_opportunities_by_filter: function(params, opportunities) {
                for (var i = 0, n = opportunities.length; i < n; i++) {
                    _dealItemFactory(opportunities[i]);
                }
                ASC.CRM.ListDealView.dealList = opportunities;
                jq(window).trigger("getDealsFromApi", [params, opportunities]);


                ASC.CRM.ListDealView.Total = params.__total || 0;
                var startIndex = params.__startIndex || 0;

                if (ASC.CRM.ListDealView.Total === 0) {
                    ASC.CRM.ListDealView.noDealsForQuery = true;
                }
                else {
                    ASC.CRM.ListDealView.noDealsForQuery = false;
                }

                if (ASC.CRM.ListDealView.noDealsForQuery) {
                    jq("#dealList").hide();
                    jq("#dealButtonsPanel").show();
                    jq("#mainExportCsv").next("img").hide();
                    jq("#mainExportCsv").hide();

                    jq("#dealFilterContainer").show();
                    _resizeFilter();
                    jq("#emptyContentForDealsFilter").show();
                    LoadingBanner.hideLoading();
                    return false;
                }

                jq("#totalDealsOnPage").text(ASC.CRM.ListDealView.Total);
                jq("#emptyContentForDealsFilter").hide();
                jq("#dealButtonsPanel").show();
                jq("#mainExportCsv").next("img").show();
                jq("#mainExportCsv").show();

                jq("#dealFilterContainer").show();
                _resizeFilter();

                jq("#dealList").show();
                jq("#dealTable tbody").replaceWith(jq("#dealListTmpl").tmpl({ opportunities: ASC.CRM.ListDealView.dealList }));

                ASC.CRM.Common.RegisterContactInfoCard();

                var tmpTotal;
                if (startIndex >= ASC.CRM.ListDealView.Total)
                    tmpTotal = startIndex + 1;
                else
                    tmpTotal = ASC.CRM.ListDealView.Total;
                dealPageNavigator.drawPageNavigator((startIndex / ASC.CRM.ListDealView.entryCountOnPage).toFixed(0) * 1 + 1, tmpTotal);

                jq("#simpleDealPageNavigator").html("");
                var $simplePN = jq("<div></div>");
                var lengthOfLinks = 0;
                if (jq("#divForDealPager .pagerPrevButtonCSSClass").length != 0) {
                    lengthOfLinks++;
                    jq("#divForDealPager .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
                }
                if (jq("#divForDealPager .pagerNextButtonCSSClass").length != 0) {
                    lengthOfLinks++;
                    if (lengthOfLinks === 2) {
                        jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
                    }
                    jq("#divForDealPager .pagerNextButtonCSSClass").clone().appendTo($simplePN);
                }
                if ($simplePN.children().length != 0) {
                    $simplePN.appendTo("#simpleDealPageNavigator");
                    jq("#simpleDealPageNavigator").show();
                }
                else
                    jq("#simpleDealPageNavigator").hide();

                if (ASC.CRM.ListDealView.bidList.length == 0)
                    jq("#dealList .showTotalAmount").hide();
                else
                    jq("#dealList .showTotalAmount").show();

                window.scrollTo(0, 0);
                LoadingBanner.hideLoading();
            },

            get_opportunities_for_contact: function(params, opportunities) {
                for (var i = 0, n = opportunities.length; i < n; i++) {
                    _dealItemFactory(opportunities[i]);
                    ASC.CRM.ListDealView.dealList.push(opportunities[i]);
                }
                jq(window).trigger("getDealsFromApi", [params, opportunities]);

                ASC.CRM.ListDealView.Total = params.__total || 0;
                if (typeof (params.__nextIndex) == "undefined")
                    ASC.CRM.ListDealView.showMore = false;

                if (!params.__startIndex)
                    _onGetDealsComplete();
                else
                    _onGetMoreDealsComplete();


                jq("#showMoreDealsButtons .crm-loadingLink").hide();
                if (ASC.CRM.ListDealView.showMore)
                    jq("#showMoreDealsButtons .crm-showMoreLink").show();
                else
                    jq("#showMoreDealsButtons .crm-showMoreLink").hide();
            }
        },

        init: function(contactID, rowsCount, currentPageNumber, cookieKey, currencyDecimalSeparator, anchor) {
            ASC.CRM.ListDealView.contactID = contactID;
            ASC.CRM.ListDealView.entryCountOnPage = rowsCount;
            ASC.CRM.ListDealView.currentPageNumber = currentPageNumber;
            ASC.CRM.ListDealView.currencyDecimalSeparator = currencyDecimalSeparator;
            ASC.CRM.ListDealView.cookieKey = cookieKey;

            ASC.CRM.ListDealView.isFilterVisible = false;

            var currentAnchor = location.hash;
            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#' ? currentAnchor.substring(1) : currentAnchor;

            if (currentAnchor == "" || decodeURIComponent(anchor) == currentAnchor)
                ASC.CRM.ListDealView.isFirstTime = true;
            else
                ASC.CRM.ListDealView.isFirstTime = false;

            ASC.CRM.ListDealView.bidList = new Array();
            ASC.CRM.ListDealView.dealList = new Array();

            dealPageNavigator.NavigatorParent = '#divForDealPager';
            dealPageNavigator.changePageCallback = function(page) {
                ASC.CRM.ListDealView.currentPageNumber = page;
                _setCookie(page, dealPageNavigator.EntryCountOnPage);

                var startIndex = dealPageNavigator.EntryCountOnPage * (page - 1);
                _renderContent(startIndex);
            }

            jq(window).bind("afterResetSelectedContact", function(event, obj, objName) {
                if (objName === "contactSelectorForFilter" && ASC.CRM.myFilter.filterId) {
                    jq('#' + ASC.CRM.myFilter.filterId).advansedFilter('resize');
                }
            });
        },

        initTab: function(contactID, rowsCount, currencyDecimalSeparator) {
            ASC.CRM.ListDealView.contactID = contactID;
            ASC.CRM.ListDealView.entryCountOnPage = rowsCount;
            ASC.CRM.ListDealView.currencyDecimalSeparator = currencyDecimalSeparator;

            jq("#showMoreDealsButtons .crm-showMoreLink").bind("click", function() {
                _addRecordsToContent();
            });

            ASC.CRM.ListDealView.showMore = true;

            ASC.CRM.ListDealView.isTabActive = false;

            ASC.CRM.ListDealView.bidList = new Array();
            ASC.CRM.ListDealView.dealList = new Array();

        },

        setFilter: function(evt, $container, filter, params, selectedfilters) { _changeFilter(); },
        resetFilter: function(evt, $container, filter, selectedfilters) { _changeFilter(); },

        activate: function() {
            if (ASC.CRM.ListDealView.isTabActive == false) {
                ASC.CRM.ListDealView.isTabActive = true;
                LoadingBanner.displayLoading();
                _getDealsForContact(0);
            }
        },

        getFilterSettings: function(startIndex) {
            startIndex = startIndex || 0;

            var settings = {
                startIndex: startIndex,
                count: ASC.CRM.ListDealView.entryCountOnPage
            };

            if (!ASC.CRM.ListDealView.advansedFilter) return settings;

            var param = ASC.CRM.ListDealView.advansedFilter.advansedFilter();

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

        expectedValue: function(bidType, perPeriodPalue) {
            switch (bidType) {
                case 1:
                    return CRMJSResources.BidType_PerHour + " " + jq.format(CRMJSResources.PerPeriodHours, perPeriodPalue);
                case 2:
                    return CRMJSResources.BidType_PerDay + " " + jq.format(CRMJSResources.PerPeriodDays, perPeriodPalue);
                case 3:
                    return CRMJSResources.BidType_PerWeek + " " + jq.format(CRMJSResources.PerPeriodWeeks, perPeriodPalue);
                case 4:
                    return CRMJSResources.BidType_PerMonth + " " + jq.format(CRMJSResources.PerPeriodMonths, perPeriodPalue);
                case 5:
                    return CRMJSResources.BidType_PerYear + " " + jq.format(CRMJSResources.PerPeriodYears, perPeriodPalue);
                default:
                    return "";
            }
        },

        changeCountOfRows: function(newValue) {
            if (isNaN(newValue)) return;
            var newCountOfRows = newValue * 1;
            ASC.CRM.ListDealView.entryCountOnPage = newCountOfRows;
            dealPageNavigator.EntryCountOnPage = newCountOfRows;

            _setCookie(1, newCountOfRows);
            _renderContent(0);
        },

        exportToCsv: function() {
            var index = window.location.href.indexOf('#');
            var basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href;
            var anchor = index >= 0 ? window.location.href.substr(index, window.location.href.length) : "";
            jq("#exportDialog").hide();
            window.location.href = basePath + "?action=export" + anchor;
        },

        openExportFile: function() {
            var index = window.location.href.indexOf('#');
            var basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href;
            jq("#exportDialog").hide();
            window.open(basePath + "?action=export&view=editor");
        },

        showExportDialog: function() {
            jq.dropdownToggle().toggle("#mainExportCsv", "exportDialog", 7, -20);
        },

        numberFormat: function(_number, _cfg) {
            function obj_merge(obj_first, obj_second) {
                var obj_return = {};
                for (key in obj_first) {
                    if (typeof obj_second[key] !== 'undefined') obj_return[key] = obj_second[key];
                    else obj_return[key] = obj_first[key];
                }
                return obj_return;
            }
            function thousands_sep(_num, _sep) {
                if (_num.length <= 3) return _num;
                var _count = _num.length;
                var _num_parser = '';
                var _count_digits = 0;
                for (var _p = (_count - 1); _p >= 0; _p--) {
                    var _num_digit = _num.substr(_p, 1);
                    if (_count_digits % 3 == 0 && _count_digits != 0 && !isNaN(parseFloat(_num_digit))) _num_parser = _sep + _num_parser;
                    _num_parser = _num_digit + _num_parser;
                    _count_digits++;
                }
                return _num_parser;
            }
            if (typeof _number !== 'number') {
                _number = parseFloat(_number);
                if (isNaN(_number)) return false;
            }
            var _cfg_default = { before: '', after: '', decimals: 2, dec_point: '.', thousands_sep: ',' };
            if (_cfg && typeof _cfg === 'object') {
                _cfg = obj_merge(_cfg_default, _cfg);
            }
            else _cfg = _cfg_default;
            _number = _number.toFixed(_cfg.decimals);
            if (_number.indexOf('.') != -1) {
                var _number_arr = _number.split('.');
                var _number = thousands_sep(_number_arr[0], _cfg.thousands_sep) + _cfg.dec_point + _number_arr[1];
            }
            else var _number = thousands_sep(_number, _cfg.thousands_sep);
            return _cfg.before + _number + _cfg.after;
        },

        showExchangeRatePopUp: function() {
            if (ASC.CRM.ListDealView.bidList.length == 0) return;
            ASC.CRM.ExchangeRateView.renderTotalAmount(ASC.CRM.ListDealView.bidList, ASC.CRM.ListDealView.currencyDecimalSeparator);
            ASC.CRM.Common.blockUI('#exchangeRatePopUp', 550, 700, 0)
        }
    };
})(jQuery);

ASC.CRM.DealActionView = (function($) {
    function _initFields(CurrencyDecimalSeparator, KeyCodeCurrencyDecimalSeparator, dateMask) {

        ASC.CRM.Common.renderCustomFields(customFieldList, "custom_field_", "customFieldRowTemplate", "#dealForm dl:last");
        jq("#dealForm .headerExpand").click();

        jq("input.textEditCalendar").mask(dateMask);
        jq("input.textEditCalendar").datepicker();

        jq.forceIntegerOnly("#perPeriodValue, #probability");
        jq("#probability").focusout(function(e) {
            var probability = jq.trim(jq("#probability").val());
            if (probability != "" && probability * 1 > 100)
                jq("#probability").val(100);
        });

        jq("#bidValue").keypress(function(event) {
            // Backspace, Del,
            var controlKeys = [8, 9];
            // IE doesn't support indexOf
            var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
            // Some browsers just don't raise events for control keys. Easy.
            if ((!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
              (48 <= event.which && event.which <= 57) || // Always 0 through 9
              isControlKey) ||
              (jq(this).val().length - jq(this).val().replace(CurrencyDecimalSeparator, '').length < 1 &&
                        event.which == KeyCodeCurrencyDecimalSeparator * 1)) {
                return;
            } else {
                event.preventDefault();
            }
        });

        jq("#bidValue").unbind('paste').bind('paste', function(e) {
            var oldValue = this.value;
            var $obj = this;
            setTimeout(
               function() {
                   var text = jq($obj).val();
                   if (isNaN(text)) {
                       jq($obj).val(oldValue);
                   }
                   else if (CurrencyDecimalSeparator != '.') {
                       text.replace('.', CurrencyDecimalSeparator);
                       jq($obj).val(text);
                   }
               }, 0);
            return true;
        });



        for (var i = 0; i < dealMilestones.length; i++) {
            var dealMilestone = dealMilestones[i];

            jq("<option>")
            .attr("value", dealMilestone.id)
            .text(dealMilestone.title)
            .appendTo(jq("#dealMilestone"));
        }

        jq("#probability").val(dealMilestones[jq("#dealMilestone")[0].selectedIndex].probability);

    }

    return {

        init: function(CurrencyDecimalSeparator, KeyCodeCurrencyDecimalSeparator, dateMask, today) {

            _initFields(CurrencyDecimalSeparator, KeyCodeCurrencyDecimalSeparator, dateMask);

            if (jq.getURLParam("id") != null) {
                jq("#nameDeal").val(targetDeal.title);
                jq("#descriptionDeal").val(targetDeal.description);
                jq("#bidValue").val(targetDeal.bid_value.toString().replace(/[\.\,]/g, CurrencyDecimalSeparator));

                jq(jq.format("#bidCurrency option[value={0}]", targetDeal.bid_currency)).attr("selected", "selected");

                if (targetDeal.bid_type > 0) {
                    jq(jq.format("#bidType [value={0}]", targetDeal.bid_type)).attr("selected", "selected");
                    jq("#bidType").nextAll().show();

                    jq("#perPeriodValue").val(targetDeal.per_period_value);
                    jq("#bidType").change();
                }

                jq("#probability").val(targetDeal.deal_milestone_probability);
                jq("#expectedCloseDate").val(targetDeal.expected_close_date);

                jq(jq.format("#dealMilestone option[value={0}]", targetDeal.deal_milestone)).attr("selected", "selected");

                jq("#contactID").val("contact_id");
            }
            else jq("#expectedCloseDate").val(today);


            jq(window).bind("editContactInSelector", function(event, $itemObj, objName) {
                if (objName == "dealMemberSelector")
                    dealClientSelector.ExcludedArrayIDs = dealMemberSelector.SelectedContacts.slice(0);
                if (objName == "dealClientSelector")
                    dealMemberSelector.ExcludedArrayIDs = [];
            });


            jq(window).bind("deleteContactFromSelector", function(event, $itemObj, objName) {
                if (objName == "dealMemberSelector") {
                    dealClientSelector.ExcludedArrayIDs = dealMemberSelector.SelectedContacts.slice(0);
                    if (jq("div[id^='item_dealMemberSelector_']").length == 1) {
                        jq("#dealMembersHeader").hide();
                        jq("#dealMembersBody").hide();
                        jq("#item_dealClientSelector_0").removeClass("hasMembers");
                    }
                }
                if (objName == "dealClientSelector")
                    dealMemberSelector.ExcludedArrayIDs = [];
            });

            jq("#selectorContent_dealClientSelector_0 .crm-addNewLink").parent().remove();
            jq("#newContactContent_dealClientSelector_0 .crm-addNewLink").remove();
            jq("#infoContent_dealClientSelector_0 .crm-addNewLink").removeAttr("onclick");

            jq("#infoContent_dealClientSelector_0 .crm-addNewLink").click(function() {
                jq("#item_dealClientSelector_0").addClass("hasMembers");
                if (jq("div[id^='item_dealMemberSelector_']").length == 0) {
                    dealMemberSelector.AddNewSelector(jq(this));
                }
                jq("#dealMembersHeader").show();
                jq("#dealMembersBody").show();
            });
            if (dealMemberSelector.SelectedContacts.length != 0) {
                jq("#item_dealClientSelector_0").addClass("hasMembers");
            }


            jq("#infoContent_dealClientSelector_0 .crm-editLink").bind('click', function() {
                dealMemberSelector.ExcludedArrayIDs = [];
            });

        },

        chooseMainContact: function(obj, input) {
            dealClientSelector.setContact(input, obj.id, obj.title, obj.img);
            dealClientSelector.showInfoContent(input);
            dealMemberSelector.ExcludedArrayIDs = [obj.id];
        },
        chooseMemberContact: function(obj, input) {
            dealMemberSelector.setContact(input, obj.id, obj.title, obj.img);
            dealMemberSelector.showInfoContent(input);
            dealClientSelector.ExcludedArrayIDs.push(obj.id);
        },

        selectBidTypeEvent: function(selectObj) {
            var idx = selectObj.selectedIndex;
            if (idx != 0)
                jq(selectObj).nextAll().show();
            else
                jq(selectObj).nextAll().hide();

            var elems = jq(selectObj).nextAll('span.splitter');
            var resourceValue = "";
            switch (idx) {
                case 1:
                    resourceValue = CRMJSResources.PerPeriodHours;
                    break;
                case 2:
                    resourceValue = CRMJSResources.PerPeriodDays;
                    break;
                case 3:
                    resourceValue = CRMJSResources.PerPeriodWeeks;
                    break;
                case 4:
                    resourceValue = CRMJSResources.PerPeriodMonths;
                    break;
                case 5:
                    resourceValue = CRMJSResources.PerPeriodYears;
                    break;
            }

            var labels = resourceValue.split("{0}");

            jq(elems).each(function(index) {
                jq(this).text(jq.trim(labels[index]));
            });

        },

        deleteDeal: function(dealID) {
            if (!confirm(jq.format(CRMJSResources.DeleteDealConfirmMessage, jq("#nameDeal").val()) + "\n" + CRMJSResources.DeleteConfirmNote))
                return;
            var contact_id = jq.trim(jq.getURLParam("contact_id"));

            Teamlab.removeCrmOpportunity({ contact_id: contact_id }, dealID, {
                before: function() {
                    jq("#dealForm input, #dealForm select, #dealForm textarea").attr("disabled", true);
                    jq("#dealForm  .ajax_info_block span").text(CRMJSResources.DeleteDealInProgress);
                    jq("#dealForm  .action_block").hide();
                    jq("#dealForm  .ajax_info_block").show();
                },
                success: function(params, opportunity) {
                    jq("#dealForm  .action_block").show();
                    jq("#dealForm  .ajax_info_block").hide();
                    if (params.contact_id != "")
                        location.href = jq.format("default.aspx?id={0}#deals", params.contact_id);
                    else
                        location.href = "deals.aspx";
                }
            });
        },

        submitForm: function() {
            var isValid = true;

            if (jq.trim(jq("#nameDeal").val()) == "") {
                ShowRequiredError(jq("#nameDeal"));
                isValid = false;
            }
            else RemoveRequiredErrorClass(jq("#nameDeal"));

            if (userSelector.SelectedUserId == null) {
                ShowRequiredError(jq("#inputUserName"));
                isValid = false;
            }
            else RemoveRequiredErrorClass(jq("#inputUserName"));

            if (!isValid)
                return false;

            var dealMilestoneProbability = jq.trim(jq("#probability").val());

            if (dealMilestoneProbability == "") {
                dealMilestoneProbability = 0;
                jq("#probability").val(0);
            }
            else {
                dealMilestoneProbability = dealMilestoneProbability * 1;

                if (dealMilestoneProbability > 100)
                    jq("#probability").val(100);
            }

            jq("#dealForm input, select, textarea")
            .attr("readonly", "readonly")
            .addClass('disabled');

            jq("#responsibleID").val(userSelector.SelectedUserId);

            if (dealClientSelector.SelectedContacts.length > 0)
                jq("#selectedContactID").val(dealClientSelector.SelectedContacts[0]);
            else
                jq("#selectedContactID").val(0);

            jq("#selectedMembersID").val(dealMemberSelector.SelectedContacts.join(","));

            if (jq.getURLParam("id") != null)
                jq("div.ajax_info_block span").text(CRMJSResources.EditingDealProgress);
            else
                jq("div.ajax_info_block span").text(CRMJSResources.AddNewDealProgress);

            jq("div.action_block").hide();
            jq("div.ajax_info_block").show();

            if (!jq("#isPrivate").is(":checked")) {
                SelectedUsers.IDs = new Array();
                jq("#cbxNotify").removeAttr("checked");
            }

            jq("#isPrivateDeal").val(jq("#isPrivate").is(":checked"));
            jq("#notifyPrivateUsers").val(jq("#cbxNotify").is(":checked"));
            jq("#selectedPrivateUsers").val(SelectedUsers.IDs.join(","));

            var $checkboxes = jq("#dealForm input[type='checkbox'][id^='custom_field_']");
            if ($checkboxes) {
                for (var i = 0; i < $checkboxes.length; i++) {
                    if (jq($checkboxes[i]).is(":checked")) {
                        var id = $checkboxes[i].id.replace('custom_field_', '');
                        jq("#dealForm input[name='customField_" + id + "']").val(jq($checkboxes[i]).is(":checked"));
                    }
                }
            }

            return true;
        },
        gotoAddCustomFieldPage: function() {
            if (confirm(CRMJSResources.ConfirmGoToCustomFieldPage)) {
                document.location = "settings.aspx?type=custom_field&view=opportunity";
            }
        }
    };
})(jQuery);

ASC.CRM.DealDetailsView = (function($) {
    return {
        init: function() {
            for (var i = 0; i < customFieldList.length; i++) {
                var field = customFieldList[i];
                if (jQuery.trim(field.mask) == "") continue;
                field.mask = jq.evalJSON(field.mask);
            }
            jq("#customFieldTmpl").tmpl(customFieldList).appendTo("#dealInfo");

            jq("#dealInfo dt.headerBase").each(
                function(index) {
                    var item = jq(this);
                    if (item.next().nextUntil("dt.headerBase").length == 0) {
                        item.next().remove();
                        item.remove();
                        return;
                    }
                    jq(this).click();
                });


            jq.dropdownToggle({ dropdownID: 'dealMilestoneDropDown', switcherSelector: '#dealMilestoneSwitcher', addTop: 5, addLeft: jq("#dealMilestoneSwitcher").width() - 81 });

//            jq(window).bind("getContactsFromApi", function(event, contacts) {
//                var contactLength = contacts.length;
//
//                if (contactLength == 0) {
//                    jq("#emptyDealParticipantPanel").show();
//                    //jq("#eventLinkToPanel").hide();
//                }
//                else {
//                    jq("#addDealParticipantButton").show();
//
//                    var contactIDs = [];
//                    for (var i = 0; i < contactLength; i++)
//                        contactIDs.push(contacts[i].id);
//                    dealContactSelector.SelectedContacts = contactIDs;
//                }
//            });
//
//            dealContactSelector.SelectItemEvent = ASC.CRM.DealDetailsView.addMemberToDeal;
        },

        changeDealMilestone: function(dealID, milestoneID, milestoneTitle) {

            AjaxPro.DealDetailsView.ChangeDealMilestone(dealID * 1, milestoneID * 1, function(response) {
                if (response.error == null) {
                    jq("#dealMilestoneDropDown").hide();
                    jq("#dealMilestoneSwitcher .baseLinkAction").text(milestoneTitle);
                    jq("#dealMilestoneProbability").text(response.value.rs1 + "%");

                    if (response.value.rs3 == "true") { //closed
                        jq("#closeDealDate span").text(CRMJSResources.ActualCloseDate + ":");
                        jq("#closeDealDate p").text(response.value.rs2);
                    }
                    else { //opened
                        if (response.value.rs2 != "") {
                            jq("#closeDealDate span").text(CRMJSResources.ExpectedCloseDate + ":");
                            jq("#closeDealDate p").text(response.value.rs2);
                        }
                        else {
                            jq("#closeDealDate span").text(CRMJSResources.ExpectedCloseDate + ":");
                            jq("#closeDealDate p").text(CRMJSResources.NoCloseDate);
                        }
                    }
                }
            });
        },

        removeMemberFromDeal: function(id) {
            Teamlab.removeCrmEntityMember({ contactID: id }, entityData.type, entityData.id, id, {
                before: function(params) {
                    jq("#trashImg_" + params.contactID).hide();
                    jq("#loaderImg_" + params.contactID).show();
                },
                after: function(params) {

                    var index = jq.inArray(params.contactID, dealContactSelector.SelectedContacts);
                    dealContactSelector.SelectedContacts.splice(index, 1);

                    jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                    ASC.CRM.HistoryView.removeOptionFromContact(params.contactID);

                    //ASC.CRM.Common.changeCountInTab("delete", "contacts");

                    setTimeout(function() {
                        jq("#contactItem_" + params.contactID).remove();
                        if (dealContactSelector.SelectedContacts.length == 0) {
                            jq("#addDealParticipantButton").hide();
                            jq("#dealParticipantPanel").hide();
                            jq("#emptyDealParticipantPanel").show();
                        }
                    }, 500);
                }
            });
        },

        addMemberToDeal: function(obj) {
            if (jq("#contactItem_" + obj.id).length > 0) return false;
            var data =
                {
                    contactid: obj.id,
                    opportunityid: entityData.id
                };
            Teamlab.addCrmEntityMember({}, entityData.type, entityData.id, obj.id, data, {
                success: function(params, contact) {
                    jq("#simpleContactTmpl").tmpl(contact).prependTo("#contactTable tbody");
                    //ASC.CRM.Common.changeCountInTab("add", "contacts");
                    ASC.CRM.Common.RegisterContactInfoCard();

                    dealContactSelector.SelectedContacts.push(contact.id);

                    ASC.CRM.HistoryView.appendOptionToContact({ value: contact.id, title: contact.displayName });
                }
            });
        }

    };
})(jQuery);

ASC.CRM.ExchangeRateView = (function($) {
    var convertAmount = function() {
        var amount = jq("#amount").val().trim();
        if (amount != "") {
            jq("#introducedAmount").text(amount);
            jq("#conversionResult").text(amount * 1 * ASC.CRM.ExchangeRateView.Rate + " " + jq("#toCurrency").val());
        }
    };

    return {
        init: function() {
            ASC.CRM.ExchangeRateView.Rate = 1;

            jq.forceIntegerOnly("#amount", convertAmount);

            jq("#amount").keyup(function(event) {
                convertAmount();
            });

            ASC.CRM.ExchangeRateView.ExchangeRates = {};
            if (typeof (exchangeRatesArray) != "undefined")
                for (var i = 0; i < exchangeRatesArray.length; i++)
                    ASC.CRM.ExchangeRateView.ExchangeRates[exchangeRatesArray[i].abbreviation] = exchangeRatesArray[i].value;
        },

        changeCurrency: function() {
            var fromCurrency = jq("#fromCurrency").val();
            var toCurrency = jq("#toCurrency").val();

            AjaxPro.ExchangeRateView.ChangeCurrency(fromCurrency, toCurrency, function(response) {
                if (response.error == null) {
                    ASC.CRM.ExchangeRateView.Rate = response.value.Rate;
                    jq("#conversionRate").text("1 " + fromCurrency + " = " + response.value.Rate + " " + toCurrency);
                    jq("#introducedFromCurrency").text(response.value.FromCurrencyTitle + ":");
                    jq("#introducedToCurrency").text(response.value.ToCurrencyTitle + ":");
                    convertAmount();
                }
            });


        },

        updateSummaryTable: function(newCurrency) {

            AjaxPro.ExchangeRateView.UpdateSummaryTable(newCurrency, function(response) {
                if (response.error == null) {
                    for (var i = 0; i < response.value.length; i++)
                        jq("#" + response.value[i].CurrencyAbbr).text(response.value[i].Rate);
                }
            });
        },

        renderTotalAmount: function(bidList, currencyDecimalSeparator) {
            jq("#totalOnPage .diferrentBids").html("");
            jq("#totalOnPage .totalBid").html("");
            var sum = 0;
            var tmpBidNumberFormat;

            var isTotalBidAndExchangeRateShow = false;

            for (var i = 0, len = bidList.length; i < len; i++) {

                if (bidList[i].bidCurrencyAbbreviation != defaultCurrency.Abbreviation)
                    isTotalBidAndExchangeRateShow = true;

                tmpBidNumberFormat = ASC.CRM.ListDealView.numberFormat(bidList[i].bidValue,
                            { before: bidList[i].bidCurrencySymbol, thousands_sep: " ", dec_point: currencyDecimalSeparator });

                jq("#bidFormat").tmpl({ number: tmpBidNumberFormat, abbreviation: bidList[i].bidCurrencyAbbreviation }).appendTo("#totalOnPage .diferrentBids");

                if (!bidList[i].isConvertable)
                    jq("#bidFormat").tmpl({ number: tmpBidNumberFormat, abbreviation: bidList[i].bidCurrencyAbbreviation }).appendTo("#totalOnPage .totalBid");

                var tmp_rate = ASC.CRM.ExchangeRateView.ExchangeRates[bidList[i].bidCurrencyAbbreviation];
                if (bidList[i].isConvertable && typeof (tmp_rate) != "undefined")
                    sum += bidList[i].bidValue / tmp_rate;
            }

            tmpBidNumberFormat = ASC.CRM.ListDealView.numberFormat(sum, { before: defaultCurrency.Symbol, thousands_sep: " ", dec_point: currencyDecimalSeparator });

            jq("#bidFormat").tmpl({ number: tmpBidNumberFormat, abbreviation: defaultCurrency.Abbreviation }).prependTo("#totalOnPage .totalBid");

            if (isTotalBidAndExchangeRateShow == true)
                jq("#totalOnPage .totalBidAndExchangeRateLink").show();
            else
                jq("#totalOnPage .totalBidAndExchangeRateLink").hide();

            jq("#totalOnPage").show();
        }


    }
})(jQuery);


jq(document).ready(function() {
    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintStages",
        dropdownID: "files_hintStagesPanel",
        fixWinSize: false
    });
});
