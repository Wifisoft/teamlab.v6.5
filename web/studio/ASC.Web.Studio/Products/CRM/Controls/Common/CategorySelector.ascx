<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategorySelector.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.CategorySelector" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>

<div id="<%=jsObjName%>">

    <% if (!MobileVer) { %>
    <div style="width:<%= MaxWidth %>px;" class="categorySelector-selector-container clearFix" id="<%= jsObjName %>_selectorContainer" onclick="javascript:<%= jsObjName %>.showSelectorContent();">
        <input type="text" value="<%= SelectedCategory.Title %>" style="width:<%= MaxWidth - 25 %>px; height:16px; border:none; padding:2px; float: left;" class="textEdit" id="<%= jsObjName %>_categoryTitle" readonly="readonly">
        <div class="categorySelector-selector">
            <img src='<%= WebImageSupplier.GetAbsoluteWebPath("expand.gif", ProductEntryPoint.ID) %>' style="height:14px; width:14px; margin-left:2px; margin-top:2px;">
        </div>
        <input type="hidden" id="<%= jsObjName %>_categoryID" value="<%= SelectedCategory.ID %>" />
    </div>
    <div class="categorySelector-categoriesContainer" id="<%= jsObjName %>_categoriesContainer">
        <div class="categorySelector-categories">
    <% } %>
    <% else {%>
    <select id="<%= jsObjName %>_select" style="width:<%= MaxWidth %>px;" onchange="javascript:<%= jsObjName %>.changeContact(jq(this).find('option:selected'));">
    <% } %>
            <asp:Repeater ID="itemRepeater" runat="server">
                <ItemTemplate>
                    <% if (!MobileVer) { %>
                    <div onclick="javascript:<%= jsObjName %>.changeContact(this);" class="categorySelector-category" id="<%=jsObjName%>_category_<%# (Container.DataItem as ASC.CRM.Core.Entities.ListItem).ID%>">
                        <img src="<%# WebImageSupplier.GetAbsoluteWebPath((Container.DataItem as ASC.CRM.Core.Entities.ListItem).AdditionalParams, ProductEntryPoint.ID) %>" style="float:left;padding:4px;">
                        <div style="padding:15px 0 0 40px;"><%# (Container.DataItem as ASC.CRM.Core.Entities.ListItem).Title%></div>
                    </div>
                    <% } %>
                    <% else {%>
                    <option id="<%=jsObjName%>_category_<%# (Container.DataItem as ASC.CRM.Core.Entities.ListItem).ID%>"
                    value="<%# (Container.DataItem as ASC.CRM.Core.Entities.ListItem).ID%>">
                        <%# (Container.DataItem as ASC.CRM.Core.Entities.ListItem).Title%>
                    </option>
                    <% } %>
                </ItemTemplate>
            </asp:Repeater>
            
    <% if (!MobileVer) { %>
        </div>
    </div>
    <% } %>
    <% else {%>
    </select>
    <% } %>
    
    <script>
        var <%=jsObjName%> = new ASC.CRM.CategorySelector.CategorySelectorPrototype('<%=jsObjName%>');
        <% if (!MobileVer) { %>
        jq(document).click(function(event)
        {
            var switcher = "#" + jq('div.categorySelector-selector-container', <%= jsObjName %>.Me()).attr("id");
            var dropdown = "#" + jq('div.categorySelector-categoriesContainer', <%= jsObjName %>.Me()).attr("id");
            jq.dropdownToggle().registerAutoHide(event, switcher, dropdown); 
        });
        <% } %>
    </script>
    
</div>