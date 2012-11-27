<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClosedProjectTeamView.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Projects.ClosedProjectTeamView" %>    
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Web.Studio.Controls.Common" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>

<asp:Repeater runat="server" ID="_rptContent">
    <HeaderTemplate>
       <div class="pm-projectTeam-h-line"><!– –></div>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="pm-projectTeam-participantContainer <%#IsNewUser((Guid)Eval("UserInfo.ID")) %> clearFix <%#IsOldUser((Guid)Eval("UserInfo.ID")) %>">
            <div style="float: left;overflow: hidden;width: 40%;">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="pm-projectTeam-userPhotoContainer">
                            <img src="<%# ASC.Core.Users.UserInfoExtension.GetSmallPhotoURL((ASC.Core.Users.UserInfo)Eval("UserInfo"))%>" />
                        </td>
                        <td>
                            <a class="linkHeader" href="<%# ASC.Web.Studio.Utility.CommonLinkUtility.GetUserProfile(((ASC.Core.Users.UserInfo)Eval("UserInfo")).ID, ProductEntryPoint.ID)%>" style="font-size: 14px;">
                                <%# ASC.Web.Core.Users.DisplayUserSettings.GetFullUserName((ASC.Core.Users.UserInfo)Eval("UserInfo"))%>
                            </a>
                            <div>
                                <%# ((ASC.Core.Users.UserInfo)Eval("UserInfo")).Title.HtmlEncode()%>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="float: left;overflow: hidden;width: 50%;">
                <div style="margin: 10px 0;">
                    <span class="<%#GetCssClass((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Messages)%>"
                        <%#GetOnClickEvent((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Messages)%>>
                        <span>
                            <%=MessageResource.Messages%>
                        </span>
                    </span>    
                    <span class="splitter"></span>
                    <span class="<%#GetCssClass((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Files)%>"
                        <%#GetOnClickEvent((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Files)%>>
                        <span>
                            <%=ProjectsFileResource.Documents%>
                        </span>
                    </span>
                    <span class="splitter"></span>
                    <span class="<%#GetCssClass((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Tasks)%>"
                    <%#GetOnClickEvent((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Tasks)%>>
                        <span>
                            <%=TaskResource.AllTasks%>
                        </span>
                    </span>
                    <span class="splitter"></span>
                    <span class="<%#GetCssClass((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Milestone)%>"
                    <%#GetOnClickEvent((Guid)Eval("UserInfo.ID"), ProjectTeamSecurity.Milestone)%>>
                        <span>
                            <%=MilestoneResource.Milestones%>
                        </span>
                    </span>
                </div>
            </div>            
            <div style="overflow: hidden;text-align:right;">
				<div style="margin: 10px 0;">
                    <a class="pm-projectTeam-linkActions" id="actionPanelSwitcher_<%#Eval("UserInfo.ID")%>" onclick="javascript:showActions(this,'<%#Eval("UserInfo.ID")%>');">
                        <span class="baseLinkAction"><%=ReportResource.Actions%></span>
                        <img src="<%=WebImageSupplier.GetAbsoluteWebPath("min_expand.gif", ProductEntryPoint.ID) %>" align="absmiddle"/>
                    </a> 
                </div>
            </div>
        </div>
        <div class="pm-projectTeam-h-line <%#IsOldUser((Guid)Eval("UserInfo.ID")) %>"><!– –></div>              
    </ItemTemplate>
</asp:Repeater>

<div style="display:none;" class="pm-projectTeam-actionPanel pm-dropdown" id="actionPanel">
    <div style="left: 20px;" class="popupZub"></div>
    <div id="actionPanelContent"></div>
</div>

