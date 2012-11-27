window.ASC.Files.Folders = (function () {
    var tasksTimeout = null;
    var bulkStatuses = false;
    var currentFolderId = "";

    var folderContainer = "";

    var eventAfter = null;
    var typeNewDoc = "";

    var moveToFolder = "";
    var isCopyTo = false;

    /* Methods*/

    var madeAnchor = function (folderId, safemode) {
        if (!ASC.Files.Common.isCorrectId(folderId)) {
            folderId = ASC.Files.Folders.currentFolderId;
        }

        if (safemode === true) {
            ASC.Controls.AnchorController.safemove(folderId);
        } else {
            ASC.Controls.AnchorController.move(folderId);
        }
    };

    var navigationSet = function (param, safemode) {
        ASC.Files.UI.resetSelectAll(false);
        ASC.Files.UI.amountPage = 0;

        if (ASC.Files.Common.isCorrectId(param)) {
            if (!safemode || ASC.Files.Folders.currentFolderId != param) {
                ASC.Files.Folders.currentFolderId = param;
                ASC.Files.Folders.folderContainer = "";
            }
            madeAnchor(null, safemode);
        } else {
            ASC.Files.Folders.currentFolderId = "";
            ASC.Files.Folders.folderContainer = param;
            madeAnchor(param, safemode);
        }
    };

    var defaultFolderSet = function () {
        if (ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT == "") {
            if (ASC.Files.Constants.FOLDER_ID_MY_FILES == "") {
                ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_COMMON_FILES);
            } else {
                if (ASC.Files.Folders.currentFolderId != ASC.Files.Constants.FOLDER_ID_MY_FILES) {
                    ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_MY_FILES);
                }
            }
        } else {
            if (ASC.Files.Folders.currentFolderId != ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT) {
                ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT);
            }
        }
    };

    var getFolderItems = function (isAppend, countAppend) {
        var filterSettings = ASC.Files.Filter.getFilterSettings();

        serviceManager.getFolderItems(ASC.Files.TemplateManager.events.GetFolderItems,
            {
                folderId: ASC.Files.Folders.currentFolderId,
                from: (isAppend ? jq("#files_mainContent li.fileRow[name!='addRow']").length : 0),
                count: countAppend || ASC.Files.Constants.COUNT_ON_PAGE,
                append: isAppend === true,
                filter: filterSettings.filter,
                subject: filterSettings.subject,
                text: filterSettings.text,
                compactView: jq("#files_mainContent").hasClass("compact")
            }, {orderBy: filterSettings.sorter});
    };

    var clickOnFolder = function (folderId) {
        if (ASC.Files.Folders.folderContainer == "trash") {
            return;
        }
        ASC.Files.Folders.navigationSet(folderId);
    };

    var clickOnFile = function (fileId, version, fileTitle, isNew) {
        if (ASC.Files.Folders.folderContainer == "trash") {
            return;
        }

        if (ASC.Files.Share) {
            ASC.Files.Share.removeNewIcon("file", fileId);
        }

        fileTitle = fileTitle || ASC.Files.UI.getEntryTitle("file", fileId);

        var url = ASC.Files.Utility.GetFileViewUrl(fileId, version);

        if (ASC.Files.Utility.CanWebView(fileTitle)) {
            url = ASC.Files.Utility.GetFileWebViewerUrl(fileId, version) + (isNew ? "&new=true" : "");
            window.open(url, "_blank");
            return;
        }

        if (typeof ASC.Files.ImageViewer != "undefined" && ASC.Files.Utility.CanImageView(fileTitle)) {
            url = ASC.Files.ImageViewer.getPreviewUrl(fileId);
            ASC.Controls.AnchorController.move(url);
            return;
        }

        if (jq.browser.mobile) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_MobileDownload, true);
        } else {
            window.open(url, "_blank");
        }
    };

    var download = function (entryType, entryId, version) {
        var list = new Array();
        if (!ASC.Files.Common.isCorrectId(entryId)) {
            list = jq("#files_mainContent li.fileRow:has(div.checkbox input:checked)");
            if (list.length == 0) {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.EmptyListSelectedForDownload, true);
                return;
            }
            if (list.length == 1) {
                var itemData = ASC.Files.UI.getObjectData(list[0]);
                entryId = itemData.entryId;
                entryType = itemData.entryType;
            }
        }

        if (entryType === "file") {
            if (jq.browser.mobile) {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_MobileDownload, true);
                return;
            }

            if (ASC.Files.Share) {
                ASC.Files.Share.removeNewIcon(entryType, entryId);
            }

            location.href = ASC.Files.Utility.GetFileDownloadUrl(entryId, version);
            return;
        }

        if (ASC.Files.Folders.bulkStatuses == true) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SecondDownload, true);
            return;
        }

        var data = {};
        data.entry = new Array();

        if (ASC.Files.Common.isCorrectId(entryId)) {
            data.entry.push(Encoder.htmlEncode(entryType + "_" + entryId));

            if (ASC.Files.Share) {
                ASC.Files.Share.removeNewIcon(entryType, entryId);
            }
        } else {
            list.each(function () {
                var itemData = ASC.Files.UI.getObjectData(this);

                data.entry.push(Encoder.htmlEncode(itemData.entryType + "_" + itemData.entryId));
                if (ASC.Files.Share) {
                    ASC.Files.Share.removeNewIcon(itemData.entryType, itemData.entryId);
                }
            });
        }
        ASC.Files.Folders.bulkStatuses = true;
        serviceManager.download(ASC.Files.TemplateManager.events.Download, {doNow: true}, {stringList: data});
    };

    var createFolder = function () {
        if (ASC.Files.Folders.folderContainer == "trash"
            || !ASC.Files.UI.accessibleItem()) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SecurityException, true);
            ASC.Files.Folders.eventAfter = createFolder;
            ASC.Files.Folders.defaultFolderSet();
            return;
        }

        jq(document).scrollTop(0);

        var newFolderObj = ASC.Files.UI.getEntryObject("folder", 1).filter("[spare_data='NEW_FOLDER']");
        if (newFolderObj.length != 0) {
            return;
        }

        var emptyFolder = {
            folder:
                {
                    id: 1,
                    spare_data: "NEW_FOLDER",
                    title: ASC.Files.FilesJSResources.TitleNewFolder,
                    access: 0,
                    shared: false,
                    isnew: false,
                    shareable: false
                }
        };
        var stringData = serviceManager.jsonToXml(emptyFolder);

        var curTemplate = ASC.Files.TemplateManager.templates.getFolderItem;
        var xslData = ASC.Files.TemplateManager.getTemplate(curTemplate);

        try {
            var htmlXML = ASC.Controls.XSLTManager.translateFromString(stringData, xslData);
        } catch (err) {
            return undefined;
        }

        ASC.Files.EmptyScreen.hideEmptyScreen();
        var mainContent = jq("#files_mainContent");
        mainContent.prepend(htmlXML);

        jq("#files_mainContent li.newFolder").yellowFade().removeClass("newFolder");
        ASC.Files.UI.paintRows();

        newFolderObj = ASC.Files.UI.getEntryObject("folder", 1).filter("[spare_data='NEW_FOLDER']");
        newFolderObj.addClass("rowRename");

        var obj = newFolderObj.find("div.entryTitle a.name");

        var ftClass = ASC.Files.Utility.getFolderCssClass();
        newFolderObj.find(".thumb-folder").addClass(ftClass);

        var newContainer = document.createElement("input");
        newContainer.id = "files_prompt_create_folder";
        newContainer.type = "text";
        newContainer.style.display = "none";
        document.body.appendChild(newContainer);

        newContainer = jq("#files_prompt_create_folder");
        newContainer.attr("maxlength", ASC.Files.Constants.MAX_NAME_LENGTH);
        newContainer.addClass("textEdit renameInput");
        newContainer.val(ASC.Files.FilesJSResources.TitleNewFolder);
        newContainer.insertAfter(obj);
        newContainer.show();
        obj.hide();
        newContainer.focus();
        if (!jq.browser.mobile) {
            newContainer.select();
        }

        ASC.Files.UI.checkCharacter(newContainer);

        var saveFolder = function () {
            var newFolderSaveObj = ASC.Files.UI.getEntryObject("folder", 1).filter("[spare_data='NEW_FOLDER']");

            var newName = ASC.Files.Common.replaceSpecCharacter(jq("#files_prompt_create_folder").val().trim());
            if (newName == "" || newName == null) {
                newName = ASC.Files.FilesJSResources.TitleNewFolder;
            }

            newFolderSaveObj.find("div.entryTitle a.name").show().text(newName);
            newFolderSaveObj.removeClass("rowRename");
            jq("#files_prompt_create_folder").remove();
            newFolderSaveObj.find("div.renameAction").remove();

            var params = {parentFolderID: ASC.Files.Folders.currentFolderId, title: newName};

            ASC.Files.UI.blockObject(newFolderSaveObj, true, ASC.Files.FilesJSResources.DescriptCreate);
            serviceManager.createFolder(ASC.Files.TemplateManager.events.CreateFolder, params);
        };

        newFolderObj.append("<div class='renameAction'><div class='apllyName' title=" + ASC.Files.FilesJSResources.TitleCreate +
            "></div><div class='cancelName' title=" + ASC.Files.FilesJSResources.TitleCancel + "></div></div>");
        newFolderObj.find("div.apllyName").click(saveFolder);
        newFolderObj.find("div.cancelName").click(function () {
            var newFolderCancelObj = ASC.Files.UI.getEntryObject("folder", 1).filter("[spare_data='NEW_FOLDER']");
            newFolderCancelObj.remove();
            if (jq("#files_mainContent li.fileRow").length == 0) {
                ASC.Files.EmptyScreen.displayEmptyScreen();
            } else {
                ASC.Files.UI.paintRows();
            }
        });

        jq("#files_prompt_create_folder").keypress(function (event) {
            if (jq("#files_prompt_create_folder").length == 0) {
                return;
            }

            if (!e) {
                var e = event;
            }
            var code = e.keyCode || e.which;

            switch (code) {
            case ASC.Files.Common.keyCode.esc:
                var newFolderCancelObj = ASC.Files.UI.getEntryObject("folder", 1).filter("[spare_data='NEW_FOLDER']");
                newFolderCancelObj.remove();
                if (jq("#files_mainContent li.fileRow").length == 0) {
                    ASC.Files.EmptyScreen.displayEmptyScreen();
                } else {
                    ASC.Files.UI.paintRows();
                }
                break;
            case ASC.Files.Common.keyCode.enter:
                saveFolder();
                break;
            }
        });
    };

    var createNewDoc = function () {
        jq(document).scrollTop(0);

        if (ASC.Files.Folders.folderContainer == "trash"
            || !ASC.Files.UI.accessibleItem()) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SecurityException, true);
            ASC.Files.Folders.eventAfter = createNewDoc;
            ASC.Files.Folders.defaultFolderSet();
            return;
        }

        var newFileObj = ASC.Files.UI.getEntryObject("file", 1).filter("[spare_data='NEW_FILE']");
        newFileObj.remove();

        var titleNewDoc;
        switch (ASC.Files.Folders.typeNewDoc) {
        case "document":
            titleNewDoc = ASC.Files.FilesJSResources.TitleNewFileText + ASC.Files.Constants.typeNewDoc.document;
            break;
        case "spreadsheet":
            titleNewDoc = ASC.Files.FilesJSResources.TitleNewFileSpreadsheet + ASC.Files.Constants.typeNewDoc.spreadsheet;
            break;
        case "presentation":
            titleNewDoc = ASC.Files.FilesJSResources.TitleNewFilePresentation + ASC.Files.Constants.typeNewDoc.presentation;
            break;
        case "image":
            titleNewDoc = ASC.Files.FilesJSResources.TitleNewFileImage + ASC.Files.Constants.typeNewDoc.image;
            break;
        default:
            return;
        }

        if (!ASC.Files.Utility.CanWebEdit(titleNewDoc)) {
            return;
        }

        var emptyFile = {
            file:
                {
                    id: 1,
                    spare_data: "NEW_FILE",
                    title: titleNewDoc,
                    access: 0,
                    shared: false,
                    version: 0
                }
        };

        var stringData = serviceManager.jsonToXml(emptyFile);

        var curTemplate = ASC.Files.TemplateManager.templates.getFolderItem;
        var xslData = ASC.Files.TemplateManager.getTemplate(curTemplate);

        try {
            var htmlXML = ASC.Controls.XSLTManager.translateFromString(stringData, xslData);
        } catch (err) {
            return undefined;
        }

        ASC.Files.EmptyScreen.hideEmptyScreen();
        var mainContent = jq("#files_mainContent");
        mainContent.prepend(htmlXML);

        jq("#files_mainContent li.newFile").show().yellowFade().removeClass("newFile");
        ASC.Files.UI.paintRows();

        newFileObj = ASC.Files.UI.getEntryObject("file", 1).filter("[spare_data='NEW_FILE']");
        newFileObj.addClass("rowRename");

        var obj = newFileObj.find("div.entryTitle a.name");

        var ftClass = ASC.Files.Utility.getCssClassByFileTitle(titleNewDoc);
        newFileObj.find(".thumb-file").addClass(ftClass);

        var lenExt = ASC.Files.Utility.GetFileExtension(titleNewDoc).length;
        titleNewDoc = titleNewDoc.substring(0, titleNewDoc.length - lenExt);

        var newContainer = document.createElement("input");
        newContainer.id = "files_prompt_create_file";
        newContainer.type = "text";
        newContainer.style.display = "none";
        document.body.appendChild(newContainer);
        newContainer = jq("#files_prompt_create_file");
        newContainer.attr("maxlength", ASC.Files.Constants.MAX_NAME_LENGTH - lenExt);
        newContainer.addClass("textEdit renameInput");
        newContainer.val(titleNewDoc);
        newContainer.insertAfter(obj);
        newContainer.show().focus();
        if (!jq.browser.mobile) {
            newContainer.select();
        }

        ASC.Files.UI.checkCharacter(newContainer);

        var saveFile = function () {
            var newFileSaveObj = ASC.Files.UI.getEntryObject("file", 1).filter("[spare_data='NEW_FILE']");

            var newName = ASC.Files.Common.replaceSpecCharacter(jq("#files_prompt_create_file").val().trim());
            var oldName = ASC.Files.UI.getObjectTitle(newFileSaveObj);
            if (newName == "" || newName == null) {
                newName = oldName;
            } else {
                var lenExt = ASC.Files.Utility.GetFileExtension(oldName).length;
                newName += oldName.substring(oldName.length - lenExt);
            }

            newFileSaveObj.find("div.entryTitle a.name").show().text(newName);
            newFileSaveObj.removeClass("rowRename");
            jq("#files_prompt_create_file").remove();
            newFileSaveObj.find("div.renameAction").remove();

            var params = {folderID: ASC.Files.Folders.currentFolderId, fileTitle: newName};

            ASC.Files.UI.blockObject(newFileObj, true, ASC.Files.FilesJSResources.DescriptCreate);
            serviceManager.createNewFile(ASC.Files.TemplateManager.events.CreateNewFile, params);
        };

        newFileObj.append("<div class='renameAction'><div class='apllyName' title=" + ASC.Files.FilesJSResources.TitleCreate +
            "></div><div class='cancelName' title=" + ASC.Files.FilesJSResources.TitleCancel + "></div></div>");
        newFileObj.find("div.apllyName").click(saveFile);
        newFileObj.find("div.cancelName").click(function () {
            var newFileCancelObj = ASC.Files.UI.getEntryObject("file", 1).filter("[spare_data='NEW_FILE']");
            newFileCancelObj.remove();
            if (jq("#files_mainContent li.fileRow").length == 0) {
                ASC.Files.EmptyScreen.displayEmptyScreen();
            } else {
                ASC.Files.UI.paintRows();
            }
        });

        jq("#files_prompt_create_file").keypress(function (event) {
            if (jq("#files_prompt_create_file").length == 0) {
                return;
            }

            if (!e) {
                var e = event;
            }
            var code = e.keyCode || e.which;

            switch (code) {
            case ASC.Files.Common.keyCode.esc:
                var newFileCancelObj = ASC.Files.UI.getEntryObject("file", 1).filter("[spare_data='NEW_FILE']");
                newFileCancelObj.remove();
                if (jq("#files_mainContent li.fileRow").length == 0) {
                    ASC.Files.EmptyScreen.displayEmptyScreen();
                } else {
                    ASC.Files.UI.paintRows();
                }
                break;
            case ASC.Files.Common.keyCode.enter:
                saveFile();
                break;
            }
        });
    };

    var rename = function (entryType, entryId) {
        var entryObj = ASC.Files.UI.getEntryObject(entryType, entryId);

        var lenExt = 0;

        if (jq("#files_prompt_rename").length != 0) {
            jq("#files_mainContent li.fileRow.rowRename div.cancelName").click();
        }

        entryObj.addClass("rowRename");
        ASC.Files.UI.selectRow(entryObj, false);
        ASC.Files.UI.updateMainContentHeader();

        var entryTitle = ASC.Files.UI.getObjectTitle(entryObj);

        if (entryType == "file") {
            lenExt = ASC.Files.Utility.GetFileExtension(entryTitle).length;
            entryTitle = entryTitle.substring(0, entryTitle.length - lenExt);
        }

        var newContainer = document.createElement("input");
        newContainer.id = "files_prompt_rename";
        newContainer.type = "text";
        newContainer.style.display = "none";
        document.body.appendChild(newContainer);

        newContainer = jq("#files_prompt_rename");
        newContainer.attr("maxlength", ASC.Files.Constants.MAX_NAME_LENGTH - lenExt);
        newContainer.addClass("textEdit renameInput");
        newContainer.val(entryTitle);
        newContainer.insertAfter(entryObj.find("div.entryTitle a.name"));
        newContainer.show().focus();
        if (!jq.browser.mobile) {
            newContainer.select();
        }

        ASC.Files.UI.checkCharacter(newContainer);

        var saveRename = function () {
            var entryRenameData = ASC.Files.UI.getObjectData(jq("#files_prompt_rename"));
            var entryRenameObj = entryRenameData.entryObject;
            var entryRenameType = entryRenameData.entryType;
            var entryRenameId = entryRenameData.entryId;
            var oldName = ASC.Files.UI.getObjectTitle(entryRenameObj);

            var newName = ASC.Files.Common.replaceSpecCharacter(jq("#files_prompt_rename").val().trim());
            if (newName == "" || newName == null) {
                return;
            }

            if (entryRenameType == "file") {
                var lenExtRename = ASC.Files.Utility.GetFileExtension(oldName).length;
                newName += oldName.substring(oldName.length - lenExtRename);
            }

            entryRenameObj.find("div.entryTitle a.name").show().text(newName);
            entryRenameObj.removeClass("rowRename");
            jq("#files_prompt_rename").remove();
            entryRenameObj.find(".renameAction").remove();

            if (newName != oldName) {
                ASC.Files.UI.blockObject(entryRenameObj, true, ASC.Files.FilesJSResources.DescriptRename);

                if (entryRenameType == "file") {
                    serviceManager.renameFile(ASC.Files.TemplateManager.events.FileRename, {fileId: entryRenameId, name: oldName, newname: newName, show: true});
                } else {
                    serviceManager.renameFolder(ASC.Files.TemplateManager.events.FolderRename, {folderId: entryRenameId, name: oldName, newname: newName});
                }
            }
        };

        entryObj.append("<div class='renameAction'><div class='apllyName' title=" + ASC.Files.FilesJSResources.TitleRename +
            "></div><div class='cancelName' title=" + ASC.Files.FilesJSResources.TitleCancel + "></div></div>");
        entryObj.find("div.apllyName").click(saveRename);
        entryObj.find("div.cancelName").click(function () {
            var entryObjCancel = jq("#files_prompt_rename").closest("li.fileRow");
            jq("#files_prompt_rename").remove();
            entryObjCancel.removeClass("rowRename");
            entryObjCancel.find(".renameAction").remove();
        });

        entryObj.removeClass("row-selected user_select_none");

        jq("#files_prompt_rename").keypress(function (event) {
            if (jq("#files_prompt_rename").length === 0) {
                return;
            }

            if (!e) {
                var e = event;
            }
            var code = e.keyCode || e.which;

            if ((code == ASC.Files.Common.keyCode.esc || code == ASC.Files.Common.keyCode.enter)
                && jq("#files_prompt_rename").length != 0) {
                switch (code) {
                case ASC.Files.Common.keyCode.esc:
                    var entryObjCancel = jq("#files_prompt_rename").closest("li.fileRow");
                    jq("#files_prompt_rename").remove();
                    entryObjCancel.removeClass("rowRename");
                    entryObjCancel.find(".renameAction").remove();
                    break;
                case ASC.Files.Common.keyCode.enter:
                    saveRename();
                    break;
                }
            }
        });
    };

    var showVersions = function (fileObj, fileId) {
        if (ASC.Files.Folders.folderContainer == "trash") {
            return;
        }

        fileObj = jq(fileObj);
        if (!fileObj.is("li.fileRow")) {
            fileObj = fileObj.closest("li.fileRow");
        }

        if (fileObj.hasClass("folderRow")) {
            return;
        }

        if (fileObj.find("div.version").length == 0) {
            return;
        }

        if (jq("#content_versions:visible").length != 0) {
            var close = fileObj.find("#content_versions").length != 0;
            closeVersions();
            if (close) {
                return;
            }
        }

        ASC.Files.UI.blockObject(fileObj, true, ASC.Files.FilesJSResources.DescriptLoadVersion);

        fileId = fileId || ASC.Files.UI.getObjectData(fileObj).entryId;
        serviceManager.getFileHistory(ASC.Files.TemplateManager.events.GetFileHistory, {fileId: fileId}, {});
    };

    var makeCurrentVersion = function (fileId, version) {
        jq("div.version_operation").css("visibility", "hidden");
        ASC.Files.UI.blockObjectById("file", fileId, true, ASC.Files.FilesJSResources.DescriptSetVersion);
        serviceManager.setCurrentVersion(ASC.Files.TemplateManager.events.SetCurrentVersion, {fileId: fileId, version: version});
    };

    var closeVersions = function () {
        jq("#content_versions").remove();
        jq("#files_mainContent .row-over").removeClass("row-over");
    };

    var replaceVersion = function (fileId, show) {
        serviceManager.getFile(ASC.Files.TemplateManager.events.ReplaceVersion,
            {
                fileId: fileId,
                show: show,
                isStringXml: false
            });
    };

    var getFolderInfo = function () {
        serviceManager.getFolderInfo(ASC.Files.TemplateManager.events.GetFolderInfo, {folderId: ASC.Files.Folders.currentFolderId});
    };

    var curItemFolderMoveTo = function (folderToId, folderToTitle, pathDest) {
        var thirdParty = typeof ASC.Files.ThirdParty != "undefined";
        var takeThirdParty = thirdParty && (ASC.Files.Folders.isCopyTo == true || ASC.Files.ThirdParty.isThirdParty());

        if (folderToId === ASC.Files.Folders.currentFolderId) {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.isCopyTo = false;
            return;
        }

        var data = {};
        data.entry = new Array();

        jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):has(div.checkbox input:checked)").each(function () {
            var entryData = ASC.Files.UI.getObjectData(this);
            var entryType = entryData.entryType;
            var entryObj = entryData.entryObject;
            var entryId = entryData.entryId;

            if (ASC.Files.Folders.isCopyTo == true
                || (ASC.Files.UI.accessAdmin(entryObj) && !ASC.Files.UI.editingFile(entryObj))) {

                if (jq.inArray(entryId, pathDest) != -1) {
                    ASC.Files.UI.displayInfoPanel(((ASC.Files.Folders.isCopyTo == true) ? ASC.Files.FilesJSResources.InfoFolderCopyError : ASC.Files.FilesJSResources.InfoFolderMoveError), true);
                } else {
                    if (takeThirdParty
                        || !thirdParty
                            || !ASC.Files.ThirdParty.isThirdParty(entryObj)) {
                        ASC.Files.UI.blockObject(entryObj,
                            true,
                            (ASC.Files.Folders.isCopyTo == true) ? ASC.Files.FilesJSResources.DescriptCopy : ASC.Files.FilesJSResources.DescriptMove);
                        data.entry.push(Encoder.htmlEncode(entryType + "_" + entryId));
                    }
                }
            }
        });

        ASC.Files.Actions.hideAllActionPanels();

        if (data.entry && data.entry.length != 0) {
            serviceManager.moveFilesCheck(ASC.Files.TemplateManager.events.MoveFilesCheck,
                {
                    folderToTitle: folderToTitle,
                    folderToId: folderToId,
                    list: data,
                    isCopyOperation: (ASC.Files.Folders.isCopyTo == true)
                },
                {stringList: data});
        }

        ASC.Files.Folders.isCopyTo = false;
    };

    var showMore = function () {
        if (jq("#files_pageNavigatorHolder:visible").length == 0
            || jq("#files_pageNavigatorHolder a").text() == ASC.Files.FilesJSResources.ButtonShowMoreLoad) {
            return;
        }

        jq("#files_pageNavigatorHolder a").text(ASC.Files.FilesJSResources.ButtonShowMoreLoad);

        ASC.Files.Folders.getFolderItems(true);
    };

    var emptyTrash = function () {
        if (ASC.Files.Folders.folderContainer != "trash") {
            return;
        }

        ASC.Files.Actions.hideAllActionPanels();

        ASC.Files.UI.checkSelectAll(true);

        jq("#confirmRemoveText").html("<b>" + ASC.Files.FilesJSResources.ConfirmEmptyTrash + "</b>");
        jq("#confirmRemoveList").hide();
        jq("#confirmRemoveTextDescription").show();

        jq("#removeConfirmBtn").unbind("click").click(function () {
            PopupKeyUpActionProvider.CloseDialog();

            serviceManager.emptyTrash(ASC.Files.TemplateManager.events.EmptyTrash, {doNow: true});
        });

        ASC.Files.Common.blockUI(jq("#files_confirm_remove"), 420, 0, -150);
        PopupKeyUpActionProvider.EnterAction = 'jq("#removeConfirmBtn").click();';
    };

    var deleteItem = function (entryType, entryId) {
        if (ASC.Files.Folders.folderContainer == "forme" && ASC.Files.Share) {
            ASC.Files.Share.unSubscribeMe(entryType, entryId);
            return;
        }

        if (!ASC.Files.UI.accessibleItem()) {
            return;
        }

        ASC.Files.Actions.hideAllActionPanels();

        var caption = ASC.Files.FilesJSResources.ConfirmRemoveList;
        var list = new Array();

        if (entryType && entryId) {
            var entryObj = ASC.Files.UI.getEntryObject(entryType, entryId);
            if (!ASC.Files.UI.accessAdmin(entryObj)) {
                return;
            }
            if (ASC.Files.UI.editingFile(entryObj)) {
                return;
            }

            list.push({entryType: entryType, entryId: entryId});

        } else {
            jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):has(div.checkbox input:checked)").each(function () {
                var entryRowData = ASC.Files.UI.getObjectData(this);
                var entryRowObj = entryRowData.entryObject;
                var entryRowType = entryRowData.entryType;
                var entryRowId = entryRowData.entryId;

                if (ASC.Files.ThirdParty && !ASC.Files.ThirdParty.isThirdParty()
                    && ASC.Files.ThirdParty.isThirdParty(entryRowData.entryObject)) {
                    return true;
                }
                if (!ASC.Files.UI.editingFile(entryRowObj)
                    && ASC.Files.UI.accessAdmin(entryRowObj)) {
                    list.push({entryType: entryRowType, entryId: entryRowId});
                }
            });
        }

        if (list.length == 0) {
            return;
        }

        if (list.length == 1) {
            if (list[0].entryType == "file") {
                caption = ASC.Files.FilesJSResources.ConfirmRemoveFile;
            } else {
                caption = ASC.Files.FilesJSResources.ConfirmRemoveFolder;
            }
        }

        var textFolder = "";
        var textFile = "";
        var strHtml = "<label title='{0}'><input type='checkbox' entryType='{1}' entryId='{2}' checked='checked'>{0}</label>";
        for (var i = 0; i < list.length; i++) {
            var entryRowTitle = ASC.Files.UI.getEntryTitle(list[i].entryType, list[i].entryId);
            if (list[i].entryType == "file") {
                textFile += strHtml.format(entryRowTitle, list[i].entryType, list[i].entryId);
            } else {
                textFolder += strHtml.format(entryRowTitle, list[i].entryType, list[i].entryId);
            }
        }

        jq("#confirmRemoveText").html("<b>" + caption + "</b>");
        jq("#confirmRemoveList dd.confirmRemoveFiles").html(textFile);
        jq("#confirmRemoveList dd.confirmRemoveFolders").html(textFolder);

        jq("#confirmRemoveList .confirmRemoveFolders, #confirmRemoveList .confirmRemoveFiles").show();
        if (textFolder == "") {
            jq("#confirmRemoveList .confirmRemoveFolders").hide();
        }
        if (textFile == "") {
            jq("#confirmRemoveList .confirmRemoveFiles").hide();
        }

        if (ASC.Files.Folders.folderContainer == "trash"
            || ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT != ""
                || (ASC.Files.ThirdParty && ASC.Files.ThirdParty.isThirdParty())) {
            jq("#confirmRemoveTextDescription").show();
        } else {
            jq("#confirmRemoveTextDescription").hide();
        }

        jq("#removeConfirmBtn").unbind("click").click(function () {
            PopupKeyUpActionProvider.CloseDialog();

            var data = {};
            var list = jq("#confirmRemoveList input:checked");
            if (list.length == 0) {
                return;
            }
            data.entry = new Array();
            for (var i = 0; i < list.length; i++) {
                var entryConfirmType = jq(list[i]).attr("entryType");
                var entryConfirmId = jq(list[i]).attr("entryId");
                var entryConfirmObj = ASC.Files.UI.getEntryObject(entryConfirmType, entryConfirmId);
                ASC.Files.UI.blockObject(entryConfirmObj, true, ASC.Files.FilesJSResources.DescriptRemove);
                data.entry.push(Encoder.htmlEncode(entryConfirmType + "_" + entryConfirmId));
            }
            serviceManager.deleteItem(ASC.Files.TemplateManager.events.DeleteItem, {list: data.entry, doNow: true}, {stringList: data});
        });

        ASC.Files.Common.blockUI(jq("#files_confirm_remove"), 420, 0, -150);

        PopupKeyUpActionProvider.EnterAction = 'jq("#removeConfirmBtn").click();';
    };

    var cancelTasksStatuses = function () {
        ASC.Files.Folders.bulkStatuses = false;

        jq("#files_tasks .progress").css("width", "0%");
        jq("#files_tasks .percent").text("0%");
        jq("#files_tasks .headerBaseMedium").html("");
        jq("#files_tasks").hide();
    };

    var terminateTasks = function (isImport) {
        clearTimeout(ASC.Files.Folders.tasksTimeout);

        serviceManager.terminateTasks(ASC.Files.TemplateManager.events.TerminateTasks, {isImport: isImport, isTerminate: true, doNow: true});
    };

    var getTasksStatuses = function (doNow) {
        clearTimeout(ASC.Files.Folders.tasksTimeout);

        ASC.Files.Folders.tasksTimeout = setTimeout(
            function () {
                serviceManager.getTasksStatuses(ASC.Files.TemplateManager.events.GetTasksStatuses, {doNow: false});
            }, ASC.Files.Constants.REQUEST_STATUS_DELAY / (doNow === true ? 2 : 1));
    };

    return {
        eventAfter: eventAfter,

        currentFolderId: currentFolderId,
        navigationSet: navigationSet,
        defaultFolderSet: defaultFolderSet,

        moveToFolder: moveToFolder,
        isCopyTo: isCopyTo,

        createFolder: createFolder,
        getFolderInfo: getFolderInfo,
        replaceVersion: replaceVersion,

        showVersions: showVersions,
        makeCurrentVersion: makeCurrentVersion,
        closeVersions: closeVersions,

        curItemFolderMoveTo: curItemFolderMoveTo,

        rename: rename,
        deleteItem: deleteItem,
        emptyTrash: emptyTrash,

        getFolderItems: getFolderItems,

        folderContainer: folderContainer,

        clickOnFolder: clickOnFolder,
        clickOnFile: clickOnFile,

        showMore: showMore,

        createNewDoc: createNewDoc,
        typeNewDoc: typeNewDoc,

        download: download,
        getTasksStatuses: getTasksStatuses,
        cancelTasksStatuses: cancelTasksStatuses,
        terminateTasks: terminateTasks,
        tasksTimeout: tasksTimeout,
        bulkStatuses: bulkStatuses
    };
})();

