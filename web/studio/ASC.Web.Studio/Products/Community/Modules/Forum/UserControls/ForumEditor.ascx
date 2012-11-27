<%@ Assembly Name="ASC.Web.Community.Forum"%>
<%@ Import Namespace="ASC.Web.Community.Forum" %>
<%@ Import Namespace="ASC.Web.Community.Forum.Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForumEditor.ascx.cs" Inherits="ASC.Web.Community.Forum.ForumEditor" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascwc"  %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascwc_ws"  %>
  
  <input type="hidden" id="forum_editCategoryID" value="" />
  <input type="hidden" id="forum_securityObjID" value"" />
   
<%--edit category dlg--%>
<div id="forum_editCategoryDialog" style="display: none;">
    <ascwc:Container ID="EditCategoryContainer" runat="server">
        <Header>
            <%=ForumResource.ThreadCategoryEditor%>
          </Header>
        <Body>            
                <div id="forum_editCategoryMessage" class='infoPanel alert' style='margin:10px 0;'>
                </div>
                <div class="clearFix" style="margin-top: 3px;">
                    <div>
                        <%=ForumResource.ThreadCategoryName%>:</div>
                    <div style="margin-top: 3px;">
                        <input class="textEdit" style="width:99%;" type="text" id="forum_editCategoryName"
                            value="" /></div>
                </div>
                <div class="clearFix" style="margin-top: 10px;">
                    <div>
                        <%=ForumResource.ThreadCategoryDescription%>:</div>
                    <div style="margin-top: 3px;">
                        <textarea style="width: 99%; height: 100px;" id="forum_editCategoryDescription"></textarea></div>
                </div>
                <div class="clearFix" style="margin-top: 16px;" id="forum_edit_categ_panel_buttons">
                    <a class="baseLinkButton" style="float: left;" href="javascript:ForumMakerProvider.SaveCategory('edit');">
                        <%=ForumResource.SaveButton%></a> <a class="grayLinkButton" style="float: left; margin-left: 8px;"
                            href="javascript:ForumMakerProvider.CloseDialogByID('forum_editCategoryDialog');">
                            <%=ForumResource.CancelButton%></a>
                </div>  
                <div style="margin-top: 16px; display: none;" id="forum_edit_categ_action_loader"
                    class="clearFix">                    
                    <div class="textMediumDescribe" style="margin: -1px 0pt 0pt 10px; float: left;">
                        <%=ForumResource.PleaseWaitMessage%>
					</div>
					<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>           
        </Body>
        <Options IsPopup="true"/>
    </ascwc:Container>
</div>
 
 <%--new cat--%>
