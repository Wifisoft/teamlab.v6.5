<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master" CodeBehind="Templates.aspx.cs" Inherits="ASC.Web.Projects.Templates" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<%@ Register Src="Controls/Reports/TemplateList.ascx" TagPrefix="uc" TagName="TemplateList" %>
<%@ Register Src="Controls/Reports/ReportTemplateView.ascx" TagPrefix="uc" TagName="ReportTemplateView" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>




<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projects.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%= PathProvider.GetFileStaticRelativePath("reports.css") %>" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("reports.js") %>"></script>
</asp:Content>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <uc:TemplateList runat="server" ID="templateList" />
    <uc:ReportTemplateView runat="server" ID="reportTemplateView" />
 </asp:Content>
 
 <asp:Content ID="SidePanel" ContentPlaceHolderID="BTSidePanel" runat="server" >
    <ascwc:SideNavigator runat="server" ID="SideNavigatorPanel">
    </ascwc:SideNavigator>
</asp:Content>