<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListDealView.ascx.cs"
            Inherits="ASC.Web.CRM.Controls.Deals.ListDealView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Src="../Tasks/TaskActionView.ascx" TagPrefix="ctrl" TagName="TaskActionView" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="aswscc" %>
<%@ Register TagPrefix="ascwc" TagName="ExchangeRateView" Src="../Deals/ExchangeRateView.ascx" %>
<%@ Register TagPrefix="ascwc" TagName="ContactSelector" Src="../Common/ContactSelector.ascx" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="System.Security" %>


<% if (!NoDeals) %>
<% { %>
<script type="text/javascript" >
    jq(document).ready(function() {

        <% if (contactID != 0) %>
        <% { %>
            ASC.CRM.ListDealView.initTab(
                    <%= contactID %>,
                    <%= Global.EntryCountOnPage %>,
                    "<%= System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator %>");

        <% } %>
        <% else %>
        <% { %>
            ASC.CRM.ListDealView.init(
                <%= contactID %>,
                <%= EntryCountOnPage %>,
                <%= CurrentPageNumber %>,
                "<%= CookieKeyForPagination %>",
                "<%= System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator %>",
                "<%= Anchor %>");

            if (!jq("#dealsAdvansedFilter").advansedFilter) return;
            var tmpDate = new Date();
            var today = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);
            var yesterday = new Date(new Date(today).setDate(tmpDate.getDate() - 1));
            var beginningOfThisMonth = new Date(new Date(today).setDate(1));

            var endOfLastMonth = new Date(new Date(beginningOfThisMonth).setDate(beginningOfThisMonth.getDate() - 1));
            var beginningOfLastMonth = new Date(new Date(endOfLastMonth).setDate(1));


            var todayString = Teamlab.serializeTimestamp(today);
            var yesterdayString = Teamlab.serializeTimestamp(yesterday);
            var beginningOfThisMonthString = Teamlab.serializeTimestamp(beginningOfThisMonth);
            var beginningOfLastMonthString = Teamlab.serializeTimestamp(beginningOfLastMonth);
            var endOfLastMonthString = Teamlab.serializeTimestamp(endOfLastMonth);

            ASC.CRM.ListDealView.advansedFilter = jq("#dealsAdvansedFilter")
                        .advansedFilter({
                            anykey     : false,
                            help       : '<%= String.Format(CRMCommonResource.AdvansedFilterInfoText.ReplaceSingleQuote(),"<b>","</b>") %>',
                            maxfilters : 3,
                            colcount   : 2,
                            maxlength  : "100",
                            store      : true,
                            inhash     : true,
                            filters    : [
                                        {
                                            type         : "combobox",
                                            id           : "opportunityStagesID",
                                            apiparamname : "opportunityStagesID",
                                            title        : "<%= CRMDealResource.ByStage%>",
                                            group        : "<%= CRMDealResource.FilterByStageOrStageType%>",
                                            groupby      : "stageType",
                                            options      : dealMilestones,
                                            defaulttitle : "<%= CRMCommonResource.Choose %>",
                                            enable       : dealMilestones.length > 0
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "stageTypeOpen",
                                            apiparamname : "stageType",
                                            title        : "<%= DealMilestoneStatus.Open.ToLocalizedString() %>",
                                            group        : "<%= CRMDealResource.FilterByStageOrStageType%>",
                                            filtertitle  : "<%= CRMDealResource.ByStageType %>",
                                            groupby      : "stageType",
                                            options      :
                                                    [
                                                    {value : "<%= DealMilestoneStatus.Open %>",  classname : '', title : "<%= DealMilestoneStatus.Open.ToLocalizedString() %>", def : true},
                                                    {value : "<%= DealMilestoneStatus.ClosedAndWon %>",  classname : '', title : "<%= DealMilestoneStatus.ClosedAndWon.ToLocalizedString() %>"},
                                                    {value : "<%= DealMilestoneStatus.ClosedAndLost %>",  classname : '', title : "<%= DealMilestoneStatus.ClosedAndLost.ToLocalizedString() %>"}
                                                    ]
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "stageTypeClosedAndWon",
                                            apiparamname : "stageType",
                                            title        : "<%= DealMilestoneStatus.ClosedAndWon.ToLocalizedString() %>",
                                            filtertitle  : "<%= CRMDealResource.ByStageType %>",
                                            group        : "<%= CRMDealResource.FilterByStageOrStageType%>",
                                            groupby      : "stageType",
                                            options      :
                                                    [
                                                    {value : "<%= DealMilestoneStatus.Open %>",  classname : '', title : "<%= DealMilestoneStatus.Open.ToLocalizedString() %>"},
                                                    {value : "<%= DealMilestoneStatus.ClosedAndWon %>",  classname : '', title : "<%= DealMilestoneStatus.ClosedAndWon.ToLocalizedString() %>", def : true},
                                                    {value : "<%= DealMilestoneStatus.ClosedAndLost %>",  classname : '', title : "<%= DealMilestoneStatus.ClosedAndLost.ToLocalizedString() %>"}
                                                    ]
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "stageTypeClosedAndLost",
                                            apiparamname : "stageType",
                                            title        : "<%= DealMilestoneStatus.ClosedAndLost.ToLocalizedString() %>",
                                            filtertitle  : "<%= CRMDealResource.ByStageType %>",
                                            group        : "<%= CRMDealResource.FilterByStageOrStageType%>",
                                            groupby      : "stageType",
                                            options      :
                                                    [
                                                    {value : "<%= DealMilestoneStatus.Open %>",  classname : '', title : "<%= DealMilestoneStatus.Open.ToLocalizedString() %>"},
                                                    {value : "<%= DealMilestoneStatus.ClosedAndWon %>",  classname : '', title : "<%= DealMilestoneStatus.ClosedAndWon.ToLocalizedString() %>"},
                                                    {value : "<%= DealMilestoneStatus.ClosedAndLost %>",  classname : '', title : "<%= DealMilestoneStatus.ClosedAndLost.ToLocalizedString() %>", def : true}
                                                    ]
                                        },


                                        {
                                            type         : "combobox",
                                            id           : "lastMonth",
                                            apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                            title        : "<%= CRMCommonResource.LastMonth %>",
                                            filtertitle  : "<%= CRMCommonResource.FilterByDate%>",
                                            group        : "<%= CRMCommonResource.FilterByDate%>",
                                            groupby      : "byDate",
                                            options      :
                                                    [
                                                    {value : jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]),  classname : '', title : "<%= CRMCommonResource.LastMonth %>", def : true},
                                                    {value : jq.toJSON([yesterdayString, yesterdayString]),  classname : '', title : "<%= CRMCommonResource.Yesterday %>"},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMCommonResource.Today %>"},
                                                    {value : jq.toJSON([beginningOfThisMonthString, todayString]),  classname : '', title : "<%= CRMCommonResource.ThisMonth %>"}
                                                    ]
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "yesterday",
                                            apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                            title        : "<%= CRMCommonResource.Yesterday %>",
                                            filtertitle  : "<%= CRMCommonResource.FilterByDate%>",
                                            group        : "<%= CRMCommonResource.FilterByDate%>",
                                            groupby      : "byDate",
                                            options      :
                                                    [
                                                    {value : jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]),  classname : '', title : "<%= CRMCommonResource.LastMonth %>"},
                                                    {value : jq.toJSON([yesterdayString, yesterdayString]),  classname : '', title : "<%= CRMCommonResource.Yesterday %>", def : true},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMCommonResource.Today %>"},
                                                    {value : jq.toJSON([beginningOfThisMonthString, todayString]),  classname : '', title : "<%= CRMCommonResource.ThisMonth %>"}
                                                    ]
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "today",
                                            apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                            title        : "<%= CRMCommonResource.Today %>",
                                            filtertitle  : "<%= CRMCommonResource.FilterByDate%>",
                                            group        : "<%= CRMCommonResource.FilterByDate%>",
                                            groupby      : "byDate",
                                            options      :
                                                    [
                                                    {value : jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]),  classname : '', title : "<%= CRMCommonResource.LastMonth %>"},
                                                    {value : jq.toJSON([yesterdayString, yesterdayString]),  classname : '', title : "<%= CRMCommonResource.Yesterday %>"},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMCommonResource.Today %>", def : true},
                                                    {value : jq.toJSON([beginningOfThisMonthString, todayString]),  classname : '', title : "<%= CRMCommonResource.ThisMonth %>"}
                                                    ]
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "thisMonth",
                                            apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                            title        : "<%= CRMCommonResource.ThisMonth %>",
                                            filtertitle  : "<%= CRMCommonResource.FilterByDate%>",
                                            group        : "<%= CRMCommonResource.FilterByDate%>",
                                            groupby      : "byDate",
                                            options      :
                                                    [
                                                    {value : jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]),  classname : '', title : "<%= CRMCommonResource.LastMonth %>"},
                                                    {value : jq.toJSON([yesterdayString, yesterdayString]),  classname : '', title : "<%= CRMCommonResource.Yesterday %>"},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMCommonResource.Today %>"},
                                                    {value : jq.toJSON([beginningOfThisMonthString, todayString]),  classname : '', title : "<%= CRMCommonResource.ThisMonth %>", def : true}
                                                    ]
                                        },


                                        {
                                            type        : "daterange",
                                            id          : "fromToDate",
                                            title       : "<%= CRMDealResource.CustomDateFilter %>",
                                            filtertitle : "<%= CRMCommonResource.FilterByDate%>",
                                            group       : "<%= CRMCommonResource.FilterByDate%>",
                                            groupby      : "byDate"
                                        },

                                        {
                                            type      : ASC.CRM.myFilter.type,
                                            id        : ASC.CRM.myFilter.idFilterByParticipant,
                                            apiparamname : jq.toJSON(["contactID", "contactAlsoIsParticipant"]),
                                            title     : "<%= CRMDealResource.FilterByParticipant %>",
                                            group     : "<%= CRMCommonResource.Other %>",
                                            groupby   : "contact",
                                            hashmask  : '',
                                            create    : ASC.CRM.myFilter.createFilterByParticipant,
                                            customize : ASC.CRM.myFilter.customizeFilterByParticipant,
                                            destroy   : ASC.CRM.myFilter.destroyFilterByParticipant,
                                            process   : ASC.CRM.myFilter.processFilter
                                        },
                                        {
                                            type      : ASC.CRM.myFilter.type,
                                            id        : ASC.CRM.myFilter.idFilterByContact,
                                            apiparamname : jq.toJSON(["contactID", "contactAlsoIsParticipant"]),
                                            title     : "<%= CRMDealResource.FilterByContact %>",
                                            group     : "<%= CRMCommonResource.Other %>",
                                            groupby   : "contact",
                                            hashmask  : '',
                                            create    : ASC.CRM.myFilter.createFilterByContact,
                                            customize : ASC.CRM.myFilter.customizeFilterByContact,
                                            destroy   : ASC.CRM.myFilter.destroyFilterByContact,
                                            process   : ASC.CRM.myFilter.processFilter
                                        },
                                        {
                                            type         : "combobox",
                                            id           : "tags",
                                            apiparamname : "tags",
                                            title        : "<%= CRMCommonResource.FilterWithTag %>",
                                            group        : "<%= CRMCommonResource.Other %>",
                                            options      : dealTags,
                                            defaulttitle : "<%= CRMCommonResource.Choose %>",
                                            enable       : dealTags.length > 0,
                                            multiple     : true
                                        },


                                        {
                                            type         : "flag",
                                            id           : "my",
                                            apiparamname : "responsibleID",
                                            title        : "<%= CRMDealResource.MyDealFilter%>",
                                            group        : "<%= CRMCommonResource.FilterByResponsible%>",
                                            groupby      : "responsible",
                                            defaultparams:      { value: "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>" }
                                        },
                                        {
                                            type         : "person",
                                            id           : "responsibleID",
                                            apiparamname : "responsibleID",
                                            title        : "<%= CRMDealResource.CustomResponsibleFilter%>",
                                            group        : "<%= CRMCommonResource.FilterByResponsible%>",
                                            groupby      : "responsible",
                                            filtertitle  : "<%= CRMCommonResource.FilterByResponsible%>"
                                        }
                                     ],
                            sorters: [
                                        { id: "title", title: "<%=CRMCommonResource.Title%>", dsc: false, def: false },
                                        { id: "responsible", title: "<%=CRMCommonResource.Responsible%>", dsc: false, def: false },
                                        { id: "stage", title: "<%=CRMDealResource.DealMilestone %>", dsc: false, def: true },
                                        { id: "bidvalue", title: "<%=CRMDealResource.ExpectedValue%>", dsc: false, def: false },
                                        { id: "dateandtime", title: "<%=CRMDealResource.Estimated %>", dsc: false, def: false }
                                     ]
                        })
                        .bind("setfilter", ASC.CRM.ListDealView.setFilter)
                        .bind("resetfilter", ASC.CRM.ListDealView.resetFilter);
    <% }%>
    });

    jq(document).click(function(event)
    {
        jq.dropdownToggle().registerAutoHide(event, "#mainExportCsv", "#exportDialog");
    });
