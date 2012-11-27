<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Masters/BasicTemplate.Master"
            CodeBehind="Milestones.aspx.cs" Inherits="ASC.Web.Projects.Milestones" %>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("milestones.css") %>" type="text/css" rel="stylesheet"/>
    <link href="<%= PathProvider.GetFileStaticRelativePath("subtasks.css") %>" type="text/css" rel="stylesheet"/>

    <% if (RequestContext.IsInConcreteProject()) { %>
        <script type="text/javascript">
            jq(document).ready(function() {
                serviceManager.init("<%=SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project",
                    "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>", "<%=RequestContext.GetCurrentProjectId().ToString(CultureInfo.InvariantCulture)%>");
            });
        </script>
    <% } else { %>
      <script type="text/javascript">
          jq(document).ready(function() {
              serviceManager.init("<%=SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project", "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>");
          });
      </script>
    <% } %>
</asp:Content>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <% if (CanCreateMilestone())
       {%>
        <span id="newMilestoneButton">
            <span class="dottedLink"><%= MilestoneResource.CreateNewMilestone %></span>
        </span>
    <% } %>

    <asp:PlaceHolder runat="server" ID="contentPlaceHolder"></asp:PlaceHolder>
    <div class="presetContainer">
        <a id="preset_my" class="baseLinkAction"><%= ProjectsFilterResource.MyMilestones %></a>
        <span>,</span>
        <% if (!RequestContext.IsInConcreteProject())
           {%>
        <a id="preset_inmyproj" class="baseLinkAction"><%= ProjectsFilterResource.MilestonesInMyProj %></a>
        <span>,</span>
        <% } %>
        <a id="preset_upcoming" class="baseLinkAction"><%= ProjectsFilterResource.UpcomingMilestones %></a>
    </div>

    <div id="questionWindowTasks" style="display: none">
        <ascw:Container ID="_hintPopupTasks" runat="server">
            <Header>
                <%= MilestoneResource.CloseMilestone %>
            </Header>
            <Body>
                <p><%= MilestoneResource.NotCloseMilWithActiveTasks %></p>
                <div class="popupButtonContainer">
                    <a class="baseLinkButton" id="linkToTasks"><%= ProjectResource.ViewActiveTasks %></a>
                    <span class="button-splitter"></span>
                    <a class="grayLinkButton"><%= ProjectsCommonResource.Cancel %></a>
                </div>
            </Body>
        </ascw:Container>
    </div>

    <div id="questionWindowDeleteMilestone" style="display: none">
        <ascw:Container ID="_hintPopupTaskRemove" runat="server">
        <Header>
            <%= MilestoneResource.DeleteMilestone %>
        </Header>
        <Body>
            <p><%= MilestoneResource.DeleteMilestonePopup %> </p>
            <p><%= ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton remove"><%= MilestoneResource.DeleteMilestone %></a>
                <span class="button-splitter"></span>
                <a class="grayLinkButton cancel"><%= ProjectsCommonResource.Cancel %></a>
            </div>
        </Body>
        </ascw:Container>
    </div>

    <div id="milestonesListContainer">
        <div id="EmptyListMilestone" class="noContent"></div>
        <div id="EmptyListForFilter" class="noContent"></div>
        <table id="milestonesList">
            <thead>
                <tr>
                    <th class="status"></th>
                    <th class="title"><%= MilestoneResource.Title %></th>
                    <th class="tasksCount" colspan="3"><%= TaskResource.Tasks %></th>
                    <th class="deadline"><%= MilestoneResource.DeadlineHeader %></th>
                    <th class="responsible"><%= MilestoneResource.Responsible %></th>
                    <th class="actions"></th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
        <div id="showNextMilestonesProcess"></div>
    </div>
    <span id="showNextMilestonesButton"><%= MilestoneResource.ShowNextMilestones %></span>

    <div id="statusListContainer">
        <div class="containerTop"></div>
        <ul>
            <li class="open"><%= MilestoneResource.StatusOpen %></li>
            <li class="closed"><%= MilestoneResource.StatusClosed %></li>
        </ul>
    </div>

    <div id="descriptionPanel" class="actionPanel" objid="">
        <div class="popup-corner"></div>
        <div class="param">
            <% if (!RequestContext.IsInConcreteProject()) { %>
            <div class="project"><%= ProjectResource.Project %>:</div>
            <% } %>
            <div class="createdby"><%= MilestoneResource.CreatedBy %>:</div>
            <div class="created"><%= MilestoneResource.CreatingDate %>:</div>
            <div class="description"><%= MilestoneResource.Description %>:</div>
        </div>
        <div class="value">
            <% if (!RequestContext.IsInConcreteProject()) { %>
            <div class="project"><a></a></div>
            <% } %>
            <div class="createdby"></div>
            <div class="created"></div>
            <div class="description"></div>
        </div>
    </div>

    <div id="milestoneActionContainer">
        <div class="containerTop"></div>
        <div>
            <div id="updateMilestoneButton"><span><%= ProjectsCommonResource.Edit %></span></div>
            <div id="removeMilestoneButton"><span><%= ProjectsCommonResource.Delete %></span></div>
            <div id="addMilestoneTaskButton"><span><%= TaskResource.AddTask %></span></div>
        </div>
    </div>

    <asp:PlaceHolder runat="server" ID="milestoneListPlaceHolder"></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="milestoneActionPlaceHolder"></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="taskActionPlaceHolder"></asp:PlaceHolder>

</asp:Content>
