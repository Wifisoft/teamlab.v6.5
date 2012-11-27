using System.IO;
using System.Text.RegularExpressions;
using ASC.Common.Notify.Patterns;
using ASC.Notify.Messages;
using Textile;
using Textile.Blocks;

namespace ASC.Notify.Textile
{
    public class TextileStyler : IPatternStyler
    {
        private static readonly Regex VelocityArguments = new Regex("nostyle(?<arg>.*?)/nostyle", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        static TextileStyler()
        {
            BlockAttributesParser.Styler = new StyleReader(LoadResourceString("style.css"));
            Master = Resources.TemplateResource.HtmlMaster;
        }

        protected static string Master { get; set; }

        public void ApplyFormating(NoticeMessage message)
        {
            var output = new StringBuilderTextileFormatter();
            var formatter = new TextileFormatter(output);
            message.Subject = VelocityArguments.Replace(message.Subject, argMatchReplace);
            formatter.Format(message.Body);
            message.Body = Master.Replace("%CONTENT%", output.GetFormattedText());
        }

        internal static Stream LoadResource(string name)
        {
            return typeof(TextileStyler).Assembly.GetManifestResourceStream(string.Format("ASC.Notify.Textile.Resources.{0}",name));
        }

        internal static string LoadResourceString(string name)
        {
            using (var reader = new StreamReader(LoadResource(name)))
            {
                return reader.ReadToEnd();
            }
        }

        private static string argMatchReplace(Match match)
        {
            return match.Result("${arg}");
        }
    }
}