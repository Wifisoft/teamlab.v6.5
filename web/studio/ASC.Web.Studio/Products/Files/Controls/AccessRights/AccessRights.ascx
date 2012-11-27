<%@ Assembly Name="ASC.Web.Files" %>
<%@ Assembly Name="ASC.Web.Controls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRights.ascx.cs" Inherits="ASC.Web.Files.Controls.AccessRights" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>

<link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/AccessRights/accessrights.css" )%>" type="text/css"
    rel="stylesheet" />

<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/AccessRights/accessrights.js" ) %>"></script>

<asp:PlaceHolder ID="_sharingContainer" runat="server"></asp:PlaceHolder>

<script type="text/javascript" language="javascript">
        ZeroClipboard.setMoviePath('<%=VirtualPathUtility.ToAbsolute("~/products/files/flash/zeroclipboard/ZeroClipboard10.swf")%>');
</script>

<div style="display:none;">
    <div id="shareLinkItem" class="clearFix">
        <div class="header headerBase borderBase">
            <%=FilesUCResource.SharingLinkCaption%>
        </div>
        <div class="shareLinkRow">
            <div id="shareLinkPanel">
                <span style="padding-left: 17px;"><%=FilesUCResource.Link%>:</span>
                <span id="switchDisplayLink" class="baseLinkAction"></span>
                <a id="shareLink_copy" class="baseLinkAction"><span><%=FilesUCResource.CopyToClipboard%></span></a>
                <%if (!string.IsNullOrEmpty(Global.BitlyUrl))
                  { %>
                <a id="getShortenLink" class="baseLinkAction"><%=FilesUCResource.GetShortenLink%></a>
                <% } %>
                <textarea id="shareLink" class="textEdit" cols="10" rows="2"readonly="readonly"></textarea>
            </div>
        </div>
    </div>

    <div id="shareMessagePanel">
        <label>
            <input type="checkbox" id="shareMessageSend" checked="checked"/>
            <%=FilesUCResource.SendShareNotify%>
        </label>
        <a id="shareAddMessage" class="baseLinkAction"><%=FilesUCResource.AddShareMessage%></a>
        <a id="shareRemoveMessage" class="baseLinkAction"><%=FilesUCResource.RemoveShareMessage%></a>
        <textarea id="shareMessage"></textarea>
    </div>
</div>
<div id="files_confirm_unsubscribe" class="popupModal" style="display: none;">
    <ascw:container id="confirmUnsubscribeDialog" runat="server">
        <header><%=FilesUCResource.ConfirmRemove%></header>
        <body>
            <div id="confirmUnsubscribeText">
                <%=FilesUCResource.ConfirmUnsubscribe%>
            </div>
            <div id="confirmUnsubscribeList" class="files_remove_list">
                <dl>
                    <dt class="confirmRemoveFolders">
                        <%=FilesUCResource.Folders%>:</dt>
                    <dd class="confirmRemoveFolders">
                    </dd>
                    <dt class="confirmRemoveFiles">
                        <%=FilesUCResource.Documents%>:</dt>
                    <dd class="confirmRemoveFiles">
                    </dd>
                </dl>
            </div>
            <div class="clearFix" style="padding-top: 16px">
                <a id="unsubscribeConfirmBtn" class="baseLinkButton" style="float: left;">
                    <%=FilesUCResource.ButtonOk%>
                </a><a class="grayLinkButton" href="" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;"
                    style="float: left; margin-left: 8px;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
        </body>
    </ascw:container>
</div>
