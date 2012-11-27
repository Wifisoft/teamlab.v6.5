<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WhatsNewBody.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.WhatsNewBody" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwsc" %>

<%@ Import Namespace="ASC.Web.Core.Users.Activity" %>

<input type='hidden' id='hiddenProductId' value='<%=ProductId %>' />
<div class="whatsNewFilter">
	<%--modules--%>
	<div>
		<%--<span class='whatsNewFilterTitle'><%=Resources.UserControlsCommonResource.ListModules%></span>--%>
		<div class='clearFix whatsNewFilterBodyModule'>
			<%=InitModuleList() %>
			<div class='whatsNewFilterModuleAll'>
				<a class='linkAction' href='#' onclick='javascript:WhatsNew.MadeAnchor(null);return false;'><%=Resources.UserControlsCommonResource.SelectAllModules %></a>
				|
				<a class='linkAction' href='#' onclick='javascript:WhatsNew.MadeAnchor(new Array());return false;'><%=Resources.UserControlsCommonResource.DeselectAllModules%></a>
			</div>
		</div>
	</div>
	
	
	<%--User and department filters--%>
	<div class="clearFix" style="margin: 0px 0 4px -5px;">
		<div style="float: left;">
			<ascwsc:UserSelector runat="server" DepartmentsOnChangeFunction="WhatsNew.SelectGroupOption();" UsersOnChangeFunction="WhatsNew.SelectUserOption();" ElementCssClass="whatsNewCombobox"></ascwsc:UserSelector>
		</div>
		<div style="float: left;">
			<select class='comboBox whatsNewFilterTypeSelect' id='selectTypeWhatsNew' onchange='WhatsNew.SelectTypeOption()' style="width: 170px;">
					<option id='optionTypeWhatsNew_0' value='0' selected='selected'><%=Resources.UserControlsCommonResource.ActionType%></option>
					<option id='optionTypeWhatsNew_1' value='1'><%=Resources.UserControlsCommonResource.ActiionTypeContent%></option>
					<option id='optionTypeWhatsNew_2' value='2'><%=Resources.UserControlsCommonResource.ActionTypeActivity%></option>
				</select>
		</div>
		
		<%--activity type--%>
		<%--<div style="margin-left:400px;">--%>
			<%--<span class='whatsNewFilterTitle'><%=Resources.UserControlsCommonResource.ActionType%></span>--%>
			<%--<div class='whatsNewFilterBody'>--%>
				
			<%--</div>--%>
		<%--</div>--%>
	</div>
	
	
	<%--date period--%>
	<div class="clearFix">
		<div style="float:left;">
			<%--<span class='whatsNewFilterTitle'><%=Resources.UserControlsCommonResource.TimePeriod%></span>--%>
			<div class='clearFix whatsNewFilterBodyDate'>
				<select class='comboBox whatsNewFilterDateSelect' id='selectDateWhatsNew' onchange='WhatsNew.SelectDateOption()'>
					<option id='optionDateWhatsNew_0' value='0'><%=Resources.UserControlsCommonResource.Today%></option>
					<option id='optionDateWhatsNew_1' value='1'><%=Resources.UserControlsCommonResource.Yesterday%></option>
					<option id='optionDateWhatsNew_2' value='2' selected='selected'><%=Resources.UserControlsCommonResource.LastWeek%></option>
					<option id='optionDateWhatsNew_3' value='3'><%=Resources.UserControlsCommonResource.LastMonth%></option>
					<option id='optionDateWhatsNew_4' value='4'><%=Resources.UserControlsCommonResource.LastYear%></option>
					<option id='optionDateWhatsNew_5' value='5'><%=Resources.UserControlsCommonResource.Other%></option>
				</select>
				<div class="whatsNewDateIntervalLabel"><%=Resources.UserControlsCommonResource.From%></div>
				<input type='text' id='txtFromDateWhatsNew' class='textEditCalendar whatsNewFilterDateInput'/>
				<div class="whatsNewDateIntervalLabel" style="width: 35px;"><%=Resources.UserControlsCommonResource.To%></div>
				<input type='text' id='txtToDateWhatsNew' class='textEditCalendar whatsNewFilterDateInput'/>
				<script type="text/javascript">
					jq(function(){
						jq(".textEditCalendar").mask('<%= System.DateTimeExtension.DateMaskForJQuery %>');
					});
				</script>
			</div>
		</div>
		
	</div>
	
	
	
		
</div>

<div id='divWhatNewContainer' class='clearFix'></div>
<div id="divForNavigation" style="padding: 30px 0 5px 0"></div>

<p style="display:none;">
	<textarea id="jTemplateTableWhatsNew" cols="1" rows="1">
		{#for i = 0 to $T.table.length-1}
			{#if $T.i==0 || $T.table[$T.i].AgoSentence != $T.table[$T.i-1].AgoSentence}
			<div class='headerBase {#if $T.i==0}whatsNewItemAgoFirst{#else}whatsNewItemAgo{#/if}'>
				{$T.table[$T.i].AgoSentence}
			</div>
			{#/if}
			<div class="itemUserActivity clearFix {#cycle values=['even','odd']}">
				<img alt='{$T.table[$T.i].ModuleName}' title='{$T.table[$T.i].ModuleName}' src="{$T.table[$T.i].ModuleIconUrl}" />
				<div class='whatsNewItem'>
					<a href='{$T.table[$T.i].URL}' title="{$T.table[$T.i].Title}">{$T.table[$T.i].Title}</a>
					<span class='actionText'>{$T.table[$T.i].ActionText}</span>
					<span class="textMediumDescribe" style="white-space:nowrap;">{$T.table[$T.i].Date}</span>
					<span class='actionText'>{$T.table[$T.i].UserProfileLink}</span>
				</div>
			</div>
		{#/for}
	</textarea>
</p>