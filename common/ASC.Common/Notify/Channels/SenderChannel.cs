using System;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;

namespace ASC.Notify.Channels
{
    public class SenderChannel : ISenderChannel
    {
        private ISink firstSink;
        private ISink senderSink;
        private Context context;


        public string SenderName
        {
            get;
            private set;
        }


        public SenderChannel(Context context, string senderName, ISink decorateSink, ISink senderSink)
        {
            if (senderName == null) throw new ArgumentNullException("senderName");
            if (context == null) throw new ArgumentNullException("context");
            if (senderSink == null) throw new ApplicationException(string.Format("channel with tag {0} not created sender sink", senderName));

            this.context = context;
            this.SenderName = senderName;
            this.firstSink = decorateSink;
            this.senderSink = senderSink;
            
            var dispatcherSink = new DispatchSink(SenderName, this.context.DispatchEngine);
            this.firstSink = AddSink(firstSink, dispatcherSink);
        }

        public void SendAsync(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            firstSink.ProcessMessageAsync(message);
        }

        public SendResponse DirectSend(INoticeMessage message)
        {
            return senderSink.ProcessMessage(message);
        }

        
        private ISink AddSink(ISink firstSink, ISink addedSink)
        {
            if (firstSink == null) return addedSink;
            if (addedSink == null) return firstSink;

            ISink current = firstSink;
            while (current.NextSink != null)
            {
                current = current.NextSink;
            }
            current.NextSink = addedSink;
            return firstSink;
        }
    }
}
