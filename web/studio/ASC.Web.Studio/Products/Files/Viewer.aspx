<%@ Assembly Name="ASC.Web.Files" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Viewer.aspx.cs" Inherits="ASC.Web.Files.Viewer" %>

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
        jq(function() {
            if (window.addEventListener) {
                window.addEventListener("message", listener, false);
            } else {
                window.attachEvent("onmessage", listener);
            }

            jq("#iframeDocViewer").parents().css("height", "100%").removeClass("clearFix");

            if (!jq.browser.mobile) {
                jq("body").css("overflow-y", "hidden");
            } else {
                jq("#iframeDocViewer").parents().css("overflow", "visible");
            }

            if (typeof ASC.Files.Actions == "undefined") ASC.Files.Actions = (function() { return { hideAllActionPanels: function() { } }; })();

            if (ErrorMessage) {
                ASC.Files.UI.displayInfoPanel(ErrorMessage, true);
            }

            var regExpError = /^#error\/(\S+)?/;
            if (regExpError.test(location.hash)) {
                var errorString = regExpError.exec(location.hash)[1];
                ASC.Files.UI.displayInfoPanel(decodeURIComponent(errorString).replace(/\+/g, " "), true);
            }
        });

        function listener(event) {

            var input = jq.parseJSON(event.data);
            switch (input.type) {
                case "download":
                    if (!jq.browser.mobile)
                        location.href = ASC.Files.Utility.GetFileDownloadUrl(fileId, fileVersion);
                    break;
                case "share":
                    if (ASC.Files.Share) {
                        PopupKeyUpActionProvider.CloseDialogAction = 'jq("body").css("overflow", "hidden");';
                        ASC.Files.Share.getSharedInfo("file", fileId, fileTitle);
                    }
                    break;
                case "edit":
                    location.href = ASC.Files.Utility.GetFileWebEditorUrl(fileId) + ShareLink;
                    break;
                case "tofiles":
                    if (!jq.browser.mobile)
                        location.href = folderUrl;
                    break;
            }
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
        <iframe id="iframeDocViewer" width="100%" height="100%" frameborder="0" src="<%=SrcIframe%>" style="background-color: Transparent; min-width: 1000px;"></iframe>
    </form>
</body>
</html>
