<%@ Assembly Name="ASC.Web.Talk" %>
<%@ Page Language="C#" MasterPageFile="~/Masters/BaseTemplate.master" AutoEventWireup="true" CodeBehind="JabberClient.aspx.cs" Inherits="ASC.Web.Talk.JabberClient" Title="Untitled Page" %>

<%@ Import Namespace="ASC.Web.Talk.Addon" %>
<%@ Import Namespace="ASC.Data.Storage" %>
<%@ Import Namespace="ASC.Web.Talk.Resources" %>

<%@ Register Src="~/addons/talk/UserControls/TabsContainer.ascx" TagName="TabsContainer" TagPrefix="asct" %>
<%@ Register Src="~/addons/talk/UserControls/RoomsContainer.ascx" TagName="RoomsContainer" TagPrefix="asct" %>
<%@ Register Src="~/addons/talk/UserControls/MeseditorContainer.ascx" TagName="MeseditorContainer" TagPrefix="asct" %>
<%@ Register Src="~/addons/talk/UserControls/ContactsContainer.ascx" TagName="ContactsContainer" TagPrefix="asct" %>

<asp:Content ID="TalkHeaderContent" ContentPlaceHolderID="TopHeaderContent" runat="server">
  <!--[if IE 9]>
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE9" />
  <![endif]-->
</asp:Content>

