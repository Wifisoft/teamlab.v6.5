window.milestones = (function() {
    var isInit = false;

    var myGuid;
    var isAdmin;

    var statusListContainer;
    var milestoneActionContainer;

    var currentProjectId;
    var currentFilter = {};

    var selectedStatusCombobox;
    var selectedActionCombobox;

    var newMilestoneButton;
    var showNextMilestonesButton;
    var advansedFilter;
    var presetContainer;

    var descriptionTimeout;
    var overDescriptionPanel = false;

    var basePath = 'sortBy=deadline&sortOrder=ascending';

    var currentMilestonesCount = 0;

    var getMilestoneActiveTasksLink = function(prjId, milestoneId) {
        return 'tasks.aspx?prjID=' + prjId + '#' + basePath + '&milestone=' + milestoneId + '&status=open';
    };

    var getMilestoneClosedTasksLink = function(prjId, milestoneId) {
        return 'tasks.aspx?prjID=' + prjId + '#' + basePath + '&milestone=' + milestoneId + '&status=closed';
    };

    var getMyGuid = function() {
        return myGuid;
    };

    var getCurrentProjectId = function() {
        return currentProjectId;
    };

    var setCurrentFilter = function(filter) {
        currentFilter = filter;
    };

    var showAdvansedFilter = function() {
        advansedFilter.css('visibility', 'visible');
        presetContainer.css('visibility', 'visible');
    };

    var hideAdvansedFilter = function() {
        advansedFilter.css('visibility', 'hidden');
        presetContainer.css('visibility', 'hidden');
    };

    var showNewMilestoneButton = function() {
        newMilestoneButton.css('visibility', 'visible');
    };

    var hideNewMilestoneButton = function() {
        newMilestoneButton.css('visibility', 'hidden');
    };

    var initPresets = function() {
        //my milestones
        var path = '#' + basePath + '&user_tasks=' + getMyGuid() + '&status=open';
        jq('.presetContainer #preset_my').attr('href', path);
        //in my projects
        path = '#' + basePath + "&myprojects=true&status=open";
        jq('.presetContainer #preset_inmyproj').attr('href', path);
        //upcoming
        var date = new Date();
        var deadlineStart = date.getTime();
        date.setDate(date.getDate() + 7);
        var deadlineStop = date.getTime();
        path = '#' + basePath + "&user_tasks=" + getMyGuid() + "&deadlineStart=" + deadlineStart + "&deadlineStop=" + deadlineStop;
        jq('.presetContainer #preset_upcoming').attr('href', path);
    };

    //filter Set
    var onSetFilterMilestones = function(evt, $container) {
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            initPresets();
            ASC.Projects.ProjectsAdvansedFilter.presetAlign();
        }
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
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
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        milestones.setCurrentFilter(filter);
        milestones.getMilestones(filter);
        milestones.showPreloader();
    	if (path !== hash) {
    		ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };

    //filter Reset
    var onResetFilterMilestones = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        milestones.setCurrentFilter(filter);
        milestones.getMilestones(filter);
        milestones.showPreloader();
    };

    var init = function(isAdministrartor, guid) {
        isAdmin = isAdministrartor === 'True';
        myGuid = guid;

        if (isInit === false) {
            isInit = true;
        }

        currentProjectId = jq.getURLParam('prjID');

        //filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(myGuid, basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterMilestones;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterMilestones;

        jq("#EmptyListForFilter").on("click", '.emptyScrBttnPnl .baseLinkAction', function() {
            ASC.Controls.AnchorController.move(basePath);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
            return false;
        });

        // popup handlers
        jq('#questionWindowDeleteMilestone .remove').on('click', function() {
            var milestoneId = jq("#questionWindowDeleteMilestone").attr("milestoneId");
            jq('#milestonesList tr#' + milestoneId).html('<td class="process" colspan="8"></td>');
            deleteMilestone(milestoneId);
            jq("#questionWindowDeleteMilestone").removeAttr("milestoneId");
            jq.unblockUI();
            return false;
        });
        jq('#questionWindowDeleteMilestone .cancel').on('click', function() {
            jq.unblockUI();
            return false;
        });

        statusListContainer = jq('#statusListContainer');
        milestoneActionContainer = jq('#milestoneActionContainer');

        newMilestoneButton = jq('#newMilestoneButton');
        advansedFilter = jq('#AdvansedFilter');
        presetContainer = jq('.presetContainer');
        showNextMilestonesButton = jq('#showNextMilestonesButton');

        if (!currentProjectId) {
            if (typeof (tags) == 'undefined' || typeof (projects) == 'undefined') return;

            tags = jq.parseJSON(jQuery.base64.decode(tags)).response;
            projects = Teamlab.create('prj-projects', null, jq.parseJSON(jQuery.base64.decode(projects)).response);

            var filterTags = [];
            var filterProjects = [];

            for (var j = 0; j < tags.length; j++) {
                filterTags.push({ value: tags[j].id, title: tags[j].title });
            }

            var milestoneProject = jq('#milestoneProject');
            var taskProject = jq('#taskProject');

            for (var i = 0; i < projects.length; i++) {
                var project = projects[i];
                var obj = { value: project.id, title: project.title };
                filterProjects.push(obj);

                if (isAdmin || (project.responsible.id == myGuid && project.status == 0)) {
                    milestoneProject.append(createOptionElement(obj));
                    taskProject.append(createOptionElement(obj));
                }
            }

            milestoneProject.tlcombobox();
            taskProject.tlcombobox();
            createAdvansedFilter(filterTags, filterProjects);
        }
        else {
            if (typeof (projectParticipants) != 'undefined') {
                projectParticipants = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectParticipants)).response);

                var milestoneResponsible = jq('#milestoneResponsible');
                for (var k = 0; k < projectParticipants.length; k++) {
                    var p = projectParticipants[k];
                    milestoneResponsible.append(jq('<option value="' + p.id + '"></option>').html(p.displayName));
                }
            }
            createAdvansedFilter();
        }

        var listEscs = jq('.noContentBlock');
        jq(listEscs[0]).appendTo('#EmptyListForFilter');
        jq(listEscs[1]).appendTo('#EmptyListMilestone');
        jq(listEscs).show();
        if (jq('#newMilestoneButton').length == 0) {
            jq('#EmptyListMilestone .noContentBlock .emptyScrBttnPnl').remove();
        }

        // ga-track-events

        //add
        jq("#newMilestoneButton").trackEvent(ga_Categories.milestones, ga_Actions.createNew, 'create-new-milestone');

        //presets
        jq('.presetContainer #preset_my').trackEvent(ga_Categories.milestones, ga_Actions.presetClick, 'my-milestones');
        jq('.presetContainer #preset_inmyproj').trackEvent(ga_Categories.milestones, ga_Actions.presetClick, 'in-my-projects');
        jq('.presetContainer #preset_upcoming').trackEvent(ga_Categories.milestones, ga_Actions.presetClick, 'upcoming');

        //show next
        jq("#showNextMilestonesButton").trackEvent(ga_Categories.milestones, ga_Actions.next, 'next-milestones');

        //filter
        jq("#AdvansedFilter .advansed-filter-list li[data-id='me_responsible_for_milestone'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'me_responsible_for_milestone');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='responsible_for_milestone'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'responsible_for_milestone');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='me_tasks'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'me_tasks');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='user_tasks'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'user_tasks');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='open'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'open');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='closed'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'closed');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'myprojects');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='project'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'project');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='tag'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'tag');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='overdue'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'overdue');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='today'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'today');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='upcoming'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'upcoming');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='deadline'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'user-period');

        jq("#AdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'sort');

        jq("#AdvansedFilter .advansed-filter-input").keypress(function(e) {
            if (e.which == 13) {
                try {
                    if (window._gat) {
                        window._gaq.push(['_trackEvent', ga_Categories.milestones, ga_Actions.filterClick, 'text']);
                    }
                } catch (err) {
                }
            }
            return true;
        });

        //change status
        jq("#statusListContainer .open").trackEvent(ga_Categories.milestones, ga_Actions.changeStatus, "open");
        jq("#statusListContainer .closed").trackEvent(ga_Categories.milestones, ga_Actions.changeStatus, "closed");

        //responsible
        jq(".responsible span").trackEvent(ga_Categories.milestones, ga_Actions.userClick, "milestone-responsible");

        //actions
        jq("#addMilestoneTaskButton").trackEvent(ga_Categories.milestones, ga_Actions.actionClick, "add-task-in-milestone");
        //end ga-track-events
    };

    var createOptionElement = function(obj) {
        var option = document.createElement('option');
        option.setAttribute('value', obj.value);
        option.appendChild(document.createTextNode(obj.title));
        return option;
    };

    var getMilestones = function(filter, showNext) {
        var params = {};
        var success = onGetNextMilestones;
        if (!showNext) {
            currentMilestonesCount = 0;
            success = onGetMilestones;
        }
        filter.Count = 30;
        filter.StartIndex = currentMilestonesCount;

        Teamlab.getPrjMilestones(params, { filter: filter, success: success });
    };

    var onGetMilestones = function(params, milestones) {
        clearTimeout(descriptionTimeout);
        currentMilestonesCount += milestones.length;

        var tmplMile, listTmplMiles = new Array();
        if (milestones.length) {
            showNewMilestoneButton();
            showAdvansedFilter();

            for (var i = 0; i < milestones.length; i++) {
                tmplMile = getMilestoneTemplate(milestones[i]);
                listTmplMiles.push(tmplMile);
            }
            jq('#milestonesList tbody').empty();
            jq('#milestoneTemplate').tmpl(listTmplMiles).appendTo('#milestonesList tbody');
            jq('#milestonesListContainer .noContent').hide();

            if (milestones.length == 30) {
                showNextMilestonesButton.show();
            }
            else {
                showNextMilestonesButton.hide();
            }

            jq('#milestonesList').show();
            hidePreloader();
        }
        else {
            hidePreloader();
            jq('#milestonesList tbody').empty();
            jq('#milestonesList').hide();
            jq('#descriptionPanel').hide();
            showNextMilestonesButton.hide();
            if (ASC.Controls.AnchorController.getAnchor() == basePath) {
                jq('#EmptyListForFilter').hide();
                jq('#EmptyListMilestone').show();
                hideNewMilestoneButton();
                hideAdvansedFilter();
                showNextMilestonesButton.hide();
            } else {
                jq('#EmptyListMilestone').hide();
                showAdvansedFilter();
                jq('#EmptyListForFilter').show();
            }
            showNextMilestonesButton.hide();
        }
    };

    var onGetNextMilestones = function(params, milestones) {
        currentMilestonesCount += milestones.length;

        var tmplMile, listTmplMiles = new Array();
        if (milestones.length != 0) {
            for (var i = 0; i < milestones.length; i++) {
                tmplMile = getMilestoneTemplate(milestones[i]);
                listTmplMiles.push(tmplMile);
            }
            jq('#milestoneTemplate').tmpl(listTmplMiles).insertAfter('#milestonesList tbody tr:last');
            jq('#showNextMilestonesProcess').hide();
            if (milestones.length == 30) {
                showNextMilestonesButton.show();
            }
        }
        else {
            jq('#showNextMilestonesProcess').hide();
        }
    };

    var getMilestoneTemplate = function(milestone) {
        var id = milestone.id;
        var prjId = milestone.projectId;
        var template = {
            id: id,
            isKey: milestone.isKey,
            isNotify: milestone.isNotify,
            title: milestone.title,
            activeTasksCount: milestone.activeTaskCount,
            activeTasksLink: getMilestoneActiveTasksLink(prjId, id),
            closedTasksCount: milestone.closedTaskCount,
            closedTasksLink: getMilestoneClosedTasksLink(prjId, id),
            canEdit: milestone.canEdit,
            projectId: prjId,
            projectTitle: milestone.projectTitle,
            createdById: milestone.createdBy.id,
            createdBy: milestone.createdBy.displayName,
            description: milestone.description,
            created: milestone.displayDateCrtdate
        };

        if (milestone.responsible) {
            template.responsible = milestone.responsible.displayName;
            template.responsibleId = milestone.responsible.id;
        } else {
            template.responsible = null;
            template.responsibleId = null;
        }

        var today = new Date();
        var status = milestone.status == 0
            ? today < milestone.deadline
                ? 'active'
                : 'overdue'
            : 'closed';

        template.status = status;
        template.deadline = milestone.displayDateDeadline;

        return template;
    };

    var showPreloader = function() {
        LoadingBanner.displayLoading();
    };

    var hidePreloader = function() {
        LoadingBanner.hideLoading();
    };

    jq('body').on('click', function(event) {
        var target = (event.target) ? event.target : event.srcElement;
        var element = jq(target);
        if (!element.is('.milestoneActionsLink')) {
            hideMilestoneActionContainer();
        }
        if (!(element.is('span.overdue') || element.is('span.active') || element.is('span.closed'))) {
            hideStatusListContainer();
        }
    });

    jq('#newMilestoneButton').on('click', function() {
        showNewMilestonePopup();
    });

    jq('#EmptyListMilestone').on('click', '.noContentBlock .baseLinkAction', function() {
        showNewMilestonePopup();
    });

    var showNewMilestonePopup = function() {
        milestoneaction.clearPanel();
        var milestoneActionButton = jq('#milestoneActionButton');
        milestoneActionButton.html(milestoneActionButton.attr('add'));

        var milestoneActionHeader = jq('#milestoneActionPanel .containerHeaderBlock table td:first');
        milestoneActionHeader.html(ProjectJSResources.AddMilestone);
        
        jq('#milestoneProjectContainer').show();
        
        if (currentProjectId) {
            if (typeof (projectParticipants) != 'undefined') {
                milestoneaction.onGetProjectParticipants({ serverData: true });
            }
            return false;
        }

        var res = /project=(\d*)/.exec(location.hash);
        if (res) {
            jq('#milestoneProject').val(res[1]).change();
        }

        showMilestoneActionPanel();
        return false;
    };

    var showMilestoneActionPanel = function() {
        var margintop = jq(window).scrollTop() - 100;
        margintop = margintop + 'px';
        jq.blockUI({ message: jq('#milestoneActionPanel'),
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
    };

    jq('#milestonesList').on('click', 'td.status .changeStatusCombobox.canEdit', function(event) {
        var element = (event.target) ? event.target : event.srcElement;
        var status = jq(element).attr('class');
        var currentMilestone = selectedStatusCombobox !== undefined ? selectedStatusCombobox.attr('milestoneId') : -1;
        jq('#milestonesList tr#' + currentMilestone + ' td.status .changeStatusCombobox').removeClass('selected');
        selectedStatusCombobox = jq(this);

        if (statusListContainer.attr('milestoneId') !== selectedStatusCombobox.attr('milestoneId')) {
            statusListContainer.attr('milestoneId', selectedStatusCombobox.attr('milestoneId'));
            showStatusListContainer(status);
        } else {
            toggleStatusListContainer(status);
        }
        return false;
    });

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

        statusListContainer.toggle();
    };

    var hideStatusListContainer = function() {
        if (statusListContainer.is(':visible')) {
            selectedStatusCombobox.removeClass('selected');
        }
        statusListContainer.hide();
    };

    jq('#statusListContainer li').on('click', function() {
        if (jq(this).is('.selected')) return;
        var milestoneId = jq('#statusListContainer').attr('milestoneId');
        var status = jq(this).attr('class');
        if (status == 'closed') {
            var text = jq.trim(jq('#' + milestoneId + ' td.activeTasksCount').text());
            if (text != '' && text != '0') {
                showQuestionWindow(milestoneId);
                return;
            }
        }
        milestoneaction.updateMilestone(milestoneId, { status: status });
    });

    var showQuestionWindow = function(milestoneId) {
        var proj = jq("tr#" + milestoneId + " td.title").find("a").attr("projectid");
        jq("#linkToTasks").attr("href", getMilestoneActiveTasksLink(proj, milestoneId));

        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq('#questionWindowTasks'),
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
        jq('#questionWindowTasks .grayLinkButton').on('click', function() {
            jq.unblockUI();
            return false;
        });
    };

    var showQuestionWindowMilestoneRemove = function(milestoneId) {
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq("#questionWindowDeleteMilestone"),
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
        jq("#questionWindowDeleteMilestone").attr("milestoneId", milestoneId);
    };

    jq('#milestonesList').on('mouseenter', 'tr .title a', function(event) {
        descriptionTimeout = setTimeout(function() {
            var targetObject = event.target;
            jq('#descriptionPanel .value div, #descriptionPanel .param div').hide();
            if (typeof jq(targetObject).attr('projectTitle') != 'undefined') {
                jq('#descriptionPanel .value .project a').html(htmlEncode(jq(targetObject).attr('projectTitle')));
                jq('#descriptionPanel .value .project').attr('projectId', jq(targetObject).attr('projectId'));
                jq('#descriptionPanel .project').show();
            }
            if (typeof jq(targetObject).attr('description') != 'undefined') {
                var description = jq(targetObject).attr('description');
                if (description != '') {
                    jq('#descriptionPanel .value .description').html(htmlEncode(description));
                    jq('#descriptionPanel .description').show();
                }
            }
            if (typeof jq(targetObject).attr('created') != 'undefined') {
                var date = jq(targetObject).attr('created');
                jq('#descriptionPanel .value .created').html(date);
                jq('#descriptionPanel .created').show();
            }
            if (typeof jq(targetObject).attr('createdBy') != 'undefined') {
                jq('#descriptionPanel .value .createdby').html(htmlEncode(jq(targetObject).attr('createdBy')));
                jq('#descriptionPanel .value .createdby').attr('createdById', jq(targetObject).attr('createdById'));
                jq('#descriptionPanel .createdby').show();
            }
            var listParams = jq('#descriptionPanel .param div');
            var listValues = jq('#descriptionPanel .value div');
            showDescriptionPanel(targetObject);
            overDescriptionPanel = true;
            for (var i = 0; i < listValues.length; i++) {
                var height = jq(listValues[i]).height();
                jq(listParams[i]).height(height);
            }
        }, 400, this);

        function htmlEncode(value) {
            return jq('<div/>').text(value).html().replace(/\n/ig, '<br/>');
        }
    });

    jq('#milestonesList').on('mouseleave', 'tr .title a', function() {
        clearTimeout(descriptionTimeout);
        overDescriptionPanel = false;
        hideDescriptionPanel();
    });

    var showDescriptionPanel = function(obj) {
        var x, y;
        jq('#descriptionPanel').show();

        x = jq(obj).offset().left;
        y = jq(obj).offset().top + 20;

        jq('#descriptionPanel .pm').show();

        if (typeof y == 'undefined')
            y = jq(obj).offset().top + 20;

        jq('#descriptionPanel').css({ left: x, top: y });
    };

    var hideDescriptionPanel = function() {
        setTimeout(function() {
            if (!overDescriptionPanel) {
                jq('#descriptionPanel').hide(100);
            }
        }, 200);
    };

    jq('#descriptionPanel').on('mouseenter', function() {
        overDescriptionPanel = true;
    });

    jq('#descriptionPanel').on('mouseleave', function() {
        overDescriptionPanel = false;
        hideDescriptionPanel();
    });

    jq('#descriptionPanel .value .project').on('click', function() {
        var projectId = jq(this).attr('projectId');
        var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project', projectId);
        path = jq.removeParam('tag', path);
        ASC.Controls.AnchorController.move(path);
        ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
        hideDescriptionPanel();
    });

    jq('#milestonesList').on('click', 'td.responsible span', function() {
        var responsibleId = jq(this).attr('responsibleId');
        var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'responsible_for_milestone', responsibleId);
        path = jq.removeParam('user_tasks', path);
        ASC.Controls.AnchorController.move(path);
        ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
    });

    jq('#milestonesList').on('click', 'td.actions .milestoneActionsLink', function() {
        var currentMilestone = selectedActionCombobox !== undefined ? selectedActionCombobox.attr('milestoneId') : -1;
        jq('#milestonesList tr#' + currentMilestone + ' td.actions .milestoneActionsLink').removeClass('selected');
        selectedActionCombobox = jq(this);

        if (selectedActionCombobox.attr('milestoneId') !== milestoneActionContainer.attr('milestoneId')) {
            milestoneActionContainer.attr('milestoneId', selectedActionCombobox.attr('milestoneId'));
            milestoneActionContainer.attr('projectId', selectedActionCombobox.attr('projectId'));
            showMilestoneActionContainer(selectedActionCombobox);
        }
        else {
            toggleMilestoneActionContainer(selectedActionCombobox);
        }
        // ga-track
        try {
            if (window._gat) {
                window._gaq.push(['_trackEvent', ga_Categories.milestones, ga_Actions.actionClick, "milestone-menu"]);
            }
        } catch (err) {
        }
        return false;
    });

    var showMilestoneActionContainer = function() {
        selectedActionCombobox.addClass('selected');

        var top = selectedActionCombobox.offset().top + selectedActionCombobox.innerHeight() - 3;
        var left = selectedActionCombobox.offset().left - milestoneActionContainer.innerWidth() + 26;

        milestoneActionContainer.css({ 'top': top, 'left': left });

        var currentStatus = selectedActionCombobox.attr('status');
        if (currentStatus == 'closed') {
            jq('#updateMilestoneButton').hide();
            jq('#addMilestoneTaskButton').hide();
        }
        else {
            jq('#updateMilestoneButton').show();
            jq('#addMilestoneTaskButton').show();
        }

        milestoneActionContainer.show();
    };

    var toggleMilestoneActionContainer = function() {
        if (milestoneActionContainer.is(':visible')) {
            selectedActionCombobox.removeClass('selected');
        } else {
            selectedActionCombobox.addClass('selected');
        }

        var currentStatus = selectedActionCombobox.attr('status');
        if (currentStatus == 'closed') {
            jq('#updateMilestoneButton').hide();
            jq('#addMilestoneTaskButton').hide();
        }
        else {
            jq('#updateMilestoneButton').show();
            jq('#addMilestoneTaskButton').show();
        }

        milestoneActionContainer.toggle();
    };

    var hideMilestoneActionContainer = function() {
        if (selectedActionCombobox) {
            selectedActionCombobox.removeClass('selected');
        }
        milestoneActionContainer.hide();
    };

    jq('#updateMilestoneButton').on('click', function() {
        var milestoneId = milestoneActionContainer.attr('milestoneId');
        milestoneActionContainer.hide();

        var milestoneRow = jq('#milestonesList tr#' + milestoneId);

        var milestone =
        {
            id: milestoneRow.attr('id'),
            project: milestoneRow.find('td.title a').attr('projectId'),
            responsible: milestoneRow.find('td.responsible span').attr('responsibleId'),
            deadline: milestoneRow.find('td.deadline span').text(),
            title: milestoneRow.find('td.title a').text(),
            description: milestoneRow.find('td.title a').attr('description'),
            isKey: milestoneRow.attr('isKey'),
            isNotify: milestoneRow.attr('isNotify')
        };

        milestoneaction.onGetMilestoneBeforeUpdate(milestone);
    });

    jq('#removeMilestoneButton').on('click', function() {
        var milestoneId = milestoneActionContainer.attr('milestoneId');
        milestoneActionContainer.hide();
        showQuestionWindowMilestoneRemove(milestoneId);
    });

    jq('#addMilestoneTaskButton').on('click', function() {
        var milestoneId = milestoneActionContainer.attr('milestoneId');
        var projectId = milestoneActionContainer.attr('projectId');

        milestoneActionContainer.hide();
        taskaction.clearAddTaskForm();

        if (currentProjectId) {
            serviceManager.getMilestonesByProject({ eventType: 'getMilestonesFF', milestoneId: milestoneId }, currentProjectId, taskaction.onGetMilestonesFF);
            serviceManager.getTeam('getTeamFF', { projectid: projectId });
        }
        else {
            jq('#taskProject').val(projectId).change();
            jq('#taskMilestone').attr('val', milestoneId);
        }
    });

    jq('#showNextMilestonesButton').on('click', function() {
        showNextMilestonesButton.hide();
        jq('#showNextMilestonesProcess').show();
        getMilestones(currentFilter, true);
        return false;
    });

    var onAddMilestone = function(params, milestone) {
        var milestoneTemplate = getMilestoneTemplate(milestone);

        var firstMilestone = jq('#milestonesList tbody tr:first');
        var addedMilestone = jq('#milestoneTemplate').tmpl(milestoneTemplate);

        if (firstMilestone.length == 0) {
            showAdvansedFilter();
            jq('#milestonesList tbody').append(addedMilestone);
            jq('.noContent').hide();
            showNewMilestoneButton();
            jq('#milestonesList').show();
            milestoneaction.unlockMilestoneActionPage();
            jq.unblockUI();
            return;
        }

        addedMilestone.insertBefore(firstMilestone);
        addedMilestone.yellowFade();
        milestoneaction.unlockMilestoneActionPage();
        jq.unblockUI();
    };

    var onAddMilestoneError = function() {
        milestoneaction.unlockMilestoneActionPage();
        jq.unblockUI();
    };

    var onUpdateMilestone = function(params, milestone) {
        var milestoneTemplate = getMilestoneTemplate(milestone);

        var updatedMilestone = jq('#milestonesList tr#' + milestone.id);
        var newMilestone = jq('#milestoneTemplate').tmpl(milestoneTemplate);

        updatedMilestone.replaceWith(newMilestone);
        newMilestone.yellowFade();
        milestoneaction.unlockMilestoneActionPage();
        jq.unblockUI();
    };

    var onUpdateMilestoneError = function() {
        milestoneaction.unlockMilestoneActionPage();
        jq.unblockUI();
    };

    var deleteMilestone = function(milestoneId) {
        var params = {};
        Teamlab.removePrjMilestone(params, milestoneId, { success: onDeleteMilestone });
    };

    var onDeleteMilestone = function(params, milestone) {
        var milestoneId = milestone.id;
        jq('#milestonesList tr#' + milestoneId).yellowFade();
        jq('#milestonesList tr#' + milestoneId).remove();
        if (jq('#milestonesList tbody tr:first').length == 0) {
            jq('#milestonesList').hide();
            clearTimeout(descriptionTimeout);
            jq('#descriptionPanel').hide();
            if (ASC.Controls.AnchorController.getAnchor() == basePath) {
                jq('#EmptyListMilestone').show();
                hideNewMilestoneButton();
                hideAdvansedFilter();
            } else {
                jq('#EmptyListForFilter').show();
            }
        }
    };

    var onGetMilestoneAfterAddTask = function(params, milestone) {
        var milestoneTemplate = milestones.getMilestoneTemplate(milestone);

        var updatedMilestone = jq('#milestonesList tr#' + milestoneTemplate.id);
        var newMilestone = jq('#milestoneTemplate').tmpl(milestoneTemplate);

        updatedMilestone.replaceWith(newMilestone);
        newMilestone.yellowFade();
    };

    return {
        init: init,
        getMyGuid: getMyGuid,
        getCurrentProjectId: getCurrentProjectId,
        setCurrentFilter: setCurrentFilter,
        getMilestones: getMilestones,
        onAddMilestone: onAddMilestone,
        onAddMilestoneError: onAddMilestoneError,
        onUpdateMilestone: onUpdateMilestone,
        onUpdateMilestoneError: onUpdateMilestoneError,
        onDeleteMilestone: onDeleteMilestone,
        onGetMilestoneAfterAddTask: onGetMilestoneAfterAddTask,
        getMilestoneTemplate: getMilestoneTemplate,
        showMilestoneActionPanel: showMilestoneActionPanel,
        showPreloader: showPreloader,
        hidePreloader: hidePreloader
    };
})(jQuery);

jq(function() {
    jQuery.fn.yellowFade = function() {
        this.css({ backgroundColor: "#ffffcc" });
        this.animate({ backgroundColor: "#ffffff" }, { queue: false, duration: 1000 });
        var resetStyle = function(self) { jq(self).removeAttr('style'); };
        setTimeout(resetStyle, 1100, this);
    };
});
