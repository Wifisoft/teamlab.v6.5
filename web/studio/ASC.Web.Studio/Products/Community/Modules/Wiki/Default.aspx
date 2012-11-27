<%@ Assembly Name="ASC.Web.Community.Wiki" %>
<%@ Assembly Name="ASC.Web.UserControls.Wiki" %>
<%@ Assembly Name="ASC.Web.Core" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASC.Web.Community.Wiki._Default"
    MasterPageFile="~/Products/Community/Modules/Wiki/Wiki.Master" %>

<%@ Import Namespace="ASC.Web.UserControls.Wiki.Resources" %>
<%@ Import Namespace="ASC.Web.Community.Wiki" %>
<%@ Import Namespace="ASC.Web.UserControls.Wiki.Data" %>
<%@ Register Src="WikiUC/ViewPage.ascx" TagName="ucViewPage" TagPrefix="wiki" %>
<%@ Register Src="WikiUC/EditPage.ascx" TagName="ucEditPage" TagPrefix="wiki" %>
<%@ Register Src="WikiUC/ViewFile.ascx" TagName="ucViewFile" TagPrefix="wiki" %>
<%@ Register Src="WikiUC/EditFile.ascx" TagName="ucEditFile" TagPrefix="wiki" %>
<%@ Register TagPrefix="uc" Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" %>
<asp:Content ContentPlaceHolderID="HeadContent" runat="Server">
    <link href="<%=ASC.Web.Core.Utility.Skins.WebSkin.GetUserSkin().GetAbsoluteWebPath("/products/community/modules/wiki/app_themes/<theme_folder>/css/wikicssprint.css")%>"
        rel="stylesheet" type="text/css" media="print" />

    <script type="text/javascript">
        
        function scrollPreview()
        {
            jq.scrollTo(jq('#_PrevContainer').position().top, {speed:500});
        }
        function HidePreview()
        {
            jq('#_PrevContainer').hide();
            jq.scrollTo(jq('#edit_container').position().top, {speed:500});
        }
        function WikiEditBtns()
        {
            jq('#' + panelEditBtnsID).hide();
            jq('#action_loader').show();
        }
        
        var panelEditBtnsID = '<%=pEditButtons.ClientID%>';
        
    </script>

