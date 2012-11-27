<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSelector.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserSelector" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<div id="usrdialog_<%=_selectorID%>" style="display:none;">
<ascwc:Container runat="server" ID="_container">
<Header>
    <%=HttpUtility.HtmlEncode(this.Title??"")%>
</Header>
<Body>


    <%if (isMobileVersion)
      {%>
    <div style="color: #1E7497; font-size: 16px; font-weight: bold; margin-bottom: 10px;"><%=HttpUtility.HtmlEncode(this.SelectedUserListTitle ?? "")%></div>
    <select id="selectmobile_<%=_selectorID%>" multiple="multiple" size="15"
        class="comboBox" style="width: 300px" onchange="<%=BehaviorID%>.ChangeMobileSelect();">
    </select>
    <%}
      else
      {%>

    <div class="clearFix studioUserSelector">
        <div class="leftBox">
            <div class="title"><%=HttpUtility.HtmlEncode(this.UserListTitle ?? "")%></div>
            <div id="usrdialog_leftBox_<%=_selectorID%>" class="content borderBase">            
            </div>
            <div class="borderBase" style="margin-top: 2px;">
				<div id="employeeFilterInputCloseImage" class="employeeFilterInputCloseImageGrey"
					style="float: right;"
					onclick="employeeFilterInputCloseImageClick();">&nbsp;</div>
				<input type="text" id="employeeFilterInput" class="employeeFilterInputGreyed"
					value="<%=Resources.Resource.QuickSearch%>"
					title="<%=Resources.Resource.QuickSearch%>"
					onkeyup="filterEmployees();"					
					onfocus="onEmployeeFilterInputFocus();"
					onblur="onEmployeeFilterInputFocusLost();" 
					style="border: solid 0px White; height: 17px;"/>
            </div>
        </div>
        <div class="centerBox">            
            <div>
                <a href="#" onclick="<%=BehaviorID%>.Select(); filterEmployees(); return false;">                    
                    <img style="border:none" src="<%=WebImageSupplier.GetAbsoluteWebPath("to_right.png")%>" alt="" />
                </a>
            </div>
            <div style="margin-top:20px;">
                <a href="#" onclick="<%=BehaviorID%>.Unselect(); filterEmployees(); return false;">
                    <img style="border:none" src="<%=WebImageSupplier.GetAbsoluteWebPath("to_left.png")%>" alt="" />
                </a>
            </div>
        </div>
        <div class="rightBox">
            <div class="title"><%=HttpUtility.HtmlEncode(this.SelectedUserListTitle ?? "")%> <span id="clear_usrdialog_rightBox" class="baseLinkAction" onclick="<%=BehaviorID%>.ClearSelection (); <%=BehaviorID%>.ClearUsrdialogRightBox();"><%=Resources.Resource.ClearButton%></span></div>
            <div id="usrdialog_rightBox_<%=_selectorID%>" class="content borderBase" style="height: 377px;">
            </div>
        </div>
    </div>

    <%} %>
    
    <div class="clearFix">
        <%=CustomBottomHtml%>
    </div>
    <div class="clearFix" style="margin-top:16px;">
        <a class="baseLinkButton" style="float:left; margin-right:8px;" href="#" onclick="<%=BehaviorID%>.ApplyAndCloseDialog(); return false;"><%=Resources.Resource.SaveButton%></a>
        <a class="grayLinkButton" style="float:left;" href="#" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;"><%=Resources.Resource.CancelButton%></a>        
    </div>    
</Body>
</ascwc:Container>
</div>