<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListCasesView.ascx.cs"
                                Inherits="ASC.Web.CRM.Controls.Cases.ListCasesView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="aswscc" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>


<% if (!NoCases) %>
<% { %>
<script type="text/javascript" >
    jq(document).ready(function() {

        ASC.CRM.ListCasesView.init(<%= EntryCountOnPage %>,
                                   <%= CurrentPageNumber %>,
                                  "<%= CookieKeyForPagination %>",
                                  "<%= Anchor %>");

        if (!jq("#casesAdvansedFilter").advansedFilter) return;

        ASC.CRM.ListCasesView.advansedFilter = jq("#casesAdvansedFilter")
                    .advansedFilter({
                        anykey     : false,
                        help       : '<%= String.Format(CRMCommonResource.AdvansedFilterInfoText.ReplaceSingleQuote(),"<b>","</b>") %>',
                        maxfilters : 3,
                        maxlength  : "100",
                        store      : true,
                        inhash     : true,
                        filters    : [
                                    {
                                        type         : "combobox",
                                        id           : "opened",
                                        apiparamname : "isClosed",
                                        title        : "<%= CRMCasesResource.CaseStatusOpened%>",
                                        filtertitle  : "<%= CRMCasesResource.CasesByStatus%>",
                                        group        : "<%= CRMCasesResource.CasesByStatus%>",
                                        groupby      : "caseStatus",
                                        options      :
                                                [
                                                {value : false,  classname : '', title : "<%= CRMCasesResource.CaseStatusOpened%>", def : true},
                                                {value : true,  classname : '', title : "<%= CRMCasesResource.CaseStatusClosed%>"}
                                                ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "closed",
                                        apiparamname : "isClosed",
                                        title        : "<%= CRMCasesResource.CaseStatusClosed%>",
                                        filtertitle  : "<%= CRMCasesResource.CasesByStatus%>",
                                        group        : "<%= CRMCasesResource.CasesByStatus%>",
                                        groupby      : "caseStatus",
                                        options      :
                                                [
                                                {value : false,  classname : '', title : "<%= CRMCasesResource.CaseStatusOpened%>"},
                                                {value : true,  classname : '', title : "<%= CRMCasesResource.CaseStatusClosed%>", def : true}
                                                ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "tags",
                                        apiparamname : "tags",
                                        title        : "<%= CRMCommonResource.FilterWithTag %>",
                                        group        : "<%= CRMCommonResource.Other %>",
                                        options      : caseTags,
                                        defaulttitle : "<%= CRMCommonResource.Choose %>" ,
                                        enable       : caseTags.length > 0,
                                        multiple     : true
                                    }
                                ],

                        sorters:[
                                    { id: "title", title: "<%=CRMCommonResource.Title%>", dsc: false, def: true }
                                ]
                    })
                .bind("setfilter", ASC.CRM.ListCasesView.setFilter)
                .bind("resetfilter", ASC.CRM.ListCasesView.resetFilter);

    });

    jq(document).click(function(event)
    {
        jq.dropdownToggle().registerAutoHide(event, "#mainExportCsv", "#exportDialog");
    });
</script>

<div id="caseButtonsPanel">
    <a class="linkAddMediumText" href="<%= String.Format("cases.aspx?{0}=manage",UrlConstant.Action) %>">
        <%= CRMCasesResource.AddCase %>
    </a>
    <% if (!MobileVer) { %>
    <span class="splitter"></span>
    <a id="importCasesLink" class="crm-importLink" href="cases.aspx?action=import">
        <%= CRMCasesResource.ImportCases %>
    </a>
    <% if (CRMSecurity.IsAdmin) %>
    <% { %>
    <span class="splitter"></span>
    <span id="mainExportCsv" title="<%= CRMCommonResource.ExportCurrentListToCsv %>" class="crm-exportToCsvLink"
     onclick="ASC.CRM.ListCasesView.showExportDialog();">
        <a class="baseLinkAction"><%= CRMCommonResource.ExportCurrentListToCsv%></a>
        <img align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID)%>">
    </span>
    <% } %>
    <% } %>
</div>

<div id="caseFilterContainer">
    <aswscc:advansedfilter runat="server" id="filter" blockid="casesAdvansedFilter"></aswscc:advansedfilter>
    <br />
</div>

<div id="caseList">
    <div id="simpleCasesPageNavigator">
    </div>

    <table id="caseTable" class="tableBase" cellpadding="15" cellspacing="0">
        <tbody>
        </tbody>
    </table>

    <table id="tableForCasesNavigation" class="crm-navigationPanel" cellpadding="4" cellspacing="0" border="0">
        <tbody>
        <tr>
            <td>
                <div id="divForCasesPager">
                    <asp:PlaceHolder ID="_phPagerContent" runat="server"></asp:PlaceHolder>
                </div>
            </td>
            <td style="text-align:right;">
                <span class="grayText"><%= CRMCasesResource.TotalCases %>:</span>
                <span class="grayText" id="totalCasesOnPage"></span>

                <span class="grayText"><%= CRMCommonResource.ShowOnPage %>:&nbsp;</span>
                <select>
                    <option value="25">25</option>
                    <option value="50">50</option>
                    <option value="75">75</option>
                    <option value="100">100</option>
                </select>
                <script type="text/javascript">
                    jq('#tableForCasesNavigation select')
                    .val(<%= EntryCountOnPage %>)
                    .change(function(evt) {
                        ASC.CRM.ListCasesView.changeCountOfRows(this.value);
                    })
                    .tlCombobox();
                </script>
            </td>
        </tr>
        </tbody>
    </table>
</div>

<aswscc:EmptyScreenControl ID="emptyContentForCasesFilter" runat="server"></aswscc:EmptyScreenControl>

<div style="display: none;" id="exportDialog" class="dropDownDialog">
    <div class="dropDownCornerLeft" style="margin-left: -5px"></div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.ListCasesView.exportToCsv();">
            <%= CRMCommonResource.DownloadOneFile%>
        </a>
        <a class="dropDownItem" onclick="ASC.CRM.ListCasesView.openExportFile();">
            <%= CRMCommonResource.OpenToTheEditor%>
        </a>
    </div>
</div>

<script id="caseListTmpl" type="text/x-jquery-tmpl">
    <tbody>
        {{tmpl(cases) "#caseTmpl"}}
    </tbody>
</script>

<script id="caseTmpl" type="text/x-jquery-tmpl">
    <tr id="casesItem_${id}">
        <td class="borderBase">
            {{if isPrivate == true}}
                <img align="absmiddle" style="margin-bottom: 3px;" src="<%=WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID)%>">
            {{/if}}

            <a
            {{if isClosed == true}}
                class="linkHeaderMedium grayText"
            {{else}}
                class="linkHeaderMedium"
            {{/if}}
             href="?id=${id}">
                ${title}
            </a>
        </td>
    </tr>
</script>

<% } %>
<% else %>
<% { %>
<aswscc:EmptyScreenControl ID="casesEmptyScreen" runat="server"></aswscc:EmptyScreenControl>
<% } %>