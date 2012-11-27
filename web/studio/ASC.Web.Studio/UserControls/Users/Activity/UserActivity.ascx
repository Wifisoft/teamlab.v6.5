<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserActivity.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.Activity.UserActivity" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Core" %>
<div class="clearFix" style="padding-bottom: 20px;">
    <asp:Label ID="lblNoActivities" CssClass="actionText" Visible="false" runat="Server" />
    <asp:Repeater ID="rpUserActivity" runat="server">
        <ItemTemplate>
            <div class="itemUserActivity <%#(Container.ItemIndex % 2 == 0) ? "even" : "odd"%>">
                <img alt="<%# Eval("ModuleName") %>" title="<%# Eval("ModuleName") %>" src="<%# Eval("ModuleIconUrl") %>" />
                <div style="padding: 15px 0pt 8px 10px; display: table;">
                    <asp:HyperLink ID="hlTitle" runat="server" Text='<%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))).Length > 40 ? HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))).Substring(0, 40) + "..." : HttpUtility.HtmlEncode(Convert.ToString(Eval("Title")))%>'
                        NavigateUrl='<%# Eval("Url") %>' ToolTip='<%# Eval("Title") %>'/>
                    <span class="actionText">
                        <%# string.Format("&nbsp;-&nbsp;{0}", Eval("ActionText")) %></span> <span class="textMediumDescribe" style="white-space:nowrap">
                            <%# Eval("AgoSentence")%></span>
                    
                </div>
                <span>&nbsp;</span>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
