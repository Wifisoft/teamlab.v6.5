jq(document).ready(function() {
    jq.dropdownToggle({
        switcherSelector: "#showDocumentPanel, #createFirstDocument, #firstDocComb",
        anchorSelector: ".newDocComb:visible",
        dropdownID: "files_newDocumentPanel"
    });
    jq.dropdownToggle({
        switcherSelector: "#emptyDocumentPanel .hintCreate",
        dropdownID: "files_hintCreatePanel",
        fixWinSize: false
    });

    jq.dropdownToggle({
        switcherSelector: "#emptyDocumentPanel .hintUpload",
        dropdownID: "files_hintUploadPanel",
        fixWinSize: false
    });

    jq.dropdownToggle({
        switcherSelector: "#emptyDocumentPanel .hintOpen",
        dropdownID: "files_hintOpenPanel",
        fixWinSize: false
    });

    jq.dropdownToggle({
        switcherSelector: "#emptyDocumentPanel .hintEdit",
        dropdownID: "files_hintEditPanel",
        fixWinSize: false
    });
});

window.Attachments = (function() {
    var moduleName,
        isInit = false,
        isLoaded = false,
        entityId = null,
        projectId = null,
        rootFolderId = null,
        entityType = "",
        banOnEditingFlag = false,
        characterString = "@#$%&*+:;\"'<>?|\/",
        characterRegExp = new RegExp("[@#$%&*\+:;\"'<>?|\\\\/]", 'gim');

    var replaceSpecCharacter = function(str) {
        return str.replace(characterRegExp, '_');
    };
    var checkCharacter = function(input) {
        jq(input).unbind("keyup").bind("keyup", function() {
            var str = jq(this).val();
            if (str.search(characterRegExp) != -1) {
                jq(this).val(replaceSpecCharacter(str));
                jq("#wrongSign").show();
                setInterval('jq("#wrongSign").hide();', 15000);
            }
        });
    };

    var paintLines = function() {
        jq("#attachmentsContainer tr.even").removeClass("even");
        jq("#attachmentsContainer tr:even").addClass("even");
    };
    var getRootFolder = function() {
        switch (moduleName) {
            case "projects":
                {
                    Teamlab.getPrjProjectFolder(null, projectId, function() { onGetRootFolder(arguments); });
                    break;
                }
            case "crm":
                {
                    Teamlab.getCrmFolder(null, "root", function() { onGetRootFolder(arguments); });
                    break;
                }
            default:
                alert("Error module name!!!");
        }
    };

    var getEntityFiles = function() {
        switch (moduleName) {
            case "projects":
                {
                    Teamlab.getPrjEntityFiles(null, entityId, entityType, function() { onGetFiles(arguments); });
                    break;
                }
            case "crm":
                {
                    Teamlab.getCrmEntityFiles(null, entityId, entityType, function() { onGetFiles(arguments); });
                    break;
                }
            default:
                alert("Error module name!!!");
        }
    };
    var loadFiles = function() {
        if (!isLoaded) {
            LoadingBanner.displayLoading();
            getEntityFiles();
        }
    };
    var checkEditingSupport = function() {
        var listTypes = "";
        listTypes += ASC.Files.Utility.CanWebEdit(".docx") ? "" : "#files_create_text,";
        listTypes += ASC.Files.Utility.CanWebEdit(".xlsx") ? "" : "#files_create_spreadsheet,";
        listTypes += ASC.Files.Utility.CanWebEdit(".pptx") ? "" : "#files_create_presentation,";
        listTypes += ASC.Files.Utility.CanWebEdit(".svg") ? "" : "#files_create_picture,";

        jq(listTypes).hide();
        jq(listTypes).addClass('noSupported');
        if (jq("#files_newDocumentPanel ul li.noSupported").length == 4) {
            jq('#createFirstDocument, #firstDocComb, #showDocumentPanel').hide();
        }
    };
    var init = function() {
        if (!isInit) {
            isInit = true;

            checkEditingSupport();

            entityId = jq.getURLParam("id");

            var projId = jq(".wrapperFilesContainer").attr("projectId");
            var module = jq(".wrapperFilesContainer").attr("moduleName");

            if (projId != "0") projectId = projId;
            if (module != "") moduleName = module;

            entityType = jq(".wrapperFilesContainer").attr("entityType");
            if (!jq.browser.mobile) {
                var warnText = jq(".infoPanelAttachFile #wrongSign").text() + " " + characterString;
                jq(".infoPanelAttachFile #wrongSign").text(warnText);
            } else {
                jq("#emptyDocumentPanel .emptyScrBttnPnl").remove();
                jq(".infoPanelAttachFile").remove();
                jq(".containerAction").remove();
            }
        }
        jq("#newDocTitle").live("keypress", function(evt) {
            if (evt.keyCode == 13) {
                createFile();
            } else {
                if (evt.keyCode == 27) {
                    removeNewDocument();
                } else {
                    checkCharacter(jq("#newDocTitle"));
                }
            }
        });
        jq('#attachProjDocuments').live('click', function(event) {
            ProjectDocumentsPopup.showPortalDocUploader();
            return false;
        });
        jq('#questionWindowAttachments #noButton').bind('click', function() {
            jq.unblockUI();
            return false;
        });
    };
    var hideNewFileMenu = function() {
        jq("#files_newDocumentPanel").hide();
    };

    var imageScale = function(windowResize) {
        var listBigImage = jq("div[id^='imgZoom_'] img");
        if (windowResize) {
            jq("div[id^='imgZoom_'] img").removeClass('scale');
        }
        var screenWidth = document.documentElement.clientWidth;
        var screenHeight = document.documentElement.clientHeight;
        for (var i = 0; i < listBigImage.length; i++) {
            var img = listBigImage[i];
            if (!jq(img).hasClass('scale')) {
                var imgWidth = jq(img).parent().width();
                var imgHeight = jq(img).parent().height();
                if (screenWidth <= imgWidth) {
                    imgWidth = screenWidth - 100;
                    jq(img).width(imgWidth);
                    jq(img).addClass('scale');

                    if (screenHeight <= imgHeight) {
                        imgHeight = screenHeight - 100;
                        jq(img).height(imgHeight);
                        jq(img).addClass('scale');
                    }
                } else {
                    if (screenHeight <= imgHeight) {
                        imgHeight = screenHeight - 100;
                        jq(img).height(imgHeight);
                        jq(img).addClass('scale');
                    }
                }

            }
        }
    };

    var initFancyBox = function() {
        jQuery("a.fancyzoom").fancybox({
            cyclic: true,
            centerOnScroll: true,
            hideOnContentClick: true,
            scrolling: 'no',
            onStart: function() { imageScale(true); }
        });
    };

    var createAjaxUploader = function(buttonId) {
        if (!jq.browser.mobile) {
            if (moduleName == 'crm') {
                var ajaxUploader = Teamlab.createCrmUploadFile(
                    null,
                    entityType, entityId,
                    {
                        buttonId: buttonId,
                        autoSubmit: true
                    },
                    {
                        before: LoadingBanner.displayLoading,
                        error: function(params, errors) { onError(errors); },
                        success: onUploadFiles
                    }
                );
            } else {
                var ajaxUploader = Teamlab.createDocUploadFile(
                    null,
                    rootFolderId,
                    {
                        buttonId: buttonId,
                        autoSubmit: true,
                        data: {
                            createNewIfExist: true
                        }
                    },
                    {
                        before: LoadingBanner.displayLoading,

                        error: function(params, errors) { onError(errors); },

                        success: onUploadFiles
                    }
                );
            }
        }
        return;
    };

    var showQuestionWindow = function(fileId) {
        jq('#questionWindowAttachments #okButton').unbind('click');
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({
            message: jq("#questionWindowAttachments"),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '400px',

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-200px',
                'margin-top': margintop,
                'background-color': 'White'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            baseZ: 666,

            fadeIn: 0,
            fadeOut: 0,

            onBlock: function() {
            }
        });
        jq('#questionWindowAttachments #okButton').bind('click', function() {
            jq.unblockUI();
            jq(document).trigger("deleteFile", fileId);
            return false;
        });
    };

    var createNewDocument = function(type) {
        hideNewFileMenu();
        jq("#emptyDocumentPanel").hide();
        jq("#attachmentsContainer tr.newDoc").remove();

        var tdClass = ASC.Files.Utility.getCssClassByFileTitle(type, true);
        var tmpl = { tdclass: tdClass, type: type };
        var htmlNewDoc = jq("#newFileTmpl").tmpl(tmpl);
        jq("#attachmentsContainer tbody").prepend(htmlNewDoc);
        jq("#attachmentsContainer tr.newDoc").show();
        jq("#newDocTitle").focus().select();
        paintLines();
    };
    var removeNewDocument = function() {
        jq("#attachmentsContainer tr.newDoc").remove();
        if (jq("#attachmentsContainer tbody tr").length == 0) {
            jq("#emptyDocumentPanel").show();
        }
    };
    var createFile = function() {
        var hWindow = null;
        hWindow = window.open('');
        var title = jq("#newDocTitle").val();
        if (jq.trim(title) == "") {
            title = jq("#newDocTitle").attr("data");
        }
        var ext = jq(".createFile").attr("id");
        title = title + ext;
        Teamlab.addDocFile(
            { handler: hWindow },
            rootFolderId,
            "file",
            { title: title, content: '', createNewIfExist: true },
            function() { onCreateFile(arguments); },
            { error: function(params) { params.handler.close(); } }
        );
        removeNewDocument();
        paintLines();
    };

    var createFileTmpl = function(fileData) {
        var fileName = decodeURIComponent(fileData.title);

        var exttype = ASC.Files.Utility.getCssClassByFileTitle(fileName, true);

        var access = fileData.access;

        if (ASC.Files.Utility.CanImageView(fileName)) {
            type = "image";
        } else {
            if (ASC.Files.Utility.CanWebEdit(fileName)) {
                type = "editedFile";
            } else {
                if (ASC.Files.Utility.CanWebView(fileName)) {
                    type = "viewedFile";
                } else {
                    type = "noViewedFile";
                }
            }
        }
        var fileId = fileData.id;
        var viewUrl = fileData.viewUrl;
        var version = parseInt(fileData.version);
        if (!version) {
            version = 1;
            viewUrl = ASC.Files.Utility.GetFileViewUrl(fileId, version);
        }
        var downloadUrl = ASC.Files.Utility.GetFileDownloadUrl(fileId);
        var docViewUrl = ASC.Files.Utility.GetFileWebViewerUrl(fileId);
        var editUrl = ASC.Files.Utility.GetFileWebEditorUrl(fileId);
        var fileStatus = fileData.fileStatus;

        var fileTmpl = { title: fileName, access: access, type: type, exttype: exttype, id: fileId, version: version, ViewUrl: viewUrl, downloadUrl: downloadUrl, editUrl: editUrl, docViewUrl: docViewUrl, fileStatus: fileStatus };

        return fileTmpl;
    };
    var isAddedFile = function(title, fileId) {
        var listAttachFiles = jq("#attachmentsContainer tbody tr td:first-child");
        for (var i = 0; i < listAttachFiles.length; i++) {
            var fileName = jq.trim(jq(listAttachFiles[i]).children("a").children(".attachmentsTitle").text());
            var id = jq(listAttachFiles[i]).attr("id").split("_")[1];
            if (fileName == title && id == fileId) {
                return listAttachFiles[i];
            }
        }
        return false;
    };

    var deleteFileFromLayout = function(fileId) {
        if (isLoaded) {
            var td = jq("#af_" + fileId);
            jq(td).parent().remove();
            var files = jq("#attachmentsContainer tr");
            if (!files.length) {
                jq("#attachmentsContainer tbody").empty();
                jq("#emptyDocumentPanel").show();
                jq(".containerAction").hide();
                createAjaxUploader("uploadFirstFile");
                return;
            }
            paintLines();
        }
    };

    var deattachFile = function(fileId) {
        if (moduleName == "crm") {
            showQuestionWindow(fileId);
        } else {
            jq(document).trigger("deleteFile", fileId);
        }
    };
    var appendToListAttachFiles = function(listFiles) {
        jq("#emptyDocumentPanel").hide();
        jq(".containerAction").show();
        jq("#attachmentsContainer tbody").prepend(jq("#fileAttachTmpl").tmpl(listFiles));

        jq("#attachmentsContainer tbody tr").show();
        initFancyBox();
        paintLines();
        if (entityType == "CRM") {
            jq("#attachmentsContainer tr td .unlinkDoc").remove();
        } else {
            jq("#attachmentsContainer tr td .deleteCrmFile").remove();
        }
        LoadingBanner.hideLoading();
    };

    var publicAppendToListAttachFiles = function(listFiles) {
        if (isLoaded) {
            jq("#emptyDocumentPanel").hide();
            jq(".containerAction").show();
            var listFileTempl = new Array();
            for (var i = 0; i < listFiles.length; i++) {
                var fileTmpl = createFileTmpl(listFiles[i]);
                listFileTempl.push(fileTmpl);
            }
            jq("#attachmentsContainer tbody").prepend(jq("#fileAttachTmpl").tmpl(listFileTempl));

            jq("#attachmentsContainer tbody tr").show();
            initFancyBox();
            paintLines();
            if (entityType == "CRM") {
                jq("#attachmentsContainer tr td .unlinkDoc").remove();
            } else {
                jq("#attachmentsContainer tr td .deleteCrmFile").remove();
            }
        }
    };

    var showAttachedFiles = function(documents) {
        var files = new Array();
        for (var i = 0; i < documents.length; i++) {
            var file = createFileTmpl(documents[i]);
            files.push(file);
        }
        if (files.length == 0) {
            jq("#attachmentsContainer tbody").empty();
            jq("#emptyDocumentPanel").show();
            jq(".containerAction").hide();
            LoadingBanner.hideLoading();
            return;
        }
        jq("#attachmentsContainer tbody").empty();
        appendToListAttachFiles(files);
        LoadingBanner.hideLoading();
    };

    var showAddedFile = function(files) {
        if (files.length) {
            jq(".containerAction").show();
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var elem = isAddedFile(file.title, file.id);
                if (elem) {
                    jq(elem).parents("tr").remove();
                } else {
                    jq(document).trigger("addFile", file);
                }
                appendToListAttachFiles(createFileTmpl(file));
            }
        }
        LoadingBanner.hideLoading();
    };
    var editDocument = function(fileId) {
        var tr = jq("#af_" + fileId).parent();
        tr.find(".unlinkDoc").remove();
        tr.find(".deleteCrmFile").remove();
        tr.find("[id^='editDoc_']").remove();

        tr.find(".editingDoc").show();

        var editUrl = ASC.Files.Utility.GetFileWebEditorUrl(fileId);
        window.open(editUrl, "_blank");
    };

    var bind = function(eventName, handler) {
        jq(document).bind(eventName, handler);
    };

    var banOnEditing = function() {
        banOnEditingFlag = true;
    };
    var onError = function(error) {
        LoadingBanner.hideLoading();

        jq("#errorFileUpload").text(error[0]).show();
        setInterval('jq("#errorFileUpload").hide();', 15000);
    };
    var onGetFiles = function(response) {
        isLoaded = true;
        showAttachedFiles(response[1]);

        if (response[1].length == 0) {
            getRootFolder();
        } else {
            jq(document).trigger("loadAttachments", response[1].length);
            switch (moduleName) {
                case "crm":
                    {
                        rootFolderId = response[1][0].parentId;
                        createAjaxUploader('linkNewDocumentUpload');
                        break;
                    }
                default:
                    {
                        rootFolderId = response[1][0].parentId;
                        createAjaxUploader('linkNewDocumentUpload');
                        ProjectDocumentsPopup.init(rootFolderId);
                    }
            }
        }
        if(banOnEditingFlag) {
            jq(".containerAction").remove();
            jq("#emptyDocumentPanel .emptyScrBttnPnl").remove();
            jq("#attachmentsContainer").find(".unlinkDoc").remove();
        }
    };

    var onGetRootFolder = function(response) {
        switch (moduleName) {
            case "projects":
                {
                    var project = response[1];
                    rootFolderId = project.id;
                    createAjaxUploader('uploadFirstFile');
                    ProjectDocumentsPopup.init(rootFolderId);
                    ProjectDocumentsPopup.onGetFolderFiles(response);
                    break;
                }
            case "crm":
                {
                    rootFolderId = response[1].id;
                    createAjaxUploader('uploadFirstFile');
                    break;
                }
            default:
                alert("Error module name!!!");
        }
    };

    var onUploadFiles = function(params, file) {
        createAjaxUploader('linkNewDocumentUpload');
        showAddedFile([file]);
    };

    var onCreateFile = function(response) {
        var file = response[1];

        file.fileStatus = 1;

        jq(document).trigger("addFile", file);
        createAjaxUploader('linkNewDocumentUpload');

        appendToListAttachFiles(createFileTmpl(file));
        jq(".containerAction").show();
        var mass = [];
        mass.push(file.id);
        if (response[0].handler.location) {
            response[0].handler.location.href = ASC.Files.Utility.GetFileWebEditorUrl(file.id) + "&new=true";
        }
    };

    return {
        init: init,
        bind: bind,
        loadFiles: loadFiles,
        isLoaded: isLoaded,
        appendToListAttachFiles: appendToListAttachFiles,
        isAddedFile: isAddedFile,
        deattachFile: deattachFile,
        appendFilesToLayout: publicAppendToListAttachFiles,
        deleteFileFromLayout: deleteFileFromLayout,
        createNewDocument: createNewDocument,
        removeNewDocument: removeNewDocument,
        createFile: createFile,
        banOnEditing: banOnEditing,

        editDocument: editDocument
    };
})(jQuery);

jq(document).ready(function() {
    Attachments.init();
});