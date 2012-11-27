<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
    CodeBehind="projectTeam.aspx.cs" Inherits="ASC.Web.Projects.ProjectTeam" %>
<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Web.Studio.Controls.Common" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<%@ Import Namespace="ASC.Web.Projects.Resources" %>


<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js") %>"></script>
    <script type="text/javascript" language="javascript">
        jq(function() {
            serviceManager.init("<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/')%>" + "/project", "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>", "<%=Project.ID%>");

            var onAddTask = function() {
                jq.unblockUI();
            }

            Teamlab.bind(Teamlab.events.addPrjTask, onAddTask);

            UserSelector.OnOkButtonClick = function() {
                var userID = new Array();

                jq(UserSelector.GetSelectedUsers()).each(function(i, el) { userID.push(el.ID); });

                var notifyIsChecked = jq('#notify').is(':checked');
                var projId =parseInt(jq.getURLParam("prjID"));
                AjaxPro.Project.UserManager(userID.join(","), notifyIsChecked, projId,
               function(res) {

                   jq("#team_container").css('display', 'block');
                   jq("#button").css('display', 'block');
                   jq("#empty").css('display', 'none');

                   jq("#team_container").html(res.value);
                   jq("#team_container").unblock();


                   jq("div.NewUserInTeam").each(function() {
                       jq(this).css({ "background-color": "#ffffcc" });
                       jq(this).animate({ backgroundColor: "#ffffff" }, 1000);

                       var src = jq(jq(this).find("table td:first img")).attr("src");
                       var name = jq(jq(this).find("table td:last a")).text().trim();
                       var title = jq(jq(this).find("table td:last div")).text().trim();
                       var id = jq(jq(this).find("a[id^=actionPanelSwitcher]")).attr("id").split("_")[1];

                       var a = jq("<a></a>").addClass("pm-dropdown-item").css("min-height", "45px").bind('click', function() {
                           ASC.Projects.TaskActionPage.changeResponsible(id);
                       });
                       var img = jq("<img>").attr("src", src).css("float", "left").css("padding", "4px");
                       var nameDiv = jq("<div></div>").attr("id", "userName_" + id).css("padding", "4px 0pt 0pt 40px").text(name);
                       var titleDiv = jq("<div></div>").addClass("pm-grayText").css("padding", "4px 0pt 0pt 40px").text(title);
                       var infoDiv = jq("<div></div>").append(nameDiv).append(titleDiv);
                       jq(a).append(img).append(infoDiv);
                       jq("#userSelector_dropdown").append(a);

                   });

                   jq("div.OldUserInTeam").each(function() {
                       var switcher = jq(this).find("a[id^=actionPanelSwitcher]");
                       if (switcher.length > 0) {
                           var id = jq(switcher).attr("id").split("_")[1];
                           jq("#userSelector_dropdown #userName_" + id).parent().parent().remove();
                       }
                       jq(this).animate({ opacity: "hide" }, "slow");
                   });

                   jq("div").each(function() {
                       jq(this).removeClass("OldUserInTeam");
                       jq(this).removeClass("NewUserInTeam");
                   });

               });
            }
        });

        function showNewTaskPopup(userid, displayName) {
          jq('.actionPanel').hide();
          jq('#addTaskPanel .pm-fieldRight.notify').show();
          jq('#addTaskPanel .pm-headerLeft.notify').show();
          jq('#addTaskPanel .baseLinkButton').html(jq('#addTaskPanel .baseLinkButton').attr('add'));
          jq('#addTaskPanel .containerHeaderBlock table td:first').html(ProjectJSResources.CreateNewTask);
          taskaction.showTaskForm(false);
          jq('#fullFormUserList').show();
          jq('#fullFormUserList div').remove();
          jq('#fullFormUserList').append('<div value="' + userid + '" class="user">' + displayName + '</div>');
          
          if (jq("#addTaskPanel #fullFormUserList div.user").length == jq("#addTaskPanel #fullFormUserList div.user[value=" + serviceManager.getMyGUID() + "]").length) {
              jq('#addTaskPanel #notify').attr('disabled', 'true');
          } else {
              jq('#addTaskPanel #notify').removeAttr('disabled');
          }  
                  
          return false;
      };
      
    function changePermission(obj, id, module)
    {
        var prjID = jq.getURLParam("prjID");
        var visible = jq(obj).hasClass("pm-projectTeam-modulePermissionOn");

        AjaxPro.Project.SetTeamSecurity(prjID, id, module, !visible, function(res)
        {
            if (res.error != null) {alert(res.error.Message);return;}
            if(visible)
                jq(obj).removeClass("pm-projectTeam-modulePermissionOn").addClass("pm-projectTeam-modulePermissionOff");
            else
                jq(obj).removeClass("pm-projectTeam-modulePermissionOff").addClass("pm-projectTeam-modulePermissionOn");
        });                
    }

    jq(document).click(function(event) 
    {
        var elt = (event.target) ? event.target : event.srcElement;
        var isHide = true;
        if(jq(elt).hasClass('pm-dropdown-item'))
        {
            jq("#actionPanel").hide();
        }
        if (jq(elt).is('[id="actionPanel"]') || jq(elt).is('[id^="actionPanelSwitcher_"]'))
            isHide = false;
        if (isHide)
            jq(elt).parents().each(function() {
                if (jq(this).is('[id="actionPanel"]') || jq(this).is('[id^="actionPanelSwitcher_"]')) {
                    isHide = false;
                }
            });    

        if (isHide)
            jq("#actionPanel").hide();
    });
    
    function showActions(obj, id)
    {
        var prjID = jq.getURLParam("prjID");
        AjaxPro.Project.GetActionContent(prjID, id, function(res){
            if (res.error != null) {alert(res.error.Message);return;}
            jq('#actionPanelContent').html(res.value);
            jq.dropdownToggle().toggle(jq(obj).children()[1],'actionPanel',5,-20);
            jq("#actionPanel").show();
        });
    }
    jq(function() {
        jq('#PrivateProjectHelp').click(function() {
            jq(this).helper({ BlockHelperID: 'AnswerForPrivateProjectTeam' });
        });

        jq('#RestrictAccessHelp').click(function() {
            jq(this).helper({ BlockHelperID: 'AnswerForRestrictAccessTeam' });
        });
    });
    </script>
    <script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("reports.js") %>"></script>
    <link href="<%= PathProvider.GetFileStaticRelativePath("projectTeam.css") %>" rel="stylesheet" type="text/css" />