(function ($) {
    $(function () {

        jq("#files_pageNavigatorHolder a").click(function () {
            ASC.Files.Folders.showMore();
            return false;
        });

        jq("#topNewFolder a").click(function () {
            ASC.Files.Folders.createFolder();
        });

        jq("#files_cancelUpload").click(function () {
            PopupKeyUpActionProvider.CloseDialog();
        });

        jq("#files_deleteButton, #files_mainDelete").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.deleteItem();
        });

        jq("#files_emptyTrashButton, #files_mainEmptyTrash").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.emptyTrash();
        });

        jq("#files_downloadButton, #files_mainDownload").click(function () {
            if (jq(this).is("#files_mainDownload") && !jq(this).is(".unlockAction")) {
                return;
            }
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.download();
        });

        jq("#files_selectAll_check").click(function () {
            ASC.Files.UI.checkSelectAll(jq("#files_selectAll_check").is(":checked") == true);
            jq(this).blur();
        });

        jq("#files_mainContent").on("click", "div.version", function () {
            ASC.Files.Folders.showVersions(this);
            return false;
        });

        jq("#files_mainContent").on("click",
            "li.fileRow:not(.folderRow):not([spare_data]) div.entryTitle a.name, li.fileRow:not(.folderRow):not([spare_data]) div.thumb-file",
            function () {
                var fileData = ASC.Files.UI.getObjectData(this);
                var fileObj = fileData.entryObject;
                var fileId = fileData.id;
                var fileTitle = fileData.title;
                var isNew = fileObj.hasClass("isNewForWebEditor");
                ASC.Files.Folders.clickOnFile(fileId, null, fileTitle, isNew);
                fileObj.removeClass("isNewForWebEditor");
                return false;
            });

        jq("#files_mainContent").on("click",
            "li.folderRow:not(.errorEntry):not([spare_data]) div.entryTitle a.name, li.folderRow:not(.errorEntry):not([spare_data]) div.thumb-folder",
            function () {
                var fileId = ASC.Files.UI.getObjectData(this).id;
                ASC.Files.Folders.clickOnFolder(fileId);
                return false;
            });

        jq("#files_mainContent").on("mouseover", "#content_versions .versionRow", function () {
            jq("#content_versions .actionRow").removeClass("actionRow");
            jq(this).addClass("actionRow");
        });

        jq("#files_mainContent").on("mouseout", "#content_versions .versionRow.actionRow", function () {
            jq(this).removeClass("actionRow");
        });

        jq("#files_mainContent").on("click", "#content_versions .previewVersion", function () {
            var fileData = ASC.Files.UI.getObjectData(this);
            var fileId = fileData.id;
            var fileTitle = fileData.title;
            var version = jq(this).closest(".versionRow").attr("data-version");
            ASC.Files.Folders.clickOnFile(fileId, version, fileTitle);
            return false;
        });

        jq("#files_mainContent").on("click", "#content_versions .downloadVersion", function () {
            var fileId = ASC.Files.UI.getObjectData(this).id;
            var version = jq(this).closest(".versionRow").attr("data-version");
            ASC.Files.Folders.download("file", fileId, version);
            return false;
        });

        jq("#files_mainContent").on("click", "#content_versions .version_restore", function () {
            var fileId = ASC.Files.UI.getObjectData(this).id;
            var version = jq(this).closest(".versionRow").attr("data-version");
            ASC.Files.Folders.makeCurrentVersion(fileId, version);
            return false;
        });

        if (typeof ASC.Files.Share == "undefined") {
            jq("#files_shareaccess_folders, #files_shareaccess_files,\
                #files_unsubscribe_files, #files_unsubscribe_folders").remove();
        }

        jq(window).scroll(function () {
            ASC.Files.UI.toggleMainMenu();
            if (jq(document).height() - jq(window).height() <= jq(window).scrollTop() + 350) {
                ASC.Files.Folders.showMore();
            }

            return true;
        });

        LoadingBanner.displayLoading(true);

    });
})(jQuery);