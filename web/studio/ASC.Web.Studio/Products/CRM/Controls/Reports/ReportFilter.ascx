<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportFilter.ascx.cs"
                            Inherits="ASC.Web.CRM.Controls.Reports.ReportFilter" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<%@ Import Namespace="ASC.Web.CRM.Controls.Reports" %>
<%@ Import Namespace="ASC.Web.Core.Helpers" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>


<% if (ReportType == ASC.CRM.Core.Dao.ReportType.PipelineByMilestone) %>
<% { %>
<div id="reportFilters">
    <div class="headerBase pm-report-filtersHeader">
        <%=CRMReportResource.Filter%></div>
    <table cellpadding="5px" cellspacing="0px">
        <tr>
            <td class="textBigDescribe" colspan="2">
                <%= CRMReportResource.ChooseTimeInterval%>:
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="width: 450px;">
                    <div id="otherInterval" style="display:none;">
                        <div style="float: left; padding-left: 15px; padding-right: 15px;">
                            <asp:TextBox runat="server" ID="usersActivityFromDate" CssClass="pm-ntextbox textEditCalendar" Width="75px" />
                        </div>
                        <div>
                            <asp:TextBox runat="server" ID="usersActivityToDate" CssClass="pm-ntextbox textEditCalendar" Width="75px" />
                        </div>
                    </div>
                    <div style="clear: left;"></div>    
                </div>         
            </td>
        </tr>
        <tr>
            <td class="textBigDescribe">
                <%=CRMReportResource.Managers%>:
            </td>
        </tr>
        <tr>
            <td>
                <select id="Users" class="comboBox pm-report-select">
                    <%=InitManagersDdl()%>
                </select>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript" language="javascript">
    jq(function() {
        jq("[id$=usersActivityFromDate],[id$=usersActivityToDate]").mask('<%= System.DateTimeExtension.DateMaskForJQuery %>');
        jq("[id$=usersActivityFromDate],[id$=usersActivityToDate]").datepick({ selectDefaultDate: true, showAnim: '' });

//        jq("input.textEditCalendar").mask('<%= DateTimeExtension.DateMaskForJQuery %>');
//        jq("input.textEditCalendar").datepick({
//            popupContainer: "body",
//            selectDefaultDate: true,
//            showAnim: ''
//        });
        
        
        
        ASC.CRM.Reports.setFiltersValueInObjectsFromURL();
    });
    

    
    function changeInterval()
    {
        if(jq('#TimeIntervals option:selected').val()=='0')
            jq('#otherInterval').show();
        else
            jq('#otherInterval').hide();
    }
</script>
<% } %>

  
<% if (ASC.Core.SecurityContext.IsAuthenticated) { %>
<input id="cbxSaveAsTemplate" type="checkbox" onclick="javascript:if(jq(this).attr('checked')){viewReportTemplateContainer(-1,'<%= GetReportTypeTitle() %>');}" />
<label for="cbxSaveAsTemplate">
    <%=CRMReportResource.SaveAsTemplate%>
</label>
<% } %>
<div class="pm-h-line"></div>

<asp:PlaceHolder ID="reportTemplatePh" runat="server"></asp:PlaceHolder>


<div>
    <div class="pm-action-block">
        <a onclick="ASC.Projects.Reports.generateReportInNewWindow();" class="baseLinkButton">
            <%=CRMReportResource.GenerateReport%>
        </a>
    </div>
    <div class='pm-ajax-info-block' style="display: none;">
        <span class="textMediumDescribe">
            <%= CRMReportResource.BuildingReport%>
            
            </span><br />
        <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
    </div>
</div>

<asp:HiddenField ID="HiddenFieldViewReportFilters" runat="server" />
<asp:HiddenField ID="HiddenFieldViewReportTemplate" runat="server" />