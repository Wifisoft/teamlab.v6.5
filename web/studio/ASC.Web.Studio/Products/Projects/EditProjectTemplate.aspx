<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditProjectTemplate.aspx.cs" Inherits="ASC.Web.Projects.EditProjectTemplate" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"%>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projecttemplates.css") %>"
        rel="stylesheet" type="text/css" />

    <script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("projecttemplates.js") %>"></script>
    <script>
        jq(document).ready(function() {
            ASC.Projects.EditProjectTemplates.init();
        });
    </script>
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
<div style="display: none">
    <p id="milestoneError"><%=ProjectTemplatesResource.MilestoneError %></p>
    <p id="taskError"><%=ProjectTemplatesResource.TaskError %></p>
</div>

<script id="milestoneTmpl" type="text/x-jquery-tmpl">
    <div class="milestone" id="m_${number}">
        <div class="mainInfo menuButtonContainer">
            <span class="daysCount" value="${duration}"><span>${duration}</span></span>
            <span class="title">${title}</span>
            {{if tasks.length == 0}}
                <a class="addTask"> + <%=ProjectTemplatesResource.Task %></a>
            {{else}}
                <a class="addTask hide"> + <%=ProjectTemplatesResource.Task %></a>
            {{/if}}
            <span class="menuButton"></span>
        </div>
        {{if displayTasks}}
        <div class="milestoneTasksContainer" style="display: block;">
        {{else}}
        <div class="milestoneTasksContainer">
        {{/if}}
            <div class="listTasks" milestone="m_${number}">
                {{each(i, task) tasks}}
                    <div id="${number}_${i+1}" class="task menuButtonContainer">
                        <span class="title">${task.title}</span>
                        <span class="menuButton"></span>
                    </div>
                {{/each}}
            </div>
            {{if displayTasks}}
            <div class="addTaskContainer" style="display:block;">
            {{else}}
            <div class="addTaskContainer">
            {{/if}}
                <a class="baseLinkAction"><%=ProjectResource.AddTask %></a>
            </div>
        </div>
    </div>
</script>
<script id="taskTmpl" type="text/x-jquery-tmpl">
    <div class="task menuButtonContainer" id="t_${number}">
          <span class="title">${title}</span>
          <span class="menuButton"></span>
    </div>
</script> 
<div id="templateTitleContainer" class="requiredField">
    <span class="requiredErrorText"><%=ProjectTemplatesResource.TitleErrorTemplate %></span>  
    <div class="templTitle headerPanel"><%=ProjectTemplatesResource.TemplateTitle %>:</div>
    <input id="templateTitle" type="text" value="<%= Templ != null ? HttpUtility.HtmlEncode(Templ.Title) : ""%>" class="textEdit"/>
</div>
    
<div class="templTitle"><%=Templ != null ? ProjectTemplatesResource.EditTmplStructure : ProjectTemplatesResource.TemplateStructure %></div>

<div id="listAddedMilestone">
    
</div>

<div id="addMilestone"><a class="baseLinkAction"><%=ProjectTemplatesResource.AddMilestone%></a></div>

<p class="unlocatedTaskTitle"><%=ProjectTemplatesResource.TasksWithoutMilestone%></p>
<div id="noAssignTaskContainer">
    <div id="listNoAssignListTask"></div>
    <div class="addTaskContainer">
           <a class="baseLinkAction"><%=ProjectTemplatesResource.AddTask %></a>
    </div>
</div>

<div id="addMilestoneContainer" target="">
    <select>
        <option selected="selected" duration="0.5" value='<%=ChooseMonthNumeralCase(0.5)%>'><%=ChooseMonthNumeralCase(0.5)%></option>
        <% for (double i  = 1; i  <= 12; i = i + 0.5)
           {%>
            <option duration="<%=i.ToString() %>" value='<%=ChooseMonthNumeralCase(i)%>'><%=ChooseMonthNumeralCase(i)%></option>
           <%} %>
    </select>
    <input id="newMilestoneTitle" placeholder="<%=ProjectTemplatesResource.AddMilestoneTitle %>"/>
</div>

<div id="addTaskContainer" target="">
    <input id="newTaskTitle" placeholder="<%=ProjectTemplatesResource.AddTaskTitle %>"/>
</div>

<% if (Templ != null)
   {%>
    <div  class="buttonContainer">
        <a id="saveTemplate" class="baseLinkButton" style="margin-right: 10px;"><%=ProjectTemplatesResource.SaveChanges %></a>
        <a id="createProject" href="createprojectfromtemplate.aspx?id=<%=Templ.Id%>" class="grayLinkButton" style="margin-right: 10px;"><%= ProjectTemplatesResource.CreateProjFromTmpl %></a>
        <a class="grayLinkButton" href="projectTemplates.aspx"><%=ProjectsCommonResource.Cancel%></a>
    </div>
<% }else
   {%>
    <div  class="buttonContainer">
        <a id="saveTemplate" class="baseLinkButton" style="margin-right: 10px;"><%=ProjectTemplatesResource.SaveTemplate %></a>
        <a id="createProject" href="javascript:void(0)" class="grayLinkButton" style="margin-right: 10px;"><%= ProjectTemplatesResource.SaveAndCreateProjFromTmpl %></a>
        <a class="grayLinkButton" href="projectTemplates.aspx"><%=ProjectsCommonResource.Cancel%></a>
    </div>
   <%} %>

<div id="taskActionPanel" class="actionPanel">
	<div class="popup-corner"></div>
	<ul class="actionList">
	    <li id="editTask"><%=ProjectTemplatesResource.Edit %></li>
	    <li id="removeTask"><%=ProjectTemplatesResource.Delete %></li>
	</ul>        
</div>

<div id="milestoneActionPanel" class="actionPanel">
	<div class="popup-corner"></div>
	<ul class="actionList">
	    <li id="editMilestone"><%=ProjectTemplatesResource.Edit %></li>
	    <li id="addTaskInMilestone"><%=ProjectTemplatesResource.AddTask%></li>
	    <li id="removeMilestone"><%=ProjectTemplatesResource.Delete %></li>
	</ul>        
</div>
    
</asp:Content>
