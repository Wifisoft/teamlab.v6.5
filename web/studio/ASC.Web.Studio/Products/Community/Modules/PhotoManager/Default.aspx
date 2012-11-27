<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.PhotoManager.Resources" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>

<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true" EnableViewState="false"
    CodeBehind="Default.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.Default"
    Title="Untitled Page" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<%@ Register Src="Controls/LastEvents.ascx" TagPrefix="ctrl" TagName="LastEvents" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">
    <link href="<%=ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/css/photomanagerstyle.css")%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

    	var EventsManager = new function() {
    		this.isEditing = false;
    		this.currentID = null;

    		this.SaveEvent = function() {

    			var id = this.currentID;
    			var name = jq('#event_name').val();
    			var description = jq('#event_desc').val();
    			var date;
    			try {
    				date = jq.datepick.parseDate(jq.datepick.dateFormat, jq('#<%=event_date.ClientID%>').val());
    				jq('#<%=event_date.ClientID%>').datepick('setDate', date);
    			}
    			catch (e) { jq('#<%=event_date.ClientID%>').datepick('setDate', new Date()); }

    			var date = jq('#<%=event_date.ClientID%>').val();

    			if (this.isEditing) {
    				if (id != null && name != '' && date != '') {
    					AjaxPro.onLoading = function(b) { if (b) { jq.blockUI(); } else { jq.unblockUI(); } };
    					Default.SaveEvent(id, name, description, date, this.callBackSave);
    				}
    			}
    		};

    		this.callBackSave = function(result) {
    			if (result.value != null) {
    				if (result.value != '') {
    					window.open('Default.aspx?<%=Request.QueryString%>', '_self');
    					//jq('#events_container').html(result.value);
    				}
    				EventsManager.HideForm();
    			}
    		};

    		this.EditEvent = function(id) {
    			this.isEditing = true;
    			this.currentID = id;

    			Default.GetInfoEvent(id, this.callBackInfo);
    		};

    		this.callBackInfo = function(result) {
    			if (result != null) {
    				var rs = result.value;
    				EventsManager.SetDataToForm(rs.rs1, rs.rs2, rs.rs3);
    				EventsManager.ShowForm();
    			}
    		};

    		this.SetDataToForm = function(name, description, date) {
    			jq('#event_name').val(name);
    			jq('#event_desc').val(description);

    			jq("#<%=event_date.ClientID%>").mask('<%= System.DateTimeExtension.DateMaskForJQuery %>');
    			jq('#<%=event_date.ClientID%>').datepick({ defaultDate: date, selectDefaultDate: true, showAnim: '', popupContainer: "#event_form" });
    			//            jq('#<%=event_date.ClientID%>').val(date);
    		};

    		this.ShowForm = function() {
    			try {

    				jq.blockUI({ message: jq("#event_form"),
    					css: {
    						left: '50%',
    						top: '50%',
    						opacity: '1',
    						border: 'none',
    						padding: '0px',
    						width: '340px',
    						height: '320px',
    						cursor: 'default',
    						textAlign: 'left',
    						'margin-left': '-170px',
    						'margin-top': '-150px',
    						'background-color': 'Transparent'
    					},

    					overlayCSS: {
    						backgroundColor: '#AAA',
    						cursor: 'default',
    						opacity: '0.3'
    					},
    					focusInput: false,
    					baseZ: 666,
    					//                    CloseDialogAction 
    					fadeIn: 0,
    					fadeOut: 0
    				});
    			}
    			catch (e) { };

    			PopupKeyUpActionProvider.ClearActions();
    			PopupKeyUpActionProvider.CloseDialogAction = 'EventsManager.HideForm();';
    		};

    		this.HideForm = function() {
    			jq('#<%=event_date.ClientID%>').datepick('destroy');
    		};

    		this.RemoveEvent = function(id) {
    			if (confirm('<%=PhotoManagerResource.ConfirmRemoveEventMessage%>')) {
    				AjaxPro.onLoading = function(b) { if (b) { jq.blockUI(); } else { jq.unblockUI(); } };
    				Default.RemoveEvent(id, this.callBackRemove);
    			}
    		};

    		this.callBackRemove = function(result) {
    			if (result.value != null && result.value != '') {
    				window.open('Default.aspx', '_self');
    			}
    		};
    	}
    
    EventsManager.isEditing = false;
    EventsManager.currentID = null;
    </script>
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <div id="event_form" style="display: none;">
        <ascw:container id="formContainer" runat="server" >
        <header><div ><%=PhotoManagerResource.EventTitle%></div></header>
        <body>
        <div >
        <div>
            <span>
                <%=PhotoManagerResource.NameTitle%></span><br />
            <input id="event_name" type="text" class="textEdit" maxlength="255" style="width: 300px; margin-top: 2px;" />
        </div>
        <div style="margin-top: 10px;">
            <span>
                <%=PhotoManagerResource.DateTitle%></span><br />
            <asp:TextBox ID="event_date" CssClass="textEditCalendar" Style="margin-top: 2px;" runat="server"></asp:TextBox>
        </div>
        <div style="margin-top: 10px;">
            <span>
                <%=PhotoManagerResource.DescriptionTitle%></span><br />
            <textarea id="event_desc" class="textEdit" style="width: 300px; margin-top: 2px;
                height: 120px;"></textarea>
        </div>
        <div style="margin-top: 20px;">
            <a class="baseLinkButton" href="javascript:EventsManager.SaveEvent();" style="width: 80px;">
                <%=PhotoManagerResource.SaveButton%></a><a class="baseLinkButton" href="javascript:PopupKeyUpActionProvider.CloseDialog();" style="margin-left:5px;width: 80px;">
                <%=PhotoManagerResource.CancelButton%></a>
        </div>
        </div>
        </body>
        </ascw:container>
    </div>
    <div >
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="PhotoManager_FirstColumn">
                    <ascw:container id="mainContainer" runat="server">
                    <header></header>
                    <body>
                        <div>
                            <div>
                                <asp:PlaceHolder runat="server" ID="_contentHolder"></asp:PlaceHolder>                                
                            </div>
                        </div>
                        <div style="padding: 5px;">
                            <asp:PlaceHolder ID="pageNavigatorHolder" runat="server"></asp:PlaceHolder>
                        </div>
                    </body>
                    </ascw:container>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
    <ctrl:ActionContainer ID="actions" runat="server" />
    <ascwc:SideRecentActivity id="sideRecentActivity" runat="server" />
    <ctrl:LastEvents ID="lastEvents" runat="server"></ctrl:LastEvents>
</asp:Content>
