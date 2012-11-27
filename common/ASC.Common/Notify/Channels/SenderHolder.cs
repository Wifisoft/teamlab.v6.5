using System;
using System.Collections.Generic;

namespace ASC.Notify.Channels
{
    public class SenderHolder
    {
        private readonly Dictionary<string, ISenderChannel> channels = new Dictionary<string, ISenderChannel>(1);


        public void RegisterSender(ISenderChannel sender)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (sender.SenderName == null) throw new ArgumentException("sender.SenderName is empty", "sender");

            lock (channels)
            {
                if (channels.ContainsKey(sender.SenderName))
                {
                    if (channels[sender.SenderName].Equals(sender)) return;
                    channels.Remove(sender.SenderName);
                }
                channels.Add(sender.SenderName, sender);
            }
        }

        public void UngeristerSender(ISenderChannel sender)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (sender.SenderName == null) throw new ArgumentException("sender.SenderName is empty", "sender");

            lock (channels)
            {
                channels.Remove(sender.SenderName);
            }
        }

        public ISenderChannel GetSender(string senderName)
        {
            lock (channels)
            {
                ISenderChannel channel;
                channels.TryGetValue(senderName, out channel);
                return channel;
            }
        }
    }
}