<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectsWidget.ascx.cs"
                        Inherits="ASC.Web.Projects.Controls.Dashboard.ProjectsWidget" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<div class="pm-dashboard-fadeParent">
<%if (IsMyProjectsExist) { %>
            
<div id="myProjectsListContainer">
    <div class="listHeader"><%= ProjectsCommonResource.PersonalProjAndTasks %></div>
    <asp:Repeater ID="MyProjectsRepeater" runat="server">
        <ItemTemplate>
            <div class="projectContainer">
                <div class="pm-dashboard-fade"></div>
                <div class="projectLinkContainer">
                    <a class="projectLink" href="<%# GetProjectLink(((Project)Container.DataItem).ID) %>"
                                            title="<%# ((Project)Container.DataItem).Title.HtmlEncode() %>">
                        <%# ((Project)Container.DataItem).Title.HtmlEncode() %>
                    </a>
                </div>
                <div class="tasksContainer">
                    <asp:Repeater ID="OverdueTasksRepeater" datasource="<%# GetMyOverdueTasks(((Project)Container.DataItem).ID) %>" runat="server">
                        <ItemTemplate>
                            <span class="taskIndicator overdueTask"><%# GetShortTaskDeadline(((Task)Container.DataItem)) %></span>
                            <div class="taskLinkContainer">
                                <div class="pm-dashboard-fade"></div>
                                <a class="taskLink" href="<%# GetTaskLink((Task)Container.DataItem) %>">
                                    <%# ((Task)Container.DataItem).Title.HtmlEncode() %>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="NewTasksRepeater" datasource="<%# GetMyNewTasks(((Project)Container.DataItem).ID) %>" runat="server">
                        <ItemTemplate>
                            <span class="taskIndicator nextTask"><%# TaskResource.NewTaskIndicator %></span>
                                <div class="taskLinkContainer">
                                    <div class="pm-dashboard-fade"></div>
                                    <a class="taskLink" href="<%# GetTaskLink((Task)Container.DataItem) %>">
                                        <%# ((Task)Container.DataItem).Title.HtmlEncode() %>
                                    </a>
                                </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="OtherTasksRepeater" datasource="<%# GetMyOtherTasks(((Project)Container.DataItem).ID) %>" runat="server">
                        <ItemTemplate>
                            <div class="taskLinkContainer">
                                <div class="pm-dashboard-fade"></div>
                                <a class="taskLink" href="<%# GetTaskLink((Task)Container.DataItem) %>">
                                    <%# ((Task)Container.DataItem).Title.HtmlEncode() %>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div class="linksContainer">
    <a href="tasks.aspx#sortBy=deadline&sortOrder=ascending&tasks_rasponsible=<%= SecurityContext.CurrentAccount.ID %>">
        <%= ProjectsCommonResource.AllMyTasks %>
    </a>
    <span class="splitter">|</span>
    <a href="projects.aspx#sortBy=create_on&sortOrder=ascending&team_member=<%= SecurityContext.CurrentAccount.ID %>">
        <%= ProjectsCommonResource.AllMyProjects %>
    </a>
</div>
<% } %>
<% if (ShowFollowingProjects && IsFollowingProjectsExist) { %>

<div id="followingProjectsListContainer">  
    <div class="listHeader"><%= ProjectsCommonResource.FollowedProjects %></div>
    <asp:Repeater ID="FollowingProjectsRepeater" runat="server">
        <ItemTemplate>
            <div class="projectContainer">
                <div class="pm-dashboard-fade"></div>
                <div class="projectLinkContainer follow">
                    <a class="projectLink" href="<%# GetProjectLink(((Project)Container.DataItem).ID) %>"
                                            title="<%# ((Project)Container.DataItem).Title.HtmlEncode() %>">
                        <%# ((Project)Container.DataItem).Title.HtmlEncode() %>
                    </a>
                </div>                
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<% } %>
<% if ((ShowFollowingProjects && IsFollowingProjectsExist) || ((ShowFollowingTasks && IsFollowingTasksExist))) { %>
    <div class="linksContainer">
        <a href="projects.aspx#sortBy=create_on&sortOrder=ascending&followed=true">
            <%= ProjectsCommonResource.AllFollowedProjects %>
        </a>
    </div>
<% } %>
</div>
