<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SingleBookmarkUserControl.ascx.cs"
	Inherits="ASC.Web.UserControls.Bookmarking.SingleBookmarkUserControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<div class="clearFix" id="<%=GetSingleBookmarkID() %>">
	<div class="clearFix bookmarkingSingleBookmarkArea">
		<div class="clearFix" style="padding-left: 4px;">
			
			<%--Thumbnail--%>
			<div style="float: left; margin-right: 15px;">
				<a href="<%=URL %>" title="<%=UserBookmarkName %>" target="_blank" style="color: White; border: solid 0px White;">
					<img src="<%=GetThumbnailUrl()%>" alt="<%=URL %>" class="bookmarkingThumbnail" title="<%=UserBookmarkName %>"
						width="96" height="72"
						id='<%=System.Guid.NewGuid().ToString()%>'/>
				</a>
			</div>			
				
			<div style="float: left;" class="longWordsBreak bookmarkingInfoArea">

				<%--Bookmark header--%>
				<div class="clearFix bookmarkingInfoArea">
					<%--Raiting panel--%>
					<div style="float: right;" id="<%=GetUniqueId() %>">
						<%=GetUserBookmarkRaiting() %>
					</div>			
					<a href="<%=URL %>" title="<%=HttpUtility.HtmlEncode(UserBookmarkDescription)%>" class="linkHeaderLightBig" target="_blank"
						id="<%=GetSingleBookmarkID() %>Name"><%=UserBookmarkName %></a>			
				</div>
				
				<%--Bookmark description--%>
				<%if(HasDescription()) { %>
				<div class="clearFix bookmarkingSingleBookmarkDescriptionArea bookmarkingGreyText"
						id="<%=GetSingleBookmarkID() %>Description"><%=UserBookmarkDescription %></div>				
				<% } %>
				
				<%--Tags, use for add bookmark to favourites only--%>
				<div id="<%=GetSingleBookmarkID() %>Tags" style="display: none;"><%=HttpUtility.HtmlEncode(UserTagsString)%></div>

				<%--Creator, comments and tags--%>	
				<div class="clearFix bookmarkingSingleBookmarkDescriptionArea">
				
					<%--Details link--%>
					<%if(!IsBookmarkInfoMode) { %>
						<div style="float: left;" class="bookmarkingDetailsDivWithImage bookmarkingAreaWithRightMargin10">				
							<a href="<%=GetBookmarkInfoUrl()%>" class="linkDescribe"
								title="<%=BookmarkingUCResource.BookmarkDetailsLinkTitle %>">
								<%=BookmarkingUCResource.Details%>
							</a>
						</div>
					<% } else if(IsCurrentUserBookmark()) { %>
						<div style="float: left;" class="bookmarkingEditDivWithImage bookmarkingAreaWithRightMargin10">				
							<a href="javascript:void(0);" class="linkDescribe"
								title="<%=BookmarkingUCResource.EditFavouriteBookmarkLinkTitle %>"
								onclick="addBookmarkToFavourite('<%=URL.ReplaceSingleQuote()%>',
																'<%=GetUniqueIDFromSingleBookmark(GetSingleBookmarkID()) %>',
																'<%=GetSingleBookmarkID() %>',
																'<%=GetUniqueIDFromSingleBookmark(GetSingleBookmarkID()) %>',
																<%=IsPromo %>);">
								<%=BookmarkingUCResource.EditFavouriteBookmarkLink%>
							</a>
						</div>
					<% } %>
					
					<%--Bookmark creator--%>
					<div style="float: left;" class="bookmarkingAreaWithRightMargin10">
						<%=RenderProfileLink() %>
					</div>
					
					<%--Comments--%>			
					<%if(!IsBookmarkInfoMode && (CommentsCount != 0)){ %>
						<div style="float: left;" class="bookmarkingAreaWithRightMargin10">
							<div style="float:left" class="bookmarkingCommentsDiv">&nbsp;</div>
							<a href="<%=HttpUtility.HtmlEncode(GetBookmarkInfoUrl())%>" class="linkDescribe" 
								title="<%=BookmarkingUCResource.CommentsLinkTitle %>"><%=CommentString%></a>
						</div>
					<%} %>
				</div>
				<%--Tags--%>
				<div class="clearFix bookmarkingSingleBookmarkDescriptionArea">
					<%if (IsTagsIncluded()) {%>
					<%--User tags if exists--%>			
					<span class="textMediumDescribe" style="margin-top: 2px;">
						<asp:Repeater runat="server" ID="TagsRepeater">
							<HeaderTemplate>
								<div style="float: left;" class="bookmarkingTag16">&nbsp;</div>
							</HeaderTemplate>
							<ItemTemplate>
								<a href="<%#GetSearchByTagUrl(DataBinder.Eval(Container.DataItem, "Name"))%>"
                                    class="linkDescribe" style="margin-left: 3px;">
									<%#HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "Name").ToString())%></a></ItemTemplate>
							<%--Separator--%>
							<SeparatorTemplate>,&nbsp;</SeparatorTemplate>
						</asp:Repeater>
					</span>	
					<%}%>
				</div>
			</div>
		</div>
		<%--Panel to attach add bookmark to favourites--%>
		<div class="clearFix" id="<%=GetUniqueId()%>ToAppend"></div>
	</div>
</div>