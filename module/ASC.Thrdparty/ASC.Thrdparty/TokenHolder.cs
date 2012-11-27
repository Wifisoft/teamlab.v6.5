using System;
using System.Web;

namespace ASC.Thrdparty
{
    public class TokenHolder
    {
        private static void EnsureHttpSession()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                throw new InvalidOperationException(
                    "This operation can be run only in Http context and with Http session");
            }
        }

        public static string GetToken(string tokenKey)
        {
            EnsureHttpSession();
            return HttpContext.Current.Session[tokenKey] as string;
        }

        public static void AddToken(string tokenKey, string tokenvalue)
        {
            EnsureHttpSession();
            HttpContext.Current.Session[tokenKey]=tokenvalue;
        }
    }
}