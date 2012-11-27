ASC.Projects.Reports = (function() {
    // Private Section

    return { // Public Section

        viewAll: function(id) {
            if (jq('#img' + id).attr('title') == ProjectJSResources.Expand) {
                jq("tr[value='" + id + "']").each(function() {
                    jq(this).show();
                }
        );
                jq('#img' + id).attr('title', ProjectJSResources.Collapse);
            }
            else {
                jq("tr[value='" + id + "']").each(function() {
                    jq(this).hide();
                }
        );
                jq('#img' + id).attr('title', ProjectJSResources.Expand);
            }

            var src = jq('#img' + id).attr('src');
            if (src.indexOf("expand") > -1) {
                var newSrc = src.replace("expand", "collapce");
                jq('#img' + id).attr('src', newSrc);
            }

            if (src.indexOf("collapce") > -1) {
                var newSrc = src.replace("collapce", "expand");
                jq('#img' + id).attr('src', newSrc);
            }

        },

        changeDate: function() {

            var selectedIndex = jq('#selectDate option:selected').val();
            var startDate;
            var finishDate;

            AjaxPro.Reports.SetStartDateTbx(selectedIndex,
        function(res) {
            jq("[id$=tbxStartDate]").val(res.value);
            startDate = Date.parse(res.value);
        }
        );

            AjaxPro.Reports.SetFinishDateTbx(selectedIndex,
        function(res) {
            jq("[id$=tbxFinishDate]").val(res.value);
            finishDate = Date.parse(res.value);
        }
        );

        },

        applyFilter: function(actionType) {

            AjaxPro.onLoading = function(b) {
                if (b) {
                    ASC.Projects.Reports.lockForm();
                }
                else {
                    ASC.Projects.Reports.unlockForm();
                }
            }

            AjaxPro.Reports.RenderControlByFilter(actionType, jq("[id$=tbxStartDate]").val(), jq("[id$=tbxFinishDate]").val(),
        function(res) {
            if (res.value == 'error') {
                alert(ProjectJSResources.IncorrectDate);
            }
            else {
                jq('#container').html(res.value);
                if (jq('#selectDate option:selected').val() == 8) ASC.Projects.Reports.unlock();
                else ASC.Projects.Reports.lock();
            }
        }
        );

        },
        changeTimeRange: function(DateTimeFormat, mask) {
            var index = jq('#selectDate option:selected').val()
            var currentTime = new Date();

            var from_date = "";
            var to_date = "";

            switch (parseInt(index)) {

                case 0:   // Today

                    ASC.Projects.Reports.lock();
                    from_date = currentTime.format(DateTimeFormat);
                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 1:  // Yesterday

                    ASC.Projects.Reports.lock();
                    currentTime.setDate(currentTime.getDate() - 1);

                    from_date = currentTime.format(DateTimeFormat);
                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 2:  // This week

                    ASC.Projects.Reports.lock();
                    currentTime.setDate(-currentTime.getDay() + currentTime.getDate() + 1);

                    from_date = currentTime.format(DateTimeFormat);

                    currentTime.setDate(currentTime.getDate() + 6);

                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 3:  // Last Week


                    ASC.Projects.Reports.lock();
                    currentTime.setDate(-currentTime.getDay() + 1 + currentTime.getDate() - 7);

                    from_date = currentTime.format(DateTimeFormat);
                    currentTime.setDate(currentTime.getDate() + 6);

                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 4: // This month

                    ASC.Projects.Reports.lock();
                    currentTime.setMonth(currentTime.getMonth() + 1);
                    currentTime.setDate(-1);
                    currentTime.setDate(currentTime.getDate() + 1);

                    to_date = currentTime.format(DateTimeFormat);

                    currentTime.setDate(1);

                    from_date = currentTime.format(DateTimeFormat);

                    break;

                case 5: // Last month

                    ASC.Projects.Reports.lock();
                    currentTime.setMonth(currentTime.getMonth());
                    currentTime.setDate(-1);
                    currentTime.setDate(currentTime.getDate() + 1);

                    to_date = currentTime.format(DateTimeFormat);

                    currentTime.setDate(1);
                    from_date = currentTime.format(DateTimeFormat);

                    break;

                case 6: // This Year

                    ASC.Projects.Reports.lock();
                    currentTime.setFullYear(currentTime.getFullYear(), 0, 1);

                    from_date = currentTime.format(DateTimeFormat);

                    currentTime.setFullYear(currentTime.getFullYear() + 1, 0, 1);

                    currentTime.setDate(0);

                    to_date = currentTime.format(DateTimeFormat);


                    break;

                case 7: // Last Year

                    ASC.Projects.Reports.lock();
                    currentTime.setFullYear(currentTime.getFullYear() - 1, 0, 1);

                    from_date = currentTime.format(DateTimeFormat);

                    currentTime.setFullYear(currentTime.getFullYear() + 1, 0, 1);

                    currentTime.setDate(0);

                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 8: // Other

                    ASC.Projects.Reports.unlock(mask);
                    from_date = jq("[id$=tbxStartDate]").val();
                    to_date = jq("[id$=tbxFinishDate]").val();

                    break;

            }

            jq("[id$=tbxStartDate]").val(from_date);
            jq("[id$=tbxFinishDate]").val(to_date);


        },
        lock: function() {
            jq("[id$=tbxStartDate]").unmask().attr("readonly", "readonly").addClass("disabled");
            jq("[id$=tbxFinishDate]").unmask().attr("readonly", "readonly").addClass("disabled");
        },
        unlock: function(mask) {
            jq("[id$=tbxStartDate]").removeAttr("readonly").removeClass("disabled").mask(mask);
            jq("[id$=tbxFinishDate]").removeAttr("readonly").removeClass("disabled").mask(mask);
        },
        lockForm: function() {
            jq("[id$=tbxStartDate]").attr("disabled", "disabled");
            jq("[id$=tbxFinishDate]").attr("disabled", "disabled");
            jq("#selectDate").attr("disabled", "disabled");
            jq("#container").block({ message: null });
            jq("#container").attr("readonly", "readonly").addClass("disabled");
        },
        unlockForm: function() {
            jq("[id$=tbxStartDate]").removeAttr("disabled");
            jq("[id$=tbxFinishDate]").removeAttr("disabled");
            jq("#selectDate").removeAttr("disabled");
            jq("#container").unblock();
            jq("#container").removeAttr("readonly").removeClass("disabled").css("position", "");
        },
        changeProject: function(tag, prj, user) {

            tag = typeof (tag) == "undefined" ? jq('#Tags option:selected').val() : tag.toString();
            prj = typeof (prj) == "undefined" ? jq('#Projects option:selected').val() : prj.toString();
            user = typeof (user) == "undefined" ? "-1" : user.toString();

            if (jq('#Tags').length != 0 && prj == "-1") {
                AjaxPro.Reports.InitUsersDdlByTag(tag,
                function(res) {
                    jq("#Users").html(res.value);
                    jq("#ddlUser" + user).attr('selected', 'selected');
                });
            }
            else {
                AjaxPro.Reports.InitUsersDdlByProject(prj,
                function(res) {
                    jq("#Users").html(res.value);
                    jq("#ddlUser" + user).attr('selected', 'selected');
                });
            }
        },
        changeProject1: function() {
            var prj = jq('#Projects option:selected').val();
            jq("[id$=HiddenFieldForProject]").val(prj);
            if (prj != "-1") {
                jq("#projectStatusFilterHeader").hide();
                jq("#projectStatusFilterBody").hide();
            }
            else {
                jq("#projectStatusFilterHeader").show();
                jq("#projectStatusFilterBody").show();
            }
            AjaxPro.Reports.InitUsersDdlByProject(prj,
        function(res) {
            jq("#Users").html(res.value);
            jq("#ddlUser" + jq("[id$=HiddenFieldForUser]").val()).attr('selected', 'selected');
        }
        );

        },

        changeTaskStatus: function(value) {
            ASC.Projects.Reports.setFiltersValueInURL("taskStatus", value);
        },

        changeMilestoneStatus: function(value) {
            ASC.Projects.Reports.setFiltersValueInURL("milestoneStatus", value);
        },

        changeDepartment: function(dep, user) {

            dep = typeof (dep) == "undefined" ? jq('#Departments option:selected').val() : dep.toString();
            user = typeof (user) == "undefined" ? "-1" : user.toString();

            AjaxPro.Reports.InitUsersDdlByDepartment(dep,
            function(res) {
                jq("#Users").html(res.value);
                jq("#ddlUser" + user).attr('selected', 'selected');
            });
        },

        changeResponsible: function() {

            responsibleID = jq('#Users option:selected').val()

            if (responsibleID != '-1') {
                jq("#cbxShowTasksWithoutResponsible").attr("disabled", "disabled").removeAttr("checked");
            }
            else {
                jq("#cbxShowTasksWithoutResponsible").removeAttr("disabled");
            }
        },

        changeTag: function(tag, prj, user) {

            tag = typeof (tag) == "undefined" ? jq('#Tags option:selected').val() : tag.toString();
            prj = typeof (prj) == "undefined" ? "-1" : prj.toString();
            user = typeof (user) == "undefined" ? "-1" : user.toString();

            AjaxPro.Reports.InitProjectsDdlByTag(tag,
            function(res) {
                jq("#Projects").html(res.value);
                jq("#ddlProject" + prj).attr('selected', 'selected');
            });
            if (jq('#Users').length != 0) {
                AjaxPro.Reports.InitUsersDdlByTag(tag,
                function(res) {
                    jq("#Users").html(res.value);
                    jq("#ddlUser" + user).attr('selected', 'selected');
                });
            }
        },

        changePeriod: function(periodItem, hour) {
            var period = jq('#periods option:selected').val();

            AjaxPro.Reports.InitPeriodItemsDdlByPeriod(period,
        function(res) {
            jq("#periodItems").html(res.value);
            if (period != 0) {
                jq("#additionalHours").show();
                jq("#pretext").hide();
            }
            else {
                jq("#additionalHours").hide();
                jq("#pretext").show();
            }
            if (arguments.length === 2) {
                jq("#ddlPeriodItems" + periodItem).attr("selected", "selected");
                jq("#ddlHours" + hour).attr("selected", "selected");
            }

        }
        );
        },
        slideFilters: function(first, last) {
            jq("#reportFilters").slideToggle("slow");

            var src = jq("#slideFiltersImg").attr('src');

            if (jq("[id$=HiddenFieldViewReportFilters]").val() == "0") {
                jq("[id$=HiddenFieldViewReportFilters]").val("1");
                var newSrc = first;
                jq("#slideFiltersLink").html(ProjectJSResources.HideFilters);
                jq("#slideFiltersImg").attr('src', newSrc);
                jq("#slideFiltersImg").attr('title', ProjectJSResources.Collapse);
                jq("#slideFiltersImg").attr('alt', ProjectJSResources.Collapse);
            }
            else {
                jq("[id$=HiddenFieldViewReportFilters]").val("0");
                var newSrc = last;
                jq("#slideFiltersLink").html(ProjectJSResources.ViewFilters);
                jq("#slideFiltersImg").attr('src', newSrc);
                jq("#slideFiltersImg").attr('title', ProjectJSResources.Expand);
                jq("#slideFiltersImg").attr('alt', ProjectJSResources.Expand);
            }

        },
        viewFilters: function(first, last) {

            var src = jq("#slideFiltersImg").attr('src');

            if (jq("[id$=HiddenFieldViewReportFilters]").val() == "0") {
                var newSrc = last;
                jq("#slideFiltersLink").html(ProjectJSResources.ViewFilters);
                jq("#slideFiltersImg").attr('src', newSrc);
                jq("#slideFiltersImg").attr('title', ProjectJSResources.Expand);
                jq("#slideFiltersImg").attr('alt', ProjectJSResources.Expand);

            }
            else {
                var newSrc = first;
                jq("#slideFiltersLink").html(ProjectJSResources.HideFilters);
                jq("#slideFiltersImg").attr('src', newSrc);
                jq("#slideFiltersImg").attr('title', ProjectJSResources.Collapse);
                jq("#slideFiltersImg").attr('alt', ProjectJSResources.Collapse);
            }

        },

        changeReportType: function(val) {
            location.replace("reports.aspx?reportType=" + val);
        },

        changeReportType1: function(val) {
            jq("#type_rbl_" + val).attr("checked", "checked");
            if (val == 0) {
                jq("#projectHeader").hide()
                jq("#projectBody").hide()
                jq("#departmentHeader").show()
                jq("#departmentBody").show()
                jq('#ddlDepartment-1').attr('selected', 'selected');
                jq('[id$=HiddenFieldForDepartment]').val('-1');
                jq('#ddlProject-1').attr('selected', 'selected');
                jq('[id$=HiddenFieldForProject]').val('-1');
                jq('#ddlUser-1').attr('selected', 'selected');
                jq('[id$=HiddenFieldForUser]').val('-1');
                jq("td.tagsContent").each(function() { jq(this).hide(); })
                jq('#ddlTag').attr('selected', 'selected');
                ASC.Projects.Reports.changeDepartment();
            }
            else {
                jq("#departmentHeader").hide()
                jq("#departmentBody").hide()
                jq("#projectHeader").show()
                jq("#projectBody").show()
                jq('#ddlDepartment-1').attr('selected', 'selected');
                jq('[id$=HiddenFieldForDepartment]').val('-1');
                jq('#ddlProject-1').attr('selected', 'selected');
                jq('[id$=HiddenFieldForProject]').val('-1');
                jq('#ddlUser-1').attr('selected', 'selected');
                jq('[id$=HiddenFieldForUser]').val('-1');
                jq("td.tagsContent").each(function() { jq(this).show(); })
                jq('#ddlTag').attr('selected', 'selected');
                ASC.Projects.Reports.changeProject();
            }
        },

        sortTable: function(column) {
            var url = window.location.href;
            var str = "&sortParam=";
            var src = jq("#sortImg_" + column).attr('src');

            if (src.indexOf("sort_down") != -1)
                str = str + column + "_1";
            else
                str = str + column + "_0";

            if (url.indexOf("sortParam") != -1) {
                var str1 = url.substr(0, url.indexOf("sortParam") - 1);
                var str2 = url.substr(url.indexOf("sortParam") + 13);
                window.location.href = str1 + str2 + str;
            }
            else {
                window.location.href = window.location.href + str;
            }
        },

        viewReportCategory: function(category, first, last) {
            jq('[id$=reportCategory_HiddenField]').val(category);

            jq("#rblCategory0").removeClass("pm-hidden").addClass("pm-hidden");
            jq("#imgCategory0").attr('src', first).attr('title', ProjectJSResources.Expand).attr('alt', ProjectJSResources.Expand);
            jq("#rblCategory1").removeClass("pm-hidden").addClass("pm-hidden");
            jq("#imgCategory1").attr('src', first).attr('title', ProjectJSResources.Expand).attr('alt', ProjectJSResources.Expand);
            jq("#rblCategory2").removeClass("pm-hidden").addClass("pm-hidden");
            jq("#imgCategory2").attr('src', first).attr('title', ProjectJSResources.Expand).attr('alt', ProjectJSResources.Expand);

            if (jq("#rblCategory" + category).attr('class') != "") {
                jq("#rblCategory" + category).removeClass("pm-hidden");
            }
            else {
                jq("#rblCategory" + category).addClass("pm-hidden");
            }

            var src = jq("#imgCategory" + category).attr('src');

            if (src.indexOf("expand") > -1) {
                var newSrc = first;
                jq("#imgCategory" + category).attr('src', newSrc);
                jq("#imgCategory" + category).attr('title', ProjectJSResources.Expand);
                jq("#imgCategory" + category).attr('alt', ProjectJSResources.Expand);
            }
            if (src.indexOf("collapse") > -1) {
                var newSrc = last;
                jq("#imgCategory" + category).attr('src', newSrc);
                jq("#imgCategory" + category).attr('title', '');
                jq("#imgCategory" + category).attr('alt', '');

            }
        },

        printReport: function() {

            window.print();

        },

        generateReportInNewWindow: function() {
            var reportType = jq.getURLParam("reportType") != null ? parseInt(jq.getURLParam("reportType")) : 0;
            var projectID = jq('#Projects').length != 0 ? jq('#Projects option:selected').val() : "";
            var departmentID = jq('#Departments').length != 0 ? jq('#Departments option:selected').val() : "";
            var userID = jq('#Users').length != 0 ? jq('#Users option:selected').val() : "";
            var tag = jq('#Tags').length != 0 ? jq('#Tags option:selected').val() : "";
            var viewType = jq("#type_rbl_0").length != 0 ? jq("#type_rbl_0").is(':checked') : "";
            var projectStatuses = jq("#cbxViewClosedProjects").length != 0 ? jq("#cbxViewClosedProjects").is(':checked') : "";
            var timeInterval = jq('#TimeIntervals').length != 0 ? jq('#TimeIntervals option:selected').val() : "";
            var noRes = false; 

            var toDate = "";
            var fromDate = "";

            if (reportType == 1 || reportType == 9 || reportType == 10) {
                timeInterval = 1;
                var tmp = (new Date().toUTCString()).split(" ");
                for (var i = 0; i < tmp.length - 1; i++) {
                    fromDate += tmp[i] + " ";
                }
                toDate = jq('#UpcomingIntervals option:selected').val();
            }
            if (reportType == 5 && timeInterval == '0') {
                fromDate = jq("[id$=usersActivityFromDate]").val();
                toDate = jq("[id$=usersActivityToDate]").val();
            }
            if (reportType == 8 && timeInterval == '0') {
                fromDate = jq("[id$=timeSpendFromDate]").val();
                toDate = jq("[id$=timeSpendToDate]").val();
            }

            var milestonesStatus = "";

            var tasksStatus = "";
            if (jq("[id$=cblTaskStatus]").length != 0) {
                if (jq("[id$=cblTaskStatus_2]").is(':checked')) {
                    tasksStatus = tasksStatus + jq("[id$=cblTaskStatus_2]").is(':checked') + ",";
                    tasksStatus = tasksStatus + jq("[id$=cblTaskStatus_2]").is(':checked') + ",";
                }
                else {
                    tasksStatus = tasksStatus + jq("[id$=cblTaskStatus_0]").is(':checked') + ",";
                    tasksStatus = tasksStatus + jq("[id$=cblTaskStatus_1]").is(':checked') + ",";
                }

                if (reportType == 10) {
                    tasksStatus = tasksStatus + false + ",";
                }
                else {
                    tasksStatus = tasksStatus + jq("#cbxShowTasksWithoutResponsible").is(':checked') + ",";
                }
            }
            if (reportType == 11) {
                noRes = true;
            }
            if (reportType == 9) {
                noRes = jq("#cbxShowTasksWithoutResponsible").is(':checked');
            }

            var param = '{ "ReportType" : "' + reportType +
                '", "Project" : "' + projectID +
                '", "Department" : "' + departmentID +
                '", "User" : "' + userID +
                '", "Tag" : "' + tag +
                '", "FromDate" : "' + fromDate +
                '", "ToDate" : "' + toDate +
                '", "ProjectStatuses" : "' + projectStatuses +
                '", "MilestoneStatuses" : "' + milestonesStatus +
                '", "TaskStatuses" : "' + tasksStatus +
                '", "TimeInterval" : "' + timeInterval +
                '", "ViewType" : "' + viewType +
                '", "NoRes" : "' + noRes + '"}';

            var url = AjaxPro.Reports.GetReportUrl(param).value;

            if (jq("#cbxSaveAsTemplate").is(':checked')) {
                var sendEmail = jq("#cbxSendOnEmail").is(':checked');
                var templateTitle = jq("#templateTitle").val();
                var period = jq('#periods option:selected').val();
                var periodItem = jq('#periodItems option:selected').val();
                var hour = jq('#additionalHours option:selected').val();

                AjaxPro.Reports.SaveTemplate(param, templateTitle, period, periodItem, hour, sendEmail);
            }
            jq("#cbxSaveAsTemplate").removeAttr("checked");
            open(url, "displayReportWindow", "status=yes,toolbar=yes,menubar=yes,scrollbars=yes,resizable=yes,location=yes,directories=yes,menubar=yes,copyhistory=yes");
        },

        generateReportByTemplateInNewWindow: function(templateID) {

            var id = parseInt(jq("#Templates option:selected").val());

            if (typeof templateID != "undefined" && templateID != null && templateID > 0)
                id = templateID;

            if (id != -1) {
                var url = "reports.aspx?action=generate&ID=" + id;
                open(url, "displayReportWindow", "status=yes,toolbar=yes,menubar=yes,scrollbars=yes");
            }
            else {
                alert(jq("#Templates option:selected").html());
            }
        },

        exportToCsv: function() {

            window.location.href = window.location.href + "&format=csv";
        },

        exportToHTML: function() {

            window.location.href = window.location.href + "&format=html";
        },

        exportToXml: function() {

            window.location.href = window.location.href + "&format=xml";
        },

        generateReportByUrl: function(url) {
            open(url, "displayReportWindow", "status=yes,toolbar=yes,menubar=yes,scrollbars=yes");
        },

        changeTimeRange: function(DateTimeFormat, mask) {
            var index = jq('#selectDate option:selected').val();
            var currentTime = new Date();

            var from_date = "";
            var to_date = "";

            switch (parseInt(index)) {

                case 0:   // Today

                    from_date = currentTime.format(DateTimeFormat);
                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 1:  // Yesterday

                    currentTime.setDate(currentTime.getDate() - 1);

                    from_date = currentTime.format(DateTimeFormat);
                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 2:  // This week

                    currentTime.setDate(-currentTime.getDay() + currentTime.getDate() + 1);

                    from_date = currentTime.format(DateTimeFormat);

                    currentTime.setDate(currentTime.getDate() + 6);

                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 3:  // Last Week

                    currentTime.setDate(-currentTime.getDay() + 1 + currentTime.getDate() - 7);

                    from_date = currentTime.format(DateTimeFormat);
                    currentTime.setDate(currentTime.getDate() + 6);

                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 4: // This month

                    currentTime.setMonth(currentTime.getMonth() + 1);
                    currentTime.setDate(-1);
                    currentTime.setDate(currentTime.getDate() + 1);

                    to_date = currentTime.format(DateTimeFormat);

                    currentTime.setDate(1);

                    from_date = currentTime.format(DateTimeFormat);

                    break;

                case 5: // Last month

                    currentTime.setMonth(currentTime.getMonth());
                    currentTime.setDate(-1);
                    currentTime.setDate(currentTime.getDate() + 1);

                    to_date = currentTime.format(DateTimeFormat);

                    currentTime.setDate(1);
                    from_date = currentTime.format(DateTimeFormat);

                    break;

                case 6: // This Year

                    currentTime.setFullYear(currentTime.getFullYear(), 0, 1);

                    from_date = currentTime.format(DateTimeFormat);

                    currentTime.setFullYear(currentTime.getFullYear() + 1, 0, 1);

                    currentTime.setDate(0);

                    to_date = currentTime.format(DateTimeFormat);


                    break;

                case 7: // Last Year

                    currentTime.setFullYear(currentTime.getFullYear() - 1, 0, 1);

                    from_date = currentTime.format(DateTimeFormat);

                    currentTime.setFullYear(currentTime.getFullYear() + 1, 0, 1);

                    currentTime.setDate(0);

                    to_date = currentTime.format(DateTimeFormat);

                    break;

                case 8: // Other

                    from_date = jq("[id$=tbxFromDate]").val();
                    to_date = jq("[id$=tbxToDate]").val();

                    break;

            }

            jq("[id$=tbxFromDate]").val(from_date);
            jq("[id$=tbxToDate]").val(to_date);

            jq("[id$=hiddenCurrentTimeRange]").val(index);
            jq("#" + index).attr('selected', 'true');

            ASC.Projects.Reports.setFiltersValueInURL("timeRange", index);

        },


        changeStatusForTasks: function(oldStatus, value) {
            var newStatus = oldStatus;

            var flag = false;

            if (oldStatus != "") {
                if (oldStatus.indexOf(String(value)) > -1) {
                    flag = true;

                    if (!jq("[id$=cblTaskStatus_" + value + "]").is(':checked')) {
                        newStatus = oldStatus.replace(String(value), "");
                    }
                }

                if (!flag) {
                    newStatus += value;
                }
            }
            else {
                for (var i = 0; i < 3; i++) {
                    if (jq("[id$=cblTaskStatus_" + i + "]").is(':checked')) {
                        newStatus += "" + i + "";
                    }
                }
            }

            jq("[id$=HiddenFieldForTaskStatus]").val(newStatus);

            return newStatus;
        },

        changeStatusForMilestones: function(oldStatus, value) {
            var newStatus = oldStatus;

            var flag = false;

            if (oldStatus != "") {

                if (oldStatus.indexOf(String(value)) > -1) {
                    flag = true;

                    if (!jq("[id$=cblMilestoneStatus_" + value + "]").is(':checked')) {
                        newStatus = oldStatus.replace(String(value), "");
                    }
                }

                if (!flag) {
                    newStatus += value;
                }
            }
            else {
                for (var i = 0; i < 3; i++) {
                    if (jq("[id$=cblMilestoneStatus_" + i + "]").is(':checked')) {
                        newStatus += "" + i + "";
                    }
                }
            }

            jq("[id$=HiddenFieldForMilestoneStatus]").val(newStatus);

            return newStatus;
        },

        setFiltersValueInURL: function(key, value) {
            var url = window.location.href.split('#');
            var filters = new Array();
            var flag = false;
            var newFilters = "";
            if (url[1] != null && url[1] != "") {
                filters = url[1].split('&');
            }
            for (var i = 0; i < filters.length; i++) {
                var filterKey = filters[i].split('=')[0];

                if (filterKey == key) {
                    flag = true;

                    if (key == "taskStatus")
                        value = ASC.Projects.Reports.changeStatusForTasks(filters[i].split('=')[1], value);

                    if (key == "milestoneStatus")
                        value = ASC.Projects.Reports.changeStatusForMilestones(filters[i].split('=')[1], value);

                    if (value != "")
                        filters[i] = key + "=" + value;
                    else
                        filters.splice(i, 1);

                }
            }
            if (!flag) {
                if (key == "taskStatus")
                    value = ASC.Projects.Reports.changeStatusForTasks("", value);

                if (key == "milestoneStatus")
                    value = ASC.Projects.Reports.changeStatusForMilestones("", value);

                if (value != "")
                    filters.push(key + "=" + value);
            }
            for (var i = 0; i < filters.length; i++) {
                if (filters[i] != null) {
                    newFilters += filters[i];
                    if (i != filters.length - 1) newFilters += "&";
                }
            }
            window.location.href = url[0] + "#" + newFilters;

        },

        setFiltersValueInHiddenFields: function() {
            var url = window.location.href.split('#');
            var filters = new Array();
            var filtersKey = new Array();
            var filtersValue = new Array();
            if (url[1] != null && url[1] != "") {
                filters = url[1].split('&');
            }
            for (var i = 0; i < filters.length; i++) {
                filtersKey.push(filters[i].split('=')[0]);
                filtersValue.push(filters[i].split('=')[1]);
            }
            for (var i = 0; i < filters.length; i++) {
                if (filtersKey[i] == "departmentID") {
                    jq('[id$=HiddenFieldForDepartment]').val(filtersValue[i]);
                }
                if (filtersKey[i] == "projectID") {
                    jq('[id$=HiddenFieldForProject]').val(filtersValue[i]);
                }
                if (filtersKey[i] == "userID") {
                    jq('[id$=HiddenFieldForUser]').val(filtersValue[i]);
                }
                if (filtersKey[i] == "reportType") {
                    jq('[id$=HiddenFieldForType]').val(filtersValue[i]);
                }
                if (filtersKey[i] == "taskStatus") {
                    jq('[id$=HiddenFieldForTaskStatus]').val(filtersValue[i]);
                }
                if (filtersKey[i] == "milestoneStatus") {
                    jq('[id$=HiddenFieldForMilestoneStatus]').val(filtersValue[i]);
                }
                if (filtersKey[i] == "timeRange") {
                    jq('[id$=hiddenCurrentTimeRange]').val(filtersValue[i]);
                }
            }
        },

        setFiltersValueInURLFromHiddenFields: function() {
            var url = window.location.href.split('#');
            if (jq('[id$=HiddenFieldGetFiltersFromUrl]').val() == "0") {
                var newFilters = "";

                if (jq('[id$=HiddenFieldForDepartment]').length != 0) {
                    newFilters += "departmentID=" + jq('[id$=HiddenFieldForDepartment]').val() + "&";
                }
                if (jq('[id$=HiddenFieldForProject]').length != 0) {
                    newFilters += "projectID=" + jq('[id$=HiddenFieldForProject]').val() + "&";
                }
                if (jq('[id$=HiddenFieldForUser]').length != 0) {
                    newFilters += "userID=" + jq('[id$=HiddenFieldForUser]').val() + "&";
                }
                if (jq('[id$=HiddenFieldForType]').length != 0) {
                    newFilters += "reportType=" + jq('[id$=HiddenFieldForType]').val() + "&";
                }
                if (jq('[id$=HiddenFieldForTaskStatus]').length != 0) {
                    newFilters += "taskStatus=" + jq('[id$=HiddenFieldForTaskStatus]').val() + "&";
                }
                if (jq('[id$=HiddenFieldForMilestoneStatus]').length != 0) {
                    newFilters += "milestoneStatus=" + jq('[id$=HiddenFieldForMilestoneStatus]').val() + "&";
                }
                if (jq('[id$=hiddenCurrentTimeRange]').length != 0) {
                    newFilters += "timeRange=" + jq('[id$=hiddenCurrentTimeRange]').val() + "&";
                }

                window.location.href = url[0] + "#" + newFilters;
            }
        },

        setFiltersValueInObjectsFromURL: function() {
            var reportType = jq.getURLParam("reportType") != null ? parseInt(jq.getURLParam("reportType")) : 0;

            switch (reportType) {
                case 0:
                    var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                    jq("#ddlTag" + tag).attr("selected", "selected");
                    var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                    jq("#ddlProject" + prj).attr("selected", "selected");
                    ASC.Projects.Reports.changeTag(tag, prj, "-1");
                    break;
                case 1:
                    var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                    jq("#ddlTag" + tag).attr("selected", "selected");
                    var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                    jq("#ddlProject" + prj).attr("selected", "selected");
                    var fto = jq.getURLParam("fto") != null ? jq.getURLParam("fto") : 0;
                    if (fto == 0) {
                        jq("#UpcomingIntervals option:first").attr("selected", "selected");
                    }
                    else {
                        var fromDate = new Date();
                        fromDate.setHours(0, 0, 0, 0);
                        var toDate = new Date(parseInt(fto.substring(0, 4)), parseInt(fto.substring(4, 6)) - 1, parseInt(fto.substring(6, 8)));
                        var days = Math.floor((toDate.getTime() - fromDate.getTime()) / (1000 * 60 * 60 * 24));
                        jq("#ddlUpcomingInterval" + days).attr("selected", "selected");
                    }
                    ASC.Projects.Reports.changeTag(tag, prj, "-1");
                    break;
                case 2:
                    var fv = CurrFilter.ViewType;
                    ASC.Projects.Reports.changeReportType1(fv);
                    if (fv == 0) {
                        var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                        jq("#ddlDepartment" + dep).attr("selected", "selected");
                        var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                        jq("#ddlUser" + user).attr("selected", "selected");
                        ASC.Projects.Reports.changeDepartment(dep, user);
                    }
                    if (fv == 1) {
                        var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                        jq("#ddlTag" + tag).attr("selected", "selected");
                        var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                        jq("#ddlProject" + prj).attr("selected", "selected");
                        var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                        jq("#ddlUser" + user).attr("selected", "selected");
                        ASC.Projects.Reports.changeTag(tag, prj, user);
                        ASC.Projects.Reports.changeProject(tag, prj, user);
                    }
                    break;
                case 3:
                    var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                    jq("#ddlDepartment" + dep).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    ASC.Projects.Reports.changeDepartment(dep, user);
                    break;
                case 4:
                    var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                    jq("#ddlDepartment" + dep).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    ASC.Projects.Reports.changeDepartment(dep, user);
                    break;
                case 5:
                    var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                    jq("#ddlDepartment" + dep).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    var timeInterval = CurrFilter.TimeInterval;
                    var ftime = jq.getURLParam("ftime") != null ? jq.getURLParam("ftime") : "";
                    if (timeInterval == 0) {
                        if (ftime == "absolute") {
                            jq("#ddlTimeInterval0").attr("selected", "selected");
                            jq("[id$=usersActivityFromDate]").val(CurrFilterDate.ForomDate);
                            jq("[id$=usersActivityToDate]").val(CurrFilterDate.ToDate);
                            jq("#otherInterval").show();
                        }
                        else jq("#ddlTimeInterval6").attr("selected", "selected");
                    }
                    else jq("#ddlTimeInterval" + timeInterval).attr("selected", "selected");
                    ASC.Projects.Reports.changeDepartment(dep, user);
                    break;
                case 6:
                    var fv = CurrFilter.ViewType;
                    ASC.Projects.Reports.changeReportType1(fv);
                    if (fv == 0) {
                        var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                        jq("#ddlDepartment" + dep).attr("selected", "selected");
                        var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                        jq("#ddlUser" + user).attr("selected", "selected");
                        ASC.Projects.Reports.changeDepartment(dep, user);
                    }
                    if (fv == 1) {
                        var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                        jq("#ddlTag" + tag).attr("selected", "selected");
                        var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                        jq("#ddlProject" + prj).attr("selected", "selected");
                        var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                        jq("#ddlUser" + user).attr("selected", "selected");
                        ASC.Projects.Reports.changeTag(tag, prj, user);
                        ASC.Projects.Reports.changeProject(tag, prj, user);
                    }
                    break;
                case 7:
                    var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                    jq("#ddlDepartment" + dep).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    if (CurrFilter.ProjectStatuses.length == 0)
                        jq("#cbxViewClosedProjects").attr("checked", "checked");
                    ASC.Projects.Reports.changeDepartment(dep, user);
                    break;
                case 8:
                    var dep = CurrFilter.DepartmentId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.DepartmentId;
                    jq("#ddlDepartment" + dep).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    var timeInterval = CurrFilter.TimeInterval;
                    var ftime = jq.getURLParam("ftime") != null ? jq.getURLParam("ftime") : "";
                    if (timeInterval == 0) {
                        if (ftime == "absolute") {
                            jq("#ddlTimeInterval0").attr("selected", "selected");
                            jq("[id$=timeSpendFromDate]").val(CurrFilterDate.ForomDate);
                            jq("[id$=timeSpendToDate]").val(CurrFilterDate.ToDate);
                            jq("#otherInterval").show();
                        }
                        else jq("#ddlTimeInterval6").attr("selected", "selected");
                    }
                    else jq("#ddlTimeInterval" + timeInterval).attr("selected", "selected");
                    if (CurrFilter.ViewType == 1) jq("#type_rbl_1").attr("checked", "checked");
                    ASC.Projects.Reports.changeDepartment(dep, user);
                    break;
                case 9:
                    var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                    jq("#ddlTag" + tag).attr("selected", "selected");
                    var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                    jq("#ddlProject" + prj).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    var fto = jq.getURLParam("fto") != null ? jq.getURLParam("fto") : 0;
                    if (fto == 0) {
                        jq("#UpcomingIntervals option:first").attr("selected", "selected");
                        if (CurrFilterDate.TStatus == 1)
                            jq("#UpcomingIntervals").attr("disabled", "disabled");
                    }
                    else {
                        var fromDate = new Date();
                        fromDate.setHours(0, 0, 0, 0);
                        var toDate = new Date(parseInt(fto.substring(0, 4)), parseInt(fto.substring(4, 6)) - 1, parseInt(fto.substring(6, 8)));
                        var days = Math.floor((toDate.getTime() - fromDate.getTime()) / (1000 * 60 * 60 * 24));
                        jq("#ddlUpcomingInterval" + days).attr("selected", "selected");
                    }
                    ASC.Projects.Reports.changeTag(tag, prj, user);
                    ASC.Projects.Reports.changeProject(tag, prj, user);
                    switch (CurrFilterDate.TStatus) {
                        case 0:
                            jq("[id$=cblTaskStatus_0]").attr("checked", "checked");
                            break;
                        case 1:
                            jq("[id$=cblTaskStatus_1]").attr("checked", "checked");
                            break;
                        case 2:
                            jq("[id$=cblTaskStatus_2]").attr("checked", "checked");
                            break;
                        case 3:
                            jq("[id$=cblTaskStatus_0]").attr("checked", "checked");
                            jq("#cbxShowTasksWithoutResponsible").attr("checked", "checked");
                            break;
                        case 4:
                            jq("[id$=cblTaskStatus_1]").attr("checked", "checked");
                            jq("#cbxShowTasksWithoutResponsible").attr("checked", "checked");
                            break;
                        case 5:
                            jq("[id$=cblTaskStatus_2]").attr("checked", "checked");
                            jq("#cbxShowTasksWithoutResponsible").attr("checked", "checked");
                            break;
                    }
                    break;
                case 10:
                    var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                    jq("#ddlTag" + tag).attr("selected", "selected");
                    var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                    jq("#ddlProject" + prj).attr("selected", "selected");
                    var user = CurrFilter.UserId == "00000000-0000-0000-0000-000000000000" ? -1 : CurrFilter.UserId;
                    jq("#ddlUser" + user).attr("selected", "selected");
                    var fto = jq.getURLParam("fto") != null ? jq.getURLParam("fto") : 0;
                    if (fto == 0) {
                        jq("#UpcomingIntervals option:first").attr("selected", "selected");
                        if (CurrFilterDate.TStatus == 1)
                            jq("#UpcomingIntervals").attr("disabled", "disabled");
                    }
                    else {
                        var fromDate = new Date();
                        fromDate.setHours(0, 0, 0, 0);
                        var toDate = new Date(parseInt(fto.substring(0, 4)), parseInt(fto.substring(4, 6)) - 1, parseInt(fto.substring(6, 8)));
                        var days = Math.floor((toDate.getTime() - fromDate.getTime()) / (1000 * 60 * 60 * 24));
                        jq("#ddlUpcomingInterval" + days).attr("selected", "selected");
                    }
                    ASC.Projects.Reports.changeTag(tag, prj, user);
                    ASC.Projects.Reports.changeProject(tag, prj, user);
                    switch (CurrFilterDate.TStatus) {
                        case 0:
                            jq("[id$=cblTaskStatus_0]").attr("checked", "checked");
                            break;
                        case 1:
                            jq("[id$=cblTaskStatus_1]").attr("checked", "checked");
                            break;
                        case 2:
                            jq("[id$=cblTaskStatus_2]").attr("checked", "checked");
                            break;
                    }
                    jq("#cbxShowTasksWithoutResponsible").attr("disabled", "disabled");
                    break;
                case 11:
                    var tag = CurrFilter.ProjectTag == null ? "" : CurrFilter.ProjectTag;
                    jq("#ddlTag" + tag).attr("selected", "selected");
                    var prj = CurrFilter.ProjectIds.length == 1 ? CurrFilter.ProjectIds[0] : "-1";
                    jq("#ddlProject" + prj).attr("selected", "selected");
                    ASC.Projects.Reports.changeTag(tag, prj, "-1");
                    break;
            }
        },

        deleteTemplates: function(WhichTemplates) {
            var IsExistsChecked = false;
            var templatesCount = jq("[id^=selectTmpl]").length;

            jq("[id^=selectTmpl]").each
            (
                function() {
                    if (jq(this).is(':checked') === true || WhichTemplates === "all") {
                        IsExistsChecked = true;
                    }
                }
            )
            if (IsExistsChecked === false) {
                alert(ProjectJSResources.SelectedTemplatesInfo);
                return;
            }

            if (confirm(ProjectJSResources.ConfirmTemplateDeleting)) {
                var TemplatesToDel = new Array();
                jq("[id^=selectTmpl]").each
                (
                    function() {
                        if ((WhichTemplates === "selected" && jq(this).is(':checked') === true) || WhichTemplates === "all")
                            TemplatesToDel.push(jq(this).attr("id").substring(10, jq(this).attr("id").length));
                    }
                );
                if (TemplatesToDel.length > 0) {
                    AjaxPro.Templates.DeleteTemplates(TemplatesToDel, function(res) {
                        var Out = new Array();
                        Out = res.value;
                        for (var i = 0; i < Out.length; i++) {
                            jq("#Template" + Out[i]).remove();
                        }
                        if (templatesCount == Out.length) {
                            jq("#divTemplateList").hide();
                            jq("#emptyScreenControl").show();
                        }
                    })
                }
            }
        },

        deleteAllTemplates: function() {
            if (confirm(ProjectJSResources.ConfirmTemplateDeleting)) {
                AjaxPro.Reports.DeleteAllTemplates(function(res) {
                    if (res.error != null) { alert(res.error.Message); return; }
                    location.reload();
                });
            }
        },

        deleteAutoTemplates: function() {
            if (confirm(ProjectJSResources.ConfirmTemplateDeleting)) {
                AjaxPro.Reports.DeleteAutoTemplates(function(res) {
                    if (res.error != null) { alert(res.error.Message); return; }
                    location.reload();
                });
            }
        },

        EditTemplate: function(Id, Name, period, periodItem, hour, AutoGenerated) {
            if (arguments.length === 6 && Id > 0) {
                AjaxPro.Reports.UpdateTemplate(Id, Name, period, periodItem, hour, AutoGenerated, function(res) {
                    if (res.value !== undefined) {
                        if (res.value.Message === "success") {
                            Name = res.value.Name;
                            jq("#TmplName" + Id).html(Name);
                            jq("#AutoGenValue" + Id).val(AutoGenerated);
                            if (AutoGenerated === true) {
                                //jq("#AutoGenImage" + Id).show();
                                //jq("#AutoGenImage" + Id).attr('title',AjaxPro.Templates.GetAutugeneratedTime(res.value.Cron).value); 
                                jq("#AutoGenTime" + Id).show();
                                jq("#AutoGenTime" + Id).html(AjaxPro.Templates.GetAutugeneratedTime(res.value.Cron).value);
                            }
                            else {
                                //jq("#AutoGenImage" + Id).hide();
                                jq("#AutoGenTime" + Id).hide();
                            }
                            //jq("#TmplEditLink" + Id).attr("onclick", "javascript:viewReportTemplateContainer("+Id+ ", '"+Name+"', "+period+", "+periodItem+", "+hour+", "+AutoGenerated+");");
                            //jq("#TmplEditLink" + Id).click("javascript:viewReportTemplateContainer("+Id+ ", '"+Name+"', "+period+", "+periodItem+", "+hour+", "+AutoGenerated+");");
                            jq("#TmplEditLink" + Id).click(function() { return viewReportTemplateContainer(Id, Name, period, periodItem, hour, AutoGenerated); });
                        }
                    }
                });
            }
        }

    }
})();
