window.ASC.Files.Tree = (function() {
    var isInit = false;
    var rootContainer = "files_trewViewContainer";
    var rootSContainer = "files_trewViewSelector";

    var pathParts = new Array();

    var init = function() {
        if (isInit === false) {
            isInit = true;

            serviceManager.bind(ASC.Files.TemplateManager.events.GetTreeSubFolders, onGetTreeSubFolders);

            jq.dropdownToggle({
                switcherSelector: "#files_breadCrumbsRoot",
                dropdownID: "files_treeViewPanel",
                showFunction: ASC.Files.Tree.showTreePath
            });
        }
    };

    var getTreeNode = function(folderId) {
        return jq("#files_trewViewContainer li.tree_node[data-id='" + folderId + "']");
    };

    var getStreeNode = function(folderId) {
        return jq("#files_trewViewSelector li.stree_node[data-id='" + folderId + "']");
    };

    var getFolderId = function(treeNode) {
        return treeNode.attr("data-id");
    };

    var getFolderTitle = function(folderId) {
        return ASC.Files.Tree.getStreeNode(folderId).children("a").text().trim();
    };

    var registerHideTree = function(event) {
        if (!jq((event.target) ? event.target : event.srcElement).parents().andSelf()
            .is("#files_treeViewPanelSelector, #files_moveto_folders, #files_moveto_files, #files_copyto_folders, #files_copyto_files,\
                 #files_restore_folders, #files_restore_files, #files_movetoButton, #files_copytoButton, #files_restoreButton,\
                 #files_mainMove, #files_mainCopy, #files_mainRestore, #files_importToFolderBtn")) {
            ASC.Files.Actions.hideAllActionPanels();
            jq("body").unbind("click", ASC.Files.Tree.registerHideTree);
        }
    };

    var showTreeViewPanel = function() {
        ASC.Files.Actions.hideAllActionPanels();
        ASC.Files.Tree.showTreePath();
        jq.dropdownToggle().toggle("#files_breadCrumbsRoot", "files_treeViewPanel");
    };

    var showMoveToSelector = function(isCopy) {
        if (ASC.Files.Import)
            ASC.Files.Import.isImport = false;
        ASC.Files.Folders.isCopyTo = (isCopy === true);

        if (!ASC.Files.UI.accessibleItem() && !ASC.Files.Folders.isCopyTo)
            return;

        ASC.Files.Tree.showTreePath();
        ASC.Files.Tree.showSelect(ASC.Files.Folders.currentFolderId);
        ASC.Files.Actions.hideAllActionPanels();

        if (!isCopy
            && jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):not(.onEdit):has(div.checkbox input:checked)").length == 0) {
            return undefined;
        }

        if (ASC.Files.ThirdParty
            && !ASC.Files.ThirdParty.isThirdParty()
            && !ASC.Files.Folders.isCopyTo
            && jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):not(.thirdPartyEntry):has(div.checkbox input:checked)").length == 0) {
            return undefined;
        }

        if (jq("#files_mainContent li.fileRow:not(.checkloading):not(.newFolder):not(.newFile):not(.errorEntry):has(div.checkbox input:checked)").length == 0) {
            return undefined;
        }

        jq("#files_treeViewPanelSelector").removeClass("without-third-party");

        jq.dropdownToggle().toggle("li.menuActionSelectAll", "files_treeViewPanelSelector");

        jq("body").bind("click", ASC.Files.Tree.registerHideTree);
    };

    var showTreePath = function() {
        var treeNode = ASC.Files.Tree.getTreeNode(ASC.Files.Tree.pathParts[0].Key);
        if (treeNode.length != 0) {
            for (var i = 0; i < ASC.Files.Tree.pathParts.length - 1; i++) {
                var folderId = ASC.Files.Tree.pathParts[i].Key;

                treeNode = ASC.Files.Tree.getTreeNode(folderId);

                var treeNodeUl = treeNode.children("ul");
                if (treeNodeUl.html() == null || treeNodeUl.html() == "") {
                    ASC.Files.Tree.getTreeSubFolders(folderId);
                } else {
                    treeNode.addClass("jstree-open").removeClass("jstree-closed");

                    var streeNode = ASC.Files.Tree.getStreeNode(folderId);
                    streeNode.children(".jstree-icon").parent().addClass("jstree-open").removeClass("jstree-closed");
                }
            }
        }
        ASC.Files.Tree.select(ASC.Files.Tree.getTreeNode(ASC.Files.Folders.currentFolderId));
        ASC.Files.Tree.showSelect(ASC.Files.Folders.currentFolderId);
    };

    var renderTreeView = function(idContainers, xmlData) {
        var xslData = ASC.Files.TemplateManager.getTemplate(ASC.Files.TemplateManager.templates.getFoldersTree);
        var htmlData = ASC.Controls.XSLTManager.translate(xmlData, xslData);

        var treeNode = ASC.Files.Tree.getTreeNode(idContainers);
        var streeNode = ASC.Files.Tree.getStreeNode(idContainers);

        treeNode.addClass("jstree-open").removeClass("jstree-closed");
        streeNode.addClass("jstree-open").removeClass("jstree-closed");

        if (htmlData != "") {
            treeNode.removeClass("jstree-empty").append("<ul>" + htmlData + "</ul>");
            var shtmlData = htmlData.replace( /tree_node/g , "stree_node");
            streeNode.removeClass("jstree-empty").append("<ul>" + shtmlData + "</ul>");
        } else {
            treeNode.addClass("jstree-empty");
            streeNode.addClass("jstree-empty");
        }
    };

    var select = function(treeNode) {
        jq("#" + rootContainer + " a").removeClass("selected");
        jq("#" + rootSContainer + " a").removeClass("selected");

        treeNode.children("a").addClass("selected");

        treeNode.parents("#files_trewViewContainer li.jstree-closed")
            .addClass("jstree-open").removeClass("jstree-closed");
    };

    var showSelect = function(folderId) {
        ASC.Files.Tree.getStreeNode(folderId).children("a").addClass("selected");

        ASC.Files.Tree.getTreeNode(folderId + "']")
            .parents("#files_trewViewSelector li.jstree-closed")
            .addClass("jstree-open").removeClass("jstree-closed");
    };

    var selectS = function(treeNode) {
        var folderId = ASC.Files.Tree.getFolderId(treeNode);

        if (ASC.Files.Import && ASC.Files.Import.isImport == true) {
            var importToFolder = "";
            var liArray = treeNode.parents("li.stree_node").andSelf();
            for (var j = 0; j < liArray.length; j++) {
                if (j > 0) importToFolder += " > ";
                importToFolder += jq(liArray[j]).children("a").text().trim();
            }

            ASC.Files.Import.setFolderImportTo(folderId, importToFolder);
        } else {
            var titleDf = ASC.Files.Tree.getStreeNode(folderId).children("a").text().trim();

            var pathDest = ASC.Files.Tree.getStreeNode(folderId).parents("li").map(function() { return ASC.Files.Tree.getFolderId(jq(this)); });
            pathDest = jq.makeArray(pathDest);
            pathDest.unshift(folderId);

            ASC.Files.Folders.curItemFolderMoveTo(folderId, titleDf, pathDest);
        }
    };

    var expand = function(treeNode) {
        var folderId = ASC.Files.Tree.getFolderId(treeNode);

        if (treeNode.children().is("ul")) {
            treeNode.toggleClass("jstree-closed").toggleClass("jstree-open");
        } else {
            ASC.Files.Tree.getTreeSubFolders(folderId);
        }
    };

    var resetNode = function(folderId) {
        if (!ASC.Files.Common.isCorrectId(folderId))
            return;

        ASC.Files.Tree.getTreeNode(folderId)
            .addClass("jstree-closed").removeClass("jstree-open jstree-empty")
            .children("ul").remove();

        ASC.Files.Tree.getStreeNode(folderId)
            .addClass("jstree-closed").removeClass("jstree-open jstree-empty")
            .children("ul").remove();
    };

    //request

    var getTreeSubFolders = function(folderId) {
        ASC.Files.Tree.getTreeNode(folderId).find(".jstree-icon.expander").addClass("jstreeLoadNode");
        ASC.Files.Tree.getStreeNode(folderId).find(".jstree-icon.expander").addClass("jstreeLoadNode");

        serviceManager.request("post",
            "xml",
            ASC.Files.TemplateManager.events.GetTreeSubFolders,
            { folderId: folderId },
            { orderBy: ASC.Files.Filter.getOrderByAZ(true) },
            "folders", "subfolders?parentId=" + encodeURIComponent(folderId));
    };

    //event handler

    var onGetTreeSubFolders = function(xmlData, params, errorMessage, commentMessage) {
        var folderId = params.folderId;
        ASC.Files.Tree.getTreeNode(folderId).find(".jstree-icon.expander").removeClass("jstreeLoadNode");
        ASC.Files.Tree.getStreeNode(folderId).find(".jstree-icon.expander").removeClass("jstreeLoadNode");
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            return undefined;
        }

        var treeNodeUl = ASC.Files.Tree.getTreeNode(folderId).find("ul");
        if (treeNodeUl.html() == null || treeNodeUl.html() == "") {
            renderTreeView(folderId, xmlData);
            ASC.Files.Tree.getTreeNode(folderId).children().show();
            ASC.Files.Tree.getStreeNode(folderId).children().show();
        }
        ASC.Files.Tree.select(ASC.Files.Tree.getTreeNode(ASC.Files.Folders.currentFolderId));
        ASC.Files.Tree.showSelect(ASC.Files.Folders.currentFolderId);
    };

    return {
        init: init,
        expand: expand,
        resetNode: resetNode,

        select: select,
        selectS: selectS,

        getTreeNode: getTreeNode,
        getStreeNode: getStreeNode,
        getFolderId: getFolderId,
        getFolderTitle: getFolderTitle,

        pathParts: pathParts,

        showSelect: showSelect,
        showTreeViewPanel: showTreeViewPanel,
        showMoveToSelector: showMoveToSelector,

        registerHideTree: registerHideTree,

        showTreePath: showTreePath,
        getTreeSubFolders: getTreeSubFolders
    };
})();

