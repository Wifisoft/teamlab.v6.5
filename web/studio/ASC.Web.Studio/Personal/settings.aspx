<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/StudioTemplate.master" AutoEventWireup="true" CodeBehind="settings.aspx.cs" Inherits="ASC.Web.Studio.Personal.settings" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="StudioPageContent" runat="server">
    <ascwc:Container ID="_settingsContainer" runat="server">
        <Header>
        </Header>
        <Body>         
             <%--timezone & language--%>
            <div class="headerBase borderBase" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px;
                    border-top: none; border-right: none; border-left: none;">
                    <%=Resources.Resource.StudioTimeLanguageSettings%>
             </div>
             <div id="studio_lngTimeSettingsInfo">            
             </div>
             
             <div id="studio_lngTimeSettingsBox" style="padding:0px 20px 15px 20px;"> 
                <asp:PlaceHolder ID="_timelngHolder" runat="server"></asp:PlaceHolder>
                
                <div class="clearFix" style="margin-top: 20px;">
                        <a class="baseLinkButton<%=(SetupInfo.WorkMode == WorkMode.Promo?" promoAction":"") %>" style="float: left;" onclick="StudioManagement.SaveLanguageTimeSettings();"
                            href="javascript:void(0);">
                            <%=Resources.Resource.SaveButton %></a>
                    </div>
             </div> 
             
             <%--themes--%>
             <asp:PlaceHolder runat="server" ID="_themesHolder"></asp:PlaceHolder>   
           
          
         
        </Body>
    </ascwc:Container>
</asp:Content>