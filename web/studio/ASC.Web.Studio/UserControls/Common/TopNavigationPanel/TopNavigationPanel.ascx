<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopNavigationPanel.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.TopNavigationPanel" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Common" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="Resources" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<div class="studioTopNavigationPanel clearFix">
    <div class="studio-top-panel mainPageLayout clearFix">    
        <ul>
          <li class="studio-top-logo">
            <a class="topLogo" href="<%=CommonLinkUtility.GetDefault()%>">&nbsp;</a>
        </li>
        <asp:PlaceHolder ID="_productListHolder" runat="server">
<%--            <li style="float:left;">
                <asp:Repeater runat="server" ID="_productRepeater">                            
                    <ItemTemplate>    
                        <%#(!_currentProductID.Equals(((IWebItem)Container.DataItem).ID)) ?
                            String.Format(@"<a href=""{0}"">{1}</a>", VirtualPathUtility.ToAbsolute(((IWebItem)Container.DataItem).StartURL), HttpUtility.HtmlEncode(((IWebItem)Container.DataItem).Name)) :
                                                        String.Format(HttpUtility.HtmlEncode(((IWebItem)Container.DataItem).Name))
                        %>                        
                    </ItemTemplate>                    
                    <SeparatorTemplate>
						<span class="spacer">|</span>
                    </SeparatorTemplate>
                </asp:Repeater>
            </li>
