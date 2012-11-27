<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectActivity.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Projects.ProjectActivity" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<link href="<%= PathProvider.GetFileStaticRelativePath("common.css") %>" type="text/css" rel="stylesheet" />
<link href="<%= PathProvider.GetFileStaticRelativePath("projectsactivity.css") %>" type="text/css" rel="stylesheet" />

<script type="text/javascript">
    function generateReportByUrl(url)
    {
        open(url, "displayReportWindow", "status=yes,toolbar=yes,menubar=yes,scrollbars=yes");
    }
</script>

<div class="showReport">
    <a onclick="generateReportByUrl('<%=GetTasksReportUri()%>')" class="grayLinkButton showReportBtn">
        <%= ReportResource.ShowUserTasksReport%>
    </a><span class="headerBase">
        <%= ProjectResource.Projects %></span>
</div>
<table class="pm-tablebase" cellpadding="14" cellspacing="0" id="projectTable">
    <tbody>
        <asp:Repeater ID="ProjectsRepeater" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="borderBase nameProject">
                        <a href='products/projects/projects.aspx?prjID=<%# Eval("ProjectID") %>'>
                            <%# ((string)Eval("ProjectTitle")).HtmlEncode() %>
                        </a>
                    </td>
                    <td class="borderBase">
                        <a href='products/projects/tasks.aspx?prjID=<%# Eval("ProjectID") %>&action=3&userID=<%=UserID %>'>
                            <%# Eval("OpenedTasksCount")%>
                            <%# GetOpenedTasksString((int)Eval("OpenedTasksCount"))%>
                        </a>
                    </td>
                    <td class="borderBase">
                        <a href='products/projects/tasks.aspx?prjID=<%# Eval("ProjectID") %>&action=3&userID=<%=UserID %>&view=all'>
                            <%# Eval("ClosedTasksCount")%>
                            <%# GetClosedTasksString((int)Eval("ClosedTasksCount"))%>
                        </a>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
<div style="<%= ProjectsRepeater.Items.Count==0? "display:block;": "display:none;"%>">
    <%=ProjectResource.NotInvolvedInAnyProject%></div>
<div class="headerBase showReport">
    <a onclick="generateReportByUrl('<%=GetActivityReportUri()%>')" class="grayLinkButton showReportBtn">
        <%= ReportResource.ShowUserActivityReport%>
    </a>
    <span class="headerBase">
        <%=ProjectsCommonResource.RecentActivity%>
    </span>
</div>    
<table class="pm-tablebase" cellpadding="14" cellspacing="0">
    <tbody>
        <asp:Repeater ID="LastActivityRepeater" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="borderBase">
                        <%# Eval("DateString") %>
                    </td>
                    <td class="borderBase textBigDescribe">
                        <%# Eval("TimeString") %>
                    </td>
                    <td class="borderBase">
                        <%# Eval("EntityPlate") %>
                    </td>
                    <td class="borderBase" style="width: 100%">
                        <%# Eval("EntityType") %>
                        <span class="splitter"></span>
                        <%# Eval("EntityParentContainers")%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
<div style="<%= LastActivityRepeater.Items.Count==0? "display:block": "display:none"%>"><%=ProjectResource.NoActivity%></div>
