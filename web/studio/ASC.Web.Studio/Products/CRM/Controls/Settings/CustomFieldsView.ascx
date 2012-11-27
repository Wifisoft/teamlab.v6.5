<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomFieldsView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Settings.CustomFieldsView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<a class="linkAddMediumText baseLinkAction" onclick="ASC.CRM.SettingsPage.showAddFieldPanel();" >
    <%= CRMSettingResource.AddNewField%>
</a>

<div id="manageField" style="display: none">
    <ascwc:Container ID="_manageFieldPopup" runat="server">
        <Header>
           <%= CRMSettingResource.AddNewField%>
        </Header>
        <Body>
            <dl>
                <dt></dt>
                <dd>
                    <div class="requiredField">
                        <span class="requiredErrorText"><%= CRMSettingResource.EmptyLabelError %></span>
                        <div class="headerPanelSmall headerBaseSmall" style="margin-bottom:5px;">
                            <%= CRMSettingResource.Label%>:
                        </div>
                        <input type="text" class="textEdit" maxlength="255"/>
                    </div>
                </dd>

                <dt><%= CRMSettingResource.Type %>:</dt>
                <dd>
                    <select onchange="ASC.CRM.SettingsPage.selectTypeEvent(this);" class="comboBox">
                        <option value="0">
                            <%= CRMSettingResource.TextField %>
                        </option>
                        <option value="1">
                            <%= CRMSettingResource.TextArea %>
                        </option>
                        <option value="2">
                            <%= CRMSettingResource.SelectBox%>
                        </option>
                        <option value="3">
                            <%= CRMSettingResource.CheckBox%></option>
                        <option value="4">
                            <%= CRMSettingResource.Heading%></option>
                        <option value="5">
                            <%= CRMSettingResource.Date%></option>
                    </select>
                </dd>
                <dt class="field_mask text_field" style="display: block;">
                    <%= CRMSettingResource.Size%>:
                </dt>
                <dd class="field_mask text_field" style="display: block;">
                    <input id="text_field_size" value="40" class="textEdit"/>
                </dd>
                <dt class="field_mask textarea_field">
                    <%= CRMSettingResource.Rows%>:
                </dt>
                <dd class="field_mask textarea_field">
                    <input id="textarea_field_rows" value="2" class="textEdit"/>
                </dd>
                <dt class="field_mask textarea_field">
                    <%= CRMSettingResource.Cols%>:
                </dt>
                <dd class="field_mask textarea_field">
                    <input id="textarea_field_cols" value="30" class="textEdit"/>
                </dd>
                <dt class="field_mask select_options">
                    <%= CRMSettingResource.SelectOptions%>:</dt>
                <dd class="field_mask select_options">
                    <ul>
                        <li style="display: none">
                            <input type="text" class="textEdit" maxlength="255"/>
                            <img alt="<%= CRMSettingResource.RemoveOption%>" title="<%= CRMSettingResource.RemoveOption%>"
                                src="<%= WebImageSupplier.GetAbsoluteWebPath("delete_small.png", ProductEntryPoint.ID ) %>"
                                onclick="jq(this).parent().remove()" />
                        </li>
                    </ul>
                    <span onclick="ASC.CRM.SettingsPage.toSelectBox(this)" title="<%= CRMSettingResource.AddOption%>"
                        id="addOptionButton" class="baseLinkAction">
                        <%= CRMSettingResource.AddOption%></span>
                </dd>
            </dl>
            <div class="h_line">
                <!--– –-->
            </div>
            <div class="action_block">
                <a class="baseLinkButton"><%= CRMSettingResource.AddThisField%></a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                     <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe"> <%= CRMSettingResource.AddFieldInProgressing %> </span>
                <br />
                <img alt="<%= CRMSettingResource.AddFieldInProgressing %>" title="<%= CRMSettingResource.AddFieldInProgressing %>" src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </ascwc:Container>
</div>


<ascwc:ViewSwitcher runat="server" ID="_switcherEntityType">
    <SortItems>
        <ascwc:ViewSwitcherLinkItem runat="server"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server"></ascwc:ViewSwitcherLinkItem>
    </SortItems>
</ascwc:ViewSwitcher>
<br />

<ul id="customFieldList" class="clearFix ui-sortable">
</ul>

