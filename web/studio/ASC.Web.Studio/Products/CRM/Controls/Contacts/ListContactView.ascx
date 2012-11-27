<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListContactView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Contacts.ListContactView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="aswscc" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Core.Users" %>

<% if (!IsSimpleView) %>
<% { %>
<% if (Global.DebugVersion) %>
<% { %>
<link href="<%= PathProvider.GetFileStaticRelativePath("tasks.css") %>" rel="stylesheet" type="text/css" />
<link href="<%= PathProvider.GetFileStaticRelativePath("settings.css") %>" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("tasks.js")%>"></script>
<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("fileUploader.js")%>"></script>
<% } %>

<script type="text/javascript" >

    jq(document).ready(function() {

        ASC.CRM.ListContactView.init(
            <%= EntryCountOnPage %>,
            <%= CurrentPageNumber %>,
            <%= MailSender.GetQuotas() %>,
            "<%= ASC.Core.CoreContext.UserManager.GetUsers(ASC.Core.SecurityContext.CurrentAccount.ID).DisplayUserName() %>",
            "<%= ASC.Core.SecurityContext.CurrentAccount.ID %>",
            <%= CRMSecurity.IsAdmin.ToString().ToLower() %>,
            "<%= CookieKeyForPagination %>",
            "<%= Anchor %>"
        );

        jq("#mainSelectAll").attr("checked", false);
        ASC.CRM.ListContactView.selectAll(jq("#mainSelectAll"));

        <% if(!MobileVer) {%>
            ASC.CRM.FileUploader.activateUploader();
        <% } %>

        (function($) {
            if (!jq("#contactsAdvansedFilter").advansedFilter) return;

            ASC.CRM.ListContactView.advansedFilter = jq("#contactsAdvansedFilter")
                .advansedFilter({
                    anykey: false,
                    help : '<%= String.Format(CRMCommonResource.AdvansedFilterInfoText.ReplaceSingleQuote(),"<b>","</b>") %>',
                    maxfilters: 3,
                    maxlength: "100",
                    store      : true,
                    inhash     : true,
                    filters:    [
                                    {
                                        type         : "combobox",
                                        id           : "company",
                                        apiparamname : "contactListView",
                                        title        : "<%=ContactListViewType.Company.ToLocalizedString()%>",
                                        filtertitle  : "<%=CRMCommonResource.Show%>",
                                        group        : "<%=CRMCommonResource.Show%>",
                                        groupby      : 'type',
                                        options      :
                                                   [
                                                    {value : "<%=ContactListViewType.Company.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.Company.ToLocalizedString()%>", def : true},
                                                    {value : "<%=ContactListViewType.Person.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.Person.ToLocalizedString()%>"},
                                                    {value : "<%=ContactListViewType.WithOpportunity.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.WithOpportunity.ToLocalizedString()%>"}
                                                   ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "person",
                                        apiparamname : "contactListView",
                                        title        : "<%=ContactListViewType.Person.ToLocalizedString()%>",
                                        filtertitle  : "<%=CRMCommonResource.Show%>",
                                        group        : "<%=CRMCommonResource.Show%>",
                                        groupby      : 'type',
                                        options      :
                                                   [
                                                    {value : "<%=ContactListViewType.Company.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.Company.ToLocalizedString()%>"},
                                                    {value : "<%=ContactListViewType.Person.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.Person.ToLocalizedString()%>", def : true},
                                                    {value : "<%=ContactListViewType.WithOpportunity.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.WithOpportunity.ToLocalizedString()%>"}
                                                   ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "withopportunity",
                                        apiparamname : "contactListView",
                                        title        : "<%=ContactListViewType.WithOpportunity.ToLocalizedString()%>",
                                        filtertitle  : "<%=CRMCommonResource.Show%>",
                                        group        : "<%=CRMCommonResource.Show%>",
                                        groupby      : 'type',
                                        options      :
                                                   [
                                                    {value : "<%=ContactListViewType.Company.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.Company.ToLocalizedString()%>"},
                                                    {value : "<%=ContactListViewType.Person.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.Person.ToLocalizedString()%>"},
                                                    {value : "<%=ContactListViewType.WithOpportunity.ToString().ToLower() %>",  classname : '', title : "<%=ContactListViewType.WithOpportunity.ToLocalizedString()%>", def : true}
                                                   ]
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "contactType",
                                        apiparamname : "contactType",
                                        title        : "<%= CRMContactResource.AfterStage%>",
                                        group        : "<%= CRMCommonResource.Other%>",
                                        options      : contactTypes,
                                        defaulttitle : "<%= CRMCommonResource.Choose %>",
                                        enable       : contactTypes.length > 0
                                    },
                                    {
                                        type         : "combobox",
                                        id           : "tags",
                                        apiparamname : "tags",
                                        title        : "<%= CRMCommonResource.FilterWithTag%>",
                                        group        : "<%= CRMCommonResource.Other%>",
                                        options      : contactTags,
                                        defaulttitle: "<%= CRMCommonResource.Choose %>",
                                        multiple: true,
                                        enable: contactTags.length > 0
                                    }
                                ],
                    sorters:    [
                                    { id: "displayname", title: "<%=CRMCommonResource.Title%>", dsc: false, def : true },
                                    { id: "contacttype", title: "<%=CRMContactResource.AfterStage%>", dsc: false, def: false }
                                ]
                })
                .bind("setfilter", ASC.CRM.ListContactView.changeFilter)
                .bind("resetfilter", ASC.CRM.ListContactView.changeFilter);

        })(jQuery);

    });

    jq(document).click(function(event)
    {
        jq.dropdownToggle().registerAutoHide(event, "#mainAddTag", "#addTagDialog");
        jq.dropdownToggle().registerAutoHide(event, "#mainSendEmail", "#sendEmailDialog");
        jq.dropdownToggle().registerAutoHide(event, "#mainExportCsv", "#exportDialog");
    });

</script>

<div id="mainContactList">
    <div>
        <a class="linkAddMediumText" href="default.aspx?action=manage">
            <%=CRMContactResource.AddNewCompany%>
        </a>
        <span class="splitter"></span>
        <a class="linkAddMediumText" href="default.aspx?action=manage&type=people">
            <%=CRMContactResource.AddNewContact%>
        </a>
        <% if (!MobileVer) %>
        <% { %>
        <span class="splitter"></span>
        <a id="importContactLink" class="crm-importLink" href="default.aspx?action=import">
            <%= CRMContactResource.ImportContacts%>
        </a>
        <% if (CRMSecurity.IsAdmin) %>
        <% { %>
        <span class="splitter"></span>
        <span id="mainExportCsv" title="<%= CRMCommonResource.ExportCurrentListToCsv %>" class="crm-exportToCsvLink"
         onclick="ASC.CRM.ListContactView.showExportDialog();">
            <a class="baseLinkAction"><%= CRMCommonResource.ExportCurrentListToCsv%></a>
            <img align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("arrow_down.gif", ProductEntryPoint.ID)%>">
        </span>
        <% } %>
        <% } %>
    </div>

    <br />

    <div class="clearFix">
        <div id="contactsFilterContainer">
            <aswscc:advansedfilter runat="server" id="filter" blockid="contactsAdvansedFilter"></aswscc:advansedfilter>
        </div>
        <br />
        <div id="companyListBox" style="display: none">
            <ul id="contactsHeaderMenu" class="clearFix contentMenu contentMenuDisplayAll" style="display: block; padding-left: 7px;">
                <li class="crm-menuActionSelectAll">
                    <div style="float:left; margin:1px 0 0 1px;">
                        <input type="checkbox" id="mainSelectAll" title="<%=CRMCommonResource.SelectAll%>" onclick="ASC.CRM.ListContactView.selectAll(this);" />
                    </div>
                </li>
                <% if (CRMSecurity.IsAdmin) %>
                <% { %>
                <li id="mainSendEmail" class="menuAction menuActionSendEmail" title="<%= CRMCommonResource.SendEmail %>">
                    <span><%= CRMCommonResource.SendEmail%></span>
                    <img src="<%=WebImageSupplier.GetAbsoluteWebPath("arrow_down_light.png", ProductEntryPoint.ID)%>" align="absmiddle"/>
                </li>
                <% } %>
                <li id="mainAddTag" class="menuAction menuActionAddTag" title="<%= CRMCommonResource.AddNewTag %>">
                    <span><%=CRMCommonResource.AddNewTag%></span>
                    <img src="<%=WebImageSupplier.GetAbsoluteWebPath("arrow_down_light.png", ProductEntryPoint.ID)%>" align="absmiddle"/>
                </li>
                <li id="mainSetPermissions" class="menuAction menuActionPermissions" title="<%= CRMCommonResource.SetPermissions %>">
                    <span><%=CRMCommonResource.SetPermissions%></span>
                </li>
                <li id="mainDelete" class="menuAction menuActionDelete" title="<%= CRMCommonResource.Delete %>">
                    <span><%= CRMCommonResource.Delete%></span>
                </li>
                <li id="simpleContactPageNavigator">
                </li>
                <li id="checkedContactsCount" style="display:none;">
                    <span></span>
                    <a class="linkDescribe baseLinkAction" style="margin-left:10px;" onclick="ASC.CRM.ListContactView.deselectAll();">
                        <%= CRMContactResource.DeselectAll%>
                    </a>
                </li>
                <li id="onTop">
                    <a class="crm-onTopLink" onclick="javascript:window.scrollTo(0, 0);">
                        <%= CRMCommonResource.OnTop%>
                    </a>
                </li>
            </ul>
            <div id="contactsHeaderMenuSpacer" style="display: none;">&nbsp;</div>
            <table id="companyTable" class="tableBase" cellpadding="4" cellspacing="0">
                <tbody>
                </tbody>
            </table>

            <table id="tableForContactNavigation" class="crm-navigationPanel" cellpadding="4" cellspacing="0" border="0">
                <tbody>
                <tr>
                    <td>
                        <div id="divForContactPager">
                            <asp:PlaceHolder ID="_phPagerContent" runat="server"></asp:PlaceHolder>
                        </div>
                    </td>
                    <td style="text-align:right;">
                        <span class="grayText"><%= CRMContactResource.TotalContacts%>:</span>
                        <span class="grayText" id="totalContactsOnPage"></span>

                        <span class="grayText"><%= CRMCommonResource.ShowOnPage%>:&nbsp;</span>
                        <select>
                            <option value="25">25</option>
                            <option value="50">50</option>
                            <option value="75">75</option>
                            <option value="100">100</option>
                        </select>
                        <script type="text/javascript">
                            jq('#tableForContactNavigation select')
                            .val(<%= EntryCountOnPage %>)
                            .change(function(evt) {
                                ASC.CRM.ListContactView.changeCountOfRows(this.value);
                            })
                            .tlCombobox();
                        </script>
                    </td>
                </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>

<asp:PlaceHolder ID="_phTaskActionView" runat="server"></asp:PlaceHolder>

<aswscc:EmptyScreenControl ID="emptyContentForContactsFilter" runat="server"></aswscc:EmptyScreenControl>

<aswscc:EmptyScreenControl ID="contactsEmptyScreen" runat="server"></aswscc:EmptyScreenControl>

<div id="deletePanel" style="display: none;">
    <ascwc:container id="_deletePanel" runat="server">
        <header>
            <%= CRMCommonResource.Confirmation%>
        </header>
        <body>
            <div>
                <b><%= CRMCommonResource.ConfirmationDeleteText%></b>
            </div>

            <div id="deleteList" class="selectedContactsList">
                <dl>
                    <dt class="confirmRemoveCompanies">
                        <%= CRMContactResource.Companies%>:
                    </dt>
                    <dd class="confirmRemoveCompanies">
                    </dd>
                    <dt class="confirmRemovePersons">
                        <%= CRMContactResource.Persons%>:
                    </dt>
                    <dd class="confirmRemovePersons">
                    </dd>
                </dl>
            </div>

            <div class="h_line">&nbsp;</div>

            <div class="action-block">
                <a class="baseLinkButton" onclick="ASC.CRM.ListContactView.deleteBatchContacts();">
                    <%= CRMCommonResource.OK%>
                </a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="jq.unblockUI();">
                    <%= CRMCommonResource.Cancel%>
                </a>
            </div>

            <div class='info-block' style="display: none;">
                <span class="textMediumDescribe">
                    <%= CRMContactResource.DeletingContacts%>
                </span><br />
                <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>
        </body>
    </ascwc:container>
</div>

<div id="setPermissionsPanel" style="display: none;">
    <ascwc:container id="_setPermissionsPanel" runat="server">
        <header>
            <%= CRMCommonResource.SetPermissions%>
        </header>
        <body>
            <% if (!CRMSecurity.IsAdmin) %>
            <% { %>
            <div style="margin-top:10px">
                <b><%= CRMCommonResource.AccessRightsLimit%></b>
            </div>
            <% } %>
            <asp:PlaceHolder runat="server" ID="_phPrivatePanel"></asp:PlaceHolder>

            <div class="h_line">&nbsp;</div>

            <div class="action-block">
                <a class="baseLinkButton setPermissionsLink">
                    <%= CRMCommonResource.OK%>
                </a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="jq.unblockUI();">
                    <%= CRMCommonResource.Cancel%>
                </a>
            </div>

            <div class='info-block' style="display: none;">
                <span class="textMediumDescribe">
                    <%= CRMCommonResource.SaveChangesProggress%>
                </span><br />
                <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>
        </body>
    </ascwc:container>
</div>

<div id="createLinkPanel" style="display: none;">
    <ascwc:container id="_createLinkPanel" runat="server">
        <header>
            <%= CRMContactResource.GenerateLinks%>
        </header>
        <body>

            <div class="headerPanel-splitter bold clearFix">
                <input type="checkbox" id="cbxBlind" style="float:left">
                <label for="cbxBlind" style="float:left;padding: 3px 0 0 4px;">
                    <%= CRMContactResource.BlindLinkInfoText%>
                </label>
            </div>

            <div class="textBigDescribe headerPanel-splitter">
                    <%= CRMContactResource.BatchSizeInfoText%>
                </div>

            <div class="headerPanel-splitter">
                <b  style="padding-right:5px;"><%= CRMContactResource.BatchSize%>:</b>
                <input maxlength="10" class="textEdit" id="tbxBatchSize" style="width:100px;" />
            </div>

            <div id="linkList" style="display:none;"></div>

            <div class="h_line">&nbsp;</div>

            <div class="action-block">
                <a class="baseLinkButton">
                    <%= CRMContactResource.Generate%>
                </a>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="jq.unblockUI();">
                    <%= CRMCommonResource.Cancel%>
                </a>
            </div>

            <div class="info-block" style="display: none;">
                <span class="textMediumDescribe">
                    <%= CRMContactResource.Generation%>
                </span><br />
                <img src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>

        </body>
    </ascwc:container>
</div>

<div id="smtpSettingsPanel" style="display: none;">
    <ascwc:container id="_smtpSettingsPanel" runat="server">
        <header>
            <%= CRMSettingResource.ConfigureTheSMTP%>
        </header>
        <body>
            <div class="headerPanel-splitter">
                <b><%= CRMSettingResource.ConfigureTheSMTPInfo%></b>
            </div>
            <div id="smtpSettingsContent">
                <table cellpadding="5" cellspacing="0">
                    <tr>
                        <td>
                            <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.Host%>:</div>
                            <input type="text" class="textEdit" style="width: 200px;" id="tbxHost"/>
                        </td>
                        <td>
                            <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.Port%>:</div>
                            <div>
                                <input type="text" class="textEdit" style="width: 50px; float: left;" id="tbxPort" maxlength="5"/>
                                <input id="cbxAuthentication" type="checkbox" style="margin:4px 6px 0 10px; float: left;" onchange="ASC.CRM.ListContactView.changeAuthentication()">
                                <label for="cbxAuthentication" class="headerBaseSmall" style="line-height: 21px;"><%=CRMSettingResource.Authentication%></label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.HostLogin%>:</div>
                            <input type="text" class="textEdit" style="width: 200px;" id="tbxHostLogin"/>
                        </td>
                        <td>
                            <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.HostPassword%>:</div>
                            <input type="password" class="textEdit" style="width: 200px;" id="tbxHostPassword"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.SenderDisplayName%>:</div>
                            <input type="text" class="textEdit" style="width: 200px;" id="tbxSenderDisplayName"/>
                        </td>
                        <td>
                            <div class="headerBaseSmall headerTitle"><%=CRMSettingResource.SenderEmailAddress%>:</div>
                            <input type="text" class="textEdit" style="width: 200px;" id="tbxSenderEmailAddress"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input id="cbxEnableSSL" type="checkbox" style="margin-left: 0px; float: left; margin-right: 6px;">
                            <label for="cbxEnableSSL" class="headerBaseSmall" style="float: left; line-height: 20px;">
                                <%=CRMSettingResource.EnableSSL%>
                            </label>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <div class="h_line">&nbsp;</div>

            <div class="action-block">
                <% if (!MobileVer)
                   {%>
                <a class="baseLinkButton" onclick="ASC.CRM.ListContactView.saveSMTPSettings(window.FCKeditorAPI.Instances.<%= Editor.ClientID %>);">
                    <%= CRMCommonResource.Save%>
                </a>
                <% }
                   else
                   { %>
                <a class="baseLinkButton" onclick="ASC.CRM.ListContactView.saveSMTPSettings();">
                    <%= CRMCommonResource.Save%>
                </a>
                <% } %>
                <span class="splitter"></span>
                <a class="grayLinkButton" onclick="jq.unblockUI();">
                    <%= CRMCommonResource.Cancel%>
                </a>
            </div>

            <div class='info-block' style="display: none;">
                <span class="textMediumDescribe">
                    <%= CRMContactResource.SavingChangesProgress%>
                </span><br />
                <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>
        </body>
    </ascwc:container>
</div>

<div id="sendEmailPanel" style="display:none;margin-top: 20px;">
    <div id="createContent" style="margin-bottom: 20px;">
        <div class="headerPanel-splitter">
            <%= CRMCommonResource.ComposeMailDescription%>
        </div>
        <div class="headerPanel-splitter">
            <b style="padding-right:5px;"><%= CRMCommonResource.MailFrom%>:</b>
            <span id="emailFromLabel"></span>
        </div>
        <div class="headerPanel-splitter">
            <b style="padding-right:5px;"><%= CRMCommonResource.MailTo%>:</b>
            <span id="emailAddresses"></span>
        </div>
        <div class="headerPanel-splitter">
            <div class="headerPanelSmall-splitter bold">
                <%= CRMCommonResource.Subject%>:
            </div>
            <input type="text" class="textEdit" id="tbxEmailSubject" style="width: 100%" />
        </div>
        <div class="headerPanel-splitter">
            <b style="padding-right:5px;"><%= CRMContactResource.PersonalTags%>:</b>
            <select id="emailTagTypeSelector" onchange="ASC.CRM.ListContactView.renderTagSelector()">
                <option value="person" selected="selected"><%=CRMContactResource.Person%></option>
                <option value="company"><%=CRMContactResource.Company%></option>
            </select>
            <select id="emailPersonTagSelector">
                <%=RenderTagSelector(false)%>
            </select>
            <select id="emailCompanyTagSelector" style="display: none;">
                <%=RenderTagSelector(true)%>
            </select>
            <a onclick="ASC.CRM.ListContactView.emailInsertTag()" class="grayLinkButton" style="margin: 0 0 3px 5px">
                <%= CRMContactResource.AddToLetterBody%>
            </a>
        </div>
        <div class="headerPanel-splitter requiredField">
            <span class="requiredErrorText"></span>
            <div class="headerPanelSmall headerPanelSmall-splitter bold">
                <%= CRMCommonResource.LetterBody%>:
            </div>
            <input type="hidden" id="requiredMessageBody"/>
            <% if (!MobileVer)
               {%>
            <asp:PlaceHolder ID="_phFCKeditor" runat="server"></asp:PlaceHolder>
            <% }
               else
               { %>
            <textarea ID="mobileMessageBody" style="width:100%; height:200px;"></textarea>
            <% } %>
        </div>
        <div class="headerPanel-splitter">
            <span class="textMediumDescribe">*<%= CRMContactResource.TeamlabWatermarkInfo%></span>
        </div>
        <div class="clearFix">
            <% if (!MobileVer)
               {%>
            <div style="float:right;">
                <a class="attachLink baseLinkAction" onclick="javascript:jq('#attachOptions').show(); jq(this).hide().next().show();" >
                    <%= CRMCommonResource.ShowAttachPanel%>
                </a>
                <a class="attachLink baseLinkAction" onclick="javascript:jq('#attachOptions').hide();jq(this).hide().prev().show();" style="display: none;" >
                    <%= CRMCommonResource.HideAttachPanel%>
                </a>
            </div>
            <% } %>
            <div>
                <input id="storeInHistory" type="checkbox" style="float: left;"/>
                <label for="storeInHistory" style="float: left; padding: 2px 0 0 4px;">
                    <%= CRMCommonResource.StoreThisLetterInHistory%>
                </label>
            </div>
        </div>
        <% if (!MobileVer)
           {%>
        <div id="attachOptions" style="display:none;margin: 10px 0;">
            <asp:PlaceHolder ID="_phfileUploader" runat="server" />
        </div>
        <% } %>
    </div>
    <div id="previewContent" style="display:none;margin-bottom: 20px;">
        <div class="headerPanel-splitter">
            <%= CRMCommonResource.PreviewMailDescription%>
        </div>
        <div class="headerPanel-splitter">
            <b style="padding-right:5px;"><%= CRMCommonResource.From%>:</b>
            <span id="previewEmailFromLabel"></span>
        </div>
        <div class="headerPanel-splitter">
            <b style="padding-right:5px;"><%= CRMCommonResource.To%>:</b>
            <span id="previewEmailAddresses"></span>
        </div>
        <div class="headerPanel-splitter">
            <div class="headerPanelSmall-splitter bold"><%= CRMCommonResource.Subject%>:</div>
            <div id="previewSubject"></div>
        </div>
        <div class="headerPanel-splitter">
            <div class="headerPanelSmall-splitter bold"><%= CRMCommonResource.LetterBody%>:</div>
            <div id="previewMessage" style="max-height: 400px;overflow-y: auto;"></div>
        </div>
        <div class="headerPanel-splitter" id="previewAttachments" style="display:none;">
            <div class="headerPanelSmall-splitter bold"><%= CRMCommonResource.Attachments%>:</div>
            <img src="<%= WebImageSupplier.GetAbsoluteWebPath("skrepka.gif", ProductEntryPoint.ID) %>" align="absmiddle"/>
            <span></span>
        </div>
    </div>
    <div class="h_line">&nbsp;</div>
    <div class="action-block">
        <span id="backButton" style="display:none;">
            <a class="baseLinkButton">
                <%= CRMCommonResource.Back%>
            </a>
            <span class="splitter"></span>
        </span>
        <a class="baseLinkButton" id="sendButton">
            <%= CRMJSResource.NextPreview%>
        </a>
        <span class="splitter"></span>
        <a class="grayLinkButton" onclick="ASC.CRM.ListContactView.closeSendEmailPanel();">
            <%= CRMCommonResource.Cancel%>
        </a>
    </div>
    <div class="info-block" style="display: none;">
        <span class="textMediumDescribe">
            <%= CRMCommonResource.PleaseWait%>
        </span><br />
        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
    </div>
</div>

<div id="sendProcessPanel" style="display:none;">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <img src="<%=WebImageSupplier.GetAbsoluteWebPath("mail_send.png", ProductEntryPoint.ID)%>"/>
            </td>
            <td style="color: #787878;font-size: 17px;">
                <%= String.Format(CRMContactResource.MassSendInfo, "<br/>")%>
                <div class="clearFix" style="margin-top: 15px;">
                    <div style="float: right;margin-left: 10px;">
                        <a class="grayLinkButton" onclick="ASC.CRM.ListContactView.abortMassSend()" id="abortButton" style="width: 90px;">
                            <%= CRMContactResource.AbortMassSend%>
                        </a>
                        <a class="grayLinkButton" onclick="ASC.CRM.ListContactView.closeSendEmailPanel()" id="okButton" style="display: none;width: 90px;">
                            <%= CRMCommonResource.OK%>
                        </a>
                    </div>
                    <div id="sendProcessProgress" class="progress_box">
                        <div class="progress" style="width: 0%"></div>
                        <span class="percent">0%</span>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <dl id="sendProcessInfo">
                    <dt class="textBigDescribe">
                         <%= CRMContactResource.MassSendEmailsTotal%>:
                    </dt>
                    <dd id="emailsTotalCount"></dd>
                    <dt class="textBigDescribe">
                         <%= CRMContactResource.MassSendAlreadySent%>:
                    </dt>
                    <dd id="emailsAlreadySentCount"></dd>
                    <dt class="textBigDescribe">
                         <%= CRMContactResource.MassSendEstimatedTime%>:
                    </dt>
                    <dd id="emailsEstimatedTime"></dd>
                    <dt class="textBigDescribe">
                         <%= CRMContactResource.MassSendErrors%>:
                    </dt>
                    <dd id="emailsErrorsCount"></dd>
                </dl>
            </td>
        </tr>
    </table>
</div>

<div style="display: none;" id="addTagDialog" class="dropDownDialog">
    <div class="dropDownCornerLeft" style="margin-left: -3px"></div>
    <div class="dropDownContent"></div>
    <div class="h_line">&nbsp;</div>
    <div style="margin-bottom:5px;"><%= CRMCommonResource.CreateNewTag%>:</div>
    <input type="text" maxlength="50" class="textEdit">
    <a onclick="ASC.CRM.ListContactView.addNewTag();" class="baseLinkButton" id="addThisTag">
        <%= CRMCommonResource.OK%>
    </a>
</div>

<div style="display: none;" id="sendEmailDialog" class="dropDownDialog">
    <div class="dropDownCornerLeft" style="margin-left: -3px"></div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.ListContactView.showCreateLinkPanel()">
            <%=CRMSettingResource.ExternalClient%>
        </a>
        <% if (!MobileVer)
           {%>
        <a class="dropDownItem" onclick="ASC.CRM.ListContactView.showSendEmailPanel(window.FCKeditorAPI.Instances.<%= Editor.ClientID %>)">
            <%=String.Format(CRMSettingResource.InternalSMTP, MailSender.GetQuotas())%>
        </a>
        <% }
           else
           { %>
        <a class="dropDownItem" onclick="ASC.CRM.ListContactView.showSendEmailPanel()">
            <%=String.Format(CRMSettingResource.InternalSMTP, MailSender.GetQuotas())%>
        </a>
        <% } %>
    </div>
</div>

<div style="display: none;" id="exportDialog" class="dropDownDialog">
    <div class="dropDownCornerLeft" style="margin-left: -5px"></div>
    <div class="dropDownContent">
        <a class="dropDownItem" onclick="ASC.CRM.ListContactView.exportToCsv();">
            <%= CRMCommonResource.DownloadOneFile%>
        </a>
        <a class="dropDownItem" onclick="ASC.CRM.ListContactView.openExportFile();">
            <%= CRMCommonResource.OpenToTheEditor%>
        </a>
    </div>
</div>

<div id="contactActionMenu" class="dropDownDialog">
    <div class="dropDownCornerRight">&nbsp;</div>
    <div class="dropDownContent">
        <a class="showProfileLink dropDownItem"><%= CRMContactResource.ShowContactProfile%></a>
        <a class="addPhoneLink dropDownItem"><%= CRMJSResource.AddNewPhone%></a>
        <a class="addEmailLink dropDownItem"><%= CRMJSResource.AddNewEmail%></a>
        <a class="addTaskLink dropDownItem"><%= CRMTaskResource.AddNewTask%></a>
        <a class="addDealLink dropDownItem"><%= CRMDealResource.CreateNewDeal %></a>
        <a class="addCaseLink dropDownItem"><%= CRMCasesResource.AddCase %></a>
        <a class="setPermissionsLink dropDownItem"><%= CRMCommonResource.SetPermissions%></a>
        <a class="editContactLink dropDownItem"><%= CRMContactResource.EditContact%></a>
        <a class="deleteContactLink dropDownItem"><%= CRMContactResource.DeleteContact%></a>
<%--        <a class="sendEmailLink dropDownItem"><%= CRMCommonResource.SendEmail %></a>--%>
    </div>
</div>

<script id="contactListTmpl" type="text/x-jquery-tmpl">
    <tbody>
        {{tmpl(contacts) "#contactTmpl"}}
    </tbody>
</script>

<script id="contactTmpl" type="text/x-jquery-tmpl">
    <tr id="contactItem_${id}" class="with-crm-menu">
        <td class="borderBase" style="width:15px; padding: 0 0 0 10px;">
            <input type="checkbox" id="check_contact_${id}" onclick="ASC.CRM.ListContactView.selectItem(this);" style="margin-left: 2px;" {{if isChecked == true}}checked="checked"{{/if}}>
            <img style="display:none;" src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID)%>" id="loaderImg_${id}">
        </td>

        <td class="borderBase" style="width:40px;">
            <img src="${smallFotoUrl}" />
        </td>

        <td class="borderBase" style="width:50%;">
            <div style="line-height: 20px;">
                {{if isPrivate == true}}
                    <img align="absmiddle" style="margin-bottom: 3px;" src="<%=WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID)%>">
                {{/if}}
                <a class="linkHeaderMedium" href="default.aspx?id=${id}">
                    ${displayName}
                </a>
            </div>
            {{if isCompany == false && company != null}}
                <div style="line-height: 20px;">
                    <%=CRMContactResource.Company%>:
                    <a href="default.aspx?id=${company.id}" data-id="${company.id}" id="contact_${id}_company_${company.id}" class="linkMedium crm-companyInfoCardLink">
                        ${company.displayName}
                    </a>
                </div>
            {{/if}}
        </td>

        <td class="borderBase" style="width:200px">
            <div class="primaryDataContainer">
                <input type="text" id="addPrimaryPhone_${id}" class="textEdit addPrimaryDataInput" autocomplete="off" maxlength="100"/>
            {{if primaryPhone != null}}
                <span class="primaryPhone" title="${primaryPhone.data}">${primaryPhone.data}</span>
            {{/if}}
            </div>
            <div class="primaryDataContainer">
                <input type="text" id="addPrimaryEmail_${id}" class="textEdit addPrimaryDataInput" autocomplete="off" maxlength="100"/>
            {{if primaryEmail != null}}
                <a class="primaryEmail linkMedium" title="${primaryEmail.data}" href="mailto:${primaryEmail.data}">${primaryEmail.data}</a>
            {{/if}}
            </div>
        </td>

        <td class="borderBase" style="width:200px">
            {{if nearTask != null}}
                <span id="taskTitle_${nearTask.id}" class="headerBaseSmall nearestTask"
                    ttl_label="<%=CRMCommonResource.Title%>" ttl_value="${nearTask.title}"
                    dscr_label="<%=CRMCommonResource.Description%>" dscr_value="${nearTask.description}"
                    resp_label="<%=CRMCommonResource.Responsible%>" resp_value="${nearTask.responsible.displayName}">
                        ${nearTask.category.title} ${nearTask.deadLineString}
                </span>
            {{/if}}
        </td>
        <td class="borderBase" style="width:28px;padding:5px;">
            <div id="contactMenu_${id}" class="crm-menu" title="<%= CRMCommonResource.Actions %>"
                 onclick='ASC.CRM.ListContactView.showActionMenu(${id}, ${isCompany}, "${encodeURI(displayName)}", {{if primaryEmail != null}} "${primaryEmail}" {{else}} null {{/if}}, "${createdBy.id}");'>
            </div>
        </td>
    </tr>
</script>

<% } %>
<% else %>
<% {%>

<div id="contactListBox">
    <table id="contactTable" class="tableBase" cellpadding="4" cellspacing="0">
        <tbody>
        </tbody>
    </table>
</div>

<script id="simpleContactTmpl" type="text/x-jquery-tmpl">
    <tr id="contactItem_${id}">

        <td class="borderBase" style="width:40px;">
            <img src="${smallFotoUrl}" />
        </td>

        <td class="borderBase" style="width:50%;">
            {{if isPrivate == true}}
            <img align="absmiddle" style="margin-bottom: 3px;" src="<%=WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID)%>">
            {{/if}}
            {{if typeof(id)=="number"}}
            <a class="linkHeaderMedium" href="default.aspx?id=${id}">${displayName}</a>
            {{else}}
            <span class="headerBaseSmall">${displayName}</span>
            {{/if}}
        </td>

         <td class="borderBase">
            <div class="primaryDataContainer">
                {{each(i, item) commonData}}
                    {{if item.infoType == 0 && item.isPrimary}}
                        <span title="${item.data}">${item.data}</span>
                    {{/if}}
                {{/each}}
            </div>
            <div class="primaryDataContainer">
                {{each(i, item) commonData}}
                    {{if item.infoType == 1 && item.isPrimary}}
                        <a title="${item.data}" href="mailto:${item.data}" class="linkMedium">${item.data}</a>
                    {{/if}}
                {{/each}}
            </div>
        </td>
        <td class="borderBase" style="width:25px;">
            {{if canEdit == true}}
            <img src="<%=WebImageSupplier.GetAbsoluteWebPath("trash.png", ProductEntryPoint.ID)%>"
               title="<%= CRMCommonResource.Delete %>"
               onclick='ASC.CRM.ListContactView.removeMember({{if typeof(id)=="number"}}${id}{{else}}"${id}"{{/if}});'
               id="trashImg_${id}" style="cursor:pointer">
            <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID)%>"
               id="loaderImg_${id}" style="display:none">
            {{/if}}
        </td>
    </tr>
</script>

<% } %>