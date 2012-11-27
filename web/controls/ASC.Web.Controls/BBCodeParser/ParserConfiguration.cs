using System;
using System.Collections.Generic;
using System.Xml;

namespace ASC.Web.Controls.BBCodeParser
{
    public class ParserConfiguration
    {
        public bool IsHTMLEncode { get; set; }

        public List<TagConfiguration> TagConfigurations { get; private set; }

        public List<ExpressionReplacement> ExpressionReplacements { get; private set; }

        public List<RegularExpressionTemplate> RegExpTemplates { get; private set; }


        public TagConfiguration GetTagConfiguration(string tag)
        {
            foreach (TagConfiguration tagConfiguration in this.TagConfigurations)
            {
                if (tagConfiguration.Tag.ToLower() == tag.ToLower())
                {
                    return tagConfiguration;
                }
            }
            return null;

        }

        #region Construtors
        public ParserConfiguration()
            : this(null, true)
        {
        }

        public ParserConfiguration(bool isHTMLEncode)
            : this(null, isHTMLEncode)
        {
        }

        
        public ParserConfiguration(string configurationFile)
            : this(configurationFile, true)
        {
        }

        public ParserConfiguration(string configurationFile, bool isHTMLEncode)
        {
            this.IsHTMLEncode = isHTMLEncode;
            this.TagConfigurations = new List<TagConfiguration>();
            this.ExpressionReplacements = new List<ExpressionReplacement>();
            this.RegExpTemplates = new List<RegularExpressionTemplate>();

            if (!String.IsNullOrEmpty(configurationFile))
            {
                this.LoadConfigurationFromXml(configurationFile);
            }
        }
        #endregion


        public void LoadConfigurationFromXml(string configurationFile)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(configurationFile);

            XmlNodeList nodes = xmlDocument.SelectNodes("/configuration/parser/expressionReplacements/expressionReplacement");
            foreach (XmlNode node in nodes)
            {
                // Get the expression,
                string expression = node.SelectSingleNode("@expression").Value;
                expression = expression.Replace("\\n", "\n");

                // Get the replacement.
                string replacement = node.SelectSingleNode("@replacement").Value;
                replacement = replacement.Replace("\\n", "\n");

                // Build the expression replacement.
                ExpressionReplacement expressionReplacement =
                    new ExpressionReplacement(expression, replacement);
                this.ExpressionReplacements.Add(expressionReplacement);
            }

            nodes = xmlDocument.SelectNodes("/configuration/tags/tag");
            foreach (XmlNode node in nodes)
            {
                string tag = node.SelectSingleNode("@name").InnerText;
                string replacement = node.SelectSingleNode("@replacement").InnerText;
                string alternativeReplacement = node.SelectSingleNode("@alternativeReplacement") != null ? node.SelectSingleNode("@alternativeReplacement").InnerText : null;
                try
                {
                    bool parseContent = Boolean.Parse(node.SelectSingleNode("@parseContent").InnerText);
                    this.TagConfigurations.Add(new TagConfiguration(tag, replacement, alternativeReplacement, parseContent));
                }
                catch (NullReferenceException)
                {
                    this.TagConfigurations.Add(new TagConfiguration(tag, replacement, alternativeReplacement));
                }
            }
        }

    }
}
