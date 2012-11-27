window.milestoneaction = (function() {
    var isInit = false;
    var myGuid;

    var currentProjectId;

    var init = function(guid) {
        if (isInit === false) {
            isInit = true;
        }

        myGuid = guid;

        currentProjectId = jq.getURLParam('prjID');

        Teamlab.bind(Teamlab.events.addPrjTask, onAddTask);
        Teamlab.bind(Teamlab.events.getPrjMilestones, onGetMilestonesByProject);

        jq('#milestoneProject').on('change', function() {
            if (jq(this).val() > 0) {
                jq('#milestoneProjectContainer').removeClass('requiredFieldError');
            	jq('#milestoneResponsibleContainer').show();
            	jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();
                getProjectParticipants(jq(this).val(), { responsible: jq(this).attr('responsible') });
            }
            if (jq('#milestoneProject option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneProject)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneProject)').removeClass('left-align');
            }
        });

        jq('#milestoneResponsible').on('change', function() {
            if (jq(this).val() != -1) {
                jq('#milestoneResponsibleContainer').removeClass('requiredFieldError');
            }
            if (jq(this).val() != -1 && jq(this).val() != myGuid) {
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').show();
            	jq('#notifyResponsibleCheckbox').attr('checked', true);
            } else {
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();
            }

            if (jq('#milestoneResponsible option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').removeClass('left-align');
            }
        });

        jq('#milestoneTitleInputBox').keyup(function() {
            if (jq(this).val().trim() != '') {
                jq('#milestoneTitleContainer').removeClass('requiredFieldError');
            }
        });

        jq('#milestoneActionPanel .deadlineLeft').on('click', function() {
            jq('#milestoneActionPanel .deadlineLeft').css('border-bottom', '1px dotted').css('font-weight', 'normal');
            jq(this).css('border-bottom', 'none').css('font-weight', 'bold');
            var daysCount = parseInt(jq(this).attr('value'));
            var date = new Date();
            date.setDate(date.getDate() + daysCount);
            jq("#milestoneDeadlineInputBox").datepicker('setDate', date);
        });

        jq('#milestoneActionButton').on('click', function() {
            jq('#milestoneProjectContainer').removeClass('requiredFieldError');
            jq('#milestoneResponsibleContainer').removeClass('requiredFieldError');
            jq('#milestoneDeadlineContainer').removeClass('requiredFieldError');
            jq('#milestoneTitleContainer').removeClass('requiredFieldError');

            var data = {};
            var milestoneId = jq('#milestoneActionPanel').attr('milestoneId');

            var milestoneProject = jq('#milestoneProject');
            if (milestoneProject.val() > 0) {
                data.projectId = jq('#milestoneProject').val();
            }
            else if (currentProjectId) {
                data.projectId = currentProjectId;
            }

            var milestoneResponsible = jq('#milestoneResponsible');
            if (milestoneResponsible.val().length && milestoneResponsible.val() !== '-1') {
                data.responsible = milestoneResponsible.val();
            };

            data.notifyResponsible = jq('#notifyResponsibleCheckbox').is(':checked');

            if (jq('#milestoneDeadlineInputBox').val().length) {
                data.deadline = jq('#milestoneDeadlineInputBox').datepicker('getDate');
                data.deadline.setHours(0);
                data.deadline.setMinutes(0);
                data.deadline = Teamlab.serializeTimestamp(data.deadline);
            }

            data.title = jq.trim(jq('#milestoneTitleInputBox').val());
            data.description = jq('#milestoneDescriptionInputBox').val();

            data.isKey = jq('#milestoneKeyCheckBox').is(':checked');
            data.isNotify = jq('#milestoneNotifyManagerCheckBox').is(':checked');

            var isError = false;
            if (!data.projectId) {
                jq('#milestoneProjectContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.responsible) {
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();
                jq('#milestoneResponsibleContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.deadline) {
                jq('#milestoneDeadlineContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.title.length) {
                jq('#milestoneTitleContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (isError) { return false; }

            jq('#milestoneActionButtonsContainer').hide();
            jq('#milestoneActionProcessContainer').show();

            if (jq('#milestoneActionPanel').attr('type') == 'update') {
                updateMilestone(milestoneId, data);
            }
            else {
                addMilestone(data);
            }
            return false;
        });
    };

    var addMilestone = function(milestone) {
        lockMilestoneActionPage();

        var params = {};
        Teamlab.addPrjMilestone(params, milestone.projectId, milestone,
            { success: milestones.onAddMilestone, error: milestones.onAddMilestoneError });
    };

    var updateMilestone = function(milestoneId, milestone) {
        lockMilestoneActionPage();

        var params = {};
        Teamlab.updatePrjMilestone(params, milestoneId, milestone,
            { success: milestones.onUpdateMilestone, error: milestones.onUpdateMilestoneError });
    };

    var lockMilestoneActionPage = function() {
        jq('#milestoneDeadlineInputBox').attr('disabled', true);
        jq('#milestoneTitleInputBox').attr('disabled', true);
        jq('#milestoneDescriptionInputBox').attr('disabled', true);
        jq('#milestoneKeyCheckBox').attr('disabled', true);
        jq('#milestoneNotifyManagerCheckBox').attr('disabled', true);
    };

    var unlockMilestoneActionPage = function() {
        jq('#milestoneDeadlineInputBox').removeAttr('disabled');
        jq('#milestoneTitleInputBox').removeAttr('disabled');
        jq('#milestoneDescriptionInputBox').removeAttr('disabled');
        jq('#milestoneKeyCheckBox').removeAttr('disabled');
        jq('#milestoneNotifyManagerCheckBox').removeAttr('disabled');
    };

    var clearPanel = function() {
        jq('#milestoneActionPanel').removeAttr('type');

        jq('#milestoneProject').tlcombobox();
        jq('#milestoneResponsible').tlcombobox();

        jq('#notifyResponsibleCheckbox').attr('checked', true);
    	jq('#milestoneResponsibleContainer').hide();
        jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();

        jq('#milestoneProjectContainer').removeClass('requiredFieldError');
        jq('#milestoneProject').val('-1').change();

        jq('#milestoneDeadlineContainer').removeClass('requiredFieldError');

        jq('#milestoneActionPanel .deadlineLeft').css('border-bottom', '1px dotted').css('font-weight', 'normal');

        var milestoneDeadline = jq('#milestoneDeadlineInputBox');
        milestoneDeadline.datepicker(
            { popupContainer: '#milestoneActionPanel', selectDefaultDate: true, onSelect: function() { milestoneDeadline.blur(); } }
        );
        var date = new Date();
        date.setDate(date.getDate() + 7);
        milestoneDeadline.datepicker('setDate', date);
        var elemDuration3Days = jq(milestoneDeadline).siblings(".dottedLink[value=7]");
        jq(elemDuration3Days).css("border-bottom", "medium none");
        jq(elemDuration3Days).css("font-weight", "bold");


        jq('#milestoneResponsibleContainer').removeClass('requiredFieldError');
        jq('#milestoneResponsible').val('-1').change();

        jq('#milestoneTitleContainer').removeClass('requiredFieldError');
        jq('#milestoneTitleInputBox').val('');

        jq('#milestoneDescriptionInputBox').val('');

        jq('#milestoneKeyCheckBox').removeAttr('checked');

        jq('#milestoneNotifyManagerCheckBox').removeAttr('checked');

        jq('#milestoneActionButtonsContainer').show();
        jq('#milestoneActionProcessContainer').hide();
    };

    var onGetProjectParticipants = function(params, participants) {
        var milestoneResponsible = jq('#milestoneResponsible');
        if (!params.serverData) {
            milestoneResponsible.children('option[value!=0][value!=-1]').remove();
            for (var i = 0; i < participants.length; i++) {
                var p = participants[i];
                milestoneResponsible.append(jq('<option value="' + p.id + '"></option>').html(p.displayName));
            }
        }
        milestoneResponsible.tlcombobox();
        if (params.responsible) {
            milestoneResponsible.val(params.responsible).change();
        } else {
            if (jq('#milestoneResponsible option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').removeClass('left-align');
            }
        }

    	jq("#milestoneResponsibleContainer").show();
        milestones.showMilestoneActionPanel();
        jq('#milestoneProject').removeAttr('responsible');
    };

    var onAddTask = function(params, task) {
        var milestoneId = task.milestone.id;
        if (!milestoneId) return;
        getMilestone(milestoneId);
        jq.unblockUI();
    };

    var getMilestone = function(milestoneId) {
        var params = {};
        Teamlab.getPrjMilestone(params, milestoneId, { success: milestones.onGetMilestoneAfterAddTask });
    };

    var onGetMilestonesByProject = function(params, milestones) {
        if (params.eventType == 'getMilestonesFF') {
            if (params.milestoneId) {
                jq('#taskMilestone').val(params.milestoneId).change();
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

                onBlock: function() { }
            });
        }
    };

    var onGetMilestoneBeforeUpdate = function(milestone) {
        clearPanel();
        
        jq('#milestoneActionPanel').attr('type', 'update');
        jq('#milestoneActionPanel').attr('milestoneId', milestone.id);

        var milestoneActionButton = jq('#milestoneActionButton');
        milestoneActionButton.html(milestoneActionButton.attr('update'));

        var milestoneActionHeader = jq('#milestoneActionPanel .containerHeaderBlock table td:first');
        milestoneActionHeader.html(ProjectJSResources.EditMilestone);

        if (milestone.deadline) {
            jq('#milestoneDeadlineInputBox').datepicker("setDate", milestone.deadline);
            var elemDurationDays = jq("#milestoneDeadlineInputBox").siblings(".dottedLink");
            jq(elemDurationDays).css("border-bottom", "1px dotted");
            jq(elemDurationDays).css("font-weight", "normal");
        }

        jq('#milestoneTitleInputBox').val(milestone.title);
        jq('#milestoneDescriptionInputBox').val(milestone.description);
        
    	jq('#milestoneProjectContainer').hide();
        
        if (milestone.isKey == 'true') {
            jq('#milestoneKeyCheckBox').attr('checked', 'checked');
        }

        if (milestone.isNotify == 'true') {
            jq('#milestoneNotifyManagerCheckBox').attr('checked', 'checked');
        }

        if (milestone.responsible) {
            jq('#milestoneProject').attr('responsible', milestone.responsible);
        }

        if (!currentProjectId) {
            jq('#milestoneProject').val(milestone.project).change();
        }
        else {
            onGetProjectParticipants({ serverData: true, responsible: milestone.responsible });
        }
    };

    var getProjectParticipants = function(projectId, params) {
        Teamlab.getPrjProjectTeamPersons(params, projectId, { success: onGetProjectParticipants });
    };

    return {
        init: init,
        updateMilestone: updateMilestone,
        onGetProjectParticipants: onGetProjectParticipants,
        onGetMilestoneBeforeUpdate: onGetMilestoneBeforeUpdate,
        onGetMilestonesByProject: onGetMilestonesByProject,
        clearPanel: clearPanel,
        unlockMilestoneActionPage: unlockMilestoneActionPage
    };
})(jQuery);

jq(document).ready(function() {  
  jq('textarea').autoResize({});
});