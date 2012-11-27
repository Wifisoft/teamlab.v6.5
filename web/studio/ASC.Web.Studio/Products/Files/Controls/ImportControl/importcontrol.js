window.ASC.Files.Import = (function() {
    var importStatus = false;
    var isImport = false;
    var isInit = false;

    var init = function() {
        if (isInit === false) {
            isInit = true;

            serviceManager.bind(ASC.Files.TemplateManager.events.IsZohoAuthentificated, onIsZohoAuthentificated);
            serviceManager.bind(ASC.Files.TemplateManager.events.GetImportData, onGetImportData);
            serviceManager.bind(ASC.Files.TemplateManager.events.ExecImportData, ASC.Files.EventHandler.onGetTasksStatuses);

            jq(document).click(function(event) {
                jq.dropdownToggle().registerAutoHide(event, "#topImport", "#importListPanel");
            });
        }
    };

    var canStart = function() {
        if (ASC.Files.Import.importStatus) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoImprotNotFinish, true);
            return false;
        }
        return true;
    };

    var selectEventBySource = function(source) {
        switch (source) {
        case "boxnet":
            return ASC.Files.Import.importBoxnet;
        case "google":
            return ASC.Files.Import.importGoogle;
        case "zoho":
            return ASC.Files.Import.importZoho;
        default:
            return ASC.Files.Import.showImportPanel;
        }
    };

    var importBoxnet = function() {
        ASC.Files.Actions.hideAllActionPanels();
        if (canStart()) {
            OAuthCallback = ASC.Files.Import.getImportData;
            OAuthPopup(ASC.Files.Constants.URL_OAUTH_BOXNET);
        }
    };

    var importGoogle = function() {
        ASC.Files.Actions.hideAllActionPanels();
        if (canStart()) {
            OAuthCallback = ASC.Files.Import.getImportData;
            OAuthPopup(ASC.Files.Constants.URL_OAUTH_GOOGLE);
        }
    };

    var importZoho = function() {
        ASC.Files.Actions.hideAllActionPanels();
        if (canStart())
            showSubmitLoginDialog();
    };

    var showImportPanel = function() {
        ASC.Files.Actions.hideAllActionPanels();
        if (ASC.Files.Import.importStatus) {
            ASC.Files.Common.blockUI(jq("#ImportDialog"), 900, 540);
        } else {
            jq.dropdownToggle().toggle("#topImport img", "importListPanel");
        }
    };

    var setFolderImportTo = function(idTo, folderName) {
        jq("#files_importToFolder").text(folderName);
        jq("#files_importToFolderId").val(idTo);

        ASC.Files.Actions.hideAllActionPanels();
    };

    var showSubmitLoginDialog = function() {
        jq("#files_pass").val("");
        jq("#files_login, #files_pass").removeAttr("disabled");
        jq("#ImportLoginDialog .action-block").show();
        jq("#ImportLoginDialog .ajax-info-block").hide();
        jq("#ImportLoginDialog div[id$='InfoPanel']").hide();

        ASC.Files.Common.blockUI(jq("#ImportLoginDialog"), 400, 500);
        PopupKeyUpActionProvider.EnterAction = 'jq("#files_submitLoginDialog").click();';
        PopupKeyUpActionProvider.CloseDialogAction = 'jq("#files_pass").val("");';

        jq("#files_login").focus();
    };

    var submitLoginDialog = function() {
        var login = jq("#files_login").val().trim();
        var password = jq("#files_pass").val().trim();

        var infoBlock = jq("#ImportLoginDialog div[id$='InfoPanel']").hide();

        if (login == "" || password == "") {
            infoBlock.show().find("div").text(ASC.Files.FilesJSResources.ErrorMassage_FieldsIsEmpty);
            return;
        }

        jq("#files_pass").val("");

        jq("#files_login, #files_pass").attr("disabled", "disabled");
        jq("#ImportLoginDialog .action-block").hide();
        jq("#ImportLoginDialog .ajax-info-block").show();

        ASC.Files.Import.getImportData("", "zoho", login, password);
    };

    var execImportData = function(params) {
        var checkedDocuments = jq("#ImportDialog input[name='checked_document']:checked");
        var infoBlock = jq("#ImportDialog div[id$='InfoPanel']");

        if (checkedDocuments.length == 0) {
            if (infoBlock.css("display") == "none")
                infoBlock
                    .removeClass("infoPanel")
                    .addClass("errorBox")
                    .css("margin", "10px 16px 0")
                    .show();

            infoBlock.find("div").text(ASC.Files.FilesJSResources.EmptyListSelectedForImport);

            return;
        }

        var dataToSend = "<ArrayOfDataToImport>";
        Encoder.EncodeType = "!entity";
        checkedDocuments.each(function() {
            var cells = jq(this).closest("tr").find("td");
            dataToSend += "<DataToImport>";
            dataToSend += "<content_link>" + Encoder.htmlEncode(jq("<div/>").text(jq(this).val().trim()).html()) + "</content_link>";
            dataToSend += "<title>" + Encoder.htmlEncode(jq(cells[1]).text().trim()) + "</title>";
            dataToSend += "</DataToImport>";
        });
        Encoder.EncodeType = "entity";

        dataToSend += "</ArrayOfDataToImport>";

        params.tofolder = jq("#files_importToFolderId").val();
        params.ignoreCoincidenceFiles = jq("#ImportDialog input[name='file_conflict']:checked").val();

        var importProgressPanel = jq("#ImportDialog div.import_progress_panel");

        importProgressPanel.find(".studioFileUploaderProgressBar").width(0);
        importProgressPanel.find("span.percent").width(0);
        importProgressPanel.show();

        jq("#ImportDialog input:checkbox").attr("disabled", "disabled");
        jq("#ImportDialog input[name='file_conflict']").attr("disabled", "disabled");
        jq("#files_importToFolderBtn").css("visibility", "hidden");

        jq("#ImportDialog .action-block").hide();
        jq("#ImportDialog .action-block-progress").show();
        infoBlock.hide();

        requestImportData(params, dataToSend);
    };

    var showImportToSelector = function() {
        ASC.Files.Import.isImport = true;

        ASC.Files.Tree.showTreePath();
        ASC.Files.Tree.showSelect(jq("#files_importToFolderId").val());

        ASC.Files.Actions.hideAllActionPanels();

        jq("#files_treeViewPanelSelector").addClass("without-third-party");

        jq.dropdownToggle().toggle("#files_importToFolderBtn", "files_treeViewPanelSelector");
        jq("body").bind("click", ASC.Files.Tree.registerHideTree);
    };

    var cancelImportData = function(text, isError) {
        ASC.Files.Import.importStatus = false;
        var idTo = jq("#files_importToFolderId").val();
        ASC.Files.Tree.resetNode(idTo);

        if (jq("#ImportDialog:visible").length != 0) {
            PopupKeyUpActionProvider.CloseDialog();
            ASC.Files.Folders.navigationSet(idTo);
        }

        ASC.Files.UI.displayInfoPanel(text, isError === true);
    };

    //request

    var getImportData = function(token, source, login, password) {
        var authData =
            {
                login: login,
                password: password,
                token: token
            };

        var params =
            {
                source: source,
                showLoading: (source != "zoho"),
                authData: authData
            };

        serviceManager.request("post",
            "xml",
            (source != "zoho"
                ? ASC.Files.TemplateManager.events.GetImportData
                : ASC.Files.TemplateManager.events.IsZohoAuthentificated),
            params,
            { AuthData: authData },
            "import?source=" + source);
    };

    var requestImportData = function(params, data) {
        params.showLoading = true;

        serviceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.ExecImportData,
            params,
            data,
            "import",
            "exec?"
                + "login=" + (params.authData.login || "")
                    + "&password=" + (params.authData.password || "")
                        + "&token=" + (params.authData.token || "")
                            + "&source=" + params.source
                                + "&tofolder=" + params.tofolder
                                    + "&ignoreCoincidenceFiles=" + params.ignoreCoincidenceFiles);
    };

    //event handler
    var onIsZohoAuthentificated = function(xmlData, params, errorMessage, commentMessage) {
        var infoBlock = jq("#ImportLoginDialog div[id$='InfoPanel']").hide();

        jq("#files_pass").val("");

        if (typeof errorMessage != "undefined") {
            infoBlock.show().find("div").text(commentMessage || errorMessage);
            jq("#files_login, #files_pass").removeAttr("disabled");
            jq("#ImportLoginDialog .action-block").show();
            jq("#ImportLoginDialog .ajax-info-block").hide();
            return undefined;
        }

        infoBlock.show().find("div").text(ASC.Files.FilesJSResources.ErrorMassage_AuthentificatedFalse);

        jq("#files_login, #files_pass").removeAttr("disabled");
        jq("#ImportLoginDialog .action-block").show();
        jq("#ImportLoginDialog .ajax-info-block").hide();

        onGetImportData(xmlData, params, errorMessage, commentMessage);
    };

    var onGetImportData = function(xmlData, params, errorMessage, commentMessage) {
        PopupKeyUpActionProvider.CloseDialog();

        if (typeof errorMessage != "undefined" || xmlData == null) {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        if (xmlData.getElementsByTagName("DataToImportList").length == 0) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.EmptyDataToImport, true);
            return;
        }

        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getImportData);
        var htmlXML = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        jq("#import_data").html(htmlXML);

        jq("#files_startImportData").unbind("click").click(function() {
            ASC.Files.Import.execImportData(params);
            return false;
        });

        var importToFolderId = ASC.Files.Constants.FOLDER_ID_MY_FILES;
        var toFolderTitle = ASC.Files.Tree.getFolderTitle(importToFolderId);

        if (ASC.Files.UI.accessibleItem()
            && (!ASC.Files.ThirdParty || !ASC.Files.ThirdParty.isThirdParty())
            && ASC.Files.Folders.currentFolderId != ASC.Files.Constants.FOLDER_ID_TRASH) {
            importToFolderId = ASC.Files.Folders.currentFolderId;
            toFolderTitle = "";
            for (var i = 0; i < ASC.Files.Tree.pathParts.length; i++) {
                if (i != 0) toFolderTitle += " > ";
                toFolderTitle += ASC.Files.Tree.pathParts[i].Value;
            }
        }
        ASC.Files.Import.setFolderImportTo(importToFolderId, toFolderTitle);

        var resourceImport;
        switch (params.source) {
        case "boxnet":
            resourceImport = ASC.Files.FilesJSResources.ImportFromBoxNet;
            break;
        case "google":
            resourceImport = ASC.Files.FilesJSResources.ImportFromGoogle;
            break;
        case "zoho":
            resourceImport = ASC.Files.FilesJSResources.ImportFromZoho;
            break;
        default:
            PopupKeyUpActionProvider.CloseDialog();
        }
        jq("#ImportDialogHeader, #import_to_folderName").text(resourceImport);

        jq("#ImportDialog input:checkbox").removeAttr("disabled");
        jq("#ImportDialog input[name='file_conflict']").removeAttr("disabled");
        jq("#files_importToFolderBtn").css("visibility", "visible");

        jq("#ImportDialog .action-block").show();
        jq("#ImportDialog .action-block-progress").hide();

        jq("#ImportDialog .import_progress_panel").hide();
        jq("#ImportDialog .studioFileUploaderProgressBar").width(0);
        jq("#ImportDialog span.percent").text("0");
        jq("#ImportDialog div[id$='InfoPanel']").hide();

        jq(".startHide", "#ImportDialog").removeClass("startHide");

        PopupKeyUpActionProvider.EnterAction = 'jq("#files_startImportData").click();';

        ASC.Files.Common.blockUI(jq("#ImportDialog"), 900, 540);

    };

    var onGetImportStatus = function(data, isTerminate) {
        ASC.Files.Import.importStatus = true;

        try {
            var progress = parseInt(data.progress) || 0;
        } catch(e) {
            progress = 0;
        }

        if (jq("#ImportDialogHeader").text().length == 0) {
            var resourceImport = "";
            switch (data.source) {
            case "boxnet":
                resourceImport = ASC.Files.FilesJSResources.ImportFromBoxNet;
                break;
            case "google":
                resourceImport = ASC.Files.FilesJSResources.ImportFromGoogle;
                break;
            case "zoho":
                resourceImport = ASC.Files.FilesJSResources.ImportFromZoho;
                break;
            default:
                PopupKeyUpActionProvider.CloseDialog();
            }
            jq("#ImportDialogHeader").text(resourceImport);
        }

        if (progress > 0) {
            jq("#ImportDialog .studioFileUploaderProgressBar").css("width", progress + "%");

            jq("#ImportDialog span.percent").text(progress);
        }

        if (progress == 100) {
            ASC.Files.Import.cancelImportData(data.error || (isTerminate === true ? ASC.Files.FilesJSResources.InfoCancelImport : ASC.Files.FilesJSResources.InfoFinishImport), data.error != null);
            return true;
        }

        return false;
    };

    return {
        init: init,

        selectEventBySource: selectEventBySource,

        importBoxnet: importBoxnet,
        importGoogle: importGoogle,
        importZoho: importZoho,
        showImportPanel: showImportPanel,

        submitLoginDialog: submitLoginDialog,
        getImportData: getImportData,
        execImportData: execImportData,
        cancelImportData: cancelImportData,
        showImportToSelector: showImportToSelector,

        setFolderImportTo: setFolderImportTo,
        isImport: isImport,
        importStatus: importStatus,
        onGetImportStatus: onGetImportStatus
    };
})();

