<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DealMilestoneView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.DealMilestoneView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<a id="addDealMilestone" class="linkAddMediumText baseLinkAction" onclick="ASC.CRM.DealMilestoneView.showAddDealMilestonePanel();">
    <%= CRMSettingResource.AddNewDealMilestone%>
</a>

<p style="margin-bottom: 10px;"><%= CRMSettingResource.DescriptionTextDealMilestone %></p>
<p style="margin-bottom: 30px;"><%= CRMSettingResource.DescriptionTextDealMilestoneEditDelete %></p>

 <div id="manageDealMilestone" style="display: none">
    <ascwc:Container ID="_manageDealMilestonePopup" runat="server">
        <Header>
           <%= CRMSettingResource.AddNewDealMilestone%>
        </Header>
        <Body>
            <dl>
                <dt class="selectedColor">&nbsp;</dt>
                <dd>
                    <a class="linkAction change_color" onclick="ASC.CRM.DealMilestoneView.showColorsPanelToSelect();">
                        <span class="baseLinkAction">
                            <%= CRMSettingResource.ChangeColor %>
                        </span>
                    </a>
                    <div id="popup_colorsPanel">
                        <div class="popup-corner"></div>
                        <div class="style1" colorstyle="1"></div><div class="style2" colorstyle="2"></div><div class="style3" colorstyle="3"></div><div class="style4" colorstyle="4"></div><div class="style5" colorstyle="5"></div><div class="style6" colorstyle="6"></div><div class="style7" colorstyle="7"></div><div class="style8" colorstyle="8"></div>
                        <div class="style9" colorstyle="9"></div><div class="style10" colorstyle="10"></div><div class="style11" colorstyle="11"></div><div class="style12" colorstyle="12"></div><div class="style13" colorstyle="13"></div><div class="style14" colorstyle="14"></div><div class="style15" colorstyle="15"></div><div class="style16" colorstyle="16"></div>
                    </div>
                </dd>

                <dt></dt>
                <dd>
                    <div class="requiredField">
                        <span class="requiredErrorText"><%= CRMSettingResource.EmptyTitleError %></span>
                        <div class="headerPanelSmall headerBaseSmall" style="margin-bottom:5px;">
                            <%= CRMSettingResource.TitleItem %>:
                        </div>
                        <input type="text" class="textEdit title" maxlength="255"/>
                    </div>
                </dd>

                <dt><%= CRMSettingResource.Description %>:</dt>
                <dd>
                    <textarea rows="4" style="resize: none;"></textarea>
                </dd>

                <dt><%= CRMSettingResource.Likelihood %>:</dt>
                <dd>
                    <input type="text" class="textEdit probability" style="width: 30px;"  /> %
                </dd>

                <dt><%= CRMDealResource.DealMilestoneType%>:</dt>
                <dd>
                    <ul>
                        <li>
                            <input type="radio" id="dealMilestoneStatusOpen" name="deal_milestone_status" value="<%= (Int32)DealMilestoneStatus.Open %>" />
                            <label for="dealMilestoneStatusOpen"><%=DealMilestoneStatus.Open.ToLocalizedString() %></label>
                        </li>
                        <li>
                            <input type="radio" id="dealMilestoneStatusClosedAndWon" name="deal_milestone_status" value="<%= (Int32)DealMilestoneStatus.ClosedAndWon %>"
                                onclick="javascript:jq('#manageDealMilestone .probability').val('100');"/>
                            <label for="dealMilestoneStatusClosedAndWon"><%=DealMilestoneStatus.ClosedAndWon.ToLocalizedString()%></label>
                        </li>
                        <li>
                            <input type="radio" id="dealMilestoneStatusClosedAndLost" name="deal_milestone_status" value="<%= (Int32)DealMilestoneStatus.ClosedAndLost %>"
                                onclick="javascript:jq('#manageDealMilestone .probability').val('0');"/>
                            <label for="dealMilestoneStatusClosedAndLost"><%=DealMilestoneStatus.ClosedAndLost.ToLocalizedString()%></label>
                        </li>
                    </ul>
                </dd>
            </dl>
            <div class="h_line"><!--– –--></div>
            <div class="action_block">
                <a class="baseLinkButton"><%= CRMSettingResource.AddThisDealMilestone%></a>
                <span class="splitter"></span>
                <a class="grayLinkButton"
                    onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                    <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe"> <%= CRMSettingResource.AddDealMilestoneInProgressing%> </span>
                <br />
                <img alt="<%= CRMSettingResource.AddDealMilestoneInProgressing %>"
                    title="<%= CRMSettingResource.AddDealMilestoneInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </ascwc:Container>
