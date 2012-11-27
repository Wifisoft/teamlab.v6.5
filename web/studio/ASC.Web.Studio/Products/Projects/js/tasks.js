window.tasks = (function() {
    var isInit = false,
        paramsList,
        overTaskDescrPanel = false,
        selectedStatusCombobox = undefined,
        statusListContainer = undefined,
        projectParticipants = undefined,
        taskDescrTimeout = 0;

    var basePath = 'sortBy=deadline&sortOrder=ascending';

    var extendSelect = function($select, options) {
        var node;
        for (var i = 0, n = options.length; i < n; i++) {
            node = document.createElement('option');
            $select.append(node);
            node.setAttribute('value', options[i].value);
            node.setAttribute('canCreateTask', options[i].canCreateTask);
            node.setAttribute('isPrivate', options[i].isPrivate);
            node.appendChild(document.createTextNode(options[i].title));
        }

        return $select;
    };

    var initPresets = function() {
        //my tasks
        var path = '#' + basePath + '&tasks_rasponsible=' + serviceManager.getMyGUID() + '&status=open';
        jq('.presetContainer #preset_my').attr('href', path);
        //today
        var date = new Date();
        path = '#' + basePath + '&tasks_rasponsible=' + serviceManager.getMyGUID() + '&status=open&deadlineStart=' + date.getTime() + '&deadlineStop=' + date.getTime();
        jq('.presetContainer #preset_today').attr('href', path);
        //upcoming
        var deadlineStart = date.getTime();
        date.setDate(date.getDate() + 7);
        var deadlineStop = date.getTime();
        path = '#' + basePath + '&tasks_rasponsible=' + serviceManager.getMyGUID() + '&status=open&deadlineStart=' + deadlineStart + '&deadlineStop=' + deadlineStop;
        jq('.presetContainer #preset_upcoming').attr('href', path);
    };

    var onSetFilterTasks = function(evt, $container) {
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            initPresets();
            ASC.Projects.ProjectsAdvansedFilter.presetAlign();
        }
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', serviceManager.projectId);
        var hash = ASC.Controls.AnchorController.getAnchor();
        if (ASC.Projects.ProjectsAdvansedFilter.firstload && hash.length) {
            if (!ASC.Projects.ProjectsAdvansedFilter.coincidesWithFilter(path)) {
                ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
                return;
            }
        }
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            ASC.Projects.ProjectsAdvansedFilter.firstload = false;
        }
        var data = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', serviceManager.projectId);
        serviceManager.getFilteredTasks('tasks', { mode: 'onset' }, data);
        LoadingBanner.displayLoading();
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };

    var onResetFilterTasks = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', serviceManager.projectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        serviceManager.StartIndex = 0;
        var data = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', serviceManager.projectId);
        serviceManager.getFilteredTasks('tasks', { mode: 'onreset' }, data);
        LoadingBanner.displayLoading();
    };

    var changeCountTaskSubtasks = function(taskid, action) {
        var currentCount;
        var text;
        var task = jq(".task[taskid='" + taskid + "']");
        var subtasksCounterContainer = jq(task).find('.subtasksCount');
        var subtasksCounter = jq(subtasksCounterContainer).find('.dottedNumSubtask');

        if (subtasksCounter.length) {
            text = jq.trim(jq(subtasksCounter).text());
            text = text.substr(1, text.length - 1);
        } else {
            text = "";
        }

        if (text == "") {
            currentCount = 0;
        } else {
            currentCount = parseInt(text);
        }

        if (action == "add") {
            currentCount++;
            if (currentCount == 1) {
                jq(subtasksCounterContainer).find('.add').remove();
                subtasksCounter = '<span class="expand" taskid="' + taskid + '"><span class="dottedNumSubtask">+' + currentCount + '</span></span>';
                jq(subtasksCounterContainer).append(subtasksCounter);
            } else {
                jq(subtasksCounter).text('+' + currentCount);
            }
        }
        else if (action == "delete") {
            currentCount--;
            if (currentCount != 0) {
                jq(subtasksCounter).text('+' + currentCount);
            }
            else {
                jq(subtasksCounter).remove();
                var hoverText = jq(subtasksCounterContainer).attr('data');
                jq(subtasksCounterContainer).append('<span class="add" taskid="' + taskid + '">+ ' + hoverText + '</span>');
                jq(task).find('.subtasks').hide();
            }
        }
    };

    var onAddSubtask = function(params, subtask) {
        changeCountTaskSubtasks(params.taskid, 'add');
        subtask.projectid = params.projectid;
        subtasks.addSubtask(params.taskid, subtask);
    };
    var onUpdateSubtask = function(params, subtask) {
        subtask.projectid = params.projectid;
        subtasks.updateSubtask(subtask);
    };
    var onUpdateSubtaskStatus = function(params, subtask) {
        subtask.projectid = params.projectid;
        subtasks.updateSubtaskStatus(subtask);
        var oldSubtask = jq('.subtask[subtaskid="' + subtask.id + '"]');
        var taskid = jq(oldSubtask).attr('taskid');
        if (subtask.status == 2) {
            changeCountTaskSubtasks(taskid, 'delete');
        } else {
            changeCountTaskSubtasks(taskid, 'add');
        }
    };

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }
        subtasks.init();
        //filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(serviceManager.getMyGUID(), basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterTasks;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterTasks;

        LoadingBanner.displayLoading();

        ASC.Controls.AnchorController.bind(/^(.+)*$/, onFilter);

        ServiceManager.bind('extention', onGetException);
        serviceManager.bind('getTeamByProject', onGetTeamByProject);
        Teamlab.bind(Teamlab.events.addPrjTask, onAddTask);
        Teamlab.bind(Teamlab.events.updatePrjTask, onUpdateTask);
        Teamlab.bind(Teamlab.events.getPrjTasks, onGetPrjTasks);
        Teamlab.bind(Teamlab.events.removePrjTask, onRemoveTask);
        Teamlab.bind(Teamlab.events.removePrjSubtask, onRemoveSubtask);
        Teamlab.bind(Teamlab.events.getPrjTeam, onGetTeam);

        serviceManager.bind('gettimespend', onGetTimeSpend);
        serviceManager.bind('addtasktime', onAddTaskTime);

        statusListContainer = jq('#statusListContainer');

        var filterMilestones, filterTags, filterProjects;

        if (typeof (milestones) != 'undefined') {
            milestones = Teamlab.create('prj-milestones', null, jq.parseJSON(jQuery.base64.decode(milestones)).response);
            filterMilestones = onGetMilestones({}, milestones);
        }

        if (serviceManager.projectId == null) {
            if (typeof (tags) != 'undefined') {
                tags = jq.parseJSON(jQuery.base64.decode(tags)).response;
                filterTags = onGetTags(tags);
            }
            if (typeof (projects) != 'undefined') {
                projects = Teamlab.create('prj-projects', null, jq.parseJSON(jQuery.base64.decode(projects)).response);
                onGetProjects(projects);
                filterProjects = onGetProjectsFilter(projects);
            }
            if (filterMilestones && filterTags && filterProjects) {
                createAdvansedFilter(filterMilestones, filterTags, filterProjects);
            }
        }
        else {
            if (filterMilestones) {
                createAdvansedFilter(filterMilestones);
            }
            if (typeof (projectTeam) != 'undefined') {
                projectParticipants = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectTeam)).response);
                onGetTeam({}, projectParticipants);
            }
        }

        jq('.taskList').on('click', '.changeStatusCombobox.canEdit', function(event) {
            var element = (event.target) ? event.target : event.srcElement;
            var status = jq('span', element).attr('class');
            var current = selectedStatusCombobox !== undefined ? selectedStatusCombobox.attr('taskid') : -1;
            jq('.taskList .task[taskid=' + current + '] .changeStatusCombobox').removeClass('selected');
            selectedStatusCombobox = jq(this);

            if (statusListContainer.attr('taskid') !== selectedStatusCombobox.attr('taskid')) {
                statusListContainer.attr('taskid', selectedStatusCombobox.attr('taskid'));
                showStatusListContainer(status, element);
            } else {
                toggleStatusListContainer(status);
            }
            return false;
        });

        jq('#statusListContainer li').on('click', function() {
            if (jq(this).is('.selected')) return;
            var taskid = jq('#statusListContainer').attr('taskid');
            var status = jq(this).attr('class');
            if (status == jq('.taskList .task[taskid=' + taskid + '] .changeStatusCombobox span').attr('class')) return;
            if (status == 'closed') {
                if (jq('.taskList .subtask[taskid=' + taskid + ']').length &&
                jq('.taskList .subtask[taskid=' + taskid + ']').length != jq('.taskList .subtask.closed[taskid=' + taskid + ']').length) {
                    popupWindow(taskid);
                } else {
                    closeTask(taskid);
                }
            } else {
                jq('.taskList .task[taskid=' + taskid + '] .check').html('');
                jq('.taskList .task[taskid=' + taskid + '] .check').append('<div class="taskProcess"></div>');
                serviceManager.updateTaskStatus('changetaskstatus', { 'status': 1 }, taskid);
            }
        });

        jq('.taskList').on('click', '.task .user', function() {
            var path = '';
            if (jq(this).hasClass('not')) {
                path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'noresponsible', true);
                path = jq.removeParam('tasks_rasponsible', path);
            } else {
                path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'tasks_rasponsible', jq(this).attr('value'));
                path = jq.removeParam('noresponsible', path);
            }
            path = jq.removeParam('group', path);
            ASC.Controls.AnchorController.move(path);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
        });

        jq('#othersListPopup').on('click', '.user', function() {
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'tasks_rasponsible', jq(this).attr('value'));
            path = jq.removeParam('noresponsible', path);
            ASC.Controls.AnchorController.move(path);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
        });

        jq('#taskDescrPanel .project .value').on('click', function() {
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project', jq(this).attr('projectid'));
            path = jq.removeParam('milestone', path);
            ASC.Controls.AnchorController.move(path);
        });

        jq('#taskDescrPanel .milestone .value').on('click', function() {
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'milestone', jq(this).attr('milestone'));
            ASC.Controls.AnchorController.move(path);
        });

        jq('.addTask').on('click', function() {
            showNewTaskPopup();
            return false;
        });

        jq('.taskList').on('click', '.noContentBlock .addFirstElement', function() {
            showNewTaskPopup();
            return false;
        });

        var showNewTaskPopup = function() {
            jq('.actionPanel').hide();
            jq('#addTaskPanel .notify').show();
            jq('#addTaskPanel .baseLinkButton').html(jq('#addTaskPanel .baseLinkButton').attr('add'));
            jq('#addTaskPanel .containerHeaderBlock table td:first').html(ProjectJSResources.CreateNewTask);
            taskaction.showTaskForm(false);
        };

        jq('.taskList').on('click', '.noContentBlock .clearFilterButton', function() {
            ASC.Controls.AnchorController.move(basePath);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
            return false;
        });

        jq('.taskList').on('click', '.task .other', function() {
            jq('#othersListPopup').html(jq('.taskList .task .others[taskid="' + jq(this).attr('taskid') + '"]').html());
            showActionsPanel('othersPanel', this);
        });

        jq('#moveTaskPanel .pm-action-block a.baseLinkButton').on('click', function() {
            var data = {},
            taskid = jq('#moveTaskPanel').attr('taskid');
            data.title = jq.trim(jq('.task[taskid=' + taskid + '] .taskName a').text());
            data.responsible = jq('.task[taskid=' + taskid + '] div.user').attr('value');
            if (jq('.taskName[taskid=' + taskid + '] .high_priority').length) data.priority = 1;
            if (jq('.taskList .task .others[taskid=' + taskid + '] div').length) {
                data.responsibles = [];
                jq('.taskList .task .others[taskid=' + taskid + '] div').each(function() {
                    data.responsibles.push(jq(this).attr('value'));
                });
            }
            var deadline = jq(".task[taskid=" + taskid + "] .taskName a").attr("data-deadline");
            if (deadline.length) {
                var date = new Date(deadline);
                data.deadline = Teamlab.serializeTimestamp(date);
            }

            data.milestoneid = jq('#moveTaskPanel .milestonesList input:checked').attr('value');
            data.description = jq('.task[taskid=' + taskid + '] .taskName a').attr('description');
            serviceManager.updateTask('updatetask', data, taskid);
            jq.unblockUI();
            return false;
        });

        jq('.quickAddTaskLink span').on('click', function() {
            jq('.quickAddSubTaskField').hide();
            jq('.quickAddSubTaskLink[visible="true"]').show();
            jq('.subtasks:has(.quickAddSubTaskLink[visible="false"])').hide();
            jq('.taskList .subtask .taskName:hidden').show();

            jq('.quickAddTaskLink').hide();
            jq('.quickAddTaskField').show();
            jq('.actionPanel').hide();
            jq('.quickAddTaskField #taskName').focus();
            return false;
        });

        jq('#taskActionPanel #ta_accept').on('click', function() {
            jq('.actionPanel').hide();

            var taskId = jq('#taskActionPanel').attr('objid');
            var taskRow = jq('.task[taskid=' + taskId + ']');
            var taskLink = taskRow.find(".taskName a");

            var data = {};
            data.title = jq.trim(taskRow.find(".taskName a").text());

            var deadline = taskRow.find(".deadline span").attr("deadline");
            if (deadline) {
                data.deadline = new Date(deadline.replace(/(\d+).(\d+).(\d+)/, '$3/$2/$1'));
                data.deadline.setHours(0);
                data.deadline.setMinutes(0);
            }
            var description = taskLink.attr("description");
            if (description) {
                data.description = description;
            }
            var milestoneId = taskLink.attr("milestoneid");
            if (milestoneId) {
                data.milestoneid = milestoneId;
            }

            data.priority = taskRow.find(".high_priority").length ? 1 : 0;
            data.responsible = serviceManager.getMyGUID();
            serviceManager.updateTask('updatetask', data, taskId);
            return false;
        });

        jq('#taskActionPanel #ta_edit').on('click', function() {
            serviceManager.getTask('gettask', jq('#taskActionPanel').attr('objid'));
            return false;
        });

        jq('#taskActionPanel #ta_subtask').on('click', function() {
            subtasks.hideSubtaskFields();
            var taskid = jq('#taskActionPanel').attr('objid');
            separateSubtasks(taskid);
            jq('.quickAddSubTaskLink[taskid=' + taskid + ']').hide();
            jq('.subtasks[taskid=' + jq('#taskActionPanel').attr('objid') + ']').show();
            jq('.task[taskid=' + jq('#taskActionPanel').attr('objid') + ']').addClass('borderbott');

            jq('.quickAddSubTaskField[taskid=' + jq('#taskActionPanel').attr('objid') + ']').show();
            jq('.quickAddSubTaskField[taskid=' + jq('#taskActionPanel').attr('objid') + '] input').focus();

            jq('.actionPanel').hide();
            jq('.taskList .task .menupoint').removeClass('show');
            return false;
        });

        jq('#taskActionPanel #ta_move').on('click', function() {
            jq('.actionPanel').hide();
            if (serviceManager.projectId == null) {
                serviceManager.getMilestonesByProject({}, jq(this).attr('projectid'), onGetMilestonesForMovePanel);
            } else {
                showMoveToMilestonePanel();
            }
            return false;
        });

        jq('#taskActionPanel #ta_remove').on('click', function() {
            var taskid = jq('#taskActionPanel').attr('objid');
            jq('.actionPanel').hide();
            showQuestionWindowTaskRemove(taskid);
            return false;
        });

        jq('#taskActionPanel #ta_mesres').on('click', function() {
            var taskid = jq('#taskActionPanel').attr('objid');
            serviceManager.messageResponsible('', taskid);
            jq('.actionPanel').hide();
            return false;
        });

        jq('#taskActionPanel #ta_time').on('click', function() {
            jq('.actionPanel').hide();

            var taskid = jq('#taskActionPanel').attr('objid'),
                projectid = jq('#taskActionPanel #ta_time').attr('projectid'),
                userid = jq('#taskActionPanel #ta_time').attr('userid');
            ASC.Projects.TimeSpendActionPage.showTimer('timetracking.aspx?prjID=' + projectid + '&taskId=' + taskid + '&userID=' + userid + '&action=timer');

            return false;
        });

        jq('#timeTracking .pm-action-block a.baseLinkButton').on('click', function() {
            var data = {},
                taskid = jq('#timeTracking').attr('taskid');
            data.note = jq('#timeTracking #tbxNote').val();
            data.date = jq('#timeTracking #tbxDate').val();
            data.personId = serviceManager.getMyGUID();
            data.hours = parseInt(jq('#timeTracking #tbxHours').val());
            if (serviceManager.projectId == null)
                data.projectId = jq('#timeTracking').attr('projectid');
            else
                data.projectId = serviceManager.projectId;

            serviceManager.addTaskTime('addtasktime', data, taskid);
        });

        jq('#subtaskActionPanel #sta_edit').on('click', function() {
            subtasks.editSubtask(jq('#subtaskActionPanel').attr('objid'));
            jq('.actionPanel').hide();
            return false;
        });

        jq('#subtaskActionPanel #sta_accept').on('click', function() {
            var subtaskid = jq('#subtaskActionPanel').attr('objid');
            var taskid = jq('.taskList .subtask[subtaskid=' + subtaskid + ']').attr('taskid');
            var data = {};
            data.title = jq.trim(jq('.subtask[subtaskid=' + subtaskid + '] .taskName span').text());
            data.responsible = serviceManager.getMyGUID();
            data.description = '';
            var projectid = jq('.quickAddSubTaskField[taskid=' + taskid + ']').closest(".subtasks").attr("projectid");
            Teamlab.updatePrjSubtask({ projectid: projectid }, taskid, subtaskid, data, { success: onUpdateSubtask });
            jq('#subtaskActionPanel').hide();
        });

        jq('.taskList').on('keydown', '#quickAddSubTaskField .subTaskName', function(e) {
            var taskid, subtaskid;
            if (e.which == 13) {
                if (!jq.trim(jq(this).val()).length) {
                    return undefined;
                }
                taskid = jq(this).attr('taskid');
                subtaskid = jq(this).attr('subtaskid');
                var data = {};
                data.title = jq.trim(jq(this).val());
                data.responsible = jq('#quickAddSubTaskField .choose').attr('value');
                var projectid = jq('.quickAddSubTaskField[taskid=' + taskid + ']').closest(".subtasks").attr("projectid");
                Teamlab.updatePrjSubtask({ projectid: projectid }, taskid, subtaskid, data, { success: onUpdateSubtask });
                jq('#quickAddSubTaskField').remove();
                return false;
            } else if (e.which == 27) {
                jq('.taskList .subtask .taskName[subtaskid=' + jq('#quickAddSubTaskField').attr('subtaskid') + ']').show();
                jq('#quickAddSubTaskField').remove();
                jq(".subtask .taskName").show();
            } else if (e.which == 9) {
                taskid = jq(this).attr('taskid');
                subtaskid = jq(this).attr('subtaskid');
                if (serviceManager.projectId == null) {
                    getTeamByProject(jq('.quickAddSubTaskField[taskid=' + taskid + ']').attr('projectid'));
                    showActionsPanel('usersActionPanel', jq('.quickAddSubTaskField .choose[subtaskid=' + subtaskid + ']'));
                } else {
                    showActionsPanel('usersActionPanel', jq('.quickAddSubTaskField .choose[subtaskid=' + subtaskid + ']'));
                }
            }
        });

        jq('#subtaskActionPanel #sta_remove').on('click', function() {
            var subtaskid = jq('#subtaskActionPanel').attr('objid');
            var taskid = jq('.taskList .subtask[subtaskid=' + subtaskid + ']').attr('taskid');

            serviceManager.removeSubTask('updateSubtasks', taskid, subtaskid);

            jq('.taskList .subtask[subtaskid=' + subtaskid + ']').remove();
            jq('.actionPanel').hide();
            return false;
        });

        jq('.taskList').on('mouseenter', '.task .taskName a', function(event) {
            taskDescrTimeout = setTimeout(function() {
                var targetObject = event.target;
                jq('#taskDescrPanel .descr .value .readMore').hide();
                jq('#taskDescrPanel .date, #taskDescrPanel .milestone, #taskDescrPanel .descr, #taskDescrPanel .createdby, #taskDescrPanel .descr, #taskDescrPanel .closed, #taskDescrPanel .descr, #taskDescrPanel .closedby').hide();
                if (jq(targetObject).attr('status') == 2) {
                    if (typeof jq(targetObject).attr('updated') != 'undefined') {
                        if (jq(targetObject).attr('updated').length) {
                            jq('#taskDescrPanel .closed .value').html(jq(targetObject).attr('updated').substr(0, 10));
                            jq('#taskDescrPanel .closed').show();
                        }
                        if (jq(targetObject).attr('createdby').length) {
                            jq('#taskDescrPanel .closedby .value').html(jq(targetObject).attr('createdby'));
                            jq('#taskDescrPanel .closedby').show();
                        }
                    }
                } else {
                    if (typeof jq(targetObject).attr('created') != 'undefined') {
                        if (jq(targetObject).attr('created').length) {
                            jq('#taskDescrPanel .date .value').html(jq(targetObject).attr('created').substr(0, 10));
                            jq('#taskDescrPanel .date').show();
                        }
                    }
                    if (typeof jq(targetObject).attr('createdby') != 'undefined') {
                        jq('#taskDescrPanel .createdby .value').html(jq(targetObject).attr('createdby'));
                        jq('#taskDescrPanel .createdby').show();
                    }
                }

                if (jq('#taskDescrPanel .project').length) {
                    jq('#taskDescrPanel .project .value').html('<span class="descr_milestone">' + jq(targetObject).attr('project') + '</span>');
                    jq('#taskDescrPanel .project .value').attr('projectid', jq(targetObject).attr('projectid'));
                }
                if (typeof jq(targetObject).attr('milestone') != 'undefined') {
                    jq('#taskDescrPanel .milestone .value').html('<span class="descr_milestone">' + jq(targetObject).attr('milestone') + '</span>');
                    jq('#taskDescrPanel .milestone .value').attr('projectid', jq(targetObject).attr('projectid'));
                    jq('#taskDescrPanel .milestone .value').attr('milestone', jq(targetObject).attr('milestoneid'));
                    jq('#taskDescrPanel .milestone').show();
                    jq('#taskDescrPanel .milestone .value').removeClass('deadline_active').removeClass('deadline_late');
                    if (jq('.taskList .task[taskid=' + jq(targetObject).attr('taskid') + '] .deadline').length) jq('#taskDescrPanel .milestone .value').addClass('deadline_active');
                }
                var description = jq(targetObject).attr('description');
                if (jq.trim(description) != '') {
                    jq('#taskDescrPanel .descr .value div').html(jq.linksParser(jq(targetObject).attr('description').replace(/</ig, '&lt;').replace(/>/ig, '&gt;').replace(/\n/ig, '<br/>').replace('&amp;', '&')));
                    if (description.indexOf("\n") > 2 || description.length > 80) {
                        var link = "tasks.aspx?prjID=" + jq(targetObject).attr('projectid') + "&id=" + jq(targetObject).attr('taskid');
                        jq('#taskDescrPanel .descr .value .readMore').attr("href", link);
                        jq('#taskDescrPanel .descr .value .readMore').show();
                    }
                    jq('#taskDescrPanel .descr').show();
                }

                showActionsPanel('taskDescrPanel', targetObject);
                overTaskDescrPanel = true;
            }, 400, this);
        });

        jq('.taskList').on('mouseleave', '.task .taskName a', function() {
            clearTimeout(taskDescrTimeout);
            overTaskDescrPanel = false;
            hideDescrPanel();
        });

        jq('#taskDescrPanel').on('mouseenter', function() {
            overTaskDescrPanel = true;
        });

        jq('#taskDescrPanel').on('mouseleave', function() {
            overTaskDescrPanel = false;
            hideDescrPanel();
        });

        jq('.taskList').on('click', '.task .menupoint', function() {
            jq('.taskList .task .menupoint').removeClass('show');
            if (jq('#taskActionPanel:visible').length) jq(this).removeClass('show'); else jq(this).addClass('show');
            showActionsPanel('taskActionPanel', this);

            // ga-track
            try {
                if (window._gat) {
                    window._gaq.push(['_trackEvent', ga_Categories.tasks, ga_Actions.actionClick, "task-menu"]);
                }
            } catch (err) {
            }
            return false;
        });

        jq('.taskList').on('click', '.subtask .menupoint', function() {
            if (jq('#subtaskActionPanel:visible').length) jq(this).removeClass('show'); else jq(this).addClass('show');
            showActionsPanel('subtaskActionPanel', this);
            return false;
        });

        jq('.taskList').on('dblclick', '.subtask .taskName', function() {
            subtasks.hideSubtaskFields();
            var subtaskId = jq(this).attr('subtaskid');
            if (!jq(this).is('.canedit') || jq(this).parents('.subtask').is('.closed')) return false;
            subtasks.editSubtask(subtaskId);
            var projid = jq(this).closest('.subtask').attr('projectid');
            jq('#quickAddSubTaskField').attr('projectid', projid);
            return true;
        });

        jq('.taskList').on('click', '.subtasksCount span.expand', function() {
            var taskId = jq(this).attr('taskid');
            var subtasks = jq('.subtasks[taskid=' + taskId + ']');

            var closedSubtask = subtasks.find('.subtask.closed:first');
            if (closedSubtask.length) {
                subtasks.find('.quickAddSubTaskField').insertBefore(closedSubtask);
                subtasks.find('.quickAddSubTaskLink').insertBefore(closedSubtask);
            }

            jq(this).attr('class', 'collaps');
            jq('.taskList .task[taskid=' + taskId + ']').addClass('borderbott');
            subtasks.show('blind', { direction: 'vertical' }, 'fast', function() { });
            return false;
        });

        jq('.taskList').on('click', '.subtasksCount span.collaps', function() {
            var taskId = jq(this).attr('taskid');

            jq(this).attr('class', 'expand');
            jq('.taskList .task[taskid=' + taskId + ']').removeClass('borderbott');
            jq('.subtasks[taskid=' + taskId + ']').hide('blind', { direction: 'vertical' }, 'fast', function() { });
            return false;
        });

        jq('.taskList').on('click', '.task .taskPlace', function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            if (jq(elt).is('a')) {
                return undefined;
            }
            var taskid = jq(jq(this).children('.taskName')).attr('taskid');
            var subtaskCont = jq('.taskList .subtasks[taskid=' + taskid + ']');
            if (jq(subtaskCont).find('.subtask').length) {
                if (jq(subtaskCont).is(':visible')) {
                    jq('.taskList .subtasksCount span.collaps[taskid=' + taskid + ']').attr('class', 'expand');
                    jq('.taskList .task[taskid=' + taskid + ']').removeClass('borderbott');
                    jq('.subtasks[taskid=' + taskid + ']').hide('blind', { direction: 'vertical' }, 'fast', function() {
                    });
                } else {
                    separateSubtasks(taskid);
                    jq('.taskList .subtasksCount span.expand[taskid=' + taskid + ']').attr('class', 'collaps');
                    jq('.taskList .task[taskid=' + taskid + ']').addClass('borderbott');
                    if (!jq('.taskList .task[taskid=' + taskid + ']').hasClass('closed')) {
                        jq(subtaskCont).find('.quickAddSubTaskLink').show();
                        jq(subtaskCont).find('.quickAddSubTaskField').hide();
                    }
                    jq(subtaskCont).show('blind', { direction: 'vertical' }, 'fast', function() {
                    });
                }
            }
            return false;
        });

        jq('.taskList').on('click', '.subtasksCount span.add', function() {
            subtasks.hideSubtaskFields();
            var taskid = jq(this).attr('taskid');
            var subtaskCont = jq('.subtasks[taskid=' + taskid + ']');

            jq('.quickAddSubTaskLink[taskid="' + taskid + '"]').hide();
            jq('.quickAddSubTaskLink[taskid="' + taskid + '"]').after(jq('.quickAddSubTaskField[taskid="' + taskid + '"]'));
            jq('.quickAddSubTaskField[taskid="' + taskid + '"]').show();
            jq('.taskList .task[taskid=' + taskid + ']').addClass('borderbott');
            jq('#subtaskResponsibleContainer').appendTo(jq(subtaskCont).find('.quickAddSubTaskField')).show().find('select').val(-1).change();

            if (!jq(subtaskCont).is(':visible')) {
                separateSubtasks(taskid);
                jq('.subtasks[taskid=' + taskid + ']').show('blind', { direction: 'vertical' }, 'fast', function() { jq('.quickAddSubTaskField[taskid="' + jq(this).attr('taskid') + '"] #subTaskName').focus(); });
            }

            return false;
        });

        jq('.taskList').on('click', '.quickAddSubTaskField .chooseWrap', function() {
            if (serviceManager.projectId == null) {
                var projid = jq('.quickAddSubTaskField[taskid=' + jq(this).find('.choose').attr('taskid') + ']').attr('projectid');
                getTeamByProject(projid);
                showActionsPanel('usersActionPanel', this);
            } else {
                showActionsPanel('usersActionPanel', this);
            }
            return false;
        });

        jq('.taskList').on('change', '.subtask .check input', function() {
            var data = {},
          taskid = jq(this).attr('taskid'),
          subtaskid = jq(this).attr('subtaskid');
            if (jq(this).is(':checked')) {
                closeSubTask(taskid, subtaskid);
            } else {
                data.status = 'open';

                jq('.taskList .subtask[subtaskid=' + subtaskid + '] .check input').hide();
                jq('.taskList .subtask[subtaskid=' + subtaskid + '] .check').append('<div class="taskProcess"></div>');
                var projectid = jq('.quickAddSubTaskField[taskid=' + taskid + ']').closest(".subtasks").attr("projectid");
                Teamlab.updatePrjSubtask({ taskid: taskid, projectid: projectid }, taskid, subtaskid, data, { success: onUpdateSubtaskStatus });
            }
        });

        jq('.taskList').on('click', '.quickAddSubTaskLink span', function() {
            var taskid = jq(this).attr('taskid');
            jq('.quickAddTaskField').hide();
            jq(".subtask .taskName").show();
            jq('.quickAddTaskLink').show();
            jq('.taskList .subtask .taskName[subtaskid=' + jq('#quickAddSubTaskField').attr('subtaskid') + ']').show();
            jq('#quickAddSubTaskField').remove();

            jq('.quickAddSubTaskField').hide();
            jq('.quickAddSubTaskLink[visible="true"]').show();

            var link = jq('.quickAddSubTaskLink[taskid="' + taskid + '"]');
            var field = jq('.quickAddSubTaskField[taskid="' + taskid + '"]');
            jq(link).hide();
            jq(link).after(field);
            jq(field).show();
            jq('.quickAddSubTaskField[taskid="' + taskid + '"] #subTaskName').focus();
            jq('.actionPanel').hide();

            projectid = jq('.taskList .subtasks .subtask[taskid=' + taskid + ']').attr('projectid');
            if (jq('.teamListPopup[projectid=' + projectid + '] div span').length) {
                value = jq('.teamListPopup[projectid=' + projectid + '] div span[value!=""]:first').attr('value');
                var name = jq('.teamListPopup[projectid=' + projectid + '] div span[value!=""]:first').text();
                jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose').attr('value', value);
                jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose span').html(name);
            }

            return false;
        });

        jq('.taskList').on('keydown', '.quickAddSubTaskField #subTaskName', function(e) {
            var taskid = jq(this).attr('taskid');
            jq('.actionPanel').hide();
            if (e.which == 13) {
                if (!jq.trim(jq(this).val()).length) {
                    return undefined;
                }
                jq('.taskList .subtaskSaving[taskid=' + taskid + ']').insertBefore(jq('.taskList .quickAddSubTaskField[taskid=' + taskid + ']'));
                jq('.taskList .subtaskSaving[taskid=' + taskid + ']').show();
                jq('.taskList .quickAddSubTaskField[taskid=' + taskid + '] input').attr('disabled', 'true');
                var data = {};
                data.title = jq.trim(jq(this).val());
                data.responsible = jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose').attr('value');
                var projectid = jq('.quickAddSubTaskField[taskid=' + taskid + ']').closest(".subtasks").attr("projectid");
                Teamlab.addPrjSubtask({ 'taskid': taskid, projectid: projectid }, taskid, data, { success: onAddSubtask });
                return false;
            } else if (e.which == 27) {
                setTimeout(function() { jq('.quickAddSubTaskField #subTaskName').val(''); }, 0);
                jq('.quickAddSubTaskLink[taskid="' + taskid + '"]').show();
                jq('.quickAddSubTaskField[taskid="' + taskid + '"]').hide();
                jq(".subtask .taskName").show();
                if (!jq('.subtasks[taskid=' + taskid + '] .subtask').length) {
                    jq('.subtasks[taskid=' + taskid + ']').hide();
                    jq('.task[taskid=' + taskid + ']').removeClass('borderbott');
                }
            }
        });

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            var $elt = jq(elt);

            if (
			  $elt.is('.actionPanel') ||
			  $elt.is('#taskName') ||
			  $elt.is('.subTaskName') ||
			  $elt.is('#subTaskName') ||
			  $elt.is('.choose') ||
			  $elt.is('.choose > *') ||
			  $elt.is('.combobox-title') ||
			  $elt.is('.combobox-title-inner-text') ||
			  $elt.is('.option-item')
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
                hideStatusListContainer();
            }
        });

        jq('#showNextTasks').on('click', function() {
            jq('#showNextTasks').hide();
            jq('#showNextTaskProcess').show();
            serviceManager.getFilteredTasks('tasks', { mode: 'next' }, null, serviceManager.Count, jq('.taskList .task:last').attr('taskid'));
        });

        jq('#questionWindowTaskRemove .cancel').on('click', function() {
            jq.unblockUI();
            return false;
        });

        jq('#questionWindowTaskRemove .remove').on('click', function() {
            var taskId = jq('#questionWindowTaskRemove').attr('taskId');
            jq('.taskList .task[taskid=' + taskId + ']').html('<div class="taskProcess"></div>');
            serviceManager.removeTask('removetask', taskId);
            jq('#questionWindowTaskRemove').removeAttr('taskId');
            jq.unblockUI();
            return false;
        });
        // ga-track-events

        //add
        jq(".addTask").trackEvent(ga_Categories.tasks, ga_Actions.createNew, 'create-new-task');

        //presets
        jq('.presetContainer #preset_my').trackEvent(ga_Categories.tasks, ga_Actions.presetClick, 'my-task');
        jq('.presetContainer #preset_today').trackEvent(ga_Categories.tasks, ga_Actions.presetClick, 'today');
        jq('.presetContainer #preset_upcoming').trackEvent(ga_Categories.tasks, ga_Actions.presetClick, 'upcoming');

        //show next
        jq("#showNextTasks").trackEvent(ga_Categories.tasks, ga_Actions.next, 'next-tasks');

        //filter
        jq("#AdvansedFilter .advansed-filter-list li[data-id='me_tasks_rasponsible'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'me-tasks-rasponsible');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='tasks_rasponsible'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'tasks-rasponsible');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='group'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'group');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='noresponsible'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'noresponsible');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='open'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'open');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='closed'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'closed');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'my-projects');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='project'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'project');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='tag'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'tag');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='overdue'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'overdue');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='today'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'today');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='upcoming'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'upcoming');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='deadline'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'user-period');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='mymilestones'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'my-milestones');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='milestone'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'milestone');

        jq("#AdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'sort');

        jq("#AdvansedFilter .advansed-filter-input").keypress(function(e) {
            if (e.which == 13) {
                try {
                    if (window._gat) {
                        window._gaq.push(['_trackEvent', ga_Categories.tasks, ga_Actions.filterClick, 'text']);
                    }
                } catch (err) {
                }
            }
            return true;
        });

        //change status
        jq("#statusListContainer .open").trackEvent(ga_Categories.tasks, ga_Actions.changeStatus, "open");
        jq("#statusListContainer .closed").trackEvent(ga_Categories.tasks, ga_Actions.changeStatus, "closed");

        //responsible
        jq(".user span").trackEvent(ga_Categories.tasks, ga_Actions.userClick, "tasks-responsible");

        //actions in menu
        jq("#ta_accept").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "accept");
        jq("#ta_mesres").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "notify-responsible");
        jq("#ta_move").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "move-in-milestone");
        jq("#ta_time").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "time-track");
        jq("#ta_subtask").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "add-subtask");
        jq("#ta_remove").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "remove");
        jq("#ta_edit").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "edit");

        //actions
        jq(".subtasksCount .add").trackEvent(ga_Categories.tasks, ga_Actions.quickAction, "add-subtask");
        //end ga-track-events
    };

    var onGetTimeSpend = function(data) {
        var json = jQuery.parseJSON(data);
        var totalHoursCount = 0;
        var response = json.response;
        if (response.length)
            for (var i = 0; i < response.length; i++) {
            totalHoursCount += parseInt(response[i].Hours);
        }
        taskid = jq('#timeTracking').attr('taskid');
        jq('#timeTracking #TotalHoursCount').html(totalHoursCount);
        jq('#timeTracking #tbxHours').val(1);
        jq('#timeTracking #tbxNote').val('');
        jq('select#ddlPerson').val('');
    };

    var onAddTaskTime = function(data) {
        var json = jQuery.parseJSON(data);
        if (typeof json.error == 'object') {
            alert(json.error.message);
        } else {
            jq.unblockUI();
        }
    };

    var onUpdatetaskBeforeChangeStatus = function(params, data) {
        var taskid = data.id;
        jq('.taskList .task[taskid=' + taskid + ']:first').remove();
        jq('.taskList .subtasks[taskid=' + taskid + ']').replaceWith(jq('#taskTemplate').tmpl(data));
        serviceManager.updateTaskStatus('changetaskstatus', { 'status': 2 }, taskid);
    };

    var onFilter = function(params) {
        paramsList = params;
    };

    var compareDates = function(data) {
        var currentDate = new Date();
        if (currentDate > data) {
            return true;
        }
        else return false;
    };

    var emptyScreenList = function(isItems) {
        var pre = (ASC.Controls.AnchorController.getAnchor() == basePath) ? ':last' : ':first';

        if (isItems === undefined) {
            var tasks = jq('.taskList .task');
            if (tasks.length != 0) {
                isItems = true;
            }
        }

        if (isItems) {
            jq('.taskList .noContentBlock').remove();
            jq('#AdvansedFilter').show();
            jq('.presetContainer').show();
            jq('.taskListHeaders').show();
            jq('.addTask').show();
        } else {
            jq('.noContentBlock' + pre).clone().appendTo('.taskList');
            jq('.taskList .noContentBlock').show();
            if (pre == ':last') {
                jq('#AdvansedFilter').hide();
                jq('.presetContainer').hide();
                jq('.taskListHeaders').hide();
                jq('.addTask').hide();
            }
        }
    };

    var onGetPrjTasks = function(params, data) {
        jq('#SubTasksBody').height('auto');
        jq('#SubTasksBody .taskSaving').hide();
        jq('#SubTasksBody .quickAddTaskField').hide();
        jq('#SubTasksBody .quickAddTaskLink').show();
        jq('#SubTasksBody .quickAddTaskField #taskName').val('');

        clearTimeout(taskDescrTimeout);
        overTaskDescrPanel = false;
        hideDescrPanel();

        var tasks = data;
        if (params.mode != 'next') {
            jq('#SubTasksBody .taskList').html('');
        }

        if (!tasks.length) {
            jq('.taskListHeaders').hide();
        } else {
            jq('.taskListHeaders').show();
        }
        emptyScreenList(tasks.length);

        jq('#showNextTaskProcess').hide();
        if (tasks.length > serviceManager.Count) {
            delete tasks[tasks.length - 1];
            jq('#showNextTasks').show();
        } else {
            jq('#showNextTasks').hide();
        }

        jq('#SubTasksBody .taskList').height('auto');

        if (tasks.length) {
            jq('#taskTemplate').tmpl(tasks).appendTo('.taskList');
        }

        if (serviceManager.projectId == null) {
            jq('#SubTasksBody .choose.project span').html(jq('#SubTasksBody .choose.project').attr('choose'));
            jq('#SubTasksBody .choose.project').attr('value', '');
            jq('.quickAddTaskField .choose.responsible span').html(jq('.quickAddTaskField .choose.responsible').attr('choose'));
            jq('.quickAddTaskField .choose.responsible').attr('value', '');
        }

        LoadingBanner.hideLoading();
    };

    var onGetTags = function(tags) {
        var tagsFilter = [];
        if (tags.length)
            for (var i = 0; i < tags.length; i++) {
            tagsFilter.push({ 'value': tags[i].id, 'title': tags[i].title });
        }

        return tagsFilter;
    };

    var onGetProjects = function(projects) {
        var projectsFilter = [];
        if (projects.length)
        for (var i = 0; i < projects.length; i++) {
            if (projects[i].canCreateTask || (projects[i].isPrivate && projects[i].canCreateTask)) {
                projectsFilter.push({ 'value': projects[i].id, 'title': projects[i].title, 'canCreateTask': projects[i].canCreateTask, 'isPrivate': projects[i].isPrivate });
            }
        }
        jq('#taskProject option[value!=0][value!=-1]').remove();
        extendSelect(jq('#taskProject'), projectsFilter).tlcombobox();

        jq('#projectsListPopup div span').on('click', function() {
            jq('.quickAddTaskField .choose.project').attr('value', jq(this).attr('value'));
            getTeamByProject(jq(this).attr('value'));
            if (jq(this).attr('value').length) {
                jq('.quickAddTaskField .choose.project span').html(jq(this).html());
                jq('.quickAddTaskField .choose.responsible').show();
                jq('.quickAddTaskField .choose.responsible span').html(jq('.quickAddTaskField .choose.responsible').attr('choose'));
                jq('.quickAddTaskField .choose.responsible').attr('value', '');
            } else {
                jq('.quickAddTaskField .choose.project span').html(jq('.quickAddTaskField .chooses').attr('choose'));
                jq('.quickAddTaskField .choose.responsible').hide();
            }

            jq('.quickAddTaskField input').focus();
            jq('.actionPanel').hide();
        });
    };

    var onGetProjectsFilter = function(projects) {
        var projectsFilter = [];
        if (projects.length)
            for (var i = 0; i < projects.length; i++) {
            projectsFilter.push({ 'value': projects[i].id, 'title': projects[i].title });
        }

        return projectsFilter;
    };

    var onGetTeamByProject = function(data) {
        var json = jQuery.parseJSON(data);

        var team = json.response;
        jq('select#ddlPerson option').remove();
        if (team.length) {
            for (var i = 0; i < team.length; i++) {
                jq('select#ddlPerson').append('<option value="' + team[i].id + '">' + team[i].displayName + '</option>');
            }
        }
    };

    var onGetMilestones = function(params, milestones) {
        var milestonesFilter = [];
        jq('#milestoneTemplate').tmpl(milestones).prependTo('#moveTaskPanel .milestonesList');
        jq('.milestonesList div:even').addClass('tintMedium');
        jq('.milestonesList div:odd').addClass('tintLight');

        milestonesFilter.push({ 'value': 0, 'title': ProjectJSResources.NoMilestone });
        if (milestones.length)
            for (var i = 0; i < milestones.length; i++) {
            milestonesFilter.push({ 'value': milestones[i].id, 'title': milestones[i].displayDateDeadline + ' ' + milestones[i].title });
        }

        return milestonesFilter;
    };

    var onGetMilestonesForMovePanel = function(params, milestones) {
        jq('#moveTaskPanel .milestonesList .ms').remove();
        jq('#milestoneTemplate').tmpl(milestones).prependTo('#moveTaskPanel .milestonesList');
        jq('.milestonesList div:even').addClass('tintMedium');
        jq('.milestonesList div:odd').addClass('tintLight');
        showMoveToMilestonePanel();
    };

    var onGetTeam = function(params, team) {
        var taskid = 0;

        jq('#usersActionPanel div.loader32').remove();
        jq('#usersActionPanel').append('<div class="teamListPopup" projectid="' + params.projectid + '"></div>');

        jq('#usersActionPanel .teamListPopup[projectid=' + params.projectid + ']').html('').append('<div><span value="">' + jq('#usersActionPanel').attr('without') + '</span></div>');
        jQuery.each(team, function() {
            jq('#usersActionPanel .teamListPopup[projectid=' + params.projectid + ']').append('<div><span value="' + this.id + '">' + Encoder.htmlEncode(this.displayName) + '</span></div>');
            jq('select#ddlPerson').append('<option value="' + this.id + '">' + Encoder.htmlEncode(this.displayName) + '</option>');
        });
        jq('.teamListPopup[projectid=' + params.projectid + '] div span').on('click', function() {
            taskid = jq('#usersActionPanel').attr('objid');
            if (taskid > 0) { //subtask
                jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose').attr('value', jq(this).attr('value'));
                if (jq(this).attr('value').length)
                    jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose span').html(jq(this).html());
                else
                    jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose span').html(jq('.quickAddSubTaskField[taskid=' + taskid + '] .choose').attr('choose'));
                jq('.quickAddSubTaskField[taskid=' + taskid + '] input').focus();
            } else { //task
                jq('.quickAddTaskField .choose.responsible').attr('value', jq(this).attr('value'));
                if (jq(this).attr('value').length)
                    jq('.quickAddTaskField .choose.responsible span').html(jq(this).html());
                else
                    jq('.quickAddTaskField .choose.responsible span').html(jq('.quickAddTaskField .choose.responsible').attr('choose'));
                jq('.quickAddTaskField input').focus();
            }
            jq('.actionPanel').hide();
        });
    };

    var onRemoveTask = function(params, data) {
        var taskid = params.taskid;
        jq('.taskList .task[taskid=' + taskid + ']').remove();
        jq('.taskList .subtasks[taskid=' + taskid + ']').remove();

        if (typeof data != 'undefined') {
            emptyScreenList(data.length);
        } else {
            emptyScreenList(0);
        }
    };

    var onAddTask = function(params, data) {
        jq.unblockUI();
        jq('.taskList .noContentBlock').remove();
        jq('#taskTemplate').tmpl(data).prependTo('.taskList');
        jq('#SubTasksBody .taskSaving').hide();
        jq('#SubTasksBody .quickAddTaskLink').show();
        jq('#SubTasksBody .quickAddTaskField #taskName').val('');
        jq('#SubTasksBody .quickAddTaskField').hide();
        jq('.taskList .task:first').yellowFade();
        emptyScreenList(true);
    };

    var onGetException = function(eventType, params, error) {
        if (eventType == 'onupdateprjtask' || eventType == 'onaddprjtask') {
            jq('#addTaskPanel .infoPanel.warn').show().text(error[0]);
            jq('#addTaskPanel .pm-action-block').show();
            jq('#addTaskPanel .pm-ajax-info-block').hide();
            jq('#addTaskPanel #addtask_title').removeAttr('disabled');
            jq('#addTaskPanel #addtask_description').removeAttr('disabled');
            jq('#addTaskPanel .notify').show();
            jq('#addTaskPanel #priority-1').removeAttr('disabled');
            jq('#addTaskPanel #priority0').removeAttr('disabled');
            jq('#addTaskPanel #priority1').removeAttr('disabled');
            jq('#addTaskPanel #taskDeadline').removeAttr('disabled');
            return;
        }
    };

    var onUpdateTask = function(params, data) {
        if (params.eventType == 'changetaskstatus') {
            onChangeTaskStatus(params, data);
            return;
        } else if (params.eventType == 'updatetaskBeforeChangeStatus') {
            onUpdatetaskBeforeChangeStatus(params, data);
            return;
        }
        var taskid = data.id;
        jq('.taskList .task[taskid=' + taskid + ']:first').remove();
        jq('#taskTemplate').tmpl(data).insertBefore('.taskList .subtasks[taskid=' + taskid + ']');
        jq('.taskList .subtasks[taskid=' + taskid + ']:first').remove();
        jq('.taskList .task[taskid=' + taskid + ']').yellowFade();
        if (data.subtasks.length && jq('.taskList .subtasks:visible[taskid=' + taskid + ']').length) {
            jq('.taskList .task[taskid=' + taskid + ']').addClass('borderbott');
        }
        jq.unblockUI();
    };

    var onRemoveSubtask = function(params, subtask) {
        var taskid = params.taskid;
        var subtasksCont = jq('.taskList .subtasks[taskid=' + taskid + ']');
        jq(subtasksCont).find('.subtask[taskid=' + subtask.id + ']').remove();
        if (subtask.status == 1) {
            changeCountTaskSubtasks(taskid, 'delete');
        }
        if (!jq(subtasksCont).find('.subtask').length) {
            jq('.taskList .subtasksCount span.collaps[taskid=' + taskid + ']').attr('class', 'expand');
            jq('.taskList .task[taskid=' + taskid + ']').removeClass('borderbott');
            jq(subtasksCont).hide('blind', { direction: 'vertical' }, 'fast', function() {
            });
        }
    };

    var onChangeTaskStatus = function(params, data) {
        var status = params.status,
            taskid = params.taskid;

        if (status == 1) {
            jq('.taskList .subtasks[taskid=' + taskid + ']:first').remove();
            jq('.taskList .task[taskid=' + taskid + ']:first').replaceWith(jq('#taskTemplate').tmpl(data));
            setTimeout(function() { jq('.taskList .task[taskid=' + taskid + ']').yellowFade(); }, 0);
        } else {
            jq('.taskList .subtasks[taskid=' + taskid + ']:first').remove();
            jq('.taskList .task[taskid=' + taskid + ']:first').replaceWith(jq('#taskTemplate').tmpl(data));
            jq('.taskList .subtasks[taskid=' + taskid + ']').hide();
            setTimeout(function() { jq('.taskList .task.closed[taskid=' + taskid + ']').yellowFade(); }, 0);
        }
    };

    var popupWindow = function(taskid) {
        jq('#questionWindow .end').attr('taskid', taskid);
        jq('#questionWindow .cancel').attr('taskid', taskid);
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({
            message: jq('#questionWindow'),
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
                jq('#questionWindow .cancelButton').on('click', function() {
                    jq('.taskList .task[taskid=' + jq('#questionWindow .cancel').attr('taskid') + '] .check input').removeAttr('checked');
                });
            }
        });
        jq('#questionWindow .cancel').on('click', function() {
            jq('.taskList .task[taskid=' + jq(this).attr('taskid') + '] .check input').removeAttr('checked');
            jq.unblockUI();
            return false;
        });
        jq('#questionWindow .end').on('click', function() {
            closeTask(jq(this).attr('taskid'));
            jq.unblockUI();
            return false;
        });
    };
    var separateSubtasks = function(taskid) {
        var subtasksCont = jq('.subtasks[taskid="' + taskid + '"]');
        var closedSubtasks = jq(subtasksCont).find('.subtask.closed');
        jq(jq(subtasksCont).find('.st_separater')).after(closedSubtasks);
    };
    var showQuestionWindowTaskRemove = function(taskId) {
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq('#questionWindowTaskRemove'),
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
        jq('#questionWindowTaskRemove').attr('taskId', taskId);
    };

    var closeTask = function(taskid) {
        jq('.taskProcess').remove();
        jq('.taskList .task[taskid=' + taskid + '] .check').html('');
        jq('.taskList .task[taskid=' + taskid + '] .check').append('<div class="taskProcess"></div>');
        if (!jq('.taskList .task[taskid=' + taskid + '] div.user').length) { //if responcible doesn`t exist - accept task             
            var data = {};
            data.title = jq.trim(jq('.task[taskid=' + taskid + '] .taskName a').text());
            data.responsible = serviceManager.getMyGUID();
            data.description = jq('.task[taskid=' + taskid + '] .taskName a').attr('description');
            serviceManager.updateTask('updatetaskBeforeChangeStatus', data, taskid);
        } else {
            serviceManager.updateTaskStatus('changetaskstatus', { 'status': 2 }, taskid);
        }
    };

    var closeSubTask = function(taskid, subtaskid) {
        var data = {};
        jq('.taskList .subtask[subtaskid=' + subtaskid + '] .check input').hide();
        jq('.taskList .subtask[subtaskid=' + subtaskid + '] .check').append('<div class="taskProcess"></div>');

        data.title = jq.trim(jq('.subtask[subtaskid=' + subtaskid + '] .taskName span').text());
        data.responsible = serviceManager.getMyGUID();
        setTimeout(function() { jq('.taskList .subtask[subtaskid=' + subtaskid + ']').yellowFade(); }, 0);
        data.status = 'closed';
        var projectid = jq('.quickAddSubTaskField[taskid=' + taskid + ']').closest(".subtasks").attr("projectid");
        Teamlab.updatePrjSubtask({ taskid: taskid, projectid: projectid }, taskid, subtaskid, data, { success: onUpdateSubtaskStatus });
    };

    var showActionsPanel = function(panelId, obj) {
        var objid = '',
            objidAttr = '';
        var x, y;
        if (typeof jq(obj).attr('projectid') != 'undefined') {
            jq('#taskActionPanel #ta_move').attr('projectid', jq(obj).attr('projectid'));
            jq('#taskActionPanel #ta_time').attr('projectid', jq(obj).attr('projectid'));
        }
        if (typeof jq(obj).attr('userid') != 'undefined') {
            jq('#taskActionPanel #ta_time').attr('userid', jq(obj).attr('userid'));
        }
        if (panelId == 'taskActionPanel') objid = jq(obj).attr('taskid');
        else if (panelId == 'subtaskActionPanel') objid = jq(obj).attr('subtaskid');
        if (objid.length) objidAttr = '[objid=' + objid + ']';
        if (jq('#' + panelId + ':visible' + objidAttr).length && panelId != 'taskDescrPanel' && panelId != 'subTaskDescrPanel') {
            jq('body').off('click');
            jq('.actionPanel').hide();
        } else {
            jq('.actionPanel').hide();
            jq('#' + panelId).show();
            if (panelId == 'usersActionPanel') {
                x = jq(obj).offset().left - 7;
                y = jq(obj).offset().top + 20;
                var taskId = jq(obj).closest('.quickAddSubTaskField').attr('taskId');
                jq('#' + panelId).attr('objid', taskId);
            } else if (panelId == 'taskDescrPanel') {
                x = jq(obj).offset().left + 10;
                jq('#' + panelId).attr('objid', jq(obj).attr('taskid'));
            } else if (panelId == 'subTaskDescrPanel') {
                x = jq(obj).offset().left;
                y = jq(obj).offset().top + 26;
                jq('#' + panelId).attr('objid', jq(obj).attr('taskid'));
            } else if (panelId == 'projectsActionPanel') {
                x = jq(obj).offset().left - 40;
                y = jq(obj).offset().top + 26;
                jq('#' + panelId).attr('objid', jq(obj).attr('taskid'));
            } else if (panelId == 'othersPanel') {
                x = jq(obj).offset().left - 133;
                y = jq(obj).offset().top + 26;
            } else {
                x = jq(obj).offset().left - 173;
                jq('#' + panelId).attr('objid', objid);
                jq('#taskActionPanel .pm').show();
                jq('#subtaskActionPanel .pm').show();

                var task = jq('.task[taskid=' + objid + ']');
                var taskUser = jq(task).find(".user");
                if (task.length) { //if it`s tasks menu        
                    if (jq(task).hasClass('closed')) {
                        jq('#taskActionPanel .pm').hide(); 
                        jq('#taskActionPanel #ta_time').show();
                        jq('#taskActionPanel #ta_remove').show();
                    } else if (taskUser.length == 1) {
                        if (jq(taskUser).hasClass("not") || jq(taskUser).attr('value') == serviceManager.getMyGUID()) {
                            jq('#taskActionPanel #ta_mesres').hide();
                        }
                        if (!jq(taskUser).hasClass("not")) {
                            jq('#taskActionPanel #ta_accept').hide();
                        }
                    } else {
                        jq('#taskActionPanel #ta_mesres').show();
                    }
                }
                if (jq('.subtask[subtaskid=' + objid + ']').length) { //if it`s subtasks menu
                    if (jq('.subtask[subtaskid=' + objid + ']').hasClass('closed')) {
                        jq('#subtaskActionPanel .pm').hide();
                        jq('#subtaskActionPanel #sta_remove').show();
                    }
                    if (jq('.subtask[subtaskid=' + objid + '] .user').length && !jq('.subtask[subtaskid=' + objid + '] .user').hasClass("not")) {
                        jq('#subtaskActionPanel #sta_accept').hide();
                    }
                }

                if (jq('.task[taskid=' + objid + ']').length) {
                    switch (jq(obj).attr('canWork')) {
                        case '0':
                            jq('#taskActionPanel #ta_edit').hide();
                            jq('#taskActionPanel #ta_remove').hide();
                            jq('#taskActionPanel #ta_move').hide();
                            jq('#taskActionPanel #ta_time').hide();
                            jq('#taskActionPanel #ta_subtask').hide();
                            break;
                        case '1':
                            jq('#taskActionPanel #ta_edit').hide();
                            jq('#taskActionPanel #ta_remove').hide();
                            jq('#taskActionPanel #ta_move').hide();
                            break;
                        case '2':
                            jq('#taskActionPanel #ta_mesres').hide();
                            jq('#taskActionPanel #ta_remove').hide();
                            break;
                    }
                }
                if (jq('.task[taskid=' + objid + ']').length) {
                    if (jq(obj).attr('canWork') == 2) {
                        jq('#taskActionPanel #sta_remove').hide();
                    }
                }
            }

            if (typeof y == 'undefined')
                y = jq(obj).offset().top + 20;
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
                    jq('.taskList .menupoint').removeClass('show');
                }
            });
        }
    };

    var showMoveToMilestonePanel = function() {
        var taskid = jq('#taskActionPanel').attr('objid');
        jq('#moveTaskPanel').attr('taskid', taskid);
        var margintop = jq(window).scrollTop() - 100;
        margintop = margintop + 'px';
        jq.blockUI({
            message: jq('#moveTaskPanel'),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '550px',

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-275px',
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
                var taskId = jq('#moveTaskPanel').attr('taskid');
                jq('#moveTaskPanel b#moveTaskTitles').text(jq(".taskList .task[taskid=" + taskId + "] .taskName a").text());
                var milestoneid = jq('.taskList .task[taskid=' + taskId + ']').attr('milestoneid');

                if (typeof milestoneid != 'undefined')
                    jq('#moveTaskPanel .milestonesList input#ms_' + milestoneid).attr('checked', 'true');
                else {
                    jq('#moveTaskPanel .milestonesList input#ms_0').attr('checked', 'true');
                }
            }
        });
    };

    var getTeamByProject = function(projectid) {
        jq('#usersActionPanel .teamListPopup').hide();
        if (jq('#usersActionPanel .teamListPopup[projectid=' + projectid + ']').length) {
            jq('#usersActionPanel .teamListPopup[projectid=' + projectid + ']').show();
        } else {
            jq('#usersActionPanel').append('<div class="loader32"></div>');
            if (typeof projectParticipants != 'undefined') {
                jq('select#ddlPerson option').remove();
                if (projectParticipants.length) {
                    for (var i = 0; i < projectParticipants.length; i++) {
                        jq('select#ddlPerson').append('<option value="' + projectParticipants[i].id + '">' + projectParticipants[i].displayName + '</option>');
                    }
                }
            } else {
                serviceManager.getTeamByProject('team', { 'projectid': projectid }, projectid);
            }

        }
    };

    var openedCount = function(items) {
        var c = 0;
        for (var i = 0; i < items.length; i++) {
            if (items[i].status != 2) c++;
        }
        return c;
    };

    var hideDescrPanel = function() {
        setTimeout(function() {
            if (!overTaskDescrPanel) jq('#taskDescrPanel').hide(100);
        }, 200);
    };

    var showStatusListContainer = function(status) {
        selectedStatusCombobox.addClass('selected');
        var top = selectedStatusCombobox.offset().top + 11;
        var left = selectedStatusCombobox.offset().left + 3;
        statusListContainer.css({ left: left, top: top });

        if (status == 'overdue' || status == 'active') {
            status = 'open';
        }
        var currentStatus = statusListContainer.find('li.' + status);
        currentStatus.addClass('selected');
        currentStatus.siblings().removeClass('selected');

        statusListContainer.show();
    };

    var toggleStatusListContainer = function(status) {
        if (statusListContainer.is(':visible')) {
            selectedStatusCombobox.removeClass('selected');
        } else {
            selectedStatusCombobox.addClass('selected');
        }

        if (status == 'overdue' || status == 'active') {
            status = 'open';
        }
        var currentStatus = statusListContainer.find('li.' + status);

        currentStatus.addClass('selected');
        currentStatus.siblings().removeClass('selected');

        if (statusListContainer.is(':visible')) {
            statusListContainer.hide();
        } else {
            showStatusListContainer(status);
        }
    };

    var hideStatusListContainer = function() {
        if (statusListContainer.is(':visible')) {
            selectedStatusCombobox.removeClass('selected');
        }
        statusListContainer.hide();
    };

    return {
        init: init,
        openedCount: openedCount,
        showActionsPanel: showActionsPanel,
        compareDates: compareDates,
        onRemoveTask: onRemoveTask
    };
})(jQuery);


jq(function() {

    jQuery.fn.yellowFade = function() {
        this.css({ backgroundColor: '#ffffcc' });
        this.animate({ backgroundColor: '#ffffff' }, { queue: false, duration: 1000 });
        var resetStyle = function(self) { jq(self).removeAttr('style'); };
        setTimeout(resetStyle, 1100, this);
    };

    tasks.init();
});
