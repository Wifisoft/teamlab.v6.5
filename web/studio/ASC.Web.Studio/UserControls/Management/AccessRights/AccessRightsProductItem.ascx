<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRightsProductItem.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AccessRightsProductItem" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<% if (!ProductItem.CanNotBeDisabled) %>
<% { %>
<div class="accessRights-headerPanel headerBase <%= ProductItem.HasPermissionSettings ? "accessRights-switcher" : "" %> <%= ProductItem.Disabled ? "accessRights-disabledText" : "" %>">
    <% if (ProductItem.HasPermissionSettings && ProductItem.SubItems != null && ProductItem.SubItems.Count > 0)%>
    <% { %>
    <a class="baseLinkAction linkMedium" style="float:right; padding-top:10px;"
     onclick="ASC.Settings.AccessRights.manageProduct(event,
     '<%= ProductItem.ID %>', '<%= ProductItem.ItemName %>',
     ASC.Settings.AccessRights.Selectors.<%= ProductItem.ItemName %>.getSubjects())">
     <%= Resources.Resource.AccessRightsManage %>
    </a>
    <% } %>
    <% else %>
    <% { %>
    <a class="baseLinkAction linkMedium" style="float:right; padding-top:10px;"
     onclick="ASC.Settings.AccessRights.accessSwitch(event,this,
     '<%= ProductItem.ID %>','<%= Resources.Resource.AccessRightsOn %>','<%= Resources.Resource.AccessRightsOff %>',
     <% if (ProductItem.HasPermissionSettings)
        { %>ASC.Settings.AccessRights.Selectors.<%= ProductItem.ItemName %>.getSubjects()<% }
        else
        { %>[]<% } %>)">
     <%= ProductItem.Disabled ? Resources.Resource.AccessRightsOn : Resources.Resource.AccessRightsOff %>
    </a>
    <% } %>
    <% if (ProductItem.HasPermissionSettings)%>
    <% { %>
    <div class="accessRights-collapseDown"></div>
    <% } %>
    <img src="<%= ProductItem.IconUrl %>" align="absmiddle" <%= ProductItem.Disabled ? "style='display:none'" : "" %>/>
    <img src="<%= ProductItem.DisabledIconUrl %>" align="absmiddle" <%= ProductItem.Disabled ? "" : "style='display:none'" %>/>
    <%= ProductItem.Name %>
</div>

