if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = (function() { return {} })();

ASC.CRM.ListContactView = (function() {
    var _setCookie = function(page, countOnPage) {
        if (ASC.CRM.ListContactView.cookieKey && ASC.CRM.ListContactView.cookieKey != "") {
            var cookie = {
                page: page,
                countOnPage: countOnPage
            };
            jq.cookies.set(ASC.CRM.ListContactView.cookieKey, cookie, { path: location.pathname });
        }
    };

    var _lockMainActions = function() {
        jq("#mainDelete").removeClass("unlockAction").unbind("click");
        jq("#mainAddTag").removeClass("unlockAction").unbind("click");
        jq("#mainSetPermissions").removeClass("unlockAction").unbind("click");
        jq("#mainSendEmail").removeClass("unlockAction").unbind("click");
    };

    var _checkForLockMainActions = function() {
        if (ASC.CRM.ListContactView.selectedItems.length === 0) {
            _lockMainActions();
            return;
        }

        if (!jq("#mainDelete").hasClass("unlockAction")) {
            jq("#mainDelete").addClass("unlockAction").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.showDeletePanel();
            });
        }
        if (!jq("#mainAddTag").hasClass("unlockAction")) {
            jq("#mainAddTag").addClass("unlockAction").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.showAddTagDialog();
            });
        }
        if (!jq("#mainSetPermissions").hasClass("unlockAction")) {
            jq("#mainSetPermissions").addClass("unlockAction").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.showSetPermissionsPanel({ isBatch: true });
            });
        }

        var unlockSendEmail = false;

        for (var i = 0, len = ASC.CRM.ListContactView.selectedItems.length; i < len; i++)
            if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null)
            unlockSendEmail = true;

        if (unlockSendEmail) {
            jq("#mainSendEmail").addClass("unlockAction").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.showSendEmailDialog();
            });
        }
        else {
            jq("#mainSendEmail").removeClass("unlockAction").unbind("click");
        }
    };
    var _showAddPrimaryPhoneInput = function(contactId, primaryPhone) {
        var $phoneInput = jq("#addPrimaryPhone_" + contactId);
        $phoneInput.css("borderColor", "");

        var $phoneElement = jq("#contactItem_" + contactId).find(".primaryPhone");
        if ($phoneElement.length != 0) {
            $phoneElement.remove();
            if (primaryPhone != null)
                $phoneInput.val(primaryPhone.data);
        }
        else $phoneInput.val("");
        $phoneInput.show().focus();
        jq.forcePhoneSymbolsOnly($phoneInput);

        $phoneInput.unbind("blur").bind("blur", function() {
            var text = $phoneInput.val().trim();
            if (text.length == 0) { $phoneInput.val("").hide(); return; }
            else {
                _addPrimaryPhone(contactId, text, primaryPhone);
            }
        });

        $phoneInput.unbind("keyup").bind("keyup", function(event) {
            if (ASC.CRM.ListContactView.isEnterKeyPressed(event)) {
                var text = $phoneInput.val().trim();
                if (text.length == 0) { $phoneInput.val("").hide(); return; }
                else {
                    $phoneInput.unbind("blur");
                    _addPrimaryPhone(contactId, text, primaryPhone);
                }
            }
        });
    };

    var _addPrimaryPhone = function(contactId, phoneNumber, oldPrimaryPhone) {
        //            var reg = new RegExp(/(^\+)?(\d+)/);
        //            if (val == "" || !reg.test(val)) {

        if (oldPrimaryPhone == null) {
            var params = { contactId: contactId };
            var data = { data: phoneNumber, isPrimary: true, infoType: "Phone", category: "Work" };

            Teamlab.addCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_phone,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        }
        else {
            var params = { contactId: contactId };
            var data = { id: oldPrimaryPhone.id, data: phoneNumber, isPrimary: true, infoType: "Phone" };

            Teamlab.updateCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_phone,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        }
    };

    var _showAddPrimaryEmailInput = function(contactId, primaryEmail) {
        var $emailInput = jq("#addPrimaryEmail_" + contactId);
        $emailInput.css("borderColor", "");

        var $emailElement = jq("#contactItem_" + contactId).find(".primaryEmail");
        if ($emailElement.length != 0) {
            $emailElement.remove();
            if (primaryEmail != null)
                $emailInput.val(primaryEmail.data);
        }
        else $emailInput.val("");

        $emailInput.show().focus();

        $emailInput.unbind("blur").bind("blur", function() {
            var text = $emailInput.val().trim();

            if (text.length == 0) { $emailInput.val("").hide(); return; }
            else {
                if (ASC.CRM.ListContactView.isEmailValid(text)) {
                    _addPrimaryEmail(contactId, text, primaryEmail);
                }
                else {
                    $emailInput.css("borderColor", "#CC0000");
                    return false;
                }
            }
        });

        $emailInput.unbind("keyup").bind("keyup", function(event) {
            if (ASC.CRM.ListContactView.isEnterKeyPressed(event)) {
                var text = $emailInput.val().trim();
                if (text.length == 0) { $emailInput.val("").hide(); return; }
                else {
                    if (ASC.CRM.ListContactView.isEmailValid(text)) {
                        $emailInput.unbind("blur");
                        _addPrimaryEmail(contactId, text, primaryEmail);
                    }
                    else {
                        $emailInput.css("borderColor", "#CC0000");
                        return false;
                    }
                }
            }
        });
    };

    var _addPrimaryEmail = function(contactId, email, oldPrimaryEmail) {
        if (oldPrimaryEmail == null) {
            var params = { contactId: contactId };
            var data = { data: email, isPrimary: true, infoType: "Email", category: "Work" };

            Teamlab.addCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_email,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        }
        else {
            var params = { contactId: contactId };
            var data = { id: oldPrimaryEmail.id, data: email, isPrimary: true, infoType: "Email" };

            Teamlab.updateCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_email,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        }
    };

    var _contactItemFactory = function(contact, selectedIDs) {
        var index = jq.inArray(contact.id, selectedIDs);
        contact.isChecked = index != -1;

        contact.primaryPhone = null;
        contact.primaryEmail = null;
        contact.nearTask = null;

        for (var j = 0, n = contact.commonData.length; j < n; j++) {
            if (contact.commonData[j].isPrimary) {
                if (contact.commonData[j].infoType == 0) {
                    contact.primaryPhone = {
                        data: contact.commonData[j].data,
                        id: contact.commonData[j].id
                    };
                }
                if (contact.commonData[j].infoType == 1) {
                    contact.primaryEmail = {
                        data: contact.commonData[j].data,
                        id: contact.commonData[j].id
                    }
                }
            }
        }
    };

    return {
        CallbackMethods:
        {
            get_contacts_by_filter: function(params, contacts) {

                ASC.CRM.ListContactView.Total = params.__total || 0;
                var startIndex = params.__startIndex || 0;
                if (ASC.CRM.ListContactView.Total === 0 &&
                    typeof (ASC.CRM.ListContactView.advansedFilter) != "undefined" &&
                    ASC.CRM.ListContactView.advansedFilter.advansedFilter().length == 1) {
                    ASC.CRM.ListContactView.noContacts = true;
                    ASC.CRM.ListContactView.noContactsForQuery = true;
                }
                else {
                    ASC.CRM.ListContactView.noContacts = false;
                    if (ASC.CRM.ListContactView.Total === 0)
                        ASC.CRM.ListContactView.noContactsForQuery = true;
                    else
                        ASC.CRM.ListContactView.noContactsForQuery = false;
                }


                if (ASC.CRM.ListContactView.noContacts) {
                    jq("#companyTable tbody tr").remove();
                    jq("#mainContactList").hide();
                    jq("#contactsEmptyScreen").show();
                    LoadingBanner.hideLoading();
                    return false;
                }


                if (ASC.CRM.ListContactView.noContactsForQuery) {
                    jq("#companyListBox").hide();
                    jq("#companyTable tbody tr").remove();
                    jq("#tableForContactNavigation").hide();
                    jq("#mainSelectAll").attr("disabled", true);
                    jq("#mainExportCsv").next("img").hide();
                    jq("#mainExportCsv").hide();
                    jq("#contactsEmptyScreen").hide();
                    jq("#emptyContentForContactsFilter").show();
                    LoadingBanner.hideLoading();
                    return false;
                }

                jq("#totalContactsOnPage").text(ASC.CRM.ListContactView.Total);

                var tmpTotal;
                if (startIndex >= ASC.CRM.ListContactView.Total)
                    tmpTotal = startIndex + 1;
                else
                    tmpTotal = ASC.CRM.ListContactView.Total;
                contactPageNavigator.drawPageNavigator((startIndex / ASC.CRM.ListContactView.entryCountOnPage).toFixed(0) * 1 + 1, tmpTotal);

                jq("#simpleContactPageNavigator").html("");
                var $simplePN = jq("<div></div>");
                var lengthOfLinks = 0;
                if (jq("#divForContactPager .pagerPrevButtonCSSClass").length != 0) {
                    lengthOfLinks++;
                    jq("#divForContactPager .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
                }
                if (jq("#divForContactPager .pagerNextButtonCSSClass").length != 0) {
                    lengthOfLinks++;
                    if (lengthOfLinks === 2) {
                        jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
                    }
                    jq("#divForContactPager .pagerNextButtonCSSClass").clone().appendTo($simplePN);
                }
                if ($simplePN.children().length != 0) {
                    $simplePN.appendTo("#simpleContactPageNavigator");
                    jq("#simpleContactPageNavigator").show();
                }
                else
                    jq("#simpleContactPageNavigator").hide();

                if (contacts.length == 0) {//it can happen when select page without elements after deleting
                    jq("#contactsEmptyScreen").hide();
                    jq("#emptyContentForContactsFilter").hide();
                    jq("#companyListBox").show();
                    jq("#companyTable tbody tr").remove();
                    jq("#tableForContactNavigation").show();
                    jq("#mainSelectAll").attr("disabled", true);
                    jq("#mainExportCsv").next("img").hide();
                    jq("#mainExportCsv").hide();
                    LoadingBanner.hideLoading();
                    return false;
                }

                jq("#emptyContentForContactsFilter").hide();
                jq("#contactsEmptyScreen").hide();
                jq("#companyListBox").show();
                jq("#mainSelectAll").removeAttr("disabled");
                jq("#mainExportCsv").next("img").show();
                jq("#mainExportCsv").show();

                jq("#tableForContactNavigation").show();

                var selectedIDs = new Array();
                for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                    selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
                }

                for (var i = 0, n = contacts.length; i < n; i++) {
                    _contactItemFactory(contacts[i], selectedIDs);
                    ASC.CRM.ListContactView.fullContactList.push(contacts[i]);
                }

                ASC.CRM.ListContactView.getNearestTasks(contacts);
            },

            get_nearest_tasks: function(params, tasks) {
                if (tasks.length > 0) {
                    for (var i = 0, len_i = tasks.length; i < len_i; i++) {
                        for (var j = 0, len_j = ASC.CRM.ListContactView.fullContactList.length; j < len_j; j++)
                            if (tasks[i].contact.id == ASC.CRM.ListContactView.fullContactList[j].id) {
                            ASC.CRM.ListContactView.fullContactList[j].nearTask = tasks[i];
                            break;
                        }
                        for (var j = 0, len = params.contacts.length; j < len; j++)
                            if (tasks[i].contact.id == params.contacts[j].id) {
                            params.contacts[j].nearTask = tasks[i];
                            break;
                        }
                    }
                }

                jq("#companyTable tbody").replaceWith(jq("#contactListTmpl").tmpl({ contacts: params.contacts }));

                ASC.CRM.Common.RegisterContactInfoCard();
                ASC.CRM.Common.tooltip(".nearestTask", "tooltip", true);
                window.scrollTo(0, 0);
                LoadingBanner.hideLoading();
            },

            add_primary_phone: function(params, data) {
                jq("#addPrimaryPhone_" + params.contactId).hide();
                jq("<span></span>").attr("title", data.data).attr("class", "primaryPhone").text(data.data).appendTo(jq("#addPrimaryPhone_" + params.contactId).parent());
                for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.fullContactList[i].id == params.contactId)
                        ASC.CRM.ListContactView.fullContactList[i].primaryPhone = {
                            data: data.data,
                            id: data.id
                        };
                }
            },

            add_primary_email: function(params, data) {
                jq("#addPrimaryEmail_" + params.contactId).hide();
                jq("<a></a>").attr("title", data.data).attr("class", "primaryEmail linkMedium").text(data.data).attr("href", "mailto:" + data.data).appendTo(jq("#addPrimaryEmail_" + params.contactId).parent());
                jq("#addPrimaryEmailMenu").hide();
                for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.fullContactList[i].id == params.contactId)
                        ASC.CRM.ListContactView.fullContactList[i].primaryEmail = {
                            data: data.data,
                            id: data.id
                        }
                }
            },

            delete_batch_contacts: function(params, data) {
                var newFullContactList = new Array();
                for (var i = 0, len_i = ASC.CRM.ListContactView.fullContactList.length; i < len_i; i++) {
                    var isDeleted = false;
                    for (var j = 0, len_j = params.contactIDsForDelete.length; j < len_j; j++)
                        if (params.contactIDsForDelete[j] == ASC.CRM.ListContactView.fullContactList[i].id) {
                        isDeleted = true;
                        break;
                    }
                    if (!isDeleted)
                        newFullContactList.push(ASC.CRM.ListContactView.fullContactList[i]);

                }
                ASC.CRM.ListContactView.fullContactList = newFullContactList;

                ASC.CRM.ListContactView.Total -= params.contactIDsForDelete.length;
                jq("#totalContactsOnPage").text(ASC.CRM.ListContactView.Total);

                var selectedIDs = new Array();
                for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                    selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
                }


                for (var i = 0, len = params.contactIDsForDelete.length; i < len; i++) {
                    var $objForRemove = jq("#contactItem_" + params.contactIDsForDelete[i]);
                    if ($objForRemove.length != 0)
                        $objForRemove.remove();

                    var index = jq.inArray(params.contactIDsForDelete[i], selectedIDs);
                    if (index != -1) {
                        selectedIDs.splice(index, 1);
                        ASC.CRM.ListContactView.selectedItems.splice(index, 1);
                    }
                }
                jq("#mainSelectAll").attr("checked", false);

                _checkForLockMainActions();
                if (ASC.CRM.ListContactView.selectedItems.length != 0)
                    jq("#checkedContactsCount > span").text(jq.format(CRMJSResources.ElementsSelectedCount, ASC.CRM.ListContactView.selectedItems.length));
                else {
                    jq("#checkedContactsCount > span").text("");
                    jq("#checkedContactsCount").hide();
                }

                if (ASC.CRM.ListContactView.Total == 0
                    && (typeof (ASC.CRM.ListContactView.advansedFilter) == "undefined"
                    || ASC.CRM.ListContactView.advansedFilter.advansedFilter().length == 1)) {
                    ASC.CRM.ListContactView.noContacts = true;
                    ASC.CRM.ListContactView.noContactsForQuery = true;
                }
                else {
                    ASC.CRM.ListContactView.noContacts = false;
                    if (ASC.CRM.ListContactView.Total === 0)
                        ASC.CRM.ListContactView.noContactsForQuery = true;
                    else
                        ASC.CRM.ListContactView.noContactsForQuery = false;
                }


                if (ASC.CRM.ListContactView.noContacts) {
                    jq("#emptyContentForContactsFilter").hide();
                    jq("#mainContactList").hide();
                    jq("#contactsEmptyScreen").show();
                    jq.unblockUI();
                    return;
                }

                if (ASC.CRM.ListContactView.noContactsForQuery) {
                    jq("#companyListBox").hide();
                    jq("#mainSelectAll").attr("disabled", true);
                    jq("#mainExportCsv").next("img").hide();
                    jq("#mainExportCsv").hide();
                    jq("#contactsEmptyScreen").hide();
                    jq("#emptyContentForContactsFilter").show();
                    jq.unblockUI();
                    return;
                }

                if (jq("#companyTable tbody tr").length == 0) {
                    jq.unblockUI();
                    var startIndex = ASC.CRM.ListContactView.entryCountOnPage * (contactPageNavigator.CurrentPageNumber - 1);
                    if (startIndex >= ASC.CRM.ListContactView.Total) { startIndex -= ASC.CRM.ListContactView.entryCountOnPage; }
                    ASC.CRM.ListContactView.renderContent(startIndex);
                }
                else {
                    //contactPageNavigator.drawPageNavigator(contactPageNavigator.CurrentPageNumber, ASC.CRM.ListContactView.Total);
                    jq.unblockUI();
                }
            },

            add_tag: function(params, data) {
                jq("#addTagDialog").hide();
                if (params.isNewTag) {
                    var tag = { value: params.tagName, title: params.tagName };
                    contactTags.push(tag);
                    ASC.CRM.ListContactView.advansedFilter = ASC.CRM.ListContactView.advansedFilter.advansedFilter(
                    {
                        filters: [
                            { id: "tags", options: contactTags, enable: contactTags.length > 0 }
                        ]
                    }
                );
                }
            },

            addNewTask: function(params, task) {
                var index = 0;
                for (var i = 0; i < ASC.CRM.ListContactView.fullContactList.length; i++)
                    if (params.contactid == ASC.CRM.ListContactView.fullContactList[i].id) {
                    ASC.CRM.ListContactView.fullContactList[i].nearTask = task;
                    index = i;
                }
                jq("#contactItem_" + params.contactid).replaceWith(jq("#contactTmpl").tmpl(ASC.CRM.ListContactView.fullContactList[index]));
                ASC.CRM.Common.tooltip("#taskTitle_" + task.id, "tooltip", true);

                ASC.CRM.Common.RegisterContactInfoCard();
                PopupKeyUpActionProvider.EnableEsc = true;
                jq.unblockUI();
                taskContactSelector.SelectedContacts = new Array();
            },

            render_simple_content: function(params, contacts) {
                jq(window).trigger("getContactsFromApi", [contacts]);
                jq("#simpleContactTmpl").tmpl(contacts).prependTo("#contactTable tbody");
                ASC.CRM.Common.RegisterContactInfoCard();
                LoadingBanner.hideLoading();
            },

            removeMember: function(params, contact) {
                jq("#contactItem_" + params.contactID).remove();
                //ASC.CRM.Common.changeCountInTab("delete", "contacts");
            },

            addMember: function(params, contact) {
                jq("#simpleContactTmpl").tmpl(contact).prependTo("#contactTable tbody");
                //ASC.CRM.Common.changeCountInTab("add", "contacts");
                ASC.CRM.Common.RegisterContactInfoCard();
            },

            update_contact_rights: function(params, contacts) {
                for (var i = 0; i < contacts.length; i++) {
                    for (var j = 0; j < ASC.CRM.ListContactView.fullContactList.length; j++)
                        if (contacts[i].id == ASC.CRM.ListContactView.fullContactList[j].id) {
                        var contact_id = contacts[i].id;
                        ASC.CRM.ListContactView.fullContactList[j].isPrivate = contacts[i].isPrivate;
                        jq("#contactItem_" + contact_id).replaceWith(
                                jq("#contactTmpl").tmpl(ASC.CRM.ListContactView.fullContactList[j])
                            );

                        if (params.isBatch) {
                            jq("#check_contact_" + contact_id).attr("checked", true);
                        }
                        else {
                            ASC.CRM.ListContactView.selectedItems = new Array();
                        }

                        if (ASC.CRM.ListContactView.fullContactList[j].nearTask && ASC.CRM.ListContactView.fullContactList[j].nearTask != null)
                            ASC.CRM.Common.tooltip("#taskTitle_" + ASC.CRM.ListContactView.fullContactList[j].nearTask.id, "tooltip", false);

                        break;
                    }
                }
                ASC.CRM.Common.RegisterContactInfoCard();
                jq.unblockUI();
            }
        },

        fullContactList: new Array(),
        contactID: new Array(),
        isFirstTime: true,
        selectedItems: new Array(),
        currentUser: "",
        currentUserID: "",
        currentUserIsAdmin: false,
        entryCountOnPage: 0,
        currentPageNumber: 1,
        emailQuotas: 0,
        noContacts: false,
        noContactsForQuery: false,
        cookieKey: "",

        init: function(rowsCount, currentPageNumber, emailQuotas, userName, userID, isAdmin, cookieKey, anchor) {
            ASC.CRM.ListContactView.currentUser = userName;
            ASC.CRM.ListContactView.currentUserID = userID;
            ASC.CRM.ListContactView.currentUserIsAdmin = isAdmin;
            ASC.CRM.ListContactView.entryCountOnPage = rowsCount;
            ASC.CRM.ListContactView.currentPageNumber = currentPageNumber;
            ASC.CRM.ListContactView.emailQuotas = emailQuotas;
            ASC.CRM.ListContactView.cookieKey = cookieKey;
            LoadingBanner.displayLoading();


            var currentAnchor = location.hash;
            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#' ? currentAnchor.substring(1) : currentAnchor;

            if (currentAnchor == "" || decodeURIComponent(anchor) == currentAnchor)
                ASC.CRM.ListContactView.isFirstTime = true;
            else
                ASC.CRM.ListContactView.isFirstTime = false;

            contactPageNavigator.NavigatorParent = '#divForContactPager';
            contactPageNavigator.changePageCallback = function(page) {
                ASC.CRM.ListContactView.currentPageNumber = page;
                _setCookie(page, contactPageNavigator.EntryCountOnPage);

                var startIndex = contactPageNavigator.EntryCountOnPage * (page - 1);
                ASC.CRM.ListContactView.renderContent(startIndex);
            }

            jq("#contactActionMenu").show();
            var left = jq("#contactActionMenu .dropDownCornerRight").position().left;
            jq("#contactActionMenu").hide();
            jq.dropdownToggle({
                dropdownID: 'contactActionMenu',
                switcherSelector: '#companyTable .crm-menu',
                addTop: 4,
                addLeft: -left + 6,
                showFunction: function(switcherObj, dropdownItem) {
                    jq('#companyTable .crm-menu.active').removeClass('active');
                    if (dropdownItem.is(":hidden")) {
                        switcherObj.addClass('active');
                    }
                },
                hideFunction: function() {
                    jq('#companyTable .crm-menu.active').removeClass('active');
                }
            });

            jq(window).scroll(function() {
                ASC.CRM.Common.stickMenuToTheTop({
                    menuSelector: "#contactsHeaderMenu",
                    menuAnchorSelector: "#mainSelectAll",
                    menuSpacerSelector: "#contactsHeaderMenuSpacer",
                    userFuncInTop: function() { jq("#onTop").hide(); },
                    userFuncNotInTop: function() { jq("#onTop").show(); }
                });
            });

            if (!jq.browser.mobile) {
                ASC.CRM.FileUploader.OnAllUploadCompleteCallback_function = function() {
                    jq('#sendEmailPanel').hide();
                    jq("#sendProcessPanel").show();
                    ASC.CRM.ListContactView.changePageTitle(pageTitles.mailSend);
                    var contacts = new Array();
                    for (var i = 0, len = ASC.CRM.ListContactView.selectedItems.length; i < len; i++)
                        if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                        contacts.push(ASC.CRM.ListContactView.selectedItems[i].id);
                    }

                    var watermark = jq.format("<div style='color:#787878;font-size:12px;margin-top:10px'>{0}</div>",
                                                jq.format(CRMJSResources.TeamlabWatermark,
                                                jq.format("<a style='color:#787878;font-size:12px;' href='http://www.teamlab.com'>{0}</a>", "Teamlab.com"))
                    );

                    var subj = jq("#sendEmailPanel #tbxEmailSubject").val().trim();
                    if (subj == "") subj = CRMJSResources.NoSubject;

                    var storeInHistory = jq("#storeInHistory").is(":checked");

                    AjaxPro.ListContactView.SendEmail(ASC.CRM.FileUploader.fileIDs,
                    contacts,
                    subj,
                    ASC.CRM.ListContactView._fckEditor.GetHTML() + watermark,
                    storeInHistory, function(res) {
                        if (res.error != null) {
                            alert(res.error.Message);
                            return;
                        }
                        ASC.CRM.ListContactView.checkSendStatus(true);
                    });
                };
            }
        },

        isEnterKeyPressed: function(event) {
            //Enter key was pressed
            return event.keyCode == 13;
        },
        isEmailValid: function(email) {
            var reg = new RegExp("^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(?:\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(?:aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$", 'i');
            if (email == "" || !reg.test(email)) return false;
            return true;
        },

        getFilterSettings: function() {
            var settings =
            {
                sortBy: "displayName",
                sortOrder: "ascending",
                tags: []
            };

            if (ASC.CRM.ListContactView.advansedFilter.advansedFilter == null) return settings;

            var param = ASC.CRM.ListContactView.advansedFilter.advansedFilter();

            jq(param).each(function(i, item) {
                switch (item.id) {
                    case "sorter":
                        settings.sortBy = item.params.id;
                        settings.sortOrder = item.params.dsc == true ? "descending" : "ascending";
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

        changeFilter: function() {
            if (ASC.CRM.ListContactView.isFirstTime == true) {
                ASC.CRM.ListContactView.isFirstTime = false;

                if (typeof (contactsForFirstRequest) != "undefined")
                    contactsForFirstRequest = jq.parseJSON(jQuery.base64.decode(contactsForFirstRequest));
                else
                    contactsForFirstRequest = [];
                var startIndex = ASC.CRM.ListContactView.entryCountOnPage * (ASC.CRM.ListContactView.currentPageNumber - 1);

                ASC.CRM.ListContactView.CallbackMethods.get_contacts_by_filter(
                {
                    __startIndex: startIndex,
                    __nextIndex: contactsForFirstRequest.nextIndex,
                    __total: contactsForFirstRequest.total
                },
                Teamlab.create('crm-contacts', null, contactsForFirstRequest.response));
                return;
            }
            _setCookie(0, contactPageNavigator.EntryCountOnPage);

            ASC.CRM.ListContactView.deselectAll();

            ASC.CRM.ListContactView.renderContent(0);
        },

        renderContent: function(startIndex) {
            ASC.CRM.ListContactView.fullContactList = new Array();

            LoadingBanner.displayLoading();
            jq("#mainSelectAll").attr("checked", false);

            ASC.CRM.ListContactView.getContacts(startIndex);
        },

        addRecordsToContent: function() {
            if (!ASC.CRM.ListContactView.showMore) return false;

            jq("#showMoreContactsButtons .crm-showMoreLink").hide();
            jq("#showMoreContactsButtons .crm-loadingLink").show();

            var startIndex = jq("#companyTable tbody tr").length;

            ASC.CRM.ListContactView.getContacts(startIndex);
        },

        getContacts: function(startIndex) {
            //var isFirst = typeof startIndex == "undefined";
            var filter = ASC.CRM.ListContactView.getFilterSettings();

            if (typeof startIndex == 'undefined')
                filter.StartIndex = 0;
            else
                filter.StartIndex = startIndex;

            filter.Count = ASC.CRM.ListContactView.entryCountOnPage;

            EventTracker.Track('crm_search_contacts_by_filter');

            Teamlab.getCrmContacts({}, { filter: filter, success: ASC.CRM.ListContactView.CallbackMethods.get_contacts_by_filter });
        },

        getNearestTasks: function(currentContacts) {
            var params = { contacts: currentContacts };

            var fullContactListIDs = new Array();
            for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                fullContactListIDs.push(ASC.CRM.ListContactView.fullContactList[i].id);
            }

            var data = { contactid: fullContactListIDs };

            Teamlab.getCrmContactTasks(params, data, { success: ASC.CRM.ListContactView.CallbackMethods.get_nearest_tasks });
        },

        changeCountOfRows: function(newValue) {
            if (isNaN(newValue)) return;
            var newCountOfRows = newValue * 1;
            ASC.CRM.ListContactView.entryCountOnPage = newCountOfRows;
            contactPageNavigator.EntryCountOnPage = newCountOfRows;

            _setCookie(1, newCountOfRows);

            ASC.CRM.ListContactView.renderContent(0);
        },

        selectAll: function(obj) {
            if (jq(obj).is(":checked")) {
                ASC.CRM.ListContactView.selectedItems = new Array();
                for (var i = 0, len = ASC.CRM.ListContactView.fullContactList.length; i < len; i++)
                    ASC.CRM.ListContactView.selectedItems.push({
                        id: ASC.CRM.ListContactView.fullContactList[i].id,
                        isCompany: ASC.CRM.ListContactView.fullContactList[i].isCompany,
                        primaryEmail: ASC.CRM.ListContactView.fullContactList[i].primaryEmail,
                        displayName: ASC.CRM.ListContactView.fullContactList[i].displayName
                    });
                jq("#companyTable input:checkbox").attr("checked", true);
                jq("#checkedContactsCount > span").text(jq.format(CRMJSResources.ElementsSelectedCount, ASC.CRM.ListContactView.selectedItems.length));
                jq("#checkedContactsCount").show();
                jq("#companyTable tr").addClass("selectedRow");
                _checkForLockMainActions();
            }
            else {
                ASC.CRM.ListContactView.deselectAll();
            }
        },

        selectItem: function(obj) {
            var id = parseInt(jq(obj).attr("id").split("_")[2]);

            var selectedContact = null;
            for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                if (id == ASC.CRM.ListContactView.fullContactList[i].id)
                    selectedContact = {
                        id: ASC.CRM.ListContactView.fullContactList[i].id,
                        isCompany: ASC.CRM.ListContactView.fullContactList[i].isCompany,
                        primaryEmail: ASC.CRM.ListContactView.fullContactList[i].primaryEmail,
                        displayName: ASC.CRM.ListContactView.fullContactList[i].displayName
                    }
            }


            var selectedIDs = new Array();
            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
            }

            var index = jq.inArray(id, selectedIDs);

            if (jq(obj).is(":checked")) {
                jq("#contactItem_" + id).addClass("selectedRow");
                if (index == -1)
                    ASC.CRM.ListContactView.selectedItems.push(selectedContact);
            }
            else {
                jq("#mainSelectAll").attr("checked", false);
                jq("#contactItem_" + id).removeClass("selectedRow");
                if (index != -1) {
                    ASC.CRM.ListContactView.selectedItems.splice(index, 1);
                }
            }

            _checkForLockMainActions();

            if (ASC.CRM.ListContactView.selectedItems.length != 0) {
                jq("#checkedContactsCount > span").text(jq.format(CRMJSResources.ElementsSelectedCount, ASC.CRM.ListContactView.selectedItems.length));
                jq("#checkedContactsCount").show();
            }
            else {
                jq("#checkedContactsCount > span").text("");
                jq("#checkedContactsCount").hide();
            }
        },

        deselectAll: function() {
            ASC.CRM.ListContactView.selectedItems = new Array();
            jq("#companyTable input:checkbox").attr("checked", false);
            jq("#mainSelectAll").attr("checked", false);
            jq("#companyTable tr.selectedRow").removeClass("selectedRow");
            jq("#checkedContactsCount > span").text("");
            jq("#checkedContactsCount").hide();
            _lockMainActions();
        },

        showDeletePanel: function() {
            var showCompaniesPanel = false;
            var showPersonsPanel = false;
            jq("#deleteList dd.confirmRemoveCompanies, dd.confirmRemovePersons").html("");
            for (var i = 0, len = ASC.CRM.ListContactView.selectedItems.length; i < len; i++)
                if (ASC.CRM.ListContactView.selectedItems[i].isCompany) {
                showCompaniesPanel = true;
                var label = jq("<label></label>").attr("title", ASC.CRM.ListContactView.selectedItems[i].displayName).text(ASC.CRM.ListContactView.selectedItems[i].displayName);
                jq("#deleteList dd.confirmRemoveCompanies").append(
                            label.prepend(jq("<input>").attr("type", "checkbox").attr("checked", true).attr("id", "company_" + ASC.CRM.ListContactView.selectedItems[i].id))
                        );
            }
            else {
                showPersonsPanel = true;
                var label = jq("<label></label>").attr("title", ASC.CRM.ListContactView.selectedItems[i].displayName).text(ASC.CRM.ListContactView.selectedItems[i].displayName);
                jq("#deleteList dd.confirmRemovePersons").append(
                            label.prepend(jq("<input>").attr("type", "checkbox").attr("checked", true).attr("id", "person_" + ASC.CRM.ListContactView.selectedItems[i].id))
                        );
            }

            if (showCompaniesPanel)
                jq("#deleteList dt.confirmRemoveCompanies, dd.confirmRemoveCompanies").show();
            else
                jq("#deleteList dt.confirmRemoveCompanies, dd.confirmRemoveCompanies").hide();
            if (showPersonsPanel)
                jq("#deleteList dt.confirmRemovePersons, dd.confirmRemovePersons").show();
            else
                jq("#deleteList dt.confirmRemovePersons, dd.confirmRemovePersons").hide();
            jq("#deletePanel div.action-block").show();
            jq("#deletePanel div.info-block").hide();
            ASC.CRM.Common.blockUI("#deletePanel", 500, 500, 0);
        },

        deleteBatchContacts: function() {
            var ids = new Array();
            jq("#deletePanel input:checked").each(function() {
                ids.push(parseInt(jq(this).attr("id").split("_")[1]));
            });
            var params = { contactIDsForDelete: ids };

            Teamlab.removeCrmContact(params, ids,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.delete_batch_contacts,
                before: function(params) { jq("#deletePanel div.action-block").hide(); jq("#deletePanel div.info-block").show(); },
                after: function(params) { jq("#deletePanel div.action-block").show(); jq("#deletePanel div.info-block").hide(); }
            });
        },

        showSetPermissionsPanel: function(params) {
            if (jq("#setPermissionsPanel div.tintMedium").length > 0) {
                jq("#setPermissionsPanel div.tintMedium span.headerBase").remove();
                jq("#setPermissionsPanel div.tintMedium").removeClass("tintMedium").css("padding", "0px");
            }
            jq("#isPrivate").attr("checked", false);
            changeIsPrivateCheckBox();
            jq("#selectedUsers div.selectedUser[id^=selectedUser_]").remove();
            SelectedUsers.IDs = new Array();
            jq("#setPermissionsPanel div.action-block").show();
            jq("#setPermissionsPanel div.info-block").hide();
            jq("#setPermissionsPanel .setPermissionsLink").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.setPermissions(params);
            });

            ASC.CRM.Common.blockUI("#setPermissionsPanel", 600, 500, 0);
        },

        setPermissions: function(params) {
            var selectedUsers = SelectedUsers.IDs;
            selectedUsers.push(SelectedUsers.CurrentUserID);

            var selectedIDs = new Array();
            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
            }

            var data = {
                contactid: selectedIDs,
                isPrivate: jq("#isPrivate").is(":checked"),
                accessList: selectedUsers
            };

            Teamlab.updateCrmContactRights(params, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.update_contact_rights,
                before: function() { jq("#setPermissionsPanel div.action-block").hide(); jq("#setPermissionsPanel div.info-block").show(); },
                after: function() { jq("#setPermissionsPanel div.action-block").show(); jq("#setPermissionsPanel div.info-block").hide(); }
            });
        },

        showActionMenu: function(contactID, isCompany, displayName, primaryEmail, createBy) {
            //jq("active")
            var addPhoneLinkText = CRMJSResources.AddNewPhone;
            var addEmailLinkText = CRMJSResources.AddNewEmail;

            var primaryPhone = null;
            var primaryEmail = null;

            for (var i = 0, len_i = ASC.CRM.ListContactView.fullContactList.length; i < len_i; i++)
                if (contactID == ASC.CRM.ListContactView.fullContactList[i].id) {
                if (ASC.CRM.ListContactView.fullContactList[i].primaryPhone != null) {
                    addPhoneLinkText = CRMJSResources.EditPhone;
                    primaryPhone = ASC.CRM.ListContactView.fullContactList[i].primaryPhone;
                }

                if (ASC.CRM.ListContactView.fullContactList[i].primaryEmail != null) {
                    addEmailLinkText = CRMJSResources.EditEmail;
                    primaryEmail = ASC.CRM.ListContactView.fullContactList[i].primaryEmail;
                }
                break;
            }

            jq("#contactActionMenu .addTaskLink").unbind("click").bind("click", function() {
                jq("#contactActionMenu").hide();
                jq("#taskActionMenu").hide();
                jq('#companyTable .crm-menu.active').removeClass('active');
                ASC.CRM.ListContactView.showTaskPanel(contactID);
            });

            jq("#contactActionMenu .addDealLink").attr("href",
                            jq.format("deals.aspx?action=manage&contactID={0}", contactID));

            jq("#contactActionMenu .addCaseLink").attr("href",
                            jq.format("cases.aspx?action=manage&contactID={0}", contactID));

            jq("#contactActionMenu .addPhoneLink").text(addPhoneLinkText);
            jq("#contactActionMenu .addPhoneLink").unbind("click").bind("click", function() {
                jq("#contactActionMenu").hide();
                jq('#companyTable .crm-menu.active').removeClass('active');
                _showAddPrimaryPhoneInput(contactID, primaryPhone);
            });

            jq("#contactActionMenu .addEmailLink").text(addEmailLinkText);
            jq("#contactActionMenu .addEmailLink").unbind("click").bind("click", function() {
                jq("#contactActionMenu").hide();
                jq('#companyTable .crm-menu.active').removeClass('active');
                _showAddPrimaryEmailInput(contactID, primaryEmail);
            });

            jq("#contactActionMenu .editContactLink").attr("href",
                    jq.format("default.aspx?id={0}&action=manage{1}", contactID, !isCompany ? "&type=people" : ""));

            jq("#contactActionMenu .deleteContactLink").unbind("click").bind("click", function() {
                jq("#contactActionMenu").hide();
                jq('#companyTable .crm-menu.active').removeClass('active');
                if (ASC.CRM.ContactActionView.confirmForDelete(isCompany, decodeURI(displayName))) {
                    var ids = new Array();
                    ids.push(contactID);
                    var params = { contactIDsForDelete: ids };

                    Teamlab.removeCrmContact(params, ids,
                    {
                        success: ASC.CRM.ListContactView.CallbackMethods.delete_batch_contacts,
                        before: function(params) { jq("#contactActionMenu").hide(); },
                        after: function(params) { }
                    });
                }
            });

            jq("#contactActionMenu .showProfileLink").attr("href",
                    jq.format("default.aspx?id={0}{1}", contactID, !isCompany ? "&type=people" : ""));

            if(ASC.CRM.ListContactView.currentUserIsAdmin || ASC.CRM.ListContactView.currentUserID == createBy)
            {
                jq("#contactActionMenu .setPermissionsLink").show();
                jq("#contactActionMenu .setPermissionsLink").unbind("click").bind("click", function() {
                    jq("#contactActionMenu").hide();
                    jq('#companyTable .crm-menu.active').removeClass('active');

                    ASC.CRM.ListContactView.deselectAll();

                    ASC.CRM.ListContactView.selectedItems.push({
                        id: contactID,
                        isCompany: isCompany,
                        primaryEmail: primaryEmail,
                        displayName: displayName
                    });
                    ASC.CRM.ListContactView.showSetPermissionsPanel({ isBatch: false });
                });
            } else {
                jq("#contactActionMenu .setPermissionsLink").hide();
            }
        },

        initTaskPanel: function(contactID) {
            jq("#tbxTitle").val("");
            jq("#tbxDescribe").val("");

            taskResponsibleSelector.ClearFilter();
            taskResponsibleSelector.ChangeDepartment(taskResponsibleSelector.Groups[0].ID);

            var obj;
            if (!jq.browser.mobile) {
                obj = document.getElementById("User_" + ASC.CRM.ListContactView.currentUserID);
                if (obj != null)
                    taskResponsibleSelector.SelectUser(obj);
            } else {
                obj = jq("#taskResponsibleSelector select option[value=" + ASC.CRM.ListContactView.currentUserID + "]");
                if (obj.length > 0) {
                    taskResponsibleSelector.SelectUser(obj);
                    jq(obj).attr("selected", true);
                }
            }

            jq("#taskDeadline").datepicker('setDate', new Date());

            jq("#optDeadlineHours_-1").attr('selected', true);
            jq("#optDeadlineMinutes_-1").attr('selected', true);
            obj = taskCategorySelector.getRowByContactID(0);
            taskCategorySelector.changeContact(obj);

            var index = 0;
            for (var i = 0; i < ASC.CRM.ListContactView.fullContactList.length; i++)
                if (ASC.CRM.ListContactView.fullContactList[i].id == contactID)
                index = i;
            taskContactSelector.SelectedContacts = new Array();
            taskContactSelector.setContact(jq("#contactTitle_taskContactSelector_0"),
             ASC.CRM.ListContactView.fullContactList[index].id,
             Encoder.htmlEncode(ASC.CRM.ListContactView.fullContactList[index].displayName),
             ASC.CRM.ListContactView.fullContactList[index].smallFotoUrl);
            taskContactSelector.showInfoContent(jq("#contactTitle_taskContactSelector_0"));

            if (ASC.CRM.ListContactView.fullContactList[index].isPrivate) {
                var allUsers = new Array();
                for (var i = 0; i < taskResponsibleSelector.Groups.length; i++)
                    for (var j = 0; j < taskResponsibleSelector.Groups[i].Users.length; j++)
                    allUsers.push(taskResponsibleSelector.Groups[i].Users[j].ID)

                var accessList = new Array();
                for (var i = 0; i < ASC.CRM.ListContactView.fullContactList[index].accessList.length; i++)
                    accessList.push(ASC.CRM.ListContactView.fullContactList[index].accessList[i].id);

                for (var i = 0; i < adminList.length; i++)
                    accessList.push(adminList[i].id);

                for (var i = 0; i < allUsers.length; i++) {
                    var hasAccess = false;
                    for (var j = 0; j < accessList.length; j++)
                        if (allUsers[i] == accessList[j])
                        hasAccess = true;
                    if (!hasAccess)
                        taskResponsibleSelector.HideUser(allUsers[i]);
                }
            }
            else {
                taskResponsibleSelector.DisplayAll();
            }

            PopupKeyUpActionProvider.EnableEsc = false;
            HideRequiredError();
        },

        showTaskPanel: function(contactID) {
            ASC.CRM.ListContactView.initTaskPanel(contactID);
            ASC.CRM.Common.blockUI("#addTaskPanel", 650, 600, 0);
            jq("#addTaskPanel input[id$=tbxTitle]").focus();
            jq("#addTaskPanel a.baseLinkButton:first").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.addNewTask(contactID);
            });
        },

        addNewTask: function(contactID) {
            var deadLine = null;
            if (jq.trim(jq("#addTaskPanel input[id$=taskDeadline]").val()) != "") {
                deadLine = jq("#taskDeadline").datepicker('getDate');
                if (parseInt(jq("#taskDeadlineHours option:selected").val()) != -1)
                    deadLine.setHours(parseInt(jq("#taskDeadlineHours option:selected").val()));
                else
                    deadLine.setHours(0);

                if (parseInt(jq("#taskDeadlineMinutes option:selected").val()) != -1)
                    deadLine.setMinutes(parseInt(jq("#taskDeadlineMinutes option:selected").val()));
                else
                    deadLine.setMinutes(0);

                deadLine = Teamlab.serializeTimestamp(deadLine);
            }

            var dataTask = {
                id: 0,
                title: jq("#tbxTitle").val(),
                description: jq("#tbxDescribe").val(),
                deadline: deadLine,
                responsibleid: taskResponsibleSelector.SelectedUserId,
                categoryid: taskCategorySelector.CategoryID,
                isnotify: jq("#notifyResponsible").is(":checked"),
                contactid: contactID
            };

            var isValid = true;
            var invalidTaskTime = (parseInt(jq("#taskDeadlineHours option:selected").val()) == -1 && parseInt(jq("#taskDeadlineMinutes option:selected").val()) != -1) || (parseInt(jq("#taskDeadlineHours option:selected").val()) != -1 && parseInt(jq("#taskDeadlineMinutes option:selected").val()) == -1);

            if (jq.trim(jq("#tbxTitle").val()) == "") {
                AddRequiredErrorText(jq("#tbxTitle"), CRMJSResources.EmptyTaskTitle);
                ShowRequiredError(jq("#tbxTitle"), true);
                isValid = false;
            }
            else RemoveRequiredErrorClass(jq("#tbxTitle"));

            if (dataTask.responsibleid == null) {
                AddRequiredErrorText(jq("#addTaskPanel #inputUserName"), CRMJSResources.EmptyTaskResponsible);
                ShowRequiredError(jq("#addTaskPanel #inputUserName"), true);
                isValid = false;
            }
            else RemoveRequiredErrorClass(jq("#addTaskPanel #inputUserName"));

            if (dataTask.deadline == null || invalidTaskTime) {
                AddRequiredErrorText(jq("#taskDeadline"), CRMJSResources.EmptyTaskDeadline);
                ShowRequiredError(jq("#taskDeadline"), true);

                if (invalidTaskTime) {
                    jq("#taskDeadlineHours").addClass("requiredInputError");
                    jq("#taskDeadlineMinutes").addClass("requiredInputError");
                }
                else {
                    jq("#taskDeadlineHours").removeClass("requiredInputError");
                    jq("#taskDeadlineMinutes").removeClass("requiredInputError");
                }

                isValid = false;
            }
            else {
                RemoveRequiredErrorClass(jq("#taskDeadline"));
                jq("#taskDeadlineHours").removeClass("requiredInputError");
                jq("#taskDeadlineMinutes").removeClass("requiredInputError");
            }

            if (!isValid)
                return false;

            Teamlab.addCrmTask({ contactid: contactID }, dataTask,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.addNewTask,
                before: function(params) { jq("#addTaskPanel .action_block").hide(); jq("#addTaskPanel .ajax_info_block").show(); },
                after: function(params) { jq("#addTaskPanel .ajax_info_block").hide(); jq("#addTaskPanel .action_block").show(); }
            });

        },

        showAddTagDialog: function() {
            jq("#addTagDialog div.dropDownContent").html("");
            jq("#addTagDialog input.textEdit").val("");
            for (var i = 0; i < contactTags.length; i++) {
                var tag = jq("<a></a>").addClass("dropDownItem")
                        .text(contactTags[i].title)
                        .unbind("click").bind("click", function() {
                            ASC.CRM.ListContactView.addTag(this);
                        });
                jq("#addTagDialog div.dropDownContent").append(tag);
            }
            jq.dropdownToggle().toggle("#mainAddTag", "addTagDialog", 5, 0);
        },

        addTag: function(obj) {
            var selectedTag = jq(obj).text();

            var selectedIDs = new Array();
            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
            }
            var params = { contactIDs: selectedIDs, tagName: selectedTag, isNewTag: false };

            Teamlab.addCrmTag(params, "contact", params.contactIDs, params.tagName,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_tag,
                before: function(params) {
                    for (var i = 0; i < params.contactIDs.length; i++) {
                        jq("#check_contact_" + params.contactIDs[i]).hide();
                        jq("#loaderImg_" + params.contactIDs[i]).show();
                    }
                },
                after: function(params) {
                    for (var i = 0; i < params.contactIDs.length; i++) {
                        jq("#check_contact_" + params.contactIDs[i]).show();
                        jq("#loaderImg_" + params.contactIDs[i]).hide();
                    }
                }
            });
        },

        addNewTag: function() {
            var newTag = jq("#addTagDialog input").val().trim();
            if (newTag == "") return false;

            var selectedIDs = new Array();
            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
            }
            var params = { contactIDs: selectedIDs, tagName: newTag, isNewTag: true };

            Teamlab.addCrmTag(params, "contact", params.contactIDs, params.tagName,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_tag,
                before: function(params) {
                    for (var i = 0; i < params.contactIDs.length; i++) {
                        jq("#check_contact_" + params.contactIDs[i]).hide();
                        jq("#loaderImg_" + params.contactIDs[i]).show();
                    }
                },
                after: function(params) {
                    for (var i = 0; i < params.contactIDs.length; i++) {
                        jq("#check_contact_" + params.contactIDs[i]).show();
                        jq("#loaderImg_" + params.contactIDs[i]).hide();
                    }
                }
            });
        },

        showSendEmailDialog: function() {
            jq.dropdownToggle().toggle("#mainSendEmail", "sendEmailDialog", 5, 0);
        },

        showExportDialog: function() {
            jq.dropdownToggle().toggle("#mainExportCsv", "exportDialog", 7, -20);
        },

        showCreateLinkPanel: function() {
            var selectedEmails = new Array();
            jq("#sendEmailDialog").hide();
            jq("#createLinkPanel #linkList").html("");
            jq("#cbxBlind").attr("checked", false);
            jq("#tbxBatchSize").val("10");
            jq.forceIntegerOnly("#tbxBatchSize");
            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++)
                if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                selectedEmails.push(ASC.CRM.ListContactView.selectedItems[i].primaryEmail.data);
            }

            jq("#createLinkPanel div.action-block a:first").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.createLink(selectedEmails);
            });
            ASC.CRM.Common.blockUI("#createLinkPanel", 500, 500, 0);
        },

        createLink: function(emails) {
            jq("#createLinkPanel #linkList").html("");
            jq("#createLinkPanel div.action-block").hide();
            jq("#createLinkPanel div.info-block").show();
            var blindAttr = jq("#cbxBlind").is(":checked") ? "bcc=" : "cc=";
            var batchSize = jq("#tbxBatchSize").val().trim() == "" ? 0 : parseInt(jq("#tbxBatchSize").val().trim());
            var href = "mailto:?";
            var linkList = new Array();
            var link = href;
            var counter = 0;
            var info = "";

            if (ASC.CRM.ListContactView.selectedItems.length - emails.length > 0) {
                info = jq.format(CRMJSResources.GenerateLinkInfo,
                jq.format(CRMJSResources.RecipientsWithoutEmail, ASC.CRM.ListContactView.selectedItems.length - emails.length));
            } else {
                info = jq.format(CRMJSResources.GenerateLinkInfo, "");
            }

            jq("#createLinkPanel #linkList").append(jq("<div></div>").text(info).addClass("headerPanel-splitter"));

            if (emails.length == 1)
                linkList.push(jq.format("mailto:{0}", emails[0]));
            else if (batchSize == 1) {
                for (var i = 0; i < emails.length; i++)
                    linkList.push(jq.format("mailto:{0}", emails[i]));
            } else {
                for (var i = 0; i < emails.length; i++) {
                    link += blindAttr + emails[i] + "&";
                    counter++;
                    if (batchSize != 0 && counter == batchSize) {
                        counter = 0;
                        linkList.push(link.substring(0, link.length - 1));
                        link = href;
                    }
                    if (i == emails.length - 1 && emails.length % batchSize != 0)
                        linkList.push(link.substring(0, link.length - 1));
                }
            }

            for (var i = 0; i < linkList.length; i++) {
                counter = i + 1;
                jq("#linkList")
            .append(jq("<a></a>").text(CRMJSResources.Batch + " " + counter).attr("href", linkList[i]));
                if (i != linkList.length - 1)
                    jq("#linkList").append(jq("<span><span/>").addClass("splitter").text(","));
            }
            jq("#createLinkPanel div.action-block").show();
            jq("#createLinkPanel div.info-block").hide();
            jq("#linkList").show();
        },

        initSMTPSettingsPanel: function() {
            jq.forceIntegerOnly("#tbxPort");
            jq("#smtpSettingsContent div.errorBox").remove();
            jq("#smtpSettingsContent div.okBox").remove();
            if (smtpSettings != null) {
                jq("#tbxHost").val(smtpSettings.Host);
                jq("#tbxPort").val(smtpSettings.Port);
                jq("#tbxHostLogin").val(smtpSettings.HostLogin);
                jq("#tbxHostPassword").val(smtpSettings.HostPassword);
                jq("#tbxSenderDisplayName").val(smtpSettings.SenderDisplayName);
                jq("#tbxSenderEmailAddress").val(smtpSettings.SenderEmailAddress);
                jq("#cbxEnableSSL").attr("checked", smtpSettings.EnableSSL);
                if (smtpSettings.RequiredHostAuthentication) {
                    jq("#cbxAuthentication").attr("checked", true);
                    jq("#tbxHostLogin").removeAttr("disabled");
                    jq("#tbxHostPassword").removeAttr("disabled");
                }
                else {
                    jq("#cbxAuthentication").attr("checked", false);
                    jq("#tbxHostLogin").attr("disabled", true);
                    jq("#tbxHostPassword").attr("disabled", true);
                }
            }
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

        checkSMTPSettings: function() {

            if (smtpSettings == null)
                return false;
            if (smtpSettings.RequiredHostAuthentication)
                if (smtpSettings.Host == "" ||
                smtpSettings.Port == "" ||
                    smtpSettings.HostLogin == "" ||
                        smtpSettings.HostPassword == "" ||
                            smtpSettings.SenderDisplayName == "" ||
                                smtpSettings.SenderEmailAddress == "")
                return false;

            if (!smtpSettings.RequiredHostAuthentication)
                if (smtpSettings.Host == "" ||
                smtpSettings.Port == "" ||
                    smtpSettings.SenderDisplayName == "" ||
                        smtpSettings.SenderEmailAddress == "")
                return false;

            return true;
        },

        saveSMTPSettings: function(obj) {
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
                smtpSettings = {};
                smtpSettings.Host = host;
                smtpSettings.Port = port;
                smtpSettings.RequiredHostAuthentication = authentication;
                smtpSettings.HostLogin = hostLogin;
                smtpSettings.HostPassword = hostPassword;
                smtpSettings.SenderDisplayName = senderDisplayName;
                smtpSettings.SenderEmailAddress = senderEmailAddress;
                smtpSettings.EnableSSL = enableSSL;

                jq("#smtpSettingsContent div.errorBox").remove();
                jq("#smtpSettingsContent").prepend(
                jq("<div></div>").addClass("okBox").text(CRMJSResources.SettingsUpdated)
            );
                setTimeout(function() {
                    jq("#smtpSettingsContent div.okBox").remove();
                    if (!jq.browser.mobile) {
                        ASC.CRM.ListContactView.showSendEmailPanel(obj);
                    } else {
                        ASC.CRM.ListContactView.showSendEmailPanel();
                    }
                    jq.unblockUI();
                }, 1000);
            });
        },

        showSendEmailPanel: function(obj) {
            var selectedTargets = new Array();

            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++)
                if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                var target = {};
                target.primaryEmail = ASC.CRM.ListContactView.selectedItems[i].primaryEmail.data;
                target.title = ASC.CRM.ListContactView.selectedItems[i].displayName;
                target.id = ASC.CRM.ListContactView.selectedItems[i].id;
                selectedTargets.push(target);
            }

            if (selectedTargets.length > ASC.CRM.ListContactView.emailQuotas) {
                alert(jq.format(CRMJSResources.ErrorEmailRecipientsCount, ASC.CRM.ListContactView.emailQuotas));
                return false;
            }

            AjaxPro.onLoading = function(b) { };
            jq("#sendEmailDialog").hide();

            if (!ASC.CRM.ListContactView.checkSMTPSettings()) {
                ASC.CRM.ListContactView.initSMTPSettingsPanel();
                ASC.CRM.Common.blockUI("#smtpSettingsPanel", 500, 500, 0);
                return false;
            }

            AjaxPro.ListContactView.GetStatus(function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }
                if (res.value == null || res.value.IsCompleted) {
                    jq("#sendEmailPanel #emailFromLabel").text(jq.format("{0} ({1})", smtpSettings.SenderDisplayName, smtpSettings.SenderEmailAddress));
                    jq("#sendEmailPanel #previewEmailFromLabel").text(jq.format("{0} ({1})", smtpSettings.SenderDisplayName, smtpSettings.SenderEmailAddress));

                    if (!jq.browser.mobile) {
                        if (typeof ASC.CRM.ListContactView._fckEditor == "undefined")
                            ASC.CRM.ListContactView._fckEditor = obj;

                        ASC.CRM.ListContactView._fckEditor.Config.ToolbarSets.Email = [
                        ["Source"],
                        ["Undo", "Redo"],
                        ["Bold", "Italic", "Underline", "StrikeThrough"],
                        ["Link", "Unlink"],
                        ["TextColor", "BGColor"],
                        ["FontSize", "FontName"],
                        ["Smiley"],
                        ["UnorderedList", "OrderedList"],
                        ["JustifyLeft", "JustifyCenter", "JustifyRight"]
                    ];
                        ASC.CRM.ListContactView._fckEditor.ToolbarSet.Load("Email");


                        ASC.CRM.FileUploader.activateUploader();
                        ASC.CRM.FileUploader.fileIDs.clear();
                    }

                    jq("#tbxEmailSubject").val("");

                    if (!jq.browser.mobile)
                        ASC.CRM.ListContactView._fckEditor.SetHTML("<br />");
                    else
                        jq("#mobileMessageBody").val("");

                    jq("#storeInHistory").attr("checked", false);

                    jq("#sendEmailPanel div.action-block #sendButton").text(CRMJSResources.NextPreview).unbind("click").bind("click", function() {
                        ASC.CRM.ListContactView.showSendEmailPanelPreview(selectedTargets);
                    });

                    jq("#sendEmailPanel div.action-block #backButton a.baseLinkButton").unbind("click").bind("click", function() {
                        ASC.CRM.ListContactView.showSendEmailPanelCreate(selectedTargets);
                    });

                    AjaxPro.AjaxProHelper.ChooseNumeralCase(selectedTargets.length,
                    CRMJSResources.AddressNominative,
                    CRMJSResources.AddressGenitiveSingular,
                    CRMJSResources.AddressGenitivePlural, function(result) {
                        if (result.error != null) {
                            alert(result.error.Message);
                            return;
                        }
                        jq("#emailAddresses").html(jq.format("{0} {1}", selectedTargets.length, result.value));
                        jq("#previewEmailAddresses").html(jq.format("{0} {1}", selectedTargets.length, result.value));
                        jq("#mainContactList").hide();
                        jq("#sendEmailPanel").show();
                        jq("#sendEmailPanel #createContent").show();
                        ASC.CRM.ListContactView.changePageTitle(pageTitles.composeMail);
                        jq("#sendEmailPanel #previewContent").hide();
                        jq("#sendProcessPanel").hide();
                    });
                } else {
                    ASC.CRM.ListContactView.checkSendStatus(true);
                }
            });
        },

        closeSendEmailPanel: function() {
            jq('#mainContactList').show();
            ASC.CRM.ListContactView.changePageTitle(pageTitles.contacts);
            jq('#sendEmailPanel').hide();
            jq("#sendProcessPanel").hide();
            jq("#sendEmailPanel #tbxEmailSubject").val("");

            if (!jq.browser.mobile)
                ASC.CRM.ListContactView._fckEditor.SetHTML("<br />");
            else
                jq("#mobileMessageBody").val("");

            jq('#sendEmailPanel div.action-block #backButton').hide();
        },

        showSendEmailPanelCreate: function(selectedTargets) {
            jq('#sendEmailPanel #createContent').show();
            ASC.CRM.ListContactView.changePageTitle(pageTitles.composeMail);
            jq('#sendEmailPanel #previewContent').hide();
            jq("#sendProcessPanel").hide();
            jq('#sendEmailPanel div.action-block #backButton').hide();
            jq("#sendEmailPanel div.action-block #sendButton").text(CRMJSResources.NextPreview).unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.showSendEmailPanelPreview(selectedTargets);
            });
        },

        showSendEmailPanelPreview: function(selectedTargets) {
            AjaxPro.onLoading = function(b) {
                if (b) {
                    jq("#sendEmailPanel div.action-block").hide();
                    jq("#sendEmailPanel div.info-block").show();
                } else {
                    jq("#sendEmailPanel div.action-block").show();
                    jq("#sendEmailPanel div.info-block").hide();
                }
            };

            var mess = "";

            if (!jq.browser.mobile) {
                mess = ASC.CRM.ListContactView._fckEditor.GetHTML();
                if (mess == "<br />") {
                    AddRequiredErrorText(jq("#requiredMessageBody"), CRMJSResources.EmptyLetterBodyContent);
                    ShowRequiredError(jq("#requiredMessageBody"), true);
                    return false;
                } else RemoveRequiredErrorClass(jq("#requiredMessageBody"));
            } else {
                mess = jq("#mobileMessageBody").val().trim();
                if (mess == "") {
                    AddRequiredErrorText(jq("#requiredMessageBody"), CRMJSResources.EmptyLetterBodyContent);
                    ShowRequiredError(jq("#requiredMessageBody"), true);
                    return false;
                } else RemoveRequiredErrorClass(jq("#requiredMessageBody"));
            }

            ASC.CRM.ListContactView.changePageTitle(pageTitles.previewMail);

            var subj = jq("#sendEmailPanel #tbxEmailSubject").val().trim();
            if (subj == "") subj = CRMJSResources.NoSubject;

            AjaxPro.ListContactView.GetMessagePreview(mess, selectedTargets[0].id, function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    jq("#sendEmailPanel div.action-block").show();
                    jq("#sendEmailPanel div.info-block").hide();
                    return false;
                }

                jq("#previewSubject").text(subj);

                var watermark = jq.format("<div style='color:#787878;font-size:12px;margin-top:10px'>{0}</div>",
                jq.format(CRMJSResources.TeamlabWatermark,
                    jq.format("<a style='color:#787878;font-size:12px;' href='http://www.teamlab.com'>{0}</a>", "Teamlab.com")
                )
            );

                jq("#previewMessage").html(res.value + watermark);

                if (!jq.browser.mobile) {
                    var attachments = jq("#history_uploadContainer div.studioFileUploaderFileName");
                    jq("#previewAttachments span").html("");

                    if (attachments.length > 0) {
                        attachments.each(function(index) {
                            jq("#previewAttachments span").append(jq(this).text());
                            if (index != attachments.length - 1)
                                jq("#previewAttachments span").append(", ");
                        });
                        jq("#previewAttachments").show();
                    } else {
                        jq("#previewAttachments").hide();
                    }
                }

                jq("#sendProcessPanel").hide();
                jq("#sendEmailPanel #createContent").hide();
                jq("#sendEmailPanel #previewContent").show();
                jq('#sendEmailPanel div.action-block #backButton').show();
                jq("#sendEmailPanel div.action-block #sendButton").text(CRMJSResources.Send).unbind("click").bind("click", function() {
                    ASC.CRM.ListContactView.sendEmail(selectedTargets);
                    EventTracker.Track('crm_send_mass_email');
                });

            });

        },

        sendEmail: function(selectedTargets) {
            AjaxPro.onLoading = function(b) { };

            if (!jq.browser.mobile && fileUploader.GetUploadFileCount() > 0) {
                jq("#" + fileUploader.ButtonID).css("visibility", "hidden");
                jq("#pm_upload_btn_html5").hide();
                fileUploader.Submit();
            }
            else {
                var contacts = new Array();
                for (var i = 0; i < selectedTargets.length; i++)
                    contacts.push(selectedTargets[i].id);

                var subj = jq("#sendEmailPanel #tbxEmailSubject").val().trim();
                if (subj == "") subj = CRMJSResources.NoSubject;

                var watermark = jq.format("<div style='color:#787878;font-size:12px;margin-top:10px'>{0}</div>",
                jq.format(CRMJSResources.TeamlabWatermark,
                    jq.format("<a style='color:#787878;font-size:12px;' href='http://www.teamlab.com'>{0}</a>", "Teamlab.com")
                )
            );

                var storeInHistory = jq("#storeInHistory").is(":checked");

                var mess = "";

                if (!jq.browser.mobile)
                    mess = ASC.CRM.ListContactView._fckEditor.GetHTML();
                else
                    mess = jq("#mobileMessageBody").val().trim();

                AjaxPro.ListContactView.SendEmail(new Array(),
                 contacts,
                 subj,
                 mess + watermark,
                 storeInHistory, function(res) {
                     if (res.error != null) { alert(res.error.Message); return; }
                     ASC.CRM.ListContactView.checkSendStatus(true);
                 });
            }
        },

        checkSendStatus: function(isFirstVisit) {
            AjaxPro.onLoading = function(b) {
            };

            jq("#mainContactList").hide();
            jq("#sendEmailPanel").hide();
            jq("#sendProcessPanel").show();
            jq("#sendProcessPanel #abortButton").show();
            jq("#sendProcessPanel #okButton").hide();

            if (isFirstVisit) {
                jq("#sendProcessProgress div").css("width", "0%");
                jq("#sendProcessProgress span").text("0%");
                jq("#emailsTotalCount").html("");
                jq("#emailsAlreadySentCount").html("");
                jq("#emailsEstimatedTime").html("");
                jq("#emailsErrorsCount").html("");
            }

            ASC.CRM.ListContactView.changePageTitle(pageTitles.mailSend);

            AjaxPro.ListContactView.GetStatus(function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }

                if (res.value == null) {
                    jq("#sendProcessProgress div").css("width", "100%");
                    jq("#sendProcessProgress span").text("100%");
                    jq("#abortButton").hide();
                    jq("#okButton").show();
                    return;
                } else {
                    jq("#sendProcessProgress div").css("width", res.value.Percentage + "%");
                    jq("#sendProcessProgress span").text(res.value.Percentage + "%");
                    jq("#emailsAlreadySentCount").html(res.value.Status.DeliveryCount);
                    jq("#emailsEstimatedTime").html(res.value.Status.EstimatedTime);
                    jq("#emailsTotalCount").html(res.value.Status.RecipientCount);
                    jq("#emailsErrorsCount").html("0");
                }

                if (res.value.Error != null && res.value.Error != "") {
                    ASC.CRM.ListContactView.buildErrorList(res);
                    jq("#abortButton").hide();
                    jq("#okButton").show();
                } else {
                    if (res.value.IsCompleted) {
                        jq("#abortButton").hide();
                        jq("#okButton").show();
                    } else {
                        setTimeout("ASC.CRM.ListContactView.checkSendStatus(false)", 3000);
                    }
                }
            });
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

            jq("#emailsErrorsCount")
            .html(jq("<div></div>").addClass("redText").html(mess))
            .append(jq("<a></a>").attr("href", "settings.aspx?type=common").text(CRMJSResources.GoToSettings));
        },

        abortMassSend: function() {
            AjaxPro.onLoading = function(b) { };
            AjaxPro.ListContactView.Cancel(function(res) {
                if (res.error != null) { alert(res.error.Message); return; }
                jq('#mainContactList').show();
                jq('#sendEmailPanel').hide();
                jq("#sendProcessPanel").hide();
                jq("#sendEmailPanel #tbxEmailSubject").val("");

                if (!jq.browser.mobile)
                    ASC.CRM.ListContactView._fckEditor.SetHTML("<br />");
                else
                    jq("#mobileMessageBody").val("");

                jq('#sendEmailPanel div.action-block #backButton').hide();
                ASC.CRM.ListContactView.changePageTitle(pageTitles.contacts);
            });
        },

        changePageTitle: function(title) {
            jq("div.mainContainerClass div.containerHeaderBlock table tbody tr td div").text(title);
        },

        emailInsertTag: function() {
            var isCompany = jq("#emailTagTypeSelector option:selected").val() == "company";
            var tagName = isCompany ? jq('#emailCompanyTagSelector option:selected').val() : jq('#emailPersonTagSelector option:selected').val();

            if (!jq.browser.mobile) {
                ASC.CRM.ListContactView._fckEditor.InsertHtml(tagName);
            } else {
                var caretPos = jq("#mobileMessageBody").caret().begin;
                var oldText = jq("#mobileMessageBody").val();
                var newText = oldText.slice(0, caretPos) + tagName + oldText.slice(caretPos);
                jq("#mobileMessageBody").val(newText);
            }

        },

        renderTagSelector: function() {
            var isCompany = jq("#emailTagTypeSelector option:selected").val() == "company";
            if (isCompany) {
                jq("#emailPersonTagSelector").hide();
                jq("#emailCompanyTagSelector").show();
            } else {
                jq("#emailCompanyTagSelector").hide();
                jq("#emailPersonTagSelector").show();
            }
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

        renderSimpleContent: function() {
            if (typeof entityData != "undefined" && entityData != null && entityData.id != 0) {
                LoadingBanner.displayLoading();
                Teamlab.getCrmEntityMembers({}, entityData.type, entityData.id, { success: ASC.CRM.ListContactView.CallbackMethods.render_simple_content });
            }
        },

        removeMember: function(contactID) {
            Teamlab.removeCrmEntityMember({ contactID: contactID }, entityData.type, entityData.id, contactID, {
                before: function(params) {
                    jq("#trashImg_" + params.contactID).hide();
                    jq("#loaderImg_" + params.contactID).show();
                },
                after: ASC.CRM.ListContactView.CallbackMethods.removeMember
            });
        },

        addMember: function(contactID) {
            var data =
        {
            contactid: contactID,
            personid: contactID,
            caseid: entityData.id,
            companyid: entityData.id,
            opportunityid: entityData.id

        };
            Teamlab.addCrmEntityMember({}, entityData.type, entityData.id, contactID, data, { success: ASC.CRM.ListContactView.CallbackMethods.addMember });
        }

    };
})();


