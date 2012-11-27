using System;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Common.Notify.Patterns;
using ASC.Notify.Messages;

namespace ASC.Notify.Textile
{
    public class JabberStyler : IPatternStyler
    {
        static readonly Regex VelocityArguments = new Regex("nostyle(?<arg>.*?)/nostyle", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex LinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex TextileReplacer = new Regex(@"(h1\.|h2\.|\*|h3\.|p\.)", RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex BrReplacer = new Regex(@"<br\s*\/*>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Singleline);
        static readonly Regex TagReplacer = new Regex(@"<(.|\n)*?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Singleline);

        public void ApplyFormating(NoticeMessage message)
        {
            var body = string.Empty;
            if (!string.IsNullOrEmpty(message.Subject))
            {
                body += VelocityArguments.Replace(message.Subject, ArgMatchReplace) + Environment.NewLine;
                message.Subject = string.Empty;
            }
            if (string.IsNullOrEmpty(message.Body)) return;
            var lines = message.Body.Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.None);
            for (var i = 0; i < lines.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) { body += Environment.NewLine; continue; }
                lines[i] = VelocityArguments.Replace(lines[i], ArgMatchReplace);
                body += LinkReplacer.Replace(lines[i], EvalLink) + Environment.NewLine;
            }
            lines[lines.Length - 1] = VelocityArguments.Replace(lines[lines.Length - 1], ArgMatchReplace);
            body += LinkReplacer.Replace(lines[lines.Length - 1], EvalLink);
            body = TextileReplacer.Replace(HttpUtility.HtmlDecode(body), ""); //Kill textile markup
            body = BrReplacer.Replace(body, Environment.NewLine);
            body = TagReplacer.Replace(body, Environment.NewLine);
            message.Body = body;
        }

        private static string EvalLink(Match match)
        {
            if (match.Success)
            {
                if (match.Groups["text"].Success && match.Groups["link"].Success)
                {
                    if (match.Groups["text"].Value.Equals(match.Groups["link"].Value,StringComparison.OrdinalIgnoreCase))
                    {
                        return match.Groups["text"].Value;
                    }
                    return match.Groups["text"].Value + string.Format(" ( {0} )", match.Groups["link"].Value);
                }
            }
            return match.Value;
        }

        private static string ArgMatchReplace(Match match)
        {
            return match.Result("${arg}");
        }
    }
}
