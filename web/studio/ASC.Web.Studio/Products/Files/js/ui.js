window.ASC.Files.UI = (function() {
    var isInit = false;

    var timeIntervalInfo = null;
    var timeCheckEditing = null;
    var timeTooltip = null;

    var currentPage = 0;
    var amountPage = 0;

    var countEntityInFolder = 0;

    var currentRowOver = null;

    var startX = 0;
    var startY = 0;
    var prevX = 0;
    var prevY = 0;

    var mouseBtn = false;

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }

        ASC.Files.UI.mouseBtn = false;
    };

    var getEntryObject = function(entryType, entryId) {
        if (entryType && entryId) {
            if (!ASC.Files.Common.isCorrectId(entryId))
                return null;
            return jq("#files_mainContent li.fileRow[data-id='" + entryType + "_" + entryId + "']");
        }
    };

    var getObjectData = function(entryObject) {
        entryObject = jq(entryObject);
        if (!entryObject.is("li.fileRow"))
            entryObject = entryObject.closest("li.fileRow");
        if (entryObject.length == 0)
            return null;

        var entryDataStr = entryObject.find("input:hidden[name='entry_data']").val();
        var resulat = eval(entryDataStr);
        if (!ASC.Files.Common.isCorrectId(resulat.id))
            return null;

        resulat.entryId = resulat.id;
        resulat.entryType = (resulat.entryType === "file" ? "file" : "folder");
        resulat.create_by = entryObject.find("input:hidden[name='create_by']").val();
        resulat.modified_by = entryObject.find("input:hidden[name='modified_by']").val();
        resulat.entryObject = entryObject;
        resulat.error = (resulat.error != "" ? resulat.error : false);
        resulat.title = resulat.title.trim();
        return resulat;
    };

    var parseItemId = function(itemId) {
        if (typeof itemId == "undefined")
            return null;

        var entryType = itemId.indexOf("file_") == "0" ? "file" : "folder";
        var entryId = itemId.substring((entryType + "_").length);

        if (!ASC.Files.Common.isCorrectId(entryId))
            return null;
        return { entryType: entryType, entryId: entryId };
    };

    var getEntryTitle = function(entryType, entryId) {
        if (!ASC.Files.Common.isCorrectId(entryId))
            return null;
        return ASC.Files.UI.getObjectTitle(ASC.Files.UI.getEntryObject(entryType, entryId));
    };

    var getObjectTitle = function(entryObject) {
        entryObject = jq(entryObject);
        if (!entryObject.is("li.fileRow"))
            entryObject = entryObject.closest("li.fileRow");
        if (entryObject.length == 0)
            return null;

        return ASC.Files.UI.getObjectData(entryObject).title.trim();
    };

    var updateFolderView = function() {
        if (!ASC.Files.Common.isCorrectId(ASC.Files.Folders.currentFolderId)) {
            ASC.Controls.AnchorController.move("");
            return;
        }

        ASC.Files.Folders.getFolderItems(false);
    };

    var switchFolderView = function() {
        if (jq("#files_mainContent").hasClass("compact")) {
            jq("#switchViewFolder").addClass("compact");
            jq("#files_mainContent li.fileRow").each(function() {
                var linkRow = jq(this).find(".entryTitle a.name");
                linkRow.removeAttr("title");
            });
        } else {
            jq("#switchViewFolder").removeClass("compact");
            jq("#files_mainContent li.fileRow").each(function() {
                var linkRow = jq(this).find(".entryTitle a.name");
                linkRow.attr("title", linkRow.text());
            });

            if (jq(document).height() - jq(window).height() <= jq(window).scrollTop() + 350) {
                ASC.Files.Folders.showMore();
            }
        }
    };

    var updateFolderInfo = function(diffCountFolder, diffCountFile) {
        var currentCountFolder = (parseInt(jq("#content_info_count_folders").html()) || 0) + diffCountFolder;
        var currentCountFile = (parseInt(jq("#content_info_count_files").html()) || 0) + diffCountFile;

        var regetCount = false;

        if (currentCountFolder < 0 || currentCountFile < 0) {
            regetCount = true;
        } else {
            if (currentCountFolder < jq("#files_mainContent li.folderRow").length)
                regetCount = true;
            if (regetCount || currentCountFile < jq("#files_mainContent li.fileRow:not(.folderRow)").length)
                regetCount = true;
        }

        if (regetCount) {
            ASC.Files.Folders.getFolderInfo();
            return;
        }

        jq("#content_info_count_folders").html(currentCountFolder);
        jq("#content_info_count_files").html(currentCountFile);

        jq("#currentFolderInfo").attr("title", jq("#currentFolderInfo").text());
    };

    var toggleMainMenu = function() {

        if (jq("#mainContentHeader:visible").length == 0) return;

        var mainContentHeaderSpacer = jq("#mainContentHeaderSpacer");
        var mainContentHeader = jq("#mainContentHeader");
        var boxTop = jq("#menuActionSelectOpen");
        var newTop = boxTop.offset().top + boxTop.outerHeight();
        var winScrollTop = jq(window).scrollTop();
        var tempTop = 0;

        if (mainContentHeaderSpacer.css("display") == "none")
            tempTop += mainContentHeader.offset().top;
        else
            tempTop += mainContentHeaderSpacer.offset().top;

        if (winScrollTop >= tempTop) {
            mainContentHeaderSpacer.show();
            mainContentHeader.addClass("fixed");

            jq("#files_selectorPanel").css({
                "position": "fixed",
                "top": newTop - winScrollTop
            });

            if (jq.browser.mobile) {
                mainContentHeader.css(
                    {
                        "top": jq(document).scrollTop() + "px",
                        "position": "absolute"
                    });
            }
        } else {
            mainContentHeaderSpacer.hide();
            mainContentHeader.removeClass("fixed");

            jq("#files_selectorPanel").css({
                "position": "absolute",
                "top": newTop
            });

            if (jq.browser.mobile) {
                mainContentHeader.css(
                    {
                        "position": "static"
                    });
            }
        }
    };

    var showOverwriteMessage = function(listData, folderId, folderToTitle, isCopyOperation, data) {
        var folderTitle = ASC.Files.UI.getEntryTitle("folder", folderId) || ASC.Files.Tree.getFolderTitle(folderId);

        var message;
        if (data.length > 1) {
            var files = "";
            for (var i = 0; i < data.length; i++) {
                files += "<li title='{0}'>{0}</li>".format(data[i].Value);
            }
            message = "<b>" + ASC.Files.FilesJSResources.FilesAlreadyExist.format(data.length, folderTitle) + "</b>";
            jq("#files_overwrite_list").html(files).show();
        } else {
            jq("#files_overwrite_list").hide();
            message = ASC.Files.FilesJSResources.FileAlreadyExist.format("<span title='" + data[0].Value + "'>" + data[0].Value + "</span>", "<b>" + folderTitle + "</b>");
        }

        jq("#files_overwrite_msg").html(message);

        jq("#files_overwrite_overwrite").unbind("click").click(function() {
            PopupKeyUpActionProvider.CloseDialogAction = "";
            PopupKeyUpActionProvider.CloseDialog();
            serviceManager.moveItems(ASC.Files.TemplateManager.events.MoveItems,
                {
                    folderToId: folderId,
                    overwrite: true,
                    isCopyOperation: (isCopyOperation == true),
                    doNow: true
                },
                { stringList: listData });
        });

        jq("#files_overwrite_skip").unbind("click").click(function() {

            for (var i = 0; i < data.length; i++) {
                ASC.Files.UI.blockObjectById("file", data[i].Key);
                var pos = jq.inArray("file_" + data[i].Key, listData.entry);
                if (pos != -1)
                    listData.entry.splice(pos, 1);
            }

            PopupKeyUpActionProvider.CloseDialogAction = "";
            PopupKeyUpActionProvider.CloseDialog();
            serviceManager.moveItems(ASC.Files.TemplateManager.events.MoveItems,
                {
                    folderToId: folderId,
                    overwrite: false,
                    isCopyOperation: (isCopyOperation == true),
                    doNow: true
                },
                { stringList: listData });
        });

        jq("#files_overwrite_cancel").unbind("click").click(function() {
            for (var i = 0; i < listData.entry.length; i++) {
                var itemId = ASC.Files.UI.parseItemId(listData.entry[i]);
                ASC.Files.UI.blockObjectById(itemId.entryType, itemId.entryId);
            }

            PopupKeyUpActionProvider.CloseDialogAction = "";
            PopupKeyUpActionProvider.CloseDialog();
        });

        ASC.Files.Common.blockUI(jq("#files_overwriteFiles"), 400, 300);

        PopupKeyUpActionProvider.EnterAction = 'jq("#files_overwrite_overwrite").click();';
        PopupKeyUpActionProvider.CloseDialogAction = 'jq("#files_overwrite_cancel").click();';
    };

    var blockObjectById = function(entryType, entryId, value, message) {
        return ASC.Files.UI.blockObject(ASC.Files.UI.getEntryObject(entryType, entryId), value, message);
    };

    var blockObject = function(entryObj, value, message) {
        entryObj = jq(entryObj);
        if (entryObj.hasClass("checkloading") && value)
            return;

        ASC.Files.UI.selectRow(entryObj, false);
        ASC.Files.UI.updateMainContentHeader();
        ASC.Files.UI.hideLinks(entryObj);

        if (value === true) {
            entryObj.addClass("loading checkloading");
            entryObj.block({ message: "", baseZ: 99 });
            if (typeof message != "undefined") {
                entryObj.children("div.blockUI.blockOverlay").attr("title", message);
            }
        } else {
            entryObj.removeClass("loading checkloading");
            entryObj.unblock();
            entryObj.css("position", "static");
        }
    };

    var hideLinks = function(entryObj) {
        jq(entryObj).removeClass("row-over");
    };

    var showLinks = function(entryObj) {
        if (jq("#files_selector").css("display") == "block"
            || jq(entryObj).hasClass("loading"))
            return;

        jq(entryObj).addClass("row-over");
    };

    var editingFile = function(entryObj) {
        return !entryObj.hasClass("folderRow") && entryObj.hasClass("onEdit");
    };

    var editableFile = function(fileData) {
        var fileObj = fileData.entryObject;
        var fileType = fileData.entryType;
        var title = fileData.title;

        return fileType == "file"
            && ASC.Files.Folders.folderContainer != "trash"
                && fileObj.is("li:not(.rowRename)")
                    && ASC.Files.UI.accessibleItem(fileObj)
                        && ASC.Files.Utility.CanWebEdit(title);
    };

    var paintRows = function() {
        jq("#files_mainContent li.fileRow:odd").removeClass("even");
        jq("#files_mainContent li.fileRow:even").addClass("even");
    };

    var addRowHandlers = function(entryObject) {
        var listEntry = (entryObject || jq("#files_mainContent li.fileRow"));

        if (listEntry.is(":visible"))
            ASC.Files.UI.paintRows();

        listEntry.each(function() {
            var entryData = ASC.Files.UI.getObjectData(this);

            var entryId = entryData.entryId;
            var entryType = entryData.entryType;
            var entryObj = entryData.entryObject;
            var entryTitle = entryData.title;

            if (ASC.Files.Folders.folderContainer == "trash") {
                entryObj.find("div.entryInfo span.titleCreated").remove();
                entryObj.find("div.entryInfo span.titleRemoved").css("display", "");
                if (entryType == "folder") {
                    entryObj.find("span.create_date").remove();
                    entryObj.find("span.modified_date").css("display", "");
                } else {
                    var fortrashCreateBy = entryObj.find("div.entryInfo span.fortrash_create_by");
                    if (fortrashCreateBy.length) {
                        entryObj.find("div.entryInfo span.create_by").remove();
                        fortrashCreateBy.show().removeClass("fortrash_create_by").addClass("create_by");
                    }
                }
            } else {
                entryObj.find("div.entryInfo span.titleRemoved").remove();
                entryObj.find("div.entryInfo span.fortrash_create_by").remove();
                if (entryType == "folder") {
                    entryObj.find("span.modified_date").remove();
                }
            }
            var rowLink = entryObj.find(".entryTitle a.name");

            var ftClass = (entryType == "file" ? ASC.Files.Utility.getCssClassByFileTitle(entryTitle) : ASC.Files.Utility.getFolderCssClass());
            entryObj.find(".thumb-" + entryType).addClass(ftClass);

            if (!entryObj.hasClass("checkloading")) {

                if (ASC.Files.Folders.folderContainer == "trash"
                    || entryData.error) {
                    rowLink.attr("href", "#" + encodeURIComponent(ASC.Files.Folders.currentFolderId));
                }

                if (entryType == "file") {
                    if (rowLink.children().length == 0) {
                        var fileExt = ASC.Files.Utility.GetFileExtension(entryTitle);
                        var entrySplitTitle = entryTitle.substring(0, entryTitle.length - fileExt.length);
                        rowLink.html(
                            "{0}<span>{1}</span>".format(
                                entrySplitTitle, fileExt));
                    }

                    if (ASC.Files.Folders.folderContainer != "trash") {
                        var entryUrl = ASC.Files.Utility.GetFileDownloadUrl(entryId);
                        if (ASC.Files.Utility.CanWebView(entryTitle)) {
                            entryUrl = ASC.Files.Utility.GetFileWebViewerUrl(entryId);
                            rowLink.attr("href", entryUrl).attr("target", "_blank");
                        } else {
                            if (typeof ASC.Files.ImageViewer != "undefined" && ASC.Files.Utility.CanImageView(entryTitle)) {
                                entryUrl = ASC.Files.ImageViewer.getPreviewUrl(entryId);
                                rowLink.attr("href", entryUrl);
                            } else {
                                rowLink.attr("href", jq.browser.mobile ? "" : entryUrl);
                            }
                        }
                    }

                    if (ASC.Files.UI.editableFile(entryData)) {
                        ASC.Files.UI.lockEditFile(entryObj, ASC.Files.UI.editingFile(entryObj));
                    } else {
                        entryObj.addClass("cannotEdit");
                        entryObj.find("div.pencil").remove();
                    }
                } else {
                    rowLink.attr("href", "#" + encodeURIComponent(entryId));
                }
            }
        });

        ASC.Files.UI.switchFolderView();
    };

    var checkTarget = function(target) {
        if (target.nodeName == "HTML")
            return true;

        if (jq("div.popupModal:visible").length != 0)
            return true;

        if (jq.inArray(jq("div.mainContainerClass")[0], jq(target).parents()) == -1)
            return true;

        if (jq("#imageViewerToolbox:visible, #imageViewerContainer:visible").length != 0)
            return true;

        if (jq(target).is("li.fileRow, li.fileRow *")
            || jq(target).is("div.files_action_panel, div.files_action_panel *")
                || jq(target).is("#menuActionSelectOpen, #files_actions_open *")
                    || jq(target).is("div.treeViewPanel, div.treeViewPanel *")
                        || jq(target).is("ul#mainMenuHolder, ul#mainMenuHolder *")
                            || jq(target).is(".files_progress_box, #files_pageNavigatorHolder a")
                                || jq(target).is("#files_advansedFilter *"))
            return true;

        return false;
    };

    var beginSelecting = function(e) {

        e = ASC.Files.Common.fixEvent(e);

        if (!(e.button == 0 || (jq.browser.msie && e.button == 1)))
            return true;
        if (jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile)").length == 0)
            return true;

        var target = e.target || e.srcElement;

        try {
            if (checkTarget(target))
                return true;
        } catch (e) {
            return true;
        }

        var posX = e.pageX;
        var posY = e.pageY;

        ASC.Files.UI.startX = posX;
        ASC.Files.UI.startY = posY;

        jq("#studioPageContent").append("<div id='files_selector'></div>");

        jq("#files_selector").css({
            "left": posX + "px",
            "top": posY + "px",
            "width": "0px",
            "height": "0px",
            "z-index": "254",
            "background-color": "#6CA3BF",
            "position": "absolute"
        });
        jq("#files_selector").fadeTo(0, 0.3);

        jq("body").addClass("select_action");

        var windowFix = (jq.browser.msie && jq.browser.version < 9 ? jq("body") : jq(window));
        windowFix.unbind("mouseout").unbind("mousemove").unbind("mouseup")
            .mousemove(function(event) {
                event = ASC.Files.Common.fixEvent(event);

                var targetMove = event.target || event.srcElement;
                if (typeof targetMove == "undefined")
                    return true;

                var posXnew = event.pageX;
                var posYnew = event.pageY;

                var width, height, top, left;

                width = Math.abs(posXnew - ASC.Files.UI.startX);
                height = Math.abs(posYnew - ASC.Files.UI.startY);

                if (width < 5 && height < 5)
                    return true;

                left = Math.min(ASC.Files.UI.startX, posXnew);
                top = Math.min(ASC.Files.UI.startY, posYnew);

                jq("#files_selector").css({
                    "width": width + "px",
                    "height": height + "px",
                    "left": left + "px",
                    "top": top + "px",
                    "display": "block",
                    "border": "1px solid #034",
                    "cursor": "default"
                });

                ASC.Files.Actions.hideAllActionPanels();

                jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile)").each(function() {
                    var startXnew = Math.min(ASC.Files.UI.startX, posXnew);
                    var startYnew = Math.min(ASC.Files.UI.startY, posYnew);
                    var endX = Math.max(ASC.Files.UI.startX, posXnew);
                    var endY = Math.max(ASC.Files.UI.startY, posYnew);

                    var offset = jq(this).offset();

                    var difX = this.offsetWidth;
                    var difY = this.offsetHeight;

                    if ((offset.top > startYnew && offset.top < endY && offset.left > startXnew && offset.left < endX)
                        || (offset.top + difY > startYnew && offset.top + difY < endY && offset.left > startXnew && offset.left < endX)
                            || (offset.top + difY > startYnew && offset.top + difY < endY && offset.left + difX > startXnew && offset.left + difX < endX)
                                || (offset.top > startYnew && offset.top < endY && offset.left + difX > startXnew && offset.left + difX < endX)
                                    || (offset.top > startYnew && offset.top < endY && offset.left < startXnew && offset.left + difX > endX)
                                        || (offset.top + difY > startYnew && offset.top + difY < endY && offset.left < startXnew && offset.left + difX > endX)
                                            || (offset.top < startYnew && offset.top + difY > endY && offset.left > startXnew && offset.left < endX)
                                                || (offset.top < startYnew && offset.top + difY > endY && offset.left + difX > startXnew && offset.left + difX < endX)) {
                        ASC.Files.UI.selectRow(this, true);
                    } else {
                        if (!event.ctrlKey)
                            ASC.Files.UI.selectRow(this, false);
                    }
                });
                ASC.Files.UI.updateMainContentHeader();

                ASC.Files.UI.resetSelectAll(jq("#files_mainContent div.checkbox input:not(:checked)").length == 0);

                return false;
            });

        windowFix.bind("mouseup", function() {
            jq("#files_selector").remove();
            ASC.Files.UI.startX = 0;
            ASC.Files.UI.startY = 0;
            ASC.Files.UI.mouseBtn = false;
            windowFix.unbind("mousemove").unbind("mouseup");
            jq("body").removeClass("select_action");
        });

        return false;
    };

    var handleMove = function(e) {

        if (jq("div.files_popup_win:visible").length != 0)
            return;

        e = ASC.Files.Common.fixEvent(e);
        var posX = e.pageX;
        var posY = e.pageY;
        var noOver = true;

        jq("#files_mainContent li.fileRow").each(function() {
            var offset = jq(this).offset();

            var difX = this.offsetWidth;
            var difY = this.offsetHeight;

            if ((offset.top < posY && offset.top + difY > posY &&
                offset.left < posX && offset.left + difX > posX)) {
                if (ASC.Files.UI.currentRowOver == this) {
                    noOver = false;
                    return;
                }
                var prev = ASC.Files.UI.currentRowOver;

                ASC.Files.UI.currentRowOver = this;
                ASC.Files.UI.showLinks(this);
                noOver = false;

                if (prev != null)
                    ASC.Files.UI.hideLinks(prev);
            }
        });

        if (noOver) {
            if (ASC.Files.UI.currentRowOver != null)
                ASC.Files.UI.hideLinks(ASC.Files.UI.currentRowOver);
            ASC.Files.UI.currentRowOver = null;
        }

    };

    var preparingMoveTo = function(entryObj, e) {
        e = ASC.Files.Common.fixEvent(e);

        if (!(e.button == 0 || (jq.browser.msie && e.button == 1))) return undefined;

        if (ASC.Files.Folders.folderContainer == "trash") return false;

        if (jq("#files_treeViewPanelSelector").length == 0) return false;

        if (jq("#files_prompt_rename").length != 0) return true;

        if (!jq(entryObj).is(":not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry)")) return true;

        if (jq(entryObj).find("div.checkbox input:checked").length == 0) return true;

        ASC.Files.UI.prevX = e.pageX;
        ASC.Files.UI.prevY = e.pageY;

        jq("body").unbind("mouseout").mouseout(function(event) {
            ASC.Files.UI.beginMoveTo(event);
            return true;
        })
            .unbind("mousemove").mousemove(function(event) {
                ASC.Files.UI.beginMoveTo(event);
                return true;
            });
    };

    var beginMoveTo = function(e) {
        e = ASC.Files.Common.fixEvent(e);

        if (!(e.button == 0 || (jq.browser.msie && e.button == 1))
            || ASC.Files.UI.mouseBtn == false) {
            jq("body").unbind("mouseout").unbind("mousemove");
            return undefined;
        }

        if (Math.abs(e.pageX - ASC.Files.UI.prevX) < 5
            && Math.abs(e.pageY - ASC.Files.UI.prevY) < 5) {
            return false;
        }

        ASC.Files.Actions.hideAllActionPanels();

        var posX = jq("#files_mainContent").offset().left;
        var posY = jq("#files_mainContent").offset().top;

        var listEtryObj = jq("#files_mainContent li.fileRow");

        jq("#files_moving").remove();
        jq("#files_mainContent").parent().append("<ul id='files_moving'></ul>");

        if (jq("#files_mainContent").hasClass("compact"))
            jq("#files_moving").addClass("compact");
        if (jq("#files_mainContent").hasClass("withShare"))
            jq("#files_moving").addClass("withShare");

        var movingEntry = jq("#files_moving").css({
            "left": posX + "px",
            "top": posY + "px",
            "width": jq("#files_mainContent").width() + "px",
            "cursor": (jq.browser.msie ? "move" : "alias")
        });

        listEtryObj.each(function() {
            if (jq("div.checkbox input", jq(this)).is(":checked") && !jq(this).hasClass("errorEntry")) {
                jq(this).clone(false).fadeTo(0, 0.5).appendTo(movingEntry).css({
                    "display": "block",
                    "background-color": "#FFF8D4"
                });
            } else {
                jq(this).clone(false).appendTo(movingEntry).css({
                    "display": "block",
                    "visibility": "hidden"
                });
            }
        });

        jq("*", movingEntry).css("cursor", (jq.browser.msie ? "move" : "alias"));
        jq("* [title!='']", movingEntry).removeAttr("title");
        jq("div.checkbox", movingEntry).css("visibility", "hidden");
        jq("body").addClass("user_select_none");

        jq("#files_mainContent li.folderRow:not(.checkloading):not(.newFolder):not(.errorEntry):not(.row-selected),\
            #files_breadCrumbs a,\
            #files_breadCrumbsRoot").each(function() {
                var folderToId;
                if (jq(this).is("#files_breadCrumbs a")) {
                    folderToId = jq(this).attr("data-id");
                } else {
                    if (this.id == "files_breadCrumbsRoot") {
                        folderToId = ASC.Files.Tree.pathParts[0].Key;
                    } else {
                        folderToId = ASC.Files.UI.getObjectData(this).entryId;
                    }
                }

                var folderToObj = ASC.Files.UI.getEntryObject("folder", folderToId);
                if (ASC.Files.Common.isCorrectId(folderToId)
                && folderToId != ASC.Files.Folders.currentFolderId
                    && ASC.Files.UI.accessibleItem(folderToObj)
                        && (folderToId != ASC.Files.Constants.FOLDER_ID_TRASH
                            || ASC.Files.UI.accessibleItem())) {
                    jq(this).addClass("may-row-to");
                }
            });

        jq("body").unbind("mouseout").mouseout(function(event) {
            ASC.Files.UI.continueMoveTo(movingEntry, event);
            return true;
        })
            .unbind("mousemove").mousemove(function(event) {
                ASC.Files.UI.continueMoveTo(movingEntry, event);
                return true;
            })
            .unbind("mouseup").mouseup(function(event) {
                ASC.Files.UI.finishMoveTo(movingEntry, event);
            });
    };

    var continueMoveTo = function(movingEntry, e) {
        e = ASC.Files.Common.fixEvent(e);
        ASC.Files.Folders.moveToFolder = "";

        if (ASC.Files.UI.mouseBtn == false || typeof movingEntry == "undefined") {
            ASC.Files.UI.finishMoveTo(movingEntry, e);
            return true;
        }

        var posX = e.pageX + jq("#files_mainContent").offset().left - ASC.Files.UI.prevX;
        var posY = e.pageY + jq("#files_mainContent").offset().top - ASC.Files.UI.prevY;

        jq(movingEntry).css({
            "left": posX + "px",
            "display": "block",
            "top": posY + "px",
            "cursor": (jq.browser.msie ? "move" : "alias")
        });

        if (jq("#files_moving_tooltip").length == 0) {
            var list = jq("#files_mainContent li.fileRow:has(div.checkbox input:checked)");

            var textInfo;
            if (list.length == 1) {
                textInfo = ASC.Files.UI.getObjectTitle(list[0]);
            } else {
                textInfo = ASC.Files.FilesJSResources.InfoSelectCount.format(list.length);
            }
            textInfo = ASC.Files.FilesJSResources.InfoSelectingDescribe.format("<b>" + textInfo + "</b><br/>");

            jq("#files_mainContent").parent().append("<div id='files_moving_tooltip'></div>");
            jq("#files_moving_tooltip").html(textInfo);
        }
        jq("#files_moving_tooltip").css({ "left": e.pageX + "px", "top": e.pageY + "px" });

        if (!ASC.Files.UI.accessibleItem()
            || e.ctrlKey) {
            if (!jq(movingEntry).hasClass("copy")) {
                jq(movingEntry).css("cursor", (jq.browser.msie ? "crosshair" : "copy")).addClass("copy");
                jq("*", movingEntry).css("cursor", (jq.browser.msie ? "crosshair" : "copy"));
            }
        } else {
            if (jq(movingEntry).hasClass("copy")) {
                jq(movingEntry).css("cursor", (jq.browser.msie ? "move" : "alias")).removeClass("copy");
                jq("*", movingEntry).css("cursor", (jq.browser.msie ? "move" : "alias"));
            }
        }

        jq(".row-to").removeClass("row-to");

        jq(".may-row-to").each(function() {
            var folderToId;
            if (jq(this).is("#files_breadCrumbs a")) {
                folderToId = jq(this).attr("href").replace("#", "");
            } else {
                if (this.id == "files_breadCrumbsRoot") {
                    folderToId = ASC.Files.Tree.pathParts[0].Key;
                } else {
                    folderToId = ASC.Files.UI.getObjectData(this).entryId;
                }
            }

            if (ASC.Files.Common.isCorrectId(folderToId)) {
                var difX = this.offsetWidth;
                var difY = this.offsetHeight;

                var pos = jq(this).offset();
                if (pos.top < e.pageY
                    && pos.top + difY > e.pageY
                        && pos.left < e.pageX
                            && pos.left + difX > e.pageX) {

                    ASC.Files.Folders.moveToFolder = folderToId;
                    jq(this).addClass("row-to");
                    return true;
                }
            }
        });

        if (jq.browser.opera) {
            var target = e.target || e.srcElement;
            target = jq(target);
            if (!target.hasClass("fileRow"))
                target = jq(target).closest("li.fileRow");

            var nameFix = "fix_select_text";
            var el = target.children("#" + nameFix);
            if (el.length == 0) {
                jq("#" + nameFix).remove();
                el = document.createElement("INPUT");
                el.style.width = 0;
                el.style.height = 0;
                el.style.border = 0;
                el.style.margin = 0;
                el.style.padding = 0;
                el.id = nameFix;
                el.disabled = true;

                target.prepend(el);
                el = jq("#" + nameFix);
            }

            try {
                el.focus();
            } catch (e) {
                el.disabled = false;
                el.focus();
                el.disabled = true;
            }
        }
    };

    var finishMoveTo = function(movingEntry, e) {
        var data = {};
        data.entry = new Array();

        var folderToId = ASC.Files.Folders.moveToFolder;
        ASC.Files.Folders.moveToFolder = "";
        jq(".row-to").removeClass("row-to");
        jq(".may-row-to").removeClass("may-row-to");
        jq("body").removeClass("user_select_none");

        if (ASC.Files.Common.isCorrectId(folderToId)) {
            if (folderToId == ASC.Files.Constants.FOLDER_ID_TRASH) {
                ASC.Files.Folders.deleteItem();
            } else {
                ASC.Files.Folders.isCopyTo =
                    (!ASC.Files.UI.accessibleItem()
                        || e.ctrlKey === true);

                var thirdParty = typeof ASC.Files.ThirdParty != "undefined";
                var takeThirdParty = thirdParty && (ASC.Files.Folders.isCopyTo == true || ASC.Files.ThirdParty.isThirdParty());

                jq("li.fileRow", movingEntry).each(function() {
                    if (jq(this).css("visibility") != "hidden") {
                        var entryRowData = ASC.Files.UI.getObjectData(this);
                        var entryRowObject = entryRowData.entryObject;
                        var entryRowType = entryRowData.entryType;
                        var entryRowId = entryRowData.entryId;
                        var itemRowId = entryRowType + "_" + entryRowId;

                        if (takeThirdParty
                            || !thirdParty
                                || !ASC.Files.ThirdParty.isThirdParty(entryRowObject)) {
                            if (ASC.Files.Folders.isCopyTo == true || ASC.Files.UI.accessAdmin(entryRowObject)) {
                                if (ASC.Files.Folders.isCopyTo == true || !ASC.Files.UI.editingFile(entryRowObject)) {
                                    ASC.Files.UI.blockObject(this, true,
                                        (ASC.Files.Folders.isCopyTo == true) ?
                                            ASC.Files.FilesJSResources.DescriptCopy :
                                            ASC.Files.FilesJSResources.DescriptMove);
                                    data.entry.push(encodeURIComponent(itemRowId));
                                }
                            }
                        }
                    }
                });

                var folderToTitle = ASC.Files.UI.getEntryTitle("folder", folderToId);

                if (data.entry && data.entry.length != 0) {
                    serviceManager.moveFilesCheck(ASC.Files.TemplateManager.events.MoveFilesCheck,
                        {
                            folderToTitle: folderToTitle,
                            folderToId: folderToId,
                            list: data,
                            isCopyOperation: (ASC.Files.Folders.isCopyTo == true)
                        },
                        { stringList: data });
                }
            }
        }

        ASC.Files.Folders.isCopyTo = false;
        jq("body").unbind("mouseout").unbind("mousemove").unbind("mouseup");
        jq(movingEntry).remove();
        jq("#files_moving").remove();
        jq("#files_moving_tooltip").remove();
    };

    var clickRow = function(event) {
        var e = ASC.Files.Common.fixEvent(event);

        if (!(e.button == 0 || (jq.browser.msie && e.button == 1)))
            return true;

        var target = jq(e.srcElement || e.target);

        try {
            if (target.is("a, div.rowActions"))
                return true;
        } catch (e) {
            return true;
        }

        var entryObj =
            target.is("li.fileRow")
                ? target
                : target.closest("li.fileRow");

        ASC.Files.UI.selectRow(entryObj, !entryObj.hasClass("row-selected"));
        ASC.Files.UI.updateMainContentHeader();
    };

    var updateMainContentHeader = function() {
        if (jq("#files_mainContent li.fileRow:has(div.checkbox input:checked)").length == 0) {
            jq("#mainContentHeader .menuAction.unlockAction").removeClass("unlockAction");
            if (ASC.Files.Folders.folderContainer == "trash")
                jq("#files_mainEmptyTrash").addClass("unlockAction");
        } else {
            ASC.Files.Actions.showActionsViewPanel();
        }
    };

    var selectRow = function(entryObj, value) {
        entryObj = jq(entryObj);

        if (!entryObj.hasClass("fileRow"))
            entryObj = entryObj.closest("li.fileRow");

        if (entryObj.hasClass("row-selected") == value)
            return;

        if (entryObj.hasClass("checkloading") && value)
            return;

        if (entryObj.hasClass("newFolder") || jq(entryObj).hasClass("rowRename"))
            value = false;

        var input = entryObj.find("div.checkbox input");
        input.attr("checked", value === true);

        if (value) {
            entryObj.addClass("row-selected user_select_none");
            if (jq.browser.msie) {
                entryObj.find("*").attr("unselectable", "on");
            }
        } else {
            entryObj.removeClass("row-selected user_select_none");
            if (jq.browser.msie) {
                entryObj.find("*").removeAttr("unselectable");
            }
        }

        resetSelectAll(jq("#files_mainContent li.fileRow:has(div.checkbox input:not(:checked))").length == 0);
    };

    var resetSelectAll = function(param) {
        jq("#files_selectAll_check").attr("checked", param === true);
    };

    var checkSelectAll = function(value) {
        jq("#files_mainContent li.fileRow:not(.checkloading)").each(function() {
            ASC.Files.UI.selectRow(this, value);
        });
        ASC.Files.UI.updateMainContentHeader();
    };

    var checkSelect = function(filter) {
        ASC.Files.Actions.hideAllActionPanels();
        jq("#files_mainContent li.fileRow:not(.checkloading)").each(function() {
            var sel;
            var fileTitle = ASC.Files.UI.getObjectTitle(this);
            switch (filter) {
                case "Folder":
                    sel = jq(this).hasClass("folderRow");
                    break;
                case "File":
                    sel = !jq(this).hasClass("folderRow");
                    break;
                case "Document":
                    sel = ASC.Files.Utility.FileIsDocument(fileTitle) && !jq(this).hasClass("folderRow");
                    break;
                case "Presentation":
                    sel = ASC.Files.Utility.FileIsPresentation(fileTitle) && !jq(this).hasClass("folderRow");
                    break;
                case "Spreadsheet":
                    sel = ASC.Files.Utility.FileIsSpreadsheet(fileTitle) && !jq(this).hasClass("folderRow");
                    break;
                case "Image":
                    sel = ASC.Files.Utility.FileIsImage(fileTitle) && !jq(this).hasClass("folderRow");
                    break;
                default:
                    return false;
            }

            ASC.Files.UI.selectRow(this, sel);
        });
        ASC.Files.UI.updateMainContentHeader();
    };

    var displayInfoPanel = function(str, warn) {
        if (str === "" || typeof str === "undefined")
            return;

        clearTimeout(timeIntervalInfo);

        jq("#infoPanelContainer").removeClass("warn").children("div").text(str);
        jq("#infoPanelContainer").css("margin-left", -jq("#infoPanelContainer").width() / 2);
        if (jq.browser.mobile) {
            jq("#infoPanelContainer").css("top", jq(window).scrollTop() + "px");
        }

        if (warn === true)
            jq("#infoPanelContainer").addClass("warn");
        jq("#infoPanelContainer").show();

        timeIntervalInfo = setTimeout("ASC.Files.UI.hideInfoPanel();", 3000);
    };

    var hideInfoPanel = function() {
        clearTimeout(timeIntervalInfo);
        jq("#infoPanelContainer").hide().children("div").html("&nbsp;");
    };

    var accessibleItem = function(entryObj) {
        if (entryObj) {
            var entryData = ASC.Files.UI.getObjectData(entryObj);
            var entryId = entryData ? entryData.entryId : null;
            var entryType = entryData ? entryData.entryType : null;
        } else {
            entryType = "folder";
            entryId = ASC.Files.Folders.currentFolderId;
        }

        var access = ASC.Files.Constants.USER_ADMIN || !entryData || entryData.create_by_id == ASC.Files.Constants.USER_ID;

        if (ASC.Files.Share) {
            if (entryType == "folder") {
                if (entryId == ASC.Files.Constants.FOLDER_ID_COMMON_FILES && !ASC.Files.Constants.USER_ADMIN)
                    return false;

                if (entryId == ASC.Files.Constants.FOLDER_ID_SHARE)
                    return false;
            }

            var curAccess = parseInt(entryData ? entryData.access : ASC.Files.Constants.AceStatusEnum.None);

            if (entryType == "folder" && entryId == ASC.Files.Folders.currentFolderId) {
                curAccess = parseInt(jq("#access_current_folder").val());
            }

            switch (curAccess) {
                case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                    return true;
                case ASC.Files.Constants.AceStatusEnum.Read:
                case ASC.Files.Constants.AceStatusEnum.Restrict:
                    return false;
            }

            access = true;
        }

        return access;
    };

    var accessAdmin = function(entryObj) {
        
        if (entryObj) {
            var entryData = ASC.Files.UI.getObjectData(entryObj);
            var entryId = entryData ? entryData.entryId : null;
            var entryType = entryData ? entryData.entryType : null;
        } else {
            entryType = "folder";
            entryId = ASC.Files.Folders.currentFolderId;
        }

        var access = ASC.Files.Constants.USER_ADMIN || !entryData || entryData.create_by_id == ASC.Files.Constants.USER_ID;

        if (ASC.Files.Share) {
            var curAccess = parseInt(entryData ? entryData.access : ASC.Files.Constants.AceStatusEnum.None);

            if (entryType == "folder" && entryId == ASC.Files.Folders.currentFolderId) {
                curAccess = parseInt(jq("#access_current_folder").val() || ASC.Files.Constants.AceStatusEnum.None);
            }

            if (curAccess == ASC.Files.Constants.AceStatusEnum.Restrict) {
                entryObj.remove();
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.AceStatusEnum_Restrict, true);
                return false;
            }

            access = (curAccess == ASC.Files.Constants.AceStatusEnum.None
                || ASC.Files.Folders.folderContainer == "corporate" && ASC.Files.Constants.USER_ADMIN);
        }
        return access;
    };

    var lockEditFileById = function(fileId, edit, by) {
        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        return ASC.Files.UI.lockEditFile(fileObj, edit, by);
    };

    var lockEditFile = function(fileObj, edit, by) {
        if (fileObj.hasClass("folderRow"))
            return;

        if (edit) {
            fileObj.addClass("onEdit");

            var strBy = ASC.Files.FilesJSResources.TitleEditingFile;
            if (by) {
                strBy = ASC.Files.FilesJSResources.TitleEditingFileBy.format(by);
            }

            fileObj.find("div.fileEditing.pencil").attr("title", strBy);
        } else {
            fileObj.removeClass("onEdit");
        }
    };

    var checkEditing = function() {
        clearTimeout(ASC.Files.UI.timeCheckEditing);

        var list = jq("#files_mainContent li.fileRow.onEdit");
        if (list.length == 0) return;

        var data = {};
        data.entry = new Array();
        for (var i = 0; i < list.length; i++) {
            var fileId = ASC.Files.UI.getObjectData(list[i]).entryId;
            data.entry.push(fileId);
        }

        serviceManager.checkEditing(ASC.Files.TemplateManager.events.CheckEditing, { list: data.entry }, { stringList: data });
    };

    var displayEntryTooltip = function(entryType, entryId) {
        var entryObj = ASC.Files.UI.getEntryObject(entryType, entryId);

        var entryData = ASC.Files.UI.getObjectData(entryObj);
        var jsonData = {
            entryTooltip:
                {
                    type: entryType,
                    title: Encoder.htmlEncode(entryData.title),
                    modified_by: Encoder.htmlEncode(
                        (ASC.Files.Folders.folderContainer == "trash" || entryType == "file"
                            ? entryData.modified_by
                            : entryData.create_by)),
                    date_type:
                        (ASC.Files.Folders.folderContainer == "trash"
                            ? "remove"
                            : (entryType == "folder"
                                ? "create"
                                : (entryData.version > 1
                                    ? "update"
                                    : "upload"))),
                    modified_on:
                        (entryType == "file"
                            ? entryData.modified_on
                            : (ASC.Files.Folders.folderContainer == "trash"
                                ? entryData.modified_on
                                : entryData.create_on)),
                    version: entryData.version, //file
                    length: entryData.content_length, //file
                    error: entryData.error,

                    provider_name: entryData.provider_name,
                    total_files: parseInt(entryObj.find(".countFiles").html()) || 0, //folder
                    total_sub_folder: parseInt(entryObj.find(".countFolders").html()) || 0//folder
                }
        };
        var stringData = serviceManager.jsonToXml(jsonData);

        var curTemplate = ASC.Files.TemplateManager.templates.getTooltip;
        var xslData = ASC.Files.TemplateManager.getTemplate(curTemplate);
        var htmlTootlip = ASC.Controls.XSLTManager.translateFromString(stringData, xslData);

        jq("#entryTooltip").html(htmlTootlip);

        jq.dropdownToggle().toggle(entryObj.find("div.thumb-" + entryType), "entryTooltip");
    };

    var hideEntryTooltip = function() {
        jq("#entryTooltip").hide();
        clearTimeout(ASC.Files.UI.timeTooltip);
    };

    var checkCharacter = function(input) {
        jq(input).unbind("keyup").bind("keyup", function() {
            var str = jq(this).val();
            if (str.search(ASC.Files.Common.characterRegExp) != -1) {
                jq(this).val(ASC.Files.Common.replaceSpecCharacter(str));
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SpecCharacter.format(ASC.Files.Common.characterString), true);
            }
        });
    };

    var documentTitleFix = function() {
        if (jq.browser.msie)
            setInterval("document.title = jq('#currentFolderTitle').val() + ' - ' + ASC.Files.Constants.TITLE_PAGE;", 200);
    };

    return {
        init: init,
        parseItemId: parseItemId,
        getEntryObject: getEntryObject,
        getObjectData: getObjectData,

        getEntryTitle: getEntryTitle,
        getObjectTitle: getObjectTitle,

        currentPage: currentPage,
        amountPage: amountPage,

        countEntityInFolder: countEntityInFolder,

        currentRowOver: currentRowOver,

        showOverwriteMessage: showOverwriteMessage,

        updateFolderInfo: updateFolderInfo,

        updateFolderView: updateFolderView,
        switchFolderView: switchFolderView,

        blockObjectById: blockObjectById,
        blockObject: blockObject,
        hideLinks: hideLinks,
        showLinks: showLinks,

        preparingMoveTo: preparingMoveTo,
        beginMoveTo: beginMoveTo,
        finishMoveTo: finishMoveTo,
        continueMoveTo: continueMoveTo,

        updateMainContentHeader: updateMainContentHeader,

        checkSelectAll: checkSelectAll,
        checkSelect: checkSelect,
        selectRow: selectRow,
        clickRow: clickRow,

        paintRows: paintRows,
        addRowHandlers: addRowHandlers,

        resetSelectAll: resetSelectAll,

        beginSelecting: beginSelecting,
        handleMove: handleMove,

        mouseBtn: mouseBtn,

        startX: startX,
        startY: startY,
        prevX: prevX,
        prevY: prevY,

        displayInfoPanel: displayInfoPanel,
        hideInfoPanel: hideInfoPanel,

        editingFile: editingFile,
        editableFile: editableFile,

        accessibleItem: accessibleItem,
        accessAdmin: accessAdmin,

        checkEditing: checkEditing,
        timeCheckEditing: timeCheckEditing,

        lockEditFileById: lockEditFileById,
        lockEditFile: lockEditFile,

        checkCharacter: checkCharacter,
        documentTitleFix: documentTitleFix,

        toggleMainMenu: toggleMainMenu,

        displayEntryTooltip: displayEntryTooltip,
        hideEntryTooltip: hideEntryTooltip,
        timeTooltip: timeTooltip
    };
})();

