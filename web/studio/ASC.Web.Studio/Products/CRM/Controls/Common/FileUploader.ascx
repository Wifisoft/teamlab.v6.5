<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploader.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Common.FileUploader" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>


<div id='pm_DragDropHolder' style="border:1px solid #d1d1d1; padding:15px"> 
    <table cellpadding="0" cellspacing="0" border="0">
        <tr valign="middle">
            <td style="width:50px; padding:5px 0 0 10px;">
                <div class="pm_uploadIcon"></div>
            </td>
            <td height="20"> 
                <div class="describeUpload">
                    <%=ASC.Web.Studio.Core.FileSizeComment.GetFileSizeNote()%>
                </div>
            </td>
        </tr>
    </table>
    <div id="pm_overallprocessHolder">
    </div>
    <div id="history_uploadContainer" class="history_uploadContainer">
    </div>
    <div id="pm_upload_pnl" style="display: none; padding:15px 0 10px;">
        <div class="clearFix" id="pm_swf_button_container">
            <a class="grayLinkButton pm_upload_btn" id="pm_upload_btn" style="width:100px;"><%= CRMCommonResource.UploadFile%></a>
            <div id="ProgressFileUploader">
                <ascwc:ProgressFileUploader ID="_fileUploader" EnableHtml5="true"  runat="server" />
            </div>
            <div style="float:right;margin-top:-10px">
                <asp:PlaceHolder ID="_uploadSwitchHolder" runat="server"></asp:PlaceHolder>
            </div>
        </div>
    </div>
    <div id="pm_upload_btn_html5" class="pm_upload_btn_html5"></div>
</div>

<script type="text/javascript">
    var currUserID = "<%=ASC.Core.SecurityContext.CurrentAccount.ID%>";
    var fileSizeLimit = "<%=ASC.Web.Studio.Core.SetupInfo.MaxUploadSize%>";
    var eventID = 0;
</script>      


