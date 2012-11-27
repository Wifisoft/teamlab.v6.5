using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ASC.Api.Web.Help.Helpers
{
    public static class Highliter
    {
        private static readonly Regex Highlite=new Regex(@"\{([a-z0-9]+)\}", RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.Singleline);

        public static MvcHtmlString HighliteParams(string address)
        {
            var replace = Highlite.Replace(address, "<span class=\"bracket\">{</span><a href=\"#api-param-$1\" class=\"param-name\">$1</a><span class=\"bracket\">}</span>");
            return MvcHtmlString.Create(replace);
        }

        public static MvcHtmlString HighliteString(string toHighlite, string what)
        {
            if (string.IsNullOrEmpty(what) || string.IsNullOrEmpty(toHighlite))
                return MvcHtmlString.Create(toHighlite);

            var words = Regex.Split(what, @"\W+").Where(x => !string.IsNullOrEmpty(x));
            return
                MvcHtmlString.Create(Regex.Replace(toHighlite, "(" +string.Join("|",words.Select(x=>Regex.Escape(x)).ToArray()) + ")",
                                                   "<span class=\"label notice\">$1</span>", RegexOptions.IgnoreCase | RegexOptions.Singleline));
        }

        public static MvcHtmlString HighliteSearchString(string value)
        {
            return MvcHtmlString.Create(value);
        }
    }
}