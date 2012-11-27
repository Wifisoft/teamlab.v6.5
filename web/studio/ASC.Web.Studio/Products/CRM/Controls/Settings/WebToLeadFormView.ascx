<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebToLeadFormView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Settings.WebToLeadFormView" %>
<%@ Import Namespace="ASC.Security.Cryptography" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="aswc" %>
<br/>
<div class="tintMedium panelSplitter">
    <table cellpadding="10" cellspacing="0">
        <tr>
            <td>
                <img src="<%= WebImageSupplier.GetAbsoluteWebPath("web_to_leads.png", ProductEntryPoint.ID)%>" />
            </td>
            <td>
                <%= CRMSettingResource.WebToLeadsFormHeader%>
            </td>
        </tr>
    </table>
</div>
<div>
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td class="digits">
                1
            </td>
            <td style="padding-top: 10px;">
                <span class="headerBase">
                    <%= CRMSettingResource.FormProperties %>
                </span>
                <div class="textBigDescribe">
                    <%= CRMSettingResource.FormPropertiesDescription %>
                </div>
            </td>
        </tr>
    </table>
</div>
<div>
    <div id="properties_url_panel" class="requiredField">
        <span class="requiredErrorText">
            <%= CRMSettingResource.EmptyField %>
        </span>
        <div>
            <b><%= CRMSettingResource.ReturnURL %>:</b>
            <span class="crm-requiredField">*</span>
        </div>
        <div style="margin: 5px 0">
            <input type="text" maxlength="255" value="" style="width: 100%;" id="returnURL" name="returnURL" class="textEdit">
        </div>
        <div class="textBigDescribe">
            <%= CRMSettingResource.ReturnURLDescription %>
        </div>
    </div>
    <br />
    <div id="properties_webFormKey" class="requiredField">
        <span class="requiredErrorText">
            <%= CRMSettingResource.EmptyField %></span>
        <div>
            <b><%= CRMSettingResource.WebFormKey%>:</b>
            <span class="crm-requiredField">*</span>
        </div>
        <div style="margin: 5px 0">
            <input type="hidden" value="<%= _webFormKey %>" />
            <div class="clearFix">
                <div id="webFormKeyContainer" style="float: left;width: 240px;"><%= _webFormKey %></div>
                <img style="cursor: pointer;margin-left: 5px;"
                 title="<%= CRMCommonResource.Change %>" alt="<%= CRMCommonResource.Change %>"
                 onclick="ASC.CRM.SettingsPage.WebToLeadFormView.changeWebFormKey()"
                 src="<%= WebImageSupplier.GetAbsoluteWebPath("refresh.png", ProductEntryPoint.ID)%>"/>
            </div>
        </div>
        <div class="textBigDescribe">
            <%= CRMSettingResource.WebFormKeyDescription %>
        </div>
    </div>
</div>
<div>
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td class="digits">
                2
            </td>
            <td style="padding-top: 10px;">
                <span class="headerBase">
                    <%= CRMSettingResource.FieldsSelection %>
                </span>
                <div class="textBigDescribe">
                    <%= CRMSettingResource.FieldsSelectionDescription %>
                </div>
                <div style="margin-top: 10px;">
                    <input type="radio" id="padioCompany" value="company" name="radio" style="margin-left: 0px;"
                        onchange="ASC.CRM.SettingsPage.WebToLeadFormView.changeContactType()"/>
                    <label for="padioCompany"><%= CRMContactResource.Company %></label>
                    <input type="radio" id="radioPerson" value="person" name="radio" checked="checked"
                        onchange="ASC.CRM.SettingsPage.WebToLeadFormView.changeContactType()"/>
                    <label for="radioPerson"><%= CRMContactResource.Person %></label>
                </div>
            </td>
        </tr>
    </table>
</div>
<div style="padding: 10px 0">
    <div style="padding-bottom: 5px">
        <b> <%= CRMSettingResource.FieldList%>: </b>
    </div>
    <table width="100%" id="tblFieldList">
        <tbody>
        </tbody>
    </table>
</div>
<div class="panelSplitter">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td class="digits">
                3
            </td>
            <td style="padding-top: 10px;">
                <span class="headerBase">
                    <%= CRMSettingResource.AccessRightsAndTags%>
                </span>
                <div class="textBigDescribe">
                    <%= CRMSettingResource.AccessRightsAndTagsDescription%>
                </div>
            </td>
        </tr>
    </table>
</div>
<div class="panelSplitter">
    <asp:PlaceHolder runat="server" ID="_phPrivatePanel"></asp:PlaceHolder>
    <div style="margin: 10px 0; font-weight: bold"><%=CRMCommonResource.Tags%>:</div>
    <asp:PlaceHolder runat="server" ID="_phTagPanel"></asp:PlaceHolder>
