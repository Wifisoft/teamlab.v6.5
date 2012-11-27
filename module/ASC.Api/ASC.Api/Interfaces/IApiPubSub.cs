using ASC.Api.Publisher;

namespace ASC.Api.Interfaces
{
    public interface IApiPubSub
    {
        void PublishDataForKey(string key, object data);
        void SubscribeForKey(string key, DataAvailibleDelegate dataAvailibleDelegate, object userObject);
        void UnsubscribeForKey(string key, DataAvailibleDelegate dataAvailibleDelegate, object userObject);
    }
}