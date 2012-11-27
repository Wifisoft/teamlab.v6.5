<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsUserControl.ascx.cs"
	Inherits="ASC.Web.UserControls.Bookmarking.CommentsUserControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<div class="clearFix" style="margin-top: 5px;">
	<ascw:CommentsList ID="CommentList" runat="server">
	</ascw:CommentsList>
</div>
