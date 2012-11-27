<%@ Assembly  Name="ASC.Web.UserControls.Forum" %>
<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/Forum/Forum.Master" AutoEventWireup="true"
     EnableViewState="false" CodeBehind="Default.aspx.cs" Inherits="ASC.Web.Community.Forum.Default" Title="Untitled Page" %>
<%@ Import Namespace="ASC.Web.Community.Forum.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ForumPageContent" runat="server">
    <asp:PlaceHolder ID="_headerHolder" runat="server">
    <div class="forum_columnHeader">
            <table width="100%" style="height:100%" border="0" cellpadding="0" cellspacing="0">
                <tr>                    
                    <td style="width: 48%; padding-left:5px;"
                        valign="middle">
                        <%=ForumResource.Thread%>
                    </td>
                    <td style="width: 12%;" align="center"
                        valign="middle">
                        <%=ForumResource.TopicCount%>
                    </td>
                    <td align="center"
                        valign="middle">
                        <%=ForumResource.PostCount%>
                    </td>
                     <td style="width: 25%;"
                        valign="middle">
                        <%=ForumResource.RecentUpdate%>
                    </td>
                </tr>
            </table>
            </div>
            </asp:PlaceHolder>
          <asp:PlaceHolder ID="forumListHolder" runat="server"></asp:PlaceHolder>
</asp:Content>