(function($) {
    ASC.Files.Tree.init();
    $(function() {
        jq("#files_movetoButton, #files_copytoButton, #files_restoreButton,\
            #files_mainMove, #files_mainCopy, #files_mainRestore").click(function() {
            ASC.Files.Tree.showMoveToSelector(this.id == "files_copytoButton" || this.id == "files_mainCopy");
        });

        jq("#files_moveto_files, #files_restore_files, #files_copyto_files,\
            #files_moveto_folders, #files_restore_folders, #files_copyto_folders").click(function() {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Tree.showMoveToSelector(this.id == "files_copyto_files" || this.id == "files_copyto_folders");
        });

        jq("#files_breadCrumbsCaption").click(function() {
            if (ASC.Files.Tree.pathParts.length > 1) {
                ASC.Files.Folders.navigationSet(ASC.Files.Tree.pathParts[0].Key);
            } else {
                ASC.Files.Tree.showTreeViewPanel();
            }

            return false;
        });

        jq("#files_trewViewContainer").on("click", ".jstree-icon.expander", function() {
            ASC.Files.Tree.expand(jq(this).parent());
            return false;
        });

        jq("#files_trewViewContainer").on("click", "a", function() {
            var treeNode = jq(this).parent();
            ASC.Files.Tree.select(treeNode);
            ASC.Files.Folders.navigationSet(ASC.Files.Tree.getFolderId(treeNode));
            return false;
        });

        jq("#files_trewViewSelector").on("click", ".jstree-icon.expander", function() {
            ASC.Files.Tree.expand(jq(this).parent());
            return false;
        });

        jq("#files_trewViewSelector").on("click", "a", function() {
            var treeNode = jq(this).parent();
            var folderId = ASC.Files.Tree.getFolderId(treeNode);
            var folderObj = ASC.Files.UI.getEntryObject("folder", folderId);
            if (ASC.Files.UI.accessibleItem(folderObj)) {
                ASC.Files.Tree.selectS(treeNode);
                return false;
            } else {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SecurityException, true);
                ASC.Files.Tree.expand(treeNode);
                return false;
            }
        });

    });
})(jQuery);