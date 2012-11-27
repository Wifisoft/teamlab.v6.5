<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GreetingSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.GreetingSettings" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
        
<div class="headerBase borderBase greetingTitle clearFix">
     <div style="float: left;">
        <%=Resources.Resource.GreetingSettingsTitle%>
     </div>
	
	<div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpGreeting'});" title="<%=Resources.Resource.HelpQuestionGreetingSettings%>"></div> 
    <div class="popup_helper" id="AnswerForHelpGreeting">
     <p><%=String.Format(Resources.Resource.HelpAnswerGreetingSettings, "<br />","<b>","</b>")%>
     <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
    </div>    
	
</div>
<div id="studio_setInfGreetingSettingsInfo"></div>
<div id="studio_greetingSettingsBox" class="clearFix">
        
      <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>

     <div class="clearFix" style="margin-top:20px;">
                <a id="saveGreetSettingsBtn" class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>"  href="javascript:void(0);" ><%=Resources.Resource.SaveButton %></a>
                <a id="restoreGreetSettingsBtn" class="grayLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" href="javascript:void(0);" ><%=Resources.Resource.RestoreDefaultButton%></a>
     </div>               
</div>

      