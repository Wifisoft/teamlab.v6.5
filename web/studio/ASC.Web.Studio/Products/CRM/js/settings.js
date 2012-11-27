if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = function() { return {} };

ASC.CRM.SettingsPage = (function($) {
    var _initSortableFields = function() {
        jq("#customFieldList").sortable({
            cursor: "move",
            handle: '.sort_row_handle',
            items: 'li',
            start: function(event, ui) {
                jq("#customFieldActionMenu").hide();
                jq('#customFieldList .crm-menu.active').removeClass('active');
            },
            beforeStop: function(event, ui) {
                jq(ui.item[0]).attr("style", "");
            },
            update: function(event, ui) {
                var fieldID = new Array();

                jq("#customFieldList li").find("[id^=custom_field_]").each(
                           function() {
                               fieldID.push(jq(this).attr("id").split('_')[2]);
                           }
                        );

                AjaxPro.onLoading = function(b) { };
                AjaxPro.CustomFieldsView.ReorderFields(fieldID,
                           function(res) { }
                        );
            }
        });
    };

    var _initFieldsActionMenu = function() {
        if (jq("#customFieldActionMenu").length != 1) return;

        jq("#customFieldActionMenu").show();
        var left = jq("#customFieldActionMenu .dropDownCornerRight").position().left;
        jq("#customFieldActionMenu").hide();

        jq.dropdownToggle({
            dropdownID: 'customFieldActionMenu',
            switcherSelector: '#customFieldList .crm-menu',
            addTop: 4,
            addLeft: -left + 6,
            showFunction: function(switcherObj, dropdownItem) {
                jq('#customFieldList .crm-menu.active').removeClass('active');
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass('active');
                    if (switcherObj.attr('fieldid') != dropdownItem.attr('fieldid')) {
                        dropdownItem.attr('fieldid', switcherObj.attr('fieldid'));
                    }
                }
            },
            hideFunction: function() {
                jq('#customFieldList .crm-menu.active').removeClass('active');
            }
        });
    };

    var _findIndexOfFieldByID = function(fieldID) {
        for (var i = 0, n = ASC.CRM.SettingsPage.customFieldList.length; i < n; i++) {
            if (ASC.CRM.SettingsPage.customFieldList[i].id == fieldID) {
                return i;
            }
        }
        return -1;
    };

    var _resetManageFieldPanel = function() {
        jq("#manageField dl input:first").val("");
        jq("#manageField dl dd:last ul li:not(:first)").remove();
        jq('#manageField select').attr('value', '0');
        jq("#manageField dl .field_mask").hide();
        jq("#manageField dl .text_field").show();
    };


    var _fieldFactory = function(field, count) {
        if (jQuery.trim(field.mask) == "") field.maskObj = "";
        else field.maskObj = jq.evalJSON(field.mask);

        field.relativeItemsCount = count;
        field.relativeItemsString = ASC.CRM.SettingsPage.getLinkContactStringByIndex(count, jq.getURLParam("type"), jq.getURLParam("view"));
    };

    var _validateDataField = function() {
        if (jq("#manageField dl input:first").val().trim() == "") {
            ShowRequiredError(jq("#manageField dl input:first"), true);
            return false;
        } else {
            RemoveRequiredErrorClass(jq("#manageField dl input:first"));
            return true;
        }
    };

    var _readFieldData = function(sortOrder) {
        var view = jq.getURLParam("view");

        var field = {
            label: jq("#manageField dl input:first").val().trim(),
            fieldType: parseInt(jq("#manageField dl select:first").val()),
            entityType: view != null && view != "" ? view : "contact",
            position: sortOrder
        };

        switch (field.fieldType) {
            case 0:
                field.mask = jq.toJSON({ "size": jq("#text_field_size").val() });
                break;
            case 1:
                field.mask = jq.toJSON({
                    "rows": jq("#textarea_field_rows").val(),
                    "cols": jq("#textarea_field_cols").val()
                });
                break;
            case 2:
                var selectedItem = new Array();
                jq("#manageField dd.select_options :input").each(function() {
                    var value = jq(this).val().trim();
                    if (value == "") return;
                    selectedItem.push(jq(this).val());
                });

                if (selectedItem.length == 0) {
                    alert(CRMJSResources.EmptyItemList);
                    return;
                }
                field.mask = jq.toJSON(selectedItem);
                break;
            default:
                field.mask = "";
                break;
        }
        return field;
    };

    var _createField = function() {
        if (!_validateDataField()) return;

        var $listCustomFields = jq("#customFieldList li");

        var field = _readFieldData($listCustomFields.length);

        var index = 0;
        for (var i = 0, len = $listCustomFields.length; i < len; i++) {
            if (jq($listCustomFields[i]).find(".customFieldTitle").text().trim() == field.label) {
                index = i + 1;
                break;
            }
        }
        if (index == 0) {
            Teamlab.addCrmCustomField({}, field.entityType, field, {
                before: function(params) {
                    jq("#manageField .action_block").hide();
                    jq("#manageField .ajax_info_block").show();
                },
                after: function(params) {
                    jq("#manageField .action_block").show();
                    jq("#manageField .ajax_info_block").hide();
                },
                success: ASC.CRM.SettingsPage.CallbackMethods.add_customField
            });
        }
        else {
            ASC.CRM.Common.animateElement({
                element: jq($listCustomFields[index - 1]),
                afterScrollFunc: jq.unblockUI()
            });
        }
    };

    var _editField = function(liObj, id, indexOfField) {
        if (!_validateDataField()) return;

        var $listCustomFields = jq("#customFieldList li");

        var field = _readFieldData(indexOfField);
        field.id = id;

        var index = 0;
        for (var i = 0, n = $listCustomFields.length; i < n; i++) {
            if (jq($listCustomFields[i]).find(".customFieldTitle").text().trim() == field.label && jq($listCustomFields[i]).not(jq(liObj)).length != 0) {
                index = i + 1;
                break;
            }
        }
        if (index == 0) {
            Teamlab.updateCrmCustomField({ liObj: liObj, indexOfField: indexOfField }, field.entityType, field.id, field, {
                before: function() {
                    jq("#manageField .action_block").hide();
                    jq("#manageField .ajax_info_block").show();
                },
                after: function() {
                    jq("#manageField .action_block").show();
                    jq("#manageField .ajax_info_block").hide();
                },
                success: ASC.CRM.SettingsPage.CallbackMethods.edit_customField
            });
        }
        else {
            ASC.CRM.Common.animateElement({
                element: jq($listCustomFields[index - 1]),
                afterScrollFunc: jq.unblockUI()
            });
        }
    };

    return {
        CallbackMethods: {
            add_customField: function(params, field) {
                _fieldFactory(field, 0);
                ASC.CRM.SettingsPage.customFieldList.push(field);

                var $itemHTML = jq("#customFieldRowTemplate").tmpl(field);
                if (jq("#customFieldList li").length == 0) {
                    jq('#emptyCustomFieldContent').hide();
                    jq('#customFieldList').show();
                }
                $itemHTML.appendTo("#customFieldList");

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            },

            edit_customField: function(params, field) {
                _fieldFactory(field, 0);
                ASC.CRM.SettingsPage.customFieldList[params.indexOfField] = field;

                var $itemHTML = jq("#customFieldRowTemplate").tmpl(field);

                params.liObj.hide();
                $itemHTML.insertAfter(params.liObj);
                params.liObj.remove();

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            },

            delete_customField: function(params, field) {
                params.liObj.remove();

                var index = _findIndexOfFieldByID(field.id);
                if (index != -1) ASC.CRM.SettingsPage.customFieldList.splice(index, 1);

                if (jq("#customFieldList li").length == 0) {
                    jq('#customFieldList').hide();
                    jq('#emptyCustomFieldContent').show();
                }
            }
        },

        init: function() {
            if (typeof (customFieldList) == "undefined" ||
                typeof (relativeItems) == "undefined" ||
                customFieldList.length != relativeItems.length) return;

            for (var i = 0, len = customFieldList.length; i < len; i++) {
                _fieldFactory(customFieldList[i], relativeItems[i]);
            }

            ASC.CRM.SettingsPage.customFieldList = customFieldList;

            //jq("#customFieldList").disableSelection();

            if (ASC.CRM.SettingsPage.customFieldList.length != 0) {
                jq("#customFieldList").show();
                jq("#customFieldRowTemplate").tmpl(ASC.CRM.SettingsPage.customFieldList).appendTo("#customFieldList");
            }
            else {
                jq("#emptyCustomFieldContent").show();
            }

            _initSortableFields();
            _initFieldsActionMenu();

            jq.forceIntegerOnly("#text_field_size");
            jq.forceIntegerOnly("#textarea_field_rows");
            jq.forceIntegerOnly("#textarea_field_cols");
        },

        startExportData: function() {
            jq("#exportDataContent a.baseLinkButton").hide();
            jq("#exportDataContent p.headerBaseSmall").show();

            AjaxPro.CommonSettingsView.StartExportData(function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }
                ASC.CRM.SettingsPage.checkExportStatus(true);
            });
        },

        changeDefaultCurrency: function(ui) {
            var item = jq(ui);
            AjaxPro.onLoading = function(b) {
                if (b) {
                    item.attr("disabled", true);
                    item.next().show();
                    item.next('span').hide();
                }
                else {
                    item.attr("disabled", false);
                    item.next().hide();
                    item.next().next().show();
                }
            };

            AjaxPro.CommonSettingsView.SaveChangeSettings(item.val(), function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }
            });
        },

        showAddFieldPanel: function() {
            jq('#manageField div.containerHeaderBlock td:first').text(ASC.CRM.SettingsPage.PopupWindowText);
            jq('#manageField div.action_block a.baseLinkButton').text(ASC.CRM.SettingsPage.PopupSaveButtonText);
            _resetManageFieldPanel();
            RemoveRequiredErrorClass(jq("#manageField dl input:first"));
            jq('#manageField div.action_block a.baseLinkButton').unbind('click').click(function() {
                _createField();
            });
            ASC.CRM.Common.blockUI("#manageField", 400, 400, 0);
        },

        deleteField: function() {
            jq("#customFieldActionMenu").hide();
            jq('#customFieldList .crm-menu.active').removeClass('active');

            var fieldid = jq("#customFieldActionMenu").attr('fieldid');
            if (typeof (fieldid) === "undefined" || fieldid == "") return;

            var $menu = jq("#fieldMenu_" + fieldid);
            if ($menu.length != 1) return;

            var liObj = jq($menu.parents('li').get(0));

            var view = jq.getURLParam("view");
            var entityType = view != null && view != "" ? view : "contact";

            Teamlab.removeCrmCustomField({ liObj: liObj }, entityType, fieldid, {
                before: function(params) {
                    params.liObj.find(".crm-menu").hide();
                    params.liObj.find("div.ajax_loader").show();
                },
                success: ASC.CRM.SettingsPage.CallbackMethods.delete_customField
            });
        },

        showEditFieldPanel: function() {
            jq("#customFieldActionMenu").hide();
            jq('#customFieldList .crm-menu.active').removeClass('active');

            var fieldid = jq("#customFieldActionMenu").attr('fieldid');
            if (typeof (fieldid) === "undefined" || fieldid == "") return;

            var $menu = jq("#fieldMenu_" + fieldid);
            if ($menu.length != 1) return;

            var liObj = jq($menu.parents('li').get(0));

            var index = _findIndexOfFieldByID(fieldid);
            if (index === -1) return;

            var field = ASC.CRM.SettingsPage.customFieldList[index];

            jq('#manageField div.containerHeaderBlock td:first').text(CRMJSResources.EditSelectedCustomField.replace('{0}', field.label));
            jq('#manageField div.action_block a.baseLinkButton').text(CRMJSResources.SaveChanges);
            _resetManageFieldPanel();

            jq("#manageField dl input:first").val(field.label);
            jq("#manageField dl select:first").val(field.fieldType);

            jq("#manageField dl .field_mask").hide();

            switch (field.fieldType) {
                case 0:
                    jq("#text_field_size").val(field.maskObj.size);
                    jq("#manageField dl .text_field").show();
                    break;
                case 1:
                    jq("#textarea_field_rows").val(field.maskObj.rows);
                    jq("#textarea_field_cols").val(field.maskObj.cols);
                    jq("#manageField dl .textarea_field").show();
                    break;
                case 2:
                    for (var i = 0; i < field.maskObj.length; i++) {
                        ASC.CRM.SettingsPage.toSelectBox(jq("#addOptionButton"));
                        jq("#manageField dd.select_options ul li:last input").val(field.maskObj[i])
                    }
                    jq("#manageField dl .select_options").show();
                    break;
                default:
                    break;
            }

            RemoveRequiredErrorClass(jq("#manageField dl input:first"));
            jq('#manageField div.action_block a.baseLinkButton').unbind('click').click(function() {
                _editField(liObj, fieldid, index);
            });

            ASC.CRM.Common.blockUI("#manageField", 400, 400, 0);
        },

        selectTypeEvent: function(selectObj) {
            var idx = selectObj.selectedIndex;
            var which = parseInt(selectObj.options[idx].value);
            jq("#manageField dl .field_mask").hide();

            switch (which) {
                case 0:
                    jq("#manageField dl .text_field").show();
                    break;
                case 1:
                    jq("#manageField dl .textarea_field").show();
                    break;
                case 2:
                    jq("#manageField dl .select_options").show();
                    break;
            }
        },

        toSelectBox: function(buttonObj) {
            var ulObj = jq(buttonObj).prev();
            ulObj.children(":first").clone().show().appendTo(ulObj).focus();
        },

        getLinkContactStringByIndex: function(count, typeUrl, view) {
            var relativeItemsString = "";
            var oneElement = "";
            var manyElements = "";
            if (typeUrl == null) typeUrl = "";
            if (view == null) view = "";

            switch (typeUrl.trim()) {
                case "deal_milestone":
                    oneElement = CRMJSResources.OneDeal;
                    manyElements = CRMJSResources.ManyDeals;
                    break;
                case "contact_stage":
                    oneElement = CRMJSResources.OneContact;
                    manyElements = CRMJSResources.ManyContacts;
                    break;
                case "task_category":
                    oneElement = CRMJSResources.OneTask;
                    manyElements = CRMJSResources.ManyTasks;
                    break;
                case "history_category":
                    oneElement = CRMJSResources.OneEvent;
                    manyElements = CRMJSResources.ManyEvents;
                    break;
                case "custom_field":
                case "tag":
                    switch (view.trim()) {
                        case "deals":
                            oneElement = CRMJSResources.OneDeal;
                            manyElements = CRMJSResources.ManyDeals;
                            break;
                        case "cases":
                            oneElement = CRMJSResources.OneCase;
                            manyElements = CRMJSResources.ManyCases;
                            break;
                        case "company":
                            oneElement = CRMJSResources.OneCompany;
                            manyElements = CRMJSResources.ManyCompanies;
                            break;
                        case "people":
                            oneElement = CRMJSResources.OnePerson;
                            manyElements = CRMJSResources.ManyPersons;
                            break;
                        default:
                            oneElement = CRMJSResources.OneContact;
                            manyElements = CRMJSResources.ManyContacts;
                            break;
                    }
            }

            if (count == 0)
                return ASC.CRM.SettingsPage.getNoContactString(typeUrl, view);

            if (count == 1)
                relativeItemsString = "1 " + oneElement;

            else
                relativeItemsString = count + " " + manyElements;

            return relativeItemsString;
        },

        getNoContactString: function(typeUrl, view) {
            var noElements = "";
            if (typeUrl == null) typeUrl = "";
            if (view == null) view = "";

            switch (typeUrl.trim()) {
                case "deal_milestone":
                    noElements = CRMJSResources.NoDeals;
                    break;
                case "contact_stage":
                    noElements = CRMJSResources.NoContacts;
                    break;
                case "task_category":
                    noElements = CRMJSResources.NoTasks;
                    break;
                case "history_category":
                    noElements = CRMJSResources.NoEvents;
                    break;
                case "custom_field":
                case "tag":
                    switch (view.trim()) {
                        case "deals":
                            noElements = CRMJSResources.NoDeals;
                            break;
                        case "cases":
                            noElements = CRMJSResources.NoCases;
                            break;
                        case "company":
                            noElements = CRMJSResources.NoCompanies;
                            break;
                        case "people":
                            noElements = CRMJSResources.NoPersons;
                            break;
                        default:
                            noElements = CRMJSResources.NoContacts;
                            break;
                    }
            }
            return noElements;
        },

        toggleCollapceExpand: function(elem) {
            $Obj = jq(elem);
            var isCollapse = $Obj.hasClass("headerCollapse");
            if (isCollapse) {
                $Obj.parents('li').nextUntil('#customFieldList li.expand_collapce_element').show();
            }
            else {
                $Obj.parents('li').nextUntil('#customFieldList li.expand_collapce_element').hide();
            }
            $Obj.toggleClass('headerExpand');
            $Obj.toggleClass('headerCollapse');
        },

        changeAuthentication: function() {
            if (jq("#cbxAuthentication").is(":checked")) {
                jq("#tbxHostLogin").removeAttr("disabled");
                jq("#tbxHostPassword").removeAttr("disabled");
            }
            else {
                jq("#tbxHostLogin").attr("disabled", true);
                jq("#tbxHostPassword").attr("disabled", true);
            }
        },

        saveSMTPSettings: function() {
            jq("#smtpSettingsContent div.errorBox").remove();
            jq("#smtpSettingsContent div.okBox").remove();

            var host = jq("#tbxHost").val().trim();
            var port = jq("#tbxPort").val().trim();
            var authentication = jq("#cbxAuthentication").is(":checked");
            var hostLogin = jq("#tbxHostLogin").val().trim();
            var hostPassword = jq("#tbxHostPassword").val().trim();
            var senderDisplayName = jq("#tbxSenderDisplayName").val().trim();
            var senderEmailAddress = jq("#tbxSenderEmailAddress").val().trim();
            var enableSSL = jq("#cbxEnableSSL").is(":checked");

            var isValid = true;
            if (authentication && (host == "" || port == "" || hostLogin == "" || hostPassword == "" || senderDisplayName == "" || senderEmailAddress == ""))
                isValid = false;
            if (!authentication && (host == "" || port == "" || senderDisplayName == "" || senderEmailAddress == ""))
                isValid = false;

            if (!isValid) {
                jq("#smtpSettingsContent").prepend(
                    jq("<div></div>").addClass("errorBox").text(CRMJSResources.EmptyFieldsOfSettings)
                );
                return;
            }

            AjaxPro.onLoading = function(b) { if (b) { } else { } };

            AjaxPro.CommonSettingsView.SaveSMTPSettings(host, port, authentication, hostLogin, hostPassword, senderDisplayName, senderEmailAddress, enableSSL, function(res) {
                if (res.error != null) {
                    jq("#smtpSettingsContent").prepend(
                        jq("<div></div>").addClass("errorBox").text(res.error.Message)
                    );
                    return;
                }
                jq("#smtpSettingsContent div.errorBox").remove();
                jq("#smtpSettingsContent").prepend(
                    jq("<div></div>").addClass("okBox").text(CRMJSResources.SettingsUpdated)
                );
                setTimeout(function() {
                    jq("#smtpSettingsContent div.okBox").remove();
                }, 3000);
            });
        },

        checkExportStatus: function(isFirstVisit) {

            AjaxPro.onLoading = function(b) { };

            if (isFirstVisit) {
                ASC.CRM.SettingsPage.closeExportProgressPanel();
            }

            AjaxPro.CommonSettingsView.GetStatus(function(res) {
                if (res.error != null) { alert(res.error.Message); return false; }

                if (res.value == null) {
                    ASC.CRM.SettingsPage.closeExportProgressPanel();
                    return false;
                }

                jq("#exportDataContent div.progress_box div.progress").css("width", parseInt(res.value.Percentage) + "%");
                jq("#exportDataContent div.progress_box span.percent").text(parseInt(res.value.Percentage) + "%");
                jq("#exportDataContent a.baseLinkButton").hide();
                jq("#exportDataContent div.progress_box").parent().show();
                jq("#exportDataContent p.headerBaseSmall").show();
                jq("#exportDataContent #abortButton").show();
                jq("#exportDataContent #okButton").hide();

                if (res.value.Error != null && res.value.Error != "") {
                    ASC.CRM.SettingsPage.buildErrorList(res);
                } else {
                    if (res.value.IsCompleted) {
                        jq("#exportDataContent #exportLinkBox span").html(
                            jq("<a></a>").attr("href", res.value.Status).text("exportdata.zip")
                        );
                        jq("#exportDataContent p.headerBaseSmall").hide();
                        jq("#exportDataContent #exportLinkBox").show();
                        jq("#exportDataContent #abortButton").hide();
                        jq("#exportDataContent #okButton").show();
                    } else {
                        setTimeout("ASC.CRM.SettingsPage.checkExportStatus(false)", 3000);
                    }
                }
            });
        },

        abortExport: function() {
            AjaxPro.onLoading = function(b) { };
            AjaxPro.CommonSettingsView.Cancel(function(res) {
                if (res.error != null) { alert(res.error.Message); return; }
                ASC.CRM.SettingsPage.closeExportProgressPanel();
            });
        },

        closeExportProgressPanel: function() {
            jq("#exportDataContent div.progress_box div.progress").css("width", "0%");
            jq("#exportDataContent div.progress_box span.percent").text("0%");
            jq("#exportDataContent #abortButton").show();
            jq("#exportDataContent #okButton").hide();
            jq("#exportDataContent div.progress_box").parent().hide();
            jq("#exportDataContent a.baseLinkButton").show();
            jq("#exportDataContent #exportErrorBox").hide();
            jq("#exportDataContent #exportLinkBox").hide();
            jq("#exportDataContent p.headerBaseSmall").hide();
            jq("#exportDataContent div.progressErrorBox").html("");
            jq("#exportDataContent #exportLinkBox span").html("");
        },

        buildErrorList: function(res) {
            var mess = "error";
            switch (typeof res.value.Error) {
                case "object":
                    mess = res.value.Error.Message + "<br/>";
                    break;
                case "string":
                    mess = res.value.Error;
                    break;
            }

            jq("#exportDataContent div.progressErrorBox").html(
                jq("<div></div>").addClass("redText").html(mess)
            );
            jq("#exportDataContent #exportErrorBox").show();
        }
    };
})(jQuery);


