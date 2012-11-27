<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Community.Blogs" %>
<%@ Import Namespace="ASC.Web.Community.Blogs" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true"
    EnableViewState="false" CodeBehind="ViewBlog.aspx.cs" Inherits="ASC.Web.Community.Blogs.ViewBlog"
    Title="Untitled Page" %>

<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascws" %>
<%@ Register Src="Views/ViewBlogView.ascx" TagPrefix="ctrl" TagName="ViewBlogView" %>
<%@ Register Src="Controls/TagCloud.ascx" TagPrefix="ctrl" TagName="TagCloud" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<%@ Register Src="Controls/TopList.ascx" TagPrefix="ctrl" TagName="TopList" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">
   <%=RenderScripts()%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <div style="padding-left: 0px;">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="Blogs_FirstColumn">
                    <ascw:container id="mainContainer" runat="server">
                        <header>                            
                        </header>
                        <body>
                            <div>
                                <ascws:NotFoundControl Visible="false" ID="lblMessage" runat="server" />
                                <ctrl:ViewBlogView ID="ctrlViewBlogView" runat="server" />
                                <div style="margin-top: 30px;">
                                    <ascw:CommentsList ID="commentList" runat="server">
                                    </ascw:CommentsList>
                                </div>
                            </div>
                        </body>
                    </ascw:container>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
    <ctrl:ActionContainer ID="actions" runat="server" />
    <%--<ctrl:toplist ID="ctrlTopList" runat="server" />--%>
    <ascwc:SideRecentActivity id="sideRecentActivity" runat="server"></ascwc:SideRecentActivity>
    <ctrl:TagCloud ID="TagCloud" runat="server" />
</asp:Content>
