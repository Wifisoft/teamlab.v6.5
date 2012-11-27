<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.PhotoManager.Resources" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true"
    EnableViewState="false" CodeBehind="AddPhoto.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.AddPhoto"
    Title="Untitled Page" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/LastEvents.ascx" TagPrefix="ctrl" TagName="LastEvents" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">
    <link href="<%=ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/css/photomanagerstyle.css")%>"
        rel="stylesheet" type="text/css" />
    <style type="text/css">
        .swfupload
        {
            z-index: 1;
            position: absolute;
        }
    </style>
<script language="javascript" type="text/javascript">
    jQuery.extend({
    /*
    var result = $.format("Hello, {0}.", "world");
    //result -> "Hello, world."
    */
    format: function jQuery_dotnet_string_format(text) {
        //check if there are two arguments in the arguments list
        if (arguments.length <= 1) {
            //if there are not 2 or more arguments there's nothing to replace
            //just return the text
            return text;
        }
        //decrement to move to the second argument in the array
        var tokenCount = arguments.length - 2;
        for (var token = 0; token <= tokenCount; ++token) {
            //iterate through the tokens and replace their placeholders from the text in order
            text = text.replace(new RegExp("\\{" + token + "\\}", "gi"), arguments[token + 1]);
        }
        return text;
    }
}
);
</script>

    <script type="text/javascript">
        var fileUploader;

        var PhotoManager = new function() {
            this.selectedEvent = -1;
            this.selectedAuthor = -1;
            this.countFiles = 5;
            this.photoUploaded = false;

            this.InitUploader = function(eventID) {
                var btnId = "photo_upload_btn";

                if (typeof FileReader != 'undefined') {
                    jq("#photo_swf_button_container").hide();
                    jq('#photo_upload_pnl').hide();
                    btnId = 'photo_upload_btn_html5';
                }
                else {
                    jq("#photo_swf_button_container").show();
                    jq('#photo_upload_pnl').show();
                    btnId = "photo_upload_btn";
                }

                if (typeof ASC !== 'undefined' && typeof ASC.Controls !== 'undefined' && typeof ASC.Controls.FileUploader !== 'undefined') {
                    fileUploader = FileHtml5Uploader.InitFileUploader({
                        FileUploadHandler: 'ASC.Web.Community.PhotoManager.Common.FilesUploader,ASC.Web.Community.PhotoManager',
                        FileSizeLimit: "<%=ASC.Web.Studio.Core.SetupInfo.MaxUploadSize%>",

                        AutoSubmit: true,
                        ProgressTimeSpan: 300,
                        UploadButtonID: btnId,
                        TargetContainerID: "photo_upload_fileList",
                        Data: { uid: "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>", eventID: eventID },
                        OnUploadComplete: PhotoManager.OnUploadComplete,
                        OnAllUploadComplete: PhotoManager.OnAllUploadComplete,
                        OnProgress: PhotoManager.OnProgress,
                        file_types: "*.jpg; *.jpeg; *.bmp; *.png; *.gif",

                        AddFilesText: "<%=PhotoManagerResource.ButtonAddFiles%>",
                        SelectFilesText: "<%=PhotoManagerResource.ButtonSelectFiles%>",
                        ErrorFileTypeText: "<%=PhotoManagerResource.ErrorFileTypeText%>",

                        DeleteLinkCSSClass: 'photo_deleteLinkCSSClass',
                        LoadingImageCSSClass: 'photo_loadingCSSClass',
                        CompleteCSSClass: 'photo_completeCSSClass',
                        OverAllProcessBarCssClass: 'photo_overAllProcessBarCssClass',

                        DragDropHolder: jq("#photo_dd_panel"),
                        FilesHeaderCountHolder: jq("#photo_uploadHeader"),
                        OverAllProcessHolder: jq("#photo_overallprocessHolder"),
                        SubmitPanelAfterSelectHolder: jq("#photo_uploadDlgPanelButtons"),

                        OverallProgressText: "<%=PhotoManagerResource.OverallProgress%>",
                        FilesHeaderText: "<%=PhotoManagerResource.UploadedOf%>",
                        SelectFileText: "<%=PhotoManagerResource.SelectFilesToUpload%>",
                        DragDropText: "<%=PhotoManagerResource.OrDragDrop%>",
                        SelectedFilesText: "<%=PhotoManagerResource.SelectedFiles%>"
                    });
                }
                //jq("#photo_upload_btn").unbind("mouseover");

            };

            this.AddMore = function() {
                PhotoManager.countFiles++;
                jq('#simple_files_container').append("<div><input id='file" + PhotoManager.countFiles + "' name='file" + PhotoManager.countFiles + "' type='file' /></div>");
            };

            this.BlockUploadAreaBtn = function() {
                jq('#uploadAreaBtn').hide();
                jq('#uploadProcess').show();
            };

            this.BlockSaveAreaBtn = function() {
                jq('#saveAreaBtn').hide();
                jq('#saveProcess').show();
            };

            this.ShowCreateEvent = function() {
                try {

                    //    				jq('#SWFUpload_0').remove();
                    //    				jq('#SWFUpload_1').remove();

                    jq("#<%=event_date.ClientID%>").mask('<%= System.DateTimeExtension.DateMaskForJQuery %>');
                    jq('#<%=event_date.ClientID%>').datepick(
								{
								    defaultDate: '<%=ASC.Core.Tenants.TenantUtil.DateTimeNow().ToShortDateString()%>',
								    selectDefaultDate: true,
								    showAnim: '',
								    popupContainer: "#events_form"
								});
                    //            jq('#<%=event_date.ClientID%>').val('<%=ASC.Core.Tenants.TenantUtil.DateTimeNow().ToShortDateString()%>');
                    jq('#event_name').val('');
                    jq('#event_desc').val('');

                    jq.blockUI({ message: jq("#events_form"),
                        css: {
                            left: '50%',
                            top: '50%',
                            opacity: '1',
                            border: 'none',
                            padding: '0px',
                            width: '340px',
                            height: '390px',
                            cursor: 'default',
                            textAlign: 'left',
                            'margin-left': '-170px',
                            'margin-top': '-150px',
                            'background-color': 'Transparent'
                        },

                        overlayCSS: {
                            backgroundColor: '#AAA',
                            cursor: 'default',
                            opacity: '0.3'
                        },
                        focusInput: false,
                        baseZ: 666,

                        fadeIn: 0,
                        fadeOut: 0
                    });
                }
                catch (e) { };

                PopupKeyUpActionProvider.ClearActions();
                PopupKeyUpActionProvider.CloseDialogAction = "";
                PopupKeyUpActionProvider.CtrlEnterAction = "PhotoManager.CreateEvent();";

                //document.getElementById('events_selector').selectedIndex = this.selectedEvent;
            };

            this.CreateEvent = function() {

                try {
                    var date = jq.datepick.parseDate(jq.datepick.dateFormat, jq('#<%=event_date.ClientID%>').val());
                    jq('#<%=event_date.ClientID%>').datepick('setDate', date);
                }
                catch (e) { jq('#<%=event_date.ClientID%>').datepick('setDate', new Date()); }
                jq('#<%=event_date.ClientID%>').datepick('destroy');

                AjaxPro.onLoading = function(b) { if (b) { jq('#panel_buttons').hide(); jq('#action_loader').show(); } else { jq('#panel_buttons').show(); jq('#action_loader').hide(); } };

                if (jq('#event_name').val() != '' && jq('#<%=event_date.ClientID%>').val() != '')
                    AddPhoto.CreateEvent(jq('#event_name').val(), jq('#event_desc').val(), jq('#<%=event_date.ClientID%>').val(), this.callBackCreateEvent)
            };

            this.callBackCreateEvent = function(result) {
                if (result != null) {
                    jq('#events_selector').append(result.value);

                    document.getElementById('events_selector').selectedIndex = document.getElementById('events_selector').options.length - 1;
                    PhotoManager.EventsSelectorHandle();
                }
                PhotoManager.CloseEventForm();
            };

            this.LoadEvent = function(id) {
                this.selectedEvent = document.getElementById('events_selector').selectedIndex;
                AjaxPro.onLoading = function(b) { if (b) { jq.blockUI(); } else { jq.unblockUI(); } };
                AddPhoto.LoadEventAlbums(id, this.callBackLoadEvent);
            };

            this.EventsSelectorHandle = function() {
                var index = jq('#events_selector').val();
                if (index == -1) {
                    this.HideAuthors();
                }
                else if (index == -2) {
                    this.ShowCreateEvent();
                }
                else {
                    this.InitUploader(index)
                    jq('#uploader_container').show();
                }
            };

            this.ShowUploader = function() {
                jq('#uploader_container').show();
                this.selectedAlbum = document.getElementById('authors_selector').selectedIndex;
            };

            this.HideAuthors = function() {
                jq('#upl_save_container').hide();
                jq('#uploader_container').hide();
                this.selectedEvent = -1;
            };

            this.AuthorsSelectorHandle = function() {
                var index = jq('#authors_selector').val();
                if (index == 0) {
                    this.ShowOtherAuthor();
                }
                else {
                    jq('#pht_other_author').hide();
                    this.ShowUploader();
                }
            };

            this.HideUploader = function() {
                jq('#uploader_container').hide();
                this.selectedAlbum = -1;
            };

            this.ShowOtherAuthor = function() {
                jq('#pht_other_author').show();
            };

            this.CloseEventForm = function() {
                PopupKeyUpActionProvider.ClearActions();
                jq.unblockUI();
                //InitUploader();
            };

            this.SafeSession = function() {
                AjaxPro.onLoading = function(b) { };
                AddPhoto.SafeSession(this.callBackSafe);
            };

            this.callBackSafe = function()
            { };

            this.OnUploadComplete = function(file) {
                if (file) {
                    if (file.Success) {
                        this.photoUploaded = true;
                        PhotoManager.addImageInfo(file.Message)
                    }
                }
            };

            this.OnAllUploadComplete = function() {
                if (this.photoUploaded)
                    jq('#upl_save_container').show();
            };

            this.OnProgress = function() { };

            this.addImageInfo = function(data) {
                var temp = jq('#phtm_imagesInfo').val();

                if (temp != "")
                    temp += "|" + data;
                else
                    temp += data;

                jq('#phtm_imagesInfo').val(temp);
            };

        }
    
    PhotoManager.selectedEvent = - 1;
    PhotoManager.selectedAuthor = '<%=ASC.Core.SecurityContext.CurrentAccount.ID%>';
    PhotoManager.countFiles = 5;
    

    jq(document).ready(function() {
	    // 5 minutes
        window.setInterval("PhotoManager.SafeSession();", 5*60*1000);       
        
        if(jq('#events_selector').val() > 0)
            PhotoManager.InitUploader(jq('#events_selector').val());
    });
    
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <input id="phtm_imagesInfo" name="phtm_imagesInfo" type="hidden" value="" />
    <input type="hidden" id="isSimpleUpload" name="isSimpleUpload" value="1" />
    <div id="events_form" style="display: none;">
        <ascw:container id="formContainer" runat="server">
            <header>
        <%=PhotoManagerResource.EventCreationTitle%>
        </header>
            <body>
                <div>
                    <div>
                        <span>
                            <%=PhotoManagerResource.NameTitle%>:</span><br />
                        <input id="event_name" type="text" class="textEdit" maxlength="255" style="width: 99%;
                            margin-top: 3px;" />
                    </div>
                    <div style="margin-top: 10px;">
                        <span>
                            <%=PhotoManagerResource.DateTitle%>:</span><br />
                        <asp:TextBox ID="event_date" CssClass="textEditCalendar" Style="margin-top: 3px;"
                            runat="server"></asp:TextBox>
                    </div>
                    <div style="margin-top: 10px;">
                        <span>
                            <%=PhotoManagerResource.DescriptionTitle%>:</span><br />
                        <textarea id="event_desc" class="textEdit" style="width: 99%; margin-top: 3px; height: 120px;"></textarea>
                    </div>
                    <div id="panel_buttons" style="margin-top: 16px;">
                        <a class="baseLinkButton" href="javascript:PhotoManager.CreateEvent();">
                            <%=PhotoManagerResource.CreateButton%></a> <a class="grayLinkButton" href="javascript:PhotoManager.CloseEventForm();"
                                style="margin-left: 8px;">
                                <%=PhotoManagerResource.CancelButton%></a>
                    </div>
                    <div id="action_loader" style="margin-top: 16px; display: none;" class="clearFix">
                        <div class="textMediumDescribe">
                            <%=PhotoManagerResource.PleaseWaitShortMessage%>
                        </div>
                        <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                    </div>
                </div>
            </body>
        </ascw:container>
    </div>
    <ascw:container id="mainContainer" runat="server">
        <header>                        
        </header>
        <body>
            <div>
                <asp:Panel ID="pnlImageForm" runat="server">
                    <center id='main_container' style="margin: 80px 0px 140px 0px;">
                        <table cellpadding="0" cellspacing="0" border="0" width="500" style="text-align:left">
                            <tr>
                                <td>
                                    <div style="margin-bottom: 10px;" class="headerPanel">
                                        <%=PhotoManagerResource.CreateSelectEventMessage%>&nbsp;&nbsp;<a href="javascript:PhotoManager.ShowCreateEvent();"
                                            class="linkHeaderLight"><%=PhotoManagerResource.CreateNewTitle.ToLower()%></a></div>
                                    <%=RenderEventsSelector()%>
                                </td>
                            </tr>
                            <tr id="uploader_container" <%=(selectedAlbum == null  && requestedEvent == -1 ? "style=\"display: none;\"" : "") %>>
                                <td>
                                    <div class="clearFix" id="photo_dd_panel">
                                        <div id="photo_uploadHeader">
                                        </div>
                                        <table cellpadding="0" cellspacing="0" border="0" style="margin:15px 0;">
                                            <tr valign="middle">
                                                <td style="width: 50px; padding: 5px 0 0 10px;">
                                                    <div class="photo_uploadIcon">
                                                    </div>
                                                </td>
                                                <td height="20">
                                                    <div class="describeUpload">
                                                        <%=ASC.Web.Studio.Core.FileSizeComment.GetFileSizeNote(PhotoManagerResource.NoteFileUpload, true)%>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <div id="photo_overallprocessHolder">
                                        </div>
                                        <div id="photo_upload_fileList" class="photo_uploadContainer">
                                        </div>
                                        <div id="photo_upload_pnl" style="display: none; padding: 0 0 15px;">
                                            <div class="clearFix" id="photo_swf_button_container" style="padding-top:15px;">
                                                <a class="grayLinkButton photo_upload_btn" id="photo_upload_btn" style="width:100px;">
                                                    <%=PhotoManagerResource.ButtonSelectFiles%></a>
                                                <div id="ProgressFileUploader">
                                                    <ascw:ProgressFileUploader ID="FileUploader" EnableHtml5="true" runat="server" />
                                                </div>
                                                <div style="float: right">
                                                    <asp:PlaceHolder ID="_uploadSwitchHolder" runat="server"></asp:PlaceHolder>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="photo_upload_btn_html5">
                                        </div>
                                    </div>
                                    <%--<div style="">
                                        <div id="swf_uploader">
                                            <div id="swfu_container">
                                                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                    <tr>
                                                        <td style="height: 100px; vertical-align: middle;">
                                                            <center>
                                                                <div id="divFileProgressContainer" style="display: none; margin: 0px; padding: 0px;">
                                                                </div>
                                                                <div id='containerToUpload'>
                                                                    <span id="spanButtonPlaceholder"></span><a id='linkToUpload' class="linkHeaderLight"
                                                                        style="text-decoration: underline; margin: 0px; padding: 0px; z-index: 2; display: inline-block"
                                                                        href="javascript:void(0);">
                                                                        <%=PhotoManagerResource.ChoosePhotolink%></a>
                                                                </div>
                                                                <div id='successUploadsTitle' class="headerPanel" style="display: none; margin: 0px;
                                                                    padding: 0px;">
                                                                    <%=PhotoManagerResource.AllPhotosUploadedTitle%>
                                                                </div>
                                                            </center>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div id="files_container" class="borderBase" style="margin-top: 5px; overflow-y: auto;
                                                    height: 174px; display: none;">
                                                    <table cellpadding="7" cellspacing="0" border="0" width="100%" id="phtm_imagesStatus">
                                                    </table>
                                                </div>
                                                <div id='upl_add_more' style="display: none; margin-top: 10px;">
                                                    <div id='containerToMoreUpload' style="float: left; padding-left: 15px">
                                                        <span id="spanButtonPlaceholder2"></span><a id='upl_add_more_link' href="javascript:void(0);"
                                                            class="linkHeaderLightSmall">
                                                            <%=PhotoManagerResource.AddMorePhotosLink%></a></div>
                                                    <div style="float: right; padding-right: 25px">
                                                        <span class="" id='upl_img_count'></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="simple_uploader" style="margin-top: 20px;">
                                            <center>
                                                <div id="simple_files_container">
                                                    <div>
                                                        <input id='file1' name='file1' type='file' /></div>
                                                    <div>
                                                        <input id='file2' name='file2' type='file' /></div>
                                                    <div>
                                                        <input id='file3' name='file3' type='file' /></div>
                                                    <div>
                                                        <input id='file4' name='file4' type='file' /></div>
                                                    <div>
                                                        <input id='file5' name='file5' type='file' /></div>
                                                </div>
                                                <div style="margin-top: 10px;">
                                                    <asp:LinkButton CssClass="baseLinkButton" ID="lbtnPreUpload" OnClick="btnUpload_Click"
                                                        runat="server"><%=PhotoManagerResource.SavePhotosButton%></asp:LinkButton>
                                                    <a class="baseLinkButton" href="javascript:PhotoManager.AddMore();">
                                                        <%=PhotoManagerResource.AddButton%></a>
                                                </div>
                                            </center>
                                        </div>
                                        
                                         <div style="padding-top:30px;">
                                            <asp:PlaceHolder ID="_uploadSwitchHolder" runat="server"></asp:PlaceHolder>
                                         </div>
                                    </div>--%>
                                </td>
                            </tr>
                        </table>
                        <div id="upl_save_container" style="display: none;">
                            <div class="borderBase" style='border-right: none; border-left: none; border-bottom: none;
                                margin-top: 10px; margin-bottom: 20px; width: 600px'>
                            </div>
                            <div id="uploadAreaBtn">
                                <asp:LinkButton OnClick="btnUpload_Click" OnClientClick="javascript:PhotoManager.BlockUploadAreaBtn();"
                                    CssClass="baseLinkButton" ID="btnUpload" runat="server"></asp:LinkButton>
                            </div>
                            <div id="uploadProcess" style="height: 20px; width: 350px; display: none;">
                                <center>
                                    <div class="textMediumDescribe" style="float: left; margin-left: 10px;">
                                        <%=PhotoManagerResource.PleaseWaitShortMessage%>
                                    </div>
                                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                                </center>
                            </div>
                        </div>
                    </center>
                </asp:Panel>
                <asp:Literal ID="ltrUploadedImages" runat="server"></asp:Literal>
                <asp:Panel ID="pnlSave" Style="margin-top: 15px; padding-left: 1px;" runat="server"
                    Visible="false">
                    <div style="margin-top: 20px;">
                        <table border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td valign="middle">
                                    <div id="saveAreaBtn">
                                        <asp:LinkButton OnClick="btnSave_Click" OnClientClick="javascript:PhotoManager.BlockSaveAreaBtn();"
                                            CssClass="baseLinkButton" ID="btnSave" runat="server"></asp:LinkButton><span class="textMediumDescribe"
                                                style="margin-left: 10px"><%=PhotoManagerResource.GoToTitle%>
                                                <%=PhotoManagerResource.YouAlbumTitle%></span></div>
                                    <div id="saveProcess" style="margin-top: 16px; display: none;" class="clearFix">
                                        <div class="textMediumDescribe">
                                            <%=PhotoManagerResource.PleaseWaitShortMessage%>
                                        </div>
                                        <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <div id="console">
                </div>
            </div>
        </body>
    </ascw:container>
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
    <ctrl:ActionContainer ID="actions" runat="server" />
    <ascwc:SideRecentActivity id="sideRecentActivity" runat="server" />
    <ctrl:LastEvents ID="lastEvents" runat="server">
    </ctrl:LastEvents>
</asp:Content>
