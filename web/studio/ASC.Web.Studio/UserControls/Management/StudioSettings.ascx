<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudioSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.StudioSettings" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
        <%--side panel settings--%>
        <asp:PlaceHolder ID="_studioViewSettingsHolder" runat="server">
            <div class="headerBase borderBase" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
                border-top: none; border-right: none; border-left: none;">
                <%=Resources.Resource.StudioVisualSettings%>
            </div>
            <div id="studio_setInfStudioViewSettingsInfo">
            </div>
            <div id="studio_studioViewSettingsBox" style="padding: 20px 15px;">
                <div class="clearFix">
                   <div style="float:left;">
                    <input id="studio_sidePanelLeftViewType" type="radio" <%=(_studioViewSettings.LeftSidePanel?"checked=\"checked\"":"") %> name="sidePanelViewType" />
                    </div>
                    <div style="float:left; margin-top:3px; margin-left:5px;">
                    <label for="studio_sidePanelLeftViewType"><%=Resources.Resource.SidePanelLeftView%></label>
                    </div>
                    <div style="float:left; margin-left:20px;">
                    <input id="studio_sidePanelRightViewType" type="radio" <%=(!_studioViewSettings.LeftSidePanel?"checked=\"checked\"":"") %> name="sidePanelViewType" />
                    </div>
                    <div style="float:left; margin-top:3px; margin-left:5px;">
                    <label for="studio_sidePanelRightViewType"><%=Resources.Resource.SidePanelRightView%></label>
                    </div>
               </div>
                <div class="clearFix" style="margin-top: 20px;">
                    <a class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" style="float: left;" onclick="StudioManagement.SaveStudioViewSettings();"
                        href="javascript:void(0);">
                        <%=Resources.Resource.SaveButton %></a>
                </div>
            </div>
        </asp:PlaceHolder>
        
     
        
        <%--portal settings--%>
        <asp:PlaceHolder ID="_portalSettingsHolder" runat="server"></asp:PlaceHolder>         
         
         <%--timezone & language--%>
        <div class="headerBase borderBase clearFix" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
                border-top: none; border-right: none; border-left: none;">
                <div style="float:left;">
                <%=Resources.Resource.StudioTimeLanguageSettings%>
                </div>
                <div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpLngTime'});" title="<%=Resources.Resource.HelpQuestionLngTimeSettings%>"></div> 
                <div class="popup_helper" id="AnswerForHelpLngTime">
                    <p><%=String.Format(Resources.Resource.HelpAnswerLngTimeSettings, "<br />","<b>","</b>")%>
                    <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
                </div>      
         
         </div>
         
         <div id="studio_lngTimeSettingsInfo">            
         </div>
         <div id="studio_lngTimeSettingsBox" style="padding:20px 15px;"> 
            <asp:PlaceHolder ID="_timelngHolder" runat="server"></asp:PlaceHolder>
            
            <div class="clearFix" style="margin-top: 20px;">
                    <a class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" style="float: left;" onclick="StudioManagement.SaveLanguageTimeSettings();"
                        href="javascript:void(0);">
                        <%=Resources.Resource.SaveButton %></a>
                </div>
         </div> 
         
         
         <%--DNS settings--%>
         <% if (!ASC.Core.CoreContext.Configuration.Standalone) { %>
        <asp:PlaceHolder ID="_dnsSettingsHolder" runat="server">
            <div id="dnsSettings" class="<%=EnableDomain ? "" : "disable" %> ">
                <div class="headerBase borderBase clearFix" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px; border-top: none; border-right: none; border-left: none;">
				    <div style="float: left;"><%=Resources.Resource.DnsSettings%></div>
				    <div class="HelpCenterSwitcher title" id="HelpQuetionDNSSettings" onclick="" title="<%=Resources.Resource.HelpQuestionDNSSettings%>"></div> 
				    <%if(!EnableDomain){%>
                      <div class="disable-trial-version">
                          <span class="text"><%=String.Format(Resources.Resource.DisableTrialVersion, "<span class=\"text-orange\">", "</span>")%></span>
                          <a class="link" href="/management.aspx?type=4"><%=Resources.Resource.ViewTariffPlans %></a>
                      </div>                          
                     <% } %>
                    <div class="popup_helper" id="HelpAnswerDNSSettings">
                     <p><%=String.Format(Resources.Resource.HelpAnswerDNSSettings, "<br />", "<b>", "</b>")%>
                     <a href="http://teamlab.com/help/gettingstarted/administration.aspx#General" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
                    </div>        
	            </div>
                <div id="studio_enterDnsBox" style="padding: 20px 15px;">
				    <%if (!ASC.Core.CoreContext.Configuration.Standalone) { %>
					    <div style="padding: 3px 0px;">
							    <div style="margin:3px 0;">
							    <label for="studio_TenantAlias">
								    <%=Resources.Resource.PortalAddress%></label>
							    </div>
							    <input type="text" class="textEdit" maxlength="150" style="width: 323px;" id="studio_TenantAlias" value="<%=ASC.Core.CoreContext.TenantManager.GetCurrentTenant().TenantAlias??string.Empty%>" <%=EnableDomain ? "" : "disabled='disabled'" %> />
							    <span id="studio_TenantBaseDomain"><%=TenantBaseDomain%></span>
					    </div>
                    <%} %>
                    <div class="clearFix" style="padding: 3px 0px;">
                            <div class="clearFix" style="margin:3px 0;">
							    <input type="checkbox" id="studio_enableDnsName" onclick="jq('#studio_dnsName').attr('disabled', !this.checked);" style="margin-left: 0; float: left;" <%=EnableDnsChange ? "checked='checked'" : "" %> <%=EnableDomain ? "" : "disabled='disabled'" %> />
							    <label for="studio_enableDnsName" style="float: left; margin-top: 2px; margin-left: 2px;" onselectstart="return false;" onmousedown="return false;" ondblclick="return false;">
								    <%=Resources.Resource.CustomDomainName%>
							    </label>
                            </div>
                            <div class="clearFix">
							    <input type="text" class="textEdit" maxlength="150" style="width: 400px;" id="studio_dnsName" value="<%=ASC.Core.CoreContext.TenantManager.GetCurrentTenant().MappedDomain??string.Empty%>" <%=(EnableDnsChange && EnableDomain) ? "" : "disabled='disabled'" %>/>
                            </div>
                    </div>
                    <div class="clearFix" style="margin-top: 20px;">
                        <a  class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %> <%= EnableDomain ? "" : " disabled" %>" onclick="<%=EnableDomain ? "StudioManagement.SaveDnsSettings();" : "" %>" style="float: left;" href="javascript:void(0);">
                            <%=Resources.Resource.SaveButton %></a>
                    </div>
                    <p id="dnsChange_sent" style="display:none;"></p>
                </div>
            </div>
        </asp:PlaceHolder>  
        <%} %>
        
        <%--version settings--%>
        <asp:PlaceHolder ID="_portalVersionSettings" runat="server"></asp:PlaceHolder>
         
        <%--trusted mail domain--%>
         <asp:PlaceHolder ID="_mailDomainSettings" runat="server"></asp:PlaceHolder>
         
            <%--notify bar settings--%>
        <asp:PlaceHolder ID="_studioNotifyBarSettingsHolder" runat="server">
            
        </asp:PlaceHolder>
        
        <%-- strong security password --%> 
         <asp:PlaceHolder ID="_strongPasswordSettings" runat="server"></asp:PlaceHolder>
         
         <%-- restricted access to portal --%> 
         <asp:PlaceHolder ID="_restrictedAccessSettings" runat="server"></asp:PlaceHolder>

         <asp:PlaceHolder ID="invLink" runat="server"></asp:PlaceHolder>

         <asp:PlaceHolder ID="_smsValidationSettings" runat="server"></asp:PlaceHolder>
         
         <asp:PlaceHolder ID="_admMessSettings" runat="server"></asp:PlaceHolder>

                 