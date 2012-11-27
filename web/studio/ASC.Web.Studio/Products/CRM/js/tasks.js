if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = function() { return {} };


/*******************************************************************************
ListTaskView.ascx
*******************************************************************************/
ASC.CRM.ListTaskView = new function() {

    Teamlab.bind(Teamlab.events.getException, _onGetException);

    function _onGetException(params, errors) {
        console.log('tasks.js ', errors);
        LoadingBanner.hideLoading();
    };

    var _onGetTasksComplete = function() {
        jq("#taskTable tbody tr").remove();


        if (ASC.CRM.ListTaskView.TaskList.length == 0 //There is no tasks at all in the entity
            && (ASC.CRM.ListTaskView.ContactID != 0 || ASC.CRM.ListTaskView.EntityID != 0)) {

            ASC.CRM.ListTaskView.NoTasks = true;
            jq("#taskList").hide();
            jq("#taskButtonsPanel").hide();
            jq("#taskFilterContainer").hide();
            jq("#emptyContentForTasksFilter").hide();
            jq("#tasksEmptyScreen").show();

            LoadingBanner.hideLoading();
            return false;
        }

        if (ASC.CRM.ListTaskView.TaskList.length == 0) {
            jq("#taskList").hide();
            jq("#taskButtonsPanel").show();
            jq("#mainExportCsv").next("img").hide();
            jq("#mainExportCsv").hide();
            jq("#taskFilterContainer").show();
            ASC.CRM.ListTaskView.resizeFilter()
            jq("#emptyContentForTasksFilter").show();

            LoadingBanner.hideLoading();
            return false;
        }

        jq("#emptyContentForTasksFilter").hide();
        jq("#taskButtonsPanel").show();
        jq("#mainExportCsv").next("img").show();
        jq("#mainExportCsv").show();
        jq("#taskFilterContainer").show();
        ASC.CRM.ListTaskView.resizeFilter();
        jq("#taskList").show();

        jq("#taskTmpl").tmpl(ASC.CRM.ListTaskView.TaskList).appendTo("#taskTable tbody");
        ASC.CRM.Common.RegisterContactInfoCard();

        for (var i = 0; i < ASC.CRM.ListTaskView.TaskList.length; i++) {
            ASC.CRM.Common.tooltip("#taskTitle_" + ASC.CRM.ListTaskView.TaskList[i].id, "tooltip", true);
        }
        LoadingBanner.hideLoading();
    };

    var _onGetMoreTasksComplete = function() {
        if (ASC.CRM.ListTaskView.TaskList.length == 0) {
            return false;
        }
        jq("#taskTmpl").tmpl(ASC.CRM.ListTaskView.TaskList).appendTo("#taskTable tbody");
        ASC.CRM.Common.RegisterContactInfoCard();

        for (var i = 0; i < ASC.CRM.ListTaskView.TaskList.length; i++) {
            ASC.CRM.Common.tooltip("#taskTitle_" + ASC.CRM.ListTaskView.TaskList[i].id, "tooltip", true);
        }
    };

    var _renderContent = function() {
        if (ASC.CRM.ListTaskView.NoTasks) {//There is no tasks at all
            jq("#taskList").hide();
            jq("#taskButtonsPanel").hide();
            jq("#taskFilterContainer").hide();
            jq("#emptyContentForTasksFilter").hide();
            jq("#tasksEmptyScreen").show();
            return;
        }

        if (ASC.CRM.ListTaskView.ContactID == 0 && ASC.CRM.ListTaskView.EntityID == 0 && ASC.CRM.ListTaskView.isFirstTime == true) {
            ASC.CRM.ListTaskView.isFirstTime = false;
            if (typeof (tasksForFirstRequest) != "undefined")
                tasksForFirstRequest = jq.parseJSON(jQuery.base64.decode(tasksForFirstRequest));
            else tasksForFirstRequest = [];
            ASC.CRM.ListTaskView.CallbackMethods.get_tasks_by_filter({ startIndex: 0, __nextIndex: tasksForFirstRequest.nextIndex }, Teamlab.create('crm-tasks', null, tasksForFirstRequest.response));
            return;
        }
        LoadingBanner.displayLoading();
        ASC.CRM.ListTaskView.ShowMore = true;
        _getTasks(0);
    };

    var _addRecordsToContent = function() {
        if (!ASC.CRM.ListTaskView.ShowMore) return false;
        ASC.CRM.ListTaskView.TaskList = {};
        jq("#showMoreTasksButtons .crm-showMoreLink").hide();
        jq("#showMoreTasksButtons .crm-loadingLink").show();

        var startIndex = jq("#taskTable tbody tr").length;

        _getTasks(startIndex);
    };

    var _getTasks = function(startIndex) {               
        var filterSettings = {};
        if (ASC.CRM.ListTaskView.ContactID != 0 || ASC.CRM.ListTaskView.EntityID != 0) {

            filterSettings.entityid = ASC.CRM.ListTaskView.ContactID != 0 ? ASC.CRM.ListTaskView.ContactID : ASC.CRM.ListTaskView.EntityID;
            filterSettings.entitytype = ASC.CRM.ListTaskView.EntityType;
            filterSettings.sortBy = 'deadLine';
            filterSettings.sortOrder = 'descending';

        }
        else filterSettings = ASC.CRM.ListTaskView.getFilterSettings();

        filterSettings.Count = ASC.CRM.ListTaskView.CountOfRows;
        filterSettings.startIndex = startIndex;

        Teamlab.getCrmTasks({ startIndex: startIndex || 0 }, { filter: filterSettings, success: ASC.CRM.ListTaskView.CallbackMethods.get_tasks_by_filter });
    };

    var _changeTaskItemStatus = function(task_id, isClosed) {

        Teamlab.updateCrmTask({}, task_id, { id: task_id, isClosed: isClosed },
            {
                success: ASC.CRM.TaskActionView.CallbackMethods.edit_task,
                before: function(params) {
                    jq('#taskStatusListContainer').hide();
                    jq("#task_" + task_id + " .check").hide();
                    jq("#task_" + task_id + " .ajax_edit_task").show();
                    jq("#taskMenu_" + task_id).hide();
                },
                after: function(params) {
                    jq("#task_" + task_id + " .ajax_edit_task").hide();
                    jq("#task_" + task_id + " .check").show();
                    jq("#taskMenu_" + task_id).show();
                }
            });

        if (isClosed) {
            var task = ASC.CRM.ListTaskView.AllTaskList[ASC.CRM.ListTaskView.findIndexOfTaskByID(task_id)];
            var dataEvent = {
                content: jq.format(CRMJSResources.TaskIsOver, task.title),
                categoryId: ASC.CRM.ListTaskView.HistoryCategoryTaskClosed,
                created: Teamlab.serializeTimestamp(new Date())
            };

            if (task.contact != null) dataEvent.contactId = task.contact.id;
            if (task.entity != null) {
                dataEvent.entityId = task.entity.entityId;
                dataEvent.entityType = task.entity.entityType;
            }

            if (task.contact != null || task.entity != null) {

                var callbackMethod;
                if (ASC.CRM.ListTaskView.ContactID == 0 && ASC.CRM.ListTaskView.EntityID == 0)
                    callbackMethod = new function(params, response) { };
                else
                    callbackMethod = ASC.CRM.HistoryView.CallbackMethods.add_event;

                Teamlab.addCrmHistoryEvent({}, dataEvent, callbackMethod);
            }
        }
    };

    return {
        CallbackMethods: {
            get_tasks_by_filter: function (params, tasks) {

                if (typeof(params.__nextIndex) == "undefined") {
                    ASC.CRM.ListTaskView.ShowMore = false;
                }

                ASC.CRM.ListTaskView.TaskList = tasks;

                for (var i = 0, n = tasks.length; i < n; i++) {
                    ASC.CRM.ListTaskView.taskItemFactory(tasks[i]);
                    ASC.CRM.ListTaskView.AllTaskList.push(tasks[i]);
                }
                if (!params.startIndex) {
                    _onGetTasksComplete();
                } else {
                    _onGetMoreTasksComplete();
                }

                jq("#showMoreTasksButtons .crm-loadingLink").hide();
                if (ASC.CRM.ListTaskView.ShowMore) {
                    jq("#showMoreTasksButtons .crm-showMoreLink").show();
                } else {
                    jq("#showMoreTasksButtons .crm-showMoreLink").hide();
                }
            },

            delete_task: function (params, task) {
                jq("#task_" + task.id).animate({opacity: "hide"}, 500);
                //ASC.CRM.Common.changeCountInTab("delete", "tasks");

                setTimeout(function () {
                    jq("#task_" + task.id).remove();

                    var index = ASC.CRM.ListTaskView.findIndexOfTaskByID(task.id);
                    if (index != -1) {
                        ASC.CRM.ListTaskView.AllTaskList.splice(index, 1);
                    }

                    if (ASC.CRM.ListTaskView.AllTaskList.length == 0 && ASC.CRM.ListTaskView.ShowMore == false) {
                        ASC.CRM.ListTaskView.NoTasks = true;
                    }

                    if (jq("#taskTable tbody tr").length == 0) {
                        jq("#taskList").hide();

                        if (ASC.CRM.ListTaskView.NoTasks) { // There is no tasks at all
                            jq("#emptyContentForTasksFilter").hide();
                            jq("#taskFilterContainer").hide();
                            jq("#taskButtonsPanel").hide();
                            jq("#tasksEmptyScreen").show();
                        } else {
                            _renderContent();
                        }
                    }
                }, 500);
            }
        },

        init: function (ContactID, EntityType, EntityID, CountOfRows, HistoryCategoryTaskClosed, NoTasks, Anchor) {
            ASC.CRM.ListTaskView.ContactID = ContactID;
            ASC.CRM.ListTaskView.EntityType = EntityType;
            ASC.CRM.ListTaskView.EntityID = EntityID;

            ASC.CRM.ListTaskView.CountOfRows = CountOfRows;
            ASC.CRM.ListTaskView.HistoryCategoryTaskClosed = HistoryCategoryTaskClosed;


            ASC.CRM.ListTaskView.NoTasks = NoTasks;
            ASC.CRM.ListTaskView.ShowMore = !NoTasks;

            ASC.CRM.ListTaskView.AllTaskList = [];
            ASC.CRM.ListTaskView.TaskList = {};

            ASC.CRM.ListTaskView.isFilterVisible = false;
            ASC.CRM.ListTaskView.isTabActive = false;

            var currentAnchor = location.hash;
            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#' ? currentAnchor.substring(1) : currentAnchor;

            if (currentAnchor == "" || decodeURIComponent(Anchor) == currentAnchor) {
                ASC.CRM.ListTaskView.isFirstTime = true;
            } else {
                ASC.CRM.ListTaskView.isFirstTime = false;
            }

            ASC.CRM.ListTaskView.actionMenuPositionCalculated = false;

            jq("#showMoreTasksButtons .crm-showMoreLink").bind("click", function () {
                _addRecordsToContent();
            });


            var $taskStatusListContainer = jq('#taskStatusListContainer');
            if ($taskStatusListContainer.length === 1) {
                jq.dropdownToggle({
                    dropdownID: 'taskStatusListContainer',
                    switcherSelector: '#taskTable .changeStatusCombobox.canEdit',
                    addTop: -12,
                    addLeft: 4,
                    showFunction: function (switcherObj, dropdownItem) {
                        jq('#taskTable .changeStatusCombobox.selected').removeClass('selected');

                        if (dropdownItem.is(":hidden")) {
                            switcherObj.addClass('selected');
                            if ($taskStatusListContainer.attr('taskid') != switcherObj.attr('taskid')) {
                                $taskStatusListContainer.attr('taskid', switcherObj.attr('taskid'));
                            }
                        }
                    },
                    hideFunction: function () {
                        jq('#taskTable .changeStatusCombobox.selected').removeClass('selected');
                    }
                });

                jq('#taskStatusListContainer li').bind({
                    click:
                        function () {
                            if (jq(this).is('.selected')) {
                                $taskStatusListContainer.hide();
                                jq('#taskTable .changeStatusCombobox.selected').removeClass('selected');
                                return;
                            }
                            var taskid = $taskStatusListContainer.attr('taskid');
                            var status = jq(this).attr('class');
                            if (status == jq('#task_' + taskid + ' .changeStatusCombobox span').attr('class')) {
                                $taskStatusListContainer.hide();
                                jq('#taskTable .changeStatusCombobox.selected').removeClass('selected');
                                return;
                            }
                            _changeTaskItemStatus(taskid, status == "closed");
                        }
                });
            }
        },

        setFilter: function (evt, $container, filter, params, selectedfilters) { _renderContent(); },
        resetFilter: function (evt, $container, filter, selectedfilters) { _renderContent(); },

        activate: function () {
            if (ASC.CRM.ListTaskView.isTabActive == false) {
                ASC.CRM.ListTaskView.isTabActive = true;
                _renderContent();
            }
        },

        resizeFilter: function () {
            var visible = jq("#taskFilterContainer").is(":hidden") == false;
            if (ASC.CRM.ListTaskView.isFilterVisible == false && visible) {
                ASC.CRM.ListTaskView.isFilterVisible = true;
                if (ASC.CRM.ListTaskView.advansedFilter) {
                    jq("#tasksAdvansedFilter").advansedFilter("resize");
                }
            }
        },

        getFilterSettings: function() {
            var settings = {};

            if (ASC.CRM.ListTaskView.advansedFilter.advansedFilter == null) {
                return settings;
            }

            var param = ASC.CRM.ListTaskView.advansedFilter.advansedFilter();

            jq(param).each(function (i, item) {
                switch (item.id) {
                case "sorter":
                    settings.sortBy = item.params.id;
                    settings.sortOrder = item.params.dsc == true ? 'descending' : 'ascending';
                    break;
                case "text":
                    settings.filterValue = item.params.value;
                    break;
                case "fromToDate":
                    settings.fromDate = new Date(item.params.from);
                    settings.toDate = new Date(item.params.to);
                    break;
                default:
                    if (item.hasOwnProperty("apiparamname") && item.params.hasOwnProperty("value") && item.params.value != null) {
                        try {
                            var apiparamnames = jq.parseJSON(item.apiparamname);
                            var apiparamvalues = jq.parseJSON(item.params.value);
                            if (apiparamnames.length != apiparamvalues.length) {
                                settings[item.apiparamname] = item.params.value;
                            }
                            for (var i = 0, len = apiparamnames.length; i < len; i++) {
                                if (apiparamvalues[i].trim().length != 0) {
                                    settings[apiparamnames[i]] = apiparamvalues[i];
                                }
                            }
                        } catch (err) {
                            settings[item.apiparamname] = item.params.value;
                        }
                    }
                    break;
                }
            });
            return settings;
        },

        taskItemFactory: function (taskItem) {
            var nowDate = new Date();
            var todayDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate(), 0, 0, 0, 0);

            if (taskItem.isClosed) {
                taskItem.classForTitle = "headerBaseSmall grayText";
                taskItem.classForTaskDeadline = "grayText";
            } else {
                if (taskItem.deadLine.getHours() != 0 || taskItem.deadLine.getMinutes() != 0) {
                    if (taskItem.deadLine.getTime() < nowDate.getTime()) {
                        taskItem.classForTitle = "headerBaseSmall redText";
                        taskItem.classForTaskDeadline = "redText";
                    } else {
                        taskItem.classForTitle = "headerBaseSmall";
                        taskItem.classForTaskDeadline = "";
                    }
                } else {
                    if (taskItem.deadLine.getTime() < todayDate.getTime()) {
                        taskItem.classForTitle = "headerBaseSmall redText";
                        taskItem.classForTaskDeadline = "redText";
                    } else {
                        taskItem.classForTitle = "headerBaseSmall";
                        taskItem.classForTaskDeadline = "";
                    }
                }
            }

            if (taskItem.entity != null) {
                switch (taskItem.entity.entityType) {
                case "opportunity":
                    taskItem.entityURL = "deals.aspx?id=" + taskItem.entity.entityId;
                    taskItem.entityType = CRMJSResources.Deal;
                    break;
                case "case":
                    taskItem.entityURL = "cases.aspx?id=" + taskItem.entity.entityId;
                    taskItem.entityType = CRMJSResources.Case;
                    break;
                default:
                    taskItem.entityURL = "";
                    taskItem.entityType = "";
                    break;
                }
            }
        },

        deleteTaskItem: function (task_id) {
            if (confirm(CRMJSResources.ConfirmDeleteTask + "\n" + CRMJSResources.DeleteConfirmNote)) {
                Teamlab.removeCrmTask({}, task_id,
                    {
                        success: ASC.CRM.ListTaskView.CallbackMethods.delete_task,
                        before: function (params) {
                            jq("#task_" + task_id + " .check").hide();
                            jq("#task_" + task_id + " .ajax_edit_task").show();
                            jq("#taskMenu_" + task_id).hide();
                        }
                    });
                return true;
            } else {
                return false;
            }
        },

        findIndexOfTaskByID: function (taskID) {
            var length = ASC.CRM.ListTaskView.AllTaskList.length;
            for (var i = 0; i < length; i++) {
                if (ASC.CRM.ListTaskView.AllTaskList[i].id == taskID) {
                    return i;
                }
            }
            return -1;
        },

        showActionMenu: function (taskID, contactID, entityType, entityID) {
            if (ASC.CRM.ListTaskView.actionMenuPositionCalculated === false) {
                ASC.CRM.ListTaskView.actionMenuPositionCalculated = true;
                jq("#taskActionMenu").show();
                var left = jq("#taskActionMenu .dropDownCornerRight").position().left;
                jq("#taskActionMenu").hide();
                jq.dropdownToggle({
                    dropdownID: 'taskActionMenu',
                    switcherSelector: '#taskTable .crm-menu',
                    addTop: 4,
                    addLeft: -left + 6,
                    showFunction: function (switcherObj, dropdownItem) {
                        jq('#taskTable .crm-menu.active').removeClass('active');
                        if (dropdownItem.is(":hidden")) {
                            switcherObj.addClass('active');
                        }
                    },
                    hideFunction: function () {
                        jq('#taskTable .crm-menu.active').removeClass('active');
                    }
                });
            }
            jq("#editTaskLink").unbind("click").bind("click", function () {
                jq("#taskActionMenu").hide();
                jq('#taskTable .crm-menu.active').removeClass('active');
                ASC.CRM.TaskActionView.showTaskPanel(taskID, contactID, entityType, entityID);
            });
            jq("#deleteTaskLink").unbind("click").bind("click", function () {
                jq("#taskActionMenu").hide();
                jq('#taskTable .crm-menu.active').removeClass('active');
                ASC.CRM.ListTaskView.deleteTaskItem(taskID);
            });
        },

        exportToCsv: function () {
            var index = window.location.href.indexOf('#');
            var basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href;
            var anchor = index >= 0 ? window.location.href.substr(index, window.location.href.length) : "";
            jq("#exportDialog").hide();
            window.location.href = basePath + "?action=export" + anchor;
        },

        openExportFile: function () {
            var index = window.location.href.indexOf('#');
            var basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href;
            jq("#exportDialog").hide();
            window.open(basePath + "?action=export&view=editor");
        },

        showExportDialog: function () {
            jq.dropdownToggle().toggle("#mainExportCsv", "exportDialog", 7, -20);
        }
    };
};

