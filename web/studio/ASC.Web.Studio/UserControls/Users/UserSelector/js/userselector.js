if (typeof (ASC) == 'undefined')
    ASC = {};

if (typeof(ASC.Studio) == 'undefined')
    ASC.Studio = {};

ASC.Studio.UserSelector = new function() {

    this.UserGroupItem = function(id, name) {
        this.Name = name;
        this.ID = id;
        this.Users = new Array();
        this.PreSelected = false;
    };

    this.UserItem = function(id, name, selected, group, defaultSelected, title) {
        this.ID = id;
        this.Name = name;
        this.Selected = selected;
        this.Group = group;
        this.PreSelected = false;
        this.Disable = false;
        this.DefaultSelected = defaultSelected;
    	this.Title = title;
    };

    this.UserSelectorPrototype = function(id, objName, mobileVersion) {
        this.ID = id;
        this.ObjName = objName;
        this.Groups = new Array();

        this.OnCancelButtonClick = null;
        this.OnOkButtonClick = null;

        this.IsFirstVisit = true;
        this.MobileVersion = mobileVersion == true;

        this.RenderItems = function() {
            var userList = new String();
            var userListM = new String();
            var selectedUserList = new String();
            var selectedUserArray = new Array();

            for (var i = 0; i < this.Groups.length; i++) {
                var group = this.Groups[i];
                var isEmpty = true;

                for (var j = 0; j < group.Users.length; j++) {
                    var user = this.Groups[i].Users[j];
                    if (user.Disable)
                        continue;

                    if (this.MobileVersion) {
                        if (isEmpty && group.Name != '') {
                            userListM += "<optgroup class='tintLight' label='{0}' style='max-width:300px;'>".format(group.Name);
                            isEmpty = false;
                        }


                        userListM += "<option class='tintMedium' style='cursor:pointer; max-width:300px;' value='{0}' {2}>{1}</option>".format(
                            user.ID,
                            user.Name,
                            user.Selected ? "selected = 'selected'" : "");


                    } else {

                        if (user.Selected) {
                            selectedUserArray.push(user);
                        }
                        else {
                            if (isEmpty && group.Name != '') {
                                var groupChecked = '';
                                if (group.PreSelected) {
                                    groupChecked = 'checked="checked"';
                                }

                                userList += '<div class="clearFix">';
                                userList += '<div style="float:left; padding-top:5px; margin-rigth:5px;"><input onclick="' + this.ObjName + '.PreSelectGroup(\'' + group.ID + '\');" id="usrdialog_' + this.ID + '_dep_' + group.ID + '" type="checkbox" ' + groupChecked + ' /></div>';


                                userList += '<div class="depBox" style="cursor:pointer;"><label for="usrdialog_' + this.ID + '_dep_' + group.ID + '">' + group.Name + '</label></div>';
                                userList += '</div>';

                                userList += '<div class="depBoxContent">';
                                isEmpty = false;
                            }

                            userList += '<div class="usrBox">';

                            var checked = '';
                            if (user.PreSelected) {
                                checked = 'checked="checked"';
                            }

                            userList += '<input onclick="' + this.ObjName + '.PreSelectUser(\'' + user.ID + '\');" id="usrdialog_' + this.ID + '_usr_' + user.ID + '" type="checkbox" ' + checked + ' />';

                            userList += '<label for="usrdialog_' + this.ID + '_usr_' + user.ID + '">' + user.Name + '</label>';
                            userList += '</div>';
                        }
                    }
                }

                if (isEmpty == false && group.Name != '') {
                    if (this.MobileVersion)
                        userListM += "</optgroup>";
                    else
                        userList += '</div>';

                }
            }


            if (this.MobileVersion) {
                jq("#selectmobile_" + this.ID).html(userListM);
            } else {

                selectedUserArray.sort(function(a, b) {
                    if (a.Name > b.Name)
                        return 1;
                    if (a.Name < b.Name)
                        return -1;
                    return 0;
                });
                for (var i = 0; i < selectedUserArray.length; i++) {
                    selectedUserList += '<div class="usrBox">';
                    selectedUserList += '<input onclick="' + this.ObjName + '.PreSelectUser(\'' + selectedUserArray[i].ID + '\');" id="usrdialog_' + this.ID + '_usr_' + selectedUserArray[i].ID + '"" dep="' + group.ID + '" type="checkbox"/>';
                    selectedUserList += '<label for="usrdialog_' + this.ID + '_usr_' + selectedUserArray[i].ID + '">' + selectedUserArray[i].Name + '</label>';
                    selectedUserList += '</div>';
                }

                jq('#usrdialog_leftBox_' + this.ID).html(userList);
                jq('#usrdialog_rightBox_' + this.ID).html(selectedUserList);
            }
        };

        this.ShowNewDialog = function(zIdex) {
            this.ClearSelection();
            this.ShowDialog(zIdex);
        };

        this.ShowDialog = function(zIdex) {

            if (this.IsFirstVisit) {
                this.IsFirstVisit = false;
            }
            else {
                this.Unselect();
                for (var i = 0; i < this.Groups.length; i++) {
                    var group = this.Groups[i];
                    for (var j = 0; j < group.Users.length; j++) {
                        var user = this.Groups[i].Users[j];
                        user.Selected = user.DefaultSelected;
                    }
                }
            }

            this.RenderItems();

            var zi = 66666;
            if (zIdex != undefined && zIdex != null)
                zi = zIdex;

            try {

                var margintop = jq(window).scrollTop() - 100;
                margintop = margintop + 'px';

                jq.blockUI({ message: jq("#usrdialog_" + this.ID),
                    css: {
                        left: '50%',
                        top: '25%',
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: this.MobileVersion ? "400px" : "750px",
                        cursor: 'default',
                        textAlign: 'left',
                        position: 'absolute',
                        'margin-left': this.MobileVersion ? "-200px" : "-375px",
                        'margin-top': margintop,
                        'background-color': 'White'
                    },
                    overlayCSS: {
                        backgroundColor: '#AAA',
                        cursor: 'default',
                        opacity: '0.3'
                    },

                    focusInput: false,
                    baseZ: zi,

                    fadeIn: 0,
                    fadeOut: 0
                });
            }
            catch (e) { };

            PopupKeyUpActionProvider.CloseDialogAction = this.ObjName + '.CloseAction();';
            PopupKeyUpActionProvider.EnterAction = this.ObjName + '.ApplyAndCloseDialog();';

        };

        this.PreSelectGroup = function(groupID) {
            var state = jq('#usrdialog_' + this.ID + '_dep_' + groupID).is(':checked');
            for (var i = 0; i < this.Groups.length; i++) {
                if (this.Groups[i].ID == groupID) {
                    for (var j = 0; j < this.Groups[i].Users.length; j++) {
                        if (this.Groups[i].Users[j].Selected == false) {
                            this.Groups[i].Users[j].PreSelected = state;
                            jq('#usrdialog_' + this.ID + '_usr_' + this.Groups[i].Users[j].ID).attr('checked', state);
                        }
                    }
                    this.Groups[i].PreSelected = state;
                    return;
                }
            }
        };

        this.PreSelectUser = function(userID) {
            var state = jq('#usrdialog_' + this.ID + '_usr_' + userID).is(':checked');
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].ID == userID) {
                        this.Groups[i].Users[j].PreSelected = state;
                        if (state == false) {
                            this.Groups[i].PreSelected = false;
                            jq('#usrdialog_' + this.ID + '_dep_' + this.Groups[i].ID).attr('checked', false);
                            return;
                        }

                        for (var k = 0; k < this.Groups[i].Users.length; k++) {
                            if (this.Groups[i].Users[k].PreSelected == false)
                                return;
                        }

                        jq('#usrdialog_' + this.ID + '_dep_' + this.Groups[i].ID).attr('checked', true);
                        return;
                    }
                }
            }
        };

        this.Select = function() {
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (!this.Groups[i].Users[j].Selected && this.Groups[i].Users[j].PreSelected) {
                        var currentUserID = this.Groups[i].Users[j].ID;

                        var parentEl = jq('input[id*="' + currentUserID + '"]').parent();

                        if (!parentEl.is(':visible')) {
                            continue;
                        }

                        this.Groups[i].Users[j].Selected = true;
                    }
                    this.Groups[i].Users[j].PreSelected = false;
                }
            }

            this.RenderItems();
        };

        this.Unselect = function() {
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].Selected && this.Groups[i].Users[j].PreSelected) {
                        this.Groups[i].Users[j].Selected = false;
                    }
                    this.Groups[i].Users[j].PreSelected = false;
                }
            }
            this.RenderItems();
        };

        this.ChangeMobileSelect = function() {
            if (!this.MobileVersion) return;

            var selectId = jq("#selectmobile_" + this.ID).val();

            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    this.Groups[i].Users[j].Selected = jq.inArray(this.Groups[i].Users[j].ID, selectId) != -1;
                }
            }
        };

        this.SelectUser = function(userID) {
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].ID == userID)
                        this.Groups[i].Users[j].Selected = true;
                }
            }
        };

        this.DisableUser = function(userID) {
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].ID == userID)
                        this.Groups[i].Users[j].Disable = true;
                }
            }
        };

        this.ClearSelection = function() {
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    this.Groups[i].Users[j].Selected = false;
                    this.Groups[i].Users[j].Disable = false;
                }
            }
        };

        this.GetSelectedUsers = function() {
            var users = new Array();
            for (var i = 0; i < this.Groups.length; i++) {
                for (var j = 0; j < this.Groups[i].Users.length; j++) {
                    if (this.Groups[i].Users[j].Selected)
                        users.push(this.Groups[i].Users[j]);
                }
            }

            return users;
        };

        this.CloseAction = function() {
            if (this.OnCancelButtonClick != null)
                this.OnCancelButtonClick();
        };

        this.ApplyAndCloseDialog = function() {
            PopupKeyUpActionProvider.ClearActions();
            jq.unblockUI();
            if (this.OnOkButtonClick != null) {
                for (var i = 0; i < this.Groups.length; i++) {
                    var group = this.Groups[i];
                    for (var j = 0; j < group.Users.length; j++) {
                        var user = this.Groups[i].Users[j];
                        user.DefaultSelected = user.Selected;
                    }
                }
                this.OnOkButtonClick();
            }
        };
        this.ClearUsrdialogRightBox = function() {
            jq('div[id^="usrdialog_rightBox_"]').empty();
            jq("div.centerBox div:last-child a").click();
        };
    };
};


