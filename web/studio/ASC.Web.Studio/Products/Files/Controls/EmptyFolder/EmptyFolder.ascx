<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmptyFolder.ascx.cs" Inherits="ASC.Web.Files.Controls.EmptyFolder" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>

<% #if (DEBUG) %>
<link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/EmptyFolder/emptyfolder.css" )%>" type="text/css" rel="stylesheet" />
<% #endif %>

<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/EmptyFolder/emptyfolder.js") %>"></script>

<asp:PlaceHolder runat="server" ID="EmptyScreenFolder" />


<%-- popup window --%>
<div id="files_hintCreatePanel" class="hintDescriptionPanel">
    <div class="popup-corner" >
    </div>
    <%=FilesUCResource.TooltipCreate%>
    <a href="http://www.teamlab.com/help/guides/createdocument.aspx" target="_blank"><%=FilesUCResource.ButtonLearnMore%></a>
</div>
<div id="files_hintUploadPanel" class="hintDescriptionPanel">
    <div class="popup-corner" ></div>
    <%=FilesUCResource.TooltipUpload%>
</div>
<div id="files_hintOpenPanel" class="hintDescriptionPanel">
    <div class="popup-corner" ></div>
    <%=string.Format(FilesUCResource.TooltipOpen, ExtsWebPreviewed)%>
    <a href="http://www.teamlab.com/help/guides/editdocument.aspx" target="_blank"><%=FilesUCResource.ButtonLearnMore%></a>
</div>
<div id="files_hintEditPanel" class="hintDescriptionPanel">
    <div class="popup-corner" ></div>
    <%=string.Format(FilesUCResource.TooltipEdit, ExtsWebEdited)%>
    <a href="http://www.teamlab.com/help/guides/editdocument.aspx" target="_blank"><%=FilesUCResource.ButtonLearnMore%></a>
</div>