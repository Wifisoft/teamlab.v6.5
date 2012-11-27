using System;
using System.Text.RegularExpressions;

namespace ASC.Web.Controls.BBCodeParser
{
    public class RegularExpressionTemplate
    {
        public Regex RegExpression { get; private set; }

        public string Replacement { get; private set; }

        public RegularExpressionTemplate(Regex regExpression, string replacement)
        {
            this.RegExpression = regExpression;
            this.Replacement = replacement;
        }

        public string Parse(string text)
        {
            int start = 0;
            Match m = RegExpression.Match(text,start);
            while(m.Success)
            {
                text = text.Remove(m.Index,m.Length);
                string insertion = String.Format(Replacement,m.Value);
                text = text.Insert(m.Index,insertion);
                m = m.NextMatch();
            }

            return text;
        }

        public static RegularExpressionTemplate HTMLReferenceExpression
        {
            get
            {
                return new RegularExpressionTemplate(new Regex("((http|ftp|https|gopher|mailto|news|nntp|telnet)://){1}([0-9a-zA-Z]+[0-9a-zA-Z]+[0-9a-zA-Z\\-_]*\\.{0,1}[0-9a-zA-Z]+[0-9a-zA-Z/\\.{0,1}\\-_:]*){1}(\\?[0-9a-zA-Z;/?@&=+$\\.\\-_!~*'#()%]*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled),
                                                    "<a href=\"{0}\" target=_blank>{0}</a>");
            }
        }
    }
  
}
