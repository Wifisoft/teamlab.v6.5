using System.Collections.Generic;
using ASC.Data.Storage.Configuration;

namespace ASC.Data.Storage
{
    public class DataList : Dictionary<string, string>
    {
        public DataList(ModuleConfigurationElement config)
        {
            Add(string.Empty, config.Data);
            foreach (DomainConfigurationElement domain in config.Domains)
            {
                Add(domain.Name, domain.Data);
            }
        }

        public string GetData(string name)
        {
            if (ContainsKey(name))
            {
                return this[name] ?? string.Empty;
            }
            return string.Empty;
        }
    }
}