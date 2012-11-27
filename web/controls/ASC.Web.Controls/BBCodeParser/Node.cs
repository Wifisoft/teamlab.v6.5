using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ASC.Web.Controls.BBCodeParser
{
    internal interface IToken
    {
    }
    #region internal class Text
    internal class Text : IToken
    {
        private string _content;       
        public string Content
        {
            get
            {
                return this._content;
            }

            set
            {
                this._content = value;
            }
        }
       
        public Text(string content)         
        {
            this._content = content;
        }
    }
    #endregion
   
    internal class Node : IToken
    {        
        private List<IToken> _tokens;
        private Node _parent;

        #region Tokens
        public List<IToken> Tokens
        {
            get
            {
                return this._tokens;
            }
        } 
        #endregion
        #region Parent
        public Node Parent
        {
            get
            {
                return this._parent;
            }
        } 
        #endregion

        #region Constructors
        public Node()
        {
            this._tokens = new List<IToken>();
        }

        public Node(Node parent)
            : this()
        {
            this._parent = parent;
        } 
        #endregion

        public string ToString(ParserConfiguration configuration)
        {
            StringBuilder text = new StringBuilder();

            bool isParseTextReplacement = true;
            bool isParseTextReqularExpressions = true;
            
            int start = 0;
            int end = this._tokens.Count;
            if (this._tokens[start] is Tag)
            {
                isParseTextReplacement = configuration.GetTagConfiguration((this._tokens[start] as Tag).Name).IsParseTextReplacement;
                isParseTextReqularExpressions = configuration.GetTagConfiguration((this._tokens[start] as Tag).Name).IsParseTextReqularExpressions;
                start++;
                
            }
            if (this._tokens[end - 1] is Tag)
            {
                end--;
            }

            for (int i = start; i < end; i++)
            {                
                IToken token = this._tokens[i];            
                if (token is Text)
                {
                    string tempText = ((Text)token).Content;
                    if (isParseTextReqularExpressions)
                        tempText = ParseRegularExpressions(tempText, configuration);

                    if (!isParseTextReqularExpressions && configuration.IsHTMLEncode)
                        tempText = System.Web.HttpUtility.HtmlEncode(tempText);

                    if (isParseTextReplacement)
                    {
                        foreach (ExpressionReplacement expressionReplacement in configuration.ExpressionReplacements)
                        {
                            tempText = tempText.Replace(expressionReplacement.Expression, expressionReplacement.Replacement);
                        }
                    }

                    text.Append(tempText);
                }
                else if (token is Node)
                {
                    Tag tag = (Tag)((Node)token).Tokens[0];
                    TagConfiguration tagConfig = configuration.GetTagConfiguration(tag.Name);
                    string template = tagConfig.Replacement;
                    string alternativeTemplate = tagConfig.AlternativeReplacement;
                    string innerText;

                    if (tagConfig.IsParseContent)                    
                        innerText = ((Node)token).ToString(configuration);                    
                    else                                           
                        innerText = ((Node)token).ToString(true, tagConfig.IsParseTextReplacement, tagConfig.IsParseTextReqularExpressions, configuration);
                    
                    string outerText = "";
                    if (tag.Parameters != null)
                    {
                        #region replace tag value
                        string[] parameters = new string[tag.Parameters.Count + 1];
                        parameters[0] = innerText;
                        foreach (TagParamOption tpo in tagConfig.ParamOptions)
                        {
                            if (tpo.ParamNumber == 0)
                            {
                                if (String.IsNullOrEmpty(parameters[0]))
                                {
                                    parameters[0] = tpo.DefaultValue;
                                }
                                if (parameters[0].IndexOf(tpo.PreValue, StringComparison.InvariantCultureIgnoreCase) != 0)
                                {
                                    parameters[0] = tpo.PreValue + parameters[0];
                                }
                                break;
                            }
                        }

                        for (int j = 1; j < parameters.Length; j++)
                        {
                            parameters[j] = tag.Parameters[j - 1].Value;
                            foreach (TagParamOption tpo in tagConfig.ParamOptions)
                            {
                                if (tpo.ParamNumber == j)
                                {
                                    if (String.IsNullOrEmpty(parameters[j]))
                                    {
                                        parameters[j] = tpo.DefaultValue;
                                    }
                                    if (parameters[j].IndexOf(tpo.PreValue, StringComparison.InvariantCultureIgnoreCase) != 0)
                                    {
                                        parameters[j] = tpo.PreValue + parameters[j];
                                    }
                                    break;
                                }
                            }
                        }
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            foreach (TagParamOption tpo in tagConfig.ParamOptions)
                            {
                                if (tpo.ParamNumber == j && tpo.IsUseAnotherParamValue && String.Compare(parameters[j], tpo.PreValue + tpo.DefaultValue, StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    parameters[j] = parameters[tpo.AnotherParamNumber];
                                    if (parameters[j].IndexOf(tpo.PreValue, StringComparison.InvariantCultureIgnoreCase) != 0)
                                    {
                                        parameters[j] = tpo.PreValue + parameters[j];
                                    }
                                    break;
                                }
                            }
                        }
                        try
                        {
                            outerText = String.Format(template, parameters);
                        }
                        catch (FormatException)
                        {
                            try
                            {
                                outerText = String.Format(alternativeTemplate, parameters);
                            }
                            catch
                            {
                                outerText = tag.OriginalString + innerText;
                            }
                        } 
                        #endregion

                    }
                    else
                    {
                        try
                        {
                            outerText = String.Format(template, innerText);
                        }
                        catch (FormatException)
                        {
                            try
                            {
                                outerText = String.Format(alternativeTemplate, innerText);
                            }
                            catch
                            {
                                outerText = tag.OriginalString + innerText;
                            }
                        }
                    }
                    text.Append(outerText);
                }
            }            
            return text.ToString();
        }

        private string ParseRegularExpressions(string text, ParserConfiguration configuration)
        {
            string result = text;
            foreach (RegularExpressionTemplate regExpTemplate in configuration.RegExpTemplates)
            {
                StringBuilder sb = new StringBuilder();
                int startSearchIndex = 0;

                Match m = regExpTemplate.RegExpression.Match(text);
                while (m.Success)
                {
                    if (configuration.IsHTMLEncode)
                        sb.Append(System.Web.HttpUtility.HtmlEncode(text.Substring(startSearchIndex, m.Index - startSearchIndex)));
                    else
                        sb.Append(text.Substring(startSearchIndex, m.Index - startSearchIndex));

                    sb.Append(String.Format(regExpTemplate.Replacement, m.Value));

                    startSearchIndex += (m.Index + m.Length) - startSearchIndex;
                    m = m.NextMatch();
                }

                if (configuration.IsHTMLEncode)
                    sb.Append(System.Web.HttpUtility.HtmlEncode(text.Substring(startSearchIndex)));
                else
                    sb.Append(text.Substring(startSearchIndex));

                return sb.ToString();
            }

            return result;
        }


        #region ToString(bool ignoreContainerTags)
        private string ToString(bool ignoreContainerTags, bool isParseTextReplacement, bool isParseTextRegularExpressions, ParserConfiguration configuration)
        {
            StringBuilder text = new StringBuilder();            
            int start = ignoreContainerTags ? 1 : 0;
            int end = ignoreContainerTags ? this._tokens.Count - 1 : this._tokens.Count;
            
            for (int i = start; i < end; i++)
            {                
                IToken token = this._tokens[i];
                if (token is Text)
                {
                    string tempText = ((Text)token).Content;

                    if (isParseTextRegularExpressions)                    
                        tempText = ParseRegularExpressions(tempText, configuration);                    

                    if (!isParseTextRegularExpressions && configuration.IsHTMLEncode)
                        tempText = System.Web.HttpUtility.HtmlEncode(tempText);

                    if (isParseTextReplacement)
                    {
                        foreach (ExpressionReplacement expressionReplacement in configuration.ExpressionReplacements)
                        {
                            tempText = tempText.Replace(expressionReplacement.Expression, expressionReplacement.Replacement);
                        }
                    }
                    text.Append(tempText);                    
                }
                else if (token is Tag)
                {
                    string tempText = ((Tag)token).OriginalString;

                    if (isParseTextRegularExpressions)
                        tempText = ParseRegularExpressions(tempText, configuration);

                    if (!isParseTextRegularExpressions && configuration.IsHTMLEncode)
                        tempText = System.Web.HttpUtility.HtmlEncode(tempText);

                    
                    if (isParseTextReplacement)
                    {
                        foreach (ExpressionReplacement expressionReplacement in configuration.ExpressionReplacements)
                        {
                            tempText = tempText.Replace(expressionReplacement.Expression, expressionReplacement.Replacement);
                        }
                    }
                    text.Append(tempText); 
                }
                else if (token is Node)
                {
                    text.Append(((Node)token).ToString(false, isParseTextReplacement, isParseTextRegularExpressions,configuration));
                }
            }
            return text.ToString();
        }
        #endregion
    }
}