<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportControl.ascx.cs"
    Inherits="ASC.Web.Files.Controls.ImportControl" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Import Namespace="ASC.Web.Files.Import" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<% #if (DEBUG) %>
<link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/ImportControl/importcontrol.css" )%>"
    type="text/css" rel="stylesheet" />
<% #endif %>

<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/ImportControl/importcontrol.js" ) %>"></script>

<script type="text/javascript" language="javascript">
    ASC.Files.Constants.URL_OAUTH_GOOGLE = "<%=ASC.Web.Files.Import.Google.OAuth.Location%>";
    ASC.Files.Constants.URL_OAUTH_BOXNET = "<%=ASC.Web.Files.Import.Boxnet.BoxLogin.Location%>";
</script>

<div id="importListPanel" class="files_action_panel withIcon files_popup_win" style="margin: 5px 0 0 -150px; z-index: 260; display: none;">
    <div class="popup-corner" style="left: 150px;">
    </div>
    <ul>
        <% if (ImportConfiguration.SupportGoogleImport) %>
        <% { %>
            <li id="import_from_google"><a><%=FilesUCResource.ImportFromGoogle%></a></li>
        <% } %>
        <% if (ImportConfiguration.SupportZohoImport) %>
        <% { %>
            <li id="import_from_zoho"><a><%=FilesUCResource.ImportFromZoho%></a></li>
        <% } %>
        <% if (ImportConfiguration.SupportBoxNetImport) %>
        <% { %>
        <li id="import_from_boxnet"><a><%=FilesUCResource.ImportFromBoxNet%></a></li>
        <% } %>
    </ul>
</div>

<% if (ImportConfiguration.SupportImport) %>
<% { %>
<div id="ImportLoginDialog" class="popupModal" style="display: none;">
    <ascw:container id="LoginDialogTemp" runat="server">
        <header><%=FilesUCResource.ImportFromZoho%></header>
        <body>
            <div style="margin-bottom: 15px;">
                <div><%=FilesUCResource.Login%></div>
                <input type="text" id="files_login" class="textEdit" />
            </div>
            <div style="margin-bottom: 15px;">
                <div><%=FilesUCResource.Password%></div>
                <input type="password" id="files_pass" class="textEdit" />
            </div>
            <div class="action-block">
                <a id="files_submitLoginDialog" class="baseLinkButton">
                    <%=FilesUCResource.ButtonOk%></a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton" onclick="PopupKeyUpActionProvider.CloseDialog();return false;">
                    <%=FilesUCResource.ButtonCancel%></a>
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

<div id="ImportDialog" class="popupModal" style="display: none;">
    <ascw:container id="ImportDialogTemp" runat="server">
        <header>
           <span id="ImportDialogHeader" class="header-content"></span>
        </header>
        <body>
            <div id="import_data">
            </div>
            <div id="import_to_folder" class="startHide">
                <%=FilesUCResource.ImportToFolder%>:
                <a id="files_importToFolderBtn" style="margin:0 2px -3px;" class="import_tree"></a>
                <font id="files_importToFolder"><%=FilesUCResource.MyFiles%></font>
                <font>&nbsp;>&nbsp;</font>
                <span id="import_to_folderName"></span>
                <input type="hidden" id="files_importToFolderId" value="" />
            </div>
            <div class="startHide">
                <%=FilesUCResource.IfFileNameConflict%>:
                <label>
                    <input type="radio" name="file_conflict" value="true" checked="checked" />
                    <%=FilesUCResource.Overwrite%>
                </label>
                <label>
                    <input type="radio" name="file_conflict" value="false" />
                    <%=FilesUCResource.Ignore%>
                </label>
            </div>
            <div class="seporator startHide">
            </div>
            <div class="clearFix">
                <div class="action-block" style="float: left; display: none;">
                    <a id="files_startImportData" class="baseLinkButton">
                        <%=FilesUCResource.ButtonImport%>
                    </a>
                    <span class="button-splitter"></span>
                    <a class="grayLinkButton" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;">
                        <%=FilesUCResource.ButtonCancel%>
                    </a>
                </div>
                <div class="action-block-progress" style="float: left;">
                    <a id="files_import_minimize" class="baseLinkButton" title="<%=FilesUCResource.ButtonMinimizeImport%>">
                        <%=FilesUCResource.ButtonMinimize%>
                    </a>
                    <a id="files_import_terminate" class="grayLinkButton" style="margin-left: 8px;" title="<%=FilesUCResource.ButtonCancelImport%>">
                        <%=FilesUCResource.ButtonCancel%>
                    </a>
                </div>
                <div class="import_progress_panel">
                    <div>
                        <div style="float: left;">
                            <span class="textBigDescribe"><%=FilesUCResource.ImportProgress%>: </span>
                        </div>
                        <div>
                        </div>
                        <div style="float: right;">
                            <span class="textBigDescribe"><span class="percent">0</span>%</span>
                        </div>
                        <br style="clear: both;" />
                    </div>
                    <div class="studioFileUploaderProgressBorder">
                        <div class="studioFileUploaderProgressBar" style="width: 0px">
                        </div>
                    </div>
                </div>
            </div>
        </body>
    </ascw:container>
</div>
<% } %>