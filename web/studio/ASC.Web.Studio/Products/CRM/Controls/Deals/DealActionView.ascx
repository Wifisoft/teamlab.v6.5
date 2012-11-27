<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control EnableViewState="true" Language="C#" AutoEventWireup="true" CodeBehind="DealActionView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Deals.DealActionView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Core.Tenants" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div id="crm_dealMakerDialog">
    <% if (TargetDeal != null) %>
    <% { %>
     <a onclick="ASC.CRM.DealActionView.deleteDeal(<%= TargetDeal.ID %>);" class="crm_deleteLinkButton" style="margin-left: 14px;">
            <%= CRMDealResource.DeleteDeal %></a>
    <% } %>
    <div id="dealForm" style="margin-top:15px; width: 700px;">
        <div class="requiredField" style="padding-right:2px;margin-left:15px;">
            <span class="requiredErrorText"><%=CRMDealResource.EmptyDealName%></span>
            <div class="headerPanelSmall headerBaseSmall" style="margin-bottom: 5px;"><%=CRMDealResource.NameDeal%>:</div>
            <div>
                <input type="text" style="width: 100%" id="nameDeal" name="nameDeal" class="textEdit" maxlength="100" />
            </div>
        </div>

        <dl>
            <dt class="headerBaseSmall"><%=CRMDealResource.ClientDeal%>:</dt>
            <dd><asp:PlaceHolder ID="phDealClient" runat="server"></asp:PlaceHolder></dd>

            <dt class="headerBaseSmall" <%=(ShowMembersPanel) ? "" : "style='display:none;'"%> id="dealMembersHeader">
                <%=CRMDealResource.OtherMembersDeal%>:</dt>
            <dd <%=(ShowMembersPanel) ? "" : "style='display:none;'"%> id="dealMembersBody">
                <asp:PlaceHolder ID="phDealMembers" runat="server"></asp:PlaceHolder>
            </dd>

            <dt class="headerBaseSmall"><%=CRMDealResource.DescriptionDeal%>:</dt>
            <dd style="padding-right:2px;">
                <textarea name="descriptionDeal" id="descriptionDeal" style="width:100%;height:150px;resize:none;"
                    class="textEdit"></textarea>
            </dd>

            <dt class="headerBaseSmall" id="bidCurrencyHeader"><%=CRMDealResource.ExpectedValue%>:</dt>
            <dd>
                <select id="bidCurrency" name="bidCurrency" class="comboBox">
                    <% foreach (var keyValuePair in CurrencyProvider.GetAll())%>
                    <% { %>
                    <option value="<%=keyValuePair.Abbreviation%>" <%=IsSelectedBidCurrency(keyValuePair.Abbreviation) ? "selected=selected" : String.Empty%>>
                        <%=String.Format("{0} - {1}", keyValuePair.Abbreviation, keyValuePair.Title)%></option>
                    <% } %>
                </select>
                <span class="splitter"></span>
                <input type="text" style="width: 100px" name="bidValue" id="bidValue" class="textEdit" value="0" maxlength="15"/>
                <span class="splitter"></span>
                <select id="bidType" name="bidType" onchange="ASC.CRM.DealActionView.selectBidTypeEvent(this)" class="comboBox">
                    <option value="<%=BidType.FixedBid%>">
                        <%=CRMDealResource.BidType_FixedBid%>
                    </option>
                    <option value="<%=(int) BidType.PerHour%>">
                        <%=CRMDealResource.BidType_PerHour%></option>
                    <option value="<%=(int) BidType.PerDay%>">
                        <%=CRMDealResource.BidType_PerDay%></option>
                    <option value="<%=(int) BidType.PerWeek%>">
                        <%=CRMDealResource.BidType_PerWeek%></option>
                    <option value="<%=(int) BidType.PerMonth%>">
                        <%=CRMDealResource.BidType_PerMonth%></option>
                    <option value="<%=(int) BidType.PerYear%>">
                        <%=CRMDealResource.BidType_PerYear%></option>
                </select>

                <span class="splitter">&nbsp;</span>
                <input type="text" style="width: 50px; display:none;" id="perPeriodValue"
                    name="perPeriodValue" value="0" class="textEdit" />
                <span class="splitter">&nbsp;</span>
            </dd>

            <dt class="headerBaseSmall"><%=CRMJSResource.ExpectedCloseDate%>:</dt>
            <dd>
                <input type="text" id="expectedCloseDate" name="expectedCloseDate" class="textEdit textEditCalendar" />
            </dd>
        </dl>

        <div class="requiredField" style="margin-top:10px;margin-left:15px;">
            <span class="requiredErrorText"><%=CRMDealResource.EmptyDealResponsible%></span>
            <div class="headerPanelSmall headerBaseSmall" style="margin-bottom:5px;"><%=CRMDealResource.ResponsibleDeal%>:</div>
            <ascwc:AdvancedUserSelector runat="server" ID="userSelector">
            </ascwc:AdvancedUserSelector>
        </div>

        <dl>
            <dt class="headerBaseSmall"><%=CRMDealResource.CurrentDealMilestone%>:</dt>
            <dd>
                <select id="dealMilestone" name="dealMilestone" onchange="javascript: jq('#probability').val(dealMilestones[this.selectedIndex].probability);" class="comboBox">
                </select>
            </dd>
            <dt class="headerBaseSmall"><%=CRMDealResource.ProbabilityOfWinning%>:</dt>
            <dd>
                <input type="text" id="probability" name="probability" class="textEdit" style="width: 30px;"
                    maxlength="3" value="0" />&nbsp;(%)
            </dd>
        </dl>

        <% if (CRMSecurity.IsAdmin) %>
        <% {%>
        <div style="margin-left: 15px;margin-top: 10px;">
            <div class="bold" style="margin-bottom: 10px;"><%= CRMSettingResource.OtherFields %></div>
            <a onclick="ASC.CRM.DealActionView.gotoAddCustomFieldPage();" style="text-decoration: underline" class="linkMedium">
                <%= CRMSettingResource.SettingCustomFields %>
            </a>
        </div>
        <% }%>

        <div style="margin-top:15px;margin-left:15px; <% if (!HavePermission)
                                        {%>display: none; <% }%>">
            <asp:PlaceHolder ID="phPrivatePanel" runat="server"></asp:PlaceHolder>
        </div>

        <div style="display: none" id="autoCompleteBlock"></div>

        <div style="margin-top: 25px;" class="action_block">
            <asp:LinkButton runat="server" ID="saveDealButton" CommandName="SaveDeal" CommandArgument="0" OnClientClick="return ASC.CRM.DealActionView.submitForm();"
            OnCommand="SaveOrUpdateDeal" CssClass="baseLinkButton" />
             <span class="splitter">&nbsp;</span>
             <% if (TargetDeal == null)%>
             <% {  %>
             <asp:LinkButton runat="server" ID="saveAndCreateDealButton"  CommandName="SaveDeal" CommandArgument="1" OnClientClick="return ASC.CRM.DealActionView.submitForm();"
                OnCommand="SaveOrUpdateDeal" CssClass="grayLinkButton" />
             <span class="splitter">&nbsp;</span>
             <% } %>
            <asp:HyperLink runat="server" CssClass="grayLinkButton" ID="cancelButton"> <%= CRMCommonResource.Cancel%></asp:HyperLink>
        </div>

        <div style="display: none;margin-top: 25px;" class="ajax_info_block">
            <span class="textMediumDescribe"></span>
            <br />
            <img alt="" title="" src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
        </div>
    </div>
