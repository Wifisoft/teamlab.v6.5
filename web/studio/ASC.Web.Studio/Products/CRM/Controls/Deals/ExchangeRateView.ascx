<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeRateView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Deals.ExchangeRateView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>

<% if (Global.DebugVersion) { %>
<link href="<%= PathProvider.GetFileStaticRelativePath("fg.css") %>" type="text/css" rel="stylesheet" />
<% } %>

<script type="text/javascript" language="javascript">
    jq(document).ready(function() {
        ASC.CRM.ExchangeRateView.init();
    });
</script>

<div id="exchangeRatePopUp" style="display: none">
    <ascwc:Container ID="_exchangeRate" runat="server">
        <header>
           <%= CRMCommonResource.ConversionRates %>
        </header>
        <body>
            <ascwc:ViewSwitcher ID="ExchangeRateTabs" runat="server" RenderAllTabs="true">
                <tabitems>

                    <ascwc:ViewSwitcherTabItem ID="TotalAmountTab" runat="server">
                        <div id="TotalAmountContent" style="height:350px;">
                            <table id="totalOnPage" cellspacing="0" cellpadding="7" style="display:none;">
                                <tr>
                                    <td class="headerBaseMedium" style="text-align: right; width: 100%; vertical-align: top;">
                                        <%= CRMDealResource.TotalOnPage%>:
                                    </td>
                                    <td style="white-space: nowrap; text-align: right;">
                                        <div class="diferrentBids">
                                        </div>
                                        <div class="totalBidAndExchangeRateLink" style="display:none;">
                                            <div class="h_line" style="margin-top: 5px; margin-bottom: 5px;">&nbsp;</div>
                                            <div class="totalBid">
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ascwc:ViewSwitcherTabItem>

                    <ascwc:ViewSwitcherTabItem ID="ConverterTab" runat="server">
                        <div id="convertRateContent" style="height:350px;">
                             <dl>
                                <dt class="headerBaseSmall"><%= CRMCommonResource.EnterAmount%>:</dt>
                                <dd>
                                    <input class="textEdit" type="text" id="amount"/>
                                </dd>

                                <dt class="headerBaseSmall"><%= CRMCommonResource.From %>:</dt>
                                <dd>
                                    <select id="fromCurrency" onchange="ASC.CRM.ExchangeRateView.changeCurrency();" class="comboBox">
                                        <% foreach (var keyValuePair in AllCurrencyRates)%>
                                        <% { %>
                                        <option value="<%= keyValuePair.Abbreviation %>"
                                            <%=  String.Compare(keyValuePair.Abbreviation,  Global.TenantSettings.DefaultCurrency.Abbreviation, true) == 0 ? "selected=selected" : String.Empty %>>
                                            <%=String.Format("{0} - {1}", keyValuePair.Abbreviation, keyValuePair.Title)%>
                                        </option>
                                        <% } %>
                                    </select>
                                </dd>

                                <dt class="headerBaseSmall"><%= CRMCommonResource.To %>:</dt>
                                <dd>
                                    <select id="toCurrency" onchange="ASC.CRM.ExchangeRateView.changeCurrency();" class="comboBox">
                                        <% foreach (var keyValuePair in AllCurrencyRates)%>
                                        <% { %>
                                        <option value="<%= keyValuePair.Abbreviation %>"
                                            <%=  String.Compare(keyValuePair.Abbreviation,  Global.TenantSettings.DefaultCurrency.Abbreviation, true) == 0 ? "selected=selected" : String.Empty %>>
                                            <%=String.Format("{0} - {1}", keyValuePair.Abbreviation, keyValuePair.Title)%>
                                        </option>
                                        <% } %>
                                    </select>
                                    <div class="h_line">&nbsp;</div>
                                </dd>
                            </dl>

                            <table cellpadding="2" cellspacing="0" border="0">
                                <tr>
                                    <td class="headerBaseSmall" style="text-align: right;" id="introducedFromCurrency"><%= DefaultCurrency.Title %>:</td>
                                    <td class="headerBaseMedium" id="introducedAmount"></td>
                                </tr>
                                <tr>
                                    <td class="headerBaseSmall" style="text-align: right;"><%= CRMCommonResource.ConversionRate %>:</td>
                                    <td class="headerBaseMedium" id="conversionRate">
                                        <%= String.Format("1 {0} = 1 {0}", DefaultCurrency.Abbreviation) %>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="headerBaseSmall" id="introducedToCurrency" style="text-align: right;"><%= DefaultCurrency.Title%>:</td>
                                    <td class="headerBaseMedium" id="conversionResult"></td>
                                </tr>
                            </table>
                        </div>
                    </ascwc:ViewSwitcherTabItem>

                    <ascwc:ViewSwitcherTabItem ID="ExchangeTab" runat="server">
                         <div style="height:350px; overflow-x:hidden;">
                            <select onchange="ASC.CRM.ExchangeRateView.updateSummaryTable(this.value);" style="width: 100%;" class="comboBox">
                                        <% foreach (var keyValuePair in AllCurrencyRates)%>
                                        <% { %>
                                        <option value="<%= keyValuePair.Abbreviation %>"
                                            <%=  String.Compare(keyValuePair.Abbreviation,  Global.TenantSettings.DefaultCurrency.Abbreviation, true) == 0 ? "selected=selected" : String.Empty %>>
                                            <%=String.Format("{0} - {1}", keyValuePair.Abbreviation, keyValuePair.Title)%>
                                        </option>
                                        <% } %>
                            </select>

                            <table class="tableBase" cellpadding="5" cellspacing="0" id="ratesTable">
                                <tbody>
                                    <tr>
                                        <th style="width:30px;"></th>
                                        <th style="width:22px;"></th>
                                        <th></th>
                                        <th style="width:30px;"></th>
                                        <th style="width:22px;"></th>
                                        <th></th>
                                        <th style="width:30px;"></th>
                                        <th style="width:22px;"></th>
                                        <th></th>
                                    </tr>

                                    <% int counter = 0; %>
                                    <% foreach (var keyValuePair in CurrencyProvider.MoneyConvert(DefaultCurrency))%>
                                    <% { %>
                                    <% counter++; %>

                                    <% if (counter % 3 == 1) %>
                                    <% { %>
                                    <tr>
                                    <% }%>
                                        <td class="borderBase">
                                            <i class="b-fg b-fg_<%= keyValuePair.Key.CultureName %>">
                                                <img src="<%= WebImageSupplier.GetAbsoluteWebPath("fg.png", ProductEntryPoint.ID) %>"
                                                    alt="<%= keyValuePair.Key.Abbreviation %>" title="<%= keyValuePair.Key.Title %>"/>
                                            </i>
                                        </td>
                                        <td class="borderBase headerBaseSmall">
                                            <%= keyValuePair.Key.Abbreviation %>
                                        </td>
                                        <td class="borderBase" id="<%= keyValuePair.Key.Abbreviation %>" style="<%= counter % 3 == 0 ? "padding-right:20px;" : "padding-right:35px;" %>">
                                            <%= keyValuePair.Value %>
                                        </td>
                                    <% if (counter % 3 == 0) { %>
                                    </tr>
                                    <% }%>

                                    <% } %>
                                </tbody>
                            </table>
                        </div>
                    </ascwc:ViewSwitcherTabItem>
                </tabitems>
            </ascwc:ViewSwitcher>

            <div class="action_block clearFix" style="margin-top: 15px;">
                <div style="display:inline-block;">
                  <span class="headerBaseSmall"><%= CRMCommonResource.ConversionDate%></span>:&nbsp;   <%= String.Format("{0} {1}", CurrencyProvider.GetPublisherDate.ToShortDateString(), CurrencyProvider.GetPublisherDate.ToShortTimeString())%>
                </div>
                <div style="float: right;">
                  <span class="textMediumDescribe"><%= CRMCommonResource.InformationProvidedBy%></span> <a class="linkHeaderLightSmall" href="http://themoneyconverter.com/" target="_blank">The Money Converter.com</a>
                </div>
                 <div style="margin-top: 10px;">
                    <a class="grayLinkButton" href="javascript:void(0)" onclick="javascript: PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                        <%= CRMCommonResource.CloseThisWindow%>
                    </a>
                </div>
            </div>
        </body>
    </ascwc:Container>
</div>