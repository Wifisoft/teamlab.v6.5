<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddContentControl.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.AddContentControl" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<div id="studio_AddContentDialog" style="display:none;">
  <ascwc:Container runat="server" ID="AddContentContainer">
    <Header>
      <%=Resources.Resource.AddContentTitle%>
    </Header>
    <Body>
      <div id="studio_AddContentContent">
        <asp:Repeater runat="server" ID="ContentTypesRepeater">
          <HeaderTemplate>
            <ul class="types">
          </HeaderTemplate>
          <ItemTemplate>
            <li class="type even">
              <a class="item" href="<%#(String)Eval("Link")%>">
                <img class="icon" src="<%#(String)Eval("Icon")%>" alt="" />
                <span class="label"><%#HttpUtility.HtmlEncode((String)Eval("Label"))%></span>   
              </a>
            </li>
          </ItemTemplate>
          <FooterTemplate>
            </ul>
          </FooterTemplate>
        </asp:Repeater>

        <div class="additionMenu">        
          <a class="button grayLinkButton" href="./" onclick="return StudioManager.CloseAddContentDialog()"><%=Resources.Resource.CancelButton %></a>
        </div>
      </div>
    </Body>
  </ascwc:Container>
</div>
