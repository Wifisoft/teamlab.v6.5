<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailDomainSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.MailDomainSettings" %>

<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>

 <div class="headerBase borderBase clearFix" id="mailDomainSettingsTitle">
    <div style="float: left;">
		<%=Resources.Resource.StudioDomainSettings%>
	</div>
	<div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpMailDomain'});" title="<%=Resources.Resource.HelpQuestionMailDomainSettings%>"></div> 
    <div class="popup_helper" id="AnswerForHelpMailDomain">
         <p><%=String.Format(Resources.Resource.HelpAnswerMailDomainSettings, "<br />","<b>","</b>")%>
        <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
    </div>       
</div>
<div id="studio_domainSettingsInfo">    
</div>
<div id="studio_domainSettingsBox">

    <div class="clearFix">
        
        <input id="offMailDomains" type="radio" value="<%=(int)ASC.Core.Tenants.TenantTrustedDomainsType.None %>" name="signInType" <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.None?"checked=\"checked\"":""%>/>     
        <label class="headerBaseSmall" for="offMailDomains"><%=Resources.Resource.OffMailDomains%></label>
        
         <input id="trustedMailDomains" type="radio" value="<%=(int)ASC.Core.Tenants.TenantTrustedDomainsType.Custom %>" name="signInType" <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.Custom?"checked=\"checked\"":""%>/>     
        <label class="headerBaseSmall" for="trustedMailDomains"><%=Resources.Resource.TrustedDomainSignInTitle%></label>
       
    </div>
    
    <div id="trustedMailDomainsDescription" class="description"  <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.Custom?"":"style=\"display:none;\""%>>                
        <div class="clearFix" id="studio_domainListBox">
        <%for (int i = 0; i < _currentTenant.TrustedDomains.Count; i++)
          {var domain = _currentTenant.TrustedDomains[i];%>
          
                 <div name="<%=i%>" id="studio_domain_box_<%=i%>" class="clearFix" style="margin-bottom:15px;">
                       <input id="studio_domain_<%=i%>" type="text" maxlength="60" class="textEdit" value="<%=HttpUtility.HtmlEncode(domain)%>" style="width:300px;"/>
                       <a class="removeDomain" onclick="MailDomainSettingsManager.RemoveTrustedDomain('<%=i%>');" href="javascript:void(0);">
                         <img align="absmiddle" border="0" alt="<%=Resources.Resource.DeleteButton%>" src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("trash.png")%>"/>
                       </a>
                 </div>
        <%}%>
        </div>        
        
        <a href="javascript:void(0);" id="addTrustDomainBtn"><%=Resources.Resource.AddTrustedDomainButton%></a>        
    </div>
    
    <div id="allMailDomainsDescription" class="description"  <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.All?"":"style=\"display:none;\""%>>        
    </div>
    
    <div id="offMailDomainsDescription" class="description"  <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.None?"":"style=\"display:none;\""%>>        
    </div>
    
    <div class="btnBox clearFix">
        <a class="baseLinkButton" id="saveMailDomainSettingsBtn" href="javascript:void(0);">
            <%=Resources.Resource.SaveButton %></a>
    </div>
</div>    

