using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Core.Notify.Senders;

namespace ASC.Notify.Config
{
    static class NotifyServiceCfg
    {
        public static ConnectionStringSettings ConnectionString
        {
            get;
            private set;
        }

        public static IDictionary<string, INotifySender> Senders
        {
            get;
            private set;
        }
        public static int MaxThreads
        {
            get;
            private set;
        }

        public static int BufferSize
        {
            get;
            private set;
        }

        public static int MaxAttempts
        {
            get;
            private set;
        }

        public static TimeSpan AttemptsInterval
        {
            get;
            private set;
        }

        static NotifyServiceCfg()
        {
            var section = ConfigurationManager.GetSection("notify") as NotifyServiceCfgSectionHandler;
            if (section == null)
            {
                throw new ConfigurationErrorsException("Section notify not found.");
            }

            ConnectionString = ConfigurationManager.ConnectionStrings[section.ConnectionStringName];
            Senders = new Dictionary<string, INotifySender>();
            foreach (NotifyServiceCfgSenderElement element in section.Senders)
            {
                var sender = (INotifySender)Activator.CreateInstance(Type.GetType(element.Type, true));
                sender.Init(element.Properties);
                Senders.Add(element.Name, sender);
            }
            MaxThreads = section.Process.MaxThreads;
            BufferSize = section.Process.BufferSize;
            MaxAttempts = section.Process.MaxAttempts;
            AttemptsInterval = section.Process.AttemptsInterval;
        }
    }
}