<div class="accessRights-content">
    <div <%= ProductItem.Disabled ? "style='display:none'" : "" %>>
    <% if (ProductItem.HasPermissionSettings)%>
    <% { %>
    <table width="100%" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td width="390px" valign="top">
                    <div><%= String.Format(Resources.Resource.AccessRightsAccessToProduct, ProductItem.Name) %>:</div>
                    <div style="padding: 4px 0 3px">
                        <input type="radio" id="all_<%= ProductItem.ID %>" name="radioList_<%= ProductItem.ID %>"
                         onclick="ASC.Settings.AccessRights.changeAccessType(this,'<%= ProductItem.ItemName %>',
                         ASC.Settings.AccessRights.Selectors.<%= ProductItem.ItemName %>.getSubjects())">
                        <label for="all_<%= ProductItem.ID %>"><%= Resources.Resource.AccessRightsAllUsers %></label>  
                    </div>
                    <div style="padding: 4px 0 3px">
                        <input type="radio" id="fromList_<%= ProductItem.ID %>" name="radioList_<%= ProductItem.ID %>"
                         onclick="ASC.Settings.AccessRights.changeAccessType(this,'<%= ProductItem.ItemName %>',
                         ASC.Settings.AccessRights.Selectors.<%= ProductItem.ItemName %>.getSubjects())">
                        <label for="fromList_<%= ProductItem.ID %>"><%= Resources.Resource.AccessRightsUsersFromList %></label> 
                    </div>
                </td>
                <td valign="top">
                    <% if (ProductItem.UserOpportunities != null && ProductItem.UserOpportunities.Count > 0)%>
                    <% { %>
                    <div><%= ProductItem.UserOpportunitiesLabel %>:</div>
                    <% foreach (var item in ProductItem.UserOpportunities)%>
                    <% { %>
                    <div class="accessRights-infoText"><%= item %>;</div>
                    <% } %>
                    <% } %>
                </td>
            </tr>
        </tbody>
    </table>
    <div id="selectorContent_<%= ProductItem.ItemName %>" style="margin-top:10px">
        <div id="emptyUserListLabel_<%= ProductItem.ItemName %>" class="textBigDescribe" style="padding: 0 10px 7px 0;">
            <%= Resources.Resource.AccessRightsEmptyUserList %>
        </div>
        <div id="selectedUsers_<%= ProductItem.ItemName %>" class="clearFix">
            <% if (ProductItem.SelectedUsers.Count > 0)%>
            <% { %>
                <% foreach (var user in ProductItem.SelectedUsers)%>
                <% { %>
                    <div class="accessRights-selectedItem" id="selectedUser_<%= ProductItem.ItemName %>_<%= user.ID %>"
                     onmouseover="ASC.Settings.AccessRights.selectedItem_mouseOver(this);"
                      onmouseout="ASC.Settings.AccessRights.selectedItem_mouseOut(this);">
                        <img src="<%= PeopleImgSrc %>">
                        <img src="<%= TrashImgSrc %>" id="deleteSelectedUserImg_<%= ProductItem.ItemName %>_<%= user.ID %>"
                         onclick="ASC.Settings.AccessRights.Selectors.<%= ProductItem.ItemName %>.deleteUserFromList(this);"
                          title="<%= Resources.Resource.DeleteButton %>" style="display: none">
                        <%= user.DisplayUserName() %>
                    </div>
                <% } %>
            <% } %>
        </div>
        <div id="selectedGroups_<%= ProductItem.ItemName %>" class="clearFix">
            <% if (ProductItem.SelectedGroups.Count > 0)%>
            <% { %>
                <% foreach (var group in ProductItem.SelectedGroups)%>
                <% { %>
                    <div class="accessRights-selectedItem" id="selectedGroup_<%= ProductItem.ItemName %>_<%= group.ID %>"
                     onmouseover="ASC.Settings.AccessRights.selectedItem_mouseOver(this);"
                      onmouseout="ASC.Settings.AccessRights.selectedItem_mouseOut(this);">
                        <img src="<%= GroupImgSrc %>">
                        <img src="<%= TrashImgSrc %>" id="deleteSelectedGroupImg_<%= ProductItem.ItemName %>_<%= group.ID %>"
                         onclick="ASC.Settings.AccessRights.Selectors.<%= ProductItem.ItemName %>.deleteGroupFromList(this);"
                          title="<%= Resources.Resource.DeleteButton %>" style="display: none">
                        <%= group.Name %>
                    </div>
                <% } %>
            <% } %>
        </div>
        <div class="accessRights-selectorContent">
            <asp:PlaceHolder runat="server" ID="phUserSelector"></asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="phGroupSelector"></asp:PlaceHolder>
        </div>
    </div>
    <% if (ProductItem.SubItems != null && ProductItem.SubItems.Count > 0)%>
    <% { %>
    <div id="managePanel_<%= ProductItem.ItemName %>" style="display: none;">
        <ascwc:container id="_managePanel" runat="server">
            <header>    
                <%= String.Format(Resources.Resource.AccessRightsManageProduct, ProductItem.Name)%>
            </header>
            <body>
                <table>
                    <tr>
                        <td>
                            <input type="checkbox" id="cbxEnableProduct_<%=ProductItem.ID%>"
                                onclick="ASC.Settings.AccessRights.selectModules(this)"
                                default="<%=(!ProductItem.Disabled).ToString().ToLower()%>" />
                        </td>
                        <td>
                            <label for="cbxEnableProduct_<%=ProductItem.ID%>">
                                <%= String.Format(Resources.Resource.AccessRightsMakeVisibleProduct, ProductItem.Name)%>
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div style="margin:10px 0;">
                                <%= String.Format(Resources.Resource.AccessRightsSelectProductModules, ProductItem.Name)%>
                            </div>
                        </td>
                    </tr>
                    <% foreach (var item in ProductItem.SubItems) %>
                    <% { %>
                    <tr>
                        <td>
                            <% if (!item.DisplayedAlways) %>
                            <% { %>
                            <input type="checkbox" id="cbxEnableModule_<%=item.ID%>" default="<%=(!item.Disabled && !ProductItem.Disabled).ToString().ToLower()%>"/>
                            <% } %>
                            <% else %>
                            <% { %>
                            <input type="checkbox" default="<%=(!ProductItem.Disabled).ToString().ToLower()%>"/>
                            <% } %>
                        </td>
                        <td>
                            <% if (!item.DisplayedAlways) %>
                            <% { %>
                            <label for="cbxEnableModule_<%=item.ID%>"><%=item.Name%></label>
                            <% } %>
                            <% else %>
                            <% { %>
                            <%=item.Name%>
                            <% } %>
                        </td>
                    </tr>
                    <% } %>
                </table>
                <br />
                <div class="action-block">
                    <a class="baseLinkButton" onclick="">
                        <%=Resources.Resource.SaveButton%>
                    </a>
                    <span class="splitter"></span>
                    <a class="grayLinkButton" onclick="jq.unblockUI();">
                        <%=Resources.Resource.CancelButton%>
                    </a>
                </div>
                <div class='info-block' style="display: none;">
                    <span class="textMediumDescribe">
                        <%=Resources.Resource.LoadingProcessing%>
                    </span>
                    <br />
                    <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
                </div>
            </body>
        </ascwc:container>
    </div>
    <% } %>
    <% } %>
    <% else %>
    <% { %>
    <div><%= String.Format(Resources.Resource.AccessRightsOpenProduct, ProductItem.Name) %></div>
    <% } %>
    </div>
    <div class="accessRights-disabledText" <%= ProductItem.Disabled ? "" : "style='display:none'" %>>
        <%= String.Format(Resources.Resource.AccessRightsDisabledProduct, ProductItem.Name) %>
    </div>
