<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master"
    Inherits="ASC.Web.CRM.Contacts" %>

<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>
<%@ MasterType TypeName="ASC.Web.CRM.BasicTemplate" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="BTHeaderContent">
    <% if (Global.DebugVersion) { %>
    <link href="<%= PathProvider.GetFileStaticRelativePath("contacts.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("contacts.js")%>"></script>
    <% } %>
</asp:Content>

<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer" runat="server">
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
    <% if (Global.DebugVersion) { %>  
    <link href="<%= PathProvider.GetFileStaticRelativePath("socialmedia.css") %>" rel="stylesheet" type="text/css" />
    <% } %>
</asp:Content>

<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
    <asp:HiddenField ID="_ctrlContactID" runat="server" />
    <div id="files_hintTypesPanel" class="hintDescriptionPanel">
        <div class="popup-corner"></div>
        <%=CRMContactResource.TooltipTypes%>
        <a href="http://www.teamlab.com/help/tipstricks/contact-types.aspx" target="_blank"><%=CRMCommonResource.ButtonLearnMore%></a>
    </div>
    <div id="files_hintCsvPanel" class="hintDescriptionPanel">
        <div class="popup-corner"></div>
        <%=CRMContactResource.TooltipCsv%>
        <a href="http://www.teamlab.com/help/guides/create-CSV-file.aspx" target="_blank"><%=CRMCommonResource.ButtonLearnMore%></a>
    </div>
</asp:Content>

<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <ascwc:SideNavigator runat="server" ID="SideNavigatorPanel">
    </ascwc:SideNavigator>
</asp:Content>
