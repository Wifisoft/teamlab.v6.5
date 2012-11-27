<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GreetingSettingsContent.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.GreetingSettingsContent" %>


<script type="text/javascript">
            jq(document).ready(function() {
                jq("input.fileuploadinput").attr('accept', 'image/png,image/jpeg,image/gif');
            });
        </script>
        
<div class="clearFix">
    <div class="headerBaseSmall" style="text-align: left; padding-bottom: 3px;">
        <%=Resources.Resource.GreetingTitle%>:
    </div>
    <div>
        <input type="text" class="textEdit" maxlength="150" id="studio_greetingHeader" value="<%=HttpUtility.HtmlEncode(ASC.Core.CoreContext.TenantManager.GetCurrentTenant().Name)%>" />
    </div>
</div>
<div class="clearFix" style="margin-top: 15px;">
    <div class="headerBaseSmall" style="text-align: left; padding-bottom: 3px;">
        <%=Resources.Resource.GreetingLogo%>:
    </div>
    <div >
        <div class="clearFix">
            <div style="width: 280px;">
                <img id="studio_greetingLogo" class="borderBase" style="padding: 10px;" src="<%=_tenantInfoSettings.GetAbsoluteCompanyLogoPath()%>" />
            </div>
            <%if (!ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
              { %>
            <div style="margin-top:8px; float: left;">
                <input type="hidden" id="studio_greetingLogoPath" value="" />
                <a id="studio_logoUploader" href="javascript:void(0);">
                    <%=Resources.Resource.ChangeLogoButton%></a>
            </div>
            <% } %>
        </div>
    </div>
</div>