</div>
<% } %>

<% if (ProductItem.HasPermissionSettings) %>
<% { %>
<script>

<% if (!ProductItem.CanNotBeDisabled) %>
<% { %>
jq(document).ready(function() {
    ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.init();
});
<% } %>

ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%> = (function($)
{
return {
    init: function() {
	    userSelector_<%=ProductItem.ItemName%>.AdditionalFunction = ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.pushUserIntoList;
	    groupSelector_<%=ProductItem.ItemName%>.AdditionalFunction = ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.pushGroupIntoList;
	    
	    for(var i=0; i<SelectedUsers_<%=ProductItem.ItemName%>.IDs.length; i++)
	    	userSelector_<%=ProductItem.ItemName%>.HideUser(SelectedUsers_<%=ProductItem.ItemName%>.IDs[i]);

    	jq("input[id^=check_<%=ProductItem.ItemName%>_]:checked").each(function() {
    		userSelector_<%=ProductItem.ItemName%>.HideUser(jq(this).attr("id").split("_")[2]);
    	});
    	
    	for(var i=0; i<SelectedGroups_<%=ProductItem.ItemName%>.IDs.length; i++)
	    	groupSelector_<%=ProductItem.ItemName%>.HideGroup(SelectedGroups_<%=ProductItem.ItemName%>.IDs[i]);
	    
	    <%if(PublicModule)%>
	    <%{%>
	    jq("#all_<%=ProductItem.ID%>").attr("checked",true);
    	jq("#emptyUserListLabel_<%=ProductItem.ItemName%>").show();
	    jq("#selectorContent_<%=ProductItem.ItemName%>").hide();
	    <%}%>
	    <%else%>
	    <%{%>
	    jq("#fromList_<%=ProductItem.ID%>").attr("checked",true);
    	jq("#emptyUserListLabel_<%=ProductItem.ItemName%>").hide();
	    jq("#selectorContent_<%=ProductItem.ItemName%>").show();
	    <%}%>
    },
    
    pushUserIntoList: function (uID, uName) {
        var alreadyExist = false;

        for (var i = 0; i < SelectedUsers_<%=ProductItem.ItemName%>.IDs.length; i++)
	        if (SelectedUsers_<%=ProductItem.ItemName%>.IDs[i] == uID) {
		        alreadyExist = true;
		        break;
	        }

        if (alreadyExist) return false;

        SelectedUsers_<%=ProductItem.ItemName%>.IDs.push(uID);
        SelectedUsers_<%=ProductItem.ItemName%>.Names.push(uName);

        var item = jq("<div></div>")
	        .attr("id", "selectedUser_<%=ProductItem.ItemName%>_" + uID)
	        .addClass("accessRights-selectedItem");

        var peopleImg = jq("<img>")
	        .attr("src", SelectedUsers_<%=ProductItem.ItemName%>.PeopleImgSrc);

        var deleteImg = jq("<img>")
	        .attr("src", SelectedUsers_<%=ProductItem.ItemName%>.TrashImgSrc)
	        .css("display", "none")
	        .attr("id", "deleteSelectedUserImg_<%=ProductItem.ItemName%>_" + uID)
	        .attr("title", SelectedUsers_<%=ProductItem.ItemName%>.TrashImgTitle);

        item.append(peopleImg).append(deleteImg).append(Encoder.htmlEncode(uName));

        jq("#selectedUsers_<%=ProductItem.ItemName%>").append(item);
    	jq("#emptyUserListLabel_<%=ProductItem.ItemName%>").hide();

        jq("#selectedUser_<%=ProductItem.ItemName%>_" + uID).unbind("mouseover").bind("mouseover", function() {
	        ASC.Settings.AccessRights.selectedItem_mouseOver(jq(this));
        });

        jq("#selectedUser_<%=ProductItem.ItemName%>_" + uID).unbind("mouseout").bind("mouseout", function() {
	        ASC.Settings.AccessRights.selectedItem_mouseOut(jq(this));
        });

        jq("#deleteSelectedUserImg_<%=ProductItem.ItemName%>_" + uID).unbind("click").bind("click", function() {
	        ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.deleteUserFromList(jq(this));
        });

        userSelector_<%=ProductItem.ItemName%>.ClearFilter();
    	jq("#selectedUsers_<%=ProductItem.ItemName%>").parent().find("div[id=DepsAndUsersContainer]").hide();
    	
    	var data = {
    		id: "<%=ProductItem.ID%>",
    		enabled: true
    	};
    	
    	var subjects = ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.getSubjects();
    	
		if(subjects.length>0)
	        data.subjects = subjects;
    	
	    Teamlab.setWebItemSecurity({}, data, {success: function (){ userSelector_<%=ProductItem.ItemName%>.HideUser(uID);}});
    },
    
    pushGroupIntoList: function (group) {
        var alreadyExist = false;

        for (var i = 0; i < SelectedGroups_<%=ProductItem.ItemName%>.IDs.length; i++)
	        if (SelectedGroups_<%=ProductItem.ItemName%>.IDs[i] == group.Id) {
		        alreadyExist = true;
		        break;
	        }

        if (alreadyExist) return false;

        SelectedGroups_<%=ProductItem.ItemName%>.IDs.push(group.Id);
        SelectedGroups_<%=ProductItem.ItemName%>.Names.push(group.Name);

        var item = jq("<div></div>")
	        .attr("id", "selectedGroup_<%=ProductItem.ItemName%>_" + group.Id)
	        .addClass("accessRights-selectedItem");

        var groupImg = jq("<img>")
	        .attr("src", SelectedGroups_<%=ProductItem.ItemName%>.GroupImgSrc);

        var deleteImg = jq("<img>")
	        .attr("src", SelectedGroups_<%=ProductItem.ItemName%>.TrashImgSrc)
	        .css("display", "none")
	        .attr("id", "deleteSelectedGroupImg_<%=ProductItem.ItemName%>_" + group.Id)
	        .attr("title", SelectedGroups_<%=ProductItem.ItemName%>.TrashImgTitle);

        item.append(groupImg).append(deleteImg).append(Encoder.htmlEncode(group.Name));

        jq("#selectedGroups_<%=ProductItem.ItemName%>").append(item);
    	jq("#emptyUserListLabel_<%=ProductItem.ItemName%>").hide();

        jq("#selectedGroup_<%=ProductItem.ItemName%>_" + group.Id).unbind("mouseover").bind("mouseover", function() {
	        ASC.Settings.AccessRights.selectedItem_mouseOver(jq(this));
        });

        jq("#selectedGroup_<%=ProductItem.ItemName%>_" + group.Id).unbind("mouseout").bind("mouseout", function() {
	        ASC.Settings.AccessRights.selectedItem_mouseOut(jq(this));
        });

        jq("#deleteSelectedGroupImg_<%=ProductItem.ItemName%>_" + group.Id).unbind("click").bind("click", function() {
	        ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.deleteGroupFromList(jq(this));
        });

        groupSelector_<%=ProductItem.ItemName%>.ClearFilter();
    	jq("#selectedUsers_<%=ProductItem.ItemName%>").parent().find("div[id^=groupSelectorContainer_]").hide();
    	
    	var data = {
    		id: "<%=ProductItem.ID%>",
    		enabled: true
    	};
    	
    	var subjects = ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.getSubjects();
    	
		if(subjects.length>0)
	        data.subjects = subjects;
    	
	    Teamlab.setWebItemSecurity({}, data, {success: function (){ groupSelector_<%=ProductItem.ItemName%>.HideGroup(group.Id);}});
    },

    deleteUserFromList: function (obj)
    {
	    var uID = jq(obj).attr("id").split("_")[2];
	    jq(obj).parent().remove();

	    for (var i = 0; i < SelectedUsers_<%=ProductItem.ItemName%>.IDs.length; i++) {
		    if (SelectedUsers_<%=ProductItem.ItemName%>.IDs[i] == uID) {
			    SelectedUsers_<%=ProductItem.ItemName%>.IDs.splice(i, 1);
			    SelectedUsers_<%=ProductItem.ItemName%>.Names.splice(i, 1);
			    break;
		    }
	    }
        
        if(SelectedUsers_<%=ProductItem.ItemName%>.IDs.length==0 && SelectedGroups_<%=ProductItem.ItemName%>.IDs.length==0)
	        jq("#emptyUserListLabel_<%=ProductItem.ItemName%>").show();
        
    	var data = {
    		id: "<%=ProductItem.ID%>",
    		enabled: true
    	};
    	
    	var subjects = ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.getSubjects();
    	
		if(subjects.length>0)
	        data.subjects = subjects;
	    
	    Teamlab.setWebItemSecurity({}, data, {success: function (){ userSelector_<%=ProductItem.ItemName%>.HideUser(uID, false);}});
    	
    },
    
    deleteGroupFromList: function (obj)
    {
	    var gId = jq(obj).attr("id").split("_")[2];
	    jq(obj).parent().remove();

	    for (var i = 0; i < SelectedGroups_<%=ProductItem.ItemName%>.IDs.length; i++) {
		    if (SelectedGroups_<%=ProductItem.ItemName%>.IDs[i] == gId) {
			    SelectedGroups_<%=ProductItem.ItemName%>.IDs.splice(i, 1);
			    SelectedGroups_<%=ProductItem.ItemName%>.Names.splice(i, 1);
			    break;
		    }
	    }
    	
    	if(SelectedUsers_<%=ProductItem.ItemName%>.IDs.length==0 && SelectedGroups_<%=ProductItem.ItemName%>.IDs.length==0)
    		jq("#emptyUserListLabel_<%=ProductItem.ItemName%>").show();

    	var data = {
    		id: "<%=ProductItem.ID%>",
    		enabled: true
    	};

    	var subjects = ASC.Settings.AccessRights.Selectors.<%=ProductItem.ItemName%>.getSubjects();
    	
		if(subjects.length>0)
	        data.subjects = subjects;
	    
	    Teamlab.setWebItemSecurity({}, data, {success: function (){ groupSelector_<%=ProductItem.ItemName%>.HideGroup(gId, false);}});
    },
    
    getSubjects: function() {
        return SelectedUsers_<%=ProductItem.ItemName%>.IDs.concat(SelectedGroups_<%=ProductItem.ItemName%>.IDs);
    },
    
    setAdmin: function(obj, pId) {
		var uId = jq(obj).attr("id").split("_")[2];
		var isChecked = jq(obj).is(":checked");

		var params = { };
	    var data = {
		    productid: pId,
		    userid: uId,
		    administrator : isChecked
	    };
		Teamlab.setProductAdministrator(params, data, {
			success: function() {
            	if (isChecked)
            		userSelector_<%=ProductItem.ItemName%>.HideUser(uId, true);
            	else
            		userSelector_<%=ProductItem.ItemName%>.HideUser(uId, false);
			}
		});
    }
}
})();
</script>
<% } %>