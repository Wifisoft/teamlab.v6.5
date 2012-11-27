<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactDetailsView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Contacts.ContactDetailsView" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users"
    TagPrefix="ascws" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register TagPrefix="ascws" Namespace="ASC.Web.Studio.Controls.Common" %>
<%@ Register Src="../SocialMedia/UserSearchView.ascx" TagPrefix="ctrl" TagName="UserSearchView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.CRM.Core.Entities" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<% if (Global.DebugVersion) { %>
<link href="<%= PathProvider.GetFileStaticRelativePath("tasks.css") %>" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("tasks.js") %>"></script>

<link href="<%= PathProvider.GetFileStaticRelativePath("socialmedia.css") %>" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("socialmedia.js") %>"></script>

<link href="<%= PathProvider.GetFileStaticRelativePath("deals.css") %>" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("deals.js")%>"></script>
<% } %>

<script id="collectionContainerTmpl" type="text/x-jquery-tmpl">
    <dt class="describeText">${Type}:</dt>
    <dd class="clearFix">
    </dd>
</script>

<script id="collectionTmpl" type="text/x-jquery-tmpl">
    {{if InfoType == 0 || InfoType == 3 || InfoType == 15 || InfoType == 16 ||
    InfoType == 5 || InfoType == 8 || InfoType == 9 || InfoType == 10 || InfoType == 11 || InfoType == 12 || InfoType == 13}}
        <div class="collectionItem"><span>${Data}</span><span class="textMediumDescribe"> (${CategoryName})</span></div>

    {{else InfoType == 1}}
        <div class="collectionItem"><a href="mailto:${Data}" class="linkMedium">${Data}</a><span class="textMediumDescribe"> (${CategoryName})</span></div>

    {{else InfoType == 2}}
        <div class="collectionItem"><a href="${Href}" target="_blank" class="linkMedium">${Data}</a><span class="textMediumDescribe"> (${CategoryName})</span></div>
<%--
    {{else InfoType == 2 || InfoType == 5 || InfoType == 8 || InfoType == 9 || InfoType == 10 || InfoType == 11 || InfoType == 12 || InfoType == 13}}
        <div class="collectionItem"><a href="${Data}" target="_blank">${Data}</a><span class="textMediumDescribe"> (${CategoryName})</span></div>
--%>
    {{else InfoType == 4}}
        <a class="collectionItem linkMedium" href="http://twitter.com/${Data}" target="_blank">${Data}</a><span class="textMediumDescribe"> (${CategoryName})</span><br/>

    {{else InfoType == 6}}
        <a class="collectionItem linkMedium" href="http://facebook.com/${Data}" target="_blank">${Data}</a><span class="textMediumDescribe"> (${CategoryName})</span><br/>

    {{else InfoType == 14}}
        <a class="collectionItem linkMedium" href="http://www.icq.com/people/${Data}" target="_blank">${Data}</a><span class="textMediumDescribe"> (${CategoryName})</span><br/>
    {{/if}}
</script>

<script id="addressTmpl" type="text/x-jquery-tmpl">
    <div class="collectionItem"></div>
</script>

<script id="showOnMapAndAddressTypeTmpl" type="text/x-jquery-tmpl">
    <span class="textMediumDescribe"> (${category})</span><br/>
    <a style="text-decoration: underline;" href="http://maps.google.com/maps?q=${value}" target="_blank" class="linkMedium">
        <%= CRMContactResource.ShowOnMap %>
    </a>
</script>

<script id="customFieldTmpl" type="text/x-jquery-tmpl">
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
        <dt class="headerBase headerExpand" onclick="javascript:jq(this).next('dd:first').nextUntil('#contactProfile dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
             ${label}
        </dt>
        <dd class="underHeaderBase clearFix">&nbsp;</dd>
    {{/if}}
</script>

<script id="SocialMediaAvatarTmpl" type="text/x-jquery-tmpl">
    <div class="ImageHolderOuter" onclick="ASC.CRM.SocialMedia.SaveUserAvatar(event,'${SocialNetwork}','${Identity}');">
        <img src="${ImageUrl}" alt="Avatar" class="AvatarImage" />
    </div>
</script>

