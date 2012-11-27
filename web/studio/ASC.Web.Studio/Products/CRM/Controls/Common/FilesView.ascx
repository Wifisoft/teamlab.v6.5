<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilesView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Common.FilesView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<script type="text/javascript" language="javascript">

    jq(function() {

            var FileUploaderConfig = {                   
                    FileUploadHandler: "ASC.Web.CRM.Classes.FileUploaderHandler, ASC.Web.CRM",               
                    AutoSubmit : false,
                    UploadButtonID : (typeof FileReader != 'undefined' ? 'pm_upload_btn_html5': 'uploadButton'),
                    TargetContainerID : 'filesContainer',
                    ProgressTimeSpan: 300,
                    FileSizeLimit : 100000,                   
                    Data : {
                            "ContactID": '<%= ContactID %>',
                            "UserID":'<%=ASC.Core.SecurityContext.CurrentAccount.ID%>',
                            "ASPSESSID": "<%=Session.SessionID %>",                            
                            "AUTHID": "<% = Request.Cookies["asc_auth_key"]==null ? "" : Request.Cookies["asc_auth_key"].Value %>"
                          },
                    FileSizeLimit : "<%=ASC.Web.Studio.Core.SetupInfo.MaxUploadSize / 1024%>"
                };   
});

</script>

<ascwc:ProgressFileUploader ID="_fileUploader" EnableHtml5="false" runat="server" />


<asp:PlaceHolder ID="_uploadSwitchHolder" runat="server"></asp:PlaceHolder>

<a class="grayLinkButton" id="uploadButton" >Attach Files</a>

<div id="filesContainer" class="message_uploadContainer">

</div>

<%--


//   if (typeof(uploadedFiles) !== 'undefined')              
//   FileUploaderConfig.UploadedFiles  = uploadedFiles;  
//   FileHtml5Uploader.InitFileUploader(FileUploaderConfig); 
//                    OnAllUploadComplete : OnAllUploadCompleteCallback_function,
//                    OnUploadComplete : OnUploadCompleteCallback_function, 
//                    OnBegin : OnBeginCallback_function,                    
//                    DeleteLinkCSSClass : 'pm_deleteLinkCSSClass',
//                    LoadingImageCSSClass : 'pm_loadingCSSClass',
//                    CompleteCSSClass : 'pm_completeCSSClass',
//                    DragDropHolder : jq("#pm_DragDropHolder"),
//                    OverAllProcessHolder : jq("#pm_overallprocessHolder"),
//                  OverAllProcessBarCssClass : 'pm_overAllProcessBarCssClass'
//                    FilesHeaderCountHolder : jq("#pm_uploadHeader"),
//                    AddFilesText : "<%=ProjectsFileResource.AttachFiles%>", //ASC.Files.Resources.ButtonAddFiles,
//                    SelectFilesText : "<%=ProjectsFileResource.AttachFiles%>",// ASC.Files.Resources.ButtonSelectFiles,
//                    OverallProgressText : "<%=ProjectsFileResource.OverallProgress%>"// ASC.Files.Resources.OverallProgress,
//                    FilesHeaderText : "{0} / {1} files uploaded", // ASC.Files.Resources.UploadedOf,
//                    SelectFileText : "Select Files to upload",// ASC.Files.Resources.SelectFilesToUpload,
//                    DragDropText : "or drag'n'drop to this window", //ASC.Files.Resources.OrDragDrop,
//                    SelectedFilesText : "{0} selected files" //ASC.Files.Resources.SelectedFiles//,                
--%>