</div>

<div class="panelSplitter">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td class="digits">
                4
            </td>
            <td style="padding-top: 10px;">
                <span class="headerBase">
                    <%=HttpUtility.HtmlEncode(CustomNamingPeople.Substitute<CRMSettingResource>("NotifyUsers"))%>
                </span>
                <div class="textBigDescribe">
                    <%= HttpUtility.HtmlEncode(CustomNamingPeople.Substitute<CRMSettingResource>("NotifyUsersDescription"))%>
                </div>
            </td>
        </tr>
    </table>
</div>
<div class="panelSplitter">
    <asp:PlaceHolder runat="server" ID="_phUserSelectorListView"></asp:PlaceHolder>
</div>

<div class="h_line">
    <!--– –-->
</div>
<div class="panelSplitter">
    <a class="baseLinkButton" onclick="javascript: ASC.CRM.SettingsPage.WebToLeadFormView.generateSampleForm();">
        <%= CRMSettingResource.GenerateForm %>
    </a>
</div>
<div id="resultContainer" class="panelSplitter" style="display: none;">
    <div class="panelSplitter">
        <div class="taskRow headerBase">
            <%= CRMSettingResource.FormCode %>
        </div>
        <div class="textBigDescribe">
            <%= CRMSettingResource.FormCodeDescription%>
        </div>
    </div>
    <textarea onclick="this.select()" style="width: 100%; resize: none;" rows="10"></textarea>
</div>
<div class="panelSplitter" id="previewHeader" style="display: none;">
    <div class="taskRow headerBase">
        <%= CRMSettingResource.FormPreview%>
    </div>
    <div class="textBigDescribe">
        <%= CRMSettingResource.FormPreviewDescription%>
    </div>
    <br />
    <div class="content">
    </div>
</div>

<script id="sampleFormTmpl" type="text/x-jquery-tmpl">
  <form name='sampleForm' method='POST' action='<%= GetHandlerUrl %>' accept-charset='UTF-8'>
    <meta content='text/html;charset=UTF-8' http-equiv='content-type'>
    <style type="text/css">
        #sampleFormPanel
        {
           padding: 10px;
        }

        #sampleFormPanel dt
        {
           float: left;
           text-align: right;
           width: 40%;

         }
        #sampleFormPanel dd
        {
             margin-bottom: 5px;
             margin-left: 40%;
             padding-left: 10px;
        }
        #sampleFormPanel input[type=text]
        {
             border: solid 1px #C7C7C7;
        }
    </style>

    <dl id="sampleFormPanel" class="tintMedium">
         {{each fieldListInfo}}
           <dt>
            ${title}:
           </dt>
           <dd>
              <input name="${name}" type="text"/>
           </dd>
        {{/each}}
        {{each tagListInfo}}
           <input type="hidden" name="tag" value="${title}" />
        {{/each}}
        <dt>
        </dt>
        <dd>
            <input name="${name}" value="<%= CRMSettingResource.SubmitFormData %>" type="submit"
                onclick="javascript:
                            var isValid = true;
                            var form = document.getElementById('sampleFormPanel');
                            var childs = form.getElementsByTagName('input');
                            var firstName;
                            var lastName;
                            var companyName;
                            for(var i = 0; i < childs.length; i++) {
                                if(childs[i].getAttribute('name')=='firstName')
                                    firstName = childs[i];
                                if(childs[i].getAttribute('name')=='lastName')
                                    lastName = childs[i];
                                if(childs[i].getAttribute('name')=='companyName')
                                    companyName = childs[i];
                            }
                            if(typeof firstName != 'undefined' && typeof lastName != 'undefined') {
                                if (firstName.value.trim() == ''){
                                    alert('<%= CRMContactResource.ErrorEmptyContactFirstName %>');
                                    isValid = false;
                                }
                                else if (lastName.value.trim() == ''){
                                    alert('<%= CRMContactResource.ErrorEmptyContactLastName %>');
                                    isValid = false;
                                }
                            }
                            else if(typeof companyName != 'undefined') {
                                if(companyName.value.trim() == '') {
                                    alert('<%= CRMContactResource.ErrorEmptyCompanyName %>');
                                    isValid = false;
                                }
                            } else {
                                isValid = false;
                            }
                            return isValid;"/>
        </dd>
    </dl>
    <input type="hidden" name="return_url" value="${returnURL}" />
    <input type="hidden" name="web_form_key"  value="${webFormKey}"/>
    <input type="hidden" name="notify_list" value="${notifyList}"/>
    <input type="hidden" name="private_list" value="${privateList}"/>
  </form>
</script>

<script type="text/javascript">

    jq(document).ready(function() {
        ASC.CRM.SettingsPage.WebToLeadFormView.init();
    });

</script>

