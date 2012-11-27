<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagsUserControl.ascx.cs" Inherits="ASC.Web.UserControls.Bookmarking.TagsUserControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>

<%--Sorting--%>
<div class="clearFix bookmarkingAreaWithBottomBorder" id="TagsSortingPanel" runat="server" style="margin-bottom:15px;">
	<div style="float: left;" runat="server" id="TagsSortPanel">
	</div>
</div>

<div id="TagsContainer" runat="server"></div>

<div class="clearFix" runat="server" id="BookmarkingTagsCloudPanel"></div>

<div id="BookmarkingPaginationContainer" runat="server" class="bookmarkingPagination">
</div>