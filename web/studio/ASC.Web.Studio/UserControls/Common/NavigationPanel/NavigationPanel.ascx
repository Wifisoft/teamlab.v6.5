<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationPanel.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.NavigationPanel" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Common" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="im" %>

<div id="communityNavigationPanel" class="navigationPanel tintMedium borderBase<%=isMinimized() ? " minimized" : ""%>">
  <div class="additionMenu">
    <div id="btnToggleNavigator" class="button" onclick="StudioManager.ToggleNavigationPanel(this)"></div>
  </div>
  <table class="mainPanel">
    <tr>
      <td style="width:30%;">
        <div class="greetingBlock">
          <img <%=TenantInfoSettings.CompanyLogoSize.Height>50?"height='50'":"" %> class="logo" src="<%=TenantInfoSettings.GetAbsoluteCompanyLogoPath()%>" alt="" />
          <span class="label"><%=RenderGreetingTitle()%></span>
        </div>        
      </td>
      <td style="width:70%;">
        <div class="navigationBlock">
          <asp:Repeater runat="server" ID="buttonRepeater">
            <ItemTemplate>
              <a class="item" href="<%#HttpUtility.HtmlEncode((String)Eval("Link"))%>">
                <img  class="icon" src="<%#(String)Eval("Icon")%>" alt="" />
                <span class="label"><%#HttpUtility.HtmlEncode((String)Eval("Label"))%></span>
              </a>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </td>
    </tr>
  </table>
</div>
<asp:PlaceHolder runat="server" ID="ImportEmploeeysNavigationPanel"> </asp:PlaceHolder>
<im:ImportUsersWebControl ID="imm" runat="server" />