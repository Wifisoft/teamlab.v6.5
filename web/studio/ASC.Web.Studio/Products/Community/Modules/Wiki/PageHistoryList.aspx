<%@ Assembly Name="ASC.Web.Community.Wiki" %>
<%@ Assembly Name="ASC.Web.UserControls.Wiki" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageHistoryList.aspx.cs"
    Inherits="ASC.Web.Community.Wiki.PageHistoryList" MasterPageFile="~/Products/Community/Modules/Wiki/Wiki.Master" %>

<%@ Import Namespace="ASC.Web.UserControls.Wiki.Resources" %>
<%@ Import Namespace="ASC.Web.UserControls.Wiki.Data" %>
<asp:Content ContentPlaceHolderID="HeadContent" runat="Server">

    <script language="javascript" type="text/javascript">

        function versionSelected(obj) {
            var isNewClicked = obj.id.indexOf('rbNewDiff') > 0;
            var version = obj.parentNode.getAttribute('_Version');
            var spans = document.getElementsByTagName('span');
            var curVersion;

            for (var i = 0; i < spans.length; i++) {

                if (spans[i].getAttribute('_Version')) {
                    curVersion = spans[i].getAttribute('_Version') * 1;

                    var thisColumn = (!isNewClicked && spans[i].firstChild.id.indexOf('rbNewDiff') < 0) || (isNewClicked && spans[i].firstChild.id.indexOf('rbNewDiff') > 0)
                    if (thisColumn) {
                        spans[i].firstChild.checked = (curVersion == version);
                    }
                    else {
                        if ((isNewClicked && curVersion >= version) ||
                        (!isNewClicked && curVersion <= version)) {
                            spans[i].style.display = 'none';
                        }
                        else {
                            spans[i].style.display = '';
                        }
                    }
                }
            }
        }

        function VersionRevernConfirm() {
            return confirm('<%=WikiResource.wikiRevertConfirm%>');
        }
             
    </script>

</asp:Content>
<asp:Content ContentPlaceHolderID="WikiContents" runat="Server">
    <div class="wikiCredits" style="margin: 6px 0px 0px 0px; clear: both; border: none;">
        <asp:LinkButton ID="cmdDiff_Top" CssClass="baseLinkButton" runat="Server" OnClick="cmdDiff_Click" />
    </div>
    <asp:Repeater ID="rptPageHistory" runat="server">
        <HeaderTemplate>
            <table width="100%" class="listGrid" border="0" cellpadding="0" cellspacing="0">
                <tr class="row">
                    <th scope="col" align="center" class="wikiLeftSideTable wikiVersionLeft" valign="middle">
                        <%#WikiResource.wikiVersion_Number %>
                    </th>
                    <th scope="col" colspan="2" style="padding-left: 10px; width: 45px; padding-bottom: 0px;
                        height: 0px;">
                        &nbsp;
                    </th>
                    <th style="width: 135px;" scope="col">
                        <%#WikiResource.Date %>
                    </th>
                    <th scope="col">
                        <%#WikiResource.Author %>
                    </th>
                    <th style="width: 60px;" scope="col">
                        &nbsp
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="row <%#Container.ItemIndex % 2 == 0 ? "alter" : string.Empty %>">
                <td class="wikiLeftSideTable wikiVersionLeft" align="center">
                    <asp:Literal ID="litDiff" Text='<%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version > 9 ? (Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version.ToString() : "0" + (Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version.ToString()%>'
                        runat="Server" Visible="false" />
                    <asp:HyperLink runat="server" ID="HyperLink1" Text='<%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version%>'
                        NavigateUrl='<%#GetPageViewLink(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>' />
                </td>
                <td>
                    <asp:RadioButton ID="rbOldDiff" CssClass="wikiDiffRadion" Style='<%#Container.ItemIndex == 0 ? "display:none": ""%>'
                        onclick="javascript:versionSelected(this);" _Version='<%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version%>'
                        runat="Server" Checked='<%#Container.ItemIndex == 1%>' />
                </td>
                <td>
                    <asp:RadioButton ID="rbNewDiff" CssClass="wikiDiffRadion" Style='<%#Container.ItemIndex != 0 ? "display:none": ""%>'
                        onclick="javascript:versionSelected(this);" _Version='<%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version%>'
                        runat="Server" Checked='<%#Container.ItemIndex == 0%>' />
                </td>
                <td align="left">
                    <%#GetDate(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>
                </td>
                <td align="left">
                    <span class="wikiVersionInfo">
                        <%#GetAuthor(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page)%>
                    </span>
                </td>
                <td align="right" class="wikiRightSideTable">
                    <asp:LinkButton ID="cmdRevert" Text='<%#WikiResource.cmdRevert%>' runat="Server"
                        OnClick="cmdRevert_Click" CommandName='<%#(Container.DataItem as ASC.Web.UserControls.Wiki.Data.Page).Version%>'
                        Visible='<%#Container.ItemIndex != 0%>' OnClientClick="javascript:return VersionRevernConfirm();" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div class="wikiCredits" style="margin: 6px 0px 36px 0px; clear: both; border: none;">
        <asp:LinkButton ID="cmdDiff" CssClass="baseLinkButton" runat="Server" OnClick="cmdDiff_Click" />
    </div>
</asp:Content>
