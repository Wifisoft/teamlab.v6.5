<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmMobileActivation.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ConfirmMobileActivation" %>

<div id="error" class="errorBox">
</div>

<div id="sendAuthCode" style="display:<%= Activate?"block":"none" %>">
        <div class="label">
          <%= String.Format(Resources.Resource.ActivateMobilePhoneDescription,"<br />") %> 
        </div>
        <div>
        <div class="phoneLabel">
        <%= Resources.Resource.ActivateMobilePhoneLabel%>
        </div>
            <input type="text" id="studio_phone" maxlength="64" value="" class="pwdLoginTextbox" />
        </div>
        <div class="btn">
            <a class="highLinkButton send" onclick="ConfirmMobileManager.SendAuthCode(); return false;"><%= Resources.Resource.ActivateGetCodeButton %></a>
        </div>

</div>

<div id="putAuthCode" style="display: <%= !Activate?"block":"none" %>">
    <div class="label">
        <%= String.Format(Resources.Resource.ActivateCodeDescription,"<span class='noise'>",GetPhoneNoise(),"</span>",Resources.Resource.ActivateSendButton,Resources.Resource.ActivateAgainGetCodeButton,"<br />") %>
    </div>
    <div style="position: relative;margin-top: 20px;">
        <input type="text" id="studio_phone_authcode" maxlength="6" value="" class="pwdLoginTextbox" />
        <div class="again">
            <a class="grayLinkButton again" onclick="ConfirmMobileManager.SendAuthCodeAgain(); return false;"><%= Resources.Resource.ActivateAgainGetCodeButton %></a>
        </div>
    </div>
    <div>
        <div class="clearFix check">
            <input type="checkbox" id="studio_phone_keepUser" checked="checked" />
            <label for="studio_phone_keepUser">
                <%= Resources.Resource.ActivateRememberMe %></label>
        </div>
    </div>
    <div style="margin-top: 20px;">
        <a class="highLinkButton send" onclick="ConfirmMobileManager.ValidateAuthCode(); return false;"><%= Resources.Resource.ActivateSendButton %></a>
    </div>
    <div id="GreetingBlock">
        <div class="submenu clearFix">
            <asp:PlaceHolder ID="_communitations" runat="server"></asp:PlaceHolder>
        </div>
    </div>
</div>
<script type="text/javascript">
    jq(function() {
        ConfirmMobileManager._cellHint = "<%= Resources.Resource.ActivateMobilePhoneHint %>";
        ConfirmMobileManager._codeHint = "<%= Resources.Resource.ActivateCodeLabel %>";
        ConfirmMobileManager._emptyCell = "<%= Resources.Resource.ActivateMobilePhoneEmptyPhoneNumber %>";
        ConfirmMobileManager._emptyCode = "<%= Resources.Resource.ActivateMobilePhoneEmptyCode %>";
        ConfirmMobileManager._activate = <%= Activate.ToString().ToLower() %>;
        ConfirmMobileManager.Init();
    });
</script>

<%--popup window--%>
<div id="answerPhoneFormat" class="hintDescriptionPanel help">
     Please enter mobile phone number using an international format with country code
     <div class="popup-corner" ></div>
</div>    