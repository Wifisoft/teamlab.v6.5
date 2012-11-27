<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AdminSettings" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascwc" %>

<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Core.Users" %>
 
<ascwc:TreeViewProScriptManager ID="treeViewProScriptManager" runat="server"></ascwc:TreeViewProScriptManager>        
<asp:PlaceHolder runat="server" ID="_userSelectorHolder"></asp:PlaceHolder>

<span id="questionForHelp" style="display:none;"><%=Resources.Resource.HelpQuestionAdministrators%></span>

<div id="studio_adminSettingsInfo"></div>
<div id="studio_adminSettingsBox" class="clearFix">

    <div class="ownerSettings clearFix">
       <div class="curOwner">
            <span class="headerBaseMedium"><%=Resources.Resource.PortalOwner%>:</span> <%=_owner.RenderProfileLink(Guid.Empty) %>
       </div>
       <%if (_canOwnerEdit)
         { %>
            
            <div class="changeOwner">
                <div class="clearFix">
                <span class="headerBaseMedium"><%=Resources.Resource.ChangeOwnerTitle%></span>                                
                <asp:DropDownList CssClass="combox" runat="server" ID="_ownerSelector"></asp:DropDownList>    
                <a class="grayLinkButton" href="javascript:void(0);" id="changeOwnerBtn"><%=Resources.Resource.ChangeButton%></a>                
                <span class="textBigDescribe"><%=Resources.Resource.ChangeOwnerDescription%></span>
                </div>
            </div>
       <%} %>
   </div>
    

  <div id="studio_adminsListBox">  
    <asp:PlaceHolder runat="server" ID="_adminListHolder"></asp:PlaceHolder>     
 </div>
    <div class="btnBox clearFix">
        <a id="editAdminBtn" href="javascript:void(0);" 
            class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>"><%=Resources.Resource.EditButton%></a>
    </div>
</div>


<%--popup window--%>
  <div class="popup_helper" id="AnswerForHelpAdministrators">
       <p><%=String.Format(Resources.Resource.HelpAnswerAdministrators, "<br />", "<b>", "</b>")%>
       <a href="http://teamlab.com/help/gettingstarted/administration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
  </div>    