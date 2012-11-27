<%@ Assembly Name="ASC.Web.Files" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Editor.aspx.cs" Inherits="ASC.Web.Files.Editor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="icon" href="~/favicon.ico" type="image/x-icon" />
    <title></title>
    
    <asp:PlaceHolder ID="aspHeaderContent" runat="server"></asp:PlaceHolder>
    
    <style type="text/css">
        div.files_infoPanel
        {
            display:none;
            left: 50%;
            margin: 0;
            max-width: 722px;
            position: fixed;
            top: 0;
            z-index: 260;
            color: #666668 !important;
            font-size: 13px;
            font-weight: normal;
            vertical-align: middle;
        }
        div.files_infoPanel div
        {
            background-color: #FBFAD5;
            overflow: hidden;
            padding: 10px;
        }
    </style>
    
    <script type="text/javascript">
        var trackEditInterval = null;
        jq(function() {
            if (window.addEventListener) {
                window.addEventListener("message", listener, false);
            } else {
                window.attachEvent("onmessage", listener);
            }

            jq("#iframeDocEditor").parents().css("height", "100%").removeClass("clearFix");
            jq("body").css("overflow-y", "hidden");

            trackEditInterval = setInterval(trackEdit, 5000);

            window.documentChanged = false;
            window.onbeforeunload = finishEdit;

            if (typeof ASC.Files.Actions == "undefined") ASC.Files.Actions = (function() { return { hideAllActionPanels: function() { } }; })();

        });
        
        function listener(event) {
            var input = jq.parseJSON(event.data);
            
            switch (input.type) {
                case "share":
                    if (ASC.Files.Share) {
                        PopupKeyUpActionProvider.CloseDialogAction = 'jq("body").css("overflow", "hidden");';
                        ASC.Files.Share.getSharedInfo("file", fileId, fileTitle);
                    }
                    break;
                case "change":
                    window.documentChanged = (input.data == 1);
                    break;
                case "save":
                    jq.ajax({
                            type: "get",
                            url: ASC.Files.Constants.URL_HANDLER_SAVE.format(fileId, fileVersion, encodeURIComponent(input.data)) + FileSaveAsNew + ShareLink + "&_=" + new Date().getTime(),
                            complete: completeSave
                        });
                    break;
                case "tofiles":
                    location.href = folderUrl;
                    break;
            }
        };

        function completeSave() {
            clearInterval(trackEditInterval);

            try {
                var responseText = jq.parseJSON(arguments[0].responseText);
            } catch (e) {
                responseText = arguments[0].responseText.split("title>")[1].split("</")[0];
            }

            if (arguments[1] == "error" || responseText && responseText.error) {
                var errorMessage =  responseText.message || responseText;
                PostMessage("SavingError");
                ASC.Files.UI.displayInfoPanel(errorMessage, true);
            } else {
                window.documentChanged = false;
                trackEditInterval = setInterval(trackEdit, 5000);
                PostMessage("saved");
            }
        };

        serviceManager.bind("TrackEditFile", completeReq);
        function trackEdit() {
            serviceManager.trackEditFile("TrackEditFile", { fileID: fileId, docKeyForTrack: DocKeyForTrack, shareLink: ShareLink });
        };
        
        function finishEdit() {
            serviceManager.trackEditFile("FinishTrackEditFile", { fileID: fileId, docKeyForTrack: DocKeyForTrack, shareLink: ShareLink, finish: true, ajaxsync: true });
            if (window.documentChanged)
                return ASC.Files.FilesJSResources.WarningMessage_DocumentChanged;
        };
        
        function completeReq(jsonData, params, errorMessage,commentMessage) {
            if (typeof errorMessage != "undefined") {
                errorMessage = commentMessage || errorMessage;
                ASC.Files.UI.displayInfoPanel(errorMessage, true);
                clearInterval(trackEditInterval);
            }
        };

        function PostMessage(postBody) {
            var win = jq("#iframeDocEditor")[0].contentWindow;
            win.postMessage(jq.toJSON(postBody), "*");
        };

        //copy from js/auto/common.js
        String.prototype.format = function() {
            var txt = this,
                i = arguments.length;

            while (i--) {
                txt = txt.replace(new RegExp("\\{" + i + "\\}", "gm"), arguments[i]);
            }
            return txt;
        };

        //copy from js/auto/common.js
        var PopupKeyUpActionProvider = new function() {
            //close dialog by esc
            jq(document).keyup(function(event) {

                if (!jq(".popupContainerClass").is(":visible"))
                    return;

                var code;
                if (!e) var e = event;
                if (e.keyCode) code = e.keyCode;
                else if (e.which) code = e.which;

                if (code == 27 && PopupKeyUpActionProvider.EnableEsc) {
                    PopupKeyUpActionProvider.CloseDialog();
                }

                else if ((code == 13) && e.ctrlKey) {
                    if (PopupKeyUpActionProvider.CtrlEnterAction != null && PopupKeyUpActionProvider.CtrlEnterAction != "")
                        eval(PopupKeyUpActionProvider.CtrlEnterAction);
                }
                else if (code == 13) {
                    if (e.target.nodeName.toLowerCase() !== "textarea" && PopupKeyUpActionProvider.EnterAction != null && PopupKeyUpActionProvider.EnterAction != "")
                        eval(PopupKeyUpActionProvider.EnterAction);
                }

            });

            this.CloseDialog = function() {
                jq.unblockUI();

                if (PopupKeyUpActionProvider.CloseDialogAction != null && PopupKeyUpActionProvider.CloseDialogAction != "")
                    eval(PopupKeyUpActionProvider.CloseDialogAction);

                PopupKeyUpActionProvider.ClearActions();
            };

            this.CloseDialogAction = "";
            this.EnterAction = "";
            this.CtrlEnterAction = "";
            this.EnableEsc = true;

            this.ClearActions = function() {
                this.CloseDialogAction = "";
                this.EnterAction = "";
                this.CtrlEnterAction = "";
                this.EnableEsc = true;
            };
        };
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnableScriptGlobalization="true" EnableScriptLocalization="true" runat="server" ></asp:ScriptManager>
        <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
        <div id="infoPanelContainer" class="infoPanel files_infoPanel"><div>&nbsp;</div></div>
        <iframe id="iframeDocEditor" width="100%" height="100%" frameborder="0" src="<%=SrcIframe%>" style="background-color: Transparent; min-width: 1000px;"></iframe>
    </form>
</body>
</html>
