<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskActionView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Tasks.TaskActionView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="aswc" %>

<script type="text/javascript" language="javascript">

    jq(function() {
        ASC.CRM.TaskActionView.init('<%= DateTimeExtension.DateMaskForJQuery %>', '<%= SecurityContext.CurrentAccount.ID %>');
    });

    function selectResponsible(guid, name) {
        if (guid == "<%=SecurityContext.CurrentAccount.ID%>") {
            jq("#notifyResponsible").attr("checked", false).attr("disabled", true);
        }
        else {
            jq("#notifyResponsible").attr("checked", true).attr("disabled", false);
        }
    }

</script>

<div id="addTaskPanel" style="display: none;">
    <ascwc:container id="_taskContainer" runat="server">
        <header>
        <%= CRMTaskResource.AddNewTask%>
        </header>
        <body>
            <div class="headerPanel-splitter requiredField">
                <span class="requiredErrorText"></span>
                <div class="headerPanelSmall headerPanelSmall-splitter bold"><%= CRMTaskResource.TaskTitle%>:</div>
                <input class="textEdit" id="tbxTitle" style="width:100%" type="text" maxlength="100"/>
            </div>

            <div class="headerPanel-splitter">
                <div class="headerPanelSmall-splitter bold"><%= CRMTaskResource.TaskCategory%>:</div>
                <asp:PlaceHolder ID="phCategorySelector" runat="server"></asp:PlaceHolder>
            </div>

            <div class="headerPanel-splitter"<% if (HideContactSelector) { %>style="display:none;"<% } %>>
                <div class="headerPanelSmall-splitter bold">
                    <%= CRMTaskResource.ConnectWithAContact%>:
                </div>
                <asp:PlaceHolder ID="phContactSelector" runat="server"></asp:PlaceHolder>
            </div>

            <div class="headerPanel-splitter requiredField" id="taskDeadlineContainer">
                <span class="requiredErrorText"></span>
                <div class="headerPanelSmall headerPanelSmall-splitter bold"><%= CRMTaskResource.DueDate%>:</div>
                <a id="deadline_0" class="baseLinkAction linkMedium" onclick="ASC.CRM.TaskActionView.changeDeadline(this);">
                    <%= CRMTaskResource.Today%>
                </a>
                <span class="splitter">&nbsp;</span>
                <a id="deadline_3" class="baseLinkAction linkMedium" onclick="ASC.CRM.TaskActionView.changeDeadline(this);">
                    <%= CRMTaskResource.ThreeDays%>
                </a>
                <span class="splitter">&nbsp;</span>
                <a id="deadline_7" class="baseLinkAction linkMedium" onclick="ASC.CRM.TaskActionView.changeDeadline(this);">
                    <%= CRMTaskResource.Week%>
                </a>
                <span class="splitter">&nbsp;</span>

                <input id="taskDeadline" type="text" onkeypress="ASC.CRM.TaskActionView.keyPress(event);"
                                class="pm-ntextbox textEditCalendar" />

                <span class="splitter">&nbsp;</span>
                <span class="bold"><%= CRMTaskResource.Time%>:</span>
                <span class="splitter">&nbsp;</span>
                <select class="comboBox" id="taskDeadlineHours" style="width:45px;" onchange="ASC.CRM.TaskActionView.changeTime(this);">
                    <%=InitHoursSelect()%>
                </select>
                <b style="padding: 0 3px;">:</b>
                <select class="comboBox" id="taskDeadlineMinutes" style="width:45px;" onchange="ASC.CRM.TaskActionView.changeTime(this);">
                    <%=InitMinutesSelect()%>
                </select>
            </div>

            <div class="headerPanel-splitter requiredField">
                <span class="requiredErrorText"></span>
                <div class="headerPanelSmall headerPanelSmall-splitter bold">
                    <%= CRMTaskResource.TaskResponsible%>:
                </div>
                <div>
                    <div style="float:right;">
                        <input type="checkbox" id="notifyResponsible" style="float:left">
                        <label for="notifyResponsible" style="float:left;padding: 2px 0 0 4px;">
                            <%= CRMCommonResource.Notify%>
                        </label>
                    </div>
                    <aswc:AdvancedUserSelector runat="server" ID="taskResponsibleSelector"></aswc:AdvancedUserSelector>
                </div>
            </div>

            <div class="headerPanel-splitter">
                <div class="headerPanelSmall-splitter bold">
                    <%= CRMTaskResource.TaskDescription%>:
                </div>
                <textarea id="tbxDescribe" style="width:100%;resize:none;" rows="3" cols="70"></textarea>
            </div>

            <div class="h_line">&nbsp;</div>

            <div class="action_block">
                <a class="baseLinkButton">
                    <%= CRMJSResource.AddThisTask%>
                </a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                    <%= CRMTaskResource.Cancel%>
                </a>
            </div>

            <div class='ajax_info_block' style="display: none;">
                <span class="textMediumDescribe">
                    <%= CRMTaskResource.SavingTask%></span><br />
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>
        </body>
    </ascwc:container>
</div>