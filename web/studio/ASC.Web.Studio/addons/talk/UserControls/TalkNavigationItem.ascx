<%@ Assembly Name="ASC.Web.Talk" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TalkNavigationItem.ascx.cs" Inherits="ASC.Web.Talk.UserControls.TalkNavigationItem" %>

<%@ Import Namespace="ASC.Data.Storage" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Talk.Addon" %>

<%@ Register Assembly="ASC.Web.Core" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>


<script type="text/javascript" src="<%=WebPath.GetPath("addons/talk/js/talk.navigationitem.js")%>"></script>

<script type="text/javascript">
  ASC.Controls.JabberClient.init('<%=GetUserName()%>', '<%=GetJabberClientPath()%>', '<%=GetOpenContactHandler()%>');
  ASC.Controls.NavigationItem.init('<%=GetUnreadMessagesHandler()%>', '<%=GetUpdateInterval()%>');
</script>

<li class="top-item-box talk">
  <span id="talkMsgLabel" class="inner-text" title="<%=GetMessageStr()%>" onclick="ASC.Controls.JabberClient.open()">
    <span id="talkMsgCount" class="inner-label">0</span>
  </span>
</li>
