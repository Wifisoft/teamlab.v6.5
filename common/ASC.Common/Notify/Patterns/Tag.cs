using System;
using System.Diagnostics;

namespace ASC.Notify.Patterns
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class Tag : ITag
    {
        public string Name
        {
            get;
            private set;
        }


        public Tag(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");

            Name = name;
        }

        public override bool Equals(object obj)
        {
            var t = obj as ITag;
            return t != null && string.Equals(Name, t.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}