<div id="emptyCustomFieldContent">
    <asp:PlaceHolder ID="_phEmptyContent" runat="server"></asp:PlaceHolder>
</div>

<div id="customFieldActionMenu" class="dropDownDialog" fieldid="">
    <div class="dropDownCornerRight">&nbsp;</div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.SettingsPage.showEditFieldPanel();"><%= CRMSettingResource.EditCustomField %></a>
        <a class="dropDownItem" onclick="ASC.CRM.SettingsPage.deleteField();"><%= CRMSettingResource.DeleteCustomField %></a>
    </div>
</div>

<script id="customFieldRowTemplate" type="text/x-jquery-tmpl">
{{if fieldType ==  3}}
    <li class="with-crm-menu">
        <table class="field_row" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_row_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="borderBase" style="width:200px;"></td>
                    <td class="borderBase">
                        <label>
                            {{tmpl "#customFieldTemplate"}}
                            <span class="customFieldTitle">${label}</span>
                        </label>
                    </td>
                    <td class="borderBase count_link_contacts grayText" style="width:100px;">
                        ${relativeItemsString}
                    </td>
                    <td class="borderBase" style="width:27px;">
                    {{if relativeItemsCount == 0 }}
                        <div id="fieldMenu_${id}" fieldid="${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>">
                        </div>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    {{/if}}
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
{{else fieldType ==  4}}
      <li class="expand_collapce_element with-crm-menu">
         <table class="field_row" cellspacing="0" cellpadding="0" border="0" width="100%">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_row_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="borderBase" style="padding-left: 10px">
                        {{tmpl "#customFieldTemplate"}}
                    </td>
                    <td class="borderBase count_link_contacts" style="width:100px;"></td>
                    <td class="borderBase" style="width:27px;">
                        <div id="fieldMenu_${id}" fieldid="${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>">
                        </div>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    </td>
                </tr>
             </tbody>
          </table>
       </li>
{{else}}
       <li class="with-crm-menu">
          <table class="field_row" cellspacing="0" cellpadding="0" border="0" width="100%">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_row_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="headerBaseSmall borderBase customFieldTitle" style="width:200px; padding-left: 10px">${label}</td>
                    <td class="borderBase">
                       {{tmpl "#customFieldTemplate"}}
                    </td>
                    <td class="borderBase count_link_contacts grayText" style="width:100px;">
                        ${relativeItemsString}
                    </td>
                    <td class="borderBase" style="width:27px;">
                    {{if relativeItemsCount == 0 }}
                        <div id="fieldMenu_${id}" fieldid="${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>">
                        </div>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    {{/if}}
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
{{/if}}
</script>

<script id="customFieldTemplate" type="text/x-jquery-tmpl">
{{if fieldType ==  0}}
    <input id="custom_field_${id}" name="custom_field_${id}" size="${maskObj.size}" type="text" class="textEdit">
{{else fieldType ==  1}}
    <textarea rows="${maskObj.rows}" cols="${maskObj.cols}" name="custom_field_${id}" id="custom_field_${id}"></textarea>
{{else fieldType ==  2}}
    <select class="comboBox" name="custom_field_${id}" id="custom_field_${id}" disabled="disabled">
      {{each maskObj}}
         <option value="${$value}">${$value}</option>
      {{/each}}
    </select>
{{else fieldType ==  3}}
          <input name="custom_field_${id}" id="custom_field_${id}" type="checkbox" style="vertical-align: middle;" disabled="disabled"/>
{{else fieldType ==  4}}
   <span id="custom_field_${id}" class="headerBase headerExpand customFieldTitle" onclick="ASC.CRM.SettingsPage.toggleCollapceExpand(this)">${label}</span>
{{else fieldType ==  5}}
  <input type="text" id="custom_field_${id}"  name="custom_field_${id}" class="textEdit textEditCalendar" />
{{/if}}
</script>

<script type="text/javascript" language="javascript">
    jq(function() {
        ASC.CRM.SettingsPage.init();
        ASC.CRM.SettingsPage.PopupWindowText = "<%= CRMSettingResource.AddNewField%>";
        ASC.CRM.SettingsPage.PopupSaveButtonText = "<%= CRMSettingResource.AddThisField%>";
    });
</script>