ASC.CRM.ListItemView = (function($) {
    var _initSortableElements = function() {
        jq("#listView").sortable({
            cursor: "move",
            handle: '.sort_drag_handle',
            items: 'li',
            start: function(event, ui) {
                jq("#colorsPanel").hide();
                jq(".iconsPanelSettings").hide();
                jq("#popup_colorsPanel").hide();
                jq("#listItemActionMenu").hide();
                jq('#listView .crm-menu.active').removeClass('active');
            },
            beforeStop: function(event, ui) {
                jq(ui.item[0]).attr("style", "");
            },
            update: function(event, ui) {
                var itemsName = new Array();

                jq("#listView li[id*='list_item_id_']").each(function() {
                    itemsName.push(jq(this).find("td.headerBaseSmall").text());
                });

                AjaxPro.onLoading = function(b) { };
                AjaxPro.ListItemView.ReorderItems(itemsName, ASC.CRM.ListItemView.CurrentType, function(res) {
                    if (res.error != null) { }
                });
            }
        });
    };

    var _initElementsActionMenu = function() {
        if (jq("#listItemActionMenu").length != 1) return;

        jq("#listItemActionMenu").show();
        var left = jq("#listItemActionMenu .dropDownCornerRight").position().left;
        jq("#listItemActionMenu").hide();

        jq.dropdownToggle({
            dropdownID: 'listItemActionMenu',
            switcherSelector: '#listView .crm-menu',
            addTop: 4,
            addLeft: -left + 6,
            showFunction: function(switcherObj, dropdownItem) {
                jq('#listView .crm-menu.active').removeClass('active');
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass('active');
                    if (switcherObj.attr('listitemid') != dropdownItem.attr('listitemid')) {
                        dropdownItem.attr('listitemid', switcherObj.attr('listitemid'));
                    }
                }
            },
            hideFunction: function() {
                jq('#listView .crm-menu.active').removeClass('active');
            }
        });
    };


    var _findIndexOfItemByID = function(id) {
        for (var i = 0, n = ASC.CRM.ListItemView.itemList.length; i < n; i++) {
            if (ASC.CRM.ListItemView.itemList[i].id == id) {
                return i;
            }
        }
        return -1;
    };

    var _resetManageItemPanel = function() {
        jq("#manageItem input").val("");
        jq("#manageItem textarea").val("");

        if (ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3) {
            var $iconsPanel = jq("#iconsPanel_" + ASC.CRM.ListItemView.CurrentType);
            if ($iconsPanel.length == 1) {
                var $first_icon = $iconsPanel.children('img:first');
                if ($first_icon.length == 1) {
                    jq("#manageItem img.selectedIcon").attr('src', $first_icon.attr('src'));
                    jq("#manageItem img.selectedIcon").attr('title', $first_icon.attr('title'));
                    jq("#manageItem img.selectedIcon").attr('alt', $first_icon.attr('alt'));
                }
            }
        }
        else {
            var $divColors = jq('#colorsPanel > div').not(jq('#colorsPanel div.popup-corner'));
            var ind = Math.floor(Math.random() * $divColors.length);
            jq("#manageItem .selectedColor").css("background-color", ASC.CRM.Common.getHexRGBColor(jq($divColors.get(ind)).css("background-color")));
        }
    };

    var _changeColor = function(Obj, listItemId, color) {
        AjaxPro.ListItemView.ChangeColor(listItemId, color, function(res) {
            if (res.error != null) { alert(res.error.Message); return; }

            var index = _findIndexOfItemByID(listItemId);
            if (index === -1) return;
            ASC.CRM.ListItemView.itemList[index].color = res.value;

            jq(Obj).css('background-color', res.value);
            jq("#colorsPanel").hide();
        });
    };

    var _changeIcon = function(Obj, listItemId, src, title, alt) {
        AjaxPro.onLoading = function(b) {
            if (b) {
                jq(Obj).hide();
                jq(Obj).parent().find('div.ajax_change_icon').show();
            }
            else { }
        }

        var icon = src.split('/')[src.split('/').length - 1];
        AjaxPro.ListItemView.ChangeIcon(listItemId, icon, function(res) {
            if (res.error != null) { alert(res.error.Message); return; }


            var index = _findIndexOfItemByID(listItemId);
            if (index === -1) return;
            ASC.CRM.ListItemView.itemList[index].imageName = icon;

            jq(Obj).attr('src', src);
            jq(Obj).attr('title', title);
            jq(Obj).attr('alt', alt);
            jq(Obj).parent().find('div.ajax_change_icon').hide();
            jq(Obj).show();
            jq(".iconsPanelSettings").hide();
        });
    };

    var _getIconBySrc = function(src) {
        var $iconsPanel = jq('#iconsPanel_' + ASC.CRM.ListItemView.CurrentType);
        if ($iconsPanel.length != 1)
            return null;

        var parts = src.split('/');
        var id = parts[parts.length - 1].split('.')[0];

        var $icon = $iconsPanel.find('#' + id);
        if ($icon.length != 1) return null;
        return $icon;
    };

    var _itemFactory = function(item) {
        item.relativeItemsString = ASC.CRM.SettingsPage.getLinkContactStringByIndex(item.relativeItemsCount, jq.getURLParam("type"), null);
        if (ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3) {
            if (item.hasOwnProperty("imagePath") && item.imagePath != "") {
                var $icon = _getIconBySrc(item.imagePath);
                if ($icon != null) {
                    item.imageTitle = $icon.attr('title');
                    item.imageAlt = $icon.attr('alt');
                }
                else {
                    item.imageTitle = "";
                    item.imageAlt = "";
                }
            }
            else {
                item.imageTitle = "";
                item.imageAlt = "";
            }
        }
    };

    var _validateDataItem = function() {
        if (jq("#manageItem input:first").val().trim() == "") {
            ShowRequiredError(jq("#manageItem input:first"), true);
            return false;
        } else {
            RemoveRequiredErrorClass(jq("#manageItem input:first"));
            return true;
        }
    };

    var _readItemData = function(sortOrder) {
        var item = {
            title: jq("#manageItem input:first").val().trim(),
            description: jq("#manageItem textarea").val().trim(),
            sortOrder: sortOrder
        };

        if (ASC.CRM.ListItemView.CurrentType === 1) {
            item.color = ASC.CRM.Common.getHexRGBColor(jq("#manageItem .selectedColor").css("background-color"));
        }
        if (ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3) {
            var icon = jq("#manageItem img.selectedIcon").attr('src');
            item.imageName = icon.split('/')[icon.split('/').length - 1];
        }

        return item;
    };

    var _createItem = function() {
        if (!_validateDataItem()) return;

        var $listItems = jq("#listView li");

        var item = _readItemData($listItems.length);

        var index = 0;
        for (var i = 0, len = $listItems.length; i < len; i++) {
            if (jq($listItems[i]).find("td.item_title").text().trim() == item.title) {
                index = i + 1;
                break;
            }
        }
        if (index == 0) {
            Teamlab.addCrmListItem({}, ASC.CRM.ListItemView.CurrentType, item, {
                before: function(params) {
                    jq("#manageItem .action_block").hide();
                    jq("#manageItem .ajax_info_block").show();
                },
                after: function(params) {
                    jq("#manageItem .action_block").show();
                    jq("#manageItem .ajax_info_block").hide();
                },
                success: ASC.CRM.ListItemView.CallbackMethods.add_item
            });
        }
        else {
            ASC.CRM.Common.animateElement({
                element: jq($listItems[index - 1]),
                afterScrollFunc: jq.unblockUI()
            });
        }
    };

    var _editItem = function(liObj, id, indexOfItem) {
        if (!_validateDataItem()) return;

        var $listItems = jq("#listView li");

        var item = _readItemData(indexOfItem);
        item.id = id;

        var index = 0;
        for (var i = 0, len = $listItems.length; i < len; i++) {
            if (jq($listItems[i]).find("td.item_title").text().trim() == item.title && jq($listItems[i]).not(jq(liObj)).length != 0) {
                index = i + 1;
                break;
            }
        }
        if (index == 0) {
            Teamlab.updateCrmListItem({ liObj: liObj, indexOfItem: indexOfItem }, ASC.CRM.ListItemView.CurrentType, item.id, item, {
                before: function(params) {
                    jq("#manageItem .action_block").hide();
                    jq("#manageItem .ajax_info_block").show();
                },
                after: function(params) {
                    jq("#manageItem .action_block").show();
                    jq("#manageItem .ajax_info_block").hide();
                },
                success: ASC.CRM.ListItemView.CallbackMethods.edit_item
            });
        }
        else {
            ASC.CRM.Common.animateElement({
                element: jq($listItems[index - 1]),
                afterScrollFunc: jq.unblockUI()
            });
        }
    };

    return {
        CallbackMethods: {
            delete_item: function(params, item) {
                params.liObj.remove();

                var index = _findIndexOfItemByID(item.id);
                if (index != -1) ASC.CRM.ListItemView.itemList.splice(index, 1);

                if (jq('#listView li').length == 1) {
                    jq(jq('#listView li').get(0)).find(".deleteButtonImg").hide();
                }
            },

            add_item: function(params, item) {
                _itemFactory(item);
                ASC.CRM.ListItemView.itemList.push(item);

                var $itemHTML = jq("#listItemsTmpl").tmpl(item);
                if (jq("#listView li").length == 1) {
                    jq($listItems[0]).find(".deleteButtonImg").show();
                }
                $itemHTML.appendTo("#listView");

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            },

            edit_item: function(params, item) {
                _itemFactory(item);
                ASC.CRM.ListItemView.itemList[params.indexOfItem] = item;

                var $itemHTML = jq("#listItemsTmpl").tmpl(item);
                jq(params.liObj).hide();
                $itemHTML.insertAfter(params.liObj);
                jq(params.liObj).remove();

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            }
        },

        init: function(currentType) {
            ASC.CRM.ListItemView.CurrentType = currentType;

            if (typeof (itemList) != "undefined")
                itemList = jq.parseJSON(jQuery.base64.decode(itemList)).response;
            else
                itemList = [];
            //itemList = Teamlab.create('crm-listitem', null, itemList);

            for (var i = 0, len = itemList.length; i < len; i++) {
                _itemFactory(itemList[i]);
            }
            ASC.CRM.ListItemView.itemList = itemList;

            jq("#listItemsTmpl").tmpl(ASC.CRM.ListItemView.itemList).appendTo("#listView");

            if (ASC.CRM.ListItemView.itemList.length == 1) {
                jq("#listView li .deleteButtonImg").hide();
            }

            _initSortableElements();
            _initElementsActionMenu();

            ASC.CRM.ListItemView.IsDropdownToggleRegistered = false;
            if (ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3) {
                jq.dropdownToggle({ dropdownID: 'iconsPanel_' + ASC.CRM.ListItemView.CurrentType, switcherSelector: '#listView img.currentIcon', noActiveSwitcherSelector: '#manageItem .change_icon', addTop: 5, addLeft: -20 });
            }
            else {
                jq.dropdownToggle({ dropdownID: 'colorsPanel', switcherSelector: '#listView .currentColor', noActiveSwitcherSelector: "#manageItem .change_color", addTop: 5, addLeft: -20 });
            }
        },

        showAddItemPanel: function() {
            jq('#manageItem div.containerHeaderBlock td:first').text(ASC.CRM.ListItemView.PopupWindowText);
            jq('#manageItem div.action_block a.baseLinkButton').text(ASC.CRM.ListItemView.PopupSaveButtonText);
            _resetManageItemPanel();

            jq('#manageItem div.action_block a.baseLinkButton').unbind('click').click(function() {
                _createItem();
            });
            RemoveRequiredErrorClass(jq("#manageItem input:first"));
            PopupKeyUpActionProvider.CloseDialogAction = "javascript:jq('.iconsPanelSettings').hide();jq('#colorsPanel').hide();jq('#popup_colorsPanel').hide();";
            ASC.CRM.Common.blockUI("#manageItem", 400, 400, 0);

            if (!ASC.CRM.ListItemView.IsDropdownToggleRegistered) {
                ASC.CRM.ListItemView.IsDropdownToggleRegistered = true;
                if (ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3) {
                    jq.dropdownToggle({ dropdownID: 'popup_iconsPanel_' + ASC.CRM.ListItemView.CurrentType, switcherSelector: '#manageItem .change_icon', noActiveSwitcherSelector: '#listView img.currentIcon', addTop: 5, addLeft: jq("#manageItem .change_icon").width() - 30, position: "fixed" });
                }
                else {
                    jq.dropdownToggle({ dropdownID: 'popup_colorsPanel', switcherSelector: '#manageItem .change_color', noActiveSwitcherSelector: '#listView .currentColor', addTop: 5, addLeft: jq("#manageItem .change_color").width() - 21, position: "fixed" });
                }
            }
        },

        showEditItemPanel: function() {
            jq("#listItemActionMenu").hide();
            jq('#listView .crm-menu.active').removeClass('active');

            var listitemid = jq("#listItemActionMenu").attr('listitemid');
            if (typeof (listitemid) === "undefined" || listitemid == "") return;

            var index = _findIndexOfItemByID(listitemid);
            if (index === -1) return;
            var item = ASC.CRM.ListItemView.itemList[index];

            var liObj = jq("#list_item_id_" + listitemid);
            if (liObj.length != 1) return;

            jq('#manageItem div.containerHeaderBlock td:first').text(ASC.CRM.ListItemView.PopupWindowEditButtonText.replace('{0}', item.title));
            jq('#manageItem div.action_block a.baseLinkButton').text(CRMJSResources.SaveChanges);

            jq('#manageItem input:first').val(item.title);
            jq('#manageItem textarea').val(item.description);
            jq('#manageItem div.add_params input').val(item.additionalParams);

            if (jq(liObj).find("img.currentIcon").length > 0) {
                var currIcon = jq(liObj).find("img.currentIcon");
                jq("#manageItem img.selectedIcon").attr("src", jq(currIcon).attr("src"));
                jq("#manageItem img.selectedIcon").attr("title", jq(currIcon).attr("title"));
                jq("#manageItem img.selectedIcon").attr("alt", jq(currIcon).attr("alt"));
            }
            else {
                var currentColor = jq(liObj).find("div.currentColor").css("background-color");
                jq("#manageItem .selectedColor").css("background-color", currentColor);
            }

            jq('#manageItem div.action_block a.baseLinkButton').unbind('click').click(function() {
                _editItem(liObj, item.id, index);
            });
            RemoveRequiredErrorClass(jq("#manageItem input:first"));
            PopupKeyUpActionProvider.CloseDialogAction = "javascript:jq('.iconsPanelSettings').hide();jq('#colorsPanel').hide();jq('#popup_colorsPanel').hide();";
            ASC.CRM.Common.blockUI("#manageItem", 400, 400, 0);

            if (!ASC.CRM.ListItemView.IsDropdownToggleRegistered) {
                ASC.CRM.ListItemView.IsDropdownToggleRegistered = true;
                if (ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3) {
                    jq.dropdownToggle({ dropdownID: 'popup_iconsPanel_' + ASC.CRM.ListItemView.CurrentType, switcherSelector: '#manageItem .change_icon', noActiveSwitcherSelector: '#listView img.currentIcon', addTop: 5, addLeft: jq("#manageItem .change_icon").width() - 30, position: "fixed" });
                }
                else {
                    jq.dropdownToggle({ dropdownID: 'popup_colorsPanel', switcherSelector: '#manageItem .change_color', noActiveSwitcherSelector: '#listView .currentColor', addTop: 5, addLeft: jq("#manageItem .change_color").width() - 21, position: "fixed" });
                }
            }
        },

        deleteItem: function() {
            jq("#listItemActionMenu").hide();
            jq('#listView .crm-menu.active').removeClass('active');

            var listitemid = jq("#listItemActionMenu").attr('listitemid');
            if (typeof (listitemid) === "undefined" || listitemid == "") return;

            var liObj = jq("#list_item_id_" + listitemid);
            if (liObj.length != 1) return;

            if (jq("#listView li").length == 1) {
                if (ASC.CRM.ListItemView.CurrentType == 1) {
                    alert(jq.format(CRMJSResources.ErrorTheLastContactStage,
                        jq(liObj).find(".item_title").text()) + "\n" + CRMJSResources.PleaseRefreshThePage);
                    jq("#listView").find(".crm-menu").hide();
                    return;
                }
                if (ASC.CRM.ListItemView.CurrentType == 2) {
                    alert(jq.format(CRMJSResources.ErrorTheLastTaskCategory,
                        jq(liObj).find(".item_title").text()) + "\n" + CRMJSResources.PleaseRefreshThePage);
                    jq("#listView").find(".crm-menu").hide();
                    return;
                }
                return;
            }

            Teamlab.removeCrmListItem({ liObj: liObj }, ASC.CRM.ListItemView.CurrentType, listitemid, {
                before: function(params) {
                    params.liObj.find(".crm-menu").hide();
                    params.liObj.find("div.ajax_loader").show();
                },
                success: ASC.CRM.ListItemView.CallbackMethods.delete_item
            });
        },

        showColorsPanel: function(switcherUI) {
            jq('#colorsPanel > div').not(jq('#colorsPanel div.popup-corner')).unbind('click').click(function() {
                _changeColor(switcherUI, jq(switcherUI).parents('li').get(0).id.replace('list_item_id_', '') * 1, ASC.CRM.Common.getHexRGBColor(jq(this).css('background-color')));
            });
        },

        showColorsPanelToSelect: function() {
            jq('#popup_colorsPanel > div').not(jq('#popup_colorsPanel div.popup-corner')).unbind('click').click(function() {
                jq("#manageItem .selectedColor").css('background', ASC.CRM.Common.getHexRGBColor(jq(this).css("background-color")));
                jq("#popup_colorsPanel").hide();
            });
        },

        showIconsPanel: function(switcherUI) {
            var $iconsPanel = jq('#iconsPanel_' + ASC.CRM.ListItemView.CurrentType);
            if ($iconsPanel.length != 1) return;
            $iconsPanel.children('img').unbind('click').click(function() {
                _changeIcon(switcherUI, jq(switcherUI).parents('li').get(0).id.replace('list_item_id_', '') * 1, jq(this).attr('src'), jq(this).attr('title'), jq(this).attr('alt'));
            });
        },

        showIconsPanelToSelect: function() {
            var $popup_iconsPanel = jq('#popup_iconsPanel_' + ASC.CRM.ListItemView.CurrentType);
            if ($popup_iconsPanel.length != 1) return;
            $popup_iconsPanel.children('img').unbind('click').click(function() {
                jq("#manageItem img.selectedIcon").attr('src', jq(this).attr('src'));
                jq("#manageItem img.selectedIcon").attr('title', jq(this).attr('title'));
                jq("#manageItem img.selectedIcon").attr('alt', jq(this).attr('alt'));
                $popup_iconsPanel.hide();
            });
        }
    };
})(jQuery);


