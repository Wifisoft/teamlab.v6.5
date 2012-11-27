using System;

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class Action : IAction
    {
        public Guid ID { get; private set; }

        public string Name { get; private set; }

        public bool AdministratorAlwaysAllow { get; private set; }

        public bool Conjunction { get; private set; }


        public Action(Guid id, string name)
            : this(id, name, true, true)
        {
        }

        public Action(Guid id, string name, bool administratorAlwaysAllow, bool conjunction)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("id");

            ID = id;
            Name = name;
            AdministratorAlwaysAllow = administratorAlwaysAllow;
            Conjunction = conjunction;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var a = obj as Action;
            return a != null && a.ID == ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
