<%@ Assembly Name="ASC.Web.Community.Forum" %>
<%@ Assembly Name="ASC.Web.Controls" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForumMaker.ascx.cs"
    Inherits="ASC.Web.Community.Forum.ForumMaker" %>
<%@ Import Namespace="ASC.Web.Community.Forum" %>
<%@ Import Namespace="ASC.Web.Community.Forum.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<input type="hidden" id="forum_fmCallback" value="" />
<div id="forum_fmDialog" style="display: none;">
    <ascwc:Container ID="_forumMakerContainer" runat="server">
        <header>
            <%=ForumResource.AddThreadCategoryTitle%>
          </header>
        <body>
            <div id="forum_fmContent">
                <div id="forum_fmMessage" class='infoPanel alert' style='margin: 10px 0;'>
                </div>
                <div class="clearFix" id="forum_fmCategoryList">
                </div>
                <div class="clearFix requiredField" id="forum_fmCaregoryNameBox" style="margin-top: 10px;">
                    <span class="requiredErrorText"></span>
                    <div class="headerPanel" style="font-size: 12px; font-weight: normal;">
                        <%=ForumResource.ThreadCategoryName%>:</div>
                    <div style="margin-top: 3px;">
                        <input class="textEdit" style="width: 99%;" type="text" id="forum_fmCategoryName"
                            value="" /></div>
                </div>
                <div class="clearFix requiredField" style="margin-top: 10px;">
                    <span class="requiredErrorText"></span>
                    <div class="headerPanel" style="font-size: 12px; font-weight: normal;">
                        <%=ForumResource.ThreadName%>:</div>
                    <div style="margin-top: 3px;">
                        <input class="textEdit" style="width: 99%;" type="text" id="forum_fmThreadName" value="" /></div>
                </div>
                <div class="clearFix" style="margin-top: 10px;">
                    <div>
                        <%=ForumResource.ThreadDescription%>:</div>
                    <div style="margin-top: 3px;">
                        <textarea style="width: 99%; height: 100px;" id="forum_fmThreadDescription"></textarea></div>
                </div>
                <div class="clearFix" style="margin-top: 16px;" id="forum_fm_panel_buttons">
                    <a class="baseLinkButton" style="float: left;" href="javascript:ForumMakerProvider.SaveThreadCategory();">
                        <%=ForumResource.CreateButton%></a> <a class="grayLinkButton" style="float: left;
                            margin-left: 8px;" href="javascript:ForumMakerProvider.CloseFMDialog();">
                            <%=ForumResource.CancelButton%></a>
                </div>
                <div style="margin-top: 16px; display: none;" id="forum_fm_action_loader"
                    class="clearFix">                    
                    <div class="textMediumDescribe">
                        <%=ForumResource.PleaseWaitMessage%>
					</div>
					<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>
            </div>
            <div id="forum_fmInfo" style="padding: 20px 0px; text-align: center; display: none;">
                <%=ForumResource.SuccessfullyCreateForumMessage%>
            </div>
        </body>
        <options ispopup="true" />
    </ascwc:Container>
</div>
