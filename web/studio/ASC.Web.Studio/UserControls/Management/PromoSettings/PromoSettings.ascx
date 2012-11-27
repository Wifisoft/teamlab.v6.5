<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PromoSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.PromoSettings" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<div class="headerBase borderBase clearFix"  style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
    border-top: none; border-right: none; border-left: none;">
    <div style="float:left;">
    <%=Resources.Resource.NotifyBarSettings%>
    </div>
	
	<div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpAdvertisement'});" title="<%=Resources.Resource.HelpQuestionAdvertisementPanelSetting%>"></div> 
    <div class="popup_helper" id="AnswerForHelpAdvertisement">
         <p><%=String.Format(Resources.Resource.HelpAnswerAdvertisementPanelSettings, "<br />","<b>","</b>")%>
         <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
    </div>       

	
</div>
<div id="studio_setInfNotifyBarSettingsInfo">
</div>
<div id="studio_NotifyBarSettingsBox" style="padding: 20px 15px;">

    <div class="clearFix">
        <div style="float: left;">
            <input id="studio_showPromotions" type="radio" <%=(_studioNotifyBarSettings.ShowPromotions?"checked=\"checked\"":"") %>
                name="ShowingPromotions" />
        </div>
        <div style="float: left; margin-top: 3px; margin-left: 5px;">
            <label for="studio_showPromotions">
                <%=Resources.Resource.ShowPromotions%></label>
        </div>
        <div style="float: left; margin-left: 20px;">
            <input id="studio_dontShowPromotions" type="radio" <%=(!_studioNotifyBarSettings.ShowPromotions?"checked=\"checked\"":"") %>
                name="ShowingPromotions" />
        </div>
        <div style="float: left; margin-top: 3px; margin-left: 5px;">
            <label for="studio_dontShowPromotions">
                <%=Resources.Resource.DontShowPromotions%></label>
        </div>
    </div>
    <div class="clearFix" style="margin-top: 20px;">
        <a class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>"
            style="float: left;" onclick="PromoSettings.SaveNotifyBarSettings();" href="javascript:void(0);">
            <%=Resources.Resource.SaveButton %></a>
    </div>
</div>
