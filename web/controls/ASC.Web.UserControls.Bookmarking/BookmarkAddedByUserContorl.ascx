<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookmarkAddedByUserContorl.ascx.cs"
	Inherits="ASC.Web.UserControls.Bookmarking.BookmarkAddedByUserContorl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>
<div id="<%=DivID%>" class='boormarkedByAreaWithBorder <%= TintFlag ? "bookmarkingAreaWithWhiteBackground" : "tintMedium" %>'>
	<table style="width: 100%;" cellpadding="0" cellspacing="0">
		<tr>
			<td style="width: 40px; padding: 3px 3px 3px 10px;">
				<%=UserImage%>
			</td>
			<td class="longWordsBreak" style="width: 200px;">
				<%=UserPageLink%>
			</td>
			<td class="bookmarkingGreyText">
				<div class="longWordsBreak" style="width: 322px; padding-top: 10px; padding-bottom: 10px;">
					<%=UserBookmarkDescription%>
				</div>
			</td>
			<td class="bookmarkingGreyText" style="width: 135px; text-align: right;	padding-right: 10px;">
				<%=DateAddedAsString%>
			</td>
		</tr>
	</table>
</div>
