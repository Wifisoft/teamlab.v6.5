<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Customization.ascx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.Customization" %>
<div>
<div id="wizard_cusmotizing"></div>
<div class="clearFix">
<div style="float:left">

<div class="title">
        <%= Resources.Resource.WizardGenTemplates%></div>
    <asp:PlaceHolder ID="_namingPeopleHolder" runat="server"></asp:PlaceHolder>

</div>
<div style="float:right;margin-left:70px;">
<div class="title">
        <%= Resources.Resource.WizardGenTimeLang %></div>
    <asp:PlaceHolder ID="_dateandtimeHolder" runat="server"></asp:PlaceHolder> 
    <div class="title secondTitle">
        <%= Resources.Resource.WizardGenScheme %></div>
    <asp:PlaceHolder ID="_schemaHolder" runat="server"></asp:PlaceHolder>

</div>
</div>
    
   
</div>
