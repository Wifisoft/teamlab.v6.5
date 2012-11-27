using System;
using System.Web;

namespace ASC.Data.Storage
{
    public static class SecureHelper
    {
        public static bool IsSecure()
        {
            return HttpContext.Current != null && Uri.UriSchemeHttps.Equals(HttpContext.Current.Request.GetUrlRewriter().Scheme, StringComparison.OrdinalIgnoreCase);
        }
    }
}