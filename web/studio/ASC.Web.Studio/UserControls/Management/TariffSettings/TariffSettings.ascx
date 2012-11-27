<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TariffSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.TariffSettings" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users" TagPrefix="ascwc" %>

<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Core.Users" %>
 

<div class="tariffPlanBox">
    
    <%if (ASC.Core.Billing.TariffState.Paid <= tariff.State)
   { %>
        <div class="headerBase">
             <%=String.Format(Resources.Resource.PremiumTariffEndDateTitle, " <span class=\"date\">",  tariff.DueDate.ToShortDateString() + "</span>")%>
        </div>
        
    <%}
      else if (ASC.Core.Billing.TariffState.Frozen == tariff.State)
    { %>
        <div class="headerBase">  
            <%=String.Format(Resources.Resource.ProlongPremiumTitle,"<span class=\"date\">","</span>")%>
        </div> 
    <%}%>
        
        <asp:Repeater runat="server" ID="_paymentsRepeater">
        <HeaderTemplate>
            <div class="headerBase" id="paymentsTitle">
                <a href="#" id="paymentsHistoryBtn"><%=Resources.Resource.PaymentsTitle%></a>
            </div>     
            <div class="paymentsTableBox">
            <table class="paymentsTable">
        </HeaderTemplate>
            <ItemTemplate>
                <tr class="borderBase <%#Container.ItemIndex %2 ==0?"tintMedium":""%>">
                    <td>#<%#Eval("CartId")%></td> 
                    <td><%# ((DateTime)Eval("Date")).ToShortDateString() %> <%#((DateTime)Eval("Date")).ToShortTimeString() %></td>
                    <td><%#((decimal)Eval("Price")).ToString("###,##")%> <%#Eval("Currency")%></td>
                    <td><%#Eval("Method")%></td>
                    <td><%#Eval("Name")%></td>
                    <td><%#Eval("Email")%></td>
                </tr>
            </ItemTemplate>                        
            <FooterTemplate>
                </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>
        
       <%  if (ASC.Core.Billing.TariffState.Paid <= tariff.State)
           {%>
        <div class="extentPeriodText">
              <%=Resources.Resource.PremiumTariffExtendPeriodAutomatically%>
        </div>
        <%} %>
        
         <asp:PlaceHolder runat="server" ID="_buyNowHolder"></asp:PlaceHolder>
         
         <%--coupon--%>
        <%if (ASC.Core.Billing.TariffState.Paid >= tariff.State)
          { %>        
        <div id="couponBox">
            <div id="couponMsg"></div>
            <div class="headerBase"><%=Resources.Resource.CouponTitle%></div>
            <div class="btnBox clearFix">
                <input class="textEdit" type="text" id="couponCode"/>
                <a class="baseLinkButton" href="#" id="couponCodeBtn"><%=Resources.Resource.EnterCouponButton%></a>
            </div>
        </div>
            
        <%} %>
</div>

<script language="javascript">
    jq(function() {
        jq('#paymentsHistoryBtn').click(function() {
            if (jq('.tariffPlanBox .paymentsTableBox').is(':visible'))
                jq('.tariffPlanBox .paymentsTableBox').slideUp();
            else
                jq('.tariffPlanBox .paymentsTableBox').slideDown();
        });

        jq('#couponCode').Watermark('<%=Resources.Resource.CouponHelpTitle%>', 'describeText');

        jq('#couponCodeBtn').click(function() {

            LoadingBanner.animateDelay = 500;
            AjaxPro.onLoading = function(b) {
                if(b)
                    LoadingBanner.displayLoading(true);     
                else
                    LoadingBanner.hideLoading(true);    
            };                
            
            TariffSettingsController.UseCoupon(jq('#couponCode').val(), function(result) {
                var res = result.value;
                if (res.status == 0)
                    window.location.reload(true)
                else {
                    jq('#couponMsg').attr('class', 'errorBox');
                    jq('#couponMsg').html(res.message);
                }
            })

        });
    });
    
</script>