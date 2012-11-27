<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyNavigation.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Company.CompanyNavigation" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>
<div> 
    <ascwc:SideContainer ID="EmployeeSiteNavigator" runat="Server">
        <div>
            <div style="padding: 5px 0px; overflow: hidden;">
                <asp:TreeView NodeStyle-CssClass="linkAction" NodeWrap="true"
                    NodeStyle-VerticalPadding="6px" ID="categoryTree" runat="server">
                </asp:TreeView>
            </div>
        </div>
    </ascwc:SideContainer>
</div>

