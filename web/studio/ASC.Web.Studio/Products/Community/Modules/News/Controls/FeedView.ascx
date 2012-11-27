<%@ Assembly Name="ASC.Web.Community.News" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeedView.ascx.cs" Inherits="ASC.Web.Community.News.Controls.FeedView" %>
<%@ Import Namespace="ASC.Web.Community.News.Resources" %>
<div id="feedPrevDiv">
    <div id="feedPrevDiv_Body" class="feedPrevBody longWordsBreak">
        <asp:PlaceHolder runat="server" ID="pollHolder"></asp:PlaceHolder>
        <asp:Literal runat="server" ID="newsText"></asp:Literal></div>
    <div class="feedPrevCredits"><span style="float:left;">
        <%=NewsResource.PostedBy%><span id="feedPrevDiv_PostedBy" style="padding-right: 12px;
            padding-left: 8px;">
            <asp:Literal ID="profileLink" runat="server"></asp:Literal></span><span id="feedPrevDiv_PostedOn">
                <asp:Literal ID="Date" runat="server"></asp:Literal></span></span><span style="float:right;">
                    <asp:Literal runat="server" ID="EditorButtons"></asp:Literal></span><div style="clear:both;">&nbsp;</div></div>
</div>
