using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace ASC.Notify.Patterns
{
    public class XmlPatternProvider : IPatternProvider
    {
        private ConstPatternProvider constProvider;
        private static readonly Regex resPattern = new Regex(@"\|(?<key>.+)\|(?<namespace>.+),(?<assembly>.+)", RegexOptions.Compiled);
        private static readonly Dictionary<string, ResourceManager> resMans = new Dictionary<string, ResourceManager>();


        public static Func<string, Assembly, ResourceManager> CreateResourceManager
        {
            get;
            set;
        }


        static XmlPatternProvider()
        {
            CreateResourceManager = (n, a) => new ResourceManager(n, a);
        }

        public XmlPatternProvider(string xmlContent)
        {
            if (xmlContent == null) throw new ArgumentNullException("xmlContent");
            LoadXml(XmlReader.Create(new StringReader(xmlContent)));
        }

        public XmlPatternProvider(Stream xmlStream)
        {
            if (xmlStream == null) throw new ArgumentNullException("xmlStream");
            LoadXml(XmlReader.Create(xmlStream));
        }

        public XmlPatternProvider(Assembly assembly, string resourceName)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            LoadXml(XmlReader.Create(stream));
        }

        public XmlPatternProvider(XmlReader xmlReader)
        {
            if (xmlReader == null) throw new ArgumentNullException("xmlReader");
            LoadXml(xmlReader);
        }

        public IPattern GetPattern(string id)
        {
            return constProvider.GetPattern(id);
        }

        public IPattern[] GetPatterns()
        {
            return constProvider.GetPatterns();
        }

        public IPatternFormatter GetFormatter(IPattern pattern)
        {
            return constProvider.GetFormatter(pattern);
        }


        private void LoadXml(XmlReader xmlReader)
        {
            XPathNavigator nav = new XPathDocument(xmlReader).CreateNavigator();
            var manager = new XmlNamespaceManager(nav.NameTable);
            manager.AddNamespace("pt", "urn:asc.notify.pattern.xsd");
            var result = new List<KeyValuePair<IPattern, IPatternFormatter>>();
            XPathNodeIterator nodes = nav.Select("pt:catalog/block", manager);
            while (nodes.MoveNext())
            {
                XPathNavigator blockNav = nodes.Current;
                string blockContentType = blockNav.GetAttribute("contentType", "");
                XPathNodeIterator formatterNode = blockNav.SelectChildren("formatter", "");
                string formatterTypeName = "";
                if (formatterNode.MoveNext()) formatterTypeName = formatterNode.Current.GetAttribute("type", "");
                Type formatterType = Type.GetType(formatterTypeName, true);
                var formatter = (IPatternFormatter)formatterType.Assembly.CreateInstance(formatterType.FullName);

                XPathNodeIterator patternsNodes = nodes.Current.Select("./patterns/pattern", manager);
                while (patternsNodes.MoveNext())
                {
                    XPathNodeIterator subjectNode = patternsNodes.Current.SelectChildren("subject", "");
                    string subject = "";
                    if (subjectNode.MoveNext())
                    {
                        subject = GetValue(subjectNode);
                    }
                    XPathNodeIterator bodyNode = patternsNodes.Current.SelectChildren("body", "");
                    string body = "";
                    string bodyStyler = "";
                    if (bodyNode.MoveNext())
                    {
                        body = GetValue(bodyNode);
                        if (bodyNode.Current != null)
                        {
                            bodyStyler = bodyNode.Current.GetAttribute("styler", "");
                        }
                    }
                    string patternContentType = patternsNodes.Current.GetAttribute("contentType", "");
                    if (String.IsNullOrEmpty(patternContentType))
                        patternContentType = blockContentType;
                    IPattern newpattern = new Pattern(
                        patternsNodes.Current.GetAttribute("id", ""),
                        patternsNodes.Current.GetAttribute("name", ""),
                        subject,
                        body,
                        patternContentType
                        ) { Styler = bodyStyler };
                    result.Add(new KeyValuePair<IPattern, IPatternFormatter>(newpattern, formatter));
                }
            }

            constProvider = new ConstPatternProvider(result.ToArray());
        }

        private string GetValue(XPathNodeIterator node)
        {
            var v = string.Empty;
            if (node.Current != null)
            {
                var resource = node.Current.GetAttribute("resource", "");
                if (!string.IsNullOrEmpty(resource))
                {
                    var match = resPattern.Match(resource);
                    //Assume that string will be in form of |key|namespace,Assembly
                    //Try load resource
                    if (match.Success)
                    {
                        var key = match.Groups["namespace"].Value + ":" + match.Groups["assembly"].Value;
                        if (!resMans.ContainsKey(key))
                        {
                            var resMan = CreateResourceManager(match.Groups["namespace"].Value, Assembly.Load(match.Groups["assembly"].Value));
                            resMans.Add(key, resMan);
                        }
                        v = resMans[key].GetString(match.Groups["key"].Value);
                    }
                }
                else
                {
                    v = node.Current.Value;
                }
            }
            return v;
        }
    }
}