<%@ Assembly Name="ASC.Web.Files" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocEditor.aspx.cs" Inherits="ASC.Web.Files.DocEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="icon" href="~/favicon.ico" type="image/x-icon" />
    <title></title>
    
    <asp:PlaceHolder ID="aspHeaderContent" runat="server"></asp:PlaceHolder>
    
    <style type="text/css">
        html
        {
            height: 100%;
            width: 100%;
        }        
        body
        {
            background: #fafbf6;
            font-weight: normal;
            margin: 0px;
            padding: 0px;
            font-size: 12px;
            color: #111111;
            font-family: Arial, Tahoma,sans-serif;
            text-decoration: none;
            height: 100%;
        }
        div
        {
            padding: 0px;
            margin: 0px;
        }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
        <asp:PlaceHolder ID="CommonContainerHolder" runat="server"></asp:PlaceHolder>
        <div id="iframeEditor"></div>
    </form>
</body>
</html>