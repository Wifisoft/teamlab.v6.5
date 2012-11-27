window.ASC.Files.ThirdParty = (function() {
    var isInit = false;
    var thirdPartyList = {
        BoxNet: { key: "BoxNet", customerTitle: "BoxNet", typeTitle: "BoxNet" },
        DropBox: { key: "DropBox", customerTitle: "DropBox", typeTitle: "DropBox" },
        Google: { key: "Google", customerTitle: "Google", typeTitle: "Google" }
    };

    var init = function() {
        if (isInit === false) {
            isInit = true;

            serviceManager.bind(ASC.Files.TemplateManager.events.SaveThirdParty, onSaveThirdParty);
            serviceManager.bind(ASC.Files.TemplateManager.events.DeleteThirdParty, onDeleteThirdParty);

            jq.dropdownToggle({
                switcherSelector: "#files_addThirdParty_btn, #topAddThirdParty img",
                dropdownID: "addThirdPartyPanel",
                anchorSelector: "#topAddThirdParty img"
            });
        }
    };

    var isThirdParty = function(entryObject) {
        if (entryObject == null)
            return ASC.Files.ThirdParty.getThirdPartyType() != null;

        entryObject = jq(entryObject);
        if (!entryObject.is("li.fileRow"))
            entryObject = entryObject.closest("li.fileRow");
        return entryObject.hasClass("thirdPartyEntry");
    };

    var getThirdPartyType = function(entryObject) {
        var entryData = ASC.Files.UI.getObjectData(entryObject);

        var thirdPartyType = entryData
            ? entryData.provider_name
            : jq("#currentProviderType").val();

        switch (thirdPartyType) {
        case thirdPartyList.BoxNet.key:
            return thirdPartyList.BoxNet;
        case thirdPartyList.DropBox.key:
            return thirdPartyList.DropBox;
        case thirdPartyList.Google.key:
            return thirdPartyList.Google;
        default:
            return null;
        }
    };

    var setToken = function(token, source) {
        jq("#thirdPartyGetToken span").show();

        jq("#thirdPartyGetToken a").hide();
        jq("#thirdPartyGetToken input:hidden").val(token);
    };

    var showBoxNetEditor = function() {
        showEditorDialog(thirdPartyList.BoxNet);
    };

    var showDropBoxEditor = function() {
        showEditorDialog(thirdPartyList.DropBox);
    };

    var showGoogleEditor = function() {
        showEditorDialog(thirdPartyList.Google);
    };

    var showEditorDialog = function(thirdParty) {
        jq("#ThirdPartyEditor div[id$='InfoPanel']").hide();

        jq("#thirdPartyTitle").val(thirdParty.customerTitle);
        ASC.Files.UI.checkCharacter(jq("#thirdPartyTitle"));

        jq("#thirdPartyPanel input").removeAttr("disabled");

        jq("#thirdPartyPass").val("");
        jq("#thirdPartyCorporate").removeAttr("checked");

        jq("#thirdPartyGetToken input:hidden").val("");

        if (thirdParty.getTokenUrl) {
            jq("#thirdPartyNamePass").hide();
            jq("#thirdPartyGetToken").show();

            jq("#thirdPartyGetToken span").hide();

            jq("#thirdPartyGetToken a")
                .show()
                .text(ASC.Files.FilesJSResources.ThirdPartyGetToken.format(thirdParty.typeTitle))
                .unbind("click")
                .click(function() {
                    OAuthCallback = ASC.Files.ThirdParty.setToken;
                    OAuthPopup(thirdParty.getTokenUrl);
                    return false;
                });
        } else {
            jq("#thirdPartyNamePass").show();
            jq("#thirdPartyGetToken").hide();
        }

        jq("#ThirdPartyEditor .action-block").show();
        jq("#ThirdPartyEditor .ajax-info-block").hide();

        jq("#files_submitThirdParty").unbind("click").click(function() {
            ASC.Files.ThirdParty.submitEditor(thirdParty);
            return false;
        });

        if (thirdParty.id) {
            jq("#thirdPartyName").val(thirdParty.userName);
            jq("#thirdPartyCorporate").attr("checked", ASC.Files.Folders.folderContainer == "corporate");

            jq("#thirdPartyDialogCaption").text(ASC.Files.FilesJSResources.ThirdPartyEditorCaption.format(thirdParty.typeTitle));
        } else {
            jq("#thirdPartyDialogCaption").text(ASC.Files.FilesJSResources.ThirdPartyEditorCaptionAppend.format(thirdParty.typeTitle));
        }

        ASC.Files.Common.blockUI(jq("#ThirdPartyEditor"), 400, 300);
        PopupKeyUpActionProvider.EnterAction = 'jq("#files_submitThirdParty").click();';
        PopupKeyUpActionProvider.CloseDialogAction = 'jq("#thirdPartyPass").val("");';

        //remove all third party key's
        jq("#thirdPartyDivTitle").removeClass(
            thirdPartyList.BoxNet.key
                + " " + thirdPartyList.DropBox.key
                    + " " + thirdPartyList.Google.key);

        jq("#thirdPartyDivTitle").addClass(thirdParty.key);

        jq("#thirdPartyTitle").focus();
    };

    var submitEditor = function(thirdParty) {
        var folderTitle = jq("#thirdPartyTitle").val().trim();
        folderTitle = ASC.Files.Common.replaceSpecCharacter(folderTitle);
        var login = jq("#thirdPartyName").val().trim();
        var password = jq("#thirdPartyPass").val().trim();
        var token = jq("#thirdPartyGetToken input:hidden").val();
        var corporate = (jq("#thirdPartyCorporate").is(":checked") === true);

        var infoBlock = jq("#ThirdPartyEditor div[id$='InfoPanel']");
        infoBlock.hide();

        if (thirdParty.getTokenUrl) {
            if (folderTitle == "" || token == "") {
                infoBlock.show().find("div").text(ASC.Files.FilesJSResources.ErrorMassage_FieldsIsEmpty);
                return;
            }
        } else {
            if (folderTitle == "" || login == "" || password == "") {
                infoBlock.show().find("div").text(ASC.Files.FilesJSResources.ErrorMassage_FieldsIsEmpty);
                return;
            }
        }

        jq("#thirdPartyPass").val("");

        jq("#thirdPartyPanel input").attr("disabled", "disabled");
        jq("#ThirdPartyEditor .action-block").hide();
        jq("#ThirdPartyEditor .ajax-info-block").show();

        if (thirdParty.folderId) {
            ASC.Files.UI.blockObjectById("folder", thirdParty.folderId, true, ASC.Files.FilesJSResources.DescriptChangeInfo);
        }

        saveProvider(thirdParty.id, thirdParty.key, folderTitle, login, password, token, corporate, thirdParty.folderId);
    };

    var showChangeDialog = function(folderObject, folderId, title, folderData) {
        var thirdPartyTmp = ASC.Files.ThirdParty.getThirdPartyType(folderObject);
        var thirdParty =
            {
                getTokenUrl: thirdPartyTmp.getTokenUrl,
                key: thirdPartyTmp.key,
                typeTitle: thirdPartyTmp.typeTitle,

                id: folderData.provider_id,
                userName: folderData.provider_username,
                customerTitle: title,
                folderId: folderId
            };

        showEditorDialog(thirdParty);
    };

    var showDeleteDialog = function(folderObject, folderId, title) {
        var thirdPartyType = ASC.Files.ThirdParty.getThirdPartyType(folderObject);
        var customerTitle = title;

        var typeTitle = thirdPartyType.typeTitle;
        jq("#thirdPartyDeleteDescr").html("<b>" + ASC.Files.FilesJSResources.ConfirmDeleteThirdParty.format(customerTitle, typeTitle) + "</b>");

        jq("#files_deleteThirdParty").unbind("click").click(function() {
            ASC.Files.UI.blockObject(folderObject, true, ASC.Files.FilesJSResources.DescriptRemove);
            PopupKeyUpActionProvider.CloseDialog();
            ASC.Files.ThirdParty.deleteProvider(folderId, customerTitle);
            return false;
        });

        ASC.Files.Common.blockUI(jq("#ThirdPartyDelete"), 400, 300);
        PopupKeyUpActionProvider.EnterAction = 'jq("#files_showDeleteDialog").click();';
    };

    //request

    var saveProvider = function(providerId, providerKey, folderTitle, login, password, token, corporate, folderId) {
        var params =
            {
                providerId: providerId,
                providerKey: providerKey,
                folderTitle: folderTitle,
                login: login,
                password: password,
                token: token,
                corporate: corporate,
                folderId: folderId
            };

        var data = {
            third_party: {
                auth_data:
                    {
                        login: login,
                        password: password,
                        token: token
                    },
                corporate: corporate,
                customer_title: folderTitle,
                provider_id: providerId,
                provider_name: providerKey
            }
        };

        serviceManager.request("post",
            "xml",
            ASC.Files.TemplateManager.events.SaveThirdParty,
            params,
            data,
            "thirdparty", "save");
    };

    var deleteProvider = function(folderId, customerTitle) {
        var params =
            {
                folderId: folderId,
                customerTitle: customerTitle,
                parentId: ASC.Files.Folders.currentFolderId
            };

        serviceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.DeleteThirdParty,
            params,
            "thirdparty", "delete?folderId=" + encodeURIComponent(folderId));
    };

    //event handler
    var onSaveThirdParty = function(xmlData, params, errorMessage, commentMessage) {
        var infoBlock = jq("#ThirdPartyEditor div[id$='InfoPanel']");

        jq("#thirdPartyPass").val("");

        if (typeof errorMessage != "undefined") {
            infoBlock.show().find("div").text(commentMessage || errorMessage);
            jq("#thirdPartyPanel input").removeAttr("disabled");
            jq("#ThirdPartyEditor .action-block").show();
            jq("#ThirdPartyEditor .ajax-info-block").hide();
            return undefined;
        }

        PopupKeyUpActionProvider.CloseDialog();

        var folderPlaceId = params.corporate ? ASC.Files.Constants.FOLDER_ID_COMMON_FILES : ASC.Files.Constants.FOLDER_ID_MY_FILES;
        var folderTitle = params.folderTitle;

        var folderObj;
        var folderId = params.folderId;
        var countSubFolder = 0;
        var countSubFile = 0;

        if (folderPlaceId == ASC.Files.Folders.currentFolderId) {
            var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderItem);
            var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

            ASC.Files.EmptyScreen.hideEmptyScreen();

            folderObj = ASC.Files.UI.getEntryObject("folder", folderId);
            if (folderObj) {
                countSubFolder -= 1 + (parseInt(folderObj.find(".countFolders").html()) || 0);
                countSubFile -= parseInt(folderObj.find(".countFiles").html()) || 0;

                folderObj.replaceWith(htmlXML);
                folderObj = ASC.Files.UI.getEntryObject("folder", folderId);
            } else {
                jq("#files_mainContent").prepend(htmlXML);
                folderObj = jq("#files_mainContent li.newFolder:first");
            }

            folderObj.yellowFade().removeClass("newFolder");

            ASC.Files.UI.resetSelectAll();

            folderTitle = ASC.Files.UI.getObjectTitle(folderObj);

            countSubFolder += 1 + (parseInt(folderObj.find(".countFolders").html()) || 0);
            countSubFile += parseInt(folderObj.find(".countFiles").html()) || 0;

        } else {
            if (folderId) {
                folderObj = ASC.Files.UI.getEntryObject("folder", folderId);

                countSubFolder = -1 - (parseInt(folderObj.find(".countFolders").html()) || 0);
                countSubFile = -parseInt(folderObj.find(".countFiles").html()) || 0;

                folderObj.remove();
            }
        }

        if (folderObj && folderObj.is(":visible")) {
            ASC.Files.UI.addRowHandlers(folderObj);
        } else {
            ASC.Files.UI.paintRows();
        }

        ASC.Files.UI.updateFolderInfo(countSubFolder, countSubFile);

        ASC.Files.Tree.resetNode(folderPlaceId);

        ASC.Files.UI.displayInfoPanel(
            params.folderId
                ? ASC.Files.FilesJSResources.InfoChangeThirdParty.format(folderTitle)
                : ASC.Files.FilesJSResources.InfoSaveThirdParty.format(folderTitle, params.corporate ? ASC.Files.FilesJSResources.CorporateFiles : ASC.Files.FilesJSResources.MyFiles));
    };

    var onDeleteThirdParty = function(jsonData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var parentId = params.parentId;
        var folderId = params.folderId;
        var folderTitle = params.customerTitle;
        var folderObj = ASC.Files.UI.getEntryObject("folder", folderId);

        if (folderObj != null) {
            var countSubFolder = 1 + (parseInt(folderObj.find(".countFolders").html()) || 0);
            var countSubFile = parseInt(folderObj.find(".countFiles").html()) || 0;

            folderObj.remove();

            ASC.Files.UI.updateFolderInfo(-countSubFolder, -countSubFile);

            ASC.Files.UI.paintRows();

            var countAppend = ASC.Files.Constants.COUNT_ON_PAGE - jq("#files_mainContent li.fileRow").length;
            if (countAppend > 0) {
                if (ASC.Files.UI.currentPage < ASC.Files.UI.amountPage)
                    ASC.Files.Folders.getFolderItems(true, countAppend);
                else {
                    if (countAppend >= ASC.Files.Constants.COUNT_ON_PAGE) {
                        ASC.Files.EmptyScreen.displayEmptyScreen();
                    }
                }
            }
        }

        ASC.Files.Tree.resetNode(parentId);

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRemoveThirdParty.format(folderTitle));
    };

    return {
        init: init,
        thirdPartyList: thirdPartyList,

        isThirdParty: isThirdParty,
        getThirdPartyType: getThirdPartyType,

        setToken: setToken,

        showBoxNetEditor: showBoxNetEditor,
        showDropBoxEditor: showDropBoxEditor,
        showGoogleEditor: showGoogleEditor,

        showChangeDialog: showChangeDialog,
        showDeleteDialog: showDeleteDialog,

        submitEditor: submitEditor,
        deleteProvider: deleteProvider
    };
})();

