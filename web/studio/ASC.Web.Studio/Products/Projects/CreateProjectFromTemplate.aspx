<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateProjectFromTemplate.aspx.cs" Inherits="ASC.Web.Projects.CreateProjectFromTemplate"  MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"%>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>


<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projecttemplates.css") %>"
        rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("projecttemplates.js") %>"></script>
   
   <script>
        jq(document).ready(function() {
        ASC.Projects.CreateProjectFromTemplate.init("<span class='chooseResponsible nobody'><span class='dottedLink'><%=ProjectTemplatesResource.ChooseResponsible %></span></span>", "<%= DateTimeExtension.DateMaskForJQuery %>");  
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
            <span class="dueDate"><span>${date}</span></span>
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
<div id="projectTitleContainer" class="requiredField">
    <span class="requiredErrorText"><%=ProjectTemplatesResource.TitleErrorProject %></span>  
    <div class="templTitle headerPanel"><%=ProjectTemplatesResource.TemplateTitle %></div>
    <input id="projectTitle" type="text" class="textEdit" defText="<%=ProjectTemplatesResource.DefaultProjTitle %>"/>
</div>
    
<div class="templTitle"><%=ProjectTemplatesResource.Description %></div>
    <textarea cols="20" rows="5" id="projectDescription" class="textEdit"></textarea> 

<div id="pmContainer" class="requiredField">
    <span class="requiredErrorText"><%=ProjectTemplatesResource.ErrorManager %></span>
    <div class="templTitle headerPanel"><%=ProjectTemplatesResource.DefinePM %></div>
    <div id="projectManagerContainer" class="requiredField">
        <asp:PlaceHolder ID="projectManagerPlaceHolder" runat="server"></asp:PlaceHolder>
        <% if (IsAdmin) { %>
            <div class="notifyContainer">
                <input creatorId="<%=ASC.Core.SecurityContext.CurrentAccount.ID%>" id="notifyManagerCheckbox" disabled="disabled" type="checkbox" checked="checked"/>
                <label for="notifyManagerCheckbox"><%=ProjectTemplatesResource.NotifyPM%></label>
            </div>
        <% } %>
        <div style="clear: both;"></div>
    </div>
</div>

<div class="templTitle"><%=ProjectTemplatesResource.DefineTeam %></div>
<div id="projectTeamContainer">
    <div id="Team"></div>
    <span id="manageTeamButton" class="dottedLink"><%= ProjectTemplatesResource.AddTeamMembers%></span>

    <asp:PlaceHolder runat="server" ID="projectTeamPlaceHolder" />
</div>
      
<div class="templTitle"><%=ProjectTemplatesResource.EditProjStructure %></div>

<div id="listAddedMilestone">
    
</div>

<div id="addMilestone"><a class="baseLinkAction"><%=ProjectResource.AddMilestone%></a></div>

<p class="unlocatedTaskTitle"><%=ProjectTemplatesResource.TasksWithoutMilestone%></p>
<div id="noAssignTaskContainer">
    <div id="listNoAssignListTask"></div>
    <div class="addTaskContainer">
           <a class="baseLinkAction"><%=ProjectResource.AddTask %></a>
    </div>
</div>

<div id="addMilestoneContainer" class="newProject" target="">
    <input class="textEditCalendar" id="dueDate" type="text"/>
    <input id="newMilestoneTitle" type="text" placeholder="<%=ProjectTemplatesResource.AddMilestoneTitle %>"/>
</div>

<div id="addTaskContainer" class="newProject" target="">
    <input id="newTaskTitle" type="text" placeholder="<%=ProjectTemplatesResource.AddTaskTitle %>"/>
</div>

<div class="notifyContainer">
    <div class="templTitle clearFix">
        <div style="float: left">
        <%= ProjectResource.HiddenProject %>
        </div>
        <div class="HelpCenterSwitcher" style="margin:2px 0 0 5px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForPrivateProject'});" title="<%=ProjectsCommonResource.HelpQuestionPrivateProject%>"></div> 
        <div class="popup_helper" id="AnswerForPrivateProject">
                 <p><%=String.Format(ProjectsCommonResource.HelpAnswerPrivateProject, "<br />", "<b>", "</b>")%><br />
                 <a href="http://www.teamlab.com/help/guides/set-access-rights.aspx" target="_blank"><%=ProjectsCommonResource.LearnMoreLink%></a></p>
        </div>   
    </div>
    <div class="checkboxPrivateProj">
        <input id="projectPrivacyCkeckbox" type="checkbox" />
        <label for="projectPrivacyCkeckbox"><%= ProjectResource.IUnerstandForEditHidden %></label>
    </div>
</div>

<div class="notifyContainer">
     <input id="notifyResponsibles" type="checkbox"/>
     <label for="notifyManagerCheckbox"><%=ProjectTemplatesResource.NotifyResponsibles%></label>
</div>

<div  class="buttonContainer" style="padding-top: 28px;">
    <a id="createProject" class="baseLinkButton" style="margin-right: 10px;"><%=ProjectTemplatesResource.CreateProject %></a>
    <a class="grayLinkButton" href="projectTemplates.aspx"><%=ProjectsCommonResource.Cancel%></a>
</div>

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

<div id="projectMemberPanel" class="actionPanel">
	<div class="popup-corner"></div>
	<ul nobodyItemText="<%=ProjectTemplatesResource.NoResponsible %>" chooseRespText="<%=ProjectTemplatesResource.ChooseResponsible %>" class="actionList">

	</ul>        
</div>
    
</asp:Content>