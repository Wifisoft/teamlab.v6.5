#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using AjaxPro;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    [AjaxNamespace("AjaxPro.ExchangeRateView")]
    public partial class ExchangeRateView : BaseUserControl
    {
        #region Property

        public static String Location { get { return PathProvider.GetFileStaticRelativePath("Deals/ExchangeRateView.ascx"); } }

        protected CurrencyInfo DefaultCurrency { get; set; }

        protected List<CurrencyInfo> AllCurrencyRates { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(GetType());

            DefaultCurrency = Global.TenantSettings.DefaultCurrency;
            AllCurrencyRates = CurrencyProvider.GetAll().Where(n => n.IsConvertable).ToList();
            _exchangeRate.Options.IsPopup = true;

            
            ExchangeRateTabs.DisableJavascriptSwitch = true;

            ConverterTab.TabName = CRMCommonResource.MoneyCalculator;
            ExchangeTab.TabName = CRMCommonResource.SummaryTable;
            TotalAmountTab.TabName = CRMDealResource.TotalAmount;

            RegisterClientScriptForTotalAmount();

        }

        #endregion

        #region Methods

        private void RegisterClientScriptForTotalAmount()
        {
            Utility.RegisterTypeForAjax(typeof(AjaxProHelper));

            var exchangeRates = CurrencyProvider.MoneyConvert(DefaultCurrency);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ba178c20-f0ef-4f50-b240-6d62e7e429db",
                                                           "exchangeRatesArray = " +
                                                           JavaScriptSerializer.Serialize(exchangeRates.ToList().ConvertAll(
                                                               item => new
                                                               {
                                                                   abbreviation = item.Key.Abbreviation,
                                                                   value = item.Value
                                                               })) + ";", true);

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "47304ce8-f1a3-466a-abed-bcfda2f24968",
                                                        "defaultCurrency = " + JavaScriptSerializer.Serialize(DefaultCurrency) + ";", true);

        }

        #region Ajax Methods

        public class RateInfo
        {
            public decimal Rate { get; set; }
            public string FromCurrencyTitle { get; set; }
            public string ToCurrencyTitle { get; set; }

            public RateInfo(decimal rate, string fromCurrencyTitle, string toCurrencyTitle)
            {
                Rate = rate;
                FromCurrencyTitle = fromCurrencyTitle;
                ToCurrencyTitle = toCurrencyTitle;
            }

        }

        public class RateAndAbbr
        {
            public decimal Rate { get; set; }
            public string CurrencyAbbr { get; set; }

            public RateAndAbbr(decimal rate, string currencyAbbr)
            {
                Rate = rate;
                CurrencyAbbr = currencyAbbr;

            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ConvertAmount(decimal amount, string fromCurrency, string toCurrency)
        {
            var conversionResult = CurrencyProvider.MoneyConvert(amount, fromCurrency, toCurrency);
            return String.Format("{0} {1} = {2} {3}", amount, fromCurrency, conversionResult, toCurrency);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public RateInfo ChangeCurrency(string fromCurrency, string toCurrency)
        {
            return new RateInfo(CurrencyProvider.MoneyConvert(1, fromCurrency, toCurrency),
                                CurrencyProvider.Get(fromCurrency).Title, CurrencyProvider.Get(toCurrency).Title);

        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public List<RateAndAbbr> UpdateSummaryTable(string newCurrency)
        {
            var table = CurrencyProvider.MoneyConvert(CurrencyProvider.Get(newCurrency));
            return table.Select(tableItem => new RateAndAbbr(tableItem.Value, tableItem.Key.Abbreviation)).ToList();

        }
        

        #endregion

        #endregion
        
    }
}