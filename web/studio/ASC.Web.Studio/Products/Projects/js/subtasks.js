window.subtasks = (function() {
    var init = function() {
        jq('.taskList').on('mouseenter', '.subtask .taskName span', function(event) {
            showDescribePanel(event);
        });
        jq('#subtaskContainer').on('mouseenter', '.subtask .taskName span', function(event) {
            showDescribePanel(event);
        });
        jq('.taskList').on('mouseleave', '.subtask .taskName span', function() {
            hideDescribePanel();
        });
        jq('#subtaskContainer').on('mouseleave', '.subtask .taskName span', function() {
            hideDescribePanel();
        });
        //ga-track-events
        jq(".quickAddSubTaskLink").trackEvent(ga_Categories.subtask, ga_Actions.createNew, "create-new-subtask");
        jq(".subtask[status=1] .check input").trackEvent(ga_Categories.subtask, ga_Actions.changeStatus, "close");
        jq(".subtask[status=2] .check input").trackEvent(ga_Categories.subtask, ga_Actions.changeStatus, "open");
        //end ga-track-events
    };
    var showDescribePanel = function(event) {
        subtaskDescrTimeout = setTimeout(function() {
            var targetObject;
            if (jq(event.target).is('a')) {
                targetObject = jq(event.target).parent();
            } else {
                targetObject = event.target;
            }
            jq('#subTaskDescrPanel .created, #subTaskDescrPanel .createdby, #subTaskDescrPanel .closed, #subTaskDescrPanel .closedby').hide();
            var taskName = jq(targetObject).closest('.taskName');
            if (jq(taskName).attr('status') == 2) {
                if (typeof jq(targetObject).parent().attr('updated') != 'undefined') {
                    jq('#subTaskDescrPanel .closed .value').html(jq(targetObject).parent().attr('updated').substr(0, 10));
                    jq('#subTaskDescrPanel .closed').show();
                }
                if (typeof jq(targetObject).parent().attr('createdby') != 'undefined') {
                    jq('#subTaskDescrPanel .closedby .value').html(jq(targetObject).parent().attr('createdby'));
                    jq('#subTaskDescrPanel .closedby').show();
                }
            } else {
                if (typeof jq(targetObject).parent().attr('created') != 'undefined') {
                    jq('#subTaskDescrPanel .created .value').html(jq(targetObject).parent().attr('created').substr(0, 10));
                    jq('#subTaskDescrPanel .created').show();
                }
                if (typeof jq(targetObject).parent().attr('createdby') != 'undefined') {
                    jq('#subTaskDescrPanel .createdby .value').html(jq(targetObject).parent().attr('createdby'));
                    jq('#subTaskDescrPanel .createdby').show();
                }
            }

            if (jq(targetObject).closest('#subtaskContainer').length) {
                ASC.Projects.TaskDescroptionPage.showActionsPanel('subTaskDescrPanel', targetObject);
            } else {
                tasks.showActionsPanel('subTaskDescrPanel', targetObject);
            }
            overSubtaskDescrPanel = true;
        }, 400, this);
    };

    var hideDescribePanel = function() {
        clearTimeout(subtaskDescrTimeout);
        overSubtaskDescrPanel = false;
        jq('#subTaskDescrPanel').hide();
    };
    var editSubtask = function(subtaskid) {
        hideSubtaskFields();
        jq("#quickAddSubTaskField").remove();
        var elem = jq(".subtask[subtaskid='" + subtaskid + "']'");
        jq(elem).removeClass('menuopen');
        var responsible = {};
        var user = "";
        if (jq(elem).children(".user").length) {
            user = jq(elem).children(".user").attr('value');
            responsible = { id: user, displayName: jq(elem).children(".user").text() };
        } else {
            responsible = null;
        }
        var taskid = jq(elem).attr('taskid');
        var projid = jq(elem).closest(".subtasks").find(".quickAddSubTaskField:first").attr("projectid");
        if (jq.trim(projid) == "") {
            projid = jq.getURLParam("prjID");
        }
        var data = { title: jq(elem).children(".taskName").text(), subtaskid: subtaskid, responsible: responsible, taskid: taskid, projectid: projid };

        jq('#AddSubTaskFieldTemplate').tmpl(data).insertAfter(elem);
        jq(".subtask[subtaskid=" + subtaskid + "] .taskName").hide();
        jq("#quickAddSubTaskField").show();
        jq(".subTaskName").focus();
    };
    var addSubtask = function(taskid, subtask) {
        subtask.taskid = taskid;
        var subtaskCont = jq('.quickAddSubTaskField:visible').closest('.subtasks');
        if (jq(subtaskCont).find('.subtask').length) {
            var elem = jq(subtaskCont).find('.quickAddSubTaskLink');
            jq('#newSubtaskTemplate').tmpl(subtask).insertBefore(elem);

        } else {
            jq('#newSubtaskTemplate').tmpl(subtask).prependTo(subtaskCont);
            jq(subtaskCont).show();
        }
        jq(subtaskCont).find('.subtaskSaving').hide();
        var subtaskInput = jq(subtaskCont).find('.quickAddSubTaskField input');
        jq(subtaskInput).removeAttr('disabled');
        jq(subtaskInput).val('');
        jq(subtaskInput).focus();
    };
    var updateSubtask = function(subtask) {
        var subtaskid = subtask.id;
        var oldSubtask = jq('.subtask[subtaskid="' + subtaskid + '"]');
        subtask.taskid = jq(oldSubtask).attr('taskid');
        jq(oldSubtask).hide();
        jq('#newSubtaskTemplate').tmpl(subtask).insertBefore(oldSubtask);
        jq(oldSubtask).remove();
    };
    var updateSubtaskStatus = function(subtask) {
        var subtaskid = subtask.id;
        var newstatus = subtask.status;
        var oldSubtask = jq('.subtask[subtaskid="' + subtaskid + '"]');
        var subtaskCont = jq(oldSubtask).closest('.subtasks');
        if (newstatus == 1) {
            jq(oldSubtask).prependTo(subtaskCont);
            jq(oldSubtask).removeClass('closed');
            jq(oldSubtask).find('.check input').removeAttr('checked');

        } else {
            jq(oldSubtask).addClass('closed');
            var user = jq(oldSubtask).find(".user");
            if (jq(user).hasClass("not")) {
                var displayName = subtask.responsible.displayName;
                if (serviceManager.getMyGUID() == subtask.responsible.id) {
                    displayName = "<b>" + jq(user).attr("me") + "</b>";
                    jq(user).removeClass("not");
                }
                jq(user).attr('value', subtask.responsible.id);
                jq(user).html(displayName);
            }
            jq(oldSubtask).find('.check input').attr('checked', 'checked');
            jq(oldSubtask).appendTo(subtaskCont);
        }
        changeSubtaskAtributes(oldSubtask, subtask);
        jq('.taskProcess').remove();
        jq(oldSubtask).find('.check input').show();

    };
    var changeSubtaskAtributes = function(oldSubtask, subtask) {
        var taskName = jq(oldSubtask).find('.taskName');
        jq(taskName).attr('created', subtask.displayDateCrtdate);
        if (subtask.createdBy) {
            jq(taskName).attr('createdBy', subtask.createdBy.displayName);
        }
        if (subtask.updatedBy) {
            jq(taskName).attr('updatedBy', subtask.updatedBy.displayName);
        }
        if (subtask.updated) {
            jq(taskName).attr('updated', subtask.displayDateUptdate);
        }
        jq(taskName).attr('status', subtask.status);
    };
    var hideSubtaskFields = function() {
        var fields = jq(".quickAddSubTaskField:visible");
        for (var i = 0; i < fields.length; i++) {
            if (jq.trim(jq(fields[i]).attr("id")).length && jq(fields[i]).attr("id") != "addNewSubtaskField") {
                jq(fields[i]).remove();
            } else {
                jq(fields[i]).hide();
                jq(fields[i]).siblings('.quickAddSubTaskLink').show();
            }
        }
        jq(".subtask .taskName").show();
    };
    return {
        init: init,
        updateSubtask: updateSubtask,
        updateSubtaskStatus: updateSubtaskStatus,
        addSubtask: addSubtask,
        editSubtask: editSubtask,
        hideSubtaskFields: hideSubtaskFields
    };
})(jQuery)