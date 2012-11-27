using System.Web;
using System.Web.Mvc;

namespace ASC.Api.Web.Help.Helpers
{

    public static class HtmlExtensions
    {
        public static CssFileCollection Style(this HtmlHelper helper, string outurl)
        {
            return Style(helper, outurl, !HttpContext.Current.IsDebuggingEnabled);
        }

        public static CssFileCollection Style(this HtmlHelper helper, string outurl, bool combine)
        {
            return Style(helper, outurl, combine, 0);
        }

        public static CssFileCollection Style(this HtmlHelper helper, string outurl, int version)
        {
            return Style(helper, outurl, !HttpContext.Current.IsDebuggingEnabled, version);
        }

        public static CssFileCollection Style(this HtmlHelper helper, string outurl, bool combine, int version)
        {
            var url = ((Controller) helper.ViewContext.Controller).Url;
            return new CssFileCollection(outurl,helper,url,combine, version);
        }

        public static JsFileCollection Script(this HtmlHelper helper, string outurl, bool combine)
        {
            return Script(helper, outurl, combine, 0);
        }

        public static JsFileCollection Script(this HtmlHelper helper, string outurl)
        {
            return Script(helper, outurl, 0);
        }

        public static JsFileCollection Script(this HtmlHelper helper, string outurl, int version)
        {
            return Script(helper, outurl, !HttpContext.Current.IsDebuggingEnabled, version);
        }



        public static JsFileCollection Script(this HtmlHelper helper, string outurl, bool combine, int version)
        {
            var url = ((Controller)helper.ViewContext.Controller).Url;
            return new JsFileCollection(outurl, helper, url,combine,version);
        }
    }
}