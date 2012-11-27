<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VisitorsChart.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Statistics.VisitorsChart" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<div class="chartBlock">
  <div class="headerBase borderBase header">
    <%=Resources.Resource.VisitorsChartTitle%>
  </div>
  <div class="content">
    <ul id="visitorsFilter">
      <li id="filterByWeek" class="filter"><%=Resources.Resource.FilterLastWeek%></li>
      <li id="filterByMonth" class="filter"><%=Resources.Resource.FilterLastMonth%></li>
      <li id="filterBy3Months" class="filter"><%=Resources.Resource.FilterLast3Months%></li>
      <li id="filterByPeriod" class="filter"><%=Resources.Resource.FilterInPeriod%></li>
    </ul>
    <div id="periodSelection" class="date">
      <asp:HiddenField runat="server" ID="jQueryDateMask" />
      <div id="startDate" class="date">
        <span class="label"><%=Resources.Resource.FilterInPeriodFromLabel%></span>
        <asp:TextBox runat="server" ID="studio_chart_FromDate" CssClass="textEditCalendar" />
      </div>
      <div id="endDate" class="date">
        <span class="label"><%=Resources.Resource.FilterInPeriodToLabel%></span>
        <asp:TextBox runat="server" ID="studio_chart_ToDate" CssClass="textEditCalendar" />
      </div>
    </div>
    <br class="clear" />
    <div id="visitorsChartCanvas"></div>
    <ul id="chartLegend">
      <li class="label default">
        <div class="color"></div>
        <div class="title"></div>
      </li>
    </ul>
    <div class="subPanel">
      <a id="chartDownloadStatistics" class="baseLinkButton disabled" href="./">Download statistics File</a>
    </div>
  </div>
</div>
