<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Studio" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Masters/BasicTemplate.Master"
    CodeBehind="ProjectOverview.aspx.cs" Inherits="ASC.Web.Projects.ProjectOverview" %>

<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projectoverview.css") %>"
        type="text/css" rel="stylesheet" />

    <script src="<%= PathProvider.GetFileStaticRelativePath("projectoverview.js") %>"
        type="text/javascript"></script>

</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <div id="headerBlockContainer">
        <a class="containerBreadCrumbLink" href="projects.aspx">
            <%= ProjectResource.Projects %>
        </a><span class="containerBreadCrumbSpiter">&gt;</span> <span class="containerBreadCrumbLink">
            <%= HttpUtility.HtmlEncode(ProjectFat.Project.HtmlTitle) %></span>
    </div>
    <div id="projectTitleContainer">
        <% if (ProjectFat.Project.Private)
           { %>
        <div id="privateProjectMark">
        </div>
        <% } %>
        <div id="statusProjectMark" class="<%= GetProjectStatus() %>">
        </div>
        <div class="headerProjectTitle" title="<%= HttpUtility.HtmlEncode(ProjectFat.Project.Title) %>">
            <%= GetProjectTitle() %></div>
        <div style="clear: both">
        </div>
    </div>

    <div id="projectActionsContainer">
        <% if (CanEditProject()){ %>
        <div id="updateProject">
            <a class="dottedLink" href="<%= GetUpdateProjectLink() %>">
                <%= ProjectResource.EditProject %>
            </a>
        </div>
        <% } %>
        <%if (CanDeleteProject()){ %>
        <div id="deleteProject">
            <a class="dottedLink">
                <%= ProjectResource.DeleteProject %>
            </a>
        </div>
        <%} %>
        <div id="seeHistoryButton">
            <a class="dottedLink" href="<%= GetHistoryProjectLink() %>">
                <%= ProjectResource.SeeHistory %>
            </a>
        </div>
        <% if (!IsInProjectTeam()){ %>
        <div id="followProjectButton">
            <% if (IsFollowProject())
               { %>
            <span class="dottedLink" follow="<%= ProjectResource.FollowingProjects %>">
                <%= ProjectResource.UnFollowingProjects%>
            </span>
            <% }
               else
               { %>
            <span class="dottedLink" follow="<%= ProjectResource.UnFollowingProjects %>">
                <%= ProjectResource.FollowingProjects %>
            </span>
            <% } %>
        </div>
        <% } %>
    </div>

    <% if (IsProjectDescriptionNotEmpty())
       { %>
    <div id="projectDescriptionContainer">
        <div class="headerBase">
            <%= ProjectResource.ProjectDescription %></div>
        <div class="textBase">
            <%= GetProjectDescription() %></div>
    </div>
    <% } %>
    <div id="projectTeamContainer">
        <div class="headerBase">
            <%= ProjectResource.ProjectLeader %></div>
        <%--<div class="headerProjectManager">
            <%= ProjectResource.ProjectLeader %></div>--%>
        <div id="projectManagerContainer">
            <asp:PlaceHolder runat="server" ID="projectManagerPlaceHolder"></asp:PlaceHolder>
        </div>
        <div id="projectTeamLinksContainer">
            <div class="headerBase">
                <%= ProjectResource.ProjectTeam %>:</div>
            <div class="links">
                <a href="<%= GetProjectTeamLink() %>" id="teamCount">
                    <%= GetGrammaticalHelperParticipantCount() %></a>
                <% if (ProjectSecurity.CanEditTeam(ProjectFat.Project))
                   { %>
                <span id="manageTeamButton" class="dottedLink">
                    <%= ProjectResource.ManagmentTeam %></span>
                <% } %>
            </div>
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="teamManagementPlaceHolder" />
    <div id="projectStatisticContainer">
        <div class="headerBase">
            <%= ProjectResource.Statistics %></div>
        <table id="statisticList">
            <tr>
                <td class="title">
                    <%= ProjectsCommonResource.Milestones %>
                </td>
                <td class="count">
                    <%= GetMilestones() %>
                </td>
                <td class="activeCount">
                    <%= GetOpenMilestones() %>
                </td>
                <td class="overdueCount">
                    <%= GetOverdueMilestones() %>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <%= TaskResource.Tasks %>
                </td>
                <td class="count">
                    <%= GetTasks() %>
                </td>
                <td class="activeCount">
                    <%= GetOpenTasks() %>
                </td>
                <td class="overdueCount">
                    <%= GetOverdueTasks() %>
                </td>
            </tr>
            <%if (ProjectSecurity.CanReadMessages(ProjectFat.Project)){%>
            <tr>
                <td class="title">
                    <%=MessageResource.Messages%>
                </td>
                <td class="count" colspan="3">
                    <%=GetDiscussions()%>
                </td>
            </tr>
            <% } %>
            <%if (ProjectSecurity.CanReadFiles(ProjectFat.Project)){%>
            <tr>
                <td class="title">
                    <%= ProjectResource.Files %>
                </td>
                <td class="count" colspan="3">
                    <%= GetFiles() %>
                </td>
            </tr>            
            <% } %>
        </table>
    </div>
    <div style="clear: both">
    </div>
    <div id="questionWindow" style="display: none">
        <ascw:Container ID="_hintPopup" runat="server">
            <header><%= ProjectResource.DeleteProject %></header>
            <body>
                <p>
                    <%= ProjectResource.DeleteProjectPopup %>
                </p>
                <p>
                    <%= ProjectsCommonResource.PopupNoteUndone %></p>
                <div class="popupButtonContainer">
                    <a class="baseLinkButton remove">
                        <%= ProjectResource.DeleteProject %></a> <span class="button-splitter"></span>
                    <a class="grayLinkButton cancel">
                        <%= ProjectsCommonResource.Cancel %></a>
                </div>
            </body>
        </ascw:Container>
    </div>
</asp:Content>
