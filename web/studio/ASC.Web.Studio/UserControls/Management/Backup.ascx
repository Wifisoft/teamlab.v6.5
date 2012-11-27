<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Backup.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.Backup" %>
<%@ Import Namespace="Resources" %>

        <asp:PlaceHolder ID="backupContainer" runat="server" />
        <div id="backupSettings" class="<%=EnableBackup ? "" : "disable" %>">
        <div class="headerBase borderBase clearFix" style="padding-left: 15px; padding-bottom: 5px;
            border-top: none; border-right: none; border-left: none;">
            <div style="float: left;">
                <%=Resources.Resource.DataBackup%>
            </div>
	         <div id="HelpQuestionDataBackup" class="HelpCenterSwitcher title"  title="<%=Resources.Resource.HelpQuestionDataBackup%>"></div> 
	         <%if(!EnableBackup){%>
                      <div class="disable-trial-version">
                          <span class="text"><%=String.Format(Resources.Resource.DisableTrialVersion, "<span class=\"text-orange\">", "</span>")%></span>
                          <a class="link" href="/management.aspx?type=4"><%=Resources.Resource.ViewTariffPlans %></a>
                      </div>                          
              <% } %>
             <div class="popup_helper" id="AnswerForHelpDataBackup">
                 <p><%=String.Format(Resources.Resource.HelpAnswerDataBackup, "<br />", "<b>", "</b>")%><br />
                 <a href="http://teamlab.com/content/TeamLabinstall.pdf" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
             </div>   

        </div>
        <div style="padding: 10px 15px 20px;">
            <p><b>
            <%=string.Format(Resources.Resource.BackupDesc,"<br/>")%>
            </b></p>

            <script type="text/javascript">
                BackupManager = new function() {

                    var timer;
                    var backupID;
                    var already = false;

                    this.CreateBackup = function() {
                        if (BackupManager.already)
                            return;

                        BackupManager.already = true;

                        var url = "<%=Url%>/services/backup/service.svc/add?_=" + Math.floor(Math.random() * 1000000);
                        jq("#backup_error").hide();
                        this.InitProcess();

                        jq("#create_btn").attr("class", "disableLinkButton");
                        jq("#create_btn").blur();

                        jq("#progressbar_container").show();


                        jq.ajax({
                            type: "GET",
                            url: url,
                            dataType: "json",
                            success: function(result) {
                                BackupManager.backupID = result.id;
                                BackupManager.timer = setInterval("BackupManager.CheckBackupProcess()", 10000);
                            }
                        });

                    };

                    this.InitProcess = function() {
                        jq("#progressbar").progressbar({ value: 1 });
                        jq("#backup_percent").text("1% ");
                    };

                    this.SetBackupProcess = function(percent) {
                        var pcent = percent == 0 ? 1 : percent;

                        jq("#progressbar").progressbar('value', percent);
                        jq("#backup_percent").text(percent + "% ");
                    };

                    this.CheckBackupProcess = function() {
                        var url = "<%=Url%>/services/backup/service.svc/list/" + BackupManager.backupID + "?_=" + Math.floor(Math.random() * 1000000);

                        jq.ajax({
                            type: "GET",
                            url: url,
                            dataType: "json",
                            success: function(result) {
                                if (result.completed == true) {
                                    clearInterval(BackupManager.timer);
                                    jq("#backup_link").html("<a href='" + result.link + "' target='_blank'>" + result.link + "</a>")
                                    jq("#progressbar_container").hide();
                                    jq("#backup_ready").show();
                                } else if (result.status == 5) {
                                    clearInterval(BackupManager.timer);
                                    jq("#backup_error").text("<%=Resources.Resource.BackupError%>");
                                    jq("#backup_error").show();
                                }

                                BackupManager.SetBackupProcess(result.percentdone);
                            }
                        });
                    };

                    this.Deactivate = function() {
                        AjaxPro.onLoading = function(b) {
                            if (b) {
                                jq.blockUI();
                            } else {
                                jq.unblockUI();
                            }
                        };
                        Backup.Deactivate(true, BackupManager.callBackDeactivate);
                    };

                    this.Delete = function() {
                        AjaxPro.onLoading = function(b) {
                            if (b) {
                                jq.blockUI();
                            } else {
                                jq.unblockUI();
                            }
                        };
                        Backup.Delete(true, BackupManager.callBackDelete);
                    };

                    this.callBackDeactivate = function(result) {
                        if (result != null) {
                            jq("#deativate_sent").html(result.value);
                            jq("#deativate_sent").show();

                        }
                    };

                    this.callBackDelete = function(result) {
                        if (result != null) {
                            jq("#delete_sent").html(result.value);
                            jq("#delete_sent").show();
                        }
                    };

                };
                jq(function() {
                    if (jq('#backupSettings').hasClass('disable')) {
                        jq('#HelpQuestionDataBackup').unbind('click').attr('title','');

                    } else {
                        jq('#HelpQuestionDataBackup').bind('click', function() {
                            jq(this).helper({ BlockHelperID: 'AnswerForHelpDataBackup' });
                        });
                    }
                });
            BackupManager.already = false;
            </script>

            <div style="margin-top: 15px;">
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <tr valign="top">
                        <td width="100">
                            <div>
                                <a id="create_btn" class="baseLinkButton <%= EnableBackup ? "" : "disabled" %>" style="width: 100px;" href="javascript:void(0);"
                                onclick="<%= EnableBackup ? "BackupManager.CreateBackup();" : "" %>"
                                    ><%=Resources.Resource.PerformBackupButton%></a>
                            </div>
                        </td>
                        <td>
                            <div style="padding-left: 20px;">
                                <div id="progressbar_container" style="display: none; margin-bottom: 10px;">
                                    <div id="progressbar">
                                    </div>
                                    <div style="padding-top: 2px;" class="textMediumDescribe">
                                        <%=Resources.Resource.CreatingBackup %>
                                        <span id="backup_percent"></span>
                                    </div>
                                </div>
                                <div id="backup_error" style="color:red; display:none;">
                                </div>
                                <div id="backup_ready" style="display: none;">
                                    <div id="backup_link" class="longWordsBreak" style="width: 540px;">
                                    </div>
                                    <%=string.Format(Resources.Resource.BackupReadyText,"<p>","</p><p>","</p>")%>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        </div>
        <div class="headerBase borderBase clearFix" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
            border-top: medium none; border-right: medium none; border-left: medium none;">
            <div style="float: left;">
            <%=Resources.Resource.AccountDeactivation%>
            </div>
	        <div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpAccountDeactivation'});" title="<%=Resources.Resource.HelpQuestionAccountDeactivation%>"></div> 
            <div class="popup_helper" id="AnswerForHelpAccountDeactivation">
               <p><%=String.Format(Resources.Resource.HelpAnswerAccountDeactivation, "<br />", "<b>", "</b>")%></p>
            </div>   


        </div>
        <div style="padding: 10px 15px 20px;">
             <%=string.Format(Resource.DeactivationDesc, "<p><b>", "</b></p>", string.Empty)%>
            <div style="margin-top: 15px;">
                <a class="baseLinkButton" href="javascript:void(0);" onclick="BackupManager.Deactivate()" style="width: 100px;"><%=Resources.Resource.DeactivateButton%></a>
            </div>
            <p id="deativate_sent" style="display:none;"></p>
        </div>
        <div class="headerBase borderBase" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
            border-top: medium none; border-right: medium none; border-left: medium none;">
            
            <%=Resources.Resource.AccountDeletion%>
           
            
        </div>
        <div style="padding: 10px 15px 20px;">
        <p><b>
            <%=Resources.Resource.DeletionDesc%></b></p>
            <div style="margin-top: 15px;">
                <a class="baseLinkButton" href="javascript:void(0);" onclick="BackupManager.Delete()" style="width: 100px;"><%=Resources.Resource.DeleteButton%></a>
            </div>
            <p id="delete_sent" style="display:none;"></p>
        </div>