(function($) {
    ASC.Files.ThirdParty.init();

    $(function() {
        jq("#ThirdPartyEditor div[id$='InfoPanel']")
            .removeClass("infoPanel")
            .addClass("errorBox")
            .css("margin", "10px 16px 0");

        ASC.Files.ThirdParty.thirdPartyList.BoxNet.customerTitle = ASC.Files.FilesJSResources.FolderTitleBoxNet;
        ASC.Files.ThirdParty.thirdPartyList.BoxNet.typeTitle = ASC.Files.FilesJSResources.TypeTitleBoxNet;

        ASC.Files.ThirdParty.thirdPartyList.DropBox.customerTitle = ASC.Files.FilesJSResources.FolderTitleDropBox;
        ASC.Files.ThirdParty.thirdPartyList.DropBox.typeTitle = ASC.Files.FilesJSResources.TypeTitleDropBox;
        ASC.Files.ThirdParty.thirdPartyList.DropBox.getTokenUrl = ASC.Files.Constants.URL_OAUTH_DROPBOX;

        ASC.Files.ThirdParty.thirdPartyList.Google.customerTitle = ASC.Files.FilesJSResources.FolderTitleGoogle;
        ASC.Files.ThirdParty.thirdPartyList.Google.typeTitle = ASC.Files.FilesJSResources.TypeTitleGoogle;
        ASC.Files.ThirdParty.thirdPartyList.Google.getTokenUrl = ASC.Files.Constants.URL_OAUTH_GOOGLE;

        jq("#add_boxnet").click(function() {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showBoxNetEditor();
            return false;
        });

        jq("#add_dropbox").click(function() {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showDropBoxEditor();
            return false;
        });

        jq("#add_google").click(function() {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showGoogleEditor();
            return false;
        });

    });
})(jQuery);