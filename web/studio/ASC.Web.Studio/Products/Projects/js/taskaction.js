window.taskaction = (function() {
    var 
    isInit = false;

    var extendSelect = function($select, options) {
        var node;
        for (var i = 0, n = options.length; i < n; i++) {
            node = document.createElement('option');
            node.setAttribute('value', options[i].value);
            jq(node).html(options[i].title);
            $select.append(node);
        }

        return $select;
    };

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }

        Teamlab.bind(Teamlab.events.getPrjTask, onGetTask);
        Teamlab.bind(Teamlab.events.getPrjTeam, onGetTeamFF);

        jq('#addTaskPanel #addtask_description').autoResize();

        jq('#taskProject').on('change', function() {
            if (jq(this).val() > 0) {
                jq('.popupActionPanel').hide();
                jq('#addTaskPanel .requiredErrorText.project').hide();
                jq('#pm-milestoneBlock').show();
                jq('.pm-headerLeft.userAddHeader').show();
                jq('.pm-fieldRight.userAdd').show();

                jq('#fullFormUserList').html('').hide();

                serviceManager.getMilestonesByProject({ eventType: 'getMilestonesFF' }, jq(this).val(), onGetMilestonesFF);

                if (typeof projectTeam != 'undefined') {
                    var team = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectTeam)).response);
                    onGetTeamFF(null, team);
                } else {
                    serviceManager.getTeamByProject('getTeamFF', {}, jq(this).val());
                }

            }
            if (jq('#taskProject option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskProject)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskProject)').removeClass('left-align');
            }
        });

        jq('#taskMilestone').on('change', function() {
            if (jq('#taskMilestone option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskMilestone)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskMilestone)').removeClass('left-align');
            }
        });

        jq('#addTaskPanel #userSelector_switcher').on('click', function() {
            if (!jq('span', this).hasClass('disable')) jq('#userSelector_dropdown').show();
        });

        jq('#addTaskPanel').on('click', '#fullFormUserList .user', function() {
            value = jq(this).attr('value');
            jq(this).remove();
            jq('#taskResponsible').tlcombobox(true);
            jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').show();
            if (!jq('#fullFormUserList div.user').length) jq('#fullFormUserList').hide();
            if (jq('#addTaskPanel #fullFormUserList div.user').length == jq("#addTaskPanel #fullFormUserList div.user[value=" + serviceManager.getMyGUID() + "]").length) {
                jq('#addTaskPanel .notify').hide();
            }
        });

        jq('#addTaskPanel #milestone_switcher').on('click', function() {
            if (!jq('span', this).hasClass('disable')) jq('#milestone_dropdown').show();
        });

        jq('#milestone_dropdown span').on('click', function() {
            jq('#milestone_switcher span').html(jq(this).html());
            jq('#milestone_switcher span').attr('value', jq(this).attr('value'));
            jq('.popupActionPanel').hide();
        });

        jq('#addTaskPanel .deadline_left').on('click', function() {
            jq('#addTaskPanel .deadline_left').css('border-bottom', '1px dotted').css('font-weight', 'normal');
            jq(this).css('border-bottom', 'none').css('font-weight', 'bold');
            var daysCount = parseInt(jq(this).attr('value'));
            var date = new Date();
            date.setDate(date.getDate() + daysCount);
            jq('#taskDeadline').datepicker('setDate', date);
        });

        jq('#addTaskPanel .baseLinkButton').on('click', function() {
            jq('#addTaskPanel .infoPanel.warn').hide();
            jq('#addTaskPanel .titlePanel').removeClass('requiredFieldError');
            jq('#addTaskPanel .requiredErrorText').html('');
            jq('#addTaskPanel .requiredErrorText.project').hide();
            var data = {};
            data.title = jq.trim(jq('#addTaskPanel #addtask_title').val());
            data.description = jq('#addTaskPanel #addtask_description').val();
            if (jq('#addTaskPanel #fullFormUserList div').length)
                data.responsible = jq('#addTaskPanel #fullFormUserList div').attr('value');
            else
                data.responsible = '';

            if (jq('#addTaskPanel #fullFormUserList div').length) {
                data.responsibles = [];
                jq('#addTaskPanel #fullFormUserList div').each(function() {
                    data.responsibles.push(jq(this).attr('value'));
                });
            }

            if (jq('#addTaskPanel input#notify').is(':checked')) {
                data.notify = true;
            }

            data.milestoneid = jq('#taskMilestone').val();
            if (data.milestoneid == -1) data.milestoneid = 0;
            data.priority = jq('#addTaskPanel input[name="priority"]').is(':checked') ? 1 : 0;
            if (jq('#addTaskPanel #taskDeadline').val().length) {
                data.deadline = jq('#addTaskPanel #taskDeadline').datepicker('getDate');
                data.deadline.setHours(0);
                data.deadline.setMinutes(0);
                data.deadline = Teamlab.serializeTimestamp(data.deadline);
            }

            var isError = false;
            if (!data.title.length) {
                jq('#addTaskPanel .titlePanel').addClass('requiredFieldError');
                jq('#addTaskPanel .requiredErrorText.title').html(jq('#addTaskPanel .requiredErrorText').attr('error'));
                isError = true;
            }
            if (serviceManager.projectId == null && !(jq('#taskProject').val() > 0)) {
                jq('#addTaskPanel .requiredErrorText.project').show().html(jq('#addTaskPanel .requiredErrorText.project').attr('error'));
                isError = true;
            }
            else {
                data.projectId = jq('#taskProject').val();
            }
            if (isError) { return undefined; }

            jq('#addTaskPanel .pm-action-block').hide();
            jq('#addTaskPanel .pm-ajax-info-block').show();
            jq('#addTaskPanel #addtask_title').attr('disabled', 'true');
            jq('#addTaskPanel #addtask_description').attr('disabled', 'true');
            jq('#addTaskPanel .notify').hide();
            jq('#addTaskPanel #priority1').attr('disabled', 'true');
            jq('#addTaskPanel #taskDeadline').attr('disabled', 'true');

            if (jq('#addTaskPanel').attr('type') == 'edit') {
                serviceManager.updateTask('updatetask', data, jq('#addTaskPanel').attr('taskid'));
            } else
                if (serviceManager.projectId == null) {
                var projectId = jq('#taskProject').val();
                serviceManager.addTaskToProject('addtask', {}, data, projectId);
            } else {
                serviceManager.addTask('addtask', {}, data);
            }
        });
    };

    var clearAddTaskForm = function(action) {
        jq('#taskProject').tlcombobox();
        jq('#taskResponsible').tlcombobox();
        jq('#taskMilestone').tlcombobox();
        jq('#addTaskPanel #addtask_title').val('');
        jq('#addTaskPanel #addtask_description').val('');
        jq('#addTaskPanel .infoPanel.warn').hide();
        jq('#addTaskPanel .deadline_left').css('border-bottom', '1px dotted').css('font-weight', 'normal');
        jq('#addTaskPanel input[name="priority"]').removeAttr('checked');
        jq('[id$=taskDeadline]').val('');
        jq('#addTaskPanel .titlePanel').removeClass('requiredFieldError');
        jq('#addTaskPanel .requiredErrorText').html('');
        jq('#addTaskPanel .requiredErrorText.project').hide('');

        if (!jq('#taskDeadline').hasClass('hasDatepicker') || action == 'add') {
            jq('#taskDeadline').datepicker({ selectDefaultDate: false, onSelect: function() { jq('#taskDeadline').blur(); } });
        }

        jq('#addTaskPanel .infoPanel.warn').hide();
        jq('#addTaskPanel .pm-action-block').show();
        jq('#addTaskPanel .pm-ajax-info-block').hide();
        jq('#addTaskPanel #addtask_title').removeAttr('disabled');
        jq('#addTaskPanel #addtask_description').removeAttr('disabled');
        jq('#addTaskPanel .notify').show();
        jq('#addTaskPanel #priority1').removeAttr('disabled');
        jq('#addTaskPanel #taskDeadline').removeAttr('disabled');

        if (jq('#addTaskPanel #fullFormUserList div.user').length == jq("#addTaskPanel #fullFormUserList div.user[value=" + serviceManager.getMyGUID() + "]").length) {
            jq('#addTaskPanel .notify').hide();
        }

        if (typeof action == 'undefined') action = 'add';
        if (action == 'edit' || jq('#addTaskPanel').attr('type') == 'edit') {
            jq('#addTaskPanel #fullFormUserList div').remove();
            jq('#fullFormUserList').hide();
            jq('#taskProject').val('-1').change();
            jq('#milestone_switcher span').html(jq('#milestone_switcher span').attr('select'));
            jq('#milestone_switcher span').attr('value', '');
            jq('#addTaskPanel #taskMilestone').val(-1).change();
        }
        if (action == 'add') {
            value = serviceManager.getMyGUID();
            if (!jq('#addTaskPanel #fullFormUserList div').length && jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').length) {
                jq('#fullFormUserList').show();
                jq('#fullFormUserList').append('<div value="' + value + '" class="user">' + jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').attr('title') + '</div>');
                jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').hide();
                jq('#addTaskPanel .notify').hide();
            }
        }

        if (!(jq('#taskProject').val() > 0) || jq('#addTaskPanel').attr('type') == 'edit') {
            if (serviceManager.projectId == null) {
                jq('#pm-milestoneBlock').hide();
                jq('.pm-headerLeft.userAddHeader').hide();
                jq('.pm-fieldRight.userAdd').hide();
            }
        }

        jq('#addTaskPanel #taskMilestone').removeAttr('val');
        jq('#addTaskPanel').removeAttr('type');
    };

    var onGetTask = function(params, task) {
        if (params.eventType != 'gettask') return;
        jq('.actionPanel').hide();
        clearAddTaskForm('edit');
        jq('#addTaskPanel').attr('taskid', task.id);
        jq('#addTaskPanel #addtask_title').val(task.title);
        jq('#addTaskPanel #addtask_description').val(task.description);

        if (task.milestone != null) {
            jq('#addTaskPanel #taskMilestone').attr('val', task.milestone.id);
        }
        if (jq('#taskProject').length) {
            jq('#taskProject').val(task.projectOwner.id).change();
        } else {
            if (task.milestone != null)
                jq('#addTaskPanel #taskMilestone').val(task.milestone.id).change();
        }

        jq('#addTaskContainer .notify').remove();

        if (task.responsibles.length) {
            jq('#fullFormUserList').show();
            jQuery.each(task.responsibles, function() {
                jq('#fullFormUserList').append('<div value="' + this.id + '" class="user">' + this.displayName + '</div>');
            });
            if (jq("#addTaskPanel #fullFormUserList div.user").length == jq("#addTaskPanel #fullFormUserList div.user[value=" + serviceManager.getMyGUID() + "]").length) {
                jq('#addTaskPanel .notify').hide();
            } else {
                jq('#addTaskPanel .notify').show();
            }
        }

        if (typeof task.deadline != 'undefined') {
            jq('#taskDeadline').datepicker('setDate', task.displayDateDeadline);
            var elemDurationDays = jq('#taskDeadline').siblings('.dottedLink');
            jq(elemDurationDays).css('border-bottom', '1px dotted');
            jq(elemDurationDays).css('font-weight', 'normal');

        }

        if (task.priority) {
            jq('#addTaskPanel input[name="priority"]').attr('checked', 'true');
        } else {
            jq('#addTaskPanel input[name="priority"]').removeAttr('checked');
        }

        jq('#addTaskPanel').attr('type', 'edit');
        jq('#addTaskPanel').attr('taskid', params.taskid);
        jq('#addTaskPanel .baseLinkButton').html(jq('#addTaskPanel .baseLinkButton').attr('update'));
        jq('#addTaskPanel .containerHeaderBlock table td:first').html(ProjectJSResources.EditThisTask);
        showTaskForm(true);
    };

    var showTaskForm = function(editTask) {
        var team;
        if (serviceManager.projectId != null) {
            if (typeof projectTeam != 'undefined') {
                team = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectTeam)).response);
                onGetTeamFF(null, team);
            }
            else {
                serviceManager.getTeam('getTeamFF', { projectid: serviceManager.projectId });
            }
            if (typeof milestones != 'undefined') {
                onGetMilestonesFF(null, milestones);
            }
            else {
                serviceManager.getMilestonesByProject({}, serviceManager.projectId, onGetMilestonesFF);
            }
        }
        else {
            if (!editTask) {
                jq('#pm-projectBlock').show();
                var project = parseInt(jq.getAnchorParam('project', ASC.Controls.AnchorController.getAnchor()));
                if (project) {
                    jq('#taskProject').val(project);
                    if (jq('#taskProject option[value=' + project + ']').text().length < 15) {
                        jq('#taskProject').addClass('left-align');
                        jq('span.tl-combobox.tl-combobox-container:has(select#taskProject)').addClass('left-align');
                    } else {
                        jq('#taskProject').removeClass('left-align');
                        jq('span.tl-combobox.tl-combobox-container:has(select#taskProject)').removeClass('left-align');
                    }

                    if (typeof projectTeam != 'undefined') {
                        team = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectTeam)).response);
                        onGetTeamFF(null, team);
                    } else {
                        serviceManager.getTeamByProject('getTeamFF', { projectid: project }, project);
                    }
                    serviceManager.getMilestonesByProject({}, project, onGetMilestonesFF);
                }
            } else {
                jq('#pm-projectBlock').hide();
            }
        }
        var margintop = jq(window).scrollTop() - 60;
        margintop = margintop + 'px';
        jq.blockUI({
            message: jq('#addTaskPanel'),
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
                var e = jQuery.Event('keydown');
                jq('#addTaskPanel #addtask_description').trigger(e);
                if (typeof editTask != 'undefined' && editTask) {
                    jq('#taskResponsible').tlcombobox();
                    jq('#taskMilestone').tlcombobox();
                } else {
                    clearAddTaskForm('add');
                }
                jq('.userAdd .combobox-container').show();
                jq('#fullFormUserList div').each(function() {
                    var value = jq(this).attr('value');
                    jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').hide();

                    if (!jq('.userAdd .combobox-container li.option-item:visible').length) {
                        jq('#taskResponsible').tlcombobox(false);
                    }
                });
                jq('.userAdd .combobox-container').hide();
            }
        });
    };

    var onGetMilestonesFF = function(params, milestones) {
        var options = [],
            milestonesInd = milestones ? milestones.length : 0;
        while (milestonesInd--) {
            options.unshift({ value: milestones[milestonesInd].id, title: '[' + milestones[milestonesInd].displayDateDeadline + '] ' + milestones[milestonesInd].title });
        }
        jq('#taskMilestone option[value!=0][value!=-1]').remove();
        extendSelect(jq('#taskMilestone'), options);
        if (typeof jq('#taskMilestone').attr('val') != 'undefined') {
            jq('#taskMilestone').val(jq('#taskMilestone').attr('val')).change();
        }

        jq('#taskMilestone').tlcombobox();
    };

    var onGetTeamFF = function(params, team) {
        var options = [],
            teamInd = team ? team.length : 0;

        while (teamInd--) {
            options.unshift({ value: team[teamInd].id, title: team[teamInd].displayName });
        }
        jq('#taskResponsible option[value!=0][value!=-1]').remove();
        extendSelect(jq('#taskResponsible'), options).tlcombobox();

        jq('#fullFormUserList div').each(function(i) {
            var value = jq(this).attr('value');
            jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').hide();
        });
        if (jq('.userAdd .combobox-container li.option-item').length - 1 == jq('#fullFormUserList div').length) {
            jq('#taskResponsible').tlcombobox(false);
        }

        jq('#teamListPopup_fullForm').html('');

        jq('#teamListPopup_fullForm div span').on('click', function() {
            var userName = jq(this).html();
            var value = jq(this).attr('value');
            if (!jq('#fullFormUserList div[value="' + value + '"]').length) {
                jq('#fullFormUserList').show().append('<div value="' + value + '" class="user">' + userName + '</div>');
                jq('#taskResponsible').val(-1).change();
            }
            jq('.popupActionPanel').hide();
        });
        jq('#taskResponsible').on('change', function(evt) {
            var value = jq(this).val();
            if (value == -1) {
                jq(this).val('-1');
                return 'undefined';
            }
            var userName = jq(this).find('option[value="' + value + '"]').html();
            if (!jq('#fullFormUserList div[value="' + value + '"]').length) {
                jq('#fullFormUserList').show().append('<div value="' + value + '" class="user">' + userName + '</div>');
                jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').hide();
                var addedUserCount = jq("#fullFormUserList .user").length;
                var usersCount = jq("#taskResponsible option").length - 1;
                if (addedUserCount == usersCount) {
                    jq('#taskResponsible').tlcombobox(false);
                }
            }
            jq(this).val('-1').change();
            if (jq('#addTaskPanel #fullFormUserList div.user').length == jq("#addTaskPanel #fullFormUserList div.user[value=" + serviceManager.getMyGUID() + "]").length) {
                jq('#addTaskPanel .notify').hide();
            } else {
                jq('#addTaskPanel .notify').show();
            }
            evt.stopPropagation();
        });
    };

    return {
        init: init,
        clearAddTaskForm: clearAddTaskForm,
        onGetTask: onGetTask,
        onGetMilestonesFF: onGetMilestonesFF,
        showTaskForm: showTaskForm
    };
})(jQuery);


jq(document).ready(function() {
  taskaction.init();
  jq('textarea').autoResize({});
});
