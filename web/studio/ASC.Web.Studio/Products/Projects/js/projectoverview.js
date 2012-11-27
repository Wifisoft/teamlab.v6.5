jq(document).ready(function() {
    jq('#deleteProject').click(function() {
        showQuestionWindow();
    });

    var deleteProject = function(projectId) {
        var params = {};
        Teamlab.removePrjProject(params, projectId, { success: onDeleteProject, error: onDeleteProjectError });
    };

    var onDeleteProject = function() {
        document.location.replace("projects.aspx");
    };

    var onDeleteProjectError = function() {
        jq.unblockUI();
    };

    UserSelector.OnOkButtonClick = function() {
        var projectId = jq.getURLParam("prjID");
        var team = new Array();
        jq(UserSelector.GetSelectedUsers()).each(function(i, el) { team.push(el.ID); });
        var notify = true;

        Teamlab.updatePrjTeam({}, projectId, { projectId: projectId, participants: team, notify: notify }, { success: onUpdateProjectTeam });

        function onUpdateProjectTeam(params, str) {
            jq("#teamCount").text(str);
        }
    };

    jq('#followProjectButton span').on('click', function() {
        var projectId = jq.getURLParam("prjID");
        Teamlab.followingPrjProject({}, projectId, { projectId: projectId }, { success: onFollowingProject });
    });
	
	function onFollowingProject(params, project) {
        if (project) {
        	var link = jq('#followProjectButton span');
        	var oldText = link.text();
            link.text(link.attr('follow'));
            link.attr('follow', oldText);
        }
    };

    jq('#manageTeamButton').click(function() {
        UserSelector.ShowDialog();
    });

    var showQuestionWindow = function() {
        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq('#questionWindow'),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '400px',

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-200px',
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

        jq('#questionWindow .remove').bind('click', function() {
            var projectId = jq.getURLParam("prjID");
            deleteProject(projectId);
            return false;
        });

        jq('#questionWindow .cancel').bind('click', function() {
            jq.unblockUI();
            return false;
        });
    };
});
