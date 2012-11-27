<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Page Language="C#" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="TMDocs.aspx.cs" Inherits="ASC.Web.Projects.TMDocs" Title="Untitled Page" %>
<%@ MasterType  TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Register Assembly="ASC.Web.Files" Namespace="ASC.Web.Files" TagPrefix="ascf" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascwc" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="BTHeaderContent" runat="server">

    <style type="text/css">
        div.mainContainerClass div.containerHeaderBlock{display:none;}
    </style>
    <script type="text/javascript">
        jq(document).ready(function() {
            jq("div.mainContainerClass").children("div.containerHeaderBlock").hide()
        });
    </script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="BTPageContentWithoutCommonContainer" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
</asp:Content>
