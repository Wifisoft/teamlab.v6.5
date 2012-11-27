<%@ Assembly Name="ASC.Web.UserControls.Forum" %>
<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/Forum/Forum.Master" EnableViewState="false" AutoEventWireup="true" CodeBehind="Topics.aspx.cs" Inherits="ASC.Web.Community.Forum.Topics" Title="Untitled Page" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ForumPageContent" runat="server">
<asp:PlaceHolder ID="topicsHolder" runat="server"></asp:PlaceHolder>      
</asp:Content>
