<%@ Assembly Name="ASC.Forum" %>
<%@ Assembly Name="ASC.Web.UserControls.Forum" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicControl.ascx.cs" Inherits="ASC.Web.UserControls.Forum.TopicControl" %>
<%@ Import Namespace="ASC.Forum" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Common" %>
<div id="forum_topic_<%=Topic.ID%>" class="<%=TopicCSSClass%> borderBase clearFix" style="padding:5px 0px; margin-top:-1px; border-right:none; border-left:none; overflow:hidden;">    
    <table cellpadding="0" cellspacing="0" style="width:100%;">
        <tr valign="top">
        <td align="center" style="width:35px; padding:0px 10px;">
            <img alt="<%=(Topic.IsNew()?"new":"") %>" src="<%=_imageURL%>"/>
         </td>
         <td style="width:40%; padding-top:8px;">
         <div class="clearFix">            
              <%="<a class=\"linkHeaderLightSmall\" href=\"" + _settings.LinkProvider.PostList(Topic.ID) + "\">" + HttpUtility.HtmlEncode(Topic.Title) + "</a>"%>
                  <%=RenderPages()%>             
                  <%=RenderModeratorFunctions()%>
                </div>                              
               <%if (IsShowThreadName)
                     Response.Write("<div style=\"margin-top:3px;\"><a href=\"" + _settings.LinkProvider.TopicList(Topic.ThreadID) + "\">" + HttpUtility.HtmlEncode(Topic.ThreadTitle) + "</a></div>");
               %>
              <div style="padding:5px 0px;" class="clearFix">
                <%= ASC.Core.Users.StudioUserInfoExtension.RenderProfileLink(ASC.Core.CoreContext.UserManager.GetUsers(Topic.PosterID), _settings.ProductID)%>             
             </div>
             <% %>
             <asp:Panel runat="server" ID="_tagsPanel">
               <asp:Repeater ID="tagRepeater" runat="server">
                 <HeaderTemplate>
                     <div class="textMediumDescribe clearFix" style="padding-top: 2px;">
                     <img alt='' style="margin-right:3px;" src="<%=WebImageSupplier.GetAbsoluteWebPath("tags.png", _settings.ImageItemID)%>" />
                 </HeaderTemplate>
                 <ItemTemplate>
                     <a class="linkDescribe" href="<%#_settings.LinkProvider.SearchByTag((int)Eval("ID"))%>">
                         <%# HttpUtility.HtmlEncode((string)Eval("Name")) %>
                     </a>
                 </ItemTemplate>
                 <SeparatorTemplate>, </SeparatorTemplate>
                 <FooterTemplate></div></FooterTemplate>
             </asp:Repeater> 
             </asp:Panel>
         </td>        
         <td class="headerBaseMedium" style="width:12%; padding-top:8px; text-align:center;">
            <%=Topic.ViewCount%>            
         </td>
         <td class="headerBaseMedium" style="padding-top:8px; text-align:center;">
            <%=Topic.PostCount%>              
         </td>
         <td style="width:25%; padding-top:8px;">
            <%=RenderLastUpdates()%>
         </td>         
         </tr>
        </table>
</div>