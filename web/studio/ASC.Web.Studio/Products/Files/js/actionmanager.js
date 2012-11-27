window.ASC.Files.Actions = (function () {
    var isInit = false;

    var init = function () {
        if (isInit === false) {
            isInit = true;

            jq(document).click(function (event) {
                jq.dropdownToggle().registerAutoHide(event, "", "#files_actionsPanel");
                jq.dropdownToggle().registerAutoHide(event, "li.row-selected div.rowActions", "#files_actionPanel");
            });

            jq.dropdownToggle(
                {
                    switcherSelector: "#menuActionSelectOpen",
                    dropdownID: "files_selectorPanel",
                    anchorSelector: "li.menuActionSelectAll"
                });
        }
    };

    /* Methods*/

    var showActionsViewPanel = function (event) {
        jq("#files_unsubscribeButton, #files_deleteButton, #files_movetoButton, #files_copytoButton").hide();
        jq("#mainContentHeader .unlockAction").removeClass("unlockAction");
        var count = jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):has(div.checkbox input:checked)").length;
        var countWithRights = count;
        var countIsNew = 0;
        var tmpAccessable = ASC.Files.UI.accessAdmin();
        var onlyThirdParty = (ASC.Files.ThirdParty && !ASC.Files.ThirdParty.isThirdParty());
        var countThirdParty = 0;

        jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):has(div.checkbox input:checked)").each(function () {
            var entryObj = jq(this);

            if (ASC.Files.UI.editingFile(entryObj) || !ASC.Files.UI.accessAdmin(entryObj)) {
                countWithRights--;
            } else {
                tmpAccessable = ASC.Files.UI.accessAdmin(entryObj);
            }

            if (ASC.Files.Folders.folderContainer == "forme"
                && entryObj.find("div.is_new:visible").length > 0) {
                countIsNew++;
            }

            if (ASC.Files.ThirdParty) {
                if (!ASC.Files.ThirdParty.isThirdParty() && ASC.Files.ThirdParty.isThirdParty(entryObj)) {
                    countThirdParty++;
                } else {
                    onlyThirdParty = false;
                }
            } else {
                onlyThirdParty = false;
            }
        });

        if (count > 0) {
            jq("#files_downloadButton, #files_unsubscribeButton, #files_restoreButton, #files_copytoButton").show().find("span").html(count);
            jq("#files_mainDownload, #files_mainUnsubscribe, #files_mainRestore, #files_mainCopy").addClass("unlockAction");
            if (ASC.Files.Folders.folderContainer != "forme") {
                jq("#files_unsubscribeButton").hide();
            }
        }

        if (countIsNew > 0) {
            jq("#files_mainMarkRead").addClass("unlockAction");
        }

        if (countWithRights > 0) {
            jq("#files_deleteButton, #files_movetoButton").show().find("span").html(countWithRights - countThirdParty);
            jq("#files_mainDelete, #files_mainMove").addClass("unlockAction");
        }

        if (ASC.Files.Folders.folderContainer == "trash") {
            jq("#files_deleteButton, #files_movetoButton, #files_copytoButton").hide();
            jq("#files_mainDelete, #files_mainMove, #files_mainCopy").removeClass("unlockAction");
        }

        if (countWithRights == 1 && !tmpAccessable
            || countWithRights != 1 && !ASC.Files.UI.accessAdmin()) {
            jq("#files_deleteButton, #files_movetoButton").hide();
            jq("#files_mainDelete, #files_mainMove").removeClass("unlockAction");
        }

        jq("#files_restoreButton, #files_emptyTrashButton").hide();
        jq("#files_mainRestore, #files_mainEmptyTrash").removeClass("unlockAction");
        if (ASC.Files.Folders.folderContainer == "trash") {
            if (count > 0) {
                jq("#files_downloadButton, #files_deleteButton, #files_restoreButton").show();
                jq("#files_mainDownload, #files_mainDelete, #files_mainRestore").addClass("unlockAction");
            } else {
                jq("#files_downloadButton").hide();
                jq("#files_mainDownload").removeClass("unlockAction");
            }
            jq("#files_emptyTrashButton").show();
            jq("#files_mainEmptyTrash").addClass("unlockAction");
        }

        if (onlyThirdParty) {
            jq("#files_deleteButton, #files_movetoButton").hide();
            jq("#files_mainDelete, #files_mainMove").removeClass("unlockAction");
        }

        if (typeof event != "undefined") {
            var e = ASC.Files.Common.fixEvent(event);

            var dropdownItem = jq("#files_actionsPanel");
            dropdownItem.css({
                "position": "absolute",
                "top": e.pageY,
                "left": e.pageX
            });

            dropdownItem.toggle();
        }
    };

    var showActionsPanel = function (event, entryData) {
        var e = ASC.Files.Common.fixEvent(event);

        var target = jq(e.srcElement || e.target);

        entryData = entryData || ASC.Files.UI.getObjectData(target);

        var entryObj = entryData.entryObject;
        if (entryObj.is(".loading")) {
            return true;
        }

        var entryType = entryData.entryType;
        var entryId = entryData.id;

        ASC.Files.Actions.hideAllActionPanels();

        ASC.Files.UI.checkSelectAll(false);
        ASC.Files.UI.selectRow(entryObj, true);
        ASC.Files.UI.updateMainContentHeader();

        var entryTitle = entryData.title;

        jq("#files_actionPanel_folders, #files_actionPanel_files").hide();
        if (entryType === "file") {
            jq("#files_open_files,\
                #files_edit_files,\
                #files_download_files,\
                #files_shareaccess_files,\
                #files_unsubscribe_files,\
                #files_versions_files,\
                #files_moveto_files,\
                #files_copyto_files,\
                #files_rename_files,\
				#files_restore_files,\
                #files_remove_files").css("display", "");

            jq("#files_download_files").unbind("click").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Folders.download(entryType, entryId);
            });

            jq("#files_rename_files").unbind("click").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Folders.rename(entryType, entryId);
            });

            jq("#files_remove_files").unbind("click").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Folders.deleteItem(entryType, entryId);
            });

            if (ASC.Files.Utility.CanWebView(entryTitle)
                || typeof ASC.Files.ImageViewer != "undefined" && ASC.Files.Utility.CanImageView(entryTitle)) {
                jq("#files_open_files").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    var isNew = entryObj.hasClass("isNewForWebEditor");
                    ASC.Files.Folders.clickOnFile(entryId, null, entryTitle, isNew);
                    entryObj.removeClass("isNewForWebEditor");
                    return false;
                });
            } else {
                jq("#files_open_files").hide();
            }

            var countVersion = entryData.version || 0;
            if (countVersion < 2
                || ASC.Files.Folders.folderContainer == "trash"
                    || jq("#content_versions").length != 0) {
                jq("#files_versions_files").hide();
            } else {
                jq("#files_versions_files span").html(countVersion);
                jq("#files_versions_files").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    ASC.Files.Folders.showVersions(entryObj, entryId);
                });
            }

            if (ASC.Files.UI.editingFile(entryObj)) {
                jq("#files_edit_files,\
                    #files_versions_files,\
                    #files_moveto_files,\
                    #files_rename_files,\
                    #files_remove_files").hide();
            } else {
                if (ASC.Files.UI.editableFile(entryData)
                    && !ASC.Files.UI.editingFile(entryObj)) {
                    jq("#files_edit_files").unbind("click").click(function () {
                        ASC.Files.Actions.hideAllActionPanels();
                        PopupKeyUpActionProvider.CloseDialog();
                        ASC.Files.Actions.checkEditFile(entryId, entryTitle);
                    });
                } else {
                    jq("#files_edit_files").hide();
                }
            }

            if (ASC.Files.Folders.folderContainer == "trash") {
                jq("#files_open_files,\
                    #files_edit_files,\
                    #files_shareaccess_files,\
                    #files_versions_files,\
                    #files_moveto_files,\
                    #files_copyto_files,\
                    #files_rename_files").hide();

                jq("#files_remove_files,#files_restore_files").css("display", "");
                ;
            } else {
                jq("#files_restore_files").hide();
            }

            if (ASC.Files.Folders.folderContainer == "project") {
                jq("#files_shareaccess_files").hide();
            }

            if (!ASC.Files.UI.accessibleItem(entryObj)) {
                jq("#files_edit_files,\
                    #files_rename_files").hide();
            }

            if (ASC.Files.UI.accessAdmin(entryObj)) {
                if (jq("#files_mainContent").hasClass("withShare")) {
                    jq("#files_shareaccess_files").unbind("click").click(function () {
                        ASC.Files.Actions.hideAllActionPanels();
                        ASC.Files.Share.getSharedInfo(entryType, entryId, entryTitle);
                    });
                } else {
                    jq("#files_shareaccess_files").hide();
                }
            } else {
                jq("#files_moveto_files,\
                    #files_shareaccess_files,\
                    #files_remove_files").hide();
            }

            if (ASC.Files.Folders.folderContainer == "forme") {
                jq("#files_unsubscribe_files").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    ASC.Files.Share.unSubscribeMe(entryType, entryId);
                });
            } else {
                jq("#files_unsubscribe_files").hide();
            }

            if (ASC.Files.ThirdParty && ASC.Files.ThirdParty.isThirdParty(entryObj)) {
                jq("#files_shareaccess_files").hide();
                if (!ASC.Files.ThirdParty.isThirdParty()) {
                    jq("#files_remove_files").hide();
                }
            }

            jq("#files_actionPanel_files").show();

        } else {
            jq("#files_open_folders,\
				#files_download_folders,\
				#files_shareAccess_folders,\
				#files_unsubscribe_folders,\
				#files_moveto_folders,\
				#files_copyto_folders,\
				#files_rename_folders,\
				#files_restore_folders,\
                #files_remove_folders,\
                #files_remove_thirdparty,\
                #files_change_thirdparty").css("display", "");

            jq("#files_open_folders").unbind("click").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Folders.navigationSet(entryId);
            });

            jq("#files_download_folders").unbind("click").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Folders.download(entryType, entryId);
            });

            jq("#files_rename_folders").unbind("click").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Folders.rename(entryType, entryId);
            });

            if (ASC.Files.Folders.folderContainer == "trash") {
                jq("#files_open_folders,\
                    #files_shareAccess_folders,\
                    #files_moveto_folders,\
                    #files_copyto_folders,\
                    #files_rename_folders").hide();
            } else {
                jq("#files_restore_folders").hide();
            }

            if (ASC.Files.Folders.folderContainer == "project") {
                jq("#files_shareAccess_folders").hide();
            }

            if (!ASC.Files.UI.accessibleItem(entryObj)) {
                jq("#files_rename_folders,\
                    #files_remove_thirdparty,\
                    #files_change_thirdparty").hide();
            }

            if (ASC.Files.UI.accessAdmin(entryObj)) {
                if (jq("#files_mainContent").hasClass("withShare")
                    && ASC.Files.Folders.folderContainer == "corporate") {
                    jq("#files_shareAccess_folders").unbind("click").click(function () {
                        ASC.Files.Actions.hideAllActionPanels();
                        ASC.Files.Share.getSharedInfo(entryType, entryId, entryTitle);
                    });
                } else {
                    jq("#files_shareAccess_folders").hide();
                }

                jq("#files_remove_folders").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    ASC.Files.Folders.deleteItem(entryType, entryId);
                });

                jq("#files_remove_thirdparty").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    ASC.Files.ThirdParty.showDeleteDialog(entryObj, entryId, entryTitle);
                });

                jq("#files_change_thirdparty").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    ASC.Files.ThirdParty.showChangeDialog(entryObj, entryId, entryTitle, entryData);
                });
            } else {
                jq("#files_moveto_folders,\
                    #files_shareAccess_folders,\
                    #files_remove_folders,\
                    #files_remove_thirdparty\
                    #files_change_thirdparty").hide();
            }

            if (ASC.Files.Folders.folderContainer == "forme") {
                jq("#files_unsubscribe_folders").unbind("click").click(function () {
                    ASC.Files.Actions.hideAllActionPanels();
                    ASC.Files.Share.unSubscribeMe(entryType, entryId);
                });
            } else {
                jq("#files_unsubscribe_folders").hide();
            }

            if (ASC.Files.ThirdParty && ASC.Files.ThirdParty.isThirdParty(entryObj)) {
                jq("#files_shareAccess_folders").hide();
                if (ASC.Files.ThirdParty.isThirdParty()) {
                    jq("#files_remove_thirdparty,\
                        #files_change_thirdparty").hide();
                } else {
                    jq("#files_remove_folders,\
                        #files_moveto_folders").hide();

                    if (entryData.error) {
                        jq("#files_open_folders,\
                            #files_download_folders,\
                            #files_copyto_folders,\
                            #files_rename_folders").hide();
                    }
                }
            } else {
                jq("#files_remove_thirdparty,\
                    #files_change_thirdparty").hide();
            }

            jq("#files_actionPanel_folders").show();
        }

        var dropdownItem = jq("#files_actionPanel");

        if (target.is("div.rowActions")) {
            dropdownItem.css(
                {
                    "top": target.offset().top + target.outerHeight(),
                    "left": "auto",
                    "right": jq(window).width() - target.offset().left - target.width(),
                    "margin": "5px -10px 0 0"
                });
            dropdownItem.find(".popup-corner").css({
                "left": "auto",
                "right": "12px"
            });
        } else {
            dropdownItem.css({
                "top": e.pageY,
                "left": e.pageX,
                "right": "auto",
                "margin": "5px 0 0 -25px"
            });
            dropdownItem.find(".popup-corner").css({
                "left": "20px",
                "right": "auto"
            });
        }

        dropdownItem.toggle();
        return false;
    };

    var onContextMenu = function (event) {
        ASC.Files.Actions.hideAllActionPanels();

        var e = ASC.Files.Common.fixEvent(event);

        if (typeof e == "undefined" || !e) {
            return true;
        }

        var target = jq(e.srcElement || e.target);
        if (target.is('[id="files_prompt_rename"]')) {
            return true;
        }

        var entryData = ASC.Files.UI.getObjectData(target);
        if (!entryData) {
            return true;
        }
        var entryObj = entryData.entryObject;

        if (entryObj.hasClass("newFolder") || entryObj.hasClass("rowRename")) {
            return true;
        }

        if (target.is("div.entryTitle a.name") && jq(target, entryObj).length) {
            return true;
        }

        jq("#files_mainContent .row-over").removeClass("row-over");
        var count = jq("#files_mainContent .fileRow.row-selected").length;

        if (count > 1 && entryObj.hasClass("row-selected")) {
            ASC.Files.Actions.showActionsViewPanel(event);
        } else {
            ASC.Files.Actions.showActionsPanel(event, entryData);
        }

        return false;
    };

    var hideAllActionPanels = function () {
        jq("div.files_popup_win").hide();
        ASC.Files.UI.hideEntryTooltip();
    };

    var checkEditFile = function (fileId, title, isNew) {
        isNew = (isNew === true);
        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        if (!isNew && fileObj.hasClass("isNewForWebEditor")) {
            isNew = true;
        }
        fileObj.removeClass("isNewForWebEditor");

        ASC.Files.UI.lockEditFile(fileObj, true);
        ASC.Files.UI.checkEditing();

        if (ASC.Files.Share) {
            ASC.Files.Share.removeNewIcon("file", fileId);
        }

        var url = ASC.Files.Utility.GetFileWebEditorUrl(fileId) + (isNew ? "&new=true" : "");
        var winEditor = window.open(url, "_blank");

        try {
            winEditor.onload = function () {
                if (fileId) {
                    ASC.Files.UI.lockEditFile(fileObj, true);
                    ASC.Files.UI.checkEditing();
                }
            };
        } catch (ex) {
        }

        try {
            winEditor.onunload = function () {
                if (fileId) {
                    ASC.Files.UI.checkEditing();
                }
            };
        } catch (ex) {
        }
        return;
    };

    var mouseBindDocument = function () {
        jq(document).unbind("mousedown");
        jq(document).unbind("mouseup");
        jq(document).unbind("mousemove");

        jq(document).mouseup(function () {
            ASC.Files.UI.mouseBtn = false;
        });

        jq(document).mousedown(function (event) {
            var result = (ASC.Files.UI.beginSelecting(event) == true);
            ASC.Files.UI.mouseBtn = true;
            return result;
        });

        jq("#files_mainContent").mousemove(function (event) {
            if (ASC.Files.UI.mouseBtn == false) {
                ASC.Files.UI.handleMove(event);
            }
            return true;
        });
        ASC.Files.UI.mouseBtn = false;
    };

    return {
        init: init,
        showActionsViewPanel: showActionsViewPanel,
        showActionsPanel: showActionsPanel,
        onContextMenu: onContextMenu,

        checkEditFile: checkEditFile,

        hideAllActionPanels: hideAllActionPanels,

        mouseBindDocument: mouseBindDocument
    };
})();

(function($) {
    ASC.Files.Actions.init();
    $(function() {

        jq("#files_selectAll").click(function() {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.UI.checkSelectAll(true);
        });

        jq("#files_selectFile, #files_selectFolder, #files_selectDocument,\
            #files_selectPresentation, #files_selectSpreadsheet, #files_selectImage").click(function() {
            ASC.Files.UI.checkSelect(this.id.replace("files_select", ""));
        });

        jq("#files_mainContent").attr("oncontextmenu", "return ASC.Files.Actions.onContextMenu(event);");

        jq("#files_mainContent").on("click", "div.rowActions", ASC.Files.Actions.showActionsPanel);

        jq("#files_mainContent").on("click", "div.fileEdit.pencil", function() {
            var entryData = ASC.Files.UI.getObjectData(this);
            var entryId = entryData.entryId;
            var entryTitle = entryData.title;

            ASC.Files.Actions.checkEditFile(entryId, entryTitle, false);
            return false;
        });

        ASC.Files.Actions.mouseBindDocument();
    });
})(jQuery);