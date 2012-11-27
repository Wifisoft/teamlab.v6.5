<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListTaskView.ascx.cs"
        Inherits="ASC.Web.CRM.Controls.Tasks.ListTaskView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="aswscc" %>
<%@ Register TagPrefix="ctrl" TagName="TaskActionView" Src="TaskActionView.ascx" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<script type="text/javascript" >
    jq(document).ready(function() {

        ASC.CRM.ListTaskView.init(<%= CurrentContact != null ? CurrentContact.ID : 0 %>,
                                   '<%=CurrentEntityType.ToString().ToLower() %>',
                                   <%= EntityID %>,
                                   <%= Global.EntryCountOnPage %>,
                                   <%= (int)HistoryCategorySystem.TaskClosed %>,
                                   <%= NoTasks.ToString().ToLower() %>,
                                   '<%= Anchor %>');

        ASC.CRM.Common.changeImage(jq("img[id^=['taskMenu_']"), "<%=WebImageSupplier.GetAbsoluteWebPath("taskmenu.png", ProductEntryPoint.ID)%>", "<%=WebImageSupplier.GetAbsoluteWebPath("taskmenu_disabled.png", ProductEntryPoint.ID)%>")


        <% if (CurrentContact == null && EntityID == 0) %>
        <% { %>
        if (!jq("#tasksAdvansedFilter").advansedFilter) return;

        var tmpDate = new Date();

        var today = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);
        var yesterday = new Date(new Date(today).setDate(tmpDate.getDate() - 1));
        var tomorrow = new Date(new Date(today).setDate(tmpDate.getDate() + 1));

        var todayString = Teamlab.serializeTimestamp(today);
        var yesterdayString = Teamlab.serializeTimestamp(yesterday);
        var tomorrowString = Teamlab.serializeTimestamp(tomorrow);

        ASC.CRM.ListTaskView.advansedFilter = jq("#tasksAdvansedFilter")
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
                                        type          : "flag",
                                        id            : "my",
                                        apiparamname  : "responsibleID",
                                        title         : "<%=CRMTaskResource.MyTasksFilter %>",
                                        group         : "<%=CRMCommonResource.FilterByResponsible%>",
                                        groupby       : "responsible",
                                        defaultparams : { value: "<%= SecurityContext.CurrentAccount.ID %>" }
                                    },
                                    {
                                        type         : "person",
                                        id           : "responsibleID",
                                        apiparamname : "responsibleID",
                                        title        : "<%=CRMTaskResource.CustomResponsibleFilter%>",
                                        filtertitle  : "<%=CRMCommonResource.FilterByResponsible%>",
                                        group        : "<%=CRMCommonResource.FilterByResponsible%>",
                                        groupby      : "responsible"
                                    },

                                    {
                                        type         : "combobox",
                                        id           : "overdue",
                                        apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                        title        : "<%= CRMTaskResource.OverdueTasksFilter%>",
                                        filtertitle  : "<%= CRMTaskResource.ByDueDate%>",
                                        group        : "<%= CRMTaskResource.ByDueDate %>",
                                        groupby      : "deadline",
                                        options      :
                                                    [
                                                    {value : jq.toJSON(["", yesterdayString]),  classname : '', title : "<%=CRMTaskResource.OverdueTasksFilter%>", def : true},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMTaskResource.TodayTasksFilter%>"},
                                                    {value : jq.toJSON([tomorrowString, ""]),  classname : '', title : "<%= CRMTaskResource.TheNextTasksFilter%>"}
                                                    ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "today",
                                        apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                        title        : "<%= CRMTaskResource.TodayTasksFilter%>",
                                        filtertitle  : "<%= CRMTaskResource.ByDueDate%>",
                                        group        : "<%= CRMTaskResource.ByDueDate%>",
                                        groupby      : "deadline",
                                        options      :
                                                    [
                                                    {value : jq.toJSON(["", yesterdayString]), classname : '', title : "<%=CRMTaskResource.OverdueTasksFilter%>"},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMTaskResource.TodayTasksFilter%>", def : true},
                                                    {value : jq.toJSON([tomorrowString, ""]),  classname : '', title : "<%= CRMTaskResource.TheNextTasksFilter%>"}
                                                    ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "theNext",
                                        apiparamname : jq.toJSON(["fromDate", "toDate"]),
                                        title        : "<%= CRMTaskResource.TheNextTasksFilter%>",
                                        filtertitle  : "<%= CRMTaskResource.ByDueDate%>",
                                        group        : "<%= CRMTaskResource.ByDueDate%>",
                                        groupby      : "deadline",
                                        options      :
                                                    [
                                                    {value : jq.toJSON(["", yesterdayString]),  classname : '', title : "<%=CRMTaskResource.OverdueTasksFilter%>"},
                                                    {value : jq.toJSON([todayString, todayString]),  classname : '', title : "<%= CRMTaskResource.TodayTasksFilter%>"},
                                                    {value : jq.toJSON([tomorrowString, ""]),  classname : '', title : "<%= CRMTaskResource.TheNextTasksFilter%>", def : true}
                                                    ]
                                    },
                                    {
                                        type        : "daterange",
                                        id          : "fromToDate",
                                        title       : "<%= CRMTaskResource.CustomDateFilter%>",
                                        filtertitle : "<%= CRMTaskResource.ByDueDate%>",
                                        group       : "<%= CRMTaskResource.ByDueDate%>",
                                        groupby     : "deadline"
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "openTask",
                                        apiparamname : "isClosed",
                                        title        : "<%= CRMTaskResource.OnlyOpenTasks%>",
                                        filtertitle  : "<%= CRMTaskResource.TasksByStatus %>",
                                        group        : "<%= CRMTaskResource.TasksByStatus %>",
                                        groupby      : "taskStatus",
                                        options      :
                                                [
                                                {value : false,  classname : '', title : "<%= CRMTaskResource.OnlyOpenTasks%>", def : true},
                                                {value : true,  classname : '', title : "<%= CRMTaskResource.OnlyClosedTasks%>"}
                                                ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "closedTask",
                                        apiparamname : "isClosed",
                                        title        : "<%= CRMTaskResource.OnlyClosedTasks%>",
                                        filtertitle  : "<%= CRMTaskResource.TasksByStatus %>",
                                        group        : "<%= CRMTaskResource.TasksByStatus %>",
                                        groupby      : "taskStatus",
                                        options      :
                                                [
                                                {value : false,  classname : '', title : "<%= CRMTaskResource.OnlyOpenTasks%>"},
                                                {value : true,  classname : '', title : "<%= CRMTaskResource.OnlyClosedTasks%>", def : true}
                                                ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "categoryID",
                                        apiparamname : "categoryID",
                                        title        : "<%= CRMCommonResource.ByCategory%>",
                                        group        : "<%= CRMCommonResource.Other %>",
                                        options      : taskCategories,
                                        defaulttitle : "<%= CRMCommonResource.Choose %>"
                                    }
                                ],

                        sorters: [
                                    { id: "title", title: "<%=CRMCommonResource.Title%>", dsc: false, def: false },
                                    { id: "category", title: "<%=CRMCommonResource.Category%>", dsc: false, def: false },
                                    { id: "deadline", title: "<%=CRMTaskResource.DueDate%>", dsc: false, def: true }
                                 ]
                    })
                    .bind("setfilter", ASC.CRM.ListTaskView.setFilter)
                    .bind("resetfilter", ASC.CRM.ListTaskView.resetFilter);
        <% } %>
    });

    jq(document).click(function(event)
    {
        jq.dropdownToggle().registerAutoHide(event, "#mainExportCsv", "#exportDialog");
    });
