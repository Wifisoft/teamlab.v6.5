using ASC.Notify.Messages;

namespace ASC.Notify.Engine
{
    public interface INotifyDispatcher
    {
        void DispatchNoticeAsync(INoticeMessage message, string senderName);

        SendResponse DispatchNoticeSync(INoticeMessage message, string senderName);
    }
}