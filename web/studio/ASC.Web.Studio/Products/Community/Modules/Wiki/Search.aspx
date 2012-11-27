<%@ Assembly Name="ASC.Web.Community.Wiki" %>
<%@ Assembly Name="ASC.Web.UserControls.Wiki" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="ASC.Web.Community.Wiki.Search"
    MasterPageFile="~/Products/Community/Modules/Wiki/Wiki.Master" %>

<%@ Import Namespace="ASC.Web.UserControls.Wiki.Resources" %>
<%@ Import Namespace="ASC.Web.UserControls.Wiki.Data" %>
<asp:Content ContentPlaceHolderID="WikiContents" runat="Server">
    <asp:Repeater ID="rptPageList" runat="server">
        <HeaderTemplate>
            <table width="100%" class="listGrid" border="0" cellpadding="0" cellspacing="0">
                <tr class="row ">
                    <th scope="col" class="wikiLeftSideTable" align="left">
                        <%=WikiResource.ListWikiPageName%>
                    </th>
                    <th align="left" scope="col" style="width: 180px">
                        <%=WikiResource.Author%>
                    </th>
                    <th align="left" scope="col" style="width: 135px">
                        <%=WikiResource.Date%>
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="row <%#Container.ItemIndex % 2 == 0 ? "alter" : string.Empty %>">
                <td align="left" class="wikiLeftSideTable">
                    <asp:HyperLink runat="server" ID="hlPageLink" ToolTip="<%#GetPageName(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>"
                        Text="<%#GetPageName(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).HtmlEncode()%>"
                        NavigateUrl='<%#GetPageViewLink(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>' />
                </td>
                <td align="left">
                    <span class="wikiVersionInfo">
                        <%#GetAuthor(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>
                    </span>
                </td>
                <td align="left" class="wikiRightSideTable">
                    <%#GetDate(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>
                    <%-- <asp:HyperLink runat="Server" ID="hlPageEdit" Text='<%#WikiResource.cmdEdit%>' NavigateUrl='<%#GetPageEditLink(Container.DataItem as Pages)%>' />
                        &nbsp;
                        <asp:LinkButton runat="Server" ID="cmdDelete" OnClick="cmdDelete_Click" Text='<%#WikiResource.cmdDelete%>'
                            CommandName='<%#(Container.DataItem as Pages).PageName %>' OnClientClick='<%#string.Format("javascript:return confirm(\"{0}\");", WikiResource.cfmDeletePage)%>'
                            Enabled='<%#!string.IsNullOrEmpty((Container.DataItem as Pages).PageName)%>' />--%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
