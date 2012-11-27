<%@ Assembly Name="ASC.Web.Community.Blogs" %>
<%@ Import Namespace="ASC.Web.Community.Blogs" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true" EnableViewState="false"
    CodeBehind="Default.aspx.cs" Inherits="ASC.Web.Community.Blogs.Default" Title="Untitled Page" %>

<%@ Register Src="Controls/TagCloud.ascx" TagPrefix="ctrl" TagName="TagCloud" %>
<%@ Register Src="Controls/TopList.ascx" TagPrefix="ctrl" TagName="TopList" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="SettingsHeaderContent" ContentPlaceHolderID="CommunityPageHeader"
    runat="server">
    <%=RenderScripts()%>
</asp:Content>
<asp:Content ID="SettingsPageContent" ContentPlaceHolderID="CommunityPageContent"
    runat="server">
    <div style="padding-left: 0px;">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="Blogs_FirstColumn">
                    <ascw:container ID="mainContainer" runat="server">
                        <header></header>
                        <body>
                            <asp:PlaceHolder ID="placeContent" runat="server"></asp:PlaceHolder>
                            <asp:Literal ID="ltrBody" runat="server"></asp:Literal>
                        <div style="margin-top: 20px; ">
                        <asp:PlaceHolder ID="pageNavigatorHolder" runat="server"></asp:PlaceHolder>
                    </div></body>
                    </ascw:container>
                    
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
    <ctrl:ActionContainer ID="actions" runat="server" />
    <%--<ctrl:toplist ID="ctrlTopList" runat="server" />--%>
    <ascwc:SideRecentActivity id="sideRecentActivity" runat="server"></ascwc:SideRecentActivity>
    <ctrl:TagCloud ID="TagCloud" runat="server" />
</asp:Content>

