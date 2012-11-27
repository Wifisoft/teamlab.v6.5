<%@ Assembly Name="ASC.Web.UserControls.Forum" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PostListControl.ascx.cs" Inherits="ASC.Web.UserControls.Forum.PostListControl" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

    <asp:PlaceHolder runat="server" ID="pollHolder"></asp:PlaceHolder>    
    <div class="clearFix">
        <asp:PlaceHolder ID="postListHolder" runat="server"></asp:PlaceHolder>
    </div>
    <div class="clearFix" style="margin-top: 10px;">
        <asp:PlaceHolder ID="bottomPageNavigatorHolder" runat="server"></asp:PlaceHolder>
    </div>

