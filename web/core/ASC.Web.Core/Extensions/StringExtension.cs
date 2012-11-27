using System.Text.RegularExpressions;
using System.Web;

namespace System
{
    public static class StringExtension
    {
        private static readonly Regex reStrict = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");


        public static string HtmlEncode(this string str)
        {
            return !string.IsNullOrEmpty(str) ? HttpUtility.HtmlEncode(str) : str;
        }

        /// <summary>
        /// Replace ' on ′
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSingleQuote(this string str)
        {
            return str == null ? null : str.Replace("'", "′");
        }

        public static bool TestEmailRegex(this string emailAddress)
        {
            return !string.IsNullOrEmpty(emailAddress) ? reStrict.IsMatch(emailAddress) : false;
        }
    }
}
