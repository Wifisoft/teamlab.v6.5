
;window.ServiceManager = (function (helper) {
  var
    ADD = 'post',
    UPDATE = 'put',
    REMOVE = 'delete',
    GET = 'get',
    isInit = false;

  function isArray (o) {
   return o ? o.constructor.toString().indexOf("Array") != -1 : false;
  }

  function getQuery (o) {
    return o && typeof o === 'object' && o.hasOwnProperty('query') ? o.query : null;
  }

  function getContentDisposition (file) {
    return !file ? file : [
      'attachment',
      'filename==?UTF-8?B?' + Base64.encode(file) + '?='
      //'modification-date=' + new Date().toDateString() + ' ' + new Date().toTimeString()
    ].join(';');
  }

  /* <common> */
  var getQuotas = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'settings/quota.json',
      null,
      options
    );
  };
  /* </common> */

  /* <people> */
  var getProfile = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'people/' + id + '.json',
      null,
      options
    );
  };

  var getProfiles = function (eventname, params, options) {
    var query = getQuery(options);
    return helper.request(
      eventname,
      params,
      GET,
      'people' + (query ? '/@search/' + query : '') + '.json',
      null,
      options
    );
  };

  var getGroup = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'group/' + id + '.json',
      null,
      options
    );
  };

  var getGroups = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'group.json',
      null,
      options
    );
  };
  /* </people> */

  /* <community> */
  var addCmtBlog = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'blog.json',
      data,
      options
    );
    return true;
  };

  var getCmtBlog = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'blog/' + id + '.json',
      null,
      options
    );
  };

  var getCmtBlogs = function (eventname, params, options) {
    var query = getQuery(options);
    return helper.request(
      eventname,
      params,
      GET,
      'blog' + (query ? '/@search/' + query : '') + '.json',
      null,
      options
    );
  };

  var addCmtForumTopic = function (eventname, params, threadid, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'forum/' + threadid + '.json',
      data,
      options
    );
    return true;
  };

  var getCmtForumTopic = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'forum/topic/' + id + '.json',
      null,
      options
    );
  };

  var getCmtForumTopics = function (eventname, params, options) {
    var query = getQuery(options);
    return helper.request(
      eventname,
      params,
      GET,
      'forum' + (query ? '/@search/' + query : '/topic/recent') + '.json',
      null,
      options
    );
  };

  var getCmtForumCategories = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'forum.json',
      null,
      options
    );
  };

  var addCmtEvent = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'event.json',
      data,
      options
    );
    return true;
  };

  var getCmtEvent = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'event/' + id + '.json',
      null,
      options
    );
  };

  var getCmtEvents = function (eventname, params, options) {
    var query = getQuery(options);
    return helper.request(
      eventname,
      params,
      GET,
      'event' + (query ? '/@search/' + query : '') + '.json',
      null,
      options
    );
  };

  var addCmtBookmark = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'bookmark.json',
      data,
      options
    );
    return true;
  };

  var getCmtBookmark = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'bookmark/' + id + '.json',
      null,
      options
    );
  };

  var getCmtBookmarks = function (eventname, params, options) {
    var query = getQuery(options);
    return helper.request(
      eventname,
      params,
      GET,
      'bookmark' + (query ? '/@search/' + query : '/top/recent') + '.json',
      null,
      options
    );
  };

  var addCmtForumTopicPost = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'forum/topic/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var addCmtBlogComment = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'blog/' + id + '/comment.json',
      data,
      options
    );
    return true;
  };

  var getCmtBlogComments = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'blog/' + id + '/comment.json',
      null,
      options
    );
  };

  var addCmtEventComment = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'event/' + id + '/comment.json',
      data,
      options
    );
    return true;
  };

  var getCmtEventComments = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'event/' + id + '/comment.json',
      null,
      options
    );
  };

  var addCmtBookmarkComment = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'bookmark/' + id + '/comment.json',
      data,
      options
    );
    return true;
  };

  var getCmtBookmarkComments = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'bookmark/' + id + '/comment.json',
      null,
      options
    );
  };
  /* </community> */

  /* <projects> */
  var getPrjTags = function(eventname, params, options) {
      return helper.request(
      eventname,
      params,
      'get',
      'project/tag.json',
      null,
      options
    );
  };

  var getPrjTagsByName = function(eventname, params, name, data, options) {
      return helper.request(
      eventname,
      params,
      'get',
      'project/tag/search.json',
      data,
      options
    );
  };

  var addPrjSubtask = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'project/task/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var updatePrjSubtask = function (eventname, params, parentid, id, data, options) {
    var updateStatus = false;
    for (var fld in data) {
      if (data.hasOwnProperty(fld)) {
        switch (fld) {
          case 'status' :
            updateStatus = true;
            break;
        }
      }
    }

    helper.request(
      eventname,
      params,
      UPDATE,
      'project/task/' + parentid + '/' + id + (updateStatus ? '/status' : '') + '.json',
      data,
      options
    );
    return true;
  };

  var removePrjSubtask = function (eventname, params, parentid, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/task/' + parentid + '/' + id + '.json',
      id,
      options
    );
    return true;
  };

  var addPrjTask = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'project/' + id + '/task.json',
      data,
      options
    );
    return true;
  };

  var updatePrjTask = function (eventname, params, id, data, options) {
    var updateStatus = false;
    for (var fld in data) {
      if (data.hasOwnProperty(fld)) {
        switch (fld) {
          case 'status' :
            updateStatus = true;
            break;
        }
      }
    }

    helper.request(
      eventname,
      params,
      UPDATE,
      'project/task/' + id + (updateStatus ? '/status' : '') + '.json',
      data,
      options
    );
    return true;
  };

  var removePrjTask = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/task/' + id + '.json',
      id,
      options
    );
    return true;
  };

  var getPrjTask = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/task/' + id + '.json',
      null,
      options
    );
  };

  var getPrjTasks = function (eventname, params, id, type, status, options) {
    if (!id || !type || !status || !options) {
      var filter = null, _id = null, _type = null, _status = null, _options = null;
      for (var i = 2, n = arguments.length; i < n; i++) {
        switch (arguments[i]) {
          case '@all' :
          case '@self' :
            _type = _type || arguments[i];
            break;
          case 'notaccept' :
          case 'open' :
          case 'closed' :
          case 'disable' :
          case 'unclassified' :
          case 'notinmilestone' :
            _status = _status || arguments[i];
            break;
          default :
            _options = _options || (typeof arguments[i] === 'function' || typeof arguments[i] === 'object' ? arguments[i] : _options);
            _id = _id || (isFinite(+arguments[i]) ? +arguments[i] : _id);
            break;
        }
      }

      options = _options;
      status = _status;
      type = _type;
      id = _id;
    }
    if (options && typeof options === 'object' && options.hasOwnProperty('filter')) {
      filter = options.filter;
    }

    return helper.request(
      eventname,
      params,
      GET,
      'project' + (id ? '/' + id : '') + '/task' + (type ? '/' + type : '') + (status ? '/' + status : '') + (filter ? '/filter' : '') + '.json',
      null,
      options
    );
  };

  var getPrjTaskFiles = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/task/' + id + '/files.json',
      null,
      options
    );
  };

  var getPrjTeam = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/' + id + '/team.json',
      null,
      options
    );
  };

  var updatePrjTeam = function(eventname, params, id, data, options) {
    return helper.request(
      eventname,
      params,
      UPDATE,
      'project/' + id + '/team.json',
      data,
      options
    );
  };

  var getPrjProjectFolder = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/' + id + '/files.json',
      null,
      options
    );
  };

  var addPrjEntityFiles = function (eventname, params, id, type, data, options) {
    options = options || {};
    if (typeof options === 'function') {
      options = {success : options};
    }
    if (!options.hasOwnProperty('filter')) {
      options.filter = {};
    }
    if (!options.filter.hasOwnProperty('entityType')) {
      options.filter.entityType = type;
    }

    helper.request(
      eventname,
      params,
      ADD,
      'project/' + id + '/entityfiles.json',
      isArray(data) ? {files : data} : data,
      options
    );
    return true;
  };

  var removePrjEntityFiles = function (eventname, params, id, type, data, options) {
    options = options || {};
    if (typeof options === 'function') {
      options = {success : options};
    }
    if (!options.hasOwnProperty('filter')) {
      options.filter = {};
    }
    if (!options.filter.hasOwnProperty('entityType')) {
      options.filter.entityType = type;
    }

    if (data && typeof data === 'object' && !data.hasOwnProperty('entityType')) {
      data.entityType = type;
    }

    helper.request(
      eventname,
      params,
      REMOVE,
      'project/' + id + '/entityfiles.json',
      typeof data === 'number' || typeof data === 'string' ? {entityType : type, fileid : data} : data,
      options
    );
    return true;
  };

  var getPrjEntityFiles = function (eventname, params, id, type, options)  {
    options = options || {};
    if (typeof options === 'function') {
      options = {success : options};
    }
    if (!options.hasOwnProperty('filter')) {
      options.filter = {};
    }
    if (!options.filter.hasOwnProperty('entityType')) {
      options.filter.entityType = type;
    }

    return helper.request(
      eventname,
      params,
      GET,
      'project/' + id + '/entityfiles.json',
      null,
      options
    );
  };

  var addPrjMilestone = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'project/' + id + '/milestone.json',
      data,
      options
    );
    return true;
  };

  var updatePrjMilestone = function (eventname, params, id, data, options) {
    var
      fldInd = 0,
      updateItem = null;
    for (var fld in data) {
      if (data.hasOwnProperty(fld)) {
        fldInd++;
        switch (fld) {
          case 'status' :
            updateItem = 'status';
            break;
        }
      }
    }
    if (fldInd > 1) {
      updateItem = null;
    }

    helper.request(
      eventname,
      params,
      UPDATE,
      'project/milestone/' + id + (updateItem ? '/' + updateItem : '') + '.json',
      data,
      options
    );
    return true;
  };

  var removePrjMilestone = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/milestone/' + id + '.json',
      id,
      options
    );
    return true;
  };

  var getPrjMilestone = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/milestone/' + id + '.json',
      null,
      options
    );
  };

  var getPrjMilestones = function (eventname, params, id, options) {
    var type = null;

    var _id = id, _type = type;
    switch (id) {
      case 'late' :
        _type = id;
        _id = null;
        break;
    }

    if (id instanceof Date) {
      _type = id.getFullYear() + '/' + (id.getMonth() + 1);
      _id = null;
    }

    id = _id;
    type = _type;

    if (options && typeof options === 'object' && options.hasOwnProperty('filter')) {
      var filter = options.filter;
      for (var fld in filter) {
        switch (fld) {
          case 'participant' :
          case 'tag' :
          case 'projectId' :
          case 'status':
          case 'deadlineStart' :
          case 'deadlineStop' :
          case 'sortBy' :
          case 'sortOrder':
            type = type || 'filter';
            break;
        }
      }
    }

    return helper.request(
      eventname,
      params,
      GET,
      'project' + (id ? '/' + id : '') + '/milestone' + (type ? '/' + type : '') + '.json',
      null,
      options
    );
  };

  var addPrjDiscussion = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'project/' + id + '/message.json',
      data,
      options
    );
    return true;
  };

  var updatePrjDiscussion = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'project/message/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var removePrjDiscussion = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/message/' + id + '.json',
      id,
      options
    );
    return true;
  };

  var getPrjDiscussion = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/message/' + id + '.json',
      null,
      options
    );
  };

  var getPrjDiscussions = function (eventname, params, id, options) {
    var type = null;
    if (options && typeof options === 'object' && options.hasOwnProperty('filter')) {
      var filter = options.filter;
      for (var fld in filter) {
        switch (fld) {
          case 'participant' :
          case 'tag' :
          case 'projectId' :
          case 'sortBy' :
          case 'sortOrder' :
            type = type || 'filter';
            break;
        }
      }
    }

    return helper.request(
      eventname,
      params,
      GET,
      'project' + (id ? '/' + id : '') + '/message' + (type ? '/' + type : '') + '.json',
      null,
      options
    );
  };

  var addPrjProject = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'project.json',
      data,
      options
    );
    return true;
  };

  var updatePrjProject = function (eventname, params, id, data, options) {
    var
      fldInd = 0,
      updateItem = null;
    for (var fld in data) {
      if (data.hasOwnProperty(fld)) {
        fldInd++;
        switch (fld) {
          case 'tags' :
            updateItem = 'tag';
            break;
          case 'status' :
            updateItem = 'status';
            break;
        }
      }
    }
    if (fldInd > 1) {
      updateItem = null;
    }

    helper.request(
      eventname,
      params,
      UPDATE,
      'project/' + id + (updateItem ? '/' + updateItem : '') + '.json',
      data,
      options
    );
    return true;
  };

  var updatePrjProjectStatus = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'project/' + id + '/status.json',
      data,
      options
    );
    return true;
  };

  var removePrjProject = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/' + id + '.json',
      id,
      options
    );
    return true;
  };

  var followingPrjProject = function(eventname, params, id, data, options) {
    return helper.request(
      eventname,
      params,
      UPDATE,
      'project/' + id + '/follow.json',
      data,
      options
    );
  };

  var getPrjProject = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/' + id + '.json',
      null,
      options
    );
  };

  var getPrjProjects = function (eventname, params, type, options) {
    if (arguments.length < 4) {
      options = type;
      type = null;
    }

    var filter = null;
    if (options && typeof options === 'object' && options.hasOwnProperty('filter')) {
      filter = options.filter;
      for (var fld in filter) {
        switch (fld) {
          case 'tag' :
          case 'follow' :
          case 'status' :
          case 'participant' :
            type = type || 'filter';
            break;
        }
      }
    }

    var query = getQuery(options);

    return helper.request(
      eventname,
      params,
      GET,
      'project' + (type ? '/' + type : '') + (query ? '/@search/' + query : '') + '.json',
      null,
      options
    );
  };

  var getPrjSelfProjects = function (eventname, params, options) {
    return getPrjProjects(eventname, params, '@self', options);
  };

  var getPrjFollowProjects = function (eventname, params, options) {
    return getPrjProjects(eventname, params, '@follow', options);
  };

  var addPrjProjectRequest = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'api/1.0/project/request.json',
      data,
      options
    );
  };

  var updatePrjProjectRequest = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'api/1.0/project/' + id + '/request.json',
      data,
      options
    );
  };

  var removePrjProjectRequest = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'api/1.0/project/' + id + '/request.json',
      null,
      options
    );
  };

  var updatePrjComment = function (eventname, params, id, data, options) {
    if (!data.parentid) {
      data.parentid = '00000000-0000-0000-0000-000000000000';
    }
    if (!data.hasOwnProperty('text') && data.hasOwnProperty('content')) {
      data.text = data.content;
    }

    helper.request(
      eventname,
      params,
      UPDATE,
      'project/comment/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var removePrjComment = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/comment/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var addPrjTaskComment = function (eventname, params, id, data, options) {
    if (!data.parentid) {
      data.parentid = '00000000-0000-0000-0000-000000000000';
    }

    helper.request(
      eventname,
      params,
      ADD,
      'project/task/' + id + '/comment.json',
      data,
      options
    );
    return true;
  };

  var getPrjTaskComments = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/task/' + id + '/comment.json',
      null,
      options
    );
  };

  var addPrjDiscussionComment = function (eventname, params, id, data, options) {
    if (!data.parentid) {
      data.parentid = '00000000-0000-0000-0000-000000000000';
    }

    helper.request(
      eventname,
      params,
      ADD,
      'project/message/' + id + '/comment.json',
      data,
      options
    );
    return true;
  };

  var getPrjDiscussionComments = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/message/' + id + '/comment.json',
      null,
      options
    );
  };

  var addPrjMilestoneComment = function (eventname, params, id, data, options) {
    if (!data.parentid) {
      data.parentid = '00000000-0000-0000-0000-000000000000';
    }

    helper.request(
      eventname,
      params,
      ADD,
      'project/milestone/' + id + '/comment.json',
      data,
      options
    );
    return true;
  };

  var getPrjMilestoneComments = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/milestone/' + id + '/comment.json',
      null,
      options
    );
  };

  var addPrjProjectTeamPerson = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'project/' + id + '/team.json',
      data,
      options
    );
    return true;
  };

  var removePrjProjectTeamPerson = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'project/' + id + '/team.json',
      data,
      options
    );
    return true;
  };

  var getPrjProjectTeamPersons = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/' + id + '/team.json',
      null,
      options
    );
  };

  var getPrjProjectFiles = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'project/' + id + '/entityfiles.json',
      null,
      options
    );
  };
  
  // time-traking

  var updatePrjTime = function(eventname, params, id, data, options) {
      helper.request(
      eventname,
      params,
      UPDATE,
      'project/time/' + id + '.json',
      data,
      options
    );
    return true;
  };
	
  var removePrjTime = function(eventname, params, id, options) {
      helper.request(
      eventname,
      params,
      REMOVE,
      'project/time/' + id + '.json',
      id,
      options
    );
    return true;
  };

    
  //activities
  var getPrjActivities = function(eventname, params, options) {
      return helper.request(
      eventname,
      params,
      GET,
      'project/activities/filter.json',
      null,
      options
    );
  };
    
  /* </projects> */

  /* <documents> */
  var createDocUploadFile = function (eventname, params, id, data, options) {
    return helper.uploader(
      eventname,
      params,
      'file',
      'files/' + id + '/upload.tml',
      data,
      options
    );
  };

  var addDocFile = function (eventname, params, id, type, data, options) {
    options = options || {};
    if (typeof options === 'function') {
      options = {success : options};
    }
    if (!options.hasOwnProperty('headers')) {
      options.headers = [];
    }
    options.headers.push({header : 'Content-Disposition', value : getContentDisposition(data.name || null)});

    helper.request(
      eventname,
      params,
      ADD,
      'files/' + id + (type ? '/' + type : '') + '.json',
      data,
      options
    );
    return true;
  };

  var getDocFile = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'files/file/' + id + '.json',
      null,
      options
    );
  };

  var addDocFolder = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'files/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var getDocFolder = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'files/' + id + '.json',
      null,
      options
    );
  };
  /* </documents> */

  /* <crm> */
  var createCrmUploadFile = function (eventname, params, type, id, data, options) {
    return helper.uploader(
      eventname,
      params,
      'file',
      'crm/' + type + '/' + id + '/files/upload.tml',
      data,
      options
    );
  };

  var addCrmContactInfo = function (eventname, params, contactid, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/' + contactid + '/data.json',
      data,
      options
    );
    return true;
  };

  var updateCrmContactInfo = function (eventname, params, contactid, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/contact/' + contactid + '/data/' + data.id + '.json',
      data,
      options
    );
    return true;
  };

  var addCrmContactTwitter = function (eventname, params, contactid, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/' + contactid + '/data.json',
      data,
      options
    );
    return true;
  };

  var addCrmEntityNote = function (eventname, params, type, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/' + type + '/' + id + '/files/text.json',
      data,
      options
    );
    return true;
  };

  var addCrmContact = function (eventname, params, data, options) {
    return false;
  };

  var addCrmCompany = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/company' + (isArray(data) ? '/quick' : '') + '.json',
      isArray(data) ? {data : data} : data,
      options
    );
    return true;
  };
  
  var updateCrmCompany = function (eventname, params, id, data, options) {
    helper.request(
        eventname,
        params,
        UPDATE,
        'crm/contact/company/' + id + '.json',
        data,
        options
      );    
    return true;
  };

  var addCrmPerson = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/person' + (isArray(data) ? '/quick' : '') + '.json',
      isArray(data) ? {data : data} : data,
      options
    );
    return true;
  };
  
  var updateCrmPerson = function (eventname, params, id, data, options) {  
    helper.request(        
        eventname,
        params,
        UPDATE,
        'crm/contact/person/' + id + '.json',
        data,
        options
      );    
    return true;
  };

  var addCrmContactData = function (eventname, params, id, data, options) {        
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/' + id + '/batch.json',
      isArray(data) ? {data : data} : data,
      options
    );
    return true;
  };

  var updateCrmContactData = function (eventname, params, id, data, options) {        
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/contact/' + id + '/batch.json',
      isArray(data) ? {data : data} : data,
      options
    );
    return true;
  };

  var removeCrmContact = function (eventname, params, ids, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/contact' + (ids && (typeof ids === 'number' || typeof ids === 'string') ? '/' + ids : '') + '.json',
      ids && typeof ids === 'object' ? {contactids : ids} : null,
      options
    );
    return true;
  };

  var addCrmTag = function (eventname, params, type, ids, tagname, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/' + type + (typeof ids === 'object' ? '/taglist' : '/' + ids + '/tag') + '.json',
      {entityid : ids, tagName : tagname},
      options
    );
    return true;
  };

  var removeCrmTag = function (eventname, params, type, id, tagname, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/' + type + '/' + id + '/tag/' + tagname + '.json',
      null,
      options
    );
    return true;
  };

  var getCrmTags = function (eventname, params, type, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/' + type + '/' + id + '/tag.json',
      null,
      options
    );
  };

  var addCrmEntityTag = function (eventname, params, type, tagname, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/' + type + '/tag.json',
      {entityType : type, tagName : tagname},
      options
    );
    return true;
  };

  var removeCrmEntityTag = function (eventname, params, type, tagname, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/' + type + '/tag.json',
      {tagName: tagname},
      options
    );
    return true;
  };

  var removeCrmUnusedTag = function (eventname, params, type, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/' + type + '/tag/unused.json',
      null,
      options
    );
    return true;
  };

  var addCrmCustomField = function (eventname, params, type, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/' + type + '/customfield.json',
      data,
      options
    );
    return true;
  };

  var updateCrmCustomField = function (eventname, params, type, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/' + type + '/customfield/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var removeCrmCustomField = function (eventname, params, type, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/' + type + '/customfield/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var addCrmDealMilestone = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/opportunity/stage.json',
      data,
      options
    );
    return true;
  };

  var updateCrmDealMilestone = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/opportunity/stage/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var removeCrmDealMilestone = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/opportunity/stage/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var addCrmContactStatus = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/type.json',
      data,
      options
    );
    return true;
  };

  var updateCrmContactStatus = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/contact/type/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var removeCrmContactStatus = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/contact/type/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var addCrmListItem = function (eventname, params, type, data, options) {
    if (type === 1)//ContactStatus
    {
      helper.request(
        eventname,
        params,
        ADD,
        'crm/contact/type.json',
        data,
        options
      );
      return true;
    }
    if (type === 2)//TaskCategory
    {
      helper.request(
        eventname,
        params,
        ADD,
        'crm/task/category.json',
        data,
        options
      );
      return true;
    }
    if (type === 3)//HistoryCategory
    {
      helper.request(
        eventname,
        params,
        ADD,
        'crm/history/category.json',
        data,
        options
      );
      return true;
    }
  return false;
  };

  var updateCrmListItem = function (eventname, params, type, id, data, options) {
    if (type === 1)//ContactStatus
    {
      helper.request(
        eventname,
        params,
        UPDATE,
        'crm/contact/type/' + id + '.json',
        data,
        options
      );
      return true;
    }
    if (type === 2)//TaskCategory
    {
      helper.request(
        eventname,
        params,
        UPDATE,
        'crm/task/category/' + id + '.json',
        data,
        options
      );
      return true;
    }
    if (type === 3)//HistoryCategory
    {
      helper.request(
        eventname,
        params,
        UPDATE,
        'crm/history/category/' + id + '.json',
        data,
        options
      );
      return true;
    }
  return false;
  };

  var removeCrmListItem = function (eventname, params, type, id, options) {
    if (type === 1)//ContactStatus
    {
      helper.request(
        eventname,
        params,
        REMOVE,
        'crm/contact/type/' + id + '.json',
        null,
        options
      );
      return true;
    }
    if (type === 2)//TaskCategory
    {
      helper.request(
        eventname,
        params,
        REMOVE,
        'crm/task/category/' + id + '.json',
        null,
        options
      );
      return true;
    }
    if (type === 3)//HistoryCategory
    {
      helper.request(
        eventname,
        params,
        REMOVE,
        'crm/history/category/' + id + '.json',
        null,
        options
      );
      return true;
    }
  return false;
  };

  var addCrmTask = function (eventname, params, data, options) {    
    helper.request(
      eventname,
      params,
      ADD,
      'crm/task.json',
      data,
      options
    );
    return true;
  };

  var getCrmTask = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/task/' + id + '.json',
      null,
      options
    );
  };

  var updateCrmTask = function (eventname, params, id, data, options) {
    var isUpdateStatusAction = data.hasOwnProperty('isClosed');

    if (isUpdateStatusAction) {
      helper.request(
        eventname,
        params,
        UPDATE,
        !!data.isClosed ? 'crm/task/' + id + '/close.json' : 'crm/task/' + id + '/reopen.json',
        data,
        options
      );
    } else {
      helper.request(
        eventname,
        params,
        UPDATE,
        'crm/task/' + id + '.json',
        data,
        options
      );
    }

    return true;
  };

  var removeCrmTask = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/task/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var addCrmContactMember = function (eventname, params, type, entityid, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/' + type + '/' + entityid + '/contact' + (type === 'opportunity' ? '/' + id : '') + '.json',
      data,
      options
    );
  };

  var removeCrmContactMember = function (eventname, params, type, entityid, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/' + type + '/' + entityid + '/contact/' + id + '.json',
      null,
      options
    );
  };

  var getCrmContactMembers = function (eventname, params, type, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/' + type + '/' + id + '/contact.json',
      null,
      options
    );
  };

  var addCrmPersonMember = function (eventname, params, type, entityid, id, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/' + type + '/' + entityid + '/person.json',
      data,
      options
    );
  };

  var removeCrmPersonMember = function (eventname, params, type, entityid, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/contact/' + type + '/' + entityid + '/person.json',
      {personid : id},
      options
    );
  };

  var getCrmPersonMembers = function (eventname, params, type, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/contact/' + type + '/' + id + '/person.json',
      null,
      options
    );
  };

  var getCrmContactTasks = function (eventname, params, data, options) {
    return helper.request(
      eventname,
      params,
      ADD,
      'crm/contact/task/near.json',
      typeof data === 'number' || typeof data === 'string' ? {contactid : [data]} : data,
      options
    );
  };

  var getCrmCases = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/case/filter.json',
      null,
      options
    );
  };

  var getCrmContacts = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/contact/filter.json',
      null,
      options
    );
  };

  var getCrmContact = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/contact/' + id + '.json',
      null,
      options
    );
  };

  var getCrmTasks = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/task/filter.json',
      null,
      options
    );
  };

  var getCrmOpportunities = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/opportunity/filter.json',
      null,
      options
    );
  };

  var removeCrmOpportunity = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/opportunity/' + id + '.json',
      null,
      options
    );
  };

  var addCrmHistoryEvent = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/history.json',
      data,
      options
    );
  };

  var removeCrmHistoryEvent = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/history/' + id + '.json',
      null,
      options
    );
  };

  var getCrmHistoryEvents = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/history/filter.json',
      null,
      options
    );
  };

  var removeCrmFile = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/files/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var getCrmFolder = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/files/' + id + '.json',
      null,
      options
    );
  };

  var updateCrmContactRights = function (eventname, params, id, data, options) {
    if (!data || !options) {
      options = data;
      data = id;
      id = null;
    }

    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/contact' + (id ? '/' + id : '') + '/access.json',
      data,
      options
    );
    return true;
  };

  var addCrmEntityFiles = function (eventname, params, id, type, data, options) {
    if (data && typeof data === 'object' && !data.hasOwnProperty('entityType')) {
      data.entityType = type;
    }

    helper.request(
      eventname,
      params,
      ADD,
      'crm' + (type ? '/' + type : '') + '/' + id + '/files.json',
      isArray(data) ? {entityType: type, entityid: id, fileids : data} : data,
      options
    );
    return true;
  };

  var removeCrmEntityFiles = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/files/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var getCrmEntityFiles = function (eventname, params, id, type, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm' + (type ? '/' + type : '') + '/' + id + '/files.json',
      null,
      options
    );
  };

  var getCrmTaskCategories = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/task/category.json',
      null,
      options
    );
  };

  var getCrmHistoryCategories = function (eventname, params, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/history/category.json',
      null,
      options
    );
  };

  var addCrmEntityTaskTemplateContainer = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/' + data.entityType + '/tasktemplatecontainer.json',
      data,
      options
    );
    return true;
  };

  var updateCrmEntityTaskTemplateContainer = function (eventname, params, id, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/tasktemplatecontainer/' + id + '.json',
      data,
      options
    );
    return true;
  };

  var removeCrmEntityTaskTemplateContainer = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/tasktemplatecontainer/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var getCrmEntityTaskTemplateContainer = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/tasktemplatecontainer/' + id + '.json',
      null,
      options
    );
  };

  var getCrmEntityTaskTemplateContainers = function (eventname, params, type, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/' + type + '/tasktemplatecontainer.json',
      null,
      options
    );
  };

  var addCrmEntityTaskTemplate = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      ADD,
      'crm/tasktemplatecontainer/' + data.containerid + '/tasktemplate.json',
      data,
      options
    );
    return true;
  };

  var updateCrmEntityTaskTemplate = function (eventname, params, data, options) {
    helper.request(
      eventname,
      params,
      UPDATE,
      'crm/tasktemplatecontainer/' + data.containerid + '/tasktemplate.json',
      data,
      options
    );
    return true;
  };

  var removeCrmEntityTaskTemplate = function (eventname, params, id, options) {
    helper.request(
      eventname,
      params,
      REMOVE,
      'crm/tasktemplatecontainer/tasktemplate/' + id + '.json',
      null,
      options
    );
    return true;
  };

  var getCrmEntityTaskTemplate = function (eventname, params, id, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/tasktemplatecontainer/tasktemplate/' + id + '.json',
      null,
      options
    );
  };

  var getCrmEntityTaskTemplates = function (eventname, params, containerid, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'crm/tasktemplatecontainer/' + containerid + '/tasktemplate.json',
      null,
      options
    );
  };
  /* </crm> */

  /* <settings> */
  var getWebItemSecurityInfo = function (eventname, params, data, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'settings/security.json',
      typeof data === 'number' || typeof data === 'string' ? {ids : [data]} : data,
      options
    );
  };

  var setWebItemSecurity = function (eventname, params, data, options) {
    return helper.request(
      eventname,
      params,
      UPDATE,
      'settings/security.json',
      data,
      options
    );
  };

  var setProductAdministrator = function (eventname, params, data, options) {
    return helper.request(
      eventname,
      params,
      UPDATE,
      'settings/security/administrator.json',
      data,
      options
    );
  };

  var isProductAdministrator = function (eventname, params, data, options) {
    return helper.request(
      eventname,
      params,
      GET,
      'settings/security/administrator.json',
      data,
      options
    );
  };
  /* </settings> */

  return {
    test    : helper.test,
    init    : helper.init,
    bind    : helper.bind,
    exec    : helper.exec,
    joint   : helper.joint,
    start   : helper.start,
    login   : helper.login,
    logged  : helper.logged,

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

    getPrjTags              : getPrjTags,
    getPrjTagsByName        : getPrjTagsByName,
    addPrjSubtask           : addPrjSubtask,
    updatePrjSubtask        : updatePrjSubtask,
    removePrjSubtask        : removePrjSubtask,
    addPrjTask              : addPrjTask,
    updatePrjTask           : updatePrjTask,
    removePrjTask           : removePrjTask,
    getPrjTask              : getPrjTask,
    getPrjTasks             : getPrjTasks,
    getPrjTaskFiles         : getPrjTaskFiles,
    getPrjProjectFolder     : getPrjProjectFolder,
    addPrjEntityFiles       : addPrjEntityFiles,
    removePrjEntityFiles    : removePrjEntityFiles,
    getPrjEntityFiles       : getPrjEntityFiles,
    addPrjMilestone         : addPrjMilestone,
    updatePrjMilestone      : updatePrjMilestone,
    removePrjMilestone      : removePrjMilestone,
    getPrjMilestone         : getPrjMilestone,
    getPrjMilestones        : getPrjMilestones,
    addPrjDiscussion        : addPrjDiscussion,
    updatePrjDiscussion     : updatePrjDiscussion,
    removePrjDiscussion     : removePrjDiscussion,
    getPrjDiscussion        : getPrjDiscussion,
    getPrjDiscussions       : getPrjDiscussions,
    addPrjProject           : addPrjProject,
    updatePrjProject        : updatePrjProject,
    updatePrjProjectStatus  : updatePrjProjectStatus,
    removePrjProject        : removePrjProject,
    followingPrjProject     : followingPrjProject,
    getPrjProject           : getPrjProject,
    getPrjProjects          : getPrjProjects,
    getPrjSelfProjects      : getPrjSelfProjects,
    getPrjFollowProjects    : getPrjFollowProjects,
    updatePrjTime           : updatePrjTime,
    removePrjTime           : removePrjTime,

    addPrjProjectRequest    : addPrjProjectRequest,
    updatePrjProjectRequest : updatePrjProjectRequest,
    removePrjProjectRequest : removePrjProjectRequest,

    addPrjTaskComment           : addPrjTaskComment,
    updatePrjTaskComment        : updatePrjComment,
    removePrjTaskComment        : removePrjComment,
    getPrjTaskComments          : getPrjTaskComments,
    addPrjDiscussionComment     : addPrjDiscussionComment,
    updatePrjDiscussionComment  : updatePrjComment,
    removePrjDiscussionComment  : removePrjComment,
    getPrjDiscussionComments    : getPrjDiscussionComments,
    addPrjMilestoneComment      : addPrjMilestoneComment,
    updatePrjMilestoneComment   : updatePrjComment,
    removePrjMilestoneComment   : removePrjComment,
    getPrjMilestoneComments     : getPrjMilestoneComments,

    getPrjTeam                  : getPrjTeam,
    updatePrjTeam               : updatePrjTeam,
    addPrjProjectTeamPerson     : addPrjProjectTeamPerson,
    removePrjProjectTeamPerson  : removePrjProjectTeamPerson,
    getPrjProjectTeamPersons    : getPrjProjectTeamPersons,
    getPrjProjectFiles          : getPrjProjectFiles,

    getPrjActivities            : getPrjActivities,
    
    createDocUploadFile : createDocUploadFile,
    addDocFile          : addDocFile,
    getDocFile          : getDocFile,
    addDocFolder        : addDocFolder,
    getDocFolder        : getDocFolder,

    createCrmUploadFile : createCrmUploadFile,

    addCrmContactInfo     : addCrmContactInfo,
    updateCrmContactInfo  : updateCrmContactInfo,
    addCrmContactTwitter  : addCrmContactTwitter,
    addCrmEntityNote      : addCrmEntityNote,

    addCrmContact     : addCrmContact,
    addCrmCompany     : addCrmCompany,
    updateCrmCompany   : updateCrmCompany,
    addCrmPerson      : addCrmPerson,
    updateCrmPerson   : updateCrmPerson,
    addCrmContactData : addCrmContactData,
    updateCrmContactData : updateCrmContactData,
   
    removeCrmContact  : removeCrmContact,

    addCrmTag            : addCrmTag,
    addCrmEntityTag      : addCrmEntityTag,
    removeCrmTag         : removeCrmTag,
    removeCrmEntityTag   : removeCrmEntityTag,
    removeCrmUnusedTag   : removeCrmUnusedTag,
    addCrmCustomField    : addCrmCustomField,
    updateCrmCustomField : updateCrmCustomField,
    removeCrmCustomField : removeCrmCustomField,
    addCrmDealMilestone     : addCrmDealMilestone,
    updateCrmDealMilestone  : updateCrmDealMilestone,
    removeCrmDealMilestone  : removeCrmDealMilestone,
    addCrmContactStatus     : addCrmContactStatus,
    updateCrmContactStatus  : updateCrmContactStatus,
    removeCrmContactStatus  : removeCrmContactStatus,
    addCrmListItem          : addCrmListItem,
    updateCrmListItem       : updateCrmListItem,
    removeCrmListItem       : removeCrmListItem,
    addCrmTask    : addCrmTask,
    getCrmTask    : getCrmTask,
    updateCrmTask : updateCrmTask,
    removeCrmTask : removeCrmTask,

    addCrmPersonMember      : addCrmPersonMember,
    removeCrmPersonMember   : removeCrmPersonMember,
    addCrmContactMember     : addCrmContactMember,
    removeCrmContactMember  : removeCrmContactMember,

    getCrmTags              : getCrmTags,
    getCrmContactMembers    : getCrmContactMembers,
    getCrmPersonMembers     : getCrmPersonMembers,
    getCrmContactTasks      : getCrmContactTasks,
    getCrmCases             : getCrmCases,
    getCrmContacts          : getCrmContacts,
    getCrmContact           : getCrmContact,
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
})(ServiceHelper);
