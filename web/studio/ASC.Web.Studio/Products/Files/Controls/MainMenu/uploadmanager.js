window.ASC.Files.Uploads = (function() {
    var isInit = false;
    var fileUploader = null;
    var totalCount = 0;

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }
    };

    var onUploadComplete = function(fileUploadResult) {
        var result = {
            Success: fileUploadResult.Success,
            Message: fileUploadResult.Message
        };

        if (result.Success) {
            try {
                var file = eval("file=" + fileUploadResult.Data);
                var stringXmlFile = serviceManager.jsonToXml({ file: file });
                var params = {
                    fileId: file.id,
                    show: false,
                    isStringXml: true
                };

                var fileObj = ASC.Files.UI.getEntryObject("file", file.id);
                if (fileObj.length != 0) {
                    ASC.Files.EventHandler.onReplaceVersion(stringXmlFile, params);
                } else {
                    ASC.Files.EventHandler.onGetFile(stringXmlFile, params);
                }
            } catch(e) {
            }
            totalCount++;
        } else {
            ASC.Files.UI.displayInfoPanel(result.Message, true);

            if (FileHtml5Uploader.EnableHtml5 === false &&
                (ASC.Controls.FlashDetector.DetectFlashVer(8, 0, 0) === false || ASC.Controls.FileUploaderGlobalConfig.DisableFlash === true))
                OnAllUploadComplete();
        }
    };

    var OnProgress = function(data) {
        jq("#uploads_message").text(ASC.Files.FilesJSResources.Uploading + " " + data.CurrentFile);
    };

    var OnAllUploadComplete = function() {
        jq("#files_uploadDialog .cancelButton").show();
        jq("#files_swf_button_container").hide();
        jq("#upload_finish").show();

        PopupKeyUpActionProvider.CloseDialogAction = "";
        PopupKeyUpActionProvider.EnterAction = 'jq("#files_okUpload").click();';
    };

    var showUploadedFiles = function() {
        var list = jq("#files_mainContent li.newFile");
        if (list.length > 0) {
            ASC.Files.EmptyScreen.hideEmptyScreen();
            list.removeClass("newFile").show().yellowFade();

            ASC.Files.UI.paintRows();
        }

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoUploadedSuccess.format(totalCount));
    };

    var showUploadPopup = function() {
        if (jq.browser.mobile
            || jq("#files_uploadDialog:visible").length != 0)
            return;

        if (ASC.Files.Folders.folderContainer == "trash"
            || !ASC.Files.UI.accessibleItem()) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SecurityException, true);
            ASC.Files.Folders.eventAfter = showUploadPopup;
            ASC.Files.Folders.defaultFolderSet();
            return;
        }

        jq("#files_upload_select").css("display", "");
        jq("#files_swf_button_container").show();
        jq("#panelButtons").show();
        jq("#files_upload_btn_html5").show();
        jq("#files_uploadDlgPanelButtons").hide();

        jq("#upload_finish").hide();
        jq("#files_uploadDialog .cancelButton").show();

        jq("#uploadDialogContainerHeader").html('{0} "<span title="{1}">{1}</span>"'.format(ASC.Files.FilesJSResources.UploadFileTo, jq("#currentFolderTitle").val()));

        ASC.Files.Common.blockUI(jq("#files_uploadDialog"), 600, 600);

        ASC.Files.UI.documentTitleFix();
        PopupKeyUpActionProvider.EnableEsc = false;
        PopupKeyUpActionProvider.CloseDialogAction = "ASC.Files.UI.documentTitleFix();";
        PopupKeyUpActionProvider.EnterAction = jq.browser.mozilla ? "" : 'jq("#files_uploadSubmit").click();';

        jq("#files_upload_fileList").html("");

        activateUploader("files_upload_fileList", ASC.Files.Folders.currentFolderId);

        jq("#asc_fileuploaderSWFContainer").removeAttr("style");
    };

    var activateUploader = function(pnlId, folderId) {

        jq("object[id*='SWFUpload']").before('<span id="asc_fileuploaderSWFObj"></span>');

        totalCount = 0;
        try {
            jq("object[id*='SWFUpload']").remove();
        } catch(e) {
        }

        var btnId = jq("#files_upload_btn").attr("id") ||
            jq("#files_swf_button_container a.files_upload_btn").attr("id"); //fix basic uploader

        if (ASC.Files.DragDrop.isFileAPI() && FileHtml5Uploader.EnableHtml5) {
            jq("#files_swf_button_container").hide();
            jq("#files_upload_pnl").hide();
            btnId = jq(".files_upload_btn_html5", "#files_uploadDialog").attr("id"); //fix basic uploader, when html5 on and flash off
        } else {
            jq("#files_swf_button_container").show();
            jq("#files_upload_pnl").show();
        }

        if (typeof ASC !== "undefined" && typeof ASC.Controls !== "undefined" && typeof ASC.Controls.FileUploader !== "undefined") {

            fileUploader = FileHtml5Uploader.InitFileUploader({
                FileUploadHandler: "ASC.Web.Files.Classes.FilesUploader,ASC.Web.Files",
                FileSizeLimit: ASC.Files.Constants.MAX_UPLOAD_SIZE,

                AutoSubmit: false,
                ProgressTimeSpan: 1000,
                UploadButtonID: btnId,
                TargetContainerID: pnlId,
                Data: { folderId: folderId },
                OnUploadComplete: onUploadComplete,
                OnAllUploadComplete: OnAllUploadComplete,
                OnProgress: OnProgress,

                AddFilesText: ASC.Files.FilesJSResources.ButtonAddFiles,
                SelectFilesText: ASC.Files.FilesJSResources.ButtonSelectFiles,

                DeleteLinkCSSClass: "files_deleteLinkCSSClass",
                LoadingImageCSSClass: "files_loadingCSSClass",
                CompleteCSSClass: "files_completeCSSClass",
                OverAllProcessBarCssClass: "files_overAllProcessBarCssClass",

                DragDropHolder: jq("#files_uploadDialog div.panelContent"),
                FilesHeaderCountHolder: jq("#files_uploadHeader"),
                OverAllProcessHolder: jq("#files_overallprocessHolder"),
                SubmitPanelAfterSelectHolder: jq("#files_uploadDlgPanelButtons"),

                OverallProgressText: ASC.Files.FilesJSResources.OverallProgress,
                FilesHeaderText: ASC.Files.FilesJSResources.UploadedOf,
                SelectFileText: ASC.Files.FilesJSResources.SelectFilesToUpload,
                DragDropText: ASC.Files.FilesJSResources.OrDragDrop,
                SelectedFilesText: ASC.Files.FilesJSResources.SelectedFiles
            });
        }
        jq("#" + btnId).unbind("mouseover");
    };

    var fileUploaderSubmit = function() {
        PopupKeyUpActionProvider.EnterAction = "";
        totalCount = 0;
        if (fileUploader != null) {
            if (jq("#files_upload_fileList").html() == "") {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.EmptyListUpload, true);
                PopupKeyUpActionProvider.EnterAction = jq.browser.mozilla ? "" : 'jq("#files_uploadSubmit").click();';
                return;
            }
            jq("#uploads_message").text(ASC.Files.FilesJSResources.UploadProcess);
            jq("#panelButtons").hide();
            jq("#files_upload_btn_html5").hide();
            jq("#files_uploadDialog .cancelButton").hide();

            fileUploader.Submit();
        }
    };

    return {
        init: init,

        showUploadPopup: showUploadPopup,

        fileUploaderSubmit: fileUploaderSubmit,
        showUploadedFiles: showUploadedFiles
    };
})();

(function($) {
    ASC.Files.Uploads.init();

    $(function() {

        jq("#files_uploadSubmit").click(function() {
            ASC.Files.Uploads.fileUploaderSubmit();
        });

        jq("#topUpload a").click(function() {
            ASC.Files.Uploads.showUploadPopup();
        });

        jq("#emptyContainer .emptyContainer_upload").click(function() {
            ASC.Files.Uploads.showUploadPopup();
            return false;
        });

        jq("#files_okUpload").click(function() {
            ASC.Files.Uploads.showUploadedFiles();
            PopupKeyUpActionProvider.CloseDialog();
        });

    });
})(jQuery);