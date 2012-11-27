<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Masters/BasicTemplate.Master"
    CodeBehind="History.aspx.cs" Inherits="ASC.Web.Projects.History" %>

<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="BTHeaderContent" runat="server">
    <link href="<%= PathProvider.GetFileStaticRelativePath("history.css") %>" type="text/css"
        rel="stylesheet" />

</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <asp:PlaceHolder runat="server" ID="contentPlaceHolder"></asp:PlaceHolder>
    <div id="activitiesListContainer">
        <div id="EmptyListActivities" class="noContent">
        </div>
        <div id="EmptyListForFilter" class="noContent">
        </div>
        <table id="activitiesList">
            <thead>
                <tr>
                    <th class="date">
                    </th>
                    <th class="entityType">
                    </th>
                    <th class="title">
                    </th>
                    <th class="actionText">
                    </th>
                    <th class="user">
                    </th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
        <div id="showNextActivitiesProcess">
        </div>
    </div>
    <span id="showNextActivitiesButton">
        <%= ProjectResource.ShowNextActivities %></span>
    <asp:PlaceHolder runat="server" ID="activitiesListPlaceHolder"></asp:PlaceHolder>
    
    <div id="descriptionPanel" class="actionPanel" objid="">
	    <div class="popup-corner"></div>  
	    <div class="param">
	        <% if (!RequestContext.IsInConcreteProject()) { %>
	        <div class="project"><%= ProjectResource.Project %>:</div>
	        <% } %>
	    </div>         
	    <div class="value">
	        <% if (!RequestContext.IsInConcreteProject()) { %>
	        <div class="project"><a></a></div>
	        <% } %>
	    </div>
    </div>
    
</asp:Content>
