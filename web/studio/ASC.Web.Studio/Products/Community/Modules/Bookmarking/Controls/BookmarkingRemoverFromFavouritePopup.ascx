<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookmarkingRemoverFromFavouritePopup.ascx.cs" Inherits="ASC.Web.UserControls.Bookmarking.BookmarkingRemoverFromFavouritePopup" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>


<div id="removeBookmarkConfirmDialog" style="display: none;">
	 <ascwc:Container runat="server" ID="BookmarkingRemoveFromFavouriteContainer">
        <Header>
           <%=BookmarkingUCResource.RemoveFromFavouriteTitle%>
        </Header>
        <Body>
        
	        <%--Are you sure you want to remove the bookmark?--%>
            <div class="clearFix"><%=string.Format(BookmarkingUCResource.RemoveFromFavouriteConfirmMessage, "<a href='javascript: void(0);' id='BookmarkToRemoveFromFavouriteName'>&nbsp;</a>")%></div>            
			
            <div class="clearFix" style="margin-top: 16px;" id="bookmarkingRemoveFromFavouriteButtonsDiv">                    
                <ascwc:ActionButton ButtonID="BookmarkingRemoveFromFavouriteLink" ID="BookmarkingRemoveFromFavouriteLink"					
					ButtonCssClass="baseLinkButton promoAction" ButtonStyle="float: left;" runat="server"></ascwc:ActionButton>
                    
                <a class="grayLinkButton" style="float: left; margin-left:8px;" href="javascript:jq.unblockUI();">
                    <%=BookmarkingUCResource.Cancel%></a>
            </div>
        </Body>
    </ascwc:Container>
</div>