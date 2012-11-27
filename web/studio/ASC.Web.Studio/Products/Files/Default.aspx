<%@ Assembly Name="ASC.Web.Files" %>
<%@ Page Language="C#" MasterPageFile="~/Products/Files/Masters/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASC.Web.Files._Default" %>
<%@ MasterType TypeName="ASC.Web.Files.Masters.BasicTemplate" %>

<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent">
</asp:Content>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer"
    runat="server">
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <asp:PlaceHolder ID="SidePanelHolder" runat="server"></asp:PlaceHolder>
</asp:Content>