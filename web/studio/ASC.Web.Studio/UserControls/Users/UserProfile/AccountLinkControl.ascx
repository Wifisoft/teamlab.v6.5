<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountLinkControl.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserProfile.AccountLinkControl" %>

<% if (SettingsView){%>

    <div id="accountLinks">
        <ul class="clearFix">
            <%
           foreach (var acc in Infos)
           {%>
            <li class="<%=acc.Provider%>" style="<%=SettingsView ? "" : "float:left"%>">
           
            <span class="<%=acc.Linked ? "linked" : ""%>"> <%=acc.Linked
                                         ? Resources.Resource.AssociateAccountConnected
                                         : Resources.Resource.AssociateAccountNotConnected%>.</span> 
                <a href="<%=acc.Url%>" class="popup <%=acc.Linked ? "linked" : ""%>"
                    id="<%=acc.Provider%>">
                    <%=acc.Linked
                                     ? Resources.Resource.AssociateAccountDisconnect
                                     : Resources.Resource.AssociateAccountConnect%>
                </a></li>
            <%
           }%>
        </ul>
    </div>
<%}else{%>
    <div id="accountLinks">
        <ul class="clearFix">
            <%foreach (var acc in Infos){%>
                <li><a href="<%=acc.Url %>" class="popup <%=acc.Provider%> <%=acc.Linked?"linked":""%>" id="<%=acc.Provider%>"></a>
                </li>
            <%}%>
        </ul>
    </div>
<%}%>