</div>

<script id="customFieldTemplate" type="text/x-jquery-tmpl">
{{if fieldType ==  0}}
    <input id="custom_field_${id}" name="customField_${id}" size="${mask.size}"
            type="text" class="textEdit" maxlength="255" value="${value}"/>
{{else fieldType ==  1}}
    <textarea rows="${mask.rows}" cols="${mask.cols}" name="customField_${id}"
            id="custom_field_${id}" maxlength="255">${value}</textarea>
{{else fieldType ==  2}}
    <select class="comboBox" name="customField_${id}" id="custom_field_${id}">
         <option value="">&nbsp;</option>
      {{each mask}}
         <option value="${$value}">${$value}</option>
      {{/each}}
    </select>
{{else fieldType ==  3}}
    {{if value == "true"}}
      <input id="custom_field_${id}" type="checkbox" style="vertical-align: middle;" checked="checked"/>
    {{else}}
      <input id="custom_field_${id}" type="checkbox" style="vertical-align: middle;"/>
    {{/if}}
{{else fieldType ==  4}}
    ${label}
{{else fieldType ==  5}}
    <input type="text" id="custom_field_${id}" name="customField_${id}" class="textEdit textEditCalendar" value="${value}"/>
{{/if}}
</script>

<script id="customFieldRowTemplate" type="text/x-jquery-tmpl">
{{if fieldType ==  3}}
    <dt class="headerBaseSmall">
         <label>
           {{tmpl "#customFieldTemplate"}}
            ${label}
         </label>
    </dt>
    <dd><input type="hidden" name="customField_${id}" /></dd>
{{else fieldType ==  4}}
    <dt class="headerBase headerExpand" onclick="javascript:jq(this).next('dd:first').nextUntil('#dealForm dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');" >
          {{tmpl "#customFieldTemplate"}}
    </dt>
    <dd class="underHeaderBase">&nbsp;</dd>
{{else}}
    <dt class="headerBaseSmall">${label}</dt>
    <dd>
        {{tmpl "#customFieldTemplate"}}
    </dd>
{{/if}}
</script>

<script type="text/javascript" language="javascript">
    jq(function() {

        ASC.CRM.DealActionView.init('<%= System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator %>',
            '<%= (int)System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0] %>',
            '<%= DateTimeExtension.DateMaskForJQuery %>',
            '<%= TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern) %>');


        dealClientSelector.SelectItemEvent = ASC.CRM.DealActionView.chooseMainContact;
        dealMemberSelector.SelectItemEvent = ASC.CRM.DealActionView.chooseMemberContact;
    });
</script>

<input type="hidden" id="responsibleID" name="responsibleID" />
<input type="hidden" id="selectedContactID" name="selectedContactID" />
<input type="hidden" id="selectedMembersID" name="selectedMembersID" />
<input type="hidden" id="selectedPrivateUsers" name="selectedPrivateUsers" value="" />
<input type="hidden" id="isPrivateDeal" name="isPrivateDeal" value="" />
<input type="hidden" id="notifyPrivateUsers" name="notifyPrivateUsers" value="" />