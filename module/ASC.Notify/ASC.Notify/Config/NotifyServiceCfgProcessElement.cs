using System.Configuration;
using System;

namespace ASC.Notify.Config
{
    class NotifyServiceCfgProcessElement : ConfigurationElement
    {
        [ConfigurationProperty("maxThreads", DefaultValue = 2)]
        public int MaxThreads
        {
            get { return (int)base["maxThreads"]; }
        }

        [ConfigurationProperty("bufferSize", DefaultValue = 10)]
        public int BufferSize
        {
            get { return (int)base["bufferSize"]; }
        }

        [ConfigurationProperty("maxAttempts", DefaultValue = 10)]
        public int MaxAttempts
        {
            get { return (int)base["maxAttempts"]; }
        }

        [ConfigurationProperty("attemptsInterval", DefaultValue = "0:5:0")]
        public TimeSpan AttemptsInterval
        {
            get { return (TimeSpan)base["attemptsInterval"]; }
        }
    }
}
