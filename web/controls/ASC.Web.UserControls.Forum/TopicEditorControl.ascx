<%@ Assembly Name="ASC.Web.UserControls.Forum" %>
<%@ Assembly Name="ASC.Forum" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicEditorControl.ascx.cs" Inherits="ASC.Web.UserControls.Forum.TopicEditorControl" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Resources" %>
<%@ Import Namespace="ASC.Forum" %>

<input type="hidden" id="forum_topicType" value="<%=((int)EditableTopic.Type).ToString()%>" />
<div id="forum_errorMessage">
<%=_errorMessage%>
</div>

    <div class="clearFix">
        
        <%
            if (EditableTopic.Type == TopicType.Informational)
            {
                Response.Write("<div class=\"headerPanel\">" + ForumUCResource.Topic + "</div>");
                Response.Write("<div  style=\"margin-top:3px;\">" + ForumUCResource.Topic +
                    "<input class=\"textEdit\" style=\"width:100%;\" maxlength=\"300\" name=\"forum_subject\" id=\"forum_subject\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(_subject) + "\" />" +
                    "</div>");
            }
         %>
    </div>
    <ascwc:PollFormMaster runat="server" ID="_pollMaster" />    
    <%=RenderAddTags()%>  
    <div class="clearFix" style="margin:10px 0px 5px 0px;">
    <a class="baseLinkButton" style="float:left;" href="javascript:ForumManager.SaveEditTopic();"><%=ForumUCResource.SaveButton%></a>
    </div>      
