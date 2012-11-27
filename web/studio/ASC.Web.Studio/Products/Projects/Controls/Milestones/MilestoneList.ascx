<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestoneList.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Milestones.MilestoneList" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("jquery.tmpl.min.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("milestones.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("common_filter_projects.js") %>"></script>

<script type="text/javascript">
    jq(document).ready(function() {
        milestones.init('<%= ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID) %>', '<%= SecurityContext.CurrentAccount.ID %>');
    });
</script>

<% if (RequestContext.IsInConcreteProject())
   { %>

<script type="text/javascript">
    function createAdvansedFilter() {
    	var now = new Date();
        var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
    	var inWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
    	inWeek.setDate(inWeek.getDate() + 7);
        
        jq('#AdvansedFilter').advansedFilter(
        {
        	store: true,
            anykey: true,
            colcount: 2,
            anykeytimeout: 1000,
            filters:
            [
                // Responsible
                {
                    type: "person",
                    id: "me_responsible_for_milestone",
                    title: "<%= ProjectsFilterResource.Me %>",
                    filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                    group: "<%= ProjectsFilterResource.ByResponsible %>",
                    hashmask: "person/{0}",
                    groupby: "userid",
                    bydefault: { id: "<%= SecurityContext.CurrentAccount.ID %>" }
                },
                {
                    type: "person",
                    id: "responsible_for_milestone",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                    group: "<%= ProjectsFilterResource.ByResponsible %>",
                    hashmask: "person/{0}",
                    groupby: "userid"
                },
                //Tasks
                {
                    type: "person",
                    id: "me_tasks",
                    title: "<%= ProjectsFilterResource.MyTasks %>",
                    filtertitle: "<%= ProjectsFilterResource.Tasks %>:",
                    group: "<%= ProjectsFilterResource.Tasks %>",
                    hashmask: "person/{0}",
                    groupby: "taskuserid",
                    bydefault: { id: "<%= SecurityContext.CurrentAccount.ID %>"}
                },
                {
                    type: "person",
                    id: "user_tasks",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.Tasks %>:",
                    group: "<%= ProjectsFilterResource.Tasks %>",
                    hashmask: "person/{0}",
                    groupby: "taskuserid"
                },
                // Status
                { 
                    type: "combobox",
                    id: "open",
                    title: "<%= ProjectsFilterResource.StatusOpenMilestone %>",
                    filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                    group: "<%= ProjectsFilterResource.ByStatus %>",
                    hashmask: "combobox/{0}",
                    groupby: "status",
                    options: 
                        [
                            { value : "open", title : "<%= ProjectsFilterResource.StatusOpenMilestone %>", def : true },
                            { value : "closed", title : "<%= ProjectsFilterResource.StatusClosedMilestone %>" }
                        ]
                },
                { 
                    type: "combobox",
                    id: "closed",
                    title: "<%= ProjectsFilterResource.StatusClosedMilestone %>",
                    filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                    group: "<%= ProjectsFilterResource.ByStatus %>",
                    hashmask: "combobox/{0}",
                    groupby: "status",
                    options: 
                        [
                            { value : "open", title : "<%= ProjectsFilterResource.StatusOpenMilestone %>" },
                            { value : "closed", title : "<%= ProjectsFilterResource.StatusClosedMilestone %>", def : true }
                        ]
                        },
                //Due date
                { 
                    type: "flag",
                    id: "overdue",
                    title: "<%= ProjectsFilterResource.Overdue %>",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "overdue",
                    groupby: "deadline"
                },
                { 
                    type: "daterange",
                    id: "today",
                    title: "<%= ProjectsFilterResource.Today %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "deadline/{0}/{1}",
                    groupby: "deadline",
                    bydefault: {from : today.getTime(), to : today.getTime()}
                },
                { 
                    type: "daterange",
                    id: "upcoming",
                    title: "<%= ProjectsFilterResource.UpcomingMilestones %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "deadline/{0}/{1}",
                    groupby: "deadline",
                    bydefault: {from : today.getTime(), to : inWeek.getTime()}
                },
                {
                    type: "daterange",
                    id: "deadline",
                    title: "<%= ProjectsFilterResource.CustomPeriod %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "deadline/{0}/{1}",
                    groupby: "deadline"
                }
            ],
            sorters:
            [
                { id: "deadline", title: "<%= ProjectsFilterResource.ByDeadline %>", sortOrder: "ascending", def: true },
                { id: "create_on", title: "<%= ProjectsFilterResource.ByCreateDate %>", sortOrder: "descending" },
                { id: "title", title: "<%= ProjectsFilterResource.ByTitle %>", sortOrder: "ascending" }
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

      ASC.Projects.ProjectsAdvansedFilter.init = true;
    }
</script>

<% }
   else
   { %>

<script type="text/javascript">
    function createAdvansedFilter(tags, projects) {
        var now = new Date();
        var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
    	var inWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
    	inWeek.setDate(inWeek.getDate() + 7);
        
        jq('#AdvansedFilter').advansedFilter(
        {
        	store: true,
            anykey: true,
            colcount: 2,
            anykeytimeout: 1000,
            filters:
            [
                // Responsible
                { 
                    type: "person",
                    id: "me_responsible_for_milestone",
                    title: "<%= ProjectsFilterResource.Me %>",
                    filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                    group: "<%= ProjectsFilterResource.ByResponsible %>",
                    hashmask: "person/{0}",
                    groupby: "userid",
                    bydefault: { id: "<%= SecurityContext.CurrentAccount.ID %>" }
                },
                { 
                    type: "person",
                    id: "responsible_for_milestone",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                    group: "<%= ProjectsFilterResource.ByResponsible %>",
                    hashmask: "person/{0}",
                    groupby: "userid"
                },
                //Tasks
                {
                    type: "person",
                    id: "me_tasks",
                    title: "<%= ProjectsFilterResource.MyTasks %>",
                    filtertitle: "<%= ProjectsFilterResource.Tasks %>:",
                    group: "<%= ProjectsFilterResource.Tasks %>",
                    hashmask: "person/{0}",
                    groupby: "taskuserid",
                    bydefault: { id: "<%= SecurityContext.CurrentAccount.ID %>" }
                },
                {
                    type: "person",
                    id: "user_tasks",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.Tasks %>:",
                    group: "<%= ProjectsFilterResource.Tasks %>",
                    hashmask: "person/{0}",
                    groupby: "taskuserid"
                },
                // Projects
                {
                    type: "flag",
                    id: "myprojects",
                    title: "<%= ProjectsFilterResource.MyProjects %>",
                    group: "<%= ProjectsFilterResource.ByProject %>",
                    hashmask: "myprojects",
                    groupby: "projects"
                },
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
                {
                    type: "combobox",
                    id: "tag",
                    title: "<%= ProjectsFilterResource.ByTag %>",
                    filtertitle: "<%=ProjectsFilterResource.Tag %>:",
                    group: "<%= ProjectsFilterResource.ByProject %>",
                    options: tags,
                    groupby: "projects",
                    defaulttitle: "<%= ProjectsFilterResource.Select%>"
                },
                // Status
                                {
                                type: "combobox",
                                id: "open",
                                title: "<%= ProjectsFilterResource.StatusOpenMilestone %>",
                                filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                                group: "<%= ProjectsFilterResource.ByStatus %>",
                                hashmask: "combobox/{0}",
                                groupby: "status",
                                options:
                        [
                            { value: "open", title: "<%= ProjectsFilterResource.StatusOpenMilestone %>", def: true },
                            { value: "closed", title: "<%= ProjectsFilterResource.StatusClosedMilestone %>" }
                        ]
                            },
                {
                    type: "combobox",
                    id: "closed",
                    title: "<%= ProjectsFilterResource.StatusClosedMilestone %>",
                    filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                    group: "<%= ProjectsFilterResource.ByStatus %>",
                    hashmask: "combobox/{0}",
                    groupby: "status",
                    options:
                        [
                            { value: "open", title: "<%= ProjectsFilterResource.StatusOpenMilestone %>" },
                            { value: "closed", title: "<%= ProjectsFilterResource.StatusClosedMilestone %>", def: true }
                        ]
                },
                // Due date
                { 
                    type: "flag",
                    id: "overdue",
                    title: "<%= ProjectsFilterResource.Overdue %>",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "overdue",
                    groupby: "deadline"
                },
                { 
                    type: "daterange",
                    id: "today",
                    title: "<%= ProjectsFilterResource.Today %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "deadline/{0}/{1}",
                    groupby: "deadline",
                    bydefault: {from : today.getTime(), to : today.getTime()}
                },
                { 
                    type: "daterange",
                    id: "upcoming",
                    title: "<%= ProjectsFilterResource.UpcomingMilestones %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "deadline/{0}/{1}",
                    groupby: "deadline",
                    bydefault: {from : today.getTime(), to : inWeek.getTime()}
                },
                {
                    type: "daterange",
                    id: "deadline",
                    title: "<%= ProjectsFilterResource.CustomPeriod %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.DueDate %>",
                    hashmask: "deadline/{0}/{1}",
                    groupby: "deadline"
                }
            ],
            sorters:
            [
                { id: "deadline", title: "<%= ProjectsFilterResource.ByDeadline %>", sortOrder: "ascending", def: true },
                { id: "create_on", title: "<%= ProjectsFilterResource.ByCreateDate %>", sortOrder: "descending" },
                { id: "title", title: "<%= ProjectsFilterResource.ByTitle %>", sortOrder: "ascending" }
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

      ASC.Projects.ProjectsAdvansedFilter.init = true;
    }
</script>

<% } %>

<script id="milestoneTemplate" type="text/x-jquery-tmpl">
    <tr id = "${id}" class="${status}" isKey="${isKey}" isNotify="${isNotify}">
        <td class="status">
            {{if canEdit}}
                <div class="changeStatusCombobox canEdit" milestoneId="${id}">
                {{if status == 'closed'}}
                    <span class="${status}" title="<%= MilestoneResource.StatusClosed %>"></span>
                {{else}}
                    <span class="${status}" title="<%= MilestoneResource.StatusOpen %>"></span>
                {{/if}}
                </div>
            {{else}}
                <div class="changeStatusCombobox noEdit">
                {{if status == 'closed'}}
                    <span class="${status}" title="<%= MilestoneResource.StatusClosed %>"></span>
                {{else}}
                    <span class="${status}" title="<%= MilestoneResource.StatusOpen %>"></span>
                {{/if}}
                </div>
            {{/if}}
        </td>
        <td class="title">
            <div>
                {{if isKey == true}}
                    <img src="<%=PathProvider.GetFileStaticRelativePath("key.png")%>"
                        alt="<%= MilestoneResource.RootMilestone %>" title="<%= MilestoneResource.RootMilestone %>"/>
                {{/if}}
                <a href="javascript:void(0)" projectId="${projectId}" projectTitle="${projectTitle}" createdById="${createdById}" createdBy="${createdBy}"
                    created="${created}" description="${description}">${title}</a>
            </div>
        </td>
        <td class="activeTasksCount">
            {{if activeTasksCount != 0 || closedTasksCount != 0}}
                <a href="${activeTasksLink}">${activeTasksCount}</a>
            {{/if}}
        </td>
        <td class="slash">
            {{if activeTasksCount != 0 || closedTasksCount != 0}}
                /
            {{/if}}
        </td>
        <td class="closedTasksCount">
            {{if activeTasksCount != 0 || closedTasksCount != 0}}
                <a href="${closedTasksLink}">${closedTasksCount}</a>
            {{/if}}
        </td>
        <td class="deadline">
            {{if status == "overdue"}}  
                <span class="overdue">${deadline}</span>
            {{else}}
                <span>${deadline}</span>
            {{/if}}
        </td>
        <td class="responsible">
            <div>
                {{if responsibleId == milestones.getMyGuid()}}  
                    <span style="font-weight: 700; color: #666666" responsibleId='${responsibleId}'><%= MilestoneResource.My %></span>
                {{else}}
                    <span responsibleId='${responsibleId}' title="${responsible}">${responsible}</span>
                {{/if}}
            </div>
        </td>
        <td class="actions">
            {{if canEdit}}
                <div class="milestoneActionsLink" milestoneId="${id}" projectId="${projectId}" status="${status}"></div>
            {{/if}}
        </td>
    </tr>
</script>

