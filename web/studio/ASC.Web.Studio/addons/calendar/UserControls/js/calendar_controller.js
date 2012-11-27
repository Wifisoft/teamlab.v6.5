﻿if (typeof ASC === 'undefined')
    ASC = {};

jq.ajaxSetup({
    cache: false
});

if (typeof ASC.Api === 'undefined')
    ASC.Api = {};

ASC.Api.TypeConverter = new function() {

    this.ClientTimeToServer = function(clientTime, offset) {

        var date = new Date();
        var minutesOffset = date.getTimezoneOffset();

        if (offset != undefined)
            minutesOffset = (-1) * offset;

        date.setTime(clientTime.getTime() + (minutesOffset * 60 * 1000));

        var m = (date.getMonth() + 1);
        var d = date.getDate();
        var h = date.getHours();
        var min = date.getMinutes();

        var str = date.getFullYear() + '-' + (m > 9 ? m : ('0' + m)) + '-' + (d > 9 ? d : ('0' + d))
                    + 'T' + (h > 9 ? h : ('0' + h)) + '-' + (min > 9 ? min : ('0' + min)) + "-00.000Z";  

        return str;
    }

    this.ServerTimeToClient = function(serverTime) {

        var str = serverTime.replace(/\..*/gi, '');
        var date = str.split('T')[0].split('-');
        var time = str.split('T')[1].split(':');
        return new Date(date[0], date[1] - 1, date[2], time[0], time[1]);        
    }
}

