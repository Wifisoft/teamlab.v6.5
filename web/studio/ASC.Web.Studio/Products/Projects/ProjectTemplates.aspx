<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
    CodeBehind="ProjectTemplates.aspx.cs" Inherits="ASC.Web.Projects.ProjectTemplates" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
    
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>


<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projecttemplates.css") %>"
        rel="stylesheet" type="text/css" />

    <script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("projecttemplates.js") %>"></script>
    <script>
        jq(document).ready(function () {
            ASC.Projects.ListProjectsTemplates.init();
        })
    </script>
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    
<% if(EmptyListTemplates)
   { %>
        <div id="emptyListTemplates">
   <% } else
   {%>
        <div id="emptyListTemplates" style="display:none;">
   <% } %>
            <asp:PlaceHolder ID="_escNoTmpl" runat="server"></asp:PlaceHolder>

        </div>
 
 <% if(!EmptyListTemplates)
   { %>
        
    <script id="templateTmpl" type="text/x-jquery-tmpl">
        <div class="template menuButtonContainer" id="${id}">
              <a href="editprojecttemplate.aspx?id=${id}" class="title">${title}</a>
              <span class="description">(${milestones} <%= ProjectTemplatesResource.Milestones %>, ${tasks} <%= ProjectTemplatesResource.Tasks %>)</span>
              <span class="menuButton"></span>
        </div>
    </script> 
       
        <a href="editprojecttemplate.aspx" class="addTemplate"><%= ProjectTemplatesResource.CreateTmpl %></a>
        
        <div id="listTemplates">

        </div>
        
    <div id="templateActionPanel" class="actionPanel" target="">
	    <div class="popup-corner"></div>
	    <ul class="actionList">
	        <li id="editTmpl"><%= ProjectTemplatesResource.Edit %></li>
	        <li id="createProj"><%= ProjectTemplatesResource.CreateProjFromTmpl %></li>
	        <li id="deleteTmpl"><%= ProjectTemplatesResource.Delete %></li>
	    </ul>        
    </div>
    
    <div id="questionWindow" style="display: none">
        <ascw:Container ID="_hintPopup" runat="server">
            <Header>
                <%= ProjectTemplatesResource.RemoveTemplateTitlePopup%>
            </Header>
            <Body>        
                <p><%= ProjectTemplatesResource.RemoveQuestion%> </p>
                <p><%= ProjectsCommonResource.PopupNoteUndone %></p>
                <div class="popupButtonContainer">
                    <a class="baseLinkButton remove"><%= ProjectTemplatesResource.RemoveTemplateTitlePopup%></a>
                    <span class="button-splitter"></span>
                    <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel%></a>
                </div>     
            </Body>
        </ascw:Container>
    </div>
 <% } %>   

</asp:Content>
