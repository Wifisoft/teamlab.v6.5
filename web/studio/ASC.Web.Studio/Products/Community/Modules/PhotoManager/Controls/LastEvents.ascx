<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LastEvents.ascx.cs" Inherits="ASC.Web.Community.PhotoManager.Controls.LastEvents" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<ascwc:SideContainer ID="eventsContainer" runat="server">         
        <asp:Literal ID="ltrEvents" runat="server"></asp:Literal>
    </ascwc:SideContainer>