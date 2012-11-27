<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Core" %>

<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskList.ascx.cs" Inherits="ASC.Web.Projects.Controls.Tasks.TaskList" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Src="~/Products/Projects/Controls/Tasks/TaskAction.ascx" TagName="TaskAction" TagPrefix="ascta" %>
    
<link href="<%= PathProvider.GetFileStaticRelativePath("alltasks.css") %>" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("tasks.js") %>"></script>
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("common_filter_projects.js") %>"></script>


<div id="SubTasksBody">
    <div class="taskListHeaders">  
      <div class="check">&nbsp;</div>       
      <div class="taskTitle"><%=TaskResource.Title%></div>
      <div class="taskDeadline"><%=TaskResource.DeadlineHeader%></div>
      <div class="taskResponsible"><%=TaskResource.TaskResponsible%></div>
    </div>
    <div class="taskList">         
    
    </div> 
    <div class="noResult"><%= TaskResource.NoTasks%></div>
    <span id="showNextTasks"><%= TaskResource.ShowNextTasks%></span>
    <div class="taskProcess" id="showNextTaskProcess"></div>
     <ascta:TaskAction runat="server" ID="taskAction" BlockID="TaskList"></ascta:TaskAction>
</div>     
    <div id="templatesForParsing">
        <script id="taskTemplate" type="text/x-jquery-tmpl">
            <div class="task clearFix{{if !$item.data.subtasks.length}} noSubtasks{{/if}}{{if $item.data.canEdit}} canedit{{/if}}{{if $item.data.status == 2}} closed{{/if}}" taskid="${id}"{{if $item.data.milestone != null}}  milestoneid="${milestone.id}"{{/if}}>        
                
                {{if $item.data.status == 2}}
                    {{if $item.data.canWork == 3}}
                        <div class="menupoint" taskid="${id}" projectid="${projectOwner.id}" canWork="${canWork}" {{if $item.data.responsible != null}} userid="${responsible.id}"{{/if}}></div>
                    {{else}}
                        <div class="nomenupoint"></div>
                    {{/if}}
                {{else}}
                    {{if $item.data.canWork}}
                        <div class="menupoint" taskid="${id}" projectid="${projectOwner.id}" canWork="${canWork}" {{if $item.data.responsible != null}} userid="${responsible.id}"{{/if}}></div>
                    {{else}}
                        <div class="nomenupoint"></div>
                    {{/if}}
                {{/if}}                 
                
                <div class="check">
                  <div taskid="${id}" class="changeStatusCombobox{{if $item.data.canEdit}} canEdit{{/if}}">
                      {{if $item.data.status == 2}}
                          <span title="<%= TaskResource.Closed%>" class="closed"></span>
                      {{else}}
                          <span title="<%= TaskResource.Open%>" class="open"></span>
                      {{/if}}                    
                  </div>
                </div>
                
                <div class="taskPlace">
                  <div class="taskName" taskid="${id}">
                    {{if $item.data.priority == 1}}
                      <span class="high_priority"></span>
                    {{/if}}
                    <a taskid="${id}" href="tasks.aspx?prjID=${projectOwner.id}&id=${id}" 
                        projectid = ${projectOwner.id}
                        description = "${description}"
                        {{if $item.data.milestone != null}}milestoneid="${milestone.id}" milestone="[${milestone.displayDateDeadline}] ${milestone.title}" milestoneurl="milestones.aspx?prjID=${projectOwner.id}&id=${milestone.id}"{{/if}}
                        {{if typeof $item.data.responsibles != 'undefined' && $item.data.responsibles.length}} responsible="${responsibles[0].displayName}"{{/if}}
                        {{if typeof $item.data.createdBy != 'undefined'}}createdBy="${createdBy.displayName}"{{/if}}
                        project =  "${projectOwner.title}"
                        created="${displayDateCrtdate}" 
                        updated="${displayDateUptdate}"                         
                        status="${status}"
                        data-deadline="${deadline}">
                    ${title}                    
                    </a>                    
					</div>
                   
				<div  class="subtasksCount" data="<%=TaskResource.Subtask %>"> 
                        {{if window.tasks.openedCount($item.data.subtasks)}}
                            <span class="expand" taskid="${id}"><span class="dottedNumSubtask">+{{html window.tasks.openedCount($item.data.subtasks)}}</span></span>
                        {{else}}
                            {{if $item.data.canEdit}}
                            <span class="add" taskid="${id}">+ <%=TaskResource.Subtask %></span>
                            {{/if}}                          
                        {{/if}} 
                    </div>   
                    
                </div>  
                                                                                                                             
                {{if $item.data.displayDateDeadline.length}} 
                    <div class="deadline">
                      {{if window.tasks.compareDates($item.data.deadline)}}
                          <span id = "${id}" class="timeLeft red" deadline = ${displayDateDeadline}>${displayDateDeadline}</span>
                      {{else}}
                          <span id = "${id}" class="timeLeft" deadline = ${displayDateDeadline}>${displayDateDeadline}</span>
                      {{/if}}                                                            
                    </div>
                {{else}}
                    {{if $item.data.milestone != null}}                            
                        <div class='deadline'>
                            {{if !window.tasks.compareDates($item.data.milestone.deadline)}}
                                <span id = "${id}" class="timeLeft" deadline = ${milestone.displayDateDeadline}>${milestone.displayDateDeadline}</span>
                            {{else}}
                                <span id = "${id}" class="timeLeft red" deadline = ${milestone.displayDateDeadline}>${milestone.displayDateDeadline}</span>
                            {{/if}}                            
                        </div>
                    {{/if}} 
                {{/if}} 
                                                       
                {{if $item.data.responsibles.length > 1}}
                    <div taskid="${id}"  value="${responsibles[0].id}">
                        <div class="otherMargin"><span taskid="${id}" class="other">${responsibles.length} <%= TaskResource.Responsibles %></span></div>
                        <div class="others" taskid="${id}">
                            {{each responsibles}}
                               {{if $index >= 0}}
                                    <div value="${id}" class="user"><span title="${displayName}">${displayName}</span></div>
                               {{/if}}                          
                            {{/each}}
                        </div>                
                    </div>                        
                {{else}}
                
                    {{if $item.data.responsible == null}}
                        <div class="not user" taskid="${id}">
                                <span><%= TaskResource.NoResponsible%></span>
                        </div>
                    {{else}}
                        <div class="user" taskid="${id}" value="${responsible.id}">
                            {{if window.serviceManager.getMyGUID() == $item.data.responsible.id}}
                                <span><b><%= ProjectsFilterResource.Me%></b></span>
                            {{else}}
                                <span title="${responsible.displayName}">${responsible.displayName}</span>
                            {{/if}}                                                    
                        </div>            
                    {{/if}}                                
                {{/if}}
            </div> 
            <div taskid="${id}" class="subtasks"{{if !$item.data.subtasks.length || $item.data.status == 2}} style="display:none;"{{/if}} projectid="${projectOwner.id}">    
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
                    <div class="quickAddSubTaskField" taskid="${id}" projectid="${projectOwner.id}">
                        <input taskid="${id}" id="subTaskName" maxlength="250"/>
                        <div class="chooseWrap"><span class="choose responsible" value="" choose="<%= TaskResource.ChooseResponsible%>" taskid="${id}" projectid="${projectOwner.id}"><span class="dottedLink"><%= TaskResource.ChooseResponsible%></span></span></div>
                    </div>
                {{/if}}  
                <div class="st_separater" taskid="${id}"></div>       
             </div>     
        </script>
        <asp:PlaceHolder ID="_subtasksTemplates" runat="server"></asp:PlaceHolder>
        <script id="milestoneTemplate" type="text/x-jquery-tmpl">               
            <div class="ms">
                <input id="ms_${id}" value="${id}" type="radio" name="milestones"/>
                <label for="ms_${id}">[${displayDateDeadline}] ${title}</label>
            </div>             
        </script>
    </div>  
    
