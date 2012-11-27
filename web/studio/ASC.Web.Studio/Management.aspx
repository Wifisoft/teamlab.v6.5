<%@ Page MasterPageFile="~/Masters/StudioTemplate.master" Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="Management.aspx.cs" Inherits="ASC.Web.Studio.Management" Title="Untitled Page" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="ascwc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="StudioPageContent" runat="server">
    <ascwc:Container ID="_settingsContainer" runat="server">
        <Header>
        </Header>
        <Body>
            <asp:PlaceHolder runat="server" ID="_contentHolder"></asp:PlaceHolder>   
        </Body>
    </ascwc:Container>
</asp:Content>