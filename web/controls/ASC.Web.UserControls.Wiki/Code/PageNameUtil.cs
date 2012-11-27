using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace ASC.Web.UserControls.Wiki
{
    public class PageNameUtil
    {
        public static string Clean(string str)
        {
            return str.Replace("_", " ");
        }

        public static string Encode(string str)
        {
            string result = str.Replace(" ", "_");
            return HttpUtility.UrlEncode(result);
        }

        public static string Decode (string str)
        {
            string result = str; //HttpUtility.UrlDecode(str);//BUG: removed due + problem
            Regex nameReg = new Regex(@"_", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            result = nameReg.Replace(result, " ");
            return result;
        }

        public static string NormalizeNameCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return str;
        }
    }
}
