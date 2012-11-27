<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmsValidationSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.SmsValidationSettings" %>
 <div class="headerBase borderBase clearFix" id="smsValidationSettingsTitle">
    <div class="title">
		<%= Resources.Resource.SmsAuthTitle %>
		
	</div>
</div>
<div id="studio_smsValidationSettingsInfo">
</div>
<div id="studio_smsValidationSettings">
    <div style="margin-bottom: 10px;">
    <%= Resources.Resource.SmsAuthDescription %>
    </div>
    
    <div class="clearFix">
        <input type="checkbox" id="chk_studio_2FactorAuth" <%=(_studioSmsNotifSettings.Enable?"checked=\"checked\"":"")%> />
        <label for="chk_studio_2FactorAuth">
                <%=Resources.Resource.SmsAuthCheckboxValue%></label>
        </div>
    <div class="clearFix" style="margin-top: 20px;">
        <a class="baseLinkButton"
            style="float: left;" onclick="SmsValidationSettings.SaveSmsValidationSettings();" href="javascript:void(0);">
            <%=Resources.Resource.SaveButton %></a>
    </div>
</div>
