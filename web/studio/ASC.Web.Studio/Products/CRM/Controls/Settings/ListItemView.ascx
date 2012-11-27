<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListItemView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.ListItemView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<a id="addItem" class="linkAddMediumText baseLinkAction" onclick="ASC.CRM.ListItemView.showAddItemPanel();" >
    <%= AddPopupWindowText %>
</a>

<p style="margin-bottom: 10px;"><%= DescriptionText %></p>
<p style="margin-bottom: 30px;"><%= DescriptionTextEditDelete%></p>


<div id="manageItem" style="display: none">
    <ascwc:Container ID="_manageItemPopup" runat="server">
        <Header>
           <%= AddPopupWindowText %>
        </Header>
        <Body>
            <div class="clearFix" style="margin-bottom:10px;">
                <% if (CurrentTypeValue == ListType.TaskCategory || CurrentTypeValue == ListType.HistoryCategory) %>
                <% { %>
                    <div style="float: left;">
                        <img class="selectedIcon" alt="" title="" src="" />
                    </div>
                    <div style="padding-top: 10px">
                        <a class="linkAction change_icon" onclick="ASC.CRM.ListItemView.showIconsPanelToSelect();">
                            <span class="baseLinkAction">
                                <%= CRMSettingResource.ChangeIcon %>
                            </span>
                        </a>
                    </div>

                    <% if (CurrentTypeValue == ListType.TaskCategory) %>
                    <% { %>
                    <div id="popup_iconsPanel_<%= (int)CurrentTypeValue %>" class="iconsPanelSettings" style="width: 180px;height: 135px;">
                        <div class="popup-corner"></div>
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_call.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Call %>" title="<%= CRMTaskResource.TaskCategory_Call %>"/>
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_deal.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Deal %>" title="<%= CRMTaskResource.TaskCategory_Deal %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_demo.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Demo %>" title="<%= CRMTaskResource.TaskCategory_Demo %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_email.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Email %>" title="<%= CRMTaskResource.TaskCategory_Email %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_fax.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Fax %>" title="<%= CRMTaskResource.TaskCategory_Fax %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_follow_up.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_FollowUP %>" title="<%= CRMTaskResource.TaskCategory_FollowUP %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_lunch.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Lunch %>" title="<%= CRMTaskResource.TaskCategory_Lunch %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_meeting.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Meeting %>" title="<%= CRMTaskResource.TaskCategory_Meeting %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_note.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Note %>" title="<%= CRMTaskResource.TaskCategory_Note %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_ship.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Ship %>" title="<%= CRMTaskResource.TaskCategory_Ship %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_social_networks.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_SocialNetworks %>" title="<%= CRMTaskResource.TaskCategory_SocialNetworks %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_thank_you.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_ThankYou %>" title="<%= CRMTaskResource.TaskCategory_ThankYou %>" />
                    </div>
                    <% } %>
                    <% else %>
                    <% { %>
                    <div id="popup_iconsPanel_<%= (int)CurrentTypeValue %>" class="iconsPanelSettings" style="width: 90px; height: 90px;">
                        <div class="popup-corner"></div>
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_note.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Note %>" title="<%= CRMCommonResource.HistoryCategory_Note %>"/>
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_email.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Email %>" title="<%= CRMCommonResource.HistoryCategory_Email %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_call.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Call %>" title="<%= CRMCommonResource.HistoryCategory_Call %>" />
                        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_meeting.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Meeting %>" title="<%= CRMCommonResource.HistoryCategory_Meeting %>" />
                    </div>
                    <% } %>

                <% } %>
                <% else %>
                <% { %>
                    <div class="selectedColor">&nbsp;</div>
                    <a class="linkAction change_color" onclick="ASC.CRM.ListItemView.showColorsPanelToSelect();">
                        <span class="baseLinkAction">
                            <%= CRMSettingResource.ChangeColor %>
                        </span>
                    </a>
                    <div id="popup_colorsPanel">
                        <div class="popup-corner"></div>
                        <div class="style1" colorstyle="1"></div><div class="style2" colorstyle="2"></div><div class="style3" colorstyle="3"></div><div class="style4" colorstyle="4"></div><div class="style5" colorstyle="5"></div><div class="style6" colorstyle="6"></div><div class="style7" colorstyle="7"></div><div class="style8" colorstyle="8"></div>
                        <div class="style9" colorstyle="9"></div><div class="style10" colorstyle="10"></div><div class="style11" colorstyle="11"></div><div class="style12" colorstyle="12"></div><div class="style13" colorstyle="13"></div><div class="style14" colorstyle="14"></div><div class="style15" colorstyle="15"></div><div class="style16" colorstyle="16"></div>
                    </div>
                <% } %>
            </div>

            <div class="requiredField" style="margin-bottom:10px;">
                <span class="requiredErrorText"><%= CRMSettingResource.EmptyTitleError %></span>
                <div style="margin-bottom:5px;" class="headerPanelSmall headerBaseSmall">
                    <b><%= CRMSettingResource.TitleItem %>:</b>
                </div>
                <input type="text" class="textEdit" maxlength="255" />
            </div>

            <div>
                <div style="margin-bottom:5px;">
                    <b><%= CRMSettingResource.Description %>:</b>
                </div>
                <textarea rows="4" style="resize: none;"></textarea>
            </div>

            <div class="h_line"><!--– –--></div>
            <div class="action_block">
                <a class="baseLinkButton"><%= AddButtonText %></a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();"><%= CRMCommonResource.Cancel %></a>
            </div>
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe"> <%= AjaxProgressText%> </span>
                <br />
                <img alt="<%= AjaxProgressText %>" title="<%= AjaxProgressText %>" src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </ascwc:Container>
