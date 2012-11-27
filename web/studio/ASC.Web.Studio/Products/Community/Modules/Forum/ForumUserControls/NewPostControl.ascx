<%@ Assembly Name="ASC.Web.UserControls.Forum" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewPostControl.ascx.cs"
    Inherits="ASC.Web.UserControls.Forum.NewPostControl" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Common" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Resources" %>
    
    <script type="text/javascript">
        function FCKConfig_OnLoad(config) {
            config.MaxImageWidth = 540;
            config.RedirectUrlToUpload('<%=RenderRedirectUpload()%>');  
        }
    </script> 
    
<ascwc:JsHtmlDecoder id="JsHtmlDecoder" runat="Server"></ascwc:JsHtmlDecoder>

<input id="forum_postType" name="forum_postType" type="hidden" value="<%=(int)PostType%>" />
<input id="forum_attachments" name="forum_attachments" type="hidden" value="<%=_attachmentsString%>" />
<div id="forum_errorMessage">
    <%=_errorMessage%>
</div>
<div id="post_container" style="padding: 0px 0px 5px 0px;">
    <%=RenderForumSelector()%>
    <%TopicHeader();%>
    <ascwc:PollFormMaster runat="server" ID="_pollMaster" />
    <div class="headerPanel" style="margin-top: 20px;">
        <%=ForumUCResource.Message%></div>            
    
    <div>
    <% if (_mobileVer){%>    
        <textarea id="forum_mobile_text" class="forum_text" style="width:100%;" rows="18" cols="40">
            
        </textarea>
        
        
        <textarea id="forum_text" name="forum_text" style="display:none;"><%=_text%></textarea>
        <script type="text/javascript">
            jq(function() {

                var node =jq('<div>'+jq('#forum_text').val()+'</div>').get(0);
                jq('#forum_mobile_text').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
            })
            
        </script>
        
        
    <%}else{ %>       
    
        <FCKeditorV2:FCKeditor ID="FCKeditor" Height="400px" runat="server">
        </FCKeditorV2:FCKeditor>
        <textarea id="forum_text" name="forum_text"  style="display:none;"></textarea>
    
    <%} %>
    
    
    </div>
    
    <div>
        <%if (PostAction == PostAction.Edit)
              Response.Write(PostControl.AttachmentsList(EditedPost, SettingsID));
        %>
    </div>
    <%AddTags();%>
    <div <%=(!IsAllowCreateAttachment)?"style=\"display:none;\"":""%>>
        <div class="clearFix" style="margin-top: 15px;">            
            <div style="padding: 15px; border:1px solid #d1d1d1" id="forum_uploadDialogContainer">
                <table id='forum_upload_select' cellpadding="0" cellspacing="0" border="0">
                    <tr valign="middle">
                        <td style="width:50px; padding:5px 0 0 10px;">
                            <div class="forum_uploadIcon"></div>
                        </td>
                        <td height="20"> 
                            <div class="describeUpload">
                                <%=string.Format(ForumUCResource.MaximumAttachmentSize, ASC.Web.Studio.Core.SetupInfo.MaxUploadSize / 1024 / 1024)%></div>
                        </td>
                    </tr>
                </table>
                
                <div id="forum_overallprocessHolder">
                </div>
                <div id="forum_uploadContainer" class="forum_uploadContainer">
                </div>
                <div id="forum_upload_pnl" style="display: none; padding:15px 0 10px;">
                    <div class="clearFix" id="forum_swf_button_container">
                        <a class="grayLinkButton forum_uploadButton" id="forum_uploadButton" style="width:100px;"> 
                            <%=ForumUCResource.FileUploadAddButton%></a>
                        <div id="ProgressFileUploader">
                            <ascwc:ProgressFileUploader ID="_fileUploader" EnableHtml5="true"  runat="server" />
                        </div>
                        <div style="float:right;margin-top:-10px">
                            <asp:PlaceHolder ID="_uploadSwitchHolder" runat="server"></asp:PlaceHolder>
                        </div>
                    </div>
                </div>
                <div id="forum_upload_btn_html5">
                </div>
                
                
                <script type="text/javascript">
                    jq(document).ready(function() {
                        if (typeof FileReader == 'undefined')
                            jq("#forum_upload_pnl").show();

                        FileHtml5Uploader.InitFileUploader({
                            FileUploadHandler: "ASC.Web.UserControls.Forum.ForumAttachmentUploadHanler,ASC.Web.UserControls.Forum",
                            AutoSubmit: true,
                            UploadButtonID: (typeof FileReader != 'undefined' ? 'forum_upload_btn_html5' : 'forum_uploadButton'),
                            TargetContainerID: 'forum_uploadContainer',
                            ProgressTimeSpan: 300,
                            Data: { 'SettingsID': '<%=SettingsID%>',
                                'ThreadID': '<%=_threadForAttachFiles%>',
                                'UserID': '<%=ASC.Core.SecurityContext.CurrentAccount.ID%>'
                            },

                            FileSizeLimit: '<%=ASC.Web.Studio.Core.SetupInfo.MaxUploadSize%>',

                            DeleteLinkCSSClass: 'forum_deleteLinkCSSClass',
                            LoadingImageCSSClass: 'forum_loadingCSSClass',
                            CompleteCSSClass: 'forum_completeCSSClass',
                            DragDropHolder: jq("#forum_uploadDialogContainer"),
                            //                        FilesHeaderCountHolder : jq("#forum_uploadHeader"),
                            OverAllProcessHolder: jq("#forum_overallprocessHolder"),
                            OverAllProcessBarCssClass: 'forum_overAllProcessBarCssClass',

                            AddFilesText: "<%=ForumUCResource.FileUploadAddButton%>",
                            SelectFilesText: "<%=ForumUCResource.FileUploadAddButton%>",
                            OverallProgressText: "<%=ForumUCResource.OverallProgress%>",

                            OnBegin: function(data) { ForumManager.BlockButtons(); },
                            AfterUploadDeleteCallback: function(serverData) {
                                AjaxPro.onLoading = function(b) {
                                    if (b)
                                        jq.blockUI();
                                    else
                                        jq.unblockUI();
                                }
                                PostCreator.RemoveAttachment(serverData.Data.SettingsID, serverData.Data.OffsetPhysicalPath, function(res) { });
                            },

                            OnUploadComplete: function(serverData) {
                                if (serverData)
                                    if (serverData.Data) {
                                    var str = jq('#forum_attachments').val();
                                    str += '%$%' + serverData.Data.OffsetPhysicalPath + "$@$" + serverData.Data.FileName + "$@$" + serverData.Data.Size + "$@$" + serverData.Data.ContentType;
                                    jq('#forum_attachments').val(str);
                                }
                            },

                            OnAllUploadComplete: function() { ForumManager.UnblockButtons(); }
                        });
                    });
        
                </script>

            </div>
        </div>
    </div>
