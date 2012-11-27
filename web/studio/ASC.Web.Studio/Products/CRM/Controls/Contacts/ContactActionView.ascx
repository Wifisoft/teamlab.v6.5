<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactActionView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Contacts.ContactActionView" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.CRM.Core.Entities" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Register Src="../SocialMedia/ContactsSearchView.ascx" TagPrefix="ctrl" TagName="ContactsSearchView" %>

<% if (Global.DebugVersion) { %> 
<link href="<%= PathProvider.GetFileStaticRelativePath("socialmedia.css") %>" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("socialmedia.js") %>"></script>
<% } %>

<ctrl:ContactsSearchView runat="server" ID="_ctrlCompaniesSearchView"></ctrl:ContactsSearchView>
<input type="hidden" id="defaultContactIcon"
    value="<%= TargetContact != null && TargetContact is Person || TargetContact == null && UrlParameters.Type == "people"  ? ContactPhotoManager.GetBigSizePhoto(0, false) : ContactPhotoManager.GetBigSizePhoto(0, true) %>" />

<div id="crm_contactMakerDialog">
    <% if (TargetContact != null) %>
    <% { %>
    <asp:LinkButton runat="server" ID="deleteContactButton" OnClick="DeleteContact"
        CssClass="crm_deleteLinkButton" Style="margin-left: 17px;" />
    <% } %>
    <input type="hidden" name="typeAddedContact" id="typeAddedContact" value="<%= TypeAddedContact %>" />

    <div class="clearFix" id="contactProfileEdit">
        <div class="contactInfo">
        <% if (TargetContact != null && TargetContact is Person || TargetContact == null && UrlParameters.Type == "people") %>
        <% { %>
            <div class="info_for_person clearFix">
                <div class="requiredField" style="width: 344px;">
                    <span class="requiredErrorText"><%= CRMContactResource.ErrorEmptyContactFirstName%></span>
                    <div class="headerPanelSmall"><%= CRMContactResource.FirstName%></div>
                    <input type="text" class="textEdit generalField" name="baseInfo_firstName" maxlength="100"
                       value="<%= GetFirstName() %>"/>
                </div>
                <div class="requiredField" style="margin: 0 0 0 20px;">
                    <span class="requiredErrorText"><%= CRMContactResource.ErrorEmptyContactLastName%></span>
                    <div class="headerPanelSmall"><%= CRMContactResource.LastName%></div>
                    <input type="text" class="textEdit generalField" name="baseInfo_lastName" maxlength="100"
                        value="<%= GetLastName() %>"/>
                </div>
                <div>
                    <div class="bold"><%= CRMContactResource.CompanyName%></div>
                    <div id="companySelectorsContainer">
                        <div>
                            <asp:PlaceHolder ID="phContactSelector" runat="server"></asp:PlaceHolder>
                            <input type="hidden" name="baseInfo_compID" value="<%= GetCompanyIDforPerson() %>" />
                            <input type="hidden" name="baseInfo_compName"/>
                        </div>
                    </div>
                </div>

                <div style="margin: 0 0 0 14px;">
                    <div class="bold"><%= CRMContactResource.PersonPosition%></div>
                    <input type="text" class="textEdit generalField" name="baseInfo_personPosition"
                        maxlength="100" value="<%= GetTitle()%>" />
                </div>

                <a onclick="ASC.CRM.SocialMedia.FindContacts(false);" class="findInSocialMediaButton_Enabled baseLinkAction"
                    <%= TargetContact == null ? "style='display:none;'" : ""%> >
                    <%= CRMSocialMediaResource.FindInSocialMedia%></a>

                <a class="findInSocialMediaButton_Disabled baseLinkAction"
                    <%= TargetContact != null ? "style='display:none;'" : ""%> >
                    <%= CRMSocialMediaResource.FindInSocialMedia%></a>
            </div>
        <% }
           else
           { %>
            <div class="info_for_company clearFix">
                <div class="requiredField">
                    <span class="requiredErrorText"><%= CRMContactResource.ErrorEmptyCompanyName%></span>
                    <div class="headerPanelSmall"><%= CRMContactResource.CompanyName%></div>
                    <input type="text" class="textEdit generalField" name="baseInfo_companyName" maxlength="100"
                        value="<%= GetCompanyName()%>" />
                </div>
                <a onclick="ASC.CRM.SocialMedia.FindContacts(true);" class="findInSocialMediaButton_Enabled baseLinkAction"
                        style="margin-top: 8px;<%= TargetContact == null ? "display:none;" : ""%>" >
                    <%= CRMSocialMediaResource.FindInSocialMedia%></a>

                <a class="findInSocialMediaButton_Disabled baseLinkAction"
                        style="margin-top: 8px;<%= TargetContact != null ? "display:none;" : ""%>" >
                    <%= CRMSocialMediaResource.FindInSocialMedia%></a>
            </div>
        <% } %>


        <dl id="generalListEdit" class="clearFix">
            <dt class="headerBase headerExpand" onclick="javascript:jq(this).next('dd:first').nextUntil('#contactProfileEdit dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
                <%=CRMContactResource.GeneralInformation%>
            </dt>
            <dd class="underHeaderBase">&nbsp;</dd>

            <dt class="bold crm-withGrayPlus"><%= ContactInfoType.Email.ToLocalizedString()%></dt>
            <dd id="emailContainer" onclick="ASC.CRM.ContactActionView.editCommunicationsEvent(event, jq(this).attr('id'))">
                <div style="display:none !important;">
                    <table class="borderBase input_with_type" cellpadding="0" cellspacing="0">
                        <tr>
                            <td><input type="text" class="textEdit" name="contactInfo_Email_0_<%= (int)ContactInfoBaseCategory.Work %>_0" value="" maxlength="100"/></td>
                            <td style="width:1%;"><a onclick="ASC.CRM.ContactActionView.showBaseCategoriesPanel(this);"><%= ContactInfoBaseCategory.Work.ToLocalizedString()%></a></td>
                        </tr>
                    </table>
                    <div class="actions_for_item">
                        <a class="crm-deleteLink" title="<%=CRMCommonResource.Delete%>"></a>
                        <a class="is_primary not_primary_field" title="<%=CRMJSResource.CheckAsPrimary%>"></a>
                        <a class="crm-addNewLink" title="<%=CRMJSResource.AddNewEmail%>"></a>
                    </div>
                    <span class="requiredErrorText" style="float:right;"><%=CRMContactResource.ErrorInvalidEmail%></span>
                </div>
            </dd>


            <dt class="bold crm-withGrayPlus"><%= ContactInfoType.Phone.ToLocalizedString()%></dt>
            <dd id="phoneContainer" onclick="ASC.CRM.ContactActionView.editCommunicationsEvent(event, jq(this).attr('id'))">
                <div style="display:none !important;">
                    <table class="borderBase input_with_type" cellpadding="0" cellspacing="0">
                        <tr>
                            <td><input type="text" class="textEdit" name="contactInfo_Phone_0_<%= (int)PhoneCategory.Work %>_0" value="" maxlength="100"/></td>
                            <td style="width:1%;"><a onclick="ASC.CRM.ContactActionView.showPhoneCategoriesPanel(this);"><%= PhoneCategory.Work.ToLocalizedString()%></a></td>
                        </tr>
                    </table>
                    <div class="actions_for_item">
                        <a class="crm-deleteLink" title="<%=CRMCommonResource.Delete%>"></a>
                        <a class="is_primary not_primary_field" title="<%=CRMJSResource.CheckAsPrimary%>"></a>
                        <a class="crm-addNewLink" title="<%=CRMJSResource.AddNewPhone%>"></a>
                    </div>
                    <span class="requiredErrorText" style="float:right;"><%=CRMContactResource.ErrorInvalidPhone%></span>
                </div>
            </dd>


            <dt class="bold crm-withGrayPlus"><%= CRMContactResource.ContactWebSiteAndSocialProfiles%></dt>
            <dd id="websiteAndSocialProfilesContainer" onclick="ASC.CRM.ContactActionView.editCommunicationsEvent(event, jq(this).attr('id'))">
                <div style="display: none !important;">
                    <table class="borderBase input_with_type" cellpadding="0" cellspacing="0">
                        <tr>
                            <td><input type="text" name="contactInfo_<%= ContactInfoType.Website + "_0_" + (int)ContactInfoBaseCategory.Work %>_0" class="textEdit" value="" maxlength="255"/></td>
                            <td style="width:1%;">
                                 <a class="social_profile_type" onclick="ASC.CRM.ContactActionView.showSocialProfileCategoriesPanel(this);">
                                    <%= ContactInfoType.Website.ToLocalizedString()%>
                                </a>
                            </td>
                            <td style="width:1%;">
                                <a class="social_profile_category" onclick="ASC.CRM.ContactActionView.showBaseCategoriesPanel(this);">
                                    <%= ContactInfoBaseCategory.Work.ToLocalizedString()%>
                                </a>
                            </td>
                        </tr>
                    </table>
                    <%--<div class="textMediumDescribe" style="min-height:14px;"> </div>
                    --%>
                    <div class="actions_for_item">
                        <a class="crm-deleteLink" title="<%=CRMCommonResource.Delete%>"></a>
                        <a class="find_profile" title="" style="display: none;" onclick=""></a>
                        <a class="crm-addNewLink" title="<%=CRMCommonResource.Add%>"></a>
                    </div>
                </div>
            </dd>


            <dt class="bold crm-withGrayPlus"><%= ContactInfoType.Address.ToLocalizedString()%></dt>
            <dd id="addressContainer" onclick="ASC.CRM.ContactActionView.editAddressEvent(event)">
                <div style="display:none !important;" selectname="contactInfo_Address_0_<%= (int)AddressCategory.Billing + "_"  + AddressPart.Country%>_0">
                    <table class="" cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="3">
                                <div style="padding: 0 3px 2px 0; margin: 0 3px 1px 0;">
                                    <textarea class="contact_street" rows="4" cols="5" maxlength="255"
                                        name="contactInfo_Address_0_<%=(int)AddressCategory.Billing + "_" + AddressPart.Street %>_0"></textarea>
                                </div>
                            </td>
                        </tr>
                        <tr style="height: 22px;">
                            <td>
                                <div style="padding: 0 5px 1px 0; margin: 0 5px 1px 0;">
                                    <input type="text" class="contact_city textEdit" maxlength="255"
                                        name="contactInfo_Address_0_<%=(int)AddressCategory.Billing + "_" + AddressPart.City %>_0" />
                                </div>
                            </td>
                            <td>
                                <div style="padding: 0 3px 1px 0; margin: 0 3px 1px 0;">
                                    <input type="text" class="contact_state textEdit" maxlength="255"
                                        name="contactInfo_Address_0_<%=(int)AddressCategory.Billing + "_" + AddressPart.State %>_0"/>
                                </div>
                            </td>
                            <td>
                                <div style="padding: 0 1px 1px 1px; margin: 0 5px 1px 2px;">
                                    <input type="text" class="contact_zip textEdit" maxlength="255"
                                        name="contactInfo_Address_0_<%=(int)AddressCategory.Billing + "_" + AddressPart.Zip %>_0"/>
                                </div>
                            </td>
                        </tr>
                        <tr style="height: 22px;">
                            <td colspan="2">
                                <select id="contactCountry" class="contact_country comboBox" runat="server">
                                </select>
                            </td>
                            <td style="min-width:50px; padding: 0 0 0 3px;">
                                <select class="address_category comboBox" onchange="ASC.CRM.ContactActionView.changeAddressCategory(this)">
                                    <% foreach (AddressCategory item in Enum.GetValues(typeof(AddressCategory))) %>
                                    <% { %>
                                        <option category="<%=(int)item%>" <%= item == AddressCategory.Billing ? "selected='selected'" : "" %> ><%=item.ToLocalizedString()%></option>
                                    <% } %>
                                </select>
                            </td>
                        </tr>
                    </table>
                    <div class="actions_for_item">
                        <a class="crm-deleteLink" alt="<%=CRMCommonResource.Delete%>" title="<%=CRMCommonResource.Delete%>"></a>
                        <a class="is_primary not_primary_field"
                            title="<%=CRMJSResource.CheckAsPrimary%>" alt="<%=CRMJSResource.CheckAsPrimary%>"></a>
                        <a class="crm-addNewLink"
                            title="<%=CRMContactResource.AddNewAddress%>" alt="<%=CRMContactResource.AddNewAddress%>"></a>
                    </div>
                </div>
            </dd>


            <dt <%= (TargetContact != null && !String.IsNullOrEmpty(TargetContact.About)) ? "class='bold'": "class='bold crm-withGrayPlus'"%> ><%= CRMContactResource.Overview%></dt>
            <dd id="overviewContainer" onclick="ASC.CRM.ContactActionView.editCommunicationsEvent(event, jq(this).attr('id'))">
                <div style="display:none !important;">
                    <textarea type="text" rows="4" name="baseInfo_contactOverview" class="textEdit baseInfo_contactOverview"></textarea>
                    <div class="actions_for_item">
                        <a class="crm-deleteLink" title="<%=CRMCommonResource.Delete%>"></a>
                    </div>
                </div>

                <% if (TargetContact != null && !String.IsNullOrEmpty(TargetContact.About)) %>
                <% {%>
                <div>
                    <textarea type="text" rows="4" name="baseInfo_contactOverview" class="textEdit baseInfo_contactOverview"><%= TargetContact.About.HtmlEncode()%></textarea>
                    <div class="actions_for_item">
                        <a class="crm-deleteLink" title="<%=CRMCommonResource.Delete%>"></a>
                    </div>
                </div>
                <% } %>
            </dd>
        </dl>

        <% if (CRMSecurity.IsAdmin) %>
        <% {%>
        <div style="margin-left: 18px;margin-top: 10px;">
            <div class="bold" style="margin-bottom: 10px;"><%= CRMSettingResource.OtherFields %></div>
            <a onclick="ASC.CRM.ContactActionView.gotoAddCustomFieldPage();" style="text-decoration: underline" class="linkMedium">
                <%= CRMSettingResource.SettingCustomFields %>
            </a>
        </div>
        <% }%>

        <% if (UrlParameters.Type != "people") %>
        <% {%>
        <dl id="assignedContactsListEdit">
            <dt class="headerBase headerExpand" >
                <span onclick="ASC.CRM.ContactActionView.toggleAssignedContactsListEdit(this)"><%=CRMContactResource.AllPersons%></span>
                <a class="linkAssignContacts baseLinkAction" style="float:right"
                    onclick="ASC.CRM.ContactActionView.showAssignedContactPanel()" >
                    <%=CRMContactResource.AssignContact%>
                </a>
            </dt>
            <dd class="underHeaderBase">&nbsp;</dd>

            <dt class="bold assignedContacts hiddenFields"><%= CRMContactResource.AssignPersonFromExisting%></dt>
            <dd class="assignedContacts hiddenFields">
                <asp:PlaceHolder ID="_phAssignedContactSelector" runat="server"></asp:PlaceHolder>
            </dd>

            <dt style="margin-top:20px;">
                <asp:PlaceHolder ID="_phAssignedPersonsContainer" runat="server"></asp:PlaceHolder>
            </dt>
            <dd>
                <input type="hidden" name="baseInfo_assignedContactsIDs" />
            </dd>
        </dl>
         <% } %>

        <% if (HavePermission) %>
        <% { %>
            <div id="privatePanel" style="margin-top: 15px; width: 100%;">
                <asp:PlaceHolder ID="phPrivatePanel" runat="server"></asp:PlaceHolder>
                <input id="selectedPrivateUsers" type="hidden" value="" name="selectedPrivateUsers" />
                <input id="isPrivateContact" type="hidden" value="" name="isPrivateContact" />
                <input id="notifyPrivateUsers" type="hidden" value="" name="notifyPrivateUsers" />
            </div>
        <% } %>

        </div>

        <div class="additionInfo">
            <div id="contactPhoto">
                <img class="contact_photo" src="<%= TargetContact != null ? ContactPhotoManager.GetBigSizePhoto(TargetContact.ID, TargetContact is Company) : ContactPhotoManager.GetBigSizePhoto(0, UrlParameters.Type != "people")  %>" />
                <br />
               <%-- <a id="uploadPhoto" class="baseLinkAction"><%= CRMContactResource.LoadPhoto%></a>--%>
            </div>
            <div id="uploadPhotoMessage"></div>
            <input type="hidden" value="" id="uploadPhotoPath" name="uploadPhotoPath"/>
        </div>

    </div>

    <div style="margin-top: 25px;" id="contactActionPanelButtons" class="clearFix">
        <asp:LinkButton runat="server" CommandName="SaveContact" CommandArgument="0" ID="saveContactButton"
            OnCommand="SaveOrUpdateContact" CssClass="save_button baseLinkButton" />
        <span class="splitter"></span>
        <% if (TargetContact == null) %>
        <% { %>
        <asp:LinkButton runat="server" CommandName="SaveContact" CommandArgument="1" ID="saveAndCreateContactButton"
            OnCommand="SaveOrUpdateContact" CssClass="save_button grayLinkButton" />
        <span class="splitter"></span>
         <% } %>
        <asp:LinkButton runat="server" ID="cancelButton" class="grayLinkButton">
            <%= CRMCommonResource.Cancel%></asp:LinkButton>
    </div>
    <div style="display: none;margin-top: 25px;" class="ajax_info_block clearFix">
        <span class="textMediumDescribe">
            <%= AjaxProgressText%>
        </span>
        <br />
        <img alt="<%= AjaxProgressText %>" title="<%= AjaxProgressText %>" src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
    </div>
