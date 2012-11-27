<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupSelector.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.GroupSelector" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>


<div style="position: relative;">
    <%if (ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
      { %>
      
      <select id="grpselector_mgroupList_<%=_selectorID%>" class="comboBox addGroupLink" style="width: 230px;">
      </select>
      
          <% }
      else
      {%>
      
        <span id="groupSelectorBtn_<%=_selectorID%>" class="addGroupLink">
            <a class="baseLinkAction linkMedium"><%=LinkText.HtmlEncode()%></a>
            <img src="<%= WebImageSupplier.GetAbsoluteWebPath("sort_down_black.png") %>" align="absmiddle"/>
        </span>

        <div id="groupSelectorContainer_<%=_selectorID%>" class="borderBase tintLight groupSelectorContainer <%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context) ? "groupSelectorContainerMobile" : ""%>">
              
            <div class="clearFix filterBox">
                <input type="text" id="grpselector_filter_<%=_selectorID%>" class="textEdit" autocomplete="off"/>    
                <a id="grpselector_clearFilterBtn_<%=_selectorID%>" class="baseLinkAction" style="float:left;"><%=Resources.Resource.ClearFilterButton%></a>
            </div>
            
            <div id="grpselector_groupList_<%=_selectorID%>" class="grpselector_groupList"></div>
            
        </div>
    <% } %>
</div>
