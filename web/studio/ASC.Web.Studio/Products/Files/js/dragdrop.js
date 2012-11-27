window.ASC.Files.DragDrop = (function() {
    var isInit = false;
    var totalDragDrop = 0;
    var countDragDrop = 0;
    var successDragDrop = 0;

    var checkReady = true;
    var fileStek = new Array();

    var init = function() {
        if (isInit === false) {
            isInit = true;

            if (canDragDrop() && isFileAPI()) {
                jq("#files_mainContent, #emptyContainer")
                    .bind("dragover", function(event) { return ASC.Files.DragDrop.over(event); })// show area for drop
                    .bind("dragenter", function() { return false; })//empty event
                    .bind("dragleave", function(event) { return ASC.Files.DragDrop.leave(event); })//hide area
                    .bind("drop", function(event) { return ASC.Files.DragDrop.drop(event); }); // drop event

                blockDocumentDrop();
            } else {
                jq("#emptyContainer .emptyContainer_dragDrop").remove();
            }
        }
    };

    var canDragDrop = function() {
        return "draggable" in document.createElement("span")
            && typeof(new XMLHttpRequest()).upload != "undefined";
    };

    var isFileAPI = function() {
        return typeof FileReader != "undefined"
            && typeof(new XMLHttpRequest()).upload != "undefined";
    };

    var over = function(e) {
        var dt = e.originalEvent.dataTransfer;
        if (!dt) return true;

        if (ASC.Files.Folders.folderContainer == "trash"
            || !ASC.Files.UI.accessibleItem()) {
            return true;
        }

        //file?
        //FF
        if (dt.types.contains && !dt.types.contains("Files")) return true;
        //Chrome
        if (dt.types.indexOf && dt.types.indexOf("Files") == -1) return true;

        //bugfix chrome
        if (jq.browser.webkit) dt.dropEffect = "copy";

        jq("#files_mainContent, #emptyContainer").addClass("selected");

        return false;
    };

    var leave = function() {
        return hideHighlight();
    };

    var drop = function(e) {
        var dt = e.originalEvent.dataTransfer;
        if (!dt && !dt.files) return true;

        if (ASC.Files.Folders.folderContainer == "trash"
            || !ASC.Files.UI.accessibleItem()) {
            return true;
        }

        hideHighlight();
        var allFiles = dt.files;
        var filesToUpload = new Array(0);

        for (var i = 0; i < allFiles.length; i++) {
            if (correctFile(allFiles[i])) {
                filesToUpload.push(allFiles[i]);
            }
        }

        ASC.Files.DragDrop.totalDragDrop = ASC.Files.DragDrop.totalDragDrop + filesToUpload.length;

        if (filesToUpload.length != 0) {
            for (var item = 0; item < filesToUpload.length; item++) {
                ASC.Files.DragDrop.uploadFile({ file: filesToUpload[item], folder: ASC.Files.Folders.currentFolderId });
            }
        }
        return false;
    };

    var hideHighlight = function() {
        jq("#files_mainContent, #emptyContainer").removeClass("selected");
        return false;
    };

    var correctFile = function(obj) {
        var sizeF = (obj.fileSize || obj.size);

        if (sizeF <= 0) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_EmptyFile, true);
            return false;
        }

        if (ASC.Files.Constants.MAX_UPLOAD_SIZE < sizeF) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.Constants.MAX_UPLOAD_EXCEED, true);
            return false;
        }

        return true;
    };

    var blockDocumentDrop = function() {
        jq(document)
            .bind("dragenter", function() { return false; })
            .bind("dragleave", function() { return false; })
            .bind("dragover", function(e) {
                var dt = e.originalEvent.dataTransfer;
                if (!dt) {
                    return true;
                }
                dt.dropEffect = "none";
                return false;
            }.bind(this));
    };

    //DragDrop
    var uploadFile = function(fileElem) {
        if (fileStek.length == 0 && checkReady == true) {
            checkReady = false;
            startUpload(fileElem);
        } else {
            fileStek.push(fileElem);
        }
    };

    var startUploadWithDelay = function() {
        var file = fileStek.shift();
        if (file != undefined) {
            checkReady = false;
            startUpload(file);
        } else {
            onEndProgress();
        }
    };

    var startUpload = function(fileElem) {
        var file = fileElem.file;
        var folder = fileElem.folder;

        try {
            var fileName = ASC.Files.Common.replaceSpecCharacter(file.name || file.fileName);
            if (jq("#drag_drop_process").length == 0) {
                jq("#files_progress_template").clone().attr("id", "drag_drop_process").appendTo("#files_bottom_loader");
                jq("#drag_drop_process").prepend('<div title="{0}" class="terminateTasks"></div>'.format(ASC.Files.FilesJSResources.TitleCancel));
            }
            jq("#drag_drop_process .progress").css("width", "0%");
            jq("#drag_drop_process .percent").text("0%");
            jq("#drag_drop_process .headerBaseMedium").html(ASC.Files.FilesJSResources.FileUploading + " " + fileName);
            jq("#drag_drop_process").show();

            var xhr = new XMLHttpRequest();

            xhr.upload.addEventListener("progress", function(e) { ASC.Files.DragDrop.onProgress(e); }, false);
            xhr.onprogress = function(e) { ASC.Files.DragDrop.onProgress(e); }.bind(this);

            xhr.onload = function(e) { onCompleteUpload(e); }.bind(this);

            var sitePath = ASC.Files.Common.getSitePath();

            sitePath += ASC.Files.Constants.URL_HANDLER_UPLOAD.format(folder, encodeURIComponent(fileName));

            // parameter address is passed to the file name - otherwise nothing
            xhr.open("POST", sitePath, true);
            //xmlhttp.setRequestHeader("Content-Type", "application/octet-stream");
            xhr.send(file);
        } catch(error) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_DnDError, true);
            onCompleteUpload();
        }
        return false;
    };

    var onCompleteUpload = function(e) {
        checkReady = true;
        ASC.Files.DragDrop.countDragDrop++;

        var successUpload = true;

        if (typeof e != "undefined") {
            var result = { error: true };
            try {
                result = eval("result=" + e.target.responseText);
            } catch(e) {
            }
            if (result.error === true) {
                var strTitle = result.message;
                if (strTitle && strTitle.length > 0) {
                    ASC.Files.UI.displayInfoPanel(strTitle, true);
                    successUpload = false;
                }
            } else {
                ASC.Files.DragDrop.successDragDrop++;

                //current folder
                var folderDnD = result.folderId;
                if (ASC.Files.Common.isCorrectId(folderDnD) && folderDnD == ASC.Files.Folders.currentFolderId) {
                    var file = result.file;
                    var stringXmlFile = serviceManager.jsonToXml({ file: file });
                    var params = {
                        fileId: file.id,
                        show: true,
                        isStringXml: true
                    };

                    var fileObj = ASC.Files.UI.getEntryObject("file", file.id);
                    if (fileObj.length != 0) {
                        ASC.Files.EventHandler.onReplaceVersion(stringXmlFile, params);
                    } else {
                        ASC.Files.EventHandler.onGetFile(stringXmlFile, params);
                    }
                }
            }
        }

        jq("#drag_drop_process .progress").css("width", "100%");
        jq("#drag_drop_process .percent").text("100%").css("color", "white");
        jq("#drag_drop_process .textSmallDescribe").text(ASC.Files.FilesJSResources.ProcessUploadCountFiles.format(ASC.Files.DragDrop.countDragDrop, ASC.Files.DragDrop.totalDragDrop));

        if (ASC.Files.DragDrop.successDragDrop > 0 && successUpload)
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoUploadedSuccess.format(ASC.Files.DragDrop.successDragDrop));

        //end DnD
        if (fileStek.length == 0) {
            onEndProgress();
        } else {
            if (checkReady == true) {
                setTimeout("ASC.Files.DragDrop.startUploadWithDelay()", 100);
            }
        }
    };

    var onProgress = function(e) {
        var progress = Math.min(Math.round(e.loaded * 100 / e.total), 100);
        jq("#drag_drop_process .progress").css("width", progress + "%");
        jq("#drag_drop_process .percent").text(progress + "%");
        jq("#drag_drop_process .textSmallDescribe").text(ASC.Files.FilesJSResources.ProcessUploadCountFiles.format(ASC.Files.DragDrop.countDragDrop, ASC.Files.DragDrop.totalDragDrop));
    };

    var onEndProgress = function() {
        ASC.Files.DragDrop.totalDragDrop = 0;
        ASC.Files.DragDrop.countDragDrop = 0;
        ASC.Files.DragDrop.successDragDrop = 0;

        setTimeout('jq("#drag_drop_process").hide();', 800);
    };

    var terminateTasks = function() {
        fileStek.clear();
    };

    return {
        init: init,

        isFileAPI: isFileAPI,

        leave: leave,
        over: over,
        drop: drop,

        totalDragDrop: totalDragDrop,
        countDragDrop: countDragDrop,
        successDragDrop: successDragDrop,

        uploadFile: uploadFile,
        startUploadWithDelay: startUploadWithDelay,
        onProgress: onProgress,
        terminateTasks: terminateTasks
    };
})();

(function($) {
    $(function() {
        ASC.Files.DragDrop.init();
        jq("#files_bottom_loader").on("click", "#drag_drop_process .terminateTasks", function() {
            ASC.Files.DragDrop.terminateTasks();
            return false;
        });
    });
})(jQuery);