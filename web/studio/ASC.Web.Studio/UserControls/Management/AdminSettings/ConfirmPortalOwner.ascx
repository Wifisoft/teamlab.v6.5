<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPortalOwner.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ConfirmPortalOwner" %> 

<div class="tintMedium borderBase confirmTitle">    
    <div class="headerBase"><%=String.Format(Resources.Resource.ConfirmOwnerPortalTitle, _newOwnerName)%></div>
</div>

<asp:PlaceHolder ID="_confirmContentHolder" runat="server">
    <div class="clearFix" style="margin-top:50px;">
        <a class="baseLinkButton" onclick="document.forms[0].submit(); return false;" href="#"><%=Resources.Resource.SaveButton%></a>
        <a class="grayLinkButton" href="./" style="margin-left:10px;"><%=Resources.Resource.CancelButton %></a>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_messageHolder" runat="server">
    <script type="text/javascript" language="javascript">
        setTimeout("window.open('./','_self');",10000);
    </script>        
    <div style="margin-top:50px; width:400px;">
        <%=string.Format(Resources.Resource.ConfirmOwnerPortalSuccessMessage, "<br/>", "<a href=\"" + ASC.Web.Studio.Utility.CommonLinkUtility.GetDefault() + "\">", "</a>")%>
    </div>
</asp:PlaceHolder>