ASC.CRM.DealMilestoneView = (function($) {
    var _initSortableDealMilestones = function() {
        jq("#dealMilestoneList").sortable({
            cursor: "move",
            handle: '.sort_drag_handle',
            items: 'li',
            start: function(event, ui) {
                jq("#colorsPanel").hide();
                jq("#popup_colorsPanel").hide();
                jq("#dealMilestoneActionMenu").hide();
                jq('#dealMilestoneList .crm-menu.active').removeClass('active');
            },
            beforeStop: function(event, ui) {
                jq(ui.item[0]).attr("style", "");
            },
            update: function(event, ui) {
                var itemsIDs = new Array();

                jq("#dealMilestoneList li[id^='deal_milestone_id_']").each(function() {
                    itemsIDs.push(jq(this).attr("id").replace("deal_milestone_id_", "") * 1);
                });

                AjaxPro.DealMilestoneView.ReorderDealMilestone(itemsIDs, function(res) {
                    if (res.error != null) { }
                });
            }
        });
    };

    var _initDealMilestonesActionMenu = function() {
        jq("#dealMilestoneActionMenu").show();
        var left = jq("#dealMilestoneActionMenu .dropDownCornerRight").position().left;
        jq("#dealMilestoneActionMenu").hide();

        jq.dropdownToggle({
            dropdownID: 'dealMilestoneActionMenu',
            switcherSelector: '#dealMilestoneList .crm-menu',
            addTop: 4,
            addLeft: -left + 6,
            showFunction: function(switcherObj, dropdownItem) {
                jq('#dealMilestoneList .crm-menu.active').removeClass('active');
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass('active');
                    if (switcherObj.attr('dealmilestoneid') != dropdownItem.attr('dealmilestoneid')) {
                        dropdownItem.attr('dealmilestoneid', switcherObj.attr('dealmilestoneid'));
                    }
                }
            },
            hideFunction: function() {
                jq('#dealMilestone .crm-menu.active').removeClass('active');
            }
        });
    };

    var _resetManageItemPanel = function() {
        jq("#manageDealMilestone .title").val("");
        jq("#manageDealMilestone .probability").val("0");
        jq("#manageDealMilestone textarea").val("");
        jq("#manageDealMilestone [name=deal_milestone_status]:first").attr("checked", true);

        var $divColors = jq('#colorsPanel > div').not(jq('#colorsPanel div.popup-corner'));
        var ind = Math.floor(Math.random() * $divColors.length);
        jq("#manageDealMilestone .selectedColor").css("background-color", ASC.CRM.Common.getHexRGBColor(jq($divColors.get(ind)).css("background-color")));
    };

    var _findIndexOfDealMilestoneByID = function(id) {
        for (var i = 0, n = ASC.CRM.DealMilestoneView.dealMilestoneList.length; i < n; i++) {
            if (ASC.CRM.DealMilestoneView.dealMilestoneList[i].id == id) {
                return i;
            }
        }
        return -1;
    };

    var _changeColor = function(Obj, listItemId, color) {
        AjaxPro.DealMilestoneView.ChangeColor(listItemId, color, function(res) {
            if (res.error != null) { alert(res.error.Message); return; }
            jq(Obj).css('background', res.value);
            jq("#colorsPanel").hide();
        });
    };

    var _dealMilestoneItemFactory = function(dealMilestone) {
        dealMilestone.relativeItemsString = ASC.CRM.SettingsPage.getLinkContactStringByIndex(dealMilestone.relativeItemsCount, jq.getURLParam("type"), null);
    };

    var _readDealMilestoneData = function(sortOrder) {
        var dealMilestone =
        {
            title: jq("#manageDealMilestone .title").val().trim(),
            sortOrder: sortOrder
        };

        dealMilestone.description = jq("#manageDealMilestone dl textarea").val().trim();
        dealMilestone.color = ASC.CRM.Common.getHexRGBColor(jq("#manageDealMilestone .selectedColor").css("background-color"));

        var percent = jq("#manageDealMilestone .probability").val();
        if (percent * 1 > 100) { percent = "100"; }

        dealMilestone.successProbability = percent;
        dealMilestone.stageType = jq("#manageDealMilestone [name=deal_milestone_status]:checked").val() * 1;

        return dealMilestone;
    };

    var _editDealMilestone = function(liObj, id) {
        if (jq("#manageDealMilestone .title").val().trim() == "") {
            ShowRequiredError(jq("#manageDealMilestone .title"), true);
            return;
        }
        else RemoveRequiredErrorClass(jq("#manageDealMilestone .title"));

        var $listItems = jq("#dealMilestoneList li");

        var dealMilestone = _readDealMilestoneData($listItems.length);
        dealMilestone.id = id;

        var index = 0;
        for (var i = 0, len = $listItems.length; i < len; i++) {
            if (jq($listItems[i]).find("td.deal_milestone_title").text().trim() == dealMilestone.title && jq($listItems[i]).not(jq(liObj)).length != 0) {
                index = i + 1;
                break;
            }
        }
        if (index == 0) {
            Teamlab.updateCrmDealMilestone({ liObj: liObj }, dealMilestone.id, dealMilestone, {
                before: function() {
                    jq("#manageDealMilestone .action_block").hide();
                    jq("#manageDealMilestone .ajax_info_block").show();
                },
                after: function() {
                    jq("#manageDealMilestone .action_block").show();
                    jq("#manageDealMilestone .ajax_info_block").hide();
                },
                success: ASC.CRM.DealMilestoneView.CallbackMethods.edit_dealMilestone
            });
        }
        else {
            ASC.CRM.Common.animateElement({
                element: jq($listItems[index - 1]),
                afterScrollFunc: jq.unblockUI()
            });
        }
    };

    var _createDealMilestone = function() {
        if (jq("#manageDealMilestone .title").val().trim() == "") {
            ShowRequiredError(jq("#manageDealMilestone .title"), true);
            return;
        }
        else RemoveRequiredErrorClass(jq("#manageDealMilestone .title"));

        var $listItems = jq("#dealMilestoneList li");

        var dealMilestone = _readDealMilestoneData($listItems.length);

        var index = 0;
        for (var i = 0, len = $listItems.length; i < len; i++) {
            if (jq($listItems[i]).find("td.deal_milestone_title").text().trim() == dealMilestone.title) {
                index = i + 1;
                break;
            }
        }

        if (index == 0) {
            Teamlab.addCrmDealMilestone({}, dealMilestone, {
                before: function() {
                    jq("#manageDealMilestone .action_block").hide();
                    jq("#manageDealMilestone .ajax_info_block").show();
                },
                after: function() {
                    jq("#manageDealMilestone .action_block").show();
                    jq("#manageDealMilestone .ajax_info_block").hide();
                },
                success: ASC.CRM.DealMilestoneView.CallbackMethods.add_dealMilestone
            });
        }
        else {
            ASC.CRM.Common.animateElement({
                element: jq($listItems[index - 1]),
                afterScrollFunc: jq.unblockUI()
            });
        }
    };

    return {
        CallbackMethods: {
            add_dealMilestone: function(params, dealMilestone) {
                _dealMilestoneItemFactory(dealMilestone);

                ASC.CRM.DealMilestoneView.dealMilestoneList.push(dealMilestone);

                var $itemHTML = jq("#dealMilestoneTmpl").tmpl(dealMilestone);
                $itemHTML.appendTo("#dealMilestoneList");

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            },

            edit_dealMilestone: function(params, dealMilestone) {
                _dealMilestoneItemFactory(dealMilestone);

                var index = _findIndexOfDealMilestoneByID(dealMilestone.id);
                if (index != -1) {
                    ASC.CRM.DealMilestoneView.dealMilestoneList[index] = dealMilestone;
                }

                var $itemHTML = jq("#dealMilestoneTmpl").tmpl(dealMilestone);

                params.liObj.hide();
                $itemHTML.insertAfter(params.liObj);
                params.liObj.remove();

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            },

            delete_dealMilestone: function(params, dealMilestone) {
                params.liObj.remove();
                var index = _findIndexOfDealMilestoneByID(dealMilestone.id);
                if (index != -1) ASC.CRM.DealMilestoneView.dealMilestoneList.splice(index, 1);
            }
        },

        init: function() {
            if (typeof (dealMilestoneList) != "undefined")
                dealMilestoneList = jq.parseJSON(jQuery.base64.decode(dealMilestoneList)).response;
            else
                dealMilestoneList = [];
            ASC.CRM.DealMilestoneView.dealMilestoneList = Teamlab.create('crm-dealmilestone', null, dealMilestoneList);

            for (var i = 0, len = ASC.CRM.DealMilestoneView.dealMilestoneList.length; i < len; i++) {
                _dealMilestoneItemFactory(ASC.CRM.DealMilestoneView.dealMilestoneList[i]);
            }
            jq("#dealMilestoneTmpl").tmpl(ASC.CRM.DealMilestoneView.dealMilestoneList).appendTo("#dealMilestoneList");

            _initSortableDealMilestones();
            _initDealMilestonesActionMenu();

            ASC.CRM.DealMilestoneView.IsDropdownToggleRegistered = false;
            jq.dropdownToggle({ dropdownID: 'colorsPanel', switcherSelector: '#dealMilestoneList .currentColor', noActiveSwitcherSelector: "#manageDealMilestone .change_color", addTop: 5, addLeft: -20 });

            jq.forceIntegerOnly("#manageDealMilestone .probability");
        },

        showAddDealMilestonePanel: function() {
            jq('#manageDealMilestone div.containerHeaderBlock td:first').text(ASC.CRM.DealMilestoneView.PopupWindowText);
            jq('#manageDealMilestone div.action_block a.baseLinkButton').text(ASC.CRM.DealMilestoneView.PopupSaveButtonText);
            _resetManageItemPanel();

            jq('#manageDealMilestone div.action_block a.baseLinkButton').unbind('click').click(function() {
                _createDealMilestone();
            });
            RemoveRequiredErrorClass(jq("#manageDealMilestone .title"));

            ASC.CRM.Common.blockUI("#manageDealMilestone", 400, 400, 0);

            if (!ASC.CRM.DealMilestoneView.IsDropdownToggleRegistered) {
                ASC.CRM.DealMilestoneView.IsDropdownToggleRegistered = true;
                jq.dropdownToggle({
                    dropdownID: 'popup_colorsPanel',
                    switcherSelector: '#manageDealMilestone .change_color',
                    noActiveSwitcherSelector: '#dealMilestoneList .currentColor',
                    addTop: 5,
                    addLeft: jq("#manageDealMilestone .change_color").width() - 21,
                    position: "fixed"
                });
            }
        },

        showEditDealMilestonePanel: function() {
            jq("#dealMilestoneActionMenu").hide();
            jq('#dealMilestoneList .crm-menu.active').removeClass('active');

            var dealmilestoneid = jq("#dealMilestoneActionMenu").attr('dealmilestoneid');
            if (typeof (dealmilestoneid) === "undefined" || dealmilestoneid == "") return;

            var liObj = jq("#deal_milestone_id_" + dealmilestoneid);
            if (liObj.length != 1) return;

            var index = _findIndexOfDealMilestoneByID(dealmilestoneid);
            if (index === -1) return;
            var dealMilestone = ASC.CRM.DealMilestoneView.dealMilestoneList[index];


            jq('#manageDealMilestone div.containerHeaderBlock td:first').text(ASC.CRM.DealMilestoneView.PopupWindowEditButtonText.replace('{0}', dealMilestone.title));
            jq('#manageDealMilestone div.action_block a.baseLinkButton').text(CRMJSResources.SaveChanges);

            jq('#manageDealMilestone .title').val(dealMilestone.title);
            jq('#manageDealMilestone textarea').val(dealMilestone.description);
            jq('#manageDealMilestone .probability').val(dealMilestone.successProbability);
            jq("#manageDealMilestone [name=deal_milestone_status][value=" + dealMilestone.stageType + "]").attr("checked", true);

            jq('#manageDealMilestone div.action_block a.baseLinkButton').unbind('click').click(function() {
                _editDealMilestone(liObj, dealMilestone.id);
            });
            jq("#manageDealMilestone .selectedColor").css("background-color", dealMilestone.color);

            RemoveRequiredErrorClass(jq("#manageDealMilestone .title"));
            ASC.CRM.Common.blockUI("#manageDealMilestone", 400, 400, 0);

            if (!ASC.CRM.DealMilestoneView.IsDropdownToggleRegistered) {
                ASC.CRM.DealMilestoneView.IsDropdownToggleRegistered = true;
                jq.dropdownToggle({
                    dropdownID: 'popup_colorsPanel',
                    switcherSelector: '#manageDealMilestone .change_color',
                    noActiveSwitcherSelector: '#dealMilestoneList .currentColor',
                    addTop: 5,
                    addLeft: jq("#manageDealMilestone .change_color").width() - 21,
                    position: "fixed"
                });
            }
        },

        deleteDealMilestone: function() {
            jq("#dealMilestoneActionMenu").hide();
            jq('#dealMilestoneList .crm-menu.active').removeClass('active');

            var dealmilestoneid = jq("#dealMilestoneActionMenu").attr('dealmilestoneid');
            if (typeof (dealmilestoneid) === "undefined" || dealmilestoneid == "") return;

            var liObj = jq("#deal_milestone_id_" + dealmilestoneid);
            if (liObj.length != 1) return;

            if (jq('#dealMilestoneList li').length == 0) {
                alert(CRMJSResources.ErrorTheLastDealMilestone);
                return;
            }

            Teamlab.removeCrmDealMilestone({ liObj: liObj }, dealmilestoneid, {
                before: function(params) {
                    params.liObj.find(".crm-menu").hide();
                    params.liObj.find("div.ajax_loader").show();
                },
                success: ASC.CRM.DealMilestoneView.CallbackMethods.delete_dealMilestone
            });
        },

        showColorsPanel: function(switcherUI) {
            jq('#colorsPanel > div').not(jq('#colorsPanel div.popup-corner')).unbind('click').click(function() {
                _changeColor(switcherUI, jq(switcherUI).parents('li').get(0).id.replace('deal_milestone_id_', '') * 1, ASC.CRM.Common.getHexRGBColor(jq(this).css('background-color')));
            });
        },

        showColorsPanelToSelect: function() {
            jq('#popup_colorsPanel > div').not(jq('#popup_colorsPanel div.popup-corner')).unbind('click').click(function() {
                jq("#manageDealMilestone .selectedColor").css('background', ASC.CRM.Common.getHexRGBColor(jq(this).css("background-color")));
                jq("#popup_colorsPanel").hide();
            });
        }
    };
})(jQuery);


