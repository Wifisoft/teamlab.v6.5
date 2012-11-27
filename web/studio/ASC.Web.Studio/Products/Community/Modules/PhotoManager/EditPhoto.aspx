<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.PhotoManager.Resources" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true"
    CodeBehind="EditPhoto.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.EditPhoto"
    Title="Untitled Page" %>

<%@ Register Src="Controls/LastEvents.ascx" TagPrefix="ctrl" TagName="LastEvents" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">
    <link href="<%=ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/css/photomanagerstyle.css")%>"
        rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function BlockSaveAreaBtn()
        {
            jq('#saveAreaBtn').hide();
            jq('#saveProcess').show();        
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="PhotoManager_FirstColumn">
                    <ascw:container id="mainContainer" runat="server">
                        <header>
                        </header>
                        <body>
                            <div>
                                <asp:PlaceHolder ID="imageListHolder" runat="server"></asp:PlaceHolder>
                                <div id="saveAreaBtn" style="margin-top: 20px;">
                                    <asp:LinkButton ID="btnSave" OnClientClick="javascript:BlockSaveAreaBtn();" CssClass="baseLinkButton" runat="server"
                                        Width="80" OnClick="btnSave_Click"></asp:LinkButton>
                                </div>
                                <div style="margin-top: 20px; display: none;" id="saveProcess" class="clearFix">                                    
                                    <div class="textMediumDescribe">
                                        <%=PhotoManagerResource.PleaseWaitShortMessage%>
									</div>
									<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
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
    <ascwc:SideRecentActivity id="sideRecentActivity" runat="server" />
    <ctrl:LastEvents ID="lastEvents" runat="server"></ctrl:LastEvents>
</asp:Content>
