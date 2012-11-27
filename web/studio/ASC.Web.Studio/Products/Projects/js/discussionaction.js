window.discussionaction = (function() {
    var myGuid;
    var fckId;

    var init = function(guid, fckid) {
        myGuid = guid;
        fckId = fckid;
    };

    var getMyGuid = function() {
        return myGuid;
    };    
    
    var getFckId = function() {
        return fckId;
    };

    return { init: init, getMyGuid: getMyGuid, getFckId: getFckId };
})();

jq(document).ready(function() {
    var id = jq.getURLParam("id");
    var projectId = jq.getURLParam("prjID");
    var isMobile = jq.browser.mobile;

    Attachments.bind("addFile", function(ev, file) {
        Teamlab.addPrjEntityFiles(null, id, "message", [file.id], function() { });
    });
    Attachments.bind("deleteFile", function(ev, fileId) {
        Teamlab.removePrjEntityFiles({}, id, "message", fileId, function() { });
        Attachments.deleteFileFromLayout(fileId);
    });
    Attachments.bind("loadAttachments", function() { });

    jq('#discussionTabs li').click(function() {
        jq('#discussionTabs li').removeClass('current');
        jq(this).addClass('current');
        if (jq(this).attr('container') == 'discussionFilesContainer' && id) {
            Attachments.loadFiles();
        }
        jq('#discussionTabsContent > div').hide();
        jq('#discussionTabsContent #' + jq(this).attr('container')).show();
    });

    jq('#discussionParticipantsContainer .discussionParticipant span').live('click', function() {
        var userId = jq(this).attr('guid');
        if (userId != discussionaction.getMyGuid()) {
            jq(this).parent().remove();
            discussionParticipantsSelector.DisableUser(userId);
        }
    });

    jq('#addDiscussionParticipantButton a').click(function() {
        discussionParticipantsSelector.ClearSelection();

        jq('#discussionParticipantsContainer span.userLink').each(function() {
            var userId = jq(this).attr('guid');
            discussionParticipantsSelector.SelectUser(userId);
        });

        discussionParticipantsSelector.IsFirstVisit = true;
        discussionParticipantsSelector.ShowDialog();
    });

    discussionParticipantsSelector.OnOkButtonClick = function() {
        jq('#discussionParticipantsContainer .discussionParticipant').remove();

        var participants = discussionParticipantsSelector.GetSelectedUsers();
        for (var i = 0; i < participants.length; i++) {
            var pid = participants[i].ID;
            var pname = participants[i].Name;

            var newParticipant = ('<div class="discussionParticipant"><span class="userLink" guid=' + pid + '>' + pname + '</span></div>');
            var lastParticipant = jq('#discussionParticipantsContainer .discussionParticipant:last');
            if (lastParticipant.length) {
                lastParticipant.after(newParticipant);
            } else {
                jq('#discussionParticipantsContainer .inviteMessage').after(newParticipant);
            }
        }
    };

    jq('#hideDiscussionPreviewButton').click(function() {
        jq('#discussionPreviewContainer').hide();
    });

    jq('#discussionProject').change(function() {
        if (jq(this).val() != -1) {
            jq('#discussionProject option[value=-1]').remove();
            jq('#discussionProjectContainer').removeClass('requiredFieldError');
        }
    });

    jq('#discussionTitleContainer input').keyup(function() {
        if (jq(this).val().trim() != '') {
            jq('#discussionTitleContainer').removeClass('requiredFieldError');
        }
    });

    jq('#discussionActionButton').click(function() {
        jq('#discussionProjectContainer').removeClass('requiredFieldError');
        jq('#discussionTitleContainer').removeClass('requiredFieldError');

        var projectid = projectId ? projectId : jq('#discussionProject').val();
        var title = jq('#discussionTitleContainer input').val().trim();

        var isError = false;
        if (projectid == -1) {
            jq('#discussionProjectContainer').addClass('requiredFieldError');
            isError = true;
        }

        if (title == '') {
            jq('#discussionTitleContainer').addClass('requiredFieldError');
            isError = true;
        }

        if (isError) {
            var scroll = jq('#discussionProjectContainer').offset().top;
            jq('body, html').animate({
                scrollTop: scroll
            }, 500);
            return;
        }

        var discussion =
        {
            projectid: projectid,
            title: title,
            content: isMobile ? ASC.Controls.HtmlHelper.Text2EncodedHtml(jq('[id$=discussionContent]').val())
                              : FCKeditorAPI.GetInstance(discussionaction.getFckId()).GetHTML(true)
        };

        var discussionId = jq(this).attr('discussionId');
        if (discussionId != -1) {
            discussion.messageid = discussionId;
        }

        var participants = [];
        jq('#discussionParticipantsContainer .discussionParticipant span').each(function() {
            participants.push(jq(this).attr('guid'));
        });
        discussion.participants = participants.join(',');

        lockDiscussionActionPageElements();
        if (discussionId == -1) {
            addDiscussion(discussion);
        }
        else {
            updateDiscussion(discussion);
        }
    });

    function lockDiscussionActionPageElements() {
        jq('#discussionProject').attr('disabled', 'disabled').addClass('disabled');
        jq('#discussionTitleContainer input').attr('readonly', 'readonly').addClass('disabled');
        jq('iframe[id^=ctl00_ctl00]').contents().find('iframe').contents().find('#fckbodycontent').attr('readonly', 'readonly').addClass('disabled');
        jq('#discussionButtonsContainer').hide();
        jq('#discussionActionsInfoContainer').show();
    }

    function unlockDiscussionActionPageElements() {
        jq('#discussionProject').removeAttr('disabled').removeClass('disabled');
        jq('#discussionTitleContainer input').removeAttr('readonly').removeClass('disabled');
        jq('iframe[id^=ctl00_ctl00]').contents().find('iframe').contents().find('#fckbodycontent').removeAttr('readonly').removeClass('disabled');
        jq('#discussionActionsInfoContainer').hide();
        jq('#discussionButtonsContainer').show();
    }

    var addDiscussion = function(discussion) {
        var params = {};
        Teamlab.addPrjDiscussion(params, discussion.projectid, discussion, { success: onAddDiscussion, error: onAddDiscussionError });
    };

    var onAddDiscussion = function(params, discussion) {
        window.location.replace('messages.aspx?prjID=' + discussion.projectId + '&id=' + discussion.id);
    };

    var onAddDiscussionError = function() {
        if (this.__errors[0] == "Access denied.") {
            window.location.replace("messages.aspx");
        }
        unlockDiscussionActionPageElements();
    };
    var onUpdateDiscussionError = function () {
        if (this.__errors[0] == "Access denied.") {
            window.location.replace("messages.aspx");
        }
        unlockDiscussionActionPageElements();
    };
    var updateDiscussion = function(discussion) {
        var params = {};
        Teamlab.updatePrjDiscussion(params, discussion.messageid, discussion, { success: onUpdateDiscussion, error: onUpdateDiscussionError });
    };

    var onUpdateDiscussion = function(params, discussion) {
        window.location.replace('messages.aspx?prjID=' + discussion.projectId + '&id=' + discussion.id);
    };

    jq('#discussionPreviewButton').click(function() {
        var discussion =
        {
            title: jq('#discussionTitleContainer input').val(),
            authorName: jq(this).attr('authorName'),
            authorTitle: jq(this).attr('authorTitle'),
            authorAvatarUrl: jq(this).attr('authorAvatarUrl'),
            createOn: formatDate(new Date()),
            content: isMobile ? ASC.Controls.HtmlHelper.Text2EncodedHtml(jq('[id$=discussionContent]').val())
                              : FCKeditorAPI.GetInstance(discussionaction.getFckId()).GetHTML(true)
        };

        jq('#discussionPreviewContainer .discussionContainer').remove();
        jq('#discussionTemplate').tmpl(discussion).prependTo('#discussionPreviewContainer');
        jq('#discussionPreviewContainer').show();
    });

    jq('#discussionCancelButton').click(function() {
        var projectId = jq(this).attr('projectId');
        var discussionId = jq.getURLParam("id");
        if (discussionId != "") {
            window.location.replace('messages.aspx?prjID=' + projectId + '&id=' + id);
        } else {
            window.location.replace('messages.aspx?prjID=' + projectId);
        }

    });

    var formatDate = function(date) {
        var dateArray =
            ['0' + date.getDate(), '0' + (date.getMonth() + 1), '0' + date.getFullYear(), '0' + date.getHours(), '0' + date.getMinutes()];
        for (var i = 0; i < dateArray.length; i++) {
            dateArray[i] = dateArray[i].slice(-2);
        }
        var shortDate = dateArray[0] + '.' + dateArray[1] + '.' + dateArray[2] + ' ' + dateArray[3] + ':' + dateArray[4];
        return shortDate;
    };
});
