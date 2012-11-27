<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reports.aspx.cs"
 MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master" Inherits="ASC.Web.CRM.Reports" %>
<%@ MasterType  TypeName="ASC.Web.CRM.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer" runat="server">
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <% if (Global.DebugVersion) { %> 
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("reports.js") %>"></script>
    <link type="text/css" href="<%= PathProvider.GetFileStaticRelativePath("reports.css") %>" rel="stylesheet"/>
    <% } %>
    <script>
    	jq(document).ready(function() {
    		ASC.CRM.Reports.bred();
    	});
    </script>
</asp:Content>

<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
    <p><noscript><strong style="color: red;">Для отображения данных необходимо включить JavaScript!</strong></noscript></p>
    <!-- тут будет выводится график -->
    <div id="placeholder" style="width:600px;height:300px;"></div>    
</asp:Content>

<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <ascwc:SideNavigator runat="server" ID="SideNavigatorPanel">
    </ascwc:SideNavigator>
</asp:Content>
