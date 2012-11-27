ASC.Files.ImageViewer = (function() {

    var isInit = false;
    var scalingInProcess = false;
    var action;
    var imageCollection;

    var odimensions = { };
    var ndimensions = { };
    var imgRef;
    var oScale = 0;
    var nScale = 0;
    var scaleStep = 15;
    var rotateAngel = 0;
    var mouseOffset;
    var windowDimensions = { };
    var imageAreaDimensions = { };
    var centerArea = { };

    var displayLoading = function() {
        LoadingBanner.loaderCss = "loading_container_wrapper";
        LoadingBanner.displayLoading();
    };

    var hideLoading = function() {
        LoadingBanner.loaderCss = "";
        LoadingBanner.hideLoading(true);
    };

    var prepareWorkspace = function() {
        LoadingBanner.hideLoading(true);
        displayLoading();
        ASC.Files.UI.finishMoveTo({ }, { });
        jq("body").addClass("scroll_fix");

        if (jq.browser.msie && jQuery.browser.version <= 7) {
            jq("html").addClass("scroll_fix");
            jq("<style type='text/css'> .studioContentClassOverride{ position: static !important; } </style>").appendTo("head");
            jq("#studioContent").addClass("studioContentClassOverride");
        }

        jq(document).bind("mousewheel", mouseWheelEvent);
        jq(document).keydown(keyDownEvent);
        jq(window).bind("resize", positionParts);

        jq.blockUI({
            message: null,
            css: {
                left: "50%",
                top: "50%",
                opacity: "1",
                border: "none",
                padding: "0px"
            },
            overlayCSS: {
                backgroundColor: "Black",
                cursor: "default",
                opacity: "0.5"
            },
            allowBodyStretch: false
        });

        jq("#imageViewerToolbox, #imageViewerClose").show();

        jq("div.blockUI.blockOverlay").click(function() {
            ASC.Files.ImageViewer.closeImageViewer();
            return false;
        });
    };

    var resetWorkspace = function() {
        jq("#imageViewerInfo, #imageViewerToolbox, #imageViewerContainer, #imageViewerClose, #other_actions").hide();
        jq("body").removeClass("scroll_fix");

        if (jq.browser.msie && jQuery.browser.version <= 7) {
            jq("html").removeClass("scroll_fix");
            jq("#studioContent").removeClass("studioContentClassOverride");

        }
        hideLoading();
        jq.unblockUI();
    };

    var getnewSize = function(side, nvalue) {
        var otherside = (side == "w") ? "h" : "w";
        var newSize;
        if (typeof nvalue == "undefined" || nvalue == null) {
            newSize = ndimensions[otherside] * odimensions[side] / odimensions[otherside];
        } else {
            newSize = ( /%/ .test(nvalue)) ? parseInt(nvalue) / 100 * odimensions[side] : parseInt(nvalue);
        }
        ndimensions[side] = Math.round(newSize);
    };

    var setNewDimensions = function(setting, callbackHandle) {

        var sortbysize = (odimensions.w > odimensions.h) ? ["w", "h"] : ["h", "w"];
        setting[sortbysize[0]] = setting.ls;
        setting[sortbysize[1]] = null;

        var sortbyavail = (setting.w) ? ["w", "h"] : (setting.h) ? ["h", "w"] : [];

        var oldDimensions = { w: ndimensions.w, h: ndimensions.h };

        getnewSize(sortbyavail[0], setting[sortbyavail[0]]);
        getnewSize(sortbyavail[1], setting[sortbyavail[1]]);

        scalingInProcess = true;

        if (typeof callbackHandle == "undefined" || callbackHandle == null)
            callbackHandle = function() {
                scalingInProcess = false;
                jq("#imageViewerInfo").hide();
                oScale = nScale;
            };

        if (typeof setting.speed == "undefined" || setting.speed == null)
            setting.speed = 250;

        if (setting.speed != 0) {

            var scaleSign = 1;

            var getDimensionOffset = function(side) {
                var diff = (ndimensions[side] - oldDimensions[side]) / 2;

                if (diff < 0) scaleSign = -1;

                if (diff > 0)
                    return "-=" + diff + "px";
                else
                    return "+=" + (-diff) + "px";
            };

            imgRef.animate({ width: ndimensions.w + "px", height: ndimensions.h + "px", left: getDimensionOffset("w"), top: getDimensionOffset("h") },
                {
                    duration: setting.speed,
                    easing: "linear",
                    complete: callbackHandle,
                    step: function(now, obj) {
                        jq("#imageViewerInfo span").text(Math.round(oScale + scaleSign * scaleStep * obj.pos) + "%");
                    }
                });
        } else {
            imgRef.css({ width: ndimensions.w + "px", height: ndimensions.h + "px" });
            callbackHandle();
        }
    };

    var imageOnLoad = function() {
        hideLoading();

        imgRef[0].style.cssText = "";

        var tempimg = jq('<img src="' + imgRef.attr('src') + '" style="position:absolute; top:0; left:0; visibility:hidden" />').prependTo("body");

        odimensions = { w: tempimg.width(), h: tempimg.height() };

        if (odimensions.w == 0 && odimensions.h == 0) {
            odimensions.w = imgRef.attr("naturalWidth") || imgRef[0].naturalWidth;
            odimensions.h = imgRef.attr("naturalHeight") || imgRef[0].naturalHeight;
        }

        var similarityFactors = { w: odimensions.w / imageAreaDimensions.w, h: odimensions.h / imageAreaDimensions.h };
        var lsf = (similarityFactors.w > similarityFactors.h) ? "w" : "h";

        if (similarityFactors[lsf] > 1)
            nScale = Math.round(imageAreaDimensions[lsf] * 100.0 / odimensions[lsf]);
        else
            nScale = 100;

        nScale = Math.max(nScale, scaleStep);

        if (action == "open") {
            ndimensions = { w: 10, h: 10 };

            imgRef.css({
                "display": "block",
                "left": centerArea.w - ndimensions.w / 2,
                "top": centerArea.h - ndimensions.h / 2
            });

            setNewDimensions({ ls: nScale + "%" });
        } else {
            setNewDimensions({ ls: nScale + "%", speed: 0 }, function() {
                scalingInProcess = false;

                imgRef.css({
                    "display": "block",
                    "left": centerArea.w - ndimensions.w / 2,
                    "top": centerArea.h - ndimensions.h / 2
                });
                oScale = nScale;
            });
        }

        tempimg.remove();

        resetImageInfo();

        if (ASC.Files.Share)
            ASC.Files.Share.removeNewIcon("file", imageCollection[imageCollection.selectedIndex].id);
    };

    var resetImageInfo = function() {
        if (isImageLoadCompleted()) {
            var text = "{0} ({1}x{2})".format(imageCollection[imageCollection.selectedIndex].title, odimensions.w, odimensions.h);
            jq("#imageViewerToolbox div.imageInfo").text(text).attr("title", text);
        } else {
            jq("#imageViewerToolbox div.imageInfo").html("&nbsp").removeAttr("title");
        }
    };

    var isImageLoadCompleted = function() {
        return !jQuery.isEmptyObject(odimensions);
    };

    var showImage = function() {
        odimensions = { };
        ndimensions = { };
        rotateAngel = 0;

        jq("#imageViewerInfo, #imageViewerContainer").hide();

        displayLoading();

        var fileId = imageCollection[imageCollection.selectedIndex].id;
        var imageVersion = imageCollection[imageCollection.selectedIndex].vetsion;

        imgRef[0].onload = imageOnLoad;

        imgRef.removeAttr("src");
        imgRef.attr("src", ASC.Files.Utility.GetFileViewUrl(fileId, imageVersion));

        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);

        if (ASC.Files.UI.accessAdmin(fileObj)) {
            jq("#other_actions .action_delete").show();
        } else {
            jq("#other_actions .action_delete").hide();
            //TODO: Hide button, if empty list actions
        }
    };

    var mouseDownEvent = function(event) {
        mouseOffset = { x: event.pageX - imgRef.offset().left, y: event.pageY - imgRef.offset().top };

        jq(document).unbind("mouseup");
        jq(document).unbind("mousemove");
        jq(document).mouseup(mouseUpEvent);
        jq(document).mousemove(mouseMoveEvent);

        document.ondragstart = function() { return false; };
        document.body.onselectstart = function() { return false; };
        imgRef[0].ondragstart = function() { return false; };

        jq("#imageViewerToolbox, #other_actions").hide();
    };

    var mouseMoveEvent = function(event) {
        var x = event.pageX,
            y = event.pageY;
        if (x < 0 || y < 0 || x > jq(window).width() || y + 100 > jq(window).height())
            return;
        imgRef.css({
            "cursor": "pointer",
            "left": x - mouseOffset.x,
            "top": y - mouseOffset.y
        });
    };

    var mouseUpEvent = function() {
        imgRef.css("cursor", "move");
        jq(document).unbind("mouseup");
        jq(document).unbind("mousemove");

        imgRef[0].ondragstart = null;
        document.ondragstart = null;
        document.body.onselectstart = null;

        jq("#imageViewerToolbox").show();
    };

    var mouseWheelEvent = function(event) {
        if (scalingInProcess) return;

        var delta = 0;
        var e = ASC.Files.Common.fixEvent(event).originalEvent;
        if (e.wheelDelta) {
            delta = e.wheelDelta / 120;
        } else if (e.detail) {
            delta = -e.detail / 3;
        }

        if (delta > 0)
            ASC.Files.ImageViewer.zoomIn();
        else if (delta < 0)
            ASC.Files.ImageViewer.zoomOut();

        return false;
    };

    var mouseDoubleClickEvent = function() {
        if (scalingInProcess) return;

        var ls = windowDimensions.w > windowDimensions.h ? windowDimensions.w : windowDimensions.h;

        setNewDimensions({ ls: ls + "px" });
    };

    var keyDownEvent = function(e) {
        var keyCode = e.keyCode || e.which;
        var arrow = ASC.Files.Common.keyCode;

        switch (keyCode) {
        case arrow.left:
            ASC.Files.ImageViewer.prevImage();
            return false;
        case arrow.spaceBar:
        case arrow.right:
            ASC.Files.ImageViewer.nextImage();
            return false;
        case arrow.up:
            ASC.Files.ImageViewer.zoomIn();
            return false;
        case arrow.down:
            ASC.Files.ImageViewer.zoomOut();
            return false;
        case arrow.esc:
            ASC.Files.ImageViewer.closeImageViewer();
            return false;
        case arrow.deleteKey:
            ASC.Files.ImageViewer.deleteImage();
            return false;
        case arrow.home:
        case arrow.end:
        case arrow.pageDown:
        case arrow.pageUP:
            return false;
        }
    };

    var rotateImage = function() {
        if (!jq.browser.msie) {
            var rotateCssAttr = "rotate({0}deg)".format(rotateAngel);

            imgRef.css({
                "-moz-transform": rotateCssAttr,
                "-o-transform": rotateCssAttr,
                "-webkit-transform": rotateCssAttr,
                "transform": rotateCssAttr
            });
        } else {
            var rad = (rotateAngel * Math.PI) / 180.0;
            var filter = 'progid:DXImageTransform.Microsoft.Matrix(sizingMethod="auto expand", M11 = ' + Math.cos(rad) + ', M12 = ' + (-Math.sin(rad)) + ', M21 = ' + Math.sin(rad) + ', M22 = ' + Math.cos(rad) + ')';

            var imgOffset = imgRef.offset();

            imgRef.css(
                {
                    "-ms-filter": filter,
                    "filter": filter
                });

            var rotateOffset = { left: -Math.round((imgRef.width() - ndimensions.w) / 2), top: -Math.round((imgRef.height() - ndimensions.h) / 2) };

            imgRef.css(
                {
                    "left": imgOffset.left + rotateOffset.left,
                    "top": imgOffset.top + rotateOffset.top
                });

            ndimensions.w = imgRef.width();
            ndimensions.h = imgRef.height();
        }
    };

    var calculateDimensions = function() {
        windowDimensions = { w: jq(window).width(), h: jq(window).height() };

        var centerAreaOX = windowDimensions.w / 2 + jq(window).scrollLeft();
        var centerAreaOY = windowDimensions.h / 2 + jq(window).scrollTop() - jq("#imageViewerToolbox").height() / 2;

        centerArea = { w: centerAreaOX, h: centerAreaOY };

        imageAreaDimensions = { w: windowDimensions.w, h: windowDimensions.h - jq("#imageViewerToolbox").height() };
    };

    var positionParts = function() {
        calculateDimensions();

        jq("#imageViewerInfo").css({
            "left": centerArea.w,
            "top": centerArea.h
        });

        jq("#imageViewerClose").css({
            "right": 10,
            "top": jq(window).scrollTop() + 15
        });

        jq("#imageViewerToolbox").css({
            "top": windowDimensions.h + jq(window).scrollTop() - jq("#imageViewerToolbox").height(),
            "left": "0px"
        });
    };

    //    var toBatchLoader = function() {
    //        var idList = jq("#imageBatchLoader").data("idList");

    //        if (jQuery.inArray(fileId, idList) != -1) return;

    //        idList.push(imageID);

    //        jq("#imageBatchLoader").append(jq("<img/>").attr("src", ASC.Files.Utility.GetFileViewUrl(fileId)));
    //    };

    var fetchImage = function(asc) {
        if (scalingInProcess) return;

        if (imageCollection.length == 0 || imageCollection.selectedIndex < 0) {
            ASC.Files.ImageViewer.closeImageViewer();
            return;
        }

        if (!asc) {
            imageCollection.selectedIndex--;

            if (imageCollection.selectedIndex < 0) {
                imageCollection.selectedIndex = imageCollection.length - 1;
            }

            action = "prevImage";

            var fileId = imageCollection[imageCollection.selectedIndex].id;
            ASC.Controls.AnchorController.safemove(ASC.Files.ImageViewer.getPreviewUrl(fileId));
        } else {
            imageCollection.selectedIndex++;

            if (imageCollection.selectedIndex > imageCollection.length - 1) {
                imageCollection.selectedIndex = 0;
            }

            action = "nextImage";

            var fileId = imageCollection[imageCollection.selectedIndex].id;
            ASC.Controls.AnchorController.safemove(ASC.Files.ImageViewer.getPreviewUrl(fileId));
        }

        showImage();
    };

    var onGetImageViewerData = function(jsonData, params, errorMessage, commentMessage) {
        if ((typeof errorMessage != "undefined") || typeof jsonData == "undefined" || (jsonData.length == 0)) {
            ASC.Files.UI.displayInfoPanel(commentMessage || errorMessage, true);
            ASC.Files.ImageViewer.closeImageViewer();
            return undefined;
        }

        imageCollection = jsonData;

        imageCollection.selectedIndex = imageCollection.length;

        for (var i = 0; i < imageCollection.length; i++) {
            var str = imageCollection[i].Value.split("#");

            imageCollection[i].id = imageCollection[i].Key;
            imageCollection[i].vetsion = str[0];
            imageCollection[i].title = str[1];

            if (imageCollection[i].id == params.fileId) {
                imageCollection.selectedIndex = i;
            }
        }

        if (imageCollection.selectedIndex >= imageCollection.length) {
            ASC.Files.ImageViewer.closeImageViewer();
            return;
        }

        showImage();
    };

    return {
        init: function(fileId) {
            jq(document).unbind("mousedown");
            jq(document).unbind("mouseup");
            jq(document).unbind("mousemove");
            ASC.Files.UI.mouseBtn = false;
            if (isInit === false) {
                isInit = true;

                serviceManager.bind(ASC.Files.TemplateManager.events.GetSiblingsFile, onGetImageViewerData);
                imgRef = jq("#imageViewerContainer");
                imgRef.dblclick(mouseDoubleClickEvent);
                imgRef.mousedown(mouseDownEvent);

                jq("#imageViewerToolbox ul li img").hover(
                    function() {
                        var img = jq(this);
                        var imgSrc = img.attr("src");
                        if (imgSrc.indexOf("_hover.png") != -1) return;

                        var hoverSrc = imgSrc.replace(".png", "_hover.png");
                        img.attr("src", hoverSrc);

                        jq("#imageViewerToolbox div.imageInfo").text(img.attr("title"));
                    },
                    function() {
                        var img = jq(this);
                        var originSrc = img.attr("src").replace("_hover.png", ".png");
                        img.attr("src", originSrc);

                        resetImageInfo();
                    }
                );
            }

            prepareWorkspace();
            positionParts();

            action = "open";

            var filterSettings = ASC.Files.Filter.getFilterSettings();

            serviceManager.request("post",
                "json",
                ASC.Files.TemplateManager.events.GetSiblingsFile,
                { fileId: fileId },
                { orderBy: filterSettings.sorter },
                "folders",
                "files",
                "siblings?fileID=" + encodeURIComponent(fileId)
                    + "&filter=" + filterSettings.filter
                        + "&subjectID=" + filterSettings.subject
                            + "&search=" + encodeURIComponent(filterSettings.text)
            );
        },

        getPreviewUrl: function(fileId) {
            return "#preview/{0}".format(fileId);
        },

        closeImageViewer: function() {
            if (isImageLoadCompleted()) {
                setNewDimensions({ ls: "10px", speed: 500 }, resetWorkspace);
            } else {
                imgRef[0].onload = null;
                imgRef.removeAttr("src");
                resetWorkspace();
            }

            jq(window).unbind("resize");
            jq(document).unbind("mousewheel");
            jq(document).unbind("keydown");
            jq(document).unbind("mousedown");
            jq(document).unbind("mouseup");
            jq(document).unbind("mousemove");
            ASC.Files.UI.mouseBtn = false;
            ASC.Files.Actions.mouseBindDocument();

            ASC.Files.Folders.navigationSet(ASC.Files.Folders.currentFolderId, ASC.Files.Common.isCorrectId(ASC.Files.Folders.currentFolderId));
        },

        prevImage: function() {
            fetchImage(false);
        },
        nextImage: function() {
            fetchImage(true);
        },

        rotateLeft: function() {
            if (scalingInProcess) return;

            rotateAngel -= 90;
            rotateImage();
        },

        rotateRight: function() {
            if (scalingInProcess) return;

            rotateAngel += 90;
            rotateImage();
        },

        fullScale: function() {
            if (scalingInProcess || nScale == 100) return;

            nScale = 100;
            jq("#imageViewerInfo span").text(nScale + "%");
            jq("#imageViewerInfo").show();
            setNewDimensions({ ls: nScale + "%" });
        },

        zoomIn: function() {
            if (scalingInProcess) return;

            nScale += scaleStep;
            nScale = Math.min(nScale, 1000);

            jq("#imageViewerInfo span").text(nScale + "%");
            jq("#imageViewerInfo").show();
            setNewDimensions({ ls: nScale + "%" });
        },

        zoomOut: function() {
            if (scalingInProcess) return;

            nScale -= scaleStep;
            nScale = Math.max(nScale, scaleStep);

            jq("#imageViewerInfo span").text(nScale + "%");
            jq("#imageViewerInfo").show();
            setNewDimensions({ ls: nScale + "%" });
        },

        deleteImage: function() {
            if (scalingInProcess)
                return;

            var fileId = imageCollection[imageCollection.selectedIndex].id;

            var fileObj = ASC.Files.UI.getEntryObject("file", fileId);

            if (!ASC.Files.UI.accessAdmin(fileObj))
                return;

            var data = { };
            data.entry = new Array();
            ASC.Files.UI.blockObjectById("file", fileId, true, ASC.Files.FilesJSResources.DescriptRemove);
            data.entry.push("file_" + fileId);

            serviceManager.deleteItem(ASC.Files.TemplateManager.events.DeleteItem, { list: [fileId], doNow: true }, { stringList: data });

            imageCollection.splice(imageCollection.selectedIndex, 1);
            imageCollection.selectedIndex--;

            ASC.Files.ImageViewer.nextImage();
        },

        downloadImage: function() {
            if (scalingInProcess) return;

            var fileId = imageCollection[imageCollection.selectedIndex].id;
            var imageVersion = imageCollection[imageCollection.selectedIndex].vetsion;
            window.open(ASC.Files.Utility.GetFileViewUrl(fileId, imageVersion), "new", "fullscreen = 1, resizable = 1, location=1, toolbar=1");
        }
    };
})();

(function($) {
    jq.dropdownToggle({
        switcherSelector: "#other_actions_switch",
        dropdownID: "other_actions"
    });
})(jQuery);

//        batchLoaderBound = { left: imageCollection.selectedIndex - 5, right: imageCollection.selectedIndex + 5 };

//        if (batchLoaderBound.left < 0) batchLoaderBound.left = 0;
//        if (batchLoaderBound.right > imageCollection.length - 1) batchLoaderBound.right = imageCollection.length - 1;

//        for (var index = batchLoaderBound.left; index < batchLoaderBound.right; index++)
//            toBatchLoader(imageCollection[index]);

//            jq("#imageBatchLoader").data("idList", new Array());
//            toBatchLoader(fileId);