--%>             
            <li class="product-menu <%= DisplayModuleList ? "with-subitem" : string.Empty %>">
               <%-- <span class="active-icon <%=CurrentProductClassName %>"></span>--%>
                <a class="product-cur-link baseLinkAction" title="<%= CurrentProductName %>" href=""
                    <%= DisplayModuleList ? "onclick=\"return false;\"" : string.Empty %>>
                    <%= CurrentProductName.HtmlEncode() %>
                </a>

                <% if (DisplayModuleList)
                   { %>
                <div id="studio_productListPopupPanel" class="studio-action-panel">
                    <div class="corner-top left"></div>
                    <ul class="dropdown-content">
                        <asp:Repeater runat="server" ID="_productRepeater">
                            <ItemTemplate>
                                <%# String.Format(@"<li><a href=""{0}"" class=""dropdown-item menu-products-item"">{1}</a></li>",
                                        VirtualPathUtility.ToAbsolute(((IWebItem)Container.DataItem).StartURL),
                                                                            (((IWebItem)Container.DataItem).Name).HtmlEncode())%>
                            </ItemTemplate>
                        </asp:Repeater>
                        <div class="dropdown-item-seporator"></div>
                          <asp:Repeater runat="server" ID="_addonRepeater">
                            <ItemTemplate>
                                <%# String.Format(@"<li><a href=""{0}"" class=""dropdown-item menu-products-item "">{1}</a></li>",
                                        VirtualPathUtility.ToAbsolute(((IWebItem)Container.DataItem).StartURL),
                                                            (((IWebItem)Container.DataItem).Name).HtmlEncode())%>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
                <% } %>
            </li>

        </asp:PlaceHolder>  
        <%if (!DisableTrialPeriod) {%>
         <li class="studio-top-trial-period">
            <a href="/management.aspx?type=4"><%=getNumDaysPeriod%> </a>
        </li>
         <% } %>
        <asp:PlaceHolder ID="_guestInfoHolder" runat="server">
            <li>                                
                <a href="<%=VirtualPathUtility.ToAbsolute("~/auth.aspx")%>">
                    <%=Resources.Resource.LoginButton%>
                </a>
            </li>
        </asp:PlaceHolder>
        
        <asp:PlaceHolder ID="_userInfoHolder" runat="server">
            
             <%--my staff--%>
             <%if (!ASC.Core.SecurityContext.DemoMode)
               {%>              
             <li class="staff-profile-box" onselectstart="return false;" onmousedown="return false;">
                <span class="userLink">
                    <span class="usr-prof baseLinkAction" title="<%= ASC.Core.Users.UserInfoExtension.DisplayUserName(CurrentUser) %>">                      
                        <%=ASC.Core.Users.UserInfoExtension.DisplayUserName(CurrentUser)%>
                    </span>     
                </span>
            </li>
            <%} 
               else
               { %>  
               
                <li style="float:right;">
                    <span class="userProfileBox"> <%=Resources.UserControlsCommonResource.DemoUserName%></span>
               </li>
               
            <%} %>            
            
            <%=RenderCustomNavigation()%>
        </asp:PlaceHolder>
        
        <% if (IsAdministrator)
           { %>
           <li class="top-item-box settings" >
               <a class="inner-text" href="<%= CommonLinkUtility.GetAdministration(ManagementType.General) %>" title="<%= Resource.Administration %>"></a>
           </li>
        <% } %>

            <li class="clear"></li>
        </ul>
        
        <div id="studio_myStaffPopupPanel" class="studio-action-panel">
            <div class="corner-top left"></div>
                <ul class="dropdown-content">
                    <li><a class="dropdown-item" href="<%=CommonLinkUtility.GetMyStaff(MyStaffType.General)%>"><%=Resources.Resource.Profile%></a></li>
                    <li><a class="dropdown-item" href="<%=CommonLinkUtility.GetMyStaff(MyStaffType.Activity)%>"><%=Resources.Resource.RecentActivity%></a></li>
                    <li><a class="dropdown-item" href="<%=CommonLinkUtility.GetMyStaff(MyStaffType.Subscriptions)%>"><%=Resources.Resource.Subscriptions%></a></li>
                    <li><a class="dropdown-item" href="<%=CommonLinkUtility.GetMyStaff(MyStaffType.Customization)%>"><%=Resources.Resource.Customization%></a></li>
                    
                     <%= RenderDebugInfo() %>
                     
                    <asp:Repeater runat="server" ID="myToolsItemRepeater">
                        <ItemTemplate>
                            <a class="myStaffItem" href="<%#CommonLinkUtility.GetMyStaff((string)Eval("ParameterName"))%>"><%#HttpUtility.HtmlEncode((string)Eval("TabName"))%></a>        
                        </ItemTemplate>
                    </asp:Repeater>	
                     
                     <%--Logout--%>
                    <%if (CanLogout)
                      { %>
                       <li class="sign-out-link-seporator"></li>
                       <li><a class="dropdown-item" href="<%= CommonLinkUtility.Logout %>"><%= UserControlsCommonResource.LogoutButton %></a></li>
                    <%} %>		
                </ul>
        </div>
        
         <div id="MoreProductsPopupPanel" class="studio-action-panel">
            <div class="corner-top left"></div>
            <ul class="dropdown-content">            
                <asp:Repeater runat="server" ID="MoreProductsRepeater">
                    <ItemTemplate>
						    <div>							
							    <%#String.Format("<a href='{0}' class='myStaffItem'>{1}</a>", VirtualPathUtility.ToAbsolute(((IWebItem)Container.DataItem).StartURL), HttpUtility.HtmlEncode(((IWebItem)Container.DataItem).Name))	%>
                            </div>
                    </ItemTemplate> 
                    <FooterTemplate>
					    <li class="borderFooterNavPanel"></li>
                    </FooterTemplate>
                </asp:Repeater>            
                <li><a class="dropdown-item" href="<%= CommonLinkUtility.GetDefault() %>"><%= UserControlsCommonResource.AllProductsTitle %></a></li>
             </ul>
        </div>        
 
        <asp:PlaceHolder runat="server" ID="_customNavControls"></asp:PlaceHolder>
    </div>
    <asp:PlaceHolder runat="server" ID="_contentSectionHolder">
        <div class="contentSection">
        <div class="infoBox">
            <div class="mainPageLayout clearFix">
                
                <%--header--%>
                <a <%=String.IsNullOrEmpty(_titleURL) ? "" : "style=\"cursor:pointer;\" href=\""+_titleURL+"\""%> class="titleBox clearFix">
                    <%=String.IsNullOrEmpty(_titleIconURL) ? "" : "<div style='float:left;'><img alt=\"\" src=\"" + _titleIconURL + "\"/></div>"%>
                    <%=String.IsNullOrEmpty(_title) ? "" : "<div style='float:left;' class=\"title\">" + HttpUtility.HtmlEncode(_title) + "</div>"%>                    
                </a>
                
                <%--html injection--%>
                <%=String.IsNullOrEmpty(CustomInfoHTML) ? "" : "<div style='float:left;'>" + CustomInfoHTML + "</div>"%>
            
                <%--search--%>
               
                <asp:PlaceHolder ID="_searchHolder" runat="server">
                
                    <div class="searchBox" style="float:right;">
                        <div <%=_singleSearch?"class=\"singleLeftSearchPanel\"":"class=\"leftSearchPanel\" onclick=\"Searcher.ShowSearchPlaces();\""%> id="studio_searchSwitchButton">
                        <%if (!String.IsNullOrEmpty(_searchLogoUrl))
                          { %>
                            <img alt="" style="margin-top:2px; margin-left:2px;" id="studio_activeSearchHandlerLogo" align="absmiddle" src="<%=_searchLogoUrl%>"/>
                        <%} %>
                        </div>
                        <div class="mainSearchPanel">
                             <input type="text" id="studio_search" class="textEditMainSearch" value="<%=_searchText%>" style="width:130px;" maxlength="255" />
                        </div>
                        
                        <div class="mainSearchButton" onclick="Searcher.Search();">
                            <input type="hidden" value=""/>
                        </div>
                        
                        <div id="studio_searchSwitchBox" class="switchBox" style="display:none; z-index:100; position:absolute;">
                            <%=RenderSearchHandlers()%>
                        </div>

                    </div>
                </asp:PlaceHolder>
            </div>       
            </div>
        
            <%--tabs with navigation--%>
            <asp:Repeater runat="server" ID="_navItemRepeater">
                <HeaderTemplate>    
                <div class="navigationBox">   
                    <div class=" mainPageLayout clearFix">            
                </HeaderTemplate>
                <ItemTemplate>
					<%#RenderNavigationItem(Container.DataItem as ASC.Web.Studio.Controls.Common.NavigationItem)%>
                    <%--<a class="<%#((bool)Eval("Selected"))?"selectedTab":"tab"%>" href="<%#Eval("URL")%>" <%#((bool)Eval("RightAlign"))?"style='float: right;'":""%>>                        
                        <span>
                            <%#HttpUtility.HtmlEncode((string)Eval("Name"))%></span>                            
                            <%#string.IsNullOrEmpty((string)Eval("ModuleStatusIconFileName"))?"":(string.Format(@"<img src='{0}' style='border: 0px solid White; margin-left: 2px; padding-top:-2px;'/>", (string)Eval("ModuleStatusIconFileName")))%>
                    </a>--%>
                </ItemTemplate>
                <FooterTemplate>            
                    </div></div>  
                </FooterTemplate>
            </asp:Repeater>
    
     </div>
    </asp:PlaceHolder>
</div>

