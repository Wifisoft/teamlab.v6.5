<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Controls" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonesWidget.ascx.cs" Inherits="ASC.Web.Projects.Controls.Dashboard.MilestonesWidget" %>

<%@ Import Namespace="ASC.Common.Utils" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<div class="pm-dashboard-fadeParent">
<% if(IsExistOverdueMilestones) { %>
<div class="myMilestonesListHeader"><%= MilestoneResource.Overdue %></div>
<div class="myMilestonesListContainer">              
    <asp:Repeater ID="OverdueMilestonesRepeater" runat="server">
        <HeaderTemplate>
            <table class="myMilestonesList">
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="firstColumn">
                    <div class="myMilestoneDeadline overdueDeadline">
                        <%# GetShortMilestoneDeadline(((Milestone)Container.DataItem).DeadLine) %>
                    </div>
                </td>
                <td>
                    <%# IsKeyMilestone(((Milestone)Container.DataItem).IsKey) %> 
                    <div class="myMilestoneTitleContainer">
                        <span class="myMilestoneTitle">
                            <%# HtmlUtil.GetText(((Milestone)Container.DataItem).Title, 28).HtmlEncode() %>
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <div class="myMilestoneProjectLinkContainer">
                        <a class="myMilestoneProjectLink" href="<%# GetProjectLink(((Milestone)Container.DataItem).Project.ID) %>">
                            <%# ((Milestone)Container.DataItem).Project.Title.HtmlEncode() %>
                        </a>
                    </div>
                </td>
            </tr>            
            <tr>
                <td class="lastRow">
                </td>
                <td class="lastRow">
                     <div class="discussionAuthorContainer">
                        <div class="pm-dashboard-fade"></div>
                        <span class="discussionAuthorLink"><%# ResponsibleProfileLink(((Milestone)Container.DataItem).Responsible) %></span>
                    </div>
                </td>
            </tr>              
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
<% } %>
<% if(IsShouldRenderNextMilestones) { %>
<div class="myMilestonesListHeader"><%= MilestoneResource.Next %></div>
<div class="myMilestonesListContainer">
<asp:Repeater ID="NewMilestonesRepeater" runat="server">
    <HeaderTemplate>
        <table class="myMilestonesList">
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="firstColumn">
                <div class="myMilestoneDeadline nextDeadline">
                    <%# GetShortMilestoneDeadline(((Milestone)Container.DataItem).DeadLine) %>
                </div>
            </td>
            <td>
                <%# IsKeyMilestone(((Milestone)Container.DataItem).IsKey) %> 
                <div class="myMilestoneTitleContainer">
                    <span class="myMilestoneTitle">
                        <%# HtmlUtil.GetText(((Milestone)Container.DataItem).Title, 28).HtmlEncode() %>
                    </span>
                </div>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <div class="myMilestoneProjectLinkContainer">
                    <a class="myMilestoneProjectLink" href="<%# GetProjectLink(((Milestone)Container.DataItem).Project.ID) %>">
                        <%# ((Milestone)Container.DataItem).Project.Title.HtmlEncode() %>
                    </a>
                </div>
            </td>
        </tr>
        <tr>
            <td class="lastRow">
            </td>
            <td class="lastRow">
                 <div class="discussionAuthorContainer">
                    <div class="pm-dashboard-fade"></div>
                    <span class="discussionAuthorLink"><%# ResponsibleProfileLink(((Milestone)Container.DataItem).Responsible) %></span>
                </div>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<div style="margin-bottom: 16px;">
    <a class="generateReportLink" onclick="ASC.Projects.Reports.generateReportByUrl('<%= GetReportUri() %>')">
        <%= ReportResource.GenerateReport %>
    </a>
</div>
</div>
<% } %>
</div>
