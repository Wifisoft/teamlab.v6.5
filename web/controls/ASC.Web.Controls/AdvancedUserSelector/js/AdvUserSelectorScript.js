if (typeof (ASC) == 'undefined')
    ASC = { };
if (typeof (ASC.Controls) == 'undefined')
    ASC.Controls = { };


function jqHtmlEncode(html) {
    if (html == undefined)
        return '';
        
    return jq('<div/>').text(html).html();
}

jq(function() {

    ASC.Controls.AdvancedUserSelector.Groups = new Array();

    var data = ASC.Controls.AdvancedUserSelector.Groups;
    var groups = jq.parseJSON(jq.base64.decode(ASC.Controls.AdvancedUserSelector._groups));
    groups = groups ? groups.response : [];
    var profiles = jq.parseJSON(jq.base64.decode(ASC.Controls.AdvancedUserSelector._profiles));
    profiles = profiles ? profiles.response : [];

    var commonGroup = new ASC.Controls.AdvancedUserSelector.Group(ASC.Controls.AdvancedUserSelector.EmptyId, 'All Departments');
    data.push(commonGroup);

    if (groups.length == 0)
        groups.push({ id: ASC.Controls.AdvancedUserSelector.EmptyId, name: 'All Departments' });

    for (var i = 0, n = groups.length; i < n; i++) {

        var gr = groups[i];
        var curGroup = null;
        for (var k = 0; k < data.length; k++) {
            if (data[i].ID == gr.id) {
                curGroup = data[i];
                break;
            }
        }
        if (curGroup == null) {
            curGroup = new ASC.Controls.AdvancedUserSelector.Group(gr.id, jqHtmlEncode(gr.name));
            data.push(curGroup);
        }

        //users
        for (var j = 0, m = profiles.length; j < m; j++) {

            var user = profiles[j];

            var group = null;
            if (user.groups == null || user.groups.length == 0) {
                var group = commonGroup;
            } else {
                for (var k = 0, t = user.groups.length; k < t; k++) {
                    if (user.groups[k].id == curGroup.ID) {
                        group = curGroup;
                        break;
                    }
                }
            }

            if (group != null) {
                var exists = false;
                for (var k = 0, t = group.Users.length; k < t; k++) {
                    if (group.Users[k].ID == user.id) {
                        exists = true;
                        break;
                    }
                }

                if (!exists) {
                    group.Users.push(new ASC.Controls.AdvancedUserSelector.User(user.id,
                    user.displayName,
                    group, jqHtmlEncode(user.title), user.avatarSmall));
                }
            }
        }
    }

});