</div>


<div id="phoneCategoriesPanel" class="dropDownDialog">
    <div class="dropDownContent">
    <% foreach (PhoneCategory item in Enum.GetValues(typeof(PhoneCategory))) %>
    <% { %>
        <a category="<%=(int)item%>" class="dropDownItem"><%=item.ToLocalizedString()%></a>
    <% } %>
    </div>
</div>

<div id="baseCategoriesPanel" class="dropDownDialog">
    <div class="dropDownContent">
    <% foreach (ContactInfoBaseCategory item in Enum.GetValues(typeof(ContactInfoBaseCategory))) %>
    <% { %>
        <a category="<%=(int)item%>" class="dropDownItem"><%=item.ToLocalizedString()%></a>
    <% } %>
    </div>
</div>

<div id="socialProfileCategoriesPanel" class="dropDownDialog">
    <div class="dropDownContent">
    <% foreach (var item in ContactInfoTypes) %>
    <% { %>
        <a category="<%=(int)item%>" categoryName="<%=item.ToString()%>" class="dropDownItem">
            <%=item.ToLocalizedString()%>
        </a>
    <% } %>
    </div>
</div>

<script id="customFieldTmpl" type="text/x-jquery-tmpl">
{{if fieldType ==  0}}
    <dt class="bold">${label}</dt>
    <dd>
        <input id="custom_field_${id}" name="customField_${id}" size="${mask.size}"
                type="text" class="textEdit" maxlength="255" value="${value}" />
    </dd>
{{else fieldType ==  1}}
    <dt class="bold">${label}</dt>
    <dd>
        <textarea id="custom_field_${id}" rows="${mask.rows}" cols="${mask.cols}"
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
    <dt class="headerBase headerExpand" onclick="javascript:jq(this).next('dd:first').nextUntil('#contactProfileEdit dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
       ${label}
    </dt>
    <dd class="underHeaderBase">&nbsp;</dd>
{{else fieldType ==  5}}
   <dt class="bold">${label}</dt>
   <dd>
      <input id="custom_field_${id}" type="text" name="customField_${id}" class="textEdit textEditCalendar" value="${value}"/>
   </dd>
{{/if}}
</script>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {
        ASC.CRM.ContactActionView.init('<%= DateTimeExtension.DateMaskForJQuery %>');
        ASC.CRM.SocialMedia.init();
        ASC.CRM.SocialMedia.emptyPeopleLogo = '<%=WebImageSupplier.GetAbsoluteWebPath("empty_people_logo_40_40.png",ProductEntryPoint.ID)%>';
    });
