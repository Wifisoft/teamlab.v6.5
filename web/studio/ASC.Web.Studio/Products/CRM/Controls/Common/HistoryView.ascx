<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Core.Tenants" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.CRM.Core.Entities" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Common.HistoryView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="aswc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="aswscc" %>

<% if (Global.DebugVersion) { %>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("fileUploader.js") %>"></script>
<% } %>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {

        ASC.CRM.HistoryView.init(<%= TargetContactID %>,
            '<%=TargetEntityType.ToString().ToLower() %>',
            <%= TargetEntityID %>,
            <%= Global.EntryCountOnPage %>,
            '<%= TenantUtil.DateTimeNow().ToShortDateString() %>',
            '<%= DateTimeExtension.DateMaskForJQuery %>');

        //var anchor = ASC.Controls.AnchorController.getAnchor();
        //if(anchor!="")
        //    jq.scrollTo(jq("a[name="+anchor+"]").position().top-50);

        if (!jq("#eventsAdvansedFilter").advansedFilter) return;

        var tmpDate = new Date();

        var today = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);
        var yesterday = new Date(new Date(today).setDate(tmpDate.getDate() - 1));

        var todayString = Teamlab.serializeTimestamp(today);
        var yesterdayString = Teamlab.serializeTimestamp(yesterday);

        ASC.CRM.HistoryView.advansedFilter = jq("#eventsAdvansedFilter")
                .advansedFilter({
                    anykey     : false,
                    help       : '<%= String.Format(CRMCommonResource.AdvansedFilterInfoText.ReplaceSingleQuote(),"<b>","</b>") %>',
                    maxfilters : 3,
                    maxlength  : "100",
                    filters: [
                                {
                                    type         : "flag",
                                    id           : "my",
                                    apiparamname : "createBy",
                                    title        : "<%= CRMCommonResource.MyEventsFilter%>",
                                    group        : "<%=CRMCommonResource.FilterByResponsible%>",
                                    groupby      : "responsible",
                                    defaultparams : { value: "<%= SecurityContext.CurrentAccount.ID %>" }
                                },
                                {
                                    type         : "person",
                                    id           : "responsibleID",
                                    apiparamname : "createBy",
                                    title        : "<%= CRMCommonResource.CustomResponsibleFilter%>",
                                    group        : "<%=CRMCommonResource.FilterByResponsible%>",
                                    groupby      : "responsible",
                                    filtertitle  : "<%= CRMCommonResource.FilterByResponsible%>"
                                },
                                {
                                    type         : "combobox",
                                    id           : "today",
                                    apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                    title        : "<%= CRMCommonResource.Today%>",
                                    filtertitle  : "<%= CRMCommonResource.Date %>",
                                    group        : "<%= CRMCommonResource.Date %>",
                                    groupby      : "date",
                                    options      :
                                            [
                                            {value : jq.toJSON([todayString, todayString]), classname : '', title : "<%= CRMCommonResource.Today %>", def : true},
                                            {value : jq.toJSON([yesterdayString, yesterdayString]), classname : '', title : "<%= CRMCommonResource.Yesterday %>"}
                                            ]
                                },
                                {
                                    type         : "combobox",
                                    id           : "yesterday",
                                    apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                    title        : "<%= CRMCommonResource.Yesterday%>",
                                    filtertitle  : "<%= CRMCommonResource.Date %>",
                                    group        : "<%= CRMCommonResource.Date %>",
                                    groupby      : "date",
                                    options      :
                                            [
                                            {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMCommonResource.Today %>"},
                                            {value : jq.toJSON([yesterdayString, yesterdayString]), classname : '', title : "<%= CRMCommonResource.Yesterday %>", def : true}
                                            ]
                                },
                                {
                                    type        : "daterange",
                                    id          : "fromToDate",
                                    title       : "<%= CRMCommonResource.CustomDateFilter%>",
                                    group       : "<%= CRMCommonResource.Date %>",
                                    groupby     : "date",
                                    filtertitle : "<%= CRMCommonResource.Date%>"
                                },
                                {
                                    type         : "combobox",
                                    id           : "categoryID",
                                    apiparamname : "categoryID",
                                    title        : "<%= CRMCommonResource.ByCategory%>",
                                    group        : "<%= CRMCommonResource.Other %>",
                                    options      : eventsCategories,
                                    defaulttitle : "<%= CRMCommonResource.Choose %>"
                                }
                            ],
                    sorters: [
                                { id: "created", title: "<%=CRMCommonResource.Date%>", dsc: true, def: true },
                                { id: "createby", title: "<%=CRMCommonResource.Author%>", dsc: false, def: false },
                                { id: "content", title: "<%=CRMCommonResource.Content%>", dsc: false, def: false },
                                { id: "category", title: "<%=CRMCommonResource.Category%>", dsc: false, def: false }
                             ]
                })
                .bind("setfilter", ASC.CRM.HistoryView.setFilter)
                .bind("resetfilter", ASC.CRM.HistoryView.resetFilter);
    });
    <% if (!MobileVer) { %>
    jq(document).click(function(event) {
        <% if (TargetContactID != 0) { %>
            jq.dropdownToggle().registerAutoHide(event, "#historyTypeSwitcher", "#historyTypePanel");
            jq.dropdownToggle().registerAutoHide(event, "#historyItemSwitcher", "#historyDealPanel");
            jq.dropdownToggle().registerAutoHide(event, "#historyItemSwitcher", "#historyCasePanel");
        <% } else { %>
            jq.dropdownToggle().registerAutoHide(event, "#historyContactSwitcher", "#historyContactPanel");
        <% } %>
    });
    <% } %>
