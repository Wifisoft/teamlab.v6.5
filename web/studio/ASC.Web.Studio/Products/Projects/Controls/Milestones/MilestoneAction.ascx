<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestoneAction.ascx.cs" Inherits="ASC.Web.Projects.Controls.Milestones.MilestoneAction" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<link href="<%= PathProvider.GetFileStaticRelativePath("milestoneaction.css") %>" type="text/css" rel="stylesheet"/>
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("milestoneaction.js") %>"></script>
<script type="text/javascript">
    jq(document).ready(function() {
        milestoneaction.init('<%= SecurityContext.CurrentAccount.ID %>');
    });
</script>


<div id="milestoneActionPanel">
    <ascwc:container id="milestoneActionContainer" runat="server">
        <header>
            <%= ProjectResource.AddMilestone %>
        </header>
        <body>
            <div id="milestoneTitleContainer" class="requiredField">
                <span class="requiredErrorText"><%= MilestoneResource.NoTitleMessage %></span>
                <div class="headerPanelSmall">
                    <%= MilestoneResource.Title %>:
                </div>
                <input id="milestoneTitleInputBox" MaxLength="250" />
            </div>
            
            <div id="milestoneDescriptionContainer">
                <div class="headerPanelSmall">
                    <%= MilestoneResource.Description %>:
                </div>
                <textarea id="milestoneDescriptionInputBox" cols="20" rows="3"  MaxLength="250"></textarea>
            </div>
            
            <% if (!RequestContext.IsInConcreteProject()) %>
            <% { %>
            <div id="milestoneProjectContainer" class="requiredField">
                <span class="requiredErrorText"><%= MilestoneResource.ChooseProject %></span>
                <div class="headerPanelSmall">
                    <%= ProjectResource.Project %>:
                </div>
                <select id="milestoneProject" class="full-width">
                    <option value="-1" class="hidden"><%= ProjectsCommonResource.Select %></option>
                </select>
            </div>
            <% } %>
            
            <div id="milestoneResponsibleContainer" class="requiredField">
                <span class="requiredErrorText"><%= MilestoneResource.ChooseResponsible %></span>
                <div class="headerPanelSmall">
                    <%= MilestoneResource.Responsible %>:
                </div>
                <select id="milestoneResponsible" class="full-width">
                  <option value="-1" class="hidden"><%= ProjectsCommonResource.Select %></option>
                </select>
                <div class="notifyResponsibleContainer">
                    <input id="notifyResponsibleCheckbox" type="checkbox" checked="checked"/>
                    <label for="notifyResponsibleCheckbox"> <%= MilestoneResource.NotifyResponsible %></label>
                </div>
            </div>
            <div style="clear: both"></div>
                
            <div id="milestoneDeadlineContainer" class="requiredField">
                <div class="headerPanelSmall">
                    <%= MilestoneResource.MilestoneDeadline %>:
                </div>
                <div id="milestoneDeadlineSelectorContainer">
                    <input type="text" id="milestoneDeadlineInputBox" class="textEditCalendar"/>
                    <span class="dottedLink deadlineLeft" value="7">
                        <%= MilestoneResource.DeadlineInWeek %>
                    </span>
                    <span class="dottedLink deadlineLeft" value="30">
                        <%= MilestoneResource.DeadlineInMonth %>
                    </span>
                    <span class="dottedLink deadlineLeft" value="60">
                        <%= MilestoneResource.DeadlineInTwoMonths %>
                    </span>
                </div>
            </div>
                        
            <div id="milestoneKeyContainer">
                <input id="milestoneKeyCheckBox" type="checkbox"/>
                <label for="milestoneKeyCheckBox"> <%= MilestoneResource.RootMilestone %></label>
                <div class="HelpCenterSwitcher" style="margin:3px 0 0 3px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForProjectMilestone',popup:true});" title="<%=ProjectsCommonResource.HelpQuestionProjectMilestone%>"></div> 
                <div class="popup_helper" id="AnswerForProjectMilestone">
                  <p><%=String.Format(ProjectsCommonResource.HelpAnswerProjectMilestone, "<br />", "<b>", "</b>")%></p>
                </div>     
            </div>
                
            <div id="milestoneNotifyManagerContainer">
                <input id="milestoneNotifyManagerCheckBox" type="checkbox"/>
                <label for="milestoneNotifyManagerCheckBox"> <%= MilestoneResource.RemindMe %></label>
            </div>
              
            <div class="pm-h-line"></div>
            
            <div id="milestoneActionButtonsContainer">
                <a id="milestoneActionButton" href="javascript:void(0)" class="baseLinkButton" 
                    add="<%= MilestoneResource.AddMilestoneButton %>" update="<%= ProjectsCommonResource.SaveChanges %>">
                </a>
                <a id="milestoneActionCancelButton" href="javascript:void(0)" class="grayLinkButton" onclick="javascript:jq.unblockUI();">
                    <%= ProjectsCommonResource.Cancel %>
                </a>
            </div>
            
            <div id="milestoneActionProcessContainer">
                <div class="textMediumDescribe">
                    <%= ProjectResource.LoadingWait %>
                </div>
                <img src="<%= ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>"
                        alt="<%= ProjectResource.LoadingWait %>"/>
            </div> 
        </body>
    </ascwc:container>
</div>
