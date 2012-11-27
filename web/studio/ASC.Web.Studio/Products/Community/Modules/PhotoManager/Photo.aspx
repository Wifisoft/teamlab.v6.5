<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.PhotoManager.Resources" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true"
    CodeBehind="Photo.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.Photo" Title="Untitled Page" %>

<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<%@ Register Src="Controls/LastEvents.ascx" TagPrefix="ctrl" TagName="LastEvents" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">
    <link href="<%=ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/css/photomanagerstyle.css")%>" rel="stylesheet" type="text/css" />
    <script type='text/javascript'>
		var PhotoManager = new function()
		{
			this.ShowRemoveAlbumContainer = function()
			{        
				try{
					jq.blockUI({message:jq("#confirm_form"),
							css: { 
								left: '50%',
								top: '50%',
								opacity: '1',
								border: 'none',
								padding: '0px',
								width: '340px',
								height: '390px',
								cursor: 'default',
								textAlign : 'left',
								'margin-left': '-140px',
								'margin-top': '-90px',
								'background-color':'Transparent'
							},
		                    
							overlayCSS: { 
								backgroundColor: '#AAA',
								cursor: 'default',
								opacity: '0.3' 
							},
							focusInput : false,
							baseZ : 666,
		                    
							fadeIn: 0,
							fadeOut: 0
						});     
				}
				catch(e){};
			};
		}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="PhotoManager_FirstColumn">
                    <ascw:container id="mainContainer" runat="server">
                        <header>
                        </header>
                        <body>
                            <div>
                                <%--<asp:HiddenField ID="hdnMode" runat="server" />
                                <asp:HiddenField ID="hdnViewMode" runat="server" />--%>
                                <div style="padding-top: 10px;">
                                    <asp:Panel ID="pnlCurrentAlbum" runat="server">
                                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                            <tr valign="top">
                                                <td style="width: 276px;">
                                                    <table cellpadding="0" cellspacing="0" border="0">
                                                        <tr>
                                                            <td style="width: 276px; background-color: #F5F5F5; text-align: center; vertical-align: middle;">
                                                                <asp:Literal ID="ltrAlbumFace" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <asp:Literal ID="ltrAlbumInfo" runat="server"></asp:Literal>
                                                    <asp:Panel ID="pnlEditPhoto" runat="server" Visible="false">
                                                        <div style="padding: 10px;">                                                            
                                                            <asp:LinkButton CssClass="linkDescribe" ID="lbtnEdit" runat="server" 
																OnClick="lbtnEdit_Click"><%=PhotoManagerResource.EditAlbumButton%></asp:LinkButton><span class="textMediumDescribe splitter">|</span><a 
																href="#" class='linkAction <%= ASC.Web.Studio.Core.SetupInfo.WorkMode == ASC.Web.Studio.Core.WorkMode.Promo ? "promoAction" : "" %>' 
																onclick='PhotoManager.ShowRemoveAlbumContainer();return false;' ><%=PhotoManagerResource.RemoveAlbumButton%></a>
															<div id='confirm_form' style="display:none">
																<ascw:container id='confirmContainer' runat='server'>
																	<header>
																		<%=PhotoManagerResource.RemoveAlbumButton%>
																	</header>
																	<body>
																		<span><%=PhotoManagerResource.ConfirmRemoveAlbumMessage %></span>
																		<div class="clearFix" style="margin-top:16px;">
																			<asp:LinkButton ID="lbtnRemove" runat="server" CssClass="baseLinkButton" style="float:left;" OnClick="lbtnRemove_Click"><%=PhotoManagerResource.RemoveButton %></asp:LinkButton>
																			<a class="grayLinkButton" style="float:left; margin-left:8px;" href="#" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;"><%=PhotoManagerResource.CancelButton %></a>
																		</div>
																	</body>
																</ascw:container>
															</div>
                                                        </div>
                                                    </asp:Panel>
                                                    <div style="padding: 5px 10px;">
                                                        <asp:Literal ID="ltrLinkAllPhoto" runat="server"></asp:Literal>
                                                    </div>
                                                </td>
                                                <td>
                                                    <div style="margin-left: 20px;">
                                                        <asp:Literal ID="ltrPhoto" runat="server"></asp:Literal></div>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlUserAlbums" runat="server">
                                        <asp:PlaceHolder ID="_contentHolder" runat="server"></asp:PlaceHolder>
                                    </asp:Panel>
                                    <div>
                                        <asp:PlaceHolder ID="pageNavigatorHolder" runat="server"></asp:PlaceHolder>
                                    </div>
                                </div>
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
    <asp:Literal ID="ltrNotify" runat="server"></asp:Literal>
    <ascwc:SideContainer ID="albumsContainer" runat="server">
        <asp:Literal ID="ltrAlbums" runat="server"></asp:Literal>
    </ascwc:SideContainer>
    <ctrl:LastEvents ID="lastEvents" runat="server">
    </ctrl:LastEvents>
    
</asp:Content>
