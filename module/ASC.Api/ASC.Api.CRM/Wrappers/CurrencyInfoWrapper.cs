using System;
using System.Runtime.Serialization;
using ASC.Web.CRM.Classes;

namespace ASC.Api.CRM
{

    /// <summary>
    ///  Информация о валюте
    /// </summary>
    [DataContract(Name = "currencyInfo", Namespace = "")]
    public class CurrencyInfoWrapper
    {

        public CurrencyInfoWrapper()
        {
            

        }

        public CurrencyInfoWrapper(CurrencyInfo currencyInfo)
        {
            Abbreviation = currencyInfo.Abbreviation;
            CultureName = currencyInfo.CultureName;
            Symbol = currencyInfo.Symbol;
            Title = currencyInfo.Title;
            IsConvertable = currencyInfo.IsConvertable;
        }

        [DataMember]
        public String Title { get; set; }

        [DataMember]
        public String Symbol { get; set; }

        [DataMember]
        public String Abbreviation { get; set; }

        [DataMember]
        public String CultureName { get; set; }

        [DataMember]
        public bool IsConvertable { get; set; } 

        public static CurrencyInfoWrapper GetSample()
        {
            return new CurrencyInfoWrapper
                       {
                           Title = "Chinese Yuan",
                           Abbreviation = "CNY",
                           Symbol = "¥",
                           CultureName = "CN",
                           IsConvertable = true
                       };
        }

    }
    
}
