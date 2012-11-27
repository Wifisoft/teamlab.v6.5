<%@ Assembly Name="ASC.Web.Community" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Core.Users"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardEmptyScreen.ascx.cs" Inherits="ASC.Web.Community.Controls.DashboardEmptyScreen" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="im" %>


<div class="DashBoardCommunityBlock borderBase">
    <div class="HeaderBlock">
        <p><%=ASC.Web.Community.Resources.CommunityResource.DashboardTitle%></p>
    </div>
    <div class="ContentBlock">
    
    <%if (ASC.Core.SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser))
      { %>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/community/app_themes/default/images/icon-employees.png")%>" />
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleSecondLine%></li>
           </ul>
           <a onclick="javascript:ImportUsersManager.ShowImportControl(); return false;" href="#" class="links"><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleLink%></a>
       </div>
       
       <%}
      else
      {%>
      <style>.DashBoardCommunityBlock .ContentBlock .ModuleBlock {width: 280px;}</style>
     <%} %>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/community/app_themes/default/images/icon-blogforum.png")%>" />
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleSecondLine%></li>
           </ul>
           <span><a href="<%=VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/addblog.aspx")%>"  class="links"><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleLink1%></a>
           <%if (ASC.Core.CoreContext.UserManager.GetUsers(ASC.Core.SecurityContext.CurrentAccount.ID).IsAdmin())
           {%>
           &nbsp;<%=ASC.Web.Community.Resources.CommunityResource.ModuleBetweenLinks%>&nbsp;
           <a href="<%=VirtualPathUtility.ToAbsolute("~/products/community/modules/forum/managementcenter.aspx")%>"  class="links"><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleLink2%></a></span>
           <%} %>
       </div>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/community/app_themes/default/images/icon-wikibookmarks.png")%>" />
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleSecondLine%></li>
           </ul>
           <span><a href="<%=VirtualPathUtility.ToAbsolute("~/products/community/modules/wiki/default.aspx")%>?action=new" class="links"><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleLink1%></a>
           <br/><%=ASC.Web.Community.Resources.CommunityResource.ModuleBetweenLinks%>&nbsp;
           <a href="<%=VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/createbookmark.aspx")%>" class="links"><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleLink2%></a></span>
       </div>
       <div class="ModuleBlock">
           <img src="<%=VirtualPathUtility.ToAbsolute("~/products/community/app_themes/default/images/icon-helpcenter.png")%>" />
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleSecondLine%></li>
           </ul>
           <a href="http://www.teamlab.com/help/helpcenter.aspx" target="_blank" class="links"><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleLink%></a>
       </div>
    </div>
    
</div>
<im:ImportUsersWebControl ID="imm" runat="server" />