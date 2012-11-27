window.ProjectDocumentsPopup = (function() {
    var isInit = false,
        projId, rootFolderId,
        firstLoad = true;

    var init = function(projectFolderId) {
        if (!isInit) {
            isInit = true;
            projId = jq(".fileContainer").attr("projId");
            rootFolderId = projectFolderId;

            var projectTitle = jq("#projTitle").text();
            var rootName = "<a class='root' id='" + rootFolderId + "' >" + projectTitle + "</a>";
            jq(".popupContainerBreadCrumbs").append(rootName);
        }

        jq("#popupDocumentUploader .buttonContainer").on('click', '.baseLinkButton', function() {

            if (!jq(this).hasClass('disable')) {
                attachSelectedFiles();
            }
            return false;
        });
        jq("#popupDocumentUploader .buttonContainer").on('click', '.grayLinkButton', function() {
            ProjectDocumentsPopup.EnableEsc = true;
            jq.unblockUI();
            return false;
        });

        jq("#popupDocumentUploader .popupContainerBreadCrumbs").on('click', 'a', function() {
            openPreviosFolder(this);
            var links = jq("#popupDocumentUploader .popupContainerBreadCrumbs").find('a');
            if (links.length - 1 > 1) {
                jq("#popupDocumentUploader .popupContainerBreadCrumbs a:first").removeClass("root");
            } else {
                jq("#popupDocumentUploader .popupContainerBreadCrumbs a:first").addClass("root");
            }
            return false;
        });

    	jq("#popupDocumentUploader").on("click", ".fileList input", function() {
    		if (!jq(this).is(":checked")) {
    			jq("#checkAll").attr("checked", false);
    			return;
    		}
    		var checkedAll = true;
    		jq(".fileList input").each(function() {
    			if (!jq(this).is(":checked")) {
    				checkedAll = false;
    				return;
    			}
    		});
    		if (checkedAll) {
    			jq("#checkAll").attr("checked", true);
    		}
    	});
    };

    var getListFolderFiles = function(id) {
        Teamlab.getDocFolder(null, id, function() { onGetFolderFiles(arguments); });
    };

    var showEmptyScreen = function() {
        jq("#popupDocumentUploader .fileContainer").find("#emptyFileList").show();
        jq("#popupDocumentUploader .buttonContainer .baseLinkButton").addClass("disable");
    };
    var hideEmptyScreen = function() {
        jq("#popupDocumentUploader .fileContainer").find("#emptyFileList").hide();
        jq("#popupDocumentUploader .buttonContainer .baseLinkButton").removeClass("disable");
    };

    var showCheckAll = function() {
        var checkboxs = jq(".fileList li input");
        if (checkboxs.length) {
            jq(".containerCheckAll").show();
        } else {
            jq(".containerCheckAll").hide();
        }
    };

    var onGetFolderFiles = function(args) {
        if (firstLoad) {
            firstLoad = false;
        }
        var content = new Array();
        var folders = args[1].folders;
        for (var i = 0; i < folders.length; i++) {
            var folderName = folders[i].title;

            var folId = folders[i].id;

            var folder = { title: folderName, exttype: "", id: folId, type: "folder" };
            content.push(folder);
        }
        var files = args[1].files;
        for (var i = 0; i < args[1].files.length; i++) {
            var fileName = decodeURIComponent(files[i].title);

            var exttype = ASC.Files.Utility.getCssClassByFileTitle(fileName, true);

            var fileId = files[i].id;
            var access = files[i].access;
            var viewUrl = files[i].viewUrl;
            var version = files[i].version;
            var file = { title: fileName, access: access, exttype: exttype, version: version, id: fileId, type: "file", ViewUrl: viewUrl };
            content.push(file);
        }

        jq(".fileList").empty();
        if (content.length == 0) {
            showEmptyScreen();
            jq(".containerCheckAll").hide();
            jq(".loader").hide();
            return;
        }
        hideEmptyScreen();
        jq(".fileContainer").find("#emptyFileList").hide();
        var template = "{{if type=='folder'}}<li onclick='ProjectDocumentsPopup.openFolder(this);' id='${id}'><span class='folder'>${title}</span></li>" +
        "{{else}}" +
        "<li><input type='checkbox' version='${version}' access='${access}' id='${id}'/><label class='${exttype}' for='${id}'>${title}</label></li>" +
        "{{/if}}";
        jq.template("fileTmpl", template);
        jq.tmpl("fileTmpl", content).appendTo(jq(".fileList"));
        jq("ul.fileList li:even").addClass("even");
        showCheckAll();
        jq(".loader").hide();
    };

    var addItemInBreadCrumbs = function(id, title) {
        jq(".popupContainerBreadCrumbs a.current").removeClass("current");
        jq(".popupContainerBreadCrumbs a:first").removeClass("root");
        var newItem = "<span> > </span><a class='current' id='" + id + "'>" + title + "</a>";
        jq(".popupContainerBreadCrumbs").append(newItem);
    };

    var openFolder = function(folder) {
        var id = jq(folder).attr("id");
        var text = jq(folder).children("span").text();
        getListFolderFiles(id);
        addItemInBreadCrumbs(id, text);
    };

    var removeItemInBreadCrumbs = function(folder) {
        var firstFolder = jq(".popupContainerBreadCrumbs a:first-child").attr("id");
        var lastFolder = jq(".popupContainerBreadCrumbs a:last-child").attr("id");

        var id = jq(folder).attr("id");
        while (id != lastFolder) {
            jq(".popupContainerBreadCrumbs a:last-child").remove();
            jq(".popupContainerBreadCrumbs span:last-child").remove();
            lastFolder = jq(".popupContainerBreadCrumbs a:last-child").attr("id");
        }
        if (firstFolder == lastFolder) return;
        jq(".popupContainerBreadCrumbs a:last-child").addClass("current");
    };

    var openPreviosFolder = function(folder) {
        var id = jq(folder).attr("id");
        getListFolderFiles(id);
        removeItemInBreadCrumbs(folder);
    };

    var levelUp = function() {
        var openfolder = jq("#popupDocumentUploader .popupContainerBreadCrumbs a:last-child");
        if (openfolder != jq("#popupDocumentUploader .popupContainerBreadCrumbs a:first-child")) {
            var prevfolder = jq(openfolder).prev().prev();
            jq(prevfolder).click();
        }
    };

    var showPortalDocUploader = function() {
        if (firstLoad) {
            getListFolderFiles(rootFolderId);
        }
        jq("#checkAll").removeAttr("checked");
        jq(".fileList li input").removeAttr("checked");

        var margintop = jq(window).scrollTop() - 135;
        margintop = margintop + 'px';

        PopupKeyUpActionProvider.EnableEsc = false;
        jq.blockUI({ message: jq("#popupDocumentUploader"),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '650px',

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-300px',
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

            onBlock: function() { }
        });
    };

    var attachSelectedFiles = function() {
        var listElems = jq(".fileList li input:checked").parent("li");
        var listfiles = new Array();
        var massFileId = new Array();
        for (var i = 0; i < listElems.length; i++) {
            var fileName = jq(listElems[i]).children("label").text();
            var exttype = jq(listElems[i]).children("label").attr("class");
            var fileId = parseInt(jq(listElems[i]).children("input").attr("id"));
            var version = jq(listElems[i]).children("input").attr("version");
            var access = parseInt(jq(listElems[i]).children("input").attr("access"));
            var type;
            if (ASC.Files.Utility.CanImageView(fileName)) {
                type = "image";
            }
            else {
                if (ASC.Files.Utility.CanWebEdit(fileName)) {
                    type = "editedFile";
                }
                else {
                    if (ASC.Files.Utility.CanWebView(fileName)) {
                        type = "viewedFile";
                    }
                    else {
                        type = "noViewedFile";
                    }
                }
            }
            if (!Attachments.isAddedFile(fileName, fileId)) {
                massFileId.push(fileId);
                var downloadUrl = ASC.Files.Utility.GetFileDownloadUrl(fileId);
                var viewUrl = ASC.Files.Utility.GetFileViewUrl(fileId);
                var docViewUrl = ASC.Files.Utility.GetFileWebViewerUrl(fileId);
                var editUrl = ASC.Files.Utility.GetFileWebEditorUrl(fileId);
                var fileTmpl = { title: fileName, access: access, type: type, exttype: exttype, id: fileId, version: version, ViewUrl: viewUrl, downloadUrl: downloadUrl, editUrl: editUrl, docViewUrl: docViewUrl };
                listfiles.push(fileTmpl);
                jq(document).trigger("addFile", fileTmpl);
            }
        }
        if (massFileId.length != 0) {
            Attachments.appendToListAttachFiles(listfiles);
        }
        PopupKeyUpActionProvider.EnableEsc = true;
        jq.unblockUI();
    };
    var selectAll = function() {
        jq("ul.fileList li input").attr("checked", jq("#checkAll").is(":checked"));
    };
    return {
        init: init,
        onGetFolderFiles: onGetFolderFiles,
        showPortalDocUploader: showPortalDocUploader,
        openFolder: openFolder,
        attachSelectedFiles: attachSelectedFiles,
        openPreviosFolder: openPreviosFolder,
        selectAll: selectAll,
        levelUp: levelUp
    };
})(jQuery);