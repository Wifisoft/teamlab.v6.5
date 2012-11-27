//----common----//
function showActionsPanel(panelId, obj) {
    var x, y;

    jq('.actionPanel').hide();

    x = jq(obj).offset().left;
    y = jq(obj).offset().top;
    
    if (panelId == "projectMemberPanel") {
        x = x - 12;
        y = y + 20;
    } else {
        x = x - 158;
        y = y + 17;
    }

    jq('#' + panelId).css("left", x + "px");
    jq('#' + panelId).css("top", y + "px");
    jq('#' + panelId).show();

    jq('body').click(function(event) {
        var elt = (event.target) ? event.target : event.srcElement;
        var isHide = true;
        if (jq(elt).is('[id="' + panelId + '"]')) {
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
            jq('.milestone').removeClass('open');
            jq('.task').removeClass('open');
            jq(".template").removeClass('open');
        }
    });
}
function showAction(target) {
    if (target == 'noAssign') {
        var listNoAssignTask = jq("#listNoAssignListTask .task");
        if (listNoAssignTask.length > 0) {
            jq("#noAssignTaskContainer .addTaskContainer").appendTo(jq("#noAssignTaskContainer"));
            jq("#noAssignTaskContainer .addTaskContainer").show();
        }
    }
};

function hideAddMilestoneContainer () {
    jq("#addMilestoneContainer").hide();
    if (jq("#addMilestoneContainer").hasClass("edit")) {
        var target = jq("#addMilestoneContainer").attr("target");
        jq("#" + target + " .mainInfo").show();
        jq("#addMilestoneContainer").removeClass("edit");
    }
    if (jq("#addMilestoneContainer").hasClass('edit')) {
        jq("#" + jq("#addMilestoneContainer").attr('target')).find(".mainInfo").show();
    } 
    jq(".addTaskContainer").show();
    jq("#addMilestoneContainer #newMilestoneTitle").val('');
    jq("#addMilestone").show();
};
function hideAddTaskContainer() {
    jq("#addTaskContainer").hide();
    jq('.task').show();
    var target = jq("#addTaskContainer").attr("target");
    var elem = jq("#" + target);
    var containerTask;
    if (jq(elem).hasClass("milestone")) {
        containerTask = jq(elem).find(".milestoneTasksContainer");
        if (jq(containerTask).find(".task").length == 0) {
            jq(containerTask).hide();
            jq(containerTask).closest(".milestone").find(".addTask").removeClass("hide");
            jq(containerTask).find(".addTaskContainer").hide();
        } else {
            jq(containerTask).find(".addTaskContainer").show();
        }
    }
    else {
        var container = jq(elem).parent();
        if (target == "noAssign" || jq(container).attr('id') == "listNoAssignListTask") {
                jq("#noAssignTaskContainer .addTaskContainer").show();
        } else {
            containerTask = jq(elem).closest(".milestoneTasksContainer");
            if (jq(containerTask).find(".task").length == 0) {
                jq(containerTask).hide();
                jq(containerTask).closest(".milestone").find(".addTask").removeClass("hide");
                jq(containerTask).find(".addTaskContainer").hide();
            } else {
                jq(containerTask).find(".addTaskContainer").show();
            }
        }
    }
    jq("#addTaskContainer").removeAttr("target");
    jq("#newTaskTitle").val('');
};

jq(document).ready(function() {
    jq("#newMilestoneTitle, #newTaskTitle").val('');

    jq(".task .menuButton").live('click', function() {
        hideAddTaskContainer();
        hideAddMilestoneContainer();
        var target = jq(this).parents('.task').attr('id');
        jq("#" + target).addClass("open");
        jq("#taskActionPanel").attr('target', target);
        showActionsPanel("taskActionPanel", this);
        return false;
    });
    jq(".milestone .mainInfo .menuButton").live('click', function() {
        hideAddTaskContainer();
        hideAddMilestoneContainer();
        var target = jq(this).parents('.milestone').attr('id');
        jq("#milestoneActionPanel").attr('target', target);
        jq("#" + target).addClass("open");
        showActionsPanel("milestoneActionPanel", this);
        return false;
    });

    jq(".addTaskContainer .baseLinkAction").live('click', function() {
        hideAddMilestoneContainer();
        if (jq('#addTaskContainer').hasClass('edit')) {
            jq('#' + jq('#addTaskContainer').attr('target')).show();
            jq('#addTaskContainer').removeClass('edit');
        }
        jq("#newTaskTitle").val("");
        var target;
        var parent = jq(this).parent().parent();
        if (jq(parent).attr("id") == "noAssignTaskContainer") {
            target = "noAssign";
        } else {
            target = jq(parent).parent().attr("id");
        }
        if (jq("#addTaskContainer").attr("target") != "") {
            var elem = jq("#addTaskContainer").parent();
            if (jq(elem).attr('id') == "noAssignTaskContainer") {
                jq("#noAssignTaskContainer .addTaskContainer").appendTo("#noAssignTaskContainer");
            }
            jq(elem).children(".addTaskContainer").show();
        }
        jq("#addTaskContainer").attr("target", target);
        jq("#addTaskContainer").appendTo(parent);
        jq(parent).children(".addTaskContainer").hide();
        jq("#addTaskContainer").show();
        jq("#addTaskContainer #newTaskTitle").focus();
    });
});

