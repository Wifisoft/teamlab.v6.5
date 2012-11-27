window.discussiondetails = (function() {
    var myGuid;

    var init = function(guid) {
        myGuid = guid;
    };

    var getMyGuid = function() {
        return myGuid;
    };

    return { init: init, getMyGuid: getMyGuid };
})();

jq(document).ready(function() {

    var isCommentEdit = false;
    var commentsCount = jq('#mainContainer div[id^=container_] div[id^=comment_] table').length;

    if (commentsCount == 0) {
        jq('#commentsContainer').hide();
        jq('#emptyCommentsContainer').show();
    }

    jq('#changeSubscribeButton a').click(function() {
        var discussionId = jq.getURLParam("id");
        var subscribed = jq(this).attr('subscribed') === '1';
        AjaxPro.onLoading = function(b) {
            if (b) {
                jq("#changeSubscribeButton").block({ message: null });
            } else {
                jq("#changeSubscribeButton").unblock();
            }
        };

        AjaxPro.DiscussionDetails.ChangeSubscribe(discussionId,
            function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }

                if (res.value == "")
                    return;

                var currentLink = jq('#discussionParticipantsContainer [data-uid=' + discussiondetails.getMyGuid() + ']');
                if (subscribed) {
                    currentLink.parent().hide();
                    currentLink.parent().addClass('hidden');
                    jq('#changeSubscribeButton a').attr('subscribed', '0');
                    if (window.discussionParticipantsSelector) {
                        discussionParticipantsSelector.DisableUser(discussiondetails.getMyGuid());
                    }

                } else {
                    currentLink = jq('#currentLink');
                    currentLink.show();
                    currentLink.removeClass('hidden');
                    jq('#changeSubscribeButton a').attr('subscribed', '1');
                }
                var participantsCount = jq('#discussionParticipantsContainer .discussionParticipant').not('.hidden').length;
                updateTabTitle('participants', participantsCount);
                jq("#changeSubscribeButton a").text(jq.trim(res.value));
            });
    });

    jq('#deleteDiscussionButton a').click(function() {
        showQuestionWindow();
    });

    var showQuestionWindow = function() {
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq('#questionWindow'),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '400px',

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-200px',
                'margin-top': margintop,
                'background-color': 'White'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            baseZ: 666,

            fadeIn: 0,
            fadeOut: 0,

            onBlock: function() {
            }
        });

        jq('#questionWindow .remove').bind('click', function() {
            var discussionId = jq.getURLParam("id");
            deleteDiscussion(discussionId);
            return false;
        });

        jq('#questionWindow .cancel').bind('click', function() {
            jq.unblockUI();
            return false;
        });
    };

    var deleteDiscussion = function(discussionId) {
        var params = {};
        Teamlab.removePrjDiscussion(params, discussionId, { success: onDeleteDiscussion, error: onDeleteDiscussionError });
    };

    var onDeleteDiscussion = function(params, discussion) {
        window.location.replace("messages.aspx?prjID=" + discussion.projectId);
    };
    var onDeleteDiscussionError = function() {
        if (this.__errors[0] == "Access denied.") {
            window.location.replace("messages.aspx");
        }
    };
    jq('#discussionTabs li').click(function() {
        jq('#discussionTabs li').removeClass('current');
        jq(this).addClass('current');
        if (jq(this).attr('container') == 'discussionFilesContainer') {
            Attachments.loadFiles();
        }
        jq('#discussionTabsContent > div').hide();
        jq('#discussionTabsContent #' + jq(this).attr('container')).show();
    });

    jq('#addFirstCommentButton').click(function() {
        jq('#emptyCommentsContainer').hide();
        jq('#commentsContainer').show();
        jq('#add_comment_btn').click();
    });

    jq('#btnCancel').live('click', '#btnCancel', function() {
        commentsCount = jq('#mainContainer div[id^=container_] div[id^=comment_] table').length;
        if (commentsCount == 0) {
            jq('#commentsContainer').hide();
            jq('#emptyCommentsContainer').show();
        }
        isCommentEdit = false;
    });

    jq('#btnAddComment').click(function() {
        if (!isCommentEdit) {
            var text = jq('iframe[id^=CommentsFckEditor]').contents().find('iframe').contents().find('#fckbodycontent').text();
            if (text.trim() != '') {
                commentsCount = jq('#mainContainer div[id^=container_] div[id^=comment_] table').length;
                updateTabTitle('comments', commentsCount + 1);
            }
        } else {
            isCommentEdit = false;
        }
    });

    jq('a[id^=edit_]').live('click', function() {
        isCommentEdit = true;
    });

    jq('a[id^=remove_]').live('click', function() {
        commentsCount = jq('#mainContainer div[id^=container_] div[id^=comment_] table').length;
        updateTabTitle('comments', commentsCount - 1);
        if (commentsCount == 1) {
            jq('#commentsContainer').hide();
            jq('#mainContainer').empty();
            jq('#commentsTitle').empty();
            jq('#emptyCommentsContainer').show();
        }
    });

    jq('#manageParticipantsButton a').click(function() {
        var participantsTab = jq("#discussionTabs li[container=discussionParticipantsContainer]");
        if (!participantsTab.is(".current")) {
            jq("#discussionTabs li").removeClass("current");
            participantsTab.addClass("current");
            jq('#discussionTabsContent > div').hide();
            jq('#discussionTabsContent #' + participantsTab.attr('container')).show();
        }

        jq('#discussionParticipantsContainer span.userLink:visible').each(function() {
            var userId = jq(this).attr('data-uid');
            discussionParticipantsSelector.SelectUser(userId);
        });

        discussionParticipantsSelector.IsFirstVisit = true;
        discussionParticipantsSelector.ShowDialog();
    });

    if (window.discussionParticipantsSelector) {
        discussionParticipantsSelector.OnOkButtonClick = function() {
            var participants = [];
            var participantsIds = [];

            var pid = jq('#hiddenProductId').val();
            var myId = discussiondetails.getMyGuid();

            var users = discussionParticipantsSelector.GetSelectedUsers();
            for (var i = 0; i < users.length; i++) {
                var userId = users[i].ID;
                var userName = users[i].Name;

                participants.push({ userId: userId, userName: userName });
                participantsIds.push(userId);
            }

            var discussionId = jq.getURLParam("id");

            AjaxPro.DiscussionDetails.ChangeDiscussionParticipants(discussionId, participantsIds.join(','),
                function(res) {
                    if (res.error != null) {
                        alert(res.error.Message);
                        return;
                    }

                    jq('#discussionParticipantsContainer .discussionParticipant').not('#currentLink').remove();
                    var lastParticipant = jq('#discussionParticipantsContainer .discussionParticipant:last');

                    for (var i = 0; i < participants.length; i++) {
                        var p = participants[i];
                        var newParticipant = '<div class="discussionParticipant"><span class="userLink" id="element_' + i + '" data-uid="' + p.userId +
                            '" data-pid="' + pid + '"><a class="linkDescribe" href="javascript:void(0)">' + p.userName + '</a></span></div>';
                        StudioUserProfileInfo.RegistryElementOnLive('element_' + i, "\"" + p.userId + "\",\"" + pid + "\"");

                        if (lastParticipant.length) {
                            lastParticipant.after(newParticipant);
                        } else {
                            jq('#discussionParticipantsContainer').prepend(newParticipant);
                        }
                    }

                    jq("#changeSubscribeButton a").text(jq.trim(res.value));

                    var participantsCount = jq('#discussionParticipantsContainer .discussionParticipant').length - 1;
                    updateTabTitle('participants', participantsCount);
                });
        };
    }
});

function updateTabTitle(tabTitle, count) {
    var container;
    switch (tabTitle) {
        case 'comments':
            container = 'discussionCommentsContainer';
            break;
        case 'participants':
            container = 'discussionParticipantsContainer';
            break;
        case 'files':
            container = 'discussionFilesContainer';
            break;
    }
    if (!container) return;
    
    var tab = jq('#discussionTabs li[container=' + container + ']');
    var oldTitle = tab.text();
    var ind = oldTitle.lastIndexOf('(');
    var newTitle = oldTitle;
    if (ind > -1 && count == 0) {
        newTitle = oldTitle.slice(0, ind);
    }
    else if (ind > -1 && count != 0) {
        newTitle = oldTitle.slice(0, ind) + '(' + count + ')';
    }
    else {
        if (count > 0)
            newTitle = oldTitle + '(' + count + ')';
    }
    tab.text(newTitle);
}