<div id="questionWindow" style="display: none">
    <ascwc:Container ID="_hintPopup" runat="server">
    <Header>
    <%= TaskResource.ClosingTheTask%>
    </Header>
    <Body>        
        <p><%= TaskResource.TryingToCloseTheTask%>. </p>
        <p><%= TaskResource.BetterToReturn%>.</p>
        <div class="popupButtonContainer">
            <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel%></a>    
            <span class="button-splitter"></span>
            <a class="baseLinkButton end"><%= TaskResource.EndAllSubtasksCloseTask%></a>
        </div>    
    </Body>
    </ascwc:Container>
</div>
<div id="questionWindowTaskRemove" style="display: none">
    <ascwc:Container ID="_hintPopupTaskRemove" runat="server">
    <Header>
    <%= TaskResource.RemoveTask%>
    </Header>
    <Body>        
        <p><%= TaskResource.RemoveTaskPopup%> </p>
        <p><%=ProjectsCommonResource.PopupNoteUndone %></p>
        <div class="popupButtonContainer">
            <a class="baseLinkButton remove"><%= TaskResource.RemoveTask%></a>
            <span class="button-splitter"></span>
            <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel%></a> 
        </div>
            
    </Body>
    </ascwc:Container>
</div>
<div id="timeTracking" style="display: none;">
    <ascwc:container id="timeTrackingContainer" runat="server">
        <header>    
            <%= TaskResource.TimeTraking%>
        </header>
        <body>
        
            <div id="TimeLogTaskTitle" class="headerBase pm-headerPanelSmall-splitter"></div>
               
            <div class="infoPanel addLogPanel-infoPanel">
                <div class="addLogPanel-infoPanelBody">
                    <span class="headerBase pm-grayText">
                        <%= TaskResource.TimeSpent%>
                    </span>
                    <span class="button-splitter"></span>
                    <span id="TotalHoursCount" class="headerBase"></span>
                    <span class="button-splitter"></span>
                    <span class="headerBase pm-grayText">
                        <%= TaskResource.hours%>
                    </span>
                </div>
            </div> 
            
            <div class="pm-headerPanelSmall-splitter" style="float:right">
                <div class="headerPanelSmall">
                    <b><%= TaskResource.TaskResponsible%>:</b>
                </div>
                <select style="width: 200px;" class="comboBox pm-report-select" id="ddlPerson">
                </select>
            </div>
            
            <div>   
            <div class="pm-headerPanelSmall-splitter" style="float:left;margin-right:20px">
                <div class="headerPanelSmall">
                    <b><%= TaskResource.AddTime%>:</b>
                </div>
                <input maxlength="5" id="tbxHours" class="textEdit" style="width:60px;" type="text">
                <span class="button-splitter"></span>
                <%= TaskResource.hours%>
            </div>
               
            <div class="pm-headerPanelSmall-splitter">
                <div class="headerPanelSmall">
                    <b><%= TaskResource.Date%>:</b>
                </div>
                <input id="tbxDate" type="text" class="textEditCalendar"/>
            </div>
            </div>
            
            <div style="clear:both"></div>
               
            <div class="pm-headerPanelSmall-splitter">
                <div class="headerPanelSmall">
                    <b><%= TaskResource.Description%>:</b>
                </div>
                <textarea rows="7" cols="20" id="tbxNote" class="pm-ntextbox " style="width: 99%; resize: none;"></textarea>
            </div>
               
            <div class="pm-h-line"><!--– –--></div>
               
            <div style="display: block;" class="pm-action-block">
                <a href="javascript:void(0)" class="baseLinkButton">
                    <%= TaskResource.AddTime%>
                </a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton" href="javascript:void(0)" onclick="javascript: jq.unblockUI();">
                    <%= TaskResource.Cancel%>
                </a>
            </div>
                
            <div class="pm-ajax-info-block" style="display: none;">
                <span class="textMediumDescribe">
                    <%= TaskResource.Saving%></span><br>
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>">
            </div>
            
            <input id="NumberDecimalSeparator" value="." type="hidden">
                
               
        </body>
    </ascwc:container>