<script type="text/javascript" language="javascript">

    jq(document).ready(function() {

        ASC.CRM.ContactDetailsView.init();
        ASC.CRM.SocialMedia.init();

         //----Attachments------//
        ASC.Controls.AnchorController.bind(/files/, Attachments.loadFiles);

        Attachments.bind("addFile", function(ev, file) {
            //ASC.CRM.Common.changeCountInTab("add", "files");
            var contactID = jq.getURLParam("id") * 1;
            var type = "contact";
            var fileids = [];
            fileids.push(file.id);

            Teamlab.addCrmEntityFiles({}, contactID, type, {
                entityid: contactID,
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
            else{
                Teamlab.removeCrmEntityFiles({ fileId: fileId }, fileId, {
                    success: function (params) {
                        Attachments.deleteFileFromLayout(params.fileId);
                        //ASC.CRM.Common.changeCountInTab("delete", "files");
                    }
                });

            }
        });
        //-----end Attachments

        ASC.Controls.AnchorController.bind(/history/, ASC.CRM.HistoryView.activate);
        ASC.Controls.AnchorController.bind(null, ASC.CRM.HistoryView.activate);
        ASC.Controls.AnchorController.bind(/tasks/, ASC.CRM.ListTaskView.activate);

        ASC.CRM.ListContactView.isContentRendered = false;
        ASC.Controls.AnchorController.bind(/contacts/, function(){
            if (ASC.CRM.ListContactView.isContentRendered == false) {
                ASC.CRM.ListContactView.isContentRendered = true;
                ASC.CRM.ListContactView.renderSimpleContent();
            }
        });

        ASC.Controls.AnchorController.bind(/twitter/, ASC.CRM.SocialMedia.activate);
        ASC.Controls.AnchorController.bind(/deals/, ASC.CRM.ListDealView.activate);

        <% if(!ShowEventLinkToPanel) %>
        <% { %>
        jq("#eventLinkToPanel").hide();
        <% } %>
    });
</script>

<% if (TargetContact is Person) %>
<% { %>
    <input type="hidden" name="baseInfo_firstName" value="<%= ((Person)TargetContact).FirstName.HtmlEncode() %>" />
    <input type="hidden" name="baseInfo_lastName" value="<%= ((Person)TargetContact).LastName.HtmlEncode() %>" />
<% } else {%>
    <input type="hidden" name="baseInfo_companyName" value="<%= ((Company)TargetContact).CompanyName.HtmlEncode() %>" />
<% } %>

<div class="clearFix" id="contactProfile">
    <a class="linkEditButton" style="margin-bottom: 10px; float: left;" href="<%= String.Format("default.aspx?id={0}&action=manage{1}",
            TargetContact.ID, (TargetContact is Company) ? string.Empty : "&type=people") %>">
        <%= (TargetContact is Company) ? CRMContactResource.EditProfileCompany : CRMContactResource.EditProfilePerson %>
    </a>
    <% if (!MobileVer) %>
    <% { %>
    <a class="merge_button baseLinkAction" onclick="ASC.CRM.ContactDetailsView.showMergePanel('<%= (TargetContact is Company).ToString().ToLower() %>', <%= ContactsToMerge.Count %>);">
        <%= CRMContactResource.MergeButtonText %>
    </a>
    <% } %>
    <div class="contactInfo">
        <dl id="generalList">
            <dt class="headerBase headerExpand" onclick="javascript:jq(this).next('dd:first').nextUntil('#contactProfile dt.headerBase').toggle(); jq(this).toggleClass('headerExpand'); jq(this).toggleClass('headerCollapse');">
                <%=CRMContactResource.GeneralInformation%>
            </dt>
            <dd class="underHeaderBase clearFix">&nbsp;</dd>

            <% if (TargetContact is Person) %>
            <% { %>
                <% if (!String.IsNullOrEmpty(((Person)TargetContact).JobTitle)) %>
                <% { %>
                <dt class="describeText"><%= CRMContactResource.PersonPosition %>:</dt>
                <dd class="clearFix"><%=((Person)TargetContact).JobTitle.HtmlEncode()%></dd>
                <% } %>
            <% } %>

            <% if (TargetContact is Person && ((Person)TargetContact).CompanyID != 0)%>
            <% { %>
                <dt class="describeText"><%= CRMContactResource.CompanyName%>:</dt>
                <dd class="clearFix">
                    <%= Global.DaoFactory.GetContactDao().GetByID(((Person)TargetContact).CompanyID).RenderLinkForCard() %>
                </dd>
            <% } %>

            <% if (!String.IsNullOrEmpty(TargetContact.Industry)) %>
            <% { %>
                <dt class="describeText"><%= CRMContactResource.Industry %>:</dt>
                <dd class="clearFix"><%= TargetContact.Industry.HtmlEncode()%></dd>
            <% } %>

            <% if ((TargetContact is Company) || (TargetContact is Person && (TargetContact as Person).CompanyID == 0)) %>
            <% { %>
                <dt class="describeText"><%= CRMJSResource.Stage%>:</dt>
                <dd class="slider_dd clearFix" style="height: 15px">
                    <div id="loyaltySliderDetails">
                    </div>
                </dd>
            <% } %>


            <dt class="describeText addressContainerDetails hiddenFields"><%= ContactInfoType.Address.ToLocalizedString()%>:</dt>
            <dd class="addressContainerDetails hiddenFields clearFix">
            </dd>

            <dt class="describeText"><%= CRMContactResource.Tags %>:</dt>
            <dd class="tagContainer">
                <asp:PlaceHolder ID="phTagContainer" runat="server"></asp:PlaceHolder>
            </dd>

            <% if (!String.IsNullOrEmpty(TargetContact.About)) %>
            <% { %>
            <dt class="describeText"><%= CRMContactResource.Overview %>:</dt>
            <dd class="clearFix"><%=TargetContact.About.Trim().HtmlEncode().Replace("\n","<br/>") %></dd>
            <% } %>
        </dl>
    </div>
    <div class="additionInfo">
        <div style="display: block;">
            <img class="contact_photo" src="<%= ContactPhotoManager.GetBigSizePhoto(TargetContact.ID, TargetContact is Company) %>" />
        </div>
        <% if(!MobileVer) {%>
        <div class="under_logo">
            <div style="display: block;">
                <a id="linkChangePhoto" onclick="ASC.CRM.SocialMedia.OpenLoadPhotoWindow(); return false;" class="baseLinkAction linkMedium">
                    <%= CRMContactResource.ChangePhoto%></a>
            </div>
            <div style="display: none;" class="ajax_info_block clearFix">
                <span class="textMediumDescribe">
                    <%= CRMContactResource.LoadingPhotoProgress%>
                </span>
                <br />
                <img width="150px;" alt="<%= CRMContactResource.LoadingPhotoProgress %>" title="<%= CRMContactResource.LoadingPhotoProgress %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </div>
        <% } %>
    </div>
</div>
<br />

<div id="divLoadPhotoWindow" style="display: none;">
    <ascwc:Container ID="_ctrlLoadPhotoContainer" runat="server">
        <Header>
            <%= CRMContactResource.ChooseProfilePhoto%>
        </Header>
        <Body>
            <div id="divLoadPhotoFromPC" style="margin-top: -20px;">
                <h4>
                    <%= CRMContactResource.LoadPhotoFromPC%></h4>
                <div class="describeText" style="margin-bottom: 5px;">
                    <%=ASC.Web.Studio.Core.FileSizeComment.GetFileImageSizeNote(CRMContactResource.ContactPhotoInfo, true)%>
                </div>
                <div>
                    <input id="changeLogo" type="file" style="margin-left: 0; outline: none;" />
                </div>
            </div>
            <div id="divLoadPhotoFromSocialMedia">
                <h4>
                    <%= CRMContactResource.LoadPhotoFromSocialMedia%></h4>
                <div id="divImagesHolder">
                    <div class="ImageHolderOuter" onclick="ASC.CRM.SocialMedia.DeleteContactAvatar();">
                        <asp:Image CssClass="AvatarImage" runat="server" ID="_ctrlImgDefaultAvatarSmall" />
                    </div>
                </div>
                <div style="clear: both;">
                    <div id="divAjaxImageContainerPhotoLoad">
                    </div>
                </div>
            </div>
        </Body>
    </ascwc:Container>
</div>
<div id="mergePanel" style="display: none;">
    <ascwc:Container ID="_mergePanelPopup" runat="server">
        <Header>
            <%= CRMContactResource.MergePanelHeaderText%>
        </Header>
        <Body>
            <div class="describeText"><%= CRMContactResource.MergePanelDescriptionText%></div>
            <ul id="listContactsToMerge">
                <% foreach (Contact contact in ContactsToMerge) %>
                <% { %>
                <li>
                    <input type="radio" name="contactToMerge" value="<%= contact.ID %>" />
                    <span><%= contact.GetTitle().HtmlEncode()%></span>
                </li>
                <% } %>
                <li>
                    <input type="radio" name="contactToMerge" value="0"  <%= ContactsToMerge.Count == 0 ? "style='display: none'" : "" %> />

                    <div style="display:inline-block; vertical-align: top; width: 99%; <%= ContactsToMerge.Count == 0 ? "margin-left: -20px;" : "" %>">
                        <asp:PlaceHolder runat="server" ID="_phContactToMergeSelector"></asp:PlaceHolder>
                    </div>
                    <input type="hidden" name="selectedContactToMergeID" value="0" />

                </li>
            </ul>

            <div class="h_line"><!--– –--></div>
            <div class="action_block">
                <a class="baseLinkButton" onclick="ASC.CRM.ContactDetailsView.mergeContacts(<%= TargetContact.ID %>,
                        '<%= (TargetContact is Company).ToString().ToLower() %>');">
                    <%= CRMContactResource.MergePanelButtonStartText%></a>
                <span class="splitter">&nbsp;</span>
                <a class="grayLinkButton" onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                    <%= CRMCommonResource.Cancel %></a>
            </div>
            <div style="display: none;" class="ajax_info_block">
                <span class="textMediumDescribe">
                    <%= CRMContactResource.MergePanelProgress%>
                </span>
                <br />
                <img alt="<%=CRMContactResource.MergePanelProgress%>" title="<%=CRMContactResource.MergePanelProgress%>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </ascwc:Container>