ASC.CalendarController = new function() {

    var clone = function(o) {
     if(!o || 'object' !== typeof o)  {
       return o;
     }
     var c = 'function' === typeof o.pop ? [] : {};
     var p, v;
     for(p in o) {
     if(o.hasOwnProperty(p)) {
      v = o[p];
      if(v && 'object' === typeof v) {
        c[p] = clone(v);
      }
      else {
        c[p] = v;
      }
     }
    }
     return c;
    }

    this.ApiUrl = '';
    var _controller = this;
    var callbackFunc = null;

    var sharingManager = new SharingSettingsManager(undefined, null);

    var AlertError = function(message) {

        try {
            console.log(message);
        }
        catch (e) {
            alert(message);
        }
    }

    this.LoadCalendars = function(startDate, endDate, callback) {

        callbackFunc = callback;

        var start = encodeURIComponent(ASC.Api.TypeConverter.ClientTimeToServer(startDate));
        var end = encodeURIComponent(ASC.Api.TypeConverter.ClientTimeToServer(endDate));

        jq.ajax({ type: 'get',
            url: _controller.ApiUrl + "/calendars/" + start + "/" + end + ".json",
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {

                    for (var i = 0; i < data.response.length; i++) {

                        for (var j = 0; j < data.response[i].events.length; j++) {

                            data.response[i].events[j].start = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].events[j].start);
                            data.response[i].events[j].end = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].events[j].end);
                            data.response[i].events[j].repeatRule = ASC.Api.iCal.ParseRRuleFromString(data.response[i].events[j].repeatRule);
                        }
                    }

                    callback({ result: true, eventSources: data.response });
                    return true;
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        })
    }

    this.GetiCalUrl = function(calendarId, callback) {
        callbackFunc = callback;
        jq.ajax({ type: "get",
            url: _controller.ApiUrl + "/" + calendarId + "/icalurl.json",
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, url: data.response });
                }
                else {
                    callbackFunc({ result: false, url: '' });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.CreateiCalStream = function(url, name, textColor, backgroundColor) {

        jq.ajax({ type: "post",
            url: _controller.ApiUrl + "/calendarUrl.json",
            data: {
                iCalUrl: url,
                name: name,
                textColor: textColor,
                backgroundColor: backgroundColor
            },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, source: data.response });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.CreateCalendar = function(name, description, textColor, backgroundColor, timeZone, eventAlertType, sharingOptions) {

        jq.ajax({ type: "post",
            url: _controller.ApiUrl + ".json",
            data: { name: name,
                description: description,
                textColor: textColor,
                backgroundColor: backgroundColor,
                timeZone: timeZone,
                alertType: eventAlertType,
                sharingOptions: sharingOptions
            },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, source: data.response,
                        importUrl: _controller.ApiUrl + "/" + data.response.objectId + "/import"
                    });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.UpdateCalendar = function(calendarId, name, description, textColor, backgroundColor, timeZone, eventAlertType, hideEvents, sharingOptions) {

        jq.ajax({ type: "put",
            url: _controller.ApiUrl + "/" + calendarId + ".json",
            data: {
                name: name,
                description: description,
                textColor: textColor,
                backgroundColor: backgroundColor,
                timeZone: timeZone,
                alertType: eventAlertType,
                hideEvents: hideEvents,
                sharingOptions: sharingOptions
            },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, source: data.response,
                        importUrl: _controller.ApiUrl + "/" + data.response.objectId + "/import"
                    });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.RemoveCalendar = function(calendarId) {
        jq.ajax({ type: "delete",
            url: _controller.ApiUrl + "/" + calendarId + ".json",
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, source: undefined });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.UpdateCalendarView = function(calendarId, name, textColor, backgroundColor, timeZone, eventAlertType, hideEvents) {
        jq.ajax({ type: "put",
            url: _controller.ApiUrl + "/" + calendarId + "/view.json",
            data: {
                calendarId: calendarId,
                name: name,
                textColor: textColor,
                backgroundColor: backgroundColor,
                timeZone: timeZone,
                alertType: eventAlertType,
                hideEvents: hideEvents
            },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, source: data.response });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.DoRequestToCalendar = function(params, callback) {
        callbackFunc = callback;

        //permissions
        var sharingOptions = new Array();
        if (params.action === 1 || params.action === 2) {
            if (params.permissions != undefined && params.permissions.data != undefined) {
                for (var i = 0; i < params.permissions.data.items.length; i++) {
                    var item = params.permissions.data.items[i];
                    if (item.selectedAction.id == 'owner')
                        continue;

                    sharingOptions.push({ actionId: item.selectedAction.id, itemId: item.id, isGroup: item.isGroup });
                }
            }
        }

        var timeZone = params.timeZone? params.timeZone.id : 'UTC';
        //create
        if (params.action === 1 && (params.iCalUrl == undefined || params.iCalUrl == null || params.iCalUrl == ''))
            _controller.CreateCalendar(params.title, params.description, params.textColor, params.backgroundColor, timeZone, params.defaultAlert.type, sharingOptions);

        //create stream
        else if (params.action === 1 && params.iCalUrl != undefined && params.iCalUrl != null && params.iCalUrl != '')
            _controller.CreateiCalStream(params.iCalUrl, params.title, params.textColor, params.backgroundColor);

        //update
        else if (params.action === 2)
            _controller.UpdateCalendar(params.objectId, params.title, params.description, params.textColor, params.backgroundColor, timeZone, params.defaultAlert.type, params.isHidden, sharingOptions);

        //delete
        else if (params.action === 3)
            _controller.RemoveCalendar(params.objectId);

        //show - hide events
        else if (params.action === 4)
            _controller.UpdateCalendarView(params.objectId, params.title, params.textColor, params.backgroundColor, timeZone, params.defaultAlert.type, params.isHidden);

        //cancel
        else if (params.action === 5)
            _controller.CancelEditDialog(params.permissions);
    }

    this.GetEventDays = function(params, callback) {
        callbackFunc = callback;

        var start = encodeURIComponent(ASC.Api.TypeConverter.ClientTimeToServer(params.startDate));
        var end = encodeURIComponent(ASC.Api.TypeConverter.ClientTimeToServer(params.endDate));

        jq.ajax({ type: 'get',
            url: _controller.ApiUrl + "/eventdays/" + start + "/" + end + ".json",
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {

                    for (var i = 0; i < data.response.length; i++) {
                        data.response[i] = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i]);
                    }
                    callback({ result: true, days: data.response });
                    return true;
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        })

    }


    this.DoRequestToEvent = function(params, callback) {
        callbackFunc = callback;

        //permissions
        var sharingOptions = new Array();
        if (params.action === 1 || params.action === 2) {
            if (params.permissions != undefined && params.permissions.data != undefined) {
                for (var i = 0; i < params.permissions.data.items.length; i++) {
                    var item = params.permissions.data.items[i];
                    if (item.selectedAction.id == 'owner')
                        continue;

                    sharingOptions.push({ actionId: item.selectedAction.id, itemId: item.id, isGroup: item.isGroup });
                }
            }
        }

        //create
        if (params.action === 1) {

            _controller.CreateEvent(params.newSourceId, params.title, params.description,
                                    params.start, params.end, params.repeatRule.ToiCalString(), params.alert.type, params.allDay, params.newTimeZone, sharingOptions);
        }

        //update
        else if (params.action === 2) {
            var tz = params.source.timeZone;
            if (params.source.objectId != params.newSourceId)
                tz = params.newTimeZone;

            _controller.UpdateEvent(params.newSourceId, params.objectId, params.title, params.description,
                                    params.start, params.end, params.repeatRule.ToiCalString(), params.alert.type, params.allDay, tz, sharingOptions);
        }


        //delete
        else if (params.action === 3)
            _controller.RemoveEvent(params.objectId, params.type, params.date);

        //unsubscribe
        else if (params.action === 4)
            _controller.UnsubscribeEvent(params.objectId);

        //cancel
        else if (params.action === 5)
            _controller.CancelEditDialog(params.permissions);


    }

    this.CancelEditDialog = function(permissions) {

        if (oldPermissions != null && permissions) {

            permissions.data = oldPermissions.data;
            permissions.users = oldPermissions.users;
        }

        oldPermissions = null;
    }

    //unsubscribe
    this.UnsubscribeEvent = function(eventId) {

        jq.ajax({ type: 'delete',
            url: _controller.ApiUrl + "/events/" + eventId + "/unsubscribe.json",
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true, event: undefined });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.RemoveEvent = function(eventId, type, date) {

        //all series
        if (type == 2) {
            jq.ajax({ type: 'delete',
                url: _controller.ApiUrl + "/events/" + eventId + ".json",
                complete: function(d) {
                    var data = jq.evalJSON(d.responseText);
                    if (data.status === 0) {
                        callbackFunc({ result: true, event: undefined });
                    }
                    else {
                        callbackFunc({ result: false });
                        AlertError(data.error.message);
                    }
                }
            });
        }
        else {
            jq.ajax({ type: 'delete',
                url: _controller.ApiUrl + "/events/" + eventId + "/custom.json",
                data: {
                    type: type,
                    date: ASC.Api.TypeConverter.ClientTimeToServer(date, 0)
                },
                complete: function(d) {
                    var data = jq.evalJSON(d.responseText);
                    if (data.status === 0) {

                        for (var i = 0; i < data.response.length; i++) {
                            data.response[i].start = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].start);
                            data.response[i].end = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].end);
                            data.response[i].repeatRule = ASC.Api.iCal.ParseRRuleFromString(data.response[i].repeatRule);
                        }
                        callbackFunc({ result: true, event: data.response });
                    }
                    else {
                        callbackFunc({ result: false });
                        AlertError(data.error.message);
                    }
                }
            });
        }
    }

    this.CreateEvent = function(calendarId, name, description, startDate, endDate, repeatType, alertType, isAllDayLong, timeZone, shareOptions) {

        if (endDate == null || endDate == 'null' || endDate == '')
            endDate = startDate;

        var offset = isAllDayLong ? 0 : timeZone.offset;

        startDate = ASC.Api.TypeConverter.ClientTimeToServer(startDate, offset);
        endDate = ASC.Api.TypeConverter.ClientTimeToServer(endDate, offset);

        jq.ajax({ type: 'post',
            url: _controller.ApiUrl + "/" + calendarId + "/event.json",
            data: {
                calendarId: calendarId,
                name: name,
                description: description,
                startDate: startDate,
                endDate: endDate,
                repeatType: repeatType,
                alertType: alertType,
                isAllDayLong: isAllDayLong,
                sharingOptions: shareOptions
            },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {

                    for (var i = 0; i < data.response.length; i++) {
                        data.response[i].start = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].start);
                        data.response[i].end = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].end);
                        data.response[i].repeatRule = ASC.Api.iCal.ParseRRuleFromString(data.response[i].repeatRule);
                    }
                    callbackFunc({ result: true, event: data.response });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.UpdateEvent = function(calendarId, eventId, name, description, startDate, endDate, repeatType, alertType, isAllDayLong, timeZone, shareOptions) {

        if (endDate == null || endDate == 'null' || endDate == '')
            endDate = startDate;

        var offset = isAllDayLong ? 0 : timeZone.offset;

        startDate = ASC.Api.TypeConverter.ClientTimeToServer(startDate, offset);
        endDate = ASC.Api.TypeConverter.ClientTimeToServer(endDate, offset);

        jq.ajax({ type: 'put',
            url: _controller.ApiUrl + "/" + calendarId + "/" + eventId + ".json",
            data: {
                calendarId: calendarId,
                eventId: eventId,
                name: name,
                description: description,
                startDate: startDate,
                endDate: endDate,
                repeatType: repeatType,
                alertType: alertType,
                isAllDayLong: isAllDayLong,
                sharingOptions: shareOptions
            },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    for (var i = 0; i < data.response.length; i++) {
                        data.response[i].start = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].start);
                        data.response[i].end = ASC.Api.TypeConverter.ServerTimeToClient(data.response[i].end);
                        data.response[i].repeatRule = ASC.Api.iCal.ParseRRuleFromString(data.response[i].repeatRule);
                    }
                    callbackFunc({ result: true, event: data.response });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    //permissions
    var SetAccessForCalendar = function(sharingData) {
        var permissions = new Array();
        for (var i = 0; i < sharingData.items.length; i++) {
            var item = sharingData.items[i];
            if (item.selectedAction.id == 'owner')
                continue;

            permissions.push({ objectId: item.id, name: item.name });
        }
        callbackFunc({ result: true, permissions: { users: permissions, data: sharingData} });
    }

    var oldPermissions = null;
    this.RemovePermissions = function(params, callback) {


        if (params.permissions != undefined && params.permissions.data != undefined) {

            oldPermissions = clone(params.permissions);
            
            for (var i = 0; i < params.permissions.data.items.length; i++) {
                var item = params.permissions.data.items[i];
                if (item.selectedAction.id == 'owner')
                    continue;

                if (item.id == params.userId) {
                    params.permissions.data.items.splice(i, 1);

                    for (var k = 0; k < params.permissions.users.length; k++) {
                        if (params.permissions.users[k].objectId == params.userId) {
                            params.permissions.users.splice(k, 1);
                            break;
                        }
                    }

                    callback({ result: true, permissions: params.permissions });
                    return;
                }
            }
        }

        callback({ result: false });
    }

    
    this.EditPermissions = function(param, callback) {

        callbackFunc = callback;
        oldPermissions = param.permissions;

        if (param.permissions.data == undefined) {

            jq.ajax({ type: 'get',
                url: _controller.ApiUrl + "/sharing.json",
                complete: function(d) {
                    var data = jq.evalJSON(d.responseText);
                    if (data.status === 0) {
                        sharingManager.OnSave = SetAccessForCalendar;
                        sharingManager.UpdateSharingData(data.response);
                        sharingManager.ShowDialog();
                        return;
                    }
                    else {
                        callbackFunc({ result: false });
                        AlertError(data.error.message);
                    }
                }
            });
        }
        else {
            sharingManager.OnSave = SetAccessForCalendar;
            sharingManager.UpdateSharingData(param.permissions.data);
            sharingManager.ShowDialog();
            return;
        }
    }

    // ViewChangedHandler
    this.ViewChangedHandler = function(currentViewName, currentDate) {
        if (currentViewName != '') {
            try {
                ASC.Controls.AnchorController.safemove(currentViewName + '/' + currentDate.getTime());
            }
            catch (e) { }
        }
    }


    //subscriptions
    this.LoadSubscriptions = function(callback) {

        callbackFunc = callback;
        jq.ajax({ type: 'get',
            url: _controller.ApiUrl + "/subscriptions.json",
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);

                if (data.status === 0)
                    callbackFunc({ result: true, subscriptions: data.response });
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }

    this.ManageSubscriptions = function(params, callback) {
        callbackFunc = callback;
        var states = new Array();
        for (var i = 0; i < params.length; i++) {
            states.push({ id: params[i].objectId, isAccepted: (params[i].action == 1) });
        }

        jq.ajax({ type: 'put',
            url: _controller.ApiUrl + "/subscriptions/manage.json",
            data: { states: states },
            complete: function(d) {
                var data = jq.evalJSON(d.responseText);
                if (data.status === 0) {
                    callbackFunc({ result: true });
                }
                else {
                    callbackFunc({ result: false });
                    AlertError(data.error.message);
                }
            }
        });
    }
}