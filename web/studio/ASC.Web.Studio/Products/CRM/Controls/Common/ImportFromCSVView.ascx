<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportFromCSVView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Common.ImportFromCSVView" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>

<script type="text/javascript" language="javascript">
jq(function() {
	
	jq("#delimiterCharacterSelect,#encodingSelect, #quoteCharacterSelect").tlcombobox();
	
	AjaxPro.Utils.ImportFromCSV.GetStatus(<%=(int)EntityType%>, function(res) {
		if (res.error != null) {
			alert(res.error.Message);
			return;
		}
		if (res.value == null || res.value.IsCompleted) {
			ASC.CRM.ImportContacts.init(<%=(int)EntityType%>);
        } else {
			jq("#importFromCSVSteps").hide();
			jq("#importStartedFinalMessage").show();
			ASC.CRM.ImportContacts.checkImportStatus(true);
		}
	});
	
});
</script>

<br />
<dl id="importFromCSVSteps">
    <dt>1</dt>
    <dd>
        <span class="headerBase">
            <%= ImportFromCSVStepOneHeaderLabel %>
        </span>
        <br />
        <span class="textBigDescribe">
            <%=  ASC.Web.Studio.Core.FileSizeComment.GetFileSizeNote(true)%>
        </span>
        <br />
        <br />
        <span class="textBigDescribe">
            <%= String.Format(ImportFromCSVStepOneDescriptionLabel, ASC.Web.CRM.Classes.ImportFromCSV.GetQuotas())%></span>
        <div style="width: 700px">
            <br />
            <span style="display: none;"></span><a id="uploadCSVFile" class="import_button baseLinkAction"
                style="border-bottom-color: #646567;">
                <%= CRMJSResource.SelectCSVFileButton%></a>
            <br />
            <br />
            <div>
                <span style="font-weight: bold;">
                    <%= CRMCommonResource.ImportFromCSV_ReadFileSettings_Header%>:</span>
                <br />
                <span class="textBigDescribe">
                    <%= CRMCommonResource.ImportFromCSV_ReadFileSettings_Description%>
                </span>
                <br />
                <div>
                    <span>
                        <%= CRMCommonResource.ImportFromCSV_ReadFileSettings_DelimiterCharacter%></span>:
                    <select id="delimiterCharacterSelect">
                        <option selected="selected" value="<%=(int)","[0] %>"> <%= CRMCommonResource.Comma%></option>
                        <option value="<%= (int)";"[0] %>"> <%= CRMCommonResource.Semicolon%></option>
                        <option value="<%= (int)":"[0] %>"> <%= CRMCommonResource.Colon%></option>
                        <option value="<%= (int)"\t"[0] %>" >
                            <%= CRMCommonResource.Tabulation%></option>
                        <option value="<%= (int)" "[0] %>">
                            <%= CRMCommonResource.Space%></option>
                    </select>
                </div>
                <div>
                    <span>
                        <%= CRMCommonResource.ImportFromCSV_ReadFileSettings_Encoding%></span>:
                    <select id="encodingSelect">
                        <% foreach (var encoding in Encoding.GetEncodings()) %>
                        <%{%>
                        
                        <% if (encoding.CodePage == 65001)%>
                        <%{%>
                         <option selected="selected" value='<%=encoding.CodePage%>'>
                            <%=encoding.DisplayName%></option>
                        <%}%>
                        <%else%>
                        <%{%>
                         <option value='<%=encoding.CodePage%>'>
                            <%=encoding.DisplayName%></option>
                        <%}%>
                       
                        <%}%>
                    </select>
                </div>
                <div>
                    <span><%= CRMCommonResource.ImportFromCSV_ReadFileSettings_QuoteCharacter%></span>:
                      <select id="quoteCharacterSelect">
                           <option selected="selected" value="<%= (int)"\""[0] %>"> <%= CRMCommonResource.DoubleQuote%></option>
                           <option value="<%= (int)"'"[0] %>"> <%= CRMCommonResource.SingleQuote%></option>
                    </select>
                </div>
            </div>
            <div id="removingDuplicatesBehaviorPanel" style="display: none;">
                 <br />
                <span style="font-weight: bold;">
                    <%= CRMCommonResource.DuplicateRecords %>:</span>
                <br />
                <span class="textBigDescribe">
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_Description%>
                </span>
                <br />
                <br />
                <label>
                    <input type="radio" name="removingDuplicatesBehavior" value="1" />
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_Skip%>
                </label>
                <div title="<%= CRMCommonResource.ShowInformation %>" onclick="jq(this).helper({ BlockHelperID: 'SkipDescription'});"
                    class="HelpCenterSwitcher">
                </div>
                <div id="SkipDescription" class="popup_helper">
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_SkipDescription%>
                    <div class="pos_top">
                    </div>
                </div>
                <br />
                <label>
                    <input type="radio" name="removingDuplicatesBehavior" value="2" />
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_Overwrite%>
                </label>
                <div title="<%= CRMCommonResource.ShowInformation %>" onclick="jq(this).helper({ BlockHelperID: 'OverwriteDescription'});"
                    class="HelpCenterSwitcher">
                </div>
                <div id="OverwriteDescription" class="popup_helper">
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_OverwriteDescription%>
                    <div class="pos_top">
                    </div>
                </div>
                <br />
                <label>
                    <input type="radio" name="removingDuplicatesBehavior" checked="checked" value="3" />
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_Clone%>
                </label>
                <div title="<%= CRMCommonResource.ShowInformation %>" onclick="jq(this).helper({ BlockHelperID: 'CloneDescription'});"
                    class="HelpCenterSwitcher">
                </div>
                <div id="CloneDescription" class="popup_helper">
                    <%= CRMCommonResource.ImportFromCSV_DublicateBehavior_CloneDescription%>
                    <div class="pos_top">
                    </div>
                </div>
            </div>
            <br />
            <label>
                <input type="checkbox" id="ignoreFirstRow" checked="checked" />
                <%= CRMCommonResource.IgnoreFirstRow%>
            </label>
            <br />
            <br />
            <asp:PlaceHolder runat="server" ID="_phPrivatePanel"></asp:PlaceHolder>
            <div class="h_line">
                <!--– –-->
            </div>
            <div class="action_block">
                <a class="disableLinkButton" href="javascript:void(0)" onclick="ASC.CRM.ImportContacts.startUploadCSVFile()">
                    <%= CRMContactResource.Continue%>
                </a><span class="splitter"></span><a href="default.aspx" class="grayLinkButton">
                    <%= CRMCommonResource.Cancel%></a>
            </div>
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe">
                    <%= CRMCommonResource.ImportFromCSVStepOneProgressLable %></span>
                <br />
                <img src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </div>
    </dd>
    <dt style="display: none">2</dt>
    <dd style="display: none">
        <span class="headerBase">
            <%= ImportFromCSVStepTwoHeaderLabel %></span><br />
        <span class="textBigDescribe">
            <%= String.Format(ImportFromCSVStepTwoDescriptionLabel, System.DateTimeExtension.DateFormatPattern)%></span>
        <table id="columnMapping" cellspacing="0" cellpadding="10" class="tableBase">
            <thead>
                <tr>
                    <td style="width: 20%">
                        <%= CRMCommonResource.Column %>:
                    </td>
                    <td style="width: 20%">
                        <%= CRMCommonResource.AssignedField %>:
                    </td>
                    <td style="width: 60%">
                       <div>
                        <span style="float: left;">
                            <%= CRMCommonResource.SampleValues %>: 
                        </span><span style="float: right;"><a id="prevSample"
                                class="linkAction" href="javascript:void(0)" onclick="javascript:ASC.CRM.ImportContacts.getPrevSampleRow()"
                                style="display: none">
                                <%= CRMCommonResource.PrevSample %></a><span class="splitter" style="display: none;">|</span><a
                                    class="linkAction" href="javascript:void(0)" onclick="javascript:ASC.CRM.ImportContacts.getNextSampleRow();"
                                    id="nextSample"><%=CRMCommonResource.NextSample%></a> </span>
                       </div>
                    </td>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
        <div class="h_line">
            <!--– –-->
        </div>
        <div class="action_block">
            <a class="baseLinkButton" href="javascript:void(0)" onclick="ASC.CRM.ImportContacts.startImport()">
                <%= StartImportLabel %></a><span class="splitter"></span> <a onclick="ASC.CRM.ImportContacts.prevStep(0)"
                    class="grayLinkButton">
                    <%= CRMCommonResource.PrevStep%></a> <span class="splitter"></span><a href="default.aspx"
                        class="grayLinkButton">
                        <%= CRMCommonResource.Cancel%></a>
        </div>
        <div style="display: none;" class="ajax_info_block">
            <span class="textMediumDescribe">
                <%= CRMCommonResource.ImportFromCSVStepTwoProgressLable %></span>
            <br />
            <img src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
        </div>
    </dd>
