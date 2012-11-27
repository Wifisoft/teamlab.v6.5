<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master" CodeBehind="Projects.aspx.cs" Inherits="ASC.Web.Projects.Projects" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ MasterType  TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("allprojects.css") %>" type="text/css" rel="stylesheet" />
    <%if (!IsEmptyListProjects)
      {%>
    <script>
        jq(document).ready(function() {
            serviceManager.init("<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project", "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>");
        });
    </script>
    <% } %>
</asp:Content>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server"> 

    <%if (IsEmptyListProjects)
      {%>
        <asp:PlaceHolder ID="_escNoProj" runat="server"></asp:PlaceHolder>
    <% }
      else
      {%>
        <%if(ProjectSecurity.CanCreateProject()){%>
            <a href="projects.aspx?action=add" class="addProject"><%=ProjectResource.CreateNewProject%></a>
        <% } %>
        <asp:PlaceHolder runat="server" ID="_content"></asp:PlaceHolder>
        <div class="presetContainer">
            <a id="preset_my" class="baseLinkAction"><%=ProjectsFilterResource.MyProjects %></a>
            <span>,</span>
            <a id="preset_follow" class="baseLinkAction"><%=ProjectsFilterResource.ProjectsFollow%></a>
             <span>,</span>
            <a id="preset_open" class="baseLinkAction"><%=ProjectsFilterResource.OpenProjects%></a>
        </div> 
        <asp:PlaceHolder runat="server" ID="projectListEmptyScreen"></asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="__listProjects"></asp:PlaceHolder> 
    <% } %>                              
</asp:Content>

