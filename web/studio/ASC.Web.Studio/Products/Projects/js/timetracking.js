ASC.Projects.TimeSpendActionPage = (function() {
    var projectsFilter = [];
    var basePath = 'sortBy=date&sortOrder=descending';
    var projId = null;
    var isTask = false;
    var dateFormat = function(times) {
        for (var i = 0; i < times.length; i++) {
            var t, d;
            if (times[i].date != null) {
                t = times[i].date.split(/[- : T]/);
                d = new Date(t[0], t[1] - 1, t[2]);
                t = d.toString().split(/[ ]/);
                times[i].dayMonth = t[2] + " " + t[1];
            }
        }
        return times;
    };

    var onSetFilterTime = function(evt, $container) {
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) ASC.Projects.ProjectsAdvansedFilter.presetAlign();

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
        serviceManager.getTimes('getTimes', { mode: 'onset' }, data);
        LoadingBanner.displayLoading();
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };

    var onResetFilterTime = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', serviceManager.projectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        serviceManager.StartIndex = 0;
        var data = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', serviceManager.projectId);
        serviceManager.getTimes('getTimes', { mode: 'onreset' }, data);
        LoadingBanner.displayLoading();
    };

    var initFilter = function() {
        //filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(serviceManager.getMyGUID(), basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterTime;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterTime;

        createAdvansedFilter();
    };

    var showEmptyScreen = function(isItems) {
        if (isTask) {
            if (isItems) {
                jq('#emptyScreen').hide();
                jq('.timesHeader').show();
                jq('#actionContainer').show();
            } else {
                jq('#emptyScreen').show();
                jq('.timesHeader').hide();
                jq('#actionContainer').hide();
            }
        } else {
            var oldRecord = jq('#timeSpendsList .timeSpendRecord');
            if (oldRecord.length) {
                if (!isItems) {
                    jq('.timesHeader').hide();
                    jq('#emptyListForFilter').show();
                    jq('#actionContainer').show();
                }
            } else {
                if (ASC.Controls.AnchorController.getAnchor() == basePath) {
                    if (!isItems) {
                        jq('.timesHeader').hide();
                        jq('#actionContainer').hide();
                        jq('#emptyScreen').show();
                    } else {
                        jq('#emptyListForFilter').hide();
                        jq('.timesHeader').show();
                    }
                } else {
                    if (!isItems) {
                        jq('#emptyListForFilter').show();
                        jq('.timesHeader').hide();
                    } else {
                        jq('#emptyListForFilter').hide();
                        jq('.timesHeader').show();
                    }
                }
            }
        }
    };

    var onGetTimes = function(data, params) {
        var json = jQuery.parseJSON(data);
        var times = json.response;
        times = dateFormat(times);
        showEmptyScreen(times.length);

        if (params.mode != 'next') {
            jq('#timeSpendsList').html('');
        }


        jq('#timeTrackingTemplate').tmpl(times).appendTo('#timeSpendsList');

        jq('#showNextProcess').hide();
        if (times.length > serviceManager.Count) {
            delete times[times.length - 1];
            jq('#showNext').show();
        } else {
            jq('#showNext').hide();
        }

        LoadingBanner.hideLoading();
    };

    var onUpdateTime = function(params, time) {
        time = dateFormat([time]);

        var currentRow = jq('#timeSpendsList .timeSpendRecord[timeid=' + time[0].id + ']');
        var totalHorsRow = jq('#TotalHoursCount');

        var totalHours = parseFloat(totalHorsRow.attr('hours').replace(',', '.')) - parseFloat(currentRow.attr('hours').replace(',', '.')) + time[0].hours;

        jq('#TotalHoursCount').attr('hours', totalHours);
        jq('#TotalHoursCount').text(jq.timeFormat(totalHours));
        
        currentRow.replaceWith(jq('#timeTrackingTemplate').tmpl(time));
    };

    var onRemoveTime = function(params, data) {
        jq("#timeSpendRecord" + params.timeid).animate({ opacity: "hide" }, 500);
        setTimeout(function() {
            jq("#timeSpendRecord" + params.timeid).remove();
            showEmptyScreen(jq("#timeSpendsList .timeSpendRecord").length);
        }, 500);
    };

    var onGetProjectsFilter = function(projects) {
        var projectsFilter = [];
        if (projects.length)
            for (var i = 0; i < projects.length; i++) {
            projectsFilter.push({ 'value': projects[i].id, 'title': projects[i].title });
        }

        filterOptions.projectsLoad(projectsFilter);
    };

    var onGetTags = function(tags) {
        var tagsFilter = [];
        if (tags.length)
            for (var i = 0; i < tags.length; i++) {
            tagsFilter.push({ 'value': tags[i], 'title': tags[i] });
        }

        filterOptions.tagsLoad(tagsFilter);
    };

    var onGetMilestones = function(params, milestones) {
        var milestonesFilter = [];
        milestonesFilter.push({ 'value': 0, 'title': 'Без вехи' });
        if (milestones.length)
            for (var i = 0; i < milestones.length; i++) {
            milestonesFilter.push({ 'value': milestones[i].id, 'title': milestones[i].displayDateDeadline + ' ' + milestones[i].title });
        }

        filterOptions.milestonesLoad(milestonesFilter);
    };

    var onGetTeamByProject = function(params, team) {
        if (params.eventType == 'getTeamForTimeSpendForm') {
            jq('#editLogPanel select[id$=editLogPanel_ddlPerson] option').remove();
            for (var i = 0; i < team.length; i++) {
                jq('#editLogPanel select[id$=editLogPanel_ddlPerson]').append('<option value="' + team[i].id + '">' + team[i].displayName + '</option>');
            }
            ASC.Projects.TimeSpendActionPage.activateEditTimeLogPanel(params.id);
        }
    };

    var showActionsPanel = function(panelId, obj) {
        var objid = '',
            objidAttr = '',
            y = 0;
        if (typeof jq(obj).attr('timeid') != 'undefined') {
            jq('#timeActionPanel .pm').attr('timeid', jq(obj).attr('timeid')).attr('prjid', jq(obj).attr('prjid')).attr('userid', jq(obj).attr('userid'));
        }
        if (panelId == 'timeActionPanel') objid = jq(obj).attr('timeid');
        if (objid.length) objidAttr = '[objid=' + objid + ']';
        if (jq('#' + panelId + ':visible' + objidAttr).length) {
            jq("body").unbind("click");
            jq('.actionPanel').hide();
        } else {
            jq('.actionPanel').hide();
            jq('#' + panelId).show();

            x = jq(obj).offset().left - 134;
            jq('#' + panelId).attr('objid', objid);
            y = jq(obj).offset().top + 21;
            jq('#' + panelId).css({ left: x, top: y });

            jq('body').click(function(event) {
                var elt = (event.target) ? event.target : event.srcElement;
                var isHide = true;
                if (jq(elt).is('[id="' + panelId + '"]') || (elt.id == obj.id && obj.id.length) || jq(elt).is('.menupoint')) {
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
                    jq('.menupoint').removeClass('show');
                }
            });

        }
    };

    return {
        initFilter: initFilter,
        onUpdateTime: onUpdateTime,
        deleteTimeSpend: function(id) {
            serviceManager.removeTime('removeTime', id);
        },
        showTimer: function(url) {
            var width = 287;
            var height = 600;

            if (jq.browser.webkit) {
                height = 598;
            }
            if (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1) {
                width = 287;
                height = 550;
            }
            if (jq.browser.msie) {
                height = 585;
            }
            var hWnd = null;
            var isExist = false;

            try {
                hWnd = window.open('', "displayTimerWindow", "width=" + width + ",height=" + height + ",resizable=yes");
            } catch (err) {
            }
            try {
                isExist = typeof hWnd.ASC === 'undefined' ? false : true;
            } catch (err) {
                isExist = true;
            }

            if (!isExist) {
                hWnd = window.open(url, "displayTimerWindow", "width=" + width + ",height=" + height + ",resizable=yes");
                isExist = true;
            }

            if (!isExist) {
                return undefined;
            }
            try {
                hWnd.focus();
            } catch (err) {
            }
        },
        taskDescriptionPage: false,

        init: function() {
            projId = jq.getURLParam("prjID");
            serviceManager.bind('getTimes', onGetTimes);
            serviceManager.bind('updateTime', onUpdateTime);
            //serviceManager.bind('removeTime', onRemoveTime);
            Teamlab.bind(Teamlab.events.removePrjTime, onRemoveTime);
            if (typeof timesForFirstRequest != 'undefined') {
                isTask = true;
                var json = jq.parseJSON(jQuery.base64.decode(timesForFirstRequest));
                var times = json.response;
                if (times.length) {
                    times = dateFormat(times);
                    jq('#timeTrackingTemplate').tmpl(times).appendTo('#timeSpendsList');
                }
                showEmptyScreen(times.length);
            } else {
                Teamlab.bind(Teamlab.events.getPrjTeam, onGetTeamByProject);
                /*				setTimeout(function() {
                if (serviceManager.projectId == null) {
                if (tags) {
                tags = Teamlab.create('prj-tags', null, jq.parseJSON(jQuery.base64.decode(tags)).response);
                onGetTags(tags);
                }
                if (filterProjects) {
                filterProjects = Teamlab.create('prj-projects', null, jq.parseJSON(jQuery.base64.decode(filterProjects)).response);
                onGetProjectsFilter(filterProjects);
                }
                }
                if (milestones) {
                milestones = Teamlab.create('prj-milestones', null, jq.parseJSON(jQuery.base64.decode(milestones)).response);
                onGetMilestones({ }, milestones);
                }
                }, 0);*/
            }

            jq('#timeSpendsList .menupoint').live('click', function() {
                jq('#timeSpendsList .menupoint').removeClass('show');
                if (jq('.actionPanel:visible').length) jq(this).removeClass('show');
                else jq(this).addClass('show');
                showActionsPanel('timeActionPanel', this);
                return false;
            });

            jq('#startTimer a, #emptyScreen .addFirstElement').click(function() {
                if (isTask) {
                    var taskId = jq.getURLParam("ID");
                    ASC.Projects.TimeSpendActionPage.showTimer('timetracking.aspx?prjID=' + projId + '&ID=' + taskId + '&action=timer');
                } else {
                    ASC.Projects.TimeSpendActionPage.showTimer('timetracking.aspx?prjID=' + projId + '&action=timer');
                }
            });

            jq('#timeActionPanel #ta_edit').live('click', function() {
                var id = jq(this).attr('timeid');
                var record = jq('.timeSpendRecord[timeid=' + id + ']');
                var taskId = jq(record).attr('taskid');
                var taskTitle = jq(record).find('.pm-ts-noteColumn').find('a').text();
                var recordNote = jq(record).find('.pm-ts-noteColumn').find('span').text();
                var date = jq(record).attr('date');
                var hours = jq.trim(jq(record).find('.pm-ts-hoursColumn').text());
                var responsible = jq(record).find('.pm-ts-personColumn').find('span').attr('value');
                jq('#timeActionPanel').hide();
                ASC.Projects.TimeTrakingEdit.showPopup(taskId, taskTitle, id, hours, date, recordNote, responsible);
            });

            jq('#timeActionPanel #ta_remove').live('click', function() {
                var id = jq(this).attr('timeid');
                jq('#timeActionPanel').hide();
                ASC.Projects.TimeSpendActionPage.deleteTimeSpend(id);
            });

            jq('#showNext').bind('click', function() {
                jq('#showNext').hide();
                jq('#showNextProcess').show();
                serviceManager.getTimes('getTimes', { mode: 'next' }, null, serviceManager.Count, jq('#timeSpendsList .timeSpendRecord:last').attr('timeid'));
            });

            jq('.pm-ts-personColumn span').live('click', function() {
                if (!isTask) {
                    var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'tasks_rasponsible', jq(this).attr('value'));
                    path = jq.removeParam('noresponsible', path);
                    ASC.Controls.AnchorController.move(path);
                    ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
                }
            });
            jq('#emptyListForFilter').on('click', '.noContentBlock .clearFilterButton', function() {
                jq('#AdvansedFilter').advansedFilter(null);
                return false;
            });

            //ga-track-events
            //show next
            jq("#showNext").trackEvent(ga_Categories.timeTrack, ga_Actions.next, 'next-times');

            //filter
            jq("#AdvansedFilter .advansed-filter-list li[data-id='me_tasks_rasponsible'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'me-tasks-responsible');
            jq("#AdvansedFilter .advansed-filter-list li[data-id='tasks_rasponsible'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'tasks-responsible');

            jq("#AdvansedFilter .advansed-filter-list li[data-id='group'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'group');

            jq("#AdvansedFilter .advansed-filter-list li[data-id='milestone'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'milestone');

            jq("#AdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'sort');

            jq("#AdvansedFilter .advansed-filter-input").keypress(function(e) {
                if (e.which == 13) {
                    try {
                        if (window._gat) {
                            window._gaq.push(['_trackEvent', ga_Categories.timeTrack, ga_Actions.filterClick, 'text']);
                        }
                    } catch (err) {
                    }
                }
                return true;
            });

            //responsible
            jq("div[id^=person_").trackEvent(ga_Categories.timeTrack, ga_Actions.userClick, "tasks-responsible");

            //end ga-track-events
        }
    };
})();

jq(function() {
  if (!(jq('#taskDescriptionTemplate').length || jq('#timerTime').length || jq('.taskList').length)) ASC.Projects.TimeSpendActionPage.init();  
});
