<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
    CodeBehind="timeTracking.aspx.cs" Inherits="ASC.Web.Projects.TimeTracking" %>

<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Register Src="~/Products/Projects/Controls/TimeSpends/TimeSpendActionView.ascx"
    TagPrefix="ctrl" TagName="TimeSpendActionView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("reports.css") %>" rel="stylesheet"
        type="text/css" />
    <link href="<%= PathProvider.GetFileStaticRelativePath("timetracking.css")%>" rel="stylesheet"
        type="text/css" />

    <script src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js") %>"></script>

    <script src="<%= PathProvider.GetFileStaticRelativePath("common_filter_projects.js") %>"
        type="text/javascript"></script>

    <script src="<%= PathProvider.GetFileStaticRelativePath("timetracking.js")%>" type="text/javascript"></script>

    <script src="<%= PathProvider.GetFileStaticRelativePath("reports.js") %>" type="text/javascript"></script>

    <% if(RequestContext.IsInConcreteProject()) { %>

    <script type="text/javascript">
            jq(document).ready(function() {
                serviceManager.init("<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project",
                    "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>", "<%=RequestContext.GetCurrentProjectId().ToString(CultureInfo.InvariantCulture)%>");
            });
    </script>

    <% } else { %>

    <script type="text/javascript">
          jq(document).ready(function() {
              serviceManager.init("<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project", "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>");
          });
    </script>

    <% } %>
    <% if(TaskID <= 0 && !IsTimer) { %>

    <script type="text/javascript">
             jq(document).ready(function() {
                 ASC.Projects.TimeSpendActionPage.initFilter();
             });
             function createAdvansedFilter() {
                 jq('#AdvansedFilter').advansedFilter(
                     {
                         store: true,
                         anykey: true,
                         nykeytimeout: 1000,
                         filters: [
                             { 
                                 type: "person",
                                 id: "me_tasks_rasponsible",
                                 title: "<%= ProjectsFilterResource.MyTasks%>",
                                 filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                                 group: "<%= ProjectsFilterResource.ByResponsible %>",
                                 hashmask: "person/{0}",
                                 groupby: "userid",
                                 bydefault: { id: "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>" }
                             },
                             { 
                                 type: "person",
                                 id: "tasks_rasponsible",
                                 title: "<%= ProjectsFilterResource.OtherUsers%>",
                                 filtertitle: "<%= ProjectsFilterResource.ByResponsible %>:",
                                 group: "<%= ProjectsFilterResource.ByResponsible %>",
                                 hashmask: "person/{0}",
                                 groupby: "userid"
                             },
                             { 
                                 type: "group",
                                 id: "group",
                                 title: "<%= ProjectsFilterResource.Groups%>",
                                 filtertitle: "<%= ProjectsFilterResource.Group %>:",
                                 group: "<%= ProjectsFilterResource.ByResponsible%>",
                                 groupby: "userid",
                                 hashmask: "group/{0}"
                             }
                    
/*                             <% if (!RequestContext.IsInConcreteProject()){ %>
                             { 
                                type: "combobox", 
                                 id: "tag", 
                                 title: "<%= ProjectsFilterResource.ByTag%>",
                                 filtertitle: "<%= ProjectsFilterResource.ByTag %>:",
                                 group: "<%= ProjectsFilterResource.ByProject%>", 
                                 hashmask: "tag/{0}",
                                 defaulttitle: "<%= ProjectsFilterResource.Select%>"
                            },
                            <%} %>                
                             {
                                 type: "combobox",
                                 id: "milestone",
                                 title: "<%= ProjectsFilterResource.ByMilestone%>",
                                 filtertitle: "<%= ProjectsFilterResource.ByMilestone%>:",
                                 group: "<%= ProjectsFilterResource.ByProject%>",
                                 hashmask: "milestone/{0}"
                             }
                             <% if (!RequestContext.IsInConcreteProject()){ %>
                            , {
                                type: "combobox",
                                id: "project",
                                title: "<%=ProjectsFilterResource.ByProject%>",
                                filtertitle: "<%= ProjectsFilterResource.ByProject %>:",
                                group: "<%=ProjectsFilterResource.ByProject %>",
                                hashmask: "project/{0}",
                                defaulttitle: "<%= ProjectsFilterResource.Select%>"
                            }                   
                             <%} %>*/
                 ],
                 sorters: [
                     { id: "date", title: "<%= ProjectsFilterResource.ByDate%>", sortOrder: "descending", def: true },
                     { id: "hours", title: "<%= ProjectsFilterResource.ByHours%>", sortOrder: "ascending"},
                     { id: "note", title: "<%= ProjectsFilterResource.ByNote%>", sortOrder: "ascending"}
                 ]
             }
             )
              .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
                  .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);
               
              ASC.Projects.ProjectsAdvansedFilter.init = true;
             }
    </script>

    <% } %>
    <% if (IsTimer) %>
    <% { %>

    <script type="text/javascript">
        jq(document).ready(function() {
            jq("#studio_onlineUsersBlock").remove();
            jq("#studioFooter").remove();
            jq("div.studioTopNavigationPanel").remove();
            jq("div.infoPanel").remove();
            jq("div.containerHeaderBlock").remove();
            jq("#studioPageContent").css("padding-bottom", "0");
            jq("div.containerBodyBlock").css("padding", "0");
            jq("div.mainPageLayout").css("width", "100%");
            jq("div.mainPageLayout div:first").remove();
            jq("div.mainContainerClass").css("border", "medium none");
            jq("#studioContent").css("overflow", "hidden");

            window.onbeforeunload = function(evt) {
                if (jq("#timerTime .start").hasClass("stop")) {
                    window.ASC.Projects.TimeTraking.playPauseTimer();
                    return '';
                }
                
                if (window.ASC.Projects.TimeTraking.ifNotAdded) {
                    return '';
                }
                
                return;
            };
        });
    </script>

    <% } %>
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <% if (!IsTimer) %>
    <% { %>
    <div id="MainPageContainer" runat="server">

        <div id="actionContainer">
            <span class="totalHoursContainer">
            <%=TimeTrackingResource.TotalTimeSpend %>
                <span id="TotalHoursCount" style="font-weight: bold" hours="<%=TotalTime %>">
                    <%=GetTimes() %>
                </span>
            </span>
           <%if(CanCreateTime())
           {%>
                <span id="startTimer" style="margin-bottom: 15px;display: inline-block"><a class="baseLinkAction">
                    <%= ProjectsCommonResource.AutoTimer %></a> 
                </span>
           <% } %>
            <asp:PlaceHolder runat="server" ID="_filter"></asp:PlaceHolder>
        </div>
        
        <div id="mainContent" class="pm-headerPanel-splitter timeSpendsList">
            <div class="timesHeader">
                <div class="pm-ts-dateColumn">
                    <%= ProjectsCommonResource.Date %></div>
                <div class="pm-ts-noteColumn">
                    <%=ProjectsCommonResource.TaskTitle%></div>
                <div class="pm-ts-hoursColumn">
                    <%= ProjectsCommonResource.HoursCount %></div>
                <div class="pm-ts-personColumn">
                    <%=TaskResource.TaskResponsible %></div>
                <div class="pm-ts-actionsColumn">
                </div>
            </div>
            <div id="timeSpendsList" class="<%if(TaskID < 0){ %>forProject<% } %>">
            </div>
            <span id="showNext">
                <%= ProjectsCommonResource.ShowNext%></span>
            <div class="taskProcess" id="showNextProcess">
            </div>
        </div>

        <script id="timeTrackingTemplate" type="text/x-jquery-tmpl">
            <div id="timeSpendRecord${id}" class="timeSpendRecord" timeid="${id}" taskid="${relatedTask}" date="${date}"  hours="${hours}">
                    <div class="pm-ts-dateColumn" id="date_ts${id}">
                            ${dayMonth}
                    </div>
                    <div class="pm-ts-noteColumn" title="${note}" id="note_ts${id}">
                        <div{{if !$item.data.note.length}} class="alone"{{/if}}>
                            <a href="tasks.aspx?prjID=${relatedProject}&id=${relatedTask}">${relatedTaskTitle}</a>
                            <span>${note}</span>
                        </div>
                    </div>
                    <div class="pm-ts-hoursColumn" id="hours_ts${id}">
                            ${jq.timeFormat(hours)}
                    </div>                                                                
                    <div class="pm-ts-personColumn" id="person_ts${id}">
                        {{if window.serviceManager.getMyGUID() == $item.data.createdBy.id}}
                            <span value="${createdBy.id}"><b><%= ProjectsFilterResource.Me%></b></span>
                        {{else}}
                            <span value="${createdBy.id}" title="${createdBy.displayName}">${createdBy.displayName}</span>
                        {{/if}}                                                    
                    </div>
                    {{if canEdit == true}}
                    <div class="pm-ts-actionsColumn">
                        <div class="menupoint" timeid="${id}" prjId="${relatedProject}" userid="${createdBy.id}"></div>
                    </div>
                    {{/if}}
            </div>
        </script>

        <div id="timeActionPanel" class="actionPanel" objid="">
            <div class="popup-corner">
            </div>
            <div>
                <div id="ta_edit" class="pm">
                    <span>
                        <%= TaskResource.Edit%></span></div>
                <div id="ta_remove" class="pm">
                    <span>
                        <%= TaskResource.Remove%></span></div>
            </div>
        </div>
        <ctrl:TimeSpendActionView runat="server" ID="_timeSpendActionView" />
        <asp:HiddenField ID="hiddenCssClass" runat="server" />
        <div id="emptyScreen">
            <asp:PlaceHolder ID="emptyScreen" runat="server"></asp:PlaceHolder>
        </div>
        <div id="emptyListForFilter">
            <asp:PlaceHolder ID="emptyScreenFilter" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    <% } %>
    <% else %>
    <% { %>
    <asp:PlaceHolder ID="_phTimeSpendTimer" runat="server"></asp:PlaceHolder>
    <% } %>
</asp:Content>
