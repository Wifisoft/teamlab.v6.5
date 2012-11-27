<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.MailSettings" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
        
        <div id="studio_setInfSmtpSettingsInfo"></div>
        <div id="studio_smtpSettingsBox" class="clearFix">
        
            <div class="smtpModeBox clearFix">
                <div style="float:left;">
					<div class="clearFix">
						<input id="studio_corporateSMTPButton" name="studio_settingsSMTPSwitch" <%=!_isPesonalSMTP?"checked=\"checked\"":"" %> type="radio" />                
						<label for="studio_corporateSMTPButton" class="headerBaseSmall"><%=Resources.Resource.CorporateSMTP %></label>
                    </div>
                </div>
                
                <div style="float:left; margin-left:20px;">
					<div class="clearFix">
						<input id="studio_personalSMTPButton" name="studio_settingsSMTPSwitch" <%=_isPesonalSMTP?"checked=\"checked\"":"" %> type="radio" />                
						<label for="studio_personalSMTPButton" class="headerBaseSmall"><%=Resources.Resource.PersonalSMTP %></label>
                    </div>
                </div>
                
                <div style="float:right;"><a target="_blank" class="linkDescribe" href="<%=ASC.Web.Studio.UserControls.Management.StudioSettings.ModifyHowToAdress("howto.aspx#Admin_ConfigureMailSettings")%>"><%=Resources.Resource.Help%></a></div>
                
            </div>
          
            <%--Host and Port--%>
            <div class="clearFix" style="margin-top:40px;">
                <div class="headerBaseSmall" style="float:left; text-align:right; padding-right:10px; width:150px;">
                    <%=Resources.Resource.Host%>:                 
                </div>
                 <div style="float:left;">
                    <input class="textEdit studioHeaderBaseSmallValue" type="text" style="width: 250px;" id="studio_smtpAddress"
                        value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.Host??""%>" />
                </div>
                <div class="headerBaseSmall" style="float: left; margin-left: 20px; padding-right:10px;">
                    <%=Resources.Resource.Port%>:
               </div>
               <div style="float: left;">
                    <input class="textEdit studioHeaderBaseSmallValue" style="width: 50px;" type="text" id="studio_smtpPort" value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.Port??25%>" />
               </div>
               <%--Secure section--%>
               <div class="headerBaseSmall" style="float: left; margin-left: 20px; padding-right:10px;">
                    <%=Resources.Resource.EnableSSL%>:
               </div>
               <div style="float: left;" class="studioHeaderBaseSmallValue">
                    <input type="checkbox" id="studio_smtpEnableSSL" <%=ASC.Core.CoreContext.Configuration.SmtpSettings.EnableSSL ? "checked='checked'": ""%> />
               </div>
            </div>
            
            <%--SenderAddress--%>
            <div class="clearFix" style="margin-top:20px;">
                <div class="headerBaseSmall" style="float:left; text-align:right; padding-right:10px; width:150px;">
                    <%=Resources.Resource.SenderAddress%>:                
                </div>
                <div style="float:left;">
                    <input class="textEdit" type="text" style="width: 250px;" id="studio_smtpSenderEmail"
                        value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.SenderAddress??""%>" />
                </div>
            </div>
            
            <%--SenderDisplayName--%>
            <div class="clearFix" style="margin-top:20px;">   
             <div class="headerBaseSmall" style="float:left; text-align:right; padding-right:10px; width:150px;">            
                    <%=Resources.Resource.SenderDisplayName%>:
                </div>
                <div style="float:left;">
                    <input class="textEdit" style="width: 250px;" type="text" id="studio_smtpSenderDisplayName"
                        value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.SenderDisplayName??""%>" />
                </div>
            </div>
            
            <%--personal smtp--%>
            <div id="studio_smtp_personal" style="<%=_isPesonalSMTP?"":"display:none;"%>">
            
                <%--CredentialsDomain--%>
                <div class="clearFix" style="margin-top:20px;">                  
                 <div class="headerBaseSmall" style="float:left; text-align:right; padding-right:10px; width:150px;">                
                        <%=Resources.Resource.CredentialsDomain%>:
                  </div>
                    <div style="float:left;">
                        <input class="textEdit" style="width: 250px;" type="text" id="studio_smtpCredentialsDomain"
                            value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.CredentialsDomain??""%>" />
                    </div>
                </div>
                
                <%--CredentialsUserName--%>
                <div class="clearFix" style="margin-top:20px;">
                    <div class="headerBaseSmall" style="float:left; text-align:right; padding-right:10px; width:150px;">                                
                         <%=Resources.Resource.CredentialsUserName%>:                
                    </div>     
                    <div style="float:left;">
                            <input class="textEdit" style="width: 250px;" type="text" id="studio_smtpCredentialsUserName"
                                value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.CredentialsUserName??""%>" />
                     </div>                     
                 </div>
                    
                 <%--CredentialsUserPassword--%>
                 <div class="clearFix" style="margin-top:20px;">
                    <div class="headerBaseSmall" style="float:left; text-align:right; padding-right:10px; width:150px;"> 
                        <%=Resources.Resource.CredentialsUserPassword%>:   
                    </div>            
                   <div style="float:left;">
                            <input class="textEdit" style="width: 250px;" type="password" id="studio_smtpCredentialsUserPwd"
                                value="" />
                    </div>
                 </div>  
                
            </div>
                                  
             <div class="clearFix" style="margin-top:20px;">
                <a id="saveMailSettingsBtn" class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" style="float:left;" href="javascript:void(0);" ><%=Resources.Resource.SaveButton %></a>                
            </div>
            
            
             <div style="margin-top: 25px;">
             
                 <div class="headerBaseSmall" style="float:left; padding-right:10px;"> 
                        <%=Resources.Resource.RecipientAddress%>:
                 </div>
                    <div style="float:left;">
                        <input id="studio_smtpRecipientAddress" type="text" style="width: 250px;" class="textEdit" />
                    </div>
            
                    <a id="sendTestMailBtn" class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" style="float: left; margin-left:20px;" href="javascript:void(0);">
                        <%=Resources.Resource.SendTestMailButton %></a>
             </div>
        </div>
