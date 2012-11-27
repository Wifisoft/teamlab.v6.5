<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/Forum/Forum.Master" EnableViewState="false"
    AutoEventWireup="true" CodeBehind="Posts.aspx.cs" Inherits="ASC.Web.Community.Forum.Posts"
    Title="Untitled Page" %>

<%@ Import Namespace="ASC.Web.Community.Forum.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ForumPageContent" runat="server">
    <asp:PlaceHolder ID="postListHolder" runat="server"></asp:PlaceHolder>
</asp:Content>
