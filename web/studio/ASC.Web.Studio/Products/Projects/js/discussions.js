window.discussions = (function() {
    var isInit = false;

    var myGuid;
    var currentProjectId;

    var basePath = 'sortBy=create_on&sortOrder=descending';

    var newDiscussionButton;
    var showNewDiscussionButton = function() {
        newDiscussionButton.css('visibility', 'visible');
    };
    var hideNewDiscussionButton = function() {
        newDiscussionButton.css('visibility', 'hidden');
    };
    var updateNewDiscussionButton = function(prjId) {
    	if (prjId) {
    		newDiscussionButton.children('a').attr('href', 'messages.aspx?prjID=' + prjId + '&action=add');
    	} else {
    		newDiscussionButton.children('a').attr('href', 'messages.aspx?action=add');
    	}
    };

    var advansedFilter;
    var presetContainer;
    var showAdvansedFilter = function() {
        advansedFilter.css('visibility', 'visible');
        presetContainer.css('visibility', 'visible');
    };
    var hideAdvansedFilter = function() {
        advansedFilter.css('visibility', 'hidden');
        presetContainer.css('visibility', 'hidden');
    };

    var discussionsList;
    var showNextDiscussionsButton;

    var currentDiscussionsCount = 0;

    var currentFilter = {};

    var setCurrentFilter = function(filter) {
        currentFilter = filter;
    };

    jq('#EmptyListDiscussion .noContentBlock .baseLinkAction').live('click', function() {
        if (currentProjectId > 0) {
            window.location.replace('messages.aspx?prjID=' + currentProjectId + '&action=add');
        } else {
        	window.location.replace('messages.aspx?action=add');
        }
    });

    jq('#showNextDiscussionsButton').live('click', function() {
        getDiscussions(currentFilter, true);
    });

    var showPreloader = function() {
        LoadingBanner.displayLoading();
    };

    var hidePreloader = function() {
        LoadingBanner.hideLoading();
    };
    var initPresets = function() {
        //my milestones
        var path = '#' + basePath + "&author=" + myGuid;
        jq('.presetContainer #preset_my').attr('href', path);
        //follow
        path = '#' + basePath + "&followed=true";
        jq('.presetContainer #preset_follow').attr('href', path);
        //latest
        var date = new Date();
        var createdStop = date.getTime();
        date.setDate(date.getDate() - 7);
        var createdStart = date.getTime();
        path = '#' + basePath + "&createdStart=" + createdStart + "&createdStop=" + createdStop;
        jq('.presetContainer #preset_latest').attr('href', path);
    };
    //filter Set
    var onSetFilterDiscussions = function(evt, $container) {
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
        discussions.setCurrentFilter(filter);
        discussions.getDiscussions(filter);
        discussions.showPreloader();
        if (path !== hash) {
        	ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
    		location.hash = path;
    	}
    };

    //filter Reset
    var onResetFilterDiscussions = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        discussions.setCurrentFilter(filter);
        discussions.getDiscussions(filter);
        discussions.showPreloader();
    };


    var init = function(guid) {
        myGuid = guid;

        if (isInit === false) {
            isInit = true;
        }

        ASC.Projects.ProjectsAdvansedFilter.initialisation(myGuid, basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterDiscussions;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterDiscussions;

        jq("#EmptyListForFilter .emptyScrBttnPnl .baseLinkAction").live("click", function() {
            jq('#AdvansedFilter').advansedFilter(null);
            return false;
        });

        newDiscussionButton = jq('#newDiscussionButton');
        advansedFilter = jq('#AdvansedFilter');
        presetContainer = jq('.presetContainer');
        discussionsList = jq('#discussionsList');
        showNextDiscussionsButton = jq('#showNextDiscussionsButton');

        currentProjectId = jq.getURLParam('prjID');

        if (!currentProjectId) {
            if (typeof (tags) == 'undefined' || typeof (projects) == 'undefined') return;

            tags = jq.parseJSON(jQuery.base64.decode(tags)).response;
            projects = Teamlab.create('prj-projects', null, jq.parseJSON(jQuery.base64.decode(projects)).response);

            var filterTags = [];
            var filterProjects = [];

            for (var j = 0; j < tags.length; j++) {
                filterTags.push({ value: tags[j].id, title: tags[j].title });
            }

            for (var k = 0; k < projects.length; k++) {
                var obj = { value: projects[k].id, title: projects[k].title };
                filterProjects.push(obj);
            }

            createAdvansedFilter(filterTags, filterProjects);
        } else {
            createAdvansedFilter();
        }

        var listEscs = jq('.noContentBlock');
        jq(listEscs[0]).appendTo('#EmptyListForFilter');
        jq(listEscs[1]).appendTo('#EmptyListDiscussion');
        jq(listEscs).show();
        if (newDiscussionButton.length == 0) {
            jq('#EmptyListDiscussion .noContentBlock .emptyScrBttnPnl').remove();
        }

        // ga-track-events

        //add
        jq("#newDiscussionButton").trackEvent(ga_Categories.discussions, ga_Actions.createNew, 'create-new-discussion');

        //presets
        jq('.presetContainer #preset_my').trackEvent(ga_Categories.discussions, ga_Actions.presetClick, 'my-discussions');
        jq('.presetContainer #preset_follow').trackEvent(ga_Categories.discussions, ga_Actions.presetClick, 'followed');
        jq('.presetContainer #preset_latest').trackEvent(ga_Categories.discussions, ga_Actions.presetClick, 'latest');

        //show next
        jq("#showNextDiscussionsButton").trackEvent(ga_Categories.discussions, ga_Actions.next, 'next-discussions');

        //filter
        jq("#AdvansedFilter .advansed-filter-list li[data-id='me_author'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'me-author');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='author'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'author');
        
        jq("#AdvansedFilter .advansed-filter-list li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'myprojects');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='project'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'project');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='tag'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'tag');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='followed'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'followed');

        jq("#AdvansedFilter .advansed-filter-list li[data-id='today2'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'today');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='recent'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'recent-7-days');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='created'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'user-period');

        jq("#AdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'sort');

        jq("#AdvansedFilter .advansed-filter-input").keypress(function(e) {
            if (e.which == 13) {
                try {
                    if (window._gat) {
                        window._gaq.push(['_trackEvent', ga_Categories.discussions, ga_Actions.filterClick, 'text']);
                    }
                } catch (err) {
                }
            }
            return true;
        });
        //end ga-track-events
    };

    var getDiscussions = function(filter, showNext) {
        var params = {};
        if (!showNext) {
            currentDiscussionsCount = 0;
        } else {
            params.showNext = true;
        }
        filter.Count = 10;
        filter.StartIndex = currentDiscussionsCount;

        Teamlab.getPrjDiscussions(params, { filter: filter, success: onGetDiscussions });
    };

    var onGetDiscussions = function(params, discussions) {       
        if (discussions.length) {
            renderNewDiscussionButton();
            if (!params.showNext) {
                discussionsList.empty();
            }
            showAdvansedFilter();
            showDiscussions(discussions);
        } else {
        	showNextDiscussionsButton.hide();
        	if (params.showNext) {
        		return;
        	}
            discussionsList.empty();
            if (ASC.Controls.AnchorController.getAnchor() == basePath) {
            	hideNewDiscussionButton();
                jq('#EmptyListForFilter').hide();
                jq('#EmptyListDiscussion').show();
                hideAdvansedFilter();
            } else {
            	renderNewDiscussionButton();
                jq('#EmptyListDiscussion').hide();
                showAdvansedFilter();
                jq('#EmptyListForFilter').show();
            }
            hidePreloader();
        }
    };

    var renderNewDiscussionButton = function() {
        var project = parseInt(jq.getAnchorParam("project", ASC.Controls.AnchorController.getAnchor()));
        if (currentProjectId) {
            updateNewDiscussionButton(currentProjectId);
        }
        else if (project) {
            updateNewDiscussionButton(project);
        } else {
        	updateNewDiscussionButton();
        }
    	showNewDiscussionButton();
    };

    var showDiscussions = function(discussions) {
        var templates = [];
        for (var i = 0; i < discussions.length; i++) {
            var discussion = discussions[i];
            var template = getDiscussionTemplate(discussion);
            templates.push(template);
        }

        jq('#discussionTemplate').tmpl(templates).appendTo(discussionsList);
        //discussionsList.children('.discussionContainer:even').addClass('odd');
        discussionsList.show();

        currentDiscussionsCount += templates.length;

        if (templates.length == 10) {
            showNextDiscussionsButton.show();
        } else {
            showNextDiscussionsButton.hide();
        }
        jq('#EmptyListForFilter').hide();
        jq('#EmptyListDiscussion').hide();

        hidePreloader();
    };

    var getDiscussionTemplate = function(discussion) {
        var discussionId = discussion.id;
        var prjId = discussion.projectId;

        var template =
        {
            createdDate: discussion.displayCrtdate,
            title: discussion.title,
            discussionUrl: getDiscussionUrl(prjId, discussionId),
            authorAvatar: discussion.createdBy.avatar,
            authorName: discussion.createdBy.displayName,
            authorPost: discussion.createdBy.title,
            text: discussion.text,
            commentsCount: discussion.commentsCount,
            commentsUrl: getCommentsUrl(prjId, discussionId)
        };
        if (/*projectId <= 0*/true) {
            template.projectTitle = discussion.projectTitle;
            template.projectUrl = getProjectUrl(discussion.projectId);
        }
        return template;
    };

    var getDiscussionUrl = function(prjId, discussionId) {
        return 'messages.aspx?prjID=' + prjId + '&id=' + discussionId;
    };

    var getProjectUrl = function(prjId) {
        return 'projects.aspx?prjID=' + prjId;
    };

    var getCommentsUrl = function(prjId, discussionId) {
        return 'messages.aspx?prjID=' + prjId + '&id=' + discussionId + '#comments';
    };

    return {
        init: init,
        setCurrentFilter: setCurrentFilter,
        getDiscussions: getDiscussions,
        showPreloader: showPreloader,
        hidePreloader: hidePreloader
    };
})();
