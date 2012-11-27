<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ProductActivity.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.Activity.ProductActivity" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Core" %>
<div class="clearFix">
    <div>
    <asp:Repeater ID="rpModuleActivity" runat="server">
        <HeaderTemplate>
            <table cellspacing="0" cellpadding="0" width="100%" border="0">
        </HeaderTemplate>
        <ItemTemplate>
            <tr style="vertical-align: top;">
                <td width='20'>
                    <asp:Label ID="lbAgo" CssClass="textMediumDescribe" Text='<%# Eval("DateFormated") %>'
                            runat="server" />
                    </td>
                    <td>
                        <div style="padding-left: 10px;">
                            <asp:Label ID="lbModule" CssClass="headerBaseSmall" Text='<%# Eval("ModuleName") %>'
                                runat="server" /><span class="textBigDescribe" style="font-size: 14px;"> > </span>
                            <asp:HyperLink Style="padding: 0 5px 0 0px;" ID="hlTitle" runat="server" Text='<%# HttpUtility.HtmlEncode(Eval("Title").ToString()) %>'
                                NavigateUrl='<%# Eval("Url") %>' />
                            <div style="margin-top: 5px; margin-bottom: 15px;">
                    <asp:Literal ID="ltProfile" runat="server" Text='<%# RenderProfileLink((Guid)Eval("UserID")) %>' />
                    </div>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <%if (rpModuleActivity.Items.Count == 0)
	{ %>
		<div class='empty-widget' style='padding:40px; text-align:center;'>
			<%=Resources.Resource.NoRecentActivity%>
		</div>
	<%}
	else
	{%>	
		<div style='margin-top:10px;'>
			<a href='<%=ASC.Web.Studio.WhatsNew.GetUrlForModule(ProductId, null)%>'><%= Resources.Resource.ToWhatsNewPage%></a>
		</div>
    <%} %>
    </div>
</div>
