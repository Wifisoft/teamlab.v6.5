<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master" CodeBehind="settings.aspx.cs" Inherits="ASC.Web.Projects.Settings" %>

<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("projects.css") %>" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .headerPanelSmall
        {
            margin-bottom: 5px;
        }
        .pm-headerPanelSmall-splitter
        {
            margin-bottom: 10px;
        }
    </style>

    <script language="javascript">
    
    var _timeout = null;
    
    jq(function(){
                    jq("#changeVersion").bind("click", function() { changeVersion(); });
                    checkImportStatus();
                    jq("#agreement").removeAttr("checked");
                    /*Default tab comment 
                    var currTab = parseInt(jq('[id$=hfCurrentTab]').val());
                    jq('#rbl_'+currTab).attr('checked','checked');
                    */
                 });
    
    function startImport()
    {
        var disableNotifications = !jq("#sendInvitations").is(':checked');
        var importClosed = jq("#importClosed").is(':checked');
        
        //validate
        if(jq('[id$=tbxURL]').val()=='')
        {
            alert('<%= SettingsResource.EmptyURL.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>');
            return false;
        }
        if(jq('[id$=tbxToken]').val()=='')
        {
            alert('<%= SettingsResource.EmptyToken.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>');
            return false;
        }
        var patt=/^http[s]{0,1}:\/\/.*\.basecamphq\.com[/]{0,1}$/gi;
        if(!patt.test(jq('[id$=tbxURL]').val()))
        {
            alert('<%= SettingsResource.NotBasecampUrl.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>');
            return false;
        }
        
        AjaxPro.SettingsPage.StartImportFromBasecamp(jq("[id$=tbxURL]").val(), jq("[id$=tbxToken]").val(),importClosed, disableNotifications,
        function(res) {
            if (res.error != null) {
                alert('<%= SettingsResource.ImportFailed.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>\r\n' + res.error.Message);
                return;
            }
            else {
                lockImportTools();
                viewImportInfoPanel();
                jq('#importPeopleStatus').html("<sapn class='pm-grayText'><%= SettingsResource.StatusAwaiting.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %></sapn>");
                jq('#importProjectsStatus').html("<sapn class='pm-grayText'><%= SettingsResource.StatusAwaiting.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %></sapn>");
                jq('#importFilesStatus').html("<sapn class='pm-grayText'><%= SettingsResource.StatusAwaiting.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %></sapn>");
                jq('#importProgress').html('0');
                jq('#popupPanelBodyError').hide();
                setTimeout("checkImportStatus()", 5000);
            }
        }
        );
        
    }

    function checkImportStatus() {
        
        AjaxPro.SettingsPage.GetImportFromBasecampStatus(function(res) {
            if(res.error!=null)
            {
                if (res.error.Type!="System.Collections.Generic.KeyNotFoundException")
                {
                    alert('<%= SettingsResource.ImportFailed.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>\r\n' + res.error.Message);
                    jq('#popupPanelBodyError').hide();
                }
                unlockImportTools();
            }
            else if(res.value!=null && res.value.Completed && res.error==null)
            {
                jq('#importPeopleStatus').html('<img src="<%=WebImageSupplier.GetAbsoluteWebPath("status_closed.png", ProductEntryPoint.ID)%>" style="margin-top: 5px;"/>');
                jq('#importProjectsStatus').html('<img src="<%=WebImageSupplier.GetAbsoluteWebPath("status_closed.png", ProductEntryPoint.ID)%>" style="margin-top: 5px;"/>');
                jq('#importFilesStatus').html('<img src="<%=WebImageSupplier.GetAbsoluteWebPath("status_closed.png", ProductEntryPoint.ID)%>" style="margin-top: 5px;"/>');
                jq('#importProgress').html('3');
                jq('#popupPanelBodyError').hide();
                buildErrorList(res);
                unlockImportTools();
            }
            else if(res.value!=null && res.value.Error!=null && res.value.Error!="")
            {
                buildErrorList(res);
                unlockImportTools();
            }    
            else
            {
                lockImportTools();
                setTimeout("checkImportStatus()", 5000);
                buildErrorList(res);
                if(res.value!=null)
                {
                if(res.value.UserProgress>0) {
                    jq('#importPeopleStatus').html(Math.round(res.value.UserProgress)+'%');
                    if(res.value.UserProgress==100)
                    {
                        jq('#importPeopleStatus').html('<img src="<%=WebImageSupplier.GetAbsoluteWebPath("status_closed.png", ProductEntryPoint.ID)%>" style="margin-top: 5px;"/>');
                        jq('#importProgress').html('1');
                    }
                }
                if(res.value.ProjectProgress>0) {
                    jq('#importProjectsStatus').html(Math.round(res.value.ProjectProgress)+'%');
                    if(res.value.ProjectProgress==100)
                    {
                        jq('#importProjectsStatus').html('<img src="<%=WebImageSupplier.GetAbsoluteWebPath("status_closed.png", ProductEntryPoint.ID)%>" style="margin-top: 5px;"/>');
                        jq('#importProgress').html('2');
                    }
                }
                if(res.value.FileProgress>0) {
                    jq('#importFilesStatus').html(Math.round(res.value.FileProgress)+'%');
                    if(res.value.FileProgress==100)
                    {
                        jq('#importFilesStatus').html('<img src="<%=WebImageSupplier.GetAbsoluteWebPath("status_closed.png", ProductEntryPoint.ID)%>" style="margin-top: 5px;"/>');
                        jq('#importProgress').html('3');
                    }
                }     
                }       
            }
        });
    }
    
    function buildErrorList(res)
    {
            if (res.value==null)
                return;
            if (res.value.Log==null)
                return;
            if (res.value.Log.length==0)
                return;
                
            var statusStr = "";
            if (res.value.Error!=null && res.value.Error!="")
            {
                statusStr = statusStr+'<div class="errorBox fatal">'+res.value.Error + '</div>';
            }
            for(var i=res.value.Log.length-1; i>=0; i--){
                
                var messageClass = "ok";
                if (res.value.Log[i].Type == "warn" || res.value.Log[i].Type == "error")
                    messageClass = "warn";
                    
                statusStr = statusStr+'<div class="'+messageClass+'Box">'+res.value.Log[i].Message + '</div>';
            }

            if (statusStr!="")
            {
                jq('#popupPanelBodyError').html(statusStr);
                jq('#popupPanelBodyError').show();
            }
            else
            {
                jq('#popupPanelBodyError').hide();
            }
    }
    
    function lockImportTools() {
            jq('[id$=tbxURL]').attr("readonly", "readonly").addClass("disabled");
            jq('[id$=tbxToken]').attr("readonly", "readonly").addClass("disabled");
            
            jq("#importClosed").attr('disabled','disabled');
            jq("#sendInvitations").attr('disabled','disabled');
            
            jq('div.pm-action-block').hide();
            jq('div.pm-ajax-info-block').show();

            jq('#importCompletedContent').hide();
            
            /*Default tab comment 
            jq('#rbl_0').attr("disabled", "disabled");
            jq('#rbl_1').attr("disabled", "disabled");
            jq('#rbl_2').attr("disabled", "disabled");
            jq('#rbl_3').attr("disabled", "disabled");
            */
    }
    
    function unlockImportTools() {

            jq('[id$=tbxURL]').removeAttr("readonly").removeClass("disabled");
            jq('[id$=tbxToken]').removeAttr("readonly").removeClass("disabled");
            
            jq("#importClosed").removeAttr('disabled');
            jq("#sendInvitations").removeAttr('disabled');
            
            jq('div.pm-action-block').show();
            jq('div.pm-ajax-info-block').hide();
            
            jq('#importCompletedContent').show();
            
            /*Default tab comment 
            jq('#rbl_0').removeAttr("disabled");
            jq('#rbl_1').removeAttr("disabled");
            jq('#rbl_2').removeAttr("disabled");
            jq('#rbl_3').removeAttr("disabled");
            */
        }
            
    function viewImportInfoPanel()
    {
        jq("#import_info_attention_popup").hide();
        
        var margintop =  jq(window).scrollTop()-135;
        margintop = margintop+'px';
                        
        
        jq.blockUI({message:jq("#import_info_popup"),
                    css: { 
                        left: '50%',
                        top: '35%',
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: '520px',
                       
                        cursor: 'default',
                        textAlign : 'left',
                        position: 'absolute',
                        'margin-left': '-300px',
                        'margin-top': margintop,                      
                        'background-color':'White'
                    },
                    
                    overlayCSS: { 
                        backgroundColor: '#AAA',
                        cursor: 'default',
                        opacity: '0.3' 
                    },
                    focusInput : false,
                    baseZ : 6667,
                    
                    fadeIn: 0,
                    fadeOut: 0
                }); 
                
                
    }
    
    function viewImportInfoAttentionPanel()
    {
        jq('#import_info_attention_popup a:first').unbind('click').removeClass('baseLinkButton').addClass('disableLink');
        if(_timeout!=null) clearTimeout(_timeout);  
        
        if(jq('[id$=tbxURL]').val()=='')
        {
            alert('<%= SettingsResource.EmptyURL.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>');
            return false;
        }
        if(jq('[id$=tbxToken]').val()=='')
        {
            alert('<%= SettingsResource.EmptyToken.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>');
            return false;
        }
        if(jq('[id$=HiddenFieldForPermission]').val()=='0')
        {
            alert('<%= SettingsResource.AccessDenied.Replace("\"", "&quot;").Replace("\'", "&rsquo;") %>');
            return false;
        }

        _timeout = setTimeout(function() {
                			jq("#import_info_attention_popup a:first")
                			.unbind("click")
                			.bind("click", function(){startImport();})
                			.removeClass('disableLink')
                			.addClass('baseLinkButton');
                		}, 3000);
        
        var margintop =  jq(window).scrollTop()-135;
        margintop = margintop+'px';
        
        jq.blockUI({message:jq("#import_info_attention_popup"),
                    css: { 
                        left: '50%',
                        top: '35%',
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: '520px',
                       
                        cursor: 'default',
                        textAlign : 'left',
                        position: 'absolute',
                        'margin-left': '-300px',
                        'margin-top': margintop,                      
                        'background-color':'White'
                    },
                    
                    overlayCSS: { 
                        backgroundColor: '#AAA',
                        cursor: 'default',
                        opacity: '0.3' 
                    },
                    focusInput : false,
                    baseZ : 6666,
                    
                    fadeIn: 0,
                    fadeOut: 0
                }); 
                
                
    }
    
    function changeAgreementCheckBox(obj)
    {
        if(jq(obj).is(':checked'))
        {
            jq("div.pm-action-block a:first")
		    .unbind("click")
		    .bind("click", function(){startImport();})
		    .removeClass('disableLink')
		    .addClass('baseLinkButton');
        }
        else 
        {
            jq("div.pm-action-block a:first")
		    .unbind("click")
		    .removeClass('baseLinkButton')
		    .addClass('disableLink');
        }
    }
        
    function changeAgreementCheckBox(obj)
    {
        if(jq(obj).is(':checked'))
        {
            jq("div.pm-action-block a:first")
		    .unbind("click")
		    .bind("click", function(){startImport();})
		    .removeClass('disableLink')
		    .addClass('baseLinkButton');
        }
        else 
        {
            jq("div.pm-action-block a:first")
		    .unbind("click")
		    .removeClass('baseLinkButton')
		    .addClass('disableLink');
        }
    }
    </script>

