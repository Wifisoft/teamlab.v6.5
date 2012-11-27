using System;
using System.Collections;
using ASC.Notify.Messages;

namespace ASC.Notify.Sinks
{
    public abstract class Sink : ISink
    {
        public ISink NextSink
        {
            get;
            set;
        }

        public abstract SendResponse ProcessMessage(INoticeMessage message);

        public virtual void ProcessMessageAsync(INoticeMessage message)
        {
            NextSink.ProcessMessageAsync(message);
        }
    }
}
