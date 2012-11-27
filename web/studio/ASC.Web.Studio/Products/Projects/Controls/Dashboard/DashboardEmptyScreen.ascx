<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardEmptyScreen.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Dashboard.DashboardEmptyScreen" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<style type="text/css">
    #contentFooter td.digits
    {
        color: #3186AF;
        font-size: 46px;
        font-family: Verdana;
        font-weight: bold;
        float: left;
        padding-right: 5px;
    }
    #contentFooter div.screen-block .header
    {
        font-size: 16px;
        font-weight: bold;
    }
    #contentFooter div.screen-block .content
    {
        padding-top: 4px;
        font-size: 12px;
    }
    #contentFooter div.screen-block
    {
        width: 33%;
        float: left;
        padding-top: 30px;
    }
    #contentHeader
    {
        text-align: center;
    }
    #contentHeader a
    {
        font-size: 30px;
        font-weight: bold;
    }
    #contentHeader div
    {
        margin-top: 5px;
        font-size: 14px;
    }
</style>
<div id="content" class="DashBoardProjectsBlock">
  <%--  <div id="contentHeader">
        <a class="create-new-project" href="projects.aspx?action=add">
            <%= ProjectResource.CreateNewProject %></a>
        <div>
            <%= ProjectsCommonResource.ScreenBlock_Content%>
        </div>
        <div>
            <%= ProjectsCommonResource.Or%>
        </div>
        <a href="settings.aspx" class="linkMediumDark" style="font-size:14px;font-weight:bolder;">
            <%= SettingsResource.ImportProjectsFromTheBaseCamp%></a>
    </div>
    <div id="contentFooter" class="clearFix">
        <div class="screen-block">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="digits">
                        1
                    </td>
                    <td>
                        <span class="header">
                            <%= ProjectsCommonResource.ScreenBlock1_Header%></span>
                        <div class="content">
                            <%= ProjectsCommonResource.ScreenBlock1_Content%>
                        </div>
                    </td>
                </tr>
            </table>
            <img title="<%= ProjectsCommonResource.ScreenBlock1_Header%>" alt="<%= ProjectsCommonResource.ScreenBlock1_Header%>"
                width="284px" height="177px" src="<%= WebImageSupplier.GetAbsoluteWebPath("dashboard_start_01.png", ProductEntryPoint.ID) %>" />
        </div>
        <div class="screen-block">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="digits">
                        2
                    </td>
                    <td>
                        <span class="header">
                            <%= ProjectsCommonResource.ScreenBlock2_Header%></span>
                        <div class="content">
                            <%= ProjectsCommonResource.ScreenBlock2_Content%>
                        </div>
                    </td>
                </tr>
            </table>
            <img title="<%= ProjectsCommonResource.ScreenBlock2_Header%>" alt="<%= ProjectsCommonResource.ScreenBlock2_Header%>"
                width="284px" height="177px" src="<%= WebImageSupplier.GetAbsoluteWebPath("dashboard_start_02.png", ProductEntryPoint.ID) %>" />
        </div>
        <div class="screen-block">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="digits">
                        3
                    </td>
                    <td>
                        <span class="header">
                            <%= ProjectsCommonResource.ScreenBlock3_Header%></span>
                        <div class="content">
                            <%= ProjectsCommonResource.ScreenBlock3_Content%>
                        </div>
                    </td>
                </tr>
            </table>
            <img title="<%= ProjectsCommonResource.ScreenBlock3_Header%>" alt="<%= ProjectsCommonResource.ScreenBlock3_Header%>"
                width="284px" height="177px" src="<%= WebImageSupplier.GetAbsoluteWebPath("dashboard_start_03.png", ProductEntryPoint.ID) %>" />
        </div>
        <div class="screen-block">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="digits">
                        4
                    </td>
                    <td>
                        <span class="header">
                            <%= ProjectsCommonResource.ScreenBlock4_Header%></span>
                        <div class="content">
                            <%= ProjectsCommonResource.ScreenBlock4_Content%>
                        </div>
                    </td>
                </tr>
            </table>
            <img title="<%= ProjectsCommonResource.ScreenBlock4_Header%>" alt="<%= ProjectsCommonResource.ScreenBlock4_Header%>"
                width="284px" height="177px" src="<%= WebImageSupplier.GetAbsoluteWebPath("dashboard_start_04.png", ProductEntryPoint.ID) %>" />
        </div>
        <div class="screen-block">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="digits">
                        5
                    </td>
                    <td>
                        <span class="header">
                            <%= ProjectsCommonResource.ScreenBlock5_Header%></span>
                        <div class="content">
                            <%= ProjectsCommonResource.ScreenBlock5_Content%>
                        </div>
                    </td>
                </tr>
            </table>
            <img title="<%= ProjectsCommonResource.ScreenBlock5_Header%>" alt="<%= ProjectsCommonResource.ScreenBlock5_Header%>"
                width="284px" height="177px" src="<%= WebImageSupplier.GetAbsoluteWebPath("dashboard_start_05.png", ProductEntryPoint.ID) %>" />
        </div>
        <div class="screen-block">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="digits">
                        6
                    </td>
                    <td>
                        <span class="header">
                            <%= ProjectsCommonResource.ScreenBlock6_Header%></span>
                        <div class="content">
                            <%= ProjectsCommonResource.ScreenBlock6_Content%>
                        </div>
                    </td>
                </tr>
            </table>
            <img title="<%= ProjectsCommonResource.ScreenBlock6_Header%>" alt="<%= ProjectsCommonResource.ScreenBlock6_Header%>"
                width="284px" height="177px" src="<%= WebImageSupplier.GetAbsoluteWebPath("dashboard_start_06.png", ProductEntryPoint.ID) %>" />
        </div>
    </div>--%>
    
    <div class="HeaderBlock">
        <p><%=ProjectsCommonResource.DashboardTitle%></p>
    </div>
    <div class="ContentBlock">
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/images/icon-tasks.png")%>" />
           <div class="title"><%=ProjectsCommonResource.TasksModuleTitle%></div>
           <ul>
               <li><%=ProjectsCommonResource.TasksModuleFirstLine%></li>
               <li><%=ProjectsCommonResource.TasksModuleSecondLine%></li>
               <li><%=ProjectsCommonResource.TasksModuleThirdLine%></li>
           </ul>
       </div>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/images/icon-milestones.png")%>" />
           <div class="title"><%=ProjectsCommonResource.MilestonesModuleTitle%></div>
           <ul>
               <li><%=ProjectsCommonResource.MilestonesModuleFirstLine%></li>
               <li><%=ProjectsCommonResource.MilestonesModuleSecondLine%></li>
               <li><%=ProjectsCommonResource.MilestonesModuleThirdLine%></li>
           </ul>
       </div>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/images/icon-document.png")%>" />
           <div class="title"><%=ProjectsCommonResource.DocsModuleTitle%></div>
           <ul>
               <li><%=ProjectsCommonResource.DocsModuleFirstLine%></li>
               <li><%=ProjectsCommonResource.DocsModuleSecondLine%></li>
               <li><%=ProjectsCommonResource.DocsModuleThirdLine%></li>
           </ul>
       </div>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/images/icon-discussion.png")%>" />
           <div class="title"><%=ProjectsCommonResource.DiscussionModuleTitle%></div>
           <ul>
               <li><%=ProjectsCommonResource.DiscussionModuleFirstLine%></li>
               <li><%=ProjectsCommonResource.DiscussionModuleSecondLine%></li>
               <li><%=ProjectsCommonResource.DiscussionModuleThirdLine%></li>
           </ul>
       </div>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/images/icon-report.png")%>" />
           <div class="title"><%=ProjectsCommonResource.ReportsModuleTitle%></div>
           <ul>
               <li><%=ProjectsCommonResource.ReportsModuleFirstLine%></li>
               <li><%=ProjectsCommonResource.ReportsModuleSecondLine%></li>
               <li><%=ProjectsCommonResource.ReportsModuleThirdLine%></li>
           </ul>
       </div>
    </div>
    <% if (Global.IsAdmin) %>
    <%{%>
    <div class="ButtonsProject">
        <a class="CreateProjectButton" href="projects.aspx?action=add"><%= ProjectsCommonResource.CreateProjectButtonText %></a>
        
    </div>
    <% } %>
</div>
