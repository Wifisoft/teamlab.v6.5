using System;
using System.Web;

namespace ASC.FederatedLogin.Helpers
{
    public class XrdsHelper
    {
        private const string Xrds =
            @"<xrds:XRDS xmlns:xrds=""xri://$xrds"" xmlns:openid=""http://openid.net/xmlns/1.0"" " +
            @"xmlns=""xri://$xrd*($v*2.0)""><XRD><Service " +
            @"priority=""1""><Type>http://specs.openid.net/auth/2.0/return_to</Type><URI " +
            @"priority=""1"">{0}</URI></Service><Service><Type>http://specs.openid.net/extensions/ui/icon</Type><UR" +
            @"I>{1}</URI></Service></XRD></xrds:XRDS>";


        internal static void RenderXrds(HttpResponse responce, string location, string iconlink)
        {
            responce.Write(string.Format(Xrds,location,iconlink));
        }

        public static void AppendXrdsHeader()
        {
            AppendXrdsHeader("~/openidlogin.ashx");
        }

        public static void AppendXrdsHeader(string handlerVirtualPath)
        {
            HttpContext.Current.Response.AppendHeader(
                "X-XRDS-Location",
                new Uri(HttpContext.Current.Request.GetUrlRewriter(), HttpContext.Current.Response.ApplyAppPathModifier(handlerVirtualPath)).AbsoluteUri);
        }
    }
}