(function($) {
    ASC.Files.Import.init();

    $(function() {
        jq("#ImportLoginDialog div[id$='InfoPanel']")
            .removeClass("infoPanel")
            .addClass("errorBox")
            .css("margin", "10px 16px 0");

        jq("#files_important_btn, #topImport img").click(function() {
            ASC.Files.Import.showImportPanel();
        });

        jq("#import_from_boxnet").click(function() {
            ASC.Files.Import.importBoxnet();
            return false;
        });

        jq("#import_from_google").click(function() {
            ASC.Files.Import.importGoogle();
            return false;
        });

        jq("#import_from_zoho").click(function() {
            ASC.Files.Import.importZoho();
            return false;
        });

        jq("#files_submitLoginDialog").click(function() {
            ASC.Files.Import.submitLoginDialog();
            return false;
        });

        jq("#files_importToFolderBtn").click(function() {
            ASC.Files.Import.showImportToSelector();
            return false;
        });

        jq("#files_import_minimize").click(function() {
            PopupKeyUpActionProvider.CloseDialog();
            return false;
        });

        jq("#files_import_terminate").click(function(event) {
            ASC.Files.Folders.terminateTasks(true);
            ASC.Files.Common.cancelBubble(event);
            return false;
        });

        jq("#ImportDialog").on("click", "input[name='all_checked_document']", function() {
            var value = jq(this).is(":checked");
            jq("#ImportDialog input[name='checked_document']").attr("checked", value);
        });

        jq("#ImportDialog").on("click", "input[name='checked_document']", function() {
            jq("#ImportDialog input[name='all_checked_document']").attr("checked",
                jq("#ImportDialog input[name='checked_document']:not(:checked)").length == 0);
        });

    });
})(jQuery);