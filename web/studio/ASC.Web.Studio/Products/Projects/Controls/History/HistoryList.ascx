<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryList.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.History.HistoryList" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("jquery.tmpl.min.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("history.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("common_filter_projects.js") %>"></script>

<script type="text/javascript">
    jq(document).ready(function() {
        history2.init('<%= SecurityContext.CurrentAccount.ID %>');
    });
</script>

<script type="text/javascript">
    function createAdvansedFilter(projects) {
    	var now = new Date();
        
        var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        
        var yearsterday = new Date(today);
        yearsterday.setDate(today.getDate() - 1);

        var startWeek = new Date(today);
    	startWeek.setDate(today.getDate() - today.getDay() + 1);
    	
    	var endWeek = new Date(today);
    	endWeek.setDate(today.getDate() - today.getDay() + 7);
    	
    	var startPreviousWeek = new Date(startWeek);
    	startPreviousWeek.setDate(startWeek.getDate() - 7);
        
    	var endPreviousWeek = new Date(startWeek);
    	endPreviousWeek.setDate(startWeek.getDate() - 1);
        
    	var startMonth = new Date(today);
    	startMonth.setDate(1);

    	var endMonth = new Date(startMonth);
    	endMonth.setMonth(startMonth.getMonth() + 1);
    	endMonth.setDate(endMonth.getDate() - 1);
    	
    	var startPreviousMonth = new Date(today);
    	startPreviousMonth.setMonth(today.getMonth() - 1);
    	startPreviousMonth.setDate(1);
        
    	var endPreviousMonth = new Date(startPreviousMonth);
    	endPreviousMonth.setMonth(startPreviousMonth.getMonth() + 1);
    	endPreviousMonth.setDate(endPreviousMonth.getDate() - 1);
    	
    	var startYear = new Date(today);
    	startYear.setMonth(0);
    	startYear.setDate(1);
    	
    	var endYear = new Date(startYear);
    	endYear.setFullYear(startYear.getFullYear() + 1);
    	endYear.setDate(endYear.getDate() - 1);
    	
    	var startPreviousYear = new Date(startYear);
    	startPreviousYear.setFullYear(startYear.getFullYear()- 1);
    	
    	var endPreviousYear = new Date(startYear);
    	endPreviousYear.setDate(startYear.getDate() - 1);
        
        
    	today = today.getTime();
    	yearsterday = yearsterday.getTime();
    	startWeek = startWeek.getTime();
    	endWeek = endWeek.getTime();
    	startPreviousWeek = startPreviousWeek.getTime();
    	endPreviousWeek = endPreviousWeek.getTime();
    	startMonth = startMonth.getTime();
    	endMonth = endMonth.getTime();    	
    	startPreviousMonth = startPreviousMonth.getTime();
    	endPreviousMonth = endPreviousMonth.getTime();    	
    	startYear = startYear.getTime();
    	endYear = endYear.getTime();
    	startPreviousYear = startPreviousYear.getTime();
    	endPreviousYear = endPreviousYear.getTime();
        
        jq('#AdvansedFilter').advansedFilter(
        {
        	store: true,
            anykey: true,
            colcount: 2,
            anykeytimeout: 1000,
            filters:
            [
                //Time Period
                { 
                    type: "daterange",
                    id: "today3",
                    title: "<%= ProjectsFilterResource.Today %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : today, to : today}
                },
                { 
                    type: "daterange",
                    id: "yesterday",
                    title: "<%= ProjectsFilterResource.Yesterday %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : yearsterday, to : yearsterday}
                },
                {
                    type: "daterange",
                    id: "currentweek",
                    title: "<%= ProjectsFilterResource.CurrentWeek %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : startWeek, to : endWeek}
                },
            	{
                    type: "daterange",
                    id: "previousweek",
                    title: "<%= ProjectsFilterResource.PreviousWeek %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : startPreviousWeek, to : endPreviousWeek}
                },
                {
                    type: "daterange",
                    id: "currentmonth",
                    title: "<%= ProjectsFilterResource.CurrentMonth %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : startMonth, to : endMonth}
                },
            	{
                    type: "daterange",
                    id: "previousmonth",
                    title: "<%= ProjectsFilterResource.PreviousMonth %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : startPreviousMonth, to : endPreviousMonth}
                },
                {
                    type: "daterange",
                    id: "currentyear",
                    title: "<%= ProjectsFilterResource.CurrentYear %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : startYear, to : endYear}
                },
            	{
                    type: "daterange",
                    id: "previousyear",
                    title: "<%= ProjectsFilterResource.PreviousYear %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period",
                    bydefault: {from : startPreviousYear, to : endPreviousYear}
                },
            	{
                    type: "daterange",
                    id: "period",
                    title: "<%= ProjectsFilterResource.CustomPeriod %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.TimePeriod %>",
                    hashmask: "period/{0}/{1}",
                    groupby: "period"
                },
            	
            	// Entity
                { 
                    type: "combobox",
                    id: "project_entity",
                    title: "<%= ProjectsFilterResource.Project %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>", def : true },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                            { value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "milestone_entity",
                    title: "<%= ProjectsFilterResource.Milestone %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>", def : true },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "discussion_entity",
                    title: "<%= ProjectsFilterResource.Discussion %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>", def : true },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "team_entity",
                    title: "<%= ProjectsFilterResource.Team %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>", def : true },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "task_entity",
                    title: "<%= ProjectsFilterResource.Task %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>", def : true },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "subtask_entity",
                    title: "<%= ProjectsFilterResource.Subtask %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>", def : true },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "timespend_entity",
                    title: "<%= ProjectsFilterResource.Time %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>", def : true },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>" }
                        ]
                },
            	{ 
                    type: "combobox",
                    id: "comment_entity",
                    title: "<%= ProjectsFilterResource.Comment %>",
                    filtertitle: "<%= ProjectsFilterResource.ByType %>:",
                    group: "<%= ProjectsFilterResource.ByType %>",
                    hashmask: "combobox/{0}",
                    groupby: "entity",
                    options: 
                        [
                            { value : "project", title : "<%= ProjectsFilterResource.Project %>" },
                            { value : "milestone", title : "<%= ProjectsFilterResource.Milestone %>" },
                            { value : "message", title : "<%= ProjectsFilterResource.Discussion %>" },
                            { value : "team", title : "<%= ProjectsFilterResource.Team %>" },
                            { value : "task", title : "<%= ProjectsFilterResource.Task %>" },
                            { value : "subtask", title : "<%= ProjectsFilterResource.Subtask %>" },
                        	{ value : "timespend", title : "<%= ProjectsFilterResource.Time %>" },
                            { value : "comment", title : "<%= ProjectsFilterResource.Comment %>", def : true }
                        ]
                },
                 <% if (!RequestContext.IsInConcreteProject()){ %>
                // Projects
                {
                    type: "combobox",
                    id: "project",
                    title: "<%= ProjectsFilterResource.OtherProjects %>",
                    filtertitle: "<%= ProjectsFilterResource.ByProject %>:",
                    group: "<%= ProjectsFilterResource.ByProject %>",
                    options: projects,
                    groupby: "projects",
                    defaulttitle: "<%= ProjectsFilterResource.Select%>"
                },
                <% } %>
                // User
                {
                    type: "person",
                    id: "me_user",
                    title: "<%= ProjectsFilterResource.Me %>",
                    filtertitle: "<%= ProjectsFilterResource.ByUser %>:",
                    group: "<%= ProjectsFilterResource.ByUser %>",
                    hashmask: "user/{0}",
                    groupby: "userid",
                    bydefault: { id: "<%= SecurityContext.CurrentAccount.ID %>" }
                },
                {
                    type: "person",
                    id: "user",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.ByUser %>:",
                    group: "<%= ProjectsFilterResource.ByUser %>",
                    hashmask: "user/{0}",
                    groupby: "userid"
                }
            ],
            sorters:
            [
            	{ id: "id", title: "<%= ProjectsFilterResource.ByDate %>", sortOrder: "descending", def: true },
            	{ id: "title", title: "<%= ProjectsFilterResource.ByTitle %>", sortOrder: "ascending" }
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

      ASC.Projects.ProjectsAdvansedFilter.init = true;
    }
</script>

<script id="activityTemplate" type="text/x-jquery-tmpl">
    <tr>
        <td class="date"><span>${date}</span></td>
        <td class="entityType"><span data-entity="${entityType}">${entityTitle}</span></td>
        <td class="title"><a data-projectId="${projectId}" data-projectTitle="${projectTitle}" href="${url}">${title}</a></td>
        <td class="actionText"><span>${actionText}</span></td>
        <td class="user"><span title="${userName}" data-userId=${userId}>${userName}</span></td>
    </tr>
</script>

