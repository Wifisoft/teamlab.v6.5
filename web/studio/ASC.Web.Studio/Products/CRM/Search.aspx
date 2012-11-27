<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.CRM" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" MasterPageFile="~/Products/CRM/Masters/BasicTemplate.Master"
    Inherits="ASC.Web.CRM.Search" %>

<%@ MasterType TypeName="ASC.Web.CRM.BasicTemplate" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <% if (NumberOfStaffFound > 0) %>
    <% { %>

    <asp:Repeater ID="EmployeesSearchRepeater" runat="server">
        <HeaderTemplate>
            <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="<%# Container.ItemIndex%2==0 ? "tintMedium" : "tintLight" %>">
                <td style="width: 100%;" class="borderBase">
                    <div>
                        <a href="<%# StudioUserInfoExtension.GetUserProfilePageURL(((UserInfo)Container.DataItem), ProductEntryPoint.ID) %>"
                            class="linkHeaderLightMedium">
                            <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HtmlUtility.GetText(((UserInfo)Container.DataItem).DisplayUserName(), 80))%>
                        </a>
                    </div>
                    <div class="textBigDescribe">
                        <%--  <%= ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("Department") %>: &nbsp;
                              --%>
                        <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((UserInfo)Container.DataItem).Department), ProductEntryPoint.ID, false)%>
                        ,&nbsp;
                        <%-- <%= ProjectsCommonResource.Position %>: &nbsp;--%>
                        <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((UserInfo)Container.DataItem).Title), ProductEntryPoint.ID, false)%>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
        </FooterTemplate>
    </asp:Repeater>
  
    <% } %>
</asp:Content>
