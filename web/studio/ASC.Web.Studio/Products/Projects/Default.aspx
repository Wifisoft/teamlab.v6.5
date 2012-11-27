<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    MasterPageFile="Masters/BasicTemplate.Master" Inherits="ASC.Web.Projects.Dashboard" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <script src="<%= PathProvider.GetFileStaticRelativePath("reports.js") %>" type="text/javascript"></script>
    <link href="<%= PathProvider.GetFileStaticRelativePath("dashboard.css") %>" rel="stylesheet" type="text/css"/>
</asp:Content>

<asp:Content runat="server" ID="PageContent" ContentPlaceHolderID="BTPageContent">
    <asp:PlaceHolder ID="contentPlaceHolder" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer" runat="server">
    <asp:PlaceHolder ID="navigationPanelPlaceHolder" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="widgetPanelPlaceHolder" runat="server"></asp:PlaceHolder>
</asp:Content>
