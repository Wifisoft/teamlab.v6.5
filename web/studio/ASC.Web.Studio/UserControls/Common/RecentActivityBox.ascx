<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentActivityBox.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.RecentActivityBox" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Core.Users.Activity" %>

<asp:Repeater runat="server" ID="repeaterRecentActivity">
	<ItemTemplate>
		<div class='studioRecentActivityItem'>
			<%#ASC.Core.Users.StudioUserInfoExtension.RenderProfileLink(ASC.Core.CoreContext.UserManager.GetUsers((Container.DataItem as UserActivity).UserID), (Container.DataItem as UserActivity).ProductID) %>
			<div>
				<span class='textMediumDescribe'><%# (Container.DataItem as UserActivity).ActionText.ToLower()%></span>
				<br />
				<a href='<%# ASC.Web.Studio.Utility.CommonLinkUtility.GetFullAbsolutePath((Container.DataItem as UserActivity).URL)%>' title="<%# (Container.DataItem as UserActivity).Title.HtmlEncode()%>"><%# ASC.Web.Controls.HtmlUtility.GetText((Container.DataItem as UserActivity).Title.HtmlEncode(), MaxLengthTitle)%></a>
			</div>
		</div>			
	</ItemTemplate>
</asp:Repeater>