ASC.CRM.TagSettingsView = (function($) {
    var _tagItemFactory = function(tag, count) {
        tag.relativeItemsCount = count;
        tag.relativeItemsString = ASC.CRM.SettingsPage.getLinkContactStringByIndex(count, jq.getURLParam("type"), jq.getURLParam("view"));
    };

    return {
        CallbackMethods: {
            add_tag: function(params, tag) {
                tag = { value: tag };
                _tagItemFactory(tag, 0);
                var $itemHTML = jq("#tagRowTemplate").tmpl(tag);
                if (params.tagsCount == 0) {
                    jq('#emptyTagContent').hide();
                    jq('#tagList').show();
                    jq('#deleteUnusedTagsButton').show();
                }
                $itemHTML.appendTo("#tagList");

                ASC.CRM.Common.animateElement({
                    element: $itemHTML,
                    afterScrollFunc: jq.unblockUI()
                });
            },

            delete_tag: function(params, tag) {
                params.liObj.remove();
                if (jq("#tagList li").length == 0) {
                    jq('#tagList').hide();
                    jq('#deleteUnusedTagsButton').hide();
                    jq('#emptyTagContent').show();
                }
            }
        },

        init: function() {
            //jq("#tagList").disableSelection();

            if (typeof (tagList) == "undefined" ||
                typeof (relativeItemsCountArray) == "undefined" ||
                tagList.length != relativeItemsCountArray.length)
                return;

            for (var i = 0, len = tagList.length; i < len; i++) {
                _tagItemFactory(tagList[i], relativeItemsCountArray[i]);
            }

            if (tagList.length != 0) {
                jq("#tagList").show();
                jq("#deleteUnusedTagsButton").show();
                jq("#tagRowTemplate").tmpl(tagList).appendTo("#tagList");
            }
            else {
                jq("#emptyTagContent").show();
            }
        },

        showAddTagPanel: function() {
            jq("#tagTitle").val("");
            RemoveRequiredErrorClass(jq("#tagTitle"));
            ASC.CRM.Common.blockUI("#manageTag", 400, 400, 0);
        },

        createTag: function() {
            var tagTitle = jq.trim(jq("#tagTitle").val());

            if (tagTitle == "") {
                ShowRequiredError(jq("#tagTitle"), true);
                return;
            }
            else RemoveRequiredErrorClass(jq("#tagTitle"));

            var index = 0;
            var $listTags = jq("#tagList li");
            var tagsCount = $listTags.length;

            for (var i = 0; i < tagsCount; i++) {
                if (jq($listTags[i]).find(".title").text().trim() == tagTitle) {
                    index = i + 1;
                    break;
                }
            }

            if (index == 0) {
                var view = jq.getURLParam("view");
                var entityType = view != null && view != "" ? view : "contact";

                Teamlab.addCrmEntityTag({ view: view, tagsCount: tagsCount }, entityType, tagTitle, {
                    before: function(params) {
                        jq("#manageTag .action_block").hide();
                        jq("#manageTag .ajax_info_block").show();
                    },
                    success: ASC.CRM.TagSettingsView.CallbackMethods.add_tag,
                    after: function() {
                        jq("#manageTag .action_block").show();
                        jq("#manageTag .ajax_info_block").hide();
                    }
                });
            }
            else {
                ASC.CRM.Common.animateElement({
                    element: jq($listTags[index - 1]),
                    afterScrollFunc: jq.unblockUI()
                });
            }
        },

        deleteTag: function(deleteLink) {
            var liObj = jq(jq(deleteLink).parents('li').get(0));

            var tagTitle = liObj.find(".title").text().trim();
            var view = jq.getURLParam("view");
            var entityType = view != null && view != "" ? view : "contact";

            Teamlab.removeCrmEntityTag({ liObj: liObj }, entityType, Encoder.htmlDecode(tagTitle), {
                before: function(params) {
                    params.liObj.find(".deleteButtonImg").hide();
                    params.liObj.find("div.ajax_loader").show();
                },
                success: ASC.CRM.TagSettingsView.CallbackMethods.delete_tag
            });
        },

        deleteUnusedTags: function() {
            var view = jq.getURLParam("view");
            var entityType = view != null && view != "" ? view : "contact";

            Teamlab.removeCrmUnusedTag({}, entityType, {
                success: function(params, tags) {
                    var $listTags = jq("#tagList li");

                    for (var j = 0; j < tags.length; j++) {
                        for (var i = 0; i < $listTags.length; i++) {
                            if (jq($listTags[i]).find(".title").text().trim() == tags[j]) {
                                jq($listTags[i]).remove();
                            }
                        }
                    }
                    if (jq("#tagList li").length == 0) {
                        jq('#tagList').hide();
                        jq('#deleteUnusedTagsButton').hide();
                        jq('#emptyTagContent').show();
                    }
                }
            });
        }
    };
})(jQuery);