window.ASC.Projects.ListProjectsTemplates = (function() {
    var idDeleteTempl;
    var init = function() {

        if (templates.length) {
            displayListTemplates(templates);
        }

        jq(".template .menuButton").live('click', function() {
            var tmplId = jq(this).closest('.template').attr('id');
            jq("#templateActionPanel").attr('target', tmplId);
            jq(this).closest('.template').addClass('open');
            showActionsPanel("templateActionPanel", this);
            return false;
        });

        jq("#templateActionPanel #editTmpl").bind('click', function() {
            var tmplId = jq("#templateActionPanel").attr('target');
            jq(".actionPanel").hide();
            jq(".template").removeClass('open');
            window.location.replace('editprojecttemplate.aspx?id=' + tmplId);
        });
        jq("#templateActionPanel #deleteTmpl").bind('click', function() {
            idDeleteTempl = parseInt(jq("#templateActionPanel").attr('target'));
            jq(".actionPanel").hide();
            jq(".template").removeClass('open');
            showQuestionWindow();
        });
        jq("#templateActionPanel #createProj").bind('click', function() {
            var tmplId = jq("#templateActionPanel").attr('target');
            jq(".actionPanel").hide();
            jq(".template").removeClass('open');
            window.location.replace('createprojectfromtemplate.aspx?id=' + tmplId);
        });
    };

    var onDeleteTemplate = function() {
        jq("#" + idDeleteTempl).remove();
        idDeleteTempl = 0;
        var list = jq("#listTemplates").find(".template");
        if (!list.length) {
            jq("#listTemplates").hide();
            jq(".addTemplate").hide();
            jq("#emptyListTemplates").show();
        }
    };
    var removeTemplate = function() {
        AjaxPro.ProjectTemplates2.RemoveTemplate(idDeleteTempl, onDeleteTemplate);
    };
    var createTemplateTmpl = function(template) {
        var mCount = 0, tCount = 0;

        var description = jQuery.parseJSON(template.description);
        var milestones = description.milestones;

        var tasks = description.tasks;
        if (tasks) tCount = tasks.length;
        if (milestones) {
            mCount = milestones.length;
            for (var i = 0; i < milestones.length; i++) {
                var mTasks = milestones[i].tasks;
                if (mTasks) {
                    tCount += mTasks.length;
                }
            }
        }
        var tmpl = { title: template.title, id: template.id, milestones: mCount, tasks: tCount };
        return tmpl;
    };

    var displayListTemplates = function(templates) {
        for (var i = 0; i < templates.length; i++) {
            var tmpl = createTemplateTmpl(templates[i]);
            jq("#templateTmpl").tmpl(tmpl).appendTo("#listTemplates");
        }
    };
    var showQuestionWindow = function() {
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
        jq('#questionWindow .cancel').bind('click', function() {
            jq.unblockUI();
            idDeleteTempl = 0;
            return false;
        });
        jq('#questionWindow .remove').bind('click', function() {
            removeTemplate();
            jq.unblockUI();
            return false;
        });
    };
    return {
        init: init
    };

})(jQuery);


