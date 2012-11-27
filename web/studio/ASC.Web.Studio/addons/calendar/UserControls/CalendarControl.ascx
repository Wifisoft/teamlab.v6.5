<%@ Assembly Name="ASC.Web.Calendar" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarControl.ascx.cs" Inherits="ASC.Web.Calendar.UserControls.CalendarControl" %>
<%@ Import Namespace="ASC.Data.Storage" %>
<script type="text/javascript" src="<%= WebPath.GetPath("/js/ajaxupload.3.5.js")%>"></script>
<link rel="stylesheet" type="text/css" href="<%= WebPath.GetPath("/addons/calendar/usercontrols/popup/css/popup.css")%>"/>
<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/popup/popup.js")%>"></script>

<link rel="stylesheet" type="text/css" href="<%= WebPath.GetPath("/addons/calendar/usercontrols/fullcalendar/css/asc-dialog/jquery-ui-1.8.14.custom.css")%>"/>
<link rel="stylesheet" type="text/css" href="<%= WebPath.GetPath("/addons/calendar/usercontrols/fullcalendar/css/asc-datepicker/jquery-ui-1.8.14.custom.css")%>"/>

<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/calendar_controller.js")%>"></script>
<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/recurrence_rule.js")%>"></script>

<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/jquery-ui-1.8.13.min.js")%>"></script>
<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/jquery-ui-i18n.js")%>"></script>

<script language="javascript" type="text/javascript">

    jq(function(){
        var cn = '<%=_currentCulture%>';
        if (jq.datepicker.regional[cn] === undefined) {
            cn = cn.substring(0, cn.search("-"));
        }
        if (jq.datepicker.regional[cn] === undefined) {
            cn = '';
        }
        jq.datepicker.setDefaults(jq.datepicker.regional[cn]);
    })
</script>

<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/jquery.jscrollpane.min.js")%>"></script>
<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/jquery.mousewheel.js")%>"></script>
<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/js/jquery.cookie.js")%>"></script>

<link rel="stylesheet" type="text/css" href="<%= WebPath.GetPath("/addons/calendar/usercontrols/css/jquery.jscrollpane.css")%>" />	
<link rel="stylesheet" type="text/css" href="<%= WebPath.GetPath("/addons/calendar/usercontrols/fullcalendar/css/fullcalendar.css")%>" />
<script type="text/javascript" src="<%= WebPath.GetPath("/addons/calendar/usercontrols/fullcalendar/fullcalendar.js")%>"></script>

<%@Register Src="~/addons/calendar/usercontrols/CalendarResources.ascx" TagName="CalendarResources" TagPrefix="ascwc"  %>



<ascwc:CalendarResources runat="server"></ascwc:CalendarResources>
<script type="text/javascript">
    jq(document).ready(function() {

        LoadingBanner.animateDelay = 500;
        jq(document).ajaxStart(function() {
            LoadingBanner.displayLoading(true);
        });

        jq(document).ajaxStop(function() {
            LoadingBanner.hideLoading(true);
        });


        var viewName = 'month';
        var today = new Date();
        
         try{
            ASC.Controls.AnchorController.init();

            var currentAnchor = ASC.Controls.AnchorController.getAnchor();
            viewName = currentAnchor.split('/')[0];
            var date = currentAnchor.split('/')[1];
       
            var today = (date == '' || date == undefined || isNaN(date)) ? new Date() : new Date(parseInt(date));
            if ('Invalid Date' == today || today == 'NaN')
                today = new Date();
        }
        catch(e)
        {
            today = new Date();
            viewName = 'month';
        }

        ASC.CalendarController.ApiUrl = '<%=ASC.Web.Studio.Core.SetupInfo.WebApiBaseUrl.TrimEnd('/') + "/calendar"%>';

        var calHeight = jq(window).height() -
            jq("#studioPageContent .mainContainer").outerHeight(true) -
                jq("#studioFooter").height();
                
        var timeZones = [<%=RenderTimeZones()%>];
        var defTimeZone = null;        
        var curDate = new Date();
        var clientOffset = (-1)*curDate.getTimezoneOffset();
        var neighbourTimeZones = new Array();
        jq(timeZones).each(function(i, el)
        {
            if(el.offset == clientOffset)            
                neighbourTimeZones.push(el);            
        });
        
        jq(neighbourTimeZones).each(function(i, el)
        {
            if(curDate.toString().indexOf(el.id)>=0)            
            {
                defTimeZone = el;            
                return false;
            }
        });
        
        if(neighbourTimeZones.length>0 && defTimeZone == null)
            defTimeZone = neighbourTimeZones[0];
        else if(defTimeZone == null)
            defTimeZone == {id:"UTC", name : "UTC", offset : 0};

        jq("#asc_calendar").fullCalendar({

            defaultView: (viewName == '' || viewName == undefined) ? 'month' : viewName,
            year: today.getFullYear(),
            month: today.getMonth(),
            date: today.getDate(),

            notifications: {
                editorUrl: '<%= WebPath.GetPath("/addons/calendar/usercontrols/fullcalendar/tmpl/notifications.editor.tmpl")%>'
            },

            selectable: true,
            selectHelper: true,
            editable: true,

            padding: 0,
            height: calHeight,

            onHeightChange: function() {                
                var h = jq(window).height();
                this.height = h - jq("#studioPageContent .studioTopNavigationPanel").outerHeight(true) - jq("#studioFooter").height();
            },
            
            personal: <%=ASC.Web.Studio.Core.SetupInfo.IsPersonal?"true":"false"%>,

            loadEventSources: ASC.CalendarController.LoadCalendars,
            editCalendar: ASC.CalendarController.DoRequestToCalendar,
            editEvent: ASC.CalendarController.DoRequestToEvent,            
            editPermissions: ASC.CalendarController.EditPermissions,
            removePermissions: ASC.CalendarController.RemovePermissions,
            loadSubscriptions: ASC.CalendarController.LoadSubscriptions,
            manageSubscriptions: ASC.CalendarController.ManageSubscriptions,            
            viewChanged: ASC.CalendarController.ViewChangedHandler,
            getiCalUrl: ASC.CalendarController.GetiCalUrl,
            defaultTimeZone: defTimeZone,
            timeZones : timeZones,
            getMonthEvents: ASC.CalendarController.GetEventDays
        });

    });
</script>
<asp:PlaceHolder runat="server" ID="_sharingContainer"></asp:PlaceHolder>
<div id="asc_calendar"></div>