<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Masters/BasicTemplate.Master"
                        CodeBehind="ProjectAction.aspx.cs" Inherits="ASC.Web.Projects.ProjectAction" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<%@ MasterType  TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projectaction.css") %>" rel="stylesheet" type="text/css"/>
    
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("jquery.tmpl.min.js") %>"></script>
    <script src="<%= PathProvider.GetFileStaticRelativePath("projectaction.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        jq(document).ready(function() {
            projectaction.init('<%=SecurityContext.CurrentAccount.ID%>');
        });
    </script>
</asp:Content>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">

<asp:PlaceHolder runat="server" ID="premiumStubHolder"/>

<div id="pageHeader">
    <div class="pageTitle"><%= GetPageTitle() %></div>

<div style="clear: both"></div>
</div>

<div id="infoPanel">
    <div><%= ProjectsCommonResource.ChangesSaved %></div>
</div>

<script type="text/javascript">
    var brouserVersion = parseFloat(jq.browser.version);
    if ((jq.browser.msie) && (brouserVersion < 9.0)) {
        jq(".mainContainerClass > .containerHeaderBlock table td > div:last-child").remove();
    }
</script>

<div id="projectTitleContainer" class="requiredField">
    <span class="requiredErrorText"><%= ProjectsJSResource.EmptyProjectTitle %></span>
    <div class="headerPanel">
        <%= ProjectResource.ProjectTitle %>
    </div>
    <div class="inputTitleContainer">
        <asp:TextBox ID="projectTitle" Width=100% runat="server" CssClass="textEdit" MaxLength="250"></asp:TextBox>
        <div class="textBigDescribe">
            <%= ProjectResource.Example %>
        </div>
    </div>
    <% if (!IsEditingProjectAvailable()) { %>
        <div class="followingCheckboxContainer">
            <input  id="followingProjectCheckbox" type="checkbox"/>
            <label for="followingProjectCheckbox"><%= ProjectResource.FollowingProjects %></label>
            <div class="HelpCenterSwitcher" style="margin-left:3px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForFollowProject'});" title="<%=ProjectsCommonResource.HelpQuestionFollowProject%>"></div> 
        </div>

    <% } %>
    <div style="clear: both;"></div>
</div>
<div class="popup_helper" id="AnswerForFollowProject">
  <p><%=String.Format(ProjectsCommonResource.HelpAnswerFollowProject, "<br />", "<b>", "</b>")%></p>
</div>     

<div id="projectDescriptionContainer" class="dottedHeader">
    <div class="headerPanel">
        <%= ProjectResource.ProjectDescription %>
    </div>
    <div class="dottedHeaderContent">
        <asp:TextBox ID="projectDescription" Width=100% runat="server" TextMode="MultiLine" Rows="6"></asp:TextBox>
    </div>
</div>

<div id="projectManagerContainer" class="requiredField">
    <span class="requiredErrorText"><%= ProjectsJSResource.EmptyProjectManager %></span>
    <div class="headerPanel">
        <%= ProjectResource.ProjectLeader %>
    </div>
    <asp:PlaceHolder ID="projectManagerPlaceHolder" runat="server"></asp:PlaceHolder>
    <div class="notifyManagerContainer">
        <input id="notifyManagerCheckbox" type="checkbox"/>
        <label for="notifyManagerCheckbox"><%= ProjectResource.NotifyProjectManager %></label>
            <% if (IsEditingProjectAvailable()) { %>
                <input type="hidden" value="<%= Project.Responsible %>" id="projectResponsible"/>
            <% } %>
    </div>
    <div style="clear: both;"></div>
</div>

<% if (!IsEditingProjectAvailable()) { %>
    <div id="projectTeamContainer" class="dottedHeader">
        <div class="headerPanel">
                <%= ProjectResource.ProjectTeam %>            
        </div>
        <div class="HelpCenterSwitcher" style="margin:5px 0 0 5px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForProjectTeam'});" title="<%=ProjectsCommonResource.HelpQuestionProjectTeam%>"></div> 
            <div class="popup_helper" id="AnswerForProjectTeam">
                 <p><%=String.Format(ProjectsCommonResource.HelpAnswerProjectTeam, "<br />", "<b>", "</b>")%><br />
                 <a href="http://www.teamlab.com/help/guides/modify-project.aspx#step5" target="_blank"><%=ProjectsCommonResource.LearnMoreLink%></a></p>
            </div>     
        <div id="projectParticipantsContainer">
            <table class="canedit"></table>
        </div>
        <asp:PlaceHolder runat="server" ID="projectTeamPlaceHolder"/>
        <input type="hidden" id="projectParticipants"/>
    </div>
<% } else { %>
    <div id="projectStatusContainer">
        <div class="headerPanel clearFix"><div style="float:left"><%= ProjectResource.ProjectStatus %></div>
            <div class="HelpCenterSwitcher" style="margin:5px 0 0 5px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForProjectStatus'});" title="<%=ProjectsCommonResource.HelpQuestionProjectStatus%>"></div> 
            <div class="popup_helper" id="AnswerForProjectStatus">
                  <p><%=String.Format(ProjectsCommonResource.HelpAnswerProjectStatus, "<br />", "<b>", "</b>")%></p>
            </div>     
        </div>
        <select id="projectStatus" class="comboBox">
            <option value="open"><%= ProjectResource.ActiveProject %></option>
            <option value="paused"><%= ProjectResource.PausedProject %></option>
            <option value="closed"><%= ProjectResource.ClosedProject %></option>
        </select>
    </div>
    <input type="hidden" id="activeTasks" value="<%= ActiveTasksCount %>" />
    <input type="hidden" id="activeMilestones" value="<%= ActiveMilestonesCount %>" />
<% } %>
<div id="projectTagsContainer" class="dottedHeader">
    <div class="headerPanel">
        <%= ProjectResource.Tags %>
    </div>
    <div class="dottedHeaderContent">
        <asp:TextBox ID="projectTags" Width=100% runat="server" CssClass="textEdit" MaxLength="8000" autocomplete="off"></asp:TextBox>
        <div id="tagsAutocompleteContainer"></div>
        <div class="textBigDescribe">
            <%= ProjectResource.EnterTheTags %>
        </div>
    </div>
