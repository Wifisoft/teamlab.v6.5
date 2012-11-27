<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportFilters.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Reports.ReportFilters" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Controls.Reports" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Projects.Core.Domain.Reports" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Helpers" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>

<% if (ReportType == ReportType.MilestonesExpired || ReportType == ReportType.TasksExpired) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe">
                <%=ProjectResource.Tags%>:
            </td>
            <td class="textBigDescribe">
                <%=ProjectResource.Project%>:
            </td>
        </tr>
        <tr>
            <td>
                <select id="Tags" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeTag();">
                    <%=InitTagsDdl()%>
                </select>
            </td>
            <td>
                <select id="Projects" class="comboBox pm-report-select">
                    <%=InitProjectsDdl()%>
                </select>
            </td>
        </tr>
    </table>
</div>
<script>
    jq(function()
    {
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
</script>
<% } %>
<% if (ReportType == ReportType.MilestonesNearest) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe">
                <%=ProjectResource.Tags%>:
            </td>
            <td class="textBigDescribe">
                <%=ProjectResource.Project%>:
            </td>
            <td class="textBigDescribe">
                <%= ReportResource.ChooseTimeInterval%>:
            </td>
        </tr>
        <tr>
            <td>
                <select id="Tags" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeTag();">
                    <%=InitTagsDdl()%>
                </select>
            </td>
            <td>
                <select id="Projects" class="comboBox pm-report-select">
                    <%=InitProjectsDdl()%>
                </select>
            </td>
            <td>
                <select id="UpcomingIntervals" class="comboBox pm-report-select">
                    <%=InitUpcomingIntervalsDdl(false)%>
                </select>
            </td>
        </tr>
    </table>
</div>
<script>
    jq(function()
    {
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
</script>
<% } %>
<% if (ReportType == ReportType.UsersWithoutActiveTasks || ReportType == ReportType.UsersWorkload) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td>
                <input name="type_rbl" id="type_rbl_0" value="0" type="radio" onclick="ASC.Projects.Reports.changeReportType1(parseInt(this.value));"
                    checked="checked" />
                <label for="type_rbl_0">
                    <%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("ViewByDepartments"))%></label>
            </td>
            <td>
                <input name="type_rbl" id="type_rbl_1" value="1" type="radio" onclick="ASC.Projects.Reports.changeReportType1(parseInt(this.value));" />
                <label for="type_rbl_1">
                    <%=ReportResource.ViewByProjects%></label>
            </td>
            <% if (ReportType == ReportType.UsersWorkload) %>
            <% { %>
            <td class="tagsContent" style="display:none;"></td>
            <% } %>
        </tr>
        <tr>
            <td class="tagsContent" style="display:none;">
                <div class="textBigDescribe">
                    <%=ProjectResource.Tags%>:
                </div>
            </td>
            <td>
                <div class="textBigDescribe" id="departmentHeader" style="display:none;">
                    <%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("Department"))%>:
                </div>
                <div class="textBigDescribe" id="projectHeader" style="display:none;">
                    <%=ProjectResource.Project%>:
                </div>
            </td>
            <% if (ReportType == ReportType.UsersWorkload) %>
            <% { %>
            <td>
                <div class="textBigDescribe">
                    <%=ReportResource.User%>:
                </div>
            </td>
            <% } %>
        </tr>
        <tr>
            <td class="tagsContent" style="display:none;">
                <div>
                    <select id="Tags" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeTag();">
                        <%=InitTagsDdl()%>
                    </select>
                </div>
            </td>
            <td>
                <div id="departmentBody" style="display:none;">
                    <select id="Departments" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeDepartment();">
                        <%=InitDepartmentsDdl()%>
                    </select>
                </div>
                <div id="projectBody" style="display:none;">
                    <select id="Projects" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeProject();">
                        <%=InitProjectsDdl()%>
                    </select>
                </div>
            </td>
            <% if (ReportType == ReportType.UsersWorkload) %>
            <% { %>
            <td>
                <select id="Users" class="comboBox pm-report-select">
                    <%=InitUsersDdl()%>
                </select>
            </td>
            <% } %>
        </tr>
    </table>