ASC.CRM.SettingsPage.WebToLeadFormView = (function($) {
    function _renderTable(serverData, container) {

        var residue = serverData.length % 3 > 0 ? 3 - serverData.length % 3 : 0;
        var counter = 0;
        var row;
        var cell;
        var input;
        var label;
        var indexRow = 0;

        var isCompany = jq("input[name=radio]:checked").val() == "company";

        jq(serverData).each(function() {
            if (!isCompany && (this.name == "firstName" || this.name == "lastName"))
                this.disabled = true;
            else if (isCompany && this.name == "companyName")
                this.disabled = true;
            else
                this.disabled = false;
        });

        for (var i = 0; i < serverData.length + residue; i++) {
            if (counter == 0)
                row = jq("<tr></tr>");

            cell = jq("<td></td>").attr("width", "33%");

            if (serverData[i] != null) {
                input = jq("<input>")
                                .attr("type", "checkbox")
                                .attr("id", serverData[i].name)
                                .attr("name", serverData[i].name);

                if (serverData[i].disabled)
                    jq(input).attr("checked", true).attr("disabled", true);

                label = jq("<label>").attr("for", serverData[i].name).text(serverData[i].title);

                input.data("fieldInfo", serverData[i]);

                cell.append(input).append(label);
            }

            row.append(cell);
            counter++;

            if (counter == 3 || i == serverData.length + residue - 1) {
                var cells = row.children();

                if (indexRow % 2 == 0)
                    cells.addClass("tintMedium");
                else
                    cells.addClass("tintLight");

                container.append(row);
                counter = 0;
                indexRow++;
            }
        }
    };

    function _validateInputData() {

        var regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;

        var returnURL = jq("#returnURL").val().trim();
        var webFormKey = jq("#properties_webFormKey input").val().trim();

        var isValid = true;

        if (returnURL == "" || !regexp.test(returnURL)) {
            ShowRequiredError(jq("#returnURL"));
            isValid = false;
        }
        else
            RemoveRequiredErrorClass(jq("#returnURL"));


        if (webFormKey == "") {
            ShowRequiredError(jq("#properties_webFormKey input"));
            isValid = false;
        }
        else
            RemoveRequiredErrorClass(jq("#properties_webFormKey input"));

        var firstName = jq("#tblFieldList input[name='firstName']:checked");
        var lastName = jq("#tblFieldList input[name='lastName']:checked");
        var companyName = jq("#tblFieldList input[name='companyName']:checked");

        if ((firstName.length == 0 || lastName.length == 0) && companyName.length == 0) {
            alert(CRMJSResources.ErrorNotMappingBasicColumn);
            isValid = false;
        }

        return isValid;
    };

    return {
        init: function() {

            if (!(typeof columnSelectorData === "undefined"))
                _renderTable(columnSelectorData, jq("#tblFieldList tbody"));

            jq("#privateSettingsBlock").parent().removeClass("tintMedium").css("padding", "0px");
            var text = jq("#privateSettingsBlock").parent().find("span:first").text() + ":";
            jq("#privateSettingsBlock").parent().find("span:first").replaceWith(
                jq("<div><div>").text(text).css("font-weight", "bold")
            );
        },

        changeContactType: function() {
            jq("#tblFieldList tbody").html("");

            jq(columnSelectorData).each(function() {
                this.disabled = false;
            });

            _renderTable(columnSelectorData, jq("#tblFieldList tbody"));
        },

        changeWebFormKey: function() {

            if (!confirm(CRMJSResources.ConfirmChangeKey + "\n" + CRMJSResources.ConfirmChangeKeyNote))
                return false;

            AjaxPro.onLoading = function(b) {
                if (b) {

                } else {

                }
            };

            AjaxPro.WebToLeadFormView.ChangeWebFormKey(
               function(res) {
                   if (res.error != null) {
                       alert(res.error.Message);
                   }

                   jq("#properties_webFormKey input").val(res.value);
                   jq("#webFormKeyContainer").html(res.value);
               }
            );

        },
        generateSampleForm: function() {
            if (!_validateInputData()) return;

            var fieldListInfo = jQuery.map(jq("#tblFieldList input:checked"),
                   function(a) {
                       return jq(a).data("fieldInfo");
                   });

            var tagListInfo = jq.map(jq("#tagContainer .tag_title"),
                   function(item) {
                       return {
                           name: "tag_" + jq(item).text().trim(),
                           title: jq(item).text().trim()
                       };
                   });


            if (fieldListInfo.length == 0) {

                jq("#resultContainer, #previewHeader").hide();
                jq("#previewHeader div.content").html("");

                return;
            }

            var notifyList = "";
            var privateList = "";

            if (jq("#isPrivate").is(":checked")) {
                var privateUsers = new Array(SelectedUsers.CurrentUserID);
                for (var i = 0; i < SelectedUsers.IDs.length; i++)
                    privateUsers.push(SelectedUsers.IDs[i]);

                privateList = privateUsers.join(",");

                if (jq("#cbxNotify").is(":checked"))
                    notifyList = privateUsers.join(",");
            }

            if (SelectedUsers_Notify.IDs.length != 0) {
                var notifyUsers = new Array();
                for (var i = 0, n = SelectedUsers_Notify.IDs.length; i < n; i++)
                    notifyUsers.push(SelectedUsers_Notify.IDs[i]);
                notifyList = notifyUsers.join(",");
            }

            var formContainer = jq("<div>");

            jq("#sampleFormTmpl").tmpl({
                fieldListInfo: fieldListInfo,
                tagListInfo: tagListInfo,
                returnURL: jq("#returnURL").val(),
                webFormKey: jq("#properties_webFormKey input").val(),
                notifyList: notifyList,
                privateList: privateList
            }).appendTo(formContainer);

            var sampleFrom = formContainer.html().replace(/\s{1,}/g, " ");

            jq("#resultContainer textarea").val(sampleFrom);
            jq("#resultContainer").show();

            jq("#previewHeader div.content").html(sampleFrom);
            jq("#previewHeader").show();

        }
    };
})(jQuery);

