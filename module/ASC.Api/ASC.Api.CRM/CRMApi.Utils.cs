#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Web.CRM.Classes;

#endregion

namespace ASC.Api.CRM
{
   public partial class CRMApi
    {
        /// <summary>
        ///     Returns the list of all currencies currently available on the portal
        /// </summary>
        /// <short>Get currency list</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///    List of available currencies
        /// </returns>
        [Read("settings/currency")]
        public IEnumerable<CurrencyInfoWrapper> GetAvaliableCurrency()
        {
            return CurrencyProvider.GetAll().ConvertAll(item => new CurrencyInfoWrapper(item)).ToItemList();
        }

        protected CurrencyInfoWrapper ToCurrencyInfoWrapper(CurrencyInfo currencyInfo)
        {
            if (currencyInfo ==  null) return null;

            return new CurrencyInfoWrapper(currencyInfo);

        }
    }
}
