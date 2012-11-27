<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSelectorListView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.UserSelectorListView" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="aswc" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div>
    <div id="selectedUsers<%=ObjId%>" class="clearFix" style="margin-top: 10px;">
        <% if (UsersWhoHasAccess != null && UsersWhoHasAccess.Count > 0) %>
        <% { %>
            <% foreach (var obj in UsersWhoHasAccess) %>
            <% { %>
                <div class="selectedUser">
                    <img src="<%=PeopleImgSrc%>">
                    <%=obj%>
                </div>
            <% } %>
        <% } %>
        <% if (SelectedUsers != null && SelectedUsers.Count > 0) %>
        <% { %>
            <% foreach(var obj in SelectedUsers) %>
            <% { %>
                <div class="selectedUser" id="selectedUser_<%=obj.Key%><%=ObjId%>"
                 onmouseover="ASC.CRM.UserSelectorListView<%=ObjId%>.selectedUser_mouseOver(this);"
                  onmouseout="ASC.CRM.UserSelectorListView<%=ObjId%>.selectedUser_mouseOut(this);">
                    <img src="<%=PeopleImgSrc%>">
                    <img src="<%=DeleteImgSrc%>" id="deleteSelectedUserImg_<%=obj.Key%><%=ObjId%>"
                     onclick="ASC.CRM.UserSelectorListView<%=ObjId%>.selectedUser_deleteItem(this);"
                      title="<%=CRMCommonResource.DeleteUser%>" style="display: none">
                    <%=obj.Value%>
                </div>
            <% } %>
        <% } %>
    </div>
    <% if(ShowNotifyPanel) %>
    <% { %>
    <div style="float:right" id="notifyPanel<%=ObjId%>">
        <input type="checkbox" id="cbxNotify<%=ObjId%>" style="float: left;">
        <label for="cbxNotify<%=ObjId%>" style="float:left; padding: 2px 0 0 4px;">
            <%= CRMCommonResource.Notify %>
        </label>
    </div>
    <% } %>
    <asp:PlaceHolder runat="server" ID="_phAdvUserSelector" />
</div>

<script>

    jq(document).ready(function() {
        ASC.CRM.UserSelectorListView<%=ObjId%>.init();
    });

    ASC.CRM.UserSelectorListView<%=ObjId%> = (function($) {

        return {
            init: function() {
                if (SelectedUsers<%=ObjId%>.IDs.length == 0) {
                    jq("#cbxNotify<%=ObjId%>").removeAttr("checked");
                    jq("#notifyPanel<%=ObjId%>").hide();
                }
            },

            pushUserIntoList: function (uID, uName) {
                var alreadyExist = false;

                for (var i = 0; i < SelectedUsers<%=ObjId%>.IDs.length; i++)
                    if (SelectedUsers<%=ObjId%>.IDs[i] == uID) {
                        alreadyExist = true;
                        break;
                    }

                if (alreadyExist) return false;

                SelectedUsers<%=ObjId%>.IDs.push(uID);
                SelectedUsers<%=ObjId%>.Names.push(uName);

                var item = jq("<div></div>")
                    .attr("id", "selectedUser_" + uID + "<%=ObjId%>")
                    .addClass("selectedUser");

                var peopleImg = jq("<img>")
                    .attr("src", SelectedUsers<%=ObjId%>.PeopleImgSrc)
                    .css("margin", "0px 4px -2px 0px");

                var deleteImg = jq("<img>")
                    .attr("src", SelectedUsers<%=ObjId%>.DeleteImgSrc)
                    .css("margin", "0px 4px -2px 0px")
                    .css("display", "none")
                    .css("width", "12px")
                    .css("height", "12px")
                    .css("cursor", "pointer")
                    .attr("id", "deleteSelectedUserImg_" + uID + "<%=ObjId%>")
                    .attr("title", SelectedUsers<%=ObjId%>.DeleteImgTitle);

                item.append(peopleImg).append(deleteImg).append(Encoder.htmlEncode(uName));

                jq("#selectedUsers<%=ObjId%>").append(item);

                jq("#selectedUser_" + uID + "<%=ObjId%>").unbind("mouseover").bind("mouseover", function() {
                    ASC.CRM.UserSelectorListView<%=ObjId%>.selectedUser_mouseOver(jq(this));
                });

                jq("#selectedUser_" + uID + "<%=ObjId%>").unbind("mouseout").bind("mouseout", function() {
                    ASC.CRM.UserSelectorListView<%=ObjId%>.selectedUser_mouseOut(jq(this));
                });

                jq("#deleteSelectedUserImg_" + uID + "<%=ObjId%>").unbind("click").bind("click", function() {
                    ASC.CRM.UserSelectorListView<%=ObjId%>.selectedUser_deleteItem(jq(this));
                });

                jq("#cbxNotify<%=ObjId%>").attr("checked", true);
                jq("#notifyPanel<%=ObjId%>").show();
                advUserSelectorListView<%=ObjId%>.ClearFilter();
                jq("#selectedUsers<%=ObjId%>").parent().find("div[id=DepsAndUsersContainer]").hide();
            },

            selectedUser_mouseOver: function (obj) {
                var uID = jq(obj).attr("id").split("_")[1];
                jq("#selectedUser_" + uID + "<%=ObjId%>" + " img:first").hide();
                jq("#selectedUser_" + uID + "<%=ObjId%>" + " img:last").show();
            },

            selectedUser_mouseOut: function (obj) {
                var uID = jq(obj).attr("id").split("_")[1];
                jq("#selectedUser_" + uID + "<%=ObjId%>" + " img:first").show();
                jq("#selectedUser_" + uID + "<%=ObjId%>" + " img:last").hide();
            },

            selectedUser_deleteItem: function (obj) {
                var uID = jq(obj).attr("id").split("_")[1];
                jq("#selectedUser_" + uID + "<%=ObjId%>").remove();

                for (var i = 0; i < SelectedUsers<%=ObjId%>.IDs.length; i++) {
                    if (SelectedUsers<%=ObjId%>.IDs[i] == uID) {
                        SelectedUsers<%=ObjId%>.IDs.splice(i, 1);
                        SelectedUsers<%=ObjId%>.Names.splice(i, 1);
                        break;
                    }
                }

                if (SelectedUsers<%=ObjId%>.IDs.length == 0) {
                    jq("#cbxNotify<%=ObjId%>").removeAttr("checked");
                    jq("#notifyPanel<%=ObjId%>").hide();
                }
            }
        }
    })(jQuery);
</script>