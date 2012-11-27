<%@ Assembly Name="ASC.Web.Files" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Files/Masters/BasicTemplate.Master"
    CodeBehind="Search.aspx.cs" Inherits="ASC.Web.Files.Search" %>

<%@ MasterType TypeName="ASC.Web.Files.Masters.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Configuration" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascw" %>

<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent">
    <link href="<%=PathProvider.GetFileStaticRelativePath("common.css")%>" type="text/css" rel="stylesheet" />
    
    <script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("common.js") %>"></script>
</asp:Content>

<asp:Content ID="CommonContainer"  ContentPlaceHolderID="BTPageContent" runat="server">
    <script type="text/javascript">
        jq(function() {
            jq("#searchedDocuments div.document").each(function() {
                var ftClass;
                if (jq(this).hasClass("thumb-folder")) {
                    ftClass = ASC.Files.Utility.getFolderCssClass();
                } else {
                    var title = jq(this).find("a.title").text().trim();
                    ftClass = ASC.Files.Utility.getCssClassByFileTitle(title);
                }
                jq(this).find("div.icon").addClass(ftClass);
            });
        });
    </script>

    <asp:Repeater ID="EmployeesSearchRepeater" runat="server">
        <HeaderTemplate>
            <div class="clearFix" style="padding:10px;">
                <div class="headerBase" style="float: left;">
                    <img align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("home.png")%>"
                        style="margin-right: 10px;" alt=" <%=FilesCommonResource.Employees%>" /><%=FilesCommonResource.Employees%></div>
                <%if (NumberOfStaffFound > 5)%>
                <%{%>
                <div style="float:right; padding-top: 10px;">          
                    <%=FilesCommonResource.TotalFound%>: <%=NumberOfStaffFound%>&nbsp;&nbsp;|&nbsp;&nbsp;
                    <a href="<%=Page.ResolveUrl(string.Format("~/employee.aspx?pid={0}&search={1}", ProductEntryPoint.ID, SearchText))%>">
                        <%=FilesCommonResource.ShowAllResults%>
                    </a>
                </div>
                <%}%>
            </div>
        </HeaderTemplate>
        <ItemTemplate>
            <div class="borderBase employeesSearch <%#Container.ItemIndex%2==0 ? "tintMedium" : "tintLight"%>" >
                <div>
                    <a href="<%#StudioUserInfoExtension.GetUserProfilePageURL(((UserInfo)Container.DataItem), ProductEntryPoint.ID)%>"
                        class="linkHeaderLightMedium">
                        <%#HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), ((UserInfo)Container.DataItem).DisplayUserName(), ProductEntryPoint.ID, false)%>
                    </a>
                </div>
                <div class="textBigDescribe">
                    <%#
                        (string.IsNullOrEmpty(((UserInfo)Container.DataItem).Department) ? "" :
                            FilesCommonResource.Department + " " + HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((UserInfo)Container.DataItem).Department), ProductEntryPoint.ID, false) + ", "
                        )
                        +
                        (string.IsNullOrEmpty(((UserInfo)Container.DataItem).Title)?"":
                            FilesCommonResource.Position + " " + HtmlUtility.SearchTextHighlight(HttpUtility.HtmlEncode(SearchText), HttpUtility.HtmlEncode(((UserInfo)Container.DataItem).Title), ProductEntryPoint.ID, false) + ", "
                        )
                     %>                            
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div>&nbsp; </div>  
        </FooterTemplate>
    </asp:Repeater>
    
    <asp:Repeater ID="ContentSearchRepeater" runat="server">
        <HeaderTemplate>
            <div id="searchedDocuments">
                <div class="clearFix" style="padding: 10px;">
                    <div  class="headerBase" style="float: left;">
                        <img align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("product_logo.png", ProductEntryPoint.ID)%>"
                            style="margin-right: 10px;" alt="<%=FilesCommonResource.ProductName%>"><%=FilesCommonResource.ProductName%></div>
                    <div style="float: right; padding-top: 10px;">
                        <%=FilesCommonResource.TotalFound%>:
                        
                        <%if (NumberOfFolderFound > 0) 
                          {%>
                        &nbsp;&nbsp;<%=FilesCommonResource.TitleFolders + " " + NumberOfFolderFound%>
                        <% } %>
                        
                        <%= NumberOfFileFound > 0 && NumberOfFolderFound > 0 ? "|" : "" %>
                        
                        <%if (NumberOfFileFound > 0) 
                          {%>
                          &nbsp;&nbsp;<%=FilesCommonResource.TitleFiles + " " + NumberOfFileFound%>
                        <% } %>
                    </div>
                </div>
        </HeaderTemplate>
        <ItemTemplate>
                <div class="document borderBase <%#Container.ItemIndex%2==0 ? "tintMedium" : "tintLight"%> <%#(Container.DataItem as SearchItem).IsFolder ? "thumb-folder" :String.Empty%>">
                    <div class="icon"></div>
                    <div style="margin-left:35px; overflow:hidden;">
                        <div>
                            <a class="linkHeaderLightMedium title" alt="<%#(Container.DataItem as SearchItem).FileTitle%>"
                                title="<%#(Container.DataItem as SearchItem).FileTitle%>"
                                href="<%#(Container.DataItem as SearchItem).ItemUrl%>">
                                <%#HtmlUtility.SearchTextHighlight(SearchText, (Container.DataItem as SearchItem).FileTitle, ProductEntryPoint.ID, false)%>
                            </a>
                        </div>
                        <div>
                            <%#HtmlUtility.SearchTextHighlight(SearchText, GetShortenContent((Container.DataItem as SearchItem).Body), ProductEntryPoint.ID, false)%>
                        </div>
                        <div class="textBigDescribe">
                        <div style="float:left;">
                            <%#Eval("FolderPathPart")%>
                        </div>
                        <div style="float:right">
                            <%=FilesCommonResource.Author%>&nbsp;<%#(Container.DataItem as SearchItem).Owner%>
                            <span class="separator">|</span><%=FilesCommonResource.TitleUploaded%>:&nbsp;<%#(Container.DataItem as SearchItem).Uploaded%>
                            <%#(Container.DataItem as SearchItem).IsFolder ? String.Empty:
                                "<span class='separator'>|</span>" + FilesCommonResource.Size + " " + (Container.DataItem as SearchItem).Size%>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
   
    <%if ((EmployeesSearchRepeater.Items.Count == 0) && (ContentSearchRepeater.Items.Count == 0))%>
    <%{%>
    <ascw:EmptyScreenControl runat="server" id="emptyScreenControl" ></ascw:EmptyScreenControl> 
    <%}%>
</asp:Content>
