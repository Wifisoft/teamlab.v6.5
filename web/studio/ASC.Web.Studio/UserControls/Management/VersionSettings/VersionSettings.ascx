<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersionSettings.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Management.VersionSettings.VersionSettings" %>
<div class="headerBase borderBase clearFix" id="versionSettingsTitle">
    <div style="float: left;">
        <%=Resources.Resource.StudioVersionSettings%>
    </div>
    <%--<div class="help_center">
        <span class="linkDescribe QuestionForHelpVersion" title="<%=Resources.Resource.HelpQuestionPasswordSettings%>">
        </span>
    </div>--%>
</div>
<div class="clearFix" id="studio_versionSetting">
    <div class="clearFix" style="padding: 20px 15px;">
        <div class="clearFix">
            <div style="width: 500px;" class="studioHeaderBaseSmallValue" id="versionSelector">
                <% foreach (var tenantVersion in ASC.Core.CoreContext.TenantManager.GetTenantVersions())
                   {%>
                <div>
                    <input type="radio" name="version" id="radio<%=tenantVersion.Id%>" value="<%=tenantVersion.Id%>"
                        <%=ASC.Core.CoreContext.TenantManager.GetCurrentTenant(false).Version==tenantVersion.Id?"checked=\"checked\"":"" %>
                         />
                    <%if (ASC.Core.CoreContext.TenantManager.GetCurrentTenant(false).Version == tenantVersion.Id)
                      {%>
                    <label for="radio<%= tenantVersion.Id %>">
                        <strong>
                            <%= GetLocalizedName(tenantVersion.Name) %>
                        </strong>
                    </label>
                    <% }
                      else
                      {%>
                    <label for="radio<%= tenantVersion.Id %>">
                        <%= GetLocalizedName(tenantVersion.Name)%>
                    </label>
                    <%} %>
                </div>
                <%} %>
            </div>
        </div>
        <div id="studio_versionSetting_info">
        </div>
        <div class="clearFix" style="margin-top: 20px;">
            <a class="baseLinkButton" style="float: left;" onclick="StudioVersionManagement.SwitchVersion();"
                href="javascript:void(0);">
                <%=Resources.Resource.SaveButton%></a>
        </div>
    </div>
</div>
