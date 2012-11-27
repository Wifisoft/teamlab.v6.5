<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Controls" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmptyWidget.ascx.cs" Inherits="ASC.Web.Projects.Controls.Dashboard.EmptyDashboard" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<div class="emptyWidgetImageContainer">
    <img class="emptyWidgetImage" src="App_Themes/Default/Images/product_logolarge.png"/>
</div>

<div class="emptyWidgetDescription">
<div class="emptyWidgetHeader"><%= ProjectResource.EmptyScreenMyActiveProjectsHeaderContent %></div>

<div class="emptyWidgetMessage"><%= ProjectResource.EmptyScreenMyActiveProjectsDecscriptionContent %></div>

<div>
    <a href="Projects.aspx"><%=  ProjectResource.FollowProjects %></a>
</div>
</div>
<div style="clear: both;"></div>
