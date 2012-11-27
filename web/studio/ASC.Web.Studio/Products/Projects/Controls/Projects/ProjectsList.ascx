<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectsList.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Projects.ProjectsList" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("jquery.tmpl.min.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("common_filter_projects.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("allprojects.js") %>"></script>

<script type="text/javascript">
        jq(document).ready(function() {
            allProject.init("<%= ASC.Core.SecurityContext.CurrentAccount.ID %>");
        });
        function createAdvansedFilter(tags) {
            jq('#AdvansedFilter').advansedFilter(
            {
            	store: true,
                anykey: true,
                colcount : 2,
                anykeytimeout: 1000,
                filters: 
                [
                    // Team member
                    {
                        type: "person",
                        id: "me_team_member",
                        title: "<%=ProjectsFilterResource.Me %>",
                        filtertitle: "<%= ProjectsFilterResource.TeamMember%>:",
                        group: "<%= ProjectsFilterResource.TeamMember%>",
                        hashmask: "person/{0}",
                        groupby: "userid",
                        bydefault: { id: "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>" }
                    },
                    {
                        type: "person",
                        id: "team_member",
                        filtertitle: "<%= ProjectsFilterResource.TeamMember%>:",
                        title: "<%= ProjectsFilterResource.OtherUsers%>",
                        group: "<%= ProjectsFilterResource.TeamMember%>",
                        hashmask: "person/{0}",
                        groupby: "userid"
                    },
                    // Project manager
                    {
                        type: "person",
                        id: "me_project_manager",
                        title: "<%=ProjectsFilterResource.Me %>",
                        filtertitle: "<%= ProjectsFilterResource.ProjectMenager%>:",
                        group: "<%= ProjectsFilterResource.ProjectMenager%>",
                        hashmask: "person/{0}",
                        groupby: "userid",
                        bydefault: { id: "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>" }
                    },
                    {
                        type: "person",
                        id: "project_manager",
                        filtertitle: "<%= ProjectsFilterResource.ProjectMenager%>:",
                        title: "<%= ProjectsFilterResource.OtherUsers%>",
                        group: "<%= ProjectsFilterResource.ProjectMenager%>",
                        hashmask: "person/{0}",
                        groupby: "userid"
                    },
                    //Status
                    { 
                        type: "combobox",
                        id: "open",
                        title: "<%=ProjectsFilterResource.StatusOpenProject %>",
                        filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                        group: "<%= ProjectsFilterResource.ByStatus %>",
                        hashmask: "combobox/{0}",
                        groupby: "status",
                        options: 
                                [
                                    { value : "open", title : "<%= ProjectsFilterResource.StatusOpenProject %>", def : true },
                                    { value : "paused", title : "<%= ProjectsFilterResource.StatusSuspend %>" },
                                    { value : "closed", title : "<%= ProjectsFilterResource.StatusClosedProject %>" }
                                ]
                    },
                    { 
                        type: "combobox",
                        id: "paused",
                        title: "<%=ProjectsFilterResource.StatusSuspend %>",
                        filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                        group: "<%= ProjectsFilterResource.ByStatus %>",
                        hashmask: "combobox/{0}",
                        groupby: "status",
                        options: 
                                [
                                    { value : "open", title : "<%= ProjectsFilterResource.StatusOpenProject %>" },
                                    { value : "paused", title : "<%= ProjectsFilterResource.StatusSuspend %>", def : true },
                                    { value : "closed", title : "<%= ProjectsFilterResource.StatusClosedProject %>" }
                                ]
                    },
                    { 
                        type: "combobox",
                        id: "closed",
                        title: "<%=ProjectsFilterResource.StatusClosedProject %>",
                        filtertitle: "<%= ProjectsFilterResource.ByStatus %>:",
                        group: "<%= ProjectsFilterResource.ByStatus %>",
                        hashmask: "combobox/{0}",
                        groupby: "status",
                        options: 
                                [
                                    { value : "open", title : "<%= ProjectsFilterResource.StatusOpenProject %>" },
                                    { value : "paused", title : "<%= ProjectsFilterResource.StatusSuspend %>" },
                                    { value : "closed", title : "<%= ProjectsFilterResource.StatusClosedProject %>", def : true }
                                ]
                    },
                    // Other
                    { 
                    	type: "flag",
                    	id: "followed",
                    	title: "<%=ProjectsFilterResource.FollowProjects%>",
                    	group: "<%= ProjectsFilterResource.Other%>",
                    	hashmask: "followed"
                    },
                    {
                        type: "combobox",
                        id: "tag",
                        title: "<%=ProjectsFilterResource.ByTag %>",
                        filtertitle: "<%=ProjectsFilterResource.Tag %>:",
                        group: "<%= ProjectsFilterResource.Other%>",
                        hashmask: "combobox/{0}",
                        options: tags,
                        defaulttitle: "<%= ProjectsFilterResource.Select%>"
                    }
                ],
                sorters:
                [
                    { id: "title", title: "<%=ProjectsFilterResource.ByTitle %>", sortOrder: "ascending", def: true},
                    { id: "create_on", title: "<%=ProjectsFilterResource.ByCreateDate %>", sortOrder: "descending" }
                  ]
                }
          )
          .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
          .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

          ASC.Projects.ProjectsAdvansedFilter.init = true;
   }
</script>

<div id="questionWindowTasks" style="display: none">
    <ascw:Container ID="_hintPopupTasks" runat="server">
        <header>
    <%= ProjectResource.CloseProject%>
    </header>
        <body>
            <p>
                <%=ProjectResource.NotClosePrjWithActiveTasks%></p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton" id="linkToTasks">
                    <%= ProjectResource.ViewActiveTasks%></a> <span class="button-splitter"></span>
                <a class="grayLinkButton">
                    <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </body>
    </ascw:Container>
</div>
<div id="questionWindowMilestone" style="display: none">
    <ascw:Container ID="_hintPopupMilestones" runat="server">
        <header>
    <%= ProjectResource.CloseProject%>
    </header>
        <body>
            <p>
                <%=ProjectResource.NotClosedPrjWithActiveMilestone%></p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton" id="linkToMilestines">
                    <%= ProjectResource.ViewActiveMilestones%></a> <span class="button-splitter">
                </span><a class="grayLinkButton cancel">
                    <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </body>
    </ascw:Container>
</div>
<div id="tableListProjectsContainer">
    <p class="noProject">
    </p>
    <table id="tableListProjects" class="pm-tablebase">
        <thead>
            <tr>
                <th class="action">
                </th>
                <th class="nameProject">
                    <%=ProjectsCommonResource.Title %>
                </th>
                <th class="milestoneCount">
                    <%=ProjectsCommonResource.Milestones %>
                </th>
                <th class="taskCount">
                    <%=TaskResource.Tasks%>
                </th>
                <th class="responsible">
                    <%=ProjectResource.ProjectLeader %>
                </th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>
<span id="showNextProjects">
    <%=ProjectResource.ShowNextProjects %></span>
<div class="loaderProjects">
</div>

<script id="projTmpl" type="text/x-jquery-tmpl">
         {{if status != 'open'}}
            <tr class="noActiveProj" id = "${id}">
         {{else}}
            <tr id = "${id}">
         {{/if}}
                <td class="action">
                    <div id = "statusContainer_${id}" class="statusContainer">
                      {{if canEdit}}
                        <span id="statusCombobox_${id}" class="canEdit">
                            {{if status == 'closed'}}
                                <span class="${status}" title="<%=ProjectResource.ClosedProject%>"></span>
                            {{else}}
                                <span class="${status}" title="<%=ProjectResource.ActiveProject%>"></span>
                            {{/if}}
                        </span>
                      {{else}}
                           <span class="noEdit" id="statusCombobox_${id}">
                                {{if status == 'closed'}}
                                    <span class="${status}" title="<%=ProjectResource.ClosedProject%>"></span>
                                {{else}}
                                    {{if status == 'open'}}
                                        <span class="${status}" title="<%=ProjectResource.ActiveProject%>"></span>
                                    {{else}}
                                        <span class="${status}" title="<%=ProjectResource.PausedProject%>"></span>
                                    {{/if}}
                                {{/if}}
                           </span> 
                      {{/if}}
                    </div>
                </td>
                <td class="nameProject">
                    {{if privateProj}}
                        <span title="<%=ProjectResource.HiddenProject %>" class="private"></span>
                    {{/if}}
                    <a href="${projLink}" createdby="${createdBy}" created="${created}" projectid="${id}">${title}</a>
                    <span class="description">${description}</span>
                </td>
                <td class="milestoneCount">
                    {{if milestones != 0}}
                        <a href="${linkMilest}">${milestones}</a>
                    {{/if}}
                </td>
                <td class="taskCount">
                    {{if tasks != 0}}
                        <a href="${linkTasks}">${tasks}</a>
                    {{/if}}
                </td>
                <td class="responsible">
                    {{if allProject.myGuid == responsibleId}}
                        <span id = "${responsibleId}" class='userLink my'><%=ProjectResource.Me %></span>
                    {{else}}
                        <span id = "${responsibleId}" class='userLink' title="${responsible}">${responsible}</span>
                    {{/if}}
                    {{if participants != 0}}
                        <a class="participants" href="${linkParticip}">+${participants}</a>
                    {{/if}}
                </td>
            </tr>
</script>

<div class="comboboxContainer" id="containerStatusList">
    <div class="containerTop">
    </div>
    <ul id="statusList">
        <li class="open" onclick="allProject.changeStatus(this);">
            <%=ProjectResource.ActiveProject%></li>
        <li class="paused" onclick="allProject.changeStatus(this);">
            <%=ProjectResource.PausedProject%></li>
        <li class="closed" onclick="allProject.changeStatus(this);">
            <%=ProjectResource.ClosedProject%></li>
    </ul>
</div>

<div id="projectDescrPanel" class="actionPanel" objid="">
	<div class="popup-corner"></div>
   <div class="created">
	<div class="param"><%= TaskResource.CreatingDate%>:</div>
    <div class="value"></div>
  </div>
  <div class="descr">  
     <div class="param"><%= TaskResource.Description%>:</div>
     <div class="value">
        <div class="descrValue">
        </div>
        <a class="readMore"><%=ProjectsCommonResource.ReadMore %></a>
     </div>
  </div>
</div> 