<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminMessageSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AdminMessageSettings" %>
<div class="headerBase borderBase clearFix" id="admMessSettingsTitle">
    <div class="title">
        <%= Resources.Resource.AdminMessageSettingsTitle %>
    </div>
</div>
<div id="studio_admMessSettingsInfo">
</div>
<div id="studio_admMessSettings">
    <div style="margin-bottom: 10px;">
        <%= Resources.Resource.AdminMessageSettingsDescription %>
    </div>
    <div class="clearFix">
        <input type="checkbox" id="chk_studio_admMess" <%=(_studioAdmMessNotifSettings.Enable?"checked=\"checked\"":"")%> />
        <label for="chk_studio_admMess">
            <%= Resources.Resource.AdminMessageSettingsEnable %></label>
    </div>
    <div class="clearFix" style="margin-top: 20px;">
        <a class="baseLinkButton" style="float: left;" onclick="AdmMess.SaveSettings(); return false;"
            href="#">
            <%=Resources.Resource.SaveButton %></a>
    </div>
</div>