</asp:Content>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
       
    <div class="headerBase pm-headerPanel-splitter">
        <%= ProjectResource.ProjectLeader%>
    </div>
    
    <div class="pm-headerPanel-splitter clearFix">
        <div class="pm-projectTeam-projectLeaderCard" style="float:left;">
            <asp:PlaceHolder runat="server" ID="_phProjectLeaderCard" />
        </div>
        <div style="margin-left: 380px;padding: 0 20px;">
            <div><%=ProjectResource.ClosedProjectTeamManagerPermission%>:</div>
            <div class="pm-projectTeam-infoText"><%=ProjectResource.ClosedProjectTeamManagerPermissionArticleA%></div>
            <div class="pm-projectTeam-infoText"><%=String.Format(ProjectResource.ClosedProjectTeamManagerPermissionArticleB, "<span id='PrivateProjectHelp' class='baseLinkAction'>","</span>")%></div>
            <div class="pm-projectTeam-infoText"><%=String.Format(ProjectResource.ClosedProjectTeamManagerPermissionArticleC, "<span id='RestrictAccessHelp' class='baseLinkAction'>","</span>","<br/>")%></div>
            
            <div class="popup_helper" id="AnswerForPrivateProjectTeam">
                <p><%=String.Format(ProjectsCommonResource.HelpAnswerPrivateProjectTeam, "<br />", "<b>", "</b>")%>
                <a target="_blank" href="http://teamlab.com/help/guides/restrict-access-to-project.aspx"> <%=ProjectsCommonResource.LearnMoreLink%></a></p>
            </div> 
            
            <div class="popup_helper" id="AnswerForRestrictAccessTeam">
                <p><%=String.Format(ProjectsCommonResource.HelpAnswerRestrictAccessTeam, "<br />", "<b>", "</b>")%>
                <a target="_blank" href="http://teamlab.com/help/guides/set-access-rights.aspx"> <%=ProjectsCommonResource.LearnMoreLink%></a></p>
            </div>     
            
        </div>
    </div>
    
    <div class="pm-headerPanel-splitter">
        <div class="headerBase pm-headerPanelSmall-splitter">
            <% if (ASC.Projects.Engine.ProjectSecurity.CanEditTeam(Project)) %>
            <% { %>
            <div class="pm-projectTeam-container" id="button" style="float:right;">
                <a class="baseLinkButton" onclick="javascript:UserSelector.ShowDialog();"><%=ProjectResource.ManagmentTeam%></a>    
            </div>
            <% } %>
 <%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ProjectResource>("EmployeesAndPermissions"))%>
        </div>
        <% if (Project.Private) %>
        <% { %>
        <div style="background-color:#F2F2F2;padding: 10px 20px;">
            <table width="100%" cellpadding="3" cellspacing="0">
                <tbody>
                    <tr>
                        <td><b><%=MessageResource.Messages%></b></td>
                        <td><b><%=ProjectsFileResource.Documents%></b></td>
                        <td><b><%=TaskResource.AllTasks%></b></td>
                        <td><b><%=MilestoneResource.Milestones%></b></td>
                    </tr>
                    <tr>
                        <td width="25%" valign="top"><%=ProjectResource.ClosedProjectTeamDiscussionsInfoPanel%></td>
                        <td width="25%" valign="top"><%=ProjectResource.ClosedProjectTeamDocumentsInfoPanel%></td>
                        <td width="25%" valign="top"><%=ProjectResource.ClosedProjectTeamAllTasksInfoPanel%></td>
                        <td width="25%" valign="top"><%=ProjectResource.ClosedProjectTeamMilestonesInfoPanel%></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <% } %>
    </div>
   
    <div id="team_container" class="pm-headerPanel-splitter">
        <asp:Literal runat="server" ID="_ltlTeam" />        
    </div>
     
    <% if (ASC.Projects.Engine.ProjectSecurity.CanEditTeam(Project)) %>
    <% { %>
    <div class="pm-projectTeam-container" id="button">
        <asp:PlaceHolder runat="server" ID="_phUserSelector" />
        <a class="baseLinkButton" onclick="javascript:UserSelector.ShowDialog();"><%=ProjectResource.ManagmentTeam%></a>    
    </div>
    <% } %>
    
    <asp:PlaceHolder runat="server" ID="phAddTaskPanel" />

</asp:Content>

<asp:Content ID="SidePanel" ContentPlaceHolderID="BTSidePanel" runat="server">
</asp:Content>
