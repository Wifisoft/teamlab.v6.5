<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CasesActionView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Cases.CasesActionView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div id="crm_caseMakerDialog">

    <% if (TargetCase != null) %>
    <% { %>
     <a onclick="javascript:deleteCase(<%= TargetCase.ID %>)" class="crm_deleteLinkButton" style="margin-left: 14px;">
            <%= CRMCasesResource.DeleteThisCase %></a>
    <% } %>

    <div style="margin:15px 0 0 15px; width: 700px;">
        <input type="hidden" class="contact_ID" ID="CaseID" runat="server"/>

        <div class="requiredField" style="padding: 0 2px 0 0;">
            <span class="requiredErrorText"></span>
            <div class="bold headerPanelSmall"><%= CRMCasesResource.CaseTitle %>:</div>
            <input type="text" class="textEdit" maxlength="100" style="width:100%" id="caseTitle" name="caseTitle" value="<%= GetCaseTitle() %>"/>
        </div>

        <dl class="clearFix">
            <dt class="headerBaseSmall"><%= CRMCasesResource.MembersCase%>:</dt>
            <dd id="membersCasesSelectorsContainer">
                <asp:PlaceHolder ID="phContactSelector" runat="server"></asp:PlaceHolder>
                <input type="hidden" name="memberID" value="" />
            </dd>

            <script id="casesEditCustomFieldTmpl" type="text/x-jquery-tmpl">
            {{if fieldType ==  0}}
                <dt class="bold">${label}</dt>
                <dd>
                    <input id="custom_field_${id}" name="customField_${id}" size="${mask.size}"
                            type="text" class="textEdit" maxlength="255" value="${value}" />
                </dd>
            {{else fieldType ==  1}}
                <dt class="bold">${label}</dt>
                <dd>
                    <textarea rows="${mask.rows}" cols="${mask.cols}" id="custom_field_${id}"
                            name="customField_${id}" maxlength="255">${value}</textarea>
                </dd>
            {{else fieldType ==  2}}
                <dt class="bold">${label}</dt>
                <dd>
                    <select id="custom_field_${id}" name="customField_${id}" class="comboBox">
                         <option value="">&nbsp;</option>
                      {{each mask}}
                         <option value="${$value}">${$value}</option>
                      {{/each}}
                    </select>
                </dd>
            {{else fieldType ==  3}}
               <dt class="bold">
                   <label>
                        {{if value == "true"}}
                            <input id="custom_field_${id}" type="checkbox" style="vertical-align: middle;" checked="checked"/>
                        {{else}}
                            <input id="custom_field_${id}" type="checkbox" style="vertical-align: middle;"/>
                        {{/if}}
                        ${label}
                   </label>
               </dt>
               <dd><input type="hidden" name="customField_${id}" /></dd>
            {{else fieldType ==  4}}
                <dt class="headerBase headerExpand" onclick="javascript:jq(this).next('dd:first').nextUntil('#crm_caseMakerDialog dl dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
                   ${label}
                </dt>
                <dd class="underHeaderBase">&nbsp;</dd>
            {{else fieldType ==  5}}
               <dt class="bold">${label}</dt>
               <dd>
                  <input type="text" id="custom_field_${id}" name="customField_${id}" class="textEdit textEditCalendar" value="${value}"/>
               </dd>
            {{/if}}
            </script>
        </dl>

        <% if (CRMSecurity.IsAdmin) %>
        <% {%>
        <div style="margin-top: 10px;">
            <div class="bold" style="margin-bottom: 10px;"><%= CRMSettingResource.OtherFields %></div>
            <a onclick="ASC.CRM.CasesActionView.gotoAddCustomFieldPage();" style="text-decoration: underline" class="linkMedium">
                <%= CRMSettingResource.SettingCustomFields %>
            </a>
        </div>
        <% }%>

        <div style="margin:15px 0 0;<%if(!HavePermission){%>display:none;<%}%>">
            <div id="privatePanel">
                <asp:PlaceHolder ID="phPrivatePanel" runat="server"></asp:PlaceHolder>
            </div>
        </div>

        <input type="hidden" name="isPrivateCase" id="isPrivateCase"/>
        <input type="hidden" name="notifyPrivateUsers" id="notifyPrivateUsers"/>
        <input type="hidden" name="selectedUsersCase" id="selectedUsersCase"/>
    </div>

    <div style="margin-top: 25px;" class="action_block clearFix">
        <asp:LinkButton runat="server" ID="saveCaseButton" CommandName="SaveCase" CommandArgument="0" OnClientClick="return ASC.CRM.CasesActionView.submitForm();"
            OnCommand="SaveOrUpdateCase" CssClass="save_button baseLinkButton"/>

        <span class="splitter"></span>

        <% if (TargetCase == null) %>
        <% { %>
        <asp:LinkButton runat="server" ID="saveAndCreateCaseButton"  CommandName="SaveCase" CommandArgument="1" OnClientClick="return ASC.CRM.CasesActionView.submitForm();"
            OnCommand="SaveOrUpdateCase" CssClass="grayLinkButton" />
        <span class="splitter">&nbsp;</span>
        <% } %>

        <asp:HyperLink runat="server" CssClass="grayLinkButton" ID="cancelButton"> <%= CRMCommonResource.Cancel%></asp:HyperLink>
    </div>

    <div style="display: none;margin-top: 25px;" class="ajax_info_block clearFix">
        <span class="textMediumDescribe">
            <%= TargetCase == null ? CRMCasesResource.AddingCaseProggress : CRMCommonResource.SaveChangesProggress%>
        </span>
        <br />
        <img alt="<%= TargetCase == null ? CRMCasesResource.AddingCaseProggress : CRMCommonResource.SaveChangesProggress%>"
            title="<%= TargetCase == null ? CRMCasesResource.AddingCaseProggress : CRMCommonResource.SaveChangesProggress %>"
                src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
    </div>
</div>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {
        ASC.CRM.CasesActionView.init('<%= DateTimeExtension.DateMaskForJQuery %>');
    });

    function deleteCase(id)
    {
        if (confirm(CRMJSResources.ConfirmDeleteCase + "\n" + CRMJSResources.DeleteConfirmNote))
        {
            location.replace(jq.format("cases.aspx?id={0}&action=delete",id));
            return;
        }
        else
            return;
    }
</script>