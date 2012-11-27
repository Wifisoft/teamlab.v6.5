<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DealDetailsView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Deals.DealDetailsView" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users"
    TagPrefix="ascws" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register TagPrefix="ascws" Namespace="ASC.Web.Studio.Controls.Common" %>

<% if (Global.DebugVersion) { %>
<link href="<%= PathProvider.GetFileStaticRelativePath("tasks.css") %>" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("tasks.js") %>"></script>
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("contacts.js") %>"></script>
<% } %>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {

        ASC.CRM.DealDetailsView.init();


        //----Attachments------//
        ASC.Controls.AnchorController.bind(/files/, Attachments.loadFiles);

        Attachments.bind("addFile", function(ev, file) {
            //ASC.CRM.Common.changeCountInTab("add", "files");
            var dealID = jq.getURLParam("id") * 1;
            var type = "opportunity";
            var fileids = [];
            fileids.push(file.id);

            Teamlab.addCrmEntityFiles({}, dealID, type, {
                entityid: dealID,
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

        if (dealContactSelector.SelectedContacts.length == 0) {
            jq("#emptyDealParticipantPanel").show();
        }
        else jq("#addDealParticipantButton").show();

        dealContactSelector.SelectItemEvent = ASC.CRM.DealDetailsView.addMemberToDeal;
        ASC.CRM.ListContactView.removeMember = ASC.CRM.DealDetailsView.removeMemberFromDeal;

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

<script id="customFieldTmpl" type="text/x-jquery-tmpl">
    {{if fieldType ==  0 || fieldType ==  2 || fieldType ==  5}}
        <dt class="describeText" title="${label}">${label}:</dt>
        <dd class="clearFix">
            ${value}
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
        <dt class="headerBase headerExpand" style="float: none;" onclick="javascript:jq(this).next('dd:first').nextUntil('#dealInfo dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
            ${label}
        </dt>
        <dd class="underHeaderBase clearFix">&nbsp;</dd>
    {{/if}}
</script>

<a title="<%= CRMDealResource.EditThisDealButton%>" class="linkEditButton" style="margin-bottom:20px;"
    href="<%= String.Format("deals.aspx?action=manage&id={0}", TargetDeal.ID) %>" >
    <%= CRMDealResource.EditThisDealButton%>
</a>

<div id="dealMainInfo" class="clearFix">
    <div>
        <span class="describeText">
            <%= CRMDealResource.CurrentDealMilestone%>:</span>
        <p class="headerBaseMedium">
            <span id="dealMilestoneSwitcher"><span class="baseLinkAction">
                <%= Global.DaoFactory.GetDealMilestoneDao().GetByID(TargetDeal.DealMilestoneID).Title.HtmlEncode() %></span><small>▼</small></span>
        </p>
    </div>
    <div>
        <span class="describeText">
            <%= CRMDealResource.ProbabilityOfWinning %>:</span>
        <p class="headerBaseMedium" id="dealMilestoneProbability">
            <%= TargetDeal.DealMilestoneProbability%>%</p>
    </div>
    <div>
        <span class="describeText">
            <%= CRMDealResource.ExpectedValue %>: </span>
        <p class="headerBaseMedium">
            <%=GetExpectedValueStr()%></p>
    </div>
    <div id="closeDealDate">
        <span class="describeText">
            <%= TargetDeal.ActualCloseDate == DateTime.MinValue ? CRMJSResource.ExpectedCloseDate : CRMJSResource.ActualCloseDate%>:</span>
        <p class="headerBaseMedium">
            <%= GetExpectedOrActualCloseDateStr()%></p>
    </div>
</div>
<br />
<dl id="dealInfo" class="clearFix">
    <% if (TargetDeal.ResponsibleID != Guid.Empty) %>
    <%{%>
    <dt class="describeText" style="margin-top: 0px;">
        <%=CRMDealResource.ResponsibleDeal%>: </dt>
    <dd class="clearFix" style="margin-top: 0px;">
        <%=ASC.Core.CoreContext.UserManager.GetUsers(TargetDeal.ResponsibleID).DisplayUserName()%>
    </dd>
    <%}%>
    <% if (!String.IsNullOrEmpty(TargetDeal.Description)) %>
    <%{%>
    <dt class="describeText">
        <%= CRMDealResource.DescriptionDeal %>: </dt>
    <dd class="clearFix">
        <%= TargetDeal.Description.HtmlEncode().Replace("\n", "<br/>")%>
    </dd>
    <%}%>
    <dt class="describeText">
        <%= CRMCommonResource.Tags %>: </dt>
    <dd>
        <asp:PlaceHolder ID="phTagContainer" runat="server">
        </asp:PlaceHolder>
    </dd>
</dl>
<br />


<div id="dealMilestoneDropDown" class="dropDownDialog">
    <div class="dropDownCornerLeft"></div>
    <div class="dropDownContent">
    <asp:Repeater ID="AllDealMilestonesRepeater" runat="server">
        <ItemTemplate>
            <a class="dropDownItem"
                    onclick="ASC.CRM.DealDetailsView.changeDealMilestone('<%= TargetDeal.ID %>',
                            '<%# DataBinder.Eval(Container.DataItem, "id") %>',
                            '<%# ((String)DataBinder.Eval(Container.DataItem, "title")).HtmlEncode() %>');">
                    <%#  ((String)DataBinder.Eval(Container.DataItem, "title")).HtmlEncode()%></a>
        </ItemTemplate>
    </asp:Repeater>
    </div>
</div>


<ascwc:ViewSwitcher runat="server" ID="_tabsContainer" RenderAllTabs="true">
    <TabItems>
        <ascwc:ViewSwitcherTabItem runat="server" ID="EventsTab">
            <asp:PlaceHolder runat="server" ID="_phHistoryView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="TasksTab">
            <asp:PlaceHolder runat="server" ID="_phTasksView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="ContactsTab">
            <div id="addDealParticipantButton">
                <a class="linkAddMediumText baseLinkAction" onclick="javascript:jq('#addDealParticipantButton').hide();jq('#dealParticipantPanel').show();" >
                    <%= CRMCommonResource.AddParticipant %>
                </a>
            </div>

            <div id="dealParticipantPanel">
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