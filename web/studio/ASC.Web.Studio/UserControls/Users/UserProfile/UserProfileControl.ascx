<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserProfileControl.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.UserProfileControl" %>
<%@ Register TagPrefix="ucc" Namespace="ASC.Web.Studio.UserControls.Users.UserProfile"
    Assembly="ASC.Web.Studio" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<div id="studio_userProfileCardInfo">
</div>
<div class="userProfileCard clearFix<%=(UserInfo.ActivationStatus == EmployeeActivationStatus.Pending) ? " pending" : ""%>">
    <div class="additionInfo">
        <%
            if (UserInfo.ActivationStatus == EmployeeActivationStatus.Pending && !UserHasAvatar)
            {%>
            <div class="borderBase" style="position:relative;">    
                <img alt="" src="<%=MainImgUrl%>" />
                <div class="pendingInfo borderBase tintMedium" style="top:76px;"><div><%=Resources.Resource.PendingTitle%></div></div>
            </div>
        <%
            }
            else
            {%>
            <div class="userPhoto">
                <img alt="" class="userPhoto" src="<%=MainImgUrl%>" />
            </div>
        <%
            }
            if (!MyStaffMode)
            {%>
        <ul class="info">
                <li class="field contact <%=UserIsOnline() ? "online" : "offline"%> even" data-username="<%=UserName()%>"><span>
                <%=UserIsOnline() ? Resources.Resource.Online : Resources.Resource.Offline%></span>
            </li>
        </ul>
        <%
            }%>
        <%--edit thumbnail--%>
        <%
            if (MyStaffMode && UserHasAvatar && !ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
            {%>
        <a style="margin-top: 5px;" onclick="UserPhotoThumbnail.ShowDialog();" href="javascript:void(0);"
            class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "")%>">
            <%=Resources.Resource.EditThumbnailPhoto%></a>
        <%
            }%>
    </div>
    <div class="userInfo">
        <table class="info">
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Name%>:
                </td>
                <td class="value">
                    <%=DisplayUserSettings.GetFullUserName(UserInfo)%>
                </td>
            </tr>
            <%
            if (!String.IsNullOrEmpty(UserInfo.Department))
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Department").HtmlEncode()%>:
                </td>
                <td class="value">
                    <%=RenderDepartment()%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (!String.IsNullOrEmpty(UserInfo.Title))
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("UserPost").HtmlEncode()%>:
                </td>
                <td class="value">
                    <%=HttpUtility.HtmlEncode(UserInfo.Title)%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (UserInfo.Sex != null)
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Sex%>:
                </td>
                <td class="value">
                    <%=(UserInfo.Sex.HasValue ? UserInfo.Sex.Value ? Resources.Resource.MaleSexStatus : Resources.Resource.FemaleSexStatus : string.Empty)%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (UserInfo.BirthDate.HasValue && !String.IsNullOrEmpty(UserInfo.BirthDate.Value.ToShortDateString()))
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Birthdate%>:
                </td>
                <td class="value">
                    <%=UserInfo.BirthDate == null ? string.Empty : UserInfo.BirthDate.Value.ToShortDateString()%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (UserInfo.WorkFromDate.HasValue && !String.IsNullOrEmpty(UserInfo.WorkFromDate.Value.ToShortDateString()))
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("WorkFromDate").HtmlEncode()%>:
                </td>
                <td class="value">
                    <%=UserInfo.WorkFromDate == null ? string.Empty : UserInfo.WorkFromDate.Value.ToShortDateString()%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (ShowUserLocation)
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Location%>:
                </td>
                <td class="value contact">
                    <%=HttpUtility.HtmlEncode(UserInfo.Location)%>
                </td>
            </tr>
            <%
            }%>
            <asp:PlaceHolder runat="server" ID="_phEmailControlsHolder"></asp:PlaceHolder>
            <%
            if (MyStaffMode && _allowChangePwd)
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.Password%>:
                </td>
                <td class="value">
                    <%--change pwd--%>
                    <a onclick="javascript:AuthManager.ShowPwdReminderDialog('1','<%=UserInfo.Email%>'); return false;" class="baseLinkAction">
                        <%=Resources.Resource.ChangeSelfPwd%></a>
                </td>
            </tr>
            <%
            }%>
            
            <%
            if (SetupInfo.IsVisibleSettings<StudioSmsNotificationSettings>() &&(_isAdmin && !String.IsNullOrEmpty(UserInfo.MobilePhone) || _self))
            {%>
            <tr class="field">
                <td class="name textBigDescribe">
                    <%=Resources.Resource.MobilePhone%>:
                </td>
                <td class="value contact">
                <%
                if (!String.IsNullOrEmpty(UserInfo.MobilePhone))
                {%>
                   <span class="primarymobile"><%=UserInfo.MobilePhone%></span>
                    <%
                }%>
                    <%
                if (_self)
                {%>
                    <a href="#" onclick="UserMobilePhoneManager.OpenDialogCreatePhone();" style="<%=!String.IsNullOrEmpty(UserInfo.MobilePhone) ? "margin-left: 20px;" : ""%>
                        padding: 0;" class="baseLinkAction"><%=Resources.Resource.MobilePhoneChange%></a>
                    <%
                }%>
                    
                     <%
                if (!MyStaffMode && _isAdmin && !_self)
                {%>
          <a href="#" onclick="UserMobilePhoneManager.OpenDialogErasePhone();" style="<%=!String.IsNullOrEmpty(UserInfo.MobilePhone) ? "margin-left: 20px;" : ""%>
                        padding: 0;" class="baseLinkAction"><%=Resources.Resource.MobilePhoneErase%></a>
          <%
                }%>
                </td>
            </tr>
            <%
            }%>
            <asp:Repeater ID="SocialContacts" runat="server">
                <ItemTemplate>
                    <tr class="field">
                        <td class="name textBigDescribe">
                            <%#GetContactsName(Container.DataItem as Dictionary<String, String>)%>:
                        </td>
                        <td class="value contact">
                            <%#GetContactsLink(Container.DataItem as Dictionary<String, String>)%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%
            if (!String.IsNullOrEmpty(UserInfo.Notes))
            {%>
            <tr class="field">
                <td class="name textBigDescribe" valign="top">
                    <%=Resources.Resource.Comments%>:
                </td>
                <td class="value">
                    <%=HttpUtility.HtmlEncode(UserInfo.Notes)%>
                </td>
            </tr>
            <%
            }%>
           
            <%
            if (UserInfo.Status == ASC.Core.Users.EmployeeStatus.Terminated)
            {%>
            <tr valign="top" style="height: 30px;">
                <td style="width: 110px; text-align: right;" class="textBigDescribe">
                    <div class="cornerAll disabledHeader" style="float: right;">
                        <%=Resources.Resource.DisabledEmployeeTitle%></div>
                </td>
                <td style="padding-left: 20px;">
                    <%=UserInfo.TerminatedDate == null ? string.Empty : UserInfo.TerminatedDate.Value.ToShortDateString()%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (!MyStaffMode)
            {%>
            <tr valign="top" style="height: 30px;">
                <td colspan="2" style="padding-top: 20px; color:#272727 !important;">
                    <%=RenderEditDelete()%>
                </td>
            </tr>
            <%
            }%>
            <%
            if (MyStaffMode && SetupInfo.ThirdPartyAuthEnabled)
            {%>
            <tr valign="top" class="field">
                <td class="name textBigDescribe">
                </td>
                <td class="value">
                    <div class="clearFix row">
                        &nbsp;</div>
                </td>
            </tr>
            <tr valign="top" class="field">
                <td class="name textBigDescribe social">
                
                    <div style="float:right;"><%=Resources.Resource.AssociateAccount%>:</div>
                    <div style="margin:2px 5px 0 0; float:right;" class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpSocialAccount'});" title="<%=Resources.Resource.HelpQuestionSocialAccount%>"> </div>
                    <div class="popup_helper" id="AnswerForHelpSocialAccount"><p><%=String.Format(Resources.Resource.HelpAnswerSocialAccount, "<br />", "<b>", "</b>")%></p></div>        
                </td>
                <td class="value">
                    <asp:PlaceHolder runat="server" ID="_accountPlaceholder"></asp:PlaceHolder>
                    
                </td>
            </tr>
            <%
            }%>
        </table>
    </div>
</div>
<%
            if (MyStaffMode && _allowEditSelf)
            {%>
<div class="borderBase clearFix" style="margin-top: 20px; border-bottom: none; border-right: none;
    border-left: none; padding-top: 10px;">
    <%--edit profile--%>
    <a style="float: left; margin-right: 16px;" onclick="javascript:StudioUserMaker.ShowEditUserDialog('<%=SecurityContext.CurrentAccount.ID%> ',AuthManager.EditProfileCallback); return false;"
        href="#" class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "")%>">
        <%=Resources.Resource.EditSelfProfile%></a>
         
         <%
                if (!UserInfo.IsOwner())
                {%>   
            <a class="delete baseLinkAction" onclick="javascript:ProfileManager.ShowDeleteUserWindow('<%=UserInfo.ID%>'); return false;"><%=Resources.Resource.DeleteSelfProfile%></a>
            <%
                }%>
</div>
<%
            }%>
