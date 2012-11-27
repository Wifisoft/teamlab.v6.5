<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subtasks.ascx.cs" Inherits="ASC.Web.Projects.Controls.Tasks.Subtasks" %>


<script id="AddSubTaskFieldTemplate" type="text/x-jquery-tmpl">
     <div taskid="${taskid}" class="quickAddSubTaskField" id="quickAddSubTaskField" projectid="${projectid}">                     
         <input class="subTaskName" taskid="${taskid}" subtaskid="${subtaskid}" value="${title}" maxlength="250"/>                     
         <div class="chooseWrap"><span taskid="${taskid}" subtaskid="${subtaskid}" choose="<%= TaskResource.ChooseResponsible%>"{{if $item.data.responsible != null}} value="${responsible.id}"{{/if}} class="choose responsible"><span class="dottedLink">{{if $item.data.responsible != null}}${responsible.displayName}{{else}}<%= TaskResource.ChooseResponsible%>{{/if}}</span></span></div>                 
     </div>
</script>
<script id="subtaskTemplate" type="text/x-jquery-tmpl">               
      {{each subtasks}}
                <div status="${status}" class="subtask clearFix {{if status == 2}} closed{{/if}}{{if $item.data.canEdit}} canedit{{/if}}" subtaskid="${id}" taskid="${$item.parent.data.id}" projectid="${$item.parent.data.projectOwner.id}">        
                    {{if $item.parent.data.canWork == 3 || $item.parent.data.canWork == 2}}<div class="menupoint" subtaskid="${id}" canWork="${$item.parent.data.canWork}"></div>
                    {{else}}<div class="nomenupoint"></div>{{/if}}
                    <div class="check"><input type="checkbox"{{if status == 2}} checked="true"{{/if}}{{if !$item.data.canEdit || $item.data.status == 2}} disabled="true"{{/if}} taskid="${$item.parent.data.id}" subtaskid="${id}"/></div>    
                    <div class="taskName{{if $item.data.canEdit}} canedit{{/if}}" 
                        subtaskid="${id}" 
                        created="${displayDateCrtdate}"   
                        {{if typeof $item.data.createdBy != 'undefined'}}createdBy="${createdBy.displayName}"{{/if}}
                         {{if updatedBy != null}} updatedBy="${updatedBy.displayName}" {{/if}}
                        status ="${status}"
                        updated="${displayDateUptdate}"><span>{{html jq.linksParser(Encoder.htmlEncode($item.data.subtasks[$index].title).replace('&amp;', '&'))}}</span></div>            
                    {{if $item.data.subtasks[$index].responsible == null}}
                        <div class="not user" me="<%= ProjectResource.My%>" taskid="${id}" subtaskid="${id}">
                                <span><%= TaskResource.NoResponsible%></span>
                        </div>
                    {{else}}                        
                        <div class="user" subtaskid="${$item.data.subtasks[$index].id}" value="${$item.data.subtasks[$index].responsible.id}" title="${$item.data.subtasks[$index].responsible.displayName}">
                            {{if window.serviceManager.getMyGUID() == $item.data.subtasks[$index].responsible.id}}
                             <b><%= ProjectResource.My%></b>
                            {{else}}
                                ${$item.data.subtasks[$index].responsible.displayName}
                            {{/if}}                            
                        </div>            
                    {{/if}}
                </div>      
       {{/each}}  
</script>

<script id="newSubtaskTemplate" type="text/x-jquery-tmpl">
    <div status="${status}" class="subtask clearFix canedit" subtaskid="${id}" taskid="${taskid}" projectid="${projectid}">        
                    <div class="menupoint" subtaskid="${id}" canWork="${canWork}"></div>
                    <div class="check"><input type="checkbox" taskid="${taskid}" subtaskid="${id}"/></div>    
                    <div class="taskName canedit" 
                        subtaskid="${id}" 
                        created="${displayDateCrtdate}"   
                        {{if typeof createdBy != 'undefined'}}createdBy="${createdBy.displayName}"{{/if}}
                        status ="${status}"
                        updated="${displayDateUptdate}"><span>{{html jq.linksParser(Encoder.htmlEncode(title).replace('&amp;', '&'))}}</span></div>            
                    {{if responsible == null}}
                        <div class="not user" me="<%= ProjectResource.My%>" taskid="${id}" subtaskid="${id}">
                                <span><%= TaskResource.NoResponsible%></span>
                        </div>
                    {{else}}                        
                        <div class="user" subtaskid="${id}" value="${responsible.id}">
                            {{if window.serviceManager.getMyGUID() == responsible.id}}
                             <b><%= ProjectResource.My%></b>
                            {{else}}
                                ${responsible.displayName}
                            {{/if}}                            
                        </div>            
                    {{/if}}
    </div>
</script>

<div id="subtaskActionPanel" class="actionPanel" objid="">
	<div class="popup-corner"></div>        
    <div>
        <div id="sta_edit" class="pm"><span><%= TaskResource.Edit%></span></div>
        <div id="sta_accept" class="pm"><span><%= TaskResource.AcceptSubtask%></span></div>
        <div id="sta_remove" class="pm"><span><%= ProjectsCommonResource.Delete%></span></div>
    </div>
</div> 

<div id="subTaskDescrPanel" class="actionPanel" objid="">
	<div class="popup-corner"></div>
  <div class="created">
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
</div> 

<link href="<%= PathProvider.GetFileStaticRelativePath("subtasks.css") %>" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("subtasks.js") %>"></script>