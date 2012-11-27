using System;
using ASC.Notify.Channels;
using ASC.Notify.Sinks;

namespace ASC.Notify.Model
{
    class NotifyRegistryImpl : INotifyRegistry
    {
        private readonly Context context;


        public NotifyRegistryImpl(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");
            this.context = context;
        }


        public void RegisterSender(string senderName, ISink senderSink)
        {
            context.SenderHolder.RegisterSender(new SenderChannel(context, senderName, null, senderSink));
        }

        public void UnregisterSender(string senderName)
        {
            var senderChannel = context.SenderHolder.GetSender(senderName);
            if (senderChannel != null)
            {
                context.SenderHolder.UngeristerSender(senderChannel);
            }
        }

        public INotifyClient RegisterClient(INotifySource source)
        {
            var client = new NotifyClientImpl(context, source);
            context.Invoke_NotifyClientRegistration(client);
            return client;
        }
    }
}