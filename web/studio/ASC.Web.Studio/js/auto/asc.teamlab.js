
;window.Teamlab = (function () {
  var
    isInit = false,
    eventManager = null;

  var customEvents = {
    getException      : 'ongetexception',
    getAuthentication : 'ongetauthentication',

    addComment    : 'onaddcomment',
    updateComment : 'onupdatecomment',
    removeComment : 'onremovecomment',

    getCmtBlog : 'ongetcmtyblog',

    addPrjComment           : 'onaddprjcomment',
    updatePrjComment        : 'onupdateprjcomment',
    removePrjComment        : 'onremoveprjcomment',
    updatePrjTask           : 'onupdateprjtask',
    addPrjTask              : 'onaddprjtask',
    getPrjTask              : 'ongetprjtask',
    getPrjTasks             : 'ongetprjtasks',
    addPrjSubtask           : 'onaddprjsubtask',
    removePrjSubtask        : 'onremoveprjsubtask',
    updatePrjSubtask        : 'onupdateprjsubtask',
    removePrjTask           : 'onremoveprjtask',
    getPrjDiscussion        : 'ongetprjdiscussion',
    getPrjDiscussions       : 'ongetprjdiscussions',
    getPrjProjects          : 'ongetprjprojects',
    getPrjTeam              : 'ongetprjteam',
    getPrjMilestones        : 'ongetprjmilestones',
    updatePrjProjectStatus  : 'onupdateprjprojectstatus',
    updatePrjTime           : 'onupdateprjtime',
    removePrjTime           : 'onremoveprjtime'
  },
  customEventsHash = {},
  eventManager = new CustomEvent(customEvents);
  extendCustomEventsHash();

  function isArray (o) {
   return o ? o.constructor.toString().indexOf("Array") != -1 : false;
  }

  function extendCustomEventsHash () {
    for (var fld in customEvents) {
      customEventsHash[fld] = true;
      customEventsHash[customEvents[fld]] = true;
    }
  }

  function callMethodByName (handlername, container, self, args) {
    handlername = handlername.replace(/-/g, '_');
      if (container && typeof container === 'object' && typeof container[handlername] === 'function') {
        container[handlername].apply(self, args);
      }
  }

  function returnValue (value) {
    return value && isArray(value) ? window.Teamlab : value;
  }

  var init = function () {
    if (isInit === true) {
      return undefined;
    }
    isInit = true;

    ServiceManager.bind(null, onGetResponse);
    ServiceManager.bind('event', onGetEvent);
    ServiceManager.bind('extention', onGetExtention);
    //ServiceManager.bind('me', onGetOwnProfile);
  };

  var bind = function (eventname, handler, params) {
    return eventManager.bind(eventname, handler, params);
  };

  var unbind = function (handlerid) {
    return eventManager.unbind(handlerid);
  };

  var call = function (eventname, self, args) {
    eventManager.call(eventname, self, args);
  };

  var extendEventManager = function (events) {
    for (var fld in events) {
      if (events.hasOwnProperty(fld)) {
        customEvents[fld] = events[fld];
      }
    }
    eventManager.extend(customEvents);
    extendCustomEventsHash();
  };

  function onGetEvent (eventname, self, args) {
    if (customEventsHash.hasOwnProperty(eventname)) {
      call(eventname, self, args);
    }
  }

  function onGetExtention (eventname, params, errors) {
    eventManager.call(customEvents.getException, this, [params, errors]);
  }

  function onGetResponse (params, obj) {
    if (params.hasOwnProperty('___handler') && params.hasOwnProperty('___container')) {
      var args = [params];
      for (var i = 1, n = arguments.length; i < n; i++) {
        args.push(arguments[i]);
      }
      callMethodByName(params.___handler, params.___container, this, args);
    }
  }

  function onGetOwnProfile (params, profile) {
    //console.log('me: ', profile);
  }

  var joint = function () {
    ServiceManager.joint();
    return window.Teamlab;
  };

  var start = function (params, options) {
    return ServiceManager.start(params, options);
  };

  /* <common> */
  var getQuotas = function (params, options) {
    return returnValue(ServiceManager.getQuotas(customEvents.getQuotas, params, options));
  };
  /* </Common> */

  /* <people> */
  var getProfile = function (params, id, options) {
    return returnValue(ServiceManager.getProfile(customEvents.getProfile, params, id, options));
  };

  var getProfiles = function (params, options) {
    return returnValue(ServiceManager.getProfiles(customEvents.getProfiles, params, options));
  };

  var getGroup = function (params, id, options) {
    return returnValue(ServiceManager.getGroup(customEvents.getGroup, params, id, options));
  };

  var getGroups = function (params, options) {
    return returnValue(ServiceManager.getGroups(customEvents.getGroups, params, options));
  };
  /* </people> */

  /* <community> */
  var addCmtBlog = function (params, data, options) {
    return returnValue(ServiceManager.addCmtBlog(customEvents.addCmtBlog, params, data, options));
  };

  var getCmtBlog = function (params, id, options) {
    return returnValue(ServiceManager.getCmtBlog(customEvents.getCmtBlog, params, id, options));
  };

  var getCmtBlogs = function (params, options) {
    return returnValue(ServiceManager.getCmtBlogs(customEvents.getCmtBlogs, params, options));
  };

  var addCmtForumTopic = function (params, threadid, data, options) {
    if (arguments.length === 3) {
      options = arguments[2];
      data = arguments[1];
      threadid = data && typeof data === 'object' && data.hasOwnProperty('threadid') ? data.threadid : null;
    }

    return returnValue(ServiceManager.addCmtForumTopic(customEvents.addCmtForumTopic, params, threadid, data, options));
  };

  var getCmtForumTopic = function (params, id, options) {
    return returnValue(ServiceManager.getCmtForumTopic(customEvents.getCmtForumTopic, params, id, options));
  };

  var getCmtForumTopics = function (params, options) {
    return returnValue(ServiceManager.getCmtForumTopics(customEvents.getCmtForumTopics, params, options));
  };

  var getCmtForumCategories = function (params, options) {
    return returnValue(ServiceManager.getCmtForumCategories(customEvents.getCmtForumCategories, params, options));
  };

  var addCmtEvent = function (params, data, options) {
    return returnValue(ServiceManager.addCmtEvent(customEvents.addCmtEvent, params, data, options));
  };

  var getCmtEvent = function (params, id, options) {
    return returnValue(ServiceManager.getCmtEvent(customEvents.getCmtEvent, params, id, options));
  };

  var getCmtEvents = function (params, options) {
    return returnValue(ServiceManager.getCmtEvents(customEvents.getCmtEvents, params, options));
  };

  var addCmtBookmark = function (params, data, options) {
    return returnValue(ServiceManager.addCmtBookmark(customEvents.addCmtBookmark, params, data, options));
  };

  var getCmtBookmark = function (params, id, options) {
    return returnValue(ServiceManager.getCmtBookmark(customEvents.getCmtBookmark, params, id, options));
  };

  var getCmtBookmarks = function (params, options) {
    return returnValue(ServiceManager.getCmtBookmarks(customEvents.getCmtBookmarks, params, options));
  };

  var addCmtForumTopicPost = function (params, id, data, options) {
    return returnValue(ServiceManager.addCmtForumTopicPost(customEvents.addCmtForumTopicPost, params, id, data, options));
  };

  var addCmtBlogComment = function (params, id, data, options) {
    return returnValue(ServiceManager.addCmtBlogComment(customEvents.addCmtBlogComment, params, id, data, options));
  };

  var getCmtBlogComments = function (params, id, options) {
    return returnValue(ServiceManager.getCmtBlogComments(customEvents.getCmtBlogComments, params, id, options));
  };

  var addCmtEventComment = function (params, id, data, options) {
    return returnValue(ServiceManager.addCmtEventComment(customEvents.addCmtEventComment, params, id, data, options));
  };

  var getCmtEventComments = function (params, id, options) {
    return returnValue(ServiceManager.getCmtEventComments(customEvents.getCmtEventComments, params, id, options));
  };

  var addCmtBookmarkComment = function (params, id, data, options) {
    return returnValue(ServiceManager.addCmtBookmarkComment(customEvents.addCmtBookmarkComment, params, id, data, options));
  };

  var getCmtBookmarkComments = function (params, id, options) {
    return returnValue(ServiceManager.getCmtBookmarkComments(customEvents.getCmtBookmarkComments, params, id, options));
  };
  /* </community> */

  /* <projects> */
  var getPrjTags = function (params, options) {
    return returnValue(ServiceManager.getPrjTags(customEvents.getPrjTags, params, options));
  };

  var getPrjTagsByName = function(params, name, data, options) {
    return returnValue(ServiceManager.getPrjTagsByName(customEvents.getPrjTagsByName, params, name, data, options));
  };

  var addPrjComment = function (params, type, id, data, options) {
    var fn = null;
    switch (type.toLowerCase()) {
      case 'discussion' :
        fn = ServiceManager.addPrjDiscussionComment;
        break;
    }
    if (typeof fn === 'function') {
      return returnValue(fn(customEvents.addPrjComment, params, id, data, options));
    }
    return false;
  };

  var updatePrjComment = function (params, type, id, data, options) {
    var fn = null;
    switch (type.toLowerCase()) {
      case 'discussion' :
        fn = ServiceManager.updatePrjDiscussionComment;
        break;
    }
    if (typeof fn === 'function') {
      return returnValue(fn(customEvents.updatePrjComment, params, id, data, options));
    }
    return false;
  };

  var removePrjComment = function (params, type, id, options) {
    var fn = null;
    switch (type.toLowerCase()) {
      case 'discussion' :
        fn = ServiceManager.removePrjDiscussionComment;
        break;
    }
    if (typeof fn === 'function') {
      return returnValue(fn(customEvents.removePrjComment, params, id, options));
    }
    return false;
  };

  var getPrjComments = function (params, type, id, options) {
    var fn = null;
    switch (type.toLowerCase()) {
      case 'discussion' :
        fn = ServiceManager.getPrjDiscussionComments;
        break;
    }
    if (typeof fn === 'function') {
      return returnValue(fn(customEvents.getPrjComments, params, id, options));
    }
    return false;
  };

  var addPrjTaskComment = function (params, id, data, options) {
    return returnValue(ServiceManager.addPrjTaskComment(customEvents.addPrjTaskComment, params, id, data, options));
  };

  var updatePrjTaskComment = function (params, id, data, options) {
    return returnValue(ServiceManager.updatePrjTaskComment(customEvents.updatePrjTaskComment, params, id, data, options));
  };

  var removePrjTaskComment = function (params, id, options) {
    return returnValue(ServiceManager.removePrjTaskComment(customEvents.removePrjTaskComment, params, id, options));
  };

  var getPrjTaskComments = function (params, id, options) {
    return returnValue(ServiceManager.getPrjTaskComments(customEvents.getPrjTaskComments, params, id, options));
  };

  var addPrjDiscussionComment = function (params, id, data, options) {
    return returnValue(ServiceManager.addPrjDiscussionComment(customEvents.addPrjDiscussionComment, params, id, data, options));
  };

  var updatePrjDiscussionComment = function (params, id, data, options) {
    return returnValue(ServiceManager.updatePrjDiscussionComment(customEvents.updatePrjDiscussionComment, params, id, data, options));
  };

  var removePrjDiscussionComment = function (params, id, options) {
    return returnValue(ServiceManager.removePrjDiscussionComment(customEvents.removePrjDiscussionComment, params, id, options));
  };

  var getPrjDiscussionComments = function (params, id, options) {
    return returnValue(ServiceManager.getPrjDiscussionComments(customEvents.getPrjDiscussionComments, params, id, options));
  };

  var addPrjMilestoneComment = function (params, id, data, options) {
    return returnValue(ServiceManager.addPrjMilestoneComment(customEvents.addPrjMilestoneComment, params, id, data, options));
  };

  var updatePrjMilestoneComment = function (params, id, data, options) {
    return returnValue(ServiceManager.updatePrjMilestoneComment(customEvents.updatePrjMilestoneComment, params, id, data, options));
  };

  var removePrjMilestoneComment = function (params, id, options) {
    return returnValue(ServiceManager.removePrjMilestoneComment(customEvents.removePrjMilestoneComment, params, id, options));
  };

  var getPrjMilestoneComments = function (params, id, options) {
    return returnValue(ServiceManager.getPrjMilestoneComments(customEvents.getPrjMilestoneComments, params, id, options));
  };

  var addPrjSubtask = function (params, taskid, data, options) {
    return returnValue(ServiceManager.addPrjSubtask(customEvents.addPrjSubtask, params, taskid, data, options));
  };

  var updatePrjSubtask = function (params, taskid, subtaskid, data, options) {
    return returnValue(ServiceManager.updatePrjSubtask(customEvents.updatePrjSubtask, params, taskid, subtaskid, data, options));
  };

  var removePrjSubtask = function (params, taskid, subtaskid, options) {
    return returnValue(ServiceManager.removePrjSubtask(customEvents.removePrjSubtask, params, taskid, subtaskid, options));
  };

  var addPrjTask = function (params, projectid, data, options) {
    return returnValue(ServiceManager.addPrjTask(customEvents.addPrjTask, params, projectid, data, options));
  };

  var updatePrjTask = function (params, taskid, data, options) {
    return returnValue(ServiceManager.updatePrjTask(customEvents.updatePrjTask, params, taskid, data, options));
  };

  var removePrjTask = function (params, id, options) {
    return returnValue(ServiceManager.removePrjTask(customEvents.removePrjTask, params, id, options));
  };

  var getPrjTask = function (params, id, options) {
    return returnValue(ServiceManager.getPrjTask(customEvents.getPrjTask, params, id, options));
  };

  var getPrjTasks = function (params, projectid, type, status, options) {
    return returnValue(ServiceManager.getPrjTasks(customEvents.getPrjTasks, params, projectid, type, status, options));
  };

  var getPrjTeam = function (params, projectid, options) {
    return returnValue(ServiceManager.getPrjTeam(customEvents.getPrjTeam, params, projectid, options));
  };

  var updatePrjTeam = function(params, projectid, data, options) {
    return returnValue(ServiceManager.updatePrjTeam(customEvents.updatePrjTeam, params, projectid, data, options));
  };

  var addPrjProjectRequest = function (params, data, options) {
    return returnValue(ServiceManager.addPrjProjectRequest(customEvents.addPrjProjectRequest, params, data, options));
  };

  var updatePrjProjectRequest = function (params, id, data, options) {
    return returnValue(ServiceManager.updatePrjProjectRequest(customEvents.updatePrjProjectRequest, params, id, data, options));
  };

  var removePrjProjectRequest = function (params, id, options) {
    return returnValue(ServiceManager.removePrjProjectRequest(customEvents.removePrjProjectRequest, params, id, options));
  };

  var getPrjTaskFiles = function (params, taskid, options) {
    return returnValue(ServiceManager.getPrjTaskFiles(customEvents.getPrjTaskFiles, params, taskid, options));
  };

  var getPrjProjectFolder = function (params, taskid, options) {
    return returnValue(ServiceManager.getPrjProjectFolder(customEvents.getPrjProjectFolder, params, taskid, options));
  };

  var addPrjEntityFiles = function (params, entityid, entitytype, data, options) {
    return returnValue(ServiceManager.addPrjEntityFiles(customEvents.addPrjEntityFiles, params, entityid, entitytype, data, options));
  };

  var removePrjEntityFiles = function (params, entityid, entitytype, data, options) {
    return returnValue(ServiceManager.removePrjEntityFiles(customEvents.removePrjEntityFiles, params, entityid, entitytype, data, options));
  };

  var getPrjEntityFiles = function (params, entityid, entitytype, options) {
    return returnValue(ServiceManager.getPrjEntityFiles(customEvents.getPrjEntityFiles, params, entityid, entitytype, options));
  };

  var addPrjMilestone = function (params, projectid, data, options) {
    return returnValue(ServiceManager.addPrjMilestone(customEvents.addPrjMilestone, params, projectid, data, options));
  };

  var updatePrjMilestone = function (params, id, data, options) {
    return returnValue(ServiceManager.updatePrjMilestone(customEvents.updatePrjMilestone, params, id, data, options));
  };

  var removePrjMilestone = function (params, id, options) {
    return returnValue(ServiceManager.removePrjMilestone(customEvents.removePrjMilestone, params, id, options));
  };

  var getPrjMilestone = function (params, id, options) {
    return returnValue(ServiceManager.getPrjMilestone(customEvents.getPrjMilestone, params, id, options));
  };

  var getPrjMilestones = function (params, projectid, options) {
    if (arguments.length < 3) {
      options = arguments[1];
      projectid = null;
    }

    return returnValue(ServiceManager.getPrjMilestones(customEvents.getPrjMilestones, params, projectid, options));
  };

  var addPrjDiscussion = function (params, projectid, data, options) {
    return returnValue(ServiceManager.addPrjDiscussion(customEvents.addPrjDiscussion, params, projectid, data, options));
  };

  var updatePrjDiscussion = function (params, id, data, options) {
    return returnValue(ServiceManager.updatePrjDiscussion(customEvents.updatePrjDiscussion, params, id, data, options));
  };

  var removePrjDiscussion = function (params, id, options) {
    return returnValue(ServiceManager.removePrjDiscussion(customEvents.removePrjDiscussion, params, id, options));
  };

  var getPrjDiscussion = function (params, id, options) {
    return returnValue(ServiceManager.getPrjDiscussion(customEvents.getPrjDiscussion, params, id, options));
  };

  var getPrjDiscussions = function (params, projectid, options) {
    if (arguments.length < 3) {
      options = arguments[1];
      projectid = null;
    }

    return returnValue(ServiceManager.getPrjDiscussions(customEvents.getPrjDiscussions, params, projectid, options));
  };

  var addPrjProject = function (params, data, options) {
    return returnValue(ServiceManager.addPrjProject(customEvents.addPrjProject, params, data, options));
  };

  var updatePrjProject = function(params, id, data, options) {
    return returnValue(ServiceManager.updatePrjProject(customEvents.updatePrjProject, params, id, data, options));
  };

  var updatePrjProjectStatus = function(params, id, data, options){
    return returnValue(ServiceManager.updatePrjProjectStatus(customEvents.updatePrjProjectStatus, params, id, data, options));
  };

  var removePrjProject = function (params, id, options) {
    return returnValue(ServiceManager.removePrjProject(customEvents.removePrjProject, params, id, options));
  };

  var followingPrjProject = function(params, projectid, data, options) {
    return returnValue(ServiceManager.followingPrjProject(customEvents.followingPrjProject, params, projectid, data, options));
  };

  var getPrjProject = function (params, id, options) {
    return returnValue(ServiceManager.getPrjProject(customEvents.getPrjProject, params, id, options));
  };

  var getPrjProjects = function (params, options) {
    return returnValue(ServiceManager.getPrjProjects(customEvents.getPrjProjects, params, options));
  };

  var getPrjSelfProjects = function (params, options) {
    return returnValue(ServiceManager.getPrjSelfProjects(customEvents.getPrjProjects, params, options));
  };

  var getPrjFollowProjects = function (params, options) {
    return returnValue(ServiceManager.getPrjFollowProjects(customEvents.getPrjProjects, params, options));
  };

  var addPrjProjectTeamPerson = function (params, projectid, data, options) {
    return returnValue(ServiceManager.addPrjProjectTeamPerson(customEvents.addPrjProjectTeamPerson, params, projectid, data, options));
  };

  var removePrjProjectTeamPerson = function (params, projectid, data, options) {
    return returnValue(ServiceManager.removePrjProjectTeamPerson(customEvents.removePrjProjectTeamPerson, params, projectid, data, options));
  };

  var getPrjProjectTeamPersons = function (params, projectid, options) {
    return returnValue(ServiceManager.getPrjProjectTeamPersons(customEvents.getPrjProjectTeamPersons, params, projectid, options));
  };

  var getPrjProjectFiles = function (params, projectid, options) {
    return returnValue(ServiceManager.getPrjProjectFiles(customEvents.getPrjProjectFiles, params, projectid, options));
  };

  var updatePrjTime = function(params, id, data, options) {
    return returnValue(ServiceManager.updatePrjTime(customEvents.updatePrjTime, params, id, data, options));
  };
	
  var removePrjTime = function(params, id, options) {
    return returnValue(ServiceManager.removePrjTime(customEvents.removePrjTime, params, id, options));
  };

  var getPrjActivities = function(params, options) {
      return returnValue(ServiceManager.getPrjActivities(customEvents.getPrjActivities, params, options));
  };
    
  /* </projects> */

  /* <documents> */
  var createDocUploadFile = function (params, id, data, options) {
    return returnValue(ServiceManager.createDocUploadFile(customEvents.uploadDocFile, params, id, data, options));
  };

  var addDocFile = function (params, id, type, data, options) {
    if (arguments.length < 5) {
      options = arguments[3];
      data = arguments[3];
      type = null;
    }

    return returnValue(ServiceManager.addDocFile(customEvents.addDocFile, params, id, type, data, options));
  };

  var getDocFile = function (params, id, options) {
    return returnValue(ServiceManager.getDocFile(customEvents.getDocFile, params, id, options));
  };

  var addDocFolder = function (params, id, data, options) {
    return returnValue(ServiceManager.addDocFolder(customEvents.addDocFolder, params, id, data, options));
  };

  var getDocFolder = function (params, folderid, options) {
    return returnValue(ServiceManager.getDocFolder(customEvents.getDocFolder, params, folderid, options));
  };
  /* </documents> */

  /* <crm> */
  var createCrmUploadFile = function (params, type, id, data, options) {
    return returnValue(ServiceManager.createCrmUploadFile(customEvents.uploadCrmFile, params, type, id, data, options));
  };

  var addCrmContactInfo = function (params, contactid, data, options) {
    return returnValue(ServiceManager.addCrmContactInfo(customEvents.addCrmContactInfo, params, contactid, data, options));
  };

  var updateCrmContactInfo = function (params, contactid, data, options) {
    return returnValue(ServiceManager.updateCrmContactInfo(customEvents.updateCrmContactInfo, params, contactid, data, options));
  };

  var addCrmContactTwitter = function (params, contactid, data, options) {
    return returnValue(ServiceManager.addCrmContactTwitter(customEvents.addCrmContactTwitter, params, contactid, data, options));
  };

  var addCrmEntityNote = function (params, type, id, data, options) {
    return returnValue(ServiceManager.addCrmEntityNote(customEvents.addCrmEntityNote, params, type, id, data, options));
  };

  var addCrmContact = function (params, data, options) {
    return returnValue(ServiceManager.addCrmContact(customEvents.addCrmContact, params, data, options));
  };

  var addCrmCompany = function (params, data, options) {
    return returnValue(ServiceManager.addCrmCompany(customEvents.addCrmCompany, params, data, options));
  };
  
  var updateCrmCompany = function (params, id, data, options) {
    return returnValue(ServiceManager.updateCrmCompany(customEvents.updateCrmCompany, params, id, data, options));
  };

  var addCrmPerson = function (params, data, options) {
    return returnValue(ServiceManager.addCrmPerson(customEvents.addCrmPerson, params, data, options));
  };
  
  var updateCrmPerson = function (params, id, data, options) {   
    return returnValue(ServiceManager.updateCrmPerson(customEvents.updateCrmPerson, params, id, data, options));
  };

  var addCrmContactData = function (params, id, data, options) {
    return returnValue(ServiceManager.addCrmContactData(customEvents.addCrmContactData, params, id, data, options));
  };

  var updateCrmContactData = function (params, id, data, options) {
    return returnValue(ServiceManager.updateCrmContactData(customEvents.updateCrmContactData, params, id, data, options));
  };

  var removeCrmContact = function (params, ids, options) {
    if (arguments.length === 2) {
      options = arguments[1];
      ids = null;
    }

    return returnValue(ServiceManager.removeCrmContact(customEvents.removeCrmContact, params, ids, options));
  };

  var addCrmTag = function (params, type, ids, tagname, options) {
    if (arguments.length === 4) {
      options = arguments[3];
      tagname = arguments[2];
      ids = null;
    }

    return returnValue(ServiceManager.addCrmTag(customEvents.addCrmTag, params, type, ids, tagname, options));
  };

  var addCrmEntityTag = function (params, type, tagname, options) {
    return returnValue(ServiceManager.addCrmEntityTag(customEvents.addCrmEntityTag, params, type, tagname, options));
  };

  var removeCrmTag = function (params, type, id, tagname, options) {
    return returnValue(ServiceManager.removeCrmTag(customEvents.removeCrmTag, params, type, id, tagname, options));
  };

  var removeCrmEntityTag = function(params, type, tagname, options) {
    return returnValue(ServiceManager.removeCrmEntityTag(customEvents.removeCrmEntityTag, params, type, tagname, options));
  };

  var removeCrmUnusedTag = function(params, type, options) {
    return returnValue(ServiceManager.removeCrmUnusedTag(customEvents.removeCrmUnusedTag, params, type, options));
  };

  var addCrmCustomField = function(params, type, data, options) {
    return returnValue(ServiceManager.addCrmCustomField(customEvents.addCrmCustomField, params, type, data, options));
  };

  var updateCrmCustomField = function(params, type, id, data, options) {
    return returnValue(ServiceManager.updateCrmCustomField(customEvents.updateCrmCustomField, params, type, id, data, options));
  };

  var removeCrmCustomField = function(params, type, id, options) {
    return returnValue(ServiceManager.removeCrmCustomField(customEvents.removeCrmCustomField, params, type, id, options));
  };

  var addCrmDealMilestone = function(params, data, options) {
    return returnValue(ServiceManager.addCrmDealMilestone(customEvents.addCrmDealMilestone, params, data, options));
  };

  var updateCrmDealMilestone = function(params, id, data, options) {
    return returnValue(ServiceManager.updateCrmDealMilestone(customEvents.updateCrmDealMilestone, params, id, data, options));
  };

  var removeCrmDealMilestone = function(params, id, options) {
    return returnValue(ServiceManager.removeCrmDealMilestone(customEvents.removeCrmDealMilestone, params, id, options));
  };

  var addCrmContactStatus = function(params, data, options) {
    return returnValue(ServiceManager.addCrmContactStatus(customEvents.addCrmContactStatus, params, data, options));
  };

  var updateCrmContactStatus = function(params, id, data, options) {
    return returnValue(ServiceManager.updateCrmContactStatus(customEvents.updateCrmContactStatus, params, id, data, options));
  };

  var removeCrmContactStatus = function(params, id, options) {
    return returnValue(ServiceManager.removeCrmContactStatus(customEvents.removeCrmContactStatus, params, id, options));
  };

  var addCrmListItem = function(params, type, data, options) {
    return returnValue(ServiceManager.addCrmListItem(customEvents.addCrmListItem, params, type, data, options));
  };

  var updateCrmListItem = function(params, type, id, data, options) {
    return returnValue(ServiceManager.updateCrmListItem(customEvents.updateCrmListItem, params, type, id, data, options));
  };

  var removeCrmListItem = function(params, type, id, options) {
    return returnValue(ServiceManager.removeCrmListItem(customEvents.removeCrmListItem, params, type, id, options));
  };

  var addCrmTask = function (params, data, options) {
    return returnValue(ServiceManager.addCrmTask(customEvents.addCrmTask, params, data, options));
  };

  var getCrmTask = function (params, id, options) {
    return returnValue(ServiceManager.getCrmTask(customEvents.getCrmTask, params, id, options));
  };

  var updateCrmTask = function (params, id, data, options) {
    return returnValue(ServiceManager.updateCrmTask(customEvents.updateCrmTask, params, id, data, options));
  };

  var removeCrmTask = function (params, id, options) {
    return returnValue(ServiceManager.removeCrmTask(customEvents.removeCrmTask, params, id, options));
  };

  var addCrmEntityMember = function (params, type, entityid, id, data, options) {
    var fn = null;
    switch (type) {
      case 'company'  : fn = ServiceManager.addCrmPersonMember; break;
      default         : fn = ServiceManager.addCrmContactMember; break;
    }
    if (fn) {
      return returnValue(fn(customEvents.addCrmEntityMember, params, type, entityid, id, data, options));
    }
    return false;
  };

  var removeCrmEntityMember = function (params, type, entityid, id, options) {
    var fn = null;
    switch (type) {
      case 'company'  : fn = ServiceManager.removeCrmPersonMember; break;
      default         : fn = ServiceManager.removeCrmContactMember; break;
    }
    if (fn) {
      return returnValue(fn(customEvents.removeCrmEntityMember, params, type, entityid, id, options));
    }
    return false;
  };

  var getCrmCases = function (params, options) {
    return returnValue(ServiceManager.getCrmCases(customEvents.getCrmCases, params, options));
  };

  var getCrmContacts = function (params, options) {
    return returnValue(ServiceManager.getCrmContacts(customEvents.getCrmContacts, params, options));
  };

  var getCrmContact = function (params, id, options) {
    return returnValue(ServiceManager.getCrmContact(customEvents.getCrmContact, params, id, options));
  };

  var getCrmTags = function (params, type, id, options) {
    return returnValue(ServiceManager.getCrmTags(customEvents.getCrmTags, params, type, id, options));
  };

  var getCrmEntityMembers = function (params, type, id, options) {
    var fn = null;
    switch (type) {
      case 'company'  : fn = ServiceManager.getCrmPersonMembers; break;
      default         : fn = ServiceManager.getCrmContactMembers; break;
    }
    if (fn) {
      return returnValue(fn(customEvents.getCrmEntityMembers, params, type, id, options));
    }
    return false;
  };

  var getCrmContactTasks = function (params, data, options) {
    return returnValue(ServiceManager.getCrmContactTasks(customEvents.getCrmContactTasks, params, data, options));
  };

  var getCrmTasks = function (params, options) {
    return returnValue(ServiceManager.getCrmTasks(customEvents.getCrmTasks, params, options));
  };

  var getCrmOpportunities = function (params, options) {
    return returnValue(ServiceManager.getCrmOpportunities(customEvents.getCrmOpportunities, params, options));
  };

  var removeCrmOpportunity = function(params, id, options) {
    return returnValue(ServiceManager.removeCrmOpportunity(customEvents.removeCrmOpportunity, params, id, options));
  };

  var addCrmHistoryEvent = function (params, data, options) {
    return returnValue(ServiceManager.addCrmHistoryEvent(customEvents.addCrmHistoryEvent, params, data, options));
  };

  var removeCrmHistoryEvent = function (params, id, options) {
    return returnValue(ServiceManager.removeCrmHistoryEvent(customEvents.removeCrmHistoryEvent, params, id, options));
  };

  var getCrmHistoryEvents = function (params, options) {
    return returnValue(ServiceManager.getCrmHistoryEvents(customEvents.getCrmHistoryEvents, params, options));
  };

  var removeCrmFile = function (params, id, options) {
    return returnValue(ServiceManager.removeCrmFile(customEvents.removeCrmFile, params, id, options));
  };

  var getCrmFolder = function (params, id, options) {
    return returnValue(ServiceManager.getCrmFolder(customEvents.getCrmFolder, params, id, options));
  };

  var updateCrmContactRights = function (params, id, data, options) {
    return returnValue(ServiceManager.updateCrmContactRights(customEvents.updateCrmContactRights, params, id, data, options));
  };

  var addCrmEntityFiles = function (params, id, type, data, options) {
    return returnValue(ServiceManager.addCrmEntityFiles(customEvents.addCrmEntityFiles, params, id, type, data, options));
  };

  var removeCrmEntityFiles = function (params, id, options) {
    return returnValue(ServiceManager.removeCrmEntityFiles(customEvents.removeCrmEntityFiles, params, id, options));
  };

  var getCrmEntityFiles = function (params, id, type, options) {
    return returnValue(ServiceManager.getCrmEntityFiles(customEvents.getCrmEntityFiles, params, id, type, options));
  };

  var getCrmTaskCategories = function (params, options) {
    return returnValue(ServiceManager.getCrmTaskCategories(customEvents.getCrmTaskCategories, params, options));
  };

  var getCrmHistoryCategories = function (params, options) {
    return returnValue(ServiceManager.getCrmHistoryCategories(customEvents.getCrmHistoryCategories, params, options));
  };

  var addCrmEntityTaskTemplateContainer = function(params, data, options) {
    return returnValue(ServiceManager.addCrmEntityTaskTemplateContainer(customEvents.addCrmEntityTaskTemplateContainer, params, data, options));
  };

  var updateCrmEntityTaskTemplateContainer = function (params, id, data, options) {
    return returnValue(ServiceManager.updateCrmEntityTaskTemplateContainer(customEvents.updateCrmEntityTaskTemplateContainer, params, id, data, options));
  };

  var removeCrmEntityTaskTemplateContainer = function (params, id, options) {
    return returnValue(ServiceManager.removeCrmEntityTaskTemplateContainer(customEvents.removeCrmEntityTaskTemplateContainer, params, id, options));
  };

  var getCrmEntityTaskTemplateContainer = function(params, id, options) {
    return returnValue(ServiceManager.getCrmEntityTaskTemplateContainer(customEvents.getCrmEntityTaskTemplateContainer, params, id, options));
  };

  var getCrmEntityTaskTemplateContainers = function(params, type, options) {
    return returnValue(ServiceManager.getCrmEntityTaskTemplateContainers(customEvents.getCrmEntityTaskTemplateContainers, params, type, options));
  };

  var addCrmEntityTaskTemplate = function(params, data, options) {
    return returnValue(ServiceManager.addCrmEntityTaskTemplate(customEvents.addCrmEntityTaskTemplate, params, data, options));
  };

  var updateCrmEntityTaskTemplate = function (params, data, options) {
    return returnValue(ServiceManager.updateCrmEntityTaskTemplate(customEvents.updateCrmEntityTaskTemplate, params, data, options));
  };

  var removeCrmEntityTaskTemplate = function (params, id, options) {
    return returnValue(ServiceManager.removeCrmEntityTaskTemplate(customEvents.removeCrmEntityTaskTemplate, params, id, options));
  };

  var getCrmEntityTaskTemplate = function(params, id, options) {
    return returnValue(ServiceManager.getCrmEntityTaskTemplate(customEvents.getCrmEntityTaskTemplate, params, id, options));
  };

  var getCrmEntityTaskTemplates = function(params, containerid, options) {
    return returnValue(ServiceManager.getCrmEntityTaskTemplates(customEvents.getCrmEntityTaskTemplates, params, containerid, options));
  };

  /* </crm> */
  /* <settings> */
  var getWebItemSecurityInfo = function (params, data, options) {
    return returnValue(ServiceManager.getWebItemSecurityInfo(customEvents.getWebItemSecurityInfo, params, data, options));
  };

  var setWebItemSecurity = function (params, data, options) {
    return returnValue(ServiceManager.setWebItemSecurity(customEvents.setWebItemSecurity, params, data, options));
  };

  var setProductAdministrator = function (params, data, options) {
    return returnValue(ServiceManager.setProductAdministrator(customEvents.setProductAdministrator, params, data, options));
  };

  var isProductAdministrator = function (params, data, options) {
    return returnValue(ServiceManager.isProductAdministrator(customEvents.isProductAdministrator, params, data, options));
  };
  /* </settings> */
  return {
    events : customEvents,
    constants : {
      dateFormats : ServiceFactory.dateFormats,
      contactTypes : ServiceFactory.contactTypes,
      nameCollections : ServiceFactory.nameCollections
    },

    create              : ServiceFactory.create,
    formattingDate      : ServiceFactory.formattingDate,
    serializeDate       : ServiceFactory.serializeDate,
    serializeTimestamp  : ServiceFactory.serializeTimestamp,
    getDisplayTime      : ServiceFactory.getDisplayTime,
    getDisplayDate      : ServiceFactory.getDisplayDate,
    getDisplayDatetime  : ServiceFactory.getDisplayDatetime,
    sortCommentsByTree  : ServiceFactory.sortCommentsByTree,

    joint : joint,
    start : start,

    init                : init,
    bind                : bind,
    unbind              : unbind,
    call                : call,
    extendEventManager  : extendEventManager,

    getQuotas : getQuotas,

    getProfile  : getProfile,
    getProfiles : getProfiles,
    getGroup    : getGroup,
    getGroups   : getGroups,

    addCmtBlog            : addCmtBlog,
    getCmtBlog            : getCmtBlog,
    getCmtBlogs           : getCmtBlogs,
    addCmtForumTopic      : addCmtForumTopic,
    getCmtForumTopic      : getCmtForumTopic,
    getCmtForumTopics     : getCmtForumTopics,
    getCmtForumCategories : getCmtForumCategories,
    addCmtEvent           : addCmtEvent,
    getCmtEvent           : getCmtEvent,
    getCmtEvents          : getCmtEvents,
    addCmtBookmark        : addCmtBookmark,
    getCmtBookmark        : getCmtBookmark,
    getCmtBookmarks       : getCmtBookmarks,

    addCmtForumTopicPost    : addCmtForumTopicPost,
    addCmtBlogComment       : addCmtBlogComment,
    getCmtBlogComments      : getCmtBlogComments,
    addCmtEventComment      : addCmtEventComment,
    getCmtEventComments     : getCmtEventComments,
    addCmtBookmarkComment   : addCmtBookmarkComment,
    getCmtBookmarkComments  : getCmtBookmarkComments,

    getPrjTags                  : getPrjTags,
    getPrjTagsByName            : getPrjTagsByName,
    addPrjComment               : addPrjComment,
    updatePrjComment            : updatePrjComment,
    removePrjComment            : removePrjComment,
    getPrjComments              : getPrjComments,
    addPrjTaskComment           : addPrjTaskComment,
    updatePrjTaskComment        : updatePrjTaskComment,
    removePrjTaskComment        : removePrjTaskComment,
    getPrjTaskComments          : getPrjTaskComments,
    addPrjDiscussionComment     : addPrjDiscussionComment,
    updatePrjDiscussionComment  : updatePrjDiscussionComment,
    removePrjDiscussionComment  : removePrjDiscussionComment,
    getPrjDiscussionComments    : getPrjDiscussionComments,
    addPrjMilestoneComment      : addPrjMilestoneComment,
    updatePrjMilestoneComment   : updatePrjMilestoneComment,
    removePrjMilestoneComment   : removePrjMilestoneComment,
    getPrjMilestoneComments     : getPrjMilestoneComments,

    addPrjEntityFiles           : addPrjEntityFiles,
    removePrjEntityFiles        : removePrjEntityFiles,
    getPrjEntityFiles           : getPrjEntityFiles,
    addPrjSubtask               : addPrjSubtask,
    updatePrjSubtask            : updatePrjSubtask,
    updatePrjTask               : updatePrjTask,
    removePrjSubtask            : removePrjSubtask,
    addPrjTask                  : addPrjTask,
    getPrjTask                  : getPrjTask,
    getPrjTasks                 : getPrjTasks,
    addPrjMilestone             : addPrjMilestone,
    updatePrjMilestone          : updatePrjMilestone,
    removePrjMilestone          : removePrjMilestone,
    getPrjMilestone             : getPrjMilestone,
    getPrjMilestones            : getPrjMilestones,
    addPrjDiscussion            : addPrjDiscussion,
    updatePrjDiscussion         : updatePrjDiscussion,
    removePrjDiscussion         : removePrjDiscussion,
    getPrjDiscussion            : getPrjDiscussion,
    getPrjDiscussions           : getPrjDiscussions,
    addPrjProjectRequest        : addPrjProjectRequest,
    updatePrjProjectRequest     : updatePrjProjectRequest,
    removePrjProjectRequest     : removePrjProjectRequest,
    addPrjProject               : addPrjProject,
    updatePrjProject            : updatePrjProject,
    updatePrjProjectStatus      : updatePrjProjectStatus,
    removePrjProject            : removePrjProject,
    followingPrjProject         : followingPrjProject,
    getPrjProject               : getPrjProject,
    getPrjProjects              : getPrjProjects,
    getPrjTaskFiles             : getPrjTaskFiles,
    getPrjProjectFolder         : getPrjProjectFolder,
    getPrjSelfProjects          : getPrjSelfProjects,
    getPrjFollowProjects        : getPrjFollowProjects,
    getPrjTeam                  : getPrjTeam,
    updatePrjTeam               : updatePrjTeam,
    addPrjProjectTeamPerson     : addPrjProjectTeamPerson,
    removePrjProjectTeamPerson  : removePrjProjectTeamPerson,
    getPrjProjectTeamPersons    : getPrjProjectTeamPersons,
    getPrjProjectFiles          : getPrjProjectFiles,
    updatePrjTime               : updatePrjTime,
    removePrjTime               : removePrjTime,

    getPrjActivities            : getPrjActivities,
    createDocUploadFile : createDocUploadFile,
    addDocFile          : addDocFile,
    getDocFile          : getDocFile,
    addDocFolder        : addDocFolder,
    getDocFolder        : getDocFolder,

    createCrmUploadFile : createCrmUploadFile,

    addCrmContactInfo       : addCrmContactInfo,
    updateCrmContactInfo    : updateCrmContactInfo,
    addCrmContactTwitter    : addCrmContactTwitter,
    addCrmEntityNote        : addCrmEntityNote,

    addCrmContact           : addCrmContact,
    addCrmCompany           : addCrmCompany,
    updateCrmCompany        : updateCrmCompany,
    addCrmPerson            : addCrmPerson,
    updateCrmPerson         : updateCrmPerson,
    addCrmContactData       : addCrmContactData,
    updateCrmContactData    : updateCrmContactData,
    removeCrmContact        : removeCrmContact,
    addCrmTag               : addCrmTag,
    addCrmEntityTag         : addCrmEntityTag,
    removeCrmTag            : removeCrmTag,
    removeCrmEntityTag      : removeCrmEntityTag,
    removeCrmUnusedTag      : removeCrmUnusedTag,
    addCrmCustomField       : addCrmCustomField,
    updateCrmCustomField    : updateCrmCustomField,
    removeCrmCustomField    : removeCrmCustomField,
    addCrmDealMilestone     : addCrmDealMilestone,
    updateCrmDealMilestone  : updateCrmDealMilestone,
    removeCrmDealMilestone  : removeCrmDealMilestone,
    addCrmContactStatus     : addCrmContactStatus,
    updateCrmContactStatus  : updateCrmContactStatus,
    removeCrmContactStatus  : removeCrmContactStatus,
    addCrmListItem          : addCrmListItem,
    updateCrmListItem       : updateCrmListItem,
    removeCrmListItem       : removeCrmListItem,
    removePrjTask           : removePrjTask,
    addCrmTask              : addCrmTask,
    getCrmTask              : getCrmTask,
    addCrmEntityMember      : addCrmEntityMember,
    removeCrmEntityMember   : removeCrmEntityMember,
    updateCrmTask           : updateCrmTask,
    removeCrmTask           : removeCrmTask,
    getCrmCases             : getCrmCases,
    getCrmContacts          : getCrmContacts,
    getCrmContact           : getCrmContact,
    getCrmTags              : getCrmTags,
    getCrmEntityMembers     : getCrmEntityMembers,
    getCrmContactTasks      : getCrmContactTasks,
    getCrmTasks             : getCrmTasks,
    getCrmOpportunities     : getCrmOpportunities,
    removeCrmOpportunity    : removeCrmOpportunity,
    addCrmHistoryEvent      : addCrmHistoryEvent,
    removeCrmHistoryEvent   : removeCrmHistoryEvent,
    getCrmHistoryEvents     : getCrmHistoryEvents,
    removeCrmFile           : removeCrmFile,
    getCrmFolder            : getCrmFolder,
    updateCrmContactRights  : updateCrmContactRights,
    addCrmEntityFiles       : addCrmEntityFiles,
    removeCrmEntityFiles    : removeCrmEntityFiles,
    getCrmEntityFiles       : getCrmEntityFiles,
    getCrmTaskCategories    : getCrmTaskCategories,
    getCrmHistoryCategories : getCrmHistoryCategories,

    addCrmEntityTaskTemplateContainer    : addCrmEntityTaskTemplateContainer,
    updateCrmEntityTaskTemplateContainer : updateCrmEntityTaskTemplateContainer,
    removeCrmEntityTaskTemplateContainer : removeCrmEntityTaskTemplateContainer,
    getCrmEntityTaskTemplateContainer    : getCrmEntityTaskTemplateContainer,
    getCrmEntityTaskTemplateContainers   : getCrmEntityTaskTemplateContainers,
    addCrmEntityTaskTemplate             : addCrmEntityTaskTemplate,
    updateCrmEntityTaskTemplate          : updateCrmEntityTaskTemplate,
    removeCrmEntityTaskTemplate          : removeCrmEntityTaskTemplate,
    getCrmEntityTaskTemplate             : getCrmEntityTaskTemplate,
    getCrmEntityTaskTemplates            : getCrmEntityTaskTemplates,

    getWebItemSecurityInfo  : getWebItemSecurityInfo,
    setWebItemSecurity      : setWebItemSecurity,
    setProductAdministrator : setProductAdministrator,
    isProductAdministrator  : isProductAdministrator
  };
})();
