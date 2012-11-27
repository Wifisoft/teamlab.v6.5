<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThumbnailEditor.ascx.cs" 
		Inherits="ASC.Web.Studio.UserControls.Users.ThumbnailEditor" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<div id="usrdialog_<%=_selectorID%>" style="display:none;">
<ascwc:Container runat="server" ID="_container">
<Header>
    <%=HttpUtility.HtmlEncode(this.Title??"")%>
</Header>
<Body>
    <div class="clearFix">
		<table cellpadding="0" cellspacing="0" width="100%">
			<tr>
				<td valign="top">
					<div class="thumbnailMainImg">
						<img id="mainimg_<%=_selectorID%>" src="<%=MainImgUrl%>" alt="" />
					</div>
				</td>
				<td style="width:15px;"></td>
				<td valign="top">
					<span class="thumbnailCaption textBigDescribe"><%=Description%></span>
					<div>
						<asp:PlaceHolder runat="server" ID="placeThumbnails"></asp:PlaceHolder>
					</div>
				</td>
			</tr>
        </table>
    </div>
    <div class="clearFix" style="margin-top:16px;">
        <a class="baseLinkButton" style="float:left; margin-right:8px;" href="#" onclick="<%=BehaviorID%>.ApplyAndCloseDialog(); return false;"><%=Resources.Resource.SaveButton%></a>
        <a class="grayLinkButton" style="float:left;" href="#" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;"><%=Resources.Resource.CancelButton%></a>        
    </div>
    <input type="hidden" id="UserIDHiddenInput" value="<%=UserID %>" />
</Body>
</ascwc:Container>
</div>