</dl>
<select id="columnSelectorBase" name="columnSelectorBase" style="display: none;"
    class="comboBox">
</select>
<div id="importStartedFinalMessage" style="display: none;">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr valign="top">
            <td>
                <img src="<%=ImportImgSrc%>" />
            </td>
            <td>
                <span class="headerBase">
                    <%= ImportStartingPanelHeaderLabel%>
                </span>
                <p class="headerBaseSmall">
                    <%= ImportStartingPanelDescriptionLabel %>
                </p>
                <div class="progress_box">
                    <div class="progress" style="width: 0%">
                    </div>
                    <span class="percent">0%</span>
                </div>
                <div id="importErrorBox" class="clearFix" style="margin-top: 20px; display: none;">
                    <div style="float: left">
                        <%= CRMContactResource.MassSendErrors %>:</div>
                    <div class="progressErrorBox" style="float: left; margin-left: 10px;">
                    </div>
                </div>
                <div id="importLinkBox" class="clearFix" style="margin-top: 20px;">
                    <a href="<%= GoToRedirectURL %>" class="baseLinkButton">
                        <%= ImportStartingPanelButtonLabel%></a>
                </div>
            </td>
        </tr>
    </table>
</div>

<script id="previewImportDataTemplate" type="text/x-jquery-tmpl">
   <tr>
            <td class="borderBase">
              <input type="checkbox" checked="checked" />   
            </td>
            <td class="borderBase">
              <img src="${default_image}">
            </td>
            <td style="width:100%" class="borderBase">
                  ${contact_title}
            </td>
   </tr>
</script>

<script id="columnSelectorTemplate" type="text/x-jquery-tmpl">
   {{if isHeader}}         
       <option name="is_header">${title}</option>
   {{else !isHeader}}
        <option name="${name}" >${title}</option>
   {{/if}} 
</script>

<script id="columnMappingTemplate" type="text/x-jquery-tmpl">  
     {{each firstContactFields}}
      <tr>
        <td class="borderBase">
          ${headerColumns[$index]}
        </td>
        <td class="borderBase">         
         {{html $item.renderSelector($index)}}
        </td>
        <td class="borderBase">
           ${$value}
        </td>
       </tr> 
     {{/each}}  
</script>

