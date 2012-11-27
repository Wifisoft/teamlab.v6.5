<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkinSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.SkinSettings" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>

<div class="headerBase borderBase clearFix" id="skinSettingsTitle">
    <div style="float: left;">
        <%=Resources.Resource.SkinSettings%>
    </div>
	
	<div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpSkinSettings'});" title="<%=Resources.Resource.HelpQuestionSkinSettings%>"></div> 
    <div class="popup_helper" id="AnswerForHelpSkinSettings">
     <p><%=String.Format(Resources.Resource.HelpAnswerSkinSettings, "<br />", "<b>", "</b>")%>
     <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
    </div>   
	
 </div>
<div id="studio_skinSettingsInfo">
</div>

<div id="studio_skinSettingsBox">
 
 <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
    
     <div class="clearFix" style="margin-top: 20px;">
                <a id="saveSkinSettingBtn" class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" href="javascript:void(0);"><%=Resources.Resource.SaveButton %></a>
            </div>

</div>

