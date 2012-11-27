window.history2 = (function() {
    var isInit = false;

    var myGuid;

    var projectId;
    var currentFilter;

    var showNextActivitiesButton;
    var advansedFilter;
    var presetContainer;

    var basePath = "sortBy=id&sortOrder=descending";

    var currentActivitiesCount = 0;
    var lastId = 0;

    var descriptionTimeout;
    var overDescriptionPanel = false;

    var setCurrentFilter = function(filter) {
        currentFilter = filter;
    };

    var showAdvansedFilter = function() {
        advansedFilter.css("visibility", "visible");
        presetContainer.css("visibility", "visible");
    };

    var hideAdvansedFilter = function() {
        advansedFilter.css("visibility", "hidden");
        presetContainer.css("visibility", "hidden");
    };

    var initPresets = function() {

    };

    //filter Set
    var onSetFilterActivities = function(evt, $container) {
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            initPresets();
            ASC.Projects.ProjectsAdvansedFilter.presetAlign();
        }
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, "anchor", projectId);
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
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, "data", projectId);
        setCurrentFilter(filter);
        getActivities(filter);
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };

    //filter Reset
    var onResetFilterActivities = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, "anchor", projectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, "data", projectId);
        setCurrentFilter(filter);
        getActivities(filter);
    };

    var init = function(guid) {
        if (isInit === false) {
            isInit = true;
        }

        myGuid = guid;

        //filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(myGuid, basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterActivities;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterActivities;

        jq("#EmptyListForFilter").on("click", ".emptyScrBttnPnl .baseLinkAction", function() {
            ASC.Controls.AnchorController.move(basePath);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
            return false;
        });

        advansedFilter = jq("#AdvansedFilter");
        presetContainer = jq(".presetContainer");
        showNextActivitiesButton = jq("#showNextActivitiesButton");

        projectId = jq.getURLParam("prjID");
        var filterProjects = null;
        if (!projectId) {
            if (typeof (projects) != 'undefined') {
                projects = Teamlab.create('prj-projects', null, jq.parseJSON(jQuery.base64.decode(projects)).response);
                filterProjects = [];

                for (var i = 0; i < projects.length; i++) {
                    var project = projects[i];
                    var obj = { value: project.id, title: project.title };
                    filterProjects.push(obj);
                }
            }
        }

        createAdvansedFilter(filterProjects);

        var listEscs = jq(".noContentBlock");
        jq(listEscs[0]).appendTo("#EmptyListForFilter");
        jq(listEscs[1]).appendTo("#EmptyListActivities");
        jq(listEscs).show();

        jq('#activitiesList').on('click', 'td.entityType span', function() {
            var entity = jq(this).attr('data-entity');
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'entity', entity);
            ASC.Controls.AnchorController.move(path);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
        });

        jq('#activitiesList').on('click', 'td.user span', function() {
            var userId = jq(this).attr('data-userId');
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'user', userId);
            ASC.Controls.AnchorController.move(path);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
        });

        jq('#showNextActivitiesButton').on('click', function() {
            showNextActivitiesButton.hide();
            jq('#showNextActivitiesProcess').show();
            getActivities(currentFilter, true);
            return false;
        });

        if (projectId) return;

        jq('#activitiesList').on('mouseenter', 'tr .title a', function(event) {
            descriptionTimeout = setTimeout(function() {
                var targetObject = event.target;
                jq('#descriptionPanel .value div, #descriptionPanel .param div').hide();
                if (typeof jq(targetObject).attr('data-projectTitle') != 'undefined' && jq(targetObject).attr('data-projectTitle') != "") {
                    jq('#descriptionPanel .value .project a').html(htmlEncode(jq(targetObject).attr('data-projectTitle')));
                    jq('#descriptionPanel .value .project').attr('data-projectId', jq(targetObject).attr('data-projectId'));
                    jq('#descriptionPanel .project').show();

                    var listParams = jq('#descriptionPanel .param div');
                    var listValues = jq('#descriptionPanel .value div');
                    showDescriptionPanel(targetObject);
                    overDescriptionPanel = true;
                    for (var j = 0; j < listValues.length; j++) {
                        var height = jq(listValues[j]).height();
                        jq(listParams[j]).height(height);
                    }
                }
            }, 400, this);

            function htmlEncode(value) {
                return jq('<div/>').text(value).html().replace(/\n/ig, '<br/>');
            }
        });

        jq('#activitiesList').on('mouseleave', 'tr .title a', function() {
            clearTimeout(descriptionTimeout);
            overDescriptionPanel = false;
            hideDescriptionPanel();
        });

        jq('#descriptionPanel').on('mouseenter', function() {
            overDescriptionPanel = true;
        });

        jq('#descriptionPanel').on('mouseleave', function() {
            overDescriptionPanel = false;
            hideDescriptionPanel();
        });

        jq('#descriptionPanel .value .project').on('click', function() {
            var id = jq(this).attr('data-projectId');
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project', id);
            ASC.Controls.AnchorController.move(path);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
            hideDescriptionPanel();
        });
    };

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



    var getActivities = function(filter, showNext) {
        LoadingBanner.displayLoading();

        var params = {};
        var success = onGetNextActivities;
        if (!showNext) {
            currentActivitiesCount = 0;
            success = onGetActivities;
            lastId = 0;
        }

        filter.Count = 30;
        filter.StartIndex = currentActivitiesCount;
        filter.lastId = lastId;

        Teamlab.getPrjActivities(params, { filter: filter, success: success });
    };

    var onGetActivities = function(params, activities) {
        var activitiesCount = activities.length;
        currentActivitiesCount += activitiesCount;

        var tmplActivity, tmplActivities = new Array();
        if (activitiesCount) {
            showAdvansedFilter();

            for (var i = 0; i < activitiesCount; i++) {
                tmplActivity = getActivityTemplate(activities[i]);
                tmplActivities.push(tmplActivity);
            }
            jq("#activitiesList tbody").empty();
            jq("#activityTemplate").tmpl(tmplActivities).appendTo("#activitiesList tbody");
            jq("#activitiesListContainer .noContent").hide();

            if (activities.length == 30) {
                showNextActivitiesButton.show();
            } else {
                showNextActivitiesButton.hide();
            }

            jq("#activitiesList").show();
            lastId = activities[activitiesCount - 1].id;
        } else {
            jq("#activitiesList tbody").empty();
            jq("#activitiesList").hide();
            jq("#descriptionPanel").hide();
            showNextActivitiesButton.hide();
            if (ASC.Controls.AnchorController.getAnchor() == basePath) {
                jq("#EmptyListForFilter").hide();
                jq("#EmptyListActivities").show();
                hideAdvansedFilter();
                showNextActivitiesButton.hide();
            } else {
                jq("#EmptyListActivities").hide();
                showAdvansedFilter();
                jq("#EmptyListForFilter").show();
            }
            showNextActivitiesButton.hide();
        }
        LoadingBanner.hideLoading();
    };

    var onGetNextActivities = function(params, activities) {
        var activitiesCount = activities.length;
        currentActivitiesCount += activitiesCount;

        var tmplActivity, tmplActivities = new Array();
        if (activities.length != 0) {
            for (var i = 0; i < activitiesCount; i++) {
                tmplActivity = getActivityTemplate(activities[i]);
                tmplActivities.push(tmplActivity);
            }
            jq("#activityTemplate").tmpl(tmplActivities).insertAfter("#activitiesList tbody tr:last");
            jq("#showNextActivitiesProcess").hide();
            if (activitiesCount == 30) {
                showNextActivitiesButton.show();
            }
            lastId = activities[activitiesCount - 1].id;
        } else {
            jq("#showNextActivitiesProcess").hide();
        }
        LoadingBanner.hideLoading();
    };

    var getActivityTemplate = function(activity) {
        var template = {
            projectId: activity.projectId,
            projectTitle: activity.projectTitle,
            title: activity.title,
            url: activity.url,
            actionText: activity.actionText,
            date: activity.displayDatetime,
            userId: activity.user.id,
            userName: activity.user.displayName,
            entityType: activity.entityType,
            entityTitle: activity.entityTitle
        };

        return template;
    };

    return {
        init: init
    };
})(jQuery);