</div>

<div id="projectVisibilityContainer">
    <div class="headerPanel clearFix">
        <div style="float:left;">
        <%= ProjectResource.HiddenProject %>
        </div>
        <div class="HelpCenterSwitcher" style="margin:5px 0 0 5px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForPrivateProject'});" title="<%=ProjectsCommonResource.HelpQuestionPrivateProject%>"></div> 
        <div class="popup_helper" id="AnswerForPrivateProject">
                 <p><%=String.Format(ProjectsCommonResource.HelpAnswerPrivateProject, "<br />", "<b>", "</b>")%><br />
                 <a href="http://www.teamlab.com/help/guides/set-access-rights.aspx" target="_blank"><%=ProjectsCommonResource.LearnMoreLink%></a></p>
        </div>   
    </div>
    <div>
        <input id="projectPrivacyCkeckbox" type="checkbox" <%= RenderProjectPrivacyCheckboxValue() %>/>
        <label for="projectPrivacyCkeckbox"><%= ProjectResource.IUnerstandForEditHidden %></label>
        <input type="hidden" id="secureState" value="<%= SecurityEnable ? "1" : "0" %>"/>
    </div>
</div>

<div id="projectActionsContainer">
    <a id="projectActionButton" class="baseLinkButton">
        <%= GetProjectActionButtonTitle() %>
    </a>
    <% if (IsEditingProjectAvailable()) { %>
    <%if(ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID)){ %>
        <a id="projectDeleteButton" class="grayLinkButton">
            <%=ProjectResource.ProjectDeleted%>
        </a>
        <% } %>
    <% } else { %>
        <a class="grayLinkButton" href="projects.aspx"><%= ProjectsCommonResource.Cancel %></a>
    <% } %>
</div>

<div id='projectActionsInfoContainer' class='pm-ajax-info-block'>
    <span class="textMediumDescribe"><%= ProjectResource.LoadingWait %></span><br/>
    <img src="<%= ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" 
            alt="<%= ProjectResource.LoadingWait %>"/>
</div>

<% if (IsEditingProjectAvailable()) { %>
<script type="text/javascript">
    var status = '<%= GetProjectStatus() %>';
    jq('.dottedHeader').removeClass('dottedHeader');
    jq('#projectTitleContainer .inputTitleContainer').css('width', '100%');
    jq('#projectDescriptionContainer').show();
    jq('#notifyManagerCheckbox').attr('disabled', 'disabled');
    jq('#projectStatus option[value=' + status + ']').attr('selected', true);
    jq('#projectTagsContainer').show();
</script>
<% } %>

<div id="questionWindowDeleteProject" style="display: none">
    <ascw:Container ID="_hintPopupDeleteProject" runat="server">
        <Header><%= ProjectResource.DeleteProject %></Header>
        <Body>        
            <p><%= ProjectResource.DeleteProjectPopup %> </p>
            <p><%= ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton remove"><%= ProjectResource.DeleteProject %></a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel %></a> 
            </div>            
        </Body>
    </ascw:Container>
</div>

<div id="questionWindowActiveTasks" style="display: none">
    <ascw:Container ID="_hintPopupActiveTasks" runat="server">
    <Header>
    <%= ProjectResource.CloseProject %>
    </Header>
    <Body>        
        <p><%= ProjectResource.NotClosePrjWithActiveTasks %></p>
        <div class="popupButtonContainer">
            <a class="baseLinkButton" href="<%= GetActiveTasksUrl() %>"><%= ProjectResource.ViewActiveTasks %></a>    
            <span class="button-splitter"></span>
            <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel %></a>
        </div>
    </Body>
    </ascw:Container>
</div>

<div id="questionWindowActiveMilestones" style="display: none">
    <ascw:Container ID="_hintPopupActiveMilestones" runat="server">
    <Header>
    <%= ProjectResource.CloseProject %>
    </Header>
    <Body>        
        <p><%= ProjectResource.NotClosedPrjWithActiveMilestone %></p>
        <div class="popupButtonContainer">
            <a class="baseLinkButton" href="<%= GetActiveMilestonesUrl() %>"><%= ProjectResource.ViewActiveMilestones %></a>   
            <span class="button-splitter"></span>
            <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel %></a> 
        </div>    
    </Body>
    </ascw:Container>
</div>

<script id="projectParticipant" type="text/x-jquery-tmpl">
    <tr participantId="${ID}">
        <td class="name">
            <span class="userLink">${Name}</span>
        </td>
        <td class="department">
            <span>${Group.Name}</span>
        </td>
        <td class="title">
            <span>${Title}</span>
        </td>
        <td class="delMember">
            <span userId="${ID}" title="<%=ProjectsCommonResource.Delete %>"></span>
        </td>
    </tr>
</script>

</asp:Content>