ASC.CRM.ContactDetailsView = (function($) {

    return {
        init: function() {

            if (!jq.browser.mobile) {
                new AjaxUpload('changeLogo', {
                    action: 'ajaxupload.ashx?type=ASC.Web.CRM.Classes.ContactPhotoHandler,ASC.Web.CRM',
                    autoSubmit: true,
                    data: {
                        contactID: jq.getURLParam("id")
                    },
                    onChange: function(file, extension) {

                        if (jQuery.inArray("." + extension, imgExst) == -1) {

                            alert(CRMJSResources.ErrorMessage_NotImageSupportFormat);

                            return false;
                        }

                        PopupKeyUpActionProvider.CloseDialog();
                        jq(".under_logo #linkChangePhoto").hide();
                        jq(".under_logo .ajax_info_block").show();

                        return true;

                    },
                    onComplete: function(file, response) {

                        var responseObj = jq.evalJSON(response);

                        if (!responseObj.Success) {
                            alert(CRMJSResources.ErrorMessage_SaveImageError);
                            jq(".under_logo .ajax_info_block").hide();
                            jq(".under_logo #linkChangePhoto").show();
                            return;
                        }

                        var now = new Date();

                        jq("#contactProfile .additionInfo .contact_photo").attr("src", jq.evalJSON(response).Data);
                        jq(".under_logo .ajax_info_block").hide();
                        jq(".under_logo #linkChangePhoto").show();
                    },
                    parentDialog: jq("#divLoadPhotoWindow"),
                    isInPopup: true,
                    name: "changeLogo"
                });
            }

            if (typeof (contactNetworks) != "undefined" && contactNetworks != "") {
                var $currentContainer;
                for (var i = 0, n = contactNetworks.length; i < n; i++) {
                    if (contactNetworks[i].InfoType == 7) {//Address

                        var address = jq.parseJSON(Encoder.htmlDecode(contactNetworks[i].Data));

                        var text = Encoder.htmlEncode(address.street);
                        var tmp = address.city != "" ? Encoder.htmlEncode(address.city) + ", " : "";

                        if (address.state != "") { tmp += Encoder.htmlEncode(address.state) + ", "; }
                        if (address.zip != "") { tmp += Encoder.htmlEncode(address.zip); }
                        tmp = tmp.trim();
                        tmp = tmp.charAt(tmp.length - 1) === ',' ? tmp.substring(0, tmp.length - 1) : tmp;
                        if (tmp != "") { text = text != "" ? text + ",<br/>" + tmp : tmp; }
                        text = text != "" && address.country != "" ? text + ",<br/>" + address.country : text;


                        var href = "";
                        if (address.street != "") { href += Encoder.htmlEncode(address.street) + ", "; }
                        if (address.city != "") { href += Encoder.htmlEncode(address.city) + ", "; }
                        if (address.state != "") { href += Encoder.htmlEncode(address.state) + ", "; }
                        if (address.zip != "") { href += Encoder.htmlEncode(address.zip) + ", "; }
                        if (address.country != "") { href += address.country + ", "; }
                        href = href.trim();
                        href = href.charAt(href.length - 1) === ',' ? href.substring(0, href.length - 1) : href;


                        var $Obj = jq("#addressTmpl").tmpl();
                        $Obj.html(text);
                        jq("#showOnMapAndAddressTypeTmpl").tmpl({ 'value': href, 'category': contactNetworks[i].CategoryName }).appendTo($Obj);
                        jq($Obj).appendTo(jq('#generalList dd.addressContainerDetails'))
                        jq('#generalList .addressContainerDetails').removeClass("hiddenFields");
                        continue;
                    }


                    if (i == 0 || contactNetworks[i - 1].InfoType != contactNetworks[i].InfoType) {
                        var item = contactNetworks[i];
                        $currentContainer = jq("#collectionContainerTmpl").tmpl({ Type: item.InfoTypeLocalName }).insertBefore(jq('#generalList dt.addressContainerDetails')).filter('dd');
                        if (item.InfoType == 2) { //Website
                            if (item.Data.indexOf("://") == -1)
                                item.Href = "http://" + item.Data;
                            else
                                item.Href = item.Data;
                        }
                        jq("#collectionTmpl").tmpl(item).appendTo($currentContainer);
                    }
                    else {
                        var item = contactNetworks[i];
                        if (item.InfoType == 2) { //Website
                            if (item.Data.indexOf("://") == -1)
                                item.Href = "http://" + item.Data;
                            else
                                item.Href = item.Data;
                        }
                        jq("#collectionTmpl").tmpl(item).appendTo($currentContainer);
                    }
                }

            }

            for (var i = 0, n = customFieldList.length; i < n; i++) {
                var field = customFieldList[i];
                if (jQuery.trim(field.mask) == "") continue;
                field.mask = jq.evalJSON(field.mask);
            }

            jq("#customFieldTmpl").tmpl(customFieldList).appendTo("#generalList");

            jq("#generalList dt.headerBase").each(
                function(index) {
                    var item = jq(this);

                    if (item.next().nextUntil("dt.headerBase").length == 0) {
                        item.next().remove();
                        item.remove();
                        return;
                    }
                    if (index != 0) {
                        jq(this).click();
                    }
                });

            var colors = [];
            for (var i = 0; i < sliderListItems.Items.length; i++) {
                colors[i] = sliderListItems.Items[i].Color;
            }
            var values = [];
            values[0] = "";
            var status = 0;
            for (var i = 0; i < sliderListItems.Items.length; i++) {
                values[i + 1] = sliderListItems.Items[i].Title;
                if (sliderListItems.Items[i].ID == sliderListItems.Status)
                    status = i + 1;
            }

            if (jq('#loyaltySliderDetails').length != 0) {
                jq('#loyaltySliderDetails').sliderWithSections({ value: status, values: values, max: sliderListItems.PositionsCount, colors: colors, marginWidth: 1,
                    sliderOptions: {
                        stop: function(event, ui) {
                            if (ui.value != 0) {
                                ASC.CRM.ContactDetailsView.changeStatus(sliderListItems.Items[ui.value - 1].ID);
                            }
                            else {
                                ASC.CRM.ContactDetailsView.changeStatus(0);
                            }

                        }
                    }
                });
            }

            if (jq("#listContactsToMerge").length != 0) {
                jq("#listContactsToMerge input[type='radio']:first").attr("checked", true);
                jq("#listContactsToMerge #contactTitle").attr("disabled", true);
                jq("#listContactsToMerge input[type='radio']").click(function() {
                    if (jq(this).is('#radioBtn_0')) {
                        jq("#listContactsToMerge #contactTitle").attr("disabled", false);
                    }
                    else {
                        jq("#listContactsToMerge #contactTitle").attr("disabled", true);
                    }
                });

            }
        },

        changeStatus: function(statusValue) {
            AjaxPro.onLoading = function(b) { };
            AjaxPro.ContactDetailsView.ChangeStatus(sliderListItems.ID, statusValue, function(res) {
                if (res.error != null) {
                    alert(res.message);
                }
            });
        },
        showMergePanel: function(isCompany, contactsToMergeCount) {
            jq("#mergePanel .infoPanel").hide();
            ASC.CRM.Common.blockUI("#mergePanel", 400, 400, 0);
        },

        mergeContacts: function(fromID, isCompany) {
            var toID = jq("#mergePanel input[name=contactToMerge]:checked").val() * 1;

            if (toID == 0) {
                if (typeof (contactToMergeSelector.SelectedContacts[0]) == "undefined") {
                    jq("#mergePanel .infoPanel > div").text(CRMJSResources.ErrorContactIsNotSelected);
                    jq("#mergePanel .infoPanel").show();
                    return;
                }
                toID = contactToMergeSelector.SelectedContacts[0] * 1;
            }

            jq("#mergePanel .action_block").hide();
            jq("#mergePanel div.ajax_info_block").show();

            AjaxPro.ContactDetailsView.MergeContacts(fromID, toID, isCompany == "true", function(res) {
                if (res.error != null) {
                    jq("#mergePanel .action_block").show();
                    jq("#mergePanel div.ajax_info_block").hide();
                    alert(res.error.Message);
                    return;
                }
                else {
                    location.href = res.value;
                }

            });
        },

        removePersonFromCompany: function(id) {
            Teamlab.removeCrmEntityMember({ contactID: id }, entityData.type, entityData.id, id, {
                before: function(params) {
                    jq("#trashImg_" + params.contactID).hide();
                    jq("#loaderImg_" + params.contactID).show();
                },
                after: function(params) {

                    var index = jq.inArray(params.contactID, contactSelector.SelectedContacts);
                    contactSelector.SelectedContacts.splice(index, 1);

                    jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                    //ASC.CRM.Common.changeCountInTab("delete", "contacts");

                    setTimeout(function() {
                        jq("#contactItem_" + params.contactID).remove();
                        if (contactSelector.SelectedContacts.length == 0) {
                            jq("#addPeopleButton").hide();
                            jq("#peopleInCompanyPanel").hide();
                            jq("#emptyPeopleInCompanyPanel").show();
                        }
                    }, 500);

                }
            });
        },

        addPersonToCompany: function(obj) {
            if (jq("#contactItem_" + obj.id).length > 0) return false;
            var data =
                {
                    personid: obj.id,
                    companyid: entityData.id
                };
            Teamlab.addCrmEntityMember({}, entityData.type, entityData.id, obj.id, data, {
                success: function(params, contact) {
                    jq("#simpleContactTmpl").tmpl(contact).prependTo("#contactTable tbody");
                    //ASC.CRM.Common.changeCountInTab("add", "contacts");
                    ASC.CRM.Common.RegisterContactInfoCard();

                    contactSelector.SelectedContacts.push(contact.id);
                    jq("#emptyPeopleInCompanyPanel").hide();
                }
            });

        }

    }

})(jQuery);



