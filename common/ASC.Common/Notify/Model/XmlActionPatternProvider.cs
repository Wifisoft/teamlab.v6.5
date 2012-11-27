using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using ASC.Notify.Engine;
using ASC.Notify.Patterns;
using log4net;
using Accord = System.Collections.Generic.KeyValuePair<string, ASC.Notify.Patterns.IPattern>;
using TernarAccord = System.Collections.Generic.KeyValuePair<ASC.Notify.Model.INotifyAction, System.Collections.Generic.KeyValuePair<string, ASC.Notify.Patterns.IPattern>>;

namespace ASC.Notify.Model
{
    public sealed class XmlActionPatternProvider : IActionPatternProvider
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Notify");
        private readonly IActionProvider _ActionProvider;
        private readonly IPatternProvider _PatternProvider;
        private ConstActionPatternProvider _ConstProvider;


        public XmlActionPatternProvider(string xmlPath, IActionProvider actionProvider, IPatternProvider patternProvider)
        {
            if (xmlPath == null) throw new ArgumentNullException("xmlPath");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");

            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            LoadXml(XmlReader.Create(xmlPath));
        }

        public XmlActionPatternProvider(Stream xmlStream, IActionProvider actionProvider, IPatternProvider patternProvider)
        {
            if (xmlStream == null) throw new ArgumentNullException("xmlStream");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");

            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            LoadXml(XmlReader.Create(xmlStream));
        }

        public XmlActionPatternProvider(Assembly assembly, string resourceName, IActionProvider actionProvider, IPatternProvider patternProvider)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");

            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            LoadXml(XmlReader.Create(stream));
        }

        public XmlActionPatternProvider(XmlReader xmlReader, IActionProvider actionProvider, IPatternProvider patternProvider)
        {
            if (xmlReader == null) throw new ArgumentNullException("xmlReader");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");

            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            LoadXml(xmlReader);
        }

        public IPattern GetPattern(INotifyAction action, string senderName)
        {
            return _ConstProvider.GetPattern(action, senderName);
        }

        public IPattern GetPattern(INotifyAction action)
        {
            return _ConstProvider.GetPattern(action);
        }

        public Func<INotifyAction, string, NotifyRequest, IPattern> GetPatternMethod
        {
            get;
            set;
        }


        private void LoadXml(XmlReader xmlReader)
        {
            var result = new List<TernarAccord>();
            var nav = new XPathDocument(xmlReader).CreateNavigator();
            var manager = new XmlNamespaceManager(nav.NameTable);
            manager.AddNamespace("act", "urn:asc.notify.action_pattern.xsd");
            var nodes = nav.Select("act:accordings/action", manager);
            while (nodes.MoveNext())
            {
                var actNav = nodes.Current;
                var actionID = actNav.GetAttribute("actionID", "");
                var action = _ActionProvider.GetAction(actionID);
                if (action == null)
                {
                    log.Error(String.Format("action with id=\"{0}\" not instanced", actionID));
                    continue;
                }

                var defaultActionPattern = actNav.GetAttribute("defaultPatternID", "");
                if (!String.IsNullOrEmpty(defaultActionPattern))
                {
                    var defaultPattern = _PatternProvider.GetPattern(defaultActionPattern);
                    if (defaultPattern == null)
                    {
                        log.Error(String.Format("pattern with id=\"{0}\" not instanced", defaultActionPattern)); continue;
                    }
                    result.Add(new TernarAccord(action, new Accord(null, defaultPattern)));
                }

                var sendersNodes = actNav.SelectChildren("sender", "");
                while (sendersNodes.MoveNext())
                {
                    var senderName = sendersNodes.Current.GetAttribute("senderName", "");
                    var patternID = sendersNodes.Current.GetAttribute("patternID", "");
                    var pattern = _PatternProvider.GetPattern(patternID);
                    if (pattern == null)
                    {
                        log.Error(String.Format("pattern with id=\"{0}\" not instanced", patternID));
                        continue;
                    }
                    result.Add(new TernarAccord(action, new Accord(senderName, pattern)));
                }
            }
            _ConstProvider = new ConstActionPatternProvider(result.ToArray());
        }
    }
}