</script>


<div id="taskButtonsPanel">
    <a class="linkAddMediumText baseLinkAction" onclick="ASC.CRM.TaskActionView.showTaskPanel(0, <%= CurrentContact != null ? CurrentContact.ID : 0 %>, '<%= CurrentEntityType == EntityType.Any ? String.Empty : CurrentEntityType.ToString().ToLower() %>', <%= EntityID %>);">
        <%= CRMTaskResource.AddNewTask%>
    </a>
    <% if (CurrentContact == null && EntityID == 0 && !MobileVer) { %>
    <span class="splitter"></span>
    <a id="importTasksLink" class="crm-importLink" href="tasks.aspx?action=import">
        <%= CRMTaskResource.ImportTasks %>
    </a>
    <% if (CRMSecurity.IsAdmin) %>
    <% { %>
    <span class="splitter"></span>
    <span id="mainExportCsv" title="<%= CRMCommonResource.ExportCurrentListToCsv %>" class="crm-exportToCsvLink"
     onclick="ASC.CRM.ListTaskView.showExportDialog();">
        <a class="baseLinkAction"><%= CRMCommonResource.ExportCurrentListToCsv%></a>
        <img align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID)%>"/>
    </span>
    <% } %>
    <% } %>
</div>

<% if (CurrentContact == null && EntityID == 0) %>
<% { %>
<div id="taskFilterContainer">
    <aswscc:advansedfilter runat="server" id="filter" blockid="tasksAdvansedFilter"></aswscc:advansedfilter>
    <br />
</div>
<% } %>

<div id="taskList"  style='<%= CurrentContact == null && EntityID == 0 ? "min-height: 400px;" : "min-height: 200px;" %>'>
    <table id="taskTable" class="tableBase" cellpadding="7" cellspacing="0" style="table-layout:fixed;">
        <thead>
            <tr>
                <td style="width: 54px;"></td>
                <td style="width: 45px;"></td>
                <td style="width:600px;"></td>
                <td style="width:200px;"></td>
                <td style="width: 38px;"></td>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
    <div id="showMoreTasksButtons">
        <a class="crm-showMoreLink" style="display:none;">
            <%= CRMJSResource.ShowMoreButtonText %>
        </a>
        <a class="crm-loadingLink" style="display:none;">
            <%= CRMJSResource.LoadingProcessing %>
        </a>
    </div>
