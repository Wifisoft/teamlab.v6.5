<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AccessSettings" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Controls" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="ascwc" %>

 <div class="headerBase borderBase clearFix" id="mailDomainSettingsTitle">
    <div class="title">
		<%=Resources.Resource.StudioAccessSettings %>
		
	</div>
	<%--<div class="help">
	    <a target="_blank" class="linkDescribe" href="<%=ASC.Web.Studio.UserControls.Management.StudioSettings.ModifyHowToAdress("howto.aspx#Admin_Access")%>"><%=Resources.Resource.Help%></a>
	</div>--%>
</div>
<div id="studio_accessSettingsInfo">
</div>

<div id="studio_accessSettings">
 <div style="margin-bottom:10px;">
        <%=Resources.Resource.AccessSettingsDescription%>
    </div>
    <div class="choose clearFix">
        <div class="allow">
            <input type="radio" id="rbAllow" name="commonRules" onclick="AccessSettingsManager.ChangeRuleType(this);" /><label
                for="rbAllow" class="headerBaseSmall"><%= Resources.Resource.AccessSettingsAllowAll %></label></div>
        <div class="deny">
            <input type="radio" id="rbDeny" name="commonRules" onclick="AccessSettingsManager.ChangeRuleType(this);" /><label for="rbDeny" class="headerBaseSmall"><%= Resources.Resource.AccessSettingsDenyAll %></label></div>
        <div class="restricted">
            <input type="radio" id="rbSpecialRules" name="commonRules" onclick="AccessSettingsManager.ChangeRuleType(this);" /><label for="rbSpecialRules" class="headerBaseSmall"><%= Resources.Resource.AccessSettingsRestrictedAccess %></label></div>
    </div>
    <div class="clearFix">
    <div id="dvOwnerNote" style="display:none;">
        <%= Resources.Resource.AccessSettingsDenyNote %>
    </div>
    </div>
    <div id="dvSpecialRules" style="display:none">
     <div id="dvAdd" runat="server" class="add clearFix">
            <div style="float: left;">
                <ascwc:AdvancedUserSelector runat="server" ID="shareUserSelector" IsLinkView="true">
                </ascwc:AdvancedUserSelector>
            </div>
            
            
            <span style="float: left; margin-left: 10px;">
                <asp:PlaceHolder ID="groupSelectorHolder" runat="server"></asp:PlaceHolder>
            </span>
            
            
        </div>
    <table id="userList" cellpadding="0" cellspacing="0">
    </table>
        <div id="addTagDialog" class="borderBase" style="display: none;">
            <div class="popup-corner">
            </div>
            <div style="margin-bottom: 5px;">
                IP:
            </div>
            <div style="vertical-align:top;">
            <input type="text" class="textEdit" />
            <a class="baseLinkButton" id="addThisTag">
                OK
            </a>
            </div>
        </div>
    </div>
    <div class="btnBox clearFix">
        <a class="baseLinkButton" id="saveAccessSettingsBtn" href="javascript:void(0);">
            <%=Resources.Resource.SaveButton %></a>
    </div>
</div>
