<%@ Assembly Name="ASC.Web.Community.Blogs" %>
<%@ Import Namespace="ASC.Web.Community.Blogs" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagCloud.ascx.cs" Inherits="ASC.Web.Community.Blogs.Controls.TagCloud" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<ascwc:SideContainer ID="TabCloudContainer" runat="server">         
    <asp:PlaceHolder ID="tagCloudHolder" runat="server"></asp:PlaceHolder>
</ascwc:SideContainer>