</div>

<div style="display: none;" id="exportDialog" class="dropDownDialog">
    <div class="dropDownCornerLeft" style="margin-left: -5px;"></div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.ListTaskView.exportToCsv();">
            <%= CRMCommonResource.DownloadOneFile%>
        </a>
        <a class="dropDownItem" onclick="ASC.CRM.ListTaskView.openExportFile();">
            <%= CRMCommonResource.OpenToTheEditor%>
        </a>
    </div>
</div>

<div id="taskStatusListContainer" taskid="">
    <div class="popup-corner"></div>
    <ul>
        <li class="open"><%= CRMTaskResource.TaskStatus_Open %></li>
        <li class="closed"><%= CRMTaskResource.TaskStatus_Closed %></li>
    </ul>
</div>

<div id="taskActionMenu" class="dropDownDialog">
    <div class="dropDownCornerRight">&nbsp;</div>
    <div class="dropDownContent">
        <a class="dropDownItem" id="editTaskLink">
            <%=CRMTaskResource.EditTask%>
        </a>
        <a class="dropDownItem" id="deleteTaskLink">
            <%=CRMTaskResource.DeleteTask%>
        </a>
    </div>
</div>

<ctrl:TaskActionView runat="server"  ID="_taskActionView"/>

<script id="taskTmpl" type="text/x-jquery-tmpl">
    <tr id="task_${id}" class="with-crm-menu">
        <td class="borderBase">
            <div class="check">
                <div class="changeStatusCombobox{{if canEdit == true}} canEdit{{/if}}" taskid="${id}">
                    {{if isClosed == true}}
                        <span class="closed" title="<%= CRMTaskResource.TaskStatus_Closed %>"></span>
                    {{else}}
                        <span class="open" title="<%= CRMTaskResource.TaskStatus_Open %>"></span>
                    {{/if}}
                </div>
            </div>
            <img src="<%= WebImageSupplier.GetAbsoluteWebPath("loader_small.gif", ProductEntryPoint.ID) %>" class="ajax_edit_task" alt="" title="" style="display: none;" />
        </td>

        <td class="borderBase">
            <img title="${category.title}" alt="${category.title}" src="${category.imagePath}" />
        </td>

        <td class="borderBase" style="width:600px;">
            <div class="divForTaskTitle">
                <span id="taskTitle_${id}" class="${classForTitle}"
                    dscr_label="<%=CRMCommonResource.Description%>" dscr_value="${description}"
                    resp_label="<%=CRMCommonResource.Responsible%>" resp_value="${responsible.displayName}">
                    ${title}
                </span>
            </div>
            <div style="padding-top: 5px; display: inline-block;">
                <span class="${classForTaskDeadline}">
                    ${deadLineString}
                </span>
            </div>
        </td>

        <td class="borderBase" style="white-space:nowrap; padding-right:15px;">
            {{if entity != null && ASC.CRM.ListTaskView.EntityID == 0}}
            <div class="divForEntity">
                ${entityType}: <a class="linkMedium" href="${entityURL}">${entity.entityTitle}</a>
            </div>
            {{/if}}

            {{if contact != null && ASC.CRM.ListTaskView.ContactID != contact.id}}
                <div class="divForEntity" {{if entity != null}} style="padding-top: 5px;" {{/if}}>
                {{if contact.isCompany == true}}
                    <a href="default.aspx?id=${contact.id}" data-id="${contact.id}" id="task_${id}_company_${contact.id}" class="linkMedium crm-companyInfoCardLink">
                        ${contact.displayName}
                    </a>
                {{else}}
                    <a href="default.aspx?id=${contact.id}&type=people" data-id="${contact.id}" id="task_${id}_person_${contact.id}" class="linkMedium crm-peopleInfoCardLink">
                        ${contact.displayName}
                    </a>
                {{/if}}
                </div>
            {{/if}}
        </td>

        <td class="borderBase">
        {{if canEdit == true}}
            <div id="taskMenu_${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>"
                 onclick="ASC.CRM.ListTaskView.showActionMenu(${id},
                         {{if contact != null}}
                            ${contact.id}
                         {{else}}
                            0
                         {{/if}}
                            ,
                         {{if entity != null}}
                            '${entity.entityType}', ${entity.entityId}
                         {{else}}
                            '', 0
                         {{/if}});" >&nbsp;</div>
        {{/if}}
        </td>
    </tr>
</script>


<aswscc:EmptyScreenControl ID="emptyContentForTasksFilter" runat="server"></aswscc:EmptyScreenControl>

<aswscc:EmptyScreenControl ID="tasksEmptyScreen" runat="server"></aswscc:EmptyScreenControl>

<div id="files_hintCategoriesPanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=CRMTaskResource.TooltipCategories%>
    <a href="http://www.teamlab.com/help/tipstricks/tasks-categories.aspx" target="_blank"><%=CRMCommonResource.ButtonLearnMore%></a>
</div>