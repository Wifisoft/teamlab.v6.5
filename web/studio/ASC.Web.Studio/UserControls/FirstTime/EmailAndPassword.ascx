<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailAndPassword.ascx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.EmailAndPassword" %>
<script type="text/javascript">
    jq(function() {
        ASC.Controls.EmailAndPasswordManager.changeIt = '<%= Resources.Resource.EmailAndPasswordTypeChangeIt %>';
        ASC.Controls.EmailAndPasswordManager.ok = '<%= Resources.Resource.EmailAndPasswordOK %>';
        ASC.Controls.EmailAndPasswordManager.wrongPass = '<%= Resources.Resource.EmailAndPasswordWrongPassword %>';
        ASC.Controls.EmailAndPasswordManager.emptyPass = '<%= Resources.Resource.EmailAndPasswordEmptyPassword %>';
        ASC.Controls.EmailAndPasswordManager.wrongEmail = '<%= Resources.Resource.EmailAndPasswordIncorrectEmail %>';
        ASC.Controls.EmailAndPasswordManager.portalName = '<%= _curTenant.Name %>';
        jq('#portalName').val('<%= _curTenant.Name %>');

        jq('#portalName').addClass('textEditDefault');
        ASC.Controls.EmailAndPasswordManager.SetPasswordControlSettings('portalName', ASC.Controls.EmailAndPasswordManager.portalName);
    }
    );
</script>
<div id="requiredStep" class="clearFix">
<div class="personal">
    <div class="emailBlock">
        <div class="info">
            <%= Resources.Resource.EmailAndPasswordRegEmail %></div>
        <div class="email clearFix">
            <div class="emailAddress"><%= this.GetEmail() %>
            </div>
            <div class="changeEmail clearFix">
            <div id="dvChangeMail"><a class="info baseLinkAction" onclick="ASC.Controls.EmailAndPasswordManager.ShowChangeEmailAddress();"><%= Resources.Resource.EmailAndPasswordTypeChangeIt %></a></div></div>
        </div>
        
    </div>
    <div class="passwordBlock">
        <div class="clearFix">
         <div class="pwd clearFix">
                <div class="label">
                    <%= Resources.Resource.EmailAndPasswordTypePassword %> <span class="info"><%= Resources.Resource.EmailAndPasswordTypePasswordRecommendations %></span><span>*</span></div>
                <div style="float:left;">
                    <input type="password" id="newPwd" class="textEdit" maxlength="30" />
                </div>
            </div>
            <div class="pwd">
                <div class="label">
                    <%= Resources.Resource.EmailAndPasswordConfirmPassword %><span>*</span></div>
                <div>
                    <input type="password" id="confPwd" class="textEdit" maxlength="30" />
                </div>
            </div>
        </div>
    </div>
    </div>
    <div class="portal">
        <div class="info">
            <%= Resources.Resource.EmailAndPasswordDomain%></div>
        <div class="domainname">
            <%= _curTenant.TenantDomain %></div>
        <div class="info portalname label">
            <%= Resources.Resource.EmailAndPasswordPortalName%></div>
        <div>
            <input type="text" class="textEdit" id="portalName" maxlength="255" />
        </div>
        <div style="padding-top:5px;">
        <%= Resources.Resource.WizardPortalNameDescription%>
        </div>
    </div>
</div>