<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OnlineUsers.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.OnlineUsers" %>
<%@ Import Namespace="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<div id="studio_onlineUsersBlock" style="margin: 5px 0px; padding: 5px; overflow:hidden;" class="borderBase">
    <asp:Repeater ID="rpOnlineUsers" runat="server">
        <HeaderTemplate>
            <table cellpadding="3" cellspacing="3"">
                <tr>
                    <td style="white-space: nowrap; vertical-align: top; font-family: Arial; font-size: 18px;
                        color: #1a6309;">
                        <asp:Literal ID="ltUserOnlineCount" runat="server" Text='<%# string.Format("{0} ({1}):", Resources.Resource.Online, UserOnlineManager.Instance.OnlineUsers.Count) %>' />
                    </td>
                    <td style="padding-top: 8px; vertical-align: top;">
                    <div class="clearFix">
        </HeaderTemplate>
        <ItemTemplate>
            <div style="padding: 0px 10px 7px 0px; float:left;">
                <asp:Literal ID="ltProfile" runat="server" Text='<%# RenderProfileLink(((UserOnlineManager.UserVisitInfo)Container.DataItem).UserInfo) %>' />
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div></td> </tr> </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
