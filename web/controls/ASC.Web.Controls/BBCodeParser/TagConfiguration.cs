using System.Collections.Generic;

namespace ASC.Web.Controls.BBCodeParser
{ 
    public class TagConfiguration
    {
        public List<TagParamOption> ParamOptions{get; set;}
        
        public bool IsSingleTag{get; private set;}
        
        public string Tag{get; set;}
        
        public string Replacement{get; set;}

        public string AlternativeReplacement{get; set;}

        public bool IsParseContent{get; set;}

        public bool IsParseTextReplacement { get; set; }

        public bool IsParseTextReqularExpressions{ get; set; }
        
        #region Constructors
        
        public TagConfiguration(string tag, string replacement)
            : this(tag, replacement, null, true)
        {
        }
        
        public TagConfiguration(string tag, string replacement, string alternativeReplacement)
            : this(tag, replacement, alternativeReplacement, true)
        {
        }
       
        public TagConfiguration(string tag, string replacement, bool isParseContent)
            : this(tag, replacement, null, isParseContent)
        {
        }
       
        public TagConfiguration(string tag, string replacement, string alternativeReplacement, bool isParseContent)
        {
            this.IsParseTextReplacement = true;
            this.IsParseTextReqularExpressions = true;

            this.Tag = tag;
            this.Replacement = replacement;
            this.AlternativeReplacement = alternativeReplacement;
            this.IsParseContent = isParseContent;
            this.ParamOptions = new List<TagParamOption>(0);

            // If there is a '{0}' in the replacement string the tag cannot be a single tag.
            if (replacement.Contains("{0}"))
            {
                this.IsSingleTag = false;
            }
            else
            {
                this.IsSingleTag = true;
            }
        }
        #endregion
    }

}
