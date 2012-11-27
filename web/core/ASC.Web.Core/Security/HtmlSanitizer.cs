using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ASC.Web.Core.Security
{
    public class HtmlSanitizer
    {
        private static readonly Regex ScriptReplacer = new Regex(@"<\s*script[^>]*>(.*?)<\s*/\s*script>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Singleline);

        //BUG: regex bellow not correctly catch attrs like this onclick="javascript:alert('aaa')" it catches only a part of it: onclick="javascript:alert('. Will fix later
        private static readonly Regex AttrReplacer = new Regex(@"(\S+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly List<string> BlockedAttrs = new List<string>() 
        { 
            "onload", 
            "onunload", 
            "onclick", 
            "ondblclick", 
            "onmousedown", 
            "onmouseup", 
            "onmouseover", 
            "onmousemove", 
            "onmouseout", 
            "onfocus", 
            "onblur", 
            "onkeypress", 
            "onkeydown", 
            "onkeyup",
            "onsubmit", 
            "onreset", 
            "onselect", 
            "onchange"
        };


        public static string Sanitize(string html)
        {
            return ScriptReplacer.Replace(AttrReplacer.Replace(html, new MatchEvaluator(EvalAttributes)), "");
        }

        private static string EvalAttributes(Match match)
        {
            if (match.Success)
            {
                if (match.Groups[1].Success)
                {
                    if (BlockedAttrs.Contains(match.Groups[1].Value.ToLowerInvariant().Trim()))
                    {
                        return string.Empty;
                    }
                }
                if (match.Groups[2].Success && match.Groups[2].Value.StartsWith("javascript", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }
            }
            return match.Value;
        }
    }
}