ASC.Controls.AdvancedUserSelector = new function() {


    this.EmptyId = "00000000-0000-0000-0000-000000000000";
    this.Group = function(id, name) {
        this.Name = name;
        this.ID = id;
        this.Users = new Array();

        this.Clone = function() {
            var gr = { ID: this.ID, Name: this.Name, Users: new Array() };
            for (var i = 0; i < this.Users.length; i++) {
                gr.Users.push(this.Users[i].Clone());
            }
            return gr;
        }
    };

    this.User = function(id, name, group, Title, PhotoUrl) {
        this.ID = id;
        this.Name = name;
        this.Group = group;
        this.Title = Title;
        this.PhotoUrl = PhotoUrl;
        this.Hidden = false;

        this.Clone = function() {
            return { ID: this.ID, Name: this.Name, Group: this.Group, Title: this.Title, PhotoUrl: this.PhotoUrl, Hidden: this.Hidden };
        }
    };

    this.UserSelectorPrototype = function(id, objName, EmptyUserListMsg, clearLinkTitle, mobileVersion, isLinkView, linkText) {
        this.AllDepartmentsGroupName = '';
        this.ID = id;
        this.ObjName = objName;
        this.Groups = new Array();
        this.EmptyUserListMsg = EmptyUserListMsg;
        this.ClearLinkTitle = clearLinkTitle;
        this.SelectedDepartmentId = ASC.Controls.AdvancedUserSelector.EmptyId;
        this.SelectedUserId = null;
        this.LastUserNameBegin = "";
        this.SelectedUserName = "",
        this.MobileVersion = mobileVersion === true;
        this.IsLinkView = isLinkView;
        this.AdditionalFunction = function() { };
        this.LinkText = linkText;
        this.Me = function() {
            return jq("#" + this.ObjName);
        };

        var selector = this;
        jq(function() {

            commonGroup = ASC.Controls.AdvancedUserSelector.Groups[0].Clone();
            commonGroup.Name = selector.AllDepartmentsGroupName;
            commonGroup.Users = new Array();

            for (var i = 0; i < ASC.Controls.AdvancedUserSelector.Groups.length; i++) {

                if (typeof (selector.UserIDs) != 'undefined' && typeof (selector.UserIDs) != undefined && selector.UserIDs != null && selector.UserIDs.length > 0) {

                    //custom user list
                    var gr = ASC.Controls.AdvancedUserSelector.Groups[i];
                    for (var k = 0; k < gr.Users.length; k++) {

                        for (var j = 0; j < selector.UserIDs.length; j++) {
                            if (gr.Users[k].ID == selector.UserIDs[j]) {
                                commonGroup.Users.push(gr.Users[k].Clone());
                                break;
                            }
                        }
                    }

                    if (i == 0)
                        selector.Groups.push(commonGroup);

                }
                else {

                    //filter by disabled users
                    var gr = ASC.Controls.AdvancedUserSelector.Groups[i].Clone();
                    if (gr.ID == ASC.Controls.AdvancedUserSelector.EmptyID)
                        gr.Name = selector.AllDepartmentsGroupName;

                    if (typeof (selector.DisabledUserIDs) != undefined && typeof (selector.DisabledUserIDs) != 'undefined') {
                        for (var j = 0; j < selector.DisabledUserIDs.length; j++) {
                            for (var k = 0; k < gr.Users.length; k++) {
                                if (gr.Users[k].ID == selector.DisabledUserIDs[j]) {
                                    gr.Users.splice(k, 1);
                                    k--;
                                }
                            }
                        }
                    }
                    selector.Groups.push(gr);
                }
            }
        });

        this.GetAllDepartments = function() {
            if (this.MobileVersion || this.Me().find('#divDepartments').html() !== "")
                return;

            var groupList = new String();
            for (var i = 0; i < this.Groups.length; i++)
                groupList += "<div id='Department_" + this.Groups[i].ID + "' class='adv-userselector-dep" + (i == 0 ? " adv-userselector-userHover' " : "' ") +
                    "onclick='javascript:" + this.ObjName + ".ChangeDepartment(\"" + this.Groups[i].ID + "\");'>" + this.Groups[i].Name + "</div>";
            this.Me().find('#divDepartments').html(groupList);
        };

        this.GetAllUsers = function() {
            if (this.MobileVersion)
                return;

            var userArray = new Array();
            var tempDiv = jq(document.createElement('div'));
            for (var i = 0; i < this.Groups.length; i++) {
                if (this.SelectedDepartmentId == ASC.Controls.AdvancedUserSelector.EmptyId ||
                    this.Groups[i].ID === this.SelectedDepartmentId) {
                    var userName = '';
                    for (var j = 0; j < this.Groups[i].Users.length; j++) {
                        userName = this.Groups[i].Users[j].Name.toLowerCase();
                        userName = tempDiv.html(userName).text();
                        if (userName.indexOf(this.LastUserNameBegin) != -1 && this.Groups[i].Users[j].Hidden == false)
                            userArray.push(this.Groups[i].Users[j]);
                    }
                }
            }

            userArray.sort(function(a, b) {
                return (a.Name.toLowerCase() > b.Name.toLowerCase() ? 1 : -1);
            });

            var userList = new String();
            for (var i = 0; i < userArray.length; i++) {

                var name = userArray[i].Name;
                var title = userArray[i].Title;

                userList += "<div id='User_" + userArray[i].ID + "' class='adv-userselector-user' onclick='javascript:" + this.ObjName + ".SelectUser(this);'>" +
                    "<img style='float:left;padding:4px;' alt='' src=" + userArray[i].PhotoUrl + "></img><div><div id='userName' style='padding:4px 0 0 40px;font-weight: bold;'>" +
                        name + "</div><div class='textBigDescribe' style='padding-left:40px;'>" + title + "</div></div></div>";
            }

            if (userList == "")
                userList = "<div style='padding:4px;'>" + this.EmptyUserListMsg + "</div>";

            this.Me().find('#divUsers').html(userList);

            this.SelectCurUserInDept();
        };

        this.RenderItems = function() {
            if (this.MobileVersion)
                return;

            this.SelectedUserName = this.Me().find('#inputUserName').val().trim();
            this.LastUserNameBegin = this.SelectedUserName.toLowerCase();
            this.GetAllDepartments();
            this.ChangeDepartment();
            this.Me().find("#DepsAndUsersContainer").toggle();
        };

        this.ChangeDepartment = function(DepId) {
            if (this.MobileVersion)
                return;

            this.Me().find('#divDepartments .adv-userselector-userHover').removeClass('adv-userselector-userHover');
            if (!DepId) {
                DepId = ASC.Controls.AdvancedUserSelector.EmptyId;
            }
            var selectedDept = this.Me().find('#Department_' + DepId);
            selectedDept.addClass('adv-userselector-userHover');

            this.SetFocusToSelectedElement('divDepartments');
            this.SelectedDepartmentId = DepId;
            this.GetAllUsers();
        };

        this.SelectUser = function(obj) {
            if (this.MobileVersion) {
                this.SelectedUserId = jq(obj).val();
                if (this.SelectedUserId == "" || this.SelectedUserId == "-1") {
                    this.SelectedUserId = null;
                    return;
                }
                var UserName = jq(obj).find("option:selected").text().trim();
                this.SelectedUserName = UserName;
                this.LastUserNameBegin = UserName.toLowerCase();
                this.AdditionalFunction(this.SelectedUserId, UserName);
                return;
            }

            this.SelectedUserId = obj.id.replace('User_', '');
            var UserName = jq.trim(jq(obj).find('#userName').text());
            this.Me().find('#inputUserName').val(UserName);
            this.Me().find('#DepsAndUsersContainer').hide();
            this.SelectedUserName = UserName;
            this.LastUserNameBegin = UserName.toLowerCase();

            this.Me().find("#peopleImg").show();
            this.Me().find("#searchImg").hide();

            this.AdditionalFunction(this.SelectedUserId, UserName);
        };

        this.ClearFilter = function() {
            if (this.MobileVersion)
                return;

            this.Me().find('#inputUserName').val("");
            var BegOfUserName = "";
            if ((this.LastUserNameBegin === BegOfUserName) && (this.LastUserNameBegin != ""))
                return;
            this.SelectedUserName = "";
            this.LastUserNameBegin = BegOfUserName;
            this.GetAllDepartments();
            this.GetAllUsers();

            if (this.Me().find('#DepsAndUsersContainer:visible').length < 1)
                this.Me().find('#DepsAndUsersContainer').show();

            this.SelectedUserId = null;
        };

        this.SuggestUser = function(event) {
            if (this.MobileVersion)
                return;

            this.GetAllDepartments();
            if (event.keyCode != 9 && event.keyCode != 13)
                this.SelectedUserId = null;
            var UserName = this.Me().find('#inputUserName').val().trim();
            var BegOfUserName = UserName.toLowerCase();
            if (this.LastUserNameBegin === BegOfUserName)
                return;
            this.SelectedUserName = UserName;
            this.LastUserNameBegin = BegOfUserName;
            this.GetAllUsers();
            if (this.Me().find('#DepsAndUsersContainer:visible').length < 1)
                this.Me().find('#DepsAndUsersContainer').show();

            this.SelectCurUserInDept(true);

        };


        this.HideUser = function(userID, hide) {
            if (hide == undefined)
                hide = true;

            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].ID == userID)
                        this.Groups[i].Users[j].Hidden = (hide == true);
                }
            }
            this.SelectedUserId = null;

            if (this.MobileVersion) {
                if (hide) {
                    var elem = this.Me().find("option[value='" + userID + "']");

                    if (elem.parent().is("optgroup") && elem.parent().children().length == 1)
                        elem.parent().remove();
                    else
                        elem.remove();

                    this.Me().find('option:not(:disabled):first').attr('selected', 'selected');
                    this.SelectedUserId = this.Me().find("select").val();
                } else {
                    this.RenderMobileControl();
                }
            }
        };

        this.DisplayAll = function() {
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    this.Groups[i].Users[j].Hidden = false;
                }
            }

            if (this.MobileVersion) {
                this.RenderMobileControl();
            }
        };

        this.RenderMobileControl = function() {
            if (!this.MobileVersion) return;

            var selectBox = this.Me().find("select").empty();
            var sb = "<option style='max-width:300px;' value='{0}' {2}>{1}</option>".format(
                -1,
                this.LinkText,
                this.SelectedUserId === null ? "selected = 'selected'" : "");
            for (var i = 0; i < this.Groups.length; i++) {
                var sbTmpDep = "";
                if (this.Groups[i].ID != ASC.Controls.AdvancedUserSelector.EmptyId)
                    sbTmpDep = "<optgroup label='{0}' style='max-width:300px;'>".format(this.Groups[i].Name);

                var sbTmpUser = "";
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].Hidden == false) {
                        sbTmpUser += "<option style='max-width:300px;' value='{0}' {2}>{1}</option>".format(
                            this.Groups[i].Users[j].ID,
                            this.Groups[i].Users[j].Name,
                            this.Groups[i].Users[j].ID == this.SelectedUserId ? "selected = 'selected'" : "");
                    }
                }

                if (sbTmpUser != "") {
                    sb += sbTmpDep + sbTmpUser;
                    if (this.Groups[i].ID != ASC.Controls.AdvancedUserSelector.EmptyId)
                        sb += "</optgroup>";
                }
            }

            selectBox.html(sb);
            this.SelectedUserId = selectBox.val();
        };

        this.dropdownRegAutoHide = function(event) {
            if (this.MobileVersion)
                return;

            var Obj = jq((event.target) ? event.target : event.srcElement).parents().andSelf();
            var hide = true;
            for (var i = 0; i < Obj.length; i++) {
                if (Obj[i] === this.Me().find("#uSelector")[0] || Obj[i] === this.Me().find("#DepsAndUsersContainer")[0]) {
                    hide = false;
                    break;
                }
            }
            if (hide)
                this.Me().find("#DepsAndUsersContainer").hide();
        };

        this.OnInputClick = function(el, event) {

            this.ClearFilter();
            this.Me().find("#peopleImg").hide();
            this.Me().find("#searchImg").show();
            this.SetFocusToSelectedElement();
            if (this.Me().find('#DepsAndUsersContainer').css("display") == "none") {
                jq("body").click();
                this.GetAllDepartments(); this.GetAllUsers();
            }
            this.SetFocusToSelectedElement('divUsers');
            divID = 'divUsers';
            this.Me().find('#DepsAndUsersContainer').css("display", "block");
            el.CancelBubble(event);
        };

        this.CancelBubble = function(event) {
            try {
                if (event) {
                    event.cancelBubble = true;
                    event.stopPropagation();
                }
            } catch (err) { }
        };

        this.ChangeSelection = function(e) {

            if (this.ChangeSelectedElement(e)) {
                var code = this.GetKeyCode(e);

                if (code == 13) {
                    var userID = jq('#login').val();
                    if (userID) {
                        this.SelectUser(document.getElementById('User_' + userID));
                    }
                    this.CancelBubble(e);
                    return 13;
                }
                return;
            }
        };

        this.ChangeSelectedElement = function(e) {
            if (e == null) {
                return false;
            }
            var code = this.GetKeyCode(e);

            if (!window.divID) {
                divID = 'divUsers';
            }
            var changedElement = false;

            if (code == 37)//left
            {
                divID = 'divUsers';
                this.SelectCurUserInDept();
            }
            if (code == 39) { //right
                divID = 'divDepartments';
            }

            if (code == 38)//up
            {
                this.GetPrevChild(divID);
                changedElement = true;
            }
            if (code == 40) { //down
                this.GetNextChild(divID);
                changedElement = true;
            }

            this.SetFocusToSelectedElement(divID);

            if (code == 13) {
                changedElement = true;
            }
            return changedElement;
        };

        this.GetKeyCode = function(e) {
            var code;
            if (!e) var e = window.event;
            if (e.keyCode) code = e.keyCode;
            else if (e.which) code = e.which;
            return code;
        };

        this.SelectCurUserInDept = function(setFocus) {
            if (setFocus) {
                divID = 'divUsers';
                this.SetFocusToSelectedElement(divID);
            }

            this.GetPrevChild('divUsers');
            this.GetNextChild('divUsers');
        };

        this.GetNextChild = function(divID) {
            var selectedEl = this.Me().find('#' + divID + ' .adv-userselector-userHover');
            var next = selectedEl.size() == 1 ? selectedEl.next() : null;
            if (!next || next == null || next.size() == 0) {
                next = jq(this.Me().find('#' + divID).children()[1]);
            }
            this.SetSelectedElement(next, divID);
        };

        this.GetPrevChild = function(divID) {
            var selectedEl = this.Me().find('#' + divID + ' .adv-userselector-userHover');
            var next = selectedEl.size() == 1 ? selectedEl.prev() : null;
            if (!next || next == null || next.size() == 0) {
                var children = this.Me().find('#' + divID).children();
                next = jq(children[children.length - 1]);
            }
            this.SetSelectedElement(next, divID);
        };

        this.SetSelectedElement = function(next, divID) {
            this.Me().find('#' + divID + ' .adv-userselector-userHover').removeClass('adv-userselector-userHover');
            next.addClass('adv-userselector-userHover');
            this.ScrollDivToPosition(next, divID);
            if (divID == 'divUsers' && next.length > 0) {
                var elID = next.attr('id');
                if (elID != undefined && elID != '') {
                    elID = elID.replace('User_', '');
                    jq('#login').val(elID);
                }
            }
            if (divID == 'divDepartments') {
                next.click();
            }
        };

        this.ScrollDivToPosition = function(el, divID) {
            var offsetTop = el.attr('offsetTop') - 87;
            if (offsetTop < 0) {
                offsetTop = 0;
            }
            this.Me().find('#' + divID).scrollTo(offsetTop);
        };

        this.SetFocusToSelectedElement = function(id) {
            this.Me().find('.tintMedium').removeClass('tintMedium');
            if (id) {
                this.Me().find('#' + id + ' .adv-userselector-userHover').addClass('tintMedium');
            }
        };
    };
};