/*******************************************************************************
TaskActionView.ascx
*******************************************************************************/
ASC.CRM.TaskActionView = new function() {

    var _changeSelectionDeadlineButtons = function(daysCount) {
        jq("#taskDeadlineContainer").find("a").each(function() {
            jq(this).css("border-bottom", "");
            jq(this).css("font-weight", "normal");
        });

        if (daysCount == 0 || daysCount == 3 || daysCount == 7) {
            jq("#deadline_" + daysCount).css("border-bottom", "none");
            jq("#deadline_" + daysCount).css("font-weight", "bold");
        }
    };

    return {
        CallbackMethods: {
            add_task: function(params, task) {
                var newTask = task;

                ASC.CRM.ListTaskView.taskItemFactory(newTask);
                ASC.CRM.ListTaskView.AllTaskList.push(newTask);

                if (jq("#taskTable tbody tr").length == 0) {
                    jq("#emptyContentForTasksFilter").hide();
                    jq("#tasksEmptyScreen").hide();
                    jq("#taskButtonsPanel").show();
                    jq("#taskFilterContainer").show();
                    ASC.CRM.ListTaskView.resizeFilter();
                    jq("#taskList").show();
                }

                jq("#taskTmpl").tmpl(newTask).prependTo("#taskTable tbody");

                //ASC.CRM.Common.changeCountInTab("add", "tasks");
                ASC.CRM.ListTaskView.NoTasks = false;

                ASC.CRM.Common.tooltip("#taskTitle_" + newTask.id, "tooltip", true);

                PopupKeyUpActionProvider.EnableEsc = true;
                jq.unblockUI();

                ASC.CRM.Common.RegisterContactInfoCard();
            },

            edit_task: function(params, task) {
                var newTask = task;

                ASC.CRM.ListTaskView.taskItemFactory(newTask);
                ASC.CRM.ListTaskView.AllTaskList.push(newTask);

                jq("#task_" + newTask.id).attr("id", "old_task").hide();
                jq("#taskTmpl").tmpl(newTask).insertBefore(jq("#old_task"));
                jq("#old_task").remove();

                var tooltipIsExist = jq("#tooltip" + newTask.id).length > 0;
                ASC.CRM.Common.tooltip("#taskTitle_" + newTask.id, "tooltip", !tooltipIsExist);

                PopupKeyUpActionProvider.EnableEsc = true;
                jq.unblockUI();

                ASC.CRM.Common.RegisterContactInfoCard();
            }
        },

        init: function(dateMask, currentAccountID) {
            ASC.CRM.TaskActionView.CurrentAccountID = currentAccountID;
            jq("#taskDeadline").mask(dateMask);
            jq("#taskDeadline").datepicker({
                onSelect: function(date) {
                    var selectedDate = jq("#taskDeadline").datepicker("getDate");
                    var tmpDate = new Date();
                    var today = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);
                    var daysCount = Math.floor((selectedDate.getTime() - today.getTime()) / (24 * 60 * 60 * 1000));
                    _changeSelectionDeadlineButtons(daysCount);
                    setTimeout(function() {
                        jq('<input type="text" />').insertAfter("#taskDeadline").focus().remove();
                    }, 100);
                }
            });
        },

        showTaskPanel: function(taskID, contactID, entityType, entityID) {
            var index = ASC.CRM.TaskActionView.initPanel(taskID, contactID);
            PopupKeyUpActionProvider.EnableEsc = false;
            HideRequiredError();
            ASC.CRM.Common.blockUI("#addTaskPanel", 650, 600, 0);

            jq("#addTaskPanel input[id$=tbxTitle]").focus();
            jq("#addTaskPanel a.baseLinkButton:first").unbind("click").bind("click", function() {
                ASC.CRM.TaskActionView.saveTask(taskID, entityType, entityID, index);
            });
        },

        keyPress: function(event) {
            var code;
            if (!e) var e = event;
            if (!e) var e = window.event;

            if (e.keyCode) code = e.keyCode;
            else if (e.which) code = e.which;

            if (code >= 48 && code <= 57) {
                jq("#taskDeadlineContainer").find("a").each(function() { jq(this).css("border-bottom", ""); });
            }
        },

        changeDeadline: function(object) {
            var daysCount = parseInt(jq.trim(jq(object).attr('id').split('_')[1]));
            var tmp = new Date();
            var newDate = new Date(tmp.setDate(tmp.getDate() + daysCount));
            jq("#taskDeadline").datepicker('setDate', newDate);

            _changeSelectionDeadlineButtons(daysCount);
        },

        saveTask: function(taskID, entityType, entityID, index) {

            var deadLine = null;
            if (jq.trim(jq("#addTaskPanel input[id$=taskDeadline]").val()) != "") {
                deadLine = jq("#taskDeadline").datepicker('getDate');
                if (parseInt(jq("#taskDeadlineHours option:selected").val()) != -1)
                    deadLine.setHours(parseInt(jq("#taskDeadlineHours option:selected").val()));
                else
                    deadLine.setHours(0);

                if (parseInt(jq("#taskDeadlineMinutes option:selected").val()) != -1)
                    deadLine.setMinutes(parseInt(jq("#taskDeadlineMinutes option:selected").val()));
                else
                    deadLine.setMinutes(0);

                deadLine = Teamlab.serializeTimestamp(deadLine);
            }

            var dataTask = {
                id: taskID,
                title: jq("#tbxTitle").val(),
                description: jq("#tbxDescribe").val(),
                deadline: deadLine,
                responsibleid: taskResponsibleSelector.SelectedUserId,
                categoryid: taskCategorySelector.CategoryID,
                isnotify: jq("#notifyResponsible").is(":checked")
            };

            if (entityID != 0) {
                dataTask.entityType = entityType;
                dataTask.entityid = entityID;
            }
            if (taskContactSelector.SelectedContacts.length == 1)
                dataTask.contactid = taskContactSelector.SelectedContacts[0];

            var isValid = true;
            var invalidTaskTime = (parseInt(jq("#taskDeadlineHours option:selected").val()) == -1 && parseInt(jq("#taskDeadlineMinutes option:selected").val()) != -1) || (parseInt(jq("#taskDeadlineHours option:selected").val()) != -1 && parseInt(jq("#taskDeadlineMinutes option:selected").val()) == -1);

            if (jq.trim(jq("#tbxTitle").val()) == "") {
                AddRequiredErrorText(jq("#tbxTitle"), CRMJSResources.EmptyTaskTitle);
                ShowRequiredError(jq("#tbxTitle"), true);
                isValid = false;
            }
            else RemoveRequiredErrorClass(jq("#tbxTitle"));

            if (dataTask.responsibleid == null) {
                AddRequiredErrorText(jq("#addTaskPanel #inputUserName"), CRMJSResources.EmptyTaskResponsible);
                ShowRequiredError(jq("#addTaskPanel #inputUserName"), true);
                isValid = false;
            }
            else RemoveRequiredErrorClass(jq("#addTaskPanel #inputUserName"));


            if (dataTask.deadline == null || invalidTaskTime) {
                AddRequiredErrorText(jq("#taskDeadline"), CRMJSResources.EmptyTaskDeadline);
                ShowRequiredError(jq("#taskDeadline"), true);

                if (invalidTaskTime) {
                    jq("#taskDeadlineHours").addClass("requiredInputError");
                    jq("#taskDeadlineMinutes").addClass("requiredInputError");
                }
                else {
                    jq("#taskDeadlineHours").removeClass("requiredInputError");
                    jq("#taskDeadlineMinutes").removeClass("requiredInputError");
                }

                isValid = false;
            }
            else {
                RemoveRequiredErrorClass(jq("#taskDeadline"));
                jq("#taskDeadlineHours").removeClass("requiredInputError");
                jq("#taskDeadlineMinutes").removeClass("requiredInputError");
            }

            if (!isValid)
                return false;

            if (taskID != 0) {
                ASC.CRM.ListTaskView.AllTaskList.splice(index, 1);

                Teamlab.updateCrmTask({}, taskID, dataTask,
                {
                    success: ASC.CRM.TaskActionView.CallbackMethods.edit_task,
                    before: function(params) { jq("#addTaskPanel .action_block").hide(); jq("#addTaskPanel .ajax_info_block").show(); },
                    after: function(params) { jq("#addTaskPanel .ajax_info_block").hide(); jq("#addTaskPanel .action_block").show(); }
                });
            }
            else {
                Teamlab.addCrmTask({}, dataTask,
                {
                    success: ASC.CRM.TaskActionView.CallbackMethods.add_task,
                    before: function(params) { jq("#addTaskPanel .action_block").hide(); jq("#addTaskPanel .ajax_info_block").show(); },
                    after: function(params) { jq("#addTaskPanel .ajax_info_block").hide(); jq("#addTaskPanel .action_block").show(); }
                });
            }
        },


        initPanel: function(taskID, contactID) {

            if (taskID > 0) {
                jq("div.noMatches").hide();

                var index = ASC.CRM.ListTaskView.findIndexOfTaskByID(taskID);
                if (index != -1) {
                    var task = ASC.CRM.ListTaskView.AllTaskList[index];
                    jq("#tbxTitle").val(task.title);
                    jq("#tbxDescribe").val(task.description);

                    taskResponsibleSelector.ClearFilter();
                    taskResponsibleSelector.ChangeDepartment(taskResponsibleSelector.Groups[0].ID);

                    var obj;
                    if (!jq.browser.mobile) {
                        obj = document.getElementById("User_" + task.responsible.id);
                        if (obj != null)
                            taskResponsibleSelector.SelectUser(obj);
                    } else {
                        obj = jq("#taskResponsibleSelector select option[value=" + task.responsible.id + "]");
                        if (obj.length > 0) {
                            taskResponsibleSelector.SelectUser(obj);
                            jq(obj).attr("selected", true);
                        }
                    }

                    jq("#taskDeadline").datepicker('setDate', task.deadLine);

                    if (task.deadLine.getHours() == 0 && task.deadLine.getMinutes() == 0) {
                        jq("#optDeadlineHours_-1").attr('selected', true);
                        jq("#optDeadlineMinutes_-1").attr('selected', true);
                    }
                    else {
                        jq("#optDeadlineHours_" + task.deadLine.getHours()).attr('selected', true);
                        jq("#optDeadlineMinutes_" + task.deadLine.getMinutes()).attr('selected', true);
                    }

                    var obj = taskCategorySelector.getRowByContactID(task.category.id);
                    taskCategorySelector.changeContact(obj);

                    if (task.contact != null) {
                        taskContactSelector.setContact(jq("#contactTitle_taskContactSelector_0"), task.contact.id, task.contact.displayName, task.contact.smallFotoUrl);
                        taskContactSelector.showInfoContent(jq("#contactTitle_taskContactSelector_0"));
                        taskContactSelector.SelectedContacts = [];
                        taskContactSelector.SelectedContacts.push(task.contact.id);
                    }
                    else {
                        taskContactSelector.showSelectorContent(jq('#contactTitle_taskContactSelector_0'));
                        taskContactSelector.crossButtonEventClick('taskContactSelector_0');
                        taskContactSelector.SelectedContacts = [];
                    }

                    jq("#addTaskPanel a.baseLinkButton:first").html(CRMJSResources.SaveChanges);
                    jq("#addTaskPanel div.containerHeaderBlock td:first").html(CRMJSResources.EditTask);

                }
                return index;
            }
            else {
                jq("#addTaskPanel a.baseLinkButton:first").html(CRMJSResources.AddThisTask);
                jq("#addTaskPanel div.containerHeaderBlock td:first").html(CRMJSResources.AddNewTask);

                jq("#tbxTitle").val("");
                jq("#tbxDescribe").val("");

                taskResponsibleSelector.ClearFilter();
                taskResponsibleSelector.ChangeDepartment(taskResponsibleSelector.Groups[0].ID);

                var obj;
                if (!jq.browser.mobile) {
                    obj = document.getElementById("User_" + ASC.CRM.TaskActionView.CurrentAccountID);
                    if (obj != null)
                        taskResponsibleSelector.SelectUser(obj);
                } else {
                    obj = jq("#taskResponsibleSelector select option[value=" + ASC.CRM.TaskActionView.CurrentAccountID + "]");
                    if (obj.length > 0) {
                        taskResponsibleSelector.SelectUser(obj);
                        jq(obj).attr("selected", true);
                    }
                }

                jq("#taskDeadline").datepicker('setDate', new Date());
                _changeSelectionDeadlineButtons(0);

                jq("#optDeadlineHours_-1").attr('selected', true);
                jq("#optDeadlineMinutes_-1").attr('selected', true);

                var obj = taskCategorySelector.getRowByContactID(0);
                taskCategorySelector.changeContact(obj);


                if (contactID != 0 && contactForInitTaskActionPanel) {
                    jq("#contactTitle_taskContactSelector_0").removeClass("crm-watermarked");
                    taskContactSelector.setContact(jq("#contactTitle_taskContactSelector_0"), contactForInitTaskActionPanel.id, contactForInitTaskActionPanel.displayName, contactForInitTaskActionPanel.smallFotoUrl);
                    taskContactSelector.showInfoContent(jq("#contactTitle_taskContactSelector_0"));
                }
                else {
                    taskContactSelector.changeContact('taskContactSelector_0');
                    taskContactSelector.crossButtonEventClick('taskContactSelector_0');
                }
                return -1;
            }

        },

        changeTime: function(obj) {
            var id = jq(obj).attr("id");
            var value = jq("#" + id + " option:selected").val();
            if (value == '-1') {
                jq("#optDeadlineHours_-1").attr("selected", "selected");
                jq("#optDeadlineMinutes_-1").attr("selected", "selected");
            }
        }
    }
};

jq(document).ready(function() {
    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintCategories",
        dropdownID: "files_hintCategoriesPanel",
        fixWinSize: false
    });
});