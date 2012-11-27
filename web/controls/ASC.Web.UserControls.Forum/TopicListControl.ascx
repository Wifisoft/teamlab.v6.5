<%@ Assembly Name="ASC.Web.UserControls.Forum" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicListControl.ascx.cs" Inherits="ASC.Web.UserControls.Forum.TopicListControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Resources" %>
<div class="forum_columnHeader">
<table width="100%" style="height: 100%" border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td style="width: 48%; padding-left:5px;" align="left" valign="bottom">
            <%=ForumUCResource.Topic%>
        </td>       
        <td style="width: 12%;" align="center" valign="bottom">
            <%=ForumUCResource.ViewCount%>
        </td>
        <td align="center" valign="bottom">
            <%=ForumUCResource.PostCount%>
        </td>
        <td style="width: 25%;" valign="middle">
            <%=ForumUCResource.RecentUpdate%>
        </td>
    </tr>
</table>
</div>
<div class="clearFix">
    <asp:PlaceHolder ID="topicListHolder" runat="server"></asp:PlaceHolder>
</div>
<div class="clearFix" style="padding-top:20px;">    
    <asp:PlaceHolder ID="bottomPageNavigatorHolder" runat="server"></asp:PlaceHolder>    
</div>
