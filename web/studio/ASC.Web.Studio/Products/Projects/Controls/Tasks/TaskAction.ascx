<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskAction.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Tasks.TaskAction" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<link href="<%= PathProvider.GetFileStaticRelativePath("taskaction.css") %>" type="text/css"
    rel="stylesheet" />

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("taskaction.js") %>"></script>

<div id="addTaskPanel" style="display: none;">
    <ascwc:container id="addTaskContainer" runat="server">
        <header>    
            <%= TaskResource.AddTask%>
        </header>
        <body>
            <div class="pm-headerPanelSmall-splitter">
                <div class="infoPanel warn">
                    <div>
                    </div>
                </div>
            </div>
            <div class="pm-headerPanelSmall-splitter titlePanel requiredField">
                <span class="requiredErrorText title" error="<%= TaskResource.EachTaskMustHaveTitle%>">
                </span>
                <div class="headerPanelSmall">
                    <b>
                        <%= TaskResource.TaskTitle%>:</b>
                </div>
                <input class="pm-ntextbox" id="addtask_title" style="width: 99%;" maxlength="250" />
            </div>
            <div class="pm-headerPanelSmall-splitter">
                <div class="headerPanelSmall">
                    <b>
                        <%= TaskResource.TaskDescription%>:</b>
                </div>
                <textarea style="width: 99%; resize: none" class="pm-ntextbox " id="addtask_description"
                    cols="20" rows="3"></textarea>
            </div>
            <% if (!RequestContext.IsInConcreteProject()) %>
            <%{%>
            <div id="pm-projectBlock">
                <div class="pm-headerLeft">
                    <div class="requiredField">
                        <div class="headerPanelSmall">
                            <%= ProjectResource.Project%>:
                        </div>
                    </div>
                </div>
                <div class="pm-fieldRight">
                    <span class="requiredErrorText project" error="<%= TaskResource.ChooseProject%>">
                    </span>
                    <select id="taskProject" class="full-width left-align">
                        <option value="-1" class="hidden"><%= TaskResource.Select%></option>
                    </select>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <%}%>
            <div id="pm-milestoneBlock">
                <div class="pm-headerLeft">
                    <%= MilestoneResource.Milestone%>:</div>
                <div class="pm-fieldRight">
                    <select id="taskMilestone" class="full-width left-align">
                        <option value="-1" class="hidden"><%= TaskResource.Select%></option>
                        <option value="0"><%= TaskResource.None%></option>
                    </select>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <div class="pm-headerLeft userAddHeader">
                <%= TaskResource.TaskResponsible%>:</div>
            <div class="pm-fieldRight userAdd">
                <div id="responsibleSelectContainer">
                </div>
                <div id="fullFormUserList">
                </div>
                <div class="adduser">
                </div>
                <select id="taskResponsible" class="full-width left-align">
                    <option value="-1" class="hidden"><%= TaskResource.Add%></option>
                </select>
            </div>
            <div style="clear: both">
            </div>
            <div class="pm-headerLeft notify">
            </div>
            <div class="pm-fieldRight notify">
                <input type="checkbox" id="notify" checked="checked" />
                <label for="notify"><%= MessageResource.SubscribeUsers%></label>
            </div>
            <div style="clear: both" class="notify">
            </div>
            <div class="pm-headerLeft" style="line-height: 21px;">
                <%= TaskResource.DeadLine%>:</div>
            <div class="pm-fieldRight">
                <input type="text" id="taskDeadline" class="pm-ntextbox textEditCalendar" />
                <span class="splitter"></span><span class="dottedLink deadline_left" value="0">
                    <%= ProjectsCommonResource.Today %>
                </span><span class="splitter"></span><span class="dottedLink deadline_left" value="3">
                    3
                    <%= GrammaticalResource.DayGenitiveSingular%>
                </span><span class="splitter"></span><span class="dottedLink deadline_left" value="7">
                    <%= ReportResource.Week %>
                </span>
            </div>
            <div style="clear: both">
            </div>
            <div>
                <div class="pm-headerLeft">
                    <%= TaskResource.Priority%>:</div>
                <div class="pm-fieldRight">
                    <input type="checkbox" name="priority" value="1" id="priority1" /><label class="priority high"
                        for="priority1"><%= TaskResource.HighPriority%></label>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <div class="pm-h-line">
            </div>
            <div style="display: block;" class="pm-action-block">
                <a href="javascript:void(0)" class="baseLinkButton" add="<%= TaskResource.AddThisTask%>"
                    update="<%= ProjectsCommonResource.SaveChanges%>">
                    <%= TaskResource.AddThisTask%>
                </a><span class="button-splitter"></span><a class="grayLinkButton" href="javascript:void(0)"
                    onclick="javascript: jq.unblockUI();">
                    <%= ProjectsCommonResource.Cancel%>
                </a>
            </div>
            <div class="pm-ajax-info-block" style="display: none;">
                <span class="textMediumDescribe">
                    <%= TaskResource.Saving%></span><br>
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>">
            </div>
        </body>
    </ascwc:container>
</div>

<script>
jq(document).click(function(event) {
    jq.dropdownToggle().registerAutoHide(event, "#userSelector_switcher", "#userSelector_dropdown");
    jq.dropdownToggle().registerAutoHide(event, "#milestone_switcher", "#milestone_dropdown");
    jq.dropdownToggle().registerAutoHide(event, "#priority_switcher", "#priority_dropdown");
    jq.dropdownToggle().registerAutoHide(event, "#projectSelector_switcher", "#project_dropdown");
});
</script>

