;
window.ASC.Files.Editor = (function () {
    var isInit = false;

    var docIsChanged = false;

    var docEditor = null;
    var docServiceParams = null;

    var trackEditTimeout = null;
    var fileSaveAsNew = "";
    var shareLink = "";
    var docKeyForTrack = "";
    var serverErrorMessage = null;

    var init = function () {
        if (isInit === false) {
            isInit = true;
        }

        jq("body").css("overflow-y", "hidden");

        window.onbeforeunload = ASC.Files.Editor.finishEdit;

        serviceManager.bind("TrackEditFile", completeTrack);
        serviceManager.bind("CanEditFile", completeCanEdit);
    };

    var fixSize = function () {
        var wrapEl = document.getElementById("wrap");
        if (wrapEl) {
            wrapEl.style.height = screen.availHeight + "px";
            window.scrollTo(0, -1);
            wrapEl.style.height = window.innerHeight + "px";
        }
    };

    var readyEditor = function () {
        if (ASC.Files.Editor.serverErrorMessage) {
            docEditorShowError("TeamLab", ASC.Files.Editor.serverErrorMessage);
            return;
        }

        if (checkMessageFromHash()) {
            return;
        }

        if (ASC.Files.Editor.docServiceParams && ASC.Files.Editor.docServiceParams.mode === "edit") {
            ASC.Files.Editor.trackEdit();
        }
    };

    var backEditor = function () {
        location.href = ASC.Files.Editor.docServiceParams.folderurl;
    };

    var documentStateChangeEditor = function (event) {
        if (docIsChanged != event.data) {
            document.title = ASC.Files.Editor.docServiceParams.file.title + (event.data ? " *" : "");
            docIsChanged = event.data;
        }
    };

    var errorEditor = function () {
        ASC.Files.Editor.finishEdit();
    };

    var saveEditor = function (event) {
        var urlSavedDoc = event.data;
        var urlAjax = ASC.Files.Constants.URL_HANDLER_SAVE.format(
            ASC.Files.Editor.docServiceParams.file.id,
            ASC.Files.Editor.docServiceParams.file.version,
            encodeURIComponent(urlSavedDoc));
        urlAjax += ASC.Files.Editor.fileSaveAsNew;
        urlAjax += ASC.Files.Editor.ShareLink;
        urlAjax += "&_=" + new Date().getTime();

        jq.ajax({
            type: "get",
            url: urlAjax,
            complete: completeSave
        });
    };

    var requestEditRightsEditor = function () {
        serviceManager.canEditFile("CanEditFile",
            {
                fileID: ASC.Files.Editor.docServiceParams.file.id,
                shareLink: ASC.Files.Editor.ShareLink
            });
    };

    var trackEdit = function () {
        clearTimeout(trackEditTimeout);

        serviceManager.trackEditFile("TrackEditFile",
            {
                fileID: ASC.Files.Editor.docServiceParams.file.id,
                docKeyForTrack: ASC.Files.Editor.docKeyForTrack,
                shareLink: ASC.Files.Editor.ShareLink
            });
    };

    var finishEdit = function () {
        if (trackEditTimeout !== null) {
            serviceManager.trackEditFile("FinishTrackEditFile",
                {
                    fileID: ASC.Files.Editor.docServiceParams.file.id,
                    docKeyForTrack: ASC.Files.Editor.docKeyForTrack,
                    shareLink: ASC.Files.Editor.ShareLink,
                    finish: true,
                    ajaxsync: true
                });
        }
    };

    var completeSave = function () {
        try {
            var responseText = jq.parseJSON(arguments[0].responseText);
        } catch (e) {
            responseText = arguments[0].responseText.split("title>")[1].split("</")[0];
        }

        if (arguments[1] == "error" || responseText && responseText.error) {
            var errorMessage = responseText.message || responseText;
            docEditorShowError("Save Error", errorMessage);
            ASC.Files.Editor.docEditor.clearCache();
        } else {
            ASC.Files.Editor.documentStateChangeEditor({data: false});
            ASC.Files.Editor.trackEdit();
        }
    };

    var completeTrack = function (jsonData, params, errorMessage) {
        clearTimeout(trackEditTimeout);
        if (typeof errorMessage != "undefined") {
            docEditorShowError("Error", errorMessage);
            return;
        }
        trackEditTimeout = setTimeout(ASC.Files.Editor.trackEdit, 5000);
    };

    var docEditorShowError = function (title, message) {
        ASC.Files.Editor.docEditor.showError(title, message);
    };

    var checkMessageFromHash = function () {
        var regExpError = /^#error\/(\S+)?/ ;
        if (regExpError.test(location.hash)) {
            var errorString = regExpError.exec(location.hash)[1];
            errorString = decodeURIComponent(errorString).replace( /\+/g , " ");
            docEditorShowError("TeamLab", errorString);
            return true;
        }
        return false;
    };

    var completeCanEdit = function (jsonData, params, errorMessage) {
        jsonData = jsonData === true;

        // occurs whenever the user tryes to enter edit mode
        ASC.Files.Editor.docEditor.applyEditRights(jsonData, errorMessage);

        if (jsonData) {
            ASC.Files.Editor.trackEdit();
        }
    };

    var createFrameEditor = function () {
        jq("#iframeEditor").parents().css("height", "100%").removeClass("clearFix");

        if (ASC.Files.Editor.docServiceParams) {
            var documentConfig =
                {
                    title: ASC.Files.Editor.docServiceParams.file.title,
                    url: ASC.Files.Editor.docServiceParams.url,
                    fileType: ASC.Files.Editor.docServiceParams.filetype,
                    key: ASC.Files.Editor.docServiceParams.key,
                    vkey: ASC.Files.Editor.docServiceParams.vkey,

                    outputType: ASC.Files.Editor.docServiceParams.outputtype,

                    info: {
                        author: ASC.Files.Editor.docServiceParams.file.modified_by,
                        folder: ASC.Files.Editor.docServiceParams.filepath,
                        created: ASC.Files.Editor.docServiceParams.file.modified_on,

                        sharingSettings: ASC.Files.Editor.docServiceParams.sharingsettings
                    },

                    permissions: {
                        edit: /edit/ .test(ASC.Files.Editor.docServiceParams.buttons),
                        download: /download/ .test(ASC.Files.Editor.docServiceParams.buttons)
                    }
                };

            var configRecent = ASC.Files.Editor.docServiceParams.recent;

            var configTemplates =
                jq(ASC.Files.Editor.docServiceParams.templates).map(
                    function (i, item) {
                        return {
                            name: item.Key,
                            icon: item.Value
                        };
                    }).toArray();

            var editorConfig =
                {
                    mode: ASC.Files.Editor.docServiceParams.mode,
                    canBackToFolder: (ASC.Files.Editor.docServiceParams.folderurl.length > 0),
                    canCreateNew: /create/ .test(ASC.Files.Editor.docServiceParams.buttons),
                    createUrl: ASC.Files.Constants.URL_HANDLER_CREATE,

                    recent: configRecent,

                    templates: configTemplates,

                    lang: ASC.Files.Editor.docServiceParams.lang
                };
            var typeConfig = ASC.Files.Editor.docServiceParams.type || "desktop"; // "desktop" || "mobile"
            var documentTypeConfig = ASC.Files.Editor.docServiceParams.documentType;
        }

        var eventsConfig =
            {
                "onReady": ASC.Files.Editor.readyEditor,
                "onBack": ASC.Files.Editor.backEditor,
                "onDocumentStateChange": ASC.Files.Editor.documentStateChangeEditor,
                "onRequestEditRights": ASC.Files.Editor.requestEditRightsEditor,
                "onSave": ASC.Files.Editor.saveEditor,
                "onError": ASC.Files.Editor.errorEditor
            };

        ASC.Files.Editor.docEditor = new DocsAPI.DocEditor("iframeEditor",
            {
                width: "100%",
                height: "100%",

                type: typeConfig,
                documentType: documentTypeConfig,
                document: documentConfig,
                editorConfig: editorConfig,
                events: eventsConfig
            });
    };

    return {
        init: init,
        createFrameEditor: createFrameEditor,
        fixSize: fixSize,

        docEditor: docEditor,

        //set in .cs
        docServiceParams: docServiceParams,
        fileSaveAsNew: fileSaveAsNew,
        ShareLink: shareLink,
        docKeyForTrack: docKeyForTrack,
        serverErrorMessage: serverErrorMessage,

        trackEdit: trackEdit,
        finishEdit: finishEdit,

        //event
        readyEditor: readyEditor,
        backEditor: backEditor,
        documentStateChangeEditor: documentStateChangeEditor,
        requestEditRightsEditor: requestEditRightsEditor,
        errorEditor: errorEditor,
        saveEditor: saveEditor
    };
})();

(function ($) {
    ASC.Files.Editor.init();
    $(function () {
        ASC.Files.Editor.createFrameEditor();

        if (jq.browser.mobile || ASC.Files.Editor.docServiceParams && ASC.Files.Editor.docServiceParams.type === "mobile")
        {
            window.addEventListener("load", ASC.Files.Editor.fixSize);
            window.addEventListener("orientationchange", ASC.Files.Editor.fixSize);
        }

    });
})(jQuery);