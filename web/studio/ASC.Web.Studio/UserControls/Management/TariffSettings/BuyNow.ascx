<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuyNow.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.BuyNow" %>
<div class="buyNowBox">
        <div class="subscription clearFix">
            <div class="monthlySubscription headerBase">
                <%=String.Format(Resources.Resource.TeamlabMonthlySubscription, "<span class=\"bold\">", "</span>",50)%>
            </div>
            <a id="buyMonthBtn" class="buyButton" href="<%=_monthUrl%>"><%=Resources.Resource.PremiumStubButton%></a>
        </div>
        <div class="extentPeriodText">
              <%=Resources.Resource.PremiumTariffExtendPeriodAutomatically%>
        </div>
        <input type="hidden" id="isExpired" value="<%=_isExpired?"1":"0"%>"/>
</div>    

<script language="javascript">
    jq(function() {
        jq('#buyMonthBtn').click(function() {
            if (jq('#isExpired').val() == 1)
                EventTracker.Track('buynow_paymentsExpired');
            else
                EventTracker.Track('buynow_paymentsFromFree');
        });

        jq('#buyYearBtn').click(function() {
            if (jq('#isExpired').val() == 1)
                EventTracker.Track('buynow_year_paymentsExpired');
            else
                EventTracker.Track('buynow_year_paymentsFromFree');
        });

        jq('div.yearlySubscription').width(jq('div.yearlySubscription').width() + 10);
        jq('div.monthlySubscription').width(jq('div.monthlySubscription').width() + 10);
        if (jq('div.monthlySubscription').width() < jq('div.yearlySubscription').width())
            jq('div.yearlySubscription').width(jq('div.monthlySubscription').width());
        else
            jq('div.monthlySubscription').width(jq('div.yearlySubscription').width());
    });
    
</script>