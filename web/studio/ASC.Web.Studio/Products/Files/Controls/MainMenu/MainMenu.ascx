<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.ascx.cs"
    Inherits="ASC.Web.Files.Controls.MainMenu" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>

<% #if (DEBUG) %>
    <link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/MainMenu/mainmenu.css" )%>" type="text/css" rel="stylesheet" />
<% #endif %>

<%if (SecurityContext.IsAuthenticated)%>
<%{ %>
<ul id="mainMenuHolder">
    <% if(EnableCreateFile)
       {%>
    <li>
        <span id="topNewFile">
            <a id="files_newdoc_btn" class="baseLinkAction" title="<%=FilesUCResource.NewFile%>">
                <%=FilesUCResource.NewFile%>
            </a>
            <img alt="" src="<%=PathProvider.GetImagePath("combobox_black.png")%>" />
        </span>
    </li>
    <% } %>
    <% if(EnableUpload)
       {%>
    <li>
        <span id="topUpload">
            <a class="baseLinkAction" title="<%=FilesUCResource.ButtonUpload%>">
                <%=FilesUCResource.ButtonUpload%>
            </a>
        </span>
    </li>
    <% } %>
    <% if(EnableCreateFolder)
       {%>
    <li>
        <span id="topNewFolder" >
            <a class="baseLinkAction" title="<%=FilesUCResource.ButtonCreateFolder%>">
                <%=FilesUCResource.ButtonCreateFolder%>
            </a>
        </span>
    </li>
    <% } %>
    <% if(EnableImport)
       {%>
    <li>
        <span id="topImport" >
            <a id="files_important_btn" class="baseLinkAction" title="<%=FilesUCResource.ButtonImportFrom%>">
                <%=FilesUCResource.ButtonImportFrom%>
            </a>
            <img alt="" src="<%=PathProvider.GetImagePath("combobox_black.png")%>" />
        </span>
    </li>
    <% } %>
    <%if(EnableThirdParty)%>
    <% { %>
    <li>
        <span id="topAddThirdParty" class="topAddThirdPartyBeta">
            <a id="files_addThirdParty_btn" class="baseLinkAction" title="<%= FilesUCResource.ButtonAddThirdParty %>">
                <%= FilesUCResource.ButtonAddThirdParty %>
            </a>
            <img alt="" src="<%= PathProvider.GetImagePath("combobox_black.png") %>" />
            <sup class="beta">BETA</sup>
        </span>
    </li>
    <% } %>
</ul>
<%} %>

<%-- popup window --%>
<% if(EnableCreateFile)
{%>
    <div id="files_newDocumentPanel" class="files_action_panel withIcon files_popup_win" style="margin: 5px 0 0 -33px; z-index: 260; display: none;">
        <div class="popup-corner" style="left: 33px;"></div>
        <ul>
            <li id="files_create_document">
                <a><%=FilesUCResource.ButtonCreateText%></a>
            </li>
            <li id="files_create_spreadsheet">
                <a><%=FilesUCResource.ButtonCreateSpreadsheet%></a>
            </li>
            <li id="files_create_presentation">
                <a><%=FilesUCResource.ButtonCreatePresentation%></a>
            </li>
            <li id="files_create_image">
                <a><%=FilesUCResource.ButtonCreateImage%></a>
            </li>
        </ul>
    </div>
<%} %>

<%--dialog window--%>
<% if(EnableUpload)
{%>
<div id="files_uploadDialog" class="popupModal" style="display:none;">
    <ascw:container id="uploadDialogTemp" runat="server">
        <header>
            <div id="uploadDialogContainerHeader"></div>
        </header>
        <body>
            <div class="panelContent">
                <div id="files_uploadHeader" class="header"></div>
                <table id="files_upload_select" cellpadding="0" cellspacing="0" border="0">
                    <tr valign="top">
                        <td style="width: 50px; padding: 5px 0 0 10px;">
                            <div class="files_uploadIcon"></div>
                        </td>
                        <td height="20" style="vertical-align: middle">
                            <div class="describeUpload"><%=ASC.Web.Studio.Core.FileSizeComment.GetFileSizeNote()%></div>
                        </td>
                    </tr>
                </table>
                <div id="files_overallprocessHolder"></div>
                <div id="files_upload_fileList" ></div>
                <div id="files_upload_pnl" style="display: none; padding: 15px 0 10px;">
                    <div id="files_swf_button_container" class="clearFix">
                        <a id="files_upload_btn" class="grayLinkButton files_upload_btn">
                            <%=FilesUCResource.ButtonSelectFiles%>
                        </a>
                        <div id="ProgressFileUploader">
                            <ascw:ProgressFileUploader ID="FileUploader" EnableHtml5="true" runat="server" />
                        </div>
                        <div style="float: right">
                            <asp:PlaceHolder ID="_uploadSwitchHolder" runat="server"></asp:PlaceHolder>
                        </div>
                    </div>
                </div>
                <div id="files_upload_btn_html5" class="files_upload_btn_html5" style="display: none;">
                </div>
            </div>
            <div id="files_uploadDlgPanelButtons" class="panelButtons">
                <div id="panelButtons">
                    <a id="files_uploadSubmit" class="baseLinkButton">
                        <%=FilesUCResource.ButtonUpload%>
                    </a>
                    <a id="files_cancelUpload" class="grayLinkButton" style="margin-left: 8px;">
                        <%=FilesUCResource.ButtonClose%>
                    </a>
                </div>
                <div id="upload_finish" style="display: none;">
                    <a id="files_okUpload" class="baseLinkButton" style="width: 60px;">
                        <%=FilesUCResource.ButtonOk%>
                    </a>
                </div>
            </div>
        </body>
    </ascw:container>
</div>
<%} %>