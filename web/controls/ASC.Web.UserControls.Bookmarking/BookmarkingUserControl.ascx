<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookmarkingUserControl.ascx.cs"
    Inherits="ASC.Web.UserControls.Bookmarking.BookmarkingUserControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<div class="clearFix" id="BookmarksMainPanel" runat="server">
    <%if(!IsSelectedBookmarkDisplayMode()){ %>
        <div class="clearFix bookmarkingAreaWithBottomBorder" style="width: auto;">
            <%--Sorting--%>
            <div style="float: left;" runat="server" id="BookmarkingSortPanel">
            </div>
        </div>
        <%--Create bookmark permission check--%>
        <%if(ShowCreateBookmarkLink() && PermissionCheckCreateBookmark()){ %>
        <!--	<a href="<%=GetCreateBookmarkPageUrl() %>" class="baseLinkButton promoAction" style="float: right;"
					        title="<%=BookmarkingUCResource.CreateNewBookmarkLinkTitle %>">
				        <%= BookmarkingUCResource.AddBookmarkLink %>
			        </a> --->
        <%}%>
    <%} %>
    <%--Create bookmark panel--%>
    <div class="clearFix" id="CreateBookmarkPanel" runat="server">
    </div>
    <%--Remove bookmark panel--%>
    <div id="BookmarkingRemoveFromFavouritePopupContainer" runat="server">
    </div>
</div>
<div class="longWordsBreak" id="BookmarksHolder" runat="server">
</div>
<div id="BookmarkingPaginationContainer" runat="server" class="bookmarkingPagination">
</div>
