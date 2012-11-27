<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true"
    CodeBehind="LastCommented.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.LastCommented"
    Title="Untitled Page" %>

<%@ Register Src="Controls/LastEvents.ascx" TagPrefix="ctrl" TagName="LastEvents" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">
    <link href="<%=ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/css/photomanagerstyle.css")%>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="PhotoManager_FirstColumn">
                    <ascw:container id="mainContainer" runat="server">
                        <header>
                        </header>
                        <body>
                            <asp:PlaceHolder ID="_contentHolder" runat="server"></asp:PlaceHolder>
                            <div style="text-align: left; padding-top: 30px;">
                                <asp:PlaceHolder ID="pageNavigatorHolder" runat="server"></asp:PlaceHolder>
                            </div>
                        </body>
                    </ascw:container>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
    <ctrl:ActionContainer ID="actions" runat="server" />
    <ctrl:LastEvents ID="lastEvents" runat="server">
    </ctrl:LastEvents>
</asp:Content>
