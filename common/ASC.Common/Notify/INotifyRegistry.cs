using ASC.Notify.Model;
using ASC.Notify.Sinks;

namespace ASC.Notify
{
    public interface INotifyRegistry
    {
        void RegisterSender(string senderName, ISink senderSink);

        void UnregisterSender(string senderName);

        INotifyClient RegisterClient(INotifySource source);
    }
}