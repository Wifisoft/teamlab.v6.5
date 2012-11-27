using System;

namespace ASC.Core
{
    [Serializable]
    public class TenantVersion
    {
        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }


        public TenantVersion(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var v = obj as TenantVersion;
            return v != null && v.Id == Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
