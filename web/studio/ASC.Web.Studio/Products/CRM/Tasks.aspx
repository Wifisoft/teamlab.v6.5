<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master" CodeBehind="Tasks.aspx.cs" Inherits="ASC.Web.CRM.Tasks" %>
<%@ Import Namespace="ASC.Web.CRM"%>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>
<%@ MasterType  TypeName="ASC.Web.CRM.BasicTemplate" %>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer" runat="server">
    <% if (Global.DebugVersion) { %>
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("tasks.js") %>"></script>
    <link href="<%= PathProvider.GetFileStaticRelativePath("tasks.css") %>" type="text/css" rel="stylesheet"/>
    <% } %>
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <ascwc:SideActions runat="server" ID="SideActionsPanel">
    </ascwc:SideActions>
</asp:Content>

