<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrivatePanel.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.PrivatePanel" %>

<div class="tintMedium" style="padding:15px;">

    <span class="headerBase"><%= Title.HtmlEncode()%></span>

    <p><%= Description.HtmlEncode()%></p>

    <div>
        <table class="border-panel" cellpadding="5" cellspacing="0">
            <tr>
                <td>
                    <input style="margin: 0" type="checkbox" id="isPrivate" <%=IsPrivateItem ? "checked='checked'" : "" %> onclick="changeIsPrivateCheckBox();">
                </td>
                <td style="padding-left:0">
                    <label for="isPrivate">
                        <%= CheckBoxLabel.HtmlEncode()%>
                    </label>
                </td>
            </tr>
        </table>
    </div>

    <div id="privateSettingsBlock" <%=IsPrivateItem ? "" : "style='display:none;'" %>>
        <br />
        <b><%= AccessListLable.HtmlEncode()%>:</b>
        <asp:PlaceHolder runat="server" ID="_phUserSelectorListView"></asp:PlaceHolder>
    </div>

</div>

<script type="text/javascript" language="javascript">
    function changeIsPrivateCheckBox()
    {
        if(jq("#isPrivate").is(":checked"))
        {
            jq("#privateSettingsBlock").show();
        }
        else
        {
            jq("#privateSettingsBlock").hide();
        }
    }
</script>