<asp:Content ID="TalkContent" ContentPlaceHolderID="PageContent" runat="server">
  <script type="text/javascript">
    TMTalk.init('ajaxupload.ashx?type=ASC.Web.Talk.UploadFileHanler,ASC.Web.Talk');
    ASC.TMTalk.properties.init('2.0');
    ASC.TMTalk.iconManager.init();
    ASC.TMTalk.notifications.init('<%=GetUserPhotoHandlerPath()%>', '<%=GetNotificationHandlerPath()%>');
    ASC.TMTalk.msManager.init('<%=GetValidSymbols()%>');
    ASC.TMTalk.mucManager.init('<%=GetValidSymbols()%>');
    ASC.TMTalk.roomsManager.init();
    ASC.TMTalk.contactsManager.init();
    ASC.TMTalk.messagesManager.init('<%=GetShortDateFormat()%>', '<%=GetFullDateFormat()%>', '<%=GetMonthNames()%>', '<%=GetHistoryLength()%>');
    ASC.TMTalk.connectionManager.init('<%=GetBoshUri()%>', '<%=GetJabberAccount()%>', '<%=GetResourcePriority()%>', '<%=GetInactivity()%>');

    ASC.TMTalk.properties.item('addonID', '<%=TalkAddon.AddonID%>');
    ASC.TMTalk.properties.item('enabledMassend', '<%=GetMassendState()%>');
    ASC.TMTalk.properties.item('enabledConferences', '<%=GetConferenceState()%>');
    ASC.TMTalk.properties.item('requestTransportType', '<%=GetRequestTransportType()%>');
    ASC.TMTalk.properties.item('fileTransportType', '<%=GetFileTransportType()%>');
    ASC.TMTalk.properties.item('maxUploadSize', '<%=ASC.Web.Studio.Core.SetupInfo.MaxImageUploadSize%>');
    ASC.TMTalk.properties.item('sounds', '<%=WebPath.GetPath("addons/talk/swf/sounds.swf")%>');
    ASC.TMTalk.properties.item('expressInstall', '<%=WebPath.GetPath("addons/talk/swf/expressinstall.swf")%>');
  </script>
  <div id="talkWrapper" style="display:none;">
    <div class="left-side"></div>
    <div class="right-side"></div>
    <div class="container">
      <asct:TabsContainer ID="TalkTabsContainer" runat="server" />
      <div id="talkMainContainer">
        <div id="talkDialogsContainer">
          <div class="background"></div>

          <div class="dialog browser-notifications">
            <div class="head" unselectable="on">
              <div class="left-side"></div>
              <div class="right-side"></div>
              <div class="title" unselectable="on"><%=TalkResource.TitleBrowserNotificationsDialog%></div>
              <div class="button close-dialog" unselectable="on"></div>
            </div>
            <div class="content" unselectable="on">
              <div class="in" unselectable="on">
                <div class="body" unselectable="on">
                  <div id="cbxToggleNotifications" class="block toggle-notifications disabled" unselectable="on">
                    <div class="button notifications-allow" unselectable="on"></div>
                    <div class="button notifications-delay" unselectable="on"></div>
                  </div>
                </div>
                <div class="toolbar" unselectable="on">
                  <div class="container" unselectable="on">
                    <div class="checkbox-container toggle-browser-notifications-dialog" unselectable="on">
                      <table unselectable="on">
                        <tr unselectable="on">
                          <td unselectable="on"><input id="cbxToggleNotificationsDialog" name="cbxToggleNotificationsDialog" class="checkbox toggle-notifications-dialog" type="checkbox" checked="checked" /></td>
                          <td unselectable="on"><label for="cbxToggleNotificationsDialog" unselectable="on"><%=TalkResource.LabelDoNotAsk%></label></td>
                        </tr>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="dialog kick-occupant">
            <div class="head" unselectable="on">
              <div class="left-side"></div>
              <div class="right-side"></div>
              <div class="title" unselectable="on"><%=TalkResource.TitleKickOccupantDialog%>&nbsp;<span class="value" unselectable="on"></span></div>
              <div class="button close-dialog" unselectable="on"></div>
            </div>
            <div class="content" unselectable="on">
              <div class="in" unselectable="on">
                <div class="body" unselectable="on">
                  <input id="hdnKickJId" type="hidden" />
                  <div class="singlefield" unselectable="on"><%=TalkResource.LabelKickOccupant%></div>
                </div>
                <div class="toolbar" unselectable="on">
                  <div class="container" unselectable="on">
                    <div class="button-container" unselectable="on">
                      <div class="left-side" unselectable="on"></div>
                      <div class="right-side" unselectable="on"></div>
                      <div class="button kick-occupant" unselectable="on"><%=TalkResource.BtnKick%></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="dialog create-room">
            <div class="head" unselectable="on">
              <div class="left-side"></div>
              <div class="right-side"></div>
              <div class="title" unselectable="on"><%=TalkResource.TitleCreateRoom%></div>
              <div class="button close-dialog" unselectable="on"></div>
            </div>
            <div class="content" unselectable="on">
              <div class="in" unselectable="on">
                <div class="body" unselectable="on">
                  <div class="field hint" unselectable="on">
                    <label for="txtRoomName" unselectable="on"><%=TalkResource.LabelName%>:</label>
                    <div class="textfield"><input id="txtRoomName" type="text" maxlength="255" /></div>
                    <label class="hint" for="txtRoomName" unselectable="on"><%=TalkResource.LabelValidSymbols%>:&nbsp;<%=TalkResource.ValidSymbols%></label>
                  </div>
                  <table class="field" unselectable="on">
                    <tr unselectable="on">
                      <td><input id="cbxTemporaryRoom" name="cbxTemporaryRoom" class="checkbox temporar-room" type="checkbox" checked="checked" /></td>
                      <td unselectable="on"><label for="cbxTemporaryRoom" unselectable="on"><%=TalkResource.LabelTemporaryRoom%></label></td>
                    </tr>
                  </table>
                </div>
                <div class="toolbar" unselectable="on">
                  <div class="container" unselectable="on">
                    <div class="button-container" unselectable="on">
                      <div class="left-side" unselectable="on"></div>
                      <div class="right-side" unselectable="on"></div>
                      <div class="button create-room" unselectable="on"><%=TalkResource.BtnCreate%></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="dialog create-mailing">
            <div class="head" unselectable="on">
              <div class="left-side"></div>
              <div class="right-side"></div>
              <div class="title" unselectable="on"><%=TalkResource.TitleCreateMailing%></div>
              <div class="button close-dialog" unselectable="on"></div>
            </div>
            <div class="content" unselectable="on">
              <div class="in" unselectable="on">
                <div class="body" unselectable="on">
                  <div class="field hint" unselectable="on">
                    <label for="txtMailingName" unselectable="on"><%=TalkResource.LabelName%>:</label>
                    <div class="textfield"><input id="txtMailingName" type="text" maxlength="255" /></div>
                    <label class="hint" for="txtRoomName" unselectable="on"><%=TalkResource.LabelValidSymbols%>:&nbsp;<%=TalkResource.ValidSymbols%></label>
                  </div>
                </div>
                <div class="toolbar" unselectable="on">
                  <div class="container" unselectable="on">
                    <div class="button-container" unselectable="on">
                      <div class="left-side" unselectable="on"></div>
                      <div class="right-side" unselectable="on"></div>
                      <div class="button create-mailing" unselectable="on"><%=TalkResource.BtnCreate%></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="dialog remove-room">
            <div class="head" unselectable="on">
              <div class="left-side"></div>
              <div class="right-side"></div>
              <div class="title" unselectable="on"><%=TalkResource.TitleRemoveRoomDialog%>&nbsp;<span class="value" unselectable="on"></span></div>
              <div class="button close-dialog" unselectable="on"></div>
            </div>
            <div class="content" unselectable="on">
              <div class="in" unselectable="on">
                <div class="body" unselectable="on">
                  <input id="hdnRemoveJid" type="hidden" />
                  <div class="singlefield" unselectable="on"><%=TalkResource.LabelRemoveRoom%></div>
                </div>
                <div class="toolbar" unselectable="on">
                  <div class="container" unselectable="on">
                    <div class="button-container" unselectable="on">
                      <div class="left-side" unselectable="on"></div>
                      <div class="right-side" unselectable="on"></div>
                      <div class="button remove-room" unselectable="on"><%=TalkResource.BtnRemove%></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="dialog recv-invite">
            <div class="head" unselectable="on">
              <div class="left-side"></div>
              <div class="right-side"></div>
              <div class="title" unselectable="on"><%=TalkResource.TitleRecvInvite%>&nbsp;<span class="value" unselectable="on"></span></div>
              <div class="button close-dialog" unselectable="on"></div>
            </div>
            <div class="content" unselectable="on">
              <div class="in" unselectable="on">
                <div class="body" unselectable="on">
                  <input id="hdnInvitationRoom" type="hidden" />
                  <input id="hdnInvitationContact" type="hidden" />
                  <div class="singlefield" unselectable="on"><span id="lblInviterName" class="label" unselectable="on">Name</span>&nbsp;<%=TalkResource.LabelRecvInvite%></div>
                </div>
                <div class="toolbar" unselectable="on">
                  <div class="container" unselectable="on">
                    <div class="button-container grey" unselectable="on">
                      <div class="left-side" unselectable="on"></div>
                      <div class="right-side" unselectable="on"></div>
                      <div class="button decline-invite" unselectable="on"><%=TalkResource.BtnDecline%></div>
                    </div>
                    <div class="button-container" unselectable="on">
                      <div class="left-side" unselectable="on"></div>
                      <div class="right-side" unselectable="on"></div>
                      <div class="button accept-invite" unselectable="on"><%=TalkResource.BtnAccept%></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div id="talkContentContainer" class="disabled">
          <div id="talkStartSplash" unselectable="on">
            <div class="background" unselectable="on"></div>
            <div class="container" unselectable="on">
              <div class="right-side" unselectable="on"></div>
              <div class="label" unselectable="on"><%=string.Format(TalkResource.LabelFirstSplash,"<br/>")%></div>
            </div>
          </div>
          <asct:RoomsContainer ID="TalkRoomsContainer" runat="server" />
          <div id="talkVertSlider" unselectable="on"></div>
          <asct:MeseditorContainer ID="TalkMeseditorContainer" runat="server" />
        </div>
        <div id="talkHorSlider" unselectable="on"></div>
        <asct:ContactsContainer ID="TalkContactsContainer" runat="server" />
      </div>
    </div>
  </div>
</asp:Content>
