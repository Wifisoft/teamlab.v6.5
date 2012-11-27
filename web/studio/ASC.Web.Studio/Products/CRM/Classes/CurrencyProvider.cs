using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using ASC.Web.CRM.Resources;
using log4net;

namespace ASC.Web.CRM.Classes
{
    [Serializable]
    public class CurrencyInfo
    {
        public string Title { get; set; }
        public string Symbol { get; set; }
        public string Abbreviation { get; set; }
        public string NationFlagCSSName { get; set; }


        public CurrencyInfo()
        {

        }

        public CurrencyInfo(string title, string abbreviation, string symbol, string cssname)
        {
            Title = title;
            Symbol = symbol;
            Abbreviation = abbreviation;
            NationFlagCSSName = cssname;
        }


        public override bool Equals(object obj)
        {
            var ci = obj as CurrencyInfo;
            return ci != null && string.Compare(Title, ci.Title, true) == 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(Abbreviation, "-", Title);
        }
    }


    public static class CurrencyProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CurrencyProvider));
        private static readonly object syncRoot = new object();

        private static readonly Dictionary<String, CurrencyInfo> currencies;
        private static Dictionary<String, Decimal> exchangeRates;


        static CurrencyProvider()
        {
            currencies = new[]
            {
                new CurrencyInfo(CRMCommonResource.Currency_UnitedArabEmiratesDirham, "AED", "د.إ", "AE"),
                new CurrencyInfo(CRMCommonResource.Currency_ArgentinianPeso, "ARS", "$a","AR"),
                new CurrencyInfo(CRMCommonResource.Currency_AustralianDollar,"AUD","A$","AU"),    
                new CurrencyInfo(CRMCommonResource.Currency_BrazilianReal,"BRL","R$","BR"),
                new CurrencyInfo(CRMCommonResource.Currency_CanadianDollar,"CAD","C$","CA"),  
                new CurrencyInfo(CRMCommonResource.Currency_SwissFranc,"CHF","Fr","CH"),   
                new CurrencyInfo(CRMCommonResource.Currency_ChineseYuan,"CNY","¥","CN"),
                new CurrencyInfo(CRMCommonResource.Currency_DanishKrone,"DKK","kr","DK"),     
                new CurrencyInfo(CRMCommonResource.Currency_Euro,"EUR","€","EU"),
                new CurrencyInfo(CRMCommonResource.Currency_PoundSterling,"GBP","£","GB"),
                new CurrencyInfo(CRMCommonResource.Currency_HongKongDollar,"HKD","HK$","HK"),
                new CurrencyInfo(CRMCommonResource.Currency_HungarianForint,"HUF","Ft","HU"),
                new CurrencyInfo(CRMCommonResource.Currency_IsraeliSheqel,"ILS","₪","IL" ),
                new CurrencyInfo(CRMCommonResource.Currency_IndianRupee,"INR","₨","IN"),
                new CurrencyInfo(CRMCommonResource.Currency_JapaneseYen,"JPY","¥","JP"),
                new CurrencyInfo(CRMCommonResource.Currency_SouthKoreanWon,"KRW","₩","KR"),
                new CurrencyInfo(CRMCommonResource.Currency_MoroccanDirham,"MAD","د.م","MA"),
                new CurrencyInfo(CRMCommonResource.Currency_MexicanPeso,"MXN","MEX$","MX"),
                new CurrencyInfo(CRMCommonResource.Currency_NorwegianKrone,"NOK","kr","NO"),
                new CurrencyInfo(CRMCommonResource.Currency_NewZealandDollar,"NZD","NZ$","NZ"),
                new CurrencyInfo(CRMCommonResource.Currency_PhilippinePeso,"PLN","zł","PL"),
                new CurrencyInfo(CRMCommonResource.Currency_Rouble,"RUB","руб.","RU"),
                new CurrencyInfo(CRMCommonResource.Currency_SwedishKrona,"SEK","kr","SE"),
                new CurrencyInfo(CRMCommonResource.Currency_SingaporeDollar,"SGD","S$","SG"),
                new CurrencyInfo(CRMCommonResource.Currency_ThaiBaht,"THB","฿","TH"),
                new CurrencyInfo(CRMCommonResource.Currency_TurkishNewLira,"TRY","YTL","TR"),
                new CurrencyInfo(CRMCommonResource.Currency_UnitedStatesDollar,"USD","$","US"),
                new CurrencyInfo(CRMCommonResource.Currency_SouthAfricanRand,"ZAR","R","ZA")
            }
            .ToDictionary(c => c.Abbreviation);
        }


        public static CurrencyInfo Get(string currencyAbbreviation)
        {
            return currencies[currencyAbbreviation];
        }

        public static List<CurrencyInfo> GetAll()
        {
            return currencies.Values.OrderBy(v => v.Abbreviation).ToList();
        }

        public static Dictionary<CurrencyInfo, Decimal> MoneyConvert(CurrencyInfo baseCurrency)
        {
            if (baseCurrency == null) throw new ArgumentNullException("baseCurrency");
            if (!currencies.ContainsKey(baseCurrency.Abbreviation)) throw new ArgumentOutOfRangeException("baseCurrency", "Not found.");

            var result = new Dictionary<CurrencyInfo, Decimal>();
            var rates = GetExchangeRates();
            foreach (var ci in GetAll())
            {
                var key = String.Format("{1}/{0}", baseCurrency.Abbreviation, ci.Abbreviation);
                result.Add(ci, baseCurrency.Title == ci.Title ? 1 : rates[key]);
            }
            return result;
        }

        public static Decimal MoneyConvert(decimal amount, string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.Compare(from, to, true) == 0) return amount;

            var rates = GetExchangeRates();
            var key = string.Format("{0}/{1}", to, from);
            return rates[key] * amount;
        }

        public static Decimal MoneyConvertToDefaultCurrency(decimal amount, string from)
        {
            return MoneyConvert(amount, from, Global.TenantSettings.DefaultCurrency.Abbreviation);
        }


        private static Dictionary<String, Decimal> GetExchangeRates()
        {
            if (exchangeRates == null)
            {
                lock (syncRoot)
                {
                    if (exchangeRates == null)
                    {
                        exchangeRates = new Dictionary<string, decimal>();
                        var regex = new Regex("= (?<Currency>([\\s\\.\\d]*))");
                        var separator = CultureInfo.InvariantCulture.NumberFormat.CurrencyGroupSeparator;

                        foreach (var ci in currencies.Values)
                        {
                            var filepath = Path.Combine(HttpContext.Current.Server.MapPath(@"~\products\crm\App_Data\Exchange_Rates\"), ci.Abbreviation + ".xml");

                            if (!File.Exists(filepath))
                            {
                                DownloadRSS(ci.Abbreviation, filepath);
                            }
                            if (!File.Exists(filepath))
                            {
                                continue;
                            }

                            using (var reader = XmlReader.Create(filepath))
                            {
                                var feed = SyndicationFeed.Load(reader);
                                if (feed != null)
                                {
                                    foreach (var item in feed.Items)
                                    {
                                        var currency = regex.Match(item.Summary.Text).Groups["Currency"].Value.Replace(".", separator);
                                        exchangeRates.Add(item.Title.Text, Convert.ToDecimal(currency));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return exchangeRates;
        }

        private static void DownloadRSS(string currency, string filepath)
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    var dir = Path.GetDirectoryName(filepath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var response = webClient.DownloadString(string.Format("http://themoneyconverter.com/{0}/rss.xml", currency));
                    File.WriteAllText(filepath, response);
                }
                catch (Exception error)
                {
                    log.Error(error);
                }
            }
        }
    }
}