ASC.CRM.TaskTemplateView = (function($) {

    function checkTemplateConatainers() {
        if (jq("#templateConatainerContent li").length != 0) {
            jq("#emptyContent").hide();
            jq("#templateConatainerContent").show();
        }
        else {
            jq("#templateConatainerContent").hide();
            jq("#emptyContent").show();
        }
    };

    return {
        init: function() {
            if (typeof (templateConatainerList) != "undefined")
                templateConatainerList = jq.parseJSON(jQuery.base64.decode(templateConatainerList)).response;
            else
                templateConatainerList = [];

            jq.forceIntegerOnly("#templatePanel #tbxTemplateDisplacement");

            jq("#templateContainerRow").tmpl(templateConatainerList).appendTo("#templateConatainerContent");
            checkTemplateConatainers();

            for (var i = 0; i < templateConatainerList.length; i++)
                if(templateConatainerList[i].items)
                    for (var j = 0; j < templateConatainerList[i].items.length; j++)
                        ASC.CRM.Common.tooltip("#templateTitle_" + templateConatainerList[i].items[j].id, "tooltip", true);

        },

        showTemplateConatainerPanel: function(id) {
            if(!id) {
                ASC.CRM.TaskTemplateView.initTemplateConatainerPanel();
                ASC.CRM.Common.blockUI("#templateConatainerPanel", 500, 500, 0);
            } else {
                Teamlab.getCrmEntityTaskTemplateContainer({ }, id, {
                    success: function(params, templateContainer) {
                        ASC.CRM.TaskTemplateView.initTemplateConatainerPanel(templateContainer);
                        ASC.CRM.Common.blockUI("#templateConatainerPanel", 500, 500, 0);
                    }
                });
            }
        },

        createTemplateConatainer: function() {
            var title = jq("#templateConatainerTitle").val().trim();
            var view = jq.getURLParam("view");
            var entityType = view != null && view != "" ? view : "contact";

            if (title == "") {
                ShowRequiredError(jq("#templateConatainerTitle"), true);
                return false;
            }
            else RemoveRequiredErrorClass(jq("#templateConatainerTitle"));

            Teamlab.addCrmEntityTaskTemplateContainer({}, {entityType : entityType, title : title}, {
                success: function(params, templateContainer) {
                    jq("#templateContainerRow").tmpl(templateContainer).appendTo("#templateConatainerContent");
                    checkTemplateConatainers();
                },
                before: function(params) {
                    jq("#templateConatainerPanel div.action_block").hide();
                    jq("#templateConatainerPanel div.ajax_info_block").show();
                },
                after: function(params) {
                    jq("#templateConatainerPanel div.ajax_info_block").hide();
                    jq("#templateConatainerPanel div.action_block").show();
                    jq.unblockUI();
                }
            });
        },

        editTemplateConatainer: function(containerid) {
            var title = jq("#templateConatainerTitle").val().trim();
            var view = jq.getURLParam("view");
            var entityType = view != null && view != "" ? view : "contact";

            if (title == "") {
                ShowRequiredError(jq("#templateConatainerTitle"), true);
                return false;
            }
            else RemoveRequiredErrorClass(jq("#templateConatainerTitle"));

            Teamlab.updateCrmEntityTaskTemplateContainer({}, containerid, {entityType : entityType, title : title}, {
                success: function(params, templateContainer) {
                    jq("#templateContainerBody_" + containerid).remove();
                    var newTemlateContainer = jq("#templateContainerRow").tmpl(templateContainer);
                    jq("#templateContainerHeader_"+containerid).replaceWith(newTemlateContainer);
                },
                before: function(params) {
                    jq("#templateConatainerPanel div.action_block").hide();
                    jq("#templateConatainerPanel div.ajax_info_block").show();
                },
                after: function(params) {
                    jq("#templateConatainerPanel div.ajax_info_block").hide();
                    jq("#templateConatainerPanel div.action_block").show();
                    jq.unblockUI();
                }
            });
        },

        deleteTemplateConatainer: function(containerid) {
            Teamlab.removeCrmEntityTaskTemplateContainer({ }, containerid, {
                success: function(params, templateContainer) {
                    jq("#templateContainerHeader_" + containerid).remove();
                    jq("#templateContainerBody_" + containerid).remove();
                    checkTemplateConatainers();
                },
                before: function(params) {
                    jq("#templateContainerHeader_" + containerid + " img.addImg").hide();
                    jq("#templateContainerHeader_" + containerid + " img.editImg").hide();
                    jq("#templateContainerHeader_" + containerid + " img.deleteImg").hide();
                    jq("#templateContainerHeader_" + containerid + " img.loaderImg").show();
                }
            });
        },

        initTemplateConatainerPanel: function(container) {
                if(!container) {
                    jq("#templateConatainerTitle").val("");
                    RemoveRequiredErrorClass(jq("#templateConatainerTitle"));
                    jq("#templateConatainerPanel div.containerHeaderBlock td:first")
                        .text(ASC.CRM.TaskTemplateView.ConatainerPanel_AddHeaderText);
                    jq("#templateConatainerPanel div.action_block a.baseLinkButton")
                        .text(ASC.CRM.TaskTemplateView.ConatainerPanel_AddButtonText)
                        .unbind("click").click(function() {
                        ASC.CRM.TaskTemplateView.createTemplateConatainer();
                    });
                } else {
                    jq("#templateConatainerTitle").val(container.title);
                    RemoveRequiredErrorClass(jq("#templateConatainerTitle"));
                    jq("#templateConatainerPanel div.containerHeaderBlock td:first")
                        .text(jq.format(ASC.CRM.TaskTemplateView.ConatainerPanel_EditHeaderText, container.title));
                    jq("#templateConatainerPanel div.action_block a.baseLinkButton")
                        .text(CRMJSResources.SaveChanges)
                        .unbind("click").click(function() {
                        ASC.CRM.TaskTemplateView.editTemplateConatainer(container.id);
                    });
                }
        },

        toggleCollapceExpand: function(elem) {
            $Obj = jq(elem);

            if ($Obj.hasClass("headerCollapse")) {
                var body = $Obj.parents("li").next();
                if (body.find("table").length > 0)
                    body.show();
            } else
                $Obj.parents("li").next().hide();

            $Obj.toggleClass("headerExpand");
            $Obj.toggleClass("headerCollapse");
        },

        showTemplatePanel: function(containerid, id) {
            if(!id) {
                ASC.CRM.TaskTemplateView.initTemplatePanel(containerid);
                ASC.CRM.Common.blockUI("#templatePanel", 500, 500, 0);
            } else {
                Teamlab.getCrmEntityTaskTemplate({ }, id, {
                    success: function(params, template) {
                        ASC.CRM.TaskTemplateView.initTemplatePanel(containerid, template);
                        ASC.CRM.Common.blockUI("#templatePanel", 500, 500, 0);
                    }
                });
            }
        },

        createTemplate: function(containerid) {
            var data = ASC.CRM.TaskTemplateView.getCurrentTemplateData();
            data.containerid = containerid;

            if (data.title == "") {
                ShowRequiredError(jq("#tbxTemplateTitle"), true);
                return false;
            }
            else RemoveRequiredErrorClass(jq("#tbxTemplateTitle"));

            if (data.responsibleid == null) {
                ShowRequiredError(jq("#templatePanel #inputUserName"), true);
                return false;
            }
            else RemoveRequiredErrorClass(jq("#templatePanel #inputUserName"));

            Teamlab.addCrmEntityTaskTemplate({}, data, {
                success: function(params, template) {
                    jq("#templateRow").tmpl(template).appendTo("#templateContainerBody_"+containerid);
                    if(jq("#templateContainerBody_"+containerid+" table").length > 0)
                        jq("#templateContainerBody_"+containerid).show();
                    ASC.CRM.Common.tooltip("#templateTitle_" + template.id, "tooltip", true);
                },
                before: function(params) {
                    jq("#templatePanel div.action_block").hide();
                    jq("#templatePanel div.ajax_info_block").show();
                },
                after: function(params) {
                    jq("#templatePanel div.ajax_info_block").hide();
                    jq("#templatePanel div.action_block").show();
                    jq.unblockUI();
                }
            });
        },

        editTemplate: function(containerid, templateid) {
            var data = ASC.CRM.TaskTemplateView.getCurrentTemplateData();
            data.containerid = containerid,
            data.id = templateid;

            if (data.title == "") {
                ShowRequiredError(jq("#tbxTemplateTitle"), true);
                return false;
            }
            else RemoveRequiredErrorClass(jq("#tbxTemplateTitle"));

            if (data.responsibleid == null) {
                ShowRequiredError(jq("#templatePanel #inputUserName"), true);
                return false;
            }
            else RemoveRequiredErrorClass(jq("#templatePanel #inputUserName"));

            Teamlab.updateCrmEntityTaskTemplate({}, data, {
                success: function(params, template) {
                    var newTemlate = jq("#templateRow").tmpl(template);
                    jq("#templateRow_"+templateid).replaceWith(newTemlate);
                    var tooltipIsExist = jq("#tooltip" + template.id).length > 0;
                    ASC.CRM.Common.tooltip("#templateTitle_" + template.id, "tooltip", !tooltipIsExist);
                },
                before: function(params) {
                    jq("#templatePanel div.action_block").hide();
                    jq("#templatePanel div.ajax_info_block").show();
                },
                after: function(params) {
                    jq("#templatePanel div.ajax_info_block").hide();
                    jq("#templatePanel div.action_block").show();
                    jq.unblockUI();
                }
            });
        },

        deleteTemplate: function(templateid) {
            Teamlab.removeCrmEntityTaskTemplate({ }, templateid, {
                success: function(params, template) {
                    jq("#templateRow_"+templateid).remove();
                    if(jq("#templateContainerBody_"+template.containerID+" table").length == 0)
                        jq("#templateContainerBody_"+template.containerID).hide();
                },
                before: function(params) {
                    jq("#templateRow_" + templateid + " img.addImg").hide();
                    jq("#templateRow_" + templateid + " img.editImg").hide();
                    jq("#templateRow_" + templateid + " img.deleteImg").hide();
                    jq("#templateRow_" + templateid + " img.loaderImg").show();
                }
            });
        },

        initTemplatePanel: function(containerid, template) {
                if(!template) {
                    jq("#templatePanel div.containerHeaderBlock td:first")
                        .text(ASC.CRM.TaskTemplateView.TemplatePanel_AddHeaderText);
                    jq("#templatePanel div.action_block a.baseLinkButton")
                        .text(ASC.CRM.TaskTemplateView.TemplatePanel_AddButtonText)
                        .unbind("click").click(function() {
                        ASC.CRM.TaskTemplateView.createTemplate(containerid);
                    });
                    jq("#tbxTemplateTitle").val("");
                    jq("#tbxTemplateDescribe").val("");
                    ASC.CRM.TaskTemplateView.setTemplateDeadlineFromTicks();
                    jq("#notifyResponsible").removeAttr("checked");
                    taskTemplateResponsibleSelector.ClearFilter();
                    taskTemplateResponsibleSelector.ChangeDepartment(taskTemplateResponsibleSelector.Groups[0].ID);
                    var obj = taskTemplateCategorySelector.getRowByContactID(0);
                    taskTemplateCategorySelector.changeContact(obj);

                } else {
                    jq("#templatePanel div.containerHeaderBlock td:first")
                        .text(jq.format(ASC.CRM.TaskTemplateView.TemplatePanel_EditHeaderText, template.title));
                    jq("#templatePanel div.action_block a.baseLinkButton")
                        .text(CRMJSResources.SaveChanges).unbind("click").click(function() {
                        ASC.CRM.TaskTemplateView.editTemplate(containerid, template.id);
                    });
                    jq("#tbxTemplateTitle").val(template.title);
                    jq("#tbxTemplateDescribe").val(template.description);
                    ASC.CRM.TaskTemplateView.setTemplateDeadlineFromTicks(template.offsetTicks, template.deadLineIsFixed);
                    jq("#notifyResponsible").attr("checked", template.isNotify);
                    taskTemplateResponsibleSelector.ClearFilter();
                    taskTemplateResponsibleSelector.ChangeDepartment(taskTemplateResponsibleSelector.Groups[0].ID);

                    var obj;
                    if (!jq.browser.mobile) {
                        obj = document.getElementById("User_" + template.responsible.id);
                        if (obj != null)
                            taskTemplateResponsibleSelector.SelectUser(obj);
                    } else {
                        obj = jq("#taskResponsibleSelector select option[value=" + template.responsible.id + "]");
                        if (obj.length > 0) {
                            taskTemplateResponsibleSelector.SelectUser(obj);
                            jq(obj).attr("selected", true);
                        }
                    }

                    obj = taskTemplateCategorySelector.getRowByContactID(template.category.id);
                    taskTemplateCategorySelector.changeContact(obj);
                }
        },

        getTemplateDeadlineTicks: function() {
            var displacement = jq("#tbxTemplateDisplacement").val().trim() != "" ? parseInt(jq("#tbxTemplateDisplacement").val()) : 0;
            var hour = displacement * 24 + parseInt(jq("#templateDeadlineHours option:selected").val());
            var minute = parseInt(jq("#templateDeadlineMinutes option:selected").val());

            return ((hour * 3600) + minute * 60) * 10000000;
        },

        setTemplateDeadlineFromTicks: function(ticks, isFixed) {
            if(!ticks && !isFixed) {
                jq("#deadLine_fixed").attr("checked", true);
                jq("#tbxTemplateDisplacement").val("0");
                jq("#templateDeadlineHours option:first").attr("selected", true);
                jq("#templateDeadlineMinutes option:first").attr("selected", true);
            } else {
                if(isFixed)
                    jq("#deadLine_fixed").attr("checked", true);
                else
                    jq("#deadLine_not_fixed").attr("checked", true);

                var minute = ((ticks/10000000)%3600)/60;
                var hour = (parseInt((ticks/10000000)/3600))%24;
                var displacement = parseInt((parseInt((ticks/10000000)/3600))/24);

                jq("#tbxTemplateDisplacement").val(displacement);
                jq("#optDeadlineHours_"+hour).attr("selected", true);
                jq("#optDeadlineMinutes_"+minute).attr("selected", true);
            }
        },

        getCurrentTemplateData: function() {
            var data = {
                title : jq("#tbxTemplateTitle").val().trim(),
                description : jq("#tbxTemplateDescribe").val().trim(),
                responsibleid : taskTemplateResponsibleSelector.SelectedUserId,
                categoryid : taskTemplateCategorySelector.CategoryID,
                isNotify : jq("#notifyResponsible").is(":checked"),
                offsetTicks : ASC.CRM.TaskTemplateView.getTemplateDeadlineTicks(),
                deadLineIsFixed : jq('#templatePanel #deadLine_fixed').is(":checked")
            };
            return data;
        },

        getDeadlineDisplacement: function(ticks, isFixed) {
            var minute = ((ticks/10000000)%3600)/60;
            var hour = (parseInt((ticks/10000000)/3600))%24;
            var displacement = parseInt((parseInt((ticks/10000000)/3600))/24);

            if(isFixed)
                return jq.format(CRMJSResources.TemplateFixedDeadline, displacement, hour, minute);
            else
                return jq.format(CRMJSResources.TemplateNotFixedDeadline, displacement, hour, minute);

        }
    };
})(jQuery);