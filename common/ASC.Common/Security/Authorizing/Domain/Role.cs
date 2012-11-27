using System;

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public sealed class Role : IRole
    {
        public const string Everyone = "Everyone";
        public const string Visitors = "Visitors";
        public const string Users = "Users";
        public const string Administrators = "Administrators";
        public const string System = "System";


        public Guid ID { get; internal set; }

        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }


        public Role(Guid id, string name, string description)
        {
            if (id == Guid.Empty) throw new ArgumentException("id");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            ID = id;
            Name = name;
            Description = description;
        }


        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var r = obj as Role;
            return r != null && r.ID == ID;
        }

        public override string ToString()
        {
            return string.Format("Role: {0}", Name);
        }
    }
}