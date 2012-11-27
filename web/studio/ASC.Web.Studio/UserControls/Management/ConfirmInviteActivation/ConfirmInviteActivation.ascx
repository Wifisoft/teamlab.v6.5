<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmInviteActivation.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ConfirmInviteActivation" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>
<asp:PlaceHolder runat="server" ID="_confirmHolder">
    <div class="confirmTitle headerBase">
        <%=Resources.Resource.InviteTitle%>
        <div class="subTitle">
            <% if (!String.IsNullOrEmpty(_email))
           {%>
            <%=Resources.Resource.InviteSubTitle%>
            <%
           }else
            {
            %>
            <%=Resources.Resource.InvitePublicSubTitle%>
            <%}%>
        </div>
    </div>
    <div class="clerFix confirmBlock">        
 
        <div class="rightPart">            
            
             <%if(!String.IsNullOrEmpty(_errorMessage)){%>
                <div class="errorBox"> <%=_errorMessage%></div>                
            <%} %>
            
            <%--FirstName--%>
            <% if (String.IsNullOrEmpty(_email) )
{	%>

<div class="property">
                <div class="name">                    
                        <%=Resources.Resource.Email%>:
                </div>
                <div class="value">
                    <input type="text" id="studio_confirm_Email" name="emailInput" class="textEdit" value="<%= GetEmailAddress() %>" />
                </div>
            </div>
  
<%} %>
            
            <div class="property">
                <div class="name">                    
                        <%=Resources.Resource.FirstName%>:
                </div>
                <div class="value">
                    <input type="text" id="studio_confirm_FirstName" value="<%=GetFirstName() %>" name="firstnameInput" class="textEdit" />
                </div>
            </div>
            
            <%--LastName--%>
           <div class="property">
                <div class="name">                        
                        <%=Resources.Resource.LastName%>:
                </div>
                <div class="value">
                    <input type="text" id="studio_confirm_LastName" value="<%=GetLastName()%>"  name="lastnameInput" class="textEdit" />
                </div>
            </div>
            
            <%--Pwd--%>
            <div class="property">
                <div class="name">                    
                        <%=Resources.Resource.InvitePassword%>:
                        <img class="hintImg" title="<%= UserManagerWrapper.GetPasswordHelpMessage() %>" src="<%= WebImageSupplier.GetAbsoluteWebPath("info.png") %>" />
                </div>
                <div class="value">
                    <input type="password" id="studio_confirm_pwd"  value="" name="pwdInput" class="textEdit" />
                </div>
            </div>       
            
            <%--RePwd--%>
             <div class="property">
                <div class="name">                    
                        <%=Resources.Resource.RePassword%>:
                </div>
                <div class="value">
                    <input type="password" id="studio_confirm_repwd"  value="" name="repwdInput" class="textEdit" />
                </div>
            </div>
           
            <div class="clearFix btnBox">
                <a class="bigLinkButton" href="#" onclick="AuthManager.ConfirmInvite(); return false;"><%=Resources.Resource.LoginRegistryButton%></a>
            </div>
        </div>
        
        <div class="leftPart">        
            <div class="borderBase tintMedium portalInfo">
                <a href="auth.aspx"><img class="logo" src="<%=_tenantInfoSettings.GetAbsoluteCompanyLogoPath()%>" border="0" alt="" /></a>
                <div class="headerBase"><%=HttpUtility.HtmlEncode(ASC.Core.CoreContext.TenantManager.GetCurrentTenant().Name)%></div>
                
                <div class="user borderBase">
                    <img class="avatar borderBase" src="<%=_userAvatar%>" alt="" />
                    <div class="name">
                        <div class="headerBaseSmall"><%=_userName %></div>
                        <div class="textBigDescribe"><%=_userPost %></div>
                    </div>
                </div>
            </div>
            
            <div class="description">
                <%=String.Format(Resources.Resource.InviteDescription,"<span class=\"blue\">","</span>")%>
            </div>
    <asp:PlaceHolder runat="server" ID="thrdParty" Visible="false"></asp:PlaceHolder>
        </div>
        
        </div>
    
    </asp:PlaceHolder>