window.ASC.Files.MainMenu = (function() {
    var isInit = false;

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }

        jq.dropdownToggle({
            switcherSelector: "#files_newdoc_btn, #topNewFile img, #emptyContainer .emptyContainer_create",
            dropdownID: "files_newDocumentPanel",
            anchorSelector: "#topNewFile img"
        });
    };

    var updateCreateDocList = function() {
        var listTypes = "";
        listTypes += ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.document) ? "" : "#files_create_document,";
        listTypes += ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.spreadsheet) ? "" : "#files_create_spreadsheet,";
        listTypes += ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.presentation) ? "" : "#files_create_presentation,";
        listTypes += ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.image) ? "" : "#files_create_image,";

        jq(listTypes).hide();
    };

    return {
        init: init,
        updateCreateDocList: updateCreateDocList
    };
})();

(function($) {
    ASC.Files.MainMenu.init();

    $(function() {
        ASC.Files.MainMenu.updateCreateDocList();

        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.document)) {
            jq("#files_create_document").remove();
            jq("#emptyContainer .emptyContainer_create").remove();
        }
        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.spreadsheet))
            jq("#files_create_spreadsheet").remove();
        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.presentation))
            jq("#files_create_presentation").remove();
        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Constants.typeNewDoc.image))
            jq("#files_create_image").remove();

        if (jq("#files_newDocumentPanel ul li").length == 0) {
            jq("#files_newDocumentPanel").remove();
            jq("#topNewFile").closest("li").remove();
        }

        jq("#files_create_document, #files_create_spreadsheet, #files_create_presentation, #files_create_image").click(function() {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.typeNewDoc = this.id.replace("files_create_", "");
            ASC.Files.Folders.createNewDoc();
        });

    });
})(jQuery);