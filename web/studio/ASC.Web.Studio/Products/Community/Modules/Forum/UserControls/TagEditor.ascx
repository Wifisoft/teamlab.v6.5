<%@ Assembly Name="ASC.Web.Community.Forum"%>
<%@ Import Namespace="ASC.Web.Community.Forum.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagEditor.ascx.cs" Inherits="ASC.Web.Community.Forum.TagEditor" %>
<div class="forum_columnHeader" style="margin-bottom:10px;">
 <table width="100%" style="height:100%" border="0" cellpadding="0" cellspacing="0">
            <tr>                    
                <td style="width: 30%; padding-left:5px;" valign="middle">
                    <%=ForumResource.TagName%>
                </td>
                <td align="center" valign="middle">
                    <%=ForumResource.PartnershipTopic%>
                </td>
                </tr>
  </table>        
  </div>
 <%RenderTags();%>


