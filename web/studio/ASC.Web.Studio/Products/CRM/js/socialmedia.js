﻿if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = function() { return {} };


function ddlMessageNumberClicked(event) {
    var number = jq("select[id$='_ctrlMessageCount']").val();
    jq.cookies.set("sm_msg_count", number);
    ASC.CRM.SocialMedia.LoadContactActivity();
};

function ShowErrorMessage(text) {
    jq("[id$='_ctrlErrorDescriptionContainer']").css("display", "block");
    jq("[id$='_ctrlErrorDescription']").text(text);
};

ASC.CRM.SocialMedia = (function($) {

    var CallbackMethods = {
        addAndSaveTwitter: function(params, twitter) {
            ASC.CRM.SocialMedia.LoadContactActivity();
        }
    };
    Teamlab.bind(Teamlab.events.getException, onGetException);

    function onGetException(params, errors) {
        console.log('socialmedia.js ', errors);
        LoadingBanner.hideLoading();
    };

    _AjaxProTimeout = function() {
        LoadingBanner.hideLoading();
        ShowErrorMessage("Timeout expired");
    };

    _FindUsersResponse = function(response) {
        _HideAjaxLoader();
        if (response.error != null)
            _ShowErrorAccountRelation(response.error.Message);
        else
            jq("[id$='divSearchResults']").html(response.value);
    };

    _ShowUserRelationWindowResponse = function(response) {
        _HideAjaxLoader();
        if (response.error != null)
            _ShowErrorAccountRelation(response.error.Message);
        else {
            jq("[id$='divSearchContent']").css("display", "none");
            jq("[id$='divUserContent']").html(response.value).css("display", "block");
        }
    };

    _SaveContactSocialMediaRelationResponse = function(response) {
        _HideAjaxLoader();
        if (response.error != null)
            _ShowErrorAccountRelation(response.error.Message);
        else {
            PopupKeyUpActionProvider.CloseDialog();
            location.reload(true);
        }
    };

    _ShowErrorAccountRelation = function(errorMessage) {
        jq("[id$='_ctrlUserSearchViewErrorContainer']").css("display", "block");
        jq("[id$='_ctrlUserSearchViewErrorDescription']").text(errorMessage);
    };

    _HideErrorAccountRelation = function() {
        jq("[id$='_ctrlUserSearchViewErrorContainer']").css("display", "none");
        jq("[id$='_ctrlUserSearchViewErrorDescription']").text("");
    };

    _ShowAjaxLoader = function() {
        var height = jq("#divModalContent").height() + 20;
        var width = jq("#divModalContent").width() + 20;
        var position = jq("#divModalContent").offset();
        jq("#divAjaxCloserBox").height(height).width(width).css("top", position.top - 10).css("left", position.left - 10).fadeTo("fast", 0.1);
    };

    _HideAjaxLoader = function() {
        jq("#divAjaxCloserBox").fadeOut("fast");
    };

    _GetContactActivityResponse = function(response) {
        LoadingBanner.hideLoading();
        if (response.error != null)
            ShowErrorMessage(response.error.Message);
        else
            jq("#divSocialMediaContent").html(response.value);
    };

    _GetContactSMImagesResponse = function(response) {
        if (response.error == null) {
            var result = jq.parseJSON(response.value);
            var imageCount = result.length;
            if (imageCount > 0) {
                for (var i = 0; i < imageCount; i++) {
                    jq("#SocialMediaAvatarTmpl").tmpl(result[i]).appendTo("#divImagesHolder");
                    jq("#linkSaveAvatar").css("display", "inline");
                }
            }
        }
        _FinishGettingContactImages();
    };

    _FinishGettingContactImages = function() {
        ASC.CRM.SocialMedia.ContactImageListLoaded = true;
        jq("#divAjaxImageContainerPhotoLoad [id$='_ctrlImgAjaxLoader']").remove();
        jq("#divLoadPhotoFromSocialMedia").css("height", "100px");
        jq("#divImagesHolder").css("display", "block");
    };

    _SaveUserAvatarFromSocialNetworkResponse = function(response) {
        jq(".under_logo .ajax_info_block").hide();
        jq(".under_logo #linkChangePhoto").show();
        if (response.error != null || response.value == null)
            alert(CRMJSResources.ErrorMessage_SaveImageError);
        else {
            var now = new Date();
            jq("#contactProfile .additionInfo .contact_photo").attr("src", response.value + '?' + now.getTime());
        }
    };

    _SavingAvatarTimeout = function() {
        jq(".under_logo .ajax_info_block").hide();
        jq(".under_logo #linkChangePhoto").show();
        alert(CRMJSResources.ErrorMessage_SaveImageError);
    };

    _DeleteContactAvatarResponse = function(response) {
        jq(".under_logo .ajax_info_block").hide();
        jq(".under_logo #linkChangePhoto").show();
        if (response.error != null)
            alert(CRMJSResources.ErrorMessage_SaveImageError);
        else {
            var now = new Date();
            jq("#contactProfile .additionInfo .contact_photo").attr("src", response.value + '?' + now.getTime());
        }
    };

    _FindTwitterProfilesResponse = function(target, addTop, addLeft, response) {
        if (response.error != null) {
            alert(response.error.Message);
            return;
        }
        _RenderSMProfiles(jq.parseJSON(response.value), "TwitterProfileTmpl");
        jq("#divSMProfilesWindow .divWait").hide();
        _CalculateProfilesWindowHeight();

        ASC.CRM.SocialMedia.TwitterTargetTextbox = jq(target).parent().parent().children('table').find('input');
        _ShowProfilesWindow();
    };

    _FindFacebookProfilesResponse = function(target, addTop, addLeft, response) {
        if (response.error != null) {
            alert(response.error.Message);
            return;
        }

        _RenderSMProfiles(jq.parseJSON(response.value), "FacebookProfileTmpl");
        jq("#divSMProfilesWindow .divWait").hide();
        _CalculateProfilesWindowHeight();

        ASC.CRM.SocialMedia.FacebookTargetTextbox = jq(target).parent().parent().children('table').find('input');
        _ShowProfilesWindow();
    };

    _FindLinkedInProfilesResponse = function(target, addTop, addLeft, response) {
        if (response.error != null) {
            alert(response.error.Message);
            return;
        }
        _RenderSMProfiles(jq.parseJSON(response.value), "LinkedInProfileTmpl");
        jq("#divSMProfilesWindow .divWait").hide();
        _CalculateProfilesWindowHeight();

        ASC.CRM.SocialMedia.LinkedInTargetTextbox = jq(target).parent().parent().children('table').find('input');
        _ShowProfilesWindow();
    };

    _CalculateProfilesWindowHeight = function() {
        var height = jq("#sm_tbl_UserList").outerHeight();
        if (height > 200)
            return 270;
        if (height == 0)
            return 100;
        return height + 65;
    };

    _CalculateProfilesWindowPosition = function(targetElement, addTop, addLeft) {
        var top = addTop == null ? 0 : addTop;
        var left = addLeft == null ? 0 : addLeft;

        var dropdownItem = jq("#divSMProfilesWindow");

        var targetPos = targetElement.offset();
        var elemPosTop = targetPos.top - 23 + top;
        var elemPosLeft = targetPos.left + targetElement.outerWidth() + 3 + left;

        dropdownItem.css(
            {
                'position': 'absolute',
                'top': elemPosTop,
                'left': elemPosLeft
            });
    };

    _RenderSMProfiles = function(profiles, template) {
        var profileCount = profiles.length;
        if (profileCount > 0) {
            for (var i = 0; i < profileCount; i++) {
                jq("#" + template + "").tmpl(profiles[i]).appendTo("#sm_tbl_UserList");
            }
        }
        else
            jq("#divSMProfilesWindow .divNoProfiles").css("display", "block");
    };

    _AddTwitterProfileToContactResponse = function(response) {
        if (response.error != null) alert("Error");
        _HideProfilesWindow();
    };

    _ShowProfilesWindow = function() {
        jq("#divSMProfilesWindow").show();
        var windowHeight = _CalculateProfilesWindowHeight();
        jq("#divSMProfilesWindow").css("height", windowHeight);

        jq("#divSMProfilesWindow").unbind("mouseenter").mouseenter(function() {
            ASC.CRM.SocialMedia.MouseInProfilesWindow = true;
        });
        jq("#divSMProfilesWindow").unbind("mouseleave").mouseleave(function() {
            ASC.CRM.SocialMedia.MouseInProfilesWindow = false;
        });
        jq(document).bind("click", _CheckProfilesWindow);
    };

    _ShowWaitProfilesWindow = function(name) {
        jq("#divSMProfilesWindow .divWait").show();
        jq("#divSMProfilesWindow .divHeader span").text(jq.format(CRMJSResources.PossibleSocialMediaAccounts, name));

        var windowHeight = _CalculateProfilesWindowHeight();
        jq("#divSMProfilesWindow").css("height", windowHeight);
        jq("#divSMProfilesWindow").show();
    };

    _CheckProfilesWindow = function() {
        if (ASC.CRM.SocialMedia.MouseInProfilesWindow == false) {
            _HideProfilesWindow();
        }
    };

    _HideProfilesWindow = function() {
        jq("#divSMProfilesWindow").hide();
        jq(document).unbind("click", _CheckProfilesWindow);
    };

    _ShowCrunchbaseContact = function(contactNamespace, result) {
        var resObj = jq.parseJSON(result);
        resObj.namespace = contactNamespace;
        jq("#divSMContactsSearchContainer .divWaitForSearching").hide();
        jq("#divSMContactsSearchContainer .divNoProfiles").css("display", "none");

        jq("#divContactDescription").css("display", "block");
        jq("#divCrbsContactConfirm").css("display", "block");
        jq("#divContactDescription").html("");

        jq("#CrunchbaseContactFullTmpl").tmpl(resObj).appendTo("#divContactDescription");
        if (jq("#tblCompanyFields > tbody > tr").length == 0) {
            jq("#divContactDescription").html("");
            jq("#divCrbsContactConfirm").css("display", "none");
            jq("#divSMContactsSearchContainer .divNoProfiles").css("display", "block");
        }
    };

    return {
        init: function() {
            ASC.CRM.SocialMedia.SocialMediaLoaded = false;
            ASC.CRM.SocialMedia.ContactImageListLoaded = false;
            ASC.CRM.SocialMedia.MouseInProfilesWindow = false;
            ASC.CRM.SocialMedia.selectedPersons = new Array();
        },

        activate: function() {
            if (ASC.CRM.SocialMedia.SocialMediaLoaded == false) {
                ASC.CRM.SocialMedia.SocialMediaLoaded = true;
                ASC.CRM.SocialMedia.LoadContactActivity();
            }
        },
        switchCheckedPersonsInCompany: function(checked) {
            jq("#chbPersonsRelationship input[type='checkbox']").attr("checked", checked);
        },

        LoadContactActivity: function() {
            AjaxPro.onTimeout = _AjaxProTimeout;
            jq("[id$='_ctrlErrorDescriptionContainer']").css("display", "none");
            jq("[id$='_ctrlErrorDescription']").text("");
            LoadingBanner.displayLoading();
            var contactID = jq("[id$='_ctrlContactID']").val();
            var number = jq.cookies.get("sm_msg_count")
            if (number == null || number === undefined || isNaN(number))
                number = 10;
            AjaxPro.SocialMediaUI.GetContactActivity(contactID, number, _GetContactActivityResponse);
        },

        ShowContactSearchPanel: function(searchText) {
            _HideErrorAccountRelation();
            ASC.CRM.Common.blockUI("#divSMUserSearchContainer", 500, 550, 0);
            jq("[id$='divUserContent']").css("display", "none");
            jq("[id$='divSearchContent']").css("display", "block");
            if (jq("#_ctrlSocialMediaSearch").val().length == 0)
                jq("#_ctrlSocialMediaSearch").val(searchText);
        },

        FindUsers: function() {
            AjaxPro.onTimeout = function() { _ShowErrorAccountRelation("Operation timeout"); _HideAjaxLoader(); }
            _HideErrorAccountRelation();
            var searchText = jq("[id$='_ctrlSocialMediaSearch']").val();
            var socialNetwork = jq("input[name='SelectedSocialMedia']:checked").attr("value");
            if (searchText.length == 0)
                return;
            _ShowAjaxLoader();
            AjaxPro.SocialMediaUI.FindUsers(searchText, socialNetwork, _FindUsersResponse);
        },

        ShowAccountRelationPanel: function(accountID, socialNetwork) {
            AjaxPro.onTimeout = function() { _ShowErrorAccountRelation("Operation timeout"); _HideAjaxLoader(); }
            _HideErrorAccountRelation();
            _ShowAjaxLoader();
            if (socialNetwork == "twitter")
                AjaxPro.SocialMediaUI.ShowTwitterUserRelationWindow(accountID, _ShowUserRelationWindowResponse);
            if (socialNetwork == "facebook")
                AjaxPro.SocialMediaUI.ShowFacebookUserRelationWindow(accountID, _ShowUserRelationWindowResponse);
        },

        RelateTwitterContactToSocialMedia: function() {
            AjaxPro.onTimeout = function() { _ShowErrorAccountRelation("Operation timeout"); _HideAjaxLoader(); }
            _HideErrorAccountRelation();
            var settings = {};
            settings.ContactID = jq("[id$='_ctrlContactID']").val();
            settings.TwitterUserID = jq("[id$='_ctrlHiddenTwitterUserID']").val();
            settings.TwitterScreenName = jq("[id$='_ctrlHiddenTwitterUserScreenName']").val();
            settings.UserAvatarUrl = jq("[id$='_ctrlHiddenUserAvatarUrl']").val();
            settings.RelateAccount = jq("[id$='_ctrlChbRelateToAccount']").is(":checked");
            settings.RelateAvatar = jq("[id$='_ctrlChbAddImage']").is(":checked")
            _ShowAjaxLoader();
            AjaxPro.SocialMediaUI.SaveContactTwitterRelation(settings, _SaveContactSocialMediaRelationResponse);
        },

        RelateFacebookContactToSocialMedia: function() {
            AjaxPro.onTimeout = function() { _ShowErrorAccountRelation("Operation timeout"); _HideAjaxLoader(); }
            _HideErrorAccountRelation();
            var settings = {};
            settings.ContactID = jq("[id$='_ctrlContactID']").val();
            settings.FacebookUserID = jq("[id$='_ctrlHiddenTwitterUserID']").val();
            settings.UserAvatarUrl = jq("[id$='_ctrlHiddenUserAvatarUrl']").val();
            settings.RelateAvatar = jq("[id$='_ctrlChbAddImage']").is(":checked")
            _ShowAjaxLoader();
            AjaxPro.SocialMediaUI.SaveContactFacebookRelation(settings, _SaveContactSocialMediaRelationResponse);
        },

        GetContactImageList: function() {
            var contactID = jq("input[id$='_ctrlContactID']").val();

            jq("#divAjaxImageContainerPhotoLoad").append(jq("[id$='_ctrlImgAjaxLoader']").clone().css("display", "block"));
            jq("#divAjaxImageContainerPhotoLoad").css("display", "block");

            AjaxPro.onTimeout = function() { _FinishGettingContactImages(); }
            AjaxPro.SocialMediaUI.GetContactSMImages(contactID, _GetContactSMImagesResponse);
        },

        OpenLoadPhotoWindow: function() {
            ASC.CRM.Common.blockUI("#divLoadPhotoWindow", 520, 550, 0);
            jq("[name='chbSocialNetwork']").removeAttr("checked");
            if (ASC.CRM.SocialMedia.ContactImageListLoaded == false)
                ASC.CRM.SocialMedia.GetContactImageList();
        },

        SelectUserAvatar: function(event, socialNetwork, identity) {
            if (!event.target) { event.target = event.srcElement; }
            var imageChecked = jq(event.target).is(":checked");
            if (imageChecked == false) return;
            jq("[name='chbSocialNetwork']").not(jq(event.target)).removeAttr("checked");
        },

        SaveUserAvatar: function(event, socialNetwork, identity) {
            AjaxPro.onTimeout = function() { _SavingAvatarTimeout(); }

            var contactId = jq("[id$='_ctrlContactID']").val();
            AjaxPro.SocialMediaUI.SaveUserAvatarFromSocialNetwork(contactId, socialNetwork, identity, _SaveUserAvatarFromSocialNetworkResponse);
            PopupKeyUpActionProvider.CloseDialog();
            jq(".under_logo #linkChangePhoto").hide();
            jq(".under_logo .ajax_info_block").show();
        },

        DeleteContactAvatar: function() {
            AjaxPro.onTimeout = function() { _SavingAvatarTimeout(); }
            var contactId = jq("[id$='_ctrlContactID']").val();
            AjaxPro.SocialMediaUI.DeleteContactAvatar(contactId, _DeleteContactAvatarResponse);
            PopupKeyUpActionProvider.CloseDialog();
            jq(".under_logo .ajax_info_block").hide();
            jq(".under_logo #linkChangePhoto").show();
        },

        FindTwitterProfiles: function(target, contactType, addTop, addLeft) {
            _HideProfilesWindow();
            jq("#divSMProfilesWindow .divNoProfiles").css("display", "none");
            jq("#sm_tbl_UserList").html("");

            var searchText;

            //contact type can be "company" or "people"
            if (contactType == "company")
                searchText = jq("[name='baseInfo_companyName']").val();

            if (contactType == "people")
                searchText = jq("[name='baseInfo_firstName']").val() + " " + jq("[name='baseInfo_lastName']").val();

            if (searchText === undefined || jq.trim(searchText).length == 0)
                return;

            _CalculateProfilesWindowPosition(jq(target), addTop, addLeft);
            _ShowWaitProfilesWindow(searchText);
            AjaxPro.SocialMediaUI.FindTwitterProfiles(searchText, function(response) { _FindTwitterProfilesResponse(target, addTop, addLeft, response); });
        },

        FindFacebookProfiles: function(target, contactType, addTop, addLeft) {
            _HideProfilesWindow();
            jq("#divSMProfilesWindow .divNoProfiles").css("display", "none");
            jq("#sm_tbl_UserList").html("");

            var searchText;

            //contact type can be "company" or "people"
            if (contactType == "company")
                searchText = jq("[name='baseInfo_companyName']").val();

            if (contactType == "people")
                searchText = jq("[name='baseInfo_firstName']").val() + " " + jq("[name='baseInfo_lastName']").val();

            if (searchText === undefined || jq.trim(searchText).length == 0)
                return;

            _CalculateProfilesWindowPosition(jq(target), addTop, addLeft);
            _ShowWaitProfilesWindow(searchText);
            AjaxPro.SocialMediaUI.FindFacebookProfiles(searchText, function(response) { _FindFacebookProfilesResponse(target, addTop, addLeft, response); });
        },

        FindLinkedInProfiles: function(target, contactType, addTop, addLeft) {
            _HideProfilesWindow();
            jq("#divSMProfilesWindow .divNoProfiles").css("display", "none");
            jq("#sm_tbl_UserList").html("");

            var searchText;
            //contact type can be "company" or "person"
            if (contactType == "company")
                return;
            if (contactType == "people") {
                var firstName = jq("[name='baseInfo_firstName']").val();
                var lastName = jq("[name='baseInfo_lastName']").val();

                var searchText = firstName + " " + lastName;
                if (searchText === undefined || jq.trim(searchText).length == 0)
                    return;

                _CalculateProfilesWindowPosition(jq(target), addTop, addLeft);
                _ShowWaitProfilesWindow(searchText);
                AjaxPro.SocialMediaUI.FindLinkedInProfiles(firstName, lastName, function(response) { _FindLinkedInProfilesResponse(target, addTop, addLeft, response); });
            }
        },

        AddAndSaveTwitterProfileToContact: function(twitterScreenName, contactID) {
            var params = {};
            var data = { data: twitterScreenName, isPrimary: true, infoType: "Twitter", category: "Work" };
            Teamlab.addCrmContactTwitter(params, contactID, data,
            {
                success: CallbackMethods.addAndSaveTwitter,
                before: function(params) { },
                after: function(params) { _HideProfilesWindow(); }
            });
        },

        AddTwitterProfileToContact: function(twitterScreenName) {
            jq(ASC.CRM.SocialMedia.TwitterTargetTextbox).val(twitterScreenName);
            _HideProfilesWindow();
            //AjaxPro.SocialMediaUI.AddTwitterProfileToContact(contactID, twitterScreenName, this._AddTwitterProfileToContactResponse);
        },

        AddFacebookProfileToContact: function(profileId) {
            jq(ASC.CRM.SocialMedia.FacebookTargetTextbox).val(profileId);
            _HideProfilesWindow();
        },

        AddLinkedInProfileToContact: function(profileId, companyName, position, publicProfileUrl, userName) {
            var linkedInAccounts = jq.parseJSON(jq("[id$='_ctrlLinkedInAccountsInfo']").val());

            var linkedInAccountsLength = 0;
            if (linkedInAccounts != null)
                linkedInAccountsLength = linkedInAccounts.length;
            else
                linkedInAccounts = new Array();

            linkedInAccounts[linkedInAccountsLength] =
        {
            UserID: profileId,
            PublicProfileUrl: publicProfileUrl,
            UserName: userName
        };

            jq("[id$='_ctrlLinkedInAccountsInfo']").val(jq.toJSON(linkedInAccounts));

            if (jq.trim(jq("[name='baseInfo_companyName']").val()).length == 0)
                jq("[name='baseInfo_companyName']").val(companyName);

            if (jq.trim(jq("[name='baseInfo_personPosition']").val()).length == 0)
                jq("[name='baseInfo_personPosition']").val(position);

            jq(ASC.CRM.SocialMedia.LinkedInTargetTextbox).val(publicProfileUrl);

            _HideProfilesWindow();
            ASC.CRM.SocialMedia.EnsureLinkedInAccounts();
        },

        EnsureLinkedInAccounts: function() {

            var existentAccountList = jq.parseJSON(jq("[id$='_ctrlLinkedInAccountsInfo']").val());
            if (existentAccountList == null) {
                jq("[id$='_ctrlLinkedInAccountsInfo']").val("");
                return;
            }
            var publicProfileUrlList = new Array();
            jq("[name='contactLinkedIn']").each(function(i) {
                publicProfileUrlList[i] = jq(this).val();
            });

            var newAccountList = new Array();
            for (var i = 0; i < existentAccountList.length; i++) {
                if (publicProfileUrlList.join().search(existentAccountList[i].PublicProfileUrl) != -1 && jq.toJSON(newAccountList).search(existentAccountList[i].PublicProfileUrl) == -1) {
                    newAccountList.push(existentAccountList[i]);
                }
            }
            jq("[id$='_ctrlLinkedInAccountsInfo']").val(jq.toJSON(newAccountList));
        },

        FindContacts: function(isCompany) {
            jq("#divSMContactsSearchContainer .divNoProfiles").hide();
            jq("#divSMContactsSearchContainer .divWaitForAdding").hide();

            var searchUrl = "";
            if (isCompany) {
                var name = jq("[name='baseInfo_companyName']").val().trim();
                if (name == "") return;
                searchUrl = "http://api.crunchbase.com/v/1/companies/permalink?name=" + name;
            }
            else {
                var first_name = jq("[name='baseInfo_firstName']").val().trim();
                var last_name = jq("[name='baseInfo_lastName']").val().trim();
                if (first_name == "" || last_name == "") return;
                searchUrl = "http://api.crunchbase.com/v/1/people/permalink?first_name=" + first_name + "&last_name=" + last_name;
            }

            var contactNamespace = isCompany ? "company" : "person";
            ASC.CRM.Common.blockUI("#divSMContactsSearchContainer", 550, 550, 0);

            jq("#divContactDescription").css("display", "none");
            jq("#divCrbsContactConfirm").css("display", "none");
            jq("#divSMContactsSearchContainer .divWaitForSearching").show();

            AjaxPro.ContactsSearchView.FindContactByName(searchUrl, contactNamespace, function(response) {
                if (response.error != null || response.value == "") {
                    jq("#divSMContactsSearchContainer .divWaitForSearching").hide();
                    jq("#divSMContactsSearchContainer .divNoProfiles").css("display", "block");
                }
                else
                    _ShowCrunchbaseContact(contactNamespace, response.value)
            });
        },

        ConfirmCrunchbaseContact: function() {
            jq("#divCrbsContactConfirm").hide();
            jq("#divSMContactsSearchContainer .divWaitForAdding").show();

            if (jq("#chbWebsite").is(":checked")) {
                var newValue = jq("#crbsWebSite").val();
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication("websiteAndSocialProfilesContainer", newValue);
                ASC.CRM.ContactActionView.changeSocialProfileCategory(
                    $newContact.find('a.social_profile_type'),
                    2,
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="2"]').text(),
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="2"]').attr('categoryName')
                );
                $newContact.insertAfter(jq("#websiteAndSocialProfilesContainer").children('div:last')).show();
            }

            if (jq("#chbEmail").is(":checked")) {
                var newValue = jq("#crbsEmail").val();
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication("emailContainer", newValue, jq("#emailContainer").find(".primary_field").length == 0);
                $newContact.insertAfter(jq("#emailContainer").children('div:last')).show();
                jq('#emailContainer').prev('dt').removeClass('crm-withGrayPlus');
            }

            if (jq("#chbPhoneNumber").is(":checked")) {
                var newValue = jq("#crbsPhoneNumber").val();
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication("phoneContainer", newValue, jq("#phoneContainer").find(".primary_field").length == 0);
                $newContact.insertAfter(jq("#phoneContainer").children('div:last')).show();
                jq('#phoneContainer').prev('dt').removeClass('crm-withGrayPlus');
            }

            if (jq("#chbTwitter").is(":checked")) {
                var newValue = jq("#crbsTwitterUserName").val();
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication("websiteAndSocialProfilesContainer", newValue);
                $newContact.insertAfter(jq("#websiteAndSocialProfilesContainer").children('div:last')).show();
                ASC.CRM.ContactActionView.changeSocialProfileCategory(
                    $newContact.find('a.social_profile_type'),
                    4,
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="4"]').text(),
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="4"]').attr('categoryName')
                );
            }

            if (jq("#chbBlog").is(":checked")) {
                var newValue = jq("#crbsBlogUrl").val();
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication("websiteAndSocialProfilesContainer", newValue);
                ASC.CRM.ContactActionView.changeSocialProfileCategory(
                    $newContact.find('a.social_profile_type'),
                    11,
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="11"]').text(),
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="11"]').attr('categoryName')
                );
                $newContact.insertAfter(jq("#websiteAndSocialProfilesContainer").children('div:last')).show();
            }
            if (jq("#chbBlogFeed").is(":checked")) {
                var newValue = jq("#crbsBlogFeedUrl").val();
                var $newContact = ASC.CRM.ContactActionView.createNewCommunication("websiteAndSocialProfilesContainer", newValue);
                ASC.CRM.ContactActionView.changeSocialProfileCategory(
                    $newContact.find('a.social_profile_type'),
                    11,
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="11"]').text(),
                    jq("#socialProfileCategoriesPanel div.dropDownContent").children('a[category="11"]').attr('categoryName')
                );
                $newContact.insertAfter(jq("#websiteAndSocialProfilesContainer").children('div:last')).show();
            }

            if (jq("#chbDescription").is(":checked")) {
                var newValue = jq("#crbsOverview").val();
                if (jq("#overviewContainer").children("div").length == 1) {
                    var $newContact = ASC.CRM.ContactActionView.createNewCommunication("overviewContainer", newValue);
                    $newContact.insertAfter(jq("#overviewContainer").children('div:last')).show();
                }
                else {
                    jq("#overviewContainer").children("div:last").find("[name='baseInfo_contactOverview']").val(newValue);
                }
                jq('#overviewContainer').prev('dt').removeClass('crm-withGrayPlus');
            }


            var add_new_button_class = "crm-addNewLink";
            var delete_button_class = "crm-deleteLink";
            jq('#emailContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#emailContainer').children('div:last').find("." + add_new_button_class).show();

            jq('#phoneContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#phoneContainer').children('div:last').find("." + add_new_button_class).show();

            if (jq('#websiteAndSocialProfilesContainer').children('div').length > 1) jq('#websiteAndSocialProfilesContainer').prev('dt').removeClass('crm-withGrayPlus');
            jq('#websiteAndSocialProfilesContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#websiteAndSocialProfilesContainer').children('div:last').find("." + add_new_button_class).show();


            if (jq("#chbImage").is(":checked")) {
                var imageObj = jq.parseJSON(jq("#crbsImageJSON").val()).available_sizes;

                var imgSrc = "http://www.crunchbase.com/" + imageObj[imageObj.length - 1][1];

                jq("#uploadPhotoPath").val(imgSrc);
                jq("#contactPhoto img").attr("src", imgSrc);
            }

            var relationshipObj = jq.parseJSON(jq("#crbsPeopleJSON").val());
            var data = new Array();
            for (var i = 0; i < relationshipObj.length; i++)
                if(relationshipObj[i].person)
                    if(jq("#chbPersonsRelationship input[id="+relationshipObj[i].person.permalink+"]").is(":checked")) {
                        var person =
                            {
                                Key: relationshipObj[i].person.first_name,
                                Value: relationshipObj[i].person.last_name,
                                canEdit: true,
                                displayName: relationshipObj[i].person.first_name + " " + relationshipObj[i].person.last_name,
                                id: relationshipObj[i].person.permalink,
                                isCompany: false,
                                isPrivate: false,
                                smallFotoUrl: ASC.CRM.SocialMedia.emptyPeopleLogo
                             };
                        ASC.CRM.SocialMedia.selectedPersons.push(person);
                        data.push(person);
                    }

            if(data.length>0) {
                jq("#simpleContactTmpl").tmpl(data).prependTo("#contactTable tbody");
                if (jq("#contactTable tr").length > 0)
                    jq("#contactListBox").parent().removeClass('hiddenFields');
            }

            PopupKeyUpActionProvider.CloseDialog();
        }
    };
})(jQuery);