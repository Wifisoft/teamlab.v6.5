<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskDescriptionView.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Tasks.TaskDescriptionView" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="~/Products/Projects/Controls/Tasks/TaskAction.ascx" TagName="TaskAction"
    TagPrefix="ascta" %>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("taskdescription.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("timetracking.js") %>"></script>

<link href="<%= PathProvider.GetFileStaticRelativePath("tasks.css") %>" type="text/css"
    rel="stylesheet" />

<script>  
    jq(document).ready(function() {
        window.ASC.Projects.TaskDescroptionPage.setTimeSpent('<%=TaskTimeSpend %>');

        ASC.Controls.AnchorController.bind(/subtasks/, function() { ASC.Projects.TaskDescroptionPage.showSubtasks(); });
        ASC.Controls.AnchorController.bind(/task_comments/, function() { ASC.Projects.TaskDescroptionPage.showComments(); });

        LoadingBanner.displayLoading(true);
        var id = jq.getURLParam("id");
        if (id) {
            ASC.Projects.TaskDescroptionPage.init(id);
        }
    });
</script>

<%if(CanReadFiles)
          {%>

<script>
        //------ Attachments control init-------//
        jq(document).ready(function() {
            ASC.Controls.AnchorController.bind( /task_files/ , function() { ASC.Projects.TaskDescroptionPage.showFiles(); });
            var id = jq.getURLParam("id");
            //----Attachments------//
            Attachments.bind("addFile", function(ev, file) {
                Teamlab.addPrjEntityFiles(null, id, "task", [file.id], function() {
                });
                ASC.Projects.TaskDescroptionPage.changeCountInTab('add', "filesTab");
            });
            Attachments.bind("deleteFile", function(ev, fileId) {
                Teamlab.removePrjEntityFiles({ }, id, "task", fileId, function() {
                });
                Attachments.deleteFileFromLayout(fileId);
                ASC.Projects.TaskDescroptionPage.changeCountInTab('delete', "filesTab");
            });
            Attachments.bind("loadAttachments", function(ev, count) {
                ASC.Projects.TaskDescroptionPage.changeCountInTab(count, "filesTab");
            });
            //------end Attachments----//
        });
</script>

<% } %>
<asp:PlaceHolder ID="_subtasksTemplates" runat="server"></asp:PlaceHolder>

<script id="taskTemplate" type="text/x-jquery-tmpl">
                {{tmpl '#subtaskTemplate'}}
                {{if $item.data.canEdit}}
                    {{if ($item.data.status == 1 || $item.data.status == 4) && ($item.data.subtasks.length)}}
                        <div class="quickAddSubTaskLink" taskid="${id}" visible="true">
                    {{else}}
                        <div class="quickAddSubTaskLink" taskid="${id}" visible="false" style="display:none;">
                    {{/if}}                                                
                        <span class="dottedLink" taskid="${id}"><%= TaskResource.AddNewSubtask%></span>
                    </div>        
                    <div class="subtaskSaving" taskid="${id}"></div>                  
                    <div class="quickAddSubTaskField" id="addNewSubtaskField" taskid="${id}" projectid="${projectOwner.id}">
                        <input taskid="${id}" id="subTaskName" maxlength="250"/>
                        <div class="chooseWrap"><span class="choose responsible" value="" choose="<%= TaskResource.ChooseResponsible%>" projectid="${projectOwner.id}"><span class="dottedLink"><%= TaskResource.ChooseResponsible%></span></span></div>
                    </div>
                {{/if}}  
                <div class="st_separater" taskid="${id}"></div>       
</script>

<div id="taskTitleContainer">
    <div id="statusTaskMark" class="<%= Task.Status.ToString().ToLowerInvariant() %>">
    </div>
    <span class="headerTaskTitle">
        <%=HttpUtility.HtmlEncode(Task.Title)%></span>
    <div style="clear: both">
    </div>
</div>

<div id="topTaskActionContainer">
    <%if(CanEditTask){%>
    <span id="editTaskAction" class="taskAction">
        <a class="baseLinkAction">
            <%=TaskResource.EditTask%></a> 
    </span>
    <% }%>
    <%if(CanDeleteTask){%>
    <span id="removeTask" class="taskAction">
        <a class="baseLinkAction">
            <%=TaskResource.RemoveTask%>
        </a> 
    </span>
    <% } %>
    <% if (CanCreateTimeSpend()) %>
    <%{%>
        <span id="startTimer" class="taskAction">
            <a class="baseLinkAction" projectid="<%= Task.Project.ID %>" taskid="<%= Task.ID %>">
            <%= ProjectsCommonResource.AutoTimer %></a> 
        </span>
    <% } %>
    <%if (CanCreateTask()) {%>
        <span id="addNewTask" class="taskAction">
            <a class="baseLinkAction"> <%= TaskResource.AddNewTask %></a>
        </span>
    <% } %>
    <% if (Global.EngineFactory.GetTaskEngine().IsSubscribedToTask(Task)) %>
    <% { %>
    <span id="followTaskActionTop" class="taskAction" onclick="ASC.Projects.TaskDescroptionPage.subscribeTask();">
        <a class="baseLinkAction" textvalue="<%=TaskResource.FollowTask %>">
            <%=TaskResource.UnfollowTask%>
        </a>
    </span>
    <% } else {%>
    <span id="followTaskActionTop" class="taskAction" onclick="ASC.Projects.TaskDescroptionPage.subscribeTask();">
        <a class="baseLinkAction" textvalue="<%=TaskResource.UnfollowTask %>">
            <%=TaskResource.FollowTask %></a></span>
    <% } %>
</div>
<div id="questionWindow" style="display: none">
    <ascw:Container ID="_hintPopup" runat="server">
        <header>
    <%= TaskResource.ClosingTheTask%>
    </header>
        <body>
            <p>
                <%= TaskResource.TryingToCloseTheTask%>.
            </p>
            <p>
                <%= TaskResource.BetterToReturn%>.</p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton end">
                    <%= TaskResource.EndAllSubtasksCloseTask%></a> <span class="button-splitter">
                </span><a class="grayLinkButton cancel">
                    <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </body>
    </ascw:Container>
</div>
<div id="questionWindowTaskRemove" style="display: none">
    <ascw:Container ID="_hintPopupTaskRemove" runat="server">
        <header>
    <%= TaskResource.RemoveTask%>
    </header>
        <body>
            <p>
                <%= TaskResource.RemoveTaskPopup%>
            </p>
            <p>
                <%= ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton remove">
                    <%= TaskResource.RemoveTask%></a> <span class="button-splitter"></span><a class="grayLinkButton cancel">
                        <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </body>
    </ascw:Container>
</div>

<script id="taskDescriptionTemplate" type="text/x-jquery-tmpl">
        {{if description!=""}}
            <dl class="description">
                <dt><%=TaskResource.TaskDescription%>:</dt>
                <dd>{{html jq.linksParser(window.ASC.Projects.TaskDescroptionPage.formatDescription(description))}}</dd>
            </dl>
        {{/if}}
        {{if displayDateDeadline !=""}}
            <dl>
                <dt><%=TaskResource.EndDate%>:</dt>                      
                {{if window.ASC.Projects.TaskDescroptionPage.compareDates(deadline)}}
                    <dd><span class="deadlineLate">${displayDateDeadline}</span></dd>
                {{else}}
                    <dd><span>${displayDateDeadline}</span></dd>
                {{/if}}  
            </dl>
        {{/if}}
        {{if milestone != ""}}
            <dl class="milestone">
                <dt><%= MilestoneResource.Milestone%>:</dt>
                <dd>${milestone}</dd>
            </dl>
        {{/if}}        
        {{if priority == 1 }}
            <dl class="priority">
                <dt><%= TaskResource.Priority%>:</dt>
                <dd><span class='colorPriority high'></span><%=TaskResource.HighPriority %></dd>
            </dl>
        {{/if}} 
        <dl class="responsible">
            <dt><%= TaskResource.AssignedTo%>:</dt>
            <dd>
                {{if responsibles.length == 0}}
                    <%=TaskResource.WithoutResponsible %>
                {{else}}
                    {{each(i, resp) responsibles}}
                        ${resp.displayName}
                        {{if i < responsibles.length - 1}},{{/if}}
                    {{/each}}
                {{/if}}
            </dd>
        </dl> 
                
        <dl class="timeSpend">
            <dt><%= ProjectsCommonResource.SpentTotally%>:</dt>
            <dd id="timeSpent">
                <a class="toPageTimeTraking" href="timetracking.aspx?prjID=${projId}&ID=${taskId}">
                    <span id="totalHoursCountOnPage">${timeSpend}</span></a>
            </dd>            
        </dl>          
        
        <dl class="timeInfo">
          {{if createdDate!=""}}
            <dt><%= TaskResource.CreatingDate %>:</dt>
            <dd id="startDate">${createdDate}</dd>
          {{/if}}
            <dt><%= TaskResource.TaskProducer %>:</dt>
            <dd>${createdBy}</dd>
        </dl>
               
        {{if status==2}}
            <dl class="timeInfo">
                <dt><%= TaskResource.ClosingDate %>:</dt>
                <dd id="endDate">${closedDate}</dd>
                <dt><%= TaskResource.ClosedBy %>:</dt>
                <dd>${closedBy}</dd>
            </dl>        
        {{/if}} 
        <dl class="buttonContainer">
            <dt></dt>
            <dd>
                {{if responsibles.length == 0}}
                    <a id="acceptButton" class="baseLinkButton"><%=TaskResource.Accept%></a> 
                {{else}}
                    {{if status != 2}}
                        <a id="closeButton" class="baseLinkButton"><%=TaskResource.CompleteTask %></a>
                    {{/if}}
                    {{if status == 2}}
                        <a id="resumeButton" class="baseLinkButton"><%=TaskResource.TaskReopen%></a>
                    {{/if}}             
                {{/if}}
            </dd>
        </dl>
</script>

<div class="commonInfoTaskDescription">
</div>
<input id="hiddenInput" style="display: none;" />
<ul class="taskTabs">
    <li id="subtaskTab" onclick="ASC.Controls.AnchorController.move('subtasks')" class="current">
        <%=TaskResource.Subtasks %>
        <span class="count"></span></li>
    <%if(CanReadFiles) {%>
    <li id="filesTab" onclick="ASC.Controls.AnchorController.move('task_files')">
        <%=ProjectsCommonResource.DocsModuleTitle %>
        <span class="count">
            <%if (AttachmentsCount>0)
              {%>
            (<%=AttachmentsCount %>)
            <% } %>
        </span></li>
    <%} %>
    <li id="commentsTab" onclick="ASC.Controls.AnchorController.move('task_comments')">
        <%=ProjectResource.Comments %>
        <span class="count"></span></li>
</ul>
<div id="tabsContent">
    <div id="subtaskContainer" class="visible">
        <div id="emptySubtasksPanel" style="display: none;">
            <asp:PlaceHolder runat="server" ID="_phEmptySubtasksPanel" />
        </div>
        <div class="subtasks">
        </div>
    </div>
    <div id="filesContainer">
        <asp:PlaceHolder runat="server" ID="phAttachmentsControl" />
    </div>
    <div id="commentContainer">
        <div id="emptyCommentsPanel" style="display: none;">
            <asp:PlaceHolder runat="server" ID="_phEmptyCommentsPanel" />
        </div>
        <div id="commentsListWrapper">
            <ascw:CommentsList ID="commentList" runat="server" BehaviorID="commentsList">
            </ascw:CommentsList>
        </div>
    </div>
</div>
<div id="usersActionPanel" class="actionPanel" objid="" without="<%= TaskResource.WithoutResponsible%>">
    <div class="popup-corner">
    </div>
</div>
<ascta:TaskAction runat="server" ID="taskAction" BlockID="taskActionContainer">
</ascta:TaskAction>