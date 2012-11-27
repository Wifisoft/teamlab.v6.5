if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = (function() { return {} })();

ASC.CRM.ListCasesView = (function($) {

    Teamlab.bind(Teamlab.events.getException, _onGetException);

    function _onGetException(params, errors) {
        console.log('cases.js ', errors);
        LoadingBanner.hideLoading();
    };

    var _setCookie = function(page, countOnPage) {
        if (ASC.CRM.ListCasesView.cookieKey && ASC.CRM.ListCasesView.cookieKey != "") {
            var cookie = {
                page: page,
                countOnPage: countOnPage
            };
            jq.cookies.set(ASC.CRM.ListCasesView.cookieKey, cookie, { path: location.pathname });
        }
    };

    var _changeFilter = function() {
        if (ASC.CRM.ListCasesView.isFirstTime == true) {
            ASC.CRM.ListCasesView.isFirstTime = false;

            if (typeof (casesForFirstRequest) != "undefined")
                casesForFirstRequest = jq.parseJSON(jQuery.base64.decode(casesForFirstRequest));
            else
                casesForFirstRequest = [];

            ASC.CRM.ListCasesView.CallbackMethods.get_cases_by_filter({
                __startIndex: casesForFirstRequest.startIndex,
                __nextIndex: casesForFirstRequest.nextIndex,
                __total: casesForFirstRequest.total
            },
                Teamlab.create('crm-cases', null, casesForFirstRequest.response));

            return;
        }

        _setCookie(0, casesPageNavigator.EntryCountOnPage);
        _renderContent(0);
    };

    var _renderContent = function(startIndex) {
        ASC.CRM.ListCasesView.casesList = new Array();
        LoadingBanner.displayLoading();
        _getCases(startIndex);
    };


    var _getCases = function(startIndex) {
        var filters = ASC.CRM.ListCasesView.getFilterSettings(startIndex);

        Teamlab.getCrmCases({ startIndex: startIndex || 0 }, { filter: filters, success: ASC.CRM.ListCasesView.CallbackMethods.get_cases_by_filter });
    };

    var _resizeFilter = function() {
        var visible = jq("#caseFilterContainer").is(":hidden") == false;
        if (ASC.CRM.ListCasesView.isFilterVisible == false && visible) {
            ASC.CRM.ListCasesView.isFilterVisible = true;
            if (ASC.CRM.ListCasesView.advansedFilter)
                jq("#casesAdvansedFilter").advansedFilter("resize");
        }
    };

    return {
        CallbackMethods: {
            get_cases_by_filter: function(params, cases) {

                ASC.CRM.ListCasesView.casesList = cases;

                ASC.CRM.ListCasesView.Total = params.__total || 0;
                var startIndex = params.__startIndex || 0;

                if (ASC.CRM.ListCasesView.Total === 0) {
                    jq("#caseTable tbody tr").remove();
                    jq("#caseList").hide();
                    jq("#caseButtonsPanel").show();
                    jq("#mainExportCsv").next("img").hide();
                    jq("#mainExportCsv").hide();
                    jq("#caseFilterContainer").show();
                    _resizeFilter();
                    jq("#emptyContentForCasesFilter").show();
                    LoadingBanner.hideLoading();
                    return false;
                }

                jq("#totalCasesOnPage").text(ASC.CRM.ListCasesView.Total);

                jq("#emptyContentForCasesFilter").hide();
                jq("#caseButtonsPanel").show();
                jq("#mainExportCsv").next("img").show();
                jq("#mainExportCsv").show();
                jq("#caseFilterContainer").show();
                _resizeFilter();

                jq("#caseList").show();
                jq("#caseTable tbody").replaceWith(jq("#caseListTmpl").tmpl({ cases: ASC.CRM.ListCasesView.casesList }));

                var tmpTotal;
                if (startIndex >= ASC.CRM.ListCasesView.Total)
                    tmpTotal = startIndex + 1;
                else
                    tmpTotal = ASC.CRM.ListCasesView.Total;
                casesPageNavigator.drawPageNavigator((startIndex / ASC.CRM.ListCasesView.entryCountOnPage).toFixed(0) * 1 + 1, tmpTotal);
                jq("#tableForCasesNavigation").show();

                jq("#simpleCasesPageNavigator").html("");
                var $simplePN = jq("<div></div>");
                var lengthOfLinks = 0;
                if (jq("#divForCasesPager .pagerPrevButtonCSSClass").length != 0) {
                    lengthOfLinks++;
                    jq("#divForCasesPager .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
                }
                if (jq("#divForCasesPager .pagerNextButtonCSSClass").length != 0) {
                    lengthOfLinks++;
                    if (lengthOfLinks === 2) {
                        jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
                    }
                    jq("#divForCasesPager .pagerNextButtonCSSClass").clone().appendTo($simplePN);
                }
                if ($simplePN.children().length != 0) {
                    $simplePN.appendTo("#simpleCasesPageNavigator")
                    jq("#simpleCasesPageNavigator").show();
                }
                else
                    jq("#simpleCasesPageNavigator").hide();

                window.scrollTo(0, 0);
                LoadingBanner.hideLoading();
            }
        },

        init: function(rowsCount, currentPageNumber, cookieKey, anchor) {
            ASC.CRM.ListCasesView.entryCountOnPage = rowsCount;
            ASC.CRM.ListCasesView.currentPageNumber = currentPageNumber;
            ASC.CRM.ListCasesView.cookieKey = cookieKey;

            ASC.CRM.ListCasesView.isFilterVisible = false;

            var currentAnchor = location.hash;
            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#' ? currentAnchor.substring(1) : currentAnchor;

            if (currentAnchor == "" || decodeURIComponent(anchor) == currentAnchor)
                ASC.CRM.ListCasesView.isFirstTime = true;
            else
                ASC.CRM.ListCasesView.isFirstTime = false;

            ASC.CRM.ListCasesView.casesList = new Array();

            casesPageNavigator.NavigatorParent = '#divForCasesPager';
            casesPageNavigator.changePageCallback = function(page) {
                ASC.CRM.ListCasesView.currentPageNumber = page;
                _setCookie(page, casesPageNavigator.EntryCountOnPage);

                var startIndex = casesPageNavigator.EntryCountOnPage * (page - 1);
                _renderContent(startIndex);
            }
        },

        setFilter: function(evt, $container, filter, params, selectedfilters) { _changeFilter(); },
        resetFilter: function(evt, $container, filter, selectedfilters) { _changeFilter(); },

        getFilterSettings: function(startIndex) {
            startIndex = startIndex || 0;
            var settings = {
                startIndex: startIndex,
                count: ASC.CRM.ListCasesView.entryCountOnPage
            };

            if (!ASC.CRM.ListCasesView.advansedFilter) return settings;

            var param = ASC.CRM.ListCasesView.advansedFilter.advansedFilter();

            jq(param).each(function(i, item) {
                switch (item.id) {
                    case "sorter":
                        settings.sortBy = item.params.id;
                        settings.sortOrder = item.params.dsc == true ? 'descending' : 'ascending';
                        break;
                    case "text":
                        settings.filterValue = item.params.value;
                        break;
                    default:
                        if (item.hasOwnProperty("apiparamname") && item.params.hasOwnProperty("value") && item.params.value != null) {
                            settings[item.apiparamname] = item.params.value;
                        }
                        break;
                }
            });
            return settings;
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
        changeCountOfRows: function(newValue) {
            if (isNaN(newValue)) return;
            var newCountOfRows = newValue * 1;
            ASC.CRM.ListCasesView.entryCountOnPage = newCountOfRows;
            casesPageNavigator.EntryCountOnPage = newCountOfRows;

            _setCookie(1, newCountOfRows);
            _renderContent(0);
        }
    }
})(jQuery);



ASC.CRM.CasesActionView = (function($) {
    return {
        init: function(dateMask) {
            ASC.CRM.Common.renderCustomFields(casesEditCustomFieldList, "custom_field_", "casesEditCustomFieldTmpl", "#crm_caseMakerDialog dl");
            jq("#crm_caseMakerDialog dl .headerExpand").click();

            jq("input.textEditCalendar").mask(dateMask);
            jq("input.textEditCalendar").datepicker({
                popupContainer: "body",
                selectDefaultDate: true,
                showAnim: ''
            });

            if (casesContactSelector.SelectedContacts.length == 0) {
                jq("#selector_" + casesContactSelector.ObjName).children("div:first").children("div[id^='item_']").remove();
                jq("#membersCasesSelectorsContainer").prev('dt').addClass("crm-withGrayPlus");
            }

            jq(window).bind("deleteContactFromSelector", function(event, $itemObj, objName) {
                if (jq("#selector_" + casesContactSelector.ObjName).children("div:first").children("div[id^='item_']").length == 1) {
                    jq("#membersCasesSelectorsContainer").prev('dt').addClass("crm-withGrayPlus");
                }
            });

            jq("#crm_caseMakerDialog dl .crm-withGrayPlus").live("click", function(event) {
                var container_id = "membersCasesSelectorsContainer";
                casesContactSelector.AddNewSelector(jq(this));
                jq(this).removeClass("crm-withGrayPlus");
            });
        },

        submitForm: function() {
            var title = jq("#crm_caseMakerDialog #caseTitle").val().trim();
            if (title == "") {
                AddRequiredErrorText(jq("#crm_caseMakerDialog #caseTitle"), CRMJSResources.ErrorEmptyCaseTitle);
                ShowRequiredError(jq("#crm_caseMakerDialog #caseTitle"));
                return false;
            }

            jq("#crm_caseMakerDialog .action_block").hide();
            jq("#crm_caseMakerDialog .ajax_info_block").show();

            jq("#membersCasesSelectorsContainer input[name=memberID]").val(casesContactSelector.SelectedContacts.join(","));

            if (!jq("#isPrivate").is(":checked")) {
                SelectedUsers.IDs = new Array();
                jq("#notifyPrivate").removeAttr("checked");
            }

            jq("#isPrivateCase").val(jq("#isPrivate").is(":checked"));
            jq("#notifyPrivateUsers").val(jq("#cbxNotify").is(":checked"));
            jq("#selectedUsersCase").val(SelectedUsers.IDs.join(","));

            var $checkboxes = jq("#crm_caseMakerDialog input[type='checkbox'][id^='custom_field_']");
            if ($checkboxes) {
                for (var i = 0; i < $checkboxes.length; i++) {
                    if (jq($checkboxes[i]).is(":checked")) {
                        var id = $checkboxes[i].id.replace('custom_field_', '');
                        jq("#crm_caseMakerDialog input[name='customField_" + id + "']").val(jq($checkboxes[i]).is(":checked"));
                    }
                }
            }
            return true;
        },
        gotoAddCustomFieldPage: function() {
            if (confirm(CRMJSResources.ConfirmGoToCustomFieldPage)) {
                document.location = "settings.aspx?type=custom_field&view=case";
            }
        }
    }
})(jQuery);



ASC.CRM.CasesDetailsView = (function($) {
    return {
        init: function() {
            for (var i = 0; i < casesCustomFieldList.length; i++) {
                var field = casesCustomFieldList[i];
                if (jQuery.trim(field.mask) == "") continue;
                field.mask = jq.evalJSON(field.mask);
            }

            jq("#casesCustomFieldsListTmpl").tmpl(casesCustomFieldList).appendTo("#casesCustomFieldsList");

            jq("#casesCustomFieldsList dt.headerBase").each(
                function(index) {
                    var item = jq(this);

                    if (item.next().nextUntil("dt.headerBase").length == 0) {
                        item.next().remove();
                        item.remove();
                        return;
                    }
                    jq(this).click();
                });

//            jq(window).bind("getContactsFromApi", function(event, contacts) {
//                var contactLength = contacts.length;

//                if (contactLength == 0) {
//                    jq("#emptyCaseParticipantPanel").show();
//                    jq("#eventLinkToPanel").hide();
//                }
//                else {
//                    jq("#addCaseParticipantButton").show();

//                    var contactIDs = [];
//                    for (var i = 0; i < contactLength; i++)
//                        contactIDs.push(contacts[i].id);
//                    casesContactSelector.SelectedContacts = contactIDs;
//                }
//            });

//            casesContactSelector.SelectItemEvent = ASC.CRM.CasesDetailsView.addMemberToCase;
        },

        changeCaseStatus: function(isClosed) {
            var caseID = jq.getURLParam("id") * 1;

            AjaxPro.onLoading = function(b) { };

            AjaxPro.CasesDetailsView.ChangeCaseStatus(isClosed, caseID,
                    function(res) {
                        if (res.error != null) { alert(res.error.Message); return; }
                        jq("#caseStatusSwitcher").text(res.value);
                        if (isClosed == 1) {
                            jq("#closeCase").hide();
                            jq("#openCase").show();
                        }
                        else {
                            jq("#openCase").hide();
                            jq("#closeCase").show();
                        }
                    }
                );
        },

        removeMemberFromCase: function(id) {
            Teamlab.removeCrmEntityMember({ contactID: id }, entityData.type, entityData.id, id, {
                before: function(params) {
                    jq("#trashImg_" + params.contactID).hide();
                    jq("#loaderImg_" + params.contactID).show();
                },
                after: function(params) {

                    var index = jq.inArray(params.contactID, casesContactSelector.SelectedContacts);
                    casesContactSelector.SelectedContacts.splice(index, 1);

                    jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                    ASC.CRM.HistoryView.removeOptionFromContact(params.contactID);

                    //ASC.CRM.Common.changeCountInTab("delete", "contacts");

                    setTimeout(function() {
                        jq("#contactItem_" + params.contactID).remove();
                        if (casesContactSelector.SelectedContacts.length == 0) {
                            jq("#addCaseParticipantButton").hide();
                            jq("#caseParticipantPanel").hide();
                            jq("#emptyCaseParticipantPanel").show();
                        }
                    }, 500);
                }
            });
        },

        addMemberToCase: function(obj) {
            if (jq("#contactItem_" + obj.id).length > 0) return false;
            var data =
                {
                    contactid: obj.id,
                    caseid: entityData.id
                };
            Teamlab.addCrmEntityMember({}, entityData.type, entityData.id, obj.id, data, {
                success: function(params, contact) {
                    jq("#simpleContactTmpl").tmpl(contact).prependTo("#contactTable tbody");
                    //ASC.CRM.Common.changeCountInTab("add", "contacts");
                    ASC.CRM.Common.RegisterContactInfoCard();

                    casesContactSelector.SelectedContacts.push(contact.id);

                    ASC.CRM.HistoryView.appendOptionToContact({ value: contact.id, title: contact.displayName });
                }
            });
        }
    }
})(jQuery);
