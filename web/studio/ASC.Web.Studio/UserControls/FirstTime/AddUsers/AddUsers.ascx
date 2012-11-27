<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddUsers.ascx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.AddUsers" %>
<div id="wizard_users">
    <table id="userList">
    </table>
    <div class="clearFix" id="addUserBlock">
        <div class="nameBox">
            <div>
                <%= Resources.Resource.WizardUsersName %></div>
            <div>
                <input type="text" id="firstName" class="textEdit" maxlength="64"/>
            </div>
        </div>
        <div class="lastnameBox">
            <div>
                <%= Resources.Resource.WizardUsersLastName %></div>
            <div>
                <input type="text" id="lastName" class="textEdit" maxlength="64"/>
            </div>
        </div>
        <div class="emailBox">
            <div>
                <%= Resources.Resource.WizardUsersEmail %></div>
            <div>
                <input type="text" id="email" class="textEdit" maxlength="64"/>
            </div>
        </div>
         <div class="btn">
         <div class="btnBox">
                    <a class="grayLinkButton" id="saveSettingsBtn" href="javascript:void(0);" onclick="AddUsersManager.AddUser();">
                        <%= Resources.Resource.WizardUsersAddUser %></a>
                </div>
        </div>
    </div>
</div>
<script>
AddUsersManager._mobile = <%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context).ToString().ToLower()%>;
</script>