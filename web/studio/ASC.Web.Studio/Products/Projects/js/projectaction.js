window.projectaction = (function() {
    var myGuid;
    var activeTasksCount;
    var activeMilestonesCount;

    var init = function(guid) {
        myGuid = guid;
        activeTasksCount = parseInt(jq('#activeTasks').val());
        activeMilestonesCount = parseInt(jq('#activeMilestones').val());
    };

    var getMyGuid = function() {
        return myGuid;
    };

    var getActiveTasksCount = function() {
        return activeTasksCount;
    };

    var getActiveMilestonesCount = function() {
        return activeMilestonesCount;
    };

    var clearSelectedTemplate = function() {
        jq('#SelectedTemplateID').val(0);

        jq('[id$="_TemplatesDropdownContainer"]').show();
        jq('#SelectedTemplateTitle').hide();
        jq('#ClearSelectedTemplate').hide();

        jq('label[for=notify]').text(ProjectJSResources.NotifyProjectLeader);
    };

    var changeTemplateDropdownItem = function(templateId) {
        AjaxPro.ProjectAction.GetProjectTemplateTitle(templateId, function(res) {
            if (res.error != null) {
                alert(res.error.Message);
                return;
            }

            var title = res.value;
            jq('#SelectedTemplateID').val(templateId);
            jq('[id$="_TemplatesDropdownContainer"]').hide();
            jq('#SelectedTemplateTitle').show();
            var t = jq('#SelectedTemplateTitleHidden').val();
            t = t.format("<span style='color: Black;'>«" + title + "»</span>");
            jq('#SelectedTemplateTitle').html(t);
            jq('#ClearSelectedTemplate').show();
            jq('label[for=notify]').text(ProjectJSResources.NotifyTheResponsible);
        });
    };

    return {
        init: init,
        getMyGuid: getMyGuid,
        getActiveTasksCount: getActiveTasksCount,
        getActiveMilestonesCount: getActiveMilestonesCount,
        clearSelectedTemplate: clearSelectedTemplate,
        changeTemplateDropdownItem: changeTemplateDropdownItem
    };

})();

