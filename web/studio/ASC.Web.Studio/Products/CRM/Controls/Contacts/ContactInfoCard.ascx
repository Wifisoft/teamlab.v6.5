<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactInfoCard.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Contacts.ContactInfoCard" %>
<%@ Import Namespace="ASC.CRM.Core.Entities" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<div class="crm-contactInfoCard" id="contactInfoCard_<%= ContactID %>">
    <% if (CanAccess) %>
    <% { %>
    <a style="width: 82px; margin-bottom: 10px; float: left; clear: left;" href="<%= String.Format("default.aspx?{0}={1}{2}",UrlConstant.ID, ContactID,
                Target is Company ? String.Empty : String.Format("&{0}=people",UrlConstant.Type)) %>">
        <img border="0" style="margin: 0 auto;" src="<%= ContactPhotoManager.GetMediumSizePhoto(Target.ID, Target is Company) %>"
            class="crm-contactInfoCardImg">
    </a>
    <% } %>
    <% else %>
    <% { %>
    <img border="0" style="margin: 0 auto 10px; float: left;" src="<%= ContactPhotoManager.GetMediumSizePhoto(Target.ID, Target is Company) %>" class="crm-contactInfoCardImg">
    <% } %>
    <div style="margin-left: 92px;">
        
        <% if (!CanAccess) %>
        <% { %>
            <img align="absmiddle" src="<%= WebImageSupplier.GetAbsoluteWebPath("lock.png", ProductEntryPoint.ID) %>" style="margin-bottom: 3px;"/>
            <span class="crm-task-title activeTaskTitles"><%= Target.GetTitle().HtmlEncode() %></span>  
        <% } %>
        <% else %>
        <% { %>
            <a title="<%= Target.GetTitle().HtmlEncode() %>" class="linkHeader" style="display: inline-block;
                margin-bottom: 6px;text-decoration: underline;" href="<%= String.Format("default.aspx?{0}={1}{2}",UrlConstant.ID, ContactID,
                        Target is Company ? String.Empty : String.Format("&{0}=people",UrlConstant.Type)) %>">
                <%= Target.GetTitle().HtmlEncode()%>
            </a>
            <% if (Target is Person) %>
            <% { %>
            <div style="margin-bottom: 6px;">
                <%=(Target as Person).JobTitle.HtmlEncode()%>
            </div>
            <% } %>
            <% else %>
            <% { %>
            <div style="margin-bottom: 6px;">
                <%= Target.GetEmployeesCountString()%>
            </div>
            <% } %>
            <ul>
                <% foreach (var contactInfo in Global.DaoFactory.GetContactInfoDao().GetList(Target.ID, null, null, true)) %>
                <% { %>
                <li>
                    <%= RenderContactInfo(contactInfo)%>
                </li>
                <% } %>           
            </ul>
        <% } %>
    </div>
</div>
