using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ASC.Web.Controls.BBCodeParser
{ 
    public class Parser
    {
        public ParserConfiguration Configuration{get; set;}

        public Parser(ParserConfiguration configuration)
        {
            Configuration = configuration;
        }

        private struct TagPosition
        {
            public int Numb;
            public bool IsEnd;
            public TagPosition(int numb, bool isEnd)
            {
                Numb = numb;
                IsEnd = isEnd;
            }
        }

        public string Parse(string text)
        {
            List<Tag> startTags = new List<Tag>(0);
            List<Tag> endTags = new List<Tag>(0);
            List<AccordTag> Accord = new List<AccordTag>(0);

            #region BeginTags
            
            string pattern = "\\[(?<name>\\w{1,})(=\\s*(?<first_param_val>(([^\\s\\]\\[=]*)|(\"[^\"]*\"))))?(?<parameters>(\\s{1,}\\w{1,}\\s*=\\s*((\"[^\"]*\")|([^\\s\\]\\[=]*)))*)?\\]";

            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = re.Match(text, 0);

            while (match.Success)
            {        
                Tag tag = new Tag();
                tag.IsClosingTag = false;
                tag.StartPosition= match.Index;
                tag.EndPosition = tag.StartPosition +match.Length;
                tag.Name = match.Groups["name"].Value;
                tag.OriginalString = match.Value;

                List<TagParameter> parameters = new List<TagParameter>(0);
                string firstParam = match.Groups["first_param_val"].Value;
                if (firstParam != null)
                {
                    
                    parameters.Add(new TagParameter("",firstParam.Trim('"')));
                }
                string paramString = match.Groups["parameters"].Value;                
                if (paramString != null && paramString != "")
                {                                        
                    string patternParams = "(?<param_name>\\w{1,})\\s*=\\s*(?<param_value>((\"[^\"]*\")|([^\\s\\]\\[=]*)))";
                    Regex reParams = new Regex(patternParams, RegexOptions.IgnoreCase);
                    Match matchParams = reParams.Match(paramString, 0);
                    while (matchParams.Success)
                    {
                        string key = matchParams.Groups["param_name"].Value;
                        string value = matchParams.Groups["param_value"].Value.Trim('"');                        
                        
                        parameters.Add(new TagParameter(key, value));
                        matchParams = matchParams.NextMatch();                        
                    }
                }
                tag.Parameters = parameters;
                foreach (TagConfiguration tagConf in Configuration.TagConfigurations)
                {
                    if (tagConf.Tag.ToLower() == tag.Name.ToLower())
                    {
                        if (tagConf.IsSingleTag)
                        {
                            tag.IsSingle = true;
                            Accord.Add(new AccordTag(tag,tag));
                        }
                        
                        startTags.Add(tag);
                        break;
                    }
                }
                match = match.NextMatch();
            }
           
            #endregion            
            #region EndTags
            
            pattern = "\\[/(?<name>\\w{1,})\\]";
            re = new Regex(pattern, RegexOptions.IgnoreCase);
            match = re.Match(text, 0);
            while (match.Success)
            {
                Tag tag = new Tag(match.Groups["name"].Value,true);                
                tag.StartPosition = match.Index;
                tag.EndPosition = tag.StartPosition + match.Length;                
                tag.OriginalString = match.Value;
                foreach (TagConfiguration tagConf in Configuration.TagConfigurations)
                {
                    if (tagConf.Tag.ToLower() == tag.Name.ToLower() && !tagConf.IsSingleTag)
                    {
                        endTags.Add(tag);
                        break;
                    }
                }
                match = match.NextMatch();
            }
            #endregion           
           
            int s = 0;
            int e = 0;

            while (e < endTags.Count)
            {
                Tag tag_end = endTags[e];
                if (s >= startTags.Count)
                {
                    for (int j = startTags.Count - 1; j >= 0; j--)
                    {
                        Tag internal_tag_start = startTags[j];
                        if (internal_tag_start.EndPosition > tag_end.StartPosition)
                        {
                            continue;
                        }
                        else if (!tag_end.isDisable
                                && !internal_tag_start.isDisable
                                && internal_tag_start.Name == tag_end.Name
                                && !tag_end.isCheck
                                && !internal_tag_start.isCheck)
                        {
                            internal_tag_start.isCheck = true;
                            tag_end.isCheck = true;

                            Accord.Add(new AccordTag(internal_tag_start, tag_end));

                            
                            for (int k = j + 1; k < startTags.Count; k++)
                            {
                                Tag tag_disable = startTags[k];
                                if (tag_disable.EndPosition <= tag_end.StartPosition)
                                    tag_disable.isDisable = true;
                                else
                                    break;
                            }
                            for (int k = e - 1; k >= 0; k--)
                            {
                                Tag tag_disable = endTags[k];
                                if (internal_tag_start.EndPosition <= tag_disable.StartPosition)
                                    tag_disable.isDisable = true;
                                else
                                    break;
                            }
                            break;
                        }
                    }
                    e++;
                    continue;
                }

                Tag tag_start = startTags[s];

                if (!tag_start.isDisable 
                    && !tag_end.isDisable 
                    && !tag_start.isCheck 
                    && !tag_end.isCheck 
                    && tag_start.Name == tag_end.Name 
                    && tag_start.EndPosition<= tag_end.StartPosition)
                {
                    for (int i = startTags.Count - 1; i >= 0; i--)
                    {
                        Tag tag_st = startTags[i];
                        if (!tag_st.isDisable 
                            && !tag_st.isCheck 
                            && tag_st.Name == tag_end.Name 
                            && tag_st.EndPosition<= tag_end.StartPosition)
                        {
                            tag_st.isCheck = true;
                            tag_end.isCheck = true;
                            s = i;

                            Accord.Add(new AccordTag(tag_st, tag_end));
                           
                            for (int k = s + 1; k < startTags.Count; k++)
                            {
                                Tag tag_disable = startTags[k];
                                if (tag_disable.EndPosition<= tag_end.StartPosition)
                                    tag_disable.isDisable = true;
                                else
                                    break;
                            }
                            for (int k = e - 1; k >= 0; k--)
                            {
                                Tag tag_disable = endTags[k];
                                if (tag_start.EndPosition<= tag_disable.StartPosition)
                                    tag_disable.isDisable = true;
                                else
                                    break;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = startTags.Count - 1; j >= 0; j--)
                    {
                        Tag internal_tag_start = startTags[j];
                        if (internal_tag_start.EndPosition > tag_end.StartPosition)
                        {
                            continue;
                        }
                        else if (!tag_end.isDisable 
                                && !internal_tag_start.isDisable 
                                && internal_tag_start.Name == tag_end.Name 
                                && !tag_end.isCheck 
                                && !internal_tag_start.isCheck)
                        {
                            internal_tag_start.isCheck = true;
                            tag_end.isCheck = true;

                            Accord.Add(new AccordTag(internal_tag_start, tag_end));
                           
                            for (int k = j + 1; k < startTags.Count; k++)
                            {
                                Tag tag_disable = startTags[k];
                                if (tag_disable.EndPosition<= tag_end.StartPosition)
                                    tag_disable.isDisable = true;
                                else
                                    break;
                            }
                            for (int k = e - 1; k >= 0; k--)
                            {
                                Tag tag_disable = endTags[k];
                                if (internal_tag_start.EndPosition<= tag_disable.StartPosition)
                                    tag_disable.isDisable = true;
                                else
                                    break;
                            }
                            break;
                        }
                    }
                }
                s++;
                e++;
            }
            
            #region sort
            for (int i = 0; i < Accord.Count; i++)
            {                
                for (int k = i + 1; k < Accord.Count; k++)
                {                    
                    if (Accord[k].StartTag.StartPosition < Accord[i].StartTag.StartPosition)
                    {
                        AccordTag buffer = Accord[i];
                        Accord[i] = Accord[k];
                        Accord[k] = buffer;
                    }
                }
            } 
            #endregion            

            Node root = new Node();
            #region tree
            Node currentNode = root;
            if (Accord.Count > 0)
            {
                List<AccordTag> parentsAccord = new List<AccordTag>(0);
                AccordTag curAccord = Accord[0];

                int curPos = 0;
                int curParent = -1;
                for (int i = 0; i < Accord.Count; i++)
                {
                    curAccord = Accord[i];
                    
                    if (parentsAccord.Count == 0)
                    {
                        currentNode.Tokens.Add(new Text(text.Substring(curPos, curAccord.StartTag.StartPosition - curPos)));

                        Node childNode = new Node(currentNode);
                        currentNode.Tokens.Add(childNode);
                        currentNode = childNode;
                        currentNode.Tokens.Add(curAccord.StartTag);

                        curPos = curAccord.StartTag.EndPosition;
                        parentsAccord.Add(curAccord);
                        curParent++;
                    }
                    
                    else if (curAccord.StartTag.StartPosition >= parentsAccord[curParent].StartTag.EndPosition
                        && curAccord.EndTag.EndPosition <= parentsAccord[curParent].EndTag.StartPosition)
                    {
                        AccordTag parentAccord = parentsAccord[curParent];
                        currentNode.Tokens.Add(new Text(text.Substring(curPos, curAccord.StartTag.StartPosition - curPos)));

                        Node childNode = new Node(currentNode);
                        currentNode.Tokens.Add(childNode);
                        currentNode = childNode;
                        currentNode.Tokens.Add(curAccord.StartTag);

                        curPos = curAccord.StartTag.EndPosition;
                        parentsAccord.Add(curAccord);
                        curParent++;
                    }
                    else if (curAccord.StartTag.StartPosition >= parentsAccord[curParent].EndTag.EndPosition)
                    {
                        AccordTag parentAccord = parentsAccord[curParent];
                        for (int j = curParent; j >= 0; j--)
                        {
                            parentAccord = parentsAccord[j];
                            if (parentAccord.StartTag != parentAccord.EndTag)
                            {
                                currentNode.Tokens.Add(new Text(text.Substring(curPos, parentAccord.EndTag.StartPosition - curPos)));
                                currentNode.Tokens.Add(parentAccord.EndTag);
                            }
                            currentNode = currentNode.Parent;
                            curParent--;
                            parentsAccord.RemoveAt(j);

                            curPos = parentAccord.EndTag.EndPosition;
                            if ((j - 1) >= 0)
                            {
                                AccordTag preParentAccord = parentsAccord[j - 1];
                                if (preParentAccord.EndTag.EndPosition <= curAccord.StartTag.StartPosition)
                                {
                                    currentNode.Tokens.Add(new Text(text.Substring(curPos, preParentAccord.EndTag.StartPosition - curPos)));
                                    curPos = preParentAccord.EndTag.StartPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        currentNode.Tokens.Add(new Text(text.Substring(parentAccord.EndTag.EndPosition, curAccord.StartTag.StartPosition - parentAccord.EndTag.EndPosition)));

                        Node childNode = new Node(currentNode);
                        currentNode.Tokens.Add(childNode);
                        currentNode = childNode;
                        currentNode.Tokens.Add(curAccord.StartTag);

                        curPos = curAccord.StartTag.EndPosition;
                        parentsAccord.Add(curAccord);
                        curParent++;

                    }
                }
                if (curAccord.StartTag != curAccord.EndTag)
                    currentNode.Tokens.Add(new Text(text.Substring(curAccord.StartTag.EndPosition, curAccord.EndTag.StartPosition - curAccord.StartTag.EndPosition)));

                for (int j = parentsAccord.Count - 1; j >= 0; j--)
                {
                    AccordTag parentAccord = parentsAccord[j];
                    if (parentAccord.StartTag != parentAccord.EndTag)
                        currentNode.Tokens.Add(parentAccord.EndTag);

                    currentNode = currentNode.Parent;

                    curPos = parentAccord.EndTag.EndPosition;
                    if ((j - 1) >= 0)
                    {
                        AccordTag preParentAccord = parentsAccord[j - 1];
                        currentNode.Tokens.Add(new Text(text.Substring(parentAccord.EndTag.EndPosition, preParentAccord.EndTag.StartPosition - parentAccord.EndTag.EndPosition)));
                    }
                }
                currentNode.Tokens.Add(new Text(text.Substring(curPos)));
            }
            else
                root.Tokens.Add(new Text(text)); 
            #endregion

            return root.ToString(Configuration);
        }       
    }
}