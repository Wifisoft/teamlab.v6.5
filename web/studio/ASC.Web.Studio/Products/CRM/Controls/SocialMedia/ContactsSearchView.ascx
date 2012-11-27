<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactsSearchView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.SocialMedia.ContactsSearchView" EnableViewState="false" %>

<div id="divSMContactsSearchContainer" style="display: none;">
    <ascwc:Container ID="_ctrlContactsSearchContainer" runat="server">
        <Header>
            <% = CRMSocialMediaResource.ProfilesInSocialMedia %>
        </Header>
        <Body>
            <div id="divModalContent" style="max-height: 500px; overflow: auto;">
                <div id="divContactDescription">
                </div>

                <div style="display: none;" id="divCrbsContactConfirm">
                    <div class="h_line" style="margin-top: 15px;">&nbsp;</div>

                    <a href="javascript:void(0);" class="save_button baseLinkButton"
                        onclick="ASC.CRM.SocialMedia.ConfirmCrunchbaseContact(); return false;">
                            <%= CRMSocialMediaResource.Confirm %></a>
                    <span class="splitter">&nbsp;</span>
                    <a href="" class="grayLinkButton" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;">
                            <%= CRMCommonResource.Cancel %></a>

                    <div style="float:right;">
                        <span class="textMediumDescribe"><%=CRMSocialMediaResource.InformationProvidedBy %></span>
                        <a target="_blank" href="http://crunchbase.com/" class="linkHeaderLightSmall">CrunchBase</a>
                    </div>
                </div>
            </div>
            <div class="divWaitForSearching">
                <span class="textMediumDescribe"><%= CRMSocialMediaResource.PleaseWaitForSearching %></span>
                <br />
                <img alt="<%= CRMSocialMediaResource.PleaseWait %>" title="<%= CRMSocialMediaResource.PleaseWaitForSearching %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>

            <div class="divWaitForAdding">
                <span class="textMediumDescribe">
                    <%= CRMSocialMediaResource.PleaseWait %></span>
                <br />
                <img alt="<%= CRMSocialMediaResource.PleaseWait %>" title="<%= CRMSocialMediaResource.PleaseWait %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>

            <div class="divNoProfiles"><%= CRMSocialMediaResource.NoAccountsHasBeenFound %></div>

        </Body>
    </ascwc:Container>
</div>

<script id="personTmpl" type="text/x-jquery-tmpl">
    <div>
        <input type="checkbox" id="${person.permalink}" checked="checked"/> ${person.first_name} ${person.last_name}
    </div>
</script>
<script id="CrunchbaseContactFullTmpl" type="text/x-jquery-tmpl">

<div style="height:24px; vertical-align:middle;">
    {{if namespace == "company"}}
        <a class="crunhbaseLink" href="${crunchbase_url}" target="_blank">${name}</a><br>
    {{else}}
        <a class="crunhbaseLink" href="${crunchbase_url}" target="_blank">${first_name} ${last_name}</a><br>
    {{/if}}
</div>
<table id="tblCompanyFields">
    {{if homepage_url }}
        <tr>
            <td style="width:120px;" class="textBigDescribe"><input type="checkbox" id="chbWebsite" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Website.ToLocalizedString() %>:</td>
            <td><a href="${homepage_url}" target="_blank">${homepage_url}</a></td>
        </tr>
    {{/if}}
    {{if email_address }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbEmail" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Email.ToLocalizedString() %>:</td>
            <td><a href="mailto:${email_address}">${email_address}</a></td>
        </tr>
    {{/if}}
    {{if phone_number }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbPhoneNumber" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Phone.ToLocalizedString() %>:</td>
            <td>${phone_number}</td>
        </tr>
    {{/if}}
    {{if overview }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbDescription" checked="checked"/>
                <%= CRMContactResource.Overview %>:</td>
            <td>${jq(overview).text().substring(0,60)}...</td>
        </tr>
    {{/if}}
    {{if twitter_username }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbTwitter" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Twitter.ToLocalizedString() %>:</td>
            <td><a href="http://twitter.com/#!/${twitter_username}">http://twitter.com/#!/${twitter_username}</a></td>
        </tr>
    {{/if}}
    {{if blog_url }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbBlog" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Blogger.ToLocalizedString() %>:</td>
            <td><a href="${blog_url}">${blog_url}</a></td>
        </tr>
    {{/if}}
    {{if blog_feed_url }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbBlogFeed" checked="checked"/>
                <%= CRMSocialMediaResource.BlogFeed %>:</td>
            <td><a href="${blog_feed_url}">${blog_feed_url}</a></td>
        </tr>
    {{/if}}
    {{if tag_list && false}}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbTag" checked="checked"/>
                <%= CRMContactResource.Tags %>:</td>
            <td>${tag_list}</td>
        </tr>
    {{/if}}
    {{if image }}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbImage" checked="checked"/>
                <%= CRMSocialMediaResource.Image %>:</td>
            <td><img height="25" src="http://www.crunchbase.com/${image.available_sizes[0][1]}"></td>
        </tr>
    {{/if}}
    {{if namespace == "company" && relationships && relationships.length != 0}}
        <tr>
            <td class="textBigDescribe"><input type="checkbox" id="chbRelationship" checked="checked"
                onclick="ASC.CRM.SocialMedia.switchCheckedPersonsInCompany(jq(this).is(':checked'));"/>
                <%= CRMContactResource.Persons %> (${relationships.length}):</td>
            <td id="chbPersonsRelationship">
                <div style="max-height: 100px; overflow: auto;" class="borderBase">
                    {{each relationships}}
                        {{tmpl($value) '#personTmpl'}}
                    {{/each}}
                </div>
                <div class="textMediumDescribe">
                    *<%= CRMContactResource.CrunchBaseWatermarkText %>
                </div>
            </td>
        </tr>
    {{/if}}
</table>

<input id="crbsWebSite" type="hidden" value="${homepage_url}" />
<input id="crbsEmail" type="hidden" value="${email_address}" />
<input id="crbsPhoneNumber" type="hidden" value="${phone_number}">
<input id="crbsOverview" type="hidden" value="${jq(overview).text()}" />
<input id="crbsTwitterUserName" type="hidden" value="${twitter_username}" />
<input id="crbsBlogUrl" type="hidden" value="${blog_url}" />
<input id="crbsBlogFeedUrl" type="hidden" value="${blog_feed_url}" />
<input id="crbsTagList" type="hidden" value="${tag_list}" />
<input id="crbsPeopleJSON" type="hidden" value="${jq.toJSON(relationships)}" />
<input id="crbsImageJSON" type="hidden" value="${jq.toJSON(image)}" />

</script>