jq(document).ready(function() {
    if (jq('#secureState').val() == 0) {
        jq('#projectPrivacyCkeckbox').attr('checked', false);
        jq('#projectPrivacyCkeckbox').click(function() {
            jq('#projectPrivacyCkeckbox').attr('checked', false);
            PremiumStubManager.ShowDialog();
        });
    }

    var projectResponsible = jq('#projectResponsible').val();

    var projectParticipants = new Array();
    var unselectedProjectParticipants = new Array();

    jq('body').on('click', function(event) {
        var target = (event.target) ? event.target : event.srcElement;
        var element = jq(target);
        if (!element.is('[id$=projectTags]')) {
            jq('#tagsAutocompleteContainer').empty();
        }
    });

    jq('.dottedHeader .headerPanel').click(function() {
        jq(this).siblings('div.dottedHeaderContent').toggle();
    });

    jq('#projectManagerContainer').on('click', '#divUsers [id^=User_]', function() {
        var id = jq(this).attr('id').slice(5);
        if (id != -1) {
            jq('#projectManagerContainer').removeClass('requiredFieldError');
            jq('#projectManagerSelector .borderBase').removeClass('requiredInputError');
        }
        if (id == projectaction.getMyGuid() || (projectResponsible && id == projectResponsible)) {
            jq('#notifyManagerCheckbox').removeAttr('checked');
            jq('#notifyManagerCheckbox').attr('disabled', 'disabled');
        } else {
            jq('#notifyManagerCheckbox').removeAttr('disabled');
            jq('#notifyManagerCheckbox').attr('checked', 'checked');
        }
    });

    jq('#projectManagerSelector select').on('change', function() {
        var id = jq(this).val();
        if (id != -1) {
            jq('#projectManagerContainer').removeClass('requiredFieldError');
            jq('#projectManagerSelector .borderBase').removeClass('requiredInputError');
        }
        if (id == projectaction.getMyGuid() || (projectResponsible && id == projectResponsible)) {
            jq('#notifyManagerCheckbox').removeAttr('checked');
            jq('#notifyManagerCheckbox').attr('disabled', 'disabled');
        } else {
            jq('#notifyManagerCheckbox').removeAttr('disabled');
            jq('#notifyManagerCheckbox').attr('checked', 'checked');
        }
    });

    jq('[id$=projectTitle]').keyup(function() {
        if (jq(this).val().trim() != '') {
            jq('#projectTitleContainer').removeClass('requiredFieldError');
        }
    });

    jq('[id$=inputUserName]').keyup(function(eventObject) {
        if (eventObject.which == 13) {
            var id = jq('#login').attr('value');
            if (id != -1) {
                jq('#projectManagerContainer').removeClass('requiredFieldError');
                jq('#projectManagerSelector .borderBase').removeClass('requiredInputError');
            }
            if (id == projectaction.getMyGuid() || (projectResponsible && id == projectResponsible)) {
                jq('#notifyManagerCheckbox').removeAttr('checked');
                jq('#notifyManagerCheckbox').attr('disabled', 'disabled');
            } else {
                jq('#notifyManagerCheckbox').removeAttr('disabled');
                jq('#notifyManagerCheckbox').attr('checked', 'checked');
            }
        }
    });

    jq('#projectStatus').change(function() {
        closeQuestion();
    });

    jq('#projectTeamContainer .headerPanel').click(function() {
        projectTeamSelector.ShowDialog();
        for (var i = 0; i < unselectedProjectParticipants.length; i++) {
            var id = unselectedProjectParticipants[i];
            jq('#usrdialog_' + projectTeamSelector.ID + '_usr_' + id).attr('checked', 'checked');
            projectTeamSelector.PreSelectUser(id);
            projectTeamSelector.SelectUser(id);
            projectTeamSelector.Unselect();
        }
    });

    if (window.projectTeamSelector) {
        projectTeamSelector.OnOkButtonClick = function() {
            projectParticipants = [];
            jq('#projectParticipantsContainer table').empty();
            jq('#projectParticipantsContainer').hide();
            var usersInfo = projectTeamSelector.GetSelectedUsers();
            for (var i = 0; i < usersInfo.length; i++) {
                projectParticipants.push(usersInfo[i].ID);
            }

            jq('#projectParticipants').attr('value', projectParticipants.join());
            if (projectParticipants.length > 0) {
                jq('#projectParticipant').tmpl(usersInfo).appendTo('#projectParticipantsContainer table');
                jq('#projectParticipantsContainer').show();
            }
            return false;
        };
    }

    jq('#projectParticipantsContainer').on('click', 'table td.delMember span', function() {
        var id = jq(this).attr('userId');
        jq('#projectParticipantsContainer tr[participantId=' + id + ']').remove();
        deleteItemInArray(projectParticipants, id);
        jq('#projectParticipants').attr('value', projectParticipants.join());
        if (projectParticipants.length == 0) {
            jq('#projectParticipantsContainer').hide();
        }
        unselectedProjectParticipants.push(id);
    });

    function deleteItemInArray(array, item) {
        for (var i = 0; i < array.length; i++) {
            if (array[i] == item) {
                array.splice(i, 1);
                return;
            }
        }
    };

    jq('[id$=projectTags]').keyup(function(eventObject) {
        jq('#tagsAutocompleteContainer').on('click', function() { return false; });

        var value = jq('[id$=projectTags]').val();
        var titles = value.split(',');

        var width = document.getElementById(jq(this).attr('id')).offsetWidth;
        jq('#tagsAutocompleteContainer').css('width', width + 'px');

        var code = eventObject.which;
        //left
        if (code == 37) { return; }
        //up
        else if (code == 38) { moveSelected(true); return; }
        //right
        else if (code == 39) { return; }
        //down
        else if (code == 40) { moveSelected(false); return; }

        var tagName = titles[titles.length - 1];
        if (tagName.trim() != '') {
            Teamlab.getPrjTagsByName({ titles: titles }, tagName, { tagName: tagName }, { success: onGetTags });
        }
    });

    function onGetTags(params, tags) {
        var titles = params.titles;
        jq('#tagsAutocompleteContainer').empty();

        if (tags.length > 0) {
            for (var i = 0; i < tags.length; i++) {
                var container = document.createElement('div');

                jq(container).addClass('tagAutocompleteItem');
                jq(container).text(tags[i]);

                jq(container).on('mouseover', function() {
                    jq('div.tagAutocompleteItem').each(function() {
                        jq(this).removeClass('tagAutocompleteSelectedItem');
                    });
                    jq(this).addClass('tagAutocompleteSelectedItem');
                });

                jq(container).on('click', function() {
                    var str = '';
                    for (var j = 0; j < titles.length - 1; j++) {
                        str += jq.trim(titles[j]);
                        str += ', ';
                    }
                    jq('[id$=projectTags]').val(str + jq(this).text() + ', ');
                    jq('#tagsAutocompleteContainer').empty();
                });

                jq('#tagsAutocompleteContainer').append(container);
                jq('[id$=projectTags]').focus();
            }
        }
    };

    function moveSelected(up) {
        if (jq('#tagsAutocompleteContainer').html() == '') return;

        var items = jq('#tagsAutocompleteContainer .tagAutocompleteItem');

        var selected = false;
        items.each(function(idx) {
            if (jq(this).is('.tagAutocompleteSelectedItem')) {
                selected = true;
                if (up && idx > 0) {
                    jq(this).removeClass('tagAutocompleteSelectedItem');
                    items.eq(idx - 1).addClass('tagAutocompleteSelectedItem');
                    jq('#tagsAutocompleteContainer').scrollTo(items.eq(idx - 1).position().top, 100);
                    return false;
                } else if (!up && idx < items.length - 1) {
                    jq(this).removeClass('tagAutocompleteSelectedItem');
                    items.eq(idx + 1).addClass('tagAutocompleteSelectedItem');
                    jq('#tagsAutocompleteContainer').scrollTo(items.eq(idx + 1).position().top, 100);
                    return false;
                }
            }
            return true;
        });
        if (!selected) {
            items.eq(0).addClass('tagAutocompleteSelectedItem');
        }
    };

    jq('[id$=projectTags]').keydown(function(eventObject) {
        var value = jq('[id$=projectTags]').val();
        var titles = value.split(',');

        var code = eventObject.which;
        //enter
        if (code == 13) {
            var str = '';
            for (var i = 0; i < titles.length - 1; i++) {
                str += jq.trim(titles[i]);
                str += ', ';
            }

            if (jq('.tagAutocompleteSelectedItem').length != 0) {
                jq('[id$=projectTags]').val(str + jq('.tagAutocompleteSelectedItem').text() + ', ');
            }

            jq('#tagsAutocompleteContainer').html('');
            return false;
        }
        return true;
    });

    jq('#projectActionButton').click(function() {
        var projectId = jq.getURLParam('prjID');

        jq('#projectTitleContainer').removeClass('requiredFieldError');
        jq('#projectManagerContainer').removeClass('requiredFieldError');
        jq('#projectManagerSelector .borderBase').removeClass('requiredInputError');
        jq('#projectStatusContainer').removeClass('requiredFieldError');

        var title = jq('[id$=projectTitle]').val().trim();
        var responsibleid = projectManagerSelector.SelectedUserId;
        var status = jq('#projectStatus option:selected').val();

        var isError = false;
        if (title == '') {
            jq('#projectTitleContainer').addClass('requiredFieldError');
            isError = true;
        }
        if (!responsibleid || responsibleid == -1) {
            jq('#projectManagerContainer').addClass('requiredFieldError');
            jq('#projectManagerSelector .borderBase').addClass('requiredInputError');
            isError = true;
        }
        if (projectId && status == 'closed' && closeQuestion()) {
            jq('#projectStatusContainer').addClass('requiredFieldError');
            isError = true;
        }
        if (isError) {
            return;
        }

        var project =
        {
            title: title,
            responsibleid: responsibleid,
            notify: jq('#notifyManagerCheckbox').is(':checked'),
            description: jq('[id$=projectDescription]').val(),
            tags: jq('[id$=projectTags]').val(),
            'private': jq('#projectPrivacyCkeckbox').is(':checked'),
            templateProjectId: jq('#SelectedTemplateID').val()
        };
        if (jq('#secureState').val() == 0) {
            project.private = false;
        }
        if (jq('#projectParticipants').length != 0) {
            var participants = jq('#projectParticipants').attr('value').split(',');
            if (!(participants.length == 1 && participants[0] === ''))
                project.participants = participants;
        }

        if (projectId) {
            project.status = status;
        }

        lockProjectActionPageElements();
        if (projectId) {
            updateProject(projectId, project);
        } else {
            addProject(project);
        }
    });

    function addProject(project) {
        var params = {};
        Teamlab.addPrjProject(params, project, { success: onAddProject, error: onAddProjectError });
    };

    function onAddProject(params, project) {
        project = this.__responses[0];
        var following = jq('#followingProjectCheckbox').is(':checked');
        var myGuid = projectaction.getMyGuid();

        var isManager = project.responsible.id == myGuid;
        var isInTeam = getArrayIndex(myGuid, projectParticipants) != -1;
        if (following && !isInTeam & !isManager) {
            Teamlab.followingPrjProject({}, project.id, { projectId: project.id }, { success: onFollowingProject });
        } else {
            document.location.replace('projects.aspx?prjID=' + project.id);
        }
    };

    function getArrayIndex(value, array) {
        var index = -1;
        for (var i = 0; i < array.length; i++) {
            if (array[i] === value) {
                index = i;
                break;
            }
        }
        return index;
    }

    function onFollowingProject(params, project) {
        document.location.replace('projects.aspx?prjID=' + project.id);
    }

    function onAddProjectError() {
        unlockProjectActionPageElements();
    };

    function updateProject(projectId, project) {
        var params = {};
        Teamlab.updatePrjProject(params, projectId, project, { success: onUpdateProject, error: onUpdateProjectError });
    }

    var onUpdateProject = function(params, project) {
        document.location.replace('projects.aspx?prjID=' + project.id);
    };

    var onUpdateProjectError = function() {
        unlockProjectActionPageElements();
    };

    var deleteProject = function(projectId) {
        var params = {};
        Teamlab.removePrjProject(params, projectId, { success: onDeleteProject, error: onDeleteProjectError });
    };

    var onDeleteProject = function() {
        document.location.replace('projects.aspx');
    };

    var onDeleteProjectError = function() {
        unlockProjectActionPageElements();
    };

    jq('#projectDeleteButton').click(function() {
        showQuestionWindow('#questionWindowDeleteProject');
    });

    function lockProjectActionPageElements() {
        jq('[id$=projectTitle]').attr('readonly', 'readonly').addClass('disabled');
        jq('[id$=projectDescription]').attr('readonly', 'readonly').addClass('disabled');
        jq('#inputUserName').attr('disabled', 'disabled');
        jq('#projectManagerSelector td:last').css({ 'display': 'none' });
        jq('#projectParticipantsContainer table').removeClass('canedit');
        jq('#projectStatus').attr('disabled', 'disabled').addClass('disabled');
        jq('[id$=projectTags]').attr('readonly', 'readonly').addClass('disabled');
        jq('#notifyManagerCheckbox').attr('disabled', 'disabled');
        jq('#projectPrivacyCkeckbox').attr('disabled', 'disabled');
        jq('#followingProjectCheckbox').attr('disabled', 'disabled');
        jq('#projectActionsContainer').hide();
        jq('#projectActionsInfoContainer').show();
        jq('#TemplatesComboboxContainer').hide();

        jq('#projectTeamContainer .headerPanel').off();
    }

    function unlockProjectActionPageElements() {
        jq('[id$=projectTitle]').removeAttr('readonly').removeClass('disabled');
        jq('[id$=projectDescription]').removeAttr('readonly').removeClass('disabled');
        jq('#inputUserName').removeAttr('disabled');
        jq('#projectManagerSelector td:last').css({ 'display': 'table-cell' });
        jq('#projectParticipantsContainer table').addClass('canedit');
        jq('#projectStatus').removeAttr('disabled').removeClass('disabled');
        jq('[id$=projectTags]').removeAttr('readonly').removeClass('disabled');
        jq('#notifyManagerCheckbox').removeAttr('disabled');
        jq('#projectPrivacyCkeckbox').removeAttr('disabled');
        jq('#followingProjectCheckbox').removeAttr('disabled');
        jq('#projectActionsContainer').show();
        jq('#projectActionsInfoContainer').hide();
        jq('#TemplatesComboboxContainer').show();

        jq('#projectTeamContainer .headerPanel').click(function() {
            projectTeamSelector.ShowDialog();
        });
    }

    var closeQuestion = function() {
        var activeTasksCount = projectaction.getActiveTasksCount();
        var activeMilestonesCount = projectaction.getActiveMilestonesCount();
        if (jq("#projectStatus").val() == 'closed') {
            if (activeTasksCount > 0) {
                showQuestionWindow('#questionWindowActiveTasks');
                jq('#projectStatusContainer').addClass('requiredFieldError');
            }
            else if (activeMilestonesCount > 0) {
                showQuestionWindow('#questionWindowActiveMilestones');
                jq('#projectStatusContainer').addClass('requiredFieldError');
            }
        } else {
            jq('#projectStatusContainer').removeClass('requiredFieldError');
            return false;
        }
        return activeTasksCount > 0 || activeMilestonesCount > 0;
    };

    var showQuestionWindow = function(popupId) {
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq(popupId),
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

    jq('#questionWindowDeleteProject .remove').on('click', function() {
        var projectId = jq.getURLParam('prjID');
        lockProjectActionPageElements();
        deleteProject(projectId);
        return false;
    });

    jq('#questionWindowDeleteProject .cancel, #questionWindowActiveTasks .cancel, #questionWindowActiveMilestones .cancel').on('click', function() {
        jq.unblockUI();
        return false;
    });
});
