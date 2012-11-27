using System;
using System.Diagnostics;

namespace ASC.Notify.Patterns
{
    [DebuggerDisplay("{Tag}: {Value}")]
    public class TagValue : ITagValue
    {
        public ITag Tag
        {
            get;
            private set;
        }

        public object Value
        {
            get;
            private set;
        }


        public TagValue(string tagName)
            : this(tagName, null)
        {
        }

        public TagValue(string tagName, object value)
            : this(new Tag(tagName), value)
        {
        }

        public TagValue(ITag tag, object value)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            Tag = tag;
            Value = value;
        }
    }
}