ASC.CRM.ContactActionView = (function($) {
    var isInit = false;
    var cache = {};
    this.ContactData = null;

    var _createNewAddress = function($contact, is_primary, category, street, city, state, zip, country) {
        if (jq("#addressContainer").children("div").length != 1)
            $contact.attr("style", "margin-top: 10px;");

        if (typeof (is_primary) != "undefined")
            _changeAddressPrimaryCategory($contact, is_primary);
        if (typeof (category) != "undefined") {
            var $categoryOption = $contact.find('select.address_category').children('option[category="' + category + '"]');
            if ($categoryOption.length == 1) {
                $categoryOption.attr("selected", "selected");
                _changeAddressCategory($contact.find('select.address_category'), category);
            }
        }
        var parts = jq("#addressContainer").children("div:last").attr('selectname').split('_');
        var ind = parts[2] * 1 + 1;
        $contact.find('input, textarea, select').not('.address_category').each(function() {
            if (jq(this).attr('name') != "") {
                var parts = jq(this).attr('name').split('_');
                parts[2] = ind;
                jq(this).attr('name', parts.join('_'));
            }
        });
        var parts = $contact.attr('selectname').split('_');
        parts[2] = ind;
        $contact.attr('selectname', parts.join('_'));

        if (street && street != "") $contact.find('textarea.contact_street').val(street);
        if (city && city != "") $contact.find('input.contact_city').val(city);
        if (state && state != "") $contact.find('input.contact_state').val(state);
        if (zip && zip != "") $contact.find('input.contact_zip').val(zip);
        if (country && country != "") $contact.find('select.contact_country').val(country);

        $contact.find('textarea.contact_street').Watermark(CRMJSResources.AddressWatermark, ASC.CRM.ContactActionView.WatermarkClass);
        $contact.find('input.contact_city').Watermark(CRMJSResources.CityWatermark, ASC.CRM.ContactActionView.WatermarkClass);
        $contact.find('input.contact_state').Watermark(CRMJSResources.StateWatermark, ASC.CRM.ContactActionView.WatermarkClass);
        $contact.find('input.contact_zip').Watermark(CRMJSResources.ZipCodeWatermark, ASC.CRM.ContactActionView.WatermarkClass);

        $contact.find('select.contact_country').attr('name', $contact.attr('selectname'));
        jq('<option value="" style="display:none;"></option>').prependTo($contact.find('select.contact_country'));

        return $contact;
    };


    var _changeAddressPrimaryCategory = function($divAddressObj, isPrimary) {
        var tmpNum = isPrimary ? 1 : 0;
        var tmpClass = isPrimary ? "is_primary primary_field" : "is_primary not_primary_field";

        var $switcerObj = $divAddressObj.find('.is_primary');

        $switcerObj.attr("class", tmpClass);
        $switcerObj.attr("alt", CRMJSResources.Primary);
        $switcerObj.attr("title", CRMJSResources.Primary);

        var parts = $divAddressObj.attr('selectname').split('_');
        parts[5] = tmpNum;
        $divAddressObj.attr('selectname', parts.join('_'));

        $divAddressObj.find('input, textarea, select').not('.address_category').each(function() {
            var parts = jq(this).attr('name').split('_');
            parts[5] = tmpNum;
            jq(this).attr('name', parts.join('_'));
        });
    };


    var _changeCommunicationPrimaryCategory = function($divObj, isPrimary) {
        var tmpNum = isPrimary ? 1 : 0;
        var tmpClass = isPrimary ? "is_primary primary_field" : "is_primary not_primary_field";

        var $switcerObj = $divObj.find('.is_primary');

        $switcerObj.attr("class", tmpClass);
        $switcerObj.attr("alt", CRMJSResources.Primary);
        $switcerObj.attr("title", CRMJSResources.Primary);

        var $inputObj = $divObj.find('input.textEdit');
        var parts = $inputObj.attr('name').split('_');

        parts[4] = tmpNum;
        $inputObj.attr('name', parts.join('_'));
    };

    var _changeBaseCategory = function(Obj, text, category) {
        jq(Obj).text(text);
        var $inputObj = jq(Obj).parents('tr:first').find('input');
        var parts = $inputObj.attr('name').split('_');
        parts[3] = category;
        $inputObj.attr('name', parts.join('_'));
        jq("#baseCategoriesPanel").hide();
    };

    var _changePhoneCategory = function(Obj, text, category) {
        jq(Obj).text(text);
        var $inputObj = jq(Obj).parents('tr:first').find('input');
        var parts = $inputObj.attr('name').split('_');
        parts[3] = category;
        $inputObj.attr('name', parts.join('_'));
        jq("#phoneCategoriesPanel").hide();
    };

    var _changeAddressCategory = function(switcerObj, category) {
        jq(switcerObj).parents('table:first').find('input, textarea, select').not('.address_category').each(function() {
            var parts = jq(this).attr('name').split('_');
            parts[3] = category;
            jq(this).attr('name', parts.join('_'));
        });
    }

    var removeAssignedPersonFromCompany = function(id) {

        jq("#trashImg_" + id).hide();
        jq("#loaderImg_" + id).show();

        if(typeof (id) == "number") {
            var index = jq.inArray(id, assignedContactSelector.SelectedContacts);
            assignedContactSelector.SelectedContacts.splice(index, 1);
        } else {
            for(var i=0; i<ASC.CRM.SocialMedia.selectedPersons.length; i++)
               if (ASC.CRM.SocialMedia.selectedPersons[i].id == id) {
                    ASC.CRM.SocialMedia.selectedPersons.splice(i, 1);
                    break;
                }
        }

        jq("#contactItem_" + id).animate({ opacity: "hide" }, 500);

        setTimeout(function() {
            jq("#contactItem_" + id).remove();
            if (jq("#contactTable tr").length == 0) {
                jq("#contactListBox").parent().addClass('hiddenFields');
            }
        }, 500);
    };

    var addAssignedPersonToCompany = function(obj) {
        if (jq("#contactItem_" + obj.id).length > 0) return false;

        Teamlab.getCrmContact({}, obj.id, {
            success: function(params, contact) {
                jq("#simpleContactTmpl").tmpl(contact).prependTo("#contactTable tbody");
                jq("#contactListBox").parent().removeClass('hiddenFields');
                ASC.CRM.Common.RegisterContactInfoCard();

                assignedContactSelector.SelectedContacts.push(contact.id);

                jq("#assignedContactsListEdit .assignedContacts").addClass('hiddenFields');
            }
        });

    };

    var validateEmail = function($emailInputObj) {
        var email = $emailInputObj.value.trim();
        var $tableObj = jq($emailInputObj).parents("table:first");
        var reg = new RegExp("^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(?:\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(?:aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$", 'i');

        if (email == "" || reg.test(email)) {
            $tableObj.css("borderColor", "");
            $tableObj.parent().children(".requiredErrorText").hide();
            return true;
        }
        else {
            $tableObj.css("borderColor", "#CC0000");
            $tableObj.parent().children(".requiredErrorText").show();
            $emailInputObj.focus();
            return false;
        }

    };

    var validatePhone = function($phoneInputObj) {
        var phone = $phoneInputObj.value.trim();
        var $tableObj = jq($phoneInputObj).parents("table:first");
        var reg = new RegExp(/(^\+)?(\d+)/);

        if (phone == "" || reg.test(phone)) {
            $tableObj.css("borderColor", "");
            $tableObj.parent().children(".requiredErrorText").hide();
            return true;
        }
        else {
            $tableObj.css("borderColor", "#CC0000");
            $tableObj.parent().children(".requiredErrorText").show();
            $phoneInputObj.focus();
            return false;
        }

    };

    return {
        init: function(dateMask) {
            if (isInit === false) {
                isInit = true;
                ASC.CRM.ListContactView.renderSimpleContent();
                ASC.CRM.ContactActionView.WatermarkClass = "crm-watermarked";

                ASC.CRM.Common.renderCustomFields(customFieldList, "custom_field_", "customFieldTmpl", "#generalListEdit");
                jq("#generalListEdit .headerExpand:not(:first)").click();

                jq("input.textEditCalendar").mask(dateMask);
                jq("input.textEditCalendar").datepicker();

                jq("#generalListEdit .not_primary_field").live('click', function() {
                    ASC.CRM.ContactActionView.choosePrimaryElement(jq(this), jq(this).parent().parent().parent().attr("id") == "addressContainer");
                });

                if (typeof (contactNetworks) != "undefined" && contactNetworks != "") {
                    for (var i = 0; i < contactNetworks.length; i++) {
                        if (contactNetworks[i].hasOwnProperty("InfoType") && contactNetworks[i].hasOwnProperty("Data")) {
                            if (contactNetworks[i].InfoType == 7) {//Address

                                var address = jq.parseJSON(Encoder.htmlDecode(contactNetworks[i].Data));

                                var $addressJQ = _createNewAddress(jq('#addressContainer').children('div:first').clone(), contactNetworks[i].IsPrimary, contactNetworks[i].Category, address.street, address.city, address.state, address.zip, address.country);

                                $addressJQ.insertAfter(jq('#addressContainer').children('div:last')).show();

                                continue;
                            }

                            var container_id;
                            var $newContact;

                            if (contactNetworks[i].InfoType == 0) {//Phone
                                container_id = 'phoneContainer';

                                $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, Encoder.htmlDecode(contactNetworks[i].Data), contactNetworks[i].IsPrimary);

                                _changePhoneCategory(
                                    $newContact.children('table').find('a'),
                                    jq("#phoneCategoriesPanel div.dropDownContent").children('a[category=' + contactNetworks[i].Category + ']').text(),
                                    contactNetworks[i].Category);
                            }
                            else if (contactNetworks[i].InfoType == 1) {//Email
                                container_id = 'emailContainer';

                                $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, Encoder.htmlDecode(contactNetworks[i].Data), contactNetworks[i].IsPrimary);

                                _changeBaseCategory(
                                    $newContact.children('table').find('a'),
                                    jq("#baseCategoriesPanel div.dropDownContent").children('a[category=' + contactNetworks[i].Category + ']').text(),
                                    contactNetworks[i].Category);
                            }
                            else {
                                container_id = 'websiteAndSocialProfilesContainer';

                                $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, Encoder.htmlDecode(contactNetworks[i].Data));

                                _changeBaseCategory(
                                    $newContact.find('a.social_profile_category'),
                                    jq("#baseCategoriesPanel div.dropDownContent").children('a[category=' + contactNetworks[i].Category + ']').text(),
                                    contactNetworks[i].Category);

                                ASC.CRM.ContactActionView.changeSocialProfileCategory(
                                    $newContact.find('a.social_profile_type'),
                                    contactNetworks[i].InfoType,
                                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category=' + contactNetworks[i].InfoType + ']').text(),
                                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category=' + contactNetworks[i].InfoType + ']').attr('categoryName'));
                            }


                            $newContact.insertAfter(jq('#' + container_id).children('div:last')).show();

                            continue;
                        }
                    }

                    var add_new_button_class = "crm-addNewLink";
                    var delete_button_class = "crm-deleteLink";
                    if (jq('#emailContainer').children('div').length > 1) jq('#emailContainer').prev('dt').removeClass('crm-withGrayPlus');
                    jq('#emailContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
                    jq('#emailContainer').children('div:last').find("." + add_new_button_class).show();

                    if (jq('#phoneContainer').children('div').length > 1) jq('#phoneContainer').prev('dt').removeClass('crm-withGrayPlus');
                    jq('#phoneContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
                    jq('#phoneContainer').children('div:last').find("." + add_new_button_class).show();

                    if (jq('#websiteAndSocialProfilesContainer').children('div').length > 1) jq('#websiteAndSocialProfilesContainer').prev('dt').removeClass('crm-withGrayPlus');
                    jq('#websiteAndSocialProfilesContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
                    jq('#websiteAndSocialProfilesContainer').children('div:last').find("." + add_new_button_class).show();

                    if (jq('#addressContainer').children('div').length > 1) jq('#addressContainer').prev('dt').removeClass('crm-withGrayPlus');
                    jq('#addressContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
                    jq('#addressContainer').children('div:last').find("." + add_new_button_class).show();
                }

                jq("#generalListEdit .crm-withGrayPlus").live("click", function(event) {
                    var container_id = jq(this).next('dd').attr('id');
                    //jq(this).next('dd').find(".crm-addNewLink").show();

                    if (container_id == "addressContainer")
                        ASC.CRM.ContactActionView.editAddress(jq("#addressContainer > div:first .crm-addNewLink"));
                    else
                        ASC.CRM.ContactActionView.editCommunications(jq("#" + container_id).children("div:first").find(".crm-addNewLink"), container_id);

                    jq(this).removeClass("crm-withGrayPlus");
                });

                jq.dropdownToggle({ dropdownID: 'phoneCategoriesPanel', switcherSelector: '#phoneContainer .input_with_type a', addTop: 0, addLeft: 0 });
                jq.dropdownToggle({ dropdownID: 'baseCategoriesPanel', switcherSelector: '#emailContainer .input_with_type a', noActiveSwitcherSelector: '#websiteAndSocialProfilesContainer .input_with_type a.social_profile_category', addTop: 0, addLeft: 0 });
                jq.dropdownToggle({ dropdownID: 'baseCategoriesPanel', switcherSelector: '#websiteAndSocialProfilesContainer .input_with_type a.social_profile_category', noActiveSwitcherSelector: '#emailContainer .input_with_type a', addTop: 0, addLeft: 0 });
                jq.dropdownToggle({ dropdownID: 'socialProfileCategoriesPanel', switcherSelector: '#websiteAndSocialProfilesContainer .input_with_type a.social_profile_type', addTop: 0, addLeft: 0 });


                //                jq("#phoneContainer input").live("keypress", function(event) {
                //                    // Backspace, Del, (, )
                //                    var controlKeys = [8, 9, 40, 41];
                //                    var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
                //                    if (!event.which || (48 <= event.which && event.which <= 57) || isControlKey ||
                //                        (jq(this).val().length - jq(this).val().replace('+', '').length < 1 &&
                //                            event.which == 43))  // KeyCode of '+' = 43
                //                    {
                //                        return;
                //                    } else {
                //                        event.preventDefault();
                //                    }
                //                });

                //                jq("#phoneContainer input").live('paste', function(e) {
                //                    var oldValue = this.value;
                //                    var $obj = this;
                //                    setTimeout(
                //                       function() {
                //                           var text = jq($obj).val();
                //                           //var reg = ???;
                //                           if (!reg.test(text)) {
                //                               jq($obj).val(oldValue);
                //                           }
                //                       }, 0);
                //                    return true;
                //                });

                if (typeof (assignedContactSelector) != "undefined") {
                    if (assignedContactSelector.SelectedContacts.length == 0)
                        jq("#contactListBox").parent().addClass('hiddenFields');
                    assignedContactSelector.SelectItemEvent = addAssignedPersonToCompany;
                    ASC.CRM.ListContactView.removeMember = removeAssignedPersonFromCompany;
                }

                if (jq("#typeAddedContact").val() == "people") {
                    setInterval(function() {
                        var $input1 = jq("#contactProfileEdit .info_for_person input[name=baseInfo_firstName]");
                        var $input2 = jq("#contactProfileEdit .info_for_person input[name=baseInfo_lastName]");

                        if ($input1.val().trim() == "" || $input2.val().trim() == "") {
                            jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Enabled").hide();
                            jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Disabled").show();
                        }
                        else {
                            jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Disabled").hide();
                            jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Enabled").show();
                        }
                    }, 500);
                }
                else {
                    setInterval(function() {
                        var $input = jq("#contactProfileEdit .info_for_company input[name=baseInfo_companyName]");
                        if ($input.val().trim() == "") {
                            jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Enabled").hide();
                            jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Disabled").show();
                        }
                        else {
                            jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Disabled").hide();
                            jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Enabled").show();
                        }
                    }, 500);
                }
            }
        },

        editCommunicationsEvent: function(evt, container_id) {
            evt = evt || window.event;
            var $target = jq(evt.target || evt.srcElement);
            ASC.CRM.ContactActionView.editCommunications($target, container_id);
        },

        editCommunications: function($target, container_id) {
            var add_new_button_class = "crm-addNewLink";
            var delete_button_class = "crm-deleteLink";
            var primary_class = "primary_field";

            if ($target.length == 0 && container_id == "overviewContainer" || $target.hasClass(add_new_button_class)) {
                var $lastVisibleDiv = jq('#' + container_id).children('div:visible:last');
                if ($lastVisibleDiv.length != 0) {
                    $lastVisibleDiv.find("." + add_new_button_class).hide();
                }

                var is_primary = jq('#' + container_id).find(".primary_field").length == 0 ? true : undefined;
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, "", is_primary);

                //if ($newContact.children(".actions_for_item").children(".is_primary").length != 0
                //           && jq('#' + container_id).find(".primary_field").length == 0)
                //   ASC.CRM.ContactActionView.changePrimaryCategory($newContact.children(".actions_for_item").children(".is_primary"), true, false);

                $newContact.insertAfter(jq('#' + container_id).children('div:last')).show()

                $newContact.find('input.textEdit').focus();

            } else if ($target.hasClass(delete_button_class)) {
                var $divHTML = $target.parent().parent();
                if (jq('#' + container_id).children('div').length == 2) {
                    $divHTML.parent().prev('dt').addClass("crm-withGrayPlus");
                }

                $divHTML.remove();
                if ($divHTML.find('.' + primary_class).length == 1 && jq('#' + container_id).children('div:not(:first)').length >= 1) {
                    ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#' + container_id).children('div:not(:first)')[0]).find('.is_primary'), false);
                }
                jq('#' + container_id).children('div:not(:first)').find("." + add_new_button_class).hide();
                jq('#' + container_id).children('div:last').find("." + add_new_button_class).show();
            }
        },

        createNewCommunication: function(container_id, text, is_primary) {
            var $contact = jq('#' + container_id).children('div:first').clone();

            if (container_id != "overviewContainer") {
                var $lastInputElement = jq('#' + container_id).children('div:last').find('input.textEdit');
                if ($lastInputElement.length != 0) {
                    var parts = $lastInputElement.attr('name').split('_');
                    var ind = parts[2] * 1 + 1;

                    parts = $contact.find('input.textEdit').attr('name').split('_');
                    parts[2] = ind;
                    $contact.find('input.textEdit').attr('name', parts.join('_'));
                }

                if (text && text != "")
                    $contact.children('table').find('input').val(text);

                if (container_id === "phoneContainer")
                    jq.forcePhoneSymbolsOnly($contact.children('table').find('input'));

                if (typeof (is_primary) != "undefined") {
                    var $isPrimaryElem = $contact.children(".actions_for_item").children(".is_primary");
                    if ($isPrimaryElem.length != 0) {
                        _changeCommunicationPrimaryCategory($contact, is_primary);
                    }
                }
            }
            else {
                if (text && text != "")
                    $contact.children('textarea').val(text);
            }
            return $contact;
        },


        editAddressEvent: function(evt) {
            evt = evt || window.event;
            var $target = jq(evt.target || evt.srcElement);
            ASC.CRM.ContactActionView.editAddress($target);
        },

        editAddress: function($target) {
            var add_new_button_class = "crm-addNewLink";
            var delete_button_class = "crm-deleteLink";
            var primary_class = "primary_field";
            if ($target.hasClass(add_new_button_class)) {
                var $lastVisibleDiv = jq('#addressContainer').children('div:visible:last');
                if ($lastVisibleDiv.length != 0) {
                    $lastVisibleDiv.find("." + add_new_button_class).hide();
                }
                var is_primary = jq("#addressContainer").find(".primary_field").length == 0 ? true : undefined;
                var $newContact = _createNewAddress(jq('#addressContainer').children('div:first').clone(), is_primary).insertAfter(jq('#addressContainer').children('div:last')).show();
                $newContact.find('textarea').focus();
            } else if ($target.hasClass(delete_button_class)) {
                var $divHTML = $target.parent().parent();

                if (jq('#addressContainer').children('div').length == 2) {
                    $divHTML.parent().prev('dt').addClass("crm-withGrayPlus");
                }
                $divHTML.remove();

                if ($divHTML.find('.' + primary_class).length == 1 && jq('#addressContainer').children('div:not(:first)').length >= 1) {
                    ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#addressContainer').children('div:not(:first)')[0]).find('.is_primary'), true);
                }
                jq('#addressContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
                jq('#addressContainer').children('div:last').find("." + add_new_button_class).show();
            }
        },


        changeSocialProfileCategory: function(Obj, category, text, categoryName) {
            var $divObj = jq(Obj).parents('table:first').parent();
            jq(Obj).text(text);
            $inputObj = jq(Obj).parents('tr:first').find('input');
            var parts = $inputObj.attr('name').split('_');
            parts[1] = categoryName;
            $inputObj.attr('name', parts.join('_'));

            var $findProfileObj = $divObj.find('.find_profile');
            var isShown = false;
            var func = "";
            var title = "";
            var description = " ";
            switch (category) {
                case 4:
                    isShown = true;
                    title = CRMJSResources.FindTwitter;
                    description = CRMJSResources.ContactTwitterDescription;
                    func = (function(p1, p2) { return function() { ASC.CRM.SocialMedia.FindTwitterProfiles(jq(this), jq("#typeAddedContact").val(), p1, p2); } })(-3, 5)
                    break;
                case 5:
                    if (jq("#typeAddedContact").val() === "company") isShown = false;
                    else isShown = true;
                    title = CRMJSResources.FindLinkedIn;
                    description = CRMJSResources.ContactLinkedInDescription;
                    func = (function(p1, p2) { return function() { ASC.CRM.SocialMedia.FindLinkedInProfiles(jq(this), jq("#typeAddedContact").val(), p1, p2); } })(-3, 5)
                    break;

                case 6:
                    isShown = true;
                    title = CRMJSResources.FindFacebook;
                    description = CRMJSResources.ContactFacebookDescription;
                    func = (function(p1, p2) { return function() { ASC.CRM.SocialMedia.FindFacebookProfiles(jq(this), jq("#typeAddedContact").val(), p1, p2); } })(-3, 5);
                    break;

            }

            if (isShown) {
                $findProfileObj.unbind('click').click(func);
                $findProfileObj.attr('title', title).show();
            }
            else { $findProfileObj.hide(); }

            $divObj.children(".textMediumDescribe").text(description);
            jq("#socialProfileCategoriesPanel").hide();
        },

        choosePrimaryElement: function(switcerObj, isAddress) {
            if (!isAddress) {
                var $divObj = jq(switcerObj).parent().parent();
                _changeCommunicationPrimaryCategory($divObj, true);

                jq(switcerObj).parents('dd:first').children('div:not(:first)').not($divObj).each(function() {
                    _changeCommunicationPrimaryCategory(jq(this), false);
                });
            }
            else {
                var $divAddressObj = jq(switcerObj).parent().parent();
                _changeAddressPrimaryCategory($divAddressObj, true);

                jq(switcerObj).parents('dd:first').children('div:not(:first)').not($divAddressObj).each(function() {
                    _changeAddressPrimaryCategory(jq(this), false);
                });
            }

            jq(switcerObj).parents('dd:first').find('.is_primary').not(switcerObj).attr("class", "is_primary not_primary_field");
            jq(switcerObj).parents('dd:first').find('.is_primary').not(switcerObj).attr("alt", CRMJSResources.CheckAsPrimary);
            jq(switcerObj).parents('dd:first').find('.is_primary').not(switcerObj).attr("title", CRMJSResources.CheckAsPrimary);
        },


        changeContactPhoto: function(file, response) {
            jq('#contactProfileEdit').unblock();
            var result = jq.parseJSON("(" + response + ")");
            if (result.Success) {
                jq('#uploadPhotoMessage').html('');
                jq('#uploadPhotoPath').val(result.Data);
                jq('#contactPhoto').html('<img alt="" class="contactPhoto" src="' + result.Data + '" />');
            }
            else {
                jq('#uploadPhotoMessage').html('<div class="errorBox">' + result.Message + '</div>');
            }
        },

        confirmForDelete: function(isCompany, contactName) {
            if (isCompany == 1) {
                return confirm(jq.format(CRMJSResources.DeleteCompanyConfirmMessage, Encoder.htmlDecode(contactName)) + "\n" + CRMJSResources.DeleteConfirmNote);
            }
            else {
                return confirm(jq.format(CRMJSResources.DeletePersonConfirmMessage, Encoder.htmlDecode(contactName)) + "\n" + CRMJSResources.DeleteConfirmNote)
            }
        },

        submitForm: function(buttonUnicId) {

            jq("input, select, textarea").attr("readonly", "readonly").addClass('disabled');
            jq("#contactProfileEdit .input_with_type").addClass('disabled');
            HideRequiredError();

            if (jq("#typeAddedContact").val() == "people") {
                var isValid = true;
                if (jq("#contactProfileEdit input[name=baseInfo_firstName]").val().trim() == "") {
                    ShowRequiredError(jq("#contactProfileEdit input[name=baseInfo_firstName]"));
                    isValid = false;
                }

                if (jq("#contactProfileEdit input[name=baseInfo_lastName]").val().trim() == "") {
                    ShowRequiredError(jq("#contactProfileEdit input[name=baseInfo_lastName]"));
                    isValid = false;
                }

                if (!isValid) {
                    jq("#contactProfileEdit input, select, textarea").removeAttr("readonly").removeClass('disabled');
                    jq("#contactProfileEdit .input_with_type").removeClass('disabled');
                    return false;
                }


                if (typeof (companySelector) != "undefined") {
                    jq("#companySelectorsContainer input[name=baseInfo_compID]").val(typeof (companySelector.SelectedContacts[0]) != 'undefined' ? companySelector.SelectedContacts[0] : "");
                    jq("#companySelectorsContainer input[name=baseInfo_compName]").val(jq('#contactTitle_companySelector_0').hasClass(ASC.CRM.ContactActionView.WatermarkClass) ? "" : jq('#contactTitle_companySelector_0').val());
                }
            }
            else {
                var isValid = true;
                if (jq("#contactProfileEdit input[name=baseInfo_companyName]").val().trim() == "") {
                    ShowRequiredError(jq("#contactProfileEdit input[name=baseInfo_companyName]"));
                    isValid = false;
                }

                if (!isValid) {
                    jq("#contactProfileEdit input, select, textarea").removeAttr("readonly").removeClass('disabled');
                    jq("#contactProfileEdit .input_with_type").removeClass('disabled');
                    return false;
                }
            }


            var isEmailValid = true;
            jq("#emailContainer > div:not(:first) > table input").each(function() {
                if (!validateEmail(this)) { isEmailValid = false; }
            });

            if (!isEmailValid) {
                jq("#contactProfileEdit input, select, textarea").removeAttr("readonly").removeClass('disabled');
                jq("#contactProfileEdit .input_with_type").removeClass('disabled');
                jq.scrollTo(jq("#emailContainer").position().top - 100, { speed: 400 });
                return false;
            }

            //            var isPhoneValid = true;
            //            jq("#phoneContainer > div:not(:first) > table input").each(function() {
            //                if (!validatePhone(this)) { isPhoneValid = false; }
            //            });

            //            if (!isPhoneValid) {
            //                jq("#contactProfileEdit input, select, textarea").removeAttr("readonly").removeClass('disabled');
            //                jq("#contactProfileEdit .input_with_type").removeClass('disabled');
            //                jq.scrollTo(jq("#phoneContainer").position().top - 100, { speed: 400 });
            //                return false;
            //            }


            jq("#contactActionPanelButtons").hide();
            jq("#crm_contactMakerDialog .ajax_info_block").show();

            jq('#addressContainer').children('div:first').find('input, textarea, select').attr('name', '');
            jq('#emailContainer').children('div:first').find('input').attr('name', '');
            jq('#phoneContainer').children('div:first').find('input').attr('name', '');
            jq('#websiteAndSocialProfilesContainer').children('div:first').find('input').attr('name', '');
            jq('#overviewContainer').children('div:first').find('textarea').attr('name', '');



            jq('#emailContainer').children('div:not(:first)').find('input').each(function() {
                if (jq(this).val().trim() != '') {
                    jq(this).parents('table:first').parent().addClass("not_empty");
                }
            });
            jq('#phoneContainer').children('div:not(:first)').find('input').each(function() {
                if (jq(this).val().trim() != '') {
                    jq(this).parents('table:first').parent().addClass("not_empty");
                }
            });
            jq('#websiteAndSocialProfilesContainer').children('div:not(:first)').find('input').each(function() {
                if (jq(this).val().trim() != '') {
                    jq(this).parents('table:first').parent().addClass("not_empty");
                }
            });

            jq('#addressContainer').children('div:not(:first)').each(function() {
                if (jq(this).find('.contact_street').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                    && jq(this).find('.contact_city').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                    && jq(this).find('.contact_state').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                    && jq(this).find('.contact_zip').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                    && jq(this).find('.contact_country').val() == CRMJSResources.ChooseCountry) {
                    jq(this).find('input, textarea, select').attr('name', '');
                }
                else {
                    jq(this).addClass("not_empty");
                    jq(this).find('.' + ASC.CRM.ContactActionView.WatermarkClass).val(" ");
                    if (jq(this).find('.contact_country').val() == CRMJSResources.ChooseCountry)
                        jq(this).find('.contact_country').val("");
                }

            });



            if ((jq('#emailContainer').children('div:not(:first)').find('.primary_field').length == 0 ||
                !jq('#emailContainer').children('div:not(:first)').find('.primary_field').parent().parent().hasClass('not_empty'))
                && jq('#emailContainer').children('div.not_empty').length > 0) {
                ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#emailContainer').children('div.not_empty')[0]).children(".actions_for_item").children(".is_primary"), false);
            }

            if ((jq('#phoneContainer').children('div:not(:first)').find('.primary_field').length == 0 ||
                !jq('#phoneContainer').children('div:not(:first)').find('.primary_field').parent().parent().hasClass('not_empty'))
                && jq('#phoneContainer').children('div.not_empty').length > 0) {
                ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#phoneContainer').children('div.not_empty')[0]).children(".actions_for_item").children(".is_primary"), false);
            }


            if ((jq('#addressContainer').children('div:not(:first)').find('.primary_field').length == 0 ||
                !jq('#addressContainer').children('div:not(:first)').find('.primary_field').parent().parent().hasClass('not_empty'))
                && jq('#addressContainer').children('div.not_empty').length > 0) {
                ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#addressContainer').children('div.not_empty')[0]).children(".actions_for_item").children(".is_primary"), true);
            }



            if (!jq("#isPrivate").is(":checked")) {
                SelectedUsers.IDs = new Array();
                jq("#cbxNotify").removeAttr("checked");
            }

            jq("#isPrivateContact").val(jq("#isPrivate").is(":checked"));
            jq("#notifyPrivateUsers").val(jq("#cbxNotify").is(":checked"));
            jq("#selectedPrivateUsers").val(SelectedUsers.IDs.join(","));

            var $checkboxes = jq("#generalListEdit input[type='checkbox'][id^='custom_field_']");
            if ($checkboxes) {
                for (var i = 0; i < $checkboxes.length; i++) {
                    if (jq($checkboxes[i]).is(":checked")) {
                        var id = $checkboxes[i].id.replace('custom_field_', '');
                        jq("#generalListEdit input[name='customField_" + id + "']").val(jq($checkboxes[i]).is(":checked"));
                    }
                }
            }

            if (jq("#typeAddedContact").val() == "people") return true;

            else if(ASC.CRM.SocialMedia.selectedPersons.length == 0) {
                jq("#assignedContactsListEdit input[name='baseInfo_assignedContactsIDs']").val(assignedContactSelector.SelectedContacts);
                return true;
            } else {

                var data = new Array();
                for (var i = 0; i < ASC.CRM.SocialMedia.selectedPersons.length; i++)
                   data.push({
                        Key: ASC.CRM.SocialMedia.selectedPersons[i].Key,
                        Value: ASC.CRM.SocialMedia.selectedPersons[i].Value
                   });

                Teamlab.addCrmPerson({ buttonId: buttonUnicId }, data,
                   {
                        success: function(params, persons) {
                            var personIds = new Array();
                            for (var i = 0; i < persons.length; i++)
                                if (persons[i])
                                    personIds.push(persons[i].id);

                            personIds = personIds.concat(assignedContactSelector.SelectedContacts);
                            jq("#assignedContactsListEdit input[name='baseInfo_assignedContactsIDs']").val(personIds);
                            __doPostBack(params.buttonId, '');
                        }
                    });

                return false;
            }
        },

        showBaseCategoriesPanel: function(switcherUI) {
            jq("#baseCategoriesPanel a.dropDownItem").unbind('click').click(function() {
                _changeBaseCategory(switcherUI, jq(this).text(), jq(this).attr("category"));

            });
        },

        showPhoneCategoriesPanel: function(switcherUI) {
            jq("#phoneCategoriesPanel a.dropDownItem").unbind('click').click(function() {
                _changePhoneCategory(switcherUI, jq(this).text(), jq(this).attr("category"));
            });
        },

        showSocialProfileCategoriesPanel: function(switcherUI) {
            jq("#socialProfileCategoriesPanel a.dropDownItem").unbind('click').click(function() {
                ASC.CRM.ContactActionView.changeSocialProfileCategory(switcherUI, jq(this).attr("category") * 1, jq(this).text(), jq(this).attr("categoryName"));
            });
        },

        changeAddressCategory: function(obj) {
            _changeAddressCategory(obj, jq(obj).find('option:selected').attr("category"));
        },

        showAssignedContactPanel: function() {
            jq('#assignedContactsListEdit .assignedContacts').removeClass('hiddenFields');
            if (jq('#assignedContactsListEdit .headerCollapse').length == 1) {
                jq('#assignedContactsListEdit .headerCollapse span').click();
            }
        },

        toggleAssignedContactsListEdit: function(switcher) {
            var $headerElem = jq(switcher).parent();
            if ($headerElem.hasClass('headerExpand')) {
                $headerElem.next('dd:first').nextUntil().hide();
            }
            else {
                $headerElem.next('dd:first').nextUntil().show();
            }
            $headerElem.toggleClass('headerExpand');
            $headerElem.toggleClass('headerCollapse');

        },

        gotoAddCustomFieldPage: function() {
            if (confirm(CRMJSResources.ConfirmGoToCustomFieldPage)) {
                if (jq("#typeAddedContact").val() === "company")
                    document.location = "settings.aspx?type=custom_field&view=company";
                else if (jq("#typeAddedContact").val() === "people")
                    document.location = "settings.aspx?type=custom_field&view=person";
                else
                    document.location = "settings.aspx?type=custom_field";
            }
        }
    }

})(jQuery);




