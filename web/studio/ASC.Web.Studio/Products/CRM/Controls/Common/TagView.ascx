<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.TagView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div id="tagContainer">
    <a id="addNewTag" class="baseLinkAction linkMedium" title="<%= CRMCommonResource.AddNewTag %>">
        <%= CRMCommonResource.AddNewTag%>
    </a>
    <div style="display:inline;">
        <% foreach (var tag in  Tags) %>
        <%{%>
            <span class="tag_item">
                <span class="tag_title"><%= tag.HtmlEncode()%></span>
                <a class="delete_tag" alt="<%= CRMCommonResource.DeleteTag %>"
                        title="<%= CRMCommonResource.DeleteTag %>" onclick="ASC.CRM.TagView.deleteTag(jq(this).parent())"></a>
            </span>
        <%}%>
    </div>
    
    <img class="adding_tag_loading" alt="<%= CRMSettingResource.AddTagInProgressing %>"
            title="<%= CRMSettingResource.AddTagInProgressing %>"
            src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID) %>" />

    
    <div id="addTagDialog" class="dropDownDialog">
        <div class="dropDownCornerLeft"></div>
        <div class="dropDownContent">
            <% foreach (var oneTag in AllTags) %>
            <%{%>
                <a class="dropDownItem" onclick="ASC.CRM.TagView.addExistingTag(this);"><%= oneTag.HtmlEncode()%></a>
             <%}%>
        </div>
        
        <div class="h_line">&nbsp;</div>
        <div style="margin-bottom:5px;"><%= CRMCommonResource.CreateNewTag%>:</div>
        <input type="text" class="textEdit" maxlength="50"/>

        <a id="addThisTag" class="baseLinkButton" title="<%= CRMCommonResource.OK %>" onclick="ASC.CRM.TagView.addNewTag();">
            <%= CRMCommonResource.OK %>
        </a>
    </div> 
</div>              
 
                   
<script id="taqTmpl" type="text/x-jquery-tmpl">
    <span class="tag_item">
        <span class="tag_title">${tagText}</span>
        <a class="delete_tag" alt="<%= CRMCommonResource.DeleteTag %>" title="<%= CRMCommonResource.DeleteTag %>"
            onclick="ASC.CRM.TagView.deleteTag(jq(this).parent())"></a>
     </span>
</script>

<script id="tagInAllTagsTmpl" type="text/x-jquery-tmpl">
    <a class="dropDownItem" onclick="ASC.CRM.TagView.addExistingTag(this);">${tagText}</a>
</script>


<script type="text/javascript" language="javascript">
    jq(document).ready(function() {
       ASC.CRM.TagView.init(<%= (Int32)TargetEntityType %>);
    });
</script>

               