<asp:PlaceHolder runat="server" ID="_editControlsHolder"></asp:PlaceHolder>
<div id="studio_emailChangeDialog" style="display: none;">
    <ascwc:Container runat="server" ID="_emailChangerContainer">
        <Header>
            <span id="emailActivationDialogPopupHeader" style="display: none;">
                <%=Resources.Resource.EmailActivationTitle %></span> <span id="emailChangeDialogPopupHeader"
                    style="display: none;">
                    <%=Resources.Resource.EmailChangeTitle %></span>
                    
                    <span id="resendInviteDialogPopupHeader"
                    style="display: none;">
                    <%=Resources.Resource.ResendInviteTitle%></span>
        </Header>
        <Body>
            <div id="studio_emailOperationContent" style="display: none;">
                <div class="clearFix">
                    <div id="emailInputContainer" style="display: none;">
                        <div id="divEmailOperationError" class="errorBox" style="display:none;">
                        </div>
                        <div id="emailInput">
                            <div>
                                <%=Resources.Resource.EnterEmail %>
                            </div>
                            <div style="margin: 5px 0 15px 0px;">
                                <input type="text" id="emailOperation_email" class="textEdit" style="width: 99%;" />
                            </div>
                        </div>
                        <div id="emailChangeDialogText" style="display: none;">
                            <%= String.Format(Resources.Resource.EmailChangeDescription,HttpUtility.HtmlEncode(UserInfo.FirstName + " " + UserInfo.LastName))%>
                        </div>
                        <div id="emailActivationDialogText" style="display: none;">
                            <%= String.Format(Resources.Resource.EmailActivationDescription,HttpUtility.HtmlEncode(UserInfo.FirstName + " " + UserInfo.LastName))%>
                        </div>
                         <div id="resendInviteDialogText" style="display: none;">
                            <%= String.Format(Resources.Resource.ResendInviteDescription,HttpUtility.HtmlEncode(UserInfo.FirstName + " " + UserInfo.LastName))%>
                        </div>
                    </div>
                    <div id="emailMessageContainer" style="display: none;">
                        <div id="emailActivationText" style="display: none;">
                            <%= String.Format(Resources.Resource.MessageSendEmailActivationInstructionsOnEmail, "<a name='userEmail'></a>")%>
                        </div>
                        <div id="emailChangeText" style="display: none;">
                            <%= String.Format(Resources.Resource.MessageSendEmailChangeInstructionsOnEmail, "<a name='userEmail'></a>")%>
                        </div>
                        <div id="resendInviteText" style="display: none;">
                            <%= String.Format(Resources.Resource.MessageReSendInviteInstructionsOnEmail, "<a name='userEmail'></a>")%>
                        </div>
                    </div>
                </div>
                <div class="clearFix" style="margin-top: 16px;">
                    <a class="baseLinkButton" style="float: left;" href="#" id="btEmailOperationSend">
                        <%=Resources.Resource.SendButton%></a> <a class="grayLinkButton" style="float: left;
                            margin-left: 8px;" href="javascript:EmailOperationManager.CloseEmailOperationWindow(); return false;"
                            onclick="EmailOperationManager.CloseEmailOperationWindow(); return false;">
                            <%=Resources.Resource.CancelButton%></a>
                </div>
                <div style="padding-top: 16px; display: none;" id="pwd_rem_action_loader" class="clearFix">
                    <div class="textMediumDescribe">
                        <%=Resources.Resource.PleaseWaitMessage%>
                    </div>
                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
                </div>
            </div>
            <div id="studio_emailOperationResult" style="text-align: left; display: none;" class="clearFix">
                <div id="studio_emailOperationResultText" style="margin: 5px 0 10px 0;">
                </div>
                <div>
                    <a class="grayLinkButton" style="float: left;" href="javascript:EmailOperationManager.CloseEmailOperationWindow(); return false;"
                        onclick="EmailOperationManager.CloseEmailOperationWindow(); return false;">
                        <%=Resources.Resource.CloseButton%></a>
                </div>
            </div>
        </Body>
    </ascwc:Container>
