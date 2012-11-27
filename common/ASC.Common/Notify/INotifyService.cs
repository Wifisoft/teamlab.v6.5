using System.ServiceModel;
using ASC.Notify.Messages;

namespace ASC.Notify
{
    [ServiceContract]
    public interface INotifyService
    {
        [OperationContract(IsOneWay = true)]
        void SendNotifyMessage(NotifyMessage notifyMessage);
    }
}