<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" MasterPageFile="~/Masters/StudioTemplate.master"
    CodeBehind="Wizard.aspx.cs" Inherits="ASC.Web.Studio.Wizard" %>
<%@ Import Namespace="ASC.Web.Studio" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<asp:Content ID="WizardPageContent" ContentPlaceHolderID="StudioPageContent" runat="server">
    <div class="wizardTitle">
    <%= Resources.Resource.WelcomeTitle %>
    </div>
    <div class="wizardDesc"><%= Resources.Resource.WelcomeDescription %></div>
    <div class="wizardHelper"><%= Resources.Resource.WelcomeHelper %></div>
    <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
    <script type="text/javascript">
        jq(document).ready(function() {
        ASC.Controls.FirstTimeView.CancelConfirmMessage = "<%= Resources.Resource.WizardCancelConfirmMessage %>";
        });
    </script>
</asp:Content>