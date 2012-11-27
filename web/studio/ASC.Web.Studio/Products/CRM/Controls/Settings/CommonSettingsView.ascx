<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonSettingsView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Settings.CommonSettingsView" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<script type="text/javascript">
	jq(document).ready(function() {
		<% if (!MobileVer) %>
        <% { %>
		ASC.CRM.SettingsPage.checkExportStatus(true);
		<% } %>
		jq.forceIntegerOnly("#tbxPort");
		<% if(Settings != null) %>
		<% { %>
		jq("#tbxHost").val("<%= Settings.Host %>");
        jq("#tbxPort").val("<%= Settings.Port %>");
        jq("#tbxHostLogin").val("<%= Settings.HostLogin %>");
        jq("#tbxHostPassword").val("<%= Settings.HostPassword %>");
		jq("#tbxSenderDisplayName").val("<%= Settings.SenderDisplayName %>");
        jq("#tbxSenderEmailAddress").val("<%= Settings.SenderEmailAddress %>");
        jq("#cbxEnableSSL").attr("checked", <%= Settings.EnableSSL.ToString().ToLower() %>);
		    <% if(Settings.RequiredHostAuthentication) %>
		    <% { %>
		        jq("#cbxAuthentication").attr("checked", true);
		        jq("#tbxHostLogin").removeAttr("disabled");
	            jq("#tbxHostPassword").removeAttr("disabled");
		    <% } %>
		    <% else %>
		    <% { %>
		        jq("#cbxAuthentication").attr("checked", false);
		        jq("#tbxHostLogin").attr("disabled", true);
	            jq("#tbxHostPassword").attr("disabled", true);
		    <% } %>
		<% } %>
	});
</script>

<div class="headerBase settingsHeader">
    <%=CRMSettingResource.SMTPSettings%>
</div>
<div style="padding: 0 15px" id="smtpSettingsContent">
    <p>
        <%= CRMSettingResource.SMTPSettingsDescription%>
    </p>
    <table cellpadding="5" cellspacing="0">
        <tr>
            <td>
                <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.Host%>:</div>
                <input type="text" class="textEdit" style="width: 200px;" id="tbxHost"/>
            </td>
            <td>
                <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.Port%>:</div>
                <div>
                    <input type="text" class="textEdit" style="width: 50px; float: left;" id="tbxPort" maxlength="5"/>
                    <input id="cbxAuthentication" type="checkbox" style="margin:4px 6px 0 10px; float: left;" onchange="ASC.CRM.SettingsPage.changeAuthentication()">
                    <label for="cbxAuthentication" class="headerBaseSmall" style="line-height: 21px;"><%=CRMSettingResource.Authentication%></label>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.HostLogin%>:</div>
                <input type="text" class="textEdit" style="width: 200px;" id="tbxHostLogin"/>
            </td>
            <td>
                <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.HostPassword%>:</div>
                <input type="password" class="textEdit" style="width: 200px;" id="tbxHostPassword"/>
            </td>
        </tr>
        <tr>
            <td>
                <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.SenderDisplayName%>:</div>
                <input type="text" class="textEdit" style="width: 200px;" id="tbxSenderDisplayName"/>
            </td>
            <td>
                <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.SenderEmailAddress%>:</div>
                <input type="text" class="textEdit" style="width: 200px;" id="tbxSenderEmailAddress"/>
            </td>
        </tr>
        <tr>
            <td>
                <input id="cbxEnableSSL" type="checkbox" style="margin-left: 0px; float: left; margin-right: 6px;">
                <label for="cbxEnableSSL" class="headerBaseSmall" style="float: left; line-height: 20px;">
                    <%=CRMSettingResource.EnableSSL%>
                </label>
            </td>
            <td></td>
        </tr>
    </table>
    <div style="margin: 10px 0 0 5px">
        <a href="javascript:void(0);" onclick="ASC.CRM.SettingsPage.saveSMTPSettings();"
            class="baseLinkButton"><%=CRMCommonResource.Save%></a>
    </div>
</div>

<div class="headerBase settingsHeader">
    <%=CRMSettingResource.CurrencySettings%>
</div>
<div style="padding: 0 15px">
    <p>
        <%= CRMSettingResource.CurrencySettingsDescription%>
    </p>
    <div class="headerBaseSmall headerTitle">
        <%= CRMSettingResource.DefaultCurrency%>:
    </div>
    <div>
        <select id="defaultCurrency" name="defaultCurrency" onchange="ASC.CRM.SettingsPage.changeDefaultCurrency(this)" class="comboBox">
            <% foreach (var keyValuePair in AllCurrencyRates)%>
            <% { %>
            <option value="<%= keyValuePair.Abbreviation %>" <%=  String.Compare(keyValuePair.Abbreviation,  Global.TenantSettings.DefaultCurrency.Abbreviation, true) == 0 ? "selected=selected" : String.Empty %>>
                <%=String.Format("{0} - {1}", keyValuePair.Abbreviation, keyValuePair.Title)%></option>
            <% } %>
        </select>
        <img style="display:none;" src="<%= WebImageSupplier.GetAbsoluteWebPath("loader_12.gif") %>" width="12px" height="12px"/>        
        <span style="display:none;" class="textMediumDescribe"><%= CRMSettingResource.SaveCompleted%></span>
    </div>
</div>

<% if (!MobileVer) %>
<% { %>
<div class="headerBase settingsHeader">
   <%= CRMSettingResource.ExportData %>
</div>
<div id="exportDataContent">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr valign="top">
            <td>
                <img src="<%= WebImageSupplier.GetAbsoluteWebPath("export_data.png", ProductEntryPoint.ID) %>" />
            </td>
            <td>
                <p style="margin-top: 5px;">
                    <%= CRMSettingResource.ExportDataSettingsInfo %>
                </p>
                <a class="baseLinkButton" onclick="ASC.CRM.SettingsPage.startExportData()">
                    <%= CRMSettingResource.DownloadAllDataInZipArchive %></a>
                <div class="clearFix">
                    <div style="float: right;margin-left: 10px;">
                        <a class="grayLinkButton" onclick="ASC.CRM.SettingsPage.abortExport()" id="abortButton" style="width: 90px;">
                            <%= CRMSettingResource.AbortExport %>
                        </a>
                        <a class="grayLinkButton" onclick="ASC.CRM.SettingsPage.closeExportProgressPanel()" id="okButton" style="display: none;width: 90px;">
                            <%= CRMCommonResource.OK %>
                        </a>
                    </div>
                    <div class="progress_box">
                        <div class="progress" style="width: 0%"></div>
                        <span class="percent">0%</span>
                    </div>
                </div>
                <p style="display: none;" class="headerBaseSmall">
                    <%= CRMSettingResource.DownloadingAllDataInZipArchive %>
                </p>
                <div id="exportErrorBox" class="clearFix" style="margin-top:10px; display:none;">
                    <div style="float:left"><%= CRMContactResource.MassSendErrors %>:</div>
                    <div class="progressErrorBox" style="float: left; margin-left: 10px;"></div>
                </div>
                <div id="exportLinkBox" class="clearFix" style="margin-top:10px; display:none;">
                    <div style="float:left"><%= CRMSettingResource.DownloadLinkText %>:</div>
                    <span style="float: left; margin-left: 10px;"></span>
                </div>
            </td>
        </tr>
    </table>
</div>
<% } %>