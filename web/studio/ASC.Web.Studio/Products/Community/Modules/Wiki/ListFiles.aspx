<%@ Assembly Name="ASC.Web.Community.Wiki" %>
<%@ Assembly Name="ASC.Web.UserControls.Wiki" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListFiles.aspx.cs" Inherits="ASC.Web.Community.Wiki.ListFiles"
    MasterPageFile="~/Products/Community/Modules/Wiki/Wiki.Master" %>

<%@ Import Namespace="ASC.Web.UserControls.Wiki.Resources" %>
<%@ Import Namespace="ASC.Web.UserControls.Wiki.Data" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<asp:Content ContentPlaceHolderID="HeadContent" runat="Server">
    <link href="<%=ASC.Web.Core.Utility.Skins.WebSkin.GetUserSkin().GetAbsoluteWebPath("/products/community/modules/wiki/app_themes/<theme_folder>/css/filetype_style.css")%>"
        rel="stylesheet" type="text/css" />

    <script language="JavaScript" type="text/javascript">

        var newwindow = ''
        function popitup(url) {
            if (newwindow.location && !newwindow.closed) {
                newwindow.location.href = url;
                newwindow.focus();
            }
            else {
                newwindow = window.open(url, 'htmlname', 'width=404,height=316,resizable=1');
            }

            if (!document.body.onUnload || document.body.onUnload == null) {
                document.body.onUnload = tidy;
            }
        }

        function tidy() {
            if (newwindow.location && !newwindow.closed) {
                newwindow.close();
            }
        }
   
    </script>

</asp:Content>
<asp:Content ContentPlaceHolderID="WikiContents" runat="Server">
    <div class="wikiCredits" style="margin: 6px 0px 0px 0px; clear: both; border: none;">
        <asp:HyperLink ID="cmdUploadFile_Top" CssClass="baseLinkButton" runat="Server" NavigateUrl="javascript:ShowUploadFileBox();" />
    </div>
    <asp:Repeater ID="rptFilesList" runat="server">
        <HeaderTemplate>
            <table width="100%" class="listGrid" border="0" cellpadding="0" cellspacing="0">
                <tr class="row">
                    <th scope="col" class="wikiLeftSideTable" align="left">
                        <%=WikiResource.ListWikiFileName%>
                    </th>
                    <%-- 
                    <th align="left" scope="col" style="width:76px">
                        <%=WikiResource.wikiSize%>
                    </th>
                    --%>
                    <th align="left" scope="col" style="width: 180px">
                        <%=WikiResource.Author%>
                    </th>
                    <th align="left" scope="col" style="width: 135px">
                        <%=WikiResource.Date%>
                    </th>
                    <asp:PlaceHolder ID="phDeleteArea" runat="Server" Visible='<%#hasFilesToDelete%>'>
                        <th align="left" scope="col" style="width: 85px">
                            &nbsp;
                        </th>
                    </asp:PlaceHolder>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="row <%#Container.ItemIndex % 2 == 0 ? "alter" : string.Empty %>">
                <td class="wikiLeftSideTableFiles <%# GetFileTypeClass(Container.DataItem as File)%>">
                    <asp:HyperLink ToolTip='<%#string.Format("{0} - {1}", GetFileName(Container.DataItem as File), GetFileLengthToString((Container.DataItem as File).FileSize)) %>'
                        runat="server" ID="hlFileLink" Text='<%#GetFileName(Container.DataItem as File)%>'
                        NavigateUrl='<%#GetFileViewLink(Container.DataItem as File)%>' OnClick='<%#GetFileViewLinkPopUp(Container.DataItem as File)%>' />
                </td>
                <%-- 
                <td class="wikiFileSize">
                   <%#GetFileLengthToString((Container.DataItem as Files).FileSize)%>
                </td>
                --%>
                <td>
                    <span class="wikiVersionInfo">
                        <%#GetAuthor(Container.DataItem as File)%>
                    </span>
                </td>
                <td class="wikiRightSideTable">
                    <%#GetDate(Container.DataItem as File)%>
                    <%-- 
                        <asp:HyperLink runat="Server" ID="hlFileEdit" Text='<%#WikiResource.cmdEdit%>' NavigateUrl='<%#GetFileEditLink(Container.DataItem as Files)%>' />
                                            &nbsp;
                                            <asp:LinkButton runat="Server" ID="cmdDelete" OnClick="cmdDelete_Click" Text='<%#WikiResource.cmdDelete%>'
                                                CommandName='<%#(Container.DataItem as Files).FileName %>' OnClientClick='<%#string.Format("javascript:return confirm(\"{0}\");", WikiResource.cfmDeleteFile)%>' /> --%>
                </td>
                <asp:PlaceHolder ID="phDeleteArea" runat="Server" Visible='<%#hasFilesToDelete%>'>
                    <td align="right" class="wikiRightSideTable">
                        <asp:LinkButton runat="Server" ID="cmdDelete" OnClick="cmdDelete_Click" Text='<%#WikiResource.cmdDelete%>'
                            CommandName='<%#(Container.DataItem as File).FileName %>' OnClientClick='<%#string.Format("javascript:return confirm(\"{0}\");", WikiResource.cfmDeleteFile)%>'
                            Visible='<%#CanDeleteTheFile(Container.DataItem as File)%>' />
                    </td>
                </asp:PlaceHolder>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div class="wikiCredits" style="margin: 6px 0px 36px 0px; clear: both; border: none;">
        <asp:HyperLink ID="cmdUploadFile" CssClass="baseLinkButton" runat="Server" NavigateUrl="javascript:ShowUploadFileBox();" />
    </div>
    <div id="wiki_blockOverlay" class="wikiBlockOverlay" style="display: none;">
    </div>
    <div id="wiki_UploadFileBox" class="blockMsg blockPage" style="display: none;">
        <ascwc:Container ID="UploadFileContainer" runat="server">
            <header><%=WikiResource.wikiFileUploadSubject%></header>
            <body>
                <div id="wikiUpload_Normal">
                    <asp:FileUpload ID="fuFile" runat="Server" Width="100%" size="77" />
                    <div class="wikiMaxFileSizeBlock">
                        <%=GetMaxFileUploadString()%></div>
                    <div style="padding-top: 14px;">
                        <asp:LinkButton ID="cmdFileUpload" runat="Server" CssClass="baseLinkButton" OnClientClick="javascript:ShowUploadintProcess();"
                            OnClick="cmdFileUpload_Click" />
                        &nbsp;
                        <asp:HyperLink ID="cmdFileUploadCancel" runat="Server" CssClass="baseLinkButton"
                            NavigateUrl="javascript:HideUploadFileBox();"></asp:HyperLink>
                    </div>
                </div>
                <div class="editCommandPanel clearFix" style="display: none; padding: 14px 0 15px;"
                    id="wikiUpload_Uploading">
                    <div class="textMediumDescribe">
                        <%=WikiResource.PleaseWaitMessage%>
                    </div>
                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>
            </body>
        </ascwc:Container>
    </div>

    <script language="javascript">
        function ShowUploadintProcess() {
            document.getElementById('wikiUpload_Normal').style.display = 'none';
            document.getElementById('wikiUpload_Uploading').style.display = '';
        }
        
    </script>

</asp:Content>
