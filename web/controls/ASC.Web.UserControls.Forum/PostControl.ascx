<%@ Assembly Name="ASC.Web.UserControls.Forum"%>
<%@ Assembly Name="ASC.Forum"%>
<%@ Assembly Name="ASC.Web.Controls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PostControl.ascx.cs" Inherits="ASC.Web.UserControls.Forum.PostControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Common" %>
<div id="forum_post_<%=Post.ID%>" class="<%=_postCSSClass%> borderBase clearFix" style="padding:10px 0px; margin-top:-1px; border-left:none; border-right:none;">    
    <%="<a name=\"" + Post.ID + "\"></a>"%>
    <%--user info--%>
    <table cellpadding="0" cellspacing="0" style="width:100%;">
    <tr valign="top">
    <td align="center" style="width:180px; padding:0px 5px;">    
       <div class="forum_postBoxUserSection" style="overflow: hidden; width:150px;"> 
       <% Guid elID = Guid.NewGuid(); %>
        <%="<a class=\"linkHeader\" id=\""+elID.ToString()+"\" href=\"" + CommonLinkUtility.GetUserProfile(Post.PosterID, _settings.ProductID) + "\"><span id=\"pAuthor_" + Post.ID + "\">" + ASC.Web.Core.Users.DisplayUserSettings.GetFullUserName(Post.Poster) + "</span></a>"%>
        <%= ASC.Core.Users.StudioUserInfoExtension.RenderPopupInfoScript(Post.Poster, _settings.ProductID, elID.ToString())%>
       
         <div style="margin:5px 0px;" class="textMediumDescribe">
       <%=HttpUtility.HtmlEncode(Post.Poster.Title)%>             
       </div>
       
       <a href="<%=CommonLinkUtility.GetUserProfile(Post.PosterID, _settings.ProductID) %>">
        <%=_settings.ForumManager.GetHTMLImgUserAvatar(Post.PosterID)%>
        </a>
       </div>
     </td>  
    <%--message--%>    
    <td>    
         <div style="margin-bottom:5px;">
             <div style="float:right; margin-right:5px; max-width:100px;"><%ReferenceToPost();%></div>
             <div style="padding:0px 5px;">
             <%=DateTimeService.DateTime2StringPostStyle(Post.CreateDate)%>
             </div>
        </div>
        <%=RenderEditedData()%>        
        <div id="forum_message_<%=Post.ID%>" class="<%=_messageCSSClass%>" style="width:550px;">
           <%=HtmlUtility.GetFull(Post.Text, _settings.ProductID)%>
        </div>
        <%=PostControl.AttachmentsList(this.Post, SettingsID)%>
        <div style="padding:5px;"><%=ControlButtons()%></div>
    </td>
    </tr>
    </table>  
</div>