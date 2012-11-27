<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.CRM" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master"
 CodeBehind="Cases.aspx.cs" Inherits="ASC.Web.CRM.Cases" %>
<%@ MasterType TypeName="ASC.Web.CRM.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer"
    runat="server">
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server">
    </asp:PlaceHolder>
</asp:Content>
<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent">
    <% if (Global.DebugVersion) { %>
    <link href="<%= PathProvider.GetFileStaticRelativePath("cases.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("cases.js") %>"></script>
    <% } %>
</asp:Content>
<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <ascwc:SideActions runat="server" ID="SideActionsPanel">
    </ascwc:SideActions>
    <ascwc:SideNavigator runat="server" ID="SideNavigatorPanel">
    </ascwc:SideNavigator>
</asp:Content>