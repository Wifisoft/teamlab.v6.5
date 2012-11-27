<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskTemplateView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.TaskTemplateView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<div>
    <a class="linkAddMediumText baseLinkAction" onclick="ASC.CRM.TaskTemplateView.showTemplateConatainerPanel();" >
        <%= CRMSettingResource.AddNewTaskTemplateContainer%>
    </a>
</div>

<div id="templateConatainerPanel" style="display: none">
    <ascwc:Container ID="_templateConatainerPanel" runat="server">
        <Header>
        </Header>
        <Body>
            <div class="requiredField">
                <span class="requiredErrorText"><%= CRMSettingResource.EmptyLabelError %></span>
                <div class="headerPanelSmall headerBaseSmall" style="margin-bottom:5px;">
                    <%= CRMSettingResource.TitleItem %>:
                </div>
                <input id="templateConatainerTitle" type="text" class="textEdit" style="width:100%" maxlength="255"/>
            </div>
            
            <div class="h_line"><!--– –--></div>
            
            <div class="action_block">
                <a class="baseLinkButton"></a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="jq.unblockUI();">
                     <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe"> <%= CRMSettingResource.AddTaskTemplateContainerInProgressing%> </span>
                <br />
                <img alt="<%= CRMSettingResource.AddTaskTemplateContainerInProgressing %>"
                    title="<%= CRMSettingResource.AddTaskTemplateContainerInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </ascwc:Container>
</div>

<div id="templatePanel" style="display: none;">
    <ascwc:Container id="_templatePanel" runat="server">
        <header>
        </header>
        <body>
            <div class="templateHeader-splitter requiredField">
                <span class="requiredErrorText"><%= CRMJSResource.EmptyTaskTitle%></span>
                <div class="headerPanelSmall templateHeaderSmall-splitter bold"><%= CRMTaskResource.TaskTitle%>:</div>
                <input class="textEdit" id="tbxTemplateTitle" style="width:100%" type="text" maxlength="100"/>
            </div>

            <div class="templateHeader-splitter">
                <div class="templateHeaderSmall-splitter bold"><%= CRMTaskResource.TaskCategory%>:</div>
                <asp:PlaceHolder ID="phCategorySelector" runat="server"></asp:PlaceHolder>
            </div>

            <div class="templateHeader-splitter requiredField" id="taskDeadlineContainer">
                <div class="templateHeaderSmall-splitter bold"><%= CRMTaskResource.DueDate%>:</div>
                <div class="templateHeaderSmall-splitter">
                    <input type="radio" id="deadLine_fixed" name="duedate_fixed" value="true"/>
                    <label for="deadLine_fixed"><%= CRMSettingResource.OffsetFromTheStartOfContainer%></label>
                    <br/>
                    <input type="radio" id="deadLine_not_fixed" name="duedate_fixed" value="false" />
                    <label for="deadLine_not_fixed"><%= CRMSettingResource.OffsetFromThePreviousTask%></label>
                </div>
                <%= CRMSettingResource.DisplacementInDays%>:
                <input type="text" class="textEdit" id="tbxTemplateDisplacement" style="width:50px" maxlength="5"/>
                <span class="splitter"></span>
                <%= CRMTaskResource.Time%>:
                <span class="splitter"></span>
                <select class="comboBox" id="templateDeadlineHours" style="width:45px;">
                    <%=InitHoursSelect()%>
                </select>
                <b style="padding: 0 3px;">:</b>
                <select class="comboBox" id="templateDeadlineMinutes" style="width:45px;">
                    <%=InitMinutesSelect()%>
                </select>
            </div>

            <div class="templateHeader-splitter requiredField">
                <span class="requiredErrorText"><%= CRMJSResource.EmptyTaskResponsible%></span>
                <div class="headerPanelSmall templateHeaderSmall-splitter bold">
                    <%= CRMTaskResource.TaskResponsible%>:
                </div>
                <div>
                    <div style="float:right;">
                        <input type="checkbox" id="notifyResponsible" style="float:left">
                        <label for="notifyResponsible" style="float:left;padding: 2px 0 0 4px;">
                            <%= CRMCommonResource.Notify%>
                        </label>
                    </div>
                    <ascwc:AdvancedUserSelector runat="server" ID="taskTemplateResponsibleSelector"></ascwc:AdvancedUserSelector>
                </div>
            </div>

            <div class="templateHeader-splitter">
                <div class="templateHeaderSmall-splitter bold">
                    <%= CRMTaskResource.TaskDescription%>:
                </div>
                <textarea id="tbxTemplateDescribe" style="width:100%;resize:none;" rows="3" cols="70"></textarea>
            </div>

            <div class="h_line"><!--– –--></div>

            <div class="action_block">
                <a class="baseLinkButton"></a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="jq.unblockUI();">
                    <%= CRMTaskResource.Cancel%>
                </a>
            </div>

            <div class='ajax_info_block' style="display: none;">
                <span class="textMediumDescribe"> <%= CRMSettingResource.AddTaskTemplateInProgressing%> </span>
                <br />
                <img alt="<%= CRMSettingResource.AddTaskTemplateInProgressing %>"
                    title="<%= CRMSettingResource.AddTaskTemplateInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </body>
    </ascwc:Container>
</div>

<ascwc:ViewSwitcher runat="server" ID="_switcherEntityType">
    <SortItems>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_switcherItemAllContacts"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_switcherItemPersons"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_switcherItemCompanies"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_switcherItemDeals"></ascwc:ViewSwitcherLinkItem>
        <ascwc:ViewSwitcherLinkItem runat="server" id="_switcherItemrCases"></ascwc:ViewSwitcherLinkItem>
    </SortItems>
