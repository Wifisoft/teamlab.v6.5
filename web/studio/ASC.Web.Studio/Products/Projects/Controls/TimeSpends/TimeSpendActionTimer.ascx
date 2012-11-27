<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="'ASC.Projects.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSpendActionTimer.ascx.cs" Inherits="ASC.Web.Projects.Controls.TimeSpends.TimeSpendActionTimer" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("apitimetraking.js") %>"></script>
<script>
    var buttonStyle = { 
                        "playButton": {"src": "<%= GetPlayButtonImg() %>", "title": "<%= ProjectsCommonResource.AutoTimerStart %>"},
                        "pauseButton": {"src": "<%= GetPauseButtonImg() %>", "title": "<%= ProjectsCommonResource.AutoTimerPause %>"},
                        "isPlay": true
                      };
</script>                     
<div id="timerTime">
    <div id="firstViewStyle">
        <div class="h">00</div>
        <div class="m">00</div>
        <div class="s">00</div>
        <div class="clearfix">
            <div class="start" title="<%= ProjectsCommonResource.AutoTimerStart %>"><div></div></div>
            <div class="reset" title="<%= ProjectsCommonResource.AutoTimerReset %>"><div></div></div>
        </div>
    </div>
    <div class="pm-headerPanelSmall-splitter">
        <div class="headerPanelSmall">
            <b><%= ProjectResource.Project %>:</b>
        </div>
        <select id="selectUserProjects" class="comboBox">
            <% foreach (var project in UserProjects) %>
            <% { %>
                <% if (project.ID == Project.ID) %>
                <% { %>
                <option selected="selected" value="<%= project.ID %>" id="option1"><%= project.Title.HtmlEncode()%></option>
                <% } %>
                <% else %>
                <% { %>
                <option value="<%= project.ID %>" id="optionUserProject_<%= project.ID %>"><%= project.Title.HtmlEncode()%></option>
                <% } %>
            <% } %>
        </select>
    </div>
    <div class="pm-headerPanelSmall-splitter">
        <div class="headerPanelSmall">
            <b><%= TaskResource.Task %>:</b>
        </div>
        <select id="selectUserTasks" class="comboBox">
            <optgroup id="openTasks" label="<%= TimeTrackingResource.OpenTasks %>">
            <% foreach (var task in OpenUserTasks) %>
            <% { %>
                <option <% if (Target != -1 && task.ID == Target){%> selected="selected"<% } %> value="<%= task.ID %>" id="optionUserTask_<%= task.ID %>"><%= task.Title.HtmlEncode()%></option>
            <% } %>
            </optgroup>
            <optgroup id="closedTasks"  label="<%= TimeTrackingResource.ClosedTasks %>">
            <% foreach (var task in ClosedUserTasks) %>
            <% { %>
                <option <% if (Target != -1 && task.ID == Target){%> selected="selected"<% } %> value="<%= task.ID %>"  id="optionUserTask_<%= task.ID %>"><%= task.Title.HtmlEncode()%></option>
            <% } %>
            </optgroup>
        </select>
    </div>
    <div class="pm-headerPanelSmall-splitter">
        <div class="headerPanelSmall">
            <b><%= TaskResource.ByResponsible %>:</b>
        </div>
        <select id="teamList" class="comboBox">
            <% foreach (var user in Users) %>
            <% { %>
                <% if (user.ID == CurrentUser) %>
                <% { %>
                <option selected="selected" value="<%= user.ID %>" id="optionUser_<%= user %>"><%= DisplayUserSettings.GetFullUserName(user.UserInfo)%></option>
                <% } %>
                <% else %>
                <%{%>
                <option value="<%= user.ID %>" id="optionUser_<%= user.ID %>"><%= DisplayUserSettings.GetFullUserName(user.UserInfo)%></option>
                <% } %>
            <% } %>        
        </select>
    </div>  
    <div class="pm-headerPanelSmall-splitter">
        <div class="headerPanelSmall">
            <b><%= TimeTrackingResource.Date%>:</b>
        </div>
        <input type="text" id="inputDate" class="pm-ntextbox textEditCalendar" />
    </div>      
    <div class="pm-headerPanelSmall-splitter">
        <div class="headerPanelSmall">
            <b><%= ProjectResource.ManualTimeInput%>:</b>
        </div>
        <input id="inputTimeHours" type="text" placeholder="<%=ProjectsCommonResource.WatermarkHours %>" class="textEdit" maxlength="2" />
        <span class="splitter">:</span>
        <input id="inputTimeMinutes" type="text" placeholder="<%=ProjectsCommonResource.WatermarkMinutes %>" class="textEdit" maxlength="2" />
        <div id="timeTrakingErrorPanel"></div>
    </div>
    <div class="pm-headerPanelSmall-splitter">
        <div class="headerPanelSmall">
            <b><%= ProjectResource.ProjectDescription %>:</b>
        </div>
        <textarea id="textareaTimeDesc" style="resize:none;" class="pm-ntextbox " MaxLength="250"></textarea>
    </div>
    <div class="pm-headerPanelSmall-splitter">
        <a class="highLinkButton" id="addLog">
            <%= ProjectsCommonResource.AutoTimerLogHours %>
        </a>
    </div>    
</div>