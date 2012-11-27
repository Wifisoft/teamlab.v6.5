<%@ Page Language="C#" MasterPageFile="~/Masters/StudioTemplate.master" AutoEventWireup="true" CodeBehind="confirm.aspx.cs" Inherits="ASC.Web.Studio.confirm" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StudioPageContent" runat="server">
     <asp:PlaceHolder runat="server" ID="_contentWithControl">
    
        <div align="center">
        
        <div class="headerLightMedium clearFix" style="margin-top:40px;">
          <a href="auth.aspx"><img class="logo" src="<%=_tenantInfoSettings.GetAbsoluteCompanyLogoPath()%>" border="0" alt="" /></a>
          <div style="padding:20px 0px;"><%=HttpUtility.HtmlEncode(ASC.Core.CoreContext.TenantManager.GetCurrentTenant().Name)%></div>
        </div> 
        
        
        <%if(!String.IsNullOrEmpty(ErrorMessage)){%>
        <div style="text-align:left; width: 400px; margin:10px 0px 20px 0px" id="studio_confirmMessage">
            <div class="errorBox"> <%=ErrorMessage%></div>
        </div>
        <%} %>

        
        <asp:PlaceHolder runat="server" ID="_confirmHolder"></asp:PlaceHolder>
        
        </div>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder runat="server" ID="_confirmHolder2"></asp:PlaceHolder>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StudioSidePanel" runat="server">
</asp:Content>
