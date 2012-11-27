<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StepContainer.ascx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.StepContainer" %>
<div class="step<%=StepNumber %> step">
    <div class="header clearFix" id="header" style="display:<%=!ShowHeader?"none":"block"%>">
        <div class="titleNumber">
            <%=String.Format("{0} {1}.",Resources.Resource.WizartStep, StepNumber) %></div>
        <div class="title">
            <%= Title %></div>
              <div class="skip" style="display:none">
            <a href="javascript:void(0);" onclick="<%= this.SkipButtonEvent %>"><%= Resources.Resource.ContainerSkipAll %></a></div>
    </div>
    <div class="stepData" style="display:<%= StepNumber==0?"block":"none"%>;">
    <div id="wizard_OperationInfo<%=StepNumber %>"></div>
        <div class="stepBody">
            <asp:PlaceHolder ID="content1" runat="server"></asp:PlaceHolder>
        </div>
        <div class="footer clearFix" style="display:<%= HideFooter?"none":"block"%>;">
            <div style="height: 20px; float: left;">
                <div class="btnBox">
                    <a class="baseLinkButton" id="saveSettingsBtn" href="#" onclick="<%= this.SaveButtonEvent %>">
                        <%= SaveButtonText %></a>
                </div>
                <div class="btnBox" id="btnSkip" runat="server">
                    <a class="grayLinkButton" id="cancelSettingsBtn" href="#" onclick="<%= this.CancelButtonEvent %>"><%= Resources.Resource.ContainerSkip %></a>
                </div>
            </div>
        </div>
    </div>
</div>
