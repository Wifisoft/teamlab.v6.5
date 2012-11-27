using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Web.Help.BingSearch;
using ASC.Api.Web.Help.DocumentGenerator;

namespace ASC.Api.Web.Help.Helpers
{
    public static class BingSearchHelper
    {
        public static Dictionary<MsDocEntryPoint, Dictionary<MsDocEntryPointMethod, string>> CreateResults(WebResult[] webResults)
        {
            var points = (from webResult in webResults
            let point = Documentation.GetByUri(new Uri(webResult.Url, UriKind.Absolute))
            where point != null
            select new KeyValuePair<MsDocEntryPointMethod,string>(point,webResult.Description)).Distinct(new BingComparer());

            return points.GroupBy(x=>x.Key.Parent)
                .ToDictionary(x=>x.Key,y=>y.ToDictionary(key=>key.Key,value=>value.Value));
        }
    }

    public class BingComparer : IEqualityComparer<KeyValuePair<MsDocEntryPointMethod, string>>
    {
        public bool Equals(KeyValuePair<MsDocEntryPointMethod, string> x, KeyValuePair<MsDocEntryPointMethod, string> y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(KeyValuePair<MsDocEntryPointMethod, string> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}