</script>

<div id="dealButtonsPanel">
    <a class="linkAddMediumText" href="deals.aspx?action=manage<%=contactID==0 ? String.Empty: "&contactID=" + contactID%>">
        <%= CRMDealResource.CreateNewDeal %>
    </a>
    <% if (contactID == 0 && !MobileVer) { %>
    <span class="splitter"></span>
    <a id="importDealsLink" class="crm-importLink" href="deals.aspx?action=import">
        <%= CRMDealResource.ImportDeals %>
    </a>
    <% if (CRMSecurity.IsAdmin) %>
    <% { %>
    <span class="splitter"></span>
    <span id="mainExportCsv" title="<%= CRMCommonResource.ExportCurrentListToCsv %>" class="crm-exportToCsvLink"
     onclick="ASC.CRM.ListDealView.showExportDialog();">
        <a class="baseLinkAction"><%= CRMCommonResource.ExportCurrentListToCsv%></a>
        <img align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID)%>">
    </span>
    <% } %>
    <% } %>
</div>

<% if (contactID == 0) %>
<% { %>
<div id="dealFilterContainer">
    <aswscc:advansedfilter runat="server" id="filter" blockid="dealsAdvansedFilter"></aswscc:advansedfilter>
    <br />
</div>
<% } %>

<div id="dealList" class="clearFix" style='<%= contactID == 0 ? "min-height: 400px;padding-top:10px;" : "min-height: 200px;" %>'>
    <% if (contactID == 0) %>
    <% { %>
    <div id="simpleDealPageNavigator">
    </div>
    <% } %>

    <table id="dealTable" class="tableBase" cellpadding="7" cellspacing="0">
        <tbody>
        </tbody>
    </table>


    <% if (contactID != 0) %>
    <% { %>
    <div id="showMoreDealsButtons">
        <a class="crm-showMoreLink" style="display:none;">
            <%= CRMJSResource.ShowMoreButtonText%>
        </a>
        <a class="crm-loadingLink" style="display:none;">
            <%= CRMJSResource.LoadingProcessing%>
        </a>
    </div>
    <a style="float: right;margin-top: 20px;margin-right: 8px;" class="baseLinkAction showTotalAmount"
        onclick="ASC.CRM.ListDealView.showExchangeRatePopUp();" href="javascript:void(0)">
            <%=CRMDealResource.ShowTotalAmount %>
    </a>
    <% } %>

    <ascwc:ExchangeRateView runat="server" ID="_exchangeRate" />

    <% if (contactID == 0) %>
    <% { %>
    <table id="tableForDealNavigation" class="crm-navigationPanel" cellpadding="4" cellspacing="0" border="0">
        <tbody>
        <tr>
            <td>
                <div id="divForDealPager">
                    <asp:PlaceHolder ID="_phPagerContent" runat="server"></asp:PlaceHolder>
                </div>
            </td>
            <td style="text-align:right;">
                <a style="margin-right: 25px;" class="baseLinkAction showTotalAmount"
                    onclick="ASC.CRM.ListDealView.showExchangeRatePopUp();" href="javascript:void(0)">
                        <%=CRMDealResource.ShowTotalAmount %>
                </a>
                <span class="grayText"><%= CRMDealResource.TotalDeals %>:</span>
                <span class="grayText" id="totalDealsOnPage"></span>

                <span class="grayText"><%= CRMCommonResource.ShowOnPage %>:&nbsp;</span>
                <select>
                    <option value="25">25</option>
                    <option value="50">50</option>
                    <option value="75">75</option>
                    <option value="100">100</option>
                </select>
                <script type="text/javascript">
                    jq('#tableForDealNavigation select')
                    .val(<%= EntryCountOnPage %>)
                    .change(function(evt) {
                        ASC.CRM.ListDealView.changeCountOfRows(this.value);
                    })
                    .tlCombobox();
                </script>
            </td>
        </tr>
        </tbody>
    </table>

    <% } %>

