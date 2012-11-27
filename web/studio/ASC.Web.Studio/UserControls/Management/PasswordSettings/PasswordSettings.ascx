<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.PasswordSettings" %>

 <div class="headerBase borderBase clearFix" id="mailDomainSettingsTitle">
    <div style="float: left;">
		<%=Resources.Resource.StudioPasswordSettings%>
		
	</div>
		
    <div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpPassword'});" title="<%=Resources.Resource.HelpQuestionPasswordSettings%>"></div> 
    <div class="popup_helper" id="AnswerForHelpPassword">
         <p><%=String.Format(Resources.Resource.HelpAnswerPasswordSettings, "<br />", "<b>", "</b>")%>
         <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
    </div>       

	   
</div>

<div id="studio_passwordSettingsInfo">
</div>

<div id="studio_passwordSettings">
<%-- <div style="margin-bottom:10px;">
        <%=Resources.Resource.PasswordDescription%>
    </div>--%>
    
    <div class="clearFix slider">
        <div class="title">
            <%= Resources.Resource.PasswordMinLength %></div>
        <div class="clearFix" style="margin-top:10px;">
            <div style="float: left;padding:2px 2px;border:1px solid #D1D1D1;">
                <div id="slider">
                </div>
            </div>
        <div style="float: left" id="count">
        </div>
        <div style="float: left" class="countLabel">
        <%= Resources.Resource.PasswordSymbolsCountLabel %>
        </div>
            </div>
    </div>
<div class="clearFix fieldsBox">
    <div class="upper clearFix">
        <input type="checkbox" id="chkUpperCase" />
        <label for="chkUpperCase"><%= Resources.Resource.PasswordUseUpperCase %></label>
    </div>
    <div class="digits">
        <input type="checkbox" id="chkDigits" />
        <label for="chkDigits"><%= Resources.Resource.PasswordUseDigits %></label>
    </div>
    <div class="spec">
        <input type="checkbox" id="chkSpecSymbols" />
        <label for="chkSpecSymbols"><%= Resources.Resource.PasswordUseSpecialSymbols %></label>
    </div>
</div>

<div class="btnBox clearFix">
        <a class="baseLinkButton" id="savePasswordSettingsBtn" href="javascript:void(0);">
            <%=Resources.Resource.SaveButton %></a>
    </div>
</div>

