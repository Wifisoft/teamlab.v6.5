<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentUserActivity.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.Activity.RecentUserActivity" %>
<%@ Register Src="~/UserControls/Users/Activity/UserActivity.ascx" TagPrefix="uc" TagName="UserActivity" %>
<a name="activity"></a>
<div class="clearFix">
    <div class="headerBase" style="padding: 0px 0px 5px 15px;">
        <asp:Literal ID="ltRecentActivity" Text="<%= Resources.Resource.RecentActivity %>" runat="server" />
    </div>
    <uc:UserActivity ID="uaRecentActivity" runat="Server" />
</div>