</div>
<ascwc:ViewSwitcher ID="CompanyTabs" runat="server" RenderAllTabs="true" DisableJavascriptSwitch="true">
    <TabItems>
        <ascwc:ViewSwitcherTabItem runat="server" ID="EventsTab">
            <asp:PlaceHolder runat="server" ID="_phHistoryView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="TasksTab">
            <asp:PlaceHolder runat="server" ID="_phTasksView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="ContactsTab">
            <div id="addPeopleButton">
                <a class="linkAssignContacts baseLinkAction" onclick="javascript:jq('#addPeopleButton').hide();jq('#peopleInCompanyPanel').show();" >
                    <%=CRMContactResource.AssignContact%>
                </a>
            </div>

            <div id="peopleInCompanyPanel">
                <div class="bold" style="margin-bottom:5px;"><%= CRMContactResource.AssignPersonFromExisting%>:</div>
                <asp:PlaceHolder ID="phPeopleInCompanySelector" runat="server"></asp:PlaceHolder>
            </div>

            <asp:PlaceHolder runat="server" ID="_phPeopleInCompany"></asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="_phEmptyPeopleInCompany"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="SocialMediaTab">
            <div runat="server" id="_ctrlErrorDescriptionContainer" class="infoPanel sm_UserActivityView_ErrorDescription"
                style="display: none;">
                <div runat="server" id="_ctrlErrorDescription">
                </div>
            </div>
            <div id="divSocialMediaContent">
            </div>

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
                        <%= TargetContact is Person ? CRMSocialMediaResource.NoAccountsHasBeenFound
                                                    : CRMSocialMediaResource.NoCompaniesHasBeenFound%>
                    </div>
                </div>
            </div>

            <script id="TwitterProfileTmpl" type="text/x-jquery-tmpl">
                <tr>
                    <td class="sm_tbl_UserList_clmnBtRelate">
                        <a class="ctrlRelateContactWithAccount grayLinkButton"
                            onclick="ASC.CRM.SocialMedia.AddAndSaveTwitterProfileToContact('${ScreenName}', '<%= TargetContact.ID %>'); return false;">
                            <%=CRMCommonResource.Choose%>
                        </a>
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

        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="DealsTab">
            <asp:PlaceHolder runat="server" ID="_phDealsView"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
        <ascwc:ViewSwitcherTabItem runat="server" ID="FilesTab">
            <asp:PlaceHolder id="_phFilesView" runat="server"></asp:PlaceHolder>
        </ascwc:ViewSwitcherTabItem>
    </TabItems>
</ascwc:ViewSwitcher>

<ctrl:UserSearchView runat="server" ID="_userSearchView" />

<script type="text/javascript">
    window.onload = function() {
        var smErrorMessage = jq("input[id$='_ctrlSMErrorMessage']").val();
        if (smErrorMessage != "" && smErrorMessage !== undefined)
            ShowErrorMessage(smErrorMessage);
    }

    jq(document).ready(function() {
        if (typeof (contactSelector) != "undefined") {
            if (contactSelector.SelectedContacts.length == 0)
                jq("#emptyPeopleInCompanyPanel").show();
            else
                jq("#addPeopleButton").show();
            contactSelector.SelectItemEvent = ASC.CRM.ContactDetailsView.addPersonToCompany;
            ASC.CRM.ListContactView.removeMember = ASC.CRM.ContactDetailsView.removePersonFromCompany;
        }
    });


</script>