</script>

<script id="historyRowTmpl" type="text/x-jquery-tmpl">
<tr id="event_${id}">
    <td class="borderBase entityPlace">
        <a name="${id}"></a>
        {{if typeof(category) != "undefined"}}
        <img src="${category.imagePath}" title="${category.title}" alt="${category.title}"/>
        {{/if}}
    </td>
    <td class="borderBase title">
        {{if contact != null && contact.id != ASC.CRM.HistoryView.ContactID }}
            <a class="linkDescribeMedium" href="default.aspx?id=${contact.id}">${contact.displayName}</a>
            {{if entity != null && entity.entityId != ASC.CRM.HistoryView.EntityID}}
                &nbsp;/&nbsp;
            {{/if}}
        {{/if}}
        {{if entity != null && entity.entityId != ASC.CRM.HistoryView.EntityID }}
            ${entityType}: <a class="linkDescribeMedium" href="${entityURL}">${entity.entityTitle}</a>
        {{/if}}
        <div>
            {{html Encoder.XSSEncode(content).replace(/\n/g, "<br/>") }}
        </div>
        <span class="textMediumDescribe">${createdDate} <%= CRMCommonResource.Author %>: ${createdBy.displayName}</span>
    </td>
    <td class="borderBase activityData textBigDescribe">
        {{if files != null }}
            <img src="<%= WebImageSupplier.GetAbsoluteWebPath("skrepka.gif", ProductEntryPoint.ID) %>" align="absmiddle"/>
            <a id="eventAttachSwither_${id}" class="baseLinkAction linkMedium">
                <%= CRMCommonResource.ViewFiles %>
            </a>
            <div id="eventAttach_${id}" class="dropDownDialog">
                <div class="dropDownCornerLeft"></div>
                <div class="dropDownContent">
                    {{each(i, file) files}}
                     <div id="fileContent_${file.id}" class="dropDownItem">
                        <a target="_blank" href="${file.viewUrl}" class="linkMedium">
                            ${file.title}
                        </a>
                        {{if $data.canEdit == true }}
                        <img align="absmiddle" title="<%= CRMCommonResource.Delete %>"
                            onclick="ASC.CRM.HistoryView.deleteFile(${file.id}, ${$data.id})"
                            style="cursor:pointer;margin-left: 3px;"
                            src="<%=WebImageSupplier.GetAbsoluteWebPath("trash_12.png")%>" />
                        {{/if}}
                     </div>
                    {{/each}}
                </div>
            </div>
        {{/if}}
    </td>
    <td class="borderBase" style="width: 20px;">
      {{if canEdit == true }}
      <img src="<%=WebImageSupplier.GetAbsoluteWebPath("trash.png", ProductEntryPoint.ID)%>"
           title="<%= CRMCommonResource.DeleteHistoryEvent %>"
           onclick="ASC.CRM.HistoryView.deleteEvent(${id})"
           id="eventTrashImg_${id}" style="cursor:pointer;" />
      <img src="<%=WebImageSupplier.GetAbsoluteWebPath("mini_loader.gif")%>"
            id="eventLoaderImg_${id}" style="display:none;" />
      {{/if}}
  </td>