</script>

<div id="divSMProfilesWindow" class="borderBase">
    <div class="popup-corner-left">
    </div>
    <div class="headerBaseMedium divHeader">
        <span></span>
        <img src="<%= WebImageSupplier.GetAbsoluteWebPath("cross-grey.png",ProductEntryPoint.ID) %>"
            title="<%= CRMCommonResource.CloseWindow%>" alt="<%= CRMCommonResource.CloseWindow%>"
            class="cancel_cross" onclick="jq('#divSMProfilesWindow').hide();" />
    </div>
    <div style="max-height: 200px; overflow: auto; margin: 10px;">
        <table id="sm_tbl_UserList">
        </table>
        <div class="divWait">
            <span class="textMediumDescribe">
                <%= CRMSocialMediaResource.PleaseWait%></span>
            <br />
            <img alt="<%= CRMSocialMediaResource.PleaseWait %>" title="<%= CRMSocialMediaResource.PleaseWait %>"
                src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
        </div>
        <div class="divNoProfiles">
            <%= UrlParameters.Type == "people" ? CRMSocialMediaResource.NoAccountsHasBeenFound : CRMSocialMediaResource.NoCompaniesHasBeenFound%></div>
    </div>
</div>

<script type="text/javascript">
    window.onload = function() {
        jq("[id$='saveContactButton']").bind("click", function() { ASC.CRM.SocialMedia.EnsureLinkedInAccounts(); });

    };
