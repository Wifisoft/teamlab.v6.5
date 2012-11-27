using System;
using System.Collections.Generic;

namespace ASC.Web.Controls.BBCodeParser
{
    #region internal class Tag
    internal class Tag : IToken
    {
        private string m_name;
        private string m_originalString;
        private bool m_isClosingTag;
        private int m_startPosition;
        private int m_endPosition;
        private List<TagParameter> m_parameters = null;

        #region IsSingle
        private bool m_isSingle;

        public bool IsSingle
        {
            get { return m_isSingle; }
            set { m_isSingle = value; }
        }

        #endregion
        #region Parameters
        public List<TagParameter> Parameters
        {
            get
            {
                return this.m_parameters;
            }
            set
            {
                this.m_parameters = value;
            }
        }
        #endregion
        #region Name
        public string Name
        {
            get
            {
                return this.m_name;
            }
            set
            {
                this.m_name = value;
            }
        }
        #endregion
        #region OriginalString
        public string OriginalString
        {
            get
            {
                return this.m_originalString;
            }

            set
            {
                this.m_originalString = value;
            }
        }
        #endregion
        #region IsClosingTag
        public bool IsClosingTag
        {
            get
            {
                return this.m_isClosingTag;
            }
            set
            {
                this.m_isClosingTag = value;
            }
        }
        #endregion

        #region StartPosition
        public int StartPosition
        {
            get
            {
                return this.m_startPosition;
            }
            set
            {
                this.m_startPosition = value;
            }
        }
        #endregion
        #region EndPosition
        public int EndPosition
        {
            get
            {
                return this.m_endPosition;
            }
            set
            {
                this.m_endPosition = value;
            }
        }
        #endregion

        public Tag(string name, bool isClosing)
        {
            this.m_name = name;
            this.m_isClosingTag = isClosing;
            this.IsSingle = false;
        }
        public Tag()
        {
            this.IsSingle = false;
        }

        public bool isDisable = false;
        public bool isCheck = false;
    }
    #endregion
    #region internal class TagParameter
    internal class TagParameter
    {
        private string m_key;
        private string m_value;
        public string Key
        {
            get
            {
                return m_key;
            }

            set
            {
                this.m_key = value;
            }
        }
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        public TagParameter(string key, string value)
        {
            this.m_key = key;
            this.m_value = value;
        }
    }
    #endregion

    internal struct AccordTag
    {
        public Tag StartTag;
        public Tag EndTag;
        public AccordTag(Tag startTag, Tag endTag)
        {
            this.StartTag = startTag;
            this.EndTag = endTag;
        }
    }
}