</div>


<ul id="dealMilestoneList">
</ul>


<script id="dealMilestoneTmpl" type="text/x-jquery-tmpl">
<li id="deal_milestone_id_${id}" class="with-crm-menu">
    <table class="deal_milestone_row" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="" style="width:6px; background: #FFFFFF">
                    <div class="sort_drag_handle borderBase">&nbsp;</div>
                </td>
                <td class="borderBase" style="width:25px;">
                    <div class="currentColor" style="background:${color}" onclick="ASC.CRM.DealMilestoneView.showColorsPanel(this);"></div>
                </td>
                <td class="headerBaseSmall borderBase deal_milestone_title" style="width:200px;">
                    ${title}
                </td>
                <td class="borderBase">
                    {{html Encoder.htmlEncode(description).replace(/&#10;/g, "<br/>") }}
                <td class="borderBase" style="width:60px; text-align: center;">
                      ${successProbability}%
                </td>
                <td class="borderBase count_link_contacts grayText" style="width:100px;">
                    ${relativeItemsString}
                </td>
                <td class="borderBase" style="width:27px;">
                {{if relativeItemsCount == 0 }}
                    <div id="deal_milestone_menu_${id}" dealmilestoneid="${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>"></div>
                    <div class="ajax_loader" alt="" title="" style="display: none;">&nbsp;</div>
                {{/if}}
                </td>
            </tr>
        </tbody>
    </table>
</li>
</script>

<div id="dealMilestoneActionMenu" class="dropDownDialog" dealmilestoneid="">
    <div class="dropDownCornerRight">&nbsp;</div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.DealMilestoneView.showEditDealMilestonePanel();"><%= CRMSettingResource.EditDealMilestone%></a>
        <a class="dropDownItem" onclick="ASC.CRM.DealMilestoneView.deleteDealMilestone();"><%= CRMSettingResource.DeleteDealMilestone%></a>
    </div>
</div>

<div id="colorsPanel">
    <div class="popup-corner"></div>
    <div class="style1" colorstyle="1"></div><div class="style2" colorstyle="2"></div><div class="style3" colorstyle="3"></div><div class="style4" colorstyle="4"></div><div class="style5" colorstyle="5"></div><div class="style6" colorstyle="6"></div><div class="style7" colorstyle="7"></div><div class="style8" colorstyle="8"></div>
    <div class="style9" colorstyle="9"></div><div class="style10" colorstyle="10"></div><div class="style11" colorstyle="11"></div><div class="style12" colorstyle="12"></div><div class="style13" colorstyle="13"></div><div class="style14" colorstyle="14"></div><div class="style15" colorstyle="15"></div><div class="style16" colorstyle="16"></div>
</div>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {
        ASC.CRM.DealMilestoneView.init();
        ASC.CRM.DealMilestoneView.PopupWindowEditButtonText = '<%= CRMSettingResource.EditSelectedDealMilestone.ReplaceSingleQuote() %>';
        ASC.CRM.DealMilestoneView.PopupWindowText = '<%= CRMSettingResource.AddNewDealMilestone.ReplaceSingleQuote() %>';
        ASC.CRM.DealMilestoneView.PopupSaveButtonText = '<%= CRMSettingResource.AddThisDealMilestone.ReplaceSingleQuote() %>';

    });
</script>


