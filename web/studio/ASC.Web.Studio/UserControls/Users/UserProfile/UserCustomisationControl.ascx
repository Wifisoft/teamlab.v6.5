<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserCustomisationControl.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserProfile.UserCustomisationControl" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>


<div class="userCustomisationSettings">
    
    <%--timezone & language--%>
        <div class="headerBase borderBase clearFix" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
                border-top: none; border-right: none; border-left: none;">
                <div style="float: left;"> <%=Resources.Resource.LanguageSettings%></div>
                
	            <div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpCustomizationPortal'});" title="<%=Resources.Resource.HelpQuestionCustomizationPortal%>"></div> 
                <div class="popup_helper" id="AnswerForHelpCustomizationPortal">
                     <p><%=String.Format(Resources.Resource.HelpAnswerCustomizationPortal, "<br />", "<b>", "</b>")%>
                     <a href="http://www.teamlab.com/help/gettingstarted/portaluser.aspx#link5" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
                </div>   
 
                
         </div>
         <div id="studio_lngTimeSettingsInfo">            
         </div>
         <div id="studio_lngTimeSettingsBox" style="padding:0px 20px 15px 20px;"> 
            <div class="clearFix" style="margin-top:20px;">
                <div  class="headerBaseSmall" style="float:left; width:90px; text-align:right; padding-right:10px;"><%=Resources.Resource.Language%>:</div>
                <div style="float:left; width:200px;" class="studioHeaderBaseSmallValue"><%=RenderLanguageSelector()%></div>
            </div>
           
            <div class="clearFix" style="margin-top: 20px;">
                    <a class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" style="float: left;" onclick="UserSettingsManager.SaveUserLanguageSettings();"
                        href="javascript:void(0);">
                        <%=Resources.Resource.SaveButton %></a>
                </div>
         </div> 
         
    <%--skin --%>     
   <asp:PlaceHolder ID="_userSkinSettings" runat="server"></asp:PlaceHolder>
    
</div>
