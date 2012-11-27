<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchView.ascx.cs"
 Inherits="ASC.Web.Projects.Controls.Common.SearchView" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Controls" %>

    <div class="clearFix" style="padding: 0px 10px 15px;">
        <div style="vertical-align: middle;">
            <img align="absmiddle" src="<%= WebImageSupplier.GetAbsoluteWebPath("product_logo.png", ProductEntryPoint.ID) %>" style="margin-right: 5px;" alt="<%= ProjectResource.Project %>" title="<%= ProjectResource.Project %>">
            <a class="bigLinkHeader" href="projects.aspx?prjID=<%= SearchGroup.ProjectID%>"><%=HttpUtility.HtmlEncode(SearchGroup.ProjectTitle)%></a>
        </div>
    </div> 

    <table class="pm-tablebase" cellpadding="14" cellspacing="0">
        <tbody>
            <asp:Repeater ID="SearchResultRepeater" runat="server">
                <ItemTemplate>
                    <tr class="<%# Container.ItemIndex%2==0 ? "tintMedium" : "tintLight" %>">
                    
                        <td style="width:100%;" class="borderBase">
                            <div style="width:650px; word-wrap: break-word;">
                                <a href="<%# GetItemPath((SearchItem)Container.DataItem)%>" class="linkHeaderLightMedium">
                                    <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((SearchItem)Container.DataItem).Title), ProductEntryPoint.ID, false)%>
                                </a>
                            </div>
                            <div class="textBigDescribe">
                                <%# HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(HtmlUtility.GetText(((SearchItem)Container.DataItem).Description, 100)), ProductEntryPoint.ID, false)%>
                            </div>
                        </td>
                        
                        <td class="borderBase">
                            <%# ((SearchItem)Container.DataItem).CreateOn.ToString(DateTimeExtension.DateFormatPattern)%>   
                        </td>
                        
                        <td class="textBigDescribe borderBase">
                            <%# ((SearchItem)Container.DataItem).CreateOn.ToString("HH:mm")%>
                        </td>
                        
                        <td class="borderBase">
                            <%# Global.RenderEntityPlate(((SearchItem)Container.DataItem).EntityType, true)%>
                        </td>

                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    
    <div style="padding: 10px 0px 5px; text-align:right;">
    &nbsp;
    <% if (IsListView && SearchGroup.Items.Count>5) %>
        <% { %>
            <%= ProjectsCommonResource.TotalFound %>: <%= SearchGroup.Items.Count%>&nbsp;&nbsp;
            <a href="search.aspx?prjID=<%= SearchGroup.ProjectID%>&search=<%= SearchText%>">
                <%= ProjectsCommonResource.ShowAllResults %>
            </a>
         <% } %>   
    </div>
        