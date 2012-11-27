<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactSelector.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.ContactSelector" %>
<%@ Register Src="ContactSelectorRow.ascx" TagPrefix="ctrl" TagName="ContactSelectorRow" %>

<div id="selector_<%=jsObjName%>">
    <div style="margin-bottom:5px;">
        <% if (SelectedContacts != null && SelectedContacts.Count > 0 && !ShowOnlySelectorContent) %>
        <% { %>
        <asp:Repeater runat="server" ID="rpt" OnItemDataBound="rpt_OnItemDataBound">
            <ItemTemplate>
                <ctrl:ContactSelectorRow runat="server" ID="_row"/>
            </ItemTemplate>
        </asp:Repeater>
        <% } %>
        <% else %>
        <% { %>
        <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
        <% } %>
    </div>

    <input id="entityType_<%=jsObjName%>" type="hidden" value="<%=(int)EntityType%>" />
    <input id="entityID_<%=jsObjName%>" type="hidden" value="<%=EntityID%>" />
</div>