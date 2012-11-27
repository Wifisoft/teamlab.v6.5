<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeAndLanguage.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.TimeAndLanguage" %>

<div class="clearFix">
<div class="clearFix" id="timeandlang">
    <div class="headerBaseSmall headertitle">
        <%=Resources.Resource.Language%>:</div>
    <div style="float: left; width: 200px;" class="studioHeaderBaseSmallValue">
        <%=RenderLanguageSelector()%></div>
</div>
<div class="clearFix lang" id="timeandlang">
    <div class="headerBaseSmall headertitle">
        <%=Resources.Resource.TimeZone%>:</div>
    <div style="float: left; width: 200px" class="studioHeaderBaseSmallValue">
        <%=RenderTimeZoneSelector()%></div>
</div>
</div>