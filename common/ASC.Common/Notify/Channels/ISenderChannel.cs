using ASC.Notify.Messages;

namespace ASC.Notify.Channels
{
    public interface ISenderChannel
    {
        string SenderName { get; }

        void SendAsync(INoticeMessage message);

        SendResponse DirectSend(INoticeMessage message);
    }
}