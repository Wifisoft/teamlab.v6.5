ASC.Projects.Projects = (function() {
    // Private Section

    return { // Public Section
        checkAsFollowing: function(projectID) {

            AjaxPro.onLoading = function(b) {

                if (b)
                    jq("#project_following").block({ message: null });
                else
                    jq("#project_following").unblock();

            }

            AjaxPro.ProjectDetailsView.CheckAsFollowing(projectID,
     function(res) {

         if (res.error != null) {

             alert(res.error.Message);

             return;

         }

         jq("#project_following a").text(jq.trim(res.value));

     }

    );

        },
        slideMyProjects: function(first, last) {
            jq("#myProjects").slideToggle1("slow");
            var src = jq("#slideMyPrj").attr('src');
            if (src.indexOf("expand") > -1) {
                var newSrc = last;
                jq("#slideMyPrj").attr('src', newSrc);
                jq("#slideMyPrj").attr('title', ProjectJSResources.Expand);
                jq("#slideMyPrj").attr('alt', ProjectJSResources.Expand);
            }
            if (src.indexOf("collapse") > -1) {
                var newSrc = first;
                jq("#slideMyPrj").attr('src', newSrc);
                jq("#slideMyPrj").attr('title', ProjectJSResources.Collapse);
                jq("#slideMyPrj").attr('alt', ProjectJSResources.Collapse);
            }
        },
        slideFollowingProjects: function(first, last) {
            jq("#followingProjects").slideToggle("slow");
            var src = jq("#slideFollowingPrj").attr('src');
            if (src.indexOf("expand") > -1) {
                var newSrc = last;
                jq("#slideFollowingPrj").attr('src', newSrc);
                jq("#slideFollowingPrj").attr('title', ProjectJSResources.Expand);
                jq("#slideFollowingPrj").attr('alt', ProjectJSResources.Expand);
            }
            if (src.indexOf("collapse") > -1) {
                var newSrc = first;
                jq("#slideFollowingPrj").attr('src', newSrc);
                jq("#slideFollowingPrj").attr('title', ProjectJSResources.Collapse);
                jq("#slideFollowingPrj").attr('alt', ProjectJSResources.Collapse);
            }
        },
        slideActiveProjects: function(first, last) {
            jq("#activeProjects").slideToggle("slow");
            var src = jq("#slideActivePrj").attr('src');
            if (src.indexOf("expand") > -1) {
                var newSrc = last;
                jq("#slideActivePrj").attr('src', newSrc);
                jq("#slideActivePrj").attr('title', ProjectJSResources.Expand);
                jq("#slideActivePrj").attr('alt', ProjectJSResources.Expand);
            }
            if (src.indexOf("collapse") > -1) {
                var newSrc = first;
                jq("#slideActivePrj").attr('src', newSrc);
                jq("#slideActivePrj").attr('title', ProjectJSResources.Collapse);
                jq("#slideActivePrj").attr('alt', ProjectJSResources.Collapse);
            }
        },
        unlockForm: function() {
            jq("[id$=tbxTitle]").removeAttr("readonly").removeClass("disabled");
            jq("[id$=tbxDescription]").removeAttr("readonly").removeClass("disabled");
            jq("#Select").removeAttr("readonly").removeClass("disabled");
            jq("[id$=tbxTags]").removeAttr("readonly").removeClass("disabled");
            jq("#notify").removeAttr("disabled");
            jq("#actions").show();
            jq("#info-block").hide();

            //			jq('#ProjectTemplateCombobox').removeAttr('disabled');
        },

        lockForm: function() {
            jq("[id$=tbxTitle]").attr("readonly", "readonly").addClass("disabled");
            jq("[id$=tbxDescription]").attr("readonly", "readonly").addClass("disabled");
            jq("#Select").attr("readonly", "readonly").addClass("disabled");
            jq("[id$=tbxTags]").attr("readonly", "readonly").addClass("disabled");
            jq("#notify").attr("disabled", "disabled");
            jq("#actions").hide();
            jq("#info-block").show();

            //			jq('#ProjectTemplateCombobox').attr('disabled', true);
        },

        unlockPrjSettingsFormWhenDelete: function() {
            ASC.Projects.Projects.unlockPrjSettingsPageElements();

            jq("#deleteButton").show();
            jq("#info-block-forDelete").hide();

            jq("#updateButton").show();

        },

        lockPrjSettingsFormWhenDelete: function() {
            ASC.Projects.Projects.lockPrjSettingsPageElements();

            jq("#deleteButton").hide();
            jq("#info-block-forDelete").show();

            jq("#updateButton").hide();
        },


        unlockPrjSettingsFormWhenUpdate: function() {
            ASC.Projects.Projects.unlockPrjSettingsPageElements();

            jq("#updateButton").show();
            jq("#info-block-forUpdate").hide();

            jq("#deleteButton").show();
        },

        lockPrjSettingsFormWhenUpdate: function() {
            ASC.Projects.Projects.lockPrjSettingsPageElements();

            jq("#updateButton").hide();
            jq("#info-block-forUpdate").show();

            jq("#deleteButton").hide();
        },

        unlockPrjSettingsPageElements: function() {
            jq("[id$=tbxTitle]").removeAttr("disabled");
            jq("[id$=tbxDescription]").removeAttr("disabled");
            jq("#inputUserName").removeAttr("disabled");
            jq("#Select").removeAttr("disabled");
            jq("[id$=tbxTags]").removeAttr("disabled");
            jq("#isHiddenProject").removeAttr("disabled");
            jq("#open").removeAttr("disabled");
            jq("#closed").removeAttr("disabled");
            jq("#understand").removeAttr("disabled");
        },

        lockPrjSettingsPageElements: function() {
            jq("[id$=tbxTitle]").attr("disabled", "disabled");
            jq("[id$=tbxDescription]").attr("disabled", "disabled");
            jq("#inputUserName").attr("disabled", "disabled");
            jq("#Select").attr("disabled", "disabled");
            jq("[id$=tbxTags]").attr("disabled", "disabled");
            jq("#isHiddenProject").attr("disabled", "disabled");
            jq("#open").attr("disabled", "disabled");
            jq("#closed").attr("disabled", "disabled");
            jq("#understand").attr("disabled", "disabled");
        },

        saveProject: function() {
            HideRequiredError();
            var prjID = jq.getURLParam("prjID");
            var title = jq("[id$=tbxTitle]").val().trim();
            var description = jq("[id$=tbxDescription]").val();
            var leader = userSelector.SelectedUserId;
            var IsOpen = jq("#open").is(":checked");
            var isHidden = jq('#isHiddenProject').is(':checked');
            var tags = jq("[id$=tbxTags]").val();


            if (leader == null) {
                AddRequiredErrorText(jq("#userSelector"), ProjectJSResources.EmptyProjectManager);
                ShowRequiredError(jq("#userSelector"));
                ASC.Projects.Projects.unlockForm();
                return;
            }

            AjaxPro.onLoading = function(b) {
                if (b) { ASC.Projects.Projects.lockPrjSettingsFormWhenUpdate(); }
                else { ASC.Projects.Projects.unlockPrjSettingsFormWhenUpdate(); }
            }
            if (title != "") {
                AjaxPro.ProjectSettings.SaveProject(prjID, title, description, leader, tags, IsOpen, isHidden,
                function(res) {
                    if (res.error != null) {
                        ASC.Projects.Projects.unlockPrjSettingsFormWhenUpdate();
                        AddRequiredErrorText(jq("[id$=tbxTitle]"), res.error.Message);
                        ShowRequiredError(jq("[id$=tbxTitle]"));
                        return;
                    }
                    jq("div.infoPanel div").css('display', 'block');
                    ASC.Projects.Projects.unlockPrjSettingsFormWhenUpdate();
                });
            }
            else {
                AddRequiredErrorText(jq("[id$=tbxTitle]"), ProjectJSResources.EmptyProjectTitle);
                ShowRequiredError(jq("[id$=tbxTitle]"));
            }
        },

        deleteProject: function() {
            AjaxPro.onLoading = function(b) {
                if (b) { ASC.Projects.Projects.lockPrjSettingsFormWhenDelete(); }
            }
            if (jq("#understand").is(":checked")) {
                var prjID = jq.getURLParam("prjID");
                AjaxPro.ProjectSettings.DeleteProject(prjID, function(res) {
                    if (res.error != null) {
                        ASC.Projects.Projects.unlockPrjSettingsFormWhenDelete();
                        alert(res.error.Message);
                        return;
                    }
                    jq("div.infoPanel div").css('display', 'block');
                    if (res.value != "")
                        document.location.replace(res.value);
                    else
                        ASC.Projects.Projects.unlockPrjSettingsFormWhenDelete();
                });
            }
            else {
                if (confirm(ProjectJSResources.DeleteThisProject)) {
                    var prjID = jq.getURLParam("prjID");
                    AjaxPro.ProjectSettings.DeleteProject(parseInt(prjID), function(res) {
                        if (res.error != null) {
                            ASC.Projects.Projects.unlockPrjSettingsFormWhenDelete();
                            alert(res.error.Message);
                            return;
                        }
                        jq("div.infoPanel div").css('display', 'block');
                        if (res.value != "")
                            document.location.replace(res.value);
                        else
                            ASC.Projects.Projects.unlockPrjSettingsFormWhenDelete();
                    });
                }
            }
        },

        addNewProject: function() {
            HideRequiredError();
            var title = jq("[id$=tbxTitle]").val().trim();
            var description = jq("[id$=tbxDescription]").val();
            var leader = userSelector.SelectedUserId;
            var notifyIsChecked = jq('#notify').is(':checked');
            var isHidden = jq('#isHiddenProject').is(':checked');

            if (jq('#secureState').val() == 0)
                isHidden = false

            var tags = jq("[id$=tbxTags]").val();

            if (leader == null) {
                AddRequiredErrorText(jq("#userSelector"), ProjectJSResources.EmptyProjectManager);
                ShowRequiredError(jq("#userSelector"));
                ASC.Projects.Projects.unlockForm();
                return;
            }

            if (title != "") {
                var templateProjectId = jq('#SelectedTemplateID').val();
                AjaxPro.onLoading = function(b) {
                    if (b) { ASC.Projects.Projects.lockForm(); }
                }
                AjaxPro.ProjectActionView.AddNewProject(title, description, leader, tags, notifyIsChecked, isHidden, templateProjectId,
                function(res) {
                    if (res.error != null) {
                        AddRequiredErrorText(jq("[id$=tbxTitle]"), res.error.Message);
                        ShowRequiredError(jq("[id$=tbxTitle]"));
                        ASC.Projects.Projects.unlockForm();
                        return;
                    }
                    document.location.replace("projects.aspx" + res.value);
                });
            }
            else {
                AddRequiredErrorText(jq("[id$=tbxTitle]"), ProjectJSResources.EmptyProjectTitle);
                ShowRequiredError(jq("[id$=tbxTitle]"));
                ASC.Projects.Projects.unlockForm();
            }
        },
        accept: function() {
            var requestID = jq('#requestID').val();
            var title = jq('#txtTitle').val();
            var description = jq('#txtDescription').val();
            var leaderID = jq('#Select option:selected').val();
            var tags = jq('#_tbxTags').val();
            var isHidden = jq('#isHiddenProject').is(':checked');
            var notify = jq('#notify').is(':checked');
            var status = jq('#requestBody input:radio:checked').attr('id');

            AjaxPro.Request.AcceptRequest(requestID, title, description, leaderID, tags, isHidden, notify, status,
        function(res) {
            document.location.reload();
        });
        },

        reject: function() {
            var requestID = jq('#requestID').val();

            AjaxPro.Request.RejectRequest(requestID,
        function() {
            location.reload(true);
        });
        },

        slideArchiveProjects: function(first, last) {
            jq("#archiveProjects").slideToggle("slow");
            var src = jq("#slideArchivePrj").attr('src');
            if (src.indexOf("expand") > -1) {
                var newSrc = last;
                jq("#slideArchivePrj").attr('src', newSrc);
                jq("#slideArchivePrj").attr('title', ProjectJSResources.Expand);
                jq("#slideArchivePrj").attr('alt', ProjectJSResources.Expand);
            }
            if (src.indexOf("collapse") > -1) {
                var newSrc = first;
                jq("#slideArchivePrj").attr('src', newSrc);
                jq("#slideArchivePrj").attr('title', ProjectJSResources.Collapse);
                jq("#slideArchivePrj").attr('alt', ProjectJSResources.Collapse);
            }
        },

        toggleClosedProjects: function() {

            var basePath = 'projects.aspx';
            var addition = "?";

            if (jq.trim(jq.getURLParam("filter")) != "")
            { basePath += addition + "filter=" + jq.getURLParam("filter"); addition = "&"; }

            if (jq.trim(jq.getURLParam("tag")) != "")
            { basePath += "&tag=" + jq.getURLParam("tag"); addition = "&"; }

            if (jq.getURLParam("view") == "all")
                location.href = basePath;
            else
                location.href = basePath + addition + "view=all";

        },

        tagsAutocomplete: function(event) {
            jq("#TagsAutocompleteContainer").bind("click", function() { return false; });

            document.onclick = ASC.Projects.Projects.HideTagsAutocompleteContainer;

            var value = jq("[id$=tbxTags]").val();
            var titles = value.split(',');

            var width = document.getElementById(jq("[id$=HiddenFieldForTbxTagsID]").val()).offsetWidth;
            jq("#TagsAutocompleteContainer").css("width", width + "px");

            var code;
            if (!e) var e = event;
            if (!e) var e = window.event;

            if (e.keyCode) { code = e.keyCode; }
            else if (e.which) { code = e.which; }

            //left
            if (code == 37) { return true; }
            //up
            else if (code == 38) { ASC.Projects.Projects.MoveSelected(true); return true; }
            //right
            else if (code == 39) { return true; }
            //down
            else if (code == 40) { ASC.Projects.Projects.MoveSelected(false); return true; }

            AjaxPro.onLoading = function(b) {
                if (b) { }
                else { }
            }

            AjaxPro.ProjectSettings.TagsAutocomplete(titles[titles.length - 1],
    function(res) {
        jq("#TagsAutocompleteContainer").html("");

        if (res.value.length > 0) {
            for (var i = 0; i < res.value.length; i++) {
                var container = document.createElement('div');

                jq(container).addClass('tagAutocompleteItem');
                jq(container).text(res.value[i]);

                jq(container).bind("mouseover", function() {
                    jq("div.tagAutocompleteItem").each(function() {
                        jq(this).removeClass("tagAutocompleteSelectedItem");
                    });
                    jq(this).addClass("tagAutocompleteSelectedItem");
                });

                jq(container).bind("click", function() {
                    var str = "";
                    for (var i = 0; i < titles.length - 1; i++) {
                        str += jq.trim(titles[i]);
                        str += ", ";
                    }
                    jq("[id$=tbxTags]").val(str + jq(this).text() + ", ");
                    jq("#TagsAutocompleteContainer").html("");
                });

                jq("#TagsAutocompleteContainer").append(container);
                jq("[id$=tbxTags]").focus();
            }
        }
    });

        },

        MoveSelected: function(toUp) {
            if (jq("#TagsAutocompleteContainer").html() == "") { return; }

            var result = null;
            var obj;
            var items = document.getElementById("TagsAutocompleteContainer").getElementsByTagName("DIV");

            for (var i = 0; i < items.length; i++) {
                obj = items[i];
                if (obj.className.indexOf("tagAutocompleteSelectedItem") >= 0) {
                    result = obj;
                    if (toUp && i > 0) {
                        jq(result).removeClass("tagAutocompleteSelectedItem");
                        jq(items[i - 1]).addClass("tagAutocompleteSelectedItem");
                        jq('#TagsAutocompleteContainer').scrollTo(jq(items[i - 1]).position().top, 100);
                    }
                    if (!toUp && i < items.length - 1) {
                        jq(result).removeClass("tagAutocompleteSelectedItem");
                        jq(items[i + 1]).addClass("tagAutocompleteSelectedItem");
                        jq('#TagsAutocompleteContainer').scrollTo(jq(items[i + 1]).position().top, 100);
                    }
                    break;
                }
            }

            if (result == null) { jq(items[0]).addClass("tagAutocompleteSelectedItem"); }

        },

        HideTagsAutocompleteContainer: function(event) {
            if (window.event) { ev = window.event.srcElement; }
            else { ev = event.target; }

            var itSelf = false;
            parent_div = ev;

            while (parent_div.parentNode && !itSelf) {
                if (parent_div.className == "DoNotHideTagsAutocompleteContainer") { itSelf = true; }
                parent_div = parent_div.parentNode;
            }

            if (!itSelf) {
                if (jq("#TagsAutocompleteContainer").html() != "") { jq("#TagsAutocompleteContainer").html(""); }
            }

        },

        IfEnterDown: function(event) {
            var value = jq("[id$=tbxTags]").val();
            var titles = value.split(',');

            var code;
            if (!e) var e = event;
            if (!e) var e = window.event;

            if (e.keyCode) { code = e.keyCode; }
            else if (e.which) { code = e.which; }

            //enter
            if (code == 13) {
                var str = "";
                for (var i = 0; i < titles.length - 1; i++) {
                    str += jq.trim(titles[i]);
                    str += ", ";
                }

                if (jq("div.tagAutocompleteSelectedItem").length != 0) {
                    jq("[id$=tbxTags]").val(str + jq("div.tagAutocompleteSelectedItem").text() + ", ");
                }

                jq("#TagsAutocompleteContainer").html("");
                return false;
            }
            return true;

        }

    }
})();

