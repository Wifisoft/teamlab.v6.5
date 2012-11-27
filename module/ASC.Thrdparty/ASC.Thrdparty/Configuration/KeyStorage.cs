using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace ASC.Thrdparty.Configuration
{
    public static class KeyStorage
    {
        public static string Get(string keyName)
        {
            var section = ConsumerConfigurationSection.GetSection();
            return section!=null ? section.Keys.GetKeyValue(keyName) : string.Empty;
        }

    }
}