</div>
<div class="clearFix" style="margin: 20px 0px 25px 0px;">
    <div style="float: right; text-align: right; width: 50%;">
        <%=RenderSubscription()%>
    </div>
    <div id="panel_buttons" style="float: left;">
        <a class="baseLinkButton" style="float: left;" href="javascript:void(0);"
				onclick="ForumManager.SendMessage(<%=(_mobileVer? "":("'"+FCKeditor.ClientID+"'"))%>); return false;">
            <%=ForumUCResource.PublishButton%>
        </a><a class="baseLinkButton" style="float: left; margin-left: 8px;" href="javascript:void(0);"
				onclick="ForumManager.<%=(_mobileVer? "Preview()" : ("PreviewFCK('"+FCKeditor.ClientID+"')"))%>; return false;">
            <%=ForumUCResource.PreviewButton%>
        </a>
        
        <a class="grayLinkButton cancelFckEditorChangesButtonMarker" style="float: left; margin-left: 8px;" onclick="javascript:ForumManager.BlockButtons(); ForumManager.CancelPost('<%=(EditedPost == null)?"":EditedPost.ID.ToString()%>'); return false;" href="#"
			name="<%=_mobileVer?"":FCKeditor.ClientID%>">
            <%=ForumUCResource.CancelButton%>
        </a>
    </div>
    <div id="action_loader" style="float: left; display: none;" class="clearFix">        
        <div class="textMediumDescribe">
			<%=ForumUCResource.PleaseWaitMessage%>
		</div>
		<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
    </div>
</div>
<div id="forum_previewBox">
</div>
<div id="forum_previewBoxFCK" style="display: none;">
    <div class="headerPanel">
            <%=ForumUCResource.PagePreview%>
        </div>
    <div class="tintMedium borderBase clearFix" style="padding: 10px 0px;
    border-left: none; border-right: none;">
    <table cellpadding="0" cellspacing="0" style="width: 100%;">
        <tr valign="top">
            <%--user info--%>
            <td align="center" style="width: 200px;">
                <div class="forum_postBoxUserSection" style="overflow: hidden; width: 150px;">
                    <%="<a class=\"linkHeader\"  href=\"" + CommonLinkUtility.GetUserProfile(_currentUser.ID, _settings.ProductID) + "\"><span>" + ASC.Web.Core.Users.DisplayUserSettings.GetFullUserName(_currentUser) + "</span></a>"%>
                    <div style="margin: 5px 0px;" class="textMediumDescribe">
                        <%=HttpUtility.HtmlEncode(_currentUser.Title ?? "")%>
                    </div>
                    <a href="<%=CommonLinkUtility.GetUserProfile(_currentUser.ID, _settings.ProductID) %>">
                        <%=_settings.ForumManager.GetHTMLImgUserAvatar(_currentUser.ID)%>
                    </a>
                </div>
            </td>
            <td>
                <div style="margin-bottom: 5px; padding: 0px 5px;">
                    <%=DateTimeService.DateTime2StringPostStyle(DateTimeService.CurrentDate())%>
                </div>
                <div id="forum_message_previewfck" class="forum_mesBox" style="width: 550px;">
                </div>
            </td>
        </tr>
    </table>    
    </div>
    <div style='margin-top:10px;'><a class="baseLinkButton" href='javascript:void(0);' onclick='ForumManager.HidePreview(); return false;'><%=ForumUCResource.HideButton%></a></div>
</div>
<asp:PlaceHolder ID="_recentPostsHolder" runat="server"></asp:PlaceHolder>
