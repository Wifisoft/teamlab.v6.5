<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Attachments.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.Attachments.Attachments" %>
<%@ Register TagPrefix="ascw" Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" %>
<%@ Import Namespace="Resources" %>

<div class="infoPanelAttachFile" >
    <div id="fileMaxSize"><%=ASC.Web.Studio.Core.FileSizeComment.GetFileSizeNote()%></div>
    <div class="warn" id="errorFileUpload"></div>
    <div class="warn" id="wrongSign"><%=UserControlsCommonResource.ErrorMassage_SpecCharacter %></div>
</div>
<div id="files_newDocumentPanel" class="files_action_panel files_popup_win" >
        <div class="popup-corner" style="left: 30px;"></div>
        <ul>
            <li id="files_create_text">
                <a onclick="Attachments.createNewDocument('.doc');"><%= UserControlsCommonResource.ButtonCreateText%></a>
            </li>
            <li id="files_create_spreadsheet">
                <a onclick="Attachments.createNewDocument('.xls');"><%= UserControlsCommonResource.ButtonCreateSpreadsheet%></a>
            </li>
            <li id="files_create_presentation">
                <a onclick="Attachments.createNewDocument('.pptx');"><%= UserControlsCommonResource.ButtonCreatePresentation%></a>
            </li>
            <li id="files_create_picture">
                <a onclick="Attachments.createNewDocument('.svg');"><%= UserControlsCommonResource.ButtonCreateImage %></a>
            </li>
        </ul>
    </div>
           
<div id="actionPanel" runat="server" class="containerAction">
    <span id="showDocumentPanel" ><a class="baseLinkAction"><%=MenuNewDocument %></a><img src = "<%=VirtualPathUtility.ToAbsolute("~/UserControls/Common/Attachments/Images/combobox_black.png") %>" class="newDocComb" /></span>
    <span id="linkNewDocumentUpload"><a class="baseLinkAction"><%=MenuUploadFile %></a></span>
    
    <%if (PortalDocUploaderVisible)
      {%>
    <span id="portalDocUploader" class="linkAttachFile" onclick="ProjectDocumentsPopup.showPortalDocUploader()" href="javascript: false;"><a class="baseLinkAction"><%=MenuProjectDocuments %></a></span>
    <% } %>
    
</div>
<div id="questionWindowAttachments" style="display: none">
    <ascw:Container ID="_hintPopup" runat="server">
    <Header>
    <%=UserControlsCommonResource.DeleteFile %>
    </Header>
    <Body>        
        <p><%=UserControlsCommonResource.QuestionDeleteFile%></p>
        <p><%=UserControlsCommonResource.NotBeUndone%></p>
        <p><a class="baseLinkButton" style="margin-right: 8px;" id="okButton"><%=UserControlsCommonResource.DeleteFile%></a><a id="noButton" class="grayLinkButton"><%=UserControlsCommonResource.CancelButton %></a></p>    
    </Body>
    </ascw:Container>
</div>
<%-- popup window --%>
<div id="files_hintCreatePanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=UserControlsCommonResource.TooltipCreate%>
    <a href="http://www.teamlab.com/help/guides/createdocument.aspx" target="_blank"><%=UserControlsCommonResource.ButtonLearnMore%></a>
</div>
<div id="files_hintUploadPanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=UserControlsCommonResource.TooltipUpload%>
</div>
<div id="files_hintOpenPanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=string.Format(UserControlsCommonResource.TooltipOpen, ExtsWebPreviewed)%>
    <a href="http://www.teamlab.com/help/guides/editdocument.aspx" target="_blank"><%=UserControlsCommonResource.ButtonLearnMore%></a>
</div>
<div id="files_hintEditPanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=string.Format(UserControlsCommonResource.TooltipEdit, ExtsWebEdited)%>
    <a href="http://www.teamlab.com/help/guides/editdocument.aspx" target="_blank"><%=UserControlsCommonResource.ButtonLearnMore%></a>
</div>

<div class="wrapperFilesContainer" moduleName="<%=ModuleName%>" projectId=<%=ProjectId %> entityType=<%=EntityType %>>
    <div id="emptyDocumentPanel" style="display:none;">
        <asp:PlaceHolder runat="server" ID="_phEmptyDocView"></asp:PlaceHolder>
    </div>
    <table id="attachmentsContainer">
        <tbody>
        </tbody>
    </table>
</div>
<div id="popupDocumentUploader">
    <asp:PlaceHolder id="_phDocUploader" runat="server"></asp:PlaceHolder>
</div>

<script id="newFileTmpl" type="text/x-jquery-tmpl">
    <tr class="newDoc">
        <td class="${tdclass}">
            <input id="newDocTitle" type="text" class="textEdit" data="<%= UserControlsCommonResource.NewDocument%>" maxlength="165" value="<%= UserControlsCommonResource.NewDocument%>"/>
            <span id="${type}" onclick="Attachments.createFile();" class="createFile" title="<%= Resource.AddButton%>"></span>
            <span onclick="Attachments.removeNewDocument();" title="<%= UserControlsCommonResource.QuickLinksDeleteLink%>" class="remove"></span>
        </td>
        <td></td>
    </tr>
</script>

<script id="fileAttachTmpl" type="text/x-jquery-tmpl">

<tr>
    <td id="af_${id}">
        {{if type=="image"}}
        
            <a href="#imgZoom_${id}" rel="imageGalery" class="fancyzoom ${exttype}" title="${title}">
                <div class="attachmentsTitle">${title}</div>
                {{if version > 1}}
                        <span class="version"><%= UserControlsCommonResource.Version%>${version}</span>
                {{/if}}
            </a> 
            <div id="imgZoom_${id}">
                <img src="${ViewUrl}"/>
            </div>
            
        {{else}}
            {{if type == "editedFile" || type == "viewedFile"}}
                <a href="${docViewUrl}" class="${exttype}" title="${title}" target="_blank">
                    <div class="attachmentsTitle">${title}</div>
                    {{if version > 1}}
                        <span class="version"><%= UserControlsCommonResource.Version%>${version}</span>
                    {{/if}}
                </a>
            {{else}}
                <a class="${exttype} noEdit" title="${title}" target="_blank">
                    <div class="attachmentsTitle">${title}</div>
                    {{if version > 1}}
                        <span class="version"><%= UserControlsCommonResource.Version%>${version}</span>
                    {{/if}}
                </a>
            {{/if}}
            
        {{/if}}
    </td>
    
    <td class="editFile">
        {{if (access==0 || access==1)}}
            <a class="unlinkDoc" title="<%= UserControlsCommonResource.RemoveFromList%>" href="javascript:Attachments.deattachFile(${id});"></a>
            <a class="deliteCrmFile" title="<%= UserControlsCommonResource.QuickLinksDeleteLink%>"  href="javascript:Attachments.deattachFile(${id});"></a>
        {{/if}}
        {{if (!jq.browser.mobile)}}
        <a class="downloadLink" title="<%= UserControlsCommonResource.QuickLinksDownloadLink%>" href="${downloadUrl}"></a>
        {{/if}}
        {{if (type == "editedFile")&&(access==0 || access==1)}}
            <a id="editDoc_${id}" title="<%= UserControlsCommonResource.QuickLinksEditLink%>" target="_blank" href="${editUrl}"></a>
        {{/if}}
    </td>
</tr>

</script>