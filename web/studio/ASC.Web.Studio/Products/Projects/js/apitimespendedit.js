window.ASC.Projects.TimeTrakingEdit = (function() {
    var projId, tasklistResponsibles, datepickMask;
    var init = function(dateMask) {

        projId = jq.getURLParam("prjID");
        tasklistResponsibles = projectTeam;
        datepickMask = dateMask;
        serviceManager.bind('gettimespendForTraking', onGetTimeSpend);
        serviceManager.bind('getTeamByProject', onGetTeamByProject);

        jq('#timeTrakingPopup .pm-action-block a.baseLinkButton').bind('click', function(event) {
            var data = {};
            var h = jq("#inputTimeHours").val();
            var m = jq("#inputTimeMinutes").val();

            if (parseInt(m, 10) > 59 || parseInt(m, 10) < 0 || !isInt(m)) {
                jq("#timeTrakingErrorPanel").show();
                jq("#timeTrakingErrorPanel").text(ProjectJSResources.InvalidTime);
                setInterval('jq("#timeTrakingErrorPanel").hide();', 5000);
                jq("#inputTimeMinutes").focus();
                return;
            }
            if (parseInt(h, 10) < 0 || !isInt(h)) {
                jq("#timeTrakingErrorPanel").show();
                jq("#timeTrakingErrorPanel").text(ProjectJSResources.InvalidTime);
                setInterval('jq("#timeTrakingErrorPanel").hide();', 5000);
                jq("#inputTimeHours").focus();
                return;
            }
            if (!parseInt(h, 10) && !parseInt(m, 10)) {
                jq("#timeTrakingErrorPanel").show();
                jq("#timeTrakingErrorPanel").text(ProjectJSResources.InvalidTime);
                setInterval('jq("#timeTrakingErrorPanel").hide();', 5000);
                jq("#inputTimeMinutes").focus();
                return;
            }
            m = parseInt(m, 10);
            h = parseInt(h, 10);

            if (!(h > 0)) h = 0;
            if (!(m > 0)) m = 0;

            data.hours = h + m / 60;

            data.date = jq('#timeTrakingPopup #timeTrakingDate').val();
            var isValidDate = jq.isValidDate(data.date);
            if (jq.trim(data.date) == "" || data.date == null || !isValidDate) {
                jq("#timeTrakingErrorPanel").text(ProjectJSResources.IncorrectDate);
                jq("#timeTrakingErrorPanel").show();
                setInterval('jq("#timeTrakingErrorPanel").hide();', 5000);
                jq('#timeTrakingPopup #timeTrakingDate').focus();
                return;
            }
            data.date = jq('#timeTrakingPopup #timeTrakingDate').datepicker('getDate');
            var timeid = jq("#timeTrakingPopup").attr('timeid');
            data.date = Teamlab.serializeTimestamp(data.date);
            data.note = jq('#timeTrakingPopup #timeDescription').val();
            data.personId = jq('#teamList option:selected').attr('value');

            data.projectId = projId;

            Teamlab.updatePrjTime({}, timeid, data, ASC.Projects.TimeSpendActionPage.onUpdateTime);

            jq.unblockUI();

        });
    };
    var isInt = function(input) {
        return parseInt(input, 10) == input;
    };
    var showPopup = function(taskid, taskName, timeId, hours, date, description, responsible) {

        jq("#timeTrakingPopup").attr('timeid', timeId);
        jq("#timeTrakingPopup #timeDescription").val(description);
        jq("#timeTrakingPopup #TimeLogTaskTitle").text(taskName);
        jq("#timeTrakingPopup #TimeLogTaskTitle").attr('taskid', taskid);

        var separateTime = hours.split(':');
        jq("#inputTimeHours").val(separateTime[0]);
        jq("#inputTimeMinutes").val(separateTime[1]);

        date = Teamlab.getDisplayDate(Teamlab.serializeDate(date));
        jq("#timeTrakingPopup #timeTrakingDate").mask(datepickMask);
        jq("#timeTrakingPopup #timeTrakingDate").datepicker({ popupContainer: "#timeTrakingPopup", selectDefaultDate: true });
        jq("#timeTrakingPopup #timeTrakingDate").datepicker('setDate', date);

        serviceManager.getTimeSpend("gettimespendForTraking", taskid);
        if (typeof projectTeam != 'undefined') {
            var team = Teamlab.create('prj-projectpersons', null, jq.parseJSON(jQuery.base64.decode(projectTeam)).response);
            jq('select#teamList option').remove();
            if (team.length) {
                appendListOptions(team);
            }
        } else {
            Teamlab.getPrjProjectTeamPersons({}, projId, { success: onGetTeamByProject });
        }
        selectUser(responsible);
        var margintop = jq(window).scrollTop() - 100;
        margintop = margintop + 'px';
        jq.blockUI({ message: jq("#timeTrakingPopup"),
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

            onBlock: function() {
            }
        });
    };

    var onGetTimeSpend = function(data) {
        var hours = 0;
        var reports = jQuery.parseJSON(data).response;
        for (var i = 0; i < reports.length; i++) {
            hours += reports[i].hours;
        }
        var time = jq.timeFormat(hours);
        jq(".addLogPanel-infoPanelBody #TotalHoursCount").text(time);
    };

    var appendListOptions = function(team) {
        for (var i = 0; i < team.length; i++) {
            jq('select#teamList').append('<option value="' + team[i].id + '" style="width:195px; overflow:hidden !important;">' + team[i].displayName + '</option>');
        }
        jq("select#teamList option[value=" + team[0].id + "]").attr("selected", "selected");
    };
    var selectUser = function(id) {
        var team = jq("select#teamList option");
        for (var i = 0; i < team.length; i++) {
            if (id == jq(team[i]).attr('value')) {
                jq(team[i]).attr("selected", "selected");
                break;
            }
        }
    };
    var onGetTeamByProject = function(params, data) {
        var team = data;
        jq('select#teamList option').remove();
        if (team.length) {
            appendListOptions(team);
        }
    };
    return {
        init: init,
        showPopup: showPopup
    };
})(jQuery);
