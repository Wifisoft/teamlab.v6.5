<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="TagSettingsView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.TagSettingsView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<div>
    <a id="addTag" class="linkAddMediumText baseLinkAction" onclick="ASC.CRM.TagSettingsView.showAddTagPanel();" >
        <%= CRMSettingResource.AddNewTag%>
    </a>
    <a class="crm_deleteLinkButton" id="deleteUnusedTagsButton" onclick="ASC.CRM.TagSettingsView.deleteUnusedTags();">
        <%= CRMSettingResource.DeleteUnusedTags %>
    </a>
</div>

<div id="manageTag" style="display: none">
    <ascwc:Container ID="_manageTagPopup" runat="server">
        <Header>
           <%= CRMSettingResource.AddNewTag%>
        </Header>
        <Body>
            <div class="requiredField">
                <span class="requiredErrorText"><%= CRMSettingResource.EmptyLabelError %></span>
                <div class="headerPanelSmall headerBaseSmall" style="margin-bottom:5px;">
                    <%= CRMSettingResource.Label%>:
                </div>
                <input id="tagTitle" type="text" class="textEdit" style="width:100%" maxlength="50"/>
            </div>

            <div class="h_line"><!--– –--></div>
            <div class="action_block">
                <a class="baseLinkButton" onclick="ASC.CRM.TagSettingsView.createTag();"><%= CRMSettingResource.AddThisTag%></a>
                <span class="splitter">&nbsp;</span>
                <a class="grayLinkButton" onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                     <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe"> <%= CRMSettingResource.AddTagInProgressing %> </span>
                <br />
                <img alt="<%= CRMSettingResource.AddTagInProgressing %>"
                    title="<%= CRMSettingResource.AddTagInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </ascwc:Container>
</div>

<ascwc:ViewSwitcher runat="server" ID="_switcherEntityType">
    <SortItems>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_forContacts"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_forDeals"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_forCases"></ascwc:ViewSwitcherLinkItem>
    </SortItems>
</ascwc:ViewSwitcher>
<br />


<ul id="tagList" class="clearFix">
</ul>

<div id="emptyTagContent">
    <asp:PlaceHolder ID="_phEmptyContent" runat="server"></asp:PlaceHolder>
</div>

<script id="tagRowTemplate" type="text/x-jquery-tmpl">
<li class="borderBase">
    <table class="tag_row tableBase" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="headerBaseSmall">
                    <div class="title">${value}</div>
                </td>
                <td class="count_link_contacts grayText" style="width:150px;">
                    ${relativeItemsString}
                </td>
                <td style="width:40px;">
                {{if relativeItemsCount == 0 }}
                    <img class="deleteButtonImg" title="<%= CRMSettingResource.DeleteTag %>" alt="<%= CRMSettingResource.DeleteTag %>"
                        onclick='ASC.CRM.TagSettingsView.deleteTag(this);'
                        src="<%= WebImageSupplier.GetAbsoluteWebPath("trash.png", ProductEntryPoint.ID) %>"/>
                    <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                {{/if}}
                </td>
            </tr>
        </tbody>
    </table>
</li>
</script>

<script type="text/javascript" language="javascript">
    jq(function() {
        ASC.CRM.TagSettingsView.init();
    });
</script>