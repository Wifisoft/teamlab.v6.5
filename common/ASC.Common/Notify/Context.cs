using System;
using System.Collections;
using ASC.Notify.Channels;
using ASC.Notify.Engine;
using ASC.Notify.Model;

namespace ASC.Notify
{
    public sealed class Context
    {
        public const string SYS_RECIPIENT_ID = "_#" + _SYS_RECIPIENT_ID + "#_";
        internal const string _SYS_RECIPIENT_ID = "SYS_RECIPIENT_ID";
        internal const string _SYS_RECIPIENT_NAME = "SYS_RECIPIENT_NAME";
        internal const string _SYS_RECIPIENT_ADDRESS = "SYS_RECIPIENT_ADDRESS";


        internal SenderHolder SenderHolder
        {
            get;
            private set;
        }

        public NotifyEngine NotifyEngine
        {
            get;
            private set;
        }

        public INotifyRegistry NotifyService
        {
            get;
            private set;
        }

        public DispatchEngine DispatchEngine
        {
            get;
            private set;
        }


        public event Action<Context, INotifyClient> NotifyClientRegistration;


        public Context()
        {
            SenderHolder = new SenderHolder();
            NotifyService = new NotifyRegistryImpl(this);
            NotifyEngine = new NotifyEngine(this);
            DispatchEngine = new DispatchEngine(this);
        }


        internal void Invoke_NotifyClientRegistration(INotifyClient client)
        {
            if (NotifyClientRegistration != null)
            {
                NotifyClientRegistration(this, client);
            }
        }
    }
}