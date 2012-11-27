<%@ Assembly Name="ASC.Web.Files" %>
<%@ Assembly Name="ASC.Web.Controls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThirdParty.ascx.cs" Inherits="ASC.Web.Files.Controls.ThirdParty" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>

<% #if (DEBUG) %>
    <link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/ThirdParty/thirdparty.css")%>" type="text/css" rel="stylesheet" />
<% #endif %>

<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/ThirdParty/thirdparty.js") %>"></script>

<script type="text/javascript" language="javascript">
    ASC.Files.Constants.URL_OAUTH_DROPBOX = "<%=ASC.Web.Files.Import.DropBox.Dropbox.Location%>";
    ASC.Files.Constants.URL_OAUTH_GOOGLE = "<%=ASC.Web.Files.Import.Google.OAuth.Location%>";
</script>

<div id="addThirdPartyPanel" class="files_action_panel withIcon files_popup_win"
    style="margin: 5px 0 0 -33px; z-index: 260; display: none;">
    <div class="popup-corner" style="left: 33px;">
    </div>
    <ul>
        <%if(EnableBoxNet)
          { %>
        <li id="add_boxnet"><a class="BoxNet">
            <%= FilesUCResource.ButtonAddBoxNet%></a> </li>
        <% } %>
        <%if(EnableDropBox)
          { %>
        <li id="add_dropbox"><a class="DropBox">
            <%= FilesUCResource.ButtonAddDropBox%></a> </li>
        <% } %>
        <%if(EnableGoogle)
          { %>
        <li id="add_google"><a class="Google">
            <%= FilesUCResource.ButtonAddGoogle%></a> </li>
        <% } %>
    </ul>
</div>

<div id="ThirdPartyEditor" class="popupModal" style="display: none;">
    <ascw:container id="ThirdPartyEditorTemp" runat="server">
        <header>
            <span id="thirdPartyDialogCaption"></span>
        </header>
        <body>
            <div id="thirdPartyPanel">
                <div id="thirdPartyNamePass">
                    <div><%=FilesUCResource.Login%></div>
                    <input type="text" id="thirdPartyName" maxlength="100" class="textEdit" />
                    
                    <div style="margin-top: 15px"><%=FilesUCResource.Password%></div>
                    <input type="password" id="thirdPartyPass" maxlength="100" class="textEdit" />
                </div>
                <div id="thirdPartyGetToken">
                    <a class="grayLinkButton"></a>
                    <span><%=FilesUCResource.TakeToken%></span>
                    <input type="hidden" />
                </div>
                
                <div style="margin-top: 25px"><%=FilesUCResource.ThirdPartyFolderTitle%></div>
                <div id="thirdPartyDivTitle">
                    <input type="text" id="thirdPartyTitle" maxlength="<%=Global.MAX_TITLE%>" class="textEdit" />
                </div>
                
                <%if(CurrentUserAdmin) %>
                <% { %>
                <label id="thirdPartyLabelCorporate" >
                    <input type="checkbox" id="thirdPartyCorporate" /><%= FilesUCResource.ThirdPartySetCorporate %></label>
                <% } %>
            </div>
            <div class="action-block">
                <a id="files_submitThirdParty" class="baseLinkButton">
                    <%=FilesUCResource.ButtonOk%>
                </a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton" onclick="PopupKeyUpActionProvider.CloseDialog();return false;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
            <div style="display: none;" class="ajax-info-block">
                <span class="textMediumDescribe">
                    <%=FilesUCResource.ProcessAuthentificate%>
                </span>
                <br />
                <img alt="" src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
            </div>
        </body>
    </ascw:container>
</div>

<div id="ThirdPartyDelete" class="popupModal" style="display: none;">
    <ascw:Container runat="server" ID="ThirdPartyDeleteTmp">
        <Header>
            <%=FilesUCResource.ThirdPartyDeleteCaption%>
        </Header>
        <Body>
            <div id="thirdPartyDeleteDescr"></div>
            <div class="action-block" style="padding-top: 16px">
                <a id="files_deleteThirdParty" class="baseLinkButton">
                    <%=FilesUCResource.ButtonOk%>
                </a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton" onclick="PopupKeyUpActionProvider.CloseDialog();return false;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
        </Body>
    </ascw:Container>
</div>