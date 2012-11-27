<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Controls" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionsWidget.ascx.cs" Inherits="ASC.Web.Projects.Controls.Dashboard.DiscussionsWidget" %>

<%@ Import Namespace="ASC.Common.Utils" %>
<%@ Import Namespace="ASC.Web.Projects.Controls.Dashboard" %>

<div class="pm-dashboard-fadeParent">
<table class="discussionsList">
<asp:Repeater ID="DiscussionsRepeater" runat="server">
    <ItemTemplate>
    <tr>
        <td>
            <img src="<%# ((DiscussionWrapper)(Container.DataItem)).CreatedByAvatarUrl %>"/>  
            <div class="discussionDateContainer">
                <div class="discussionCreatedDate"><%# ((DiscussionWrapper)(Container.DataItem)).CreatedDateString %></div>    
                <div><%# ((DiscussionWrapper)(Container.DataItem)).CreatedTimeString %></div>    
            </div>
        </td>
        <td>
            <div class="discussionLinkContainer">
                <div class="pm-dashboard-fade"></div>
                <div>
                    <a class="discussionLink" href="<%# ((DiscussionWrapper)(Container.DataItem)).DiscussionUrl %>"
                            title="<%# ((DiscussionWrapper)(Container.DataItem)).Discussion.Title.HtmlEncode() %>">
                        <%# HtmlUtil.GetText(((DiscussionWrapper)(Container.DataItem)).Discussion.Title, 25).HtmlEncode()%>
                    </a>
                    <span class='discussionCommentCount <%# (((DiscussionWrapper)(Container.DataItem)).IsReaded)? "pm-grayText" : "pm-redText" %>'>
                        (<%# ((DiscussionWrapper)(Container.DataItem)).CommentCount %>)
                    </span>
                </div>
            </div>
                     
            <div class="discussionDescriptionContainer">
                <div class="pm-dashboard-fade"></div>
                <span class="discussionDescription">
                    <%# HtmlUtil.GetText(((DiscussionWrapper)(Container.DataItem)).Discussion.Content, 120)%>
                </span>
            </div>

            <div class="discussionProjectContainer">
                <div class="pm-dashboard-fade"></div>
                <a class="myMilestoneProjectLink" title="<%#((DiscussionWrapper)(Container.DataItem)).Discussion.Project.Title.HtmlEncode()%>"
                    href="projects.aspx?prjID=<%# ((DiscussionWrapper)(Container.DataItem)).Discussion.Project.ID %>">
                    <%#((DiscussionWrapper)(Container.DataItem)).Discussion.Project.Title.HtmlEncode()%>
                </a> 
            </div>
            <div class="discussionAuthorContainer">
                <div class="pm-dashboard-fade"></div>
                <span class="discussionAuthorLink"><%# ((DiscussionWrapper)(Container.DataItem)).CreatedByProfileLink %></span>
            </div>
        </td>
    </tr>
    </ItemTemplate>
</asp:Repeater>
</table>
</div>