</div>

<div id="moveTaskPanel" style="display: none;">
    <ascwc:container id="moveTaskContainer" runat="server">
        <header>    
            <%= TaskResource.MoveTaskToAnotherMilestone%>
        </header>
        <body>
            <div class="borderBase ms">
                <div class="textBigDescribe">
                    <%= TaskResource.Task %>
                </div>
                <div class="taskTitls ms">
                    <b id="moveTaskTitles"></b>
                </div>
                <div class="textBigDescribe ms"><%= TaskResource.WillBeMovedToMilestone%>:</div>
                <div class="milestonesList">
                
                    <div class="milestonesButtons">
                        <input id="ms_0" type="radio" name="milestones" value="0"/>
                        <label for="ms_0"><%= TaskResource.None%></label>
                    </div>
                </div>
            </div>
            
            <div class="pm-action-block">
                <a href="javascript:void(0)" class="baseLinkButton">
                    <%= TaskResource.MoveToMilestone%>
                </a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton" href="javascript:void(0)" onclick="javascript: jq.unblockUI();">
                    <%= ProjectsCommonResource.Cancel%>
                </a>
            </div>
            <div class='pm-ajax-info-block' style="display: none;">
                <span class="textMediumDescribe">
                    <%= TaskResource.ExecutingGroupOperation%>
                </span><br />
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>
        </body>
    </ascwc:container>