<div id='forum_newCategoryDialog' style="display:none;">
<ascwc:Container ID="NewCategoryContainer" runat="server">
        <Header>
            <%=ForumResource.ThreadCategoryEditor%>
          </Header>
        <Body>
            <div id="forum_newCategoryMessage" class='infoPanel alert' style='margin:10px 0;'></div>        
            <div class="clearFix" style="margin-top:3px;">
                <div><%=ForumResource.ThreadCategoryName%>:</div>
                <div style="margin-top:3px;"><input class="textEdit" style="width:99%;" type="text" id="forum_newCategoryName" value="" /></div>    
            </div>
            <div class="clearFix" style="margin-top:10px;">
                <div><%=ForumResource.ThreadCategoryDescription%>:</div>
                <div style="margin-top:3px;"><textarea style="width:99%; height:100px;" id="forum_newCategoryDescription"></textarea></div>    
            </div>    
            
            <div class="clearFix" style="margin-top:16px;" id="forum_new_categ_panel_buttons">
            <a class="baseLinkButton" style="float:left;" href="javascript:ForumMakerProvider.SaveCategory('new');"><%=ForumResource.SaveButton%></a>
            <a class="grayLinkButton" style="float:left; margin-left:8px;" href="javascript:ForumMakerProvider.CloseDialogByID('forum_newCategoryDialog');"><%=ForumResource.CancelButton%></a>
            </div>
            <div style="margin-top: 16px; display: none;" id="forum_new_categ_action_loader"
                    class="clearFix">                    
                    <div class="textMediumDescribe">
                        <%=ForumResource.PleaseWaitMessage%>
					</div>
					<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>
            </Body>
        <Options IsPopup="true"/>
    </ascwc:Container>
 </div> 
 
 <input type="hidden" id="forum_editThreadID" value="" />
 
 <%--new forum--%>
 <div id='forum_newThreadDialog' style="display:none;">
 <ascwc:Container ID="NewThreadContainer" runat="server">
        <Header>
            <%=ForumResource.ThreadEditor%>
          </Header>
        <Body>            
                    <div id="forum_newThreadMessage" class='infoPanel alert' style='margin:10px 0;'></div>    
                    <input type="hidden" id="forum_newThreadCategoryID" value=""/>    
                    <div class="clearFix" style="margin-top:3px;">
                        <div><%=ForumResource.ThreadName%>:</div>
                        <div style="margin-top:3px;"><input class="textEdit" style="width:99%;" type="text" id="forum_newThreadName" value="" /></div>    
                    </div>
                    <div class="clearFix" style="margin-top:10px;">
                        <div><%=ForumResource.ThreadDescription%>:</div>
                        <div style="margin-top:3px;"><textarea style="width:99%; height:100px;" id="forum_newThreadDescription"></textarea></div>    
                    </div>    
                    
                    <div class="clearFix" style="margin-top:16px;" id="forum_new_thread_panel_buttons">
                    <a class="baseLinkButton" style="float:left;" href="javascript:ForumMakerProvider.SaveThread('new');"><%=ForumResource.SaveButton%></a>
                    <a class="grayLinkButton" style="float:left; margin-left:8px;" href="javascript:ForumMakerProvider.CloseDialogByID('forum_newThreadDialog');"><%=ForumResource.CancelButton%></a>
                    </div>  
                    <div style="margin-top: 16px; display: none;" id="forum_new_thread_action_loader"
                    class="clearFix">                    
                    <div class="textMediumDescribe">
                        <%=ForumResource.PleaseWaitMessage%>
					</div>
					<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>               
            </Body>
        <Options IsPopup="true"/>
    </ascwc:Container>
 </div> 
 
 <%--forum editor--%>
  <div id='forum_editThreadDialog' style="display:none;">
  <ascwc:Container ID="EditThreadContainer" runat="server">
        <Header>
            <%=ForumResource.ThreadEditor%>
          </Header>
        <Body>
                    <div id="forum_editThreadMessage" class='infoPanel alert' style='margin:10px 0;'></div>        
                    <input type="hidden" id="forum_editThreadCategoryID" value=""/>
                    <div class="clearFix" style="margin-top:3px;">
                        <div><%=ForumResource.ThreadName%>:</div>
                        <div style="margin-top:3px;"><input class="textEdit" style="width:99%;" type="text" id="forum_editThreadName" value="" /></div>    
                    </div>
                    <div class="clearFix" style="margin-top:10px;">
                        <div><%=ForumResource.ThreadDescription%>:</div>
                        <div style="margin-top:3px;"><textarea style="width:99%; height:100px;" id="forum_editThreadDescription"></textarea></div>    
                    </div>    
                    
                    <div class="clearFix" style="margin-top:16px;" id="forum_edit_thread_panel_buttons">
                    <a class="baseLinkButton" style="float:left;" href="javascript:ForumMakerProvider.SaveThread('edit');"><%=ForumResource.SaveButton%></a>
                    <a class="grayLinkButton" style="float:left; margin-left:8px;" href="javascript:ForumMakerProvider.CloseDialogByID('forum_editThreadDialog');"><%=ForumResource.CancelButton%></a>
                    </div>
                    <div style="margin-top: 16px; display: none;" id="forum_edit_thread_action_loader"
                    class="clearFix">                    					
                    <div class="textMediumDescribe">
                        <%=ForumResource.PleaseWaitMessage%>
					</div>
					<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
                </div>  
            </Body>
        <Options IsPopup="true"/>
    </ascwc:Container>
 </div> 
 

 <div class="clearFix" style="padding-bottom:10px;">
        <a class="baseLinkButton" style="float:left;" href="javascript:ForumMakerProvider.ShowForumMakerDialog(true);"><%=ForumResource.AddThreadButton %></a>
    </div> 

<div id="forum_threadCategories">
    <%=RenderForumCategories()%>
</div>
