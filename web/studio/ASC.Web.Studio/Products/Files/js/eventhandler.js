window.ASC.Files.EventHandler = (function() {
    var isInit = false;
    var timoutTasksStatuses = null;

    var init = function() {
        if (isInit === false) {
            isInit = true;

            ASC.Controls.AnchorController.bind(ASC.Files.Constants.anchorRegExp.anyanchor, ASC.Files.EventHandler.onValidationAnchor);
            ASC.Controls.AnchorController.bind(ASC.Files.Constants.anchorRegExp.error, ASC.Files.EventHandler.onError);
            ASC.Controls.AnchorController.bind(ASC.Files.Constants.anchorRegExp.folder, ASC.Files.EventHandler.onSelectAnchor);
            ASC.Controls.AnchorController.bind(ASC.Files.Constants.anchorRegExp.preview, ASC.Files.EventHandler.onPreviewAnchor);
            ASC.Controls.AnchorController.bind(ASC.Files.Constants.anchorRegExp.imprt, ASC.Files.EventHandler.onImportAnchor);

            serviceManager.bind(ASC.Files.TemplateManager.events.GetFolderItems, ASC.Files.EventHandler.onGetFolderItems);

            serviceManager.bind(ASC.Files.TemplateManager.events.CheckEditing, ASC.Files.EventHandler.onCheckEditing);

            serviceManager.bind(ASC.Files.TemplateManager.events.GetFolderInfo, ASC.Files.EventHandler.onGetFolderInfo);
            serviceManager.bind(ASC.Files.TemplateManager.events.CreateFolder, ASC.Files.EventHandler.onCreateFolder);
            serviceManager.bind(ASC.Files.TemplateManager.events.CreateNewFile, ASC.Files.EventHandler.onCreateNewFile);

            serviceManager.bind(ASC.Files.TemplateManager.events.FolderRename, ASC.Files.EventHandler.onRenameFolder);
            serviceManager.bind(ASC.Files.TemplateManager.events.FileRename, ASC.Files.EventHandler.onRenameFile);

            serviceManager.bind(ASC.Files.TemplateManager.events.GetFileHistory, ASC.Files.EventHandler.onGetFileHistory);
            serviceManager.bind(ASC.Files.TemplateManager.events.SetCurrentVersion, ASC.Files.EventHandler.onSetCurrentVersion);
            serviceManager.bind(ASC.Files.TemplateManager.events.ReplaceVersion, ASC.Files.EventHandler.onReplaceVersion);

            serviceManager.bind(ASC.Files.TemplateManager.events.MoveFilesCheck, ASC.Files.EventHandler.onMoveFilesCheck);

            serviceManager.bind(ASC.Files.TemplateManager.events.MoveItems, ASC.Files.EventHandler.onGetTasksStatuses);
            serviceManager.bind(ASC.Files.TemplateManager.events.DeleteItem, ASC.Files.EventHandler.onGetTasksStatuses);
            serviceManager.bind(ASC.Files.TemplateManager.events.EmptyTrash, ASC.Files.EventHandler.onGetTasksStatuses);
            serviceManager.bind(ASC.Files.TemplateManager.events.Download, ASC.Files.EventHandler.onGetTasksStatuses);
            serviceManager.bind(ASC.Files.TemplateManager.events.TerminateTasks, ASC.Files.EventHandler.onGetTasksStatuses);
            serviceManager.bind(ASC.Files.TemplateManager.events.GetTasksStatuses, ASC.Files.EventHandler.onGetTasksStatuses);
        }
    };

    /* Events */

    var onValidationAnchor = function(anchor) {
        if (ASC.Files.Constants.anchorRegExp.error.test(anchor)
            || ASC.Files.Constants.anchorRegExp.folder.test(anchor)
                || ASC.Files.Constants.anchorRegExp.preview.test(anchor)
                    || ASC.Files.Constants.anchorRegExp.imprt.test(anchor))
            return;

        ASC.Files.Folders.defaultFolderSet();
    };

    var onError = function(errorString) {
        ASC.Files.UI.displayInfoPanel(decodeURIComponent(errorString).replace( /\+/g , " "), true);
        if (jq.browser.msie) {
            setTimeout("ASC.Files.Folders.defaultFolderSet();", 3000);
        } else {
            ASC.Files.Folders.defaultFolderSet();
        }
    };

    var onSelectAnchor = function(itemid) {
        jq("#files_trewViewContainer li.jstree-open, #files_trewViewSelector li.jstree-open")
            .addClass("jstree-closed").removeClass("jstree-open");

        if (ASC.Files.Common.isCorrectId(itemid))
            ASC.Files.Folders.currentFolderId = itemid;
        else
            ASC.Files.Folders.currentFolderId = "";

        ASC.Files.Actions.hideAllActionPanels();

        ASC.Files.UI.updateFolderView();
    };

    var onPreviewAnchor = function(fileId) {
        if (typeof ASC.Files.ImageViewer != "undefined")
            ASC.Files.ImageViewer.init(fileId);
        else
            ASC.Files.Folders.defaultFolderSet();
    };

    var onImportAnchor = function(source) {
        source = (source || "").toLowerCase();
        if (ASC.Files.Import) {
            ASC.Files.Folders.eventAfter = ASC.Files.Import.selectEventBySource(source);
        }

        ASC.Files.Folders.defaultFolderSet();
    };

    var onGetFolderItems = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            ASC.Files.Folders.defaultFolderSet();
            return undefined;
        }

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderItems);

        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        ASC.Files.UI.resetSelectAll(false);

        ASC.Files.EmptyScreen.hideEmptyScreen();

        if (params.append == true)
            jq("#files_mainContent").append(htmlXML);
        else
            jq("#files_mainContent").html(htmlXML);

        //remove duplicate row
        jq("#files_mainContent li.fileRow[name='addRow']").each(function() {
            ASC.Files.UI.getObjectData(this).entryObject.filter("[name!='addRow']").remove();
        });

        var countTotal = 0;
        if (htmlXML != "") {
            countTotal = xmlData.getElementsByTagName("total")[0];
            countTotal = parseInt(countTotal.text || countTotal.textContent) || 0;
        }

        ASC.Files.UI.countEntityInFolder = countTotal;
        var countShowOnPage = parseInt(ASC.Files.Constants.COUNT_ON_PAGE) || 0;
        ASC.Files.UI.amountPage = parseInt((countTotal / countShowOnPage).toFixed(0));

        if (ASC.Files.UI.amountPage - (countTotal / countShowOnPage) < 0)
            ASC.Files.UI.amountPage++;

        ASC.Files.UI.currentPage = parseInt((jq("#files_mainContent li.fileRow[name!='addRow']").length - 1) / countShowOnPage) + 1;
        var countLeft = countTotal - jq("#files_mainContent li.fileRow").length;
        if (ASC.Files.UI.currentPage < ASC.Files.UI.amountPage && countLeft > 0) {
            jq("#files_pageNavigatorHolder").show();
            jq("#files_pageNavigatorHolder a")
                .text(countShowOnPage < countLeft ?
                        ASC.Files.FilesJSResources.ButtonShowMoreOf.format(countShowOnPage, countLeft) :
                        ASC.Files.FilesJSResources.ButtonShowMore.format(countLeft));
        } else {
            jq("#files_pageNavigatorHolder").hide();
        }

        if (!ASC.Files.Common.isCorrectId(ASC.Files.Folders.currentFolderId)) {
            ASC.Files.Folders.defaultFolderSet();
            return;
        }

        if (typeof xmlData != "undefined") {
            if (onGetFolderInfo(xmlData.getElementsByTagName("folder_info")[0], { id: ASC.Files.Folders.currentFolderId }))
                return;

            if (onGetPathParts(xmlData.getElementsByTagName("path_parts")[0], { id: ASC.Files.Folders.currentFolderId }))
                return;

            if (ASC.Files.Share && xmlData.getElementsByTagName("count_new").length != 0) {
                var valueNew = xmlData.getElementsByTagName("count_new")[0];
                valueNew = valueNew.childNodes[0].textContent || valueNew.childNodes[0].text;
                ASC.Files.Share.displayCountNew(valueNew, ASC.Files.Constants.FOLDER_ID_SHARE);
            }
        }

        if (htmlXML == "")
            ASC.Files.EmptyScreen.displayEmptyScreen();
        else
            ASC.Files.UI.addRowHandlers();

        ASC.Files.UI.checkEditing();

        if (ASC.Files.Folders.eventAfter != null) {
            ASC.Files.Folders.eventAfter();
            ASC.Files.Folders.eventAfter = null;
        }

        if (jq.browser.mobile) {
            setTimeout("ASC.Files.UI.toggleMainMenu();", 300);
        }
    };

    var onGetFolderInfo = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return false;
        }

        if (xmlData == null) {
            ASC.Files.Folders.defaultFolderSet();
            return true;
        }

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderInfo);
        var htmlData = ASC.Controls.XSLTManager.translate(xmlData, xslData);
        jq("#currentFolderInfo").html(htmlData);
        jq("#currentFolderInfo").attr("title", jq("#currentFolderInfo").text());

        if (ASC.Files.Share) {
            var curAccess = parseInt(jq("#access_current_folder").val());
            if (curAccess == ASC.Files.Constants.AceStatusEnum.Restrict) {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.AceStatusEnum_Restrict, true);
                ASC.Files.Folders.defaultFolderSet();
                return true;
            }
        }

        if (!ASC.Files.Common.isCorrectId(ASC.Files.Folders.currentFolderId))
            ASC.Files.Folders.defaultFolderSet();

        if (ASC.Files.Folders.currentFolderId == ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT)
            jq("#currentFolderTitle").val(ASC.Files.FilesJSResources.ProjectFiles);

        document.title = jq("#currentFolderTitle").val() + " - " + ASC.Files.Constants.TITLE_PAGE;
        return false;
    };

    var onGetPathParts = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return false;
        }
        if (typeof xmlData == "undefined" || xmlData == null)
            return false;

        var data = new Array();
        xmlData = xmlData.childNodes;
        var i;
        for (i = 0; i < xmlData.length; i++) {
            data.push(
                {
                    Key: xmlData[i].childNodes[0].textContent || xmlData[i].childNodes[0].text,
                    Value: xmlData[i].childNodes[1].textContent || xmlData[i].childNodes[1].text
                });
        }

        var visibleItems = [];

        var maxLength = 65;
        var maxLengthItem;
        var id = 0;
        var content;
        var title;

        var html = "";
        if (ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT != "") {
            var outside = -1;
            jq(data).each(function(j) {
                if (this.Key == ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT)
                    outside = j;
            });

            if (outside == -1) {
                ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT);
                return true;
            }
            ;

            data.splice(0, outside);
            data[0].Value = ASC.Files.FilesJSResources.ProjectFiles;
        }
        if (data.length > 4) {
            maxLengthItem = maxLength / 3;

            visibleItems[0] = {
                id: data[0].Key,
                title: data[0].Value,
                content: data[0].Value.substring(0, maxLengthItem)
            };

            visibleItems[1] = {
                id: data[data.length - 3].Key,
                content: "...",
                title: ""
            };

            jq(data.slice(1, data.length - 2)).each(
                function(index, obj) {
                    if (index == data.length - 4) {
                        visibleItems[1].title += obj.Value;
                        visibleItems[1].id = obj.Key;
                    } else {
                        visibleItems[1].title += obj.Value + " > ";
                    }
                }
            );

            visibleItems[2] = {
                id: data[data.length - 2].Key,
                title: data[data.length - 2].Value,
                content: data[data.length - 2].Value.substring(0, maxLengthItem) + (data[data.length - 2].Value.length > maxLengthItem ? "..." : "")
            };

            visibleItems[3] = {
                id: data[data.length - 1].Key,
                title: data[data.length - 1].Value,
                content: data[data.length - 1].Value.substring(0, maxLengthItem) + (data[data.length - 1].Value.length > maxLengthItem ? "..." : "")
            };
        } else {
            maxLengthItem = (maxLength / data.length).toFixed();

            for (i = 0; i < data.length; i++) {
                id = data[i].Key;
                content = data[i].Value;
                title = content;
                content = content.substring(0, maxLengthItem) + (content.length > maxLengthItem ? "..." : "");

                visibleItems[i] = { id: id, content: content, title: title };
            }
        }

        ASC.Files.Tree.pathParts = data;

        jq("#files_breadCrumbsCaption")
            .html(visibleItems[0].title)
            .attr("title", visibleItems[0].title)
            .attr("href", "#" + encodeURIComponent(visibleItems[0].id));

        for (i = 1; i < visibleItems.length; i++) {
            id = visibleItems[i].id;
            content = visibleItems[i].content;
            title = visibleItems[i].title;
            if (i != visibleItems.length - 1)
                //todo: remove onclick
                html += "<span>&gt;</span><a href='#{0}' data-id='{0}' onclick='ASC.Files.Folders.navigationSet(\"{0}\");return false;' title='{1}' >{2}</a>".format(id, title, content);
            else
                html += "<span>&gt;</span><span class='breadCrumbs_cur' title='{0}'>{1}</span>".format(title, content);
        }

        jq("#files_breadCrumbs").html(html);

        if (data.length == 0)
            return false;

        var container = data[0].Key;

        jq("#mainContentHeader .menuAction").removeClass("unlockAction").hide();

        jq("#files_breadCrumbsRoot, #files_mainContent")
            .removeClass("myFiles")
            .removeClass("corporateFiles")
            .removeClass("shareformeFiles")
            .removeClass("trashFiles")
            .removeClass("projectFiles");

        switch (container) {
        case ASC.Files.Constants.FOLDER_ID_MY_FILES:
            ASC.Files.Folders.folderContainer = "my";
            jq("#files_breadCrumbsRoot, #files_mainContent").addClass("myFiles");
            jq("#files_mainDownload, #files_mainMove, #files_mainCopy, #files_mainDelete").show();
            break;
        case ASC.Files.Constants.FOLDER_ID_COMMON_FILES:
            ASC.Files.Folders.folderContainer = "corporate";
            jq("#files_breadCrumbsRoot, #files_mainContent").addClass("corporateFiles");
            jq("#files_mainDownload, #files_mainMove, #files_mainCopy, #files_mainDelete").show();
            break;
        case ASC.Files.Constants.FOLDER_ID_SHARE:
            ASC.Files.Folders.folderContainer = "forme";
            jq("#files_breadCrumbsRoot, #files_mainContent").addClass("shareformeFiles");
            jq("#files_mainDownload, #files_mainCopy, #files_mainMarkRead, #files_mainUnsubscribe").show();
            break;
        case ASC.Files.Constants.FOLDER_ID_TRASH:
            ASC.Files.Folders.folderContainer = "trash";
            jq("#files_breadCrumbsRoot, #files_mainContent").addClass("trashFiles");
            jq("#files_mainDownload, #files_mainRestore, #files_mainDelete, #files_mainEmptyTrash").show();
            jq("#files_mainEmptyTrash").addClass("unlockAction");
            break;
        case ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT:
            ASC.Files.Folders.folderContainer = "project";
            jq("#files_breadCrumbsRoot, #files_mainContent").addClass("projectFiles");
            jq("#files_mainDownload, #files_mainMove, #files_mainCopy, #files_mainDelete").show();
            break;
        default:
            if (!ASC.Files.Common.isCorrectId(ASC.Files.Folders.currentFolderId)) {
                ASC.Files.Folders.defaultFolderSet();
                return true;
            }
        }

        ASC.Files.Filter.disableFilter();

        if (ASC.Files.Share
            && (container != ASC.Files.Constants.FOLDER_ID_COMMON_FILES || ASC.Files.Constants.USER_ADMIN)
                && container != ASC.Files.Constants.FOLDER_ID_TRASH
                    && container != ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT
                        && ASC.Files.UI.accessAdmin()
                            && jq("#folder_shareable").val() != "false") {
            jq("#files_mainContent").addClass("withShare");
        } else {
            jq("#files_mainContent").removeClass("withShare");
        }

        return false;
    };

    var onCreateNewFile = function(xmlData, params, errorMessage, commentMessage) {
        var fileNewObj = ASC.Files.UI.getEntryObject("file", 1).filter("[spare_data='NEW_FILE']");

        ASC.Files.UI.blockObject(fileNewObj);
        if (typeof errorMessage != "undefined") {
            fileNewObj.remove();
            if (jq("#files_mainContent li.fileRow").length == 0) {
                ASC.Files.EmptyScreen.displayEmptyScreen();
            }
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderItem);
        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        fileNewObj.replaceWith(htmlXML);
        ASC.Files.UI.resetSelectAll(false);

        //TODO: get Object?
        var fileObj = jq("#files_mainContent li.newFile").show().yellowFade().removeClass("newFile");
        var fileData = ASC.Files.UI.getObjectData(fileObj);
        fileObj = fileData.entryObject;
        var fileTitle = fileData.title;
        var fileId = fileData.entryId;

        ASC.Files.UI.addRowHandlers(fileObj);

        ASC.Files.UI.updateFolderInfo(0, 1);

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoCrateFile.format(fileTitle));

        fileObj.addClass("isNewForWebEditor");
        if (!jq.browser.mozilla) {
            ASC.Files.Actions.checkEditFile(fileId, fileTitle, true);
        }
    };

    var onCreateFolder = function(xmlData, params, errorMessage, commentMessage) {
        var folderNewObj = ASC.Files.UI.getEntryObject("folder", 1).filter("[spare_data='NEW_FOLDER']");

        if (typeof errorMessage != "undefined") {
            folderNewObj.remove();
            if (jq("#files_mainContent li.fileRow").length == 0) {
                ASC.Files.EmptyScreen.displayEmptyScreen();
            }

            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderItem);

        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        folderNewObj.replaceWith(htmlXML);
        ASC.Files.UI.resetSelectAll(false);

        //TODO: get Object
        var folderObj = jq("#files_mainContent li.newFolder").yellowFade().removeClass("newFolder");
        var folderData = ASC.Files.UI.getObjectData(folderObj);
        folderObj = folderData.entryObject;
        var folderTitle = folderData.title;

        ASC.Files.UI.addRowHandlers(folderObj);

        ASC.Files.UI.updateFolderInfo(1, 0);

        ASC.Files.Tree.resetNode(ASC.Files.Folders.currentFolderId);

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoCrateFolder.format(folderTitle));
    };

    var onRenameFolder = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderItem);
        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        var folderId = params.folderId;
        var folderObj = ASC.Files.UI.getEntryObject("folder", folderId);

        folderObj.replaceWith(htmlXML);

        folderObj = ASC.Files.UI.getEntryObject("folder", folderId);
        var itemNewData = ASC.Files.UI.getObjectData(folderObj);
        
        itemNewData = itemNewData || ASC.Files.UI.getObjectData("#files_mainContent li.newFolder");
        folderObj = itemNewData.entryObject;

        var folderNewTitle = itemNewData.title;
        folderObj.yellowFade().removeClass("newFolder");

        ASC.Files.UI.addRowHandlers(folderObj);

        ASC.Files.Tree.resetNode(ASC.Files.Folders.currentFolderId);

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRenameFolder.format(params.name, folderNewTitle));
    };

    var onRenameFile = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var fileData = ASC.Files.EventHandler.onReplaceVersion(xmlData, params, errorMessage, commentMessage);
        var newName = fileData.title;

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRenameFile.format(params.name, newName));
    };

    var onSetCurrentVersion = function(xmlData, params, errorMessage, commentMessage) {
        var fileId = params.fileId;
        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.blockObject(fileObj);
            jq("div.version_operation").css("visibility", "");
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        if (!ASC.Files.Common.isCorrectId(fileId))
            return;

        var fileData = ASC.Files.UI.getObjectData(fileObj);
        var version = parseInt(fileData.version || 0);

        var modifiedOn = fileData.modified_on;
        var contentLength = fileData.content_length;
        var modifiedByName = fileData.modified_by;

        var xmlRowString = "\
<fileList withoutheader='true'>\
    <entry>\
        <id>" + fileId + "</id>\
        <modified_by>" + modifiedByName + "</modified_by>\
        <modified_on>" + modifiedOn + "</modified_on>\
        <content_length>" + contentLength + "</content_length>\
        <version>" + version + "</version>\
    </entry>\
</fileList>";

        var xslDataRow = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFileHistory);
        var lastVersion = ASC.Controls.XSLTManager.translateFromString(xmlRowString, xslDataRow);

        jq(lastVersion).insertAfter("#content_versions span.versionsTitle");

        jq("div.version_num a:first").attr("href", ASC.Files.Utility.GetFileViewUrl(fileId, version));

        jq("#content_versions div.versionRow.even").removeClass("even");
        jq("#content_versions div.versionRow:even").addClass("even");

        try {
            var content_versions = jq("#content_versions").clone(true);
        } catch(ex) {
            ASC.Files.Folders.showVersions(fileObj, fileId);
            return false;
        }

        var curTemplate = ASC.Files.TemplateManager.templates.getFolderItem;
        var xslData = ASC.Files.TemplateManager.getTemplate(curTemplate);
        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        var newFileObj = ASC.Files.UI.getEntryObject("file", fileId);

        newFileObj.replaceWith(htmlXML);
        newFileObj = ASC.Files.UI.getEntryObject("file", fileId);

        var newFileData = ASC.Files.UI.getObjectData(newFileObj);
        var fileTitle = newFileData.title;

        jq("#files_mainContent li.newFile").removeClass("newFile").show();

        newFileObj.append(content_versions);

        ASC.Files.UI.addRowHandlers(newFileObj);

        ASC.Files.UI.blockObject(newFileObj);
        jq("div.version_operation").css("visibility", "");

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.FileVersionRecovery.format(fileTitle), false);
    };

    var onGetFileHistory = function(xmlData, params, errorMessage, commentMessage) {
        var fileId = params.fileId;
        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);

        ASC.Files.UI.blockObject(fileObj);
        ASC.Files.UI.selectRow(fileObj, true);
        ASC.Files.UI.updateMainContentHeader();

        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFileHistory);
        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        jq("#content_versions").remove();
        fileObj.append('<div id="content_versions" class="notPreview"><span class="clearFix versionsTitle">{0}</span></div>'.format(ASC.Files.FilesJSResources.TitleVersions));

        jq("#content_versions").append(htmlXML);

        if (!ASC.Files.UI.accessibleItem(fileObj)
            || ASC.Files.UI.editingFile(fileObj)) {
            jq("div.version_operation a.version_restore").remove();
        }

        var titleFile = ASC.Files.UI.getObjectTitle(fileObj);
        if (ASC.Files.Utility.CanWebView(titleFile)) {
            jq("#content_versions.notPreview").removeClass("notPreview");
        }
    };

    var onReplaceVersion = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        var fileId = params.fileId;
        if (!ASC.Files.Common.isCorrectId(fileId))
            return;

        var curTemplate = ASC.Files.TemplateManager.templates.getFolderItem;
        var xslData = ASC.Files.TemplateManager.getTemplate(curTemplate);
        var htmlXML =
            (params.isStringXml === true
                ? ASC.Controls.XSLTManager.translateFromString(xmlData, xslData)
                : ASC.Controls.XSLTManager.translate(xmlData, xslData));

        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        fileObj.replaceWith(htmlXML);

        fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        var itemNewData = ASC.Files.UI.getObjectData(fileObj);
        itemNewData = itemNewData || ASC.Files.UI.getObjectData("#files_mainContent li.newFile");
        fileObj = itemNewData.entryObject;
        if (params.show) {
            ASC.Files.EmptyScreen.hideEmptyScreen();
            itemNewData.entryObject.removeClass("newFile").show().yellowFade();
        }

        ASC.Files.UI.addRowHandlers(fileObj);

        ASC.Files.UI.resetSelectAll(false);

        ASC.Files.UI.checkEditing();

        ASC.Files.Actions.showActionsViewPanel();
        return itemNewData;
    };

    var onGetFile = function(xmlData, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        var fileId = params.fileId;
        if (!ASC.Files.Common.isCorrectId(fileId))
            return;

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFolderItem);
        var htmlXML =
            (params.isStringXml === true
                ? ASC.Controls.XSLTManager.translateFromString(xmlData, xslData)
                : ASC.Controls.XSLTManager.translate(xmlData, xslData));

        jq("#files_mainContent").prepend(htmlXML);

        ASC.Files.UI.updateFolderInfo(0, 1);

        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);

        var fileData = ASC.Files.UI.getObjectData(fileObj);
        fileData = fileData || ASC.Files.UI.getObjectData("#files_mainContent li.newFile");
        fileObj = fileData.entryObject;
        if (params.show) {
            ASC.Files.EmptyScreen.hideEmptyScreen();
            fileData.entryObject.removeClass("newFile").show().yellowFade();
        }

        ASC.Files.UI.addRowHandlers(fileObj);

        ASC.Files.UI.resetSelectAll(false);

        ASC.Files.UI.checkEditing();

        ASC.Files.Actions.showActionsViewPanel();
        return fileData;
    };

    var onMoveFilesCheck = function(data, params, errorMessage, commentMessage) {
        if (typeof errorMessage != "undefined") {
            jq(params.list.entry).each(function() {
                var curItem = ASC.Files.UI.parseItemId(this);
                ASC.Files.UI.blockObjectById(curItem.entryType, curItem.entryId);
            });
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }
        data = eval(data);

        if (data != null && data.length > 0) {
            ASC.Files.UI.showOverwriteMessage(params.list, params.folderToId, params.folderToTitle, params.isCopyOperation, data);
        } else {
            serviceManager.moveItems(ASC.Files.TemplateManager.events.MoveItems,
                {
                    folderToId: params.folderToId,
                    overwrite: false,
                    isCopyOperation: params.isCopyOperation,
                    doNow: true
                },
                { stringList: params.list });
            ASC.Files.Folders.isCopyTo = false;
        }
    };

    var onMoveItemsFinish = function(listData, isCopyOperation, countProcessed) {
        var folderToId = ASC.Files.UI.parseItemId(listData[0]).entryId;
        listData = listData.slice(1);
        var listItemId = new Array();
        for (var i = 0; i < listData.length; i++) {
            var curItem = ASC.Files.UI.parseItemId(decodeURIComponent(listData[i]));
            if (curItem == null)
                continue;
            listItemId.push(curItem);
            ASC.Files.UI.blockObjectById(curItem.entryType, curItem.entryId);
        }

        var folderToObj = ASC.Files.UI.getEntryObject("folder", folderToId);
        folderToObj.removeClass("row-to");

        var foldersCount = 0, filesCount = 0;
        var entryTitle = "";

        if (listItemId.length == 1) {
            entryTitle = ASC.Files.UI.getEntryTitle(listItemId[0].entryType, listItemId[0].entryId);
            if (typeof entryTitle == undefined || entryTitle == null)
                entryTitle = "";
        }

        for (var i = 0; i < listItemId.length; i++) {
            var entryRowObj = ASC.Files.UI.getEntryObject(listItemId[i].entryType, listItemId[i].entryId);

            if (listItemId[i].entryType == "file") {
                filesCount++;
            } else {
                foldersCount += 1 + (parseInt(entryRowObj.find(".countFolders").html()) || 0);
                filesCount += parseInt(entryRowObj.find(".countFiles").html()) || 0;
            }

            if (!isCopyOperation && ASC.Files.Folders.currentFolderId != folderToId)
                entryRowObj.remove();
        }

        if (foldersCount > 0) {
            var folderCountObj = folderToObj.find(".countFolders");

            folderCountObj.html((parseInt(folderCountObj.html()) || 0) + foldersCount);

            ASC.Files.Tree.resetNode(folderToId);
            if (!isCopyOperation && folderToId != ASC.Files.Folders.currentFolderId) {
                ASC.Files.Tree.resetNode(ASC.Files.Folders.currentFolderId);
            }
        }

        if (filesCount > 0) {
            var fileCountObj = folderToObj.find(".countFiles");

            fileCountObj.html((parseInt(fileCountObj.html()) || 0) + filesCount);
        }

        if (listItemId.length > 0) {
            ASC.Files.Folders.getFolderInfo();
        }

        if (listItemId.length > 0 && ASC.Files.Folders.currentFolderId != folderToId) {
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

        if (isCopyOperation) {
            if (listItemId.length == 1 && entryTitle != "") {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoCopyItem.format(entryTitle));
            } else {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoCopyGroup.format(countProcessed));
            }
        } else {
            if (listItemId.length == 1 && entryTitle != "") {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoMoveItem.format(entryTitle));
            } else {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoMoveGroup.format(countProcessed));
            }
        }
    };

    var onDeleteItemFinish = function(listData, totalCount) {
        var fromRootId = ASC.Files.UI.parseItemId(listData[0]).entryId;
        listData = listData.slice(1);
        var listItemId = new Array();
        for (var i = 0; i < listData.length; i++) {
            var curItem = ASC.Files.UI.parseItemId(decodeURIComponent(listData[i]));
            if (curItem == null)
                continue;
            listItemId.push(curItem);
            ASC.Files.UI.blockObjectById(curItem.entryType, curItem.entryId);
        }

        var foldersCount = 0;
        var countSubFolder = 0, countSubFile = 0;
        var entryTitle = "";
        var redrawItems =
            (ASC.Files.Tree.pathParts.length > 0
                && (ASC.Files.Tree.pathParts[0].Key === fromRootId || (ASC.Files.ThirdParty && ASC.Files.ThirdParty.isThirdParty())));

        if (listItemId.length == 1) {
            entryTitle = ASC.Files.UI.getEntryTitle(listItemId[0].entryType, listItemId[0].entryId);
            if (typeof entryTitle == undefined || entryTitle == null)
                entryTitle = "";
        }

        for (var i = 0; i < listItemId.length; i++) {
            var entryRowObj = ASC.Files.UI.getEntryObject(listItemId[i].entryType, listItemId[i].entryId);

            if (listItemId[i].entryType == "file") {
                countSubFile++;
            } else {
                foldersCount++;
                countSubFolder += 1 + (parseInt(entryRowObj.find(".countFolders").html()) || 0);
                countSubFile += parseInt(entryRowObj.find(".countFiles").html()) || 0;
            }

            if (redrawItems)
                entryRowObj.remove();
        }

        if (foldersCount > 0) {
            ASC.Files.Tree.resetNode(ASC.Files.Folders.currentFolderId);
        }

        if (listItemId.length > 0 && redrawItems) {
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

        if (listItemId.length == 1 && entryTitle != "") {
            if (foldersCount > 0)
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRemoveFolder.format(entryTitle));
            else
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRemoveFile.format(entryTitle));
        } else {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRemoveGroup.format(listItemId.length, totalCount));
        }

        if (fromRootId == ASC.Files.Constants.FOLDER_ID_TRASH && ASC.Files.Constants.MAX_UPLOAD_KB < 1024 * 1024) {
            //TODO: request get max upload kb
            window.location.reload();
        }
    };

    var onCheckEditing = function(jsonData, params, errorMessage, commentMessage) {
        clearTimeout(ASC.Files.UI.timeCheckEditing);
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        if (!jsonData)
            jsonData = [];

        var list = jq("#files_mainContent li.fileRow.onEdit");

        for (var i = 0; i < list.length; i++) {
            var fileData = ASC.Files.UI.getObjectData(list[i]);
            var fileObj = fileData.entryObject;
            var fileId = fileData.entryId;
            ASC.Files.UI.lockEditFile(fileObj, false);

            var repl = true;
            for (var j = 0; j < jsonData.length && repl; j++) {
                if (fileId == jsonData[j].Key)
                    repl = false;
            }

            if (repl)
                ASC.Files.Folders.replaceVersion(fileId, true);
        }

        for (var k = 0; k < jsonData.length; k++)
            ASC.Files.UI.lockEditFileById(jsonData[k].Key, true, jsonData[k].Value);

        if (jsonData.length > 0)
            ASC.Files.UI.timeCheckEditing = setTimeout(function() { ASC.Files.UI.checkEditing(); }, 5000);
    };

    var onGetTasksStatuses = function(data, params, errorMessage, commentMessage) {
        if (typeof data !== "object" && typeof errorMessage != "undefined" || data == null) {
            ASC.Files.Folders.cancelTasksStatuses();
            if (ASC.Files.Import) ASC.Files.Import.cancelImportData("");
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return;
        }

        if (data.length == 0) {
            ASC.Files.Folders.cancelTasksStatuses();
            if (ASC.Files.Import) ASC.Files.Import.cancelImportData("");
            return;
        }

        var finishImport = true;
        var progress = 0;
        var operationType;
        var operationTypes = [ASC.Files.FilesJSResources.TasksOperationMove,
            ASC.Files.FilesJSResources.TasksOperationCopy,
            ASC.Files.FilesJSResources.TasksOperationDelete,
            ASC.Files.FilesJSResources.TasksOperationBulkdownload];
        var blockTypes = [ASC.Files.FilesJSResources.DescriptMove,
            ASC.Files.FilesJSResources.DescriptCopy,
            ASC.Files.FilesJSResources.DescriptRemove,
            ASC.Files.FilesJSResources.DescriptBulkdownload];

        //import
        for (var i = 0; i < data.length && ASC.Files.Import; i++) {
            if (data[i].operation == 4) {
                finishImport = ASC.Files.Import.onGetImportStatus(data.splice(i, 1)[0], params.isTerminate);
                break;
            }
        }

        if (data.length != 0) {
            //show
            if (jq("#files_tasks:visible").length == 0) {
                clearTimeout(timoutTasksStatuses);

                if (jq("#files_tasks").length == 0) {
                    jq("#files_progress_template").clone().attr("id", "files_tasks").appendTo("#files_bottom_loader");
                    jq("#files_tasks").prepend('<div title="{0}" class="terminateTasks"></div>'.format(ASC.Files.FilesJSResources.TitleCancel));
                }
                jq("#files_tasks .progress").css("width", "0%");
                jq("#files_tasks .percent").text("0%");
                jq("#files_tasks .headerBaseMedium").html("");
                jq("#files_tasks").show();

                if (jq.browser.mobile) {
                    jq("#files_bottom_loader").css("bottom", "auto");
                    jq("#files_bottom_loader").css("top", jq(window).scrollTop() + jq(window).height() - jq("#files_bottom_loader").height() + "px");
                }
            }

            //type operation in progress
            if (data.length > 1) {
                operationType = ASC.Files.FilesJSResources.TasksOperationMixed.format(data.length);
            } else {
                operationType = operationTypes[data[0].operation];
            }
            jq("#files_tasks .headerBaseMedium").html(operationType);
        }

        //in each process
        for (var i = 0; i < data.length; i++) {

            //block descript on each elemets
            var splitCharacter = ":";
            var listSource = data[i].source.trim().split(splitCharacter);
            jq(listSource).each(function() {
                var itemId = ASC.Files.UI.parseItemId(decodeURIComponent(this));
                if (itemId == null) return true;
                ASC.Files.UI.blockObjectById(itemId.entryType, itemId.entryId, true, blockTypes[data[i].operation]);
            });

            var curProgress = data[i].progress;
            progress += curProgress;

            //finish
            if (curProgress == 100) {
                if (data[i].result != null) {
                    var listResult = data[i].result.trim().split(splitCharacter);

                    switch (data[i].operation) {
                    case 0:
                            //move
                        onMoveItemsFinish(listResult, false, data[i].processed);
                        break;
                    case 1:
                            //copy
                        onMoveItemsFinish(listResult, true, data[i].processed);
                        break;
                    case 2:
                            //delete
                        onDeleteItemFinish(listResult, listSource.length);
                        break;
                    case 3:
                            //download
                        if (listResult[0])
                            location.href = listResult[0];
                        ASC.Files.Folders.bulkStatuses = false;
                        break;
                    }
                }
                //unblock
                jq(listSource).each(function() {
                    var itemId = ASC.Files.UI.parseItemId(this);
                    if (itemId == null) return true;
                    ASC.Files.UI.blockObjectById(itemId.entryType, itemId.entryId);
                });

                //on error
                if (data[i].error != null) {
                    ASC.Files.UI.displayInfoPanel(data[i].error, true);
                }
            }
        }

        //progress %
        progress = (data.length == 0 ? 100 : progress / data.length);

        jq("#files_tasks .progress").css("width", progress + "%");
        jq("#files_tasks .percent").text(progress + "%");

        //complate
        if (progress == 100) {
            clearTimeout(timoutTasksStatuses);
            timoutTasksStatuses = setTimeout("ASC.Files.Folders.cancelTasksStatuses()", 500);

            if (finishImport)
                return;
        }

        //next iteration
        ASC.Files.Folders.getTasksStatuses(params.doNow);
    };

    return {
        init: init,
        onError: onError,
        onValidationAnchor: onValidationAnchor,
        onSelectAnchor: onSelectAnchor,
        onPreviewAnchor: onPreviewAnchor,
        onImportAnchor: onImportAnchor,

        onGetFolderItems: onGetFolderItems,
        onGetFolderInfo: onGetFolderInfo,
        onGetFile: onGetFile,
        onCreateNewFile: onCreateNewFile,
        onCreateFolder: onCreateFolder,
        onRenameFolder: onRenameFolder,
        onRenameFile: onRenameFile,
        onSetCurrentVersion: onSetCurrentVersion,
        onGetFileHistory: onGetFileHistory,
        onReplaceVersion: onReplaceVersion,
        onCheckEditing: onCheckEditing,

        onMoveFilesCheck: onMoveFilesCheck,
        onGetTasksStatuses: onGetTasksStatuses
    };
})();

(function($) {
    ASC.Files.EventHandler.init();

    $(function() {
        if (!startTasksStatuses) startTasksStatuses = [];
        ASC.Files.EventHandler.onGetTasksStatuses(startTasksStatuses, { doNow: true });
        startTasksStatuses = null;

        jq("#files_bottom_loader").on("click", "#files_tasks .terminateTasks", function() {
            ASC.Files.Folders.terminateTasks(false);
            return false;
        });
    });
})(jQuery);