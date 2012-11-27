<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSpendActionView.ascx.cs" Inherits="ASC.Web.Projects.Controls.TimeSpends.TimeSpendActionView" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("apitimespendedit.js") %>"></script>
<script type="text/javascript">
    jq(document).ready(function() {
        ASC.Projects.TimeTrakingEdit.init("<%= DateTimeExtension.DateMaskForJQuery %>");
    })
</script>

<div id="timeTrakingPopup" style="display:none;">
    <ascwc:container id="_timetrackingContainer" runat="server">
    
    <header>    
        <%= ProjectsCommonResource.TimeTracking %>
    </header>
    
    <body>
        
        <div id="TimeLogTaskTitle" class="headerBase pm-headerPanelSmall-splitter"></div>
           
        <div class="infoPanel addLogPanel-infoPanel" style="margin-bottom: 15px;">
            <div class="addLogPanel-infoPanelBody">
                <span class="headerBase pm-grayText">
                    <%= ProjectsCommonResource.SpentTotally %>
                </span>
                <span class="button-splitter"></span>
                <span id="TotalHoursCount" class="headerBase"></span>
                <span class="button-splitter"></span>
            </div>
        </div> 
        <div class="warnBox" style="display:none;" id="timeTrakingErrorPanel"></div>
        <div class="pm-headerPanelSmall-splitter" style="float:right">
            <div class="headerPanelSmall">
                <b><%= TaskResource.TaskResponsible %>:</b>
            </div>
            <select style="width: 220px;" class="comboBox pm-report-select" id="teamList"></select>
        </div>
        
        <div>   
        <div class="pm-headerPanelSmall-splitter" style="float:left;margin-right:20px">
            <div class="headerPanelSmall">
                <b><%= ProjectsCommonResource.Time%>:</b>
            </div>
            <input id="inputTimeHours" type="text" placeholder="<%=ProjectsCommonResource.WatermarkHours %>" class="textEdit" maxlength="2" />
            <span class="splitter">:</span>
            <input id="inputTimeMinutes" type="text" placeholder="<%=ProjectsCommonResource.WatermarkMinutes %>" class="textEdit" maxlength="2" />
        </div>
           
        <div class="pm-headerPanelSmall-splitter">
            <div class="headerPanelSmall">
                <b><%= ProjectsCommonResource.Date %>:</b>
            </div>
            <input id="timeTrakingDate" class="pm-ntextbox textEditCalendar" style="margin-right: 3px"/>
        </div>
        </div>
        
        <div style="clear:both"></div>
           
        <div class="pm-headerPanelSmall-splitter">
            <div class="headerPanelSmall">
                <b><%= ProjectResource.ProjectDescription %>:</b>
            </div>
            <textarea id="timeDescription" rows="7" cols="20" class="pm-ntextbox" style="width: 99%; resize: none;" MaxLength="250"></textarea>
        </div>
           
        <div class="pm-h-line" ></div>
           
        <div class="pm-action-block">
            <a href="javascript:void(0)" class="baseLinkButton">
                <%= ProjectsCommonResource.SaveChanges%>
            </a>
            <span class="button-splitter"></span>
            <a class="grayLinkButton" href="javascript:void(0)" onclick="javascript: jq.unblockUI();">
                <%= ProjectsCommonResource.Cancel%>
            </a>
        </div>
           
    </body>
</ascwc:container>

</div>