</div>


<ul id="listView">
</ul>


<script id="listItemsTmpl" type="text/x-jquery-tmpl">
    <li id="list_item_id_${id}" class="with-crm-menu">
        <table class="item_row" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_drag_handle borderBase">&nbsp;</div>
                    </td>

                    {{if ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3}}
                        <td class="borderBase" style="width:40px;">
                            <img class="currentIcon" alt="${imageAlt}" title="${imageTitle}" src="${imagePath}"
                                onclick="ASC.CRM.ListItemView.showIconsPanel(this);"/>
                            <div class="ajax_change_icon" alt="" title="" style="display: none;">&nbsp;</div>
                        </td>
                    {{else}}
                        <td class="borderBase" style="width:25px;">
                            <div class="currentColor" style="background:${color}" onclick="ASC.CRM.ListItemView.showColorsPanel(this);"
                                title="<%= CRMSettingResource.ChangeColor %>"></div>
                        </td>
                    {{/if}}

                    <td class="headerBaseSmall borderBase item_title" style="width:230px;">${title}</td>
                    <td class="borderBase item_description">
                        {{html Encoder.htmlEncode(description).replace(/&#10;/g, "<br/>") }}
                    </td>
                    <td class="borderBase count_link_contacts grayText" style="width:100px;">
                        ${relativeItemsString}
                    </td>
                    <td class="borderBase" style="width:27px;">
                    {{if relativeItemsCount == 0 }}
                        <div id="list_item_menu_${id}" listitemid="${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>"></div>
                        <div class="ajax_loader" alt="" title="" style="display: none;">&nbsp;</div>
                   {{/if}}
                   </td>
                </tr>
            </tbody>
        </table>
    </li>
</script>

<div id="listItemActionMenu" class="dropDownDialog" listitemid="">
    <div class="dropDownCornerRight">&nbsp;</div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.ListItemView.showEditItemPanel();"><%= EditText %></a>
        <a class="dropDownItem" onclick="ASC.CRM.ListItemView.deleteItem();"><%= DeleteText %></a>
    </div>
</div>

<% if (CurrentTypeValue == ListType.ContactStatus) %>
<% { %>
<div id="colorsPanel">
    <div class="popup-corner"></div>
    <div class="style1" colorstyle="1"></div><div class="style2" colorstyle="2"></div><div class="style3" colorstyle="3"></div><div class="style4" colorstyle="4"></div><div class="style5" colorstyle="5"></div><div class="style6" colorstyle="6"></div><div class="style7" colorstyle="7"></div><div class="style8" colorstyle="8"></div>
    <div class="style9" colorstyle="9"></div><div class="style10" colorstyle="10"></div><div class="style11" colorstyle="11"></div><div class="style12" colorstyle="12"></div><div class="style13" colorstyle="13"></div><div class="style14" colorstyle="14"></div><div class="style15" colorstyle="15"></div><div class="style16" colorstyle="16"></div>
</div>
<% } %>


<% if (CurrentTypeValue == ListType.TaskCategory) %>
<% { %>
<div id="iconsPanel_<%= (int)CurrentTypeValue %>" class="iconsPanelSettings" style="width: 180px;height: 135px;">
    <div class="popup-corner"></div>
    <img id="task_category_call" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_call.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Call %>" title="<%= CRMTaskResource.TaskCategory_Call %>"/>
    <img id="task_category_deal" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_deal.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Deal %>" title="<%= CRMTaskResource.TaskCategory_Deal %>" />
    <img id="task_category_demo" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_demo.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Demo %>" title="<%= CRMTaskResource.TaskCategory_Demo %>" />
    <img id="task_category_email" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_email.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Email %>" title="<%= CRMTaskResource.TaskCategory_Email %>" />
    <img id="task_category_fax" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_fax.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Fax %>" title="<%= CRMTaskResource.TaskCategory_Fax %>" />
    <img id="task_category_follow_up" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_follow_up.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_FollowUP %>" title="<%= CRMTaskResource.TaskCategory_FollowUP %>" />
    <img id="task_category_lunch" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_lunch.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Lunch %>" title="<%= CRMTaskResource.TaskCategory_Lunch %>" />
    <img id="task_category_meeting" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_meeting.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Meeting %>" title="<%= CRMTaskResource.TaskCategory_Meeting %>" />
    <img id="task_category_note" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_note.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Note %>" title="<%= CRMTaskResource.TaskCategory_Note %>" />
    <img id="task_category_ship" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_ship.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_Ship %>" title="<%= CRMTaskResource.TaskCategory_Ship %>" />
    <img id="task_category_social_networks" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_social_networks.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_SocialNetworks %>" title="<%= CRMTaskResource.TaskCategory_SocialNetworks %>" />
    <img id="task_category_thank_you" src="<%= WebImageSupplier.GetAbsoluteWebPath("task_category_thank_you.png", ProductEntryPoint.ID) %>"  alt="<%= CRMTaskResource.TaskCategory_ThankYou %>" title="<%= CRMTaskResource.TaskCategory_ThankYou %>" />
</div>
<% } %>
<% else if (CurrentTypeValue == ListType.HistoryCategory) %>
<% { %>
<div id="iconsPanel_<%= (int)CurrentTypeValue %>" class="iconsPanelSettings" style="width: 90px;height: 90px;">
    <div class="popup-corner"></div>
    <img id="event_category_note" src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_note.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Note %>" title="<%= CRMCommonResource.HistoryCategory_Note %>"/>
    <img id="event_category_email" src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_email.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Email %>" title="<%= CRMCommonResource.HistoryCategory_Email %>" />
    <img id="event_category_call" src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_call.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Call %>" title="<%= CRMCommonResource.HistoryCategory_Call %>" />
    <img id="event_category_meeting" src="<%= WebImageSupplier.GetAbsoluteWebPath("event_category_meeting.png", ProductEntryPoint.ID) %>"  alt="<%= CRMCommonResource.HistoryCategory_Meeting %>" title="<%= CRMCommonResource.HistoryCategory_Meeting %>" />
</div>
<% } %>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {
        ASC.CRM.ListItemView.init(<%= (int)CurrentTypeValue %>);
        ASC.CRM.ListItemView.PopupWindowEditButtonText = '<%= EditPopupWindowText %>';
        ASC.CRM.ListItemView.PopupWindowText = '<%= AddPopupWindowText %>';
        ASC.CRM.ListItemView.PopupSaveButtonText = '<%= AddButtonText %>';
    });
</script>