</div>
<script>
    jq(function() {
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
</script>
<% } %>
<% if (ReportType == ReportType.ProjectsWithoutActiveMilestones || ReportType == ReportType.ProjectsWithoutActiveTasks || ReportType == ReportType.ProjectsList) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe" colspan="2">
                <%=ReportResource.ViewByParticipant%>:
            </td>
            <% if (ReportType == ReportType.ProjectsList) %>
            <% { %>
            <td>
            </td>
            <% } %>
        </tr>
        <tr>
            <td>
                <select id="Departments" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeDepartment();jq('#ddlUser-1').attr('selected','selected');jq('[id$=HiddenFieldForUser]').val('-1');">
                    <%=InitDepartmentsDdl()%>
                </select>
            </td>
            <td>
                <select id="Users" class="comboBox pm-report-select">
                    <%=InitUsersDdl()%>
                </select>
            </td>
            <% if (ReportType == ReportType.ProjectsList) %>
            <% { %>
            <td>
                <input id="cbxViewClosedProjects" type="checkbox" />
                <label for="cbxViewClosedProjects">
                    <%=ReportResource.ViewClosedProjects%></label>
            </td>
            <% } %>
        </tr>
    </table>
</div>
<script>
    jq(function()
    {
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
</script>
<% } %>
<% if (ReportType == ReportType.UsersActivity) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe" colspan="2">
                <%= ReportResource.ChooseTimeInterval%>:
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="width: 450px;">
                    <div style="float: left;">
                        <select id="TimeIntervals" class="comboBox pm-report-select" onchange="javascript:changeInterval();">
                            <%=InitTimeIntervalsDdl()%>
                        </select>
                    </div>
                    <div id="otherInterval" style="display:none;">
                        <div style="float: left; padding-left: 15px; padding-right: 15px;">
                            <asp:TextBox runat="server" ID="usersActivityFromDate" CssClass="pm-ntextbox textEditCalendar" Width="75px" />
                        </div>
                        <div>
                            <asp:TextBox runat="server" ID="usersActivityToDate" CssClass="pm-ntextbox textEditCalendar" Width="75px" />
                        </div>
                    </div>
                    <div style="clear: left;"></div>    
                </div>         
            </td>
        </tr>
        <tr>
            <td class="textBigDescribe" width="195px">
                <%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("Department"))%>:
            </td>
            <td class="textBigDescribe">
                <%=ReportResource.User%>:
            </td>
        </tr>
        <tr>
            <td>
                <select id="Departments" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeDepartment();">
                    <%=InitDepartmentsDdl()%>
                </select>
            </td>
            <td>
                <select id="Users" class="comboBox pm-report-select">
                    <%=InitUsersDdl()%>
                </select>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript" language="javascript">
    jq(function() {
        jq("[id$=usersActivityFromDate],[id$=usersActivityToDate]").mask('<%= System.DateTimeExtension.DateMaskForJQuery %>');
        jq("[id$=usersActivityFromDate],[id$=usersActivityToDate]").datepick({ selectDefaultDate: true, showAnim: '' });
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
    function changeInterval()
    {
        if(jq('#TimeIntervals option:selected').val()=='0')
            jq('#otherInterval').show();
        else
            jq('#otherInterval').hide();
    }
</script>
<% } %>
<% if (ReportType == ReportType.TimeSpend) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe" colspan="2">
                <%= ReportResource.ChooseTimeInterval%>:
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="width: 450px;">
                    <div style="float: left;">
                        <select id="TimeIntervals" class="comboBox pm-report-select" onchange="javascript:changeInterval();">
                            <%=InitTimeIntervalsDdl()%>
                        </select>
                    </div>
                    <div id="otherInterval" style="display:none;">
                        <div style="float: left; padding-left: 15px; padding-right: 15px;">
                            <asp:TextBox runat="server" ID="timeSpendFromDate" CssClass="pm-ntextbox textEditCalendar" Width="75px" />
                        </div>
                        <div>
                            <asp:TextBox runat="server" ID="timeSpendToDate" CssClass="pm-ntextbox textEditCalendar" Width="75px" />
                        </div>
                    </div>
                    <div style="clear: left;"></div>    
                </div>    
            </td>
        </tr>
        <tr>
            <td class="textBigDescribe" width="195px">
                <%=HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("Department"))%>:
            </td>
            <td class="textBigDescribe">
                <%=ReportResource.User%>:
            </td>
        </tr>
        <tr>
            <td>
                <select id="Departments" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeDepartment();">
                    <%=InitDepartmentsDdl()%>
                </select>
            </td>
            <td>
                <select id="Users" class="comboBox pm-report-select">
                    <%=InitUsersDdl()%>
                </select>
            </td>
        </tr>
        <tr>
            <td>
                <input name="type_rbl" id="type_rbl_0" value="0" type="radio" onclick="jq('[id$=HiddenFieldForType]').val(this.value);jq('#reportDescription').html('<%=ReportResource.ReportTimeSpendSummary_Description%>')"
                    checked="checked" />
                <label for="type_rbl_0">
                    <%=ReportResource.ViewByUsers%></label>
            </td>
            <td>
                <input name="type_rbl" id="type_rbl_1" value="1" type="radio" onclick="jq('[id$=HiddenFieldForType]').val(this.value);jq('#reportDescription').html('<%=ReportResource.ReportTimeSpend_Description%>');" />
                <label for="type_rbl_1">
                    <%=ReportResource.ViewByUserTasks%></label>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript" language="javascript">
    jq(function() {
        jq("[id$=timeSpendFromDate],[id$=timeSpendToDate]").mask('<%= System.DateTimeExtension.DateMaskForJQuery %>');
        jq("[id$=timeSpendFromDate],[id$=timeSpendToDate]").datepick({ selectDefaultDate: true, showAnim: '' });
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
    function changeInterval()
    {
        if(jq('#TimeIntervals option:selected').val()=='0')
            jq('#otherInterval').show();
        else
            jq('#otherInterval').hide();
    }
</script>
<% } %>
<% if (ReportType == ReportType.TasksByProjects || ReportType == ReportType.TasksByUsers) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=ReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <% if (ReportType == ReportType.TasksByProjects) %>
            <% { %>
            <td class="textBigDescribe">
                <%=ProjectResource.Tags%>:
            </td>
            <% } %>
            <td class="textBigDescribe">
                <%=ReportType == ReportType.TasksByProjects ? ProjectResource.Project : HttpUtility.HtmlEncode(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<ReportResource>("Department"))%>:
            </td>
            <td class="textBigDescribe">
                <%=ReportResource.User%>:
            </td>
        </tr>
        <tr>
            <% if (ReportType == ReportType.TasksByProjects) %>
            <% { %>
            <td>
                <select id="Tags" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeTag();">
                    <%=InitTagsDdl()%>
                </select>
            </td>
            <% } %>
            <td>
                <% if (ReportType == ReportType.TasksByProjects) %>
                <% { %>
                <select id="Projects" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeProject();">
                    <%=InitProjectsDdl()%>
                </select>
                <% } %>
                <% if (ReportType == ReportType.TasksByUsers) %>
                <% { %>
                <select id="Departments" class="comboBox pm-report-select" onchange="ASC.Projects.Reports.changeDepartment();">
                    <%=InitDepartmentsDdl()%>
                </select>
                <% } %>
            </td>
            <td>
                <select id="Users" class="comboBox pm-report-select"  onchange="ASC.Projects.Reports.changeResponsible();">
                    <%=InitUsersDdl()%>
                </select>
            </td>
        </tr>
    </table>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe" align="right">
                <div style="padding-top: 3px;">
                    <%= TaskResource.DeadLine%>:</div>
            </td>
            <td style="padding-left: 9px;">
                <select id="UpcomingIntervals" class="comboBox pm-report-select">
                    <%=InitUpcomingIntervalsDdl(true)%>
                </select>
            </td>
        </tr>
        <tr>
            <td class="textBigDescribe" align="right">
                <div style="padding-top: 3px;">
                    <%= ReportResource.ShowTasks%>:</div>
            </td>
            <td style="padding-left: 0px;">
                <asp:RadioButtonList ID="cblTaskStatus" runat="server" RepeatDirection="Horizontal" OnPreRender="cblTaskStatus_onDataBound">
                    <asp:ListItem Value="0" Selected="True" />
                    <asp:ListItem Value="1" Selected="False" />
                    <asp:ListItem Value="2" Selected="False" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <% if (ReportType == ReportType.TasksByProjects) %>
        <% { %>
        <tr>
            <td class="textBigDescribe" align="right">
            </td>
            <td>
                <input id="cbxShowTasksWithoutResponsible" type="checkbox" <% if (ReportType == ReportType.TasksByUsers)%><%{%>disabled="disabled"<%}%>/>
                <label style="padding-top: 3px;" for="cbxShowTasksWithoutResponsible">
                    <%= TaskResource.ShowTasksWithoutResponsible%></label>
            </td>
        </tr>
        <% } %>
    </table>
</div>
<script>
    jq(function()
    {
        jq("#UpcomingIntervals").removeAttr("disabled");
        ASC.Projects.Reports.setFiltersValueInObjectsFromURL(); 
    });
    function setDisableAttr(val)
    {
        if(val == 1)
        {
            jq("#UpcomingIntervals option:first").attr("selected","selected");
            jq("#UpcomingIntervals").attr("disabled","disabled");
        }
        else
        {
            jq("#UpcomingIntervals").removeAttr("disabled");
        }
            
    }
</script>
<% } %>
<% if (ASC.Core.SecurityContext.IsAuthenticated) { %>
<input id="cbxSaveAsTemplate" type="checkbox" onclick="javascript:if(jq(this).is(':checked')){viewReportTemplateContainer(-1,'<%= GetReportTypeTitle() %>');}" />
<label for="cbxSaveAsTemplate">
    <%=ReportResource.SaveAsTemplate%>
</label>
 <div class="HelpCenterSwitcher" style="margin:02x 0 0 3px;" onclick="jq(this).helper({ BlockHelperID: 'AnswerForSaveTemplate'});" title="<%=ProjectsCommonResource.HelpQuestionSaveTemplate%>"></div> 
 <div class="popup_helper" id="AnswerForSaveTemplate">
    <p><%=String.Format(ProjectsCommonResource.HelpAnswerSaveTemplate, "<br />", "<b>", "</b>")%>
    <a target="_blank" href="http://teamlab.com/help/tipstricks/re port-templates.aspx"> <%=ProjectsCommonResource.LearnMoreLink%></a></p>
 </div>     
<% } %>
<div class="pm-h-line">
    <!– –></div>

<asp:PlaceHolder ID="reportTemplatePh" runat="server"></asp:PlaceHolder>

<div>
    <div class="pm-action-block">
        <a onclick="ASC.Projects.Reports.generateReportInNewWindow();" class="baseLinkButton">
            <%=ReportResource.GenerateReport%>
        </a>
    </div>
    <div class='pm-ajax-info-block' style="display: none;">
        <span class="textMediumDescribe">
            <%= ReportResource.BuildingReport%></span><br />
        <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
    </div>
</div>
<asp:HiddenField ID="HiddenFieldViewReportFilters" runat="server" />
<asp:HiddenField ID="HiddenFieldViewReportTemplate" runat="server" />