function filterEmployees() {
    var el = jq('div[id^="usrdialog_leftBox_"]');
    var labels = el.find("label");
    labels.each(
    		function() {
    		    jq(this).parent().show();
    		    jq(this).parent().parent().show();
    		}
    	);

    jq('#employeeFilterInputCloseImage').attr('class', 'employeeFilterInputCloseImageGrey');

    var userName = jq('#employeeFilterInput').val();
    if (userName == null || userName == '' || userName == jq('#employeeFilterInput').attr('title')) {
        return;
    }

    jq('#employeeFilterInputCloseImage').attr('class', 'employeeFilterInputCloseImageBlue');

    userName = userName.toLowerCase();
    var userNameArray = userName.split(' ');

    var hideHeaderFlag = true;

    var userCount = 0;
    var lastSelectedUser;

    var header;

    labels.each(function() {
        var child = jq(this);

        if (child.parent().attr('class').indexOf("depBox") != -1) {
            if (hideHeaderFlag && header != null) {
                header.hide();
            }
            header = child.parent().parent();
            hideHeaderFlag = true;
        }
        else {

            if (child.parent().attr('class').indexOf("usrBox") != -1) {
                var currentUserName = child.text().toLowerCase();
                var i;
                var j = 0;
                for (i = 0; i < userNameArray.length; i++) {
                    if (currentUserName.indexOf(userNameArray[i]) != -1 || currentUserName.indexOf(' ' + userNameArray[i]) != -1) {
                        j++;
                    } else {
                        child.parent().hide();
                        j = -1;
                        break;
                    }
                }
            }

            if (i == j) {
                hideHeaderFlag = false;

                userCount++;
                lastSelectedUser = child;
            }
        }
    });

    if (hideHeaderFlag && header != null) {
        header.hide();
    }
};

function onEmployeeFilterInputFocus() {
    jq('#employeeFilterInput').attr('class', 'employeeFilterInputFocus');
    var title = jq('#employeeFilterInput').attr('title');
    if (jq('#employeeFilterInput').val() == title) {
        jq('#employeeFilterInput').val('');
    }
};

function onEmployeeFilterInputFocusLost() {
    if (jq('#employeeFilterInput').val() == '') {
        jq('#employeeFilterInput').attr('class', 'employeeFilterInputGreyed');
        setTimeout("jq('#employeeFilterInput').val(jq('#employeeFilterInput').attr('title'));", 200);
    }
};

function employeeFilterInputCloseImageClick() {
    var currentUserName = jq('#employeeFilterInput').val();
    var title = jq('#employeeFilterInput').attr('title');
    if (currentUserName == null || currentUserName == '' || currentUserName == title) {
        return;
    }
    jq('#employeeFilterInput').attr('class', 'employeeFilterInputGreyed');
    jq('#employeeFilterInput').val('');
    filterEmployees();

    setTimeout("jq('#employeeFilterInput').val(jq('#employeeFilterInput').attr('title'));", 100);
};