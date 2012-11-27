<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Community" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Community/Modules/News/news.Master" EnableViewState="false"
    CodeBehind="default.aspx.cs" Inherits="ASC.Web.Community.News.Default" %>

<%@ Register Src="Controls/FeedItem.ascx" TagName="FeedItem" TagPrefix="uc" %>
<%@ Register Src="Controls/FeedView.ascx" TagName="FeedView" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.Community.News.Resources" %>
<%@ Import Namespace="ASC.Web.Community.News.Code" %>
<%@ Import Namespace="ASC.Web.Community.News.Code.DAO" %>
<asp:Content ID="PageContent" ContentPlaceHolderID="NewsContents" runat="server">
    <asp:Panel ID="MessageShow" runat="server" Visible="false">        
    </asp:Panel>
    <asp:Panel ID="ContentView" runat="server" CssClass="ContentView">
        <asp:Repeater ID="FeedRepeater" runat="server">
            <HeaderTemplate>
                <table width="100%" class="newsGrid" border="0" cellpadding="0" cellspacing="0">
                    <tr class="row">
                        <th scope="col" style="padding-left: 10px;">
                            <%=NewsResource.NameColumn%>
                        </th>
                        <th style="width: 100px;" scope="col">
                            <%=NewsResource.DateColumn%>
                        </th>
                        <th style="width: 150px;" scope="col">
                            <%=NewsResource.PostedBy%>
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="row">
                    <uc:FeedItem ID="FeedControl" runat="server" Feed='<%#(Container.DataItem as Feed)%>'
                        FeedType='<%#Request["type"]%>' FeedLink='<%#string.Format("~/products/community/modules/news/?docid={0}{1}",(Container.DataItem as Feed).Id, Info.UserIdAttribute)%>'
                        IsEditVisible='<%#ASC.Web.Community.Product.CommunitySecurity.CheckPermissions(ASC.Web.Community.News.NewsConst.Action_Add)%>' EditUrlWithParam='<%#FeedItemUrlWithParam%>'>
                    </uc:FeedItem>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <div class="navigationLinkBox news">
            <uc:PageNavigator ID="pgNavigator" runat="server" EntryCount="1" />
        </div>
    </asp:Panel>
    <asp:Panel ID="FeedView" runat="server" Visible="false">
        <div id="viewItem">
            <uc:FeedView id="FeedViewCtrl" runat="server">
            </uc:FeedView>
            <uc:CommentsList ID="commentList" Simple="true" runat="server" Style="width: 100%;">
            </uc:CommentsList>
            <asp:HiddenField runat="server" ID="hdnField" />
        </div>
    </asp:Panel>
</asp:Content>
