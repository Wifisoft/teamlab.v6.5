<%@ Page Language="C#" MasterPageFile="~/Masters/StudioTemplate.master" AutoEventWireup="true" CodeBehind="My.aspx.cs" Inherits="ASC.Web.Studio.MyStaff" Title="Untitled Page" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="ascwc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StudioPageContent" runat="server">
    <ascwc:Container ID="_myStaffContainer" runat="server">
        <Header>
        </Header>
        <Body>
            <asp:PlaceHolder runat="server" ID="_contentHolder"></asp:PlaceHolder>
        </Body>
    </ascwc:Container>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StudioSidePanel" runat="server">
	<asp:PlaceHolder ID="MyStaffSideHolder" runat="server"></asp:PlaceHolder>
</asp:Content>
