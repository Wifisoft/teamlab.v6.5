<%@ Assembly Name="ASC.Web.Community.Bookmarking" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tags.aspx.cs" Inherits="ASC.Web.Community.Bookmarking.Tags"
	MasterPageFile="~/Products/Community/Community.master" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="BookmarkingPageHeader" ContentPlaceHolderID="CommunityPageHeader" runat="server">	
</asp:Content>
<asp:Content ID="BookmarkingPageContent" ContentPlaceHolderID="CommunityPageContent" runat="server">	
</asp:Content>
<asp:Content ID="BookmarkingSidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
	<asp:PlaceHolder ID="BookmarkingSideHolder" runat="server"></asp:PlaceHolder>
	<ascwc:SideRecentActivity id="sideRecentActivity" runat="server" />
</asp:Content>
