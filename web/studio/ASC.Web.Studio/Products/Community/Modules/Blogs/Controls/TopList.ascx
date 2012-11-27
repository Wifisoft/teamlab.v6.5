<%@ Assembly Name="ASC.Web.Community.Blogs" %>
<%@ Import Namespace="ASC.Web.Community.Blogs" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopList.ascx.cs" Inherits="ASC.Web.Community.Blogs.Controls.TopList" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<ascwc:SideContainer ID="sideContainer" runat="server">  
    <asp:Literal ID="ltrTopList" runat="server"></asp:Literal>
</ascwc:SideContainer>