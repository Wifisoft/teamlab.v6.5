<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
    CodeBehind="Search.aspx.cs" Inherits="ASC.Web.Projects.Search" %>

<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    
    <link href="<%= PathProvider.GetFileStaticRelativePath("reports.css") %>" type="text/css" rel="stylesheet" />
    
</asp:Content> 

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    
   <% if (NumberOfStaffFound > 0) %>
   <% { %>
   <div class="clearFix" style="padding: 0px 10px 15px;">
        <div style="vertical-align: middle;">
            <img align="absmiddle" src="<%= WebImageSupplier.GetAbsoluteWebPath("home.png") %>" style="margin-right: 5px;" alt="<%= ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ProjectsCommonResource>("Employees") %>" title="<%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ProjectsCommonResource>("Employees"))%>">
            <a class="bigLinkHeader" href="<%= Page.ResolveUrl(string.Format("~/employee.aspx?pid={0}", ProductEntryPoint.ID)) %>">
                <%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ProjectsCommonResource>("Employees"))%>
            </a>
        </div>
    </div> 

    <table class="pm-tablebase" cellpadding="14" cellspacing="0">
        <tbody>
            <asp:Repeater ID="EmployeesSearchRepeater" runat="server">
                <ItemTemplate>
                    <tr class="<%# Container.ItemIndex%2==0 ? "tintMedium" : "tintLight" %>">
                    
                        <td style="width:100%;" class="borderBase">
                            <div>
                                <a href="<%# StudioUserInfoExtension.GetUserProfilePageURL(((UserInfo)Container.DataItem), ProductEntryPoint.ID) %>" class="linkHeaderLightMedium">
                                    <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), ((UserInfo)Container.DataItem).DisplayUserName(), ProductEntryPoint.ID, false)%>
                                </a>
                            </div>
                            <div class="textBigDescribe">
                                <%= HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("Department")) %>: &nbsp;
                                <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((UserInfo)Container.DataItem).Department), ProductEntryPoint.ID, false)%>,&nbsp;
                                <%= ProjectsCommonResource.Position %>: &nbsp;
                                <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((UserInfo)Container.DataItem).Title), ProductEntryPoint.ID, false)%>
                            </div>
                        </td>

                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    
    <div style="padding: 10px 0px 5px; text-align:right;">
    &nbsp;
    <% if (NumberOfStaffFound > 5) %>
        <% { %>
            <%= ProjectsCommonResource.TotalFound %>: <%= NumberOfStaffFound %> &nbsp;&nbsp;
            <a href="<%= Page.ResolveUrl(string.Format("~/employee.aspx?pid={0}&search={1}", ProductEntryPoint.ID, SearchText)) %>">
                <%= ProjectsCommonResource.ShowAllResults %>
            </a>
         <% } %>   
    </div>
    <% } %> 
    
    <asp:Repeater runat="server" ID="rptContent">
        <ItemTemplate>
            <div>
                <%# GetSearchView((SearchGroup)Container.DataItem)%>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    
    <div>
        <asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
    </div>
    
</asp:Content>