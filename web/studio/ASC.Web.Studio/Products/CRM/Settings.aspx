<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master"
    CodeBehind="Settings.aspx.cs" Inherits="ASC.Web.CRM.Settings" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>
<%@ MasterType TypeName="ASC.Web.CRM.BasicTemplate" %>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer"
    runat="server">
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent">
    <% if (Global.DebugVersion) { %>
    <link href="<%= PathProvider.GetFileStaticRelativePath("settings.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("settings.js") %>"></script>
    <% } %> 
</asp:Content>

<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <ascwc:SideNavigator runat="server" ID="SideNavigatorPanel">
    </ascwc:SideNavigator>
</asp:Content>
