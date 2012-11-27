<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentEdit.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Company.DepartmentEdit" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascws" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascws" %>   
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>

<ascws:TreeViewProScriptManager ID="treeViewProScriptManager" runat="server"></ascws:TreeViewProScriptManager>
<asp:PlaceHolder runat="server" ID="_userSelectorHolder"></asp:PlaceHolder>

<div class="headerBase">
    <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("DepartmentMaster").HtmlEncode()%>
</div>

<div class="clearFix" style="margin-bottom:30px; margin-top:10px;"> 
    <div style="padding: 10px; float:left; height: 100%;" class="tintMedium borderBase">
        <ascws:EmployeeUserCard ID="ucMasterUserCard" runat="server">
        </ascws:EmployeeUserCard>
    </div>
    
    <%if (CanEditDepartment()){%>
    <div style="float:right; text-align:right;  padding-top:95px;">
         <%--department head--%>          
   
     <a href="javascript:StudioSimpleUserSelector.OpenDialog('dep_manager');" class="linkAction<%=(SetupInfo.WorkMode==WorkMode.Promo?" promoAction":"")%>"
                                    style="margin-top: 102px;"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("DepEditMaster").HtmlEncode()%></a>
                                    
    <span class="splitter">|</span>   
   <%--department name--%>
    <a href="javascript:void(0);" style="" class="linkAction<%=(SetupInfo.WorkMode==WorkMode.Promo?" promoAction":"")%>" onclick="StudioConfirm.OpenDialog('depname')">
        <%=Resources.Resource.DepNameEditCaption%></a>
    
    </div>
    <%}%>
</div>

<div class="headerBase" style="padding-bottom: 10px;">
    <span style="padding-right: 15px;">
        <%=Resources.Resource.DepartmentComposition%>: <%=GetContainGroupCaption()%></span>
</div>
<asp:Panel ID="pEmployeesList" runat="Server">
    <asp:Repeater runat="Server" ID="rptEmployeesList">
        <ItemTemplate>        
            <div class="borderBase <%#(Container.ItemIndex % 2 == 0)?" tintMedium ":""%> clearFix" style="padding:10px 0px; border-bottom:none; border-right:none; border-left:none;overflow:hidden;">
                <table cellpadding="0" cellspacing="0" style="width:100%;">
                    <tr valign="top">
                        <td style="width:40px; padding:0px 10px;">     
                            <a href="<%#GetEmployeeUrl((Container.DataItem as UserInfo)) %>">
                                <img class="userMiniPhoto" src="<%#UserPhotoManager.GetSmallPhotoURL((Container.DataItem as UserInfo).ID)%>" />
                            </a>
                        </td>                 
                        <td>
                            <div style="margin-bottom:5px;">
                                <a class="linkHeaderLightSmall" href="<%#GetEmployeeUrl((Container.DataItem as UserInfo)) %>"><%#GetEmployeeName((Container.DataItem as UserInfo)) %></a>
                            </div>
                            <%#string.IsNullOrEmpty((Container.DataItem as UserInfo).Title) ? "&nbsp;" : (Container.DataItem as UserInfo).Title.HtmlEncode()%>
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>        
        <FooterTemplate>
            <asp:PlaceHolder runat="Server" Visible='<%# CanEditDepartment()%>'>
                <div class="borderBase" style="padding-top: 5px; padding-bottom: 5px;  border-bottom:none; border-right:none; border-left:none;">
                    &nbsp;
                </div>
            </asp:PlaceHolder>
        </FooterTemplate>
    </asp:Repeater>
</asp:Panel>

<%if (CanEditDepartment())
{%>
      
<div id="studio_depEditAddUserInfo"></div>
<div class="clearFix" style="margin-bottom:15px;">     
     <%--add employee--%>    
     <a href="javascript:StudioManagement.ShowEmployeeSelectorDialog();" style="float:left;" class="baseLinkButton<%=(SetupInfo.WorkMode==WorkMode.Promo?" promoAction":"")%>">
        <%=Resources.Resource.DepEditComposition%></a>
</div>
<input id="studio_depEditDepID" type="hidden" value="<%=DepId%>"  />
<%}%>
