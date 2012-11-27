<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BrokenWidget.ascx.cs" Inherits="ASC.Web.Studio.Controls.Dashboard.BrokenWidget" %>
<div class="empty-widget" style="padding:40px; overflow:auto; text-align: center; border: dotted 1px #F00; color:#F00; background: #ffeeee">
<h1>Widget failed to load</h1>
<p>
<%=HttpUtility.HtmlEncode(Exception.Message) %>
</p>
<p>
<%=HttpUtility.HtmlEncode(Exception.StackTrace) %>
</p>
</div>