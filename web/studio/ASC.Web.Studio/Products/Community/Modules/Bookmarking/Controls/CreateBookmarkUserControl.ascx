<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateBookmarkUserControl.ascx.cs" Inherits="ASC.Web.UserControls.Bookmarking.CreateBookmarkUserControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins"%>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%--Create bookmark panel--%>
<div id="AddBookmarkPanel" class="bookmarkingContentArea" style="display: none;">	
	<%if(!IsNewBookmark){ %>
	<div id="AddBookmarkPanelToMove" class="bookmarkingCreateNewBookmarkContainer bookmarkingAddToFavouritePanelWithMargin">
		<div class="clearFix headerPanel">
			<%if (IsEditMode) { %>
				<%=BookmarkingUCResource.EditFavouriteBookmarkTitle%>	
			<%} else { %>
				<%=BookmarkingUCResource.AddToFavourite%>	
			<% } %>
		</div>		
	<% } else { %>
	<div id="AddBookmarkPanelToMove">
	<% } %>
		<div class="bookmarkingContentArea" id="BookmarkURLPanel">
			<div class="headerBaseMedium bookmarkingContentArea">
				<%=BookmarkingUCResource.BookmarkUrl%>
			</div>
			<div class="clearFix">
				<%if(!IsNewBookmark){ %>
					<div style="float: left; width: 677px;" id="BookmarkUrlContainer">
				<% } else { %>
					<div style="float: left; width: 717px;" id="BookmarkUrlContainer">
				<% } %>
					<input type="text" class="bookmarkingInputUrl" onkeydown="return bookmarkInputUrlOnKeyDown(event);"
						style="background-color:#FFFFFF;" id="BookmarkUrl" name="BookmarkUrl"
						autocomplete="off"/>
				</div>
				<div id="NewBookmarkRaiting"
						style="position: absolute; float: right; min-width: 10px; max-width: 300px; margin-top: 2px; margin-left: 670px; ">
				</div>				
			</div>
						
			<div class="bookmarkingCreateNewBookmarkButtonsContainer" id="CheckBookmarkUrlButtonsPanel">
						
				<ascwc:ActionButton ButtonID="CheckBookmarkUrlLinkButton" ID="CheckBookmarkUrlLinkButton"
						OnClickJavascript="getBookmarkByUrlButtonClick(); return false;"
						ButtonCssClass="baseLinkButton" runat="server"></ascwc:ActionButton>
				
				<%if(!IsNewBookmark){ %>			
				<a href="javascript:void(0);" class="grayLinkButton"
						onclick="cancelButtonClick(); return false;">						
				<% } else { %>				
				<a href="default.aspx" class="grayLinkButton" style="margin-left: 10px;">
				<% } %>
					<%= BookmarkingUCResource.Cancel %>
				</a>
			</div>
		</div>
		<%--Bookmark name, description and tags--%>
		<div id="BookmarkDetailsPanel" style="display: none; margin-right: 5px;" class="clearFix">
			<div class="bookmarkingContentArea">
				<div class="headerBaseMedium bookmarkingContentArea">
					<%=BookmarkingUCResource.BookmarkName%>
				</div>
				<div>
					<input type="text" class="bookmarkingInputText" id="BookmarkName" name="BookmarkName" 
							onkeydown="return createBookmarkOnCtrlEnterKeyDown(event);"
							maxlength="255"/>
				</div>
			</div>
			<div class="bookmarkingContentArea">
				<div class="headerBaseMedium bookmarkingContentArea">
					<%=BookmarkingUCResource.BookmarkDescription%>
				</div>
				<div>
					<textarea rows="3" class="bookmarkingInputTextArea"
						id="BookmarkDescription" name="BookmarkDescription"
						onkeydown="return createBookmarkOnCtrlEnterKeyDown(event, true);"></textarea>
				</div>
			</div>
			<div class="bookmarkingContentArea">
				<div class="headerBaseMedium bookmarkingContentArea">
					<%=BookmarkingUCResource.Tags%>
				</div>
				<div>
					<input type="text" autocomplete="off" maxlength="255" class="bookmarkingInputText"
							id="BookmarkTagsInput" name="BookmarkTagsInput"
							onkeydown="return createBookmarkOnCtrlEnterKeyDown(event);"/>
							<div class="textMediumDescribe"><%=BookmarkingUCResource.BookmarkTagDescription%></div>
				</div>
			</div>
			
			<div class="bookmarkingCreateNewBookmarkButtonsContainer" id="bookmarkingCreateNewBookmarkButtonsDiv">				
				<%if (CreateBookmarkMode) { %>
					<ascwc:ActionButton ButtonID="SaveBookmarkButton" ID="SaveBookmarkButton"
						OnClickJavascript="createNewBookmarkButtonClick(); return false;"
						ButtonCssClass="baseLinkButton promoAction" EnableRedirectAfterAjax="true" runat="server"></ascwc:ActionButton>
						
					<ascwc:ActionButton ButtonID="SaveBookmarkButtonCopy" id="SaveBookmarkButtonCopy"
						OnClickJavascript="createNewBookmarkButtonClick(); return false;"
						ButtonCssClass="baseLinkButton promoAction" EnableRedirectAfterAjax="true" runat="server"></ascwc:ActionButton>
				<%} else { %>
					<ascwc:ActionButton ButtonID="SaveBookmarkButtonCopy" id="AddToFavouritesBookmarkButton"
						ButtonCssClass="baseLinkButton promoAction" runat="server"></ascwc:ActionButton>
				<%} %>				
				
				<%if(!IsNewBookmark){ %>			
				<a href="javascript:void(0);" class="grayLinkButton" style="margin-left: 10px;"
						onclick="hideAddBookmarkPanelWithAnimation();">
							
				<% } else { %>				
				<a href="default.aspx" class="grayLinkButton" style="margin-left: 10px;">
				<% } %>
					<%= BookmarkingUCResource.Cancel %>
				</a>				
				
			</div>
		</div>
		<div id="BookmarkingAjaxRequestImage" style="display: none;">
			<img src='<%=WebImageSupplier.GetAbsoluteWebPath("mini_loader.gif", new Guid("853B6EB9-73EE-438d-9B09-8FFEEDF36234"))%>'
				alt="Ajax request is in progress" />
		</div>
	</div>
</div>

<input type="hidden" id="EmptyBookmarkUrlErrorMessageHidden" value="<%=BookmarkingUCResource.EmptyBookmarkUrlErrorMessage %>" />


