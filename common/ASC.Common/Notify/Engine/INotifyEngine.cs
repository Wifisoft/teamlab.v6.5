using System;

namespace ASC.Notify.Engine
{
    interface INotifyEngine
    {
        bool Scheduling { get; set; }

        void QueueRequest(NotifyRequest request);

        event Action<NotifyEngine, NotifyRequest> AfterTransferRequest;

        event Action<NotifyEngine, NotifyRequest> BeforeTransferRequest;
    }
}