</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <div id="importTools" class="pm-headerPanel-splitter">
        <div class="pm-headerPanelSmall-splitter">
            <div class="headerPanelSmall">
                <%= String.Format(SettingsResource.CompanyURL, "<b>", "</b>")%>
            </div>
            <asp:TextBox ID="tbxURL" runat="server" Style="width: 400px" CssClass="textEdit"></asp:TextBox>
            <div class="textBigDescribe">
                <%= String.Format(SettingsResource.InfoURL, "<b>", "</b>") %>
            </div>
        </div>
        <div class="pm-headerPanelSmall-splitter">
            <div class="headerPanelSmall">
                <%= SettingsResource.UserToken%>
            </div>
            <asp:TextBox ID="tbxToken" runat="server" Style="width: 400px" CssClass="textEdit"></asp:TextBox>
            <div class="textBigDescribe">
                <%= SettingsResource.InfoToken %>
            </div>
            <input type="checkbox" id="importClosed" style="margin-top:10px; vertical-align: baseline;" value="<%=SettingsResource.ImportClosedTasks%>" title="<%=SettingsResource.ImportClosedTasks%>" value="false" />
            <label for="importClosed">
                <%=SettingsResource.ImportClosedTasks%></label>
            <br /> 
            <input type="checkbox" id="sendInvitations" style="margin-top:10px; vertical-align: baseline;" value="<%=SettingsResource.SendInvitations%>" title="<%=SettingsResource.SendInvitations%>" value="false" />
            <label for="sendInvitations">
                <%=SettingsResource.SendInvitations%></label>    
        </div>
        
    <%--Old records content
        <div class="pm-headerPanelSmall-splitter">
            <b>
                <%= SettingsResource.ImportInfo_Attention%></b>
        </div>
        <div class="pm-headerPanelSmall-splitter">
            <b>
                <%= SettingsResource.ImportInfo_Notyfy%></b>
        </div>--%>
        
        <div class="pm-headerPanel-splitter tintMedium">
            <div class="headerBase pm-projectSettings-container pm-redText">
                <%=SettingsResource.ImportAttantionPanelTitle%>
            </div>
            <div class="pm-projectSettings-container">
                <%=SettingsResource.ImportAttantionPanelBody%>
            </div>
            <div class="pm-projectSettings-container">
                <input type="checkbox" id="agreement" onclick="changeAgreementCheckBox(this)">
                <label for="agreement">
                    <%=SettingsResource.ImportAttantionPanelAgreement%>
                </label>
            </div>
        </div>
        
        <div class="pm-action-block">
            <a class="disableLink"><%=SettingsResource.StartImport%></a>
        </div>
        <div class='pm-ajax-info-block' style="display: none; margin-top: 20px;">
            <span class="textMediumDescribe">
                <%= SettingsResource.ImportInfoPanelContent%></span> <a onclick="javascript:viewImportInfoPanel();" class="linkAction" style="font-size: 11px !important; font-weight: bold; margin-left: 10px; cursor: pointer;">
                    <%= SettingsResource.ViewDetails%>
                </a>
            <br />
            <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
        </div>
    </div>
    
    <%--Default tab comment 
    <div class="headerBase pm-headerPanel-splitter">
        <%= SettingsResource.DefaultTab %>
    </div>
    <div>
        <div class="headerPanelSmall">
            <%= SettingsResource.DefaultTabInfo %>
        </div>
        <table width="100%" cellpadding="0" cellspacing="0">
        <tr valign="top">
            <td style="white-space:nowrap">
                <input name="rbl" id="rbl_0" value="0" type="radio" onclick="saveDefaultTabChanges(this.value);" style="margin-left:0;"/>
                <label for="rbl_0" style="margin-right: 5px"><%= ProjectsCommonResource.Dashboard%></label>
            </td>
            <td style="white-space:nowrap">
                <input name="rbl" id="rbl_1" value="1" type="radio" onclick="saveDefaultTabChanges(this.value);" />
                <label for="rbl_1" style="margin-right: 5px"><%= ProjectResource.Projects%></label>
            </td>
            <td style="white-space:nowrap">
                <input name="rbl" id="rbl_2" value="2" type="radio" onclick="saveDefaultTabChanges(this.value);" />
                <label for="rbl_2" style="margin-right: 5px"><%= TaskResource.MyTasks%></label>
            </td>
            <td style="white-space:nowrap">
                <input name="rbl" id="rbl_3" value="3" type="radio" onclick="saveDefaultTabChanges(this.value);" />
                <label for="rbl_3" style="margin-right: 5px"><%= ReportResource.Reports%></label>
            </td>
            <td width="100%"></td>
        </tr>
        </table>
        <asp:HiddenField ID="hfCurrentTab" runat="server" />
    </div>
    --%>
    
    <div id="import_info_popup" style="display: none;">
        <ascwc:container id="import_info_container" runat="server">
            <header>    
                <%= SettingsResource.PopupPanelHeader%>
            </header>
            <body>
                <div style="border: 1px solid #DCDCDC; padding: 20px 30px;">
                    <div class="headerBase pm-headerPanel-splitter">
                        <%= SettingsResource.PleaseWait%>
                    </div>
                    <div class="pm-headerPanel-splitter">
                        <table cellspacing="0" cellpadding="0">
                            <tr style="height: 30px;">
                                <td>
                                    <b>
                                        <%= SettingsResource.People%></b>
                                </td>
                                <td style="width: 100%;">
                                    <div style="border-bottom: 1px dotted rgb(0, 0, 0);">
                                        &nbsp;</div>
                                </td>
                                <td id="importPeopleStatus" style="padding-left: 10px;">
                                </td>
                            </tr>
                        </table>
                        <table cellspacing="0" cellpadding="0">
                            <tr style="height: 30px;">
                                <td>
                                    <b>
                                        <%= SettingsResource.Projects%></b>
                                </td>
                                <td style="width: 100%;">
                                    <div style="border-bottom: 1px dotted rgb(0, 0, 0);">
                                        &nbsp;</div>
                                </td>
                                <td id="importProjectsStatus" style="padding-left: 10px;">
                                </td>
                            </tr>
                        </table>
                        <table cellspacing="0" cellpadding="0">
                            <tr style="height: 30px;">
                                <td>
                                    <b>
                                        <%= SettingsResource.Files%></b>
                                </td>
                                <td style="width: 100%;">
                                    <div style="border-bottom: 1px dotted rgb(0, 0, 0);">
                                        &nbsp;</div>
                                </td>
                                <td id="importFilesStatus" style="padding-left: 10px;">
                                </td>
                            </tr>
                        </table>
                        <div style="text-align: right; padding-top: 5px;">
                            <%= SettingsResource.ImportCompleted%>: <span id="importProgress">0</span>
                            <%= SettingsResource.OutOfThree%>
                        </div>
                    </div>
                    <div class="pm-headerPanel-splitter">
                        <%=String.Format(SettingsResource.PopupPanelBody,"<br/>")%>
                        <div style="max-height: 100px; overflow:auto;border:solid 1px #DCDCDC">
                            <div id="popupPanelBodyError" style="display: none;">
                            </div>
                        </div>
                    </div>
                    <div>
                        <a class="baseLinkButton" href="javascript:void(0)" onclick="javascript: jq.unblockUI();">
                            <%= SettingsResource.Close%>
                        </a>
                    </div>
                </div>
            </body>
        </ascwc:container>
    </div>
    
    <%--Old attention content
    <div id="import_info_attention_popup" style="display: none;">
        <ascwc:container id="import_info_attention_container" runat="server">
            <header>    
                <%= SettingsResource.PopupPanelHeader%>
            </header>
            <body>
                <div style="border: 1px solid #DCDCDC; padding: 20px 30px; margin-bottom:20px; font-weight:bold;">
                        <%= SettingsResource.ImportInfo_Attention_PopUp%>
                </div>
                <a class="disableLink">
                <%= SettingsResource.YesStartImport%></a>
                <span class="splitter"></span>
                <a onclick="jq.unblockUI();" class="grayLinkButton">
                <%= ProjectsCommonResource.Cancel%></a>
            </body>
        </ascwc:container>
    </div>--%>
    
    <asp:HiddenField ID="HiddenFieldForPermission" runat="server" />
</asp:Content>
