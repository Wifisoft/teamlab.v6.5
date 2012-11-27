﻿using System;
using System.IO;
using ASC.Core.Tenants;
using ASC.FullTextIndex.Service.Config;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace ASC.FullTextIndex.Service
{
    class TextSearcher
    {
        private readonly string module;
        private readonly string path;


        public TextSearcher(string module, string path)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            this.module = module;
            this.path = path;
        }

        public TextSearchResult Search(string query, Tenant tenant)
        {
            var result = new TextSearchResult(module);

            if (string.IsNullOrEmpty(query) || !Directory.Exists(path))
            {
                return result;
            }

            var dir = Lucene.Net.Store.FSDirectory.Open(new DirectoryInfo(path));
            var searcher = new IndexSearcher(dir, false);
            try
            {
                var analyzer = new AnalyzersProvider().GetAnalyzer(tenant.GetCulture().TwoLetterISOLanguageName);
                var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Text", analyzer);
                parser.SetDefaultOperator(QueryParser.Operator.AND);
                if (TextIndexCfg.MaxQueryLength < query.Length)
                {
                    query = query.Substring(0, TextIndexCfg.MaxQueryLength);
                }
                Query q = null;
                try
                {
                    q = parser.Parse(query);
                }
                catch (Lucene.Net.QueryParsers.ParseException) { }
                if (q == null)
                {
                    q = parser.Parse(QueryParser.Escape(query));
                }

#pragma warning disable 618
                var hits = searcher.Search(q);
#pragma warning restore 618
                for (int i = 0; i < hits.Length(); i++)
                {
                    var doc = hits.Doc(i);
                    result.AddIdentifier(doc.Get("Id"));
                }
            }
            finally
            {
                searcher.Close();
                dir.Close();
            }
            return result;
        }
    }
}
