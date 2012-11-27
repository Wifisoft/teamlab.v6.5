<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectDocumentsPopup.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.ProjectDocumentsPopup.ProjectDocumentsPopup" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" %>

<ascwc:Container id="_documentUploader" runat="server">
        <header>
        <%=PopupName %>    
        </header>
        <body>
            <div class="popupContainerBreadCrumbs">
                
            </div>
            <p class="containerCheckAll" style="display:none;"><input type="checkbox" onchange="ProjectDocumentsPopup.selectAll();" title="Select all" id="checkAll"/><label for="checkAll"><%=UserControlsCommonResource.CheckAll%></label></p>
            <div class="fileContainer" projId = "<%=ProjectId %>">
                <img class="loader" src="<%= WebImageSupplier.GetAbsoluteWebPath("loader.gif")%>"/>
                <div id="emptyFileList" style="display:none;">
                    <asp:PlaceHolder runat="server" ID="_phEmptyDocView"></asp:PlaceHolder>
                </div>
                <ul class='fileList'>
                </ul>
            </div>
            <div class="buttonContainer">
                <a class="baseLinkButton disable"><%=UserControlsCommonResource.AttachFiles %></a>
                <span class="splitter"></span>
                <a class="grayLinkButton"><%=UserControlsCommonResource.CancelButton %></a>
            </div>
        </body>
     </ascwc:Container>