</div>

<aswscc:EmptyScreenControl ID="emptyContentForDealsFilter" runat="server"></aswscc:EmptyScreenControl>

<% if (contactID == 0) %>
<% { %>
<div id="hiddenBlockForContactSelector" style="display:none;width:300px;">
    <ascwc:ContactSelector runat="server" ID="contactSelectorForFilter" />
</div>

<div style="display: none;" id="exportDialog" class="dropDownDialog">
    <div class="dropDownCornerLeft" style="margin-left: -5px"></div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.ListDealView.exportToCsv();">
            <%= CRMCommonResource.DownloadOneFile%>
        </a>
        <a class="dropDownItem" onclick="ASC.CRM.ListDealView.openExportFile();">
            <%= CRMCommonResource.OpenToTheEditor%>
        </a>
    </div>
</div>
<% } %>

<script id="dealListTmpl" type="text/x-jquery-tmpl">
    <tbody>
        {{tmpl(opportunities) "#dealTmpl"}}
    </tbody>
</script>

<script id="dealTmpl" type="text/x-jquery-tmpl">
    <tr id="dealItem_${id}">
        <td class="borderBase" title="${stage.title}"
            style="background-color: ${stage.color}; padding: 0px; margin: 0px; width: 4px;">
        </td>
        <td class="borderBase dealTitle">
            <div>
                {{if isOverdue == true}}
                    <span class='redLable' style='margin-right: 10px;'>
                        <%= CRMDealResource.Overdue %>
                    </span>
                {{/if}}

                {{if isPrivate == true}}
                    <img align="absmiddle" style="margin-bottom: 1px;"
                            src="<%=WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID)%>">
                {{/if}}
                <a href="deals.aspx?id=${id}"
                    {{if isOverdue == true || closedStatusString == ""}}
                        class="linkHeaderMedium"
                    {{else}}
                        class="linkHeaderMedium grayText"
                    {{/if}} title="${title}">
                    ${title}
                </a>

                <div style="height:4px;">&nbsp;</div>

                {{if contact != null}}
                    <div>
                    {{if contact.isCompany == true}}
                        <a href="default.aspx?id=${contact.id}" data-id="${contact.id}"
                                id="deal_${id}_company_${contact.id}" class="linkMedium crm-companyInfoCardLink {{if closedStatusString != ""}}grayText{{/if}} ">
                            ${contact.displayName}
                        </a>
                    {{else}}
                        <a href="default.aspx?id=${contact.id}&type=people" data-id="${contact.id}"
                                id="deal_${id}_person_${contact.id}" class="linkMedium crm-peopleInfoCardLink {{if closedStatusString != ""}}grayText{{/if}} ">
                            ${contact.displayName}
                        </a>
                    {{/if}}
                    </div>
                {{else}}
                    <span class='grayText'><%= CRMDealResource.DealClientNoSet %></span>
                {{/if}}
            </div>
        </td>
        <td class="borderBase dealResponsible">
            <span style="white-space: nowrap;" {{if closedStatusString != ""}} class="grayText" {{/if}}
                        title="${responsible.displayName}">
                ${responsible.displayName}
            </span>

            <div style="height:4px;">&nbsp;</div>

            {{if isOverdue == true}}
                <span class='redText'>
                    <%= CRMJSResource.ExpectedCloseDate %>: ${expectedCloseDateString}
                </span>
            {{else closedStatusString != ""}}
                <span class='grayText'>
                    ${closedStatusString}
                </span>
            {{else expectedCloseDateString != ""}}
                <span>
                    <%= CRMJSResource.ExpectedCloseDate %>: ${expectedCloseDateString}
                </span>
            {{/if}}
        </td>

        <td class="borderBase dealBidValue">
            <div>

            {{if typeof bidValue != "undefined" && bidValue != 0}}
                 <span {{if closedStatusString != ""}} class="grayText" {{/if}}>${bidNumberFormat}</span><span class='textBigDescribe'> ${bidCurrency.abbreviation}</span>
            {{/if}}

            {{if typeof bidType != "undefined" && bidType != 0}}
                <div style="height:4px;">&nbsp;</div>
                <span class='textMediumDescribe'>${ASC.CRM.ListDealView.expectedValue(bidType, perPeriodValue)}</span>
            {{/if}}
            </div>
        </td>
    </tr>
</script>

<script id="bidFormat" type="text/x-jquery-tmpl">
    ${number}<span class='textBigDescribe'> ${abbreviation}</span><br/>
</script>

<% } %>
<% else %>
<% { %>
<aswscc:EmptyScreenControl ID="dealsEmptyScreen" runat="server"></aswscc:EmptyScreenControl>
<div id="files_hintStagesPanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=CRMDealResource.TooltipStages%>
    <a href="http://www.teamlab.com/help/tipstricks/opportunity-stages.aspx" target="_blank"><%=CRMCommonResource.ButtonLearnMore%></a>
</div>
<% } %>