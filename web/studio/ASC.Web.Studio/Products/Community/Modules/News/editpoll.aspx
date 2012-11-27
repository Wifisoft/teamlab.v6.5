<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/News/news.Master"
    AutoEventWireup="true" CodeBehind="editpoll.aspx.cs" Inherits="ASC.Web.Community.News.EditPoll"
    Title="Untitled Page" %>

<%@ Register Namespace="ASC.Web.Community.News.Controls" Assembly=" ASC.Web.Community.News"
    TagPrefix="ucc" %>
<%@ Register Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register TagPrefix="uc" Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.Community.News.Resources" %>
<asp:Content ID="PageContent" ContentPlaceHolderID="NewsContents" runat="server">
    <asp:Label ID="_errorMessage" runat="server"></asp:Label>
    <ascwc:PollFormMaster runat="server" ID="_pollMaster" />
    <div style="margin-top: 20px;" class="clearFix" align="left">
        <div id="panel_buttons">
            <%--<asp:LinkButton ID="lbSave" OnClientClick="javascript:NewsBlockButtons();" CssClass="baseLinkButton" OnClick="SaveFeed" CausesValidation="true"
                runat="server" Style="margin-right: 8px;"><%=NewsResource.SaveButton%></asp:LinkButton>--%>
            <a ID="lbSave" onclick="javascript:NewsBlockButtons(); CheckData();" href="javascript:void(0);" class="baseLinkButton" style="margin-right: 8px;"><%=NewsResource.SaveButton%></a>
            <asp:HyperLink ID="lbCancel" CssClass="grayLinkButton" runat="server"><%=NewsResource.CancelButton%></asp:HyperLink></div>
        <div style="display: none;" id="action_loader" class="clearFix">
            <div class="textMediumDescribe">
                <%=NewsResource.PleaseWaitMessage%>
			</div>
			<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
        </div>
    </div>
</asp:Content>
