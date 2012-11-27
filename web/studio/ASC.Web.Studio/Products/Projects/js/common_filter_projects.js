ASC.Projects.ProjectsAdvansedFilter = (function() {
    var myGuid,
        anchorMoving = false,
        init = false,
        firstload = true,
        hashFilterChanged = true;
    var basePath = "";
	var baseSortBy = "";
    var massNameFilters = {

        team_member: "team_member",
        me_team_member: "me_team_member",

        me_responsible_for_milestone: "me_responsible_for_milestone",
        responsible_for_milestone: "responsible_for_milestone",

        me_project_manager: "me_project_manager",
        project_manager: "project_manager",

        me_author: "me_author",
        author: "author",

        user: "user",
        me_user: "me_user",
        
        me_tasks_rasponsible: "me_tasks_rasponsible",
        tasks_rasponsible: "tasks_rasponsible",

        me_tasks: "me_tasks",
        user_tasks: "user_tasks",

        noresponsible: "noresponsible",

        group: "group",
        followed: "followed",
        tag: "tag",
        text: "text",

        project: "project",
        myprojects: "myprojects",

        milestone: "milestone",
        mymilestones: "mymilestones",

        status: "status",
        open: "open",
        closed: "closed",
        paused: "paused",

        overdue: "overdue",
        today: "today",
        upcoming: "upcoming",
        recent: "recent",
        deadlineStart: "deadlineStart",
        deadlineStop: "deadlineStop",
        createdStart: "createdStart",
        createdStop: "createdStop",
        periodStart: "periodStart",
        periodStop: "periodStop",
        
        entity: "entity",
        project_entity: "project_entity",
        milestone_entity: "milestone_entity",
        discussion_entity: "discussion_entity",
        team_entity: "team_entity",
        task_entity: "task_entity",
        subtask_entity: "subtask_entity",
        time_entity: "time_entity",
        comment_entity: "comment_entity",

        sortBy: "sortBy",
        sortOrder: "sortOrder"
    };
    var massFilters = [
        "team_member",
        "project_manager",
        "responsible_for_milestone",
        "tasks_rasponsible",
        "author",
    	"user",

        "noresponsible",

        "user_tasks",
        "group",
        "followed",
        "tag",
        "text",

        "project",
        "myprojects",

        "mymilestones",
        "milestone",

        "status",

        "overdue",
        "today",
        "upcoming",
        "recent",
        "deadlineStart",
        "deadlineStop",
        "createdStart",
        "createdStop",
        "periodStart",
        "periodStop",
    	
    	"entity",

        "sortBy",
        "sortOrder"];
    
    var presetAlign = function() {
        var margin = jq(".advansed-filter-container").css("marginLeft");
        jq(".presetContainer").css("marginLeft", margin);
        jq(".presetContainer").show();
    };
    
    var onMovedHash = function() {
        if (!ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged) {
            setFilterByUrl();
        }else {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = false;
        }
    };

    var initialisation = function(userGuid, bsPath) {
        myGuid = userGuid;
        basePath = bsPath;
    	var res = /sortBy=(.+)\&sortOrder=(.+)/ig .exec(basePath);
    	if (res && res.length == 3) {
    		baseSortBy = res[1];
    	}
        ASC.Controls.AnchorController.bind(/^(.+)*$/, onMovedHash);
    };

    var getUrlParam = function(name, str) {
        var regexS = "[#&]" + name + "=([^&]*)";
        var regex = new RegExp(regexS);
        var tmpUrl = "#";
        if (str) {
            tmpUrl += str;
        } else {
            tmpUrl += ASC.Controls.AnchorController.getAnchor();
        }
        var results = regex.exec(tmpUrl);
        if (results == null)
            return "";
        else
            return results[1];
    };

    var coincidesWithFilter = function(filter) {
        var hash = ASC.Controls.AnchorController.getAnchor();

        var sortOrder = getUrlParam(massNameFilters.sortOrder, hash);
        var sortBy = getUrlParam(massNameFilters.sortBy, hash);

        if (sortBy == "" && sortOrder == "") {
            hash = basePath + "&" + hash;
        }

        for (var i = 0; i < massFilters.length; i++) {
            var paramName = massFilters[i];
            var filterParam = getUrlParam(paramName, filter);
            var hashParam = getUrlParam(paramName, hash);
            if (filterParam != hashParam) {
                return false;
            }
        }
        return true;
    };

    var setFilterByUrl = function() {
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            ASC.Projects.ProjectsAdvansedFilter.firstload = false;
        }

        var hash = ASC.Controls.AnchorController.getAnchor();
        if (hash == "") {
            location.hash = basePath;
            return;
        }
        var team_member = getUrlParam(massNameFilters.team_member),
            project_manager = getUrlParam(massNameFilters.project_manager),
            responsible_for_milestone = getUrlParam(massNameFilters.responsible_for_milestone),
            tasks_rasponsible = getUrlParam(massNameFilters.tasks_rasponsible),
            author = getUrlParam(massNameFilters.author),
            user = getUrlParam(massNameFilters.user),

            user_tasks = getUrlParam(massNameFilters.user_tasks),
            noresponsible = getUrlParam(massNameFilters.noresponsible),

            group = getUrlParam(massNameFilters.group),

            followed = getUrlParam(massNameFilters.followed),
            tag = getUrlParam(massNameFilters.tag),
            text = decodeURIComponent(getUrlParam(massNameFilters.text)),

            project = getUrlParam(massNameFilters.project),
            myprojects = getUrlParam(massNameFilters.myprojects),

            mymilestones = getUrlParam(massNameFilters.mymilestones),
            milestone = getUrlParam(massNameFilters.milestone),

            status = getUrlParam(massNameFilters.status),

            overdue = getUrlParam(massNameFilters.overdue),
            deadlineStart = getUrlParam(massNameFilters.deadlineStart),
            deadlineStop = getUrlParam(massNameFilters.deadlineStop),
            createdStart = getUrlParam(massNameFilters.createdStart),
            createdStop = getUrlParam(massNameFilters.createdStop),
            periodStart = getUrlParam(massNameFilters.periodStart),
            periodStop = getUrlParam(massNameFilters.periodStop),
            
            entity = getUrlParam(massNameFilters.entity),

            sortBy = getUrlParam(massNameFilters.sortBy),
            sortOrder = getUrlParam(massNameFilters.sortOrder);

        filters = [];
        sorters = [];

        // Responsible
        if (team_member.length > 0) {
            filters.push({ type: "person", id: massNameFilters.team_member, isset: true, params: { id: team_member} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_team_member, reset: true });
            filters.push({ type: "person", id: massNameFilters.team_member, reset: true });
        }
        if (project_manager.length > 0) {
            filters.push({ type: "person", id: massNameFilters.project_manager, isset: true, params: { id: project_manager} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_project_manager, reset: true });
            filters.push({ type: "person", id: massNameFilters.project_manager, reset: true });
        }
        if (tasks_rasponsible.length > 0) {
            filters.push({ type: "person", id: massNameFilters.tasks_rasponsible, isset: true, params: { id: tasks_rasponsible} });
        } else {
            filters.push({ type: "person", id: massNameFilters.tasks_rasponsible, reset: true });
            filters.push({ type: "person", id: massNameFilters.me_tasks_rasponsible, reset: true });
        }
        if (responsible_for_milestone.length > 0) {
            filters.push({ type: "person", id: massNameFilters.responsible_for_milestone, isset: true, params: { id: responsible_for_milestone} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_responsible_for_milestone, reset: true });
            filters.push({ type: "person", id: massNameFilters.responsible_for_milestone, reset: true });
        }
        if (author.length > 0) {
            filters.push({ type: "person", id: massNameFilters.author, isset: true, params: { id: author} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_author, reset: true });
            filters.push({ type: "person", id: massNameFilters.author, reset: true });
        }
    	if (user.length > 0) {
            filters.push({ type: "person", id: massNameFilters.user, isset: true, params: { id: user} });
        } else {
            filters.push({ type: "person", id: massNameFilters.user, reset: true });
            filters.push({ type: "person", id: massNameFilters.me_user, reset: true });
        }

        if (noresponsible.length > 0) {
            filters.push({ type: "flag", id: "noresponsible", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "noresponsible", reset: true });
        }

        if (group.length > 0) {
            filters.push({ type: "group", id: massNameFilters.group, isset: true, params: { id: group} });
        } else {
            filters.push({ type: "group", id: massNameFilters.group, reset: true });
        }

        //Tasks
        if (user_tasks.length > 0) {
            filters.push({ type: "person", id: massNameFilters.user_tasks, isset: true, params: { id: user_tasks} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_tasks, reset: true });
            filters.push({ type: "person", id: massNameFilters.user_tasks, reset: true });
        }
    	
        // Milestone
        if (mymilestones.length > 0) {
            filters.push({ type: "flag", id: "mymilestones", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "mymilestones", reset: true });
        }
        if (milestone.length > 0) {
            filters.push({ type: "combobox", id: "milestone", params: { value: milestone} });
        } else {
            filters.push({ type: "combobox", id: "milestone", reset: true });
        }
    	
        // Project
        if (project.length > 0) {
            filters.push({ type: "combobox", id: "project", isset: true, params: { value: project} });
        } else {
            filters.push({ type: "combobox", id: "project", reset: true });
        }
        if (myprojects.length > 0) {
            filters.push({ type: "flag", id: "myprojects", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "myprojects", reset: true });
        }
    	
        // Tag
        if (tag.length > 0) {
            filters.push({ type: "combobox", id: "tag", isset: true, params: { value: tag} });
        } else {
            filters.push({ type: "combobox", id: "tag", reset: true });
        }
    	
        // Status
        if (status.length > 0) {
            filters.push({ type: "combobox", id: status, isset: true, params: {value: status} });
        } else {
            filters.push({ type: "combobox", id: "open", reset: true });
            filters.push({ type: "combobox", id: "paused", reset: true });
            filters.push({ type: "combobox", id: "closed", reset: true });
        }

        // due date
        if (overdue.length > 0) {
            filters.push({ type: "flag", id: "overdue", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "overdue", reset: true });
        }
        if (deadlineStart.length > 0 && deadlineStop.length > 0) {
            filters.push({ type: "daterange", id: "deadline", isset: true, params: { from: deadlineStart, to: deadlineStop} });
        } else {
        	filters.push({ type: "daterange", id: "today", reset: true });
        	filters.push({ type: "daterange", id: "upcoming", reset: true });
            filters.push({ type: "daterange", id: "deadline", reset: true });
        }
        if (createdStart.length > 0 && createdStop.length > 0) {
            filters.push({ type: "daterange", id: "created", isset: true, params: { from: createdStart, to: createdStop} });
        } else {
            filters.push({ type: "daterange", id: "today2", reset: true });
            filters.push({ type: "daterange", id: "recent", reset: true });
            filters.push({ type: "daterange", id: "created", reset: true });
        }
    	if (periodStart.length > 0 && periodStop.length > 0) {
            filters.push({ type: "daterange", id: "period", isset: true, params: { from: periodStart, to: periodStop} });
        } else {
            filters.push({ type: "daterange", id: "today3", reset: true });
            filters.push({ type: "daterange", id: "yesterday", reset: true });
            filters.push({ type: "daterange", id: "currentweek", reset: true });
            filters.push({ type: "daterange", id: "previousweek", reset: true });
            filters.push({ type: "daterange", id: "currentmonth", reset: true });
            filters.push({ type: "daterange", id: "previousmonth", reset: true });
            filters.push({ type: "daterange", id: "currentyear", reset: true });
            filters.push({ type: "daterange", id: "previousyear", reset: true });
            filters.push({ type: "daterange", id: "period", reset: true });
        }
    	
        // Text
        if (text.length > 0) {
            filters.push({ type: "text", id: "text", isset: true, params: { value: text} });
        } else {
            filters.push({ type: "text", id: "text", reset: true, params: { value: null} });
        }
    	
        // Other
        if (followed.length > 0) {
            filters.push({ type: "flag", id: "followed", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "followed", reset: true });
        }
    	
    	// Entity
        if (entity.length > 0) {
            filters.push({ type: "combobox", id: entity.toLowerCase() + "_entity", isset: true, params: {value: entity.toLowerCase()} });
        } else {
            filters.push({ type: "combobox", id: "project_entity", reset: true });
            filters.push({ type: "combobox", id: "milestone_entity", reset: true });
            filters.push({ type: "combobox", id: "discussion_entity", reset: true });
            filters.push({ type: "combobox", id: "team_entity", reset: true });
            filters.push({ type: "combobox", id: "task_entity", reset: true });
            filters.push({ type: "combobox", id: "subtask_entity", reset: true });
            filters.push({ type: "combobox", id: "comment_entity", reset: true });
            filters.push({ type: "combobox", id: "timespend_entity", reset: true });
        }

        // Sorters   	
    	if (sortBy.length > 0 && sortOrder.length > 0) {
            sorters.push({ type: "sorter", id: sortBy, selected: true, sortOrder: sortOrder });
        } else if (sortBy.length > 0) {
            sorters.push({ type: "sorter", id: sortBy, selected: true, sortOrder: "descending" });
        } else if (sortOrder.length > 0) {
        	sortBy = baseSortBy;
            sorters.push({ type: "sorter", id: sortBy, selected: true, sortOrder: sortOrder });
        }
    	
        jq("#AdvansedFilter").advansedFilter({ filters: filters, sorters: sorters });
    };

    var makeData = function($container, type, prjId) {
        var data = {}, anchor = "", filters = $container.advansedFilter();
        var projectId = prjId;
        if (projectId) {
            data.projectId = projectId;
        }
        for (var filterInd = 0; filterInd < filters.length; filterInd++) {
            switch (filters[filterInd].id) {
                case "me_team_member":
                case "team_member":
                    data.participant = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "team_member", data.participant);
                    break;
                case "me_project_manager":
                case "project_manager":
                    data.manager = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "project_manager", data.manager);
                    break;
                case "me_responsible_for_milestone":
                case "responsible_for_milestone":
                    data.milestoneResponsible = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "responsible_for_milestone", data.milestoneResponsible);
                    break;
                case "me_tasks_rasponsible":
                case "tasks_rasponsible":
                    data.participant = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "tasks_rasponsible", data.participant);
                    break;
                case "me_author":
                case "author":
                    data.participant = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "author", data.participant);
                    break;
                case "me_user":
                case "user":
                    data.user = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "user", data.user);
                    break;
                case "group":
                    data.departament = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "group", data.departament);
                    break;
                case "mymilestones":
                    data.mymilestones = "true";
                    anchor = jq.changeParamValue(anchor, "mymilestones", "true");
                    break;
                case "milestone":
                    data.milestone = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "milestone", data.milestone);
                    break;
                case "noresponsible":
                    data.participant = "00000000-0000-0000-0000-000000000000";
                    anchor = jq.changeParamValue(anchor, "noresponsible", "true");
                    break;
                case "myprojects":
                    data.myprojects = "true";
                    anchor = jq.changeParamValue(anchor, "myprojects", "true");
                    break;
                case "project":
                    data.projectId = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "project", data.projectId);
                    break;
                case "me_tasks":
                case "user_tasks":
                    data.taskResponsible = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "user_tasks", data.taskResponsible);
                    break;
                case "followed":
                    data.follow = "true";
                    anchor = jq.changeParamValue(anchor, "followed", "true");
                    break;
                case "tag":
                    data.tag = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "tag", data.tag);
                    break;
                case "open":
                case "paused":
                case "closed":
                case "status":
                    data.status = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "status", data.status);
                    break;
                case "overdue":
                    data.status = "open";
                    data.deadlineStop = Teamlab.serializeTimestamp(new Date());
                    anchor = jq.changeParamValue(anchor, "overdue", "true");
                    break;
                case "today":
                case "upcoming":
                case "deadline":
                    data.deadlineStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from));
                    data.deadlineStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
                    anchor = jq.changeParamValue(anchor, "deadlineStart", filters[filterInd].params.from);
                    anchor = jq.changeParamValue(anchor, "deadlineStop", filters[filterInd].params.to);
                    break;
                case "today2":
                case "recent":
                case "created":
                    data.createdStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from));
                    data.createdStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
                    anchor = jq.changeParamValue(anchor, "createdStart", filters[filterInd].params.from);
                    anchor = jq.changeParamValue(anchor, "createdStop", filters[filterInd].params.to);
                    break;
                case "today3":
                case "yesterday":
                case "currentweek":
                case "previousweek":
                case "currentmonth":
                case "previousmonth":
                case "currentyear":
                case "previousyear":
                case "period":
                    data.periodStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from));
                    data.periodStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
                    anchor = jq.changeParamValue(anchor, "periodStart", filters[filterInd].params.from);
                    anchor = jq.changeParamValue(anchor, "periodStop", filters[filterInd].params.to);
                    break;
                case "text":
                	if (type == "anchor") {
                		data.FilterValue = encodeURIComponent(filters[filterInd].params.value);
                	} else {
                		data.FilterValue = filters[filterInd].params.value;
                	}
                	anchor = jq.changeParamValue(anchor, "text", data.FilterValue);
                    break;
                case "project_entity":
                case "milestone_entity":
                case "discussion_entity":
                case "team_entity":
                case "task_entity":
                case "subtask_entity":
                case "timespend_entity":
                case "comment_entity":
                case "entity":
                    data.entity = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "entity", data.entity);
                    break;
                case "sorter":
                    data.sortBy = filters[filterInd].params.id;
                    data.sortOrder = filters[filterInd].params.sortOrder;
                    anchor = jq.changeParamValue(anchor, "sortBy", data.sortBy); ;
                    anchor = jq.changeParamValue(anchor, "sortOrder", data.sortOrder);
                    break;
            }
        }
        if (type == "anchor") {
            return anchor;
        } else {
            return data;
        }
    };
    return {
        getUrlParam: getUrlParam,
        basePath: basePath,
        hashFilterChanged: hashFilterChanged,
        massNameFilters: massNameFilters,
        anchorMoving: anchorMoving,
        initialisation: initialisation,
        presetAlign: presetAlign,
        firstload: firstload,
        coincidesWithFilter: coincidesWithFilter,
        init: init,
        setFilterByUrl: setFilterByUrl,
        makeData: makeData,
        onSetFilter: function() {
        },
        onResetFilter: function() {
        }
    };
})(jQuery);

window.filterOptions = (function() {
    var milestoneStatus = function(status) {
        jq("#AdvansedFilter").advansedFilter({ filters: [{ type: "combobox", id: "milestone", enable: status}] });
    };

    var filterVisible = function(type, id, visible) {
        jq("#AdvansedFilter").advansedFilter({ filters: [{ type: type, id: id, visible: visible}] });
    };

    var milestonesLoad = function(milestones) {
        jq("#AdvansedFilter").advansedFilter({ filters: [{ type: "combobox", id: "milestone", options: milestones}] });
    };

    var projectsLoad = function(projects) {
        jq("#AdvansedFilter").advansedFilter({ filters: [{ type: "combobox", id: "project", options: projects}] });
    };

    var tagsLoad = function(tags) {
        jq("#AdvansedFilter").advansedFilter({ filters: [{ type: "combobox", id: "tag", options: tags}] });
    };

    return {
        projectsLoad: projectsLoad,
        milestoneStatus: milestoneStatus,
        milestonesLoad: milestonesLoad,
        tagsLoad: tagsLoad,
        filterVisible: filterVisible
    };
})(jQuery);
