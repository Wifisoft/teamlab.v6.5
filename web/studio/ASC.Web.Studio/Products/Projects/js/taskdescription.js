window.ASC.Projects.TaskDescroptionPage = (function() {
    var isInit = false,
        currentTask = {},
        taskId = undefined,
        projId = undefined,
        currentTab = "subtasks",
        commentIsEdit = false,
        listResponsibles = new Array(),
        TaskTimeSpend = '0.00';

    var init = function(id) {
        if (isInit === false) {
            isInit = true;
        }
        subtasks.init();
        initCommentsBlock();
        taskId = id;
        projId = jq.getURLParam("prjID");

        Teamlab.bind(Teamlab.events.updatePrjTask, onUpdateTask);
        Teamlab.bind(Teamlab.events.addPrjTask, onAddTask);

        if (taskDescription) {
            taskDescription = Teamlab.create('prj-task', null, jq.parseJSON(jQuery.base64.decode(taskDescription)).response);
            onGetTaskDescription(null, taskDescription);
        }

        if (milestones) {
            milestones = Teamlab.create('prj-milestones', null, jq.parseJSON(jQuery.base64.decode(milestones)).response);
        }

        //---------Click events-------//
        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            var $elt = jq(elt);

            if (
			      $elt.is('.actionPanel') ||
			      $elt.is('.subTaskName') ||
    			  $elt.is('.choose') ||
    			  $elt.is('.choose > *') ||
			      $elt.is('#subTaskName')
			    ) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('.actionPanel')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {

                jq('#usersActionPanel').hide();

                if (jq(".subtasks div").length == 0) {
                    showEmptySubtasksPanel();
                }
            }
        });
        // task actions
        jq("#topTaskActionContainer").on("click", "#editTaskAction", function() {
            editTask();
        });
        jq("#topTaskActionContainer").on("click", "#removeTask", function() {
            showQuestionWindowTaskRemove();
        });
        jq("#topTaskActionContainer").on("click", "#addNewTask", function() {
            taskaction.clearAddTaskForm();
            taskaction.showTaskForm(false);
        });
        jq(".commonInfoTaskDescription").on('click', "#closeButton", function() {
            if (jq(".subtasks .subtask").length != jq(".subtasks .subtask.closed").length) {
                showQuestionWindow();
            }
            else {
                closeTask();
            }
        });
        jq(".commonInfoTaskDescription").on('click', "#resumeButton", function() {
            resumeTask();
        });
        jq(".commonInfoTaskDescription").on('click', '#acceptButton', function() {
            var data = {};
            data.title = currentTask.title;

            data.description = currentTask.description;

            if (currentTask.deadline) {
                data.deadline = currentTask.deadline;
            }

            if (currentTask.milestoneId) {
                data.milestoneid = currentTask.milestoneId;
            }
            data.priority = currentTask.priority;

            data.responsible = serviceManager.getMyGUID();

            Teamlab.updatePrjTask({}, taskId, data, { success: onUpdateTask });

            return false;
        });

        jq("#startTimer a").bind('click', function() {
            var taskId = jq(this).attr('taskid'),
            prjid = jq(this).attr('projectid');
            ASC.Projects.TimeSpendActionPage.showTimer('timetracking.aspx?prjID=' + prjid + '&taskId=' + taskId + '&action=timer');
        });
        jq("#emptySubtasksPanel .emptyScrBttnPnl").live('click', function() {
            jq("#addNewSubtaskField").remove();
            jq(".quickAddSubTaskLink").hide();
            jq("#emptySubtasksPanel").hide();
            var taskid = jq.getURLParam("id");
            var data = { taskid: taskid, title: "", subtaskId: "", responsible: null };

            jq('#AddSubTaskFieldTemplate').tmpl(data).appendTo(".subtasks");

            jq("#quickAddSubTaskField .subTaskName").attr("id", "subTaskName");
            jq("#quickAddSubTaskField").attr("id", "addNewSubtaskField");

            jq("#addNewSubtaskField").show();
            jq(".subtasks").show();
            jq("#subTaskName").focus();
            return false;
        });

        jq('.quickAddSubTaskField .chooseWrap').live('click', function(event) {
            if (jq('#usersActionPanel .teamListPopup').length) {
                showActionsPanel('usersActionPanel', this);
            } else {
                jq('#usersActionPanel').append('<div class="loader32"></div>');
                showActionsPanel('usersActionPanel', this);
                if (typeof projectTeam != 'undefined') {
                    var team = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectTeam)).response);
                    onGetTeam({ projectid: projId }, team);
                }
                else {
                    Teamlab.getPrjProjectTeamPersons({}, projId, { success: onGetTeam });
                }
            }
            return false;
        });

        jq('.teamListPopup span').live('click', function(event) {
            var val = jq(this).attr('value');
            jq('.quickAddSubTaskField .choose.responsible').attr('value', val);
            if (val == "") {
                jq('.quickAddSubTaskField .dottedLink').text(jq('.quickAddSubTaskField .choose.responsible').attr('choose'));
            }
            else {
                jq('.quickAddSubTaskField .dottedLink').text(jq(this).text());
            }
            jq('#usersActionPanel').hide();

            if (jq('.quickAddSubTaskField').length > 1) {
                jq('#quickAddSubTaskField input').focus();
            }
            else {
                jq('#addNewSubtaskField input').focus();
            }
            return false;
        });

        jq('#subtaskContainer').on('click', ".subtask .menupoint", function(event) {
            showActionsPanel("subtaskActionPanel", this);
            jq("#subtaskActionPanel .pm").show();
            var subtask = jq(this).closest(".subtask");
            if (jq(subtask).hasClass('closed')) {
                jq('#subtaskActionPanel #sta_accept').hide();
                jq('#subtaskActionPanel #sta_edit').hide();
                return false;
            }
            var user = jq(subtask).find(".user");
            if (jq(user).length) {
                if (!jq(user).hasClass("not")) {
                    jq('#subtaskActionPanel #sta_accept').hide();
                }
            }
            return false;
        });
        jq('#subtaskActionPanel #sta_edit').live('click', function() {
            subtasks.editSubtask(jq('#subtaskActionPanel').attr('objid'));
            jq('.actionPanel').hide();
        });
        jq('#subtaskActionPanel #sta_remove').live('click', function() {
            jq('.actionPanel').hide();
            var subtaskid = jq('#subtaskActionPanel').attr('objid');

            Teamlab.removePrjSubtask({ subtaskId: subtaskid }, taskId, subtaskid, { success: onRemoveSubtask });

            return false;
        });
        jq('.quickAddSubTaskLink').live('click', function() {
            jq(".subtask .taskName").show();
            jq("#quickAddSubTaskField").remove();
            jq('.quickAddSubTaskLink').hide();
            jq("#addNewSubtaskField").show();
            jq("#subTaskName").val('');
            jq("#subTaskName").focus();
            return false;
        });
        jq('.taskName').live('dblclick', function(event) {
            var subtaskid = jq("#quickAddSubTaskField .subTaskName").attr('subtaskid');
            jq("#quickAddSubTaskField").remove();
            jq(".subtask[subtaskid=" + subtaskid + "] .taskName").show();
            if (!jq(this).is('.canedit') || jq(this).parents('.subtask').is('.closed')) return false;
            subtasks.editSubtask(jq(this).attr('subtaskid'));
            return false;
        });
        jq('.subtask .check input').live('change', function(event) {
            var data = {},
              subtaskid = jq(this).attr('subtaskid');
            if (jq(this).is(':checked')) {
                closeSubTask(subtaskid);
            } else {
                data.status = 'open';

                jq('.subtask[subtaskid=' + subtaskid + '] .check input').hide();
                jq('.subtask[subtaskid=' + subtaskid + '] .check').append('<div class="taskProcess"></div>');

                Teamlab.updatePrjSubtask({}, taskId, subtaskid, data, { success: onUpdateSubtaskStatus });
            }
        });

        //---save subtasks changes----//
        jq('#quickAddSubTaskField .subTaskName').live('keydown', function(e) {
            var subtaskid = jq(this).attr('subtaskid');
            if (e.which == 13) {
                if (!jq.trim(jq(this).val()).length) {
                    return undefined;
                }
                var data = {};
                data.title = jq.trim(jq(this).val());
                data.responsible = jq('#quickAddSubTaskField .choose').attr('value');

                Teamlab.updatePrjSubtask({}, taskId, subtaskid, data, { success: onUpdateSubtask });

                jq('#quickAddSubTaskField').remove();
                return false;
            } else if (e.which == 27) {
                jq(".subtask[subtaskid=" + subtaskid + "] .taskName").show();
                jq('#quickAddSubTaskField').remove();
            }
        });
        jq('#subtaskActionPanel #sta_accept').click(function() {
            jq("#subtaskActionPanel").hide();
            var data = {},
            subtaskid = jq(this).closest("#subtaskActionPanel").attr('objId');
            data.title = jq.trim(jq('.subtask[subtaskid=' + subtaskid + '] .taskName').text());
            data.responsible = serviceManager.getMyGUID();
            data.description = '';

            Teamlab.updatePrjSubtask({}, taskId, subtaskid, data, { success: onUpdateSubtask });

            return false;
        });

        //---add new subtask----//
        jq("#addNewSubtaskField #subTaskName").live('keydown', function(e) {
            jq('.actionPanel').hide();
            if (e.which == 13) {
                if (!jq.trim(jq(this).val()).length) {
                    return undefined;
                }
                jq('.quickAddSubTaskField input').attr('disabled', 'disabled');
                jq('.subtaskSaving[taskid=' + taskId + ']').show();
                var data = {};
                data.title = jq.trim(jq(this).val());
                data.responsible = jq('.quickAddSubTaskField .choose').attr('value');

                Teamlab.addPrjSubtask({ 'taskid': taskId }, taskId, data, { success: onAddSubtask });

                return false;
            } else if (e.which == 27) {
                jq('.quickAddSubTaskLink').show();
                jq('.quickAddSubTaskField').hide();
                jq(".subtask .taskName").show();
                if (!jq('.subtasks .subtask').length) {
                    jq("#addNewSubtaskField").remove();
                    showEmptySubtasksPanel();
                    jq('.quickAddSubTaskLink').hide();
                }
            }
        });

        //-------------Comments-------------//
        jq("#emptyCommentsPanel .emptyScrBttnPnl").live('click', function() {
            jq("#emptyCommentsPanel").hide();

            jq("#add_comment_btn").click();
            jq("#commentsListWrapper").show();
            return false;
        });

        jq("#btnCancel").live('click', function() {

            var count = jq("#commentContainer #mainContainer div[id^='container_']").length;
            if (count == 0) {
                jq("#commentsListWrapper").hide();
                jq("#add_comment_btn").hide();
                jq("#emptyCommentsPanel").show();
            }
            commentIsEdit = false;
        });
        jq("#btnAddComment").live('click', function() {
            if (!commentIsEdit) {
                changeCountInTab('add', "commentsTab");
            }
            else {
                commentIsEdit = false;
            }
        });
        jq("#mainContainer div[id^='container_'] a[id^='remove_']").live('click', function() {
            changeCountInTab('delete', "commentsTab");
            var count = jq("#commentContainer #mainContainer div[id^='container_']").length;
            if (count - 1 == 0) {
                jq("#commentsListWrapper").hide();
                jq("#commentContainer #mainContainer").attr('style', '');
                jq("#commentContainer #mainContainer").empty();

                jq("#emptyCommentsPanel").show();
            }
        });
        jq("#mainContainer div[id^='container_'] a[id^='edit_']").live('click', function() {
            commentIsEdit = true;
        });

        /*---close/remove task----*/
        jq('#questionWindow .cancel').bind('click', function() {
            jq.unblockUI();
            return false;
        });
        jq('#questionWindow .end').bind('click', function() {
            closeTask();
            jq.unblockUI();
            return false;
        });
        jq('#questionWindowTaskRemove .cancel').bind('click', function() {
            jq.unblockUI();
            return false;
        });
        jq('#questionWindowTaskRemove .remove').bind('click', function() {
            removeTask();
            jq.unblockUI();
            return false;
        });
    };
    var initCommentsBlock = function() {
        jq("#commentsTitle").remove();
        jq("#commentContainer #mainContainer").css("width", 100 + "%");
        var count = jq("#commentContainer #mainContainer div[id^='container_']").length;
        if (count != 0) {
            changeCountInTab(count, "commentsTab");
            jq("#add_comment_btn").show();
        }
        else {
            jq("#emptyCommentsPanel").show();
        }
    };

    var setTimeSpent = function(data) {
        TaskTimeSpend = data;
    };

    var displaySubtasks = function(task) {
        jq('.subtasks').empty();
        if (task.subtasks.length == 0) {
            showEmptySubtasksPanel();
            if (task.canWork == 0) {
                jq(".subtasksEmpty").parent().remove();
            }
            if (task.status == 2) {
                jq("#emptySubtasksPanel").find(".emptyScrBttnPnl").hide();
            }
            else {
                jq("#emptySubtasksPanel").find(".emptyScrBttnPnl").show();
            }
            jq('#taskTemplate').tmpl(task).prependTo('.subtasks');
        }
        else {
            jq('#taskTemplate').tmpl(task).prependTo('.subtasks');
            jq(".st_separater").after(jq('.subtasks .closed'));

            var subtasksCount = 0;
            for (var i = 0; i < task.subtasks.length; i++) {
                if (task.subtasks[i].status != 2) subtasksCount++;
            }
            changeCountInTab(subtasksCount, "subtaskTab");
            if (task.status == 2) {
                jq(".quickAddSubTaskLink").remove();
                jq(".subtask .check input").attr('disabled', true);
            }
            jq('.subtasks').show();
        }

        jq('.taskTabs').show();
        jq('#tabsContent').show();

    };
    var displayTotalInfo = function(task) {
        var deadline = "", displayDateDeadline = "", milestone = "", closedBy = "", closedDate = "", timeSpend = "";

        if (task.deadline) {
            displayDateDeadline = task.displayDateDeadline;
            deadline = task.deadline;
        }

        timeSpend = jq.timeFormat(TaskTimeSpend);

        if (task.status == 2) {
            if (task.updatedBy != undefined)
                closedBy = task.updatedBy.displayName;
            else
                closedBy = task.createdBy.displayName;

            closedDate = task.displayDateUptdate;
        }
        var responsibles = new Array();
        listResponsibles.splice(0, listResponsibles.length);
        if (task.responsibles.length) {
            for (var i = 0; i < task.responsibles.length; i++) {
                var person = { displayName: task.responsibles[i].displayName, id: task.responsibles[i].id };
                listResponsibles.push(task.responsibles[i].id);
                responsibles.push(person);
            }
        }
        if (task.milestone) {
            milestone = task.milestone.title;
            milestone = "[ " + task.milestone.displayDateDeadline + " ] " + milestone;
        }
        var priority = task.priority;
        var descriptionInfo = {
            createdDate: task.displayDateCrtdate, createdBy: task.createdBy.displayName,
            closedDate: closedDate, closedBy: closedBy,
            timeSpend: timeSpend, deadline: deadline, displayDateDeadline: displayDateDeadline, milestone: milestone, description: task.description,
            priority: priority, status: task.status, responsibles: responsibles, taskId: taskId, projId: projId
        };

        jq('#taskDescriptionTemplate').tmpl(descriptionInfo).prependTo('.commonInfoTaskDescription');

        if (!task.canEdit) {
            jq("dl.buttonContainer").remove();
        }
        if (timeSpend != "0:00" && timeSpend != "") {
            jq(".timeSpend").show();
        }
        jq(".subscribeLink").show();
    };
    var displayTaskDescription = function(task) {
        var text = task.title;
        if (text.length > 40) {
            text = text.slice(0, 37) + "...";
        }
        jq(".mainContainerClass >.containerHeaderBlock table td > div:first-child span:last-child").text(text);

        jq(".commonInfoTaskDescription").empty();

        displayTotalInfo(task);
        displaySubtasks(task);
    };
    var showEmptySubtasksPanel = function() {
        jq("#emptySubtasksPanel").show();
    };

    var showQuestionWindow = function() {
        jq('#questionWindow .end').attr('taskid', taskId);
        jq('#questionWindow .cancel').attr('taskid', taskId);
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq("#questionWindow"),
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
    };

    var showQuestionWindowTaskRemove = function() {
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq("#questionWindowTaskRemove"),
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

    };

    var closeSubTask = function(subtaskid) {
        var data = {};
        jq('.subtask[subtaskid=' + subtaskid + '] .check input').hide();
        jq('.subtask[subtaskid=' + subtaskid + '] .check').append('<div class="taskProcess"></div>');

        data.title = jq.trim(jq('.subtask[subtaskid=' + subtaskid + '] .taskName').text());
        data.responsible = serviceManager.getMyGUID();
        data.status = 'closed';

        Teamlab.updatePrjSubtask({}, taskId, subtaskid, data, { success: onUpdateSubtaskStatus });
    };

    var showActionsPanel = function(panelId, obj, event) {
        var objid = '';
        var x, y;

        objid = jq(obj).attr('subtaskid');

        jq('.actionPanel').hide();
        jq('#' + panelId).show();

        if (panelId == 'usersActionPanel') {
            x = jq(obj).offset().left - 7;
            y = jq(obj).offset().top + 20;
        }
        else {
            x = jq(obj).offset().left - 138;
            y = jq(obj).offset().top + 20;
            jq('.subtask[subtaskid=' + objid + ']').addClass('menuopen');
        }
        if (panelId == 'subTaskDescrPanel') {
            x = jq(obj).offset().left;
            y = jq(obj).offset().top + 26;
            jq('#' + panelId).attr('objid', jq(obj).attr('taskid'));
        }
        jq('#' + panelId).attr('objid', objid);

        if (jq('.subtask[subtaskid=' + objid + ']').hasClass('closed')) {
            jq('#subtaskActionPanel .pm').hide();
            jq('#subtaskActionPanel #sta_remove').show();
        }
        else {
            jq('#subtaskActionPanel .pm').show();
        }

        if (typeof y == 'undefined')
            y = jq(obj).offset().top + 26;

        jq('#' + panelId).css({ left: x, top: y });

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is('[id="' + panelId + '"]') || (elt.id == obj.id && obj.id.length) || jq(elt).is('.menupoint') || jq(elt).is('.other') || jq(elt).is('.choose') || jq(elt).is('.choose > *')) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('[id="' + panelId + '"]')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {
                jq('.actionPanel').hide();
                jq('.menuopen').removeClass('menuopen');
            }
        });
    };

    var changeFollowLink = function() {
        var currValue = jq("#followTaskActionTop a").text();
        jq("#followTaskActionTop a").text(jq("#followTaskActionTop a").attr("textValue"));
        jq("#followTaskActionTop a").attr('textValue', currValue);
    };
    var subscribeTask = function() {
        serviceManager.subscribeToTask('subscribeToTask', taskId);
        changeFollowLink();
    };

    var removeTask = function() {
        Teamlab.removePrjTask({}, taskId, { success: onRemoveTask });
    };
    /*--------event handlers--------*/
    var onGetTaskDescription = function(params, task) {
        displayTaskDescription(task);
        currentTask = task;
        document.title = task.title + " " + document.title;
        
        if(!task.canEdit) {
            jq("#editTaskAction").remove();
        }
        
        if (task.canWork != 3) {
            jq("#removeTask").remove();
        }
        if (task.canWork < 2) {
            Attachments.banOnEditing();
        }
        if (task.status == 2) {
            jq("#topTaskActionContainer").hide();
            jq("#topTaskActionContainer").find(".taskAction").hide();
            jq("#addNewTask, #followTaskActionTop").show();
            jq("#topTaskActionContainer").show();
        }
        else {
            jq("#topTaskActionContainer").show();
            jq("#topTaskActionContainer").find(".taskAction").show();
        }
        LoadingBanner.hideLoading();
    };
    var onUpdateTask = function(params, task) {
        currentTask = task;
        if (task.status == 2) {
            jq("#topTaskActionContainer").hide();
            jq("#topTaskActionContainer").find(".taskAction").hide();
            jq("#addNewTask, #followTaskActionTop").show();
            jq("#topTaskActionContainer").show();
        }
        else {
            jq("#topTaskActionContainer").find(".taskAction").show();
        }
        if (!task.canWork > 1) {
            Attachments.banOnEditing();
        }
        jq(".commonInfoTaskDescription").empty();
        displayTaskDescription(task);
        var text = task.title;
        if (text.length > 40) {
            text = text.slice(0, 37) + "...";
        }
        jq(".containerHeaderBlock table td > div:first-child span:last-child").text(text);
        jq(".headerTaskTitle").text(task.title);
        jq.unblockUI();
    };
    var onRemoveTask = function() {
        var link = jq(".containerHeaderBlock:first table tr td:first div:first a:last");
        var newUrl = jq(link).attr("href");
        window.location.replace(newUrl);

    };
    var onRemoveSubtask = function(params) {
        var subtaskid = params.subtaskId;

        if (!jq('.subtask[subtaskid=' + subtaskid + ']').hasClass('closed')) {
            jq('.subtask[subtaskid=' + subtaskid + ']').remove();
            changeCountInTab("delete", "subtaskTab");
        }
        else {
            jq('.subtask[subtaskid=' + subtaskid + ']').remove();
            if (jq(".subtasks").find(".subtask").length == 0) {
                showEmptySubtasksPanel();
                jq('.quickAddSubTaskLink').hide();
            }
        }
    };
    var onAddSubtask = function(params, subtask) {
        subtasks.addSubtask(params.taskid, subtask);
        changeCountInTab("add", "subtaskTab");
    };
    var onUpdateSubtask = function(params, subtask) {
        subtasks.updateSubtask(subtask);
    };
    var onUpdateSubtaskStatus = function(params, subtask) {
        subtasks.updateSubtaskStatus(subtask);
        if (subtask.status == 2) {
            changeCountInTab('delete', 'subtaskTab');
        } else {
            changeCountInTab('add', 'subtaskTab');
        }
    };
    var onGetTeam = function(params, team) {
        jq('#usersActionPanel div.loader32').remove();
        jq('#usersActionPanel').append('<div class="teamListPopup" projectid="' + params.projectid + '"></div>');

        jq('#usersActionPanel .teamListPopup[projectid=' + params.projectid + ']').html('').append('<div><span value="">' + jq('#usersActionPanel').attr('without') + '</span></div>');
        jQuery.each(team, function() {
            jq('#usersActionPanel .teamListPopup[projectid=' + params.projectid + ']').append('<div><span value="' + this.id + '">' + this.displayName + '</span></div>');
            jq('select#ddlPerson').append('<option value="' + this.id + '">' + this.displayName + '</option>');
        });
    };

    var onChangeTaskStatus = function(params, task) {
        displayTaskDescription(task);
        jq("body").css("cursor", "default");
    };
    var onAddTask = function(params, task) {
        document.location = "tasks.aspx?prjID=" + task.projectOwner.id + "&id=" + task.id;
    };
    /*-------tabs-----*/
    var createCurrentTab = function(tab) {
        jq(".taskTabs li").removeClass('current');
        jq(tab).addClass("current");
    };
    var createVisible = function(block) {
        jq("#tabsContent div").removeClass("visible");
        jq(block).addClass("visible");
    };
    var showSubtasks = function() {
        if (currentTab != "subtasks") {
            currentTab = "subtasks";
            createCurrentTab("#subtaskTab");
            createVisible("#subtaskContainer");
            jq("#commentBox").hide();
            jq("#fckbodycontent").val("");
        }
    };
    var showFiles = function() {
        Attachments.loadFiles();
        if (currentTab != "task_files") {
            currentTab = "task_files";
            createCurrentTab("#filesTab");
            createVisible("#filesContainer");
            jq("#commentBox").hide();
            jq("#fckbodycontent").val("");
        }
    };
    var showComments = function() {
        if (currentTab != "task_comments") {
            currentTab = "task_comments";
            createCurrentTab("#commentsTab");
            createVisible("#commentContainer");
            showEmptyCommentsPanel();
        }
    };

    var showEmptyCommentsPanel = function() {
        if (jq("#commentContainer #mainContainer div[id^='container_']").length == 0) {
            jq("#emptyCommentsPanel").show();
        }
        else {
            jq("#add_comment_btn").show();
        }
    };

    var changeCountInTab = function(actionOrCount, tabAnchorId) {
        var currentCount;
        var text = jq.trim(jq("#" + tabAnchorId).children(".count").text());
        if (text == "") currentCount = 0;
        else {
            text = text.substr(1, text.length - 2);
            currentCount = parseInt(text);
        }

        if (typeof (actionOrCount) == "string") {
            if (actionOrCount == "add") {
                currentCount++;
                jq("#" + tabAnchorId).children(".count").text("(" + currentCount + ")");
            }
            else if (actionOrCount == "delete") {
                currentCount--;
                if (currentCount != 0) {
                    jq("#" + tabAnchorId).children(".count").text("(" + currentCount + ")");
                }
                else {
                    jq("#" + tabAnchorId).children(".count").empty();

                    if (tabAnchorId == "subtaskTab") {
                        if (!jq('.subtasks').find('.subtask').length) {
                            showEmptySubtasksPanel();
                            jq('.quickAddSubTaskLink').hide();
                        }
                    }
                    if (tabAnchorId == "commentsTab") {
                        jq("#commentsListWrapper").hide();
                        jq("#emptyCommentsPanel").show();

                    }
                }
            }
        }
        else if (typeof (actionOrCount) == "number") {
            var count = parseInt(actionOrCount);
            if (count > 0) {
                jq("#" + tabAnchorId).children(".count").text("(" + count + ")");
            } else {
                jq("#" + tabAnchorId).children(".count").empty();
            }
        }
    };
    /*---------actions--------*/
    var closeTask = function() {
        jq("body").css("cursor", "wait");

        Teamlab.updatePrjTask({}, taskId, { 'status': 2 }, { success: onChangeTaskStatus });

        jq("#statusTaskMark").removeClass("open");
        jq("#statusTaskMark").addClass("closed");
    };
    var resumeTask = function() {
        jq("body").css("cursor", "wait");

        Teamlab.updatePrjTask({}, taskId, { 'status': 1 }, { success: onChangeTaskStatus });

        jq(".pm_taskTitleClosedByPanel").hide();
        jq("#statusTaskMark").removeClass("closed");
        jq("#statusTaskMark").addClass("open");
    };

    var editTask = function() {
        taskaction.onGetTask({ eventType: "gettask" }, currentTask);
    };

    var formatDescription = function(descr) {
        var formatDescr = descr.replace(/</ig, '&lt;').replace(/>/ig, '&gt;').replace(/\n/ig, '<br/>');
        return formatDescr.replace('&amp;', '&');
    };

    var compareDates = function(data) {
        var currentDate = new Date();
        if (currentDate > data) {
            return true;
        }
        if (currentDate <= data) {
            return false;
        }
    };
    return {
        init: init,

        showSubtasks: showSubtasks,
        showFiles: showFiles,
        showComments: showComments,
        changeCountInTab: changeCountInTab,
        subscribeTask: subscribeTask,
        compareDates: compareDates,

        closeTask: closeTask,
        resumeTask: resumeTask,
        showQuestionWindowTaskRemove: showQuestionWindowTaskRemove,
        editTask: editTask,
        removeTask: removeTask,
        formatDescription: formatDescription,
        setTimeSpent: setTimeSpent,
        showActionsPanel: showActionsPanel
    };


})(jQuery);