</tr>
</script>

<div id="historyBlock">
    <p class="headerBase historyBlockHeader"><%= Title%></p>

    <table class="details-menu" width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="150px">
                <asp:PlaceHolder ID="phCategorySelector" runat="server"></asp:PlaceHolder>
            </td>
            <td width="150px">
                <%= CRMCommonResource.Date %>:
                <input type="text" class="textEditCalendar"/>
            </td>
            <td width="650px" style="white-space:nowrap">
                <div id="eventLinkToPanel" <%= (TargetContactID == 0 && ContactMembers.Count == 0 || TargetContactID != 0) ? "class='empty-select'" : "" %>>
                <% if (TargetContactID != 0) %>
                <% { %>
                    <span><%= CRMCommonResource.LinkTo %>:</span>

                    <% if (!MobileVer) { %>
                    <span id="historyTypeSwitcher" onclick="ASC.CRM.HistoryView.showDropdownPanel('#historyTypeSwitcher','historyTypePanel');">
                        <a class="baseLinkAction linkMedium"><%= CRMJSResource.Choose%></a>
                        <img align="absmiddle" style="margin-left: 3px" src="<%= WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID) %>"/>
                    </span>
                    <div id="historyTypePanel" class="dropDownDialog">
                        <div class="dropDownCornerLeft" style="margin-left: 20px"></div>
                        <div class="dropDownContent">
                            <a class="dropDownItem" onclick="ASC.CRM.HistoryView.changeHistoryType('', this);">
                                <%= CRMCommonResource.ClearFilter%>
                            </a>
                            <a class="dropDownItem" onclick="ASC.CRM.HistoryView.changeHistoryType('<%=EntityType.Opportunity.ToString().ToLower()%>', this);">
                                <%= CRMDealResource.Deal%>
                            </a>
                            <a class="dropDownItem" onclick="ASC.CRM.HistoryView.changeHistoryType('<%=EntityType.Case.ToString().ToLower()%>', this);">
                                <%= CRMCasesResource.Case%>
                            </a>
                        </div>
                    </div>
                    <span class="splitter">&nbsp;</span>
                    <span id="historyItemSwitcher" style="display: none">
                        <a class="baseLinkAction linkMedium"><%= CRMJSResource.Choose%></a>
                        <img align="absmiddle" style="margin-left: 3px" src="<%= WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID) %>"/>
                    </span>

                    <div id="historyDealPanel" class="dropDownDialog">
                        <div class="dropDownCornerLeft" style="margin-left: 20px"></div>
                        <div class="dropDownContent"></div>
                    </div>

                    <div id="historyCasePanel" class="dropDownDialog">
                        <div class="dropDownCornerLeft" style="margin-left: 20px"></div>
                        <div class="dropDownContent"></div>
                    </div>

                    <% } else {%>
                    <select id="historyTypeSelect" onchange="ASC.CRM.HistoryView.changeHistoryType();">
                        <option value="-1" type="">
                            <%= CRMJSResource.Choose%>
                        </option>
                        <option value="<%=(int)EntityType.Opportunity%>" type="<%=EntityType.Opportunity.ToString().ToLower()%>">
                            <%= CRMDealResource.Deal%>
                        </option>
                        <option value="<%=(int)EntityType.Case%>" type="<%=EntityType.Case.ToString().ToLower()%>">
                            <%= CRMCasesResource.Case%>
                        </option>
                    </select>
                    <span class="splitter"></span>
                    <select id="historyDealSelect" onchange="ASC.CRM.HistoryView.changeHistoryItem();" style="display: none;width:200px"></select>
                    <select id="historyCaseSelect" onchange="ASC.CRM.HistoryView.changeHistoryItem();" style="display: none;width:200px"></select>
                    <% } %>

                    <input type="hidden" id="typeID" value=""/>
                    <input type="hidden" id="itemID" value="0"/>
                <% } %>
                <% else { %>
                    <span><%= CRMCommonResource.AttachThisNoteToContact %>:</span>

                    <% if (!MobileVer) { %>
                    <span id="historyContactSwitcher" onclick="ASC.CRM.HistoryView.showDropdownPanel('#historyContactSwitcher','historyContactPanel')">
                        <a class="baseLinkAction linkMedium"><%= CRMJSResource.Choose %></a>
                        <img align="absmiddle" style="margin-left: 3px" src="<%= WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID) %>"/>
                    </span>
                    <div id="historyContactPanel" class="dropDownDialog">
                        <div class="dropDownCornerLeft" style="margin-left: 20px"></div>
                        <div class="dropDownContent">
                            <a class="dropDownItem" onclick="ASC.CRM.HistoryView.changeHistoryContact(-1, this);"><%= CRMCommonResource.ClearFilter %></a>
                            <% foreach (var member in ContactMembers)
                               { %>
                            <a class="dropDownItem" contactid="<%= member.ID %>" onclick="ASC.CRM.HistoryView.changeHistoryContact(<%= member.ID %>, this);"><%= member.GetTitle().HtmlEncode() %></a>
                            <% } %>
                        </div>
                    </div>
                    <% } else {%>
                    <select id="historyContactSelect" onchange="ASC.CRM.HistoryView.changeHistoryContact();" style="width:200px">
                        <option value="-1"><%= CRMJSResource.Choose%></option>
                        <% foreach (var member in ContactMembers) { %>
                        <option value="<%= member.ID %>"><%= member.GetTitle().HtmlEncode() %></option>
                        <% } %>
                    </select>
                    <% } %>

                    <input type="hidden" id="contactID" value="0"/>
                <% } %>
                </div>
            </td>
        </tr>
    </table>

    <textarea style="width:100%;height:100px;margin-top:10px; resize:none;"></textarea>

    <div style="margin: 10px 0;">
        <table width="100%">
        <tr>
            <td style="white-space:nowrap;">
                <%=CRMCommonResource.SelectUsersToNotify%>:
            </td>
            <td style="white-space:nowrap;">
                <asp:PlaceHolder runat="server" ID="_phUserSelectorListView"></asp:PlaceHolder>
            </td>
            <td width="100%" align="right">
                <% if(!MobileVer) {%>
                <div style="float:right;" id="attachButtonsPanel">
                    <a class="attachLink baseLinkAction linkMedium" onclick="ASC.CRM.HistoryView.showAttachmentPanel(true)" >
                        <%= CRMCommonResource.ShowAttachPanel%>
                    </a>
                    <a class="attachLink baseLinkAction linkMedium" onclick="ASC.CRM.HistoryView.showAttachmentPanel(false)" style="display: none;" >
                        <%= CRMCommonResource.HideAttachPanel%>
                    </a>
                </div>
                <% } %>
            </td>
        </tr>
        </table>
        <div id="selectedUsers" class="clearFix" style="margin-top: 10px;"></div>
    </div>

    <% if(!MobileVer) {%>
    <div id="attachOptions" style="display:none;margin: 10px 0;">
        <asp:PlaceHolder ID="_phfileUploader" runat="server" />
    </div>
    <% } %>

    <div class="action_block">
        <a class="disableLink" onclick="ASC.CRM.HistoryView.addEvent()">
            <%= CRMCommonResource.AddThisNote %>
        </a>
    </div>
    <div style="display: none;" class="ajax_info_block">
        <span class="textMediumDescribe">
            <%= CRMCommonResource.AddThisNoteProggress%>... </span>
        <br />
        <img alt="<%= CRMSettingResource.AddFieldInProgressing %>" title="<%= CRMSettingResource.AddFieldInProgressing %>"
            src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
    </div>

    <br />
    <div class="clearFix">
        <div id="eventsFilterContainer">
            <aswscc:advansedfilter runat="server" id="filter" blockid="eventsAdvansedFilter"></aswscc:advansedfilter>
        </div>
        <br />

        <div id="eventsList">
            <table id="eventsTable" class="tableBase" cellpadding="10" cellspacing="0">
                <tbody>
                </tbody>
            </table>
            <div id="showMoreEventsButtons">
                <a class="crm-showMoreLink" style="display:none;">
                    <%= CRMJSResource.ShowMoreButtonText %>
                </a>
                <a class="crm-loadingLink" style="display:none;">
                    <%= CRMJSResource.LoadingProcessing %>
                </a>
            </div>
        </div>

        <asp:PlaceHolder ID="_phEmptyContent" runat="server"></asp:PlaceHolder>
    </div>
</div>