</div>

<div id="taskActionPanel" class="actionPanel" objid="">
	<div class="popup-corner"></div>        
    <div>
        <div id="ta_edit" class="pm"><span><%= TaskResource.Edit%></span></div>
        <div id="ta_subtask" class="pm"><span><%= TaskResource.AddSubtask%></span></div>
        <div id="ta_accept" class="pm"><span><%= TaskResource.AcceptSubtask%></span></div>
        <div id="ta_move" class="pm"><span><%= TaskResource.MoveToMilestone%></span></div>                
        <div id="ta_mesres" class="pm"><span><%= TaskResource.MessageResponsible%></span></div>
        <% if (Global.ModuleManager.IsVisible(ModuleType.TimeTracking)) %>
          <%{%>
                <div id="ta_time" class="pm"><span><%= TaskResource.TrackTime %></span></div>
           <% } %>
        <div id="ta_remove" class="pm"><span><%= ProjectsCommonResource.Delete%></span></div>
    </div>
</div>  
  
<div id="usersActionPanel" class="actionPanel" objid="" without="<%= TaskResource.WithoutResponsible%>">
	<div class="popup-corner"></div>            
</div>  

<div id="othersPanel" class="actionPanel">
	<div class="popup-corner"></div>        
    <div id="othersListPopup">
    </div>
</div>


<div id="taskDescrPanel" class="actionPanel" objid="">
    <div class="popup-corner"></div>
    <div>
      <div class="date">  
        <div class="param"><%= TaskResource.CreatingDate%>:</div>
        <div class="value"></div>
      </div>
      <div class="createdby">
	      <div class="param"><%= TaskResource.CreatedBy%>:</div>
        <div class="value"></div>
      </div>
      <div class="closed">  
        <div class="param"><%= TaskResource.Closed%>:</div>
        <div class="value"></div>
      </div>
      <div class="closedby">
	      <div class="param"><%= TaskResource.ClosedBy%>:</div>
        <div class="value"></div>
      </div>      
    <% if (!RequestContext.IsInConcreteProject()) %>
    <%{%>        
      <div class="project"> 
        <div class="param"><%= TaskResource.Project%>:</div>
        <div class="value"></div>
      </div>    
    <%}%>        
      <div class="milestone">  
        <div class="param"><%= TaskResource.Milestone%>:</div>
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
</div>  


<div id="statusListContainer">
    <div class="containerTop"></div>
    <ul>
        <li class="open"><%= TaskResource.Open%></li>
        <li class="closed"><%= TaskResource.Closed%></li>
    </ul>
</div>