</asp:Content>
<asp:Content ContentPlaceHolderID="WikiContents" runat="Server">
    <%-- asp:LinkButton id="cmdInternal" Text="DoIt" runat="server" OnClick="cmdInternal_Click" /--%>
    <asp:Panel ID="pView" runat="Server" CssClass="wikiBody">
        <asp:Panel ID="PrintHeader" runat="Server" CssClass="PrintHeader">
            <%=PrintPageNameEncoded%>
        </asp:Panel>
        <wiki:ucViewPage runat="Server" ID="wikiViewPage" OnPageEmpty="OnPageEmpty" OnPublishVersionInfo="On_PublishVersionInfo"
            OnWikiPageLoaded="wikiViewPage_WikiPageLoaded" />
        <wiki:ucViewFile runat="Server" ID="wikiViewFile" OnPageEmpty="OnPageEmpty" OnPublishVersionInfo="On_PublishVersionInfo"
            OnWikiPageLoaded="wikiViewPage_WikiPageLoaded" />
        <asp:Panel runat="Server" ID="pPageIsNotExists">
            <asp:Label ID="txtPageEmptyLabel" CssClass="lblNotExists" runat="Server" />
        </asp:Panel>
    </asp:Panel>
    <div id="edit_container">
        <wiki:ucEditPage runat="Server" PreviewContainer="_PrevContainer" PreviewView="_PrevValue"
            OnPreviewReadyHandler="scrollPreview" ID="wikiEditPage" OnPublishVersionInfo="On_PublishVersionInfo"
            OnSaveNewCategoriesAdded="wikiEditPage_SaveNewCategoriesAdded" OnSetNewFCKMode="wikiEditPage_SetNewFCKMode"
            OnGetUserFriendlySizeFormat="wikiEditPage_GetUserFriendlySizeFormat" OnWikiPageLoaded="wikiEditPage_WikiPageLoaded" />
        <wiki:ucEditFile runat="Server" ID="wikiEditFile" OnPublishVersionInfo="On_PublishVersionInfo" />
    </div>
    <asp:Panel ID="pEditButtons" runat="Server" CssClass="editCommandPanel">
        <asp:LinkButton ID="cmdSave" CssClass="baseLinkButton" runat="Server" OnClientClick="javascript:WikiEditBtns();"
            OnClick="cmdSave_Click" /><asp:HyperLink ID="hlPreview" CssClass="baseLinkButton"
                runat="Server" Style="margin-left: 8px;" /><asp:LinkButton ID="cmdCancel" CssClass="grayLinkButton cancelFckEditorChangesButtonMarker"
                    runat="Server" OnClick="cmdCancel_Click" OnClientClick="javascript:WikiEditBtns();"
                    Style="margin-left: 8px;" />
    </asp:Panel>
    <div class="editCommandPanel clearFix" style="display: none;" id="action_loader">
        <div class="textMediumDescribe" id="action_loaderLabel">
            <%=WikiResource.PleaseWaitMessage%>
        </div>
        <div class="textMediumDescribe" id="action_loaderErrorMessage" style="display: none;">
            &nbsp;</div>
        <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
    </div>
    <div id="_PrevContainer" style="display: none;">
        <div class="headerPanel">
            <%=WikiResource.PagePreview%>
        </div>
        <div class="subHeaderPanel">
            <div id="_PrevValue" class="wiki">
            </div>
        </div>
        <div style='margin-top: 10px;'>
            <a class="baseLinkButton" onclick="HidePreview(); return false;" href="javascript:void(0);">
                <%=WikiResource.cmdHide%></a></div>
    </div>
    <asp:Panel ID="pCredits" CssClass="wikiCredits" runat="Server" Style="margin: 6px 0px 36px 0px;
        clear: both;">
        <span class="wikiVersionInfo" style="float: left; padding-left: 5px;">
            <asp:Literal runat="Server" ID="litAuthorInfo" />
        </span><span class="actionsBar">
            <asp:HyperLink ID="hlEditPage" CssClass="wikiEditButton" runat="Server" />
            <asp:Literal ID="litVersionSeparator" runat="Server" Visible="false">&nbsp;|&nbsp;</asp:Literal>
            <asp:LinkButton runat="Server" ID="cmdDelete" OnClick="cmdDelete_Click" Text="" />
            <asp:Literal ID="litVersionSeparatorDel" runat="Server" Visible="false">&nbsp;|&nbsp;</asp:Literal>
            <asp:HyperLink ID="hlVersionPage" CssClass="wikiEditButton" runat="Server" Visible="false" />
        </span>
        <div style="clear: both;">
            &nbsp;</div>
    </asp:Panel>
    <asp:PlaceHolder ID="phCategoryResult" runat="Server">
        <asp:Repeater ID="rptCategoryPageList" runat="Server">
            <HeaderTemplate>
                <table class="catDict" border="0" cellspacing="0" cellpadding="0" width="100%">
                    <tr>
            </HeaderTemplate>
            <ItemTemplate>
                <td class="catLetter">
                    <asp:Repeater ID="rptPageList" runat="server" DataSource='<%#Container.DataItem %>'>
                        <ItemTemplate>
                            <div class="block">
                                <div class="letter">
                                    <%#(Container.DataItem as PageDictionary).HeadName%>
                                </div>
                                <asp:Repeater runat="Server" ID="rptPages" DataSource='<%#(Container.DataItem as PageDictionary).Pages%>'>
                                    <HeaderTemplate>
                                        <div class="catList">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="category">
                                            <a href="<%#GetPageViewLink(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>"
                                                title="<%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).PageName%>">
                                                <%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).PageName%></a>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </div>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </ItemTemplate>
            <FooterTemplate>
                </tr> </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>
    <div id="wikiCommentsDiv">
        <uc:commentslist id="commentList" runat="server" style="width: 100%;" Visible="false">
        </uc:commentslist>
    </div>
</asp:Content>
