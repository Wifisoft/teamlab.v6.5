<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPortalActivity.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ConfirmPortalActivity" %>

<div class="tintMedium borderBase" style="border-right:none; border-left:none; border-bottom:none; margin:20px 0px; width: 600px; text-align:center; <%if (IsChangeDnsMode) { %> width: auto; <%}%>">
        <div class="headerBase" style="padding:10px 0px;"><%=_title%></div>        
</div>

<asp:PlaceHolder ID="_confirmContentHolder" runat="server">
    <div class="clearFix" style="margin-top:50px;">
        <a class="baseLinkButton" onclick="document.forms[0].submit(); return false;" href="#"><%=_buttonTitle%></a>
        <a class="grayLinkButton" href="./" style="margin-left:10px;"><%=Resources.Resource.CancelButton %></a>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_messageHolder" runat="server">
    <script type="text/javascript" language="javascript">
        setTimeout("window.open('./','_self');",10000);
    </script>        
    <div style="margin-top:50px; width:400px;">
        <%=_successMessage%>
    </div>
</asp:PlaceHolder>