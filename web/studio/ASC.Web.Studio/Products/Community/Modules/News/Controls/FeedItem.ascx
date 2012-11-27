<%@ Assembly Name="ASC.Web.Community.News" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeedItem.ascx.cs" Inherits="ASC.Web.Community.News.Controls.FeedItem" %>
<td valign="top" style="padding-top:0px;">
    <table width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="right" valign="top" class="NewsImgBlock">
                <asp:Literal runat="server" ID="Type"></asp:Literal>
            </td>
            <td valign="top" class="NewsLinkBlock">
                <asp:HyperLink runat="server" ID="NewsLink" class="longWordsBreak NewsLinkStyle">
                </asp:HyperLink>
            </td>
        </tr>
    </table>
</td>
<td valign="top" class="newsDate">
    <asp:Label runat="server" ID="Date"></asp:Label>
</td>
<td valign="top" >
    <asp:Literal runat="server" ID="profileLink"></asp:Literal>
</td>
