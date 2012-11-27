window.ASC.Files.EmptyScreen = (function() {
    var isInit = false;

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }
        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintCreate",
            dropdownID: "files_hintCreatePanel",
            fixWinSize: false
        });

        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintUpload",
            dropdownID: "files_hintUploadPanel",
            fixWinSize: false
        });

        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintOpen",
            dropdownID: "files_hintOpenPanel",
            fixWinSize: false
        });

        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintEdit",
            dropdownID: "files_hintEditPanel",
            fixWinSize: false
        });
    };

    var displayEmptyScreen = function() {
        var container = ASC.Files.Folders.folderContainer == "trash" || !ASC.Files.UI.accessibleItem();

        jq("#files_mainContent, #switchViewFolder, #mainContentHeader, #files_pageNavigatorHolder").hide();
        jq("#emptyContainer>div").hide();

        var filter = ASC.Files.Filter.getFilterSettings();
        if (filter.filter != 0 || filter.text != "") {
            jq("#emptyContainer_filter").show();
        } else {
            jq("#files_filterContainer").hide();
            if (container === true) {
                jq("#emptyContainer .emptyContainer_upload, #emptyContainer .emptyContainer_create").hide();
            } else {
                jq("#emptyContainer .emptyContainer_upload, #emptyContainer .emptyContainer_create").css("display", "");
            }

            jq("#emptyContainer_" + ASC.Files.Folders.folderContainer).show();
        }

        jq("#emptyContainer").show();
        ASC.Files.UI.toggleMainMenu();
    };

    var hideEmptyScreen = function() {
        jq("#emptyContainer").hide();

        jq("#files_filterContainer").show();
        jq("#files_mainContent, #switchViewFolder, #mainContentHeader").show();
        ASC.Files.UI.toggleMainMenu();
    };

    return {
        init: init,

        hideEmptyScreen: hideEmptyScreen,
        displayEmptyScreen: displayEmptyScreen
    };
})();

(function($) {
    ASC.Files.EmptyScreen.init();
    $(function() {
    });
})(jQuery);