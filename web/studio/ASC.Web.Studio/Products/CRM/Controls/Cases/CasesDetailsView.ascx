<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CasesDetailsView.ascx.cs"
        Inherits="ASC.Web.CRM.Controls.Cases.CasesDetailsView" %>
<%@ Register TagPrefix="ascws" Namespace="ASC.Web.Controls" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<% if (Global.DebugVersion) { %>
<link href="<%= PathProvider.GetFileStaticRelativePath("tasks.css") %>" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("tasks.js") %>"></script>
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("contacts.js") %>"></script>
<% } %>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {

        ASC.CRM.CasesDetailsView.init();

        //----Attachments------//
        ASC.Controls.AnchorController.bind(/files/, Attachments.loadFiles);

        Attachments.bind("addFile", function(ev, file) {
            //ASC.CRM.Common.changeCountInTab("add", "files");
            var caseID = jq.getURLParam("id") * 1;
            var type = "case";
            var fileids = [];
            fileids.push(file.id);

            Teamlab.addCrmEntityFiles({}, caseID, type, {
                                                            entityid: caseID,
                                                            entityType: type,
                                                            fileids: fileids
                                                        },
            {
                success: ASC.CRM.HistoryView.addEventToHistoryLayout
            });
        });

        Attachments.bind("deleteFile", function(ev, fileId) {
            var $fileLinkInHistoryView = jq("#fileContent_" + fileId);
            if ($fileLinkInHistoryView.length != 0) {
                var messageID = $fileLinkInHistoryView.parents("div[id^=eventAttach_]").attr("id").split("_")[1];
                ASC.CRM.HistoryView.deleteFile(fileId, messageID);
            }
            else {
                Teamlab.removeCrmEntityFiles({ fileId: fileId }, fileId, {
                    success: function (params) {
                        Attachments.deleteFileFromLayout(params.fileId);
                        //ASC.CRM.Common.changeCountInTab("delete", "files");
                    }
                });
            }
        });
        //-----end Attachments


        if (casesContactSelector.SelectedContacts.length == 0) {
            jq("#emptyCaseParticipantPanel").show();
        }
        else jq("#addCaseParticipantButton").show();

        casesContactSelector.SelectItemEvent = ASC.CRM.CasesDetailsView.addMemberToCase;
        ASC.CRM.ListContactView.removeMember = ASC.CRM.CasesDetailsView.removeMemberFromCase;

        ASC.Controls.AnchorController.bind(/history/, ASC.CRM.HistoryView.activate);
        ASC.Controls.AnchorController.bind(null, ASC.CRM.HistoryView.activate);
        ASC.Controls.AnchorController.bind(/tasks/, ASC.CRM.ListTaskView.activate);

        ASC.CRM.ListContactView.isContentRendered = false;
        ASC.Controls.AnchorController.bind(/contacts/, function() {
            if (ASC.CRM.ListContactView.isContentRendered == false) {
                ASC.CRM.ListContactView.isContentRendered = true;
                ASC.CRM.ListContactView.renderSimpleContent();
            }
        });

    });
</script>

<div>
    <a href="<%= String.Format("cases.aspx?id={0}&action=manage", TargetCase.ID) %>"
            class="linkEditButton" style="margin-bottom: 10px; float: left;">
        <%= CRMCasesResource.EditCase %>
    </a>

    <a href="javascript:void(0);" id="openCase" onclick="ASC.CRM.CasesDetailsView.changeCaseStatus(0)"
    <%= TargetCase.IsClosed ? "" : "style='display:none'"%> >
        <%= CRMCasesResource.OpenCase %>
    </a>

    <a href="javascript:void(0);" id="closeCase" onclick="ASC.CRM.CasesDetailsView.changeCaseStatus(1)"
    <%= TargetCase.IsClosed ? "style='display:none'" : "" %> >
        <%= CRMCasesResource.CloseCase %>
    </a>
</div>

<script id="casesCustomFieldsListTmpl" type="text/x-jquery-tmpl">
    {{if fieldType ==  0 || fieldType ==  2 || fieldType ==  5}}
        <dt class="describeText" title="${label}">${label}:</dt>
        <dd class="clearFix">
            <div id="custom_field_${id}">
                ${value}
            </div>
        </dd>
    {{else fieldType ==  1}}
        <dt class="describeText" title="${label}">${label}:</dt>
        <dd class="clearFix">
            {{html Encoder.htmlEncode(value).replace(/&#10;/g, "<br/>") }}
        </dd>
    {{else fieldType ==  3}}
       <dt class="describeText" title="${label}">${label}:</dt>
       <dd class="clearFix">
           {{if value == "true"}}
              <input id="custom_field_${id}" type="checkbox" style="vertical-align: middle;" checked="checked" disabled="disabled"/>
           {{else}}
              <input id="custom_field_${id}" type="checkbox" style="vertical-align: middle;" disabled="disabled"/>
           {{/if}}
       </dd>

    {{else fieldType ==  4}}
        <dt class="headerBase headerExpand" style="float:none;" onclick="javascript:jq(this).next('dd:first').nextUntil('#casesCustomFieldsList dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
             ${label}
        </dt>
        <dd class="underHeaderBase clearFix">&nbsp;</dd>
    {{/if}}
</script>


<dl id="casesCustomFieldsList" class="clearFix">
    <dt class="describeText" style="margin-top: 0px;"><%= CRMCasesResource.CaseStatus%>:</dt>
    <dd class="clearFix" style="margin-top: 0px;">
        <span id="caseStatusSwitcher">
            <%= TargetCase.IsClosed ? CRMCasesResource.CaseStatusClosed : CRMCasesResource.CaseStatusOpened %>
        </span>
    </dd>

    <dt class="describeText"><%= CRMCommonResource.Tags %>:</dt>
    <dd>
        <asp:PlaceHolder ID="phTagContainer" runat="server"></asp:PlaceHolder>
    </dd>
</dl>
<br />

<ascwc:ViewSwitcher ID="CasesTabs" runat="server" RenderAllTabs="true" DisableJavascriptSwitch="true">
    <TabItems>
        <ascwc:ViewSwitcherTabItem runat="server" ID="HistoryTab">
               <asp:PlaceHolder runat="server" ID="_phHistoryView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="TasksTab">
                <asp:PlaceHolder runat="server" ID="_phTasksView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="ContactsTab">
            <div id="addCaseParticipantButton">
                <a class="linkAddMediumText baseLinkAction" onclick="javascript:jq('#addCaseParticipantButton').hide();jq('#caseParticipantPanel').show();" >
                    <%= CRMCommonResource.AddParticipant %>
                </a>
            </div>
            <div id="caseParticipantPanel">
                <div class="bold" style="margin-bottom:5px;"><%= CRMContactResource.AssignContactFromExisting%>:</div>
                <asp:PlaceHolder ID="phContactSelector" runat="server"></asp:PlaceHolder>
            </div>

            <asp:PlaceHolder runat="server" ID="_phContactsView"></asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="_phEmptyPeopleView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="FilesTab">
            <asp:PlaceHolder id="_phFilesView" runat="server"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
    </TabItems>
</ascwc:ViewSwitcher>