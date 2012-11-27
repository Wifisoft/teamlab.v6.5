using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.CJK;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;

namespace ASC.FullTextIndex.Service
{
    sealed class AnalyzersProvider
    {
        private static readonly Dictionary<string, string> acronyms = new Dictionary<string, string>
        {
            {"en", "English"},
            {"ru", "Russian"},
            {"fr", "French"},
            {"de", "German"},
            {"es", "Spanish"},
            {"it", "Italian"},
        };


        public Analyzer GetAnalyzer(string lang)
        {
            if (lang == "ko" || lang == "jp" || lang == "zh")
            {
                return new CJKAnalyzer();
            }
            return acronyms.ContainsKey(lang) ?
                (Analyzer)new SnowballAnalyzer(acronyms[lang]) :
                (Analyzer)new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
        }
    }
}