</script>

<asp:HiddenField runat="server" ID="_ctrlLinkedInAccountsInfo" />

<script id="TwitterProfileTmpl" type="text/x-jquery-tmpl">
<tr>
    <td class="sm_tbl_UserList_clmnBtRelate">
        <a class="ctrlRelateContactWithAccount grayLinkButton" onclick="ASC.CRM.SocialMedia.AddTwitterProfileToContact('${ScreenName}'); return false;"><%=CRMCommonResource.Add%></a>
    </td>
    <td class="sm_tbl_UserList_clmnAvatar">
        <div style="min-height: 40px;">
            <img src="${SmallImageUrl}" width="40" />
        </div>
    </td>
    <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
        <span class="headerBaseSmall sn_userName" style="color: Black !important;">${UserName}</span>
        <br />
        ${Description}
    </td>
</tr>
</script>

<script id="FacebookProfileTmpl" type="text/x-jquery-tmpl">
<tr>
    <td class="sm_tbl_UserList_clmnBtRelate">
        <a class="ctrlRelateContactWithAccount grayLinkButton" onclick="ASC.CRM.SocialMedia.AddFacebookProfileToContact('${UserID}'); return false;"><%=CRMCommonResource.Add%></a>
    </td>
    <td class="sm_tbl_UserList_clmnAvatar">
        <div style="min-height: 40px;">
            <img src="${SmallImageUrl}" width="40" />
        </div>
    </td>
    <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
        <span class="headerBaseSmall sn_userName" style="color: Black !important;">${UserName}</span>
    </td>
</tr>
</script>

<script id="LinkedInProfileTmpl" type="text/x-jquery-tmpl">
<tr>
    <td class="sm_tbl_UserList_clmnBtRelate">
        <a class="ctrlRelateContactWithAccount grayLinkButton" onclick="ASC.CRM.SocialMedia.AddLinkedInProfileToContact('${UserID}', '${CompanyName}', '${Position}', '${PublicProfileUrl}', '${UserName}'); return false;"><%=CRMCommonResource.Add%></a>
    </td>
    <td class="sm_tbl_UserList_clmnAvatar">
        <div style="min-height: 40px;">
            <img src="${ImageUrl}" width="40" />
        </div>
    </td>
    <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
        <span class="headerBaseSmall sn_userName" style="color: Black !important;">${UserName}</span>
    </td>
</tr>
</script>

