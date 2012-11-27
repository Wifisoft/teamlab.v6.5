window.ASC.Files.Share = (function() {
    var isInit = false;
    var objectID;
    var objectTitle;
    var sharingInfo = [];
    var linkInfo;
    var linkInfoNew;
    var sharingManager = null;
    var ShareLinkCode;
    var shortenLink = { r: "", rw: "" };

    var clip = null;

    var init = function() {
        if (isInit === false) {
            isInit = true;

            serviceManager.bind(ASC.Files.TemplateManager.events.GetSharedInfo, onGetSharedInfo);
            serviceManager.bind(ASC.Files.TemplateManager.events.SetAceObject, onSetAceObject);
            serviceManager.bind(ASC.Files.TemplateManager.events.UnSubscribeMe, onUnSubscribeMe);
            serviceManager.bind(ASC.Files.TemplateManager.events.GetShortenLink, onGetShortenLink);

            serviceManager.bind(ASC.Files.TemplateManager.events.MarkAsRead, onMarkAsRead);

            sharingManager = new SharingSettingsManager(undefined, null);
            sharingManager.OnSave = setAceObject;

        }
    };

    var getAceString = function(aceStatus) {
        if (aceStatus == "owner") return ASC.Files.FilesJSResources.AceStatusEnum_Owner;
        aceStatus = parseInt(aceStatus);
        switch (aceStatus) {
            case ASC.Files.Constants.AceStatusEnum.Read:
                return ASC.Files.FilesJSResources.AceStatusEnum_Read;
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                return ASC.Files.FilesJSResources.AceStatusEnum_ReadWrite;
            case ASC.Files.Constants.AceStatusEnum.Restrict:
                return ASC.Files.FilesJSResources.AceStatusEnum_Restrict;
            default:
                return "";
        }
    };

    var updateClip = function() {
        if (ASC.Files.Share.clip)
            ASC.Files.Share.clip.destroy();

        if (jq("#shareLink_copy:visible").length == 0) return;

        var url = jq("#shareLink").val();
        ASC.Files.Share.clip = new ZeroClipboard.Client();
        ASC.Files.Share.clip.setText(url);
        ASC.Files.Share.clip.glue("shareLink_copy", "shareLink_copy",
            {
                zIndex: 670,
                left: jq("#shareLink_copy").offset().left - jq("#studio_sharingSettingsDialog").offset().left + "px",
                top: jq("#shareLink_copy").offset().top - jq("#studio_sharingSettingsDialog").offset().top + "px"
            });
        ASC.Files.Share.clip.addEventListener("onComplete", function(client, text) { jq("#shareLink, #shareLink_copy").yellowFade(); });
    };

    var hideShareLink = function() {
        jq("#shareLink, #getShortenLink").hide();
        jq("#studio_sharingSettingsDialog").removeClass("showLink");
        if (jq.browser.mobile)
            return;
        jq("#switchDisplayLink").unbind("click").click(showShareLink)
            .text(ASC.Files.FilesJSResources.ShowShareLink);
    };

    var showShareLink = function() {
        jq("#shareLink").show();

        jq("#getShortenLink").hide();
        var ace = parseInt(jq("#shareLinkSelect").val());
        switch (ace) {
            case ASC.Files.Constants.AceStatusEnum.Read:
                if (shortenLink.r == "")
                    jq("#getShortenLink").show();
                break;
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                if (shortenLink.rw == "")
                    jq("#getShortenLink").show();
                break;
        }

        jq("#studio_sharingSettingsDialog").addClass("showLink");
        if (jq.browser.mobile)
            return;
        jq("#switchDisplayLink").unbind("click").click(hideShareLink)
            .text(ASC.Files.FilesJSResources.HideShareLink);
    };

    var hideShareMessage = function() {
        jq("#shareRemoveMessage, #shareMessage").hide();
        jq("#shareAddMessage").show();
    };

    var showShareMessage = function() {
        jq("#shareAddMessage").hide();
        jq("#shareRemoveMessage, #shareMessage").show();
        jq("#shareMessageSend").attr("checked", "checked");
    };

    var changeShareLinkAce = function() {
        var fileId = ASC.Files.UI.parseItemId(objectID).entryId;
        var url = ASC.Files.Common.getSitePath() + ASC.Files.Utility.GetFileViewUrl(fileId) + "&" + encodeURI(ShareLinkCode);

        if (ASC.Files.Utility.CanWebView(objectTitle))
            url = ASC.Files.Common.getSitePath() + ASC.Files.Constants.ShareLinkRead + "?" + encodeURI(ShareLinkCode);

        var ace = parseInt(jq("#shareLinkSelect").val());
        switch (ace) {
            case ASC.Files.Constants.AceStatusEnum.Restrict:
                url = "";
                hideShareLink();
                jq("#shareLinkPanel").hide();
                break;
            case ASC.Files.Constants.AceStatusEnum.Read:
                if (shortenLink.r != "")
                    url = shortenLink.r;
                if (jq("#shareLink:visible").length != 0 || jq.browser.mobile)
                    showShareLink();
                jq("#shareLinkPanel").show();
                break;
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                if (ASC.Files.Utility.CanWebEdit(objectTitle, true))
                    url = ASC.Files.Common.getSitePath() + ASC.Files.Constants.ShareLinkReadWrite + "?" + encodeURI(ShareLinkCode);
                if (shortenLink.rw != "")
                    url = shortenLink.rw;
                if (jq("#shareLink:visible").length != 0 || jq.browser.mobile)
                    showShareLink();
                jq("#shareLinkPanel").show();
                break;
        }

        jq("#shareLink").val(url);

        updateClip();
        linkInfoNew = ace;
    };

    var displayCountNew = function(value, folderId) {
        value = parseInt(value) || 0;

        var countNew = ASC.Files.FilesJSResources.NewInShare.format(value);

        jq("#files_breadCrumbsNew").text("").hide();
        jq(".new_inshare[data-id='" + folderId + "']").text("").hide();
        //TODO: sum in all folderId
        jq("#topNavigationNewInShare").text("").hide();

        if (value > 0) {
            jq("#files_breadCrumbsNew").text(countNew).show();
            jq(".new_inshare[data-id='" + folderId + "']").text(countNew).show();

            //TODO: sum in all folderId
            jq("#topNavigationNewInShare").text(value).show();

        }
    };

    var removeNewIcon = function(entryType, entryId) {
        var isNew = ASC.Files.UI.getEntryObject(entryType, entryId).find("div.is_new");
        if (isNew.length == 0) return;

        isNew.remove();
        var spanCountNew = jq(".new_inshare[data-id='" + ASC.Files.Folders.currentFolderId + "']");
        var countNew = parseInt(spanCountNew.text().replace(/\D/g, "")) - 1;
        if (countNew != NaN)
            ASC.Files.Share.displayCountNew(countNew, ASC.Files.Folders.currentFolderId);
    };

    var renderGetLink = function(linkData) {
        jq("#studio_sharingSettingsDialog").addClass("withLink");

        if (jq("#studio_sharingSettingsDialog #shareLinkItem").length == 0) {
            jq("#shareLinkItem").prependTo("#studio_sharingSettingsDialog .containerBodyBlock");

            if (jq.browser.mobile) {
                jq("#shareLink_copy, #switchDisplayLink").remove();
                jq("#shareLink").attr("readonly", "false").attr("readonly", "").removeAttr("readonly");
            } else {
                jq("#shareLinkPanel").on("mousedown", "#shareLink", function() {
                    jq(this).select();
                    return false;
                });
            }
            jq("#shareLinkPanel").on("keypress", "#shareLink", function() { return false; });
            jq("#shareLinkPanel").on("click", "#getShortenLink", getShortenLink);
            jq("#sharingSettingsItems").on("scroll", updateClip);
        }

        jq("#shareLinkItem .sharingItem").remove();
        jq("#shareLinkPanel").before(jq("#sharingListTemplate").tmpl(linkData));

        if (!jq.browser.mobile) {
            hideShareLink();

            updateClip();
        }

        var rowShareLink = jq("#sharing_item_" + ASC.Files.Constants.ShareLinkId);

        rowShareLink.removeClass("tintMedium borderBase");

        rowShareLink.find("select").attr("id", "shareLinkSelect").bind("change", changeShareLinkAce);
        jq("#shareLinkSelect").tlcombobox();

        //huck
        if (!ASC.Files.Utility.CanWebEdit(objectTitle, true)) {
            rowShareLink.find("select option[value='" + ASC.Files.Constants.AceStatusEnum.ReadWrite + "']").remove();
            rowShareLink.find("ul li[data-value='" + ASC.Files.Constants.AceStatusEnum.ReadWrite + "']").remove();
        }

        shortenLink = { r: "", rw: "" };

        changeShareLinkAce();
    };

    var renderShareMessage = function() {
        if (jq("#studio_sharingSettingsDialog #shareMessagePanel").length == 0) {
            jq("#shareMessagePanel").insertAfter("#studio_sharingSettingsDialog .addToSharingLinks");

            jq("#shareAddMessage").click(function() {
                showShareMessage();
                return false;
            });

            jq("#shareRemoveMessage").click(function() {
                hideShareMessage();
                return false;
            });

            jq("#shareMessageSend").bind("change", function() {
                if (!jq("#shareMessageSend").is(":checked"))
                    hideShareMessage();
            });
        }

        hideShareMessage();
    };

    var unSubscribeMe = function(entryType, entryId) {
        if (ASC.Files.Folders.folderContainer != "forme")
            return;
        var list = new Array();

        var textFolder = "";
        var textFile = "";
        var strHtml = "<label title='{0}'><input type='checkbox' entryType='{1}' entryId='{2}' checked='checked'>{0}</label>";

        if (entryType && entryId) {
            list.push({ entryType: entryType, entryId: entryId });

            var entryRowTitle = ASC.Files.UI.getEntryTitle(entryType, entryId);

            if (entryType == "file")
                textFile += strHtml.format(entryRowTitle, entryType, entryId);
            else
                textFolder += strHtml.format(entryRowTitle, entryType, entryId);
        } else {
            jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):has(div.checkbox input:checked)").each(function() {
                var entryRowData = ASC.Files.UI.getObjectData(this);
                var entryRowType = entryRowData.entryType;
                var entryRowId = entryRowData.entryId;

                list.push({ entryType: entryRowType, entryId: entryRowId });

                var entryRowTitle = entryRowData.title;
                if (entryRowType == "file")
                    textFile += strHtml.format(entryRowTitle, entryRowType, entryRowId);
                else
                    textFolder += strHtml.format(entryRowTitle, entryRowType, entryRowId);
            });
        }

        if (list.length == 0)
            return;

        jq("#confirmUnsubscribeList dd.confirmRemoveFiles").html(textFile);
        jq("#confirmUnsubscribeList dd.confirmRemoveFolders").html(textFolder);

        jq("#confirmUnsubscribeList .confirmRemoveFolders, #confirmUnsubscribeList .confirmRemoveFiles").show();
        if (textFolder == "")
            jq("#confirmUnsubscribeList .confirmRemoveFolders").hide();
        if (textFile == "")
            jq("#confirmUnsubscribeList .confirmRemoveFiles").hide();


        ASC.Files.Common.blockUI(jq("#files_confirm_unsubscribe"), 420, 0, -150);

        PopupKeyUpActionProvider.EnterAction = 'jq("#unsubscribeConfirmBtn").click();';
    };

    //request

    var getSharedInfo = function(entryType, entryId, objTitle) {
        if (ASC.Files.Folders) {
            if (ASC.Files.Folders.folderContainer != "my"
                && (ASC.Files.Folders.folderContainer != "corporate" || !ASC.Files.Constants.USER_ADMIN))
                return;
        }

        objectID = entryType + "_" + entryId;
        objectTitle = objTitle;

        serviceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.GetSharedInfo,
            { showLoading: true },
            "sharedinfo?objectId=" + encodeURIComponent(objectID));
    };

    var setAceObject = function(data) {
        var dataItems = data.items;

        var ace_wrapperList = "";

        for (var i = 0; i < dataItems.length; i++) {
            var dataItem = dataItems[i];
            var change = true;
            for (var j = 0; j < sharingInfo.length && change; j++) {
                var dataItemOld = sharingInfo[j];
                if (dataItemOld.id === dataItem.id) {
                    if (dataItemOld.selectedAction.id == dataItem.selectedAction.id) {
                        change = false;
                    } else {
                        break;
                    }
                }
            }

            if (change) {
                var json =
                    {
                        id: dataItem.id,
                        is_group: dataItem.isGroup,
                        ace_status: dataItem.selectedAction.id
                    };
                ace_wrapperList += serviceManager.jsonToXml({ entry: json });
            }
        }

        //remove
        for (var j = 0; j < sharingInfo.length; j++) {
            var dataItemOld = sharingInfo[j];
            var change = true;
            for (var i = 0; i < dataItems.length; i++) {
                var dataItem = dataItems[i];
                if (dataItemOld.id === dataItem.id) {
                    change = false;
                    break;
                }
            }
            if (change) {
                var json =
                    {
                        id: dataItemOld.id,
                        is_group: dataItemOld.isGroup,
                        ace_status: ASC.Files.Constants.AceStatusEnum.None
                    };
                ace_wrapperList += serviceManager.jsonToXml({ entry: json });
            }
        }

        if (linkInfo != linkInfoNew) {
            var json =
                {
                    id: ASC.Files.Constants.ShareLinkId,
                    is_group: true,
                    ace_status: linkInfoNew
                };
            ace_wrapperList += serviceManager.jsonToXml({ entry: json });
        }

        if (ace_wrapperList == "") return;

        var dataXml = serviceManager.jsonToXml({ ace_wrapperList: ace_wrapperList });
        var notify = jq("#shareMessageSend").is(":checked") == true;
        var message = notify ? jq("#shareMessage:visible").val() || "" : "";

        serviceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.SetAceObject,
            { showLoading: true },
            dataXml,
            "setaceobject?objectId=" + encodeURIComponent(objectID)
                + "&notify=" + notify
                    + "&message=" + encodeURIComponent(message.trim()));

        if (ASC.Files.UI.documentTitleFix)
            ASC.Files.UI.documentTitleFix();
    };

    var confirmUnSubscribe = function() {
        if (jq("#files_confirm_unsubscribe:visible").length == 0)
            return;

        PopupKeyUpActionProvider.CloseDialog();

        var listChecked = jq("#confirmUnsubscribeList input:checked");
        if (listChecked.length == 0)
            return;

        var data = {};
        data.entry = new Array();

        var list = new Array();
        for (var i = 0; i < listChecked.length; i++) {
            var entryConfirmType = jq(listChecked[i]).attr("entryType");
            var entryConfirmId = jq(listChecked[i]).attr("entryId");
            var entryConfirmObj = ASC.Files.UI.getEntryObject(entryConfirmType, entryConfirmId);
            ASC.Files.UI.blockObject(entryConfirmObj, true, ASC.Files.FilesJSResources.DescriptRemove);
            data.entry.push(entryConfirmType + "_" + entryConfirmId);
            list.push({ entryId: entryConfirmId, entryType: entryConfirmType });
        }

        serviceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.UnSubscribeMe,
            { showLoading: true, list: list },
            { stringList: data },
            "removeace");
    };

    var getShortenLink = function() {

        var fileId = ASC.Files.UI.parseItemId(objectID).entryId;
        var longUrl = jq("#shareLink").val();

        serviceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.GetShortenLink,
            {},
            "shorten?fileId=" + encodeURIComponent(fileId) + "&longUrl=" + encodeURIComponent(longUrl) + "&" + ShareLinkCode);

        jq("#getShortenLink").hide();
    };

    var markAsRead = function(event) {
        var itemData;

        var data = {};
        data.entry = new Array();

        if (typeof event == "undefined") {
            jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):has(div.checkbox input:checked)").each(function() {
                itemData = ASC.Files.UI.getObjectData(this);

                if (itemData.entryObject.find("div.is_new").length > 0) {
                    data.entry.push(itemData.entryType + "_" + itemData.entryId);
                }
            });
        } else {
            var e = ASC.Files.Common.fixEvent(event);

            itemData = ASC.Files.UI.getObjectData(e.srcElement || e.target);

            data.entry.push(itemData.entryType + "_" + itemData.entryId);
        }

        if (data.entry.length == 0)
            return false;

        serviceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.MarkAsRead,
            { list: data.entry },
            { stringList: data },
            "markasread");

        return false;
    };

    //event handler

    var onGetSharedInfo = function(jsonData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined" || typeof jsonData == "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        sharingInfo = jsonData;

        var translateItems = [];
        var linkItems = [];
        linkInfo = ASC.Files.Constants.AceStatusEnum.None;
        jq(jsonData).each(function(i) {
            var item = jsonData[i];
            if (item.id === ASC.Files.Constants.ShareLinkId) {
                ShareLinkCode = item.title;
                linkInfo = item.ace_status;
                linkItems.push(
                    {
                        "id": item.id,
                        "name": ASC.Files.FilesJSResources.ShareLinkGroupTitle,
                        "isGroup": item.is_group,
                        "canEdit": !(item.locked || item.owner),
                        "selectedAction": {
                            "id": item.ace_status,
                            "name": getAceString(item.owner ? "owner" : item.ace_status),
                            "defaultAction": false
                        }
                    });
            } else {

                translateItems.push(
                    {
                        "id": item.id,
                        "name": item.title,
                        "isGroup": item.is_group,
                        "canEdit": !(item.locked || item.owner),
                        "selectedAction": {
                            "id": item.ace_status,
                            "name": getAceString(item.owner ? "owner" : item.ace_status),
                            "defaultAction": false
                        }
                    });
            }
        });
        var Actions = [
            {
                "id": ASC.Files.Constants.AceStatusEnum.Read,
                "name": getAceString(ASC.Files.Constants.AceStatusEnum.Read),
                "defaultAction": true
            },
            {
                "id": ASC.Files.Constants.AceStatusEnum.ReadWrite,
                "name": getAceString(ASC.Files.Constants.AceStatusEnum.ReadWrite),
                "defaultAction": false
            },
            {
                "id": ASC.Files.Constants.AceStatusEnum.Restrict,
                "name": getAceString(ASC.Files.Constants.AceStatusEnum.Restrict),
                "defaultAction": false
            }
        ];

        var translateData = {
            "actions": Actions,
            "items": translateItems
        };

        sharingInfo = translateItems;
        linkInfoNew = linkInfo;

        sharingManager.UpdateSharingData(translateData);
        sharingManager.ShowDialog();
        jq("body").css("overflow", "");

        jq("#studio_sharingSettingsDialog").removeClass("withLink showLink");

        if (linkItems.length != 0) {
            var linkData = {
                "actions": Actions,
                "items": linkItems
            };

            renderGetLink(linkData);
        }

        renderShareMessage();
    };

    var onSetAceObject = function(jsonData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var itemId = ASC.Files.UI.parseItemId(objectID);
        var entryObj = ASC.Files.UI.getEntryObject(itemId.entryType, itemId.entryId);
        var shareActionObj = entryObj.find(".share_action");

        if (jsonData === true) {
            shareActionObj.addClass("shareBlue");
        } else {
            shareActionObj.removeClass("shareBlue");
        }

        objectID = null;
    };

    var onUnSubscribeMe = function(jsonData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var list = params.list;

        var foldersCount = 0;
        var countSubFolder = 0, countSubFile = 0;

        for (var i = 0; i < list.length; i++) {
            if (list[i].entryType == "file") {
                countSubFile++;
            } else {
                foldersCount++;

                var folderRowObj = ASC.Files.UI.getEntryObject("folder", list[i].entryId);

                countSubFolder += 1 + (parseInt(folderRowObj.find(".countFolders").html()) || 0);
                countSubFile += parseInt(folderRowObj.find(".countFiles").html()) || 0;
            }

            ASC.Files.UI.getEntryObject(list[i].entryType, list[i].entryId).remove();
        }

        if (foldersCount > 0) {
            ASC.Files.Tree.resetNode(ASC.Files.Folders.currentFolderId);
        }

        ASC.Files.UI.updateFolderInfo(-countSubFolder, -countSubFile);

        ASC.Files.UI.paintRows();

        var countAppend = ASC.Files.Constants.COUNT_ON_PAGE - jq("#files_mainContent div.checkbox input").length;
        if (countAppend > 0) {
            if (ASC.Files.UI.currentPage < ASC.Files.UI.amountPage)
                ASC.Files.Folders.getFolderItems(true, countAppend);
            else {
                if (countAppend >= ASC.Files.Constants.COUNT_ON_PAGE) {
                    ASC.Files.EmptyScreen.displayEmptyScreen();
                }
            }
        }

        ASC.Files.Share.displayCountNew(jsonData, ASC.Files.Folders.currentFolderId);
    };

    var onGetShortenLink = function(jsonData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        if (jsonData == null || jsonData == "") return;

        jq("#shareLink").val(jsonData);

        var ace = parseInt(jq("#shareLinkSelect").val());
        switch (ace) {
            case ASC.Files.Constants.AceStatusEnum.Read:
                shortenLink.r = jsonData;
                break;
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                shortenLink.rw = jsonData;
                break;
        }
        updateClip();
    };

    var onMarkAsRead = function(jsonData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        var itemIdList = params.list;
        for (var i = 0; i < itemIdList.length; i++) {
            var itemId = ASC.Files.UI.parseItemId(itemIdList[i]);
            var entryType = itemId.entryType;
            var entryId = itemId.entryId;

            ASC.Files.Share.removeNewIcon(entryType, entryId);
        }

        ASC.Files.Share.displayCountNew(jsonData, ASC.Files.Folders.currentFolderId);

        ASC.Files.Actions.showActionsViewPanel();
    };

    return {
        init: init,

        displayCountNew: displayCountNew,
        removeNewIcon: removeNewIcon,
        markAsRead: markAsRead,

        getSharedInfo: getSharedInfo,

        setAceObject: setAceObject,
        unSubscribeMe: unSubscribeMe,
        confirmUnSubscribe: confirmUnSubscribe,

        clip: clip
    };
})();

(function($) {
    ASC.Files.Share.init();
    $(function() {

        jq("#files_unsubscribeButton, #files_mainUnsubscribe").click(function() {
            if (jq(this).is("#files_mainUnsubscribe") && !jq(this).hasClass("unlockAction")) return;
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Share.unSubscribeMe();
        });

        jq("#files_mainMarkRead").click(function() {
            ASC.Files.Share.markAsRead();
        });

        jq("#unsubscribeConfirmBtn").click(function() {
            ASC.Files.Share.confirmUnSubscribe();
        });

        jq("#files_mainContent").on("click", "div.is_new", ASC.Files.Share.markAsRead);

        jq("#files_mainContent").on("click", "div.share_action", function() {
            var entryData = ASC.Files.UI.getObjectData(this);
            var entryId = entryData.entryId;
            var entryType = entryData.entryType;
            var entryTitle = entryData.title;

            ASC.Files.Share.getSharedInfo(entryType, entryId, entryTitle);
            return false;
        });
    });
})(jQuery);