</div>

<div id="studio_deleteProfileDialog" style="display: none;">
    <ascwc:Container runat="server" ID="_deleteProfileContainer">
        <Header>
        <%= Resources.Resource.DeleteProfileTitle%>
        </Header>
        <Body>
        <div id="remove_content">
            <div>
               <%= Resources.Resource.DeleteProfileInfo%></div>
            <div class="email">
                <%= UserInfo.Email %>
            </div>
            </div>
            <div class="clearFix buttons">
                <a style="float: left; margin-right: 8px;" href="javascript:void(0);" class="baseLinkButton" onclick="ProfileManager.SendInstrunctionsToRemoveProfile();"><%=Resources.Resource.SendButton%></a>
                <a style="float: left; margin-right: 8px;" href="javascript:void(0);" class="grayLinkButton" onclick="PopupKeyUpActionProvider.CloseDialog();"><%=Resources.Resource.CloseButton%></a>
            </div>
        </Body>
    </ascwc:Container>
</div>

<div id="studio_mobilePhoneChangeDialog" style="display: none;">
    <ascwc:Container runat="server" ID="_changePhoneContainer">
        <Header>
            <%= Resources.Resource.MobilePhoneChangeTitle %>
        </Header>
        <Body>
            <div id="studio_mobilePhoneOperationContent">
                <div class="clearFix" id="desc">
                   <%= Resources.Resource.MobilePhoneChangeDescription %>
                </div>
                <div class="clearFix" style="margin-top: 16px;">
                    <a class="baseLinkButton" style="float: left;" href="#" id="phoneChangeInst">
                        <%=Resources.Resource.SendButton%></a> <a class="grayLinkButton" style="float: left;
                            margin-left: 8px;" href="javascript:return false;"
                            onclick="jq.unblockUI(); return false;">
                            <%=Resources.Resource.CancelButton%></a>
                </div>
            </div>
            <div style="padding-top: 16px; display: none;" id="studio_mobilePhoneOperationProgress"
                class="clearFix">
                <div class="textMediumDescribe">
                    <%=Resources.Resource.PleaseWaitMessage%>
                </div>
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
            </div>
            <div id="studio_mobilePhoneOperationContentResult" style="text-align: left; display: none;margin: 5px 0 10px 0;"
                class="clearFix">
                    <%= Resources.Resource.MobilePhoneChangeSent %>
            </div>
        </Body>
    </ascwc:Container>
</div>


<script type="text/javascript">
    jq(function() {
        UserMobilePhoneManager._createText = "<%=Resources.Resource.MobilePhoneChangeDescription%>";
        UserMobilePhoneManager._eraseText = "<%=Resources.Resource.MobilePhoneEraseDescription%>";
        <%if (_isAdmin && !_self){%>
        UserMobilePhoneManager.uid = "<%=UserInfo.ID%>"; 
        <%}%>
    });
</script>