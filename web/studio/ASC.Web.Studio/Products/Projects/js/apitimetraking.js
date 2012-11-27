window.ASC.Projects.TimeTraking = (function() {
    var timer,
        seconds = 0,
        timerHours = 0,
        timerMin = 0,
        timerSec = 0;

    var init = function() {
        if (!jq('#inputDate').hasClass('hasDatepicker')) {
            jq('#inputDate').datepicker({ selectDefaultDate: false, onSelect: function() { jq('#inputDate').blur(); } });
        }

        jq('#inputDate').datepicker('setDate', new Date());

        Teamlab.bind(Teamlab.events.getPrjTeam, onGetTeam);
        Teamlab.bind(Teamlab.events.getPrjTasks, onGetTasks);

        serviceManager.bind('addtasktime', onAddTaskTime);

        jq('#timerTime #selectUserProjects').bind('change', function(event) {
            var prjid = parseInt(jq("#selectUserProjects option:selected").val());
            serviceManager.getTeamByProject('getTeamTT', {}, prjid);
            var filter = { sortBy: 'title', sortOrder: 'ascending', projectId: prjid };
            Teamlab.getPrjTasks({}, null, null, null, { success: onGetTasks, filter: filter });
        });

        jq('#timerTime .start').bind('click', function(event) {
            playPauseTimer();
        });

        jq('#timerTime .reset').bind('click', function(event) {
            resetTimer();
        });

        jq('#timerTime #addLog').bind('click', function(event) {
            var prjid = parseInt(jq("#selectUserProjects option:selected").val());
            var personid = jq("#teamList option:selected").val();
            var taskid = parseInt(jq("#selectUserTasks option:selected").val());
            var description = jq("#textareaTimeDesc").val().trim();
            jq("#inputTimeHours").removeClass('error');
            jq("#inputTimeMinutes").removeClass('error');
            jq("#timeTrakingErrorPanel").hide();

            if (seconds > 0) {
                h = parseInt(jq("#firstViewStyle .h").text(), 10);
                m = parseInt(jq("#firstViewStyle .m").text(), 10);
                s = parseInt(jq("#firstViewStyle .s").text(), 10);
                if (!(h > 0)) h = 0;
                if (!(m > 0)) m = 0;
                if (!(s > 0)) s = 0;
                var hours = h + m / 60 + s / 3600;

                resetTimer();
            } else {
                var h = jq("#inputTimeHours").val();
                var m = jq("#inputTimeMinutes").val();

                if (!parseInt(h, 10) && !parseInt(m, 10)) {
                    jq("#timeTrakingErrorPanel").text(ProjectJSResources.InvalidTime).show();
                    jq("#inputTimeHours").addClass('error');
                    jq("#inputTimeMinutes").addClass('error');
                    return;
                }

                if ((parseInt(h, 10) < 0 || !isInt(h)) || (parseInt(m, 10) > 59 || parseInt(m, 10) < 0 || !isInt(m))) {
                    jq("#timeTrakingErrorPanel").text(ProjectJSResources.InvalidTime).show();
                    jq("#inputTimeHours").addClass('error');
                    jq("#inputTimeMinutes").addClass('error');
                    return;
                }

                m = parseInt(m, 10);
                h = parseInt(h, 10);

                if (!(h > 0)) h = 0;
                if (!(m > 0)) m = 0;

                var hours = h + m / 60;
            }

            var data = {};
            data.date = jq('#inputDate').datepicker('getDate');
            data.date.setHours(0);
            data.date.setMinutes(0);
            data.date = Teamlab.serializeTimestamp(data.date);
            data.hours = hours;
            data.note = description;
            data.personId = personid;
            data.projectId = prjid;

            serviceManager.addTaskTime('addtasktime', data, taskid);
        });
    };

    var isInt = function(input) {
        return parseInt(input, 10) == input;
    };

    var onAddTaskTime = function(data) {
        jq("#textareaTimeDesc").val('');
        jq("#inputTimeHours").val('');
        jq("#inputTimeMinutes").val('');
        jq("#firstViewStyle .h,#firstViewStyle .m,#firstViewStyle .s").val('00');
    };

    var playTimer = function() {
        lockElements();
        window.clearInterval(timer);

        var startHour = jq("#inputTimeHours").val().trim();
        var startMinute = jq("#inputTimeMinutes").val().trim();

        if (startMinute.length) {
            if (startMinute > 59) {
                jq("#timeTrakingErrorPanel").text(ProjectJSResources.InvalidTime).show();
                unlockElements();
                jq("#timerTime .start").removeClass("stop").attr("alt", buttonStyle.playButton.title).attr("title", buttonStyle.playButton.title);
                buttonStyle.isPlay = true;
                return;
            }
        }

        startTimer();
    };

    var startTimer = function() {
        timer = window.setInterval(function() {
            timerSec++;
            seconds++;
            if (timerSec == 60) {
                timerSec = 0;
                timerMin++;
                if (timerMin == 60) {
                    timerMin = 0;
                    timerHours++;
                }
            }

            if (timerHours < 10) showHours = "0" + timerHours; else showHours = timerHours;
            if (timerMin < 10) showMin = "0" + timerMin; else showMin = timerMin;
            if (timerSec < 10) showSec = "0" + timerSec; else showSec = timerSec;

            jq("#firstViewStyle .h").text(showHours);
            jq("#firstViewStyle .m").text(showMin);
            jq("#firstViewStyle .s").text(showSec);
        }, 1000);
    };

    var pauseTimer = function() {
        window.clearInterval(timer);
    };

    var resetTimer = function() {
        unlockElements();
        window.clearInterval(timer);

        jq("#firstViewStyle .h").text('00');
        jq("#firstViewStyle .m").text('00');
        jq("#firstViewStyle .s").text('00');

        jq("#timerTime .start").removeClass("stop").attr("alt", buttonStyle.playButton.title).attr("title", buttonStyle.playButton.title);
        buttonStyle.isPlay = true;
        seconds = 0;
        timerSec = 0;
        timerMin = 0;
        timerHours = 0;
    };

    var lockElements = function() {
        jq("#inputTimeHours").attr("disabled", "true");
        jq("#inputTimeMinutes").attr("disabled", "true");
    };

    var unlockElements = function() {
        jq("#inputTimeHours").removeAttr("disabled");
        jq("#inputTimeMinutes").removeAttr("disabled");
    };

    var playPauseTimer = function() {
        if (buttonStyle.isPlay) {
            jq("#timerTime .start").addClass("stop").attr("alt", buttonStyle.pauseButton.title).attr("title", buttonStyle.pauseButton.title);
            buttonStyle.isPlay = false;
            playTimer();
        }
        else {
            jq("#timerTime .start").removeClass("stop").attr("alt", buttonStyle.playButton.title).attr("title", buttonStyle.playButton.title);
            buttonStyle.isPlay = true;
            pauseTimer();
        }
    };

    var onGetTeam = function(params, team) {
        if (params.eventType == 'getTeamTT') {
            var teamInd = team ? team.length : 0;
            jq('#teamList option').remove();
            var i;
            for (i = 0; i < teamInd; i++) {
                jq('#teamList').append('<option value="' + team[i].id + '" id="optionUser_' + team[i].id + '">' + team[i].displayName + '</option>');
            }
        }
    };

    var onGetTasks = function(params, tasks) {
        var teamInd = tasks ? tasks.length : 0;
        jq('#selectUserTasks option').remove();
        var i;
        for (i = 0; i < teamInd; i++) {
            if (tasks[i].status == 1) {
                jq('#openTasks').append('<option value="' + tasks[i].id + '" id="optionUser_' + tasks[i].id + '">' + Encoder.htmlEncode(tasks[i].title) + '</option>');
            }
            if (tasks[i].status == 2) {
                jq('#closedTasks').append('<option value="' + tasks[i].id + '" id="optionUser_' + tasks[i].id + '">' + Encoder.htmlEncode(tasks[i].title) + '</option>');
            }
        }
    };

    var ifNotAdded = function() {
        if (jq("#inputTimeHours").val('') || jq("#inputTimeMinutes").val('') || jq("#firstViewStyle .h").val() != '' || jq("##firstViewStyle .m").val('') || jq("##firstViewStyle .s").val(''))
            return true;

        return false;
    };

    return {
        init: init,
        playPauseTimer: playPauseTimer,
        ifNotAdded: ifNotAdded
    };
})(jQuery);

jq(document).ready(function() {
    ASC.Projects.TimeTraking.init();
})