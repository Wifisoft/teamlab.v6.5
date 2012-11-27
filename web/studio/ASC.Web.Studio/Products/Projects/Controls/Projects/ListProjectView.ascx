<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListProjectView.ascx.cs"  Inherits="ASC.Web.Projects.Controls.Projects.ListProjectView" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Controls.Projects"%>
<%@ Import Namespace="ASC.Web.Projects.Configuration"%>
<%@ Import Namespace="ASC.Projects.Core.Domain"%>
<%@ Import Namespace="ASC.Core.Users"%>
<%@ Import Namespace="ASC.Web.Projects.Classes"%>

<style>
.pm-tablebase thead tr td
{
	padding:5px 15px;
	border-left:medium none!important;
	border-right:medium none!important;
	border-top:medium none!important;
	white-space:nowrap;
}
#filterBlock
{

    text-align:right;
    border-left: medium none; 
    border-right: medium none; 
    padding: 10px 0 18px;
    margin-bottom: 30px;
    
}
</style>

    <div class="clearFix borderBase"  id="filterBlock">
      <div align="left" style="float:left;">         
            <asp:PlaceHolder ID="_tabsContainer" runat="server"></asp:PlaceHolder> 
      </div>        
      <div style="text-align: right; float:right">
                <a class="grayLinkButton" onclick="javascript:ASC.Projects.Projects.toggleClosedProjects()" style="margin-top:10px">
                   <% if (ShowClosedProjects)  %>
                   <% { %>                   
                   <%= ProjectResource.HideClosedProjects%>    
                   <% } %>
                   <% else %>  
                   <% { %>                    
                   <%= ProjectResource.ShowClosedProjects%>    
                   <% } %> 
                </a>                
       </div>   
    </div> 


        <table class="pm-tablebase sortable" cellpadding="15" cellspacing="0">
        <% if (_rptContent.Items.Count != 0) %>    
        <% { %>
        <thead>
            <tr>
                <td class="borderBase unsortable" style="padding-right:0;"></td>
                <td class="borderBase unsortable" style="padding-left:0;"></td>
                <td class="borderBase sorttable_sorted" style="padding-left:0px;">
                    <span title="<%=ReportResource.ClickToSortByThisColumn%>" class="baseLinkAction user_select_none"
                        unselectable="on"><%=ProjectResource.ProjectTitle%></span>
                    <span id="sorttable_sortfwdind"></span>
                </td>
                <td class="borderBase">
                    <span title="<%=ReportResource.ClickToSortByThisColumn%>" class="baseLinkAction user_select_none"
                        unselectable="on"><%=ProjectResource.ProjectLeader%></span>
                </td>
                <td class="borderBase">
                    <span title="<%=ReportResource.ClickToSortByThisColumn%>" class="baseLinkAction user_select_none"
                        unselectable="on"><%=MilestoneResource.Milestones%></span>
                </td>
                <td class="borderBase">
                    <span title="<%=ReportResource.ClickToSortByThisColumn%>" class="baseLinkAction user_select_none"
                        unselectable="on"><%=TaskResource.Tasks%></span>
                </td>
                <td class="borderBase">
                    <span title="<%=ReportResource.ClickToSortByThisColumn%>" class="baseLinkAction user_select_none"
                        unselectable="on"><%=ProjectResource.Team%></span>
                </td>
            </tr>
        </thead>
        <% } %> 
        <tbody >
            <asp:Repeater ID="_rptContent" runat="server">
                <ItemTemplate>
                    <tr class="<%# Container.ItemIndex%2==0 ? "tintMedium" : "" %>">
                        <td class="borderBase" style="padding-right:0;">
                            <%# GetPrivateImg((int)(Container.DataItem as WrapperListProjectView).project_id) %>
                        </td>
                        <td class="borderBase" style="padding-left:0;">
                            <%= Global.RenderEntityPlate(EntityType.Project, false) %> 
                        </td>
                        <td class="borderBase" style="padding-left:0;">
                            <div style="overflow-x:hidden;width:200px;">
                                <a class="linkMediumDark" href="projects.aspx?prjID=<%# (Container.DataItem as WrapperListProjectView).project_id%>" title="<%# (Container.DataItem as WrapperListProjectView).project_title.ToString().HtmlEncode()%>">
                                    <%# (Container.DataItem as WrapperListProjectView).project_title.ToString().HtmlEncode()%>
                                </a>
                            </div>                         
                        </td>
                        <td class="borderBase">
                            <div style="overflow-x:hidden;width:130px;">
                                <%# ASC.Core.CoreContext.UserManager.GetUsers((Guid)(Container.DataItem as WrapperListProjectView).project_leader_id).RenderProfileLink(ProductEntryPoint.ID)%>
                            </div>
                        </td>
                        <td class="borderBase" style="text-align:center;">
                            <a class="linkHeaderLightMedium" href="milestones.aspx?prjID=<%# (Container.DataItem as WrapperListProjectView).project_id%>">
                            <%# Convert.ToInt32((Container.DataItem as WrapperListProjectView).milestones_count)%>
                            </a>
                        </td>
                        <td class="borderBase" style="text-align:center;">
                            <a class="linkHeaderLightMedium" href="tasks.aspx?prjID=<%# (Container.DataItem as WrapperListProjectView).project_id%>&action=<%= (int)ASC.Web.Projects.Controls.Tasks.ListTaskView.TaskFilter.AllTasks  %>&view=all">
                            <%# Convert.ToInt32((Container.DataItem as WrapperListProjectView).tasks_count)%>
                            </a>
                        </td>
                        <td class="borderBase" style="text-align:center;">
                            <a class="linkHeaderLightMedium" href="projectteam.aspx?prjID=<%# (Container.DataItem as WrapperListProjectView).project_id%>">
                            <%# Convert.ToInt32((Container.DataItem as WrapperListProjectView).participants_count)%>
                            </a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
        </table>

    <% if (_rptContent.Items.Count == 0) %>    
    <% { %>
        <asp:PlaceHolder runat="server" ID="_phEmptyScreen" ></asp:PlaceHolder>
    <% } %> 
    
    
    <script language="javascript">
        jq(function()
        {
            if (jq.browser.msie)
            {
                jq("#sorttable_sortfwdind").html('&nbsp<font face="webdings">5</font>');
            }
            else
            {
                jq("#sorttable_sortfwdind").html('&nbsp;▴');
            }
        })
    </script>