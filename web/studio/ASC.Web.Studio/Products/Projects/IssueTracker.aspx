<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
 CodeBehind="IssueTracker.aspx.cs" Inherits="ASC.Web.Projects.IssueTracker" %>
<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ MasterType  TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>   
 
<asp:Content ID="Content1" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("issueTracker.js") %>"></script>
    
    <link href="<%= PathProvider.GetFileStaticRelativePath("common.css") %>" type="text/css" rel="stylesheet" />
    <link href="<%= PathProvider.GetFileStaticRelativePath("issueTracker.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
  <asp:PlaceHolder runat="server" ID="_content"></asp:PlaceHolder>  
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="BTSidePanel" runat="server" >
 <ascwc:SideActions runat="server" ID="SideActionsPanel">

 </ascwc:SideActions>
 
</asp:Content>