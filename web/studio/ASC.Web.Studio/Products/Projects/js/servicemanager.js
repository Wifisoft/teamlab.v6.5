if (typeof window.serviceManager === 'undefined') {
    window.serviceManager = (function($) {
        var 
      isInit = false,
      servicePath = '',
      myGUID = '',
      cmdSeparator = '/',
      unreadMessages = null,
      requestTimeout = 60 * 1000,
      projectId = '',
      Count = 30,
      StartIndex = 0,
      filter = {},
      customEvents = {};

        var getRandomId = function(prefix) {
            return (typeof prefix !== 'undefined' ? prefix + '-' : '') + Math.floor(Math.random() * 1000000);
        };

        var getUniqueId = function(o, prefix) {
            var 
        iterCount = 0,
        maxIterations = 1000,
        uniqueId = getRandomId(prefix);
            while (o.hasOwnProperty(uniqueId) && iterCount++ < maxIterations) {
                uniqueId = getRandomId(prefix);
            }
            return uniqueId;
        };

        var jsonToXml = function(o, parent) {
            var 
        attr = '',
        xml = '';

            if (typeof o === 'object') {
                if (o instanceof Array) {
                    if (parent == 'int' || parent == 'string') {
                        attr = ' xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays"';
                    }
                    space = '';
                    if (parent == 'a') space = ':string';

                    for (var i = 0, n = o.length; i < n; i++) {
                        xml += '<' + parent + space + attr + '>' + o[i] + '</' + parent + space + '>';
                    }
                } else {
                    for (var i in o) {
                        if (i.charAt(0) === '_') {
                            attr += ' ' + i.substring(1) + '="' + o[i] + '"';
                        } else {
                            xml += arguments.callee(o[i], i);
                        }
                    }
                    if (typeof parent !== 'undefined') {
                        if (parent == 'MailContactInfo' || parent == 'MailLabel') attr = ' xmlns:i="http://www.w3.org/2001/XMLSchema-instance"';
                        if (parent == 'addresses') attr = ' xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays"';
                        xml = '<' + parent + attr + '>' + xml + '</' + parent + '>';
                    }
                }
            } else if (typeof o === 'string') {
                xml = o;
                if (typeof parent !== 'undefined') {
                    if (!((parent == 'Importance' || parent == 'Unread' || parent == 'PrimaryFolder') && xml.length == 0))
                        xml = '<' + parent + attr + '>' + xml + '</' + parent + '>';
                }
            } else if (typeof o !== 'undefined' && typeof o.toString !== 'undefined') {
                xml = o.toString();
                if (typeof parent !== 'undefined') {
                    xml = '<' + parent + attr + '>' + xml + '</' + parent + '>';
                }
            }
            return xml;
        };

        var execCustomEvent = function(eventType, thisArg, argsArray) {
            eventType = eventType.toLowerCase();
            thisArg = thisArg || window;
            argsArray = argsArray || [];

            if (!customEvents.hasOwnProperty(eventType)) {
                return undefined;
            }
            var customEvent = customEvents[eventType];

            for (var eventId in customEvent) {
                if (customEvent.hasOwnProperty(eventId)) {
                    customEvent[eventId].handler.apply(thisArg, argsArray);
                    if (customEvent[eventId].type & 1) {
                        delete customEvent[eventId];
                    }
                }
            }
        };

        var addCustomEvent = function(eventType, handler, params) {
            if (typeof eventType !== 'string' || typeof handler !== 'function') {
                return undefined;
            }

            eventType = eventType.toLowerCase();

            if (typeof params !== 'object') {
                params = {};
            }
            var isOnceExec = params.hasOwnProperty('once') ? params.once : false;

            var handlerType = 0;
            handlerType |= +isOnceExec * 1;  // isOnceExec - выполнить один раз и удалить из очереди

            if (!customEvents.hasOwnProperty(eventType)) {
                customEvents[eventType] = {};
            }

            var eventId = getUniqueId(customEvents[eventType]);

            customEvents[eventType][eventId] = {
                handler: handler,
                type: handlerType
            };

            return eventId;
        };

        var removeCustomEvent = function(eventType, eventId) {
            if (typeof eventType !== 'string' || typeof eventId === 'undefined') {
                return false;
            }

            if (customEvents(eventType) && customEvents[eventType].hasOwnProperty(eventId)) {
                delete userEventHandlers[eventType][eventId];
            }
            return true;
        };

        var getUrl = function() {
            var url = servicePath;
            if (arguments.length === 0) {
                return url;
            }
            if (arguments[0] == '.json') {
                url = url.substr(0, url.length - 1);
            }
            for (var i = 0, n = arguments.length - 1; i < n; i++) {
                url += arguments[i] + cmdSeparator;
            }
            return url + arguments[i];
        };

        var getNodeContent = function(o) {
            if (!o || typeof o !== 'object') {
                return '';
            }
            return o.text || o.textContent || (function(o) {
                var 
          result = '',
          childrens = o.childNodes;
                if (!childrens) {
                    return result;
                }
                for (var i = 0, n = childrens.length; i < n; i++) {
                    var child = childrens.item(i);
                    switch (child.nodeType) {
                        case 1:
                        case 5:
                            result += arguments.callee(child);
                            break;
                        case 3:
                        case 2:
                        case 4:
                            result += child.nodeValue;
                            break;
                        default:
                            break;
                    }
                }
                return result;
            })(o);
        };

        var completeRequest = function(eventType, params, xmlHttpRequest, textStatus) {
            if (textStatus === 'error') {
                var 
          errorType = '',
          errorMessage = '',
          errorComment = '',
          nodeType = null,
          nodeMessage = null,
          nodeInnerMessage = null;
                if (xmlHttpRequest.responseXML) {
                    nodeMessage = xmlHttpRequest.responseXML.getElementsByTagName('message')[0];
                    var nodeInner = xmlHttpRequest.responseXML.getElementsByTagName('inner')[0];
                    if (nodeInner && typeof nodeInner === 'object') {
                        nodeType = nodeInner.getElementsByTagName('type')[0];
                        nodeInnerMessage = nodeInner.getElementsByTagName('message')[0];
                    }
                }
                if (nodeType && typeof nodeType === 'object') {
                    errorType = getNodeContent(nodeType);
                }
                if (nodeMessage && typeof nodeMessage === 'object') {
                    errorMessage = getNodeContent(nodeMessage);
                }
                if (nodeInnerMessage && typeof nodeInnerMessage === 'object') {
                    errorComment = getNodeContent(nodeInnerMessage);
                }
                execCustomEvent(eventType, window, [undefined, params, { type: errorType, message: errorMessage, comment: errorComment}]);
                return undefined;
            }
            //var data = ASC.Controls.XSLTManager.createXML(xmlHttpRequest.responseText);
            var data = xmlHttpRequest.responseText;

            execCustomEvent(eventType, window, [data, params]);
        };

        var getCompleteCallbackMethod = function(eventType, params) {
            return function() {
                var argsArray = [eventType, params];
                for (var i = 0, n = arguments.length; i < n; i++) {
                    argsArray.push(arguments[i]);
                }
                completeRequest.apply(this, argsArray);
            };
        };

        var request = function(type, dataType, eventType, params) {
            if (typeof type === 'undefined' || typeof dataType === 'undefined' || typeof eventType !== 'string') {
                return undefined;
            }

            var 
        data = {},
        argsArray = [];

            if (typeof params !== 'object') {
                params = {};
            }

            switch (type.toLowerCase()) {
                case 'get':
                    data = arguments[4];
                    for (var i = 5, n = arguments.length; i < n; i++) {
                        argsArray.push(arguments[i]);
                    }
                    break;
                case 'delete':
                    for (var i = 4, n = arguments.length; i < n; i++) {
                        argsArray.push(arguments[i]);
                    }
                    break;
                case 'put':
                    data = arguments[4];
                    for (var i = 5, n = arguments.length; i < n; i++) {
                        argsArray.push(arguments[i]);
                    }
                    break;
                case 'post':
                    data = arguments[4];
                    for (var i = 5, n = arguments.length; i < n; i++) {
                        argsArray.push(arguments[i]);
                    }
                    break;
                default:
                    return undefined;
            }

            $.ajax({
                async: true,
                data: data,
                type: type,
                dataType: dataType,
                cache: false,
                url: getUrl.apply(this, argsArray),
                timeout: requestTimeout,
                complete: getCompleteCallbackMethod(eventType, params)
            });
        };


        var init = function(path, GUID, prjId) {
            if (typeof path !== 'string' || path.length === 0) {
                throw 'incorrect service path';
            }
            if (typeof prjId !== 'string') {
                this.projectId = null;
            } else {
                this.projectId = prjId;
            }
            myGUID = GUID;
            if (isInit === false) {
                isInit = true;
                servicePath = path[path.length - 1] === '/' ? path : path + '/';
            }
        };

        var getMyGUID = function() {
            return myGUID;
        };

        var getFilteredTasks = function(eventType, params, filter, StartIndex, lastId) {
            if (filter == null) {
                filter = this.filter;
            }
            filter.Count = this.Count + 1;
            if (typeof StartIndex == 'undefined') {
                filter.StartIndex = 0;
            } else {
                filter.StartIndex = this.filter.StartIndex + StartIndex;
            }
            this.filter = filter;
            if (typeof lastId == 'undefined') filter.lastId = 0;
            else filter.lastId = lastId;
            
            params.eventType = eventType;
            
            Teamlab.getPrjTasks(params, null, null, null, { filter: filter });
        };

        var getOverdueTasks = function(eventType, prjId) {
            var params = {};
            var data = {
                projectId: prjId,
                sortBy: 'title',
                sortOrder: 'ascending',
                status: 'late'
            };
            request('get', 'json', eventType, params, data, 'task', 'filter.json');
        };

        var getTask = function(eventType, taskid) {
            //request('get', 'json', eventType, { 'taskid': taskid }, {}, 'task', taskid + '.json');
            Teamlab.getPrjTask({ 'taskid': taskid, 'eventType': eventType }, taskid);
        };

        var addTask = function(eventType, params, data) {
            //request('post', 'json', eventType, {}, data, this.projectId, 'task.json');
            Teamlab.addPrjTask(params, this.projectId, data);
        };

        var addTaskToProject = function(eventType, params, data, prjId) {
            //request('post', 'json', eventType, {}, data, prjId, 'task.json');
            Teamlab.addPrjTask(params, prjId, data);
        };

        var addSubTask = function(eventType, taskid, data, params) {
            //request('post', 'json', eventType, params, data, 'task', taskid + '.json');
            params.eventType = eventType;
            Teamlab.addPrjSubtask(params, taskid, data);
        };

        var removeSubTask = function(eventType, taskid, subtaskid) {
            //request('delete', 'json', eventType, {}, 'task', taskid, subtaskid + '.json');
            Teamlab.removePrjSubtask({ 'eventType': eventType, 'taskid': taskid }, taskid, subtaskid);
        };

        var removeTask = function(eventType, taskid) {
            //request('delete', 'json', eventType, { 'taskid': taskid }, 'task', taskid + '.json');            
            Teamlab.removePrjTask({ 'taskid': taskid }, taskid);
        };

        var updateTaskStatus = function(eventType, data, taskid) {
            //request('put', 'json', eventType, { 'taskid': taskid, 'status': data.status }, data, 'task', taskid, 'status.json');
            Teamlab.updatePrjTask({ 'taskid': taskid, 'status': data.status, 'eventType': eventType }, taskid, data);
        };

        var updateSubTaskStatus = function(eventType, data, taskid, subtaskid) {
            //request('put', 'json', eventType, { 'taskid': taskid, 'subtaskid': subtaskid }, data, 'task', taskid, subtaskid, 'status.json');
            Teamlab.updatePrjSubtask({ 'eventType': eventType, 'taskid': taskid, 'subtaskid': subtaskid }, taskid, subtaskid, data);
        };

        var updateTask = function(eventType, data, taskid) {
            //request('put', 'json', eventType, { 'taskid': taskid }, data, 'task', taskid + '.json');
            Teamlab.updatePrjTask({ 'taskid': taskid, 'eventType': eventType }, taskid, data);
        };

        var updateSubTask = function(eventType, data, taskid, subtaskid) {
            //request('put', 'json', eventType, { 'taskid': taskid, 'subtaskid': subtaskid }, data, 'task', taskid, subtaskid + '.json');
            Teamlab.updatePrjSubtask({ 'eventType': eventType, 'taskid': taskid, 'subtaskid': subtaskid }, taskid, subtaskid, data);
        };

        var getTeam = function(eventType, params) {
            //request('get', 'json', eventType, params, {}, this.projectId, 'team.json');
            params.eventType = eventType;
            Teamlab.getPrjTeam(params, this.projectId);
        };

        var getTeamByProject = function(eventType, params, prjId) {
            //request('get', 'json', eventType, params, {}, prjId, 'team.json');
            params.eventType = eventType;
            Teamlab.getPrjTeam(params, prjId);
        };

        var getMilestonesByProject = function(params, prjId, success, after) {
            Teamlab.getPrjMilestones(params, null, { filter: { status: 'open', projectId: prjId }, success: success, after: after });
        };

        var messageResponsible = function(eventType, taskid) {
            request('get', 'json', eventType, { 'taskid': taskid }, {}, 'task', taskid, 'notify.json');
        };

        var addTaskTime = function(eventType, data, taskid) {
            request('post', 'json', eventType, { 'taskid': taskid }, data, 'task', taskid, 'time.json');
        };

        var getTimeSpend = function(eventType, taskid) {
            request('get', 'json', eventType, { 'taskid': taskid }, {}, 'task', taskid, 'time.json');
        };

        var getProjects = function(eventType) {
            request('get', 'json', eventType, {}, {}, '@self.json');
        };

        var getAllProjects = function(eventType, params, data) {
            request('get', 'json', eventType, params, data, 'filter.json');
        };

        var getProjectsByFilter = function(eventType, params, data, startIndex, lastId) {
            if (data == null) {
                data = this.filter;
            }
            data.Count = this.Count + 1;
            if (typeof startIndex == 'undefined') {
                data.StartIndex = 0;
            } else {
                data.StartIndex = this.filter.StartIndex + startIndex;
            }
            this.filter = data;
            if (typeof lastId != 'undefined') {
                data.LastId = lastId;
            }
            request('get', 'json', eventType, params, data, 'filter.json');
        };

        var getProjectTags = function(eventType, params) {
            request('get', 'json', eventType, params, {}, 'tag.json');
        };

        var updateProjectStatus = function(projId, eventType, data) {
            request('put', 'json', eventType, {}, data, projId + '.json');
        };

        var getSubscribeToTask = function(eventType, taskId) {
            request('get', 'json', eventType, {}, {}, 'task/' + taskId + '/subscribe.json');
        };
        var subscribeToTask = function(eventType, taskId) {
            request('put', 'json', eventType, {}, { 'taskid': taskId }, 'task', taskId, 'subscribe.json');
        };

        var getTimes = function(eventType, params, filter, StartIndex, lastId) {
            if (filter == null) {
                filter = this.filter;
            }
            filter.Count = this.Count + 1;
            if (typeof StartIndex == 'undefined') {
                filter.StartIndex = 0;
            } else {
                filter.StartIndex = this.filter.StartIndex + StartIndex;
            }
            this.filter = filter;
            if (typeof lastId == 'undefined') filter.lastId = 0;
            else filter.lastId = lastId;

            request('get', 'json', eventType, params, filter, 'time', 'filter.json');
        };

        var updateTime = function(eventType, data, timeid) {
            request('put', 'json', eventType, { 'timeid': timeid }, data, 'time', timeid + '.json');
        };

        var removeTime = function(eventType, timeid) {
            //request('delete', 'json', eventType, { 'timeid': timeid }, timeid, 'time', timeid + '.json');
        	Teamlab.removePrjTime({ 'timeid': timeid }, timeid);
        };

        return {
            init: init,
            getTimes: getTimes,
            bind: addCustomEvent,
            unbind: removeCustomEvent,
            getTask: getTask,
            getFilteredTasks: getFilteredTasks,
            addTask: addTask,
            addSubTask: addSubTask,
            removeSubTask: removeSubTask,
            removeTask: removeTask,
            updateTaskStatus: updateTaskStatus,
            updateSubTaskStatus: updateSubTaskStatus,
            updateTask: updateTask,
            updateSubTask: updateSubTask,
            updateProjectStatus: updateProjectStatus,
            getTeam: getTeam,
            getMyGUID: getMyGUID,
            messageResponsible: messageResponsible,
            addTaskTime: addTaskTime,
            getTimeSpend: getTimeSpend,
            getProjects: getProjects,
            getAllProjects: getAllProjects,
            addTaskToProject: addTaskToProject,
            getTeamByProject: getTeamByProject,
            getMilestonesByProject: getMilestonesByProject,
            getProjectTags: getProjectTags,
            getProjectsByFilter: getProjectsByFilter,
            getSubscribeToTask: getSubscribeToTask,
            subscribeToTask: subscribeToTask,
            projectId: projectId,
            Count: Count,
            StartIndex: StartIndex,
            filter: filter,
            getOverdueTasks: getOverdueTasks,
            updateTime: updateTime,
            removeTime: removeTime
        };
    })(jQuery);
}
