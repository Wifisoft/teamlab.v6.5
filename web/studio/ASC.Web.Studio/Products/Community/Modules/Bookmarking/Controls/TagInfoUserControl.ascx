<%@ Assembly Name="ASC.Web.UserControls.Bookmarking" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagInfoUserControl.ascx.cs"
	Inherits="ASC.Web.UserControls.Bookmarking.TagInfoUserControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Bookmarking.Resources" %>

<div class='clearFix <%=IsOdd ? "tintMedium" : "" %>' style="padding-top: 20px; padding-bottom: 20px;">	
	<div style="float: left; width: 200px; margin-top: -7px;">			
		<div style="float: left;">
			<img src="<%=GetTagsWebPath()%>" alt="<%=BookmarkingUCResource.Tags%>"/>
		</div>
		<div class="longWordsBreak" style="float: left; margin-left: 5px; margin-top: 7px; width: 150px;">
			<a href="<%=HttpUtility.HtmlEncode(GetSearchByTagUrl())%>" title="<%=BookmarkingUCResource.SearchByTag %>"
                class="linkHeader">
				<%=HttpUtility.HtmlEncode(Name) %>
			</a>
		</div>
	</div>
	<div style="float: left; width: 512px;" class="longWordsBreak">
		<asp:Repeater runat="server" ID="BookmarksRepeater">
			<ItemTemplate>
				<div class="clearFix" style="margin-bottom: 10px; padding-left: 10px;">
					<%--Raiting--%>
					<div style="float: right;">
						<%#GetBookmarkRaiting(Container.DataItem)%>
					</div>
					<a href="<%#GetBookmarkInfoUrl(DataBinder.Eval(Container.DataItem, "URL")) %>" class="linkHeaderLightMedium"
						title='<%#DataBinder.Eval(Container.DataItem, "Description") %>'><%#Eval("Name") %></a>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
</div>
