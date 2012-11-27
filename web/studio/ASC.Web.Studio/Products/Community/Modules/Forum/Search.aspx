<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/Forum/Forum.Master" AutoEventWireup="true" EnableViewState="false" CodeBehind="Search.aspx.cs" Inherits="ASC.Web.Community.Forum.Search" Title="Untitled Page" %>
<%@ Import Namespace="ASC.Web.Community.Forum" %>
<%@ Import Namespace="ASC.Web.Community.Forum.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ForumPageContent" runat="server">
            <div class="forum_columnHeader" style="<%=(_isFind?"":"display:none;")%>">
            <table width="100%" style="height:100%" border="0" cellpadding="0" cellspacing="0">
                <tr>                    
                    <td style="width: 48%; padding-left:5px;"
                        valign="middle">
                        <%=ForumResource.Topic%>
                    </td>                 
                    <td style="width: 12%;" align="center"
                        valign="middle">
                        <%=ForumResource.ViewCount%>
                    </td>
                     <td align="center"
                        valign="middle">
                        <%=ForumResource.ShotPostCount%>
                    </td>
                    <td style="width: 25%;" valign="middle">
                        <%=ForumResource.RecentUpdate%>
                    </td>
                </tr>
            </table>
            </div>
        <div class="clearFix">
            <asp:PlaceHolder ID="topicListHolder" runat="server"></asp:PlaceHolder>
        </div>
        <div class="clearFix" style="padding-top:20px;">
            <asp:PlaceHolder ID="bottomPageNavigatorHolder" runat="server"></asp:PlaceHolder>
        </div>
</asp:Content>