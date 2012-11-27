using System;
using System.Text.RegularExpressions;

namespace ASC.Api.Utils
{
    public class PathUtils
    {
        private static readonly Regex _domainReplaceRegex = new Regex(@".+\|(?'domainData'[a-zA-Z])\|.+");

        public static string GetPath(string path)
        {
            return _domainReplaceRegex.Replace(path, new MatchEvaluator(EvalAppDomainData));
        }

        private static string EvalAppDomainData(Match match)
        {
            return match.Value;
        }
    }
}