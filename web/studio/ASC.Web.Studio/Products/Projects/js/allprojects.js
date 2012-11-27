window.allProject = (function() {
    var isInit = false;
    var myGuid = null;
    var overProjDescrPanel = false;
    var projDescribeTimeout = 0;
    var basePath = 'sortBy=create_on&sortOrder=ascending';
    var linkViewProject = 'projects.aspx?prjID=';
    var linkViewMilestones = 'milestones.aspx?prjID=';
    var linkViewTasks = 'tasks.aspx?prjID=';
    var linkViewParticipants = 'projectTeam.aspx?prjID=';


    var initPresets = function() {
        //my projects
        var path = '#' + basePath + '&team_member=' + allProject.myGuid + '&status=open';
        jq('.presetContainer #preset_my').attr('href', path);
        //follow
        path = '#' + basePath + '&followed=true&status=open';
        jq('.presetContainer #preset_follow').attr('href', path);
        //open
        path = '#' + basePath + '&status=open';
        jq('.presetContainer #preset_open').attr('href', path);
    };
    //filter Set

    var onSetFilterProjects = function(evt, $container) {

        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            initPresets();
            ASC.Projects.ProjectsAdvansedFilter.presetAlign();
        }

        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor');
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
        serviceManager.StartIndex = 0;
        serviceManager.getProjectsByFilter('getProjectsByFilter', {}, ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data'));
        allProject.showPreloader();
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };


    // filter Reset
    var onResetFilterProjects = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor');
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        serviceManager.StartIndex = 0;
        serviceManager.getProjectsByFilter('getProjectsByFilter', {}, ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data'));
        allProject.showPreloader();
    };


    var init = function(guid) {
        if (isInit === false) {
            isInit = true;
        }
        allProject.myGuid = guid;
        showPreloader();

        jq('#emptyFilter').appendTo('.noProject');
        jq('.emptyScrCtrl').show();

        serviceManager.bind('getProjectsByFilter', onGetListProject);
        serviceManager.bind('getProjectsByFilterNext', onGetNextProjects);

        // filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(serviceManager.getMyGUID(), basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterProjects;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterProjects;

        if (typeof (tags) != 'undefined') {
            tags = jq.parseJSON(jQuery.base64.decode(tags)).response;
            onGetAllProjectsTags(tags);
        }

        jq('#tableListProjects td.responsible span.userLink').live('click', function() {
            var responsibleId = jq(this).attr('id');
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project_manager', responsibleId);
            path = jq.removeParam('team_member', path);
            ASC.Controls.AnchorController.move(path);
            ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
        });

        jq('#emptyFilter .emptyScrBttnPnl .baseLinkAction').click(function() {
            ASC.Controls.AnchorController.move(basePath);
            jq('#AdvansedFilter').advansedFilter(null);
            return false;
        });
        // popup handlers
        jq('#questionWindowMilestone .grayLinkButton, #questionWindowTasks .grayLinkButton').bind('click', function() {
            jq.unblockUI();
            return false;
        });
        // discribe panel
        jq("#tableListProjectsContainer").on("mouseenter", ".nameProject a", function(event) {
            projDescribeTimeout = setTimeout(function() {
            var targetObject = event.target;
                
                jq('#projectDescrPanel .descr .value .readMore').hide();
                jq('#projectDescrPanel .created, #projectDescrPanel .descr').hide();

                if (typeof jq(targetObject).attr('created') != 'undefined') {
                    if (jq(targetObject).attr('created').length) {
                        jq('#projectDescrPanel .created .value').html(jq(targetObject).attr('created'));
                        jq('#projectDescrPanel .created').show();
                    }
                }
                var description = jq(targetObject).siblings('.description').text();
                if (jq.trim(description) != '') {
                    jq('#projectDescrPanel .descr .value div').html(jq.linksParser(description.replace(/</ig, '&lt;').replace(/>/ig, '&gt;').replace(/\n/ig, '<br/>').replace('&amp;', '&')));
                    if (description.indexOf("\n") > 2 || description.length > 80) {
                        var link = "projects.aspx?prjID=" + jq(targetObject).attr('projectid');
                        jq('#projectDescrPanel .descr .value .readMore').attr("href", link);
                        jq('#projectDescrPanel .descr .value .readMore').show();
                    }
                    jq('#projectDescrPanel .descr').show();
                }
                showProjDescribePanel(targetObject);
                overProjDescrPanel = true;
            }, 500, this);
        });
        jq('#tableListProjectsContainer').on('mouseleave', '.nameProject a', function() {
            clearTimeout(projDescribeTimeout);
            overProjDescrPanel = false;
            hideDescrPanel();
        });

        jq('#projectDescrPanel').on('mouseenter', function() {
            overProjDescrPanel = true;
        });

        jq('#projectDescrPanel').on('mouseleave', function() {
            overProjDescrPanel = false;
            hideDescrPanel();
        });

        // ga-track-events

        //add
        jq(".addProject").trackEvent(ga_Categories.projects, ga_Actions.createNew, 'create-new-project');

        //presets
        jq('.presetContainer #preset_my').trackEvent(ga_Categories.projects, ga_Actions.presetClick, 'my-projects');
        jq('.presetContainer #preset_follow').trackEvent(ga_Categories.projects, ga_Actions.presetClick, 'followed');
        jq('.presetContainer #preset_open').trackEvent(ga_Categories.projects, ga_Actions.presetClick, 'open');

        //show next
        jq("#showNextProjects").trackEvent(ga_Categories.projects, ga_Actions.next, 'next-projects');

        //filter
        jq("#AdvansedFilter .advansed-filter-list li[data-id='me_team_member'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'me_team_member');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='team_member'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'team_member');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='me_project_manager'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'me_project_manager');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='project_manager'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'project_manager');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='open'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'open');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='closed'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'closed');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='paused'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'paused');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='followed'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'followed');
        jq("#AdvansedFilter .advansed-filter-list li[data-id='tag'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'tag');
        jq("#AdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'sort');

        jq("#AdvansedFilter .advansed-filter-input").keypress(function(e) {
            if (e.which == 13) {
                try {
                    if (window._gat) {
                        window._gaq.push(['_trackEvent', ga_Categories.projects, ga_Actions.filterClick, 'text']);
                    }
                } catch (err) {
                }
            }
            return true;
        });

        //change status
        jq("#statusList .open").trackEvent(ga_Categories.projects, ga_Actions.changeStatus, "open");
        jq("#statusList .closed").trackEvent(ga_Categories.projects, ga_Actions.changeStatus, "closed");
        jq("#statusList .paused").trackEvent(ga_Categories.projects, ga_Actions.changeStatus, "paused");

        //PM
        jq(".responsible .userLink").trackEvent(ga_Categories.projects, ga_Actions.userClick, "project-manager");

        //end ga-track-events
    };

    var hideDescrPanel = function() {
        setTimeout(function() {
            if (!overProjDescrPanel) jq('#projectDescrPanel').hide(100);
        }, 200);
    };

    var getProjTmpl = function(proj) {
        var projTmpl = {};
        projTmpl.title = proj.title;
        projTmpl.id = proj.id;
        projTmpl.created = Teamlab.getDisplayDate(Teamlab.serializeDate(proj.created));
        projTmpl.createdBy = proj.createdBy.displayName;
        projTmpl.projLink = linkViewProject + projTmpl.id;
        projTmpl.description = proj.description;
        projTmpl.milestones = proj.milestoneCount;
        projTmpl.linkMilest = linkViewMilestones + projTmpl.id + '#sortBy=deadline&sortOrder=ascending&status=open';
        projTmpl.tasks = proj.taskCount;
        projTmpl.linkTasks = linkViewTasks + projTmpl.id + '#sortBy=deadline&sortOrder=ascending&status=open';
        projTmpl.responsible = proj.responsible.displayName;
        projTmpl.responsibleId = proj.responsible.id;
        projTmpl.participants = proj.participantCount - 1;
        projTmpl.linkParticip = linkViewParticipants + projTmpl.id;
        projTmpl.privateProj = proj.isPrivate;
        projTmpl.canEdit = proj.canEdit;
        if (proj.status == 0) {
            projTmpl.status = 'open';
        }
        else {
            projTmpl.status = 'closed';
        }
        if (proj.status == 2) projTmpl.status = 'paused';

        return projTmpl;
    };

    var showPreloader = function() {
        LoadingBanner.displayLoading();
        jq('#showNextProjects').hide();
    };

    var hidePreloader = function() {
        LoadingBanner.hideLoading();
    };

    var showProjDescribePanel = function(targetObject) {
        var x = jq(targetObject).offset().left + 10;
        var y = jq(targetObject).offset().top + 20;
        jq('#projectDescrPanel').css({ left: x, top: y });
        jq('#projectDescrPanel').show();

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is('[id="#projectDescrPanel"]')) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('[id="#projectDescrPanel"]')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {
                jq('.actionPanel').hide();
            }
        });
    };

    var onGetListProject = function(data) {

        clearTimeout(projDescribeTimeout);
        overProjDescrPanel = false;
        hideDescrPanel();

        var json = jQuery.parseJSON(data);
        var response = json.response,
            listProj = response,
            listTmplProj = new Array(),
            projTmpl;
        if (listProj.length != 0) {
            for (var i = 0; i < listProj.length; i++) {
                projTmpl = getProjTmpl(listProj[i]);
                listTmplProj.push(projTmpl);
            }
            jq('#tableListProjects tbody').empty();
            jq('#projTmpl').tmpl(listTmplProj).appendTo('#tableListProjects tbody');
            jq('.noProject').hide();
            jq('#tableListProjects').show();
            hidePreloader();
            if (listProj.length < 31) {
                jq('#showNextProjects').hide();
            }
            else {
                jq('#showNextProjects').show();
                jq('.loaderProjects').hide();
            }
        }
        else {
            hidePreloader();
            jq('#tableListProjects').hide();
            jq('.noProject').show();
        }
    };

    var onGetNextProjects = function(data) {
        var json = jQuery.parseJSON(data);
        var response = json.response,
                listProj = response,
                listTmplProj = new Array(),
                projTmpl;
        if (listProj.length != 0) {
            for (var i = 0; i < listProj.length; i++) {
                projTmpl = getProjTmpl(listProj[i]);
                listTmplProj.push(projTmpl);
            }
            if (listProj.length < 31) {
                jq('#showNextProjects').hide();
            }
            else {
                jq('#showNextProjects').show();
            }
            jq('.loaderProjects').hide();
            jq('#projTmpl').tmpl(listTmplProj).appendTo('#tableListProjects tbody');
        }
        else {
            hidePreloader();
            jq('.loaderProjects').hide();
        }
    };

    var onGetAllProjectsTags = function(listTags) {
        var listObj = new Array();
        for (var i = 0; i < listTags.length; i++) {
            var obj, tag = listTags[i];
            obj = { value: tag.id, title: tag.title };
            listObj.push(obj);
        }
        createAdvansedFilter(listObj);
    };

    var changeStatus = function(item) {
        if (!jq(item).hasClass('current')) {
            var projId = jq(item).parents('#containerStatusList').attr('objid').split('_')[1];
            var newStatus = jq(item).attr('class');
            if (newStatus == 'closed') {
                var flag = showQuestionWindow(projId);
                if (flag) return;
            }
            var newtitle = jq(item).text();
            var data = { id: projId, status: newStatus };
            Teamlab.updatePrjProjectStatus({}, projId, data);

            changeCboxStatus(newStatus, projId, newtitle);
        }
    };

    var changeCboxStatus = function(status, projId, title) {
        jq('#statusCombobox_' + projId + ' span:first-child').attr('class', status);
        jq('#statusCombobox_' + projId + ' span:first-child').attr('title', title);
        if (status != 'open') {
            jq('tr#' + projId).addClass('noActiveProj');
        } else {
            jq('tr#' + projId).removeClass('noActiveProj');
        }
    };

    var showQuestionWindow = function(projId) {
        var proj = jq('tr#' + projId);
        var tasks = jq.trim(jq(proj).find('td.taskCount').text());
        var popupId;
        if (!tasks.length) {
            var milestones = jq.trim(jq(proj).find('td.milestoneCount').text());
            if (milestones.length) {
                popupId = '#questionWindowMilestone';
                var milUrl = linkViewMilestones + projId + '#sortBy=deadline&sortOrder=ascending&status=open';
                jq('#linkToMilestines').attr('href', milUrl);
            }
            else {
                return false;
            }
        } else {
            popupId = '#questionWindowTasks';
            var tasksUrl = linkViewTasks + projId + '#sortBy=deadline&sortOrder=ascending&status=open';
            jq('#linkToTasks').attr('href', tasksUrl);
        }

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
            fadeOut: 0
        });
        return true;
    };
    /*--------events--------*/

    jq('#showNextProjects').live('click', function() {
        jq('#showNextProjects').hide();
        jq('.loaderProjects').show();
        var listProj = jq('#tableListProjects tr');
        var lastId = jq(listProj[listProj.length - 1]).attr('id');
        lastId = parseInt(lastId);
        serviceManager.getProjectsByFilter('getProjectsByFilterNext', { mode: 'next' }, null, serviceManager.Count, lastId);
    });

    jq('body').click(function(event) {
        var elt = (event.target) ? event.target : event.srcElement;
        var isHide = true;
        var $elt = jq(elt);

        if ($elt.is('#containerStatusList')) isHide = false;
        if (isHide) {
            jq('#containerStatusList').hide();
            jq('.statusContainer').removeClass('openList');
        }
    });

    jq('td.action .canEdit').live('click', function(event) {
        showListStatus('containerStatusList', this);
        return false;
    });

    var showListStatus = function(panelId, obj, event) {
        var objid = '';
        var x, y;
        objid = jq(obj).attr('id');

        x = jq(obj).offset().left + 4;
        y = jq(obj).offset().top + 11;

        jq('#containerStatusList').attr('objid', objid);
        jq(obj).parents('tr').addClass('openList');

        jq('#containerStatusList').css({ left: x, top: y });

        var status = jq(obj).children('span').attr('class');
        jq('#containerStatusList #statusList li').show();
        jq('#containerStatusList #statusList li').removeClass('current');
        switch (status) {
            case 'closed':
                {
                    jq('#containerStatusList #statusList .closed').addClass('current');
                    jq('#containerStatusList #statusList .paused').hide();
                    break;
                }

            case 'paused':
                {
                    jq('#containerStatusList #statusList .paused').addClass('current');
                    break;
                }

            default:
                {
                    jq('#containerStatusList #statusList .open').addClass('current');
                    break;
                }
        }

        jq('#containerStatusList').show();

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is('[id="containerStatusList"]')) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('[id="containerStatusList"]')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {
                jq('#containerStatusList').hide();
                jq('#tableListProjects tr').removeClass('openList');
            }
        });
    };

    return {
        init: init,
        myGuid: myGuid,
        changeStatus: changeStatus,
        showPreloader: showPreloader,
        hidePreloader: hidePreloader
    };
})(jQuery);
