<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.CRM" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master"
    CodeBehind="Deals.aspx.cs"  Inherits="ASC.Web.CRM.Deals" %>
<%@ MasterType TypeName="ASC.Web.CRM.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>
<asp:Content ID="PageContentWithoutCommonContainer" ContentPlaceHolderID="BTPageContentWithoutCommonContainer"
    runat="server">
    <asp:PlaceHolder ID="_navigationPanelContent" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_widgetContainer" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="CommonContainer" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
    <div id="files_hintStagesPanel" class="hintDescriptionPanel">
        <div class="popup-corner"></div>
        <%=CRMDealResource.TooltipStages%>
        <a href="http://www.teamlab.com/help/tipstricks/opportunity-stages.aspx" target="_blank"><%=CRMCommonResource.ButtonLearnMore%></a>
    </div>
</asp:Content>
<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent">
    <% if (Global.DebugVersion) { %> 
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("deals.js")%>"></script>
    <link href="<%= PathProvider.GetFileStaticRelativePath("deals.css") %>" rel="stylesheet" type="text/css" />
    <% } %>
</asp:Content>
<asp:Content ID="SidePanelContainer" ContentPlaceHolderID="BTSidePanel" runat="server">
    <ascwc:SideActions runat="server" ID="SideActionsPanel">
    </ascwc:SideActions>
    <ascwc:SideNavigator runat="server" ID="SideNavigatorPanel">
    </ascwc:SideNavigator>
</asp:Content>