window.ASC.Projects.EditProjectTemplates = (function() {
    var milestoneCounter = 0,
        taskCounter = 0;
    var tmplId;
    var init = function() {

        tmplId = jq.getURLParam('id');
        if (tmplId) {
            tmplId = template.id;
            showTmplStructure(template);
        }
        else {
            jq("#templateTitle").val('');
        }

        //milestone
        jq("#addMilestone a").bind('click', function() {
            hideAddTaskContainer();
            jq("#addMilestoneContainer").hide();
            if (jq("#addMilestoneContainer").hasClass('edit')) {
                jq("#" + jq("#addMilestoneContainer").attr('target')).find(".mainInfo").show();
                jq("#addMilestoneContainer").removeClass('edit');
            }

            jq("#addMilestone").after(jq("#addMilestoneContainer"));
            jq("#addMilestone").hide();
            jq("#newMilestoneTitle").val('');

            if (jq("#dueDate").length) {
                var defDate = new Date();
                defDate.setDate(defDate.getDate() + 3);
                jq("#dueDate").datepicker("setDate", defDate);
            }

            jq("#addMilestoneContainer").show();
            jq("#addMilestoneContainer #newMilestoneTitle").focus();
        });
        jq("#newMilestoneTitle").bind('keydown', function(e) {
            var targetId = jq("#addMilestoneContainer").attr('target');
            if (e.which == 13) {
                var text = jq.trim(jq(this).val());
                if (!text.length) {
                    alert(jq("#milestoneError").text());
                    return false;
                }
                if (jq("#addMilestoneContainer").hasClass('edit')) {

                    jq("#" + targetId + " .mainInfo .title").text(jq.trim(jq(this).val()));
                    var days = jq("#addMilestoneContainer select option:selected").attr('value');
                    jq("#" + targetId + " .mainInfo .daysCount span").text(days);
                    jq("#" + targetId + " .mainInfo .daysCount").attr('value', days);

                    jq("#addMilestoneContainer").hide();
                    jq("#" + targetId + " .mainInfo").show();
                    jq("#addMilestoneContainer").removeClass('edit');
                } else {
                    milestoneCounter++;
                    var milestone = {};
                    milestone.title = jq.trim(jq(this).val());
                    milestone.duration = jq("#addMilestoneContainer select option:selected").attr('value');
                    milestone.tasks = [];
                    milestone.number = milestoneCounter;
                    jq("#milestoneTmpl").tmpl(milestone).appendTo("#listAddedMilestone");
                    jq(this).val("");
                    jq(this).focus();
                }
                return false;
            } else {
                if (e.which == 27) {
                    jq(this).val("");
                    if (jq("#addMilestoneContainer").hasClass('edit')) {
                        jq("#addMilestoneContainer").hide();
                        jq("#" + targetId + " .mainInfo").show();
                    } else {
                        jq("#addMilestoneContainer").hide();
                        jq("#addMilestone").show();
                    }
                }
            }
        });

        //milestone menu
        jq(".milestone .mainInfo .title, .milestone .mainInfo .daysCount").live('click', function() {
            hideAddTaskContainer();
            hideAddMilestoneContainer();
            jq("#addMilestoneContainer").addClass('edit');
            var target = jq(this).closest('.milestone').attr('id');
            jq("#addMilestoneContainer").attr('target', target);
            var milestone = jq("#" + target + " .mainInfo");
            jq("#addMilestoneContainer").prependTo(jq("#" + target));
            jq(milestone).hide();
            var val = jq(milestone).find(".daysCount").attr('value');
            jq("#addMilestoneContainer").show();
            jq("#addMilestoneContainer #newMilestoneTitle").val(jq(milestone).children(".title").text());
            jq("#addMilestoneContainer select option[value = '" + val + "']").attr("selected", "selected");

            jq("#addMilestoneContainer #newMilestoneTitle").focus();

        });
        jq(".milestone .mainInfo .addTask").live('click', function() {
            var target = jq(this).closest('.milestone').attr('id');
            var milestTasksCont = jq("#" + target + " .milestoneTasksContainer");
            jq("#addTaskContainer").appendTo(milestTasksCont[0]);
            jq(milestTasksCont).find('.addTaskContainer').hide();
            jq("#addTaskContainer").attr("target", target);
            jq("#addTaskContainer").show();
            jq(milestTasksCont).show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#milestoneActionPanel .actionList #removeMilestone").bind('click', function() {
            hideAddTaskContainer();
            jq("#addTaskContainer").appendTo("#noAssignTaskContainer");
            jq("#milestoneActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#" + target).removeClass("open");
            jq("#" + target).remove();
        });

        jq("#milestoneActionPanel .actionList #addTaskInMilestone").bind('click', function() {
            jq("#milestoneActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#" + target).removeClass("open");
            var listTasks = jq(".listTasks[milestone='" + target + "']");
            var milestTasksCont = jq(listTasks[0]).closest(".milestoneTasksContainer");
            jq(milestTasksCont).find('.addTaskContainer').hide();
            jq("#addTaskContainer").appendTo(milestTasksCont[0]);
            jq("#addTaskContainer").attr("target", target);
            jq("#addTaskContainer").show();
            jq(milestTasksCont).find(".addTaskContainer").hide();
            jq(milestTasksCont).show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#milestoneActionPanel .actionList #editMilestone").bind('click', function() {
            jq("#milestoneActionPanel").hide();
            hideAddMilestoneContainer();
            jq("#addMilestoneContainer").addClass('edit');
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#addMilestoneContainer").attr('target', target);
            jq("#" + target).removeClass("open");
            var milestone = jq("#" + target + " .mainInfo");
            jq("#addMilestoneContainer").prependTo(jq("#" + target));
            jq(milestone).hide();
            var val = jq(milestone).children(".daysCount").attr('value');
            jq("#addMilestoneContainer").show();
            jq("#addMilestoneContainer #newMilestoneTitle").val(jq(milestone).children(".title").text());
            jq("#addMilestoneContainer select option[value = '" + val + "']").attr("selected", "selected");

            jq("#addMilestoneContainer #newMilestoneTitle").focus();
        });

        //task

        jq(".task .title").live('click', function() {
            hideAddMilestoneContainer();
            jq("#addTaskContainer").hide();
            if (jq("#addTaskContainer").hasClass('edit')) {
                jq('#' + jq("#addTaskContainer").attr('target')).show();
            } else {
                jq("#addTaskContainer").addClass('edit');
                jq('.addTaskContainer').show();
            }

            var target = jq(this).parents('.task');

            jq("#addTaskContainer").attr('target', jq(target).attr("id"));
            jq("#addTaskContainer").insertAfter(target);
            jq(target).hide();
            jq("#addTaskContainer #newTaskTitle").val(jq(target).children(".title").text());
            jq("#addTaskContainer").show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#newTaskTitle").bind('keydown', function(e) {
            var target = jq(this).parent().attr('target');
            if (e.which == 13) {
                var text = jq.trim(jq(this).val());
                if (!text.length) {
                    alert(jq("#taskError").text());
                    return false;
                }
                if (jq("#addTaskContainer").hasClass('edit')) {
                    jq("#" + target + " .title").text(jq(this).val());
                    jq("#addTaskContainer").removeClass('edit');
                    hideAddTaskContainer();
                    jq("#" + target).show();
                } else {
                    taskCounter++;
                    var task = {};
                    task.title = jq.trim(jq(this).val());
                    task.number = taskCounter;
                    var tElem;
                    if (target == 'noAssign') {
                        tElem = jq("#listNoAssignListTask");
                    }
                    else {
                        tElem = jq(".listTasks[milestone='" + target + "']");
                        jq("#" + target).find(".addTask").addClass("hide");
                    }
                    jq("#taskTmpl").tmpl(task).appendTo(tElem);
                    jq(this).val("");
                    jq(this).focus();
                }

                return false;
            } else {
                if (e.which == 27) {
                    jq(this).val("");
                    if (jq("#addTaskContainer").hasClass('edit')) {
                        hideAddTaskContainer();
                        jq("#addTaskContainer").removeClass('edit');
                        jq("#" + target).show();
                    } else {
                        hideAddTaskContainer();
                    }
                }
            }
        });

        //task menu

        jq("#taskActionPanel .actionList #removeTask").bind('click', function() {
            jq("#taskActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#" + target).removeClass("open");
            var targetParent = jq("#" + target).parent();
            jq("#" + target).remove();
            if (jq(targetParent).hasClass('listTasks')) {
                if (jq(targetParent).children('.task').length == 0) {
                    jq(targetParent).closest('.milestone').find('.milestoneTasksContainer').hide();
                    jq(targetParent).closest(".milestone").find(".addTask").removeClass("hide");
                }
            }
        });

        jq("#taskActionPanel .actionList #editTask").bind('click', function() {
            jq("#taskActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            var task = jq("#" + target);
            jq(task).removeClass("open");
            jq("#addTaskContainer").addClass('edit');
            jq("#addTaskContainer").attr('target', target);
            jq("#addTaskContainer").insertAfter(task);
            jq(task).hide();
            jq("#addTaskContainer #newTaskTitle").val(jq(task).children(".title").text());
            jq("#addTaskContainer").show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#saveTemplate").bind("click", function() {
            jq(".requiredFieldError").removeClass("requiredFieldError");
            hideAddTaskContainer();
            hideAddMilestoneContainer();
            if (jq.trim(jq("#templateTitle").val()) == "") {
                jq("#templateTitleContainer").addClass("requiredFieldError");
                return false;
            }
            generateAndSaveTemplate('save');
            return false;
        });

        jq('#createProject').bind('click', function() {
            jq(".requiredFieldError").removeClass("requiredFieldError");
            hideAddTaskContainer();
            hideAddMilestoneContainer();
            if (jq.trim(jq("#templateTitle").val()) == "") {
                jq("#templateTitleContainer").addClass("requiredFieldError");
                return false;
            }
            generateAndSaveTemplate('saveAndCreateProj');
        });
    };

    var showTmplStructure = function(tmpl) {
        var description = jQuery.parseJSON(tmpl.description);

        var milestones = description.milestones;
        if (milestones) {
            for (var i = 0; i < milestones.length; i++) {
                milestoneCounter++;
                if (milestones[i].duration || milestones[i].duration > 6) {
                    var duration = jq("#addMilestoneContainer select option[duration='" + milestones[i].duration + "']").text();
                    if (duration == "") {
                        duration = milestones[i].duration.toString();
                        duration = duration.replace('.', ',');
                        milestones[i].duration = jq("#addMilestoneContainer select option[duration^='" + duration + "']").text();
                    } else {
                        milestones[i].duration = duration;
                    }
                } else {
                    milestones[i].duration = jq("#addMilestoneContainer select option:first-child").text();
                }
                milestones[i].number = milestoneCounter;
                milestones[i].displayTasks = milestones[i].tasks.length ? true : false;
                jq("#milestoneTmpl").tmpl(milestones[i]).appendTo("#listAddedMilestone");
            }
        }
        var noAssignTasks = description.tasks;
        if (noAssignTasks) {
            for (var i = 0; i < noAssignTasks.length; i++) {
                taskCounter++;
                noAssignTasks[i].number = taskCounter;
                jq("#taskTmpl").tmpl(noAssignTasks[i]).appendTo("#listNoAssignListTask");
            }
            jq("#addTaskContainer").attr("target", 'noAssign');
            hideAddTaskContainer();
            showAction('noAssign');
        }

    };

    var generateAndSaveTemplate = function(mode) {
        var description = { tasks: new Array(), milestones: new Array() };

        var listNoAssCont = jq('#noAssignTaskContainer .task');
        for (var i = 0; i < listNoAssCont.length; i++) {
            description.tasks.push({ title: jq(listNoAssCont[i]).children('.title').text() });
        }


        var listMilestoneCont = jq("#listAddedMilestone .milestone");
        for (var i = 0; i < listMilestoneCont.length; i++) {
            var duration = jq(listMilestoneCont[i]).children(".mainInfo").children('.daysCount').attr('value');
            duration = duration.replace(',', '.');
            duration = parseFloat(duration);
            var milestone = { title: jq(listMilestoneCont[i]).children(".mainInfo").children('.title').text(),
                duration: duration,
                tasks: new Array()
            };

            var listTaskCont = jq(listMilestoneCont[i]).children('.milestoneTasksContainer').children(".listTasks").children('.task');
            for (var j = 0; j < listTaskCont.length; j++) {
                milestone.tasks.push({ title: jq(listTaskCont[j]).children('.title').text() });
            }

            description.milestones.push(milestone);
        }

        var templateTitle = jq.trim(jq("#templateTitle").val());
        if (mode == 'save') {
            AjaxPro.EditProjectTemplate.SaveTemplate({ Id: tmplId, Title: templateTitle, Description: JSON.stringify(description) }, onSave);
        } else {
            AjaxPro.EditProjectTemplate.SaveTemplateAndCreateProject({ Id: tmplId, Title: templateTitle, Description: JSON.stringify(description) }, onSaveAndCreate);
        }

    };

    var onSave = function() {
        document.location.replace("projecttemplates.aspx");
    };
    var onSaveAndCreate = function(response) {
        var tmplId = response.value;
        if (tmplId) {
            document.location.replace("createprojectfromtemplate.aspx?id=" + tmplId);
        }
    };
    return {
        init: init
    };

})(jQuery);

window.ASC.Projects.CreateProjectFromTemplate = (function() {
    var templId, regionalFormatDate, chooseRespStr, showRespCombFlag = false,
        milestoneCounter = 0, taskCounter = 0;

    var showChooseResponsible = function() {
        var projectMembers = jq("#Team").find(".projectMember");
        var pmName = projectManagerSelector.SelectedUserName;
        if ((projectMembers.length != 0 || pmName.length != 0)) {
            var tasksMenuBtn = jq(".task").find(".menuButton");
            for (var i = 0; i < tasksMenuBtn.length; i++) {
                jq(tasksMenuBtn[i]).after(chooseRespStr);
            }
            var milestMenuBtn = jq(".milestone .mainInfo").find(".menuButton");
            for (var i = 0; i < milestMenuBtn.length; i++) {
                jq(milestMenuBtn[i]).after(chooseRespStr);
            }
            jq(".milestone .mainInfo .chooseResponsible").removeClass("nobody");
            jq("#addTaskContainer #newTaskTitle").after(chooseRespStr);
            jq("#addMilestoneContainer #newMilestoneTitle").after(chooseRespStr);
            updateProjectMemberPanel();
            showRespCombFlag = true;
        } else {
            jq(".chooseResponsible").remove();
            showRespCombFlag = false;
        }
    };

    var updateProjectMemberPanel = function() {
        jq("#projectMemberPanel .actionList li").remove();
        var projectMembers = jq("#Team").find(".projectMember");
        if (projectMembers.length) {
            for (var i = 0; i < projectMembers.length; i++) {
                var user = jq(projectMembers[i]).find(".userLink");

                if (!user.length) {
                    user = jq(projectMembers[i]);
                }

                var guid = jq(user).attr('guid');
                var name = jq(user).text();
                jq("#projectMemberPanel .actionList").append("<li id='" + guid + "'>" + name + "</li>");
            }
        }
        updateMilestoneResponsible();
        jq("#projectMemberPanel .actionList").prepend("<li id='nobody'>" + jq("#projectMemberPanel .actionList").attr("nobodyItemText") + "</li>");
    };
    var chooseMilResp = function(milestone, team) {
        var oldResp = jq(milestone).attr("guid");
        var inTeam = false;
        for (var i = 0; i < team.length; i++) {
            if (jq(team[i]).attr("guid") == oldResp) {
                inTeam = true;
                break;
            }
        }
        return inTeam;
    };
    var updateMilestoneResponsible = function() {
        var team = jq("#Team .userLink");
        var pmName = projectManagerSelector.SelectedUserName;
        var pmGuid = projectManagerSelector.SelectedUserId;
        var listMilestone = jq(".milestone .mainInfo .chooseResponsible .dottedLink");
        for (var i = 0; i < listMilestone.length; i++) {
            var isInTeam = chooseMilResp(jq(listMilestone[i]), team);
            if (!isInTeam) {
                if (pmGuid != null) {
                    jq(listMilestone[i]).attr("guid", pmGuid);
                    jq(listMilestone[i]).text(pmName);

                }
                else {
                    jq(listMilestone[i]).attr("guid", jq("#Team .userLink:last").attr("guid"));
                    jq(listMilestone[i]).text(jq("#Team .userLink:last").text());
                }
                jq(listMilestone[i]).parent().css("display", "inline-block");
            }
        }
    };

    var init = function(str, dateMask) {
        chooseRespStr = str;
        //datepicker
        jq("#dueDate").val("");
        jq("#dueDate").datepicker().mask(dateMask);
        jq("#dueDate").datepicker({
            onSelect: function() {
                jq("#newMilestoneTitle").focus();
            }
        });
        regionalFormatDate = jq("#dueDate").datepicker("option", "dateFormat");

        //get tmpl
        templId = jq.getURLParam('tmplId');
        if (template) {
            var val = jq.format(jq("#projectTitle").attr("defText"), template.title);
            jq("#projectTitle").val(val);
            showProjStructure(template);
        }

        jq("#projectDescription").val("");
        jq("#notifyManagerCheckbox").attr("disabled", "disabled");

        //team popup
        jq('#manageTeamButton').click(function() {
            jq('#Team span.userLink').each(function() {
                var userId = jq(this).attr('guid');
                projectTeamSelector.SelectUser(userId);
            });

            var pmGuid = projectManagerSelector.SelectedUserId;
            if (pmGuid != null) {
                projectTeamSelector.SelectUser(pmGuid);
            }

            projectTeamSelector.IsFirstVisit = true;
            projectTeamSelector.ShowDialog();
        });

        jq('#Team .projectMember span').live('click', function() {
            var userId = jq(this).attr('guid');
            jq(this).parent().remove();

            jq('#usrdialog_' + projectTeamSelector.ID + '_usr_' + userId).attr('checked', 'checked');
            projectTeamSelector.PreSelectUser(userId);
            projectTeamSelector.SelectUser(userId);
            projectTeamSelector.Unselect();

            var projectMembers = jq("#Team").find(".projectMember");
            if (projectMembers.length) {
                updateProjectMemberPanel();
            } else {
                showChooseResponsible();
            }
            var tasksResp = jq(".task .chooseResponsible .dottedLink[guid='" + userId + "']");
            var tasks = new Array();
            for (var i = 0; i < tasksResp.length; i++) {
                tasks.push(jq(tasksResp[i]).closest(".task"));
            }
            jq(tasksResp).closest(".chooseResponsible").remove();
            for (var i = 0; i < tasks.length; i++) {
                var button = jq(tasks[i]).find(".menuButton");
                jq(button).after(chooseRespStr);
            }
        });

        projectTeamSelector.OnOkButtonClick = function() {
            jq('#Team').empty();

            var pmGuid = projectManagerSelector.SelectedUserId;
            if (pmGuid != null) {
                var newMember = '<div class="projectMember userLink" guid=' + pmGuid + '>' + projectManagerSelector.SelectedUserName + '</div>';
                jq(newMember).appendTo("#Team");
            }

            var members = projectTeamSelector.GetSelectedUsers();
            for (var i = 0; i < members.length; i++) {
                var pid = members[i].ID;
                var pname = members[i].Name;

                if (pmGuid != pid) {
                    var newMember = ('<div class="projectMember"><span class="userLink" guid=' + pid + '>' + pname + '</span></div>');
                    jq(newMember).appendTo("#Team");
                }
            }
            if (jq(".chooseResponsible").length) {
                updateProjectMemberPanel();
            } else {
                showChooseResponsible();
            }
        };
        //choose responsible
        jq(".task .chooseResponsible").live("click", function() {
            jq(this).closest(".task").addClass('open');
            showActionsPanel("projectMemberPanel", this);
            jq("#projectMemberPanel #nobody").show();
            var target = jq(this).closest(".task").attr("id");
            jq("#projectMemberPanel").attr("target", target);
        });

        jq("#addTaskContainer .chooseResponsible").live("click", function() {
            showActionsPanel("projectMemberPanel", this);
            jq("#projectMemberPanel #nobody").show();
            jq("#projectMemberPanel").attr("target", "newTask");
        });

        jq("#addMilestoneContainer .chooseResponsible").live("click", function() {
            showActionsPanel("projectMemberPanel", this);
            jq("#projectMemberPanel #nobody").hide();
            jq("#projectMemberPanel").attr("target", "newMilestone");
        });

        jq(".milestone .mainInfo .chooseResponsible").live("click", function() {
            jq(this).closest(".milestone").addClass('open');
            showActionsPanel("projectMemberPanel", this);
            jq("#projectMemberPanel #nobody").hide();
            var target = jq(this).closest(".milestone").attr("id");
            jq("#projectMemberPanel").attr("target", target);
        });

        jq("#projectMemberPanel ul li").live("click", function() {
            jq(".actionPanel").hide();
            var target = jq("#projectMemberPanel").attr("target");
            jq("#" + target).removeClass("open");
            var type = jq("#" + target).attr("class");
            switch (type) {
                case "milestone":
                    {
                        target = jq("#" + target + " .mainInfo");
                        break;
                    }
                case "task menuButtonContainer":
                    {
                        target = jq("#" + target);
                        break;
                    }
                default:
                    {
                        if (target == "newTask") {
                            target = jq("#addTaskContainer");
                        }
                        else {
                            target = jq("#addMilestoneContainer");
                        }
                    }
            }

            var guid = jq(this).attr("id");
            var name;
            if (guid == "nobody" || guid == "") {
                name = jq("#projectMemberPanel .actionList").attr("chooseRespText");
                if (type != "newTask") {
                    jq(target).find(".chooseResponsible").addClass("nobody");
                }
            } else {
                name = jq(this).text();
                jq(target).find(".chooseResponsible").removeClass("nobody");
            }
            var member = jq(target).find(".dottedLink");
            jq(member).attr("guid", guid);
            jq(member).text(name);
            jq(target).find("input").last().focus();
        });

        // onChoosePM
        projectManagerSelector.AdditionalFunction = function() {
            var projectMembers = jq("#Team").find(".projectMember");
            if (projectMembers.length != 0 || showRespCombFlag) {
                updateProjectMemberPanel();
            } else {
                showChooseResponsible();
            }
            updateMilestoneResponsible();
            var pmGuid = projectManagerSelector.SelectedUserId;
            var creatorGuid = jq("#notifyManagerCheckbox").attr("creatorId");
            if (pmGuid != creatorGuid) {
                jq("#notifyManagerCheckbox").removeAttr("disabled");
            }
            else {
                jq("#notifyManagerCheckbox").attr("disabled", "disabled");
                jq("#notifyManagerCheckbox").removeAttr("checked");
            }
            jq('#usrdialog_' + projectTeamSelector.ID + '_usr_' + pmGuid).attr('checked', 'checked');
            projectTeamSelector.PreSelectUser(pmGuid);
            projectTeamSelector.SelectUser(pmGuid);
            projectTeamSelector.OnOkButtonClick();
        };


        //milestone
        jq(".milestone .mainInfo .title, .milestone .mainInfo .dueDate").live('click', function() {
            hideAddMilestoneContainer();
            hideAddTaskContainer();
            jq("#addMilestoneContainer").addClass('edit');
            var target = jq(this).closest('.milestone').attr('id');
            jq("#addMilestoneContainer").attr('target', target);
            var milestone = jq("#" + target + " .mainInfo");
            jq("#addMilestoneContainer").prependTo(jq("#" + target));
            jq(milestone).hide();
            var val = jq(milestone).children(".dueDate").text();
            val = jq.datepicker.parseDate(regionalFormatDate, val);

            var chooseRespBlock = jq(milestone).find(".chooseResponsible").clone();
            jq("#addMilestoneContainer .chooseResponsible").remove();
            jq("#addMilestoneContainer").append(jq(chooseRespBlock));

            var pmGuid = projectManagerSelector.SelectedUserId;
            var pm = projectManagerSelector.SelectedUserName;
            if (pmGuid) {
                jq(chooseRespBlock).find(".dottedLink").attr("guid", pmGuid);
                jq(chooseRespBlock).find(".dottedLink").text(pm);
            }

            jq("#addMilestoneContainer").show();
            jq("#addMilestoneContainer #newMilestoneTitle").val(jq(milestone).children(".title").text());

            jq("#addMilestoneContainer #dueDate").datepicker('setDate', val);
            jq("#addMilestoneContainer #newMilestoneTitle").focus();
        });

        jq("#addMilestone a").bind('click', function() {
            hideAddTaskContainer();
            jq("#addMilestoneContainer").hide();
            if (jq("#addMilestoneContainer").hasClass('edit')) {
                jq("#" + jq("#addMilestoneContainer").attr('target')).find(".mainInfo").show();
                jq("#addMilestoneContainer").removeClass('edit');
            }

            jq("#addMilestone").after(jq("#addMilestoneContainer"));
            jq("#addMilestone").hide();
            jq("#newMilestoneTitle").val('');

            if (jq("#dueDate").length) {
                var defDate = new Date();
                defDate.setDate(defDate.getDate() + 3);
                jq("#dueDate").datepicker("setDate", defDate);
            }
            var chooseRespBlock = jq("#addMilestoneContainer").find(".chooseResponsible");
            if (chooseRespBlock.length) {
                var pmGuid = projectManagerSelector.SelectedUserId;
                var pm = projectManagerSelector.SelectedUserName;
                if (pmGuid) {
                    jq(chooseRespBlock).find(".dottedLink").attr("guid", pmGuid);
                    jq(chooseRespBlock).find(".dottedLink").text(pm);
                }
            }
            jq("#addMilestoneContainer").show();
            jq("#addMilestoneContainer #newMilestoneTitle").focus();
        });

        jq("#newMilestoneTitle").bind('keydown', function(e) {
            var targetId = jq("#addMilestoneContainer").attr('target');
            if (e.which == 13) {
                var text = jq.trim(jq(this).val());
                if (!text.length) {
                    alert(jq("#milestoneError").text());
                    return false;
                }
                var milestoneId;
                var date = jq("#dueDate").datepicker("getDate");
                date = jq.datepicker.formatDate(regionalFormatDate, date);
                if (jq("#addMilestoneContainer").hasClass('edit')) {

                    jq("#" + targetId + " .mainInfo .title").text(jq.trim(jq(this).val()));

                    jq("#" + targetId + " .mainInfo .dueDate span").text(date);
                    jq("#addMilestoneContainer").hide();
                    jq("#" + targetId + " .mainInfo").show();
                    jq("#addMilestoneContainer").removeClass('edit');
                    milestoneId = targetId;
                    jq("#" + targetId + " .mainInfo .chooseResponsible").remove();
                } else {
                    milestoneCounter++;
                    milestoneId = "m_" + milestoneCounter;
                    var milestone = {};
                    milestone.title = jq.trim(jq(this).val());
                    milestone.date = date;
                    milestone.tasks = [];
                    milestone.number = milestoneCounter;
                    jq("#milestoneTmpl").tmpl(milestone).appendTo("#listAddedMilestone");
                    jq(this).val("");
                    jq(this).focus();
                }
                if (jq("#addMilestoneContainer .chooseResponsible").length) {
                    var chooseRespBlock = jq("#addMilestoneContainer .chooseResponsible").clone();
                    jq("#" + milestoneId + " .mainInfo .menuButton").after(jq(chooseRespBlock));
                    if (jq(chooseRespBlock).attr('guid') != "") {
                        jq("#" + milestoneId + " .mainInfo .chooseResponsible").removeClass("nobody");
                    } else {
                        jq("#" + milestoneId + " .mainInfo .chooseResponsible").addClass("nobody");
                    }
                }
                return false;
            } else {
                if (e.which == 27) {
                    jq(this).val("");
                    if (jq("#addMilestoneContainer").hasClass('edit')) {
                        jq("#addMilestoneContainer").hide();
                        jq("#" + targetId + " .mainInfo").show();
                    } else {
                        jq("#addMilestoneContainer").hide();
                        jq("#addMilestone").show();
                    }
                }
            }
        });

        //milestone menu

        jq(".milestone .mainInfo .addTask").live('click', function() {
            hideAddMilestoneContainer();
            hideAddTaskContainer();
            var target = jq(this).closest('.milestone').attr('id');
            var milestTasksCont = jq("#" + target + " .milestoneTasksContainer");
            jq(milestTasksCont).find('.addTaskContainer').hide();
            jq("#addTaskContainer").appendTo(milestTasksCont[0]);
            jq("#addTaskContainer").attr("target", target);
            jq("#addTaskContainer").show();
            jq(milestTasksCont).show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#milestoneActionPanel .actionList #removeMilestone").bind('click', function() {
            jq("#addTaskContainer").hide();
            jq("#addTaskContainer").appendTo("#noAssignTaskContainer");
            jq("#milestoneActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#" + target).removeClass("open");
            jq("#" + target).remove();
        });

        jq("#milestoneActionPanel .actionList #addTaskInMilestone").bind('click', function() {
            hideAddMilestoneContainer();
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#" + target).removeClass("open");
            var listTasks = jq(".listTasks[milestone='" + target + "']");
            var milestTasksCont = jq(listTasks[0]).parents(".milestoneTasksContainer");
            jq("#addTaskContainer").appendTo(milestTasksCont[0]);
            jq(milestTasksCont).find('.addTaskContainer').hide();
            jq("#addTaskContainer").attr("target", target);
            jq("#addTaskContainer").show();
            jq(milestTasksCont).show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#milestoneActionPanel .actionList #editMilestone").bind('click', function() {
            jq("#milestoneActionPanel").hide();
            hideAddMilestoneContainer();
            jq("#addMilestoneContainer").addClass('edit');
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#addMilestoneContainer").attr('target', target);
            jq("#" + target).removeClass("open");
            var milestone = jq("#" + target + " .mainInfo");
            jq("#addMilestoneContainer").prependTo(jq("#" + target));
            jq(milestone).hide();
            var val = jq(milestone).children(".dueDate").text();
            val = jq.datepicker.parseDate(regionalFormatDate, val);

            var chooseRespBlock = jq(milestone).find(".chooseResponsible").clone();
            jq("#addMilestoneContainer .chooseResponsible").remove();
            jq("#addMilestoneContainer").append(jq(chooseRespBlock));

            var pmGuid = projectManagerSelector.SelectedUserId;
            var pm = projectManagerSelector.SelectedUserName;
            if (pmGuid) {
                jq(chooseRespBlock).find(".dottedLink").attr("guid", pmGuid);
                jq(chooseRespBlock).find(".dottedLink").text(pm);
            }

            jq("#addMilestoneContainer").show();
            jq("#addMilestoneContainer #newMilestoneTitle").val(jq(milestone).children(".title").text());

            jq("#addMilestoneContainer #dueDate").datepicker('setDate', val);
            jq("#addMilestoneContainer #newMilestoneTitle").focus();
        });

        //task

        jq(".task .title").live('click', function() {
            hideAddMilestoneContainer();
            jq("#addTaskContainer").hide();
            if (jq("#addTaskContainer").hasClass('edit')) {
                jq('#' + jq("#addTaskContainer").attr('target')).show();
            } else {
                jq("#addTaskContainer").addClass('edit');
                jq('.addTaskContainer').show();
            }

            var target = jq(this).parents('.task');
            jq("#addTaskContainer").addClass('edit');
            jq("#addTaskContainer").attr('target', jq(target).attr("id"));
            jq("#addTaskContainer").insertAfter(target);
            jq(target).hide();

            var chooseRespBlock = jq(target).find(".chooseResponsible").clone();
            jq("#addTaskContainer .chooseResponsible").remove();
            jq("#addTaskContainer").append(jq(chooseRespBlock));
            jq("#addTaskContainer #newTaskTitle").val(jq(target).children(".title").text());
            jq("#addTaskContainer").show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#newTaskTitle").bind('keydown', function(e) {
            var target = jq(this).parent().attr('target');
            if (e.which == 13) {
                var text = jq.trim(jq(this).val());
                if (!text.length) {
                    alert(jq("#taskError").text());
                    return false;
                }
                var taskId;
                if (jq("#addTaskContainer").hasClass('edit')) {
                    jq("#" + target + " .title").text(jq(this).val());
                    jq("#addTaskContainer").removeClass('edit');
                    hideAddTaskContainer();
                    jq("#" + target).show();
                    taskId = target;
                    jq(".task[id='" + taskId + "'] .chooseResponsible").remove();
                } else {
                    taskCounter++;
                    taskId = "t_" + taskCounter;
                    var task = {};
                    task.title = jq.trim(jq(this).val());
                    task.number = taskCounter;
                    var tElem;
                    if (target == 'noAssign') {
                        tElem = jq("#listNoAssignListTask");
                    }
                    else {
                        tElem = jq(".listTasks[milestone='" + target + "']");
                        jq("#" + target).find(".addTask").addClass("hide");
                    }
                    jq("#taskTmpl").tmpl(task).appendTo(tElem);

                    jq(this).val("");
                    jq(this).focus();
                }
                if (jq("#addTaskContainer .chooseResponsible").length) {
                    var chooseRespBlock = jq("#addTaskContainer .chooseResponsible").clone();
                    jq(".task[id='" + taskId + "'] .menuButton").after(jq(chooseRespBlock));
                    var guid = jq(chooseRespBlock).find(".dottedLink").attr('guid');
                    if (guid != "" & guid != "nobody" & guid !== undefined) {
                        jq(".task[id='" + taskId + "'] .chooseResponsible").removeClass("nobody");
                    } else {
                        jq(".task[id='" + taskId + "'] .chooseResponsible").addClass("nobody");
                    }
                }
                return false;
            } else {
                if (e.which == 27) {
                    jq(this).val("");
                    if (jq("#addTaskContainer").hasClass('edit')) {
                        hideAddTaskContainer();
                        jq("#addTaskContainer").removeClass('edit');
                        jq("#" + target).show();
                    } else {
                        hideAddTaskContainer();
                    }
                }
            }
        });

        //task menu

        jq("#taskActionPanel .actionList #removeTask").bind('click', function() {
            jq("#taskActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            jq("#" + target).removeClass("open");
            var targetParent = jq("#" + target).parent();
            jq("#" + target).remove();
            if (jq(targetParent).hasClass('listTasks')) {
                if (jq(targetParent).children('.task').length == 0) {
                    jq(targetParent).parents('.milestone').children('.milestoneTasksContainer').hide();
                    jq(targetParent).closest(".milestone").find(".addTask").removeClass("hide");
                }
            }
        });

        jq("#taskActionPanel .actionList #editTask").bind('click', function() {
            jq("#taskActionPanel").hide();
            var target = jq(this).parents('.actionPanel').attr('target');
            var task = jq("#" + target);
            jq(task).removeClass("open");
            jq("#addTaskContainer").addClass('edit');
            jq("#addTaskContainer").attr('target', target);
            jq("#addTaskContainer").insertAfter(task);
            jq(task).hide();

            var chooseRespBlock = jq(task).find(".chooseResponsible").clone();
            jq("#addTaskContainer .chooseResponsible").remove();
            jq("#addTaskContainer").append(jq(chooseRespBlock));
            jq("#addTaskContainer #newTaskTitle").val(jq(task).children(".title").text());
            jq("#addTaskContainer").show();
            jq("#addTaskContainer #newTaskTitle").focus();
        });

        jq("#createProject").click(function() {
            jq(".requiredFieldError").removeClass("requiredFieldError");

            if (jq.trim(jq("#projectTitle").val()) == "") {
                jq("#projectTitleContainer").addClass("requiredFieldError");
                return false;
            }

            if (projectManagerSelector.SelectedUserName == "") {
                jq("#pmContainer").addClass("requiredFieldError");
                return false;
            }
            createProject();
        });
    };

    var showProjStructure = function(tmpl) {
        var description = jQuery.parseJSON(tmpl.description);

        var milestones = description.milestones;
        if (milestones) {
            for (var i = 0; i < milestones.length; i++) {
                milestoneCounter++;
                var duration = parseFloat(milestones[i].duration);
                var date = new Date();
                date.setDate(date.getDate() + duration * 30);
                milestones[i].number = milestoneCounter;
                milestones[i].date = jq.datepicker.formatDate(regionalFormatDate, date);
                milestones[i].displayTasks = milestones[i].tasks.length ? true : false;
                jq("#milestoneTmpl").tmpl(milestones[i]).appendTo("#listAddedMilestone");
            }
        }
        var noAssignTasks = description.tasks;
        if (noAssignTasks) {
            for (var i = 0; i < noAssignTasks.length; i++) {
                taskCounter++;
                noAssignTasks[i].number = taskCounter;
                jq("#taskTmpl").tmpl(noAssignTasks[i]).appendTo("#listNoAssignListTask");
            }
            jq("#addTaskContainer").attr("target", 'noAssign');
        }
        showAction('noAssign');
    };

    var getProjMilestones = function() {
        var milestones = new Array();

        var listMilestoneCont = jq("#listAddedMilestone .milestone");
        for (var i = 0; i < listMilestoneCont.length; i++) {
            var deadline = jq.datepicker.parseDate(regionalFormatDate, jq(listMilestoneCont[i]).children(".mainInfo").children('.dueDate').text());
            var milestone = { Title: jq(listMilestoneCont[i]).children(".mainInfo").children('.title').text(),
                DeadLine: '/Date(' + deadline.getTime() + ')/',
                Description: new Array()
            };

            var mResponsible = jq(listMilestoneCont[i]).find(".mainInfo").find(".dottedLink");
            if (mResponsible.length) {
                var guid = jq(mResponsible).attr("guid");
                if (typeof guid != 'undefined' && guid != "") {
                    milestone.Responsible = guid;
                }
            }

            var listTaskCont = jq(listMilestoneCont[i]).children('.milestoneTasksContainer').children(".listTasks").children('.task');
            for (var j = 0; j < listTaskCont.length; j++) {
                var task = { Title: jq(listTaskCont[j]).children('.title').text() };
                var tResponsible = jq(listTaskCont[j]).find(".dottedLink");
                if (tResponsible.length) {
                    var guid = jq(tResponsible).attr("guid");
                    if (typeof guid != 'undefined' && guid != "") {
                        task.Responsible = guid;
                    }
                }
                milestone.Description.push(task);
            }

            milestones.push(milestone);
        }

        return milestones;
    };

    var getNoAssignTasks = function() {
        var listNoAssTaskStr = new Array();
        var listNoAssCont = jq('#noAssignTaskContainer .task');

        for (var i = 0; i < listNoAssCont.length; i++) {
            var task = { Title: jq(listNoAssCont[i]).children('.title').text() };

            var responsible = jq(listNoAssCont[i]).find(".dottedLink");
            if (responsible.length) {
                var guid = jq(responsible).attr("guid");
                if (typeof guid != 'undefined' && guid != "") {
                    task.Responsible = guid;
                }
            }

            listNoAssTaskStr.push(task);
        }

        return listNoAssTaskStr;
    };

    var createProject = function() {

        var project = {
            Title: jq("#projectTitle").val(),
            Description: jq("#projectDescription").val(),
            Responsible: projectManagerSelector.SelectedUserId,
            Private: jq("#projectPrivacyCkeckbox").is(':checked')
        };

        var team = [];
        var participants = jq("#Team .projectMember .userLink");
        for (var i = 0; i < participants.length; i++) {
            team.push(jq(participants[i]).attr("guid"));
        }

        var milestones = getProjMilestones();
        var noAssignTasks = getNoAssignTasks();
        var notifyManager = jq("#notifyManagerCheckbox").is(":checked");
        var notifyResponsibles = jq('#notifyResponsibles').is(':checked');

        AjaxPro.CreateProjectFromTemplate.CreateProject(project, team, JSON.stringify(milestones), JSON.stringify(noAssignTasks), notifyManager, notifyResponsibles, onCreate);
    };

    var onCreate = function(response) {
        document.location.replace("projects.aspx" + response.value);
    };

    return {
        init: init
    };

})(jQuery);