</ascwc:ViewSwitcher>
<br />


<ul id="templateConatainerContent" class="clearFix">
</ul>

<div id="emptyContent" style="display: none">
    <asp:PlaceHolder ID="_phEmptyContent" runat="server"></asp:PlaceHolder>
</div>

<script id="templateContainerRow" type="text/x-jquery-tmpl">
<li id="templateContainerHeader_${id}">
    <table class="templateContainer_row" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="borderBase">
                    <span onclick="ASC.CRM.TaskTemplateView.toggleCollapceExpand(this)" class="headerBase headerExpand">
                        ${title}
                    </span>
                </td>
                <td class="borderBase" style="width:70px;text-align: right;padding-right: 10px;">
                    <img class="addImg" align="absmiddle"
                         title="<%= CRMSettingResource.AddNewTaskTemplate %>"
                         alt="<%= CRMSettingResource.AddNewTaskTemplate %>"
                         src="<%= WebImageSupplier.GetAbsoluteWebPath("create.png", ProductEntryPoint.ID) %>"
                         onclick="ASC.CRM.TaskTemplateView.showTemplatePanel(${id})" />    
                    <img class="editImg" align="absmiddle" 
                         title="<%= CRMSettingResource.EditTaskTemplateContainer %>"
                         alt="<%= CRMSettingResource.EditTaskTemplateContainer %>"
                         src="<%= WebImageSupplier.GetAbsoluteWebPath("pencil.png", ProductEntryPoint.ID) %>"
                         onclick="ASC.CRM.TaskTemplateView.showTemplateConatainerPanel(${id})" />
                    <img class="deleteImg" align="absmiddle"
                        title="<%= CRMSettingResource.DeleteTaskTemplateContainer %>"
                        alt="<%= CRMSettingResource.DeleteTaskTemplateContainer %>"
                        onclick="ASC.CRM.TaskTemplateView.deleteTemplateConatainer(${id})"
                        src="<%= WebImageSupplier.GetAbsoluteWebPath("trash.png", ProductEntryPoint.ID) %>"/>
                    <img class="loaderImg" align="absmiddle" style="display: none;"
                        src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID) %>"/>
                </td>
            </tr>
        </tbody>
    </table>
</li>
<li id="templateContainerBody_${id}" style="{{if typeof(items)=="undefined"}}display:none;{{/if}}">
    {{tmpl(items) "#templateRow"}}
</li>
</script>

<script id="templateRow" type="text/x-jquery-tmpl">
    <table cellspacing="0" cellpadding="0"  id="templateRow_${id}" class="templateContainer_row" style="margin-bottom: -1px;">
        <tbody>
            <tr>
                <td class="borderBase" style="width:30px;">
                    <img title="${category.title}" alt="${category.title}" src="${category.imagePath}" />
                </td>
                <td class="borderBase">
                    <div class="divForTemplateTitle">
                        <span id="templateTitle_${id}" class="templateTitle" title="${description}">${title}</span>
                    </div>
                    <div style="padding-top: 5px; display: inline-block;">
                        ${ASC.CRM.TaskTemplateView.getDeadlineDisplacement(offsetTicks, deadLineIsFixed)}
                    </div>
                </td>
                <td class="borderBase" style="width:200px;">
                    <span class="userLink">${responsible.displayName}</span>
                </td>
                <td class="borderBase" style="width:70px;text-align: right;padding-right: 10px;">
                    <img class="editImg" align="absmiddle"
                         title="<%= CRMSettingResource.EditTaskTemplate %>" alt="<%= CRMSettingResource.EditTaskTemplate %>"
                         src="<%= WebImageSupplier.GetAbsoluteWebPath("pencil.png", ProductEntryPoint.ID) %>"
                         onclick="ASC.CRM.TaskTemplateView.showTemplatePanel(${containerID}, ${id})" />
                    <img class="deleteImg" align="absmiddle"
                         title="<%= CRMSettingResource.DeleteTaskTemplate %>" alt="<%= CRMSettingResource.DeleteTaskTemplate %>"
                         onclick="ASC.CRM.TaskTemplateView.deleteTemplate(${id})"
                         src="<%= WebImageSupplier.GetAbsoluteWebPath("trash.png", ProductEntryPoint.ID) %>"/>
                    <img class="loaderImg" align="absmiddle" style="display: none;"
                         src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID) %>"/>
                </td>
            </tr>
        </tbody>
    </table>
</script>

<script type="text/javascript" language="javascript">
    jq(function() {
        ASC.CRM.TaskTemplateView.init();
    	
    	ASC.CRM.TaskTemplateView.ConatainerPanel_AddHeaderText = "<%= CRMSettingResource.AddNewTaskTemplateContainer%>";
    	ASC.CRM.TaskTemplateView.ConatainerPanel_AddButtonText = "<%= CRMSettingResource.AddThisTaskTemplateContainer%>";
    	ASC.CRM.TaskTemplateView.ConatainerPanel_EditHeaderText = "<%= CRMSettingResource.EditSelectedTaskTemplateContainer%>";
    	
    	ASC.CRM.TaskTemplateView.TemplatePanel_AddHeaderText = "<%= CRMSettingResource.AddNewTaskTemplate%>";
    	ASC.CRM.TaskTemplateView.TemplatePanel_AddButtonText = "<%= CRMSettingResource.AddThisTaskTemplate%>";
    	ASC.CRM.TaskTemplateView.TemplatePanel_EditHeaderText = "<%= CRMSettingResource.EditSelectedTaskTemplate%>";
    });
</script>