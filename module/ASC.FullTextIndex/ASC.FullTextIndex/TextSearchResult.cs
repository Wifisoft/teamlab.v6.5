﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.FullTextIndex
{
    [DataContract]
    public class TextSearchResult
    {
        [DataMember]
        public IDictionary<string, List<string>> results = new Dictionary<string, List<string>>();

        [DataMember]
        public string Module
        {
            get;
            private set;
        }


        public TextSearchResult(string module)
        {
            Module = module;
        }


        public IEnumerable<string> GetIdentifiers()
        {
            return results.Keys.ToList();
        }

        public IEnumerable<string> GetIdentifierDetails(string identifier)
        {
            return results.ContainsKey(identifier) ?
                results[identifier] :
                new List<string>();
        }

        public void AddIdentifier(string identifier)
        {
            var owner = identifier;
            var id = identifier;
            if (identifier.Contains("/"))
            {
                var split = identifier.Split('/');
                id = split[0];
                owner = split[1];
            }
            if (!results.ContainsKey(owner))
            {
                results[owner] = new List<string>();
            }
            results[owner].Add(id);
        }
    }
}
