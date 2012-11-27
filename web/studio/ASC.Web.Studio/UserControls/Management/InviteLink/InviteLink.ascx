<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InviteLink.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Management.InviteLink" %>
<div class="borderBase headerBase clearFix" id="linkInviteSettingsTitle">
    <div style="float: left">
        <%=Resources.Resource.InviteLinkTitle%>
    </div>
    
    <div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpLinkInvite'});" title="<%=Resources.Resource.HelpQuestionLinkInviteSettings%>"></div> 
    <div class="popup_helper" id="AnswerForHelpLinkInvite">
     <p><%=String.Format(Resources.Resource.HelpAnswerLinkInviteSettings, "<b>", "</b>", string.Empty)%></p>
    </div>       

    
</div>
<div id="linkInviteSettings">
    <div class="desc">
        <%=Resources.Resource.InviteLinkDescription%>
    </div>
    <div id="invLinkHolder">
    <div class="fade"></div>
        <a href="<%= _generatedLink %>" title="<%= _generatedLink %>">
            <%= _generatedLink %></a>
    </div>
</div>
