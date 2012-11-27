<%@ Page Language="C#" MasterPageFile="~/Masters/StudioTemplate.master" AutoEventWireup="true"
    EnableViewState="false" CodeBehind="Auth.aspx.cs" Inherits="ASC.Web.Studio.Auth"
    Title="Untitled Page" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Register TagPrefix="ucc" Namespace="ASC.Web.Studio.UserControls.Users.UserProfile"
    Assembly="ASC.Web.Studio" %>
<asp:Content ID="pageContent" ContentPlaceHolderID="StudioPageContent" runat="server">
    <input type="hidden" name="authtype" id="studio_authType" />
    <asp:PlaceHolder ID="pwdReminderHolder" runat="server" />
    <div id="GreetingBlock" class="authForm">
        <%--header and logo--%>
        <div class="header headerLightBig clearFix">
            <img class="logo" src="<%=_tenantInfoSettings.GetAbsoluteCompanyLogoPath()%>" alt="" />
            <h1 class="title">
                <%=HttpUtility.HtmlEncode(ASC.Core.CoreContext.TenantManager.GetCurrentTenant().Name)%></h1>
        </div>
        <div id="authForm">
            <div id="authMessage">
                <%=_loginMessage%></div>
            <%--login by email email--%>
            <div id="_AuthByEmail" class="login" runat="server">
                <div class="title">
                    <%=Resources.Resource.EmailForReminder%>:</div>
                <input maxlength="64" class="pwdLoginTextbox" type="text" id="login" name="login"
                    <%=(String.IsNullOrEmpty(_login)?"":("value=\""+HttpUtility.HtmlEncode(_login)+"\""))%> />
            </div>
            <%--password--%>
            <div class="password">
                <div class="title">
                    <%=Resources.Resource.Password%>:</div>
                <input maxlength="64" class="pwdLoginTextbox" type="password" id="pwd" name="pwd" />
            </div>
            <div class="subtext">
                <span class="pseudolink" onclick="AuthManager.ShowPwdReminderDialog()">
                    <%=Resources.Resource.ForgotPassword%></span>
            </div>
            <%--buttons--%>
            <div class="submenu clearFix">
                <div class="loginBlock clearFix">
                    <div style="float: left;">
                        <a class="highLinkButton signIn" id="loginButton" href="#" onclick="AuthManager.Login(); return false;">
                            <%=Resources.Resource.LoginButton%></a>
                    </div>
                    <div id="social" class="right">
                        <asp:Literal runat="server" ID="associateAccount" Visible="false"></asp:Literal>
                        <asp:PlaceHolder ID="signInPlaceholder" runat="server" />
                    </div>
                </div>
               <asp:PlaceHolder ID="_communitations" runat="server"></asp:PlaceHolder>
            </div>
        </div>
    </div>
    
</asp:Content>
