<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="Tasks.aspx.cs" Inherits="ASC.Web.Projects.Tasks" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ MasterType  TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>


<asp:Content ID="Content1" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js") %>"></script>
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("timetracking.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BTPageContentWithoutCommonContainer" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BTPageContent" runat="server">
<% var taskId = Request["id"];
    if(taskId == null) {%>

         <script type="text/javascript">
            var now = new Date();
            var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
            var inWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
            inWeek.setDate(inWeek.getDate() + 7);

             function createAdvansedFilter(milestones, tags, projects) {
                 jq('#AdvansedFilter').advansedFilter(
                     {
                         store: true,
                         anykey: true,
                         colcount : 2,
                         anykeytimeout: 1000,
                         filters: [
                             {
                                 type: "person",
                                 id: "me_tasks_rasponsible",
                                 title: "<%= ProjectsFilterResource.Me %>",
                                 filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                                 group: "<%= ProjectsFilterResource.ByResponsible %>",
                                 hashmask: "person/{0}",
                                 groupby: "userid",
                                 bydefault: { id: "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>" }
                             },
                             {
                                 type: "person",
                                 id: "tasks_rasponsible",
                                 title: "<%= ProjectsFilterResource.OtherUsers %>",
                                 filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                                 group: "<%= ProjectsFilterResource.ByResponsible %>",
                                 hashmask: "person/{0}",
                                 groupby: "userid"
                             },
                             {
                                 type: "group",
                                 id: "group",
                                 title: "<%= ProjectsFilterResource.Groups%>",
                                 filtertitle: "<%= ProjectsFilterResource.Group%>:",
                                 group: "<%= ProjectsFilterResource.ByResponsible%>",
                                 hashmask: "group/{0}",
                                 groupby: "userid"
                             },
                             <% if (!RequestContext.IsInConcreteProject()){ %>
                             {
                                 type: "flag",
                                 id: "myprojects",
                                 title: "<%= ProjectsFilterResource.MyProjects %>",
                                 group: "<%= ProjectsFilterResource.ByProject %>",
                                 hashmask: "myprojects",
                                 groupby: "projects",
                                 defaulttitle: "<%= ProjectsFilterResource.Select%>"
                             },
                             {
                                 type: "combobox",
                                 id: "project",
                                 title: "<%=ProjectsFilterResource.OtherProjects %>",
                                 filtertitle: "<%=ProjectsFilterResource.ByProject %>:",
                                 group: "<%=ProjectsFilterResource.ByProject %>",
                                 hashmask: "project/{0}",
                                 groupby: "projects",
                                 defaulttitle: "<%= ProjectsFilterResource.Select%>",
                                 options: projects
                             },
                             {
                                 type: "combobox",
                                 id: "tag",
                                 title: "<%= ProjectsFilterResource.ByTag%>",
                                 filtertitle: "<%= ProjectsFilterResource.Tag%>:",
                                 group: "<%= ProjectsFilterResource.ByProject%>",
                                 hashmask: "tag/{0}",
                                 groupby: "projects",
                                 options: tags,
                                 defaulttitle: "<%= ProjectsFilterResource.Select%>"
                             },
                            <%} %>
                 {
                     type: "combobox",
                     id:"open",
                     title: "<%= ProjectsFilterResource.StatusOpenTask %>",
                     filtertitle: "<%= ProjectsFilterResource.ByStatus%>:",
                     group: "<%= ProjectsFilterResource.ByStatus%>",
                     hashmask: "combobox/{0}",
                     groupby: "status",
                     options:
                     [
                         { value: "open", classname: "class1", title: "<%= ProjectsFilterResource.StatusOpenTask %>", def: true },
                         { value: "closed", classname: "class2", title: "<%= ProjectsFilterResource.StatusClosedTask %>" }
                     ]
                 },
                 {
                     type: "combobox",
                     id: "closed",
                     title: "<%= ProjectsFilterResource.StatusClosedTask %>",
                     filtertitle: "<%= ProjectsFilterResource.ByStatus%>:",
                     group: "<%= ProjectsFilterResource.ByStatus%>",
                     hashmask: "combobox/{0}",
                     groupby: "status",
                     options:
                     [
                         { value: "open", classname: "class1", title: "<%= ProjectsFilterResource.StatusOpenTask %>" },
                         { value: "closed", classname: "class2", title: "<%= ProjectsFilterResource.StatusClosedTask %>", def: true }
                     ]
                 },
                 {
                     type: "flag",
                     id: "mymilestones",
                     title: "<%= ProjectsFilterResource.MyMilestones%>",
                     group: "<%= ProjectsFilterResource.ByMilestone%>",
                     hashmask: "mymilestones",
                     groupby: "milestones"
                 },
                 {
                     type: "combobox",
                     id: "milestone",
                     title: "<%= ProjectsFilterResource.OtherMilestones%>",
                     filtertitle: "<%= ProjectsFilterResource.ByMilestone%>:",
                     group: "<%= ProjectsFilterResource.ByMilestone%>",
                     hashmask: "milestone/{0}",
                     groupby: "milestones",
                     options: milestones,
                     defaulttitle: "<%= ProjectsFilterResource.Select%>"
                 },
                 {
                     type: "flag",
                     id: "noresponsible",
                     title: "<%= ProjectsFilterResource.NoResponsible %>",
                     group: "<%= ProjectsFilterResource.ByResponsible%>",
                     hashmask: "closed",
                     groupby: "userid"
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
                     title: "<%= ProjectsFilterResource.Upcoming %>",
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
                 sorters: [
                     { id: "deadline", title: "<%= ProjectsFilterResource.ByDeadline%>", sortOrder: "ascending", def: true },
                     { id: "priority", title: "<%= ProjectsFilterResource.ByPriority%>", sortOrder: "descending" },
                     { id: "create_on", title: "<%= ProjectsFilterResource.ByCreateDate%>", sortOrder: "descending" },
                     { id: "title", title: "<%= ProjectsFilterResource.ByTitle%>", sortOrder: "ascending" }
                 ]
             })
              .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
              .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);
              ASC.Projects.ProjectsAdvansedFilter.init = true;
             }

         </script>
         <% if (CanCreateTask()){ %>
            <span class="addTask"><span class="dottedLink"><%=TaskResource.AddTask%></span></span>
        <%} %>

            <asp:PlaceHolder runat="server" ID="_filter"></asp:PlaceHolder>
 <%} %>
     <div class="presetContainer" style="margin-top: -5px;">
            <a id="preset_my" class="baseLinkAction"><%=ProjectsFilterResource.MyTasks%></a>
            <span>,</span>
            <a id="preset_today" class="baseLinkAction"><%=ProjectsFilterResource.Today%></a>
            <span>,</span>
            <a id="preset_upcoming" class="baseLinkAction"><%=ProjectsFilterResource.Upcoming%></a>
    </div>
     <script type="text/javascript">
         jq(document).ready(function() {
             <% if (RequestContext.IsInConcreteProject()) { %>
             serviceManager.init("<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project", "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>", "<%=this.GetProjectId()%>");
             <% } else { %>
             serviceManager.init("<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project", "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>");
             <% } %>
         });
     </script>
<asp:PlaceHolder ID="_content" runat="server">
</asp:PlaceHolder>

   <div id="projectsActionPanel" class="actionPanel">
           <div class="popup-corner"></div>
        <div id="projectsListPopup">

        </div>
   </div>


</asp:Content>