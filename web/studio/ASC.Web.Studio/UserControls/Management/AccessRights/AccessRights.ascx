<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRights.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AccessRights" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="aswc" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<script>
    jq(document).ready(function() {
    	ASC.Settings.AccessRights.init();
    });

    ASC.Settings.AccessRights.Selectors.full = { };
    
    ASC.Settings.AccessRights.Selectors.full.setAdmin = function (obj, pId) {
    	var uId = jq(obj).attr("id").split("_")[2];
    	var isChecked = jq(obj).is(":checked");
    	var params = { };
    	var data = {
    		productid: pId,
    		userid: uId,
    		administrator: isChecked
    	};
    	Teamlab.setProductAdministrator(params, data, {
    		success: function() {
    			if (isChecked) {
			        jq("#adminItem_" + uId + " input[type=checkbox]").attr("checked", true).attr("disabled", true);
			        jq(obj).removeAttr("disabled");
		        } else {
			        jq("#adminItem_" + uId + " input[type=checkbox]").removeAttr("checked").removeAttr("disabled");
		        }
    			ASC.Settings.AccessRights.Selectors.full.hideUser(uId, isChecked);
    		}
    	});
    };
    
    ASC.Settings.AccessRights.Selectors.full.hideUser = function (uId, hide)
    {
    	<% foreach (var p in Products) %>
		<% { %>
		userSelector_<%=p.GetSysName()%>.HideUser(uId, hide);
		<% } %>
    }

</script>

<script id="adminTmpl" type="text/x-jquery-tmpl">
    <tr id="adminItem_${id}" style="height:44px;">
        <td class="borderBase" style="width:32px;">
            <img src="${smallFotoUrl}" />
        </td>
        <td class="borderBase">
            <a class="linkHeader" href="${userUrl}">
                ${displayName}
            </a>
            <div>
                ${title}
            </div>
        </td>
        {{each(i, item) accessList}}
            <td class="borderBase cbxCell">            
                <input type="checkbox" id="check_${item.pName}_${id}"
                 {{if item.pAccess}}checked="checked"{{/if}}
                 {{if item.disabled}}disabled="disabled"{{/if}}
                 onclick="ASC.Settings.AccessRights.Selectors.${item.pName}.setAdmin(this, '${item.pId}')" >
            </td>
        {{/each}}
    </tr>
</script>

<div id="accessRightsInfo"></div>

<div class="accessRights-headerPanel headerBase" style="margin-left: 0px;">
    <%=Resources.Resource.PortalOwner%>
</div>
    
<div class="clearFix">
    <div class="accessRights-ownerCard" style="float:left;">
        <asp:PlaceHolder runat="server" ID="_phOwnerCard" />
    </div>
    <div style="margin-left: 380px;padding: 0 30px;">
        <div><%=Resources.Resource.AccessRightsOwnerCan%>:</div>
        <% foreach (var item in Resources.Resource.AccessRightsOwnerOpportunities.Split('|')) %>
        <% { %>
        <div class="accessRights-infoText"><%=item.Trim()%>;</div>
        <% } %>
    </div>
</div>

<% if (CanOwnerEdit) %>
<% { %>
<div id="ownerSelectorContent">
    <div style="margin:20px 0 10px">
        <%=Resources.Resource.AccessRightsChangeOwnerText%>
    </div>
    <div style="float: right; width: 700px;">
        <a class="baseLinkButton" id="changeOwnerBtn"><%=Resources.Resource.AccessRightsChangeOwnerButtonText%></a>
        <span class="splitter"></span>
        <span class="textBigDescribe"><%=Resources.Resource.AccessRightsChangeOwnerConfirmText%></span>
    </div>
    <aswc:AdvancedUserSelector runat="server" ID="ownerSelector"></aswc:AdvancedUserSelector>
</div>
<% } %>

<div class="accessRights-headerPanel headerBase accessRights-switcher">
    <div class="accessRights-collapseDown"></div>
    <%=Resources.Resource.AdminSettings%>
</div>

<div class="accessRights-content" style="padding-top: 10px;">
    <table id="adminTable" class="tableBase" cellpadding="4" cellspacing="0">
        <thead>
            <tr>
                <th></th>
                <th></th>
                <th class="cbxHeader">
                    <%=Resources.Resource.AccessRightsFullAccess%>
                   <div class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'full_panelQuestion'});"></div> 
                   <%-- <a class="linkQuestion full_linkQuestion"></a>--%>
                </th>
                <% foreach (var p in Products) %>
                <% { %>
                <th class="cbxHeader">
                    <%=p.Name%>
                    <% if (p.GetAdminOpportunities().Count>0) %>
                    <% { %>
                    <div class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: '<%=p.GetSysName()%>_panelQuestion'});"></div>
                    <% } %>
                </th>
                <% } %>
            </tr>
        </thead>
        <tbody></tbody>
    </table>
    <aswc:AdvancedUserSelector runat="server" ID="adminSelector"></aswc:AdvancedUserSelector>
    <div>
        <div id="full_panelQuestion" class="popup_helper"> 
            <% for (var i = 0; i < FullAccessOpportunities.Length; i++) %>
            <% { %>
            <% if (i==0) %>
            <% { %>
            <div><%=FullAccessOpportunities[i]%>:</div>
            <% } %>
            <% else %>
            <% { %>
            <div class="accessRights-alertText"><%=FullAccessOpportunities[i]%>;</div>
            <% } %>
            <% } %>
        </div>  
        <% foreach (var p in Products) %>
        <% { %>
        <% if (p.GetAdminOpportunities().Count>0) %>
        <% { %>
        <div id="<%=p.GetSysName()%>_panelQuestion" class="popup_helper">
            <div><%=String.Format(Resources.Resource.AccessRightsProductAdminsCan, p.Name)%>:</div>
            <% foreach (var oprtunity in p.GetAdminOpportunities()) %>
            <% { %>
            <div class="accessRights-infoText"><%=oprtunity%>;</div>
            <% } %>
        </div>
        <% } %>
        <% } %>
    </div>
</div>

<asp:Repeater runat="server" ID="rptProducts">
    <ItemTemplate>
        <asp:PlaceHolder ID="phProductItem" runat="server"></asp:PlaceHolder>
    </ItemTemplate>
</asp:Repeater>