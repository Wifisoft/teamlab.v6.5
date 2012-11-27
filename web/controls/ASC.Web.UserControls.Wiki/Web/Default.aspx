<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="~/WikiUC/ViewPage.ascx" TagName="ucViewPage" TagPrefix="wiki" %>
<%@ Register Src="~/WikiUC/ListPages.ascx" TagName="ucListPages" TagPrefix="wiki" %>
<%@ Register Src="~/WikiUC/EditPage.ascx" TagName="ucEditPage" TagPrefix="wiki" %>
<%@ Register Src="~/WikiUC/ViewFile.ascx" TagName="ucViewFile" TagPrefix="wiki" %>
<%@ Register Src="~/WikiUC/ListFiles.ascx" TagName="ucListFiles" TagPrefix="wiki" %>
<%@ Register Src="~/WikiUC/EditFile.ascx" TagName="ucEditFile" TagPrefix="wiki" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div style="float: left; height: 300px; widows: 150px;">
        <wiki:ucListPages runat="Server" ID="wikiListPages" />
        <wiki:ucListFiles runat="Server" ID="wikiListFiles" />
    </div>
    <div style="float: left;">
        <div style="padding:0 10px;">
            <asp:HyperLink ID="cmdAddNew" runat="Server" Text="Добавить страницу" /> &nbsp;
            <asp:HyperLink ID="cmdEdit" runat="Server" Text="Редактировать" /> &nbsp;
            <asp:HyperLink ID="cmdAddFile" runat="Server" Text="Добавить файл" /> &nbsp;
            <asp:HyperLink ID="cmdEditFile" runat="Server" Text="Редактировать файл" Visible="False" /> 
        </div>
        <wiki:ucViewPage runat="Server" ID="wikiViewPage" />
        <wiki:ucEditPage runat="Server" ID="wikiEditPage" Visible="false" />
        <wiki:ucViewFile runat="Server" ID="wikiViewFile" />
        <wiki:ucEditFile runat="Server" ID="wikiEditFile" Visible="false"/>
    </div>
    <div style="clear: both;">
    </div>
    </form>
</body>
</html>