/*
* --------------------------------------------------------------------
* jQuery-Plugin - sliderWithSections
* --------------------------------------------------------------------
*/

jQuery.fn.sliderWithSections = function(settings) {
    //accessible slider options
    var options = jQuery.extend({
        value: null,
        colors: null,
        values: null,
        defaultColor: '#E1E1E1',
        liBorderWidth: 1,
        sliderOptions: null,
        max: 0,
        marginWidth: 1,
        slide: function(e, ui) { }
    }, settings);


    //plugin-generated slider options (can be overridden)
    var sliderOptions = {
        step: 1,
        min: 0,
        orientation: 'horizontal',
        max: options.max,
        range: false, //multiple select elements = true
        slide: function(e, ui) {//slide function

            var thisHandle = jQuery(ui.handle);

            thisHandle.attr('aria-valuetext', options.values[ui.value]).attr('aria-valuenow', ui.value);

            if (ui.value != 0) {
                thisHandle.find('.ui-slider-tooltip .ttContent').html(options.values[ui.value]);
                thisHandle.removeClass("ui-slider-tooltip-hide");
            }
            else {
                thisHandle.addClass("ui-slider-tooltip-hide");
            }


            var liItems = jQuery(this).children('ol.ui-slider-scale').children('li');

            for (var i = 0; i < sliderOptions.max; i++) {
                if (i < ui.value) {
                    var color = options.colors != null && options.colors[i] ? options.colors[i] : 'transparent';
                    jQuery(liItems[i]).css('background-color', color);
                }
                else {
                    jQuery(liItems[i]).css('background-color', options.defaultColor);
                }
            }

            options.slide(e, ui);

        },

        value: options.value
    };

    //slider options from settings
    options.sliderOptions = (settings) ? jQuery.extend(sliderOptions, settings.sliderOptions) : sliderOptions;


    //create slider component div
    var sliderComponent = jQuery('<div></div>');

    var $tooltip = jQuery('<a href="#" tabindex="0" ' +
            'class="ui-slider-handle" ' +
            'role="slider" ' +
            'aria-valuenow="' + options.value + '" ' +
            'aria-valuetext="' + options.values[options.value] + '"' +
        '><span class="ui-slider-tooltip ui-widget-content ui-corner-all"><span class="ttContent"></span>' +
            '<span class="ui-tooltip-pointer-down ui-widget-content"><span class="ui-tooltip-pointer-down-inner"></span></span>' +
        '</span></a>')
        .data('handleNum', options.value)
        .appendTo(sliderComponent);
    sliderComponent.find('.ui-slider-tooltip .ttContent').html(options.values[options.value]);
    if (options.values[options.value] == "") {
        sliderComponent.children(".ui-slider-handle").addClass("ui-slider-tooltip-hide");
    }

    var scale = sliderComponent.append('<ol class="ui-slider-scale ui-helper-reset" role="presentation" style="width: 100%; height: 100%;"></ol>').find('.ui-slider-scale:eq(0)');

    //var widthVal = (1 / sliderOptions.max * 100).toFixed(2) + '%';
    var sliderWidth = jQuery(this).css('width').replace('px', '') * 1;
    var widthVal = ((sliderWidth - options.marginWidth * (sliderOptions.max - 1) - 2 * options.liBorderWidth * sliderOptions.max) / sliderOptions.max).toFixed(4);
    for (var i = 0; i <= sliderOptions.max; i++) {
        var style = (i == sliderOptions.max || i == 0) ? 'display: none;' : '';
        var liStyle = (i == sliderOptions.max) ? 'display: none;' : '';
        var color = 'transparent';

        if (i < options.value) {
            color = options.colors != null && options.colors[i] ? options.colors[i] : 'transparent';
        }
        else {
            color = options.defaultColor;
        }

        scale.append('<li style="left:' + leftVal(i, sliderWidth) + '; background-color:' + color + '; height: 100%; width:' + widthVal + 'px;' + liStyle + '"></li>');
    };

    function leftVal(i, sliderWidth) {
        var widthVal = ((sliderWidth - options.marginWidth * (sliderOptions.max - 1) - 2 * options.liBorderWidth * sliderOptions.max) / sliderOptions.max);
        return ((widthVal + 2 * options.liBorderWidth + options.marginWidth) * i).toFixed(4) + 'px';

    }
    //inject and return
    sliderComponent.appendTo(jQuery(this)).slider(options.sliderOptions).attr('role', 'application');
    sliderComponent.find('.ui-tooltip-pointer-down-inner').each(function() {
        var bWidth = jQuery('.ui-tooltip-pointer-down-inner').css('borderTopWidth');
        var bColor = jQuery(this).parents('.ui-slider-tooltip').css('backgroundColor')
        jQuery(this).css('border-top', bWidth + ' solid ' + bColor);
    });

    return this;
}

jq(document).ready(function() {

    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintTypes",
        dropdownID: "files_hintTypesPanel",
        fixWinSize: false
    });

    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintCsv",
        dropdownID: "files_hintCsvPanel",
        fixWinSize: false
    });
});