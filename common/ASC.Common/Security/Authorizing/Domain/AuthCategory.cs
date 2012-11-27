using System;
using System.Collections.Generic;

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class AuthCategory
    {
        public string Name { get; private set; }

        public List<Action> CategoryActions
        {
            get;
            private set;
        }


        public AuthCategory(string name, IEnumerable<Action> actions)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            Name = name;
            CategoryActions = actions == null ? new List<Action>() : new List<Action>(actions);
        }
    }
}