<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NamingPeopleSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.NamingPeopleSettings" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>

 <div class="headerBase borderBase clearFix" id="namingPeopleTitle">
    <div style="float: left;">
                <%=Resources.Resource.NamingPeopleSettings%>
    </div>
	
    <div class="HelpCenterSwitcher title" onclick="jq(this).helper({ BlockHelperID: 'AnswerForHelpTeamTemplate'});" title="<%=Resources.Resource.HelpQuestionTeamTemplate%>"></div> 
    <div class="popup_helper" id="AnswerForHelpTeamTemplate">
     <p><%=String.Format(Resources.Resource.HelpAnswerTeamTemplate, "<br />", "<b>", "</b>")%>
     <a href="http://teamlab.com/help/administratorguides/customize-portal.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
    </div>   

	
  </div>
  
<div id="studio_namingPeopleInfo">
</div>
<div id="studio_namingPeopleBox">
       
    <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
         
     <div class="btnBox clearFix">
            <a id="saveNamingPeopleBtn" class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" href="javascript:void(0);"><%=Resources.Resource.SaveButton %></a>
     </div>
</div>    