(function($) {
    ASC.Files.UI.init();
    $(function() {

        jq("#infoPanelContainer").click(function() {
            ASC.Files.UI.hideInfoPanel();
        });

        jq("#switchToNormal").click(function() {
            jq("#files_mainContent").removeClass("compact");
            ASC.Files.UI.switchFolderView();
        });

        jq("#switchToCompact").click(function() {
            jq("#files_mainContent").addClass("compact");
            ASC.Files.UI.switchFolderView();
        });

        jq("#files_mainContent").on("click", "li.fileRow", ASC.Files.UI.clickRow);

        jq("#files_mainContent").on("mousedown", "li.fileRow", function(event) {
            ASC.Files.UI.preparingMoveTo(this, event);
        });

        jq("#mainContent").on("mouseover", "#files_mainContent.compact li.fileRow:not(.checkloading):not(.newFolder):not(.newFile) .entryTitle a.name", function() {
            ASC.Files.UI.hideEntryTooltip();
            jq(this).one("mouseleave", function() {
                ASC.Files.UI.hideEntryTooltip();
            });

            var entryData = ASC.Files.UI.getObjectData(this);
            var entryId = entryData.entryId;
            var entryType = entryData.entryType;

            ASC.Files.UI.timeTooltip = setTimeout("ASC.Files.UI.displayEntryTooltip('" + entryType + "', '" + entryId + "')", 750);
        });

        jq("#files_mainContent").on("click", "div.checkbox input", function(event) {
            ASC.Files.UI.selectRow(this, this.checked == true);
            ASC.Files.UI.updateMainContentHeader();
            jq(this).blur();
            ASC.Files.Common.cancelBubble(event);
        });

        jq("#files_listUp").click(function() {
            jq(document).scrollTop(0);
            return false;
        });

        jq(document).keydown(function(event) {
            if (jq("div.blockDialog:visible").length != 0 ||
                jq("div.blockMsg:visible").length != 0 ||
                    jq("div.files_popup_win:visible").length != 0 ||
                        jq("#files_prompt_rename").length != 0 ||
                            jq("#files_prompt_create_folder").length != 0 ||
                                jq("#files_prompt_create_file").length != 0)
                return;

            var code;
            if (!e) var e = event;

            e = ASC.Files.Common.fixEvent(e);

            var target = e.target || e.srcElement;
            try {
                if (jq(target).is("input"))
                    return true;
            } catch(e) {
                return true;
            }

            var code = e.keyCode || e.which;

            if (code == ASC.Files.Common.keyCode.a && e.ctrlKey) {
                if (jq.browser.opera)
                    setTimeout('jq("#files_selectAll_check").focus()', 1);
                ASC.Files.UI.checkSelectAll(true);
                return false;
            }
        });

        jq(document).keyup(function(event) {
            if (jq("div.blockDialog:visible").length != 0 ||
                jq("div.blockMsg:visible").length != 0 ||
                    jq("div.files_popup_win:visible").length != 0 ||
                        jq("#files_prompt_rename").length != 0 ||
                            jq("#files_prompt_create_folder").length != 0 ||
                                jq("#files_prompt_create_file").length != 0)
                return;

            if (!e) var e = event;

            e = ASC.Files.Common.fixEvent(e);

            var target = e.target || e.srcElement;
            try {
                if (jq(target).is("input"))
                    return true;
            } catch(e) {
                return true;
            }

            var code = e.keyCode || e.which;

            if (code == ASC.Files.Common.keyCode.deleteKey) {
                ASC.Files.Folders.deleteItem();
                return false;
            }

            if (code == ASC.Files.Common.keyCode.f && e.shiftKey) {
                ASC.Files.Folders.createFolder();
                return false;
            }

            if (code == ASC.Files.Common.keyCode.n && e.shiftKey) {
                ASC.Files.Folders.typeNewDoc = "document";
                ASC.Files.Folders.createNewDoc();
                return false;
            }
        });

        jq(".comming_soon").unbind("click").click(function() {